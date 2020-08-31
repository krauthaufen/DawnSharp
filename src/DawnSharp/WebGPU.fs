namespace rec WebGPU
open System

#nowarn "49"
#nowarn "9"

type WGPUAdapterProperties =
    struct
        val mutable public pNext : nativeint
        val mutable public DeviceID : uint32
        val mutable public VendorID : uint32
        val mutable public Name : nativeptr<byte>
        val mutable public AdapterType : AdapterType
        val mutable public BackendType : BackendType
    end
[<Struct>]
type AdapterProperties =
    {
        DeviceID : uint32
        VendorID : uint32
        Name : string
        AdapterType : AdapterType
        BackendType : BackendType
    }
    member x.Pin<'a>(action : WGPUAdapterProperties -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUAdapterProperties>
        native.DeviceID <- x.DeviceID
        native.VendorID <- x.VendorID
        let pName = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Name
        try
            native.Name <- Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt pName
            native.AdapterType <- x.AdapterType
            native.BackendType <- x.BackendType
            native.pNext <- 0n
            action native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal pName
type AdapterType =
    | DiscreteGPU = 0
    | IntegratedGPU = 1
    | CPU = 2
    | Unknown = 3
type AddressMode =
    | Repeat = 0
    | MirrorRepeat = 1
    | ClampToEdge = 2
type BackendType =
    | Null = 0
    | D3D11 = 1
    | D3D12 = 2
    | Metal = 3
    | Vulkan = 4
    | OpenGL = 5
    | OpenGLES = 6
type WGPUBindGroupDescriptor =
    struct
        val mutable public pNext : nativeint
        val mutable public Label : nativeptr<byte>
        val mutable public Layout : BindGroupLayoutHandle
        val mutable public EntryCount : uint32
        val mutable public Entries : nativeptr<WGPUBindGroupEntry>
    end
[<Struct>]
type BindGroupDescriptor =
    {
        Label : string
        Layout : BindGroupLayout
        Entries : BindGroupEntry[]
    }
    member x.Pin<'a>(action : WGPUBindGroupDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUBindGroupDescriptor>
        let pLabel = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            native.Label <- Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt pLabel
            native.Layout <- x.Layout.Handle
            native.EntryCount <- (if isNull x.Entries then 0u else uint32 x.Entries.Length)
            let rec pinEntries (a : array<BindGroupEntry>) (p : array<_>) (i : int) =
                if i >= a.Length then
                    use p = fixed p
                    native.Entries <- p
                    native.pNext <- 0n
                    action native
                else
                    a.[i].Pin(fun ai -> p.[i] <- ai; pinEntries a p (i+1))
            pinEntries x.Entries (Array.zeroCreate x.Entries.Length) 0
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal pLabel
type WGPUBindGroupEntry =
    struct
        val mutable public Binding : uint32
        val mutable public Buffer : BufferHandle
        val mutable public Offset : uint64
        val mutable public Size : uint64
        val mutable public Sampler : SamplerHandle
        val mutable public TextureView : TextureViewHandle
    end
[<Struct>]
type BindGroupEntry =
    {
        Binding : uint32
        Buffer : Buffer
        Offset : uint64
        Size : uint64
        Sampler : Sampler
        TextureView : TextureView
    }
    member x.Pin<'a>(action : WGPUBindGroupEntry -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUBindGroupEntry>
        native.Binding <- x.Binding
        native.Buffer <- x.Buffer.Handle
        native.Offset <- x.Offset
        native.Size <- x.Size
        native.Sampler <- x.Sampler.Handle
        native.TextureView <- x.TextureView.Handle
        action native
type WGPUBindGroupLayoutDescriptor =
    struct
        val mutable public pNext : nativeint
        val mutable public Label : nativeptr<byte>
        val mutable public EntryCount : uint32
        val mutable public Entries : nativeptr<WGPUBindGroupLayoutEntry>
    end
[<Struct>]
type BindGroupLayoutDescriptor =
    {
        Label : string
        Entries : BindGroupLayoutEntry[]
    }
    member x.Pin<'a>(action : WGPUBindGroupLayoutDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUBindGroupLayoutDescriptor>
        let pLabel = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            native.Label <- Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt pLabel
            native.EntryCount <- (if isNull x.Entries then 0u else uint32 x.Entries.Length)
            let rec pinEntries (a : array<BindGroupLayoutEntry>) (p : array<_>) (i : int) =
                if i >= a.Length then
                    use p = fixed p
                    native.Entries <- p
                    native.pNext <- 0n
                    action native
                else
                    a.[i].Pin(fun ai -> p.[i] <- ai; pinEntries a p (i+1))
            pinEntries x.Entries (Array.zeroCreate x.Entries.Length) 0
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal pLabel
type WGPUBindGroupLayoutEntry =
    struct
        val mutable public Binding : uint32
        val mutable public Visibility : ShaderStage
        val mutable public Type : BindingType
        val mutable public HasDynamicOffset : int
        val mutable public MinBufferBindingSize : uint64
        val mutable public Multisampled : int
        val mutable public ViewDimension : TextureViewDimension
        val mutable public TextureComponentType : TextureComponentType
        val mutable public StorageTextureFormat : TextureFormat
    end
[<Struct>]
type BindGroupLayoutEntry =
    {
        Binding : uint32
        Visibility : ShaderStage
        Type : BindingType
        HasDynamicOffset : int
        MinBufferBindingSize : uint64
        Multisampled : int
        ViewDimension : TextureViewDimension
        TextureComponentType : TextureComponentType
        StorageTextureFormat : TextureFormat
    }
    member x.Pin<'a>(action : WGPUBindGroupLayoutEntry -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUBindGroupLayoutEntry>
        native.Binding <- x.Binding
        native.Visibility <- x.Visibility
        native.Type <- x.Type
        native.HasDynamicOffset <- x.HasDynamicOffset
        native.MinBufferBindingSize <- x.MinBufferBindingSize
        native.Multisampled <- x.Multisampled
        native.ViewDimension <- x.ViewDimension
        native.TextureComponentType <- x.TextureComponentType
        native.StorageTextureFormat <- x.StorageTextureFormat
        action native
type BindingType =
    | UniformBuffer = 0
    | StorageBuffer = 1
    | ReadonlyStorageBuffer = 2
    | Sampler = 3
    | ComparisonSampler = 4
    | SampledTexture = 5
    | ReadonlyStorageTexture = 6
    | WriteonlyStorageTexture = 7
    | StorageTexture = 8
type WGPUBlendDescriptor =
    struct
        val mutable public Operation : BlendOperation
        val mutable public SrcFactor : BlendFactor
        val mutable public DstFactor : BlendFactor
    end
[<Struct>]
type BlendDescriptor =
    {
        Operation : BlendOperation
        SrcFactor : BlendFactor
        DstFactor : BlendFactor
    }
    member x.Pin<'a>(action : WGPUBlendDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUBlendDescriptor>
        native.Operation <- x.Operation
        native.SrcFactor <- x.SrcFactor
        native.DstFactor <- x.DstFactor
        action native
type BlendFactor =
    | Zero = 0
    | One = 1
    | SrcColor = 2
    | OneMinusSrcColor = 3
    | SrcAlpha = 4
    | OneMinusSrcAlpha = 5
    | DstColor = 6
    | OneMinusDstColor = 7
    | DstAlpha = 8
    | OneMinusDstAlpha = 9
    | SrcAlphaSaturated = 10
    | BlendColor = 11
    | OneMinusBlendColor = 12
type BlendOperation =
    | Add = 0
    | Subtract = 1
    | ReverseSubtract = 2
    | Min = 3
    | Max = 4
type WGPUBufferCopyView =
    struct
        val mutable public pNext : nativeint
        val mutable public Layout : WGPUTextureDataLayout
        val mutable public Buffer : BufferHandle
    end
[<Struct>]
type BufferCopyView =
    {
        Layout : TextureDataLayout
        Buffer : Buffer
    }
    member x.Pin<'a>(action : WGPUBufferCopyView -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUBufferCopyView>
        x.Layout.Pin(fun _Layout ->
            native.Layout <- _Layout
            native.Buffer <- x.Buffer.Handle
            native.pNext <- 0n
            action native
        )
type WGPUBufferDescriptor =
    struct
        val mutable public pNext : nativeint
        val mutable public Label : nativeptr<byte>
        val mutable public Usage : BufferUsage
        val mutable public Size : uint64
        val mutable public MappedAtCreation : int
    end
[<Struct>]
type BufferDescriptor =
    {
        Label : string
        Usage : BufferUsage
        Size : uint64
        MappedAtCreation : int
    }
    member x.Pin<'a>(action : WGPUBufferDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUBufferDescriptor>
        let pLabel = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            native.Label <- Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt pLabel
            native.Usage <- x.Usage
            native.Size <- x.Size
            native.MappedAtCreation <- x.MappedAtCreation
            native.pNext <- 0n
            action native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal pLabel
type BufferMapAsyncStatus =
    | Success = 0
    | Error = 1
    | Unknown = 2
    | DeviceLost = 3
type BufferMapCallback = delegate of BufferMapAsyncStatus * nativeint -> unit
[<Flags>]
type BufferUsage =
    | None = 0
    | MapRead = 1
    | MapWrite = 2
    | CopySrc = 4
    | CopyDst = 8
    | Index = 16
    | Vertex = 32
    | Uniform = 64
    | Storage = 128
    | Indirect = 256
    | QueryResolve = 512
type WGPUColor =
    struct
        val mutable public R : float32
        val mutable public G : float32
        val mutable public B : float32
        val mutable public A : float32
    end
[<Struct>]
type Color =
    {
        R : float32
        G : float32
        B : float32
        A : float32
    }
    member x.Pin<'a>(action : WGPUColor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUColor>
        native.R <- x.R
        native.G <- x.G
        native.B <- x.B
        native.A <- x.A
        action native
type WGPUColorStateDescriptor =
    struct
        val mutable public pNext : nativeint
        val mutable public Format : TextureFormat
        val mutable public AlphaBlend : WGPUBlendDescriptor
        val mutable public ColorBlend : WGPUBlendDescriptor
        val mutable public WriteMask : ColorWriteMask
    end
[<Struct>]
type ColorStateDescriptor =
    {
        Format : TextureFormat
        AlphaBlend : BlendDescriptor
        ColorBlend : BlendDescriptor
        WriteMask : ColorWriteMask
    }
    member x.Pin<'a>(action : WGPUColorStateDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUColorStateDescriptor>
        native.Format <- x.Format
        x.AlphaBlend.Pin(fun _AlphaBlend ->
            native.AlphaBlend <- _AlphaBlend
            x.ColorBlend.Pin(fun _ColorBlend ->
                native.ColorBlend <- _ColorBlend
                native.WriteMask <- x.WriteMask
                native.pNext <- 0n
                action native
            )
        )
[<Flags>]
type ColorWriteMask =
    | None = 0
    | Red = 1
    | Green = 2
    | Blue = 4
    | Alpha = 8
    | All = 15
type WGPUCommandBufferDescriptor =
    struct
        val mutable public pNext : nativeint
        val mutable public Label : nativeptr<byte>
    end
[<Struct>]
type CommandBufferDescriptor =
    {
        Label : string
    }
    member x.Pin<'a>(action : WGPUCommandBufferDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUCommandBufferDescriptor>
        let pLabel = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            native.Label <- Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt pLabel
            native.pNext <- 0n
            action native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal pLabel
type WGPUCommandEncoderDescriptor =
    struct
        val mutable public pNext : nativeint
        val mutable public Label : nativeptr<byte>
    end
[<Struct>]
type CommandEncoderDescriptor =
    {
        Label : string
    }
    member x.Pin<'a>(action : WGPUCommandEncoderDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUCommandEncoderDescriptor>
        let pLabel = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            native.Label <- Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt pLabel
            native.pNext <- 0n
            action native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal pLabel
type CompareFunction =
    | Undefined = 0
    | Never = 1
    | Less = 2
    | LessEqual = 3
    | Greater = 4
    | GreaterEqual = 5
    | Equal = 6
    | NotEqual = 7
    | Always = 8
type WGPUComputePassDescriptor =
    struct
        val mutable public pNext : nativeint
        val mutable public Label : nativeptr<byte>
    end
