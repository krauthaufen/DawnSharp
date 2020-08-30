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


type Buffer with
    member x.Map(mode, offset, size, d : Device) =
        Async.FromContinuations (fun (s,e,c) ->
            x.MapAsync(mode, offset, size, BufferMapCallback(fun status _ ->
                match status with
                | BufferMapAsyncStatus.Success -> 
                    s (x.GetMappedRange(0un, 0xffffffffffffffffun))
                | _ ->
                    
                    e (Exception "could not map buffer")
            ), 0n)
        )

type Queue with
    member x.Wait(d : Device) =
        Async.FromContinuations(fun (s, e, c) ->
            let f = x.CreateFence { Label = null; InitialValue = 0UL }
            x.Signal(f, 1UL)
            f.OnCompletion(1UL, FenceOnCompletionCallback(fun status _ ->
                
                s status
            ), 0n)

        )

let run(device : Device) =

    let thread =
        System.Threading.Thread(System.Threading.ThreadStart(fun () ->
            while true do
                device.Tick()
        ), IsBackground = true)
    thread.SetApartmentState(Threading.ApartmentState.STA)
    thread.Start()

    async {
        let src = 
            device.CreateBuffer {
                Label = "a"
                Size = 4096UL    
                Usage = BufferUsage.MapWrite ||| BufferUsage.CopySrc
                MappedAtCreation = 1
            }
            

        let dst =
            device.CreateBuffer {
                Label = "b"
                Size = 4096UL    
                Usage = BufferUsage.MapRead ||| BufferUsage.CopyDst
                MappedAtCreation = 0
            }
            

        let ptr = src.GetMappedRange(0un, 4096un) 
        Marshal.Copy(Array.init 4096 byte, 0, ptr, 4096)
        src.Unmap()

        let q = device.GetDefaultQueue()
        let buf = 
            let cmd = device.CreateCommandEncoder { Label = null }
            cmd.CopyBufferToBuffer(src, 0UL, dst, 0UL, 4096UL)
            cmd.Finish { Label = "buffy" }

        q.Submit(1u, [| buf |])
        
        let! r = q.Wait(device)
        printfn "done: %A" r

        let! ptr = dst.Map(MapMode.Read, 0un, 4096un, device)
        let arr : byte[] = Array.zeroCreate 4096
        Marshal.Copy(ptr, arr, 0, arr.Length)
        dst.Unmap()

        printfn "%A" arr

        dst.Destroy()
        src.Destroy()
    }


[<EntryPoint; STAThread>]
let main argv =
    let instance = Instance()

    let adapters = instance.GetDefaultAdapters()


    for a in adapters do
        printfn "%s" a.Name
        printfn "  %A" a.BackendType
        printfn "  %A" a.DeviceType

    let dev = adapters.[2].CreateDevice()
    dev.SetUncapturedErrorCallback(ErrorCallback(fun err str _ -> 
        let message = System.Runtime.InteropServices.Marshal.PtrToStringAnsi (NativePtr.toNativeInt str)
        printfn "%A: %s" err message
    ), 0n)
    |> ignore
    run dev |> Async.RunSynchronously

    0 // return an integer exit code
