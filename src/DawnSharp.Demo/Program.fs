// Learn more about F# at http://fsharp.org

open Aardvark.Base
open System
open WebGPU
open Microsoft.FSharp.NativeInterop
open System.Runtime.InteropServices

#nowarn "9"

type AdapterHandle = struct val mutable public Handle : nativeint end
type BackendBindingHandle = struct val mutable public Handle : nativeint end

type DeviceType =
    | Discrete = 0
    | Integrated = 1
    | CPU = 2
    | Unknown = 3


type AdapterInfo =
    {
        PipelineStatisticsQuery : bool
        ShaderFloat16 : bool
        TextureCompressionBC : bool
        TimestampQuery : bool
        BackendType : BackendType
        DeviceType : DeviceType
        DeviceId : uint32
        VendorId : uint32
        Vendor : option<string>
        Name : string
    }

type ToggleInfo =
    {
        Name : string
        Description : string
        Url : string
    }

module DawnRaw =
    open System.Runtime.InteropServices

    
    type DawnBackendType =
        | D3D12 = 0
        | Metal = 1
        | Null = 2
        | OpenGL = 3
        | Vulkan = 4


    type DawnAdapterInfo =
        struct
            val mutable public PipelineStatisticsQuery : int
            val mutable public ShaderFloat16 : int
            val mutable public TextureCompressionBC : int
            val mutable public TimestampQuery : int
            val mutable public BackendType : DawnBackendType
            val mutable public DeviceType : DeviceType
            val mutable public DeviceId : uint32
            val mutable public VendorId : uint32
            val mutable public Name : nativeint
        end

    type DawnToggleInfo =
        struct
            val mutable public Name : nativeint
            val mutable public Description : nativeint
            val mutable public Url : nativeint
        end
        
    module AdapterInfo =
        open System.Runtime.InteropServices

        let ofDawnAdapterInfo(info : DawnAdapterInfo) =

            let typ =   
                match info.BackendType with
                | DawnBackendType.D3D12 -> BackendType.D3D12
                | DawnBackendType.Metal -> BackendType.Metal
                | DawnBackendType.Null -> BackendType.Null
                | DawnBackendType.OpenGL -> BackendType.OpenGL
                | DawnBackendType.Vulkan -> BackendType.Vulkan
                | _ -> BackendType.Null

            let vendorName =
                PCI.tryGetVendorName info.VendorId

            let name = Marshal.PtrToStringAnsi info.Name
            
            let name =
                match vendorName with
                | Some vendor ->
                    let vendor = vendor.Trim()
                    let replacements =
                        [
                            yield vendor
                            if vendor.ToLower().EndsWith "corporation" then yield vendor.Substring(0, vendor.Length - 11).Trim()
                            if vendor.ToLower().EndsWith "corp" then yield vendor.Substring(0, vendor.Length - 4).Trim()
                            if vendor.ToLower().EndsWith "corp." then yield vendor.Substring(0, vendor.Length - 5).Trim()
                            if vendor.ToLower().EndsWith "inc" then yield vendor.Substring(0, vendor.Length - 3).Trim()
                            if vendor.ToLower().EndsWith "inc." then yield vendor.Substring(0, vendor.Length - 4).Trim()
                        ]

                    let mutable name = name.Trim()
                    for repl in replacements do
                        let idx = name.ToLower().IndexOf(repl.ToLower())
                        if idx >= 0 then
                            let str = name.Substring(0, idx) + " " + name.Substring(idx + repl.Length)
                            name <- str.Trim()

                    if name.ToLower().StartsWith "(r)" || name.ToLower().StartsWith "(c)" then name <- name.Substring(3).Trim()

                    name
                | None ->
                    name.Trim()
            
            let vendorName =
                vendorName |> Option.map (fun vendor ->
                    let mutable vendor = vendor
                    if vendor.ToLower().EndsWith "corporation" then vendor <- vendor.Substring(0, vendor.Length - 11).Trim()
                    if vendor.ToLower().EndsWith "corp" then vendor <- vendor.Substring(0, vendor.Length - 4).Trim()
                    if vendor.ToLower().EndsWith "corp." then vendor <- vendor.Substring(0, vendor.Length - 5).Trim()
                    if vendor.ToLower().EndsWith "inc" then vendor <- vendor.Substring(0, vendor.Length - 3).Trim()
                    if vendor.ToLower().EndsWith "inc." then vendor <- vendor.Substring(0, vendor.Length - 4).Trim()
                    vendor
                )
            
            {
                PipelineStatisticsQuery = info.PipelineStatisticsQuery <> 0
                ShaderFloat16 = info.ShaderFloat16 <> 0
                TextureCompressionBC = info.TextureCompressionBC <> 0
                TimestampQuery = info.TimestampQuery <> 0
                BackendType = typ
                DeviceType = info.DeviceType
                DeviceId = info.DeviceId
                VendorId = info.VendorId
                Vendor = vendorName
                Name = name
            }
        
    [<DllImport("dawn")>]
    extern InstanceHandle dawnNewInstance()
    
    [<DllImport("dawn")>]
    extern void dawnDestroyInstance(InstanceHandle instance)
    
    [<DllImport("dawn")>]
    extern BackendBindingHandle dawnCreateBackendBinding(BackendType backendType, nativeint glfwWindow, DeviceHandle device)

    [<DllImport("dawn")>]
    extern TextureFormat dawnGetPreferredSwapChainTextureFormat(BackendBindingHandle handle)
    
    [<DllImport("dawn")>]
    extern uint64 dawnGetSwapChainImplementation(BackendBindingHandle handle)
    
    [<DllImport("dawn")>]
    extern void dawnDestroyBackendBinding(BackendBindingHandle handle)


    [<DllImport("dawn")>]
    extern void dawnEnableBackendValidation(InstanceHandle instance, int validate)
    
    [<DllImport("dawn")>]
    extern void dawnEnableBeginCaptureOnStartup(InstanceHandle instance, int beginCaptureOnStartup)
    
    [<DllImport("dawn")>]
    extern void dawnEnableGPUBasedBackendValidation(InstanceHandle instance, int enableGPUBasedBackendValidation)
    
    [<DllImport("dawn")>]
    extern DawnToggleInfo* dawnGetToggleInfo(InstanceHandle instance, nativeint name)
    
    [<DllImport("dawn")>]
    extern int dawnGetSupportedExtensions(AdapterHandle instance, int bufSize, nativeint* extNames)

    [<DllImport("dawn")>]
    extern int dawnDiscoverDefaultAdapters(InstanceHandle instance, int bufSize, AdapterHandle* adapters)
    
    [<DllImport("dawn")>]
    extern DawnAdapterInfo dawnGetAdapterInfo(AdapterHandle handle)
    
    [<DllImport("dawn")>]
    extern void dawnFreeAdapterInfo(DawnAdapterInfo handle)
    
    [<DllImport("dawn")>]
    extern DeviceHandle dawnCreateDevice(AdapterHandle handle, int extCount, nativeint* exts, int enabledCount, nativeint* enabled, int disabledCount, nativeint* disabled)
    
    [<DllImport("dawn")>]
    extern void dawnFreeAdapter(AdapterHandle handle)