[<Struct>]
type ComputePassDescriptor =
    {
        Label : string
    }
    member x.Pin<'a>(action : WGPUComputePassDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUComputePassDescriptor>
        let pLabel = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            native.Label <- Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt pLabel
            native.pNext <- 0n
            action native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal pLabel
type WGPUComputePipelineDescriptor =
    struct
        val mutable public pNext : nativeint
        val mutable public Label : nativeptr<byte>
        val mutable public Layout : PipelineLayoutHandle
        val mutable public ComputeStage : WGPUProgrammableStageDescriptor
    end
[<Struct>]
type ComputePipelineDescriptor =
    {
        Label : string
        Layout : PipelineLayout
        ComputeStage : ProgrammableStageDescriptor
    }
    member x.Pin<'a>(action : WGPUComputePipelineDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUComputePipelineDescriptor>
        let pLabel = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            native.Label <- Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt pLabel
            native.Layout <- x.Layout.Handle
            x.ComputeStage.Pin(fun _ComputeStage ->
                native.ComputeStage <- _ComputeStage
                native.pNext <- 0n
                action native
            )
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal pLabel
type CullMode =
    | None = 0
    | Front = 1
    | Back = 2
type WGPUDepthStencilStateDescriptor =
    struct
        val mutable public pNext : nativeint
        val mutable public Format : TextureFormat
        val mutable public DepthWriteEnabled : int
        val mutable public DepthCompare : CompareFunction
        val mutable public StencilFront : WGPUStencilStateFaceDescriptor
        val mutable public StencilBack : WGPUStencilStateFaceDescriptor
        val mutable public StencilReadMask : uint32
        val mutable public StencilWriteMask : uint32
    end
[<Struct>]
type DepthStencilStateDescriptor =
    {
        Format : TextureFormat
        DepthWriteEnabled : int
        DepthCompare : CompareFunction
        StencilFront : StencilStateFaceDescriptor
        StencilBack : StencilStateFaceDescriptor
        StencilReadMask : uint32
        StencilWriteMask : uint32
    }
    member x.Pin<'a>(action : WGPUDepthStencilStateDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUDepthStencilStateDescriptor>
        native.Format <- x.Format
        native.DepthWriteEnabled <- x.DepthWriteEnabled
        native.DepthCompare <- x.DepthCompare
        x.StencilFront.Pin(fun _StencilFront ->
            native.StencilFront <- _StencilFront
            x.StencilBack.Pin(fun _StencilBack ->
                native.StencilBack <- _StencilBack
                native.StencilReadMask <- x.StencilReadMask
                native.StencilWriteMask <- x.StencilWriteMask
                native.pNext <- 0n
                action native
            )
        )
type DeviceLostCallback = delegate of nativeptr<byte> * nativeint -> unit
type WGPUDeviceProperties =
    struct
        val mutable public TextureCompressionBC : int
        val mutable public ShaderFloat16 : int
        val mutable public PipelineStatisticsQuery : int
        val mutable public TimestampQuery : int
    end
[<Struct>]
type DeviceProperties =
    {
        TextureCompressionBC : int
        ShaderFloat16 : int
        PipelineStatisticsQuery : int
        TimestampQuery : int
    }
    member x.Pin<'a>(action : WGPUDeviceProperties -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUDeviceProperties>
        native.TextureCompressionBC <- x.TextureCompressionBC
        native.ShaderFloat16 <- x.ShaderFloat16
        native.PipelineStatisticsQuery <- x.PipelineStatisticsQuery
        native.TimestampQuery <- x.TimestampQuery
        action native
type ErrorCallback = delegate of ErrorType * nativeptr<byte> * nativeint -> unit
type ErrorFilter =
    | None = 0
    | Validation = 1
    | OutOfMemory = 2
type ErrorType =
    | NoError = 0
    | Validation = 1
    | OutOfMemory = 2
    | Unknown = 3
    | DeviceLost = 4
type WGPUExtent3D =
    struct
        val mutable public Width : uint32
        val mutable public Height : uint32
        val mutable public Depth : uint32
    end
[<Struct>]
type Extent3D =
    {
        Width : uint32
        Height : uint32
        Depth : uint32
    }
    member x.Pin<'a>(action : WGPUExtent3D -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUExtent3D>
        native.Width <- x.Width
        native.Height <- x.Height
        native.Depth <- x.Depth
        action native
type FenceCompletionStatus =
    | Success = 0
    | Error = 1
    | Unknown = 2
    | DeviceLost = 3
type WGPUFenceDescriptor =
    struct
        val mutable public pNext : nativeint
        val mutable public Label : nativeptr<byte>
        val mutable public InitialValue : uint64
    end
[<Struct>]
type FenceDescriptor =
    {
        Label : string
        InitialValue : uint64
    }
    member x.Pin<'a>(action : WGPUFenceDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUFenceDescriptor>
        let pLabel = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            native.Label <- Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt pLabel
            native.InitialValue <- x.InitialValue
            native.pNext <- 0n
            action native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal pLabel
type FenceOnCompletionCallback = delegate of FenceCompletionStatus * nativeint -> unit
type FilterMode =
    | Nearest = 0
    | Linear = 1
type FrontFace =
    | CCW = 0
    | CW = 1
type IndexFormat =
    | Undefined = 0
    | Uint16 = 1
    | Uint32 = 2
type InputStepMode =
    | Vertex = 0
    | Instance = 1
type WGPUInstanceDescriptor =
    struct
        val mutable public pNext : nativeint
    end
type InstanceDescriptor private () =
    static let instance = InstanceDescriptor()
    static member Instance = instance
    member x.Pin<'a>(action : WGPUInstanceDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUInstanceDescriptor>
        native.pNext <- 0n
        action native
type LoadOp =
    | Clear = 0
    | Load = 1
[<Flags>]
type MapMode =
    | None = 0
    | Read = 1
    | Write = 2
type WGPUOrigin3D =
    struct
        val mutable public X : uint32
        val mutable public Y : uint32
        val mutable public Z : uint32
    end
[<Struct>]
type Origin3D =
    {
        X : uint32
        Y : uint32
        Z : uint32
    }
    member x.Pin<'a>(action : WGPUOrigin3D -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUOrigin3D>
        native.X <- x.X
        native.Y <- x.Y
        native.Z <- x.Z
        action native
type WGPUPipelineLayoutDescriptor =
    struct
        val mutable public pNext : nativeint
        val mutable public Label : nativeptr<byte>
        val mutable public BindGroupLayoutCount : uint32
        val mutable public BindGroupLayouts : nativeptr<BindGroupLayoutHandle>
    end
[<Struct>]
type PipelineLayoutDescriptor =
    {
        Label : string
        BindGroupLayouts : BindGroupLayout[]
    }
    member x.Pin<'a>(action : WGPUPipelineLayoutDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUPipelineLayoutDescriptor>
        let pLabel = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            native.Label <- Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt pLabel
            native.BindGroupLayoutCount <- (if isNull x.BindGroupLayouts then 0u else uint32 x.BindGroupLayouts.Length)
            let _BindGroupLayouts = x.BindGroupLayouts |> Array.map (fun a -> a.Handle)
            use _BindGroupLayouts = fixed _BindGroupLayouts
            native.BindGroupLayouts <- _BindGroupLayouts
            native.pNext <- 0n
            action native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal pLabel
type PipelineStatisticName =
    | VertexShaderInvocations = 0
    | ClipperInvocations = 1
    | ClipperPrimitivesOut = 2
    | FragmentShaderInvocations = 3
    | ComputeShaderInvocations = 4
type PresentMode =
    | Immediate = 0
    | Mailbox = 1
    | Fifo = 2
type PrimitiveTopology =
    | PointList = 0
    | LineList = 1
    | LineStrip = 2
    | TriangleList = 3
    | TriangleStrip = 4
type WGPUProgrammableStageDescriptor =
    struct
        val mutable public pNext : nativeint
        val mutable public Module : ShaderModuleHandle
        val mutable public EntryPoint : nativeptr<byte>
    end
[<Struct>]
type ProgrammableStageDescriptor =
    {
        Module : ShaderModule
        EntryPoint : string
    }
    member x.Pin<'a>(action : WGPUProgrammableStageDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUProgrammableStageDescriptor>
        native.Module <- x.Module.Handle
        let pEntryPoint = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.EntryPoint
        try
            native.EntryPoint <- Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt pEntryPoint
            native.pNext <- 0n
            action native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal pEntryPoint
type WGPUQuerySetDescriptor =
    struct
        val mutable public pNext : nativeint
        val mutable public Label : nativeptr<byte>
        val mutable public Type : QueryType
        val mutable public Count : uint32
        val mutable public PipelineStatistics : nativeptr<PipelineStatisticName>
        val mutable public PipelineStatisticsCount : uint32
    end
[<Struct>]
type QuerySetDescriptor =
    {
        Label : string
        Type : QueryType
        Count : uint32
        PipelineStatistics : PipelineStatisticName[]
        PipelineStatisticsCount : uint32
    }
    member x.Pin<'a>(action : WGPUQuerySetDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUQuerySetDescriptor>
        let pLabel = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            native.Label <- Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt pLabel
            native.Type <- x.Type
            native.Count <- x.Count
            use _PipelineStatistics = fixed x.PipelineStatistics
            native.PipelineStatistics <- _PipelineStatistics
            native.PipelineStatisticsCount <- x.PipelineStatisticsCount
            native.pNext <- 0n
            action native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal pLabel
type QueryType =
    | Occlusion = 0
    | PipelineStatistics = 1
    | Timestamp = 2
type WGPURasterizationStateDescriptor =
    struct
        val mutable public pNext : nativeint
        val mutable public FrontFace : FrontFace
        val mutable public CullMode : CullMode
        val mutable public DepthBias : int
        val mutable public DepthBiasSlopeScale : float32
        val mutable public DepthBiasClamp : float32
    end
[<Struct>]
type RasterizationStateDescriptor =
    {
        FrontFace : FrontFace
        CullMode : CullMode
        DepthBias : int
        DepthBiasSlopeScale : float32
        DepthBiasClamp : float32
    }
    member x.Pin<'a>(action : WGPURasterizationStateDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPURasterizationStateDescriptor>
        native.FrontFace <- x.FrontFace
        native.CullMode <- x.CullMode
        native.DepthBias <- x.DepthBias
        native.DepthBiasSlopeScale <- x.DepthBiasSlopeScale
        native.DepthBiasClamp <- x.DepthBiasClamp
        native.pNext <- 0n
        action native
type WGPURenderBundleDescriptor =
    struct
        val mutable public pNext : nativeint
        val mutable public Label : nativeptr<byte>
    end
[<Struct>]
type RenderBundleDescriptor =
    {
        Label : string
    }
    member x.Pin<'a>(action : WGPURenderBundleDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPURenderBundleDescriptor>
        let pLabel = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            native.Label <- Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt pLabel
            native.pNext <- 0n
            action native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal pLabel
type WGPURenderBundleEncoderDescriptor =
    struct
        val mutable public pNext : nativeint
        val mutable public Label : nativeptr<byte>
        val mutable public ColorFormatsCount : uint32
        val mutable public ColorFormats : nativeptr<TextureFormat>
        val mutable public DepthStencilFormat : TextureFormat
        val mutable public SampleCount : uint32
    end
[<Struct>]
type RenderBundleEncoderDescriptor =
    {
        Label : string
        ColorFormats : TextureFormat[]
        DepthStencilFormat : TextureFormat
        SampleCount : uint32
    }
    member x.Pin<'a>(action : WGPURenderBundleEncoderDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPURenderBundleEncoderDescriptor>
        let pLabel = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            native.Label <- Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt pLabel
            native.ColorFormatsCount <- (if isNull x.ColorFormats then 0u else uint32 x.ColorFormats.Length)
            use _ColorFormats = fixed x.ColorFormats
            native.ColorFormats <- _ColorFormats
            native.DepthStencilFormat <- x.DepthStencilFormat
            native.SampleCount <- x.SampleCount
            native.pNext <- 0n
            action native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal pLabel
type WGPURenderPassColorAttachmentDescriptor =
    struct
        val mutable public Attachment : TextureViewHandle
        val mutable public ResolveTarget : TextureViewHandle
        val mutable public LoadOp : LoadOp
        val mutable public StoreOp : StoreOp
        val mutable public ClearColor : WGPUColor
    end
