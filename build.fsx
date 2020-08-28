#r "paket: groupref Build //"
#load ".fake/build.fsx/intellisense.fsx"
#load @"paket-files/build/aardvark-platform/aardvark.fake/DefaultSetup.fsx"

open System
open System.IO
open System.Diagnostics
open Aardvark.Fake
open Fake.Core
open Fake.Tools
open Fake.DotNet

do Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

DefaultSetup.install ["src/DawnSharp.sln"]


let dawnDir = Path.Combine(__SOURCE_DIRECTORY__, "packages", "build", "dawn")

module CreateProcess =
    let vc64 (name : string) (args : list<string>) =
        
        let args =
            [
                "\""
                "\"C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Enterprise\\VC\\Auxiliary\\Build\\vcvars64.bat\""
                "&&"
                String.concat " " (name :: args)
                "\""
            ]
        CreateProcess.fromRawCommandLine "cmd.exe" (String.concat " " ("/C" :: args))

module Proc =
    let exec (env : list<string*string>) (name : string) (args : list<string>) = 
        let res = 
            CreateProcess.fromRawCommand "cmd.exe" ("/C" :: name :: args)
            |> CreateProcess.redirectOutput
            |> CreateProcess.withOutputEvents System.Console.WriteLine System.Console.WriteLine
            |> CreateProcess.withWorkingDirectory dawnDir
            |> fun c -> env |> List.fold (fun c (k,v) -> CreateProcess.setEnvironmentVariable k v c) c
            //|> CreateProcess.ensureExitCode
            |> Proc.run

        if res.ExitCode <> 0 then failwithf "%s failed with code %d" name res.ExitCode




