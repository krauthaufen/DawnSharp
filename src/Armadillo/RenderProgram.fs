namespace Armadillo

open System.Collections.Generic
open System.Runtime.InteropServices
open Aardvark.Base.Runtime
open Microsoft.FSharp.NativeInterop
open WebGPU
open Aardvark.Base

#nowarn "9"


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
        
    abstract member Execute : action : (RenderPassEncoder -> unit) -> unit

[<AllowNullLiteral>]
type IRenderFragment =
    inherit System.IDisposable
    abstract member Next : IRenderFragment with get, set
    abstract member Prev : IRenderFragment
    abstract member Update : compile : (IRenderStream -> unit) -> unit

type IRenderProgram =
    inherit System.IDisposable
    abstract member First : IRenderFragment with get, set
    abstract member Create : compile : (IRenderStream -> unit) -> IRenderFragment
    abstract member Run : RenderPassEncoder -> unit

module private Native = 
    type NativeRenderPassEncoderStream(device : Device, ass : IAssemblerStream, pinning : IAdaptivePinning, encoder : nativeptr<RenderPassEncoderHandle>) =
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

            member x.Execute(action : RenderPassEncoder -> unit) =
                let ptr = 
                    pinning.Pin (fun () -> 
                        let encoder = 
                            let handle = NativePtr.read encoder
                            if handle.Handle = 0n then 
                                null 
                            else
                                DawnRaw.wgpuRenderPassEncoderReference handle
                                new RenderPassEncoder(device, handle)
                        action encoder
                    )
                ass.BeginCall(0)
                ass.Call ptr

    [<AllowNullLiteral>]
    type NativeRenderFragment(device : Device, frag : ProgramFragment, encoder : nativeptr<RenderPassEncoderHandle>) =
            
        member x.Fragment =
            frag

        member x.Dispose() =
            frag.Dispose()

        member x.Prev =
            if isNull frag.Prev then null
            else new NativeRenderFragment(device, frag.Prev, encoder) :> IRenderFragment
                
        member x.Next
            with get() = 
                if isNull frag.Next then null
                else new NativeRenderFragment(device, frag.Next, encoder) :> IRenderFragment
            and set (n : IRenderFragment) =
                let n = n :?> NativeRenderFragment
                if isNull n then frag.Next <- null
                else frag.Next <- n.Fragment

        member x.Update(compile : IRenderStream -> unit) =
            frag.Mutate (fun ass pin ->
                let s = NativeRenderPassEncoderStream(device, ass, pin, encoder)
                compile s
            )

        interface IRenderFragment with
            member x.Dispose() = x.Dispose()
            member x.Next
                with get() = x.Next
                and set v = x.Next <- v

            member x.Prev = x.Prev
            member x.Update compile = x.Update compile

    type NativeRenderProgram(device : Device) =
        let program = new FragmentProgram()

        let encoder = Marshal.AllocHGlobal sizeof<RenderPassEncoderHandle> |> NativePtr.ofNativeInt

        member x.First 
            with get() = 
                new NativeRenderFragment(device, program.First, encoder) :> IRenderFragment
            and set (v : IRenderFragment) =
                if isNull v then
                    program.First <- null
                else
                    let v = v :?> NativeRenderFragment
                    program.First <- v.Fragment

        member x.Create(compile) =
            let frag = program.NewFragment(fun ass pin -> compile(new NativeRenderPassEncoderStream(device, ass, pin, encoder) :> IRenderStream))
            new NativeRenderFragment(device, frag, encoder) :> IRenderFragment

        member x.Dispose() =
            Marshal.FreeHGlobal (NativePtr.toNativeInt encoder)
            program.Dispose()

        member x.Run(enc : RenderPassEncoder) =
            lock x (fun () ->
                if isNull enc then NativePtr.write encoder RenderPassEncoderHandle.Null
                else NativePtr.write encoder enc.Handle
                program.Run()
            )

        interface IRenderProgram with
            member x.Dispose() = x.Dispose()

            member x.First
                with get() = x.First
                and set v = x.First <- v

            member x.Create compile = x.Create compile

            member x.Run encoder = x.Run encoder