type Adapter(handle : AdapterHandle) =
    let info =
        lazy (
            let info = DawnRaw.dawnGetAdapterInfo handle
            let res = DawnRaw.AdapterInfo.ofDawnAdapterInfo info
            DawnRaw.dawnFreeAdapterInfo info
            res
        )

    let extensions =
        lazy (
            let cnt = DawnRaw.dawnGetSupportedExtensions(handle, 0, NativePtr.ofNativeInt 0n)
            let arr = Array.zeroCreate cnt
            use ptr = fixed arr
            let cnt = DawnRaw.dawnGetSupportedExtensions(handle, cnt, ptr)

            arr |> Array.map (fun ptr ->
                Marshal.PtrToStringAnsi ptr
            )

        )
    
    member x.Handle = handle
    member x.Info = info.Value
    
    member x.VendorId = x.Info.VendorId
    member x.DeviceId = x.Info.DeviceId
    member x.Vendor = 
        match x.Info.Vendor with
        | Some v -> v
        | None -> 
            match x.Info.VendorId with
            | 0u -> "NULL"
            | _ -> sprintf "PCI%08X" x.Info.VendorId

    member x.Name = x.Info.Name
    member x.DeviceType = x.Info.DeviceType
    member x.BackendType = x.Info.BackendType
    member x.Extensions = extensions.Value

    member x.CreateDevice(?extensions : string[], ?enabledToggles : string[], ?disabledToggles : string[]) =
        let extensions = defaultArg extensions [||] |> Array.map Marshal.StringToHGlobalAnsi
        let enabledToggles = defaultArg enabledToggles [||] |> Array.map Marshal.StringToHGlobalAnsi
        let disabledToggles = defaultArg disabledToggles [||] |> Array.map Marshal.StringToHGlobalAnsi
        try
            use pExt = fixed extensions
            use pEn = fixed enabledToggles
            use pDis = fixed disabledToggles
            let dh = DawnRaw.dawnCreateDevice(handle, extensions.Length, pExt, enabledToggles.Length, pEn, disabledToggles.Length, pDis)
            
            new Device(dh)
        finally
            extensions |> Array.iter Marshal.FreeHGlobal
            enabledToggles |> Array.iter Marshal.FreeHGlobal
            disabledToggles |> Array.iter Marshal.FreeHGlobal

    member private x.Dispose(disposing : bool) =
        if disposing then GC.SuppressFinalize x
        DawnRaw.dawnFreeAdapter handle

    member x.Dispose() = x.Dispose true
    override x.Finalize() = x.Dispose false
    interface IDisposable with
        member x.Dispose() = x.Dispose()

