namespace WebGPU

open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open Microsoft.FSharp.NativeInterop
open System
open System.Threading
open WebGPU

#nowarn "49"
#nowarn "9"
#nowarn "51"

open DawnRaw

exception QueueWaitFailed of status : FenceCompletionStatus
exception BufferMapFailed of status : BufferMapAsyncStatus

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
type CallbackExtensions private() =

    [<Extension>]
    static member Map(x : Buffer, mode : MapMode, offset : unativeint, size : unativeint) =
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

    [<Extension>]
    static member Wait(x : Queue) =
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


type DeviceTransferQueue(device : Device) =
    let queue = device.GetDefaultQueue()

    let uploadMem =
        {
            MemoryManagement.malloc = fun size -> device.CreateBuffer { Label = null; Size = uint64 size; Usage = BufferUsage.MapWrite ||| BufferUsage.CopySrc; MappedAtCreation = false }
            MemoryManagement.mfree = fun buffer _ -> buffer.Dispose()
            MemoryManagement.mcopy = fun src srcOff dst dstOff size -> failwith "copy not implemented"
            MemoryManagement.mrealloc = fun buf os ns -> failwith "resize not implemented"
        }
        
    let downloadMem =
        {
            MemoryManagement.malloc = fun size -> device.CreateBuffer { Label = null; Size = uint64 size; Usage = BufferUsage.MapRead ||| BufferUsage.CopyDst; MappedAtCreation = false }
            MemoryManagement.mfree = fun buffer _ -> buffer.Dispose()
            MemoryManagement.mcopy = fun src srcOff dst dstOff size -> failwith "copy not implemented"
            MemoryManagement.mrealloc = fun buf os ns -> failwith "resize not implemented"
        }

    let upload = new MemoryManagement.ChunkedMemoryManager<Buffer>(uploadMem, 64n <<< 20)
    let download = new MemoryManagement.ChunkedMemoryManager<Buffer>(downloadMem, 64n <<< 20)

    let u0 = upload.Alloc 256n
    let d0 = download.Alloc 256n

    //member x.Download(src : Buffer, srcOffset : int64, dst : nativeint, size : nativeint) =
    //    use enc = device.CreateCommandEncoder()
    //    let b = download.Alloc(size)
    //    try
    //        enc.CopyBufferToBuffer(src, uint64 srcOffset, b.Memory.Value, uint64 b.Offset, uint64 size)
    //        let cmd = enc.Finish()

    //        queue.Submit [| cmd |]
    //        queue.Wait()

    //        let srcPtr = b.Memory.Value.GetMappedRange(unativeint b.Offset, unativeint size)
    //        Marshal.Copy(srcPtr, dst, size)

    //    finally
    //        download.Free b
            
    member x.Download<'a when 'a : unmanaged>(src : Buffer, srcOffset : int64, dst : Span<'a>, count : int) =
        use enc = device.CreateCommandEncoder()
        let size = nativeint sizeof<'a> * nativeint count
        let b = download.Alloc(size)
        try
            enc.CopyBufferToBuffer(src, uint64 srcOffset, b.Memory.Value, uint64 b.Offset, uint64 size)
            let cmd = enc.Finish()

            queue.Submit [| cmd |]
            queue.Wait()
            
            let srcPtr = b.Memory.Value.Map(MapMode.Read, unativeint b.Offset, unativeint size)
            //let srcPtr = b.Memory.Value.GetMappedRange(unativeint b.Offset, unativeint size)
            let srcSpan = Span<'a>(NativePtr.toVoidPtr (NativePtr.ofNativeInt<'a> srcPtr), count)
            srcSpan.CopyTo(dst)
            b.Memory.Value.Unmap()

        finally
            download.Free b
            
    member x.Upload<'a when 'a : unmanaged>(src : Span<'a>, dst : Buffer, dstOffset : int64, count : int) =
        let size = nativeint sizeof<'a> * nativeint count
        let b = upload.Alloc(size)
        try
            use enc = device.CreateCommandEncoder()
            let tmpPtr = b.Memory.Value.Map(MapMode.Write, unativeint b.Offset, unativeint size)
            //let tmpPtr = b.Memory.Value.GetMappedRange(unativeint b.Offset, unativeint size)
            let tmpSpan = Span<'a>(NativePtr.toVoidPtr (NativePtr.ofNativeInt<'a> tmpPtr), count)
            src.CopyTo tmpSpan
            b.Memory.Value.Unmap()


            enc.CopyBufferToBuffer(b.Memory.Value, uint64 b.Offset, dst, uint64 dstOffset, uint64 size)
            use cmd = enc.Finish()

            queue.Submit [| cmd |]
            queue.Wait()

        finally
            upload.Free b
            
    member x.Download(src : Buffer, srcOffset : int64, dst : nativeint, size : nativeint) =
        let dstSpan = Span<byte>(NativePtr.toVoidPtr (NativePtr.ofNativeInt<byte> dst), int size)
        x.Download(src, srcOffset, dstSpan, int size)

    member x.Upload(src : nativeint, dst : Buffer, dstOffset : int64, size : nativeint) =
        let srcSpan = Span<byte>(NativePtr.toVoidPtr (NativePtr.ofNativeInt<byte> src), int size)
        x.Upload(srcSpan, dst, dstOffset, int size)
            
         
    member x.Upload(src : Data, dst : Buffer, dstOffset : int64, size : nativeint) =
        let b = upload.Alloc(size)
        try
            use enc = device.CreateCommandEncoder()
            let tmpPtr = b.Memory.Value.Map(MapMode.Write, unativeint b.Offset, unativeint size)
            //let tmpPtr = b.Memory.Value.GetMappedRange(unativeint b.Offset, unativeint size)
            let tmpSpan = Span<byte>(NativePtr.toVoidPtr (NativePtr.ofNativeInt<byte> tmpPtr), int size)
            src.CopyTo tmpSpan
            b.Memory.Value.Unmap()


            enc.CopyBufferToBuffer(b.Memory.Value, uint64 b.Offset, dst, uint64 dstOffset, uint64 size)
            use cmd = enc.Finish()

            queue.Submit [| cmd |]
            queue.Wait()

        finally
            upload.Free b
            
