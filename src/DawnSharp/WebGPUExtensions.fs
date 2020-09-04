namespace WebGPU

open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open Microsoft.FSharp.NativeInterop

#nowarn "49"
#nowarn "9"

open DawnRaw

[<Struct>]
type SpirVShaderModuleDescriptor =
    {
        Label : string
        Code : byte[]
    }

[<Struct>]
type GLSLShaderModuleDescriptor =
    {
        Label       : string
        EntryPoint  : string
        ShaderStage : ShaderStage
        Defines     : list<string>
        Code        : string
    }


[<AbstractClass; Sealed; Extension>]
type WebGPUExtensions private() =

    static let glslangStage (stage : ShaderStage) =
        match stage with
        | ShaderStage.Vertex -> GLSLang.ShaderStage.Vertex
        | ShaderStage.Fragment -> GLSLang.ShaderStage.Fragment
        | ShaderStage.Compute -> GLSLang.ShaderStage.Compute
        | _ -> failwithf "unknown ShaderStage: %A" stage

    [<Extension>]
    static member CreateSpirVShaderModule(device : Device, descriptor : SpirVShaderModuleDescriptor) =
        if descriptor.Code.Length &&& 3 <> 0 then failwith "Bad SpirV"
        let desc = { ShaderModuleDescriptor.Label = descriptor.Label }
        desc.Pin(fun native ->
            let mutable native = native

            let gc = GCHandle.Alloc(descriptor.Code, GCHandleType.Pinned)
            try
                let data = NativePtr.stackalloc<nativeint> 4
                NativePtr.set data 0 0n
                NativePtr.set data 1 (nativeint (int SType.ShaderModuleSPIRVDescriptor))
                NativePtr.set data 2 (nativeint (descriptor.Code.Length / 4))
                NativePtr.set data 3 (gc.AddrOfPinnedObject())
                native.Next <- NativePtr.toNativeInt data
                use ptr = fixed [| native |]
                let handle = wgpuDeviceCreateShaderModule(device.Handle, ptr)
                new ShaderModule(device, handle)
            finally
                gc.Free()
        )

    [<Extension>]
    static member TryCreateGLSLShaderModule(device : Device, descriptor : GLSLShaderModuleDescriptor) =
        let result, log = 
            GLSLang.GLSLang.tryCompile 
                (glslangStage descriptor.ShaderStage)
                descriptor.EntryPoint
                descriptor.Defines
                descriptor.Code

        match result with
        | Some result ->
            device.CreateSpirVShaderModule { Label = descriptor.Label; Code = result } |> Ok
        | None ->
            Error log
            
    [<Extension>]
    static member CreateGLSLShaderModule(device : Device, descriptor : GLSLShaderModuleDescriptor) =
        match device.TryCreateGLSLShaderModule descriptor with
        | Ok module_ -> module_
        | Error err -> failwithf "ShaderCompiler failed: %A" err



