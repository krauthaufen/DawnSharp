namespace Sg

open System
open FSharp.Data.Adaptive
open Aardvark.Base
open Microsoft.FSharp.NativeInterop
open System.Runtime.CompilerServices
open WebGPU
open System.Runtime.InteropServices

#nowarn "9"

type private EmptyMap<'a, 'b when 'a : comparison> private() =
    static let empty = MapExt.empty<'a,'b>
    static member Empty = empty


[<Struct>]
type SgAttributes =
    {
        mutable trafo                   : option<Trafo3d>
        mutable vertexAttributes        : MapExt<string, Data>
        mutable instanceAttributes      : MapExt<string, Data>
        mutable uniforms                : MapExt<string, obj>
        mutable shader                  : option<FShade.Effect>
    }

    static member Zero =
        {
            trafo = None
            vertexAttributes = EmptyMap.Empty
            instanceAttributes = EmptyMap.Empty
            uniforms = EmptyMap.Empty
            shader = None
        }

    static member inline (+) (l : SgAttributes, r : SgAttributes) =
        {
            trafo = 
                match l.trafo with 
                | Some l ->
                    match r.trafo with
                    | Some r -> Some (l * r)
                    | None -> Some l
                | None ->
                    r.trafo

            vertexAttributes = MapExt.union l.vertexAttributes r.vertexAttributes
            instanceAttributes = MapExt.union l.instanceAttributes r.instanceAttributes
            uniforms = MapExt.union l.uniforms r.uniforms
            shader =
                match r.shader with
                | Some rs -> Some rs
                | None -> l.shader
        }


module SgAttributes = 
    [<Struct>]
    type Trafo(value : Trafo3d) =
        member x.Value = value

    [<Struct>]
    type VertexAttribute(name : string, value : Data) =
        member x.Name = name
        member x.Data = value
        
    [<Struct>]
    type InstanceAttribute(name : string, value : Data) =
        member x.Name = name
        member x.Data = value
        
    [<Struct>]
    type Uniform(name : string, value : obj) =
        member x.Name = name
        member x.Value = value
        
    [<Struct>]
    type Shader(effects : list<FShade.Effect>) =
        member x.Effects = effects