module private Managed = 
    type ManagedRenderPassEncoderStream(store : System.Collections.Generic.List<RenderPassEncoder -> unit>) =
        interface IRenderStream with
            member x.Assembler = failwith ""
            member x.Pinning = failwith ""

            member x.SetPipeline(pipeline : RenderPipeline) =
                store.Add <| fun e -> e.SetPipeline pipeline
                
            member x.SetBindGroup(groupIndex : int, group : BindGroup, dynamicOffsets : int[]) =
                store.Add <| fun e -> e.SetBindGroup(groupIndex, group, dynamicOffsets)

            member x.EndPass() =
                store.Add <| fun e -> e.EndPass()

            member x.ExecuteBundles(bundles : RenderBundle[]) =
                store.Add <| fun e -> e.ExecuteBundles bundles

            member x.InsertDebugMarker (marker : string) =
                store.Add <| fun e -> e.InsertDebugMarker marker

            member x.PushDebugGroup (label : string) =
                store.Add <| fun e -> e.PushDebugGroup label

            member x.SetBlendColor (color : C4f) =
                store.Add <| fun e -> e.SetBlendColor { R = color.R; G = color.G; B = color.B; A = color.A }

            member x.PopDebugGroup() =
                store.Add <| fun e -> e.PopDebugGroup()

            member x.WriteTimestamp(querySet : QuerySet, queryIndex : int) =
                store.Add <| fun e -> e.WriteTimestamp(querySet, queryIndex)

            member x.SetIndexBuffer(a,b,c,d) =
                store.Add <| fun e -> e.SetIndexBufferWithFormat(a,b,c,d)
                    
            member x.SetScissorRect(o, s) =
                store.Add <| fun e -> e.SetScissorRect(o.X, o.Y, s.X, s.Y)

            member x.SetViewport(o, s, r) =
                store.Add <| fun e -> e.SetViewport(o.X, o.Y, s.X, s.Y, r.Min, r.Max)
                    
            member x.SetStencilReference(ref) =
                store.Add <| fun e -> e.SetStencilReference(int ref)

            member x.SetVertexBuffer(a, b, c, d) =
                store.Add <| fun e -> e.SetVertexBuffer(a, b, c, d)
                    
            member x.DrawIndirect(indexed : bool, buffer : Buffer, offset : uint64) =
                if indexed then
                    store.Add <| fun e -> e.DrawIndexedIndirect(buffer, offset)
                else
                    store.Add <| fun e -> e.DrawIndirect(buffer, offset)
                        
            member x.Draw(info : DrawInfo) =
                if info.indexed then
                    store.Add <| fun e -> e.DrawIndexed(info.count, info.instanceCount, info.first, info.baseVertex, info.firstInstance)
                else
                    store.Add <| fun e -> e.Draw(info.count, info.instanceCount, info.first, info.firstInstance)

            member x.Execute(action : RenderPassEncoder -> unit) =
                store.Add action

    [<AllowNullLiteral>]
    type ManagedRenderFragment() =
        let mutable prev : ManagedRenderFragment = null
        let mutable next : ManagedRenderFragment = null
        let code = System.Collections.Generic.List<RenderPassEncoder -> unit>()

        member x.Prev
            with get() = prev
            and set v = prev <- v

        member x.Next
            with get() = next
            and set v = next <- v

        member x.Run(enc : RenderPassEncoder) =
            for a in code do a enc

        interface IRenderFragment with
            member x.Dispose() =
                if not (isNull prev) then prev.Next <- next
                if not (isNull next) then next.Prev <- prev
                prev <- null
                next <- null
                code.Clear()

            member x.Next
                with get() = next :> IRenderFragment
                and set (n : IRenderFragment) =
                    let n = n :?> ManagedRenderFragment
                    next <- n
                    if not (isNull n) then n.Prev <- x

            member x.Prev = prev :> IRenderFragment

            member x.Update(compile : IRenderStream -> unit) =
                code.Clear()
                compile (ManagedRenderPassEncoderStream(code) :> IRenderStream)

    type ManagedRenderProgram() =
        let mutable first : ManagedRenderFragment = null

        member x.First 
            with get() = 
                first :> IRenderFragment
            and set (v : IRenderFragment) =
                let v = v :?> ManagedRenderFragment
                first <- v

        member x.Create(compile) =
            let f = new ManagedRenderFragment() :> IRenderFragment
            f.Update compile
            f
        member x.Dispose() =
            first <- null

        member x.Run(enc : RenderPassEncoder) =
            let mutable c = first
            while not (isNull c) do
                c.Run enc
                c <- c.Next

        interface IRenderProgram with
            member x.Dispose() = x.Dispose()

            member x.First
                with get() = x.First
                and set v = x.First <- v

            member x.Create compile = x.Create compile

            member x.Run encoder = x.Run encoder


