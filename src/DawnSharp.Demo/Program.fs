// Learn more about F# at http://fsharp.org

open System
open WebGPU
open Microsoft.FSharp.NativeInterop
open System.Runtime.InteropServices

#nowarn "9"

type AdapterHandle = struct val mutable public Handle : nativeint end

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
        Name : string
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

            {
                PipelineStatisticsQuery = info.PipelineStatisticsQuery <> 0
                ShaderFloat16 = info.ShaderFloat16 <> 0
                TextureCompressionBC = info.TextureCompressionBC <> 0
                TimestampQuery = info.TimestampQuery <> 0
                BackendType = typ
                DeviceType = info.DeviceType
                DeviceId = info.DeviceId
                VendorId = info.VendorId
                Name = Marshal.PtrToStringAnsi info.Name
            }
        


    [<DllImport("dawn")>]
    extern InstanceHandle dawnNewInstance()
    
    [<DllImport("dawn")>]
    extern int dawnDiscoverDefaultAdapters(InstanceHandle instance, int bufSize, AdapterHandle* adapters)
    
    [<DllImport("dawn")>]
    extern DawnAdapterInfo dawnGetAdapterInfo(AdapterHandle handle)
    
    [<DllImport("dawn")>]
    extern void dawnFreeAdapterInfo(DawnAdapterInfo handle)
    
    [<DllImport("dawn")>]
    extern DeviceHandle dawnCreateDevice(AdapterHandle handle)
    
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
    
    member x.Handle = handle
    member x.Info = info.Value

    member x.Name = x.Info.Name
    member x.DeviceType = x.Info.DeviceType
    member x.BackendType = x.Info.BackendType

    member x.CreateDevice() =
        let dh = DawnRaw.dawnCreateDevice(handle)
        Device(dh)

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

    member x.GetDefaultAdapters() =
        let arr = Array.zeroCreate 128
        use ptr = fixed arr
        let cnt = DawnRaw.dawnDiscoverDefaultAdapters(handle, 128, ptr)

        Array.init cnt (fun i ->
            let h = arr.[i]
            new Adapter(h)
        )

        
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
        let f = x.CreateFence { Label = null; InitialValue = 0UL }
        x.Signal(f, 1UL)
        f.OnCompletion(1UL, FenceOnCompletionCallback(fun status _ ->
            Volatile.Write(&res, int status)
        ), 0n)
        let mutable iter = 0
        while Volatile.Read(&res) = Int32.MaxValue do    
            x.Device.Tick()
            iter <- iter + 1
        let status = Volatile.Read(&res) |> unbox<FenceCompletionStatus>
        f.Release()

        if status <> FenceCompletionStatus.Success then raise <| QueueWaitFailed status


let run(device : Device) =
    let size = 32 <<< 20
    let src = 
        device.CreateBuffer {
            Label = "a"
            Size = uint64 size    
            Usage = BufferUsage.MapWrite ||| BufferUsage.CopySrc
            MappedAtCreation = 1
        }
        
    let real = 
        device.CreateBuffer {
            Label = "r"
            Size = uint64 size        
            Usage = BufferUsage.Vertex ||| BufferUsage.CopyDst ||| BufferUsage.CopySrc
            MappedAtCreation = 0
        }

    let dst =
        device.CreateBuffer {
            Label = "b"
            Size = uint64 size        
            Usage = BufferUsage.MapRead ||| BufferUsage.CopyDst
            MappedAtCreation = 0
        }
            
    let input = Array.init size byte
    let ptr = src.GetMappedRange(0un, unativeint size) 
    Marshal.Copy(input, 0, ptr, size)
    src.Unmap()


    let q = device.GetDefaultQueue()
    let buf = 
        let cmd = device.CreateCommandEncoder { Label = null }
        cmd.CopyBufferToBuffer(src, 0UL, real, 0UL, uint64 size)
        cmd.CopyBufferToBuffer(real, 0UL, dst, 0UL, uint64 size)

        //cmd.CopyBufferToTexture(
        //    { 
        //        Buffer = src
        //        Layout = { Offset = 0UL; BytesPerRow = 4096u; RowsPerImage = 1024u } 
        //    },
        //    { 
        //        Texture = failwith ""
        //        MipLevel = 0u; Origin = { X = 0u; Y = 0u; Z = 0u }; Aspect = TextureAspect.All 
        //    },
        //    { Width = 1024u; Height = 1024u; Depth = 1u }
        //)

        cmd.Finish { Label = null }

    q.Submit [| buf |]
    buf.Release()
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
    src.Release()
    dst.Release()
    real.Release()


[<EntryPoint>]
let main argv =
    let instance = Instance()
    let adapters = instance.GetDefaultAdapters()

    //for a in adapters do
    //    printfn "%s" a.Name
    //    printfn "  %A" a.BackendType
    //    printfn "  %A" a.DeviceType

    for a in [| adapters.[1] |] do
        match a.BackendType with
        | BackendType.Null -> 
            printfn "%s" a.Name
        | _ -> 
            let dev = a.CreateDevice()
            dev.SetUncapturedErrorCallback(ErrorCallback(fun err str _ -> 
                let message = System.Runtime.InteropServices.Marshal.PtrToStringAnsi (NativePtr.toNativeInt str)
                printfn "%A: %s" err message
            ), 0n)
            |> ignore
            printfn "%s (%A) " a.Name a.BackendType


            for i in 1 .. 10 do
                printf "  %d " i
                run dev
    
    0 // return an integer exit code
