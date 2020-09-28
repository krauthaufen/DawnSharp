namespace Armadillo

open System
open FSharp.Data.Adaptive
open Aardvark.Base
open WebGPU
open Armadillo

[<Struct>]
type SgAttributes =
    {
        viewTrafo           : option<Trafo3d>
        projTrafo           : option<Trafo3d>
        modelTrafo          : option<Trafo3d>
        shader              : option<FShade.Effect>
        blendMode           : option<BlendMode>
        depthTestMode       : option<DepthTestMode>
        stencilMode         : option<StencilMode>
        indirectBuffer      : option<BufferDescriptor>
        indexBuffer         : option<BufferDescriptor>
        vertexAttributes    : HashMap<string, BufferDescriptor>
        instanceAttributes  : HashMap<string, BufferDescriptor>
        uniforms            : HashMap<string, obj>
    }
    
module SgAttributes =
    let empty =
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

type UpdateState =
    {
        outputFormats   : TextureFormat[]
        outputs         : Map<string, (Type * int)>
        device          : Device
    }

type BufferPromise(state : UpdateState, usage : BufferUsage, data : BufferDescriptor) =
    
    let mutable cache : option<Buffer * uint64 * uint64> = None
    
    member x.Data = data

    member x.Update(newData : BufferDescriptor) =
        if newData = data then
            x
        else
            BufferPromise(state, usage, newData)

    member x.Acquire() =
        match cache with
        | Some(buf, off, size) when buf.ReferenceCount > 0 ->
            try 
                printfn "reuse buffer"
                (buf.Clone(), off, size)
            with :? System.ObjectDisposedException ->
                cache <- None
                x.Acquire()
        | _ ->
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
                printfn "create buffer %A" buffer.Handle.Handle
                let tup = (buffer, 0UL, uint64 data.Size)
                cache <- Some tup
                tup

            | BufferDescriptor.Buffer(fmt, buf, off, size) ->
                (buf.Clone(), uint64 off, uint64 size)

type ShaderPromise(state : UpdateState, effects : list<FShade.Effect>) =
    
    let glsl = 
        let cfg : FShade.EffectConfig =
            {
                FShade.EffectConfig.depthRange = Range1d(0.0, 1.0)
                FShade.EffectConfig.flipHandedness = false
                FShade.EffectConfig.lastStage = FShade.ShaderStage.Fragment
                FShade.EffectConfig.outputs = state.outputs
            }

        let backend = FShade.Backends.glsl430
        effects
        |> FShade.Effect.compose
        |> FShade.Effect.toModule cfg
        |> FShade.Imperative.ModuleCompiler.compile backend
        |> FShade.GLSL.Assembler.assemble backend

    let mutable cache : option<ShaderModule * ShaderModule> = None

    member x.Interface = glsl.iface

    member x.Update(newEffects : list<FShade.Effect>) =
        if effects = newEffects then
            x
        else
            ShaderPromise(state, newEffects)

    member x.Acquire() =    
        match cache with
        | Some (vs, fs) when vs.ReferenceCount > 0 && fs.ReferenceCount > 0 ->
            try 
                printfn "reuse shader"
                (vs.Clone(), fs.Clone())
            with :? System.ObjectDisposedException ->
                cache <- None
                x.Acquire()
        | _ -> 
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
            printfn "create shader"

            cache <- Some (vs, fs)
            (vs, fs)