type Instance() =
    let handle = DawnRaw.dawnNewInstance()

    member x.Handle = handle

    member x.EnableBackendValidation(validation : bool) =
        DawnRaw.dawnEnableBackendValidation(handle, if validation then 1 else 0)
        
    member x.EnableBeginCaptureOnStartup(captureOnStartup : bool) =
        DawnRaw.dawnEnableBeginCaptureOnStartup(handle, if captureOnStartup then 1 else 0)

    member x.EnableGPUBasedBackendValidation(validation : bool) =
        DawnRaw.dawnEnableGPUBasedBackendValidation(handle, if validation then 1 else 0)

    member x.GetDefaultAdapters() =
        let arr = Array.zeroCreate 128
        use ptr = fixed arr
        let cnt = DawnRaw.dawnDiscoverDefaultAdapters(handle, 128, ptr)

        Array.init cnt (fun i ->
            let h = arr.[i]
            new Adapter(h)
        )

type BackendBinding(device : Device, handle : BackendBindingHandle) =
    member x.Device = device
    member x.Handle = handle

    member x.GetSwapChainImplementation() =
        DawnRaw.dawnGetSwapChainImplementation(handle)
        
    member x.GetPreferredSwapChainTextureFormat() =
        DawnRaw.dawnGetPreferredSwapChainTextureFormat(handle)

    member x.Destroy() =
        DawnRaw.dawnDestroyBackendBinding(handle)

type Device with
    member x.CreateBackendBinding(backendType : BackendType, glfwWindow : nativeint) =
        BackendBinding(x, DawnRaw.dawnCreateBackendBinding(backendType, glfwWindow, x.Handle))

open System.Threading

exception QueueWaitFailed of status : FenceCompletionStatus
exception BufferMapFailed of status : BufferMapAsyncStatus

type Buffer with
    member x.Map(mode, offset, size) =
        let mutable res = Int32.MaxValue
        let mutable mapPointer = 0n
        x.MapAsync(mode, offset, size, BufferMapCallback(fun status _ ->
            match status with
            | BufferMapAsyncStatus.Success ->   
                let ptr = x.GetConstMappedRange(offset, size)
                mapPointer <- ptr
                res <- int BufferMapAsyncStatus.Success
            | _ ->
                res <- int status
        ), 0n)

        while Volatile.Read(&res) = Int32.MaxValue do
            x.Device.Tick()
        match Volatile.Read(&res) |> unbox<BufferMapAsyncStatus> with
        | BufferMapAsyncStatus.Success ->
            mapPointer
        | err ->
            raise <| BufferMapFailed err