[<Struct>]
type RenderPassColorAttachmentDescriptor =
    {
        Attachment : TextureView
        ResolveTarget : TextureView
        LoadOp : LoadOp
        StoreOp : StoreOp
        ClearColor : Color
    }
    member x.Pin<'a>(action : WGPURenderPassColorAttachmentDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPURenderPassColorAttachmentDescriptor>
        native.Attachment <- x.Attachment.Handle
        native.ResolveTarget <- x.ResolveTarget.Handle
        native.LoadOp <- x.LoadOp
        native.StoreOp <- x.StoreOp
        x.ClearColor.Pin(fun _ClearColor ->
            native.ClearColor <- _ClearColor
            action native
        )
type WGPURenderPassDepthStencilAttachmentDescriptor =
    struct
        val mutable public Attachment : TextureViewHandle
        val mutable public DepthLoadOp : LoadOp
        val mutable public DepthStoreOp : StoreOp
        val mutable public ClearDepth : float32
        val mutable public DepthReadOnly : int
        val mutable public StencilLoadOp : LoadOp
        val mutable public StencilStoreOp : StoreOp
        val mutable public ClearStencil : uint32
        val mutable public StencilReadOnly : int
    end
[<Struct>]
type RenderPassDepthStencilAttachmentDescriptor =
    {
        Attachment : TextureView
        DepthLoadOp : LoadOp
        DepthStoreOp : StoreOp
        ClearDepth : float32
        DepthReadOnly : int
        StencilLoadOp : LoadOp
        StencilStoreOp : StoreOp
        ClearStencil : uint32
        StencilReadOnly : int
    }
    member x.Pin<'a>(action : WGPURenderPassDepthStencilAttachmentDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPURenderPassDepthStencilAttachmentDescriptor>
        native.Attachment <- x.Attachment.Handle
        native.DepthLoadOp <- x.DepthLoadOp
        native.DepthStoreOp <- x.DepthStoreOp
        native.ClearDepth <- x.ClearDepth
        native.DepthReadOnly <- x.DepthReadOnly
        native.StencilLoadOp <- x.StencilLoadOp
        native.StencilStoreOp <- x.StencilStoreOp
        native.ClearStencil <- x.ClearStencil
        native.StencilReadOnly <- x.StencilReadOnly
        action native
type WGPURenderPassDescriptor =
    struct
        val mutable public pNext : nativeint
        val mutable public Label : nativeptr<byte>
        val mutable public ColorAttachmentCount : uint32
        val mutable public ColorAttachments : nativeptr<WGPURenderPassColorAttachmentDescriptor>
        val mutable public DepthStencilAttachment : nativeptr<WGPURenderPassDepthStencilAttachmentDescriptor>
        val mutable public OcclusionQuerySet : QuerySetHandle
    end
[<Struct>]
type RenderPassDescriptor =
    {
        Label : string
        ColorAttachments : RenderPassColorAttachmentDescriptor[]
        DepthStencilAttachment : RenderPassDepthStencilAttachmentDescriptor[]
        OcclusionQuerySet : QuerySet
    }
    member x.Pin<'a>(action : WGPURenderPassDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPURenderPassDescriptor>
        let pLabel = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            native.Label <- Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt pLabel
            native.ColorAttachmentCount <- (if isNull x.ColorAttachments then 0u else uint32 x.ColorAttachments.Length)
            let rec pinColorAttachments (a : array<RenderPassColorAttachmentDescriptor>) (p : array<_>) (i : int) =
                if i >= a.Length then
                    use p = fixed p
                    native.ColorAttachments <- p
                    let rec pinDepthStencilAttachment (a : array<RenderPassDepthStencilAttachmentDescriptor>) (p : array<_>) (i : int) =
                        if i >= a.Length then
                            use p = fixed p
                            native.DepthStencilAttachment <- p
                            native.OcclusionQuerySet <- x.OcclusionQuerySet.Handle
                            native.pNext <- 0n
                            action native
                        else
                            a.[i].Pin(fun ai -> p.[i] <- ai; pinDepthStencilAttachment a p (i+1))
                    pinDepthStencilAttachment x.DepthStencilAttachment (Array.zeroCreate x.DepthStencilAttachment.Length) 0
                else
                    a.[i].Pin(fun ai -> p.[i] <- ai; pinColorAttachments a p (i+1))
            pinColorAttachments x.ColorAttachments (Array.zeroCreate x.ColorAttachments.Length) 0
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal pLabel
type WGPURenderPipelineDescriptor =
    struct
        val mutable public pNext : nativeint
        val mutable public Label : nativeptr<byte>
        val mutable public Layout : PipelineLayoutHandle
        val mutable public VertexStage : WGPUProgrammableStageDescriptor
        val mutable public FragmentStage : nativeptr<WGPUProgrammableStageDescriptor>
        val mutable public VertexState : nativeptr<WGPUVertexStateDescriptor>
        val mutable public PrimitiveTopology : PrimitiveTopology
        val mutable public RasterizationState : nativeptr<WGPURasterizationStateDescriptor>
        val mutable public SampleCount : uint32
        val mutable public DepthStencilState : nativeptr<WGPUDepthStencilStateDescriptor>
        val mutable public ColorStateCount : uint32
        val mutable public ColorStates : nativeptr<WGPUColorStateDescriptor>
        val mutable public SampleMask : uint32
        val mutable public AlphaToCoverageEnabled : int
    end
[<Struct>]
type RenderPipelineDescriptor =
    {
        Label : string
        Layout : PipelineLayout
        VertexStage : ProgrammableStageDescriptor
        FragmentStage : ProgrammableStageDescriptor[]
        VertexState : VertexStateDescriptor[]
        PrimitiveTopology : PrimitiveTopology
        RasterizationState : RasterizationStateDescriptor[]
        SampleCount : uint32
        DepthStencilState : DepthStencilStateDescriptor[]
        ColorStates : ColorStateDescriptor[]
        SampleMask : uint32
        AlphaToCoverageEnabled : int
    }
    member x.Pin<'a>(action : WGPURenderPipelineDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPURenderPipelineDescriptor>
        let pLabel = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            native.Label <- Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt pLabel
            native.Layout <- x.Layout.Handle
            x.VertexStage.Pin(fun _VertexStage ->
                native.VertexStage <- _VertexStage
                let rec pinFragmentStage (a : array<ProgrammableStageDescriptor>) (p : array<_>) (i : int) =
                    if i >= a.Length then
                        use p = fixed p
                        native.FragmentStage <- p
                        let rec pinVertexState (a : array<VertexStateDescriptor>) (p : array<_>) (i : int) =
                            if i >= a.Length then
                                use p = fixed p
                                native.VertexState <- p
                                native.PrimitiveTopology <- x.PrimitiveTopology
                                let rec pinRasterizationState (a : array<RasterizationStateDescriptor>) (p : array<_>) (i : int) =
                                    if i >= a.Length then
                                        use p = fixed p
                                        native.RasterizationState <- p
                                        native.SampleCount <- x.SampleCount
                                        let rec pinDepthStencilState (a : array<DepthStencilStateDescriptor>) (p : array<_>) (i : int) =
                                            if i >= a.Length then
                                                use p = fixed p
                                                native.DepthStencilState <- p
                                                native.ColorStateCount <- (if isNull x.ColorStates then 0u else uint32 x.ColorStates.Length)
                                                let rec pinColorStates (a : array<ColorStateDescriptor>) (p : array<_>) (i : int) =
                                                    if i >= a.Length then
                                                        use p = fixed p
                                                        native.ColorStates <- p
                                                        native.SampleMask <- x.SampleMask
                                                        native.AlphaToCoverageEnabled <- x.AlphaToCoverageEnabled
                                                        native.pNext <- 0n
                                                        action native
                                                    else
                                                        a.[i].Pin(fun ai -> p.[i] <- ai; pinColorStates a p (i+1))
                                                pinColorStates x.ColorStates (Array.zeroCreate x.ColorStates.Length) 0
                                            else
                                                a.[i].Pin(fun ai -> p.[i] <- ai; pinDepthStencilState a p (i+1))
                                        pinDepthStencilState x.DepthStencilState (Array.zeroCreate x.DepthStencilState.Length) 0
                                    else
                                        a.[i].Pin(fun ai -> p.[i] <- ai; pinRasterizationState a p (i+1))
                                pinRasterizationState x.RasterizationState (Array.zeroCreate x.RasterizationState.Length) 0
                            else
                                a.[i].Pin(fun ai -> p.[i] <- ai; pinVertexState a p (i+1))
                        pinVertexState x.VertexState (Array.zeroCreate x.VertexState.Length) 0
                    else
                        a.[i].Pin(fun ai -> p.[i] <- ai; pinFragmentStage a p (i+1))
                pinFragmentStage x.FragmentStage (Array.zeroCreate x.FragmentStage.Length) 0
            )
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal pLabel
type WGPURenderPipelineDescriptorDummyExtension =
    struct
        val mutable public DummyStage : WGPUProgrammableStageDescriptor
    end
[<Struct>]
type RenderPipelineDescriptorDummyExtension =
    {
        DummyStage : ProgrammableStageDescriptor
    }
    member x.Pin<'a>(action : WGPURenderPipelineDescriptorDummyExtension -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPURenderPipelineDescriptorDummyExtension>
        x.DummyStage.Pin(fun _DummyStage ->
            native.DummyStage <- _DummyStage
            action native
        )
type SType =
    | Invalid = 0
    | SurfaceDescriptorFromMetalLayer = 1
    | SurfaceDescriptorFromWindowsHWND = 2
    | SurfaceDescriptorFromXlib = 3
    | SurfaceDescriptorFromCanvasHTMLSelector = 4
    | ShaderModuleSPIRVDescriptor = 5
    | ShaderModuleWGSLDescriptor = 6
    | SamplerDescriptorDummyAnisotropicFiltering = 7
    | RenderPipelineDescriptorDummyExtension = 8
type WGPUSamplerDescriptor =
    struct
        val mutable public pNext : nativeint
        val mutable public Label : nativeptr<byte>
        val mutable public AddressModeU : AddressMode
        val mutable public AddressModeV : AddressMode
        val mutable public AddressModeW : AddressMode
        val mutable public MagFilter : FilterMode
        val mutable public MinFilter : FilterMode
        val mutable public MipmapFilter : FilterMode
        val mutable public LodMinClamp : float32
        val mutable public LodMaxClamp : float32
        val mutable public Compare : CompareFunction
    end
[<Struct>]
type SamplerDescriptor =
    {
        Label : string
        AddressModeU : AddressMode
        AddressModeV : AddressMode
        AddressModeW : AddressMode
        MagFilter : FilterMode
        MinFilter : FilterMode
        MipmapFilter : FilterMode
        LodMinClamp : float32
        LodMaxClamp : float32
        Compare : CompareFunction
    }
    member x.Pin<'a>(action : WGPUSamplerDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUSamplerDescriptor>
        let pLabel = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            native.Label <- Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt pLabel
            native.AddressModeU <- x.AddressModeU
            native.AddressModeV <- x.AddressModeV
            native.AddressModeW <- x.AddressModeW
            native.MagFilter <- x.MagFilter
            native.MinFilter <- x.MinFilter
            native.MipmapFilter <- x.MipmapFilter
            native.LodMinClamp <- x.LodMinClamp
            native.LodMaxClamp <- x.LodMaxClamp
            native.Compare <- x.Compare
            native.pNext <- 0n
            action native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal pLabel
type WGPUSamplerDescriptorDummyAnisotropicFiltering =
    struct
        val mutable public MaxAnisotropy : float32
    end
[<Struct>]
type SamplerDescriptorDummyAnisotropicFiltering =
    {
        MaxAnisotropy : float32
    }
    member x.Pin<'a>(action : WGPUSamplerDescriptorDummyAnisotropicFiltering -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUSamplerDescriptorDummyAnisotropicFiltering>
        native.MaxAnisotropy <- x.MaxAnisotropy
        action native
type WGPUShaderModuleSPIRVDescriptor =
    struct
        val mutable public CodeSize : uint32
        val mutable public Code : nativeptr<uint32>
    end
[<Struct>]
type ShaderModuleSPIRVDescriptor =
    {
        CodeSize : uint32
        Code : uint32[]
    }
    member x.Pin<'a>(action : WGPUShaderModuleSPIRVDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUShaderModuleSPIRVDescriptor>
        native.CodeSize <- x.CodeSize
        use _Code = fixed x.Code
        native.Code <- _Code
        action native
