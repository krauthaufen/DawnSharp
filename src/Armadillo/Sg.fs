namespace Armadillo

open System
open FSharp.Data.Adaptive
open Aardvark.Base
open WebGPU
open Armadillo
open System.Runtime.InteropServices
open Microsoft.FSharp.NativeInterop
open System.Threading


type SgAttributes =
    {
        mutable viewTrafo           : option<Trafo3d>
        mutable projTrafo           : option<Trafo3d>
        mutable modelTrafo          : option<Trafo3d>
        mutable shader              : option<FShade.Effect>
        mutable blendMode           : option<BlendMode>
        mutable depthTestMode       : option<DepthTestMode>
        mutable stencilMode         : option<StencilMode>
        mutable indirectBuffer      : option<BufferDescriptor>
        mutable indexBuffer         : option<BufferDescriptor>
        mutable vertexAttributes    : HashMap<string, BufferDescriptor>
        mutable instanceAttributes  : HashMap<string, BufferDescriptor>
        mutable uniforms            : HashMap<string, IAdaptiveValue>
    }
    
module SgAttributes =
    let empty() =
        {
            viewTrafo = None
            projTrafo = None
            modelTrafo = None
            shader = None
            blendMode = None
            depthTestMode = None
            stencilMode = None
            indirectBuffer = None
            indexBuffer = None
            vertexAttributes = HashMap.empty
            instanceAttributes = HashMap.empty
            uniforms = HashMap.empty
        }

    let combine (l : SgAttributes) (r : SgAttributes) =
        {
            viewTrafo = match r.viewTrafo with | Some _ -> r.viewTrafo | None -> l.viewTrafo
            projTrafo = match r.projTrafo with | Some _ -> r.projTrafo | None -> l.projTrafo
            modelTrafo =    
                match r.modelTrafo with 
                | Some rm -> 
                    match l.modelTrafo with 
                    | Some lm -> Some (lm * rm) 
                    | None -> r.modelTrafo 
                | None -> 
                    l.modelTrafo
            shader = match r.shader with | Some _ -> r.shader | None -> l.shader
            blendMode = match r.blendMode with | Some _ -> r.blendMode | None -> l.blendMode
            depthTestMode = match r.depthTestMode with | Some _ -> r.depthTestMode | None -> l.depthTestMode
            stencilMode = match r.stencilMode with | Some _ -> r.stencilMode | None -> l.stencilMode
            indirectBuffer = match r.indirectBuffer with | Some _ -> r.indirectBuffer | None -> l.indirectBuffer
            indexBuffer = match r.indexBuffer with | Some _ -> r.indexBuffer | None -> l.indexBuffer
            vertexAttributes = HashMap.union l.vertexAttributes r.vertexAttributes
            instanceAttributes = HashMap.union l.instanceAttributes r.instanceAttributes
            uniforms = HashMap.union l.uniforms r.uniforms
        }

module SgAttribute =
    [<Struct>]
    type Trafo(value : Trafo3d) =
        member x.Value = value

    [<Struct>]
    type VertexAttribute(name : string, value : BufferDescriptor) =
        member x.Name = name
        member x.Data = value
        
    [<Struct>]
    type InstanceAttribute(name : string, value : BufferDescriptor) =
        member x.Name = name
        member x.Data = value
        
    [<Struct>]
    type VertexAttributes(values : HashMap<string, BufferDescriptor>) =
        member x.Values = values
        
    [<Struct>]
    type InstanceAttributes(values : HashMap<string, BufferDescriptor>) =
        member x.Values = values

    [<Struct>]
    type Uniform(name : string, value : IAdaptiveValue) =
        member x.Name = name
        member x.Value = value
        
    [<Struct>]
    type Shader(effects : list<FShade.Effect>) =
        member x.Effects = effects

    open Microsoft.FSharp.Quotations
    type ShaderBuilder() =
        member inline x.Bind(shader : 'a -> Expr<'b>, cont : unit -> list<FShade.Effect>) =
            FShade.Effect.ofFunction shader :: cont()
            
        member inline x.Return(()) : list<FShade.Effect> =
            []
        member inline x.Zero() : list<FShade.Effect> =
            []

        member inline x.Delay(action : unit -> list<FShade.Effect>) =
            action

        member x.Combine(l : list<FShade.Effect>, r : unit -> list<FShade.Effect>) =
            l @ r()

        member inline x.Run(action : unit -> list<FShade.Effect>) =
            action() |> Shader