type Queue with
    member x.Wait() =
        let mutable res = Int32.MaxValue
        use f = x.CreateFence { Label = null; InitialValue = 0UL }
        x.Signal(f, 1UL)
        f.OnCompletion(1UL, FenceOnCompletionCallback(fun status _ ->
            Volatile.Write(&res, int status)
        ), 0n)
        let mutable iter = 0
        while Volatile.Read(&res) = Int32.MaxValue do    
            x.Device.Tick()
            iter <- iter + 1
        let status = Volatile.Read(&res) |> unbox<FenceCompletionStatus>


        if status <> FenceCompletionStatus.Success then raise <| QueueWaitFailed status


let run(device : Device) =
    let size = 32 <<< 20



    let src = 
        device.CreateBuffer {
            Label = "a"
            Size = uint64 size    
            Usage = BufferUsage.MapWrite ||| BufferUsage.CopySrc
            MappedAtCreation = true
        }
        
    let real = 
        device.CreateBuffer {
            Label = "r"
            Size = uint64 size        
            Usage = BufferUsage.Vertex ||| BufferUsage.CopyDst ||| BufferUsage.CopySrc
            MappedAtCreation = false
        }

    let dst =
        device.CreateBuffer {
            Label = "b"
            Size = uint64 size        
            Usage = BufferUsage.MapRead ||| BufferUsage.CopyDst
            MappedAtCreation = false
        }
            
    let input = Array.init size byte
    let ptr = src.GetMappedRange(0un, unativeint size) 
    Marshal.Copy(input, 0, ptr, size)
    src.Unmap()

    let shaderCode =
        String.concat "\r\n" [
            "#version 450"
            "#ifdef Vertex"
            "layout(location = 0) in vec4 pos;"
            ""
            "layout(set = 0, binding = 0)"
            "uniform MyBuffer {"
            "    vec4 MyUniform;"
            "};"
            ""
            "void main()"
            "{"
            "    gl_Position = MyUniform + pos;"
            "}"
            "#endif"
            ""
            "#ifdef Fragment"
            "layout(location = 0) out vec4 color;"
            "void main()"
            "{"
            "    color = vec4(1,1,1,1);"
            "}"
            "#endif"
        ]

    let vertexModule =
        device.CreateGLSLShaderModule {
            ShaderStage = ShaderStage.Vertex
            Label       = "VS"
            EntryPoint  = "main"
            Defines     = ["Vertex"]
            Code        = shaderCode
        }

    let fragmentModule =
        device.CreateGLSLShaderModule {
            ShaderStage = ShaderStage.Fragment
            Label       = "FS"
            EntryPoint  = "main"
            Defines     = ["Fragment"]
            Code        = shaderCode
        }

    let pipelineLayout = 
        device.CreatePipelineLayout {
            Label = null
            BindGroupLayouts = [||]
        }

    let pipeline = 
        device.CreateRenderPipeline {
            Label  = "pipy"
            AlphaToCoverageEnabled = false
            Layout  = pipelineLayout
            VertexStage = { Module = vertexModule; EntryPoint = "main" }
            FragmentStage = Some { Module = fragmentModule; EntryPoint = "main" } 
            PrimitiveTopology = PrimitiveTopology.TriangleList
            SampleCount = 1
            SampleMask = 1
            RasterizationState =
                Some {
                    FrontFace = FrontFace.CCW
                    CullMode = CullMode.Back
                    DepthBias = 0
                    DepthBiasClamp = 0.0f
                    DepthBiasSlopeScale = 0.0f
                }

            VertexState = 
                Some { 
                    IndexFormat = IndexFormat.Undefined
                    VertexBuffers = 
                    [|
                        { 
                            StepMode = InputStepMode.Vertex
                            ArrayStride = 12UL
                            Attributes =
                                [|
                                    { Format = VertexFormat.Float3; Offset = 0UL; ShaderLocation = 0 }
                                |]
                        }
                    |]
                }
            DepthStencilState =     
                Some { 
                    Format = TextureFormat.Depth24PlusStencil8
                    DepthWriteEnabled = true
                    DepthCompare = CompareFunction.LessEqual
                    StencilFront = { Compare = CompareFunction.Always; FailOp = StencilOperation.Keep; PassOp = StencilOperation.Keep; DepthFailOp = StencilOperation.Keep }
                    StencilBack = { Compare = CompareFunction.Always; FailOp = StencilOperation.Keep; PassOp = StencilOperation.Keep; DepthFailOp = StencilOperation.Keep }
                    StencilReadMask = 1
                    StencilWriteMask = 1
                }
            ColorStates = 
                [|
                    { 
                        Format = TextureFormat.RGBA8Unorm 
                        AlphaBlend = { Operation = BlendOperation.Add; SrcFactor = BlendFactor.One; DstFactor = BlendFactor.Zero }
                        ColorBlend = { Operation = BlendOperation.Add; SrcFactor = BlendFactor.One; DstFactor = BlendFactor.Zero }
                        WriteMask = ColorWriteMask.All
                    }
                |]
        }

    

    let buf = 
        let cmd = device.CreateCommandEncoder()
        cmd.CopyBufferToBuffer(src, 0UL, real, 0UL, uint64 size)
        cmd.CopyBufferToBuffer(real, 0UL, dst, 0UL, uint64 size)

        //cmd.CopyBufferToTexture(
        //    { 
        //        Buffer = src
        //        Layout = { Offset = 0UL; BytesPerRow = 4096u; RowsPerImage = 1024u } 
        //    },
        //    { 
        //        Texture = failwith ""
        //        MipLevel = 0u
        //        Origin = { X = 0u; Y = 0u; Z = 0u }
        //        Aspect = TextureAspect.All 
        //    },
        //    { Width = 1024u; Height = 1024u; Depth = 1u }
        //)

        cmd.Finish()
        
    let q = device.GetDefaultQueue()
    q.Submit [| buf |]
    buf.Dispose()
    q.Wait()


    let ptr = dst.Map(MapMode.Read, 0un, unativeint size)
    let arr : byte[] = Array.zeroCreate size
    Marshal.Copy(ptr, arr, 0, arr.Length)
    dst.Unmap()

    if arr = input then printfn "success"
    else printfn "error"


    dst.Destroy()
    src.Destroy()
    real.Destroy()
    src.Dispose()
    dst.Dispose()
    real.Dispose()