type WGPUShaderModuleWGSLDescriptor =
    struct
        val mutable public Source : nativeptr<byte>
    end
[<Struct>]
type ShaderModuleWGSLDescriptor =
    {
        Source : string
    }
    member x.Pin<'a>(action : WGPUShaderModuleWGSLDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUShaderModuleWGSLDescriptor>
        let pSource = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Source
        try
            native.Source <- Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt pSource
            action native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal pSource
type WGPUShaderModuleDescriptor =
    struct
        val mutable public pNext : nativeint
        val mutable public Label : nativeptr<byte>
    end
[<Struct>]
type ShaderModuleDescriptor =
    {
        Label : string
    }
    member x.Pin<'a>(action : WGPUShaderModuleDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUShaderModuleDescriptor>
        let pLabel = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            native.Label <- Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt pLabel
            native.pNext <- 0n
            action native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal pLabel
[<Flags>]
type ShaderStage =
    | None = 0
    | Vertex = 1
    | Fragment = 2
    | Compute = 4
type StencilOperation =
    | Keep = 0
    | Zero = 1
    | Replace = 2
    | Invert = 3
    | IncrementClamp = 4
    | DecrementClamp = 5
    | IncrementWrap = 6
    | DecrementWrap = 7
type WGPUStencilStateFaceDescriptor =
    struct
        val mutable public Compare : CompareFunction
        val mutable public FailOp : StencilOperation
        val mutable public DepthFailOp : StencilOperation
        val mutable public PassOp : StencilOperation
    end
[<Struct>]
type StencilStateFaceDescriptor =
    {
        Compare : CompareFunction
        FailOp : StencilOperation
        DepthFailOp : StencilOperation
        PassOp : StencilOperation
    }
    member x.Pin<'a>(action : WGPUStencilStateFaceDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUStencilStateFaceDescriptor>
        native.Compare <- x.Compare
        native.FailOp <- x.FailOp
        native.DepthFailOp <- x.DepthFailOp
        native.PassOp <- x.PassOp
        action native
type StoreOp =
    | Store = 0
    | Clear = 1
type WGPUSurfaceDescriptor =
    struct
        val mutable public pNext : nativeint
        val mutable public Label : nativeptr<byte>
    end
[<Struct>]
type SurfaceDescriptor =
    {
        Label : string
    }
    member x.Pin<'a>(action : WGPUSurfaceDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUSurfaceDescriptor>
        let pLabel = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            native.Label <- Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt pLabel
            native.pNext <- 0n
            action native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal pLabel
type WGPUSurfaceDescriptorFromCanvasHTMLSelector =
    struct
        val mutable public Selector : nativeptr<byte>
    end
[<Struct>]
type SurfaceDescriptorFromCanvasHTMLSelector =
    {
        Selector : string
    }
    member x.Pin<'a>(action : WGPUSurfaceDescriptorFromCanvasHTMLSelector -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUSurfaceDescriptorFromCanvasHTMLSelector>
        let pSelector = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Selector
        try
            native.Selector <- Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt pSelector
            action native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal pSelector
type WGPUSurfaceDescriptorFromMetalLayer =
    struct
        val mutable public Layer : nativeint
    end
[<Struct>]
type SurfaceDescriptorFromMetalLayer =
    {
        Layer : nativeint
    }
    member x.Pin<'a>(action : WGPUSurfaceDescriptorFromMetalLayer -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUSurfaceDescriptorFromMetalLayer>
        native.Layer <- x.Layer
        action native
type WGPUSurfaceDescriptorFromWindowsHWND =
    struct
        val mutable public Hinstance : nativeint
        val mutable public Hwnd : nativeint
    end
[<Struct>]
type SurfaceDescriptorFromWindowsHWND =
    {
        Hinstance : nativeint
        Hwnd : nativeint
    }
    member x.Pin<'a>(action : WGPUSurfaceDescriptorFromWindowsHWND -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUSurfaceDescriptorFromWindowsHWND>
        native.Hinstance <- x.Hinstance
        native.Hwnd <- x.Hwnd
        action native
type WGPUSurfaceDescriptorFromXlib =
    struct
        val mutable public Display : nativeint
        val mutable public Window : uint32
    end
[<Struct>]
type SurfaceDescriptorFromXlib =
    {
        Display : nativeint
        Window : uint32
    }
    member x.Pin<'a>(action : WGPUSurfaceDescriptorFromXlib -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUSurfaceDescriptorFromXlib>
        native.Display <- x.Display
        native.Window <- x.Window
        action native
type WGPUSwapChainDescriptor =
    struct
        val mutable public pNext : nativeint
        val mutable public Label : nativeptr<byte>
        val mutable public Usage : TextureUsage
        val mutable public Format : TextureFormat
        val mutable public Width : uint32
        val mutable public Height : uint32
        val mutable public PresentMode : PresentMode
        val mutable public Implementation : uint64
    end
[<Struct>]
type SwapChainDescriptor =
    {
        Label : string
        Usage : TextureUsage
        Format : TextureFormat
        Width : uint32
        Height : uint32
        PresentMode : PresentMode
        Implementation : uint64
    }
    member x.Pin<'a>(action : WGPUSwapChainDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUSwapChainDescriptor>
        let pLabel = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            native.Label <- Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt pLabel
            native.Usage <- x.Usage
            native.Format <- x.Format
            native.Width <- x.Width
            native.Height <- x.Height
            native.PresentMode <- x.PresentMode
            native.Implementation <- x.Implementation
            native.pNext <- 0n
            action native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal pLabel
type TextureAspect =
    | All = 0
    | StencilOnly = 1
    | DepthOnly = 2
type TextureComponentType =
    | Float = 0
    | Sint = 1
    | Uint = 2
type WGPUTextureCopyView =
    struct
        val mutable public pNext : nativeint
        val mutable public Texture : TextureHandle
        val mutable public MipLevel : uint32
        val mutable public Origin : WGPUOrigin3D
        val mutable public Aspect : TextureAspect
    end
[<Struct>]
type TextureCopyView =
    {
        Texture : Texture
        MipLevel : uint32
        Origin : Origin3D
        Aspect : TextureAspect
    }
    member x.Pin<'a>(action : WGPUTextureCopyView -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUTextureCopyView>
        native.Texture <- x.Texture.Handle
        native.MipLevel <- x.MipLevel
        x.Origin.Pin(fun _Origin ->
            native.Origin <- _Origin
            native.Aspect <- x.Aspect
            native.pNext <- 0n
            action native
        )
type WGPUTextureDataLayout =
    struct
        val mutable public pNext : nativeint
        val mutable public Offset : uint64
        val mutable public BytesPerRow : uint32
        val mutable public RowsPerImage : uint32
    end
[<Struct>]
type TextureDataLayout =
    {
        Offset : uint64
        BytesPerRow : uint32
        RowsPerImage : uint32
    }
    member x.Pin<'a>(action : WGPUTextureDataLayout -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUTextureDataLayout>
        native.Offset <- x.Offset
        native.BytesPerRow <- x.BytesPerRow
        native.RowsPerImage <- x.RowsPerImage
        native.pNext <- 0n
        action native
type WGPUTextureDescriptor =
    struct
        val mutable public pNext : nativeint
        val mutable public Label : nativeptr<byte>
        val mutable public Usage : TextureUsage
        val mutable public Dimension : TextureDimension
        val mutable public Size : WGPUExtent3D
        val mutable public Format : TextureFormat
        val mutable public MipLevelCount : uint32
        val mutable public SampleCount : uint32
    end
[<Struct>]
type TextureDescriptor =
    {
        Label : string
        Usage : TextureUsage
        Dimension : TextureDimension
        Size : Extent3D
        Format : TextureFormat
        MipLevelCount : uint32
        SampleCount : uint32
    }
    member x.Pin<'a>(action : WGPUTextureDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUTextureDescriptor>
        let pLabel = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            native.Label <- Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt pLabel
            native.Usage <- x.Usage
            native.Dimension <- x.Dimension
            x.Size.Pin(fun _Size ->
                native.Size <- _Size
                native.Format <- x.Format
                native.MipLevelCount <- x.MipLevelCount
                native.SampleCount <- x.SampleCount
                native.pNext <- 0n
                action native
            )
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal pLabel
type TextureDimension =
    | D1D = 0
    | D2D = 1
    | D3D = 2
type TextureFormat =
    | Undefined = 0
    | R8Unorm = 1
    | R8Snorm = 2
    | R8Uint = 3
    | R8Sint = 4
    | R16Uint = 5
    | R16Sint = 6
    | R16Float = 7
    | RG8Unorm = 8
    | RG8Snorm = 9
    | RG8Uint = 10
    | RG8Sint = 11
    | R32Float = 12
    | R32Uint = 13
    | R32Sint = 14
    | RG16Uint = 15
    | RG16Sint = 16
    | RG16Float = 17
    | RGBA8Unorm = 18
    | RGBA8UnormSrgb = 19
    | RGBA8Snorm = 20
    | RGBA8Uint = 21
    | RGBA8Sint = 22
    | BGRA8Unorm = 23
    | BGRA8UnormSrgb = 24
    | RGB10A2Unorm = 25
    | RG11B10Ufloat = 26
    | RGB9E5Ufloat = 27
    | RG32Float = 28
    | RG32Uint = 29
    | RG32Sint = 30
    | RGBA16Uint = 31
    | RGBA16Sint = 32
    | RGBA16Float = 33
    | RGBA32Float = 34
    | RGBA32Uint = 35
    | RGBA32Sint = 36
    | Depth32Float = 37
    | Depth24Plus = 38
    | Depth24PlusStencil8 = 39
    | BC1RGBAUnorm = 40
    | BC1RGBAUnormSrgb = 41
    | BC2RGBAUnorm = 42
    | BC2RGBAUnormSrgb = 43
    | BC3RGBAUnorm = 44
    | BC3RGBAUnormSrgb = 45
    | BC4RUnorm = 46
    | BC4RSnorm = 47
    | BC5RGUnorm = 48
    | BC5RGSnorm = 49
    | BC6HRGBUfloat = 50
    | BC6HRGBSfloat = 51
    | BC7RGBAUnorm = 52
    | BC7RGBAUnormSrgb = 53
[<Flags>]
type TextureUsage =
    | None = 0
    | CopySrc = 1
    | CopyDst = 2
    | Sampled = 4
    | Storage = 8
    | OutputAttachment = 16
    | Present = 32
type WGPUTextureViewDescriptor =
    struct
        val mutable public pNext : nativeint
        val mutable public Label : nativeptr<byte>
        val mutable public Format : TextureFormat
        val mutable public Dimension : TextureViewDimension
        val mutable public BaseMipLevel : uint32
        val mutable public MipLevelCount : uint32
        val mutable public BaseArrayLayer : uint32
        val mutable public ArrayLayerCount : uint32
        val mutable public Aspect : TextureAspect
    end
[<Struct>]
type TextureViewDescriptor =
    {
        Label : string
        Format : TextureFormat
        Dimension : TextureViewDimension
        BaseMipLevel : uint32
        MipLevelCount : uint32
        BaseArrayLayer : uint32
        ArrayLayerCount : uint32
        Aspect : TextureAspect
    }
    member x.Pin<'a>(action : WGPUTextureViewDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUTextureViewDescriptor>
        let pLabel = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            native.Label <- Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt pLabel
            native.Format <- x.Format
            native.Dimension <- x.Dimension
            native.BaseMipLevel <- x.BaseMipLevel
            native.MipLevelCount <- x.MipLevelCount
            native.BaseArrayLayer <- x.BaseArrayLayer
            native.ArrayLayerCount <- x.ArrayLayerCount
            native.Aspect <- x.Aspect
            native.pNext <- 0n
            action native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal pLabel
type TextureViewDimension =
    | Undefined = 0
    | D1D = 1
    | D2D = 2
    | D2DArray = 3
    | Cube = 4
    | CubeArray = 5
    | D3D = 6
type WGPUVertexAttributeDescriptor =
    struct
        val mutable public Format : VertexFormat
        val mutable public Offset : uint64
        val mutable public ShaderLocation : uint32
    end
[<Struct>]
type VertexAttributeDescriptor =
    {
        Format : VertexFormat
        Offset : uint64
        ShaderLocation : uint32
    }
    member x.Pin<'a>(action : WGPUVertexAttributeDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUVertexAttributeDescriptor>
        native.Format <- x.Format
        native.Offset <- x.Offset
        native.ShaderLocation <- x.ShaderLocation
        action native