[<AbstractClass; Sealed; Extension>]
type DeviceTransferQueueExtensions private() =

    static let transfers = ConditionalWeakTable<Device, DeviceTransferQueue>()
    
    [<Extension>]
    static member GetDeviceTransferQueue(device : Device) =
        lock transfers (fun () ->
            match transfers.TryGetValue device with
            | (true, queue) -> queue
            | _ ->
                let queue = new DeviceTransferQueue(device)
                transfers.Add(device, queue) |> ignore
                queue
        )


type Buffer<'a when 'a : unmanaged> internal(buffer : Buffer, offset : nativeint, count : int) =
    static let sa = nativeint sizeof<'a>
    static let usa = unativeint sizeof<'a>

    member x.Buffer = buffer
    member x.Offset = offset
    member x.Count = count

    member x.Sub(index : int, cnt : int) = 
        let e = index + cnt
        if index < 0 || e > count then  
            raise <| IndexOutOfRangeException (sprintf "bad range [%d..%d] in buffer of size %d" index (e-1) count)
        new Buffer<'a>(buffer.Clone(), offset + sa * nativeint index, cnt)
        
    member x.Sub(index : int) = 
        if index < 0 || index > count then  
            raise <| IndexOutOfRangeException (sprintf "bad range [%d..] in buffer of size %d" index count)
        new Buffer<'a>(buffer.Clone(), offset + sa * nativeint index, count - index)
        
    member x.Take(cnt : int) = 
        if cnt < 0 || cnt > count then  
            raise <| IndexOutOfRangeException (sprintf "bad range [..%d] in buffer of size %d" (cnt - 1) count)
        new Buffer<'a>(buffer.Clone(), offset, cnt)

    member x.GetSlice(first : option<int>, last : option<int>) =
        match first with
        | Some first ->
            match last with
            | Some last ->
                if last < first || first < 0 || last >= count then
                    raise <| IndexOutOfRangeException (sprintf "bad range [%d..%d] in buffer of size %d" first last count)

                new Buffer<'a>(buffer.Clone(), offset + sa * nativeint first, 1 + last - first)
            | None ->
                if first < 0 || first >= count then 
                    raise <| IndexOutOfRangeException (sprintf "bad range [%d..] in buffer of size %d" first count)

                new Buffer<'a>(buffer.Clone(), offset + sa * nativeint first, count - first)
        | None ->
            match last with
            | Some last ->
                if last < 0 || last >= count then
                    raise <| IndexOutOfRangeException (sprintf "bad range [..%d] in buffer of size %d" last count)
                new Buffer<'a>(buffer.Clone(), offset, 1 + last)
            | None ->
                x

    member x.SetSlice(first : option<int>, last : option<int>, data : 'a[]) =
        let count = ()
        let offset = ()
        let slice = x.GetSlice(first, last)
        if slice.Count <> data.Length then
            raise <| IndexOutOfRangeException (sprintf "bad data for range: %d vs %d" slice.Count data.Length)

        let queue = buffer.Device.GetDeviceTransferQueue()
        use pData = fixed data
        let size = sa * nativeint slice.Count
        queue.Upload(NativePtr.toNativeInt pData, buffer, int64 slice.Offset, size)

        
    member x.SetSlice(first : option<int>, last : option<int>, src : Buffer<'a>) =
        let count = ()
        let offset = ()
        let dst = x.GetSlice(first, last)
        if dst.Count <> src.Count then
            raise <| IndexOutOfRangeException (sprintf "bad data for range: %d vs %d" dst.Count src.Count)

        let queue = buffer.Device.GetDefaultQueue()

        use enc = buffer.Device.CreateCommandEncoder()
        let size = usa * unativeint src.Count
        enc.CopyBufferToBuffer(src.Buffer, uint64 src.Offset, dst.Buffer, uint64 dst.Offset, uint64 size)
        use cmd = enc.Finish()
        queue.Submit [| cmd |]
        queue.Wait()

    member x.Upload(src : Span<'a>) =
        if src.Length <> count then 
            raise <| IndexOutOfRangeException(sprintf "bad destintation for upload: %d vs %d" src.Length count)

        let queue = buffer.Device.GetDeviceTransferQueue()
        queue.Upload(src, buffer, int64 offset, count)

        
    member x.Upload(src : Memory<'a>) =
        x.Upload(src.Span)
        
    member x.Upload(src : array<'a>, start : int, cnt : int) =
        x.Upload(Span<'a>(src, start, cnt))
        
    member x.Upload(src : array<'a>) =
        x.Upload(Span<'a>(src))
        
    member x.Download(dst : Span<'a>) =
        if dst.Length <> count then 
            raise <| IndexOutOfRangeException(sprintf "bad destintation for download: %d vs %d" dst.Length count)

        let queue = buffer.Device.GetDeviceTransferQueue()
        queue.Download(buffer, int64 offset, dst, count)
       
            
    member inline x.Download(dst : Memory<'a>) =
        x.Download(dst.Span)
        
    member inline x.Download(dst : array<'a>, offset : int, count : int) =
        x.Download(Span<'a>(dst, offset, count))
        
    member inline x.Download(dst : array<'a>) =
        x.Download(Span<'a>(dst))
        
    member x.Download() =
        let arr = Array.zeroCreate count
        x.Download arr
        arr

    member private x.Dispose(disposing : bool) =
        if disposing then GC.SuppressFinalize x
        buffer.Dispose()

    member x.Dispose() =
        x.Dispose true

    interface IDisposable with
        member x.Dispose() = x.Dispose()


[<AbstractClass; Sealed; Extension>]
type WebGPUExtensions private() =

    static let glslangStage (stage : ShaderStage) =
        match stage with
        | ShaderStage.Vertex -> GLSLang.ShaderStage.Vertex
        | ShaderStage.Fragment -> GLSLang.ShaderStage.Fragment
        | ShaderStage.Compute -> GLSLang.ShaderStage.Compute
        | _ -> failwithf "unknown ShaderStage: %A" stage

    [<Extension>]
    static member CreateBuffer<'a when 'a : unmanaged>(device : Device, count : int, usage : BufferUsage) =
        let buffer = 
            device.CreateBuffer {
                Label = null
                Usage = usage
                Size = uint64 sizeof<'a> * uint64 count
                MappedAtCreation = false
            }
        new Buffer<'a>(buffer, 0n, count)



    [<Extension>]
    static member CreateSpirVShaderModule(device : Device, descriptor : SpirVShaderModuleDescriptor) =
        if descriptor.Code.Length &&& 3 <> 0 then failwith "Bad SpirV"
        let desc = { ShaderModuleDescriptor.Label = descriptor.Label }
        desc.Pin(device, fun native ->
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
            for i in (GLSLang.SpirV.Module.ofArray result).instructions do
                printfn "%A" i

            device.CreateSpirVShaderModule { Label = descriptor.Label; Code = result } |> Ok
        | None ->
            Error log
            
    [<Extension>]
    static member CreateGLSLShaderModule(device : Device, descriptor : GLSLShaderModuleDescriptor) =
        match device.TryCreateGLSLShaderModule descriptor with
        | Ok module_ -> module_
        | Error err -> failwithf "ShaderCompiler failed: %A" err

        
    [<Extension>]
    static member WriteBuffer<'a when 'a : unmanaged>(queue : Queue, buffer : Buffer, bufferOffset : int64, data : 'a[], index : int, count : int) =
        use pData = fixed data
        let sa = sizeof<'a>
        let size = unativeint sa * unativeint count
        let ptr = NativePtr.toNativeInt pData + nativeint sa * nativeint index
        queue.WriteBuffer(buffer, uint64 bufferOffset, ptr, size)
        
    [<Extension>]
    static member inline WriteBuffer<'a when 'a : unmanaged>(queue : Queue, buffer : Buffer, data : 'a[], index : int, count : int) =
        queue.WriteBuffer(buffer, 0L, data, index, count)
        
    [<Extension>]
    static member inline WriteBuffer<'a when 'a : unmanaged>(queue : Queue, buffer : Buffer, data : 'a[], count : int) =
        queue.WriteBuffer(buffer, 0L, data, 0, count)
        
    [<Extension>]
    static member inline WriteBuffer<'a when 'a : unmanaged>(queue : Queue, buffer : Buffer, data : 'a[]) =
        queue.WriteBuffer(buffer, 0L, data, 0, data.Length)
        
    [<Extension>]
    static member Upload(dst : Buffer, src : Data) =
        let queue = dst.Device.GetDeviceTransferQueue()
        queue.Upload(src, dst, 0L, src.Size)
        
    [<Extension>]
    static member Download(src : Buffer, dst : 'a[]) =
        let queue = src.Device.GetDeviceTransferQueue()
        queue.Download(src, 0L, Span dst, dst.Length)


    //static member ReadBuffer<'a when 'a : unmanaged>(queue : Queue, buffer : Buffer)