type PipelinePromise(state : UpdateState, shader : ShaderPromise, vertexBuffers : HashMap<string, VertexFormat>, instanceBuffers : HashMap<string, VertexFormat>) =
    let mutable cache : HashMap<PrimitiveTopology, ShaderModule * ShaderModule * RenderPipeline> = HashMap.empty

    member x.Interface = shader.Interface

    member x.Update(newShader : ShaderPromise, newVertexBuffers : HashMap<string, VertexFormat>, newInstanceBuffers : HashMap<string, VertexFormat>) =
        if newShader = shader && newVertexBuffers = vertexBuffers && newInstanceBuffers = instanceBuffers then
            x
        else
            PipelinePromise(state, newShader, newVertexBuffers, newInstanceBuffers)

    member x.Acquire(top : PrimitiveTopology) =
        match HashMap.tryFind top cache with
        | Some (vs,fs,p) when vs.ReferenceCount > 0 && fs.ReferenceCount > 0 && p.ReferenceCount > 0 ->
            try 
                printfn "reuse pipeline"
                vs.Clone(), fs.Clone(), p.Clone()
            with :? System.ObjectDisposedException -> 
                cache <- HashMap.remove top cache
                x.Acquire top
        | _ ->

            let mutable groups : MapExt<int, MapExt<int, BindGroupLayoutEntry>> = MapExt.empty

            // TODO: StorageBuffers / Images / etc.

            for (KeyValue(_name, block)) in shader.Interface.uniformBuffers do
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

                groups <-
                    groups |> MapExt.alter block.ubSet (function
                        | Some o -> MapExt.add block.ubBinding entry o |> Some
                        | None -> MapExt.singleton block.ubBinding entry |> Some
                    )

            for (KeyValue(_name, sammy)) in shader.Interface.samplers do
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
                
                groups <-
                    groups |> MapExt.alter sammy.samplerSet (function
                        | Some o -> MapExt.add sammy.samplerBinding entry o |> Some
                        | None -> MapExt.singleton sammy.samplerBinding entry |> Some
                    )

            let groups = 
                groups |> MapExt.map (fun _ elements ->
                    let arr = 
                        elements |> MapExt.toArray |> Array.map (fun (binding, elem) ->
                            // TODO: sparse????
                            elem
                        )
                    state.device.CreateBindGroupLayout {
                        Label = null
                        Entries = arr
                    }
                )

            let layout =
                state.device.CreatePipelineLayout {
                    Label = null
                    BindGroupLayouts =
                        groups |> MapExt.toArray |> Array.map (fun (binding, elem) ->
                            // TODO: sparse????
                            elem
                        )
                }

            let (vs, fs) = shader.Acquire()


            let vbs = 
                shader.Interface.inputs |> List.toArray |> Array.map (fun par ->
                    match HashMap.tryFind par.paramSemantic vertexBuffers with
                    | Some format ->
                        {
                            ArrayStride = uint64 (VertexFormat.size format)
                            StepMode = InputStepMode.Vertex
                            Attributes =
                                [|
                                    // TODO: non-primitive type (M44f, etc.)
                                    { 
                                        Format = format
                                        Offset = 0UL
                                        ShaderLocation = par.paramLocation 
                                    }
                                |]
                        }

                    | None ->
                        match HashMap.tryFind par.paramSemantic instanceBuffers with
                        | Some format ->

                            {
                                ArrayStride = uint64 (VertexFormat.size format)
                                StepMode = InputStepMode.Instance
                                Attributes =
                                    [|
                                        // TODO: non-primitive type (M44f, etc.)
                                        { 
                                            Format = format
                                            Offset = 0UL
                                            ShaderLocation = par.paramLocation 
                                        }
                                    |]
                            }
                        | None ->
                            // TODO how to not bind attributes
                            {
                                ArrayStride = 0UL
                                StepMode = InputStepMode.Instance
                                Attributes = 
                                    [|
                                        { 
                                            Format = VertexFormat.Float4
                                            Offset = 0UL
                                            ShaderLocation = par.paramLocation 
                                        }
                                    |]
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
                
            printfn "create pipeline"
            cache <- HashMap.add top (vs, fs, pipeline) cache
            (vs, fs, pipeline)







type TraversalState =
    {
        update          : UpdateState
        shader          : option<ShaderPromise>
        vertexBuffers   : HashMap<string, BufferPromise>
        pipeline        : option<PipelinePromise>
    }


module TraversalState =     
    let empty (u : UpdateState) =
        {
            update = u
            shader = None
            vertexBuffers = HashMap.empty
            pipeline = None
        }

module Sg =
    
    type Sg = Node<TraversalState>

    type GroupComponent(e : Environment, tup) =
        inherit Component<TraversalState, SgAttributes * list<Sg>>(e, tup)

        let mutable shaderPromise : option<ShaderPromise> = None
        let mutable vertexAttributes : HashMap<string, BufferPromise> = HashMap.empty
        let mutable pipelinePromise : option<PipelinePromise> = None

        override x.Update(_prog : RenderFragment, state : TraversalState) =
            let (atts, children) = x.State

            // shader
            let state =
                match atts.shader with
                | Some e ->
                    match shaderPromise with
                    | Some o ->
                        let n = o.Update [e]
                        { state with shader = Some n }
                    | None -> 
                        let p = ShaderPromise(state.update, [e])
                        shaderPromise <- Some p
                        { state with shader = Some p }
                | None ->
                    state

            // vertexBuffers
            let state =
                if HashMap.isEmpty atts.vertexAttributes then
                    vertexAttributes <- HashMap.empty
                    state
                else
                    vertexAttributes <-
                        (atts.vertexAttributes, vertexAttributes) ||> HashMap.choose2V (fun k att prom ->
                            match att with
                            | ValueSome att ->
                                match prom with
                                | ValueSome prom ->
                                    prom.Update att |> ValueSome
                                | ValueNone ->
                                    BufferPromise(state.update, BufferUsage.CopyDst ||| BufferUsage.Vertex, att) |> ValueSome
                            | ValueNone ->
                                match prom with
                                | ValueSome _ -> ()
                                | ValueNone -> ()
                                ValueNone
                        )
                    { state with vertexBuffers = HashMap.union state.vertexBuffers vertexAttributes }

            // pipeline
            let state =
                match state.shader with
                | Some shader ->
                    match pipelinePromise with
                    | Some prom ->
                        let formats = state.vertexBuffers |> HashMap.map (fun _ b -> b.Data.Format)
                        let res = prom.Update(shader, formats, HashMap.empty)
                        pipelinePromise <- Some res
                        { state with pipeline = Some res }
                    | None ->
                        let formats = state.vertexBuffers |> HashMap.map (fun _ b -> b.Data.Format)
                        let pipe = PipelinePromise(state.update, shader, formats, HashMap.empty)
                        pipelinePromise <- Some pipe
                        { state with pipeline = Some pipe }
                | None ->
                    state

            (state, children)

    type DrawComponent(e : Environment, info : DrawInfo) =
        inherit Component<TraversalState, DrawInfo>(e, info)

        let mutable destroy = System.Collections.Generic.List<IDisposable>()

        override x.Update(prog : RenderFragment, state : TraversalState) =
            let d = destroy
            destroy <- System.Collections.Generic.List<IDisposable>()
            match state.pipeline with
            | Some p ->
                let vs, fs, pipe = p.Acquire info.mode
                destroy.Add vs
                destroy.Add fs
                destroy.Add pipe


                let buffers =
                    p.Interface.inputs |> List.toArray |> Array.choose (fun p ->
                        match HashMap.tryFind p.paramSemantic state.vertexBuffers with
                        | Some vb -> 
                            let (b,o,s) = vb.Acquire()
                            destroy.Add b
                            Some (p.paramLocation, b, o, s)
                        | None -> 
                            None
                    )

                prog.Update (fun s ->
                    s.SetPipeline pipe

                    // TODO: uniforms!!!

                    for (slot, buffer, offset, size) in buffers do
                        s.SetVertexBuffer(slot, buffer, offset, size)

                    s.Draw info
                )
            | _ ->
                failwith "no pipeline"

            for o in d do o.Dispose()
            state, []


    let draw = 
        Node.create DrawComponent

    let group = 
        let creator = Node.create GroupComponent
        fun a b -> creator(a,b)


    module Shader =
        open FShade
        type Vertex = 
            {
                [<Position>] pos : V4d
            }

        let bla (v : Vertex) =
            fragment {
                return V4d.IOOI
            }

    let testSg (pos : V3f[]) =
        let effect =
            FShade.Effect.ofFunction Shader.bla

        let top =
            { 
                SgAttributes.empty with 
                    shader = Some effect 
                    vertexAttributes = 
                        HashMap.ofList [
                            "Positions", BufferDescriptor.Data (Data.Create pos)
                        ]
            }

        group top [
            draw {
                indexed = false
                mode = PrimitiveTopology.TriangleList
                instanceCount = 1
                count = 3
                first = 0
                firstInstance = 0
                baseVertex = 0
            }
        ]