type WGPUVertexBufferLayoutDescriptor =
    struct
        val mutable public ArrayStride : uint64
        val mutable public StepMode : InputStepMode
        val mutable public AttributeCount : uint32
        val mutable public Attributes : nativeptr<WGPUVertexAttributeDescriptor>
    end
[<Struct>]
type VertexBufferLayoutDescriptor =
    {
        ArrayStride : uint64
        StepMode : InputStepMode
        Attributes : VertexAttributeDescriptor[]
    }
    member x.Pin<'a>(action : WGPUVertexBufferLayoutDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUVertexBufferLayoutDescriptor>
        native.ArrayStride <- x.ArrayStride
        native.StepMode <- x.StepMode
        native.AttributeCount <- (if isNull x.Attributes then 0u else uint32 x.Attributes.Length)
        let rec pinAttributes (a : array<VertexAttributeDescriptor>) (p : array<_>) (i : int) =
            if i >= a.Length then
                use p = fixed p
                native.Attributes <- p
                action native
            else
                a.[i].Pin(fun ai -> p.[i] <- ai; pinAttributes a p (i+1))
        pinAttributes x.Attributes (Array.zeroCreate x.Attributes.Length) 0
type VertexFormat =
    | UChar2 = 0
    | UChar4 = 1
    | Char2 = 2
    | Char4 = 3
    | UChar2Norm = 4
    | UChar4Norm = 5
    | Char2Norm = 6
    | Char4Norm = 7
    | UShort2 = 8
    | UShort4 = 9
    | Short2 = 10
    | Short4 = 11
    | UShort2Norm = 12
    | UShort4Norm = 13
    | Short2Norm = 14
    | Short4Norm = 15
    | Half2 = 16
    | Half4 = 17
    | Float = 18
    | Float2 = 19
    | Float3 = 20
    | Float4 = 21
    | UInt = 22
    | UInt2 = 23
    | UInt3 = 24
    | UInt4 = 25
    | Int = 26
    | Int2 = 27
    | Int3 = 28
    | Int4 = 29
type WGPUVertexStateDescriptor =
    struct
        val mutable public pNext : nativeint
        val mutable public IndexFormat : IndexFormat
        val mutable public VertexBufferCount : uint32
        val mutable public VertexBuffers : nativeptr<WGPUVertexBufferLayoutDescriptor>
    end
[<Struct>]
type VertexStateDescriptor =
    {
        IndexFormat : IndexFormat
        VertexBuffers : VertexBufferLayoutDescriptor[]
    }
    member x.Pin<'a>(action : WGPUVertexStateDescriptor -> 'a) : 'a =
        let x = x
        let mutable native = Unchecked.defaultof<WGPUVertexStateDescriptor>
        native.IndexFormat <- x.IndexFormat
        native.VertexBufferCount <- (if isNull x.VertexBuffers then 0u else uint32 x.VertexBuffers.Length)
        let rec pinVertexBuffers (a : array<VertexBufferLayoutDescriptor>) (p : array<_>) (i : int) =
            if i >= a.Length then
                use p = fixed p
                native.VertexBuffers <- p
                native.pNext <- 0n
                action native
            else
                a.[i].Pin(fun ai -> p.[i] <- ai; pinVertexBuffers a p (i+1))
        pinVertexBuffers x.VertexBuffers (Array.zeroCreate x.VertexBuffers.Length) 0