type UpdateState =
    {
        outputFormats   : TextureFormat[]
        outputs         : Map<string, (Type * int)>
        device          : Device
        caching         : Dict<obj, obj>
    }

    member x.GetOrCreate(key : 'a, creator : 'a -> 'b) =
        let key = struct(typeof<'a>, typeof<'b>, key)

        //try Unchecked.hash key |> ignore
        //with _ -> Log.warn "%A" key

        x.caching.GetOrCreate(key :> obj, fun o -> 
            let struct(_, _, v) = 
                try unbox<struct(Type * Type * 'a)> o
                with _ ->
                    Log.warn "%A %A %A %A" o typeof<'a> typeof<'b> key
                    reraise()

            creator v :> obj
        ) |> unbox<'b>
        
    member x.GetOrCreate(a : 'a, b : 'b, creator : 'a -> 'b -> 'c) =
        x.GetOrCreate(struct(a,b), fun struct(a,b) -> creator a b)
        
    member x.GetOrCreate(a : 'a, b : 'b, c : 'c, creator : 'a -> 'b -> 'c -> 'd) =
        x.GetOrCreate(struct(a,b,c), fun struct(a,b,c) -> creator a b c)
        
    member x.GetOrCreate(a : 'a, b : 'b, c : 'c, d : 'd, creator : 'a -> 'b -> 'c -> 'd -> 'e) =
        x.GetOrCreate(struct(a,b,c,d), fun struct(a,b,c,d) -> creator a b c d)
        
    member x.GetOrCreate(a : 'a, b : 'b, c : 'c, d : 'd, e : 'e, creator : 'a -> 'b -> 'c -> 'd -> 'e -> 'f) =
        x.GetOrCreate(struct(a,b,c,d,e), fun struct(a,b,c,d,e) -> creator a b c d e)


[<RequireQualifiedAccess>]
type BindingValue =
    | UniformBuffer of block : FShade.GLSL.GLSLUniformBuffer
    | Texture of samplerType : FShade.GLSL.GLSLSamplerType * textures : list<string * FShade.SamplerState>
    | None
    // TODO: storage, etc.

type BindingInfo =
    {
        entry : BindGroupLayoutEntry
        value : BindingValue
    }
    
[<AbstractClass>]
type ResourcePromise() =
    static let living = System.Collections.Concurrent.ConcurrentDictionary<string, ref<int>>()

    static let printer =
        startThread <| fun () ->
            let mutable last = HashMap.empty
            while true do
                Thread.Sleep 1000

                let all = 
                    living 
                    |> Seq.map (fun (KeyValue(typ, cnt)) -> typ, !cnt)
                    |> Seq.filter (fun (_,b) -> b <> 0)
                    |> HashMap.ofSeq

                if all <> last then
                    last <- all
                    Log.start "living"
                    for (typ, c) in all do
                        if c <> 0 then
                            Log.line "%s: %d" typ c
                    Log.stop()
        
    member x.HandleCreated() =
        Interlocked.Increment(&living.GetOrAdd(x.Name, fun _ -> ref 0).contents) |> ignore
        
    member x.HandleDestroyed() =
        Interlocked.Decrement(&living.GetOrAdd(x.Name, fun _ -> ref 0).contents) |> ignore


    abstract member Release : unit -> unit

    abstract member Name : string

[<AbstractClass>]
type ResourcePromise<'b>() =
    inherit ResourcePromise()
    abstract member Acquire : unit -> 'b

[<AbstractClass>]
type ResourcePromise<'s, 'a, 'b when 's :> ResourcePromise<'s, 'a, 'b>>(state : UpdateState, value : 'a) =
    inherit ResourcePromise<'b>()

    
    static let name=
        let n = typeof<'s>.Name
        if n.EndsWith "Promise" then n.Substring(0, n.Length - 7)
        else n


    let mutable refCount = 0
    let mutable cache = Unchecked.defaultof<'b>

    abstract member Create : unit -> 'b
    abstract member Destroy : 'b -> unit
    abstract member Recreate : 'a -> 's

    override x.Name = name

    member x.Update(newValue : 'a) =
        if Unchecked.equals value newValue then
            x :?> 's
        else
            state.GetOrCreate(newValue, fun newValue ->
                x.Recreate(newValue)
            )

    override x.Acquire() =
        if Interlocked.Increment(&refCount) = 1 then
            Log.line "create %s" name
            let h = x.Create()
            x.HandleCreated()
            cache <- h
            h
        else
            cache

    override x.Release() =
        if Interlocked.Decrement(&refCount) = 0 then
            Log.line "destroy %s" name
            x.Destroy cache
            x.HandleDestroyed()
            cache <- Unchecked.defaultof<_>

    interface IDisposable with
        member x.Dispose() = x.Release()

type BufferPromise(state : UpdateState, usage : BufferUsage, data : BufferDescriptor) =
    inherit ResourcePromise<BufferPromise, struct (BufferUsage * BufferDescriptor), struct(Buffer * uint64 * uint64)>(state, struct(usage, data))

    member x.Data = data

    override x.Recreate(struct(newUsage : BufferUsage, newData : BufferDescriptor)) =
        new BufferPromise(state, newUsage, newData)

    override x.Create () =
        //printfn "create buffer"
        match data with
        | BufferDescriptor.Data data ->           
            let buffer = 
                state.device.CreateBuffer {
                    Label = null
                    Usage = usage
                    Size = uint64 data.Size
                    MappedAtCreation = false
                }
            buffer.Upload(data)
            struct (buffer, 0UL, uint64 data.Size)
        | BufferDescriptor.Buffer(fmt, cnt, buf, off, size) ->
            struct(buf.Clone(), uint64 off, uint64 size)

    override x.Destroy((buffer : Buffer, _, _)) =
        //printfn "destroy buffer"
        buffer.Dispose()

type ShaderPromise(state : UpdateState, effects : list<FShade.Effect>) =
    inherit ResourcePromise<ShaderPromise, list<FShade.Effect>, struct(ShaderModule * ShaderModule)>(state, effects)

    let effect =
        effects
        |> FShade.Effect.compose

    let glsl = 
        let cfg : FShade.EffectConfig =
            {
                FShade.EffectConfig.depthRange = Range1d(0.0, 1.0)
                FShade.EffectConfig.flipHandedness = false
                FShade.EffectConfig.lastStage = FShade.ShaderStage.Fragment
                FShade.EffectConfig.outputs = state.outputs
            }

        let backend = 
            FShade.GLSL.Backend.Create {
                FShade.GLSL.Config.bindingMode = FShade.GLSL.BindingMode.Global
                FShade.GLSL.Config.createDescriptorSets = true
                FShade.GLSL.Config.createInputLocations = true
                FShade.GLSL.Config.createOutputLocations = true
                FShade.GLSL.Config.createPassingLocations = true
                FShade.GLSL.Config.createPerStageUniforms = false
                FShade.GLSL.Config.createUniformBuffers = true
                FShade.GLSL.Config.depthWriteMode = true
                FShade.GLSL.Config.reverseMatrixLogic = true
                FShade.GLSL.Config.version = FShade.GLSL.GLSLVersion(4,5,0)
                FShade.GLSL.Config.enabledExtensions = Set.empty
                FShade.GLSL.Config.stepDescriptorSets = true
                FShade.GLSL.Config.useInOut = true

            }
        effect
        |> FShade.Effect.toModule cfg
        |> FShade.Imperative.ModuleCompiler.compile backend
        |> FShade.GLSL.Assembler.assemble backend

    do Aardvark.Base.Log.line "%s" glsl.code

    let bindGroups =
        let mutable groups : MapExt<int, MapExt<int, BindingInfo>> = MapExt.empty

        // TODO: readwrite images / storage buffers

        for (KeyValue(_name, block)) in glsl.iface.uniformBuffers do
            let entry : BindGroupLayoutEntry =
                {
                    Binding = block.ubBinding
                    Visibility = ShaderStage.Vertex ||| ShaderStage.Fragment
                    Type = BindingType.UniformBuffer
                    HasDynamicOffset = false
                    MinBufferBindingSize = uint64 block.ubSize
                    Multisampled = false
                    ViewDimension = TextureViewDimension.Undefined
                    TextureComponentType = TextureComponentType.Float
                    StorageTextureFormat = TextureFormat.Undefined
                }

            let info =
                {
                    entry = entry
                    value = BindingValue.UniformBuffer block
                }

            groups <-
                groups |> MapExt.alter block.ubSet (function
                    | Some o -> MapExt.add block.ubBinding info o |> Some
                    | None -> MapExt.singleton block.ubBinding info |> Some
                )

        for (KeyValue(_name, sammy)) in glsl.iface.samplers do
            let arr = sammy.samplerType.isArray
            let dim =
                match sammy.samplerType.dimension with
                | FShade.SamplerDimension.Sampler1d ->  
                    if arr then failwith ""
                    else TextureViewDimension.D1D
                | FShade.SamplerDimension.Sampler2d -> 
                    if arr then TextureViewDimension.D2DArray
                    else TextureViewDimension.D2D
                | FShade.SamplerDimension.Sampler3d -> 
                    if arr then failwith ""
                    else TextureViewDimension.D3D
                | FShade.SamplerDimension.SamplerCube -> 
                    if arr then TextureViewDimension.CubeArray
                    else TextureViewDimension.Cube
                | _ ->
                    failwith ""

            let rec resultType (t : FShade.GLSL.GLSLType) = 
                match t with
                | FShade.GLSL.Int(true,_) -> TextureComponentType.Sint
                | FShade.GLSL.Int(false,_) -> TextureComponentType.Uint
                | FShade.GLSL.Float _ -> TextureComponentType.Float

                | FShade.GLSL.Vec(_, t) -> resultType t
                | FShade.GLSL.Mat(_, _, t) -> resultType t

                | _ -> failwith ""

            let entry : BindGroupLayoutEntry =
                {
                    Binding = sammy.samplerBinding
                    Visibility = ShaderStage.Vertex ||| ShaderStage.Fragment
                    Type = BindingType.SampledTexture
                    HasDynamicOffset = false
                    MinBufferBindingSize = 0UL
                    Multisampled = sammy.samplerType.isMS
                    ViewDimension = dim
                    TextureComponentType = resultType sammy.samplerType.valueType
                    StorageTextureFormat = TextureFormat.Undefined
                }
                
            let info =
                {
                    entry = entry
                    value = BindingValue.Texture(sammy.samplerType, sammy.samplerTextures)
                }

            groups <-
                groups |> MapExt.alter sammy.samplerSet (function
                    | Some o -> MapExt.add sammy.samplerBinding info o |> Some
                    | None -> MapExt.singleton sammy.samplerBinding info |> Some
                )

        let setCount = 
            match MapExt.tryMax groups with
            | Some v -> v + 1
            | None -> 0

        Array.init setCount (fun si ->
            match MapExt.tryFind si groups with
            | Some bindings ->
                let bindingCount =
                    match MapExt.tryMax bindings with
                    | Some max -> 1 + max
                    | None -> 0

                Array.init bindingCount (fun bi ->
                    match MapExt.tryFind bi bindings with
                    | Some b -> b
                    | None -> 
                        { 
                            entry = BindGroupLayoutEntry.Default(bi, ShaderStage.None, BindingType.SampledTexture)
                            value = BindingValue.None
                        }
                )

            | None ->
                [||]
        )

    member x.Effect = effect

    member x.BindGroups = bindGroups

    member x.Interface = glsl.iface

    override x.Recreate(newEffects : list<FShade.Effect>) =
        new ShaderPromise(state, newEffects)

    override x.Create () =
        //printfn "create shader"
        let vs = 
            state.device.CreateGLSLShaderModule {
                Label = null
                Code = glsl.code
                Defines = ["Vertex"]
                EntryPoint = "main"
                ShaderStage = ShaderStage.Vertex
            }

        let fs = 
            state.device.CreateGLSLShaderModule {
                Label = null
                Code = glsl.code
                Defines = ["Fragment"]
                EntryPoint = "main"
                ShaderStage = ShaderStage.Fragment
            }
        struct (vs, fs)

    override x.Destroy (struct(vs, fs)) =
        //printfn "destroy shader"
        vs.Dispose()
        fs.Dispose()

type BindGroupLayoutPromise(state : UpdateState, entries : list<BindGroupLayoutEntry>) =
    inherit ResourcePromise<BindGroupLayoutPromise, list<BindGroupLayoutEntry>, BindGroupLayout>(state, entries)
    
    override x.Recreate(newEntries : list<BindGroupLayoutEntry>) =
        new BindGroupLayoutPromise(state, newEntries)

    override x.Create() =
        state.device.CreateBindGroupLayout {
            Label = null
            Entries = List.toArray entries
        }

    override x.Destroy(b : BindGroupLayout) =
        b.Dispose()
        
type PipelineLayoutPromise(state : UpdateState, groups : list<BindGroupLayoutPromise>) =
    inherit ResourcePromise<PipelineLayoutPromise, list<BindGroupLayoutPromise>, PipelineLayout>(state, groups)

    override x.Recreate(newGroups : list<BindGroupLayoutPromise>) =
        new PipelineLayoutPromise(state, newGroups)

    override x.Create() =
        let g = groups |> Array.ofList |> Array.map (fun g -> g.Acquire())
        state.device.CreatePipelineLayout {
            Label = null
            BindGroupLayouts = g
        }

    override x.Destroy(p : PipelineLayout) =
        groups |> List.iter (fun g -> g.Release())
        p.Dispose()

type PipelinePromise(state : UpdateState, top : PrimitiveTopology, layout : PipelineLayoutPromise, shader : ShaderPromise, vertexBuffers : HashMap<string, VertexFormat * int>, instanceBuffers : HashMap<string, VertexFormat * int>) =
    inherit ResourcePromise<PipelinePromise, struct(PrimitiveTopology * PipelineLayoutPromise * ShaderPromise * HashMap<string, VertexFormat * int> * HashMap<string, VertexFormat * int>), RenderPipeline>(
        state,
        struct(top, layout, shader, vertexBuffers, instanceBuffers)
    )
    
    member x.Interface = shader.Interface

    member x.WithPrimitiveTopology(newTop : PrimitiveTopology) =
        x.Update(struct(newTop, layout, shader, vertexBuffers, instanceBuffers))

    override x.Recreate(struct(newTop : PrimitiveTopology, newLayout : PipelineLayoutPromise, newShader : ShaderPromise, newVertexBuffers : HashMap<string, VertexFormat * int>, newInstanceBuffers : HashMap<string, VertexFormat * int>)) =
        new PipelinePromise(state, newTop, newLayout, newShader, newVertexBuffers, newInstanceBuffers)

    override x.Create() =
        let layout = layout.Acquire()
        let struct (vs, fs) = shader.Acquire()


        let vbs = 
            shader.Interface.inputs |> List.toArray |> Array.map (fun par ->
                match HashMap.tryFind par.paramSemantic vertexBuffers with
                | Some (format, cnt) ->
                    let elemSize = VertexFormat.size format
                    {
                        ArrayStride = uint64 elemSize * uint64 cnt
                        StepMode = InputStepMode.Vertex
                        Attributes =
                            Array.init cnt (fun i ->
                                // TODO: non-primitive type (M44f, etc.)
                                { 
                                    Format = format
                                    Offset = uint64 elemSize * uint64 i
                                    ShaderLocation = par.paramLocation + i
                                }
                            )
                    }

                | None ->
                    match HashMap.tryFind par.paramSemantic instanceBuffers with
                    | Some (format, cnt) ->
                        let elemSize = VertexFormat.size format

                        {
                            ArrayStride = uint64 elemSize * uint64 cnt
                            StepMode = InputStepMode.Instance
                            Attributes =
                                Array.init cnt (fun i ->
                                    // TODO: non-primitive type (M44f, etc.)
                                    { 
                                        Format = format
                                        Offset = uint64 elemSize * uint64 i
                                        ShaderLocation = par.paramLocation  + i
                                    }
                                )
                        }
                    | None ->
                        // TODO how to not bind attributes
                        {
                            ArrayStride = 0UL
                            StepMode = InputStepMode.Instance
                            Attributes = 
                                Array.init 1 (fun i ->
                                    { 
                                        Format = VertexFormat.Float4
                                        Offset = 0UL
                                        ShaderLocation = par.paramLocation + i
                                    }
                                )
                        }
            )

        let colors =  
            state.outputFormats |> Array.map (fun fmt ->
                { 
                    Format = fmt
                    AlphaBlend = BlendDescriptor.Default
                    ColorBlend = BlendDescriptor.Default
                    WriteMask = ColorWriteMask.All
                }
            )

        let pipeline =
            state.device.CreateRenderPipeline {
                Label = null

                SampleCount = 1
                SampleMask = 1
                AlphaToCoverageEnabled = false

                Layout = 
                    layout

                VertexStage = 
                    {
                        Module = vs
                        EntryPoint = "main"
                    }

                FragmentStage = 
                    Some {
                        Module = fs
                        EntryPoint = "main"
                    }

                VertexState =
                    Some {
                        IndexFormat = IndexFormat.Uint32
                        VertexBuffers = vbs
                    }

                PrimitiveTopology = PrimitiveTopology.TriangleList


                ColorStates = 
                    colors

                RasterizationState =    
                    Some {
                        FrontFace = FrontFace.CCW
                        CullMode = CullMode.None
                        DepthBias = 0
                        DepthBiasClamp = 0.0f
                        DepthBiasSlopeScale = 0.0f
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
            }
                
        pipeline

    override x.Destroy(pipeline : RenderPipeline) =
        pipeline.Dispose()
        layout.Release()
        shader.Release()
        ()

module TypeVisitors = 
    open Aardvark.Base.IL

    type ITypeVisitor2<'r> =
        abstract member Visit<'a, 'b when 'b : unmanaged> : unit -> 'r
        
    type ITypeVisitor<'r> =
        abstract member Visit<'a> : unit -> 'r

    type TypeVisitor2<'r>() =
        static let dict = System.Collections.Concurrent.ConcurrentDictionary<Type * Type, ITypeVisitor2<'r> -> 'r>()

        static let getVisitor (src : Type) (dst : Type) =
            dict.GetOrAdd((src, dst), fun (src, dst) ->
                let m = typeof<ITypeVisitor2<'r>>.GetMethod "Visit"
                let mi = m.MakeGenericMethod [| src; dst |]

                cil {
                    do! IL.ldarg 0
                    do! IL.call mi
                    do! IL.ret
                }
            )

        static member Visit(src, dst, v) =
            getVisitor src dst v
            
    type TypeVisitor<'r>() =
        static let dict = System.Collections.Concurrent.ConcurrentDictionary<Type, ITypeVisitor<'r> -> 'r>()

        static let getVisitor (dst : Type) =
            dict.GetOrAdd((dst), fun (dst) ->
                let m = typeof<ITypeVisitor<'r>>.GetMethod "Visit"
                let mi = m.MakeGenericMethod [| dst |]

                cil {
                    do! IL.ldarg 0
                    do! IL.call mi
                    do! IL.ret
                }
            )

        static member Visit(src, v) =
            getVisitor src v

    let inline visit (srcType : Type) (v : ITypeVisitor<'r>) =
        TypeVisitor.Visit(srcType, v)

    let inline visit2 (srcType : Type) (dstType : Type) (v : ITypeVisitor2<'r>) =
        TypeVisitor2.Visit(srcType, dstType, v)

type UniformBufferSlot(parent : UniformBufferManager, buffer : Buffer, ptr : nativeint, bufferIndex : int, offset : uint64) =
    member x.Buffer = buffer
    member x.Pointer = ptr
    member x.Offset = offset

and UniformBufferManager(device : Device, block : FShade.GLSL.GLSLUniformBuffer) =
    static let usage = BufferUsage.Uniform ||| BufferUsage.CopySrc ||| BufferUsage.CopyDst
    
    let mem =
        {
            MemoryManagement.malloc = fun s -> 
                device.CreateBuffer {
                    Label = null
                    Usage = usage
                    Size = uint64 s
                    MappedAtCreation = false
                }
            MemoryManagement.mfree = fun b _ ->
                b.Dispose()

            MemoryManagement.mcopy = 
                fun src srcOff dst dstOff size -> 
                    use e = device.CreateCommandEncoder()
                    e.CopyBufferToBuffer(src, uint64 srcOff, dst, uint64 dstOff, uint64 size)
                    use cmd = e.Finish()
                    let q = device.GetDefaultQueue()
                    q.Submit [|cmd|]
                    q.Wait()

            MemoryManagement.mrealloc = 
                fun oldBuffer oldSize newSize -> 
                    if oldSize <> newSize then
                        let newBuffer = 
                            device.CreateBuffer {
                                Label = null
                                Usage = usage
                                Size = uint64 newSize
                                MappedAtCreation = false
                            }
                        use e = device.CreateCommandEncoder()
                        e.CopyBufferToBuffer(oldBuffer, 0UL, newBuffer, 0UL, uint64 (min oldSize newSize))
                        use cmd = e.Finish()
                        let q = device.GetDefaultQueue()
                        q.Submit [|cmd|]
                        q.Wait()
                        oldBuffer.Dispose()
                        newBuffer

                    else
                        oldBuffer
        }


    let blockSize = uint64 block.ubSize
    let bufferCap = 65536
    let mutable freeList : list<int> = []
    let mutable usedInLast = 0
    let blocks = System.Collections.Generic.List<struct(Buffer * nativeint)>()

    member x.Alloc() =
        match freeList with
        | idx :: t ->
            freeList <- t
            let bufferId = idx / bufferCap 
            let offset = uint64 (idx % bufferCap) * blockSize
            let struct(b, p) = blocks.[bufferId]
            UniformBufferSlot(x, b, p, bufferId, offset)
        | [] -> 
            if blocks.Count <= 0 || usedInLast >= bufferCap then
                let size = uint64 bufferCap * blockSize
                let newBuffer =
                    device.CreateBuffer {
                        Label = null
                        Usage = usage
                        Size = size
                        MappedAtCreation = false
                    }

                let ptr = Marshal.AllocHGlobal (nativeint size)
                blocks.Add(struct(newBuffer, ptr))
                usedInLast <- 0

            let idxInBuffer = usedInLast
            let bi = blocks.Count - 1
            usedInLast <- usedInLast + 1

            let struct(b, p) = blocks.[bi]
            UniformBufferSlot(x, b, p, bi, uint64 idxInBuffer * blockSize)





type UniformBufferPromise(env : UpdateQueue, state : UpdateState, block : FShade.GLSL.GLSLUniformBuffer, values : HashMap<string, IAdaptiveValue>) =
    inherit ResourcePromise<UniformBufferPromise, struct(FShade.GLSL.GLSLUniformBuffer * HashMap<string, IAdaptiveValue>), BindGroupEntry>(state, struct(block, values))

    let mutable buffer : Buffer = null
    let mutable content = 0n
    let mutable writers : list<(nativeint -> unit) * ref<IDisposable>> = []

    static let mem (device : Device) (usage : BufferUsage) =
        //let device = state.device
        //let usage = BufferUsage.Uniform ||| BufferUsage.CopySrc ||| BufferUsage.CopyDst
        {
            MemoryManagement.malloc = fun s -> 
                device.CreateBuffer {
                    Label = null
                    Usage = usage
                    Size = uint64 s
                    MappedAtCreation = false
                }
            MemoryManagement.mfree = fun b _ ->
                b.Dispose()

            MemoryManagement.mcopy = 
                fun src srcOff dst dstOff size -> 
                    use e = device.CreateCommandEncoder()
                    e.CopyBufferToBuffer(src, uint64 srcOff, dst, uint64 dstOff, uint64 size)
                    use cmd = e.Finish()
                    let q = device.GetDefaultQueue()
                    q.Submit [|cmd|]
                    q.Wait()

            MemoryManagement.mrealloc = 
                fun oldBuffer oldSize newSize -> 
                    if oldSize <> newSize then
                        let newBuffer = 
                            device.CreateBuffer {
                                Label = null
                                Usage = usage
                                Size = uint64 newSize
                                MappedAtCreation = false
                            }
                        use e = device.CreateCommandEncoder()
                        e.CopyBufferToBuffer(oldBuffer, 0UL, newBuffer, 0UL, uint64 (min oldSize newSize))
                        use cmd = e.Finish()
                        let q = device.GetDefaultQueue()
                        q.Submit [|cmd|]
                        q.Wait()
                        oldBuffer.Dispose()
                        newBuffer

                    else
                        oldBuffer
        }

    let manager = 
        let usage = BufferUsage.Uniform ||| BufferUsage.CopySrc ||| BufferUsage.CopyDst
        new MemoryManagement.ChunkedMemoryManager<_>(mem state.device usage, 32n <<< 20)

    let updateBuffer() =
        if not (isNull buffer) then 
            for (w,_) in writers do w content
            state.device.GetDeviceTransferQueue().Upload(content, buffer, 0L, nativeint block.ubSize)


    let getWriters(uniforms : HashMap<string, IAdaptiveValue>) =
        block.ubFields |> List.choose (fun f ->
            match HashMap.tryFind f.ufName uniforms with
            | Some value ->
                let srcType = value.ContentType
                let dstType = GLSLType.toType f.ufType
                let conv = PrimitiveValueConverter.getConverter srcType dstType

                let sub = ref { new IDisposable with member x.Dispose() = () }
                let write (ptr : nativeint) =
                    TypeVisitors.visit2 srcType dstType {
                        new TypeVisitors.ITypeVisitor2<int> with
                            member x.Visit<'a, 'b when 'b : unmanaged>() = 
                                let value = unbox<aval<'a>> value
                                let conv = unbox<'a -> 'b> conv
                                let v = value.GetValue()
                                let os = sub.Value
                                sub := value.AddMarkingCallback(fun () -> env.Enqueue updateBuffer)
                                os.Dispose()
                                NativeInt.write (ptr + nativeint f.ufOffset) (conv v)
                                0
                    } |> ignore

                Some(write, sub)
            | None ->
                None

        )

    member x.Block = block

    //override x.Update() =
    //    if not (isNull buffer) then
    //        updateBuffer()

    override x.Recreate(struct(newBlock, newValues)) =
        new UniformBufferPromise(env, state, newBlock, newValues)
            

    override x.Destroy(_handle : BindGroupEntry) =
        if not (isNull buffer) then
            buffer.Dispose()
            Marshal.FreeHGlobal content
            buffer <- null
            content <- 0n
            for (_,d) in writers do d.Value.Dispose()
            writers <- []

    override x.Create() =

        buffer <-
            state.device.CreateBuffer {
                Label = null
                Usage = BufferUsage.Uniform ||| BufferUsage.CopyDst
                MappedAtCreation = false
                Size = uint64 block.ubSize
            }
        content <- Marshal.AllocHGlobal block.ubSize
        writers <- getWriters values
        env.Enqueue updateBuffer


        {
            BindGroupEntry.Binding = block.ubBinding
            BindGroupEntry.Buffer = buffer
            BindGroupEntry.Offset = 0UL
            BindGroupEntry.Size = uint64 block.ubSize
            BindGroupEntry.Sampler = null
            BindGroupEntry.TextureView = null
        }
//type BindGroupPromiseEntry =
//    | UniformBuffer of UniformBufferPromise

type BindGroupPromise(env : UpdateQueue, state : UpdateState, layout : BindGroupLayoutPromise, info : list<BindingInfo>, values : HashMap<string, IAdaptiveValue>, things : list<ResourcePromise<BindGroupEntry>>) =
    inherit ResourcePromise<BindGroupPromise, struct(BindGroupLayoutPromise * list<BindingInfo> * HashMap<string, IAdaptiveValue>), BindGroup>(state, struct(layout, info, values))

    static let filterUniforms (block : FShade.GLSL.GLSLUniformBuffer) (values : HashMap<string, IAdaptiveValue>) =
        let mutable res = HashMap.empty
        for f in block.ubFields do
            match HashMap.tryFind f.ufName values with
            | Some v -> res <- HashMap.add f.ufName v res
            | None -> ()
        res

    override x.Recreate(struct(newLayout : BindGroupLayoutPromise, newInfo : list<BindingInfo>, newValues : HashMap<string, IAdaptiveValue>)) =
        new BindGroupPromise(env, state, newLayout, newInfo, newValues)
      

    override x.Create() =
        let layout = layout.Acquire()
        state.device.CreateBindGroup {
            Label = null
            Layout = layout
            Entries = things |> List.map (fun e -> e.Acquire()) |> List.toArray
        }

    override x.Destroy(h : BindGroup) =
        h.Dispose()
        layout.Release()
        things |> List.iter (fun t -> t.Release())

    new(env : UpdateQueue, state : UpdateState, layout : BindGroupLayoutPromise, info : list<BindingInfo>, values : HashMap<string, IAdaptiveValue>) =
        let things =
            info |> List.choose (fun info ->
                match info.value with
                | BindingValue.UniformBuffer block ->
                    state.GetOrCreate(block, values, fun block values ->
                        new UniformBufferPromise(env, state, block, filterUniforms block values)
                    )
                    :> ResourcePromise<_>
                    |> Some
                | BindingValue.Texture _ ->
                    failwith ""
                | BindingValue.None ->
                    None
            )
        new BindGroupPromise(env, state, layout, info, values, things)





[<CustomEquality; NoComparison>]
type TraversalState =
    {
        update              : UpdateState
        shader              : option<ShaderPromise>
        bindGroupLayouts    : list<BindGroupLayoutPromise>
        pipelineLayout      : option<PipelineLayoutPromise>
        vertexBuffers       : HashMap<string, BufferPromise>
        instanceBuffers     : HashMap<string, BufferPromise>
        pipeline            : option<PipelinePromise>
        uniformValues       : HashMap<string, IAdaptiveValue>
        bindGroups          : HashMap<int, BindGroupPromise>
    }

    override x.GetHashCode() =
        failwith ""

    override x.Equals(o : obj) =
        match o with
        | :? TraversalState as o ->
            
            let inline (==) (a : 'a) (b : 'a) =
                System.Object.ReferenceEquals(a, b)

            x.shader = o.shader &&
            (x.bindGroupLayouts == o.bindGroupLayouts || x.bindGroupLayouts = o.bindGroupLayouts) &&
            (x.pipelineLayout = o.pipelineLayout) &&
            (ShallowEqualityComparer.ShallowEquals(x.vertexBuffers, o.vertexBuffers) || x.vertexBuffers = o.vertexBuffers) &&
            (ShallowEqualityComparer.ShallowEquals(x.instanceBuffers, o.instanceBuffers) || x.instanceBuffers = o.instanceBuffers) &&

            x.pipeline = o.pipeline &&
            (ShallowEqualityComparer.ShallowEquals(x.uniformValues, o.uniformValues) || x.uniformValues = o.uniformValues) &&
            (ShallowEqualityComparer.ShallowEquals(x.bindGroups, o.bindGroups) || x.bindGroups = o.bindGroups)



        | _ ->
            false



module TraversalState =     
    let empty (u : UpdateState) =
        {
            update = u
            shader = None
            bindGroupLayouts = []
            pipelineLayout = None
            vertexBuffers = HashMap.empty
            instanceBuffers = HashMap.empty
            pipeline = None
            uniformValues = HashMap.empty
            bindGroups = HashMap.empty
        }

module Sg =
    
    type Sg = Node<TraversalState>

    type GroupComponent(e : Environment, tup) =
        inherit Component<TraversalState, (SgAttributes * list<Sg>)>(e, tup)

        static let filterUniforms (infos : list<BindingInfo>) (values : HashMap<string, IAdaptiveValue>) =
            let mutable res = HashMap.empty

            let inline take (a : string) =
                match HashMap.tryFind a values with
                | Some v -> res <- HashMap.add a v res
                | None -> ()

            for i in infos do
                match i.value with
                | BindingValue.None -> ()
                | BindingValue.Texture(_, names) ->
                    for (n, _) in names do take n
                | BindingValue.UniformBuffer block ->
                    for f in block.ubFields do
                        take f.ufName

            res
                        

        let mutable myUniforms : HashMap<string, (IAdaptiveValue -> unit) * IAdaptiveValue> = HashMap.empty
        let mutable shaderPromise : option<ShaderPromise> = None
        let mutable bindGroupLayoutPromises : BindGroupLayoutPromise[] = [||]
        let mutable pipelineLayoutPromise : option<PipelineLayoutPromise> = None
        let mutable vertexAttributes : HashMap<string, BufferPromise> = HashMap.empty
        let mutable instanceAttributes : HashMap<string, BufferPromise> = HashMap.empty
        let mutable pipelinePromise : option<PipelinePromise> = None
        let mutable bindGroupPromises : HashMap<int, BindGroupPromise> = HashMap.empty

        override x.Unmount() =  
            ()

        override x.Update(_prog : RenderFragment, state : TraversalState) =
            let (atts, children) = x.State

            // uniform values
            let state =
                if not (HashMap.isEmpty atts.uniforms) then
                    
                    let childValues = 
                        (myUniforms, atts.uniforms) ||> HashMap.choose2 (fun name mine value ->
                            match value with
                            | Some v ->
                                match mine with
                                | Some (write, value) ->
                                    if v.IsConstant then 
                                        write v
                                        Some value
                                    else    
                                        myUniforms <- HashMap.remove name myUniforms
                                        Some v
                                | None ->
                                    if v.IsConstant then
                                        // create one
                                        let write, cval = 
                                            TypeVisitors.visit v.ContentType {
                                                new TypeVisitors.ITypeVisitor<(IAdaptiveValue -> unit) * IAdaptiveValue> with
                                                    member x.Visit<'a>() =
                                                        let v = unbox<aval<'a>> v

                                                        let c = cval (AVal.force v)
                                                        let change (input : IAdaptiveValue) =
                                                            let v = AVal.force (unbox<aval<'a>> input)
                                                            transact (fun () ->
                                                                c.Value <- v
                                                            )

                                                        change, c :> IAdaptiveValue
                                            }
                                        myUniforms <- HashMap.add name (write, cval) myUniforms
                                        Some cval
                                    else
                                        Some v
                            | None ->
                                myUniforms <- HashMap.remove name myUniforms
                                None
                        )

                    { state with uniformValues = HashMap.union state.uniformValues childValues }

                else
                    state

            // shader
            let state =
                match atts.shader with
                | Some e ->
                    match shaderPromise with
                    | Some o ->
                        let n = o.Update [e]
                        { state with shader = Some n }
                    | None -> 
                        let p = state.update.GetOrCreate(e, fun e -> new ShaderPromise(state.update, [e]))
                        shaderPromise <- Some p
                        { state with shader = Some p }
                | None ->
                    state

            // layouts
            let state =
                match state.shader with
                | Some shaderPromise when Option.isSome atts.shader ->
                    let mutable state = state

                    let groups =
                        shaderPromise.BindGroups |> Array.map (Array.map (fun i -> i.entry) >> Array.toList)

                    bindGroupLayoutPromises <- 
                        if bindGroupLayoutPromises.Length = groups.Length then
                            (bindGroupLayoutPromises, groups) ||> Array.map2 (fun p v ->
                                p.Update v
                            )
                        else
                            groups |> Array.map (fun v ->
                                state.update.GetOrCreate(v, fun v -> new BindGroupLayoutPromise(state.update, v))
                            )

                    let groupPromises = Array.toList bindGroupLayoutPromises
                    state <- { state with bindGroupLayouts = groupPromises }
                    state <- 
                        match pipelineLayoutPromise with
                        | Some old ->
                            { state with pipelineLayout = Some (old.Update groupPromises) }
                        | None -> 
                            let layout = state.update.GetOrCreate(groupPromises, fun bindGroupLayoutPromises -> new PipelineLayoutPromise(state.update, bindGroupLayoutPromises))
                            pipelineLayoutPromise <- Some layout
                            { state with pipelineLayout = Some layout }

                    state
                | _ ->
                    state

            // vertexBuffers
            let state =
                if HashMap.isEmpty atts.vertexAttributes then
                    vertexAttributes <- HashMap.empty
                    state
                else
                    let usage = BufferUsage.CopyDst ||| BufferUsage.Vertex
                    vertexAttributes <-
                        (atts.vertexAttributes, vertexAttributes) ||> HashMap.choose2V (fun k att prom ->
                            match att with
                            | ValueSome att ->
                                match prom with
                                | ValueSome prom ->
                                    prom.Update(struct (usage, att)) |> ValueSome
                                | ValueNone ->
                                    state.update.GetOrCreate(usage, att, fun usage att ->
                                        new BufferPromise(state.update, usage, att) 
                                    ) |> ValueSome
                            | ValueNone ->
                                match prom with
                                | ValueSome _ -> ()
                                | ValueNone -> ()
                                ValueNone
                        )
                    { state with vertexBuffers = HashMap.union state.vertexBuffers vertexAttributes }

            // instanceBuffers
            let state =
                if HashMap.isEmpty atts.instanceAttributes then
                    instanceAttributes <- HashMap.empty
                    state
                else
                    let usage = BufferUsage.CopyDst ||| BufferUsage.Vertex
                    instanceAttributes <-
                        (atts.instanceAttributes, instanceAttributes) ||> HashMap.choose2V (fun k att prom ->
                            match att with
                            | ValueSome att ->
                                match prom with
                                | ValueSome prom ->
                                    prom.Update(struct (usage, att)) |> ValueSome
                                | ValueNone ->
                                    state.update.GetOrCreate(usage, att, fun usage att ->
                                        new BufferPromise(state.update, usage, att) 
                                    ) |> ValueSome
                            | ValueNone ->
                                match prom with
                                | ValueSome _ -> ()
                                | ValueNone -> ()
                                ValueNone
                        )
                    { state with instanceBuffers = HashMap.union state.instanceBuffers instanceAttributes }

            // pipeline
            let state = 
                let pipelineChanged = Option.isSome atts.shader // TODO: BlendMode/etc.
                match state.shader, state.pipelineLayout with
                | Some shader, Some layout when pipelineChanged ->
                    match pipelinePromise with
                    | Some prom ->
                        let formats = state.vertexBuffers |> HashMap.map (fun _ b -> b.Data.Format, b.Data.SlotCount)
                        let instanceFormats = state.instanceBuffers |> HashMap.map (fun _ b -> b.Data.Format, b.Data.SlotCount)
                        let res = prom.Update(PrimitiveTopology.TriangleList, layout, shader, formats, instanceFormats)
                        pipelinePromise <- Some res
                        { state with pipeline = Some res }
                    | None ->
                        let formats = state.vertexBuffers |> HashMap.map (fun _ b -> b.Data.Format, b.Data.SlotCount)
                        let instanceFormats = state.instanceBuffers |> HashMap.map (fun _ b -> b.Data.Format, b.Data.SlotCount)
                        let pipe = 
                            state.update.GetOrCreate(PrimitiveTopology.TriangleList, layout, shader, formats, instanceFormats, fun top layout shader a b ->
                                new PipelinePromise(state.update, top, layout, shader, a, b)
                            )
                        pipelinePromise <- Some pipe
                        { state with pipeline = Some pipe }
                | _ ->
                    state

       
            let updateUniformBuffers =
                Option.isSome atts.shader || not (HashMap.isEmpty atts.uniforms)

            let state =
                if updateUniformBuffers then
                    let layouts = state.bindGroupLayouts |> List.indexed |> HashMap.ofList
                    let infos = state.shader.Value.BindGroups |> Seq.indexed |> HashMap.ofSeq

                    let layoutsAndInfos = 
                        (layouts, infos) 
                        ||> HashMap.choose2V (fun _ l r -> 
                            match l with
                            | ValueSome l ->
                                match r with
                                | ValueSome r ->
                                    ValueSome struct(l,Array.toList r)
                                | _ ->
                                    ValueNone
                            | _ ->
                                ValueNone
                        )


                    bindGroupPromises <-
                        (bindGroupPromises, layoutsAndInfos) ||> HashMap.choose2V (fun _ p layout ->
                            match layout with
                            | ValueSome struct (layout, infos) ->
                                match p with
                                | ValueSome p -> 
                                    let uniformValues = filterUniforms infos state.uniformValues
                                    p.Update(layout, infos, uniformValues) |> ValueSome
                                | ValueNone -> 
                                    let uniformValues = filterUniforms infos state.uniformValues
                                    state.update.GetOrCreate(layout, infos, uniformValues, fun layout infos uniformValues ->
                                        new BindGroupPromise(e.Reconciler, state.update, layout, infos, uniformValues)
                                    ) |> ValueSome
                            | ValueNone ->
                                ValueNone
                        )

                    { state with bindGroups = bindGroupPromises }
                else
                    state
                

            (state, children)

    type DrawComponent(e : Environment, info : DrawInfo) =
        inherit Component<TraversalState, DrawInfo>(e, info)

        let mutable destroy = System.Collections.Generic.List<IDisposable>()

        //override x.ShouldUpdate(o, n) =
        //    true

        override x.Update(prog : RenderFragment, state : TraversalState) =
            //printfn "update draw %A" x.State
            let d = destroy
            destroy <- System.Collections.Generic.List<IDisposable>()

            let info = x.State
            match state.pipeline with
            | Some p ->
                let pp = p.WithPrimitiveTopology info.mode
                let pipe = pp.Acquire()
                destroy.Add pp


                let buffers =
                    p.Interface.inputs |> List.toArray |> Array.collect (fun p ->
                        match HashMap.tryFind p.paramSemantic state.vertexBuffers with
                        | Some vb -> 
                            destroy.Add vb
                            let struct (b,o,s) = vb.Acquire()
                            Array.init vb.Data.SlotCount (fun i ->
                                (p.paramLocation + i, b, o, s)
                            )

                        | None -> 
                            match HashMap.tryFind p.paramSemantic state.instanceBuffers with
                            | Some vb -> 
                                destroy.Add vb
                                let struct (b,o,s) = vb.Acquire()
                                Array.init vb.Data.SlotCount (fun i ->
                                    (p.paramLocation + i, b, o, s)
                                )
                            | None -> 
                                [||]
                    )

                let groups = 
                    state.bindGroups |> HashMap.map (fun _ g -> 
                        destroy.Add g
                        let g = g.Acquire()
                        g
                    )

                //let ubs =
                //    state.uniformBuffers |> HashMap.map (fun _ v ->
                //        let bb = v.Acquire()
                //        destroy.Add bb
                //        bb
                //    )


                prog.Update (fun s ->
                    s.SetPipeline pipe

                    for (idx, binding) in groups do
                        s.SetBindGroup(idx, binding, null)

                    for (slot, buffer, offset, size) in buffers do
                        
                        s.SetVertexBuffer(slot, buffer, offset, size)

                    s.Draw info
                )
            | _ ->
                failwith "no pipeline"

            for o in d do o.Dispose()
            state, []

        override x.Unmount() =
            for d in destroy do d.Dispose()
            destroy.Clear()

    type Eq<'a>(value : 'a) =
        member x.Value = value

        override x.GetHashCode() = 0
        override x.Equals o =
            match o with
            | :? Eq<'a> -> true
            | _ -> false

    type MemoComponent<'a>(e : Environment, tup) =
        inherit Component<TraversalState, 'a * Eq<aval<'a> -> alist<Sg>>>(e, tup)

        static let ctor = Node.create MemoComponent<'a>
        

        let myValue = cval (fst tup)
        let value = ()
        let mutable reader = Unchecked.defaultof<_>
        let mutable s = { new IDisposable with member x.Dispose() = () }
        let mutable children : IndexList<ReconcilerNode<TraversalState>> = IndexList.empty
        let mutable oldState = None

        static member Constructor = ctor

        override x.ReceivedValue() =    
            let (value, creator) = x.State
            transact (fun () ->
                myValue.Value <- value
            )

        override x.Mount() =
            let (_value, creator) = x.State
            let list = creator.Value (myValue :> aval<_>)
            reader <- list.GetReader()
            s <- reader.AddMarkingCallback (fun () -> x.ForceUpdate())

            ()

        override x.Unmount() =
            reader <- Unchecked.defaultof<_>
            s.Dispose()
            for c in children do c.Destroy()
            children <- IndexList.empty
            oldState <- None

        override x.Update(prog : RenderFragment, state : TraversalState) =
            let ops = reader.GetChanges()
            
            for idx, op in IndexListDelta.toSeq ops do
                match op with
                | Set v ->
                    match IndexList.tryGet idx children with
                    | Some child ->
                        child.Node <- v
                    | None ->
                        let anchor = 
                            match IndexList.tryGetPrev idx children with
                            | Some (_, f) -> f.Fragment
                            | None -> null

                        let n = ReconcilerNode<TraversalState>(e.Reconciler, e.Level + 1, v, state)
                        n.Fragment <- prog.InsertAfter(anchor)
                        children <- IndexList.set idx n children
                | Remove ->
                    match IndexList.tryRemove idx children with
                    | Some (child, rest) ->
                        child.Destroy()
                        children <- rest
                    | None -> 
                        ()
                    ()

            match oldState with
            | Some s when s <> state -> 
                for c in children do
                    c.TraversalState <- state
            | _ ->
                ()

            oldState <- Some state
            state, []

    open FSharp.Data.Traceable
    type MemoListComponent<'a>(e : Environment, tup) =
        inherit Component<TraversalState, list<'a> * Eq<alist<'a> -> alist<Sg>>>(e, tup)

        static let ctor = Node.create MemoListComponent<'a>
        

        let myValue = History IndexList.trace
        let value = ()
        let mutable reader = Unchecked.defaultof<_>
        let mutable s = { new IDisposable with member x.Dispose() = () }
        let mutable children : IndexList<ReconcilerNode<TraversalState>> = IndexList.empty
        let mutable oldState = None

        static member Constructor = ctor

        //override x.ShouldUpdate(o, n) =
        //    Log.line "%A %A" o n
        //    true

        override x.ReceivedValue() =    
            let (value, creator) = x.State
            transact (fun () ->
                let ops = IndexList.computeDeltaToList myValue.State value
                myValue.Perform ops |> ignore
            )

        override x.Mount() =
            let (value, creator) = x.State

            let list =
                { new alist<'a> with
                    member x.Content = myValue :> aval<_>
                    member x.History = Some myValue
                    member x.IsConstant = false
                    member x.GetReader() = myValue.NewReader()
                }
                
            let ops = IndexList.computeDeltaToList myValue.State value
            myValue.Perform ops |> ignore

            let list = creator.Value list
            reader <- list.GetReader()
            s <- reader.AddMarkingCallback (fun () -> x.ForceUpdate())

            ()

        override x.Unmount() =
            reader <- Unchecked.defaultof<_>
            s.Dispose()
            for c in children do c.Destroy()
            children <- IndexList.empty
            oldState <- None

        override x.Update(prog : RenderFragment, state : TraversalState) =
            let ops = reader.GetChanges()
            
            for idx, op in IndexListDelta.toSeq ops do
                match op with
                | Set v ->
                    match IndexList.tryGet idx children with
                    | Some child ->
                        child.Node <- v
                    | None ->
                        let anchor = 
                            match IndexList.tryGetPrev idx children with
                            | Some (_, f) -> f.Fragment
                            | None -> null

                        let n = ReconcilerNode<TraversalState>(e.Reconciler, e.Level + 1, v, state)
                        n.Fragment <- prog.InsertAfter(anchor)
                        children <- IndexList.set idx n children
                | Remove ->
                    match IndexList.tryRemove idx children with
                    | Some (child, rest) ->
                        child.Destroy()
                        children <- rest
                    | None -> 
                        ()
                    ()

            match oldState with
            | Some s when s <> state -> 
                for c in children do
                    c.TraversalState <- state
            | _ ->
                ()

            oldState <- Some state
            state, []


    let draw = 
        Node.create DrawComponent

    let group = 
        let creator = Node.create GroupComponent
        fun a b -> creator((a,b))

        
    let memo<'a> (value : 'a) (creator : aval<'a> -> alist<Sg>) =
        MemoComponent<'a>.Constructor (value, Eq creator)
         
    let memoList<'a> (value : list<'a>) (creator : alist<'a> -> alist<Sg>) =
        MemoListComponent<_>.Constructor (value, Eq creator)
        
    let alist<'a> (value : list<'a>) (creator : 'a -> Sg) =
        memoList value (AList.map creator)
        //MemoListComponent<'a>.Constructor (value, Eq (AList.map creator))
        
    let list<'a> (value : list<'a>) (creator : 'a -> Sg) =
        List.map creator value |> group (SgAttributes.empty ())

    let inline trafo (t : Trafo3d) = SgAttribute.Trafo t
    let inline vertexAttribute (name : string) (value : BufferDescriptor) = SgAttribute.VertexAttribute(name, value)
    let inline instanceAttribute (name : string) (value : BufferDescriptor) = SgAttribute.InstanceAttribute(name, value)

    let inline vertexAttributes (values : #seq<string * BufferDescriptor>) = SgAttribute.VertexAttributes(HashMap.ofSeq values)
    let inline instanceAttributes (values : #seq<string * BufferDescriptor>) = SgAttribute.InstanceAttributes(HashMap.ofSeq values)

    let inline vertexData (name : string) (value : Data) = SgAttribute.VertexAttribute(name, BufferDescriptor.Data value)
    let inline instanceData (name : string) (value : Data) = SgAttribute.InstanceAttribute(name, BufferDescriptor.Data value)
    let inline uniform (name : string) (value : 'a) = SgAttribute.Uniform(name, AVal.constant value :> IAdaptiveValue)
    let shader = SgAttribute.ShaderBuilder()
    let effect (l : list<FShade.Effect>) = SgAttribute.Shader l


    type AdaptiveDrawComponent(e : Environment, info : aval<DrawInfo>) =
        inherit Component<TraversalState, aval<DrawInfo>>(e, info)

        let mutable s = { new IDisposable with member x.Dispose() = () }

        override x.Mount() =
            s <- info.AddMarkingCallback (fun () -> 
                //Log.line "mark leaf"
                x.ForceUpdate()
            )
            //Log.line "mount aleaf"

        override x.Update(prog : RenderFragment, state : TraversalState) =
            //Log.line "update leaf"
            let info = x.State |> AVal.force
            state, [draw info]
         
        override x.Unmount() =
            //Log.line "unmount leaf"
            s.Dispose()
            s <- { new IDisposable with member x.Dispose() = () }

    type GroupBuilderInternal() =
        let children = ListBuilder<Sg>()
        let mutable attributes = SgAttributes.empty()

        member x.Emit(child : Sg) =
            children.Append child
            
        member x.Emit(shader : SgAttribute.Shader) =
            attributes.shader <- Some (FShade.Effect.compose shader.Effects)

        member x.Emit(trafo : SgAttribute.Trafo) =
            attributes.modelTrafo <-
                match attributes.modelTrafo with
                | Some t -> Some (t * trafo.Value)
                | None -> Some trafo.Value
                
        member x.Emit(att : SgAttribute.VertexAttribute) =
            attributes.vertexAttributes <- HashMap.add att.Name att.Data attributes.vertexAttributes
            
        member x.Emit(att : SgAttribute.InstanceAttribute) =
            attributes.instanceAttributes <- HashMap.add att.Name att.Data attributes.instanceAttributes
            
        member x.Emit(att : SgAttribute.VertexAttributes) =
            attributes.vertexAttributes <- HashMap.union attributes.vertexAttributes att.Values
            
        member x.Emit(att : SgAttribute.InstanceAttributes) =
            attributes.instanceAttributes <- HashMap.union attributes.instanceAttributes att.Values
            
        member x.Emit(att : SgAttribute.Uniform) =
            attributes.uniforms <- HashMap.add att.Name att.Value attributes.uniforms

        member x.ToSg() =
            let children = children.ToList()
            let att = attributes
            attributes <- SgAttributes.empty()
            group att children

    type GroupBuilder() =
        
        [<ThreadStatic; DefaultValue>]
        static val mutable private _GroupBuilder : GroupBuilderInternal
        
        static member GroupBuilder
            with get() = GroupBuilder._GroupBuilder
            and set v = GroupBuilder._GroupBuilder <- v
            
        member inline x.Yield(sg : Sg) =
            GroupBuilder.GroupBuilder.Emit sg

        member inline x.Yield(s : SgAttribute.Shader) =
            GroupBuilder.GroupBuilder.Emit s
            
        member inline x.Yield(s : SgAttribute.VertexAttribute) =
            GroupBuilder.GroupBuilder.Emit s

        member inline x.Yield(s : SgAttribute.InstanceAttribute) =
            GroupBuilder.GroupBuilder.Emit s
            
        member inline x.Yield(s : SgAttribute.VertexAttributes) =
            GroupBuilder.GroupBuilder.Emit s

        member inline x.Yield(s : SgAttribute.InstanceAttributes) =
            GroupBuilder.GroupBuilder.Emit s

        member inline x.Yield(s : SgAttribute.Uniform) =
            GroupBuilder.GroupBuilder.Emit s
            
        member inline x.Delay(action : unit -> unit) =
            action

        member inline x.For(s : seq<'a>, action : 'a -> unit) =  
            GroupBuilder.GroupBuilder.Emit (
                alist (Seq.toList s) (fun a ->
                    let o = GroupBuilder.GroupBuilder
                    GroupBuilder.GroupBuilder <- GroupBuilderInternal()
                    action a
                    let value = GroupBuilder.GroupBuilder.ToSg()
                    GroupBuilder.GroupBuilder <- o
                    value
                )
            )

        member inline x.Zero() =
            ()

        member inline x.Combine((), r : unit -> unit) =
            r()

        member inline x.Run(action : unit -> unit) =
            let o = GroupBuilder.GroupBuilder
            GroupBuilder.GroupBuilder <- GroupBuilderInternal()
            try 
                action()
                GroupBuilder.GroupBuilder.ToSg()
            finally 
                GroupBuilder.GroupBuilder <- o

    let bgroup = GroupBuilder()

    let adraw = 
        Node.create AdaptiveDrawComponent
        
    type InstancedComponent<'a>(e : Environment, tup) =
        inherit Component<TraversalState, struct(DrawInfo * list<'a> * Set<string> * Eq<list<'a> -> HashMap<string, Data>>)>(e, tup)

        let mutable effectCache = None
        let mutable bufferCache = None

        override x.Update(p, s) =
            let struct (info, values, types, getter) = x.State
            match s.shader with
            | Some shader ->    
                let instancedEffect =
                    match effectCache with
                    | Some (a, b) when Unchecked.equals a shader -> b
                    | _ ->
                        let instancedEffect =
                            shader.Effect |>
                            FShade.Effect.uniformsToInputs types
                        effectCache <- Some (shader, instancedEffect)
                        instancedEffect

                let atts, drawInfo =
                    match bufferCache with
                    | Some (a, b, c) when Unchecked.equals a values -> b, c
                    | _ ->
                        let atts = getter.Value values
                        let cnt = atts |> HashMap.toSeq |> Seq.map (fun (_,d) -> d.Count) |> Seq.min

                        let res = atts |> HashMap.map (fun _ d -> BufferDescriptor.Data d)
                        let info = { info with instanceCount = cnt }
                        bufferCache <- Some (values, res, info)
                        res, info

                //let a = SgAttributes.empty()
                //a.instanceAttributes <- atts
                //a.shader <- Some instancedEffect
                s, [
                    bgroup {
                        instanceAttributes atts
                        effect [ instancedEffect ]
                        draw drawInfo
                    }
                ]
                //s, [ group a [ draw drawInfo ] ]
            | None ->
                s, []
            
    type private InstancedHelpers<'a> private() =
        static let creator = Node.create InstancedComponent<'a>
        static member Create(info : DrawInfo, uniforms : Set<string>, values : list<'a>, getter : list<'a> -> HashMap<string, Data>) =
            creator struct(info, values, uniforms, Eq getter)


    let instanced (info : DrawInfo) (uniforms : Set<string>) (values : list<'a>) (getter : list<'a> -> HashMap<string, Data>) =
        InstancedHelpers<'a>.Create(info, uniforms, values, getter)

            

module SgTest = 

    module Shader =
        open FShade
        type Vertex = 
            {
                [<Position>] pos : V4d
            }

        let trafo (v : Vertex) =
            vertex {
                let m1 : V4d = uniform?trafo1
                let m2 : V4d = uniform?trafo2
                let m3 : V4d = uniform?trafo3
                let m4 : V4d = uniform?trafo4

                let m = M44d.FromRows(m1, m2, m3, m4)

                return { v with pos = m * v.pos }
            }

        let bla (v : Vertex) =
            fragment {      
                let a : V4d = uniform?a
                return a
            }

        let blubb (v : Vertex) =
            fragment {      
                let x : V4d = uniform?a
                return V4d(V3d.III - x.XYZ, 1.0)
            }

    let effect =
        FShade.Effect.ofFunction Shader.bla

    let quad =
        Data.Create [|
            V3f(-0.5f, -0.5f, 0.0f)
            V3f(0.5f, -0.5f, 0.0f)
            V3f(0.5f, 0.5f, 0.0f)
            
            V3f(-0.5f, -0.5f, 0.0f)
            V3f(0.5f, 0.5f, 0.0f)
            V3f(-0.5f, 0.5f, 0.0f)
        |]


    let blaEff = 
        FShade.Effect.compose [
            FShade.Effect.ofFunction Shader.trafo
            FShade.Effect.ofFunction Shader.bla
        ]
    let blubEff = 
        FShade.Effect.compose [
            FShade.Effect.ofFunction Shader.trafo
            FShade.Effect.ofFunction Shader.blubb
        ]


    let triData = Data.Create [| V3f(0.0, 0.0, 0.0); V3f(1.0, 0.0, 0.0); V3f(0.0, 1.0, 0.0) |]



    let testSg (color : C4b) (cnt : aval<DrawInfo>) (showQuad : bool) (triangles : list<Triangle2d>) (pos : Data) =
        Sg.bgroup {
            Sg.uniform "a" color

            Sg.effect [blaEff]
            Sg.vertexData "Positions" pos

            Sg.bgroup { 
                //Sg.vertexData "Positions" (Data.Create [| V3f(0.0, 0.0, 0.0); V3f(1.0, 0.0, 0.0); V3f(0.0, 1.0, 0.0) |])
                Sg.effect [blubEff]
                //Sg.vertexData "Positions" (Data.Create [| V3f(0.0, 0.0, 0.0); V3f(1.0, 0.0, 0.0); V3f(0.0, 1.0, 0.0) |])
                Sg.vertexData "Positions" triData //(Data.Create [| V3f(0.0, 0.0, 0.0); V3f(1.0, 0.0, 0.0); V3f(0.0, 1.0, 0.0) |])


                let info =
                    {
                        indexed = false
                        mode = PrimitiveTopology.TriangleList
                        instanceCount = 1
                        count = 3
                        first = 0
                        firstInstance = 0
                        baseVertex = 0
                    }

                if showQuad then
                    Sg.instanced info (Set.ofList ["trafo1"; "trafo2"; "trafo3"; "trafo4"]) triangles (fun triangles ->
                        let arr = triangles |> List.toArray

                        let trafos =
                            arr |> Array.map (fun tri ->
                                let u = V3d(tri.Edge01, 0.0)
                                let v = V3d(tri.Edge02, 0.0)
                                let n = Vec.cross u v

                                M44d.FromCols(V4d(u, 0.0), V4d(v, 0.0), V4d(n, 0.0), V4d(tri.P0, 0.0, 1.0))
                                |> M44f.op_Explicit
                            )

                        let t1 = trafos |> Array.map (fun m -> m.R0)
                        let t2 = trafos |> Array.map (fun m -> m.R1)
                        let t3 = trafos |> Array.map (fun m -> m.R2)
                        let t4 = trafos |> Array.map (fun m -> m.R3)

                        HashMap.ofList [
                            "trafo1", Data.Create t1
                            "trafo2", Data.Create t2
                            "trafo3", Data.Create t3
                            "trafo4", Data.Create t4
                        ]
                    )

                else
                    for tri in triangles do
                        Sg.bgroup {

                            //if tri.P0.X > 0.9 then Sg.effect [blaEff]
                            //else Sg.effect [blubEff]
                        
                            let u = V3d(tri.Edge01, 0.0)
                            let v = V3d(tri.Edge02, 0.0)
                            let n = Vec.cross u v

                            let m = M44d.FromCols(V4d(u, 0.0), V4d(v, 0.0), V4d(n, 0.0), V4d(tri.P0, 0.0, 1.0))
                            Sg.uniform "trafo1" m.R0
                            Sg.uniform "trafo2" m.R1
                            Sg.uniform "trafo3" m.R2
                            Sg.uniform "trafo4" m.R3
                            //Sg.uniform "trafo" (M44d.FromCols(V4d(u, 0.0), V4d(v, 0.0), V4d(n, 0.0), V4d(tri.P0, 0.0, 1.0)))

                            Sg.draw {
                                indexed = false
                                mode = PrimitiveTopology.TriangleList
                                instanceCount = 1
                                count = 3
                                first = 0
                                firstInstance = 0
                                baseVertex = 0
                            }
                        }
            }
            //Sg.memo elems (fun cnt ->   
            //    let rand = RandomSystem()
            //    let box = Box2d(-V2d.II, V2d.II)
            //    printfn "run memo"
            //    AList.range (AVal.constant 1) cnt 
            //    |> AList.map (fun i ->
            //        Log.line "boot %d" i
                    
            //        let color = rand.UniformC3f().ToC4b()
            //        let positions = Array.init 3 (fun _ -> V3f(rand.UniformV2d box, 0.0))

            //        Sg.bgroup {
            //            Sg.uniform "a" color
            //            Sg.vertexData "Positions" (Data.Create positions)



            //            Sg.draw {
            //                indexed = false
            //                mode = PrimitiveTopology.TriangleList
            //                instanceCount = 1
            //                count = 3
            //                first = 0
            //                firstInstance = 0
            //                baseVertex = 0
            //            }
            //        }
            //    )
            //)
            //for i in 1 .. 1000 do
            //    Sg.bgroup {
            //        Sg.uniform "a" C4b.Lime
            //        Sg.vertexData "Positions" quad

            //        Sg.draw {
            //            indexed = false
            //            mode = PrimitiveTopology.TriangleList
            //            instanceCount = 1
            //            count = 0
            //            first = 0
            //            firstInstance = 0
            //            baseVertex = 0
            //        }
            //    }


        }

        //group top [
            
        //    adraw (
        //        cnt
        //    )
        //    if showQuad then
        //        group { SgAttributes.empty with vertexAttributes = HashMap.ofList [ "Positions", BufferDescriptor.Data quad ] } [
        //            draw {
        //                indexed = false
        //                mode = PrimitiveTopology.TriangleList
        //                instanceCount = 1
        //                count = 6
        //                first = 0
        //                firstInstance = 0
        //                baseVertex = 0
        //            }
        //        ]

        //]