[<AllowNullLiteral>]
type RenderFragment(parent : RenderFragment, env : IRenderProgram) =
    let mutable next : RenderFragment = null
    let mutable prev : RenderFragment = null

    let mutable fragment : IRenderFragment = null

    let mutable first : RenderFragment = null
    let mutable last : RenderFragment = null
            
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
            if isNull last then
                if isNull prev then parent.LastFragmentInPrev
                else prev.LastFragment
            else
                last.LastFragment
        else
            fragment
               
    member x.FirstFragment =
        if isNull fragment then
            if isNull first then
                if isNull next then parent.FirstFragmentInNext
                else next.FirstFragment
            else
                first.FirstFragment
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
            fragment <- env.Create action

            let p = x.LastFragmentInPrev
            let n = x.FirstFragmentInNext

            fragment.Next <- n

            match p with
            | null -> env.First <- fragment
            | p -> p.Next <- fragment
        else
            fragment.Update action
                    

    member x.InsertAfter(ref : RenderFragment) =
        if not (isNull fragment) then
            let p = fragment.Prev
            let n = fragment.Next
            if isNull p then env.First <- n
            else p.Next <- n
            fragment.Dispose()
            fragment <- null

        let p = new RenderFragment(x, env)
        let l = if isNull ref then null else ref
        let r = if isNull ref then first else ref.Next

        p.Prev <- l
        p.Next <- r
        if not (isNull l) then l.Next <- p
        if not (isNull r) then r.Prev <- p

        if isNull first then first <- p
        if isNull last || ref = last then last <- p
        p
                
    member x.InsertBefore(ref : RenderFragment) =
        if isNull ref then 
            x.Append()
        else
            x.InsertAfter(ref.Prev)

    member x.Append() =
        x.InsertAfter(last)

    member x.Prepend() =
        x.InsertAfter(null)

    member x.Remove(program : RenderFragment) =
        let p = program.Prev
        let n = program.Next

        if isNull p then first <- n
        else p.Next <- n

        if isNull n then last <- p
        else n.Prev <- p

        program.Prev <- null
        program.Next <- null
        program.Dispose()
                
type RenderProgram(device : Device, debug : bool) =
    let fragmentProgram = 
        match RuntimeInformation.ProcessArchitecture with
        | Architecture.X64 when not debug -> 
            new Native.NativeRenderProgram(device) :> IRenderProgram
        | _ ->
            new Managed.ManagedRenderProgram() :> IRenderProgram

    let block = RenderFragment(null, fragmentProgram)

    member x.Root =
        block

    member x.Run(e : RenderPassEncoder) =
        fragmentProgram.Run e

    member x.Dispose() =
        fragmentProgram.Dispose()

    interface System.IDisposable with
        member x.Dispose() = x.Dispose()
                

    new(device : Device) = new RenderProgram(device, false)

module RenderProgramTest =
    type IRenderStream with
        member x.printfn fmt =
            fmt |> Printf.kprintf (fun str ->
                x.Execute (fun _ -> Report.Line("{0}", str))
            )

    let test() =
        let device : Device = null
        let overall = new RenderProgram(device)
        let pp = overall.Root

        let p = pp.Append()
        let p0 = p.Append()
        let p1 = p.Append()
        let p2 = p.Append()

        p2.Update(fun s -> 
            s.printfn "c"
        )
            
        p1.Update(fun s -> 
            s.printfn "b"  
        )

        p0.Update(fun s -> 
            s.printfn "a"   
        )
            
            
        let enc : RenderPassEncoder = null

        Log.start "run"
        overall.Run enc
        Log.stop()


        let p6 = p.InsertAfter(p0)
        p6.Update(fun s -> 
            s.printfn "b"   
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
            s.printfn "d"     
        )
            
        Log.start "run"
        overall.Run enc
        Log.stop()

            
        p30.Update(fun s -> 
            s.printfn "b"  
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
            s.printfn "a" 
            s.printfn "b"     
        )
        Log.start "run"
        overall.Run enc
        Log.stop()
            


        exit 0