module DawnRaw =
    open System.Runtime.InteropServices
    open System.Security

    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuBindGroupReference(BindGroupHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuBindGroupRelease(BindGroupHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuBindGroupLayoutReference(BindGroupLayoutHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuBindGroupLayoutRelease(BindGroupLayoutHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuBufferReference(BufferHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuBufferRelease(BufferHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuBufferMapAsync(BufferHandle Self, MapMode Mode, unativeint Offset, unativeint Size, nativeint Callback, nativeint Userdata)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern nativeint wgpuBufferGetMappedRange(BufferHandle Self, unativeint Offset, unativeint Size)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern nativeint wgpuBufferGetConstMappedRange(BufferHandle Self, unativeint Offset, unativeint Size)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuBufferUnmap(BufferHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuBufferDestroy(BufferHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuCommandBufferReference(CommandBufferHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuCommandBufferRelease(CommandBufferHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuCommandEncoderReference(CommandEncoderHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuCommandEncoderRelease(CommandEncoderHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern CommandBufferHandle wgpuCommandEncoderFinish(CommandEncoderHandle Self, WGPUCommandBufferDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern ComputePassEncoderHandle wgpuCommandEncoderBeginComputePass(CommandEncoderHandle Self, WGPUComputePassDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern RenderPassEncoderHandle wgpuCommandEncoderBeginRenderPass(CommandEncoderHandle Self, WGPURenderPassDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuCommandEncoderCopyBufferToBuffer(CommandEncoderHandle Self, BufferHandle Source, uint64 SourceOffset, BufferHandle Destination, uint64 DestinationOffset, uint64 Size)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuCommandEncoderCopyBufferToTexture(CommandEncoderHandle Self, WGPUBufferCopyView* Source, WGPUTextureCopyView* Destination, WGPUExtent3D* CopySize)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuCommandEncoderCopyTextureToBuffer(CommandEncoderHandle Self, WGPUTextureCopyView* Source, WGPUBufferCopyView* Destination, WGPUExtent3D* CopySize)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuCommandEncoderCopyTextureToTexture(CommandEncoderHandle Self, WGPUTextureCopyView* Source, WGPUTextureCopyView* Destination, WGPUExtent3D* CopySize)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuCommandEncoderInsertDebugMarker(CommandEncoderHandle Self, byte* MarkerLabel)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuCommandEncoderPopDebugGroup(CommandEncoderHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuCommandEncoderPushDebugGroup(CommandEncoderHandle Self, byte* GroupLabel)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuCommandEncoderResolveQuerySet(CommandEncoderHandle Self, QuerySetHandle QuerySet, uint32 FirstQuery, uint32 QueryCount, BufferHandle Destination, uint64 DestinationOffset)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuCommandEncoderWriteTimestamp(CommandEncoderHandle Self, QuerySetHandle QuerySet, uint32 QueryIndex)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuComputePassEncoderReference(ComputePassEncoderHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuComputePassEncoderRelease(ComputePassEncoderHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuComputePassEncoderInsertDebugMarker(ComputePassEncoderHandle Self, byte* MarkerLabel)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuComputePassEncoderPopDebugGroup(ComputePassEncoderHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuComputePassEncoderPushDebugGroup(ComputePassEncoderHandle Self, byte* GroupLabel)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuComputePassEncoderSetPipeline(ComputePassEncoderHandle Self, ComputePipelineHandle Pipeline)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuComputePassEncoderSetBindGroup(ComputePassEncoderHandle Self, uint32 GroupIndex, BindGroupHandle Group, uint32 DynamicOffsetCount, uint32* DynamicOffsets)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuComputePassEncoderWriteTimestamp(ComputePassEncoderHandle Self, QuerySetHandle QuerySet, uint32 QueryIndex)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuComputePassEncoderDispatch(ComputePassEncoderHandle Self, uint32 X, uint32 Y, uint32 Z)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuComputePassEncoderDispatchIndirect(ComputePassEncoderHandle Self, BufferHandle IndirectBuffer, uint64 IndirectOffset)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuComputePassEncoderEndPass(ComputePassEncoderHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuComputePipelineReference(ComputePipelineHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuComputePipelineRelease(ComputePipelineHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern BindGroupLayoutHandle wgpuComputePipelineGetBindGroupLayout(ComputePipelineHandle Self, uint32 GroupIndex)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuDeviceReference(DeviceHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuDeviceRelease(DeviceHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern BindGroupHandle wgpuDeviceCreateBindGroup(DeviceHandle Self, WGPUBindGroupDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern BindGroupLayoutHandle wgpuDeviceCreateBindGroupLayout(DeviceHandle Self, WGPUBindGroupLayoutDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern BufferHandle wgpuDeviceCreateBuffer(DeviceHandle Self, WGPUBufferDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern BufferHandle wgpuDeviceCreateErrorBuffer(DeviceHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern CommandEncoderHandle wgpuDeviceCreateCommandEncoder(DeviceHandle Self, WGPUCommandEncoderDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern ComputePipelineHandle wgpuDeviceCreateComputePipeline(DeviceHandle Self, WGPUComputePipelineDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern PipelineLayoutHandle wgpuDeviceCreatePipelineLayout(DeviceHandle Self, WGPUPipelineLayoutDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern QuerySetHandle wgpuDeviceCreateQuerySet(DeviceHandle Self, WGPUQuerySetDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern RenderBundleEncoderHandle wgpuDeviceCreateRenderBundleEncoder(DeviceHandle Self, WGPURenderBundleEncoderDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern RenderPipelineHandle wgpuDeviceCreateRenderPipeline(DeviceHandle Self, WGPURenderPipelineDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern SamplerHandle wgpuDeviceCreateSampler(DeviceHandle Self, WGPUSamplerDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern ShaderModuleHandle wgpuDeviceCreateShaderModule(DeviceHandle Self, WGPUShaderModuleDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern SwapChainHandle wgpuDeviceCreateSwapChain(DeviceHandle Self, SurfaceHandle Surface, WGPUSwapChainDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern TextureHandle wgpuDeviceCreateTexture(DeviceHandle Self, WGPUTextureDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern QueueHandle wgpuDeviceGetDefaultQueue(DeviceHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuDeviceInjectError(DeviceHandle Self, ErrorType Type, byte* Message)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuDeviceLoseForTesting(DeviceHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuDeviceTick(DeviceHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuDeviceSetUncapturedErrorCallback(DeviceHandle Self, nativeint Callback, nativeint Userdata)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuDeviceSetDeviceLostCallback(DeviceHandle Self, nativeint Callback, nativeint Userdata)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuDevicePushErrorScope(DeviceHandle Self, ErrorFilter Filter)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern int wgpuDevicePopErrorScope(DeviceHandle Self, nativeint Callback, nativeint Userdata)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuFenceReference(FenceHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuFenceRelease(FenceHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern uint64 wgpuFenceGetCompletedValue(FenceHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuFenceOnCompletion(FenceHandle Self, uint64 Value, nativeint Callback, nativeint Userdata)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuInstanceReference(InstanceHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuInstanceRelease(InstanceHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern SurfaceHandle wgpuInstanceCreateSurface(InstanceHandle Self, WGPUSurfaceDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuPipelineLayoutReference(PipelineLayoutHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuPipelineLayoutRelease(PipelineLayoutHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuQuerySetReference(QuerySetHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuQuerySetRelease(QuerySetHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuQuerySetDestroy(QuerySetHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuQueueReference(QueueHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuQueueRelease(QueueHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuQueueSubmit(QueueHandle Self, uint32 CommandCount, CommandBufferHandle* Commands)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuQueueSignal(QueueHandle Self, FenceHandle Fence, uint64 SignalValue)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern FenceHandle wgpuQueueCreateFence(QueueHandle Self, WGPUFenceDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuQueueWriteBuffer(QueueHandle Self, BufferHandle Buffer, uint64 BufferOffset, nativeint Data, unativeint Size)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuQueueWriteTexture(QueueHandle Self, WGPUTextureCopyView* Destination, nativeint Data, unativeint DataSize, WGPUTextureDataLayout* DataLayout, WGPUExtent3D* WriteSize)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleReference(RenderBundleHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleRelease(RenderBundleHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleEncoderReference(RenderBundleEncoderHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleEncoderRelease(RenderBundleEncoderHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleEncoderSetPipeline(RenderBundleEncoderHandle Self, RenderPipelineHandle Pipeline)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleEncoderSetBindGroup(RenderBundleEncoderHandle Self, uint32 GroupIndex, BindGroupHandle Group, uint32 DynamicOffsetCount, uint32* DynamicOffsets)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleEncoderDraw(RenderBundleEncoderHandle Self, uint32 VertexCount, uint32 InstanceCount, uint32 FirstVertex, uint32 FirstInstance)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleEncoderDrawIndexed(RenderBundleEncoderHandle Self, uint32 IndexCount, uint32 InstanceCount, uint32 FirstIndex, int BaseVertex, uint32 FirstInstance)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleEncoderDrawIndirect(RenderBundleEncoderHandle Self, BufferHandle IndirectBuffer, uint64 IndirectOffset)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleEncoderDrawIndexedIndirect(RenderBundleEncoderHandle Self, BufferHandle IndirectBuffer, uint64 IndirectOffset)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleEncoderInsertDebugMarker(RenderBundleEncoderHandle Self, byte* MarkerLabel)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleEncoderPopDebugGroup(RenderBundleEncoderHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleEncoderPushDebugGroup(RenderBundleEncoderHandle Self, byte* GroupLabel)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleEncoderSetVertexBuffer(RenderBundleEncoderHandle Self, uint32 Slot, BufferHandle Buffer, uint64 Offset, uint64 Size)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleEncoderSetIndexBuffer(RenderBundleEncoderHandle Self, BufferHandle Buffer, uint64 Offset, uint64 Size)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleEncoderSetIndexBufferWithFormat(RenderBundleEncoderHandle Self, BufferHandle Buffer, IndexFormat Format, uint64 Offset, uint64 Size)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern RenderBundleHandle wgpuRenderBundleEncoderFinish(RenderBundleEncoderHandle Self, WGPURenderBundleDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderReference(RenderPassEncoderHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderRelease(RenderPassEncoderHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderSetPipeline(RenderPassEncoderHandle Self, RenderPipelineHandle Pipeline)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderSetBindGroup(RenderPassEncoderHandle Self, uint32 GroupIndex, BindGroupHandle Group, uint32 DynamicOffsetCount, uint32* DynamicOffsets)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderDraw(RenderPassEncoderHandle Self, uint32 VertexCount, uint32 InstanceCount, uint32 FirstVertex, uint32 FirstInstance)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderDrawIndexed(RenderPassEncoderHandle Self, uint32 IndexCount, uint32 InstanceCount, uint32 FirstIndex, int BaseVertex, uint32 FirstInstance)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderDrawIndirect(RenderPassEncoderHandle Self, BufferHandle IndirectBuffer, uint64 IndirectOffset)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderDrawIndexedIndirect(RenderPassEncoderHandle Self, BufferHandle IndirectBuffer, uint64 IndirectOffset)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderExecuteBundles(RenderPassEncoderHandle Self, uint32 BundlesCount, RenderBundleHandle* Bundles)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderInsertDebugMarker(RenderPassEncoderHandle Self, byte* MarkerLabel)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderPopDebugGroup(RenderPassEncoderHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderPushDebugGroup(RenderPassEncoderHandle Self, byte* GroupLabel)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderSetStencilReference(RenderPassEncoderHandle Self, uint32 Reference)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderSetBlendColor(RenderPassEncoderHandle Self, WGPUColor* Color)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderSetViewport(RenderPassEncoderHandle Self, float32 X, float32 Y, float32 Width, float32 Height, float32 MinDepth, float32 MaxDepth)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderSetScissorRect(RenderPassEncoderHandle Self, uint32 X, uint32 Y, uint32 Width, uint32 Height)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderSetVertexBuffer(RenderPassEncoderHandle Self, uint32 Slot, BufferHandle Buffer, uint64 Offset, uint64 Size)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderSetIndexBuffer(RenderPassEncoderHandle Self, BufferHandle Buffer, uint64 Offset, uint64 Size)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderSetIndexBufferWithFormat(RenderPassEncoderHandle Self, BufferHandle Buffer, IndexFormat Format, uint64 Offset, uint64 Size)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderWriteTimestamp(RenderPassEncoderHandle Self, QuerySetHandle QuerySet, uint32 QueryIndex)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderEndPass(RenderPassEncoderHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPipelineReference(RenderPipelineHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPipelineRelease(RenderPipelineHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern BindGroupLayoutHandle wgpuRenderPipelineGetBindGroupLayout(RenderPipelineHandle Self, uint32 GroupIndex)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuSamplerReference(SamplerHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuSamplerRelease(SamplerHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuShaderModuleReference(ShaderModuleHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuShaderModuleRelease(ShaderModuleHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuSurfaceReference(SurfaceHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuSurfaceRelease(SurfaceHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuSwapChainReference(SwapChainHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuSwapChainRelease(SwapChainHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuSwapChainConfigure(SwapChainHandle Self, TextureFormat Format, TextureUsage AllowedUsage, uint32 Width, uint32 Height)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern TextureViewHandle wgpuSwapChainGetCurrentTextureView(SwapChainHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuSwapChainPresent(SwapChainHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuTextureReference(TextureHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuTextureRelease(TextureHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern TextureViewHandle wgpuTextureCreateView(TextureHandle Self, WGPUTextureViewDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuTextureDestroy(TextureHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuTextureViewReference(TextureViewHandle Self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuTextureViewRelease(TextureViewHandle Self)
type BindGroupHandle = struct val mutable public Handle : nativeint end
[<Struct>]
type BindGroup(device : Device, handle : BindGroupHandle) =
    member x.Handle : BindGroupHandle = handle
    member x.Device : Device = device
    member x.Reference() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuBindGroupReference(handle)
    member x.Release() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuBindGroupRelease(handle)
type BindGroupLayoutHandle = struct val mutable public Handle : nativeint end
[<Struct>]
type BindGroupLayout(device : Device, handle : BindGroupLayoutHandle) =
    member x.Handle : BindGroupLayoutHandle = handle
    member x.Device : Device = device
    member x.Reference() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuBindGroupLayoutReference(handle)
    member x.Release() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuBindGroupLayoutRelease(handle)
type BufferHandle = struct val mutable public Handle : nativeint end
[<Struct>]
type Buffer(device : Device, handle : BufferHandle) =
    member x.Handle : BufferHandle = handle
    member x.Device : Device = device
    member x.Reference() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuBufferReference(handle)
    member x.Release() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuBufferRelease(handle)
    member x.MapAsync(Mode, Offset, Size, Callback : BufferMapCallback, Userdata) : unit =
        let device = device
        let handle = handle
        let mutable _CallbackGC = Unchecked.defaultof<System.Runtime.InteropServices.GCHandle>
        let _Callback = BufferMapCallback(fun Status Userdata -> Callback.Invoke(Status, Userdata); _CallbackGC.Free())
        _CallbackGC <- System.Runtime.InteropServices.GCHandle.Alloc(_Callback)
        DawnRaw.wgpuBufferMapAsync(handle, Mode, Offset, Size, System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate _Callback, Userdata)
    member x.GetMappedRange(Offset, Size) : nativeint =
        let device = device
        let handle = handle
        DawnRaw.wgpuBufferGetMappedRange(handle, Offset, Size)
    member x.GetConstMappedRange(Offset, Size) : nativeint =
        let device = device
        let handle = handle
        DawnRaw.wgpuBufferGetConstMappedRange(handle, Offset, Size)
    member x.Unmap() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuBufferUnmap(handle)
    member x.Destroy() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuBufferDestroy(handle)
type CommandBufferHandle = struct val mutable public Handle : nativeint end
[<Struct>]
type CommandBuffer(device : Device, handle : CommandBufferHandle) =
    member x.Handle : CommandBufferHandle = handle
    member x.Device : Device = device
    member x.Reference() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuCommandBufferReference(handle)
    member x.Release() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuCommandBufferRelease(handle)
type CommandEncoderHandle = struct val mutable public Handle : nativeint end
[<Struct>]
type CommandEncoder(device : Device, handle : CommandEncoderHandle) =
    member x.Handle : CommandEncoderHandle = handle
    member x.Device : Device = device
    member x.Reference() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuCommandEncoderReference(handle)
    member x.Release() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuCommandEncoderRelease(handle)
    member x.Finish(Descriptor : CommandBufferDescriptor) : CommandBuffer =
        let device = device
        let handle = handle
        Descriptor.Pin(fun _Descriptor ->
            use _Descriptor = fixed [| _Descriptor |]
            CommandBuffer(device, DawnRaw.wgpuCommandEncoderFinish(handle, _Descriptor))
        )
    member x.BeginComputePass(Descriptor : ComputePassDescriptor) : ComputePassEncoder =
        let device = device
        let handle = handle
        Descriptor.Pin(fun _Descriptor ->
            use _Descriptor = fixed [| _Descriptor |]
            ComputePassEncoder(device, DawnRaw.wgpuCommandEncoderBeginComputePass(handle, _Descriptor))
        )
    member x.BeginRenderPass(Descriptor : RenderPassDescriptor) : RenderPassEncoder =
        let device = device
        let handle = handle
        Descriptor.Pin(fun _Descriptor ->
            use _Descriptor = fixed [| _Descriptor |]
            RenderPassEncoder(device, DawnRaw.wgpuCommandEncoderBeginRenderPass(handle, _Descriptor))
        )
    member x.CopyBufferToBuffer(Source : Buffer, SourceOffset, Destination : Buffer, DestinationOffset, Size) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuCommandEncoderCopyBufferToBuffer(handle, Source.Handle, SourceOffset, Destination.Handle, DestinationOffset, Size)
    member x.CopyBufferToTexture(Source : BufferCopyView, Destination : TextureCopyView, CopySize : Extent3D) : unit =
        let device = device
        let handle = handle
        Source.Pin(fun _Source ->
            use _Source = fixed [| _Source |]
            Destination.Pin(fun _Destination ->
                use _Destination = fixed [| _Destination |]
                CopySize.Pin(fun _CopySize ->
                    use _CopySize = fixed [| _CopySize |]
                    DawnRaw.wgpuCommandEncoderCopyBufferToTexture(handle, _Source, _Destination, _CopySize)
                )
            )
        )
    member x.CopyTextureToBuffer(Source : TextureCopyView, Destination : BufferCopyView, CopySize : Extent3D) : unit =
        let device = device
        let handle = handle
        Source.Pin(fun _Source ->
            use _Source = fixed [| _Source |]
            Destination.Pin(fun _Destination ->
                use _Destination = fixed [| _Destination |]
                CopySize.Pin(fun _CopySize ->
                    use _CopySize = fixed [| _CopySize |]
                    DawnRaw.wgpuCommandEncoderCopyTextureToBuffer(handle, _Source, _Destination, _CopySize)
                )
            )
        )
    member x.CopyTextureToTexture(Source : TextureCopyView, Destination : TextureCopyView, CopySize : Extent3D) : unit =
        let device = device
        let handle = handle
        Source.Pin(fun _Source ->
            use _Source = fixed [| _Source |]
            Destination.Pin(fun _Destination ->
                use _Destination = fixed [| _Destination |]
                CopySize.Pin(fun _CopySize ->
                    use _CopySize = fixed [| _CopySize |]
                    DawnRaw.wgpuCommandEncoderCopyTextureToTexture(handle, _Source, _Destination, _CopySize)
                )
            )
        )
    member x.InsertDebugMarker(MarkerLabel) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuCommandEncoderInsertDebugMarker(handle, MarkerLabel)
    member x.PopDebugGroup() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuCommandEncoderPopDebugGroup(handle)
    member x.PushDebugGroup(GroupLabel) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuCommandEncoderPushDebugGroup(handle, GroupLabel)
    member x.ResolveQuerySet(QuerySet : QuerySet, FirstQuery, QueryCount, Destination : Buffer, DestinationOffset) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuCommandEncoderResolveQuerySet(handle, QuerySet.Handle, FirstQuery, QueryCount, Destination.Handle, DestinationOffset)
    member x.WriteTimestamp(QuerySet : QuerySet, QueryIndex) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuCommandEncoderWriteTimestamp(handle, QuerySet.Handle, QueryIndex)
type ComputePassEncoderHandle = struct val mutable public Handle : nativeint end
[<Struct>]
type ComputePassEncoder(device : Device, handle : ComputePassEncoderHandle) =
    member x.Handle : ComputePassEncoderHandle = handle
    member x.Device : Device = device
    member x.Reference() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuComputePassEncoderReference(handle)
    member x.Release() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuComputePassEncoderRelease(handle)
    member x.InsertDebugMarker(MarkerLabel) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuComputePassEncoderInsertDebugMarker(handle, MarkerLabel)
    member x.PopDebugGroup() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuComputePassEncoderPopDebugGroup(handle)
    member x.PushDebugGroup(GroupLabel) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuComputePassEncoderPushDebugGroup(handle, GroupLabel)
    member x.SetPipeline(Pipeline : ComputePipeline) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuComputePassEncoderSetPipeline(handle, Pipeline.Handle)
    member x.SetBindGroup(GroupIndex, Group : BindGroup, DynamicOffsets : uint32[]) : unit =
        let device = device
        let handle = handle
        use _DynamicOffsets = fixed DynamicOffsets
        DawnRaw.wgpuComputePassEncoderSetBindGroup(handle, GroupIndex, Group.Handle, (if isNull DynamicOffsets then 0u else uint32 DynamicOffsets.Length), _DynamicOffsets)
    member x.WriteTimestamp(QuerySet : QuerySet, QueryIndex) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuComputePassEncoderWriteTimestamp(handle, QuerySet.Handle, QueryIndex)
    member x.Dispatch(X, Y, Z) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuComputePassEncoderDispatch(handle, X, Y, Z)
    member x.DispatchIndirect(IndirectBuffer : Buffer, IndirectOffset) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuComputePassEncoderDispatchIndirect(handle, IndirectBuffer.Handle, IndirectOffset)
    member x.EndPass() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuComputePassEncoderEndPass(handle)
type ComputePipelineHandle = struct val mutable public Handle : nativeint end
[<Struct>]
type ComputePipeline(device : Device, handle : ComputePipelineHandle) =
    member x.Handle : ComputePipelineHandle = handle
    member x.Device : Device = device
    member x.Reference() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuComputePipelineReference(handle)
    member x.Release() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuComputePipelineRelease(handle)
    member x.GetBindGroupLayout(GroupIndex) : BindGroupLayout =
        let device = device
        let handle = handle
        BindGroupLayout(device, DawnRaw.wgpuComputePipelineGetBindGroupLayout(handle, GroupIndex))
type DeviceHandle = struct val mutable public Handle : nativeint end
[<Struct>]
type Device(handle : DeviceHandle) =
    member x.Handle : DeviceHandle = handle
    member x.Reference() : unit =
        let device = x
        let handle = handle
        DawnRaw.wgpuDeviceReference(handle)
    member x.Release() : unit =
        let device = x
        let handle = handle
        DawnRaw.wgpuDeviceRelease(handle)
    member x.CreateBindGroup(Descriptor : BindGroupDescriptor) : BindGroup =
        let device = x
        let handle = handle
        Descriptor.Pin(fun _Descriptor ->
            use _Descriptor = fixed [| _Descriptor |]
            BindGroup(device, DawnRaw.wgpuDeviceCreateBindGroup(handle, _Descriptor))
        )
    member x.CreateBindGroupLayout(Descriptor : BindGroupLayoutDescriptor) : BindGroupLayout =
        let device = x
        let handle = handle
        Descriptor.Pin(fun _Descriptor ->
            use _Descriptor = fixed [| _Descriptor |]
            BindGroupLayout(device, DawnRaw.wgpuDeviceCreateBindGroupLayout(handle, _Descriptor))
        )
    member x.CreateBuffer(Descriptor : BufferDescriptor) : Buffer =
        let device = x
        let handle = handle
        Descriptor.Pin(fun _Descriptor ->
            use _Descriptor = fixed [| _Descriptor |]
            Buffer(device, DawnRaw.wgpuDeviceCreateBuffer(handle, _Descriptor))
        )
    member x.CreateErrorBuffer() : Buffer =
        let device = x
        let handle = handle
        Buffer(device, DawnRaw.wgpuDeviceCreateErrorBuffer(handle))
    member x.CreateCommandEncoder(Descriptor : CommandEncoderDescriptor) : CommandEncoder =
        let device = x
        let handle = handle
        Descriptor.Pin(fun _Descriptor ->
            use _Descriptor = fixed [| _Descriptor |]
            CommandEncoder(device, DawnRaw.wgpuDeviceCreateCommandEncoder(handle, _Descriptor))
        )
    member x.CreateComputePipeline(Descriptor : ComputePipelineDescriptor) : ComputePipeline =
        let device = x
        let handle = handle
        Descriptor.Pin(fun _Descriptor ->
            use _Descriptor = fixed [| _Descriptor |]
            ComputePipeline(device, DawnRaw.wgpuDeviceCreateComputePipeline(handle, _Descriptor))
        )
    member x.CreatePipelineLayout(Descriptor : PipelineLayoutDescriptor) : PipelineLayout =
        let device = x
        let handle = handle
        Descriptor.Pin(fun _Descriptor ->
            use _Descriptor = fixed [| _Descriptor |]
            PipelineLayout(device, DawnRaw.wgpuDeviceCreatePipelineLayout(handle, _Descriptor))
        )
    member x.CreateQuerySet(Descriptor : QuerySetDescriptor) : QuerySet =
        let device = x
        let handle = handle
        Descriptor.Pin(fun _Descriptor ->
            use _Descriptor = fixed [| _Descriptor |]
            QuerySet(device, DawnRaw.wgpuDeviceCreateQuerySet(handle, _Descriptor))
        )
    member x.CreateRenderBundleEncoder(Descriptor : RenderBundleEncoderDescriptor) : RenderBundleEncoder =
        let device = x
        let handle = handle
        Descriptor.Pin(fun _Descriptor ->
            use _Descriptor = fixed [| _Descriptor |]
            RenderBundleEncoder(device, DawnRaw.wgpuDeviceCreateRenderBundleEncoder(handle, _Descriptor))
        )
    member x.CreateRenderPipeline(Descriptor : RenderPipelineDescriptor) : RenderPipeline =
        let device = x
        let handle = handle
        Descriptor.Pin(fun _Descriptor ->
            use _Descriptor = fixed [| _Descriptor |]
            RenderPipeline(device, DawnRaw.wgpuDeviceCreateRenderPipeline(handle, _Descriptor))
        )
    member x.CreateSampler(Descriptor : SamplerDescriptor) : Sampler =
        let device = x
        let handle = handle
        Descriptor.Pin(fun _Descriptor ->
            use _Descriptor = fixed [| _Descriptor |]
            Sampler(device, DawnRaw.wgpuDeviceCreateSampler(handle, _Descriptor))
        )
    member x.CreateShaderModule(Descriptor : ShaderModuleDescriptor) : ShaderModule =
        let device = x
        let handle = handle
        Descriptor.Pin(fun _Descriptor ->
            use _Descriptor = fixed [| _Descriptor |]
            ShaderModule(device, DawnRaw.wgpuDeviceCreateShaderModule(handle, _Descriptor))
        )
    member x.CreateSwapChain(Surface : Surface, Descriptor : SwapChainDescriptor) : SwapChain =
        let device = x
        let handle = handle
        Descriptor.Pin(fun _Descriptor ->
            use _Descriptor = fixed [| _Descriptor |]
            SwapChain(device, DawnRaw.wgpuDeviceCreateSwapChain(handle, Surface.Handle, _Descriptor))
        )
    member x.CreateTexture(Descriptor : TextureDescriptor) : Texture =
        let device = x
        let handle = handle
        Descriptor.Pin(fun _Descriptor ->
            use _Descriptor = fixed [| _Descriptor |]
            Texture(device, DawnRaw.wgpuDeviceCreateTexture(handle, _Descriptor))
        )
    member x.GetDefaultQueue() : Queue =
        let device = x
        let handle = handle
        Queue(device, DawnRaw.wgpuDeviceGetDefaultQueue(handle))
    member x.InjectError(Type, Message) : unit =
        let device = x
        let handle = handle
        DawnRaw.wgpuDeviceInjectError(handle, Type, Message)
    member x.LoseForTesting() : unit =
        let device = x
        let handle = handle
        DawnRaw.wgpuDeviceLoseForTesting(handle)
    member x.Tick() : unit =
        let device = x
        let handle = handle
        DawnRaw.wgpuDeviceTick(handle)
    member x.SetUncapturedErrorCallback(Callback : ErrorCallback, Userdata) : System.IDisposable =
        let device = x
        let handle = handle
        let _CallbackGC = System.Runtime.InteropServices.GCHandle.Alloc(Callback)
        DawnRaw.wgpuDeviceSetUncapturedErrorCallback(handle, System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate Callback, Userdata)
        { new System.IDisposable with member x.Dispose() = _CallbackGC.Free() }
    member x.SetDeviceLostCallback(Callback : DeviceLostCallback, Userdata) : System.IDisposable =
        let device = x
        let handle = handle
        let _CallbackGC = System.Runtime.InteropServices.GCHandle.Alloc(Callback)
        DawnRaw.wgpuDeviceSetDeviceLostCallback(handle, System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate Callback, Userdata)
        { new System.IDisposable with member x.Dispose() = _CallbackGC.Free() }
    member x.PushErrorScope(Filter) : unit =
        let device = x
        let handle = handle
        DawnRaw.wgpuDevicePushErrorScope(handle, Filter)
    member x.PopErrorScope(Callback : ErrorCallback, Userdata) : int =
        let device = x
        let handle = handle
        let mutable _CallbackGC = Unchecked.defaultof<System.Runtime.InteropServices.GCHandle>
        let _Callback = ErrorCallback(fun Type Message Userdata -> Callback.Invoke(Type, Message, Userdata); _CallbackGC.Free())
        _CallbackGC <- System.Runtime.InteropServices.GCHandle.Alloc(_Callback)
        DawnRaw.wgpuDevicePopErrorScope(handle, System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate _Callback, Userdata)
type FenceHandle = struct val mutable public Handle : nativeint end
[<Struct>]
type Fence(device : Device, handle : FenceHandle) =
    member x.Handle : FenceHandle = handle
    member x.Device : Device = device
    member x.Reference() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuFenceReference(handle)
    member x.Release() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuFenceRelease(handle)
    member x.GetCompletedValue() : uint64 =
        let device = device
        let handle = handle
        DawnRaw.wgpuFenceGetCompletedValue(handle)
    member x.OnCompletion(Value, Callback : FenceOnCompletionCallback, Userdata) : unit =
        let device = device
        let handle = handle
        let mutable _CallbackGC = Unchecked.defaultof<System.Runtime.InteropServices.GCHandle>
        let _Callback = FenceOnCompletionCallback(fun Status Userdata -> Callback.Invoke(Status, Userdata); _CallbackGC.Free())
        _CallbackGC <- System.Runtime.InteropServices.GCHandle.Alloc(_Callback)
        DawnRaw.wgpuFenceOnCompletion(handle, Value, System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate _Callback, Userdata)
type InstanceHandle = struct val mutable public Handle : nativeint end
[<Struct>]
type Instance(device : Device, handle : InstanceHandle) =
    member x.Handle : InstanceHandle = handle
    member x.Device : Device = device
    member x.Reference() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuInstanceReference(handle)
    member x.Release() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuInstanceRelease(handle)
    member x.CreateSurface(Descriptor : SurfaceDescriptor) : Surface =
        let device = device
        let handle = handle
        Descriptor.Pin(fun _Descriptor ->
            use _Descriptor = fixed [| _Descriptor |]
            Surface(device, DawnRaw.wgpuInstanceCreateSurface(handle, _Descriptor))
        )
type PipelineLayoutHandle = struct val mutable public Handle : nativeint end
[<Struct>]
type PipelineLayout(device : Device, handle : PipelineLayoutHandle) =
    member x.Handle : PipelineLayoutHandle = handle
    member x.Device : Device = device
    member x.Reference() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuPipelineLayoutReference(handle)
    member x.Release() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuPipelineLayoutRelease(handle)
type QuerySetHandle = struct val mutable public Handle : nativeint end
[<Struct>]
type QuerySet(device : Device, handle : QuerySetHandle) =
    member x.Handle : QuerySetHandle = handle
    member x.Device : Device = device
    member x.Reference() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuQuerySetReference(handle)
    member x.Release() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuQuerySetRelease(handle)
    member x.Destroy() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuQuerySetDestroy(handle)
type QueueHandle = struct val mutable public Handle : nativeint end
[<Struct>]
type Queue(device : Device, handle : QueueHandle) =
    member x.Handle : QueueHandle = handle
    member x.Device : Device = device
    member x.Reference() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuQueueReference(handle)
    member x.Release() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuQueueRelease(handle)
    member x.Submit(Commands : CommandBuffer[]) : unit =
        let device = device
        let handle = handle
        let _Commands = Commands |> Array.map (fun s -> s.Handle)
        use _Commands = fixed _Commands
        DawnRaw.wgpuQueueSubmit(handle, (if isNull Commands then 0u else uint32 Commands.Length), _Commands)
    member x.Signal(Fence : Fence, SignalValue) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuQueueSignal(handle, Fence.Handle, SignalValue)
    member x.CreateFence(Descriptor : FenceDescriptor) : Fence =
        let device = device
        let handle = handle
        Descriptor.Pin(fun _Descriptor ->
            use _Descriptor = fixed [| _Descriptor |]
            Fence(device, DawnRaw.wgpuQueueCreateFence(handle, _Descriptor))
        )
    member x.WriteBuffer(Buffer : Buffer, BufferOffset, Data, Size) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuQueueWriteBuffer(handle, Buffer.Handle, BufferOffset, Data, Size)
    member x.WriteTexture(Destination : TextureCopyView, Data, DataSize, DataLayout : TextureDataLayout, WriteSize : Extent3D) : unit =
        let device = device
        let handle = handle
        Destination.Pin(fun _Destination ->
            use _Destination = fixed [| _Destination |]
            DataLayout.Pin(fun _DataLayout ->
                use _DataLayout = fixed [| _DataLayout |]
                WriteSize.Pin(fun _WriteSize ->
                    use _WriteSize = fixed [| _WriteSize |]
                    DawnRaw.wgpuQueueWriteTexture(handle, _Destination, Data, DataSize, _DataLayout, _WriteSize)
                )
            )
        )
type RenderBundleHandle = struct val mutable public Handle : nativeint end
[<Struct>]
type RenderBundle(device : Device, handle : RenderBundleHandle) =
    member x.Handle : RenderBundleHandle = handle
    member x.Device : Device = device
    member x.Reference() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderBundleReference(handle)
    member x.Release() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderBundleRelease(handle)
type RenderBundleEncoderHandle = struct val mutable public Handle : nativeint end
[<Struct>]
type RenderBundleEncoder(device : Device, handle : RenderBundleEncoderHandle) =
    member x.Handle : RenderBundleEncoderHandle = handle
    member x.Device : Device = device
    member x.Reference() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderBundleEncoderReference(handle)
    member x.Release() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderBundleEncoderRelease(handle)
    member x.SetPipeline(Pipeline : RenderPipeline) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderBundleEncoderSetPipeline(handle, Pipeline.Handle)
    member x.SetBindGroup(GroupIndex, Group : BindGroup, DynamicOffsets : uint32[]) : unit =
        let device = device
        let handle = handle
        use _DynamicOffsets = fixed DynamicOffsets
        DawnRaw.wgpuRenderBundleEncoderSetBindGroup(handle, GroupIndex, Group.Handle, (if isNull DynamicOffsets then 0u else uint32 DynamicOffsets.Length), _DynamicOffsets)
    member x.Draw(VertexCount, InstanceCount, FirstVertex, FirstInstance) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderBundleEncoderDraw(handle, VertexCount, InstanceCount, FirstVertex, FirstInstance)
    member x.DrawIndexed(IndexCount, InstanceCount, FirstIndex, BaseVertex, FirstInstance) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderBundleEncoderDrawIndexed(handle, IndexCount, InstanceCount, FirstIndex, BaseVertex, FirstInstance)
    member x.DrawIndirect(IndirectBuffer : Buffer, IndirectOffset) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderBundleEncoderDrawIndirect(handle, IndirectBuffer.Handle, IndirectOffset)
    member x.DrawIndexedIndirect(IndirectBuffer : Buffer, IndirectOffset) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderBundleEncoderDrawIndexedIndirect(handle, IndirectBuffer.Handle, IndirectOffset)
    member x.InsertDebugMarker(MarkerLabel) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderBundleEncoderInsertDebugMarker(handle, MarkerLabel)
    member x.PopDebugGroup() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderBundleEncoderPopDebugGroup(handle)
    member x.PushDebugGroup(GroupLabel) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderBundleEncoderPushDebugGroup(handle, GroupLabel)
    member x.SetVertexBuffer(Slot, Buffer : Buffer, Offset, Size) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderBundleEncoderSetVertexBuffer(handle, Slot, Buffer.Handle, Offset, Size)
    member x.SetIndexBuffer(Buffer : Buffer, Offset, Size) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderBundleEncoderSetIndexBuffer(handle, Buffer.Handle, Offset, Size)
    member x.SetIndexBufferWithFormat(Buffer : Buffer, Format, Offset, Size) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderBundleEncoderSetIndexBufferWithFormat(handle, Buffer.Handle, Format, Offset, Size)
    member x.Finish(Descriptor : RenderBundleDescriptor) : RenderBundle =
        let device = device
        let handle = handle
        Descriptor.Pin(fun _Descriptor ->
            use _Descriptor = fixed [| _Descriptor |]
            RenderBundle(device, DawnRaw.wgpuRenderBundleEncoderFinish(handle, _Descriptor))
        )
type RenderPassEncoderHandle = struct val mutable public Handle : nativeint end
[<Struct>]
type RenderPassEncoder(device : Device, handle : RenderPassEncoderHandle) =
    member x.Handle : RenderPassEncoderHandle = handle
    member x.Device : Device = device
    member x.Reference() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderPassEncoderReference(handle)
    member x.Release() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderPassEncoderRelease(handle)
    member x.SetPipeline(Pipeline : RenderPipeline) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderPassEncoderSetPipeline(handle, Pipeline.Handle)
    member x.SetBindGroup(GroupIndex, Group : BindGroup, DynamicOffsets : uint32[]) : unit =
        let device = device
        let handle = handle
        use _DynamicOffsets = fixed DynamicOffsets
        DawnRaw.wgpuRenderPassEncoderSetBindGroup(handle, GroupIndex, Group.Handle, (if isNull DynamicOffsets then 0u else uint32 DynamicOffsets.Length), _DynamicOffsets)
    member x.Draw(VertexCount, InstanceCount, FirstVertex, FirstInstance) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderPassEncoderDraw(handle, VertexCount, InstanceCount, FirstVertex, FirstInstance)
    member x.DrawIndexed(IndexCount, InstanceCount, FirstIndex, BaseVertex, FirstInstance) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderPassEncoderDrawIndexed(handle, IndexCount, InstanceCount, FirstIndex, BaseVertex, FirstInstance)
    member x.DrawIndirect(IndirectBuffer : Buffer, IndirectOffset) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderPassEncoderDrawIndirect(handle, IndirectBuffer.Handle, IndirectOffset)
    member x.DrawIndexedIndirect(IndirectBuffer : Buffer, IndirectOffset) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderPassEncoderDrawIndexedIndirect(handle, IndirectBuffer.Handle, IndirectOffset)
    member x.ExecuteBundles(Bundles : RenderBundle[]) : unit =
        let device = device
        let handle = handle
        let _Bundles = Bundles |> Array.map (fun s -> s.Handle)
        use _Bundles = fixed _Bundles
        DawnRaw.wgpuRenderPassEncoderExecuteBundles(handle, (if isNull Bundles then 0u else uint32 Bundles.Length), _Bundles)
    member x.InsertDebugMarker(MarkerLabel) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderPassEncoderInsertDebugMarker(handle, MarkerLabel)
    member x.PopDebugGroup() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderPassEncoderPopDebugGroup(handle)
    member x.PushDebugGroup(GroupLabel) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderPassEncoderPushDebugGroup(handle, GroupLabel)
    member x.SetStencilReference(Reference) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderPassEncoderSetStencilReference(handle, Reference)
    member x.SetBlendColor(Color : Color) : unit =
        let device = device
        let handle = handle
        Color.Pin(fun _Color ->
            use _Color = fixed [| _Color |]
            DawnRaw.wgpuRenderPassEncoderSetBlendColor(handle, _Color)
        )
    member x.SetViewport(X, Y, Width, Height, MinDepth, MaxDepth) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderPassEncoderSetViewport(handle, X, Y, Width, Height, MinDepth, MaxDepth)
    member x.SetScissorRect(X, Y, Width, Height) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderPassEncoderSetScissorRect(handle, X, Y, Width, Height)
    member x.SetVertexBuffer(Slot, Buffer : Buffer, Offset, Size) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderPassEncoderSetVertexBuffer(handle, Slot, Buffer.Handle, Offset, Size)
    member x.SetIndexBuffer(Buffer : Buffer, Offset, Size) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderPassEncoderSetIndexBuffer(handle, Buffer.Handle, Offset, Size)
    member x.SetIndexBufferWithFormat(Buffer : Buffer, Format, Offset, Size) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderPassEncoderSetIndexBufferWithFormat(handle, Buffer.Handle, Format, Offset, Size)
    member x.WriteTimestamp(QuerySet : QuerySet, QueryIndex) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderPassEncoderWriteTimestamp(handle, QuerySet.Handle, QueryIndex)
    member x.EndPass() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderPassEncoderEndPass(handle)
type RenderPipelineHandle = struct val mutable public Handle : nativeint end
[<Struct>]
type RenderPipeline(device : Device, handle : RenderPipelineHandle) =
    member x.Handle : RenderPipelineHandle = handle
    member x.Device : Device = device
    member x.Reference() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderPipelineReference(handle)
    member x.Release() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuRenderPipelineRelease(handle)
    member x.GetBindGroupLayout(GroupIndex) : BindGroupLayout =
        let device = device
        let handle = handle
        BindGroupLayout(device, DawnRaw.wgpuRenderPipelineGetBindGroupLayout(handle, GroupIndex))
type SamplerHandle = struct val mutable public Handle : nativeint end
[<Struct>]
type Sampler(device : Device, handle : SamplerHandle) =
    member x.Handle : SamplerHandle = handle
    member x.Device : Device = device
    member x.Reference() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuSamplerReference(handle)
    member x.Release() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuSamplerRelease(handle)
type ShaderModuleHandle = struct val mutable public Handle : nativeint end
[<Struct>]
type ShaderModule(device : Device, handle : ShaderModuleHandle) =
    member x.Handle : ShaderModuleHandle = handle
    member x.Device : Device = device
    member x.Reference() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuShaderModuleReference(handle)
    member x.Release() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuShaderModuleRelease(handle)
type SurfaceHandle = struct val mutable public Handle : nativeint end
[<Struct>]
type Surface(device : Device, handle : SurfaceHandle) =
    member x.Handle : SurfaceHandle = handle
    member x.Device : Device = device
    member x.Reference() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuSurfaceReference(handle)
    member x.Release() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuSurfaceRelease(handle)
type SwapChainHandle = struct val mutable public Handle : nativeint end
[<Struct>]
type SwapChain(device : Device, handle : SwapChainHandle) =
    member x.Handle : SwapChainHandle = handle
    member x.Device : Device = device
    member x.Reference() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuSwapChainReference(handle)
    member x.Release() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuSwapChainRelease(handle)
    member x.Configure(Format, AllowedUsage, Width, Height) : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuSwapChainConfigure(handle, Format, AllowedUsage, Width, Height)
    member x.GetCurrentTextureView() : TextureView =
        let device = device
        let handle = handle
        TextureView(device, DawnRaw.wgpuSwapChainGetCurrentTextureView(handle))
    member x.Present() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuSwapChainPresent(handle)
type TextureHandle = struct val mutable public Handle : nativeint end
[<Struct>]
type Texture(device : Device, handle : TextureHandle) =
    member x.Handle : TextureHandle = handle
    member x.Device : Device = device
    member x.Reference() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuTextureReference(handle)
    member x.Release() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuTextureRelease(handle)
    member x.CreateView(Descriptor : TextureViewDescriptor) : TextureView =
        let device = device
        let handle = handle
        Descriptor.Pin(fun _Descriptor ->
            use _Descriptor = fixed [| _Descriptor |]
            TextureView(device, DawnRaw.wgpuTextureCreateView(handle, _Descriptor))
        )
    member x.Destroy() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuTextureDestroy(handle)
type TextureViewHandle = struct val mutable public Handle : nativeint end
[<Struct>]
type TextureView(device : Device, handle : TextureViewHandle) =
    member x.Handle : TextureViewHandle = handle
    member x.Device : Device = device
    member x.Reference() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuTextureViewReference(handle)
    member x.Release() : unit =
        let device = device
        let handle = handle
        DawnRaw.wgpuTextureViewRelease(handle)
