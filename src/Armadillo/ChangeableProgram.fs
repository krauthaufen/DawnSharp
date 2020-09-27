namespace Armadillo

open Aardvark.Base
open FSharp.Data.Adaptive

#nowarn "9"
    
[<AutoOpen>]
module rec ChangeableProgram = 
    open Microsoft.FSharp.NativeInterop
    open System.IO
    open System.Runtime.InteropServices
    open Aardvark.Base.Runtime
    open WebGPU.MemoryManagement

    module Memory =
        let executable : Memory<nativeint> =
            {
                malloc = ExecutableMemory.alloc
                mfree = ExecutableMemory.free
                mcopy = fun (src : nativeint) (srcOff : nativeint) (dst : nativeint) (dstOff : nativeint) (size : nativeint) -> Marshal.Copy(src + srcOff, dst + dstOff, size)
                mrealloc = fun (old : nativeint) (oldSize : nativeint) (newSize : nativeint) ->
                    let n = ExecutableMemory.alloc newSize
                    Marshal.Copy(old, n, min oldSize newSize)
                    ExecutableMemory.free old oldSize
                    n
            }

    [<AbstractClass>]
    type IAdaptivePinning() =
        abstract member Pin : object : IAdaptiveObject * evaluate : (AdaptiveToken -> 'a) * release : (unit -> unit) -> nativeptr<'a>
        abstract member Pin : action : (unit -> unit) -> nativeint

        abstract member Pin<'a when 'a : unmanaged> : 'a[] -> nativeptr<'a>

        //member x.Pin<'a when 'a : unmanaged>(data : 'a[]) =
        //    let gc = GCHandle.Alloc(data, GCHandleType.Pinned)
        //    x.Pin(Unchecked.defaultof<IAdaptiveObject>, (fun t -> 0), gc.Free) |> ignore
        //    gc.AddrOfPinnedObject() |> NativePtr.ofNativeInt<'a>

        member x.Pin(value : aval<'a>) =
            x.Pin(value, value.GetValue, id)

        member x.Pin(p : FragmentProgram) =
            p.Acquire()
            x.Pin(p, (fun t -> p.Update t; 0), p.Dispose) |> ignore
            p.EntryPointer


    type AdaptivePinning () =
        inherit AdaptiveObject() 

        let dirty = Dict<IAdaptiveObject, AdaptiveToken -> unit>()
        
        let pointers = Dict<IAdaptiveObject, ref<int> * (unit -> unit) * nativeint>()
        let targets = Dict<nativeint, AdaptiveToken -> unit>()

        member x.Dispose() =
            for KeyValue(o, (r, release, ptr)) in pointers do 
                o.Outputs.Remove x |> ignore
                release()
                Marshal.FreeHGlobal ptr
            dirty.Clear()
            pointers.Clear()
            targets.Clear()

        override x.InputChangedObject(t : obj, o : IAdaptiveObject) =
            lock pointers (fun () ->
                match pointers.TryGetValue o with
                | (true, (_, _, ptr)) ->
                    match targets.TryGetValue ptr with
                    | (true, write) -> dirty.[o] <- write
                    | _ -> ()
                | _ ->
                    ()
            )

        member x.Remove(value : IAdaptiveObject) =
            lock pointers (fun () ->
                match pointers.TryGetValue value with
                | (true, (ref, release, ptr)) ->
                    ref := !ref - 1
                    if !ref = 0 then
                        pointers.Remove value |> ignore
                        targets.Remove ptr |> ignore
                        value.Outputs.Remove x |> ignore
                        dirty.Remove value |> ignore
                        release()
                | _ ->
                    ()
            )

        member x.Add(value : IAdaptiveObject, evaluate : AdaptiveToken -> 'a, release : unit -> unit) =
            lock pointers (fun () ->
                let ref, _, ptr = pointers.GetOrCreate(value, fun _ -> ref 0, release, Marshal.AllocHGlobal sizeof<'a>)
                ref := !ref + 1
                if !ref = 1 then
                    let write(token : AdaptiveToken) =
                        let v = evaluate token
                        NativeInt.write ptr v
                    targets.[ptr] <- write
                    x.EvaluateAlways AdaptiveToken.Top (fun t -> write t)

                { new System.IDisposable with 
                    member __.Dispose() = 
                        x.Remove value 
                }, NativePtr.ofNativeInt<'a> ptr
            )

        member x.Update(token : AdaptiveToken) =
            x.EvaluateIfNeeded token () (fun token ->
                for write in dirty.Values do write token
            )

        interface System.IDisposable with
            member x.Dispose() = x.Dispose()

    [<AllowNullLiteral>]
    type ProgramFragment internal(parent : FragmentProgram, block : Block<nativeint>, pinned : System.Collections.Generic.List<System.IDisposable>) =
        let mutable block = block

        static let assemble (action : IAssemblerStream -> unit) =
            use ms = new MemoryStream()
            use ass = AssemblerStream.ofStream ms
            action ass
            ms.ToArray()

        static let jumpSize = 
            assemble (fun s -> 
                s.Jump 34325324
            ) |> Array.length

        static let jumpDistance (lOffset : nativeint) (lSize : nativeint) (rOffset : nativeint) =
            let jumpEnd = lOffset + lSize
            int (rOffset - jumpEnd)

        let mutable prev : ProgramFragment = null
        let mutable next : ProgramFragment = null

        member private x.WriteJumpTo(jumpTarget : nativeint) =
            parent.Use(block, fun ptr offset size ->
                let d = jumpDistance block.Offset block.Size jumpTarget
                use m = new UnmanagedMemoryStream(NativePtr.ofNativeInt (ptr + offset + size - nativeint jumpSize), int64 jumpSize, int64 jumpSize, FileAccess.ReadWrite)
                use ass = AssemblerStream.ofStream m
                ass.Jump d
            )

        member x.Mutate(action : IAssemblerStream -> IAdaptivePinning -> unit) =
            use ms = new MemoryStream()
            use ass = AssemblerStream.ofStream ms

            let o = pinned.ToArray()
            pinned.Clear()
            let p =
                { new IAdaptivePinning() with
                    member x.Pin<'a when 'a : unmanaged>(array : 'a[]) =
                        let gc = GCHandle.Alloc(array, GCHandleType.Pinned)
                        pinned.Add { new System.IDisposable with member x.Dispose() = gc.Free() }
                        gc.AddrOfPinnedObject() |> NativePtr.ofNativeInt<'a>
                    member x.Pin(value : IAdaptiveObject, eval : AdaptiveToken -> 'a, release : unit -> unit) =
                        let d, ptr = parent.Pinning.Add(value, eval, release)
                        pinned.Add d
                        ptr
                    member x.Pin(action : unit -> unit) =
                        let pd = Marshal.PinDelegate(System.Action(action))
                        pinned.Add pd
                        pd.Pointer

                }

            action ass p
            for o in o do o.Dispose()

            let jumpTarget =
                match next with
                | null -> parent.Epilog.Offset
                | n -> n.Offset

            let newSize =
                nativeint ms.Length + 
                nativeint jumpSize

            if newSize <> block.Size then 
                parent.Free block
                let n = parent.Alloc(newSize)
                block <- n
                match prev with
                | null -> ()
                | p -> p.WriteJumpTo block.Offset

            ass.Jump(jumpDistance block.Offset block.Size jumpTarget)
            let arr = ms.ToArray()
            parent.Use(block, fun basePtr offset _size ->
                Marshal.Copy(arr, 0, basePtr + offset, arr.Length)
            )

        member x.IsDisposed =
            parent.IsDisposed || block.IsFree

        member x.Dispose() =
            if not x.IsDisposed then 
                for p in pinned do p.Dispose()
                match prev with
                | null -> ()
                | p -> p.Next <- next
                prev <- null
                next <- null
                parent.Free block

        member x.Offset = 
            if x.IsDisposed then raise <| System.ObjectDisposedException "CodeFragment"
            block.Offset

        member x.Prev
            with get() = 
                if x.IsDisposed then raise <| System.ObjectDisposedException "CodeFragment"
                prev
            and private set p = 
                if x.IsDisposed then raise <| System.ObjectDisposedException "CodeFragment"
                prev <- p

        member x.Next
            with get() = 
                if x.IsDisposed then raise <| System.ObjectDisposedException "CodeFragment"
                next
            and set n =
                if x.IsDisposed then raise <| System.ObjectDisposedException "CodeFragment"
                next <- n
                let jumpTarget = 
                    match n with
                    | null ->
                        parent.Epilog.Offset
                    | n -> 
                        n.Prev <- x
                        n.Offset

                x.WriteJumpTo jumpTarget

        interface System.IDisposable with
            member x.Dispose() = x.Dispose()

    type FragmentProgram() as this =
        inherit AdaptiveObject()

        let mutable refCount = 1

        let memory = 
            new MemoryManager<_>(Memory.executable, 16n)

        let pinning = new AdaptivePinning()

        static let assemble (action : IAssemblerStream -> unit) =
            use ms = new MemoryStream()
            use ass = AssemblerStream.ofStream ms
            action ass
            ms.ToArray()
            
        static let epilog = 
            assemble (fun s -> 
                s.EndFunction()
                s.Ret()
            )
            
        static let jumpSize = 
            assemble (fun s -> 
                s.Jump 34325324
            ) |> Array.length

        static let prologSize = 
            assemble (fun s -> 
                s.BeginFunction()
                s.Jump 34325324
            ) |> Array.length

        static let jumpDistance (lOffset : nativeint) (lSize : nativeint) (rOffset : nativeint) =
            let jumpEnd = lOffset + lSize
            int (rOffset - jumpEnd)

        let epilog =
            lazy (
                let block = memory.Alloc(nativeint epilog.Length)
                memory.Use(block, fun basePtr offset _size ->
                    Marshal.Copy(epilog, 0, basePtr + offset, epilog.Length)
                )
                block
            )

            

        let mutable run : option<unit -> unit> = None
        let prologBlock =

            let block = memory.Alloc (nativeint prologSize)
            memory.Use(block, fun basePtr offset _size ->
                use ms = new MemoryStream()
                use ass = AssemblerStream.ofStream ms

                ass.BeginFunction()
                ass.Jump (jumpDistance block.Offset block.Size epilog.Value.Offset)

                let arr = ms.ToArray()
                Marshal.Copy(arr, 0, basePtr + offset, arr.Length)

            )
            block
            
        let mutable entryPointer = 
            let ptr = NativePtr.alloc 1
            NativePtr.write ptr (memory.UnsafePointer + prologBlock.Offset)
            ptr

        let mutable prologFragment = None

        let getProlog() = 
            match prologFragment with
            | Some f -> f
            | None ->
                let f = new ProgramFragment(this, prologBlock, System.Collections.Generic.List())
                prologFragment <- Some f
                f

            //new CodeFragment(this, block)

        let fixEntry() =
            let ptr = memory.UnsafePointer + prologBlock.Offset
            let o = NativePtr.read entryPointer
            if o <> ptr then
                NativePtr.write entryPointer ptr
                run <- None

        member x.IsDisposed =
            entryPointer = NativePtr.zero

        member x.Dispose() =
            if not x.IsDisposed && System.Threading.Interlocked.Decrement(&refCount) = 0 then
                pinning.Dispose()
                memory.Dispose()
                NativePtr.free entryPointer
                entryPointer <- NativePtr.zero
                run <- None

        member internal x.Epilog : Block<nativeint> = epilog.Value

        member internal x.Alloc(newSize : nativeint) =
            let b = memory.Alloc(newSize)
            fixEntry()
            b

        member internal x.Free(b : Block<nativeint>) =
            memory.Free(b)
            fixEntry()

        member internal x.Use<'a>(block : Block<nativeint>, action : nativeint -> nativeint -> nativeint -> 'a) : 'a =
            memory.Use<'a>(block, action)

        member internal x.Pinning : AdaptivePinning = pinning

        member internal x.Acquire() =
            System.Threading.Interlocked.Increment(&refCount) |> ignore


        member x.EntryPointer =
            entryPointer

        member internal x.Update(token : AdaptiveToken) =
            x.EvaluateIfNeeded token () (fun token ->
                pinning.Update token
            )

        member x.Run() =
            match run with
            | Some run -> run()
            | None ->
                if x.IsDisposed then raise <| System.ObjectDisposedException "FragmentProgram"
                let ptr = NativePtr.read entryPointer
                let r = UnmanagedFunctions.wrap ptr
                run <- Some r
                r()
                
        member x.Run(token : AdaptiveToken) =
            x.Update token
            x.Run()

        member x.First
            with get() = 
                if x.IsDisposed then raise <| System.ObjectDisposedException("FragmentProgram")
                getProlog().Next
            and set f = 
                if x.IsDisposed then raise <| System.ObjectDisposedException("FragmentProgram")
                getProlog().Next <- f

        member x.NewFragment(write : IAssemblerStream -> IAdaptivePinning -> unit) =
            if x.IsDisposed then raise <| System.ObjectDisposedException("FragmentProgram")
            use ms = new MemoryStream()
            use ass = AssemblerStream.ofStream ms

            let disposables = System.Collections.Generic.List<System.IDisposable>()
            let p =
                { new IAdaptivePinning() with
                    member x.Pin<'a when 'a : unmanaged>(array : 'a[]) =
                        let gc = GCHandle.Alloc(array, GCHandleType.Pinned)
                        disposables.Add { new System.IDisposable with member x.Dispose() = gc.Free() }
                        gc.AddrOfPinnedObject() |> NativePtr.ofNativeInt<'a>
                    member x.Pin(value : IAdaptiveObject, eval : AdaptiveToken -> 'a, release : unit -> unit) =
                        let d, ptr = pinning.Add(value, eval, release)
                        disposables.Add d
                        ptr
                    member x.Pin(action : unit -> unit) =
                        let pd = Marshal.PinDelegate(System.Action(action))
                        disposables.Add pd
                        pd.Pointer
                }

            write ass p

            let blockSize =
                nativeint ms.Length +
                nativeint jumpSize

            let block = memory.Alloc blockSize
            fixEntry()
            ass.Jump (jumpDistance block.Offset block.Size epilog.Value.Offset)
            let arr = ms.ToArray()
            
            memory.Use(block, fun basePtr offset _size ->
                Marshal.Copy(arr, 0, basePtr + offset, arr.Length)
            )

            new ProgramFragment(x, block, disposables)

        interface System.IDisposable with
            member x.Dispose() = x.Dispose()
       
    type IAssemblerStream with
        member x.Print (pinning : IAdaptivePinning) fmt = 
            fmt |> Printf.kprintf (fun str ->
                let ptr = pinning.Pin (fun () -> Log.line "%s" str)
                x.BeginCall(0)
                x.Call ptr
            )
        
        member x.Call(program : FragmentProgram, pinning : IAdaptivePinning) =
            let ptr = pinning.Pin(program)
            for r in x.CalleeSavedRegisters do x.Push r
            x.BeginCall 0
            x.CallIndirect ptr
            for r in x.CalleeSavedRegisters do x.Pop r
     



    