module PCITable = 
    open System.IO
    open System.Text.RegularExpressions


    let generate() =
        use s = File.OpenRead @"C:\Users\Schorsch\Downloads\pci.ids"
        use r = new StreamReader(s)

        let lineRx = Regex @"^([\t]*)([0-9a-fA-F]{4})[ \t]*(.*)$"


        let mutable vendors = Map.empty
        let mutable devices = Map.empty

        let mutable currentVendor = None

        while not r.EndOfStream do
            let line = r.ReadLine()
            if not (line.StartsWith "#") then
                let m = lineRx.Match line
                if m.Success then
                    let kind = m.Groups.[1].Length
                    let id = System.UInt32.Parse(m.Groups.[2].Value, Globalization.NumberStyles.HexNumber)
                    let name = m.Groups.[3].Value.Trim()

                    match kind with
                    | 0 ->
                        // vendor
                        vendors <- Map.add id name vendors
                        currentVendor <- Some id

                    | 1 -> 
                        // device
                        match currentVendor with
                        | Some vid ->
                            devices <- Map.add (vid, id) name devices
                        | None ->
                            printfn "device with no vendor"
                    | _ ->
                        ()
                        
        printfn "%d vendors" (Map.count vendors)
        printfn "%d devices" (Map.count devices)


        let bb = System.Text.StringBuilder()
        let printfn fmt = fmt |> Printf.kprintf(fun str -> bb.AppendLine str |> ignore)

        printfn "namespace WebGPU"

        printfn "module PCI ="

        printfn "    let vendors = "
        printfn "        Map.ofArray [|"
        for (id, name) in Map.toSeq vendors do
            let name = name.Replace("\"", "\\\"")
            printfn "            0x%04Xu, \"%s\"" id name
        printfn "        |]"
        

        let devs = 
            devices 
            |> Map.toSeq 
            |> Seq.map (fun ((vid, did), name) -> vid, (did, name))
            |> Seq.groupBy fst
            |> Seq.map (fun (g,vs) -> g, vs |> Seq.map snd |> Map.ofSeq)
            |> Map.ofSeq


        printfn "    let devices = "
        printfn "        Map.ofArray [|"
        for (vid, devs) in Map.toSeq devs do
            printfn "            0x%04Xu, Map.ofArray [|" vid
            for (did, name) in Map.toSeq devs do
                let name = name.Replace("\"", "\\\"")
                printfn "                0x%04Xu, \"%s\"" did name
            printfn "            |]"
        printfn "        |]"

        printfn "    let tryGetVendorName (id : uint32) = Map.tryFind id vendors"
        printfn "    let tryGetDeviceName (vid : uint32) (did : uint32) = Map.tryFind vid devices |> Option.bind (Map.tryFind did)"


        let t = bb.ToString()
        File.WriteAllText(@"C:\Users\Schorsch\Development\DawnSharp\src\DawnSharp\PCI.fs", t)