Target.create "DawnWindows" (fun _ ->
 
    let defPath = Path.Combine(dawnDir, @"abi.def")
    let dllOut = Path.Combine(__SOURCE_DIRECTORY__, "lib", "Native", "DawnSharp", "windows", "AMD64", "dawn.dll")
   
    let writeDefFile (defFile : string) =
        let excluded = Set.ofList [ "sprintf"; "snprintf"; "sscanf"; "fprintf" ]

        let exportLibs =
            [
                Path.Combine(dawnDir, "cmake", "src", "dawn", "Release", "dawncpp.lib") 
                Path.Combine(dawnDir, "cmake", "src", "dawn", "Release", "dawn_proc.lib") 
                Path.Combine(dawnDir, "cmake", "src", "utils", "Release", "dawn_utils.lib") 
                Path.Combine(dawnDir, "cmake", "src", "dawn_native", "Release", "dawn_native.lib") 
                Path.Combine(dawnDir, "cmake", "src", "dawn_platform", "Release", "dawn_platform.lib") 
                Path.Combine(dawnDir, "cmake", "src", "dawn_wire", "Release", "dawn_wire.lib") 
                Path.Combine(dawnDir, "cmake", "src", "common", "Release", "dawn_common.lib") 
                Path.Combine(dawnDir, "cmake", "src", "dawn", "Release", "dawncpp_headers.lib") 
                Path.Combine(dawnDir, "cmake", "src", "dawn", "Release", "dawn_headers.lib") 
            ]

        let dumpbin (file : string) =
            let result = 
                CreateProcess.vc64 "dumpbin" ["/linkermember:1"; sprintf "\"%s\"" file]
                |> CreateProcess.redirectOutput
                |> Proc.run

            if result.ExitCode = 0 then
                result.Result.Output
            else
                ""

        let functions = System.Collections.Generic.List<string>()
        for l in exportLibs do
            let bin = dumpbin l

            let lines = bin.Split([| "\r\n"; "\n" |], StringSplitOptions.RemoveEmptyEntries)
            let mutable fin = false
            let mutable startPoint = false
            use e = (lines :> seq<_>).GetEnumerator()

            while not fin && e.MoveNext() do
                let l = e.Current.Trim()

                if startPoint then
                    if l = "Summary" then
                        fin <- true
                    else
                        let arr = l.Split(' ')
                        if arr.Length > 0 then
                            let name = arr.[arr.Length - 1]
                            if not (Set.contains name excluded) then
                                functions.Add name
                else
                    if l.Contains "public symbols" then
                        startPoint <- true


        let content = System.Text.StringBuilder()
        content.AppendLine("EXPORTS") |> ignore

        for f in functions do
            content.Append "    " |> ignore
            content.AppendLine f |> ignore

        File.WriteAllText(defFile, content.ToString())     

    // clean checkout dawn
    if Directory.Exists dawnDir then
        let dawnThirdParty = Path.Combine(dawnDir, "third_party")
        if Directory.Exists dawnThirdParty then Directory.Delete(dawnThirdParty, true)

        Git.CommandHelper.directRunGitCommandAndFail dawnDir "reset --hard"
        Git.CommandHelper.directRunGitCommandAndFail dawnDir "clean -xdf"
        Git.CommandHelper.directRunGitCommandAndFail dawnDir "pull"

    else
        Git.Repository.clone "." "https://dawn.googlesource.com/dawn" dawnDir

    // fix1: python2
    do
        let file = Path.Combine(dawnDir, "generator", "CMakeLists.txt")
        File.WriteAllText(
            file,
            File.ReadAllText(file)
                .Replace("PYTHON_EXECUTABLE", "Python2_EXECUTABLE")
                .Replace("find_package(PythonInterp REQUIRED)", "find_package(Python2 REQUIRED)")
        )

    // fix2: SHADERC_ENABLE_SHARED_CRT bug
    do
        let file = Path.Combine(dawnDir, "third_party", "CMakeLists.txt")
        File.WriteAllText(
            file,
            File.ReadAllText(file)
                .Replace("option(SHADERC_ENABLE_SHARED_CRT \"Use the shared CRT instead of the static CRT\" ON CACHE BOOL \"\" FORCE)", "option(SHADERC_ENABLE_SHARED_CRT \"Use the shared CRT instead of the static CRT\" ON)")
        )

    // copy gclient config
    File.Copy(Path.Combine(dawnDir, "scripts", "standalone.gclient"), Path.Combine(dawnDir, ".gclient"), true)

    // gclient sync
    let gclient =
        Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
            "depot_tools",
            "gclient.bat"
        )
    Proc.exec ["DEPOT_TOOLS_WIN_TOOLCHAIN", "0"] gclient ["sync"]

    // cmake . -B cmake
    Proc.exec [] "cmake" ["."; "-B"; "cmake"]

    // build
    "dawn/cmake/ALL_BUILD.vcxproj" |> MSBuild.build (fun (defaults:MSBuildParams) ->
        { defaults with
            Verbosity = Some(Quiet)
            Targets = ["Build"]
            Properties =
                [
                    "Configuration", "Release"
                ]
        }
    )

    // write def
    writeDefFile defPath

    // create output dir (if not existing)
    do
        let dir = Path.GetDirectoryName dllOut
        if not (Directory.Exists dir) then Directory.CreateDirectory dir |> ignore

    // linker
    let linkResult = 

        let lib (str : string) =
            if File.Exists str then sprintf "\"%s\"" str
            else failwith "file not found: %s" str

        let linkerArgs =
            [
                "/ERRORREPORT:QUEUE"
                sprintf "/OUT:\"%s\"" dllOut
                "/INCREMENTAL:NO"
                "/NOLOGO"

                Path.Combine(dawnDir, "cmake", "src", "dawn", "Release", "dawncpp.lib") |> lib
                Path.Combine(dawnDir, "cmake", "src", "dawn", "Release", "dawn_proc.lib") |> lib
                Path.Combine(dawnDir, "cmake", "src", "utils", "Release", "dawn_utils.lib") |> lib
                Path.Combine(dawnDir, "cmake", "src", "dawn_native", "Release", "dawn_native.lib") |> lib
                Path.Combine(dawnDir, "cmake", "src", "dawn_platform", "Release", "dawn_platform.lib") |> lib
                Path.Combine(dawnDir, "cmake", "third_party", "shaderc", "libshaderc_spvc", "Release", "shaderc_spvc.lib") |> lib
                Path.Combine(dawnDir, "cmake", "third_party", "shaderc", "third_party", "spirv-cross", "Release", "spirv-cross-hlsl.lib") |> lib
                Path.Combine(dawnDir, "cmake", "third_party", "shaderc", "third_party", "spirv-cross", "Release", "spirv-cross-msl.lib") |> lib
                Path.Combine(dawnDir, "cmake", "third_party", "shaderc", "third_party", "spirv-cross", "Release", "spirv-cross-glsl.lib") |> lib
                Path.Combine(dawnDir, "cmake", "third_party", "shaderc", "third_party", "spirv-cross", "Release", "spirv-cross-reflect.lib") |> lib
                Path.Combine(dawnDir, "cmake", "third_party", "shaderc", "third_party", "spirv-cross", "Release", "spirv-cross-core.lib") |> lib
                Path.Combine(dawnDir, "cmake", "src", "dawn_wire", "Release", "dawn_wire.lib") |> lib
                Path.Combine(dawnDir, "cmake", "src", "common", "Release", "dawn_common.lib") |> lib
                Path.Combine(dawnDir, "cmake", "src", "dawn", "Release", "dawncpp_headers.lib") |> lib
                Path.Combine(dawnDir, "cmake", "src", "dawn", "Release", "dawn_headers.lib") |> lib
                Path.Combine(dawnDir, "cmake", "third_party", "shaderc", "libshaderc", "Release", "shaderc.lib") |> lib
                Path.Combine(dawnDir, "cmake", "third_party", "shaderc", "libshaderc_util", "Release", "shaderc_util.lib") |> lib
                Path.Combine(dawnDir, "cmake", "third_party", "glslang", "glslang", "Release", "glslang.lib") |> lib
                Path.Combine(dawnDir, "cmake", "third_party", "glslang", "hlsl", "Release", "HLSL.lib") |> lib
                Path.Combine(dawnDir, "cmake", "third_party", "glslang", "SPIRV", "Release", "SPIRV.lib") |> lib
                Path.Combine(dawnDir, "cmake", "third_party", "SPIRV-Tools", "source", "opt", "Release", "SPIRV-Tools-opt.lib") |> lib
                Path.Combine(dawnDir, "cmake", "third_party", "SPIRV-Tools", "source", "Release", "SPIRV-Tools.lib") |> lib
                Path.Combine(dawnDir, "cmake", "third_party", "glslang", "glslang", "Release", "MachineIndependent.lib") |> lib
                Path.Combine(dawnDir, "cmake", "third_party", "glslang", "glslang", "OSDependent", "Windows", "Release", "OSDependent.lib") |> lib
                Path.Combine(dawnDir, "cmake", "third_party", "glslang", "OGLCompilersDLL", "Release", "OGLCompiler.lib") |> lib
                Path.Combine(dawnDir, "cmake", "third_party", "glslang", "glslang", "Release", "GenericCodeGen.lib") |> lib
                Path.Combine(dawnDir, "cmake", "third_party", "glfw", "src", "Release", "glfw3.lib") |> lib

                "user32.lib"
                "dxguid.lib"
                "kernel32.lib"
                "gdi32.lib"
                "winspool.lib"
                "shell32.lib"
                "ole32.lib"
                "oleaut32.lib"
                "uuid.lib"
                "comdlg32.lib"
                "advapi32.lib"
                "kernel32.lib"
                "user32.lib"
                "gdi32.lib"
                "winspool.lib"
                "comdlg32.lib"
                "advapi32.lib"
                "shell32.lib"
                "ole32.lib"
                "oleaut32.lib"
                "uuid.lib"
                "odbc32.lib"
                "odbccp32.lib"

                sprintf "/DEF:\"%s\"" defPath
                "/MANIFEST"
                "/MANIFESTUAC:NO"
                "/manifest:embed"
                "/DEBUG"
                sprintf "/PDB:\"%sb\"" (Path.ChangeExtension(dllOut, ".pdb"))
                "/SUBSYSTEM:WINDOWS"
                "/OPT:NOREF"
                "/OPT:ICF"
                "/LTCG"
                "/TLBID:1"
                "/DYNAMICBASE"
                "/NXCOMPAT"
                sprintf "/IMPLIB:\"%s\"" (Path.ChangeExtension(dllOut, ".lib"))
                "/MACHINE:X64"
                "/DLL"
                Path.Combine(__SOURCE_DIRECTORY__, "lib", "pch.obj")
                Path.Combine(__SOURCE_DIRECTORY__, "lib", "dllmain.obj")

            ]

        let args =
            [
                "\""
                "\"C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Enterprise\\VC\\Auxiliary\\Build\\vcvars64.bat\""
                "&&"
                String.concat " " ("link.exe" :: linkerArgs)

                "\""
            ]
        CreateProcess.fromRawCommandLine "cmd.exe" (String.concat " " ("/C" :: args))
        |> CreateProcess.redirectOutput
        |> CreateProcess.withOutputEvents System.Console.WriteLine System.Console.WriteLine
        |> CreateProcess.withWorkingDirectory dawnDir
        |> Proc.run

    if linkResult.ExitCode <> 0 then failwithf "linker failed"

)



entry()