module Sg =
    let trafo (t : Trafo3d) =
        SgAttributes.Trafo t

    let vertexAttribute (name : string) (data : Data) = 
        SgAttributes.VertexAttribute(name, data)

    let instanceAttribute (name : string) (data : Data) =
        SgAttributes.InstanceAttribute(name, data)

    let uniform (name : string) (value : 'a) =
        SgAttributes.Uniform(name, value :> obj)
        
    let shader (effects : list<FShade.Effect>) =
        SgAttributes.Shader(effects)
        

type PrimitiveTopology =
    | PointList = 0
    | LineList = 1
    | LineStrip = 2
    | TriangleList = 3
    | TriangleStrip = 4

type DrawInfo =
    {
        indexed : bool
        mode : PrimitiveTopology
        first : int
        count : int
        baseVertex : int
        firstInstance : int
        instanceCount : int
    }

type Sg =
    | Group of attributes : SgAttributes * children : list<Sg>
    | Draw of DrawInfo
    | Clear of sems : Map<string, C4b>

[<AutoOpen>]
module Bla = 

    type ConcList<'a> =
        | Empty
        | Single of 'a
        | Leaf of list<'a>
        | Concat of ConcList<'a> * ConcList<'a>

        member x.ToList(b : ListBuilder<'a>) =
            match x with
            | Empty -> ()
            | Single v -> b.Append v
            | Leaf l -> b.Append l
            | Concat(l, r) ->
                l.ToList b
                r.ToList b
                

        static member (+) (l : ConcList<'a>, r : ConcList<'a>) =
            match l with
            | Empty -> r
            | l ->
                match r with
                | Empty -> l
                | r -> Concat(l, r)

    type SgBuilder() =
        member inline x.Zero() =
            struct (SgAttributes.Zero, Empty)
            
        member inline x.Yield(att : SgAttributes) =
            struct (att, Empty)
            
        member inline x.Yield (t : SgAttributes.Trafo) =
            struct ({ SgAttributes.Zero with trafo = Some t.Value }, Empty)
            
        member inline x.Yield (t : SgAttributes.VertexAttribute) =
            struct ({ SgAttributes.Zero with vertexAttributes = MapExt.singleton t.Name t.Data }, Empty)

        member inline x.Yield (t : SgAttributes.InstanceAttribute) =
            struct ({ SgAttributes.Zero with instanceAttributes = MapExt.singleton t.Name t.Data }, Empty)
            
        member inline x.Yield (t : SgAttributes.Shader) =
            struct ({ SgAttributes.Zero with shader = Some (FShade.Effect.compose t.Effects) }, Empty)
            
        member inline x.Yield (t : SgAttributes.Uniform) =
            struct ({ SgAttributes.Zero with uniforms = MapExt.singleton t.Name t.Value }, Empty)
        //member inline x.Yield(att : aval<SgAttributes>) =
        //    struct (att, Empty)
        
        member inline x.For(s : seq<'a>, action : 'a -> _) =
            let mutable a = SgAttributes.Zero
            let mutable l = Empty
            for e in s do
                let struct (x,y) = action e
                a <- a + x
                l <- l + y
            struct (a,l)
            
        member inline x.Yield(sg : Sg) =
            struct (SgAttributes.Zero, Single sg)
            
        member inline x.Delay(r) = r()
        
        member inline x.Combine(struct (la, ls), struct (ra, rs)) =
            struct (la + ra, ls + rs)
                
        member inline x.Run(struct (atts, children : ConcList<Sg>)) =
            let b = ListBuilder()
            children.ToList b
            Group(atts, b.ToList())
            
    type Builder =
        val mutable public children : ListBuilder<Sg> //= ListBuilder()
        val mutable public atts : SgAttributes // = SgAttributes.Zero

        member inline x.Emit(sg : Sg) =
            x.children.Append sg
            
        member inline x.Emit(a : SgAttributes) =
            x.atts <- x.atts + a
            
        member inline x.Emit(a : SgAttributes.Trafo) =
            x.atts.trafo <- Some a.Value
            //x.atts <- { x.atts with trafo = Some a.Value }
            
        member inline x.Emit(a : SgAttributes.VertexAttribute) =
            x.atts.vertexAttributes <- MapExt.add a.Name a.Data x.atts.vertexAttributes
            //x.atts <- { x.atts with vertexAttributes = MapExt.add a.Name a.Data atts.vertexAttributes }
            
        member inline x.Emit(a : SgAttributes.InstanceAttribute) =
            x.atts.instanceAttributes <- MapExt.add a.Name a.Data x.atts.instanceAttributes
            //x.atts <- { x.atts with instanceAttributes = MapExt.add a.Name a.Data atts.instanceAttributes }
            
        member inline x.Emit(a : SgAttributes.Shader) =
            x.atts.shader <- Some (FShade.Effect.compose a.Effects)
            //atts <- { atts with shader = Some (FShade.Effect.compose a.Effects) }
            
        member inline x.Emit(a : SgAttributes.Uniform) =
            x.atts.uniforms <- MapExt.add a.Name a.Value x.atts.uniforms
            //atts <- { atts with uniforms = MapExt.add a.Name a.Value atts.uniforms }

        member inline x.Node() =
            Group(x.atts, x.children.ToList())

        new() = { atts = SgAttributes.Zero; children = ListBuilder() }

    type SgBuilder2() =

        [<ThreadStatic; DefaultValue>]
        static val mutable private _b : Builder

        static member Builder
            with get() = SgBuilder2._b
            and set v = SgBuilder2._b <- v


        member inline x.Zero() =
            ()
            
        member inline x.Yield(att : SgAttributes) =
            SgBuilder2.Builder.Emit att
            
        member inline x.Yield (t : SgAttributes.Trafo) =
            SgBuilder2.Builder.Emit t
            
        member inline x.Yield (t : SgAttributes.VertexAttribute) =
            SgBuilder2.Builder.Emit t

        member inline x.Yield (t : SgAttributes.InstanceAttribute) =
            SgBuilder2.Builder.Emit t
            
        member inline x.Yield (t : SgAttributes.Shader) =
            SgBuilder2.Builder.Emit t
            
        member inline x.Yield (t : SgAttributes.Uniform) =
            SgBuilder2.Builder.Emit t

        
        member inline x.For(s : seq<'a>, action : 'a -> _) =
            for e in s do
                action e
            
        member inline x.Yield(sg : Sg) =
            SgBuilder2.Builder.Emit sg
            
        member inline x.Delay(r) = 
            r
        
        member inline x.Combine((), r) =
            r()
                
        member inline x.Run(build) =
            let b = Builder()
            let o = SgBuilder2.Builder
            SgBuilder2.Builder <- b
            build b
            SgBuilder2.Builder <- o
            b.Node()

    let group = SgBuilder2()

module Diffing =
    open System.Collections.Generic


    [<AbstractClass>]
    type Reference<'i, 'h, 's when 's :> Reference<'i, 'h, 's>>(state : UpdateState) =
        static let mutable currentId = 0
        let id = System.Threading.Interlocked.Increment(&currentId)
        
        let mutable refCount = 0
        let mutable cache : option<'h> = None

        member x.Id = id
        override x.ToString() = sprintf "ref%d" id
        
        override x.GetHashCode() = id
        override x.Equals o =
            match o with
            | :? Reference<'i, 'h, 's> as o -> o.Id = id
            | _ -> false

        abstract member Destroy : 'h -> unit
        abstract member Create : unit -> 'h
        abstract member Update : 'i -> 's

        member x.Dispose() =
            refCount <- 0
            match cache with
            | Some h -> 
                x.Destroy h
                cache <- None
            | None -> 
                ()

        interface IDisposable with
            member x.Dispose() = x.Dispose()

        member x.Destroy() =
            lock x (fun () ->
                refCount <- 0
                state.destroy.Add x |> ignore
            )

        member x.Acquire() =
            lock x (fun () ->
                refCount <- refCount + 1

                match cache with
                | None ->
                    let handle = x.Create()
                    cache <- Some handle
                    handle
                | Some store ->
                    store
            )

        member x.Release() =
            lock x (fun () ->
                refCount <- refCount - 1
                if refCount = 0 then
                    state.destroy.Add x |> ignore
            )

    and UpdateState =
        {
            outputs         : Map<string, (Type * int)>
            device          : Device
            destroy         : HashSet<IDisposable>
        }

    type BufferReference(state : UpdateState, usage : BufferUsage, data : Data) =
        inherit Reference<Data, Buffer, BufferReference>(state)

        member x.Usage = usage
        member x.Size = data.Size
        member x.Format = data.Format

        override x.Update(newData : Data) =
            if data = newData then
                x
            else    
                new BufferReference(state, usage, newData)

        override x.Destroy(handle : Buffer) =
            printfn "delete buffer %d" x.Id
            handle.Destroy()
            handle.Dispose()

        override x.Create() = 
            printfn "create buffer %d" x.Id
            let handle = 
                state.device.CreateBuffer {
                    Label = null
                    Size = uint64 data.Size
                    Usage = usage
                    MappedAtCreation = false
                }

            handle.Upload data
            handle

    type ShaderReference(state : UpdateState, effect : FShade.Effect) =
        inherit Reference<FShade.Effect, ShaderModule * ShaderModule, ShaderReference>(state)

        let glsl = 
            let cfg : FShade.EffectConfig =
                {
                    FShade.EffectConfig.depthRange = Range1d(0.0, 1.0)
                    FShade.EffectConfig.flipHandedness = false
                    FShade.EffectConfig.lastStage = FShade.ShaderStage.Fragment
                    FShade.EffectConfig.outputs = state.outputs
                }

            let backend = FShade.Backends.glsl430
            effect
            |> FShade.Effect.toModule cfg
            |> FShade.Imperative.ModuleCompiler.compile backend
            |> FShade.GLSL.Assembler.assemble backend


        member x.Interface = glsl.iface

        override x.Update(newShader : FShade.Effect) =
            if effect = newShader then
                x
            else    
                new ShaderReference(state, newShader)

        override x.Destroy((vs : ShaderModule, fs : ShaderModule)) =
            printfn "delete shader %d" x.Id
            vs.Dispose()
            fs.Dispose()

        override x.Create() = 
            printfn "create shader %d" x.Id

            printfn "%s" glsl.code

            let vs = 
                state.device.CreateGLSLShaderModule {
                    Label       = null
                    EntryPoint  = "main"
                    ShaderStage = ShaderStage.Vertex
                    Defines     = ["Vertex"]
                    Code        = glsl.code
                }

            let fs = 
                state.device.CreateGLSLShaderModule {
                    Label       = null
                    EntryPoint  = "main"
                    ShaderStage = ShaderStage.Fragment
                    Defines     = ["Fragment"]
                    Code        = glsl.code
                }

            (vs, fs)


    type TraversalState =
        {   
            vertexBuffers       : MapExt<string, BufferReference>
            instanceBuffers     : MapExt<string, BufferReference>
            shader              : option<ShaderReference>
        }

    type BufferUpdater(state : UpdateState, usage : BufferUsage) =
        let mutable buffers : MapExt<string, BufferReference> = MapExt.empty

        member x.Destroy() =
            for (_,b) in MapExt.toSeq buffers do
                b.Destroy()
            buffers <- MapExt.empty

        member x.Update(ts : TraversalState, newData : MapExt<string, Data>) =
            let newBuffers = 
                (buffers, newData) ||> MapExt.choose2V (fun k b d ->
                    match b with
                    | ValueSome buffer ->
                        match d with
                        | ValueSome data ->
                            ValueSome (buffer.Update data)
                        | ValueNone ->
                            buffer.Destroy()
                            ValueNone
                    | ValueNone ->
                        match d with
                        | ValueSome data ->
                            new BufferReference(state, usage, data) |> ValueSome
                        | ValueNone ->
                            ValueNone
                )
            buffers <- newBuffers

            if usage &&& BufferUsage.Vertex <> BufferUsage.None then { ts with vertexBuffers = MapExt.union ts.vertexBuffers newBuffers }
            else { ts with instanceBuffers = MapExt.union ts.vertexBuffers newBuffers }

    type ShaderUpdater(state : UpdateState) =
        let mutable currentShader : option<ShaderReference> = None
        
        member x.Update(ts : TraversalState, newEffect : option<FShade.Effect>) =
            match newEffect with
            | Some shader ->
                match currentShader with
                | Some s -> 
                    let n = s.Update(shader) 
                    currentShader <- Some n
                    { ts with shader = Some n }
                | None ->
                    let n = new ShaderReference(state, shader) 
                    currentShader <- Some n
                    { ts with shader = Some n }
            | None ->
                match currentShader with
                | Some s ->
                    s.Destroy()
                    currentShader <- None
                    ts
                | None ->
                    ts

    type SgUpdater(state : UpdateState) =
        let vb = BufferUpdater(state, BufferUsage.CopyDst ||| BufferUsage.CopySrc ||| BufferUsage.Vertex)
        let ib = BufferUpdater(state, BufferUsage.CopyDst ||| BufferUsage.CopySrc ||| BufferUsage.Vertex)
        let s = ShaderUpdater(state)

        let mutable current : list<SgUpdater> = []

        let mutable clean = []

        let acquire (b : Reference<_,_,_>) =
            clean <- b.Release :: clean
            b.Acquire()

        member x.Destroy() =
            vb.Destroy()
            ib.Destroy()
            current |> List.iter (fun u -> u.Destroy())
            current <- []
            clean |> List.iter (fun f -> f())

        member x.Update(value : Sg, ts : TraversalState) : unit =
            match value with
            | Group(atts, children) ->
                let mutable ts = ts
                ts <- vb.Update(ts, atts.vertexAttributes)
                ts <- ib.Update(ts, atts.instanceAttributes)
                ts <- s.Update(ts, atts.shader)

                let traversalState = ts

                let rec run (nodes : list<SgUpdater>) (children : list<Sg>) =
                    match nodes with
                    | hn :: tn ->
                        match children with
                        | hc :: tc ->
                            hn.Update(hc, traversalState)
                            hn :: run tn tc
                        | [] ->
                            hn.Destroy()
                            tn |> List.iter (fun t -> t.Destroy())
                            []
                    | [] ->
                        match children with
                        | h :: t ->
                            let n = SgUpdater(state)
                            n.Update(h, traversalState)
                            n :: run [] t
                        | [] ->
                            []
                            

                current <- run current children

            | Draw info ->
                let destroyOld = clean
                clean <- []

                match ts.shader with
                | Some shader ->
                    let iface = shader.Interface
                    let vs, fs = acquire shader

                    let inputs = 
                        iface.inputs |> List.choose (fun p ->
                            match MapExt.tryFind p.paramSemantic ts.vertexBuffers with
                            | Some vb -> 
                                Some (p.paramLocation, acquire vb)
                            | None ->
                                match MapExt.tryFind p.paramSemantic ts.instanceBuffers with
                                | Some vb -> 
                                    Some (p.paramLocation, acquire vb)
                                | None ->
                                    printfn "bad sem : %A" p.paramSemantic
                                    None
                        )


                    printfn "draw(%A)" inputs
                | None ->
                    printfn "bad"

                for d in destroyOld do
                    d()

                ()




    [<AbstractClass>]
    type Component<'a>() =
        abstract member Mount : unit -> unit
        abstract member Unmount : unit -> unit
        abstract member Render : TraversalState * RenderPassEncoder * 'a -> unit

    type Sg =
        abstract member Visit : SgVisitor<'r> -> 'r

    and Sg<'a>(c : (UpdateState -> Component<'a>), value : 'a) =
        interface Sg with
            member x.Visit v = v.Accept x
            
        member x.Component = c
        member x.Value = value

    and SgVisitor<'r> =
        abstract member Accept : Sg<'a> -> 'r

    //type Component<'a> with
    //    member x.New(value : 'a) =
    //        Sg(x, value) :> Sg


    let groupComponent :  UpdateState -> Component<struct (SgAttributes * list<Sg>)> = failwith ""
    let drawComponent : UpdateState -> Component<DrawInfo> = failwith ""
    let hansComponent : UpdateState -> Component<string> = failwith ""

    let Group (atts : SgAttributes) (children : list<Sg>) =
        Sg(groupComponent, struct(atts, children)) :> Sg
        
    let Render draw =
        Sg(drawComponent, draw)

        
    let Hans draw =
        Sg(hansComponent, draw)


    let sg =
        Group SgAttributes.Zero [
            Group SgAttributes.Zero [
                Hans "asdasd"

                Render {
                    indexed = false
                    mode = PrimitiveTopology.LineList
                    first = 0
                    count = 2
                    firstInstance = 0
                    instanceCount = 1
                    baseVertex = 0
                }
                        
                Render {
                    indexed = false
                    mode = PrimitiveTopology.LineList
                    first = 0
                    count = 2
                    firstInstance = 0
                    instanceCount = 1
                    baseVertex = 0
                }
            ]
        ]






module Hans =

    type Object =
        {
            id : int
            trafo : Trafo3d
        }

    type Model =
        {
            trafo : Trafo3d
            datas : Data[]
            colors : Data
            count : int
        }

    let simpleScene (model : Model) =   
        group {
            Sg.shader []
            Sg.trafo model.trafo
            Sg.uniform "Color" C4b.Red
            Sg.uniform "Color1" C4b.Red
            Sg.uniform "Color2" C4b.Red
            Sg.uniform "Color3" C4b.Red
            Sg.uniform "Color4" C4b.Red
            
            Sg.vertexAttribute "Colors" model.colors
            // list<Object>
            // alist<Object>
            // list<AdaptiveObject>
            // alist<AdaptiveObject>
            for id in 0 .. model.count - 1 do
                group {
                    Sg.vertexAttribute "Positions" model.datas.[id]
                    Sg.uniform "Index" id
                    Draw {
                        indexed = false
                        mode = PrimitiveTopology.LineList
                        first = 0
                        count = 2
                        firstInstance = 0
                        instanceCount = 1
                        baseVertex = 0
                    }
                }
        }

    type Delta<'a> = 
        | Update of index : Index * srcIndex : Index * oldValue : 'a * newValue : 'a
        | New of index : Index * value : 'a
        | Remove of index : Index

    let computeDelta (editDistance : 'a -> 'a -> int) (store : IndexList<'a>) (value : list<'a>) =

        let rec run (l : Index) (store : IndexList<'a>) (value : list<'a>) =    
            
            if store.IsEmpty then
                let mutable ii = l
                value |> List.map (fun v ->
                    let id = Index.after ii
                    ii <- id
                    New(id, v)
                )
            else
                match value with
                | v0 :: vs ->
                    let mutable minDist = Int32.MaxValue
                    let mutable minValue = Unchecked.defaultof<_>
                    let mutable minIndex = Index.zero
                    for (i, si) in IndexList.toSeqIndexed store do
                        let dist = editDistance si v0
                        if dist < minDist then 
                            minDist <- dist
                            minIndex <- i
                            minValue <- si

                    if minDist = 0 && minIndex <> Index.zero then
                        if minDist = 0 then
                            run minIndex (IndexList.remove minIndex store) vs
                        else
                            let id = Index.between l store.MinIndex
                            Update(id, minIndex, minValue, v0) :: run minIndex (IndexList.remove minIndex store) vs
                    else
                        let id = Index.between l store.MinIndex
                        New(id, v0) :: run id store vs
                        //New v0 :: run l store vs
                | [] -> 
                    let bb = ListBuilder()
                    for (i, e) in IndexList.toSeqIndexed store do
                        bb.Append(Remove i)
                    bb.ToList()


        run Index.zero store value

    let rec applyDelta (l : IndexList<'a>) (deltas : list<Delta<'a>>) =
        match deltas with
        | [] -> l
        | h :: t ->
            let l = 
                match h with
                | Update(idx, srcIndex, oldValue, newValue) ->
                    printfn "update %A -> %A" oldValue newValue
                    l.Remove(srcIndex).Set(idx, newValue)
                | New(idx, value) ->
                    printfn "new %A" value
                    l.Set(idx, value)
                | Remove idx ->
                    l.Remove idx
            applyDelta l t




    let test (dev : Device) =
        let dist (a : int) (b : int) = abs (a - b)

        let mutable l = IndexList.empty
        let mutable delta = computeDelta dist l [1;2;3]
        l <- applyDelta l delta
        printfn "[] -> [1;2;3]"
        printfn "  %A" delta
        printfn "  %A" (IndexList.toArray l)

        printfn "[1;2;3] -> [0;1;2;3]"
        delta <- computeDelta dist l [0;1;2;3]
        l <- applyDelta l delta
        printfn "  %A" delta
        printfn "  %A" (IndexList.toArray l)
        exit 0

        let model =
            {
                trafo = Trafo3d.Identity
                datas = Array.init 10000 (fun _ -> Data.Create [| V3f.Zero |])
                colors = Data.Create [| C4b.Red |]
                count = 10
            }

        let state = { Diffing.device = dev; Diffing.destroy = System.Collections.Generic.HashSet(); Diffing.outputs = Map.ofList ["Colors", (typeof<V4d>, 0) ] }
        let ts = { Diffing.vertexBuffers = MapExt.empty; Diffing.instanceBuffers = MapExt.empty; Diffing.shader = None }
        let b = Diffing.SgUpdater state
        

        let update (scene : Sg) =
            b.Update(scene, ts)
            state.destroy |> Seq.iter (fun d -> d.Dispose())
            state.destroy.Clear()


        printfn "10"
        printfn ""
        update(simpleScene { model with count = 10 })
        printfn "11"
        printfn ""
        update(simpleScene { model with count = 11 })
        printfn "9"
        printfn ""
        update(simpleScene { model with count = 9 })
        printfn "0"
        printfn ""
        update(simpleScene { model with count = 0 })
        //exit 0

        for size in [1000; 2000; 3000; 10000] do    
            let model = { model with count = size }
            printfn "%d: " size
            let mutable last = Unchecked.defaultof<_>
            let mutable iter = 0
            let sw = System.Diagnostics.Stopwatch()



            for i in 1 .. 100 do
                simpleScene model |> ignore

            sw.Start()
            while sw.Elapsed.TotalSeconds < 1.0 do
                last <- simpleScene model
                iter <- iter + 1
            sw.Stop()

            let cnt = Fun.NextPowerOfTwo (5 * iter)
            printfn "  running: %d" cnt
            iter <- 0

            sw.Restart()
            while iter < cnt do
                last <- simpleScene model
                iter <- iter + 1
            sw.Stop()

            printfn "  %A %.2f/s (%d)" (sw.MicroTime / iter) (float iter / sw.Elapsed.TotalSeconds) (clamp 0 0 (Unchecked.hash last) + iter)



module ObjAlgebra = 
    type Bal<'a,'r,'s> = 
        abstract Leaf : 's * 'a -> 'r
        abstract Bin : 's * 'r * 'r -> 'r
        
    type Alg<'a,'r> = 
        abstract member Cons : int * 'a * 'r -> 'r
        abstract member Nil : int -> 'r
        abstract member Concat : int * 'r * 'r -> 'r


    type Sum() =
        interface Alg<int,int> with
            member x.Cons(_,a,b) = a + b
            member x.Concat(_,a,b) = a + b
            member x.Nil(_) = 0

    type MaxDepth() =
        interface Alg<int,int> with
            member x.Cons(d,a,b) = b
            member x.Concat(d,a,b) = max a b
            member x.Nil(c) = c


    module Patterns =
        open System.Reflection
        open Aardvark.Base.IL

        let isRange : seq<int> -> V3i =


            let rangeType = (seq { 1 .. 10 }).GetType()
            let rangeStepType = (seq { 1 .. 10 .. 100 }).GetType()

            let rangeStart = rangeType.GetField("start", BindingFlags.Public ||| BindingFlags.NonPublic ||| BindingFlags.Instance)
            let rangeStop = rangeType.GetField("stop", BindingFlags.Public ||| BindingFlags.NonPublic ||| BindingFlags.Instance)
        
            let rangeStepStart = rangeStepType.GetField("start", BindingFlags.Public ||| BindingFlags.NonPublic ||| BindingFlags.Instance)
            let rangeStepStep = rangeStepType.GetField("step", BindingFlags.Public ||| BindingFlags.NonPublic ||| BindingFlags.Instance)
            let rangeStepStop = rangeStepType.GetField("stop", BindingFlags.Public ||| BindingFlags.NonPublic ||| BindingFlags.Instance)

            let ctor = typeof<V3i>.GetConstructor [| typeof<int>; typeof<int>; typeof<int> |]

            let notRange = Label()
            let notRangeStep = Label()

            cil {
                do! IL.ldarg 0
                do! IL.isinst rangeType
                do! IL.jmp JumpCondition.False notRange

                do! IL.ldarg 0
                do! IL.ldfld rangeStart
                do! IL.ldconst 1
                do! IL.ldarg 0
                do! IL.ldfld rangeStop
                do! IL.newobj ctor
                do! IL.ret

                do! IL.mark notRange

                do! IL.ldarg 0
                do! IL.isinst rangeStepType
                do! IL.jmp JumpCondition.False notRangeStep
            
                do! IL.ldarg 0
                do! IL.ldfld rangeStepStart
                do! IL.ldarg 0
                do! IL.ldfld rangeStepStep
                do! IL.ldarg 0
                do! IL.ldfld rangeStepStop
                do! IL.newobj ctor
                do! IL.ret
            
                do! IL.mark notRangeStep
                do! IL.ldconst 0
                do! IL.ldconst 0
                do! IL.ldconst 0
                do! IL.newobj ctor
                do! IL.ret


            }


    open System.Collections.Generic

    type Builder<'a, 'r> = 
        val mutable public a : Alg<'a, 'r>

        val mutable public depth : int

        member inline x.Delay(action : unit -> 'r) = action
    
        member inline x.Bind(v : 'a, cont : unit -> 'r) =
            let old = x.depth
            x.depth <- x.depth + 1
            let rest = cont()
            x.depth <- old
            x.a.Cons(x.depth,v, rest)

        member inline x.Return(()) =
            x.a.Nil(x.depth)
        
        member inline x.Zero() =
            x.a.Nil(x.depth)
         
        member inline x.For(elems : seq<'x>, action : 'x -> 'r) =
            //if Object.ReferenceEquals(typeof<'x>, typeof<int>) then
            //    let range = Patterns.isRange (unbox elems)
            //    if range.Y = 1 then
            //        let action = unbox<int -> 'r> action
            //        let mutable res = x.a.Nil()
            //        for i in range.X .. range.Z do
            //            res <- x.a.Concat(res, action i)
            //        res
            //    elif range.Y <> 0 then
            //        let action = unbox<int -> 'r> action
            //        let mutable res = x.a.Nil()
            //        let e = range.Z + 1
            //        for i in range.X .. range.Y .. range.Z do
            //            res <- x.a.Concat(res, action i)
            //        res
            //    else
            //        let mutable res = x.a.Nil()
            //        for e in elems do
            //            let er = action e
            //            res <- x.a.Concat(res, er)
            //        res
            //else
                let mutable res = x.a.Nil(x.depth)
                for e in elems do
                    let old = x.depth
                    x.depth <- old + 1
                    let er = action e
                    x.depth <- old
                    res <- x.a.Concat(x.depth,res, er)
                res

            //let mutable res = x.a.Nil()
            //let e = elems.GetEnumerator()
            //while e.MoveNext() do
            //    res <- x.a.Concat(res, action e.Current)
            //e.Dispose()
            //res


        
        member inline x.While(guard : unit -> bool, action : unit -> 'r) =
            let mutable res = x.a.Nil(x.depth)
            while guard() do
                let er = action()
                res <- x.a.Concat(x.depth,res, er)
            res

        member inline x.Combine(l : 'r, r : unit -> 'r) =
            let (l) = l
            let (r) = r()
            x.a.Concat(x.depth, l, r)
        
        member inline x.Run(action : unit -> 'r) = 
            let r = action()
            r

        new(a : Alg<_,_>) = { a = a; depth = 0 }

    module Gah =    

        let sepp = Builder<int, int>(MaxDepth())

        let sum = Sum() :> Alg<_,_>
    

        let z = 10000000
        let test (alg : Alg<int,'r>) = 
            let a = alg.Cons(0,1,alg.Cons(1,2,alg.Nil(2)))
            a
     
        [<MethodImpl(MethodImplOptions.NoInlining)>]
        let builder (z : int) =
            sepp {
                do! 2
                //do! 3
                do! 2
                do! sepp { 
                    do! 9 
                    do! 77 
                }
                do! 100

            }
        
        [<MethodImpl(MethodImplOptions.NoInlining)>]
        let optimized (z : int) =
            let mutable sum = 0
            for i in 1 .. z do sum <- sum + i
            sum

    
        [<MethodImpl(MethodImplOptions.NoInlining)>]
        let alg (z : int) =
            let mutable s = sum.Nil(0)
            for i in 1 .. z do s <- sum.Concat(0,s, sum.Cons(1,i, sum.Nil(2)))
            s

        open System.Diagnostics
        let bench() =

            builder 10 |> printfn "%A"
            exit 0

            for e in 16 .. 30 do
                let z = 1 <<< e

                printfn "%d" z
                let iter = 10
                // warmup
                for i in 1 .. 10 do builder z |> ignore
                for i in 1 .. 10 do optimized z |> ignore
                for i in 1 .. 10 do alg z |> ignore

                let sw = Stopwatch.StartNew()
                let mutable res = 0
                for i in 1 .. iter do
                    res <- res + optimized z
                sw.Stop()
                printfn "  opt:     %A (%d)" (sw.MicroTime / iter) res
            
                let sw = Stopwatch.StartNew()
                let mutable res = 0
                for i in 1 .. iter do
                    res <- res + alg z
                sw.Stop()
                printfn "  alg:     %A (%d)" (sw.MicroTime / iter) res
            
                let sw = Stopwatch.StartNew()
                let mutable res = 0
                for i in 1 .. iter do
                    res <- res + builder z
                sw.Stop()
                printfn "  builder: %A (%d)" (sw.MicroTime / iter) res
            




