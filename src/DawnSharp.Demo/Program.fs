// Learn more about F# at http://fsharp.org

open Aardvark.Base
open System
open WebGPU
open Microsoft.FSharp.NativeInterop
open System.Runtime.InteropServices

#nowarn "9"
#nowarn "40"

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

module ExprCompiler =
    open Aardvark.Base.IL
    open System.Reflection
    open System.Reflection.Emit
    open Microsoft.FSharp.Reflection
    open Microsoft.FSharp.Quotations
    open Microsoft.FSharp.Quotations.Patterns
    open Microsoft.FSharp.Quotations.DerivedPatterns
    open Aardvark.Base

    [<AutoOpen>]
    module ExprExtensions =

        type FSharpType with
            static member GetFunctionSignature(t : Type) =
                if FSharpType.IsFunction t then
                    let (a,r) = FSharpType.GetFunctionElements t
                    if FSharpType.IsTuple a then
                        let a0 = FSharpType.GetTupleElements a |> Array.toList
                        let (args, ret) = FSharpType.GetFunctionSignature(r)
                        a0 :: args, ret
                    else
                        let a0 = [a]
                        let (args, ret) = FSharpType.GetFunctionSignature(r)
                        a0 :: args, ret
                else
                    [], t



        type Expr with
            static member TupleGet(tup : Expr, i : int) =
                let prop = tup.Type.GetProperty(sprintf "Item%d" (i + 1))
                Expr.PropertyGet(tup, prop)

        let (|PipeRight|_|) (e : Expr) =
            match e with
            | Call(None, mi, [a;f]) when mi.Name = "op_PipeRight" -> Some (f, a)
            | _ -> None
            
        let (|PipeRight2|_|) (e : Expr) =
            match e with
            | Call(None, mi, [arg ;f]) when mi.Name = "op_PipeRight2" -> 
                match arg with
                | NewTuple [a;b] -> Some (f, a, b)
                | _ -> Some (f, Expr.TupleGet(arg, 0), Expr.TupleGet(arg, 1))
            | _ -> None

        let (|PipeLeft|_|) (e : Expr) =
            match e with
            | Call(None, mi, [f;a]) when mi.Name = "op_PipeLeft" -> Some (f, a)
            | _ -> None


        let (|OptionalCoerce|_|) (e : Expr) =
            match e with
                | Coerce(e,t) -> Some(e,t)
                | _ -> Some(e, e.Type)

        /// detects foreach expressions using the F# standard-layout
        let (|ForEach|_|) (e : Expr) =
            match e with
                | Let(e, Call(Some(OptionalCoerce(seq,_)), Method("GetEnumerator",_), []),
                        TryFinally(
                            WhileLoop(Call(Some (Var e1), Method("MoveNext",_), []),
                                Let(i, PropertyGet(Some (Var e2), current, []), b)
                            ),
                            IfThenElse(TypeTest(OptionalCoerce(Var e3, oType0), dType),
                                Call(Some (Call(None, Method("UnboxGeneric",_), [OptionalCoerce(e4, oType1)])), Method("Dispose",_), []),
                                Value(_)
                            )
                        )
                    ) when e1 = e && e2 = e && e3 = e && current.Name = "Current" && oType0 = typeof<obj> && oType1 = typeof<obj> && dType = typeof<System.IDisposable> ->
                    Some(i, seq, b)
                | _ -> 
                    None

        let (|CreateRange|_|) (e : Expr) =
            match e with
                | Call(None, Method("op_RangeStep",_), [first; step; last]) -> Some(first, step, last)
                | Call(None, Method("op_Range",_), [first; last]) -> Some(first, Expr.Value(1), last)
                | _ -> None

        let rec (|RangeSequence|_|) (e : Expr) =
            match e with
                | Call(None, Method(("ToArray" | "ToList"), _), [RangeSequence(first, step, last)]) ->
                    Some(first, step, last)

                | Call(None, Method("CreateSequence",_), [RangeSequence(first, step, last)]) ->
                    Some(first, step, last)

                | CreateRange(first, step, last) ->
                    Some(first, step, last)
                

                | _ -> 
                    None


        let (|ForInteger|_|) (e : Expr) =
            match e with
                | ForIntegerRangeLoop(v,first,last,body) -> 
                    Some(v,first,Expr.Value(1),last,body)



                | Let(seq, RangeSequence(first, step, last), ForEach(v, Var seq1, body)) when seq = seq1 ->
                    Some(v, first, step, last, body)


                | _ -> None


    module private rec Compiler = 
        let rec meth (e : Expr) =
            match e with
            | Lambda(_, b) -> meth b
            | Call(_,mi,_) -> mi
            | PropertyGet(_,p,_) -> p.GetMethod
            | Sequential(_, r) -> meth r
            | Let(_,_,b) -> meth b
            | TryWith(_,_,_,_,h) -> meth h
            | _ -> failwithf "bad meth: %A" e 

        let table (a : seq<'a * 'b>) =
            let d = System.Collections.Generic.Dictionary<'a, 'b>()
            for (k, v) in a do
                d.[k] <- v
            fun a ->
                match d.TryGetValue a with
                | (true, v) -> Some v
                | _ -> None


      
        type VarAccess =
            | Local of LocalBuilder
            | Field of FieldInfo
            | Argument of int
            | Property of VarAccess * PropertyInfo
            
            member x.Type =
                match x with
                | Local l -> l.LocalType
                | Field f -> f.FieldType
                | Argument _ -> failwith "no arg type"
                | Property(_,p) -> p.PropertyType


            member x.write (il : ILGenerator) =
                match x with
                | Local l -> 
                    il.Emit(OpCodes.Stloc, l)
                | Field f -> 
                    let tmp = il.DeclareLocal(f.FieldType)
                    il.Emit(OpCodes.Stloc, tmp)
                    il.Emit(OpCodes.Ldarg_0)
                    il.Emit(OpCodes.Ldloc, tmp)
                    il.Emit(OpCodes.Stfld, f)
                | Argument idx -> 
                    il.Emit(OpCodes.Starg, idx + 1)
                | Property(a, p) ->
                    let tmp = il.DeclareLocal(p.PropertyType)
                    il.Emit(OpCodes.Stloc, tmp)
                    a.read il
                    il.Emit(OpCodes.Ldloc, tmp)
                    il.EmitCall(OpCodes.Callvirt, p.SetMethod, null)

            member x.read (il : ILGenerator) =
                match x with
                | Local l -> 
                    il.Emit(OpCodes.Ldloc, l)
                | Field f -> 
                    il.Emit(OpCodes.Ldarg_0)
                    il.Emit(OpCodes.Ldfld, f)
                | Argument idx -> 
                    match idx with
                    | 0 -> il.Emit(OpCodes.Ldarg_1)
                    | 1 -> il.Emit(OpCodes.Ldarg_2)
                    | 2 -> il.Emit(OpCodes.Ldarg_3)
                    | _ -> il.Emit(OpCodes.Ldarg, idx + 1)
                    
                | Property(a, p) -> 
                    a.read il
                    let get = p.GetMethod
                    if get.IsVirtual then il.EmitCall(OpCodes.Callvirt, get, null)
                    else il.EmitCall(OpCodes.Call, get, null)

        type State =
            {
                il                  : ILGenerator
                vars                : Map<Var, VarAccess>
                currentException    : option<VarAccess>
            }
            
        let builtins =
            table [|
                meth <@ (+) : uint8 -> uint8 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Add))
                meth <@ (+) : uint16 -> uint16 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Add))
                meth <@ (+) : uint32 -> uint32 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Add))
                meth <@ (+) : uint64 -> uint64 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Add))
                meth <@ (+) : int8 -> int8 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Add))
                meth <@ (+) : int16 -> int16 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Add))
                meth <@ (+) : int32 -> int32 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Add))
                meth <@ (+) : int64 -> int64 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Add))
                
                meth <@ (-) : uint8 -> uint8 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Sub))
                meth <@ (-) : uint16 -> uint16 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Sub))
                meth <@ (-) : uint32 -> uint32 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Sub))
                meth <@ (-) : uint64 -> uint64 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Sub))
                meth <@ (-) : int8 -> int8 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Sub))
                meth <@ (-) : int16 -> int16 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Sub))
                meth <@ (-) : int32 -> int32 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Sub))
                meth <@ (-) : int64 -> int64 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Sub))

                
                meth <@ (*) : uint8 -> uint8 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Mul))
                meth <@ (*) : uint16 -> uint16 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Mul))
                meth <@ (*) : uint32 -> uint32 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Mul))
                meth <@ (*) : uint64 -> uint64 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Mul))
                meth <@ (*) : int8 -> int8 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Mul))
                meth <@ (*) : int16 -> int16 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Mul))
                meth <@ (*) : int32 -> int32 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Mul))
                meth <@ (*) : int64 -> int64 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Mul))

            
                meth <@ (/) : uint8 -> uint8 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Div_Un))
                meth <@ (/) : uint16 -> uint16 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Div_Un))
                meth <@ (/) : uint32 -> uint32 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Div_Un))
                meth <@ (/) : uint64 -> uint64 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Div_Un))
                meth <@ (/) : int8 -> int8 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Div))
                meth <@ (/) : int16 -> int16 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Div))
                meth <@ (/) : int32 -> int32 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Div))
                meth <@ (/) : int64 -> int64 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Div))

                meth <@ (<) : int32 -> int32 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Clt))
                meth <@ (>) : int32 -> int32 -> _ @>, (fun (il : State) -> il.il.Emit(OpCodes.Cgt))
                
                meth <@ raise @>, (fun s -> 
                    s.il.Emit(OpCodes.Throw)
                )
                meth <@ try () with _ -> reraise() @>, (fun s -> 
                    let ex = s.currentException.Value
                    ex.read s.il
                    s.il.Emit(OpCodes.Throw)
                    //s.il.ThrowException ex.Type
                )

            |]

        let bAss = AssemblyBuilder.DefineDynamicAssembly(AssemblyName "compiler", AssemblyBuilderAccess.RunAndCollect)
        let bMod = bAss.DefineDynamicModule "main"

        let rec getFrontendFSharpFuncType (args : list<list<Type>>) (ret : Type) =
            let pars = 
                args |> List.map (fun a ->
                    match a with
                    | [] -> failwithf "empty tuple"
                    | [a] -> a
                    | many -> FSharpType.MakeTupleType (many |> List.toArray)
                )

            let rec buildType (pars : list<Type>) (ret : Type) = 
                match pars with
                | [] -> 
                    ret
                | a::rest ->
                    typedefof<FSharpFunc<_,_>>.MakeGenericType [| a; buildType rest ret |]

            buildType pars ret
      
        let rec getFSharpFuncType (args : list<list<Type>>) (ret : Type) =
            let pars = 
                args |> List.map (fun a ->
                    match a with
                    | [] -> failwithf "empty tuple"
                    | [a] -> a
                    | many -> FSharpType.MakeTupleType (many |> List.toArray)
                )

            let rec buildType (pars : list<Type>) (ret : Type)= 

                match pars with
                | [] -> ret
                | a::b::c::d::e::rest ->
                    typedefof<OptimizedClosures.FSharpFunc<_,_,_,_,_,_>>.MakeGenericType [| a; b; c; d; e; buildType rest ret |]
                | a::b::c::d::rest ->
                    typedefof<OptimizedClosures.FSharpFunc<_,_,_,_,_>>.MakeGenericType [| a; b; c; d; buildType rest ret |]
                | a::b::c::rest ->
                    typedefof<OptimizedClosures.FSharpFunc<_,_,_,_>>.MakeGenericType [| a; b; c; buildType rest ret |]
                | a::b::rest ->
                    typedefof<OptimizedClosures.FSharpFunc<_,_,_>>.MakeGenericType [| a; b; buildType rest ret |]
                | a::rest ->
                    typedefof<FSharpFunc<_,_>>.MakeGenericType [| a; buildType rest ret |]

            buildType pars ret

        and push (state : State) (a : list<Expr>) =
            match a with
            | [a] -> 
                compileExpr state a
                a.Type
            | many ->
                let mt = many |> List.map (fun a -> a.Type) |> List.toArray
                let tup = mt |> FSharpType.MakeTupleType
                for m in many do
                    compileExpr state m
                state.il.Emit(OpCodes.Newobj, tup.GetConstructor mt)
                tup

        and invoke (state : State) (stackType : Type) (ret : Type) (args : list<list<Expr>>) : unit =
            match args with
            | [] ->
                ()

            | [a0] ->
                let t0 = push state a0
                let typ = typedefof<int -> int>
                let typ = typ.MakeGenericType [| t0; ret |]

                let mi = 
                    typ.GetMethods(BindingFlags.Instance ||| BindingFlags.Public)
                    |> Array.find (fun mi -> mi.Name = "Invoke" && mi.GetParameters().Length = 1)
                state.il.EmitCall(OpCodes.Callvirt, mi, null)
                if mi.ReturnType = typeof<unit> then state.il.Emit(OpCodes.Pop)
            
            | [a0; a1] ->
                let t0 = push state a0
                let t1 = push state a1
                let typ = typedefof<int -> int>
                let typ = typ.MakeGenericType [| t0; t1 |]

                let mig = 
                    typ.GetMethods(BindingFlags.Static ||| BindingFlags.Public)
                    |> Array.find (fun mi -> mi.Name = "InvokeFast" && mi.GetParameters().Length = 3)
                let mi = mig.MakeGenericMethod [| ret |]
                state.il.EmitCall(OpCodes.Call, mi, null)
                if mi.ReturnType = typeof<unit> then state.il.Emit(OpCodes.Pop)
                    
            | [a0; a1; a2] ->
                let t0 = push state a0
                let t1 = push state a1
                let t2 = push state a2
                let typ = typedefof<int -> int>
                let typ = typ.MakeGenericType [| t0; t1 |]

                let mig = 
                    typ.GetMethods(BindingFlags.Static ||| BindingFlags.Public)
                    |> Array.find (fun mi -> mi.Name = "InvokeFast" && mi.GetParameters().Length = 4)
                let mi = mig.MakeGenericMethod [| t2; ret |]
                state.il.EmitCall(OpCodes.Call, mi, null)
                if mi.ReturnType = typeof<unit> then state.il.Emit(OpCodes.Pop)
                    
            | [a0; a1; a2; a3] ->
                let t0 = push state a0
                let t1 = push state a1
                let t2 = push state a2
                let t3 = push state a3
                let typ = typedefof<int -> int>
                let typ = typ.MakeGenericType [| t0; t1 |]

                let mig = 
                    typ.GetMethods(BindingFlags.Static ||| BindingFlags.Public)
                    |> Array.find (fun mi -> mi.Name = "InvokeFast" && mi.GetParameters().Length = 5)
                let mi = mig.MakeGenericMethod [| t2; t3; ret |]
                state.il.EmitCall(OpCodes.Call, mi, null)
                if mi.ReturnType = typeof<unit> then state.il.Emit(OpCodes.Pop)

            | [a0; a1; a2; a3; a4] ->
                let t0 = push state a0
                let t1 = push state a1
                let t2 = push state a2
                let t3 = push state a3
                let t4 = push state a4
                let typ = typedefof<int -> int>
                let typ = typ.MakeGenericType [| t0; t1 |]

                let mig = 
                    typ.GetMethods(BindingFlags.Static ||| BindingFlags.Public)
                    |> Array.find (fun mi -> mi.Name = "InvokeFast" && mi.GetParameters().Length = 6)
                let mi = mig.MakeGenericMethod [| t2; t3; t4; ret |]
                state.il.EmitCall(OpCodes.Call, mi, null)
                if mi.ReturnType = typeof<unit> then state.il.Emit(OpCodes.Pop)
                    
            | a0 :: a1 :: a2 :: a3 :: a4 :: rest ->
                let t0 = push state a0
                let t1 = push state a1
                let t2 = push state a2
                let t3 = push state a3
                let t4 = push state a4
                let typ = typedefof<int -> int>
                let typ = typ.MakeGenericType [| t0; t1 |]

                let mig = 
                    typ.GetMethods(BindingFlags.Static ||| BindingFlags.Public)
                    |> Array.find (fun mi -> mi.Name = "InvokeFast" && mi.GetParameters().Length = 6)
                let fType = getFSharpFuncType (args |> List.map (List.map (fun e -> e.Type))) ret
                let mi = mig.MakeGenericMethod [| t2; t3; t4; fType |]


                state.il.EmitCall(OpCodes.Call, mi, null)
                invoke state fType ret rest
                    
        and compileExpr (state : State) (e : Expr) : unit =
            match e with
            | PipeRight(f, a) | PipeLeft(f, a) ->
                compileExpr state (Expr.Application(f, a))
                
            | Call(None, mi, [arr; idx]) when mi.Name = "GetArray" ->
                compileExpr state arr
                compileExpr state idx
                state.il.Emit(OpCodes.Ldelem, arr.Type.GetElementType())

            | Call(None, mi, [exn]) when mi.Name = "Raise" && typeof<Exception>.IsAssignableFrom exn.Type ->
                compileExpr state exn
                state.il.Emit(OpCodes.Throw)
                //state.il.ThrowException(exn.Type)
            

            | Application(Lambda(v,b), e) ->
                compileExpr state (Expr.Let(v, e, b))

            | ForEach(v, seq, body) ->
                let l = state.il.DeclareLocal(v.Type)
                let e = state.il.DeclareLocal(typedefof<System.Collections.Generic.IEnumerator<_>>.MakeGenericType [| v.Type |])

                compileExpr state seq
                let getEnum = seq.Type.GetInterface(typedefof<seq<_>>.FullName).GetMethod("GetEnumerator")
                state.il.EmitCall(OpCodes.Callvirt, getEnum, null)
                state.il.Emit(OpCodes.Stloc, e)


                let moveNext = e.LocalType.GetInterface(typeof<System.Collections.IEnumerator>.FullName).GetMethod "MoveNext"
                let current = e.LocalType.GetProperty "Current"
                let dispose = e.LocalType.GetInterface(typeof<System.IDisposable>.FullName).GetMethod "Dispose"

                let _s = state.il.DefineLabel()
                let _e = state.il.DefineLabel()
                
                let ex = state.il.BeginExceptionBlock()

                state.il.MarkLabel(_s)
                state.il.Emit(OpCodes.Ldloc, e)
                state.il.EmitCall(OpCodes.Callvirt, moveNext, null)
                state.il.Emit(OpCodes.Brfalse, _e)

                state.il.Emit(OpCodes.Ldloc, e)
                state.il.EmitCall(OpCodes.Callvirt, current.GetMethod, null)
                state.il.Emit(OpCodes.Stloc, l)

                compileExpr { state with vars = Map.add v (Local l) state.vars } body

                state.il.Emit(OpCodes.Br, _s)

                state.il.MarkLabel(_e)
                
                state.il.BeginFinallyBlock()
                state.il.Emit(OpCodes.Ldloc, e)
                state.il.EmitCall(OpCodes.Callvirt, dispose, null)
                state.il.EndExceptionBlock()

            | ForInteger(v, start, step, stop, body) ->
                let l = state.il.DeclareLocal v.Type
                let e = state.il.DeclareLocal v.Type
                let s = state.il.DeclareLocal v.Type

                compileExpr state start
                state.il.Emit(OpCodes.Stloc, l)

                compileExpr state stop
                state.il.Emit(OpCodes.Stloc, e)

                compileExpr state step
                state.il.Emit(OpCodes.Stloc, s)

                let _e = state.il.DefineLabel()
                let _s = state.il.DefineLabel()

                state.il.MarkLabel(_s)
                state.il.Emit(OpCodes.Ldloc, l)
                state.il.Emit(OpCodes.Ldloc, e)
                state.il.Emit(OpCodes.Bgt, _e)

                compileExpr { state with vars = Map.add v (Local l) state.vars } body

                state.il.Emit(OpCodes.Ldloc, l)
                state.il.Emit(OpCodes.Ldloc, s)
                state.il.Emit(OpCodes.Add)
                state.il.Emit(OpCodes.Stloc, l)

                state.il.Emit(OpCodes.Br, _s)

                state.il.MarkLabel(_e)

            | CallWithWitnesses(t, original, witness, lambdas, args) ->
                match builtins original with
                | Some emit ->
                    match t with
                    | Some t -> compileExpr state t
                    | None -> ()
                    for a in args do compileExpr state a
                    emit state

                | None ->
                    match t with
                    | Some t -> compileExpr state t
                    | None -> ()

                    //inlineCode state t witness lambdas args

                    let pp = List.append lambdas args
                    for arg, p in List.zip pp (Array.toList (witness.GetParameters())) do
                        // TODO: better lambda compiler (using builtins?)
                        compileExpr state arg

                    if witness.IsVirtual then state.il.EmitCall(OpCodes.Callvirt, witness, null)
                    else state.il.EmitCall(OpCodes.Call, witness, null)
                    if witness.ReturnType = typeof<unit> then state.il.Emit(OpCodes.Pop)

            | AddressOf _ | AddressSet _ ->
                failwith "pointers not implemented"

            | DefaultValue t ->
                if t.IsValueType then state.il.Emit(OpCodes.Newobj, t.GetConstructor [||])
                else state.il.Emit(OpCodes.Ldnull)
                
            | FieldGet(Some t, fld) ->
                compileExpr state t
                state.il.Emit(OpCodes.Ldfld, fld)
            
            | FieldGet(None, fld) ->
                state.il.Emit(OpCodes.Ldsfld, fld)
            
            | FieldSet(Some t, fld, value) ->
                compileExpr state t
                compileExpr state value
                state.il.Emit(OpCodes.Stfld, fld)

            | FieldSet(None, fld, value) ->
                compileExpr state value
                state.il.Emit(OpCodes.Stsfld, fld)
                
            | IfThenElse(c, i, e) ->
                let lf = state.il.DefineLabel()
                let le = state.il.DefineLabel()
                compileExpr state c
                state.il.Emit(OpCodes.Brfalse, lf)
                compileExpr state i
                state.il.Emit(OpCodes.Br, le)
                state.il.MarkLabel lf
                compileExpr state e
                state.il.MarkLabel le
                
            | NewArray(t, args) ->
                let l = state.il.DeclareLocal(t.MakeArrayType())
                let cnt = List.length args
                state.il.Emit(OpCodes.Ldc_I4, cnt)
                state.il.Emit(OpCodes.Newarr, t)
                state.il.Emit(OpCodes.Stloc, l)

                let mutable idx = 0
                for a in args do
                    state.il.Emit(OpCodes.Ldloc, l)
                    state.il.Emit(OpCodes.Ldc_I4, idx)
                    compileExpr state a
                    state.il.Emit(OpCodes.Stelem, t)
                    idx <- idx + 1
                    
                state.il.Emit(OpCodes.Ldloc, l)

            | NewDelegate(typ, vars, body) ->
                failwith "delegate"
                
            | NewObject(ctor, args) ->
                for a in args do compileExpr state a
                state.il.Emit(OpCodes.Newobj, ctor)

            | NewRecord(typ, args) ->
                let pars = args |> List.map (fun a -> a.Type) |> List.toArray
                let ctor = typ.GetConstructor(BindingFlags.Public ||| BindingFlags.NonPublic ||| BindingFlags.Instance, Type.DefaultBinder, pars, null)
                for a in args do
                    compileExpr state a
                state.il.Emit(OpCodes.Newobj, ctor)

            | NewTuple(args) ->
                let pars = args |> List.map (fun a -> a.Type) |> List.toArray
                let ctor = e.Type.GetConstructor pars
                for a in args do
                    compileExpr state a
                state.il.Emit(OpCodes.Newobj, ctor)

            | NewUnionCase(ci, args) ->
                let meth = ci.DeclaringType.GetMethod(ci.Name)
                for a in args do
                    compileExpr state a
                state.il.EmitCall(OpCodes.Call, meth, null)



            | Let(v, (Lambdas _ as e), b) ->
                b.Substitute (fun vi ->
                    if vi = v then Some e
                    else None
                ) |> compileExpr state

            | Lambdas(args, body) ->
                let _lType, lCtor, lClosure = compileFunction args body
                for m in lClosure do
                    compileExpr state (Expr.Var m)
                state.il.Emit(OpCodes.Newobj, lCtor)
                
            | Let(v, e, b) ->
                let l = state.il.DeclareLocal(v.Type)
                compileExpr state e
                state.il.Emit(OpCodes.Stloc, l)
                compileExpr { state with vars = Map.add v (Local l) state.vars } b

            | Coerce(e, t) ->
                compileExpr state e
                match e.Type.IsValueType, t.IsValueType with
                | false, true -> state.il.Emit(OpCodes.Unbox, t)
                | true, false -> state.il.Emit(OpCodes.Box, e.Type)
                | false, false -> state.il.Emit(OpCodes.Castclass, t)
                | true, true -> failwith "cannot coerce structs"

            | Call(t, mi, args) ->
                match t with
                | Some t -> compileExpr state t
                | None -> ()
                for a in args do
                    compileExpr state a

                match builtins mi with
                | Some emit ->
                    emit state
                | None -> 
                    let code = if mi.IsVirtual then OpCodes.Callvirt else OpCodes.Call
                    state.il.EmitCall(code, mi, null)
                    if mi.ReturnType = typeof<unit> then state.il.Emit(OpCodes.Pop)

            
            | Sequential(l, r) ->
                compileExpr state l
                compileExpr state r

            | VarSet(v, e) ->
                compileExpr state e
                state.vars.[v].write state.il
                
            | PropertyGet(Some t, prop, idx) ->
                compileExpr state (Expr.Call(t, prop.GetMethod, idx))

            | PropertyGet(None, prop, idx) ->
                compileExpr state (Expr.Call(prop.GetMethod, idx))

            | PropertySet(Some t, prop, idx, value) ->
                compileExpr state (Expr.Call(t, prop.SetMethod, idx @ [value]))
                
            | PropertySet(None, prop, idx, value) ->
                compileExpr state (Expr.Call(prop.SetMethod, idx @ [value]))

            | Applications(expr, args) ->
                compileExpr state expr
                invoke state expr.Type e.Type args

            | QuoteRaw e | QuoteTyped e ->
                failwith "quote"

            | TypeTest(e, t) ->
                compileExpr state e
                if not e.Type.IsValueType then state.il.Emit(OpCodes.Box, e.Type)
                state.il.Emit(OpCodes.Isinst, t)

            | TryFinally(t, f) ->
                state.il.BeginExceptionBlock() |> ignore
                compileExpr state t
                state.il.BeginFinallyBlock()
                compileExpr state f
                state.il.EndExceptionBlock()

            | TryWith(body, _, _, var, handler) ->

                let block = state.il.BeginExceptionBlock()
                compileExpr state body

                
                let rec visit (e : Expr) =
                    match e with
                    | IfThenElse(TypeTest(Var ex, exType), comp, rest) when ex = var ->
                        state.il.BeginCatchBlock(exType)
                        let l = state.il.DeclareLocal(exType)
                        state.il.Emit(OpCodes.Stloc, l)
                        compileExpr { state with vars = Map.add ex (Local l) state.vars; currentException = Some (Local l) } comp

                        visit rest
                    | Call(None, r, []) when r.Name = "Reraise" ->
                        ()
                    | _ ->  
                        failwith "bad catch"

                visit handler

                state.il.EndExceptionBlock()

            | TupleGet(tup, item) ->
                let prop = tup.Type.GetProperty(sprintf "Item%d" (item + 1))
                compileExpr state (Expr.Call(tup, prop.GetMethod, []))

            | UnionCaseTest(e, ci) ->
                let tp = e.Type.GetProperty "Tag"
                let tt = ci.Tag
                compileExpr state <@ %%(Expr.PropertyGet(e, tp)) = tt @>

            | Var v ->
                match Map.tryFind v state.vars with
                | Some v -> v.read state.il
                | None -> failwithf "unbound variable: %A" v

            | WhileLoop(guard, body) ->
                
                let _s = state.il.DefineLabel()
                let _e = state.il.DefineLabel()


                state.il.MarkLabel(_s)
                compileExpr state guard
                state.il.Emit(OpCodes.Brfalse, _e)

                compileExpr state body
                state.il.Emit(OpCodes.Br, _s)

                state.il.MarkLabel(_e)





            | Value(value, typ) ->  
                if typ = typeof<unit> then
                    state.il.Emit(OpCodes.Ldnull)
                elif typ = typeof<string> then
                    state.il.Emit(OpCodes.Ldstr, unbox<string> value)
                elif typ = typeof<int8> then 
                    match unbox<int8> value with
                    | 0y -> state.il.Emit(OpCodes.Ldc_I4_0)
                    | 1y -> state.il.Emit(OpCodes.Ldc_I4_1)
                    | 2y -> state.il.Emit(OpCodes.Ldc_I4_2)
                    | 3y -> state.il.Emit(OpCodes.Ldc_I4_3)
                    | 4y -> state.il.Emit(OpCodes.Ldc_I4_4)
                    | 5y -> state.il.Emit(OpCodes.Ldc_I4_5)
                    | 6y -> state.il.Emit(OpCodes.Ldc_I4_6)
                    | 7y -> state.il.Emit(OpCodes.Ldc_I4_7)
                    | 8y -> state.il.Emit(OpCodes.Ldc_I4_8)
                    | v -> state.il.Emit(OpCodes.Ldc_I4, int v)
                elif typ = typeof<int> then
                    match unbox<int> value with
                    | 0 -> state.il.Emit(OpCodes.Ldc_I4_0)
                    | 1 -> state.il.Emit(OpCodes.Ldc_I4_1)
                    | 2 -> state.il.Emit(OpCodes.Ldc_I4_2)
                    | 3 -> state.il.Emit(OpCodes.Ldc_I4_3)
                    | 4 -> state.il.Emit(OpCodes.Ldc_I4_4)
                    | 5 -> state.il.Emit(OpCodes.Ldc_I4_5)
                    | 6 -> state.il.Emit(OpCodes.Ldc_I4_6)
                    | 7 -> state.il.Emit(OpCodes.Ldc_I4_7)
                    | 8 -> state.il.Emit(OpCodes.Ldc_I4_8)
                    | v -> state.il.Emit(OpCodes.Ldc_I4, v)
                else
                    failwithf "bad constant: %A" value
            | _ ->
                failwithf "bad expr: %A" e
                ()


        and compileFunction (args : list<list<Var>>) (body : Expr) : Type * ConstructorInfo * list<Var> =
        
            let loads initial (args : list<list<Var>>) =
                (initial, Seq.indexed args) ||> Seq.fold (fun map (ai, vs) ->
                    match vs with
                    | [v] -> 
                        Map.add v (Argument ai) map
                    | _ ->
                        let typ = FSharpType.MakeTupleType (vs |> List.map (fun v -> v.Type) |> List.toArray)
                        (map, Seq.indexed vs) ||> Seq.fold (fun map (ti, v) ->
                            let prop = typ.GetProperty(sprintf "Item%d" (ti + 1))
                            Map.add v (Property(Argument ai, prop)) map
                        )
                )


            let rec compile (args : list<list<Var>>) (body : Expr) : ConstructorInfo * list<Var> =
                match args with
                | ([] as args) 
                | ([_] as args) 
                | ([_;_] as args) 
                | ([_;_;_] as args) 
                | ([_;_;_;_] as args) 
                | ([_;_;_;_;_] as args) ->
                    let fType = getFSharpFuncType (args |> List.map (List.map (fun a -> a.Type))) body.Type
                    let bType = bMod.DefineType(Guid.NewGuid().ToString(), TypeAttributes.Class, fType)

                    let free = 
                        let set = body.GetFreeVars() |> Set.ofSeq 
                        (set, args) ||> List.fold (fun set a0 ->
                            (set, a0) ||> List.fold (fun s a -> Set.remove a s)
                        ) |> Set.toList

                    // ctor
                    let ctorArgs = free |> Seq.map (fun f -> f.Type) |> Seq.toArray
                    let fields = ctorArgs |> Array.mapi (fun i t -> bType.DefineField(sprintf "closure%d" i, t, FieldAttributes.InitOnly ||| FieldAttributes.Private))
                    let bCtor = bType.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, ctorArgs)
                    let il = bCtor.GetILGenerator()
                    for i, f in Seq.indexed fields do
                        il.Emit(OpCodes.Ldarg_0)
                        il.Emit(OpCodes.Ldarg, i+1)
                        il.Emit(OpCodes.Stfld, f)
                    il.Emit(OpCodes.Ret)

                    // invoke
                    let argTypes = 
                        args |> List.toArray |> Array.map (fun a0 ->
                            match a0 with 
                            | [a] -> a.Type 
                            | _ -> FSharpType.MakeTupleType(a0 |> List.map (fun a -> a.Type) |> List.toArray)
                        )

                    let bInvoke = bType.DefineMethod("Invoke", MethodAttributes.Virtual ||| MethodAttributes.Public, body.Type, argTypes)
                    let il = bInvoke.GetILGenerator()
                    let closure = 
                        (free, Array.toList fields) ||> List.map2 (fun v f -> 
                            v, Field f
                        ) 
                        |> Map.ofList

                    compileExpr { il = il; vars = loads closure args; currentException = None } body
                    if body.Type = typeof<unit> then il.Emit(OpCodes.Ldnull)
                    il.Emit(OpCodes.Ret)

                    // overrides
                    let baseInvoke = fType.GetMethod("Invoke", argTypes)
                    bType.DefineMethodOverride(bInvoke, baseInvoke)


                    let t = bType.CreateType()
                    let ctor = t.GetConstructor ctorArgs

                    

                    ctor, free

                | a0::a1::a2::a3::a4::rest ->
                    let cRest, closureRest = compile rest body
                    let tRest = getFrontendFSharpFuncType (rest |> List.map (List.map (fun a -> a.Type))) body.Type

                    compile [a0;a1;a2;a3;a4] (
                        Expr.Coerce(Expr.NewObject(cRest, closureRest |> List.map Expr.Var), tRest)
                    )


            let ctor, closure = compile args body
            let typ = getFrontendFSharpFuncType (args |> List.map (List.map (fun a -> a.Type))) body.Type
            typ, ctor, closure
    

        and inlineCode (state : State) (target : option<Expr>) (witness : MethodInfo) (lambdas : list<Expr>) (args : list<Expr>) =
            let def = Disassembler.disassemble witness
            let lambdas = List.toArray lambdas

            let pars = witness.GetParameters() |> Array.skip lambdas.Length |> Array.toList

            let mutable overrides = Map.empty

            match target with
            | Some target ->
                failwith "non-static"
            | None ->
                ()

            for pi, (p, a) in Seq.indexed (List.zip pars args) do
                compileExpr state a
                let l = state.il.DeclareLocal a.Type
                state.il.Emit(OpCodes.Stloc, l)
                overrides <- Map.add (pi + lambdas.Length) (Choice1Of2 (Local l)) overrides
            
            for pi, l in Seq.indexed lambdas do
                overrides <- Map.add pi (Choice2Of2 l) overrides
                

            let rec skip (n : int) (l : list<'a>) =
                if n <= 0 then l
                else
                    match l with
                    | [] -> []
                    | _ :: t -> skip (n-1) t
                    

            //let assState : Assembler.State = { Assembler.State.generator = state.il; locals = Map.empty; labels = Map.empty; stack = [] }

            let dict = Dict<Local, LocalBuilder>()

            let loc (l : Local) = dict.GetOrCreate(l, fun l -> state.il.DeclareLocal(l.Type))

            let rec run (stack : list<Option<Expr>>) (vars : Map<Local, Expr>) (i : list<Instruction>) =
                
                match i with
                | [] ->
                    ()

                | Ldarg idx :: rest ->
                    match Map.tryFind idx overrides with
                    | Some (Choice1Of2 acc) ->
                        acc.read state.il
                        run (None :: stack) vars rest
                    | Some (Choice2Of2 lambda) -> 
                        run (Some lambda :: stack) vars rest
                    | None ->
                        failwithf "bad arg: %A" idx

                | Stloc l :: rest ->
                    match stack with
                    | Some h :: t ->
                        run t (Map.add l h vars) rest
                    | None :: t ->
                        state.il.Emit(OpCodes.Stloc, loc l)
                        //Assembler.assembleTo state.il [Stloc l]
                        run t vars rest
                    | _ ->
                        failwith "empty stack"
                        
                | Ldloc l :: rest ->
                    match Map.tryFind l vars with
                    | Some e ->
                        run (Some e :: stack) vars rest
                    | None ->
                        state.il.Emit(OpCodes.Ldloc, loc l)
                        //Assembler.assembleTo' state.il [Ldloc l]
                        run (None :: stack) vars rest
                      
                | Ret :: rest ->
                    run stack vars rest
                    
                | Tail :: rest ->
                    run stack vars rest

                | IL.Call mi :: rest when mi.Name = "Invoke" || mi.Name = "InvokeFast" ->
                    let mi = unbox<MethodInfo> mi
                    let cnt = 
                        if mi.Name = "InvokeFast" then mi.GetParameters().Length - 1 else mi.GetParameters().Length


                    let newStack = stack |> skip cnt
                    match newStack with
                    | Some current :: newStack -> 
                        match current with
                        | Lambdas(args, body) ->
                            let mutable vv = state.vars

                            for ai, a in List.rev (List.indexed args) do
                                match a with
                                | [a] -> 
                                    let l = state.il.DeclareLocal(a.Type)
                                    state.il.Emit(OpCodes.Stloc, l)
                                    vv <- Map.add a (Local l) vv
                                    //overrides <- Map.add ai (Choice1Of2 (Local l)) overrides
                                | many ->
                                    let t = FSharpType.MakeTupleType(many |> List.map (fun a -> a.Type) |> List.toArray)
                                    let lt = state.il.DeclareLocal(t)
                                    state.il.Emit(OpCodes.Stloc, lt)
                                    //overrides <- Map.add ai (Choice1Of2 (Local lt)) overrides

                                    for ti, a in Seq.indexed many do
                                        let get = t.GetProperty(sprintf "Item%d" (ti + 1)).GetMethod
                                        let l = state.il.DeclareLocal(a.Type)
                                        state.il.Emit(OpCodes.Ldloc, lt)
                                        state.il.EmitCall(OpCodes.Call, get, null)
                                        state.il.Emit(OpCodes.Stloc, l)
                                        vv <- Map.add a (Local l) vv

                            
                            compileExpr { state with vars = vv } body

                            let newStack =
                                if mi.ReturnType <> typeof<System.Void> then None :: newStack
                                else newStack
                            run newStack vars rest
                                
                        | _ ->
                            failwith "bad witness"
                    | _ ->
                        failwith "bad call"
                        //state.il.EmitCall(OpCodes.Call, unbox mi, null)
                        //run newStack vars rest

                | i :: rest ->
                    let assState : Assembler.State =
                        { 
                            Assembler.State.generator = state.il
                            locals = Dict.toMap dict
                            labels = Map.empty
                            stack = []
                        }
                    Assembler.assembleTo' assState [i]
                    run stack vars rest

            let overrides =
                lambdas |> Array.mapi (fun i l -> i, Choice2Of2 l) |> Map.ofArray

            //for i in def.Body do
            //    printfn "%A" i

            run [] Map.empty def.Body



        type DisassemblerState =
            {
                args : Map<int, Expr>
                vars : Map<Local, Var>
            }

        module DisassemblerState =
            let ofMethod (meth : MethodBase) (def : MethodDefinition) =
                
                let mutable args, i =
                    if meth.IsStatic then Map.empty, 0
                    else Map.ofList [0, Var("this", meth.DeclaringType, true) ], 1

                for p in meth.GetParameters() do
                    let var = Var(p.Name, p.ParameterType, true)
                    args <- Map.add i var args
                    i <- i + 1



                let rec parts (idx : int) (il : list<Instruction>) =
                    match il with
                    | [] -> []
                    | Instruction.Mark l :: rest ->
                        let parts = parts (idx + 1) rest
                        (l, idx) :: parts
                    | _ :: rest ->
                        parts (idx + 1) rest

                let p = parts 0 def.Body
                printfn "%A" p

                {
                    args = args |> Map.map (fun _ -> Expr.Var)
                    vars = Map.empty
                }


        let disassemble (mi : MethodBase) =
            let def = Disassembler.disassemble mi
   
            let rec toExpr (stack : list<Expr>) (state : DisassemblerState) (i : list<Instruction>) =
                match i with
                | [] ->
                    failwithf "bad end: %A" stack
                    
                | Instruction.Tail :: rest
                | Instruction.Start :: rest ->
                    toExpr stack state rest
                    
                //| Instruction.Leave l :: rest ->
                //    toExpr stack args vars rest
                    

                //| Instruction. :: rest ->
                //    toExpr stack vars rest

                | Instruction.Call mi :: rest ->    
                    let pars = mi.GetParameters()
                    let parCount =
                        if mi.IsStatic then pars.Length
                        else 1 + pars.Length

                    let a = List.take parCount stack |> List.rev
                    let s = List.skip parCount stack

                    if mi.IsStatic then
                        toExpr (Expr.Call(unbox mi, a) :: s) state rest
                    else
                        match a with
                        | self :: a -> toExpr (Expr.Call(self, unbox mi, a) :: s) state rest
                        | [] -> failwith "bad"


                | Instruction.Ret :: _ ->
                    match stack with
                    | [e] -> e
                    | _ -> failwithf "cannot return: %A" stack

                | Instruction.Stloc l :: rest ->
                    let vars, var, isSet = 
                        match Map.tryFind l state.vars with
                        | Some var -> 
                            state.vars, var, true
                        | None ->
                            let v = Var(l.Name, l.Type, true)
                            Map.add l v state.vars, v, false

                    match stack with
                    | e :: s ->
                        if isSet then
                            Expr.Sequential(Expr.VarSet(var, e), toExpr s { state with vars = vars } rest)
                        else
                            Expr.Let(var, e, toExpr s { state with vars = vars } rest)
                    | [] ->
                        failwithf "cannot store %A (empty stack)" l
                            
                | Instruction.Ldloc l :: rest ->
                    match Map.tryFind l state.vars with
                    | Some var ->
                        toExpr (Expr.Var var :: stack) state rest
                    | None ->
                        failwithf "cannot load %A (not bound)" l

                | Instruction.LdConst c :: rest ->
                    let value = 
                        match c with
                        | Constant.Int8 v -> Expr.Value v
                        | Constant.Int16 v -> Expr.Value v
                        | Constant.Int32 v -> Expr.Value v
                        | Constant.Int64 v -> Expr.Value v
                        | Constant.UInt8 v -> Expr.Value v
                        | Constant.UInt16 v -> Expr.Value v
                        | Constant.UInt32 v -> Expr.Value v
                        | Constant.UInt64 v -> Expr.Value v
                        | Constant.Float32 v -> Expr.Value v
                        | Constant.Float64 v -> Expr.Value v
                        | Constant.NativeInt v -> Expr.Value v
                        | Constant.UNativeInt v -> Expr.Value v
                        | Constant.String v -> Expr.Value v

                    toExpr (value :: stack) state rest

                | Instruction.ConditionalJump(cond, target) :: rest ->
                    failwith ""

                | (Instruction.LdargA i | Instruction.Ldarg i) :: rest ->
                    match Map.tryFind i state.args with
                    | Some arg ->
                        toExpr (arg :: stack) state rest
                    | None ->
                        failwithf "cannot load %A (not bound)" i

                | (Instruction.Ldfld f | Instruction.LdfldA f) :: rest ->
                    match stack with
                    | a :: stack ->
                        toExpr (Expr.FieldGet(a, f) :: stack) state rest
                    | _ ->
                        failwithf "cannot load field %A (empty stack)" f
                        

                | Instruction.Add :: rest ->
                    match stack with 
                    | b :: a :: stack ->
                        let mi = (meth <@ (+) @>).GetGenericMethodDefinition().MakeGenericMethod [| a.Type; b.Type; b.Type |]
                        toExpr (Expr.Call(mi, [a;b]) :: stack) state rest
                    | _ ->
                        failwithf "cannot add (insufficient stack: %A)" stack

                | Instruction.And :: rest ->
                    match stack with 
                    | b :: a :: stack ->
                        let mi = (meth <@ (&&&) @>).GetGenericMethodDefinition().MakeGenericMethod [| a.Type |]
                        toExpr (Expr.Call(mi, [a;b]) :: stack) state rest
                    | _ ->
                        failwithf "cannot add (insufficient stack: %A)" stack

                | Instruction.Box t :: rest ->
                    match stack with
                    | a :: stack ->
                        toExpr (Expr.Coerce(a, t) :: stack) state rest
                    | [] ->
                        failwith "cannot box (empty stack)"
                        
                | i0 :: rest ->
                    failwithf "bad instruction: %A" i0

            let pars = mi.GetParameters() |> Array.mapi (fun i arg -> i, Var(arg.Name, arg.ParameterType)) |> Map.ofArray
            let args = pars |> Map.map (fun _ -> Expr.Var)

            let rec wrap (args : list<Var>) (b : Expr) =
                match args with
                | [] -> b
                | a :: rest -> Expr.Lambda(a, wrap rest b)

            let state = DisassemblerState.ofMethod mi def
            toExpr [] state def.Body |> wrap (Map.toList pars |> List.map snd)




    let compile (e : Expr<'a>) : 'a =
        let m = DynamicMethod(Guid.NewGuid().ToString(), MethodAttributes.Public ||| MethodAttributes.Static, CallingConventions.Standard, typeof<'a>, [||], Compiler.bMod, true)
        let il = m.GetILGenerator()
        Compiler.compileExpr { il = il; vars = Map.empty; currentException = None } e
        il.Emit(OpCodes.Ret)
        let f = m.CreateDelegate(typeof<Func<'a>>) |> unbox<Func<'a>>


        f.Invoke()

    let inline bla (a : ^a) =
        (^a : (static member Hans : ^a -> ^b) (a))

    type Blubber(v : int) =
        member x.Value = v
        static member Hans(b : Blubber) = b.Value * 10

    [<ReflectedDefinition>]
    let f (vec : Set<int>) =   
        let mutable a = bla (Blubber 10) 
        for i in vec do
            a <- a + i

        let mutable cnt = 0
        while a > 0 do
            a <- a / 2
            cnt <- cnt + 1

        cnt

    let meth (a : V2i) =
        a.X + a.Y

    let test() =
        let def = Compiler.disassemble (Compiler.meth <@ meth @>)
        printfn "%A" def
        exit 0

        let expr = Expr.TryGetReflectedDefinition (Compiler.meth <@ f @>) |> Option.get |> Expr.Cast<_ -> _>
        let test = compile expr
      

        let meth = test.GetType().GetMethod("Invoke")

        //for i in Aardvark.Base.IL.Disassembler.disassemble(meth).Body do
        //    printfn "%A" i

        //printfn "%A <-> %A" (test (V2i(4,4))) (f (V2i(4,4)))
        //printfn "%A <-> %A" (test (V2i(11,11))) (f (V2i(11,11)))


        let input = Set.ofList [1;2;3;47]

        try test input |> printfn "%d"
        with e -> printfn "%A" e

        try f input |> printfn "%d"
        with e -> printfn "%A" e
            
        let iter = 10000000
        for i in 1 .. 5 do
            let sw = System.Diagnostics.Stopwatch.StartNew()
            let mutable v = 0
            for i in 1 .. iter do
                v <- test input
            sw.Stop()
            printfn "compiled: %A" (sw.MicroTime / iter)
        
            let sw = System.Diagnostics.Stopwatch.StartNew()
            let mutable v = 0
            for i in 1 .. iter do
                v <- f input
            sw.Stop()
            printfn "native:   %A" (sw.MicroTime / iter)


    type Model =
        {
            a : int
            b : string
        }

    type aval<'a> = interface end
    type alist<'a> = interface end

    module AVal =
        let map (mapping : 'a -> 'b) (v : aval<'a>) : aval<'b> =
            failwith ""
        let map2 (mapping : 'a -> 'b -> 'c) (v : aval<'a>) (v1 : aval<'b>) : aval<'c> =
            failwith ""
        let bind (mapping : 'a -> aval<'b>) (v : aval<'a>) : aval<'b> =
            failwith ""

    type Dom = 
        static member Text (str : string) : Dom = failwith ""
        static member Text (str : aval<string>) : Dom = failwith ""
        
        static member Div (str : list<Dom>) : Dom = failwith ""
        static member Div (str : alist<Dom>) : Dom = failwith ""


        static member Adaptive (a : aval<#seq<Dom>>) : seq<Dom> = failwith ""
        static member Adaptive (a : aval<Dom>) : Dom = failwith ""

    type SequenceBuilder() =
      class
            member b.Zero() = Seq.empty
            member b.YieldFrom(x) = x
            member b.Yield(x) = Seq.singleton x
            member b.Combine(x,y) = Seq.append x y
            member b.Compose(p1,rest) = Seq.collect rest p1 
            member b.Using(rf,rest) = Microsoft.FSharp.Core.CompilerServices.RuntimeHelpers.EnumerateUsing rf rest
            member x.Delay f = Seq.delay f
      end
    let hans (a : aval<int>) (m : Model) =
        let seq = SequenceBuilder()
        seq.Delay (fun () ->
            seq.Combine (
                (
                    Dom.Adaptive (
                        a |> AVal.map (fun a ->
                            if a < 10 then
                                seq.Combine(
                                    seq.Yield(Dom.Text "not so much"),
                                    seq.Delay (fun () -> seq.Yield(Dom.Text "hans"))
                                )
                            else 
                                seq.Zero()
                        )
                    )
                ),
                seq.Combine(
                    seq.Yield(Dom.Text (sprintf "%d %s things" m.a m.b)),
                    seq.Delay (fun () -> 
                        seq.Yield (
                            Dom.Div [
                                Dom.Text (m.b + " hans")
                            ]
                        )
                    )
                )
            )
        )

    let view (m : Model) =
        Dom.Div [
            if m.a < 10 then
                Dom.Text "not so much"
                Dom.Text "hans"
            Dom.Text (sprintf "%d %s things" m.a m.b)
            Dom.Div [
                Dom.Text (m.b + " hans")
            ]
        ]

    let view' (a : aval<int>) (b : aval<string>) =
        Dom.Div [
            yield!
                Dom.Adaptive (
                    a |> AVal.map (fun a -> 
                        [
                            if a < 10 then
                                yield Dom.Text "not so much"
                                yield Dom.Text "hans"
                        ]
                    )
                )
            yield Dom.Text (b |> AVal.map (fun b -> (a |> AVal.map (fun a -> sprintf "%d %s things" a b))) |> AVal.bind id)
            yield Dom.Div [
                Dom.Text (b |> AVal.map (fun b -> b + " hans"))
            ]
        ]

module ComputeDeltaTests =
    open Armadillo
    open FSharp.Data.Adaptive
    
    let bla() =

        let vals = clist (List.map cval [1;2;3;4;2;7])

        let test = vals |> AList.mapA (fun c -> c :> aval<_>)
        let r = test.GetReader()
        
        let printDelta (name : string) (d : IndexListDelta<'a>) =
            let inline printDelta (i : Index, op : ElementOperation<'a>) =
                let stri = (sprintf "%A" i).TrimEnd('0')
                match op with
                | Remove -> sprintf "Remove(%s)" stri
                | Set v -> sprintf "Set(%s, %A)" stri v
            let str = d |> IndexListDelta.toSeq |> Seq.map printDelta |> String.concat "; " |> sprintf "%s [%s]" name
            Log.line "%s" str
            
        let printState (d : IndexList<'a>) =
            let str = d |> Seq.map (sprintf "%A") |> String.concat "; " |> sprintf "IndexList [%s]"
            Log.line "%s" str
            
        let print () =
            let ops = r.GetChanges(AdaptiveToken.Top)
            printDelta "Output" ops
            printState r.State

        let setContent (content : list<int>) =
            transact (fun () ->
                let diffing =   
                    ListDiffing.custom
                        (fun (c : cval<int>) (v : int) -> c.Value = v)
                        (fun v -> Log.line "new %A" v; cval v)
                        (fun _ -> ())
                        (fun (c : cval<int>) (v : int) -> Log.line "%A <- %A" c.Value v; c.Value <- v; UpdateStatus.Nop)

                let struct (_, delta) = 
                    diffing.Update(vals.Value, content)

                printDelta "Input" delta

                for i, op in IndexListDelta.toSeq delta do
                    match op with
                    | Set v ->
                        vals.[i] <- v
                    | Remove ->
                        vals.Remove i |> ignore
            )



        Log.start "initial"
        print()
        Log.stop()

        Log.start "update [1;44;3]"
        setContent [1;44;3]
        print()
        Log.stop()

        
        Log.start "update [1;2;3;100]"
        setContent [1;2;3;100]
        print()
        Log.stop()


    let star() =
        let rand = RandomSystem()
        let a = List.init 100 id
        let b = List.take 40 a @ [2000;3000;4000] @ List.skip 40 a

        let l = a |> IndexList.ofList

        let delta = IndexList.computeDeltaToList l b
        printfn "%A" delta
        printfn "%d" delta.Count

        let (n,_) = IndexList.applyDelta l delta 
        printfn "%A" (IndexList.toList n)

    let benchmark() =
        let csv = System.Text.StringBuilder()

        let types =
            [
                "single replaced element", fun (l : list<int>) ->
                    let remIndex = List.length l / 2
                    List.take remIndex l @ [10000] @ List.skip (remIndex + 1) l

                "single insertion", fun (l : list<int>) ->
                    let remIndex = List.length l / 2
                    List.take remIndex l @ [10000] @ List.skip remIndex l
                    
                "single delete", fun (l : list<int>) ->
                    let remIndex = List.length l / 2
                    List.take remIndex l @ List.skip (remIndex + 1) l

            ]

        
        csv.AppendLine (("size" :: List.map fst types) |> String.concat ";") |> ignore

        let rand = RandomSystem()
        for size in [50 .. 50 .. 5000] do

            let times =
                types |> List.map (fun (name, modify) ->

                    Log.start "%s %d" name size
                    let l = List.init size (fun _ -> rand.UniformInt 1000)
                    let a = l |> IndexList.ofList

                    let b = modify l
                    let bCnt = List.length b
                        
                    let sw = System.Diagnostics.Stopwatch.StartNew()
                    let mutable iter = 0
                    
                    while sw.Elapsed.TotalMilliseconds < 200.0 do
                        IndexList.ComputeDelta(a, b, bCnt)
                        //IndexListExtensions.computeDelta Unchecked.equals a b
                        |> ignore
                        iter <- iter + 1
                    sw.Stop()

                    let iter = 
                        Fun.NextPowerOfTwo (max 32 (iter * 5))
                    for i in 1 .. 4 do
                        IndexList.ComputeDelta(a, b, bCnt)
                        //IndexListExtensions.computeDelta Unchecked.equals a b
                        |> ignore
                
                    GC.Collect(3)
                    GC.WaitForFullGCComplete() |> ignore

                    let sw = System.Diagnostics.Stopwatch.StartNew()
                    for i in 0 .. iter-1 do
                        IndexList.ComputeDelta(a, b, bCnt)
                        //IndexListExtensions.computeDelta Unchecked.equals a b
                        |> ignore
                    sw.Stop()
          
                    Report.End(sprintf " %A" (sw.MicroTime / iter)) |> ignore
                    sw.Elapsed.TotalMilliseconds / float iter
                )

            let t = times |> Seq.map (fun v -> sprintf "%.8f" v) |> String.concat ";"
            csv.AppendLine(sprintf "%d;%s" size t) |> ignore

        let desktop = Environment.GetFolderPath Environment.SpecialFolder.Desktop
        File.writeAllText (Path.combine [desktop; "diff.csv"]) (csv.ToString())


    let validate() =  

        let printError (i : int) (a : list<int>) (b : list<int>) =
            let list = IndexList.ofList a

            Log.start "error %d" i

            let chunkSize = 8
            Log.start "left"
            for chunk in IndexList.toSeqIndexed list |> Seq.chunkBySize chunkSize do
                let str = chunk |> Seq.map (fun (i,a) -> sprintf "%A: %A" i a) |> String.concat "; "
                Log.line "%s" str
            Log.stop()

            Log.start "right"
            for chunk in Seq.chunkBySize chunkSize b do
                let str = chunk |> Seq.map (fun a -> sprintf "%A" a) |> String.concat "; "
                Log.line "%s" str
            Log.stop()

            let delta = IndexList.computeDeltaToList list  b
            Log.start "delta"
            for chunk in Seq.chunkBySize chunkSize (IndexListDelta.toSeq delta) do
                let str = chunk |> Seq.map (fun (i,a) -> sprintf "%A: %A" i a) |> String.concat "; "
                Log.line "%s" str
            Log.stop()

            let (res, _) = IndexList.applyDelta list delta
            Log.start "result"
            for chunk in Seq.chunkBySize chunkSize (IndexList.toSeq res) do
                let str = chunk |> Seq.map (fun a -> sprintf "%A" a) |> String.concat "; "
                Log.line "%s" str
            Log.stop()

            Log.stop()

        let iter = 5000
        Log.startTimed "computeDelta"
        let rand = RandomSystem()
        let mutable failed = []
        let mutable failedCount = 0
        let mutable passed = 0
        for i in 1 .. iter do
            let a = List.init (rand.UniformInt 100) (fun _ -> rand.UniformInt 1000)
            let b = List.init (rand.UniformInt 100) (fun _ -> rand.UniformInt 1000)

            let list = IndexList.ofList a
            let delta = IndexList.computeDeltaToList list b
            let (nl, _) = IndexList.applyDelta list delta

            let n = IndexList.toList nl
            if b = n then
                passed <- passed + 1
            else
                printError failedCount a b
                failed <- (a,b) :: failed
                failedCount <- failedCount + 1
            Report.Progress(float i / float iter)

        Report.Progress 1.0
        if failedCount > 0 then
            Log.line "%d tests failed" failedCount
        Log.line "%d tests passed" passed
        Log.stop()
        


open FSharp.Data.Adaptive

module SgTest = 
    open Armadillo

    let run() =
        Aardvark.Init()
 
        let glfw = Glfw.GetApi()
        glfw.Init() |> ignore
        glfw.WindowHint(WindowHintClientApi.ClientApi, ClientApi.NoApi)
        //glfw.WindowHint(WindowHintBool.Visible, false)
        let win = glfw.CreateWindow(640, 480, "Yeah", NativePtr.ofNativeInt 0n, NativePtr.ofNativeInt 0n)

        let instance = Instance()
        //instance.EnableBackendValidation true
        //instance.EnableGPUBasedBackendValidation true
        //instance.EnableBeginCaptureOnStartup true

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
                            PresentMode = PresentMode.Mailbox
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
        
            glfw.PollEvents()
            let swapChainFormat = binding.GetPreferredSwapChainTextureFormat()

            let updateState : UpdateState =
                {
                    outputFormats = [|swapChainFormat|] 
                    outputs = Map.ofList ["Colors", (typeof<V4d>, 0)]
                    device = dev
                    caching = Dict()
                }

            let mutable a = Data.Create [| V3f.OOO; V3f.IOO; V3f.OIO |]
            let mutable b = Data.Create [| V3f.OOO; -V3f.IOO; -V3f.OIO |]

            let mutable quad = true

            let cnt = cval 3
            let draw =
                cnt |> AVal.map (fun cnt -> 
                    {
                        indexed = false
                        mode = PrimitiveTopology.TriangleList
                        instanceCount = 1
                        count = cnt
                        first = 0
                        firstInstance = 0
                        baseVertex = 0
                    } 
                )
            let mutable dirty = true

            dev.SetUncapturedErrorCallback (ErrorCallback(fun typ str _ ->
                Log.warn "%s" str
            ))


            

            let runner = Reconciler()
            let p = new RenderProgram(dev, false)
            let r = ReconcilerNode<_>(runner, 0, SgTest.testSg C4b.Red draw quad [] a, TraversalState.empty updateState)
            r.Fragment <- p.Root
            runner.RunUntilEmpty()
            

            let rand = RandomSystem()
            let ndc = Box2d(-V2d.II, V2d.II)
            let triSize = Box2d(-V2d.II * 0.05, V2d.II * 0.05)
            let newTriangle() =
                let p0 = rand.UniformV2d(ndc)
                let p1 = p0 + rand.UniformV2d triSize
                let p2 = p0 + rand.UniformV2d triSize
                Triangle2d(p0, p1, p2)
                



            let mutable all = List.init 50000 (fun _ -> newTriangle())
            let mutable elemCount = 1
            let mutable elems = List.truncate elemCount all
            let rand = RandomSystem()
            glfw.SetKeyCallback(win, GlfwCallbacks.KeyCallback(fun _ k _ ac _ ->
                match ac with
                | InputAction.Press | InputAction.Repeat -> 
                    match k with
                    | Keys.W ->
                        elemCount <- elemCount + 1
                        Log.line "%d" elemCount
                        elems <- List.truncate elemCount all 
                    | Keys.S ->
                        elemCount <- max 0 (elemCount - 1)
                        Log.line "%d" elemCount
                        elems <- List.truncate elemCount all 
                    | _ ->
                        ()
                | _ ->
                    ()
                //match ac with
                //| InputAction.Press | InputAction.Repeat -> 
                //    match k with
                //    | Keys.Space ->
                //        runner.Update(r, SgTest.testSg C4b.Red draw quad 10 b)
                //        Fun.Swap(&a, &b)
                //        dirty <- true
                //        glfw.PostEmptyEvent ()
                //        ()
                //    | Keys.Enter ->
                //        quad <- not quad
                //        runner.Update(r, SgTest.testSg C4b.Red draw quad 10 a)
                //        dirty <- true
                //        glfw.PostEmptyEvent ()
                //        ()
                //    | Keys.X ->
                //        transact (fun () -> cnt.Value <- 3 - cnt.Value)
                //        runner.RunUntilEmpty()
                //        dirty <- true
                //        glfw.PostEmptyEvent ()
                //        () 
                //    | Keys.Y ->
                //        let color = rand.UniformC3f().ToC4b()
                //        runner.Update(r, SgTest.testSg color draw quad 10 a)
                //        dirty <- true
                //        glfw.PostEmptyEvent ()
                //        ()
                //    | _ ->
                //        ()
                //| _ ->
                //    ()
            )) |> ignore


            let sw = System.Diagnostics.Stopwatch()
            let mutable frames = 0


            let mutable h = 0.0


            let render() =  
                let mutable s = V2i.II
                glfw.GetFramebufferSize(win, &s.X, &s.Y)
                if false && s <> size then
                    size <- s
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
                                    ClearColor = { R = 0.5; G = 0.5; B = 0.5; A = 1.0 }
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
                    
                h <- (h + 0.005) % 1.0
                let color = HSVf(float32 h, 0.5f, 1.0f).ToC3f().ToC4b()
                let newSg = SgTest.testSg color draw quad elems a
                runner.Update(r, newSg)
                dirty <- true

                p.Run(pass)


                pass.EndPass()
           
           
                use buf = cmd.Finish()
                queue.Submit [| buf |]

                chain.Present()
                frames <- frames + 1
                if frames > 100 then
                    sw.Stop()
                    let dt = sw.Elapsed.TotalSeconds
                    if dt > 0.0 then
                        Log.line "%.2ffps" (float frames / dt)
                    frames <- 0
                    sw.Restart()

            glfw.SetWindowRefreshCallback(win, GlfwCallbacks.WindowRefreshCallback (fun w -> dirty <- true)) |> ignore
            glfw.PostEmptyEvent()
            while not (glfw.WindowShouldClose win) do
                render()
                glfw.PollEvents()


let rec listHash (a : int) (l : list<'a>) =
    match l with
    | h :: t ->
        listHash (HashCode.Combine(a, Unchecked.hash h)) t
    | [] -> 
        a

[<EntryPoint; STAThread>]
let main argv = 

    ////FSharp.Data.Adaptive.DefaultEqualityComparer.SetProvider {
    ////    new IEqualityProvider with
    ////        member x.GetEqualityComparer<'a>() =   
    ////            if typeof<'a>.IsGenericType && typeof<'a>.GetGenericTypeDefinition() = typedefof<list<_>> then

    ////                failwith ""
    ////            else
    ////                DefaultEqualityComparer<'a>.Instance
    ////}


    //let a = List.init 10_000_000 id

    //for i in 1 .. 100 do
    //    let sw = System.Diagnostics.Stopwatch.StartNew()
    //    let hash = listHash 0 a
    //    sw.Stop()
    //    printfn "%A (%A)" hash sw.MicroTime

    //exit 0
    
    SgTest.run()
    exit 0

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

    let idx = 1

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
        
        for e in a.Extensions do
            printfn "%s" e

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
                                ClearColor = { R = 0.5; G = 0.5; B = 0.5; A = 1.0 }
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
