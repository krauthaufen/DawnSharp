namespace Armadillo

open FSharp.Data.Adaptive
open Aardvark.Base
open WebGPU

#nowarn "9"


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

[<Struct>]
type BlendMode =
    {
        color       : BlendDescriptor
        alpha       : BlendDescriptor
        blendColor  : C4f
    }

    static member Default = { color = BlendDescriptor.Default; alpha = BlendDescriptor.Default; blendColor = C4f.Black }

type DepthTestMode =
    {
        compare     : CompareFunction
        write       : bool
    }

type StencilMode =
    {
        readMask    : uint32
        writeMask   : uint32
        front       : StencilStateFaceDescriptor
        back        : StencilStateFaceDescriptor
    }

type BufferDescriptor =
    | Data of Data
    | Buffer of fmt : VertexFormat * buffer : Buffer * offset : nativeint * size : nativeint

    //member x.IsBuffer =
    //    match x with
    //    | Buffer _ -> true
    //    | _ -> false

    member x.Size =
        match x with
        | Data d -> d.Size
        | Buffer(_, _, _, s) -> s

    member x.Format =
        match x with
        | Data d -> d.Format
        | Buffer(f,_,_,_) -> f

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

type Sg =
    | Render of info : DrawInfo
    | RenderIndirect of count : int
    | Group of attributes : SgAttributes * children : list<Sg>