open Silk.NET.GLFW

module NativePtr = 
    let inline pinString (str : string) (cont : nativeint -> 'a) =
        let length = 1 + System.Text.Encoding.UTF8.GetByteCount str
        let ptr = NativePtr.stackalloc<byte> length
        let l = System.Text.Encoding.UTF8.GetBytes(str.AsSpan(), Span<byte>(NativePtr.toVoidPtr ptr, length))
        NativePtr.set ptr l 0uy
        cont (NativePtr.toNativeInt ptr)

let test () =
    let ptrs = System.Collections.Generic.List<nativeptr<int>>()
    for i in 1 .. 10 do
        let a = NativePtr.stackalloc 1
        NativePtr.write a i
        ptrs.Add a

    for a in ptrs do
        printfn "%A" (NativePtr.read a)

[<EntryPoint; STAThread>]
let main argv = 
    Aardvark.Init()
 
    let glfw = Glfw.GetApi()
    glfw.Init() |> ignore
    glfw.WindowHint(WindowHintClientApi.ClientApi, ClientApi.NoApi)
    //glfw.WindowHint(WindowHintBool.Visible, false)
    let win = glfw.CreateWindow(640, 480, "Yeah", NativePtr.ofNativeInt 0n, NativePtr.ofNativeInt 0n)

    let instance = Instance()
    instance.EnableBackendValidation true
    instance.EnableGPUBasedBackendValidation true
    instance.EnableBeginCaptureOnStartup true

    let adapters = instance.GetDefaultAdapters()

    let idx = 0

    //for (idx, a) in Array.indexed adapters do
    //    printfn "%d: %s %s (%A)" idx a.Vendor a.Name a.BackendType
    //printf "select device: "
    //let idx = System.Console.ReadLine() |> int

    let a = adapters.[idx]
    printfn "using %s %s (%A)" a.Vendor a.Name a.BackendType
    match a.BackendType with
    | BackendType.Null -> 
        printfn "%s" a.Name
    | _ -> 
        let dev = a.CreateDevice()

        //let sw = System.Diagnostics.Stopwatch.StartNew()
        //for i in 1 .. 100 do
        //    let a = dev.CreateBuffer { Label = "asdsad"; Usage = BufferUsage.Vertex ||| BufferUsage.CopyDst; Size = 1024UL; MappedAtCreation = false }
        //    a.Dispose()
        //    //Console.ReadLine()
        //sw.Stop()
        //printfn "%A" sw.Elapsed.TotalMilliseconds
        //exit 0
        dev.SetUncapturedErrorCallback(ErrorCallback(fun typ msg _ ->
            printfn "%A: %s" typ msg
        ), 0n) |> ignore

        let binding = 
            dev.CreateBackendBinding(a.BackendType, NativePtr.toNativeInt win)

        let queue = dev.GetDefaultQueue()
        
        
        //let pDesc = Marshal.AllocHGlobal(sizeof<DawnRaw.WGPUSwapChainDescriptor>) |> NativePtr.ofNativeInt<DawnRaw.WGPUSwapChainDescriptor>
        
        let swapChainFormat = binding.GetPreferredSwapChainTextureFormat()
        let swapChainImpl = binding.GetSwapChainImplementation()

        let createSwapChain(size : V2i) =
            let chain = 
                dev.CreateSwapChain(
                    null,
                    {
                        Label = null
                        Format = swapChainFormat
                        Implementation = swapChainImpl
                        Usage = TextureUsage.OutputAttachment
                        Width = size.X
                        Height = size.Y
                        PresentMode = PresentMode.Immediate
                    }
                )

            chain.Configure(swapChainFormat, TextureUsage.OutputAttachment, size.X, size.Y)
            chain
            

        dev.Tick()
        let mutable size = V2i.II
        glfw.GetFramebufferSize(win, &size.X, &size.Y)
        let mutable chain = createSwapChain size

        
        let depth =
            dev.CreateTexture {
                Label = null
                Usage = TextureUsage.OutputAttachment
                Dimension = TextureDimension.D2D
                Size = { Width = size.X; Height = size.Y; Depth = 1 }
                Format = TextureFormat.Depth24PlusStencil8
                MipLevelCount = 1
                SampleCount = 1
            }

        
        let shaderCode =
            String.concat "\r\n" [
                "#version 450"
                "#ifdef Vertex"
                "layout(location = 0) in vec4 pos;"
                ""
                "void main()"
                "{"
                "    gl_Position = pos;"
                "}"
                "#endif"
                ""
                "#ifdef Fragment"
                "layout(location = 0) out vec4 color;"
                "void main()"
                "{"
                "    color = vec4(gl_FragCoord.xy / vec2(640, 480), 1, 1);"
                "}"
                "#endif"
            ]

        use vertexModule =
            dev.CreateGLSLShaderModule {
                ShaderStage = ShaderStage.Vertex
                Label       = "VS"
                EntryPoint  = "main"
                Defines     = ["Vertex"]
                Code        = shaderCode
            }

        use fragmentModule =
            dev.CreateGLSLShaderModule {
                ShaderStage = ShaderStage.Fragment
                Label       = "FS"
                EntryPoint  = "main"
                Defines     = ["Fragment"]
                Code        = shaderCode
            }

        use pipelineLayout = 
            dev.CreatePipelineLayout {
                Label = "Pipeline"
                BindGroupLayouts = [||]
            }

        use pipeline = 
            dev.CreateRenderPipeline {
                Label  = "Hans"
                AlphaToCoverageEnabled = false
                Layout  = pipelineLayout
                VertexStage = { Module = vertexModule; EntryPoint = "main" }
                FragmentStage = Some { Module = fragmentModule; EntryPoint = "main" }
                PrimitiveTopology = PrimitiveTopology.TriangleStrip
                SampleCount = 1
                SampleMask = 1
                RasterizationState =
                    Some {
                        FrontFace = FrontFace.CCW
                        CullMode = CullMode.None
                        DepthBias = 0
                        DepthBiasClamp = 0.0f
                        DepthBiasSlopeScale = 0.0f
                    }

                VertexState = 
                    Some { 
                        IndexFormat = IndexFormat.Uint32
                        VertexBuffers = 
                            [|
                                { 
                                    StepMode = InputStepMode.Vertex
                                    ArrayStride = 12UL
                                    Attributes =
                                        [|
                                            { Format = VertexFormat.Float3; Offset = 0UL; ShaderLocation = 0 }
                                        |]
                                }
                            |]
                    }
                DepthStencilState =  
                    Some {
                        Format = TextureFormat.Depth24PlusStencil8
                        DepthWriteEnabled = true
                        DepthCompare = CompareFunction.LessEqual
                        StencilFront = StencilStateFaceDescriptor.Default
                        StencilBack = StencilStateFaceDescriptor.Default
                        StencilReadMask = 0xFFFFFFFF
                        StencilWriteMask = 0xFFFFFFFF
                    }

                ColorStates = 
                    [|
                        { 
                            Format = swapChainFormat
                            AlphaBlend = BlendDescriptor.Default
                            ColorBlend = BlendDescriptor.Default
                            WriteMask = ColorWriteMask.All
                        }
                    |]
            }

        use depthView =
            depth.CreateView {
                Label = "Franz"
                Format = TextureFormat.Depth24PlusStencil8
                Dimension = TextureViewDimension.D2D
                BaseMipLevel = 0
                MipLevelCount = 1
                BaseArrayLayer = 0
                ArrayLayerCount = 1
                Aspect = TextureAspect.All
            }
        

        let arr = [| V3f.OOO; V3f.IOO; V3f.OIO; V3f.IIO |] |> Array.map (fun v -> v - V3f(0.5f, 0.5f, 0.0f))
        use buf = dev.CreateBuffer { Label = null; Size = 12UL * uint64 arr.Length; MappedAtCreation = false; Usage = BufferUsage.CopyDst ||| BufferUsage.Vertex }
        use ptr = fixed arr
        queue.WriteBuffer(buf, 0UL, NativePtr.toNativeInt ptr, 12un * unativeint arr.Length)

        let idx = [|0;1;2;3|]
        use ib = dev.CreateBuffer { Label = null; Size = 4UL * uint64 idx.Length; MappedAtCreation = false; Usage = BufferUsage.CopyDst ||| BufferUsage.Index }
        use ptr = fixed idx
        queue.WriteBuffer(ib, 0UL, NativePtr.toNativeInt ptr, 4un * unativeint arr.Length)

        glfw.PollEvents()
        let swapChainFormat = binding.GetPreferredSwapChainTextureFormat()

        let render() =  
            let mutable s = V2i.II
            glfw.GetFramebufferSize(win, &s.X, &s.Y)
            if false && s <> size then
                size <- s
                //let newChain = createSwapChain s
                //chain <- newChain
                
                chain.Configure(swapChainFormat, TextureUsage.OutputAttachment, s.X, s.Y)

            use tex = chain.GetCurrentTextureView()

            use cmd = dev.CreateCommandEncoder()
            use pass = 
                cmd.BeginRenderPass { 
                    Label = null
                    ColorAttachments = 
                        [|
                            { 
                                Attachment = tex
                                ResolveTarget = null
                                LoadOp = LoadOp.Clear
                                StoreOp = StoreOp.Store
                                ClearColor = { R = 0.5f; G = 0.5f; B = 0.5f; A = 1.0f }
                            }
                        |]
                    DepthStencilAttachment =
                        Some {
                            Attachment = depthView
                            DepthLoadOp = LoadOp.Clear
                            DepthStoreOp = StoreOp.Store
                            ClearDepth = 1.0f
                            DepthReadOnly = false
                            StencilLoadOp = LoadOp.Clear
                            StencilStoreOp = StoreOp.Store
                            ClearStencil = 0
                            StencilReadOnly = false
                        }
                        
                    OcclusionQuerySet = null
                }

            pass.SetPipeline(pipeline)
            //pass.SetIndexBufferWithFormat(ib, IndexFormat.Uint32, 0UL, 4UL * uint64 idx.Length)
            pass.SetVertexBuffer(0, buf, 0UL, 12UL * uint64 arr.Length)
            pass.Draw(arr.Length, 1, 0, 0)

            pass.EndPass()
            use buf = cmd.Finish()
            queue.Submit [| buf |]

            chain.Present()
            printfn "rendered"

        let mutable dirty = true
        glfw.SetWindowRefreshCallback(win, GlfwCallbacks.WindowRefreshCallback (fun w -> dirty <- true)) |> ignore
        glfw.PostEmptyEvent()
        while not (glfw.WindowShouldClose win) do
            if dirty then 
                dirty <- false
                render()
            glfw.WaitEvents()

    0