module Program = 
    open System.Collections.Generic
    open System.Runtime.InteropServices
    open Aardvark.Base.Runtime
    open Microsoft.FSharp.NativeInterop


    type IRenderStream =
        abstract member Pinning : IAdaptivePinning
        abstract member Assembler : IAssemblerStream
        abstract member SetPipeline : pipeline : RenderPipeline -> unit
        abstract member SetBindGroup : groupIndex : int * group : BindGroup * dynamicOffsets : array<int> -> unit
        abstract member Draw : draw : DrawInfo -> unit
        abstract member SetVertexBuffer : slot : int * buffer : Buffer * offset : uint64 * size : uint64 -> unit
        abstract member DrawIndirect : indexed : bool * indirect : Buffer * offset : uint64 -> unit
        abstract member ExecuteBundles : bundles : RenderBundle[] -> unit
        abstract member InsertDebugMarker : marker : string -> unit
        abstract member PopDebugGroup : unit -> unit
        abstract member PushDebugGroup : label : string -> unit
        abstract member SetStencilReference : ref : uint32 -> unit
        abstract member SetBlendColor : color : C4f -> unit
        abstract member SetViewport : offset : V2f * size : V2f * depthRange : Range1f -> unit
        abstract member SetScissorRect : offset : V2i * size : V2i -> unit
        abstract member SetIndexBuffer : buffer : Buffer * format : IndexFormat * offset : uint64 * size : uint64 -> unit
        abstract member WriteTimestamp : querySet : QuerySet * queryIndex : int -> unit
        abstract member EndPass : unit -> unit

    type NativeRenderPassEncoderStream(ass : IAssemblerStream, pinning : IAdaptivePinning, encoder : nativeptr<RenderPassEncoderHandle>) =
        static let lib = NativeLibrary.Load("dawn", typeof<Device>.Assembly, Unchecked.defaultof<_>)
        static let wgpuRenderPassEncoderSetPipeline = NativeLibrary.GetExport(lib, "wgpuRenderPassEncoderSetPipeline")
        static let wgpuRenderPassEncoderDraw = NativeLibrary.GetExport(lib, "wgpuRenderPassEncoderDraw")
        static let wgpuRenderPassEncoderDrawIndexed = NativeLibrary.GetExport(lib, "wgpuRenderPassEncoderDrawIndexed")
        static let wgpuRenderPassEncoderSetVertexBuffer = NativeLibrary.GetExport(lib, "wgpuRenderPassEncoderSetVertexBuffer")
        static let wgpuRenderPassEncoderSetBindGroup = NativeLibrary.GetExport(lib, "wgpuRenderPassEncoderSetBindGroup")
        static let wgpuRenderPassEncoderDrawIndirect = NativeLibrary.GetExport(lib, "wgpuRenderPassEncoderDrawIndirect")
        static let wgpuRenderPassEncoderDrawIndexedIndirect = NativeLibrary.GetExport(lib, "wgpuRenderPassEncoderDrawIndexedIndirect")
        static let wgpuRenderPassEncoderExecuteBundles = NativeLibrary.GetExport(lib, "wgpuRenderPassEncoderExecuteBundles")
        static let wgpuRenderPassEncoderInsertDebugMarker = NativeLibrary.GetExport(lib, "wgpuRenderPassEncoderInsertDebugMarker")
        static let wgpuRenderPassEncoderPopDebugGroup = NativeLibrary.GetExport(lib, "wgpuRenderPassEncoderPopDebugGroup")
        static let wgpuRenderPassEncoderPushDebugGroup = NativeLibrary.GetExport(lib, "wgpuRenderPassEncoderPushDebugGroup")
        static let wgpuRenderPassEncoderSetStencilReference = NativeLibrary.GetExport(lib, "wgpuRenderPassEncoderSetStencilReference")
        static let wgpuRenderPassEncoderSetBlendColor = NativeLibrary.GetExport(lib, "wgpuRenderPassEncoderSetBlendColor")
        static let wgpuRenderPassEncoderSetViewport = NativeLibrary.GetExport(lib, "wgpuRenderPassEncoderSetViewport")
        static let wgpuRenderPassEncoderSetScissorRect = NativeLibrary.GetExport(lib, "wgpuRenderPassEncoderSetScissorRect")
        static let wgpuRenderPassEncoderSetIndexBufferWithFormat = NativeLibrary.GetExport(lib, "wgpuRenderPassEncoderSetIndexBufferWithFormat")
        static let wgpuRenderPassEncoderWriteTimestamp = NativeLibrary.GetExport(lib, "wgpuRenderPassEncoderWriteTimestamp")
        static let wgpuRenderPassEncoderEndPass = NativeLibrary.GetExport(lib, "wgpuRenderPassEncoderEndPass")

        interface IRenderStream with
            member x.Pinning = pinning
            member x.Assembler = ass

            member x.SetPipeline(p : RenderPipeline) =
                ass.BeginCall(2)
                ass.PushArg p.Handle.Handle
                ass.PushPtrArg (NativePtr.toNativeInt encoder)
                ass.Call wgpuRenderPassEncoderSetPipeline

            member x.SetBindGroup(groupIndex : int, group : BindGroup, dynamicOffsets : int[]) =
                let struct(pOffsets, offsetCount) = 
                    if isNull dynamicOffsets then struct (NativePtr.zero, 0)
                    else struct(pinning.Pin dynamicOffsets, dynamicOffsets.Length)
                ass.BeginCall(5)
                ass.PushArg (NativePtr.toNativeInt pOffsets)
                ass.PushArg offsetCount
                ass.PushArg (group.Handle.Handle)
                ass.PushArg groupIndex
                ass.PushPtrArg (NativePtr.toNativeInt encoder)
                ass.Call wgpuRenderPassEncoderSetBindGroup

            member x.Draw(info : DrawInfo) =
                if info.indexed then
                    ass.BeginCall(6)
                    ass.PushArg info.firstInstance
                    ass.PushArg info.baseVertex
                    ass.PushArg info.first
                    ass.PushArg info.instanceCount
                    ass.PushArg info.count
                    ass.PushPtrArg (NativePtr.toNativeInt encoder)
                    ass.Call wgpuRenderPassEncoderDrawIndexed
                else
                    ass.BeginCall(5)
                    ass.PushArg info.firstInstance
                    ass.PushArg info.first
                    ass.PushArg info.instanceCount
                    ass.PushArg info.count
                    ass.PushPtrArg (NativePtr.toNativeInt encoder)
                    ass.Call wgpuRenderPassEncoderDraw

            member x.SetVertexBuffer(slot : int, buffer : Buffer, offset : uint64, size : uint64) =
                //let a = DawnRaw.wgpuRenderPassEncoderSetVertexBuffer
                ass.BeginCall(5)
                ass.PushArg (nativeint size)
                ass.PushArg (nativeint offset)
                ass.PushArg buffer.Handle.Handle
                ass.PushArg slot
                ass.PushPtrArg (NativePtr.toNativeInt encoder)
                ass.Call wgpuRenderPassEncoderSetVertexBuffer

            member x.DrawIndirect(indexed : bool, indirect : Buffer, offset : uint64) =
                if indexed then
                    ass.BeginCall(3)
                    ass.PushArg (nativeint offset)
                    ass.PushArg indirect.Handle.Handle
                    ass.PushPtrArg (NativePtr.toNativeInt encoder)
                    ass.Call wgpuRenderPassEncoderDrawIndexedIndirect
                else
                    ass.BeginCall(3)
                    ass.PushArg (nativeint offset)
                    ass.PushArg indirect.Handle.Handle
                    ass.PushPtrArg (NativePtr.toNativeInt encoder)
                    ass.Call wgpuRenderPassEncoderDrawIndirect
 
            member x.ExecuteBundles(bundles : RenderBundle[]) =
                if not (isNull bundles) && bundles.Length > 0 then
                    let handles = bundles |> Array.map (fun b -> b.Handle)
                    let pHandles = pinning.Pin(handles)
                    ass.BeginCall(3)
                    ass.PushArg (NativePtr.toNativeInt pHandles)
                    ass.PushArg (handles.Length)
                    ass.PushPtrArg (NativePtr.toNativeInt encoder)
                    ass.Call wgpuRenderPassEncoderExecuteBundles
 
            member x.InsertDebugMarker (marker : string) =
                let pStr =
                    if isNull marker then
                        NativePtr.zero
                    else
                        let arr = Array.zeroCreate (marker.Length + 1)
                        System.Text.Encoding.ASCII.GetBytes(marker, 0, marker.Length, arr, 0) |> ignore
                        pinning.Pin arr
                ass.BeginCall(2)
                ass.PushArg (NativePtr.toNativeInt pStr)
                ass.PushPtrArg (NativePtr.toNativeInt encoder)
                ass.Call wgpuRenderPassEncoderInsertDebugMarker

            member x.PopDebugGroup() =
                ass.BeginCall(1)
                ass.PushPtrArg (NativePtr.toNativeInt encoder)
                ass.Call wgpuRenderPassEncoderPopDebugGroup
                
            member x.PushDebugGroup(label : string) =
                let pStr = 
                    if isNull label then
                        NativePtr.zero
                    else
                        let arr = Array.zeroCreate (label.Length + 1)
                        System.Text.Encoding.ASCII.GetBytes(label, 0, label.Length, arr, 0) |> ignore
                        pinning.Pin arr
                ass.BeginCall(2)
                ass.PushArg (NativePtr.toNativeInt pStr)
                ass.PushPtrArg (NativePtr.toNativeInt encoder)
                ass.Call wgpuRenderPassEncoderPushDebugGroup
                
            member x.SetStencilReference (value : uint32) =
                ass.BeginCall(2)
                ass.PushArg (int value)
                ass.PushPtrArg (NativePtr.toNativeInt encoder)
                ass.Call wgpuRenderPassEncoderSetStencilReference
                
            member x.SetBlendColor(color : C4f) =
                let pColor = pinning.Pin [| { R = color.R; G = color.G; B = color.B; A = color.A } |]
                ass.BeginCall(2)
                ass.PushArg (NativePtr.toNativeInt pColor)
                ass.PushPtrArg (NativePtr.toNativeInt encoder)
                ass.Call wgpuRenderPassEncoderSetBlendColor

            member x.SetViewport(offset : V2f, size : V2f, depthRange : Range1f) =
                ass.BeginCall(7)
                ass.PushArg depthRange.Max
                ass.PushArg depthRange.Min
                ass.PushArg size.Y
                ass.PushArg size.X
                ass.PushArg offset.Y
                ass.PushArg offset.X
                ass.PushPtrArg (NativePtr.toNativeInt encoder)
                ass.Call wgpuRenderPassEncoderSetViewport

            member x.SetScissorRect(offset : V2i, size : V2i) =
                ass.BeginCall(5)
                ass.PushArg size.Y
                ass.PushArg size.X
                ass.PushArg offset.Y
                ass.PushArg offset.X
                ass.PushPtrArg (NativePtr.toNativeInt encoder)
                ass.Call wgpuRenderPassEncoderSetScissorRect

            member x.SetIndexBuffer(buffer : Buffer, format : IndexFormat, offset : uint64, size : uint64) =
                ass.BeginCall(5)
                ass.PushArg (nativeint size)
                ass.PushArg (nativeint offset)
                ass.PushArg (int format)
                ass.PushArg (buffer.Handle.Handle)
                ass.PushPtrArg (NativePtr.toNativeInt encoder)
                ass.Call wgpuRenderPassEncoderSetIndexBufferWithFormat

            member x.WriteTimestamp(querySet : QuerySet, queryIndex : int) =
                ass.BeginCall(3)
                ass.PushArg queryIndex
                ass.PushArg querySet.Handle.Handle
                ass.PushPtrArg (NativePtr.toNativeInt encoder)
                ass.Call wgpuRenderPassEncoderWriteTimestamp

            member x.EndPass() =    
                ass.BeginCall(1)
                ass.PushPtrArg (NativePtr.toNativeInt encoder)
                ass.Call wgpuRenderPassEncoderEndPass

    module Bla = 



        [<AllowNullLiteral>]
        type Block(parent : Block, env : FragmentProgram, wrap : IAssemblerStream -> IAdaptivePinning -> IRenderStream) =
            let mutable next : Block = null
            let mutable prev : Block = null

            let mutable fragment : ProgramFragment = null

            let mutable first : Block = null
            let mutable last : Block = null
            
            member x.Fragment
                with get() = fragment
                and set p = fragment <- p
                
            member x.Prev
                with get() = prev
                and set p = prev <- p
                
            member x.Next
                with get() = next
                and set p = next <- p
                
            member x.First
                with get() = first
                and set p = first <- p
                
            member x.Last
                with get() = last
                and set p = last <- p


            member x.LastFragmentInPrev =
                if isNull prev then
                    if isNull parent then null
                    else parent.LastFragmentInPrev
                else
                    prev.LastFragment

            member x.FirstFragmentInNext =
                if isNull next then
                    if isNull parent then null
                    else parent.FirstFragmentInNext
                else
                    next.FirstFragment

            member x.LastFragment =
                if isNull fragment then
                    if isNull prev then parent.LastFragmentInPrev
                    else prev.LastFragment
                else
                    fragment
               
            member x.FirstFragment =
                if isNull fragment then
                    if isNull next then parent.FirstFragmentInNext
                    else next.FirstFragment
                else
                    fragment
   
            member x.Dispose() =
                if not (isNull fragment) then 
                    fragment.Dispose()
                    fragment <- null

                if not (isNull first) then
                    let l = first.Prev
                    let r = last.Next

                    if isNull l then ()
                    else l.Next <- r
                    
                    if isNull r then ()
                    else r.Prev <- l

                    first.Prev <- null
                    last.Next <- null

                    let mutable c = first
                    while not (isNull c) do
                        let n = c.Next
                        c.Dispose()
                        c <- n
                    first <- null
                    last <- null


                if not (isNull prev) then prev.Next <- next
                elif not (isNull parent) then parent.First <- next

                if not (isNull next) then next.Prev <- prev
                elif not (isNull parent) then parent.Last <- prev

                prev <- null
                next <- null

            member x.Update(action : IRenderStream -> unit) =
                if not (isNull first) then    
                    // clear all child-programs
                    let l = first.Prev
                    let r = last.Next
                    if not (isNull l) then l.Next <- r
                    if not (isNull r) then r.Prev <- l
                    first.Prev <- null
                    last.Next <- null

                    let mutable c = first
                    while not (isNull c) do
                        let n = c.Next
                        c.Dispose()
                        c <- n
                    first <- null
                    last <- null

                // empty or fragment
                if isNull fragment then
                    fragment <- env.NewFragment (fun s p -> action (wrap s p))

                    let p = x.LastFragmentInPrev
                    let n = x.FirstFragmentInNext

                    fragment.Next <- n

                    match p with
                    | null -> env.First <- fragment
                    | p -> p.Next <- fragment
                else
                    fragment.Mutate (fun s p -> action (wrap s p))
                    

            member x.InsertAfter(ref : Block) =
                if not (isNull fragment) then
                    let p = fragment.Prev
                    let n = fragment.Next
                    if isNull p then env.First <- n
                    else p.Next <- n
                    fragment.Dispose()
                    fragment <- null

                let p = new Block(x, env, wrap)
                let l = if isNull ref then null else ref
                let r = if isNull ref then first else ref.Next

                p.Prev <- l
                p.Next <- r
                if not (isNull l) then l.Next <- p
                if not (isNull r) then r.Prev <- p

                if isNull first then first <- p
                if isNull last || ref = last then last <- p
                p
                
            member x.InsertBefore(ref : Block) =
                if isNull ref then 
                    x.Append()
                else
                    x.InsertAfter(ref.Prev)

            member x.Append() =
                x.InsertAfter(last)

            member x.Prepend() =
                x.InsertAfter(null)

            member x.Remove(program : Block) =
                let p = program.Prev
                let n = program.Next

                if isNull p then first <- n
                else p.Next <- n

                if isNull n then last <- p
                else n.Prev <- p

                program.Prev <- null
                program.Next <- null
                program.Dispose()
                
        type Program() =
            let fragmentPrgram = new FragmentProgram()
            let encoder = Marshal.AllocHGlobal sizeof<RenderPassEncoderHandle>
            let wrap = fun (ass : IAssemblerStream) (p : IAdaptivePinning) -> new NativeRenderPassEncoderStream(ass, p, NativePtr.ofNativeInt encoder) :> IRenderStream
            let block = Block(null, fragmentPrgram, wrap)

            member x.Root =
                block

            member x.Run(e : RenderPassEncoder) =
                lock x (fun () ->
                    if not (isNull e) then
                        NativePtr.write (NativePtr.ofNativeInt encoder) e.Handle

                    fragmentPrgram.Run()
                )



        let test() =
            
            let pin (action : unit -> unit) =
                let del = System.Action(action)
                let gc = GCHandle.Alloc(del)
                Marshal.GetFunctionPointerForDelegate del

            let print (str : string) =
                pin (fun () -> Log.line "%s" str)


            let a = print "a"
            let b = print "b"
            let c = print "c"
            let d = print "d"

            let overall = Program()
            let pp = overall.Root

            let p = pp.Append()
            let p0 = p.Append()
            let p1 = p.Append()
            let p2 = p.Append()

            p2.Update(fun s -> 
                let s = s.Assembler
                s.BeginCall 0
                s.Call(c)    
            )
            
            p1.Update(fun s -> 
                let s = s.Assembler
                s.BeginCall 0
                s.Call(b)    
            )

            p0.Update(fun s -> 
                let s = s.Assembler
                s.BeginCall 0
                s.Call(a)    
            )
            
            
            let enc : RenderPassEncoder = null

            Log.start "run"
            overall.Run enc
            Log.stop()


            let p6 = p.InsertAfter(p0)
            p6.Update(fun s -> 
                let s = s.Assembler
                s.BeginCall 0
                s.Call(b)    
            )

            
            Log.start "run"
            overall.Run enc
            Log.stop()


            p1.Dispose()
            
            Log.start "run"
            overall.Run enc
            Log.stop()

            let p3 = p.Append()
            let p30 = p3.Append()
            let p31 = p3.Append()

            p31.Update(fun s -> 
                let s = s.Assembler
                s.BeginCall 0
                s.Call(d)    
            )
            
            Log.start "run"
            overall.Run enc
            Log.stop()

            
            p30.Update(fun s -> 
                let s = s.Assembler
                s.BeginCall 0
                s.Call(b)    
            )
            
            Log.start "run"
            overall.Run enc
            Log.stop()

            p.Dispose()
            Log.start "run"
            overall.Run enc
            Log.stop()

            
            let p = pp.Append()
            p.Update(fun s -> 
                let s = s.Assembler
                s.BeginCall 0
                s.Call(a)   
                s.BeginCall 0
                s.Call(b)    
            )
            Log.start "run"
            overall.Run enc
            Log.stop()
            


            exit 0








    type Command =
        abstract member Run : RenderPassEncoder -> unit


    [<AllowNullLiteral>]
    type Node = 
        val mutable public Command : Command
        val mutable public Next : Node
        val mutable public Prev : Node

        new(c) = { Command = c; Prev = null; Next = null }
        
    [<AllowNullLiteral>]
    type Program2(parent : Program2, globalFirst : ref<Node>, globalLast : ref<Node>) =
        static let emptyCmd = { new Command with member x.Run _ = () }

        let mutable prev : obj = null
        let mutable next : obj = null

        let mutable first : Node = null
        let mutable last : Node = null
        
        let mutable firstChild : Program2 = null
        let mutable lastChild : Program2 = null
        
        let mutable firstKind = 0
        let mutable lastKind = 0

        member x.Clear() =
            if lastKind <> 0 then
                let (l, r) = 
                    match firstKind, lastKind with
                    | 1, 1 ->
                        let l = first.Prev
                        let r = last.Next
                        l, r
                    | 1, 2 ->
                        let l = first.Prev
                        let r = lastChild.RightAnchor
                        l, r
                    | 2, 1 ->
                        let l = firstChild.LeftAnchor
                        let r = last
                        l, r
                    | _, _ ->
                        let l = firstChild.LeftAnchor
                        let r = lastChild.RightAnchor
                        l, r

                if isNull r then globalLast := l
                else r.Prev <- l
                if isNull l then globalFirst := r
                else l.Next <- r

                first <- null
                last <- null
                firstChild <- null
                lastChild <- null
                firstKind <- 0
                lastKind <- 0

        member x.Prev
            with get(): obj = prev
            and set (p: obj) = prev <- p
            
        member x.Next 
            with get(): obj = next
            and set (p: obj) = next <- p

   
        member x.FirstChild
            with get() = firstChild
            and set p = firstChild <- p
            
        member x.LastChild
            with get() = lastChild
            and set p = lastChild <- p

   
        member x.First
            with get() = first
            and set p = first <- p
            
        member x.Last
            with get() = last
            and set p = last <- p

        member x.LastNode =
            match lastKind with
            | 0 ->
                match prev with
                | null ->
                    if isNull parent then null
                    else parent.LeftAnchor
                | :? Node as n ->
                    n
                | :? Program2 as p ->
                    p.LastNode
                | _ -> 
                    failwith ""
            | 1 ->
                last
            | _ ->
                lastChild.LastNode

        member x.FirstNode =
            match firstKind with
            | 0 ->
                match next with
                | null -> 
                    if isNull parent then null
                    else parent.RightAnchor
                | :? Node as next ->
                    next
                | :? Program2 as next ->
                    next.FirstNode
                | _ -> failwith ""
            | 1 ->
                first
            | _ ->
                firstChild.FirstNode

        member x.LeftAnchor =
            match prev with
            | null -> 
                if isNull parent then null
                else parent.LeftAnchor
            | :? Program2 as prev ->
                prev.LastNode
            | :? Node as node ->
                node
            | _ ->
                failwith ""
                
        member x.RightAnchor =
            match next with
            | null ->
                if isNull parent then null
                else parent.RightAnchor
            | :? Program2 as next ->
                next.FirstNode
            | :? Node as node ->
                node
            | _ ->
                failwith ""

        member x.Append(cmd : Command) =
            let n = Node cmd

            match lastKind with
            | 0 ->  
                assert(isNull last)
                assert(isNull lastChild)
                first <- n
                last <- n

                let p = x.LeftAnchor
                if isNull p then
                    globalFirst := n
                else
                    n.Next <- p.Next
                    if isNull p.Next then globalLast := n
                    else p.Next.Prev <- n

                    p.Next <- n
                    n.Prev <- p

                firstKind <- 1
                lastKind <- 1

            | 1 -> 
                assert( not (isNull last) )
                n.Prev <- last
                n.Next <- last.Next
                if isNull last.Next then globalLast := n
                else last.Next.Prev <- n
                last.Next <- n
                last <- n
                lastKind <- 1
            | _ (* 2 *) ->
                assert( not (isNull lastChild) )
                let p = lastChild.LastNode

                lastChild.Next <- n

                n.Prev <- p
                n.Next <- p.Next
                if isNull p.Next then globalLast := n
                else p.Next.Prev <- n
                p.Next <- n

                last <- n
                lastKind <- 1
               
               
        member x.NewProgram() =
            let p = Program2(x, globalFirst, globalLast)

            match lastKind with
            | 0 ->  
                firstChild <- p
                lastChild <- p
                firstKind <- 2
                lastKind <- 2

            | 1 ->
                p.Prev <- last
                lastChild <- p
                lastKind <- 2
            | _ ->  
                lastChild.Next <- p
                p.Prev <- lastChild
                lastChild <- p
                lastKind <- 2
                
            p
                

        member x.Run(e) =
            let mutable c = !globalFirst 
            while not (isNull c) do
                c.Command.Run e
                c <- c.Next
       

        new() = Program2(null, ref null, ref null)


    type Program(actions : SortedSetExt<struct(Index * Command)>, minIndex : Index, maxIndex : Index) =
        static let tupleComparer =
            let indexComparer = Comparer<Index>.Default
            { new IComparer<struct(Index * Command)> with
                member x.Compare(struct(l,_), struct(r,_)) =
                    indexComparer.Compare(l, r)
            }

        let mutable lastIndex = minIndex

        let newIndex() =
            let id = 
                if maxIndex = Index.zero then Index.after lastIndex
                else Index.between lastIndex maxIndex
            lastIndex <- id
            id
                
        member x.SubProgram() =
            let id0 = newIndex()
            let id1 = newIndex()
            Program(actions.GetViewBetween(struct(id0, Unchecked.defaultof<_>), struct(id1, Unchecked.defaultof<_>)), id0, id1)

        member x.Append(cmd : Command) =
            let id = newIndex()
            actions.Add(struct(id, cmd)) |> ignore

        member x.Run(e : RenderPassEncoder) =
            for struct(_,a) in actions do
                a.Run e

        member x.Set(s : #seq<Command>) =
            let eo = Array.zeroCreate actions.Count
            actions.CopyTo(eo)
            use en = (s :> seq<_>).GetEnumerator()
            
            let mutable oi = 0

            while oi < eo.Length && en.MoveNext() do
                let o = eo.[oi]
                let struct(oid,_) = o
                actions.Remove o |> ignore
                actions.Add(struct(oid, en.Current)) |> ignore
                oi <- oi + 1
            
            while oi < eo.Length do
                actions.Remove eo.[oi] |> ignore
                
                 
            while en.MoveNext() do
                let id = newIndex()
                actions.Add(struct(id, en.Current)) |> ignore
                

        member x.Clear() =
            actions.Clear()
            lastIndex <- minIndex

        new() = Program(SortedSetExt(tupleComparer), Index.zero, Index.zero)


    type Program2 with
        member x.Print str =
            x.Append { new Command with
                member x.Run _ = Log.line "%s" str
            }

    let test() =
    
        let cmd (action : unit -> unit) =
            { new Command with
                member x.Run _ = action()
            }
        let p = Program2()

        p.Print "before A"
        let a = p.NewProgram()
        p.Print "after A"
        
        a.Print "before AA"
        let aa = a.NewProgram()
        a.Print "after AA"
        
        aa.Print "before AAA"
        let aaa = aa.NewProgram()
        aa.Print "after AAA"
        
        p.Print "before B"
        let b = p.NewProgram()
        p.Print "after B"


        aaa.Append(cmd <| fun () -> Log.line "AAA")
        aa.Append (cmd <| fun () -> Log.line "AA")
        a.Append (cmd <| fun () -> Log.line "A")
        b.Append (cmd <| fun () -> Log.line "B")
        
        Log.start "run 1"
        p.Run Unchecked.defaultof<_>
        Log.stop()
        
        a.Clear()
        Log.start "run 1"
        p.Run Unchecked.defaultof<_>
        Log.stop()

        aa.Clear()
        aa.Print "YEAH"
        Log.start "run 1"
        p.Run Unchecked.defaultof<_>
        Log.stop()

        exit 0


        let p = Program()


        p.Append (cmd <| fun () -> Log.line "A")

        let s = p.SubProgram()
        s.Append (cmd <| fun () -> Log.line "B")
        s.Append (cmd <| fun () -> Log.line "C")
        s.Append (cmd <| fun () -> Log.line "D")

        p.Append (cmd <| fun () -> Log.line "E")

        Log.start "run 1"
        p.Run(Unchecked.defaultof<_>)
        Log.stop()

        
        s.Clear()
        Log.start "run 2"
        p.Run(Unchecked.defaultof<_>)
        Log.stop()

        
        s.Append(cmd <| fun () -> Log.line "INNER")
        Log.start "run 2"
        p.Run(Unchecked.defaultof<_>)
        Log.stop()

        
        s.Set [ cmd (fun () -> Log.line "new one"); cmd (fun () -> Log.line "new two")]
        Log.start "run 2"
        p.Run(Unchecked.defaultof<_>)
        Log.stop()
