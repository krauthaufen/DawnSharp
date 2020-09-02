namespace rec WebGPU
open System
open System.Security
open System.Runtime.InteropServices
#nowarn "9"
#nowarn "49"

[<StructLayout(LayoutKind.Sequential)>]
type BindGroupHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = BindGroupHandle(0n)
    end
[<StructLayout(LayoutKind.Sequential)>]
type BindGroupLayoutHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = BindGroupLayoutHandle(0n)
    end
[<StructLayout(LayoutKind.Sequential)>]
type BufferHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = BufferHandle(0n)
    end
[<StructLayout(LayoutKind.Sequential)>]
type CommandBufferHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = CommandBufferHandle(0n)
    end
[<StructLayout(LayoutKind.Sequential)>]
type CommandEncoderHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = CommandEncoderHandle(0n)
    end
[<StructLayout(LayoutKind.Sequential)>]
type ComputePassEncoderHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = ComputePassEncoderHandle(0n)
    end
[<StructLayout(LayoutKind.Sequential)>]
type ComputePipelineHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = ComputePipelineHandle(0n)
    end
[<StructLayout(LayoutKind.Sequential)>]
type DeviceHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = DeviceHandle(0n)
    end
[<StructLayout(LayoutKind.Sequential)>]
type FenceHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = FenceHandle(0n)
    end
[<StructLayout(LayoutKind.Sequential)>]
type InstanceHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = InstanceHandle(0n)
    end
[<StructLayout(LayoutKind.Sequential)>]
type PipelineLayoutHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = PipelineLayoutHandle(0n)
    end
[<StructLayout(LayoutKind.Sequential)>]
type QuerySetHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = QuerySetHandle(0n)
    end
[<StructLayout(LayoutKind.Sequential)>]
type QueueHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = QueueHandle(0n)
    end
[<StructLayout(LayoutKind.Sequential)>]
type RenderBundleHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = RenderBundleHandle(0n)
    end
[<StructLayout(LayoutKind.Sequential)>]
type RenderBundleEncoderHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = RenderBundleEncoderHandle(0n)
    end
[<StructLayout(LayoutKind.Sequential)>]
type RenderPassEncoderHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = RenderPassEncoderHandle(0n)
    end
[<StructLayout(LayoutKind.Sequential)>]
type RenderPipelineHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = RenderPipelineHandle(0n)
    end
[<StructLayout(LayoutKind.Sequential)>]
type SamplerHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = SamplerHandle(0n)
    end
[<StructLayout(LayoutKind.Sequential)>]
type ShaderModuleHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = ShaderModuleHandle(0n)
    end
[<StructLayout(LayoutKind.Sequential)>]
type SurfaceHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = SurfaceHandle(0n)
    end
[<StructLayout(LayoutKind.Sequential)>]
type SwapChainHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = SwapChainHandle(0n)
    end
[<StructLayout(LayoutKind.Sequential)>]
type TextureHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = TextureHandle(0n)
    end
[<StructLayout(LayoutKind.Sequential)>]
type TextureViewHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = TextureViewHandle(0n)
    end
type BufferMapCallback = delegate of BufferMapAsyncStatus * nativeint -> unit
type DeviceLostCallback = delegate of nativeint * nativeint -> unit
type ErrorCallback = delegate of ErrorType * nativeint * nativeint -> unit
type FenceOnCompletionCallback = delegate of FenceCompletionStatus * nativeint -> unit


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
type BufferMapAsyncStatus = 
| Success = 0
| Error = 1
| Unknown = 2
| DeviceLost = 3
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
[<Flags>]
type ColorWriteMask = 
| None = 0
| Red = 1
| Green = 2
| Blue = 4
| Alpha = 8
| All = 15
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
type CullMode = 
| None = 0
| Front = 1
| Back = 2
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
type FenceCompletionStatus = 
| Success = 0
| Error = 1
| Unknown = 2
| DeviceLost = 3
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
type LoadOp = 
| Clear = 0
| Load = 1
[<Flags>]
type MapMode = 
| None = 0
| Read = 1
| Write = 2
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
type QueryType = 
| Occlusion = 0
| PipelineStatistics = 1
| Timestamp = 2
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
type StoreOp = 
| Store = 0
| Clear = 1
type TextureAspect = 
| All = 0
| StencilOnly = 1
| DepthOnly = 2
type TextureComponentType = 
| Float = 0
| Sint = 1
| Uint = 2
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
type TextureViewDimension = 
| Undefined = 0
| D1D = 1
| D2D = 2
| D2DArray = 3
| Cube = 4
| CubeArray = 5
| D3D = 6
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


module DawnRaw =
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUAdapterProperties =
        struct
            val mutable public Next : nativeint
            val mutable public DeviceID : int
            val mutable public VendorID : int
            val mutable public Name : nativeint
            val mutable public AdapterType : AdapterType
            val mutable public BackendType : BackendType
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUBindGroupDescriptor =
        struct
            val mutable public Next : nativeint
            val mutable public Label : nativeint
            val mutable public Layout : BindGroupLayoutHandle
            val mutable public EntriesCount : int
            val mutable public Entries : nativeptr<DawnRaw.WGPUBindGroupEntry>
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUBindGroupEntry =
        struct
            val mutable public Binding : int
            val mutable public Buffer : BufferHandle
            val mutable public Offset : uint64
            val mutable public Size : uint64
            val mutable public Sampler : SamplerHandle
            val mutable public TextureView : TextureViewHandle
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUBindGroupLayoutDescriptor =
        struct
            val mutable public Next : nativeint
            val mutable public Label : nativeint
            val mutable public EntriesCount : int
            val mutable public Entries : nativeptr<DawnRaw.WGPUBindGroupLayoutEntry>
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUBindGroupLayoutEntry =
        struct
            val mutable public Binding : int
            val mutable public Visibility : ShaderStage
            val mutable public Type : BindingType
            val mutable public HasDynamicOffset : int
            val mutable public MinBufferBindingSize : uint64
            val mutable public Multisampled : int
            val mutable public ViewDimension : TextureViewDimension
            val mutable public TextureComponentType : TextureComponentType
            val mutable public StorageTextureFormat : TextureFormat
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUBlendDescriptor =
        struct
            val mutable public Operation : BlendOperation
            val mutable public SrcFactor : BlendFactor
            val mutable public DstFactor : BlendFactor
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUBufferCopyView =
        struct
            val mutable public Next : nativeint
            val mutable public Layout : DawnRaw.WGPUTextureDataLayout
            val mutable public Buffer : BufferHandle
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUBufferDescriptor =
        struct
            val mutable public Next : nativeint
            val mutable public Label : nativeint
            val mutable public Usage : BufferUsage
            val mutable public Size : uint64
            val mutable public MappedAtCreation : int
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUColor =
        struct
            val mutable public R : float32
            val mutable public G : float32
            val mutable public B : float32
            val mutable public A : float32
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUColorStateDescriptor =
        struct
            val mutable public Next : nativeint
            val mutable public Format : TextureFormat
            val mutable public AlphaBlend : DawnRaw.WGPUBlendDescriptor
            val mutable public ColorBlend : DawnRaw.WGPUBlendDescriptor
            val mutable public WriteMask : ColorWriteMask
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUCommandBufferDescriptor =
        struct
            val mutable public Next : nativeint
            val mutable public Label : nativeint
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUCommandEncoderDescriptor =
        struct
            val mutable public Next : nativeint
            val mutable public Label : nativeint
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUComputePassDescriptor =
        struct
            val mutable public Next : nativeint
            val mutable public Label : nativeint
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUComputePipelineDescriptor =
        struct
            val mutable public Next : nativeint
            val mutable public Label : nativeint
            val mutable public Layout : PipelineLayoutHandle
            val mutable public ComputeStage : DawnRaw.WGPUProgrammableStageDescriptor
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUDepthStencilStateDescriptor =
        struct
            val mutable public Next : nativeint
            val mutable public Format : TextureFormat
            val mutable public DepthWriteEnabled : int
            val mutable public DepthCompare : CompareFunction
            val mutable public StencilFront : DawnRaw.WGPUStencilStateFaceDescriptor
            val mutable public StencilBack : DawnRaw.WGPUStencilStateFaceDescriptor
            val mutable public StencilReadMask : int
            val mutable public StencilWriteMask : int
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUDeviceProperties =
        struct
            val mutable public TextureCompressionBC : int
            val mutable public ShaderFloat16 : int
            val mutable public PipelineStatisticsQuery : int
            val mutable public TimestampQuery : int
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUExtent3D =
        struct
            val mutable public Width : int
            val mutable public Height : int
            val mutable public Depth : int
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUFenceDescriptor =
        struct
            val mutable public Next : nativeint
            val mutable public Label : nativeint
            val mutable public InitialValue : uint64
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUInstanceDescriptor =
        struct
            val mutable public Next : nativeint
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUOrigin3D =
        struct
            val mutable public X : int
            val mutable public Y : int
            val mutable public Z : int
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUPipelineLayoutDescriptor =
        struct
            val mutable public Next : nativeint
            val mutable public Label : nativeint
            val mutable public BindGroupLayoutsCount : int
            val mutable public BindGroupLayouts : nativeptr<BindGroupLayoutHandle>
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUProgrammableStageDescriptor =
        struct
            val mutable public Next : nativeint
            val mutable public Module : ShaderModuleHandle
            val mutable public EntryPoint : nativeint
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUQuerySetDescriptor =
        struct
            val mutable public Next : nativeint
            val mutable public Label : nativeint
            val mutable public Type : QueryType
            val mutable public Count : int
            val mutable public PipelineStatistics : nativeptr<PipelineStatisticName>
            val mutable public PipelineStatisticsCount : int
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPURasterizationStateDescriptor =
        struct
            val mutable public Next : nativeint
            val mutable public FrontFace : FrontFace
            val mutable public CullMode : CullMode
            val mutable public DepthBias : int32
            val mutable public DepthBiasSlopeScale : float32
            val mutable public DepthBiasClamp : float32
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPURenderBundleDescriptor =
        struct
            val mutable public Next : nativeint
            val mutable public Label : nativeint
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPURenderBundleEncoderDescriptor =
        struct
            val mutable public Next : nativeint
            val mutable public Label : nativeint
            val mutable public ColorFormatsCount : int
            val mutable public ColorFormats : nativeptr<TextureFormat>
            val mutable public DepthStencilFormat : TextureFormat
            val mutable public SampleCount : int
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPURenderPassColorAttachmentDescriptor =
        struct
            val mutable public Attachment : TextureViewHandle
            val mutable public ResolveTarget : TextureViewHandle
            val mutable public LoadOp : LoadOp
            val mutable public StoreOp : StoreOp
            val mutable public ClearColor : DawnRaw.WGPUColor
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPURenderPassDepthStencilAttachmentDescriptor =
        struct
            val mutable public Attachment : TextureViewHandle
            val mutable public DepthLoadOp : LoadOp
            val mutable public DepthStoreOp : StoreOp
            val mutable public ClearDepth : float32
            val mutable public DepthReadOnly : int
            val mutable public StencilLoadOp : LoadOp
            val mutable public StencilStoreOp : StoreOp
            val mutable public ClearStencil : int
            val mutable public StencilReadOnly : int
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPURenderPassDescriptor =
        struct
            val mutable public Next : nativeint
            val mutable public Label : nativeint
            val mutable public ColorAttachmentsCount : int
            val mutable public ColorAttachments : nativeptr<DawnRaw.WGPURenderPassColorAttachmentDescriptor>
            val mutable public DepthStencilAttachment : nativeptr<DawnRaw.WGPURenderPassDepthStencilAttachmentDescriptor>
            val mutable public OcclusionQuerySet : QuerySetHandle
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPURenderPipelineDescriptor =
        struct
            val mutable public Next : nativeint
            val mutable public Label : nativeint
            val mutable public Layout : PipelineLayoutHandle
            val mutable public VertexStage : DawnRaw.WGPUProgrammableStageDescriptor
            val mutable public FragmentStage : nativeptr<DawnRaw.WGPUProgrammableStageDescriptor>
            val mutable public VertexState : nativeptr<DawnRaw.WGPUVertexStateDescriptor>
            val mutable public PrimitiveTopology : PrimitiveTopology
            val mutable public RasterizationState : nativeptr<DawnRaw.WGPURasterizationStateDescriptor>
            val mutable public SampleCount : int
            val mutable public DepthStencilState : nativeptr<DawnRaw.WGPUDepthStencilStateDescriptor>
            val mutable public ColorStatesCount : int
            val mutable public ColorStates : nativeptr<DawnRaw.WGPUColorStateDescriptor>
            val mutable public SampleMask : int
            val mutable public AlphaToCoverageEnabled : int
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPURenderPipelineDescriptorDummyExtension =
        struct
            val mutable public DummyStage : DawnRaw.WGPUProgrammableStageDescriptor
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUSamplerDescriptor =
        struct
            val mutable public Next : nativeint
            val mutable public Label : nativeint
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
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUSamplerDescriptorDummyAnisotropicFiltering =
        struct
            val mutable public MaxAnisotropy : float32
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUShaderModuleSPIRVDescriptor =
        struct
            val mutable public CodeCount : int
            val mutable public Code : nativeptr<int>
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUShaderModuleWGSLDescriptor =
        struct
            val mutable public Source : nativeint
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUShaderModuleDescriptor =
        struct
            val mutable public Next : nativeint
            val mutable public Label : nativeint
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUStencilStateFaceDescriptor =
        struct
            val mutable public Compare : CompareFunction
            val mutable public FailOp : StencilOperation
            val mutable public DepthFailOp : StencilOperation
            val mutable public PassOp : StencilOperation
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUSurfaceDescriptor =
        struct
            val mutable public Next : nativeint
            val mutable public Label : nativeint
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUSurfaceDescriptorFromCanvasHTMLSelector =
        struct
            val mutable public Selector : nativeint
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUSurfaceDescriptorFromMetalLayer =
        struct
            val mutable public Layer : nativeint
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUSurfaceDescriptorFromWindowsHWND =
        struct
            val mutable public Hinstance : nativeint
            val mutable public Hwnd : nativeint
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUSurfaceDescriptorFromXlib =
        struct
            val mutable public Display : nativeint
            val mutable public Window : int
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUSwapChainDescriptor =
        struct
            val mutable public Next : nativeint
            val mutable public Label : nativeint
            val mutable public Usage : TextureUsage
            val mutable public Format : TextureFormat
            val mutable public Width : int
            val mutable public Height : int
            val mutable public PresentMode : PresentMode
            val mutable public Implementation : uint64
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUTextureCopyView =
        struct
            val mutable public Next : nativeint
            val mutable public Texture : TextureHandle
            val mutable public MipLevel : int
            val mutable public Origin : DawnRaw.WGPUOrigin3D
            val mutable public Aspect : TextureAspect
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUTextureDataLayout =
        struct
            val mutable public Next : nativeint
            val mutable public Offset : uint64
            val mutable public BytesPerRow : int
            val mutable public RowsPerImage : int
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUTextureDescriptor =
        struct
            val mutable public Next : nativeint
            val mutable public Label : nativeint
            val mutable public Usage : TextureUsage
            val mutable public Dimension : TextureDimension
            val mutable public Size : DawnRaw.WGPUExtent3D
            val mutable public Format : TextureFormat
            val mutable public MipLevelCount : int
            val mutable public SampleCount : int
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUTextureViewDescriptor =
        struct
            val mutable public Next : nativeint
            val mutable public Label : nativeint
            val mutable public Format : TextureFormat
            val mutable public Dimension : TextureViewDimension
            val mutable public BaseMipLevel : int
            val mutable public MipLevelCount : int
            val mutable public BaseArrayLayer : int
            val mutable public ArrayLayerCount : int
            val mutable public Aspect : TextureAspect
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUVertexAttributeDescriptor =
        struct
            val mutable public Format : VertexFormat
            val mutable public Offset : uint64
            val mutable public ShaderLocation : int
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUVertexBufferLayoutDescriptor =
        struct
            val mutable public ArrayStride : uint64
            val mutable public StepMode : InputStepMode
            val mutable public AttributesCount : int
            val mutable public Attributes : nativeptr<DawnRaw.WGPUVertexAttributeDescriptor>
        end
    [<StructLayout(LayoutKind.Sequential)>]
    type WGPUVertexStateDescriptor =
        struct
            val mutable public Next : nativeint
            val mutable public IndexFormat : IndexFormat
            val mutable public VertexBuffersCount : int
            val mutable public VertexBuffers : nativeptr<DawnRaw.WGPUVertexBufferLayoutDescriptor>
        end
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuBindGroupReference(BindGroupHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuBindGroupRelease(BindGroupHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuBindGroupLayoutReference(BindGroupLayoutHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuBindGroupLayoutRelease(BindGroupLayoutHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuBufferReference(BufferHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuBufferRelease(BufferHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuBufferMapAsync(BufferHandle self, MapMode Mode, unativeint Offset, unativeint Size, nativeint Callback, nativeint Userdata)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern nativeint wgpuBufferGetMappedRange(BufferHandle self, unativeint Offset, unativeint Size)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern nativeint wgpuBufferGetConstMappedRange(BufferHandle self, unativeint Offset, unativeint Size)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuBufferUnmap(BufferHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuBufferDestroy(BufferHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuCommandBufferReference(CommandBufferHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuCommandBufferRelease(CommandBufferHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuCommandEncoderReference(CommandEncoderHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuCommandEncoderRelease(CommandEncoderHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern CommandBufferHandle wgpuCommandEncoderFinish(CommandEncoderHandle self, WGPUCommandBufferDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern ComputePassEncoderHandle wgpuCommandEncoderBeginComputePass(CommandEncoderHandle self, WGPUComputePassDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern RenderPassEncoderHandle wgpuCommandEncoderBeginRenderPass(CommandEncoderHandle self, WGPURenderPassDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuCommandEncoderCopyBufferToBuffer(CommandEncoderHandle self, BufferHandle Source, uint64 SourceOffset, BufferHandle Destination, uint64 DestinationOffset, uint64 Size)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuCommandEncoderCopyBufferToTexture(CommandEncoderHandle self, WGPUBufferCopyView* Source, WGPUTextureCopyView* Destination, WGPUExtent3D* CopySize)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuCommandEncoderCopyTextureToBuffer(CommandEncoderHandle self, WGPUTextureCopyView* Source, WGPUBufferCopyView* Destination, WGPUExtent3D* CopySize)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuCommandEncoderCopyTextureToTexture(CommandEncoderHandle self, WGPUTextureCopyView* Source, WGPUTextureCopyView* Destination, WGPUExtent3D* CopySize)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuCommandEncoderInsertDebugMarker(CommandEncoderHandle self, nativeint MarkerLabel)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuCommandEncoderPopDebugGroup(CommandEncoderHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuCommandEncoderPushDebugGroup(CommandEncoderHandle self, nativeint GroupLabel)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuCommandEncoderResolveQuerySet(CommandEncoderHandle self, QuerySetHandle QuerySet, int FirstQuery, int QueryCount, BufferHandle Destination, uint64 DestinationOffset)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuCommandEncoderWriteTimestamp(CommandEncoderHandle self, QuerySetHandle QuerySet, int QueryIndex)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuComputePassEncoderReference(ComputePassEncoderHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuComputePassEncoderRelease(ComputePassEncoderHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuComputePassEncoderInsertDebugMarker(ComputePassEncoderHandle self, nativeint MarkerLabel)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuComputePassEncoderPopDebugGroup(ComputePassEncoderHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuComputePassEncoderPushDebugGroup(ComputePassEncoderHandle self, nativeint GroupLabel)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuComputePassEncoderSetPipeline(ComputePassEncoderHandle self, ComputePipelineHandle Pipeline)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuComputePassEncoderSetBindGroup(ComputePassEncoderHandle self, int GroupIndex, BindGroupHandle Group, int DynamicOffsetsCount, int* DynamicOffsets)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuComputePassEncoderWriteTimestamp(ComputePassEncoderHandle self, QuerySetHandle QuerySet, int QueryIndex)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuComputePassEncoderDispatch(ComputePassEncoderHandle self, int X, int Y, int Z)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuComputePassEncoderDispatchIndirect(ComputePassEncoderHandle self, BufferHandle IndirectBuffer, uint64 IndirectOffset)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuComputePassEncoderEndPass(ComputePassEncoderHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuComputePipelineReference(ComputePipelineHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuComputePipelineRelease(ComputePipelineHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern BindGroupLayoutHandle wgpuComputePipelineGetBindGroupLayout(ComputePipelineHandle self, int GroupIndex)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuDeviceReference(DeviceHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuDeviceRelease(DeviceHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern BindGroupHandle wgpuDeviceCreateBindGroup(DeviceHandle self, WGPUBindGroupDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern BindGroupLayoutHandle wgpuDeviceCreateBindGroupLayout(DeviceHandle self, WGPUBindGroupLayoutDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern BufferHandle wgpuDeviceCreateBuffer(DeviceHandle self, WGPUBufferDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern BufferHandle wgpuDeviceCreateErrorBuffer(DeviceHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern CommandEncoderHandle wgpuDeviceCreateCommandEncoder(DeviceHandle self, WGPUCommandEncoderDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern ComputePipelineHandle wgpuDeviceCreateComputePipeline(DeviceHandle self, WGPUComputePipelineDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern PipelineLayoutHandle wgpuDeviceCreatePipelineLayout(DeviceHandle self, WGPUPipelineLayoutDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern QuerySetHandle wgpuDeviceCreateQuerySet(DeviceHandle self, WGPUQuerySetDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern RenderBundleEncoderHandle wgpuDeviceCreateRenderBundleEncoder(DeviceHandle self, WGPURenderBundleEncoderDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern RenderPipelineHandle wgpuDeviceCreateRenderPipeline(DeviceHandle self, WGPURenderPipelineDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern SamplerHandle wgpuDeviceCreateSampler(DeviceHandle self, WGPUSamplerDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern ShaderModuleHandle wgpuDeviceCreateShaderModule(DeviceHandle self, WGPUShaderModuleDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern SwapChainHandle wgpuDeviceCreateSwapChain(DeviceHandle self, SurfaceHandle Surface, WGPUSwapChainDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern TextureHandle wgpuDeviceCreateTexture(DeviceHandle self, WGPUTextureDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern QueueHandle wgpuDeviceGetDefaultQueue(DeviceHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuDeviceInjectError(DeviceHandle self, ErrorType Type, nativeint Message)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuDeviceLoseForTesting(DeviceHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuDeviceTick(DeviceHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuDeviceSetUncapturedErrorCallback(DeviceHandle self, nativeint Callback, nativeint Userdata)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuDeviceSetDeviceLostCallback(DeviceHandle self, nativeint Callback, nativeint Userdata)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuDevicePushErrorScope(DeviceHandle self, ErrorFilter Filter)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern int wgpuDevicePopErrorScope(DeviceHandle self, nativeint Callback, nativeint Userdata)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuFenceReference(FenceHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuFenceRelease(FenceHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern uint64 wgpuFenceGetCompletedValue(FenceHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuFenceOnCompletion(FenceHandle self, uint64 Value, nativeint Callback, nativeint Userdata)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuInstanceReference(InstanceHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuInstanceRelease(InstanceHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern SurfaceHandle wgpuInstanceCreateSurface(InstanceHandle self, WGPUSurfaceDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuPipelineLayoutReference(PipelineLayoutHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuPipelineLayoutRelease(PipelineLayoutHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuQuerySetReference(QuerySetHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuQuerySetRelease(QuerySetHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuQuerySetDestroy(QuerySetHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuQueueReference(QueueHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuQueueRelease(QueueHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuQueueSubmit(QueueHandle self, int CommandsCount, CommandBufferHandle* Commands)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuQueueSignal(QueueHandle self, FenceHandle Fence, uint64 SignalValue)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern FenceHandle wgpuQueueCreateFence(QueueHandle self, WGPUFenceDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuQueueWriteBuffer(QueueHandle self, BufferHandle Buffer, uint64 BufferOffset, nativeint Data, unativeint Size)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuQueueWriteTexture(QueueHandle self, WGPUTextureCopyView* Destination, nativeint Data, unativeint DataSize, WGPUTextureDataLayout* DataLayout, WGPUExtent3D* WriteSize)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleReference(RenderBundleHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleRelease(RenderBundleHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleEncoderReference(RenderBundleEncoderHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleEncoderRelease(RenderBundleEncoderHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleEncoderSetPipeline(RenderBundleEncoderHandle self, RenderPipelineHandle Pipeline)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleEncoderSetBindGroup(RenderBundleEncoderHandle self, int GroupIndex, BindGroupHandle Group, int DynamicOffsetsCount, int* DynamicOffsets)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleEncoderDraw(RenderBundleEncoderHandle self, int VertexCount, int InstanceCount, int FirstVertex, int FirstInstance)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleEncoderDrawIndexed(RenderBundleEncoderHandle self, int IndexCount, int InstanceCount, int FirstIndex, int32 BaseVertex, int FirstInstance)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleEncoderDrawIndirect(RenderBundleEncoderHandle self, BufferHandle IndirectBuffer, uint64 IndirectOffset)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleEncoderDrawIndexedIndirect(RenderBundleEncoderHandle self, BufferHandle IndirectBuffer, uint64 IndirectOffset)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleEncoderInsertDebugMarker(RenderBundleEncoderHandle self, nativeint MarkerLabel)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleEncoderPopDebugGroup(RenderBundleEncoderHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleEncoderPushDebugGroup(RenderBundleEncoderHandle self, nativeint GroupLabel)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleEncoderSetVertexBuffer(RenderBundleEncoderHandle self, int Slot, BufferHandle Buffer, uint64 Offset, uint64 Size)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleEncoderSetIndexBuffer(RenderBundleEncoderHandle self, BufferHandle Buffer, uint64 Offset, uint64 Size)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleEncoderSetIndexBufferWithFormat(RenderBundleEncoderHandle self, BufferHandle Buffer, IndexFormat Format, uint64 Offset, uint64 Size)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern RenderBundleHandle wgpuRenderBundleEncoderFinish(RenderBundleEncoderHandle self, WGPURenderBundleDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderReference(RenderPassEncoderHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderRelease(RenderPassEncoderHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderSetPipeline(RenderPassEncoderHandle self, RenderPipelineHandle Pipeline)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderSetBindGroup(RenderPassEncoderHandle self, int GroupIndex, BindGroupHandle Group, int DynamicOffsetsCount, int* DynamicOffsets)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderDraw(RenderPassEncoderHandle self, int VertexCount, int InstanceCount, int FirstVertex, int FirstInstance)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderDrawIndexed(RenderPassEncoderHandle self, int IndexCount, int InstanceCount, int FirstIndex, int32 BaseVertex, int FirstInstance)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderDrawIndirect(RenderPassEncoderHandle self, BufferHandle IndirectBuffer, uint64 IndirectOffset)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderDrawIndexedIndirect(RenderPassEncoderHandle self, BufferHandle IndirectBuffer, uint64 IndirectOffset)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderExecuteBundles(RenderPassEncoderHandle self, int BundlesCount, RenderBundleHandle* Bundles)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderInsertDebugMarker(RenderPassEncoderHandle self, nativeint MarkerLabel)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderPopDebugGroup(RenderPassEncoderHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderPushDebugGroup(RenderPassEncoderHandle self, nativeint GroupLabel)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderSetStencilReference(RenderPassEncoderHandle self, int Reference)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderSetBlendColor(RenderPassEncoderHandle self, WGPUColor* Color)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderSetViewport(RenderPassEncoderHandle self, float32 X, float32 Y, float32 Width, float32 Height, float32 MinDepth, float32 MaxDepth)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderSetScissorRect(RenderPassEncoderHandle self, int X, int Y, int Width, int Height)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderSetVertexBuffer(RenderPassEncoderHandle self, int Slot, BufferHandle Buffer, uint64 Offset, uint64 Size)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderSetIndexBuffer(RenderPassEncoderHandle self, BufferHandle Buffer, uint64 Offset, uint64 Size)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderSetIndexBufferWithFormat(RenderPassEncoderHandle self, BufferHandle Buffer, IndexFormat Format, uint64 Offset, uint64 Size)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderWriteTimestamp(RenderPassEncoderHandle self, QuerySetHandle QuerySet, int QueryIndex)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPassEncoderEndPass(RenderPassEncoderHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPipelineReference(RenderPipelineHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPipelineRelease(RenderPipelineHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern BindGroupLayoutHandle wgpuRenderPipelineGetBindGroupLayout(RenderPipelineHandle self, int GroupIndex)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuSamplerReference(SamplerHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuSamplerRelease(SamplerHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuShaderModuleReference(ShaderModuleHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuShaderModuleRelease(ShaderModuleHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuSurfaceReference(SurfaceHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuSurfaceRelease(SurfaceHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuSwapChainReference(SwapChainHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuSwapChainRelease(SwapChainHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuSwapChainConfigure(SwapChainHandle self, TextureFormat Format, TextureUsage AllowedUsage, int Width, int Height)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern TextureViewHandle wgpuSwapChainGetCurrentTextureView(SwapChainHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuSwapChainPresent(SwapChainHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuTextureReference(TextureHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuTextureRelease(TextureHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern TextureViewHandle wgpuTextureCreateView(TextureHandle self, WGPUTextureViewDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuTextureDestroy(TextureHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuTextureViewReference(TextureViewHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuTextureViewRelease(TextureViewHandle self)


[<Struct>]
type AdapterProperties =
    {
        DeviceID : int
        VendorID : int
        Name : string
        AdapterType : AdapterType
        BackendType : BackendType
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUAdapterProperties -> 'a) : 'a = 
        let x = x
        let _DeviceID = x.DeviceID
        let _VendorID = x.VendorID
        let _Name = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Name
        try
            let _AdapterType = x.AdapterType
            let _BackendType = x.BackendType
            let mutable native = Unchecked.defaultof<DawnRaw.WGPUAdapterProperties>
            native.Next <- 0n
            native.DeviceID <- _DeviceID
            native.VendorID <- _VendorID
            native.Name <- _Name
            native.AdapterType <- _AdapterType
            native.BackendType <- _BackendType
            callback native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _Name
[<Struct>]
type BindGroupDescriptor =
    {
        Label : string
        Layout : BindGroupLayout
        Entries : array<BindGroupEntry>
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUBindGroupDescriptor -> 'a) : 'a = 
        let x = x
        let _Label = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            let _Layout = (if isNull x.Layout then BindGroupLayoutHandle.Null else x.Layout.Handle)
            let _EntriesCount = if isNull x.Entries then 0 else x.Entries.Length
            let rec _EntriesCont (inputs : array<BindGroupEntry>) (outputs : array<DawnRaw.WGPUBindGroupEntry>) (i : int) =
                if i >= _EntriesCount then
                    use _Entries = fixed outputs
                    let mutable native = Unchecked.defaultof<DawnRaw.WGPUBindGroupDescriptor>
                    native.Next <- 0n
                    native.Label <- _Label
                    native.Layout <- _Layout
                    native.EntriesCount <- _EntriesCount
                    native.Entries <- _Entries
                    callback native
                else
                    inputs.[i].Pin(fun n -> outputs.[i] <- n; _EntriesCont inputs outputs (i + 1))
            _EntriesCont x.Entries (if _EntriesCount > 0 then Array.zeroCreate _EntriesCount else null) 0
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _Label
[<Struct>]
type BindGroupEntry =
    {
        Binding : int
        Buffer : Buffer
        Offset : uint64
        Size : uint64
        Sampler : Sampler
        TextureView : TextureView
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUBindGroupEntry -> 'a) : 'a = 
        let x = x
        let _Binding = x.Binding
        let _Buffer = (if isNull x.Buffer then BufferHandle.Null else x.Buffer.Handle)
        let _Offset = x.Offset
        let _Size = x.Size
        let _Sampler = (if isNull x.Sampler then SamplerHandle.Null else x.Sampler.Handle)
        let _TextureView = (if isNull x.TextureView then TextureViewHandle.Null else x.TextureView.Handle)
        let mutable native = Unchecked.defaultof<DawnRaw.WGPUBindGroupEntry>
        native.Binding <- _Binding
        native.Buffer <- _Buffer
        native.Offset <- _Offset
        native.Size <- _Size
        native.Sampler <- _Sampler
        native.TextureView <- _TextureView
        callback native
[<Struct>]
type BindGroupLayoutDescriptor =
    {
        Label : string
        Entries : array<BindGroupLayoutEntry>
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUBindGroupLayoutDescriptor -> 'a) : 'a = 
        let x = x
        let _Label = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            let _EntriesCount = if isNull x.Entries then 0 else x.Entries.Length
            let rec _EntriesCont (inputs : array<BindGroupLayoutEntry>) (outputs : array<DawnRaw.WGPUBindGroupLayoutEntry>) (i : int) =
                if i >= _EntriesCount then
                    use _Entries = fixed outputs
                    let mutable native = Unchecked.defaultof<DawnRaw.WGPUBindGroupLayoutDescriptor>
                    native.Next <- 0n
                    native.Label <- _Label
                    native.EntriesCount <- _EntriesCount
                    native.Entries <- _Entries
                    callback native
                else
                    inputs.[i].Pin(fun n -> outputs.[i] <- n; _EntriesCont inputs outputs (i + 1))
            _EntriesCont x.Entries (if _EntriesCount > 0 then Array.zeroCreate _EntriesCount else null) 0
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _Label
[<Struct>]
type BindGroupLayoutEntry =
    {
        Binding : int
        Visibility : ShaderStage
        Type : BindingType
        HasDynamicOffset : bool
        MinBufferBindingSize : uint64
        Multisampled : bool
        ViewDimension : TextureViewDimension
        TextureComponentType : TextureComponentType
        StorageTextureFormat : TextureFormat
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUBindGroupLayoutEntry -> 'a) : 'a = 
        let x = x
        let _Binding = x.Binding
        let _Visibility = x.Visibility
        let _Type = x.Type
        let _HasDynamicOffset = (if x.HasDynamicOffset then 1 else 0)
        let _MinBufferBindingSize = x.MinBufferBindingSize
        let _Multisampled = (if x.Multisampled then 1 else 0)
        let _ViewDimension = x.ViewDimension
        let _TextureComponentType = x.TextureComponentType
        let _StorageTextureFormat = x.StorageTextureFormat
        let mutable native = Unchecked.defaultof<DawnRaw.WGPUBindGroupLayoutEntry>
        native.Binding <- _Binding
        native.Visibility <- _Visibility
        native.Type <- _Type
        native.HasDynamicOffset <- _HasDynamicOffset
        native.MinBufferBindingSize <- _MinBufferBindingSize
        native.Multisampled <- _Multisampled
        native.ViewDimension <- _ViewDimension
        native.TextureComponentType <- _TextureComponentType
        native.StorageTextureFormat <- _StorageTextureFormat
        callback native
[<Struct>]
type BlendDescriptor =
    {
        Operation : BlendOperation
        SrcFactor : BlendFactor
        DstFactor : BlendFactor
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUBlendDescriptor -> 'a) : 'a = 
        let x = x
        let _Operation = x.Operation
        let _SrcFactor = x.SrcFactor
        let _DstFactor = x.DstFactor
        let mutable native = Unchecked.defaultof<DawnRaw.WGPUBlendDescriptor>
        native.Operation <- _Operation
        native.SrcFactor <- _SrcFactor
        native.DstFactor <- _DstFactor
        callback native
[<Struct>]
type BufferCopyView =
    {
        Layout : TextureDataLayout
        Buffer : Buffer
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUBufferCopyView -> 'a) : 'a = 
        let x = x
        x.Layout.Pin (fun _Layout ->
            let _Buffer = (if isNull x.Buffer then BufferHandle.Null else x.Buffer.Handle)
            let mutable native = Unchecked.defaultof<DawnRaw.WGPUBufferCopyView>
            native.Next <- 0n
            native.Layout <- _Layout
            native.Buffer <- _Buffer
            callback native
        )
[<Struct>]
type BufferDescriptor =
    {
        Label : string
        Usage : BufferUsage
        Size : uint64
        MappedAtCreation : bool
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUBufferDescriptor -> 'a) : 'a = 
        let x = x
        let _Label = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            let _Usage = x.Usage
            let _Size = x.Size
            let _MappedAtCreation = (if x.MappedAtCreation then 1 else 0)
            let mutable native = Unchecked.defaultof<DawnRaw.WGPUBufferDescriptor>
            native.Next <- 0n
            native.Label <- _Label
            native.Usage <- _Usage
            native.Size <- _Size
            native.MappedAtCreation <- _MappedAtCreation
            callback native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _Label
[<Struct>]
type Color =
    {
        R : float32
        G : float32
        B : float32
        A : float32
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUColor -> 'a) : 'a = 
        let x = x
        let _R = x.R
        let _G = x.G
        let _B = x.B
        let _A = x.A
        let mutable native = Unchecked.defaultof<DawnRaw.WGPUColor>
        native.R <- _R
        native.G <- _G
        native.B <- _B
        native.A <- _A
        callback native
[<Struct>]
type ColorStateDescriptor =
    {
        Format : TextureFormat
        AlphaBlend : BlendDescriptor
        ColorBlend : BlendDescriptor
        WriteMask : ColorWriteMask
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUColorStateDescriptor -> 'a) : 'a = 
        let x = x
        let _Format = x.Format
        x.AlphaBlend.Pin (fun _AlphaBlend ->
            x.ColorBlend.Pin (fun _ColorBlend ->
                let _WriteMask = x.WriteMask
                let mutable native = Unchecked.defaultof<DawnRaw.WGPUColorStateDescriptor>
                native.Next <- 0n
                native.Format <- _Format
                native.AlphaBlend <- _AlphaBlend
                native.ColorBlend <- _ColorBlend
                native.WriteMask <- _WriteMask
                callback native
            )
        )
[<Struct>]
type CommandBufferDescriptor =
    {
        Label : string
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUCommandBufferDescriptor -> 'a) : 'a = 
        let x = x
        let _Label = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            let mutable native = Unchecked.defaultof<DawnRaw.WGPUCommandBufferDescriptor>
            native.Next <- 0n
            native.Label <- _Label
            callback native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _Label
[<Struct>]
type CommandEncoderDescriptor =
    {
        Label : string
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUCommandEncoderDescriptor -> 'a) : 'a = 
        let x = x
        let _Label = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            let mutable native = Unchecked.defaultof<DawnRaw.WGPUCommandEncoderDescriptor>
            native.Next <- 0n
            native.Label <- _Label
            callback native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _Label
[<Struct>]
type ComputePassDescriptor =
    {
        Label : string
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUComputePassDescriptor -> 'a) : 'a = 
        let x = x
        let _Label = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            let mutable native = Unchecked.defaultof<DawnRaw.WGPUComputePassDescriptor>
            native.Next <- 0n
            native.Label <- _Label
            callback native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _Label
[<Struct>]
type ComputePipelineDescriptor =
    {
        Label : string
        Layout : PipelineLayout
        ComputeStage : ProgrammableStageDescriptor
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUComputePipelineDescriptor -> 'a) : 'a = 
        let x = x
        let _Label = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            let _Layout = (if isNull x.Layout then PipelineLayoutHandle.Null else x.Layout.Handle)
            x.ComputeStage.Pin (fun _ComputeStage ->
                let mutable native = Unchecked.defaultof<DawnRaw.WGPUComputePipelineDescriptor>
                native.Next <- 0n
                native.Label <- _Label
                native.Layout <- _Layout
                native.ComputeStage <- _ComputeStage
                callback native
            )
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _Label
[<Struct>]
type DepthStencilStateDescriptor =
    {
        Format : TextureFormat
        DepthWriteEnabled : bool
        DepthCompare : CompareFunction
        StencilFront : StencilStateFaceDescriptor
        StencilBack : StencilStateFaceDescriptor
        StencilReadMask : int
        StencilWriteMask : int
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUDepthStencilStateDescriptor -> 'a) : 'a = 
        let x = x
        let _Format = x.Format
        let _DepthWriteEnabled = (if x.DepthWriteEnabled then 1 else 0)
        let _DepthCompare = x.DepthCompare
        x.StencilFront.Pin (fun _StencilFront ->
            x.StencilBack.Pin (fun _StencilBack ->
                let _StencilReadMask = x.StencilReadMask
                let _StencilWriteMask = x.StencilWriteMask
                let mutable native = Unchecked.defaultof<DawnRaw.WGPUDepthStencilStateDescriptor>
                native.Next <- 0n
                native.Format <- _Format
                native.DepthWriteEnabled <- _DepthWriteEnabled
                native.DepthCompare <- _DepthCompare
                native.StencilFront <- _StencilFront
                native.StencilBack <- _StencilBack
                native.StencilReadMask <- _StencilReadMask
                native.StencilWriteMask <- _StencilWriteMask
                callback native
            )
        )
[<Struct>]
type DeviceProperties =
    {
        TextureCompressionBC : bool
        ShaderFloat16 : bool
        PipelineStatisticsQuery : bool
        TimestampQuery : bool
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUDeviceProperties -> 'a) : 'a = 
        let x = x
        let _TextureCompressionBC = (if x.TextureCompressionBC then 1 else 0)
        let _ShaderFloat16 = (if x.ShaderFloat16 then 1 else 0)
        let _PipelineStatisticsQuery = (if x.PipelineStatisticsQuery then 1 else 0)
        let _TimestampQuery = (if x.TimestampQuery then 1 else 0)
        let mutable native = Unchecked.defaultof<DawnRaw.WGPUDeviceProperties>
        native.TextureCompressionBC <- _TextureCompressionBC
        native.ShaderFloat16 <- _ShaderFloat16
        native.PipelineStatisticsQuery <- _PipelineStatisticsQuery
        native.TimestampQuery <- _TimestampQuery
        callback native
[<Struct>]
type Extent3D =
    {
        Width : int
        Height : int
        Depth : int
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUExtent3D -> 'a) : 'a = 
        let x = x
        let _Width = x.Width
        let _Height = x.Height
        let _Depth = x.Depth
        let mutable native = Unchecked.defaultof<DawnRaw.WGPUExtent3D>
        native.Width <- _Width
        native.Height <- _Height
        native.Depth <- _Depth
        callback native
[<Struct>]
type FenceDescriptor =
    {
        Label : string
        InitialValue : uint64
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUFenceDescriptor -> 'a) : 'a = 
        let x = x
        let _Label = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            let _InitialValue = x.InitialValue
            let mutable native = Unchecked.defaultof<DawnRaw.WGPUFenceDescriptor>
            native.Next <- 0n
            native.Label <- _Label
            native.InitialValue <- _InitialValue
            callback native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _Label
[<Struct>]
type InstanceDescriptor = 
    member x.Pin<'a>(callback : DawnRaw.WGPUInstanceDescriptor -> 'a) : 'a = 
        let mutable native = Unchecked.defaultof<DawnRaw.WGPUInstanceDescriptor>
        native.Next <- 0n
        callback native
[<Struct>]
type Origin3D =
    {
        X : int
        Y : int
        Z : int
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUOrigin3D -> 'a) : 'a = 
        let x = x
        let _X = x.X
        let _Y = x.Y
        let _Z = x.Z
        let mutable native = Unchecked.defaultof<DawnRaw.WGPUOrigin3D>
        native.X <- _X
        native.Y <- _Y
        native.Z <- _Z
        callback native
[<Struct>]
type PipelineLayoutDescriptor =
    {
        Label : string
        BindGroupLayouts : array<BindGroupLayout>
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUPipelineLayoutDescriptor -> 'a) : 'a = 
        let x = x
        let _Label = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            use _BindGroupLayouts = fixed (x.BindGroupLayouts |> Array.map (fun v -> if isNull v then BindGroupLayoutHandle.Null else v.Handle))
            let _BindGroupLayoutsCount = x.BindGroupLayouts.Length
            let mutable native = Unchecked.defaultof<DawnRaw.WGPUPipelineLayoutDescriptor>
            native.Next <- 0n
            native.Label <- _Label
            native.BindGroupLayoutsCount <- _BindGroupLayoutsCount
            native.BindGroupLayouts <- _BindGroupLayouts
            callback native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _Label
[<Struct>]
type ProgrammableStageDescriptor =
    {
        Module : ShaderModule
        EntryPoint : string
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUProgrammableStageDescriptor -> 'a) : 'a = 
        let x = x
        let _Module = (if isNull x.Module then ShaderModuleHandle.Null else x.Module.Handle)
        let _EntryPoint = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.EntryPoint
        try
            let mutable native = Unchecked.defaultof<DawnRaw.WGPUProgrammableStageDescriptor>
            native.Next <- 0n
            native.Module <- _Module
            native.EntryPoint <- _EntryPoint
            callback native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _EntryPoint
[<Struct>]
type QuerySetDescriptor =
    {
        Label : string
        Type : QueryType
        Count : int
        PipelineStatistics : option<PipelineStatisticName>
        PipelineStatisticsCount : int
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUQuerySetDescriptor -> 'a) : 'a = 
        let x = x
        let _Label = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            let _Type = x.Type
            let _Count = x.Count
            use _PipelineStatistics = fixed (x.PipelineStatistics |> Option.toArray)
            let _PipelineStatisticsCount = x.PipelineStatisticsCount
            let mutable native = Unchecked.defaultof<DawnRaw.WGPUQuerySetDescriptor>
            native.Next <- 0n
            native.Label <- _Label
            native.Type <- _Type
            native.Count <- _Count
            native.PipelineStatistics <- _PipelineStatistics
            native.PipelineStatisticsCount <- _PipelineStatisticsCount
            callback native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _Label
[<Struct>]
type RasterizationStateDescriptor =
    {
        FrontFace : FrontFace
        CullMode : CullMode
        DepthBias : int32
        DepthBiasSlopeScale : float32
        DepthBiasClamp : float32
    }
    member x.Pin<'a>(callback : DawnRaw.WGPURasterizationStateDescriptor -> 'a) : 'a = 
        let x = x
        let _FrontFace = x.FrontFace
        let _CullMode = x.CullMode
        let _DepthBias = x.DepthBias
        let _DepthBiasSlopeScale = x.DepthBiasSlopeScale
        let _DepthBiasClamp = x.DepthBiasClamp
        let mutable native = Unchecked.defaultof<DawnRaw.WGPURasterizationStateDescriptor>
        native.Next <- 0n
        native.FrontFace <- _FrontFace
        native.CullMode <- _CullMode
        native.DepthBias <- _DepthBias
        native.DepthBiasSlopeScale <- _DepthBiasSlopeScale
        native.DepthBiasClamp <- _DepthBiasClamp
        callback native
[<Struct>]
type RenderBundleDescriptor =
    {
        Label : string
    }
    member x.Pin<'a>(callback : DawnRaw.WGPURenderBundleDescriptor -> 'a) : 'a = 
        let x = x
        let _Label = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            let mutable native = Unchecked.defaultof<DawnRaw.WGPURenderBundleDescriptor>
            native.Next <- 0n
            native.Label <- _Label
            callback native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _Label
[<Struct>]
type RenderBundleEncoderDescriptor =
    {
        Label : string
        ColorFormats : array<TextureFormat>
        DepthStencilFormat : TextureFormat
        SampleCount : int
    }
    member x.Pin<'a>(callback : DawnRaw.WGPURenderBundleEncoderDescriptor -> 'a) : 'a = 
        let x = x
        let _Label = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            use _ColorFormats = fixed x.ColorFormats
            let _ColorFormatsCount = x.ColorFormats.Length
            let _DepthStencilFormat = x.DepthStencilFormat
            let _SampleCount = x.SampleCount
            let mutable native = Unchecked.defaultof<DawnRaw.WGPURenderBundleEncoderDescriptor>
            native.Next <- 0n
            native.Label <- _Label
            native.ColorFormatsCount <- _ColorFormatsCount
            native.ColorFormats <- _ColorFormats
            native.DepthStencilFormat <- _DepthStencilFormat
            native.SampleCount <- _SampleCount
            callback native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _Label
[<Struct>]
type RenderPassColorAttachmentDescriptor =
    {
        Attachment : TextureView
        ResolveTarget : TextureView
        LoadOp : LoadOp
        StoreOp : StoreOp
        ClearColor : Color
    }
    member x.Pin<'a>(callback : DawnRaw.WGPURenderPassColorAttachmentDescriptor -> 'a) : 'a = 
        let x = x
        let _Attachment = (if isNull x.Attachment then TextureViewHandle.Null else x.Attachment.Handle)
        let _ResolveTarget = (if isNull x.ResolveTarget then TextureViewHandle.Null else x.ResolveTarget.Handle)
        let _LoadOp = x.LoadOp
        let _StoreOp = x.StoreOp
        x.ClearColor.Pin (fun _ClearColor ->
            let mutable native = Unchecked.defaultof<DawnRaw.WGPURenderPassColorAttachmentDescriptor>
            native.Attachment <- _Attachment
            native.ResolveTarget <- _ResolveTarget
            native.LoadOp <- _LoadOp
            native.StoreOp <- _StoreOp
            native.ClearColor <- _ClearColor
            callback native
        )
[<Struct>]
type RenderPassDepthStencilAttachmentDescriptor =
    {
        Attachment : TextureView
        DepthLoadOp : LoadOp
        DepthStoreOp : StoreOp
        ClearDepth : float32
        DepthReadOnly : bool
        StencilLoadOp : LoadOp
        StencilStoreOp : StoreOp
        ClearStencil : int
        StencilReadOnly : bool
    }
    member x.Pin<'a>(callback : DawnRaw.WGPURenderPassDepthStencilAttachmentDescriptor -> 'a) : 'a = 
        let x = x
        let _Attachment = (if isNull x.Attachment then TextureViewHandle.Null else x.Attachment.Handle)
        let _DepthLoadOp = x.DepthLoadOp
        let _DepthStoreOp = x.DepthStoreOp
        let _ClearDepth = x.ClearDepth
        let _DepthReadOnly = (if x.DepthReadOnly then 1 else 0)
        let _StencilLoadOp = x.StencilLoadOp
        let _StencilStoreOp = x.StencilStoreOp
        let _ClearStencil = x.ClearStencil
        let _StencilReadOnly = (if x.StencilReadOnly then 1 else 0)
        let mutable native = Unchecked.defaultof<DawnRaw.WGPURenderPassDepthStencilAttachmentDescriptor>
        native.Attachment <- _Attachment
        native.DepthLoadOp <- _DepthLoadOp
        native.DepthStoreOp <- _DepthStoreOp
        native.ClearDepth <- _ClearDepth
        native.DepthReadOnly <- _DepthReadOnly
        native.StencilLoadOp <- _StencilLoadOp
        native.StencilStoreOp <- _StencilStoreOp
        native.ClearStencil <- _ClearStencil
        native.StencilReadOnly <- _StencilReadOnly
        callback native
[<Struct>]
type RenderPassDescriptor =
    {
        Label : string
        ColorAttachments : array<RenderPassColorAttachmentDescriptor>
        DepthStencilAttachment : option<RenderPassDepthStencilAttachmentDescriptor>
        OcclusionQuerySet : QuerySet
    }
    member x.Pin<'a>(callback : DawnRaw.WGPURenderPassDescriptor -> 'a) : 'a = 
        let x = x
        let _Label = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            let _ColorAttachmentsCount = if isNull x.ColorAttachments then 0 else x.ColorAttachments.Length
            let rec _ColorAttachmentsCont (inputs : array<RenderPassColorAttachmentDescriptor>) (outputs : array<DawnRaw.WGPURenderPassColorAttachmentDescriptor>) (i : int) =
                if i >= _ColorAttachmentsCount then
                    use _ColorAttachments = fixed outputs
                    let inline _DepthStencilAttachmentCont _DepthStencilAttachment = 
                        let _OcclusionQuerySet = (if isNull x.OcclusionQuerySet then QuerySetHandle.Null else x.OcclusionQuerySet.Handle)
                        let mutable native = Unchecked.defaultof<DawnRaw.WGPURenderPassDescriptor>
                        native.Next <- 0n
                        native.Label <- _Label
                        native.ColorAttachmentsCount <- _ColorAttachmentsCount
                        native.ColorAttachments <- _ColorAttachments
                        native.DepthStencilAttachment <- _DepthStencilAttachment
                        native.OcclusionQuerySet <- _OcclusionQuerySet
                        callback native
                    match x.DepthStencilAttachment with
                    | Some v -> v.Pin(fun n -> use ptr = fixed [|n|] in _DepthStencilAttachmentCont ptr)
                    | None -> _DepthStencilAttachmentCont (Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt 0n)
                else
                    inputs.[i].Pin(fun n -> outputs.[i] <- n; _ColorAttachmentsCont inputs outputs (i + 1))
            _ColorAttachmentsCont x.ColorAttachments (if _ColorAttachmentsCount > 0 then Array.zeroCreate _ColorAttachmentsCount else null) 0
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _Label
[<Struct>]
type RenderPipelineDescriptor =
    {
        Label : string
        Layout : PipelineLayout
        VertexStage : ProgrammableStageDescriptor
        FragmentStage : option<ProgrammableStageDescriptor>
        VertexState : option<VertexStateDescriptor>
        PrimitiveTopology : PrimitiveTopology
        RasterizationState : option<RasterizationStateDescriptor>
        SampleCount : int
        DepthStencilState : option<DepthStencilStateDescriptor>
        ColorStates : array<ColorStateDescriptor>
        SampleMask : int
        AlphaToCoverageEnabled : bool
    }
    member x.Pin<'a>(callback : DawnRaw.WGPURenderPipelineDescriptor -> 'a) : 'a = 
        let x = x
        let _Label = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            let _Layout = (if isNull x.Layout then PipelineLayoutHandle.Null else x.Layout.Handle)
            x.VertexStage.Pin (fun _VertexStage ->
                let inline _FragmentStageCont _FragmentStage = 
                    let inline _VertexStateCont _VertexState = 
                        let _PrimitiveTopology = x.PrimitiveTopology
                        let inline _RasterizationStateCont _RasterizationState = 
                            let _SampleCount = x.SampleCount
                            let inline _DepthStencilStateCont _DepthStencilState = 
                                let _ColorStatesCount = if isNull x.ColorStates then 0 else x.ColorStates.Length
                                let rec _ColorStatesCont (inputs : array<ColorStateDescriptor>) (outputs : array<DawnRaw.WGPUColorStateDescriptor>) (i : int) =
                                    if i >= _ColorStatesCount then
                                        use _ColorStates = fixed outputs
                                        let _SampleMask = x.SampleMask
                                        let _AlphaToCoverageEnabled = (if x.AlphaToCoverageEnabled then 1 else 0)
                                        let mutable native = Unchecked.defaultof<DawnRaw.WGPURenderPipelineDescriptor>
                                        native.Next <- 0n
                                        native.Label <- _Label
                                        native.Layout <- _Layout
                                        native.VertexStage <- _VertexStage
                                        native.FragmentStage <- _FragmentStage
                                        native.VertexState <- _VertexState
                                        native.PrimitiveTopology <- _PrimitiveTopology
                                        native.RasterizationState <- _RasterizationState
                                        native.SampleCount <- _SampleCount
                                        native.DepthStencilState <- _DepthStencilState
                                        native.ColorStatesCount <- _ColorStatesCount
                                        native.ColorStates <- _ColorStates
                                        native.SampleMask <- _SampleMask
                                        native.AlphaToCoverageEnabled <- _AlphaToCoverageEnabled
                                        callback native
                                    else
                                        inputs.[i].Pin(fun n -> outputs.[i] <- n; _ColorStatesCont inputs outputs (i + 1))
                                _ColorStatesCont x.ColorStates (if _ColorStatesCount > 0 then Array.zeroCreate _ColorStatesCount else null) 0
                            match x.DepthStencilState with
                            | Some v -> v.Pin(fun n -> use ptr = fixed [|n|] in _DepthStencilStateCont ptr)
                            | None -> _DepthStencilStateCont (Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt 0n)
                        match x.RasterizationState with
                        | Some v -> v.Pin(fun n -> use ptr = fixed [|n|] in _RasterizationStateCont ptr)
                        | None -> _RasterizationStateCont (Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt 0n)
                    match x.VertexState with
                    | Some v -> v.Pin(fun n -> use ptr = fixed [|n|] in _VertexStateCont ptr)
                    | None -> _VertexStateCont (Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt 0n)
                match x.FragmentStage with
                | Some v -> v.Pin(fun n -> use ptr = fixed [|n|] in _FragmentStageCont ptr)
                | None -> _FragmentStageCont (Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt 0n)
            )
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _Label
[<Struct>]
type RenderPipelineDescriptorDummyExtension =
    {
        DummyStage : ProgrammableStageDescriptor
    }
    member x.Pin<'a>(callback : DawnRaw.WGPURenderPipelineDescriptorDummyExtension -> 'a) : 'a = 
        let x = x
        x.DummyStage.Pin (fun _DummyStage ->
            let mutable native = Unchecked.defaultof<DawnRaw.WGPURenderPipelineDescriptorDummyExtension>
            native.DummyStage <- _DummyStage
            callback native
        )
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
    member x.Pin<'a>(callback : DawnRaw.WGPUSamplerDescriptor -> 'a) : 'a = 
        let x = x
        let _Label = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            let _AddressModeU = x.AddressModeU
            let _AddressModeV = x.AddressModeV
            let _AddressModeW = x.AddressModeW
            let _MagFilter = x.MagFilter
            let _MinFilter = x.MinFilter
            let _MipmapFilter = x.MipmapFilter
            let _LodMinClamp = x.LodMinClamp
            let _LodMaxClamp = x.LodMaxClamp
            let _Compare = x.Compare
            let mutable native = Unchecked.defaultof<DawnRaw.WGPUSamplerDescriptor>
            native.Next <- 0n
            native.Label <- _Label
            native.AddressModeU <- _AddressModeU
            native.AddressModeV <- _AddressModeV
            native.AddressModeW <- _AddressModeW
            native.MagFilter <- _MagFilter
            native.MinFilter <- _MinFilter
            native.MipmapFilter <- _MipmapFilter
            native.LodMinClamp <- _LodMinClamp
            native.LodMaxClamp <- _LodMaxClamp
            native.Compare <- _Compare
            callback native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _Label
[<Struct>]
type SamplerDescriptorDummyAnisotropicFiltering =
    {
        MaxAnisotropy : float32
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUSamplerDescriptorDummyAnisotropicFiltering -> 'a) : 'a = 
        let x = x
        let _MaxAnisotropy = x.MaxAnisotropy
        let mutable native = Unchecked.defaultof<DawnRaw.WGPUSamplerDescriptorDummyAnisotropicFiltering>
        native.MaxAnisotropy <- _MaxAnisotropy
        callback native
[<Struct>]
type ShaderModuleSPIRVDescriptor =
    {
        Code : array<int>
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUShaderModuleSPIRVDescriptor -> 'a) : 'a = 
        let x = x
        use _Code = fixed x.Code
        let _CodeCount = x.Code.Length
        let mutable native = Unchecked.defaultof<DawnRaw.WGPUShaderModuleSPIRVDescriptor>
        native.CodeCount <- _CodeCount
        native.Code <- _Code
        callback native
[<Struct>]
type ShaderModuleWGSLDescriptor =
    {
        Source : string
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUShaderModuleWGSLDescriptor -> 'a) : 'a = 
        let x = x
        let _Source = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Source
        try
            let mutable native = Unchecked.defaultof<DawnRaw.WGPUShaderModuleWGSLDescriptor>
            native.Source <- _Source
            callback native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _Source
[<Struct>]
type ShaderModuleDescriptor =
    {
        Label : string
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUShaderModuleDescriptor -> 'a) : 'a = 
        let x = x
        let _Label = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            let mutable native = Unchecked.defaultof<DawnRaw.WGPUShaderModuleDescriptor>
            native.Next <- 0n
            native.Label <- _Label
            callback native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _Label
[<Struct>]
type StencilStateFaceDescriptor =
    {
        Compare : CompareFunction
        FailOp : StencilOperation
        DepthFailOp : StencilOperation
        PassOp : StencilOperation
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUStencilStateFaceDescriptor -> 'a) : 'a = 
        let x = x
        let _Compare = x.Compare
        let _FailOp = x.FailOp
        let _DepthFailOp = x.DepthFailOp
        let _PassOp = x.PassOp
        let mutable native = Unchecked.defaultof<DawnRaw.WGPUStencilStateFaceDescriptor>
        native.Compare <- _Compare
        native.FailOp <- _FailOp
        native.DepthFailOp <- _DepthFailOp
        native.PassOp <- _PassOp
        callback native
[<Struct>]
type SurfaceDescriptor =
    {
        Label : string
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUSurfaceDescriptor -> 'a) : 'a = 
        let x = x
        let _Label = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            let mutable native = Unchecked.defaultof<DawnRaw.WGPUSurfaceDescriptor>
            native.Next <- 0n
            native.Label <- _Label
            callback native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _Label
[<Struct>]
type SurfaceDescriptorFromCanvasHTMLSelector =
    {
        Selector : string
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUSurfaceDescriptorFromCanvasHTMLSelector -> 'a) : 'a = 
        let x = x
        let _Selector = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Selector
        try
            let mutable native = Unchecked.defaultof<DawnRaw.WGPUSurfaceDescriptorFromCanvasHTMLSelector>
            native.Selector <- _Selector
            callback native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _Selector
[<Struct>]
type SurfaceDescriptorFromMetalLayer =
    {
        Layer : nativeint
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUSurfaceDescriptorFromMetalLayer -> 'a) : 'a = 
        let x = x
        let _Layer = x.Layer
        let mutable native = Unchecked.defaultof<DawnRaw.WGPUSurfaceDescriptorFromMetalLayer>
        native.Layer <- _Layer
        callback native
[<Struct>]
type SurfaceDescriptorFromWindowsHWND =
    {
        Hinstance : nativeint
        Hwnd : nativeint
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUSurfaceDescriptorFromWindowsHWND -> 'a) : 'a = 
        let x = x
        let _Hinstance = x.Hinstance
        let _Hwnd = x.Hwnd
        let mutable native = Unchecked.defaultof<DawnRaw.WGPUSurfaceDescriptorFromWindowsHWND>
        native.Hinstance <- _Hinstance
        native.Hwnd <- _Hwnd
        callback native
[<Struct>]
type SurfaceDescriptorFromXlib =
    {
        Display : nativeint
        Window : int
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUSurfaceDescriptorFromXlib -> 'a) : 'a = 
        let x = x
        let _Display = x.Display
        let _Window = x.Window
        let mutable native = Unchecked.defaultof<DawnRaw.WGPUSurfaceDescriptorFromXlib>
        native.Display <- _Display
        native.Window <- _Window
        callback native
[<Struct>]
type SwapChainDescriptor =
    {
        Label : string
        Usage : TextureUsage
        Format : TextureFormat
        Width : int
        Height : int
        PresentMode : PresentMode
        Implementation : uint64
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUSwapChainDescriptor -> 'a) : 'a = 
        let x = x
        let _Label = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            let _Usage = x.Usage
            let _Format = x.Format
            let _Width = x.Width
            let _Height = x.Height
            let _PresentMode = x.PresentMode
            let _Implementation = x.Implementation
            let mutable native = Unchecked.defaultof<DawnRaw.WGPUSwapChainDescriptor>
            native.Next <- 0n
            native.Label <- _Label
            native.Usage <- _Usage
            native.Format <- _Format
            native.Width <- _Width
            native.Height <- _Height
            native.PresentMode <- _PresentMode
            native.Implementation <- _Implementation
            callback native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _Label
[<Struct>]
type TextureCopyView =
    {
        Texture : Texture
        MipLevel : int
        Origin : Origin3D
        Aspect : TextureAspect
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUTextureCopyView -> 'a) : 'a = 
        let x = x
        let _Texture = (if isNull x.Texture then TextureHandle.Null else x.Texture.Handle)
        let _MipLevel = x.MipLevel
        x.Origin.Pin (fun _Origin ->
            let _Aspect = x.Aspect
            let mutable native = Unchecked.defaultof<DawnRaw.WGPUTextureCopyView>
            native.Next <- 0n
            native.Texture <- _Texture
            native.MipLevel <- _MipLevel
            native.Origin <- _Origin
            native.Aspect <- _Aspect
            callback native
        )
[<Struct>]
type TextureDataLayout =
    {
        Offset : uint64
        BytesPerRow : int
        RowsPerImage : int
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUTextureDataLayout -> 'a) : 'a = 
        let x = x
        let _Offset = x.Offset
        let _BytesPerRow = x.BytesPerRow
        let _RowsPerImage = x.RowsPerImage
        let mutable native = Unchecked.defaultof<DawnRaw.WGPUTextureDataLayout>
        native.Next <- 0n
        native.Offset <- _Offset
        native.BytesPerRow <- _BytesPerRow
        native.RowsPerImage <- _RowsPerImage
        callback native
[<Struct>]
type TextureDescriptor =
    {
        Label : string
        Usage : TextureUsage
        Dimension : TextureDimension
        Size : Extent3D
        Format : TextureFormat
        MipLevelCount : int
        SampleCount : int
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUTextureDescriptor -> 'a) : 'a = 
        let x = x
        let _Label = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            let _Usage = x.Usage
            let _Dimension = x.Dimension
            x.Size.Pin (fun _Size ->
                let _Format = x.Format
                let _MipLevelCount = x.MipLevelCount
                let _SampleCount = x.SampleCount
                let mutable native = Unchecked.defaultof<DawnRaw.WGPUTextureDescriptor>
                native.Next <- 0n
                native.Label <- _Label
                native.Usage <- _Usage
                native.Dimension <- _Dimension
                native.Size <- _Size
                native.Format <- _Format
                native.MipLevelCount <- _MipLevelCount
                native.SampleCount <- _SampleCount
                callback native
            )
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _Label
[<Struct>]
type TextureViewDescriptor =
    {
        Label : string
        Format : TextureFormat
        Dimension : TextureViewDimension
        BaseMipLevel : int
        MipLevelCount : int
        BaseArrayLayer : int
        ArrayLayerCount : int
        Aspect : TextureAspect
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUTextureViewDescriptor -> 'a) : 'a = 
        let x = x
        let _Label = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.Label
        try
            let _Format = x.Format
            let _Dimension = x.Dimension
            let _BaseMipLevel = x.BaseMipLevel
            let _MipLevelCount = x.MipLevelCount
            let _BaseArrayLayer = x.BaseArrayLayer
            let _ArrayLayerCount = x.ArrayLayerCount
            let _Aspect = x.Aspect
            let mutable native = Unchecked.defaultof<DawnRaw.WGPUTextureViewDescriptor>
            native.Next <- 0n
            native.Label <- _Label
            native.Format <- _Format
            native.Dimension <- _Dimension
            native.BaseMipLevel <- _BaseMipLevel
            native.MipLevelCount <- _MipLevelCount
            native.BaseArrayLayer <- _BaseArrayLayer
            native.ArrayLayerCount <- _ArrayLayerCount
            native.Aspect <- _Aspect
            callback native
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _Label
[<Struct>]
type VertexAttributeDescriptor =
    {
        Format : VertexFormat
        Offset : uint64
        ShaderLocation : int
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUVertexAttributeDescriptor -> 'a) : 'a = 
        let x = x
        let _Format = x.Format
        let _Offset = x.Offset
        let _ShaderLocation = x.ShaderLocation
        let mutable native = Unchecked.defaultof<DawnRaw.WGPUVertexAttributeDescriptor>
        native.Format <- _Format
        native.Offset <- _Offset
        native.ShaderLocation <- _ShaderLocation
        callback native
[<Struct>]
type VertexBufferLayoutDescriptor =
    {
        ArrayStride : uint64
        StepMode : InputStepMode
        Attributes : array<VertexAttributeDescriptor>
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUVertexBufferLayoutDescriptor -> 'a) : 'a = 
        let x = x
        let _ArrayStride = x.ArrayStride
        let _StepMode = x.StepMode
        let _AttributesCount = if isNull x.Attributes then 0 else x.Attributes.Length
        let rec _AttributesCont (inputs : array<VertexAttributeDescriptor>) (outputs : array<DawnRaw.WGPUVertexAttributeDescriptor>) (i : int) =
            if i >= _AttributesCount then
                use _Attributes = fixed outputs
                let mutable native = Unchecked.defaultof<DawnRaw.WGPUVertexBufferLayoutDescriptor>
                native.ArrayStride <- _ArrayStride
                native.StepMode <- _StepMode
                native.AttributesCount <- _AttributesCount
                native.Attributes <- _Attributes
                callback native
            else
                inputs.[i].Pin(fun n -> outputs.[i] <- n; _AttributesCont inputs outputs (i + 1))
        _AttributesCont x.Attributes (if _AttributesCount > 0 then Array.zeroCreate _AttributesCount else null) 0
[<Struct>]
type VertexStateDescriptor =
    {
        IndexFormat : IndexFormat
        VertexBuffers : array<VertexBufferLayoutDescriptor>
    }
    member x.Pin<'a>(callback : DawnRaw.WGPUVertexStateDescriptor -> 'a) : 'a = 
        let x = x
        let _IndexFormat = x.IndexFormat
        let _VertexBuffersCount = if isNull x.VertexBuffers then 0 else x.VertexBuffers.Length
        let rec _VertexBuffersCont (inputs : array<VertexBufferLayoutDescriptor>) (outputs : array<DawnRaw.WGPUVertexBufferLayoutDescriptor>) (i : int) =
            if i >= _VertexBuffersCount then
                use _VertexBuffers = fixed outputs
                let mutable native = Unchecked.defaultof<DawnRaw.WGPUVertexStateDescriptor>
                native.Next <- 0n
                native.IndexFormat <- _IndexFormat
                native.VertexBuffersCount <- _VertexBuffersCount
                native.VertexBuffers <- _VertexBuffers
                callback native
            else
                inputs.[i].Pin(fun n -> outputs.[i] <- n; _VertexBuffersCont inputs outputs (i + 1))
        _VertexBuffersCont x.VertexBuffers (if _VertexBuffersCount > 0 then Array.zeroCreate _VertexBuffersCount else null) 0


[<AllowNullLiteral>]
type BindGroup(device : Device, handle : BindGroupHandle) = 
    member x.Device = device
    member x.Handle = handle
    member x.Reference() : unit = 
        DawnRaw.wgpuBindGroupReference(handle)
    member x.Release() : unit = 
        DawnRaw.wgpuBindGroupRelease(handle)
[<AllowNullLiteral>]
type BindGroupLayout(device : Device, handle : BindGroupLayoutHandle) = 
    member x.Device = device
    member x.Handle = handle
    member x.Reference() : unit = 
        DawnRaw.wgpuBindGroupLayoutReference(handle)
    member x.Release() : unit = 
        DawnRaw.wgpuBindGroupLayoutRelease(handle)
[<AllowNullLiteral>]
type Buffer(device : Device, handle : BufferHandle) = 
    member x.Device = device
    member x.Handle = handle
    member x.Reference() : unit = 
        DawnRaw.wgpuBufferReference(handle)
    member x.Release() : unit = 
        DawnRaw.wgpuBufferRelease(handle)
    member x.MapAsync(Mode : MapMode, Offset : unativeint, Size : unativeint, Callback : BufferMapCallback, Userdata : nativeint) : unit = 
        let _Mode = Mode
        let _Offset = Offset
        let _Size = Size
        let mutable _CallbackGC = Unchecked.defaultof<System.Runtime.InteropServices.GCHandle>
        let _CallbackCB = BufferMapCallback(fun Status Userdata -> Callback.Invoke(Status, Userdata); _CallbackGC.Free())
        let _CallbackGC = System.Runtime.InteropServices.GCHandle.Alloc(_CallbackCB)
        let _Callback = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(_CallbackGC)
        let _Userdata = Userdata
        DawnRaw.wgpuBufferMapAsync(handle, _Mode, _Offset, _Size, _Callback, _Userdata)
    member x.GetMappedRange(Offset : unativeint, Size : unativeint) : nativeint = 
        let _Offset = Offset
        let _Size = Size
        DawnRaw.wgpuBufferGetMappedRange(handle, _Offset, _Size)
    member x.GetConstMappedRange(Offset : unativeint, Size : unativeint) : nativeint = 
        let _Offset = Offset
        let _Size = Size
        DawnRaw.wgpuBufferGetConstMappedRange(handle, _Offset, _Size)
    member x.Unmap() : unit = 
        DawnRaw.wgpuBufferUnmap(handle)
    member x.Destroy() : unit = 
        DawnRaw.wgpuBufferDestroy(handle)
[<AllowNullLiteral>]
type CommandBuffer(device : Device, handle : CommandBufferHandle) = 
    member x.Device = device
    member x.Handle = handle
    member x.Reference() : unit = 
        DawnRaw.wgpuCommandBufferReference(handle)
    member x.Release() : unit = 
        DawnRaw.wgpuCommandBufferRelease(handle)
[<AllowNullLiteral>]
type CommandEncoder(device : Device, handle : CommandEncoderHandle) = 
    member x.Device = device
    member x.Handle = handle
    member x.Reference() : unit = 
        DawnRaw.wgpuCommandEncoderReference(handle)
    member x.Release() : unit = 
        DawnRaw.wgpuCommandEncoderRelease(handle)
    member x.Finish(Descriptor : CommandBufferDescriptor) : CommandBuffer = 
        Descriptor.Pin (fun _DescriptorValue ->
            use _Descriptor = fixed [| _DescriptorValue |]
            CommandBuffer(device, DawnRaw.wgpuCommandEncoderFinish(handle, _Descriptor))
        )
    member x.BeginComputePass(Descriptor : ComputePassDescriptor) : ComputePassEncoder = 
        Descriptor.Pin (fun _DescriptorValue ->
            use _Descriptor = fixed [| _DescriptorValue |]
            ComputePassEncoder(device, DawnRaw.wgpuCommandEncoderBeginComputePass(handle, _Descriptor))
        )
    member x.BeginRenderPass(Descriptor : RenderPassDescriptor) : RenderPassEncoder = 
        Descriptor.Pin (fun _DescriptorValue ->
            use _Descriptor = fixed [| _DescriptorValue |]
            RenderPassEncoder(device, DawnRaw.wgpuCommandEncoderBeginRenderPass(handle, _Descriptor))
        )
    member x.CopyBufferToBuffer(Source : Buffer, SourceOffset : uint64, Destination : Buffer, DestinationOffset : uint64, Size : uint64) : unit = 
        let _Source = (if isNull Source then BufferHandle.Null else Source.Handle)
        let _SourceOffset = SourceOffset
        let _Destination = (if isNull Destination then BufferHandle.Null else Destination.Handle)
        let _DestinationOffset = DestinationOffset
        let _Size = Size
        DawnRaw.wgpuCommandEncoderCopyBufferToBuffer(handle, _Source, _SourceOffset, _Destination, _DestinationOffset, _Size)
    member x.CopyBufferToTexture(Source : option<BufferCopyView>, Destination : option<TextureCopyView>, CopySize : option<Extent3D>) : unit = 
        let inline _SourceCont _Source = 
            let inline _DestinationCont _Destination = 
                let inline _CopySizeCont _CopySize = 
                    DawnRaw.wgpuCommandEncoderCopyBufferToTexture(handle, _Source, _Destination, _CopySize)
                match CopySize with
                | Some v -> v.Pin(fun n -> use ptr = fixed [|n|] in _CopySizeCont ptr)
                | None -> _CopySizeCont (Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt 0n)
            match Destination with
            | Some v -> v.Pin(fun n -> use ptr = fixed [|n|] in _DestinationCont ptr)
            | None -> _DestinationCont (Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt 0n)
        match Source with
        | Some v -> v.Pin(fun n -> use ptr = fixed [|n|] in _SourceCont ptr)
        | None -> _SourceCont (Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt 0n)
    member x.CopyTextureToBuffer(Source : option<TextureCopyView>, Destination : option<BufferCopyView>, CopySize : option<Extent3D>) : unit = 
        let inline _SourceCont _Source = 
            let inline _DestinationCont _Destination = 
                let inline _CopySizeCont _CopySize = 
                    DawnRaw.wgpuCommandEncoderCopyTextureToBuffer(handle, _Source, _Destination, _CopySize)
                match CopySize with
                | Some v -> v.Pin(fun n -> use ptr = fixed [|n|] in _CopySizeCont ptr)
                | None -> _CopySizeCont (Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt 0n)
            match Destination with
            | Some v -> v.Pin(fun n -> use ptr = fixed [|n|] in _DestinationCont ptr)
            | None -> _DestinationCont (Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt 0n)
        match Source with
        | Some v -> v.Pin(fun n -> use ptr = fixed [|n|] in _SourceCont ptr)
        | None -> _SourceCont (Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt 0n)
    member x.CopyTextureToTexture(Source : option<TextureCopyView>, Destination : option<TextureCopyView>, CopySize : option<Extent3D>) : unit = 
        let inline _SourceCont _Source = 
            let inline _DestinationCont _Destination = 
                let inline _CopySizeCont _CopySize = 
                    DawnRaw.wgpuCommandEncoderCopyTextureToTexture(handle, _Source, _Destination, _CopySize)
                match CopySize with
                | Some v -> v.Pin(fun n -> use ptr = fixed [|n|] in _CopySizeCont ptr)
                | None -> _CopySizeCont (Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt 0n)
            match Destination with
            | Some v -> v.Pin(fun n -> use ptr = fixed [|n|] in _DestinationCont ptr)
            | None -> _DestinationCont (Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt 0n)
        match Source with
        | Some v -> v.Pin(fun n -> use ptr = fixed [|n|] in _SourceCont ptr)
        | None -> _SourceCont (Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt 0n)
    member x.InsertDebugMarker(MarkerLabel : string) : unit = 
        let _MarkerLabel = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi MarkerLabel
        try
            DawnRaw.wgpuCommandEncoderInsertDebugMarker(handle, _MarkerLabel)
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _MarkerLabel
    member x.PopDebugGroup() : unit = 
        DawnRaw.wgpuCommandEncoderPopDebugGroup(handle)
    member x.PushDebugGroup(GroupLabel : string) : unit = 
        let _GroupLabel = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi GroupLabel
        try
            DawnRaw.wgpuCommandEncoderPushDebugGroup(handle, _GroupLabel)
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _GroupLabel
    member x.ResolveQuerySet(QuerySet : QuerySet, FirstQuery : int, QueryCount : int, Destination : Buffer, DestinationOffset : uint64) : unit = 
        let _QuerySet = (if isNull QuerySet then QuerySetHandle.Null else QuerySet.Handle)
        let _FirstQuery = FirstQuery
        let _QueryCount = QueryCount
        let _Destination = (if isNull Destination then BufferHandle.Null else Destination.Handle)
        let _DestinationOffset = DestinationOffset
        DawnRaw.wgpuCommandEncoderResolveQuerySet(handle, _QuerySet, _FirstQuery, _QueryCount, _Destination, _DestinationOffset)
    member x.WriteTimestamp(QuerySet : QuerySet, QueryIndex : int) : unit = 
        let _QuerySet = (if isNull QuerySet then QuerySetHandle.Null else QuerySet.Handle)
        let _QueryIndex = QueryIndex
        DawnRaw.wgpuCommandEncoderWriteTimestamp(handle, _QuerySet, _QueryIndex)
[<AllowNullLiteral>]
type ComputePassEncoder(device : Device, handle : ComputePassEncoderHandle) = 
    member x.Device = device
    member x.Handle = handle
    member x.Reference() : unit = 
        DawnRaw.wgpuComputePassEncoderReference(handle)
    member x.Release() : unit = 
        DawnRaw.wgpuComputePassEncoderRelease(handle)
    member x.InsertDebugMarker(MarkerLabel : string) : unit = 
        let _MarkerLabel = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi MarkerLabel
        try
            DawnRaw.wgpuComputePassEncoderInsertDebugMarker(handle, _MarkerLabel)
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _MarkerLabel
    member x.PopDebugGroup() : unit = 
        DawnRaw.wgpuComputePassEncoderPopDebugGroup(handle)
    member x.PushDebugGroup(GroupLabel : string) : unit = 
        let _GroupLabel = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi GroupLabel
        try
            DawnRaw.wgpuComputePassEncoderPushDebugGroup(handle, _GroupLabel)
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _GroupLabel
    member x.SetPipeline(Pipeline : ComputePipeline) : unit = 
        let _Pipeline = (if isNull Pipeline then ComputePipelineHandle.Null else Pipeline.Handle)
        DawnRaw.wgpuComputePassEncoderSetPipeline(handle, _Pipeline)
    member x.SetBindGroup(GroupIndex : int, Group : BindGroup, DynamicOffsets : array<int>) : unit = 
        let _GroupIndex = GroupIndex
        let _Group = (if isNull Group then BindGroupHandle.Null else Group.Handle)
        use _DynamicOffsets = fixed DynamicOffsets
        let _DynamicOffsetsCount = DynamicOffsets.Length
        DawnRaw.wgpuComputePassEncoderSetBindGroup(handle, _GroupIndex, _Group, _DynamicOffsetsCount, _DynamicOffsets)
    member x.WriteTimestamp(QuerySet : QuerySet, QueryIndex : int) : unit = 
        let _QuerySet = (if isNull QuerySet then QuerySetHandle.Null else QuerySet.Handle)
        let _QueryIndex = QueryIndex
        DawnRaw.wgpuComputePassEncoderWriteTimestamp(handle, _QuerySet, _QueryIndex)
    member x.Dispatch(X : int, Y : int, Z : int) : unit = 
        let _X = X
        let _Y = Y
        let _Z = Z
        DawnRaw.wgpuComputePassEncoderDispatch(handle, _X, _Y, _Z)
    member x.DispatchIndirect(IndirectBuffer : Buffer, IndirectOffset : uint64) : unit = 
        let _IndirectBuffer = (if isNull IndirectBuffer then BufferHandle.Null else IndirectBuffer.Handle)
        let _IndirectOffset = IndirectOffset
        DawnRaw.wgpuComputePassEncoderDispatchIndirect(handle, _IndirectBuffer, _IndirectOffset)
    member x.EndPass() : unit = 
        DawnRaw.wgpuComputePassEncoderEndPass(handle)
[<AllowNullLiteral>]
type ComputePipeline(device : Device, handle : ComputePipelineHandle) = 
    member x.Device = device
    member x.Handle = handle
    member x.Reference() : unit = 
        DawnRaw.wgpuComputePipelineReference(handle)
    member x.Release() : unit = 
        DawnRaw.wgpuComputePipelineRelease(handle)
    member x.GetBindGroupLayout(GroupIndex : int) : BindGroupLayout = 
        let _GroupIndex = GroupIndex
        BindGroupLayout(device, DawnRaw.wgpuComputePipelineGetBindGroupLayout(handle, _GroupIndex))
[<AllowNullLiteral>]
type Device(handle : DeviceHandle) = 
    member x.Handle = handle
    member x.Reference() : unit = 
        DawnRaw.wgpuDeviceReference(handle)
    member x.Release() : unit = 
        DawnRaw.wgpuDeviceRelease(handle)
    member x.CreateBindGroup(Descriptor : BindGroupDescriptor) : BindGroup = 
        Descriptor.Pin (fun _DescriptorValue ->
            use _Descriptor = fixed [| _DescriptorValue |]
            BindGroup(x, DawnRaw.wgpuDeviceCreateBindGroup(handle, _Descriptor))
        )
    member x.CreateBindGroupLayout(Descriptor : BindGroupLayoutDescriptor) : BindGroupLayout = 
        Descriptor.Pin (fun _DescriptorValue ->
            use _Descriptor = fixed [| _DescriptorValue |]
            BindGroupLayout(x, DawnRaw.wgpuDeviceCreateBindGroupLayout(handle, _Descriptor))
        )
    member x.CreateBuffer(Descriptor : BufferDescriptor) : Buffer = 
        Descriptor.Pin (fun _DescriptorValue ->
            use _Descriptor = fixed [| _DescriptorValue |]
            Buffer(x, DawnRaw.wgpuDeviceCreateBuffer(handle, _Descriptor))
        )
    member x.CreateErrorBuffer() : Buffer = 
        Buffer(x, DawnRaw.wgpuDeviceCreateErrorBuffer(handle))
    member x.CreateCommandEncoder(Descriptor : CommandEncoderDescriptor) : CommandEncoder = 
        Descriptor.Pin (fun _DescriptorValue ->
            use _Descriptor = fixed [| _DescriptorValue |]
            CommandEncoder(x, DawnRaw.wgpuDeviceCreateCommandEncoder(handle, _Descriptor))
        )
    member x.CreateComputePipeline(Descriptor : ComputePipelineDescriptor) : ComputePipeline = 
        Descriptor.Pin (fun _DescriptorValue ->
            use _Descriptor = fixed [| _DescriptorValue |]
            ComputePipeline(x, DawnRaw.wgpuDeviceCreateComputePipeline(handle, _Descriptor))
        )
    member x.CreatePipelineLayout(Descriptor : PipelineLayoutDescriptor) : PipelineLayout = 
        Descriptor.Pin (fun _DescriptorValue ->
            use _Descriptor = fixed [| _DescriptorValue |]
            PipelineLayout(x, DawnRaw.wgpuDeviceCreatePipelineLayout(handle, _Descriptor))
        )
    member x.CreateQuerySet(Descriptor : QuerySetDescriptor) : QuerySet = 
        Descriptor.Pin (fun _DescriptorValue ->
            use _Descriptor = fixed [| _DescriptorValue |]
            QuerySet(x, DawnRaw.wgpuDeviceCreateQuerySet(handle, _Descriptor))
        )
    member x.CreateRenderBundleEncoder(Descriptor : RenderBundleEncoderDescriptor) : RenderBundleEncoder = 
        Descriptor.Pin (fun _DescriptorValue ->
            use _Descriptor = fixed [| _DescriptorValue |]
            RenderBundleEncoder(x, DawnRaw.wgpuDeviceCreateRenderBundleEncoder(handle, _Descriptor))
        )
    member x.CreateRenderPipeline(Descriptor : RenderPipelineDescriptor) : RenderPipeline = 
        Descriptor.Pin (fun _DescriptorValue ->
            use _Descriptor = fixed [| _DescriptorValue |]
            RenderPipeline(x, DawnRaw.wgpuDeviceCreateRenderPipeline(handle, _Descriptor))
        )
    member x.CreateSampler(Descriptor : SamplerDescriptor) : Sampler = 
        Descriptor.Pin (fun _DescriptorValue ->
            use _Descriptor = fixed [| _DescriptorValue |]
            Sampler(x, DawnRaw.wgpuDeviceCreateSampler(handle, _Descriptor))
        )
    member x.CreateShaderModule(Descriptor : ShaderModuleDescriptor) : ShaderModule = 
        Descriptor.Pin (fun _DescriptorValue ->
            use _Descriptor = fixed [| _DescriptorValue |]
            ShaderModule(x, DawnRaw.wgpuDeviceCreateShaderModule(handle, _Descriptor))
        )
    member x.CreateSwapChain(Surface : Surface, Descriptor : SwapChainDescriptor) : SwapChain = 
        let _Surface = (if isNull Surface then SurfaceHandle.Null else Surface.Handle)
        Descriptor.Pin (fun _DescriptorValue ->
            use _Descriptor = fixed [| _DescriptorValue |]
            SwapChain(x, DawnRaw.wgpuDeviceCreateSwapChain(handle, _Surface, _Descriptor))
        )
    member x.CreateTexture(Descriptor : TextureDescriptor) : Texture = 
        Descriptor.Pin (fun _DescriptorValue ->
            use _Descriptor = fixed [| _DescriptorValue |]
            Texture(x, DawnRaw.wgpuDeviceCreateTexture(handle, _Descriptor))
        )
    member x.GetDefaultQueue() : Queue = 
        Queue(x, DawnRaw.wgpuDeviceGetDefaultQueue(handle))
    member x.InjectError(Type : ErrorType, Message : string) : unit = 
        let _Type = Type
        let _Message = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi Message
        try
            DawnRaw.wgpuDeviceInjectError(handle, _Type, _Message)
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _Message
    member x.LoseForTesting() : unit = 
        DawnRaw.wgpuDeviceLoseForTesting(handle)
    member x.Tick() : unit = 
        DawnRaw.wgpuDeviceTick(handle)
    member x.SetUncapturedErrorCallback(Callback : ErrorCallback, Userdata : nativeint) : unit = 
        let _CallbackGC = System.Runtime.InteropServices.GCHandle.Alloc(Callback)
        let _Callback = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(Callback)
        let _Userdata = Userdata
        DawnRaw.wgpuDeviceSetUncapturedErrorCallback(handle, _Callback, _Userdata)
    member x.SetDeviceLostCallback(Callback : DeviceLostCallback, Userdata : nativeint) : unit = 
        let _CallbackGC = System.Runtime.InteropServices.GCHandle.Alloc(Callback)
        let _Callback = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(Callback)
        let _Userdata = Userdata
        DawnRaw.wgpuDeviceSetDeviceLostCallback(handle, _Callback, _Userdata)
    member x.PushErrorScope(Filter : ErrorFilter) : unit = 
        let _Filter = Filter
        DawnRaw.wgpuDevicePushErrorScope(handle, _Filter)
    member x.PopErrorScope(Callback : ErrorCallback, Userdata : nativeint) : bool = 
        let mutable _CallbackGC = Unchecked.defaultof<System.Runtime.InteropServices.GCHandle>
        let _CallbackCB = ErrorCallback(fun Type Message Userdata -> Callback.Invoke(Type, Message, Userdata); _CallbackGC.Free())
        let _CallbackGC = System.Runtime.InteropServices.GCHandle.Alloc(_CallbackCB)
        let _Callback = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(_CallbackGC)
        let _Userdata = Userdata
        DawnRaw.wgpuDevicePopErrorScope(handle, _Callback, _Userdata) <> 0
[<AllowNullLiteral>]
type Fence(device : Device, handle : FenceHandle) = 
    member x.Device = device
    member x.Handle = handle
    member x.Reference() : unit = 
        DawnRaw.wgpuFenceReference(handle)
    member x.Release() : unit = 
        DawnRaw.wgpuFenceRelease(handle)
    member x.GetCompletedValue() : uint64 = 
        DawnRaw.wgpuFenceGetCompletedValue(handle)
    member x.OnCompletion(Value : uint64, Callback : FenceOnCompletionCallback, Userdata : nativeint) : unit = 
        let _Value = Value
        let mutable _CallbackGC = Unchecked.defaultof<System.Runtime.InteropServices.GCHandle>
        let _CallbackCB = FenceOnCompletionCallback(fun Status Userdata -> Callback.Invoke(Status, Userdata); _CallbackGC.Free())
        let _CallbackGC = System.Runtime.InteropServices.GCHandle.Alloc(_CallbackCB)
        let _Callback = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(_CallbackGC)
        let _Userdata = Userdata
        DawnRaw.wgpuFenceOnCompletion(handle, _Value, _Callback, _Userdata)
[<AllowNullLiteral>]
type Instance(device : Device, handle : InstanceHandle) = 
    member x.Device = device
    member x.Handle = handle
    member x.Reference() : unit = 
        DawnRaw.wgpuInstanceReference(handle)
    member x.Release() : unit = 
        DawnRaw.wgpuInstanceRelease(handle)
    member x.CreateSurface(Descriptor : SurfaceDescriptor) : Surface = 
        Descriptor.Pin (fun _DescriptorValue ->
            use _Descriptor = fixed [| _DescriptorValue |]
            Surface(device, DawnRaw.wgpuInstanceCreateSurface(handle, _Descriptor))
        )
[<AllowNullLiteral>]
type PipelineLayout(device : Device, handle : PipelineLayoutHandle) = 
    member x.Device = device
    member x.Handle = handle
    member x.Reference() : unit = 
        DawnRaw.wgpuPipelineLayoutReference(handle)
    member x.Release() : unit = 
        DawnRaw.wgpuPipelineLayoutRelease(handle)
[<AllowNullLiteral>]
type QuerySet(device : Device, handle : QuerySetHandle) = 
    member x.Device = device
    member x.Handle = handle
    member x.Reference() : unit = 
        DawnRaw.wgpuQuerySetReference(handle)
    member x.Release() : unit = 
        DawnRaw.wgpuQuerySetRelease(handle)
    member x.Destroy() : unit = 
        DawnRaw.wgpuQuerySetDestroy(handle)
[<AllowNullLiteral>]
type Queue(device : Device, handle : QueueHandle) = 
    member x.Device = device
    member x.Handle = handle
    member x.Reference() : unit = 
        DawnRaw.wgpuQueueReference(handle)
    member x.Release() : unit = 
        DawnRaw.wgpuQueueRelease(handle)
    member x.Submit(Commands : array<CommandBuffer>) : unit = 
        use _Commands = fixed (Commands |> Array.map (fun v -> if isNull v then CommandBufferHandle.Null else v.Handle))
        let _CommandsCount = Commands.Length
        DawnRaw.wgpuQueueSubmit(handle, _CommandsCount, _Commands)
    member x.Signal(Fence : Fence, SignalValue : uint64) : unit = 
        let _Fence = (if isNull Fence then FenceHandle.Null else Fence.Handle)
        let _SignalValue = SignalValue
        DawnRaw.wgpuQueueSignal(handle, _Fence, _SignalValue)
    member x.CreateFence(Descriptor : FenceDescriptor) : Fence = 
        Descriptor.Pin (fun _DescriptorValue ->
            use _Descriptor = fixed [| _DescriptorValue |]
            Fence(device, DawnRaw.wgpuQueueCreateFence(handle, _Descriptor))
        )
    member x.WriteBuffer(Buffer : Buffer, BufferOffset : uint64, Data : nativeint, Size : unativeint) : unit = 
        let _Buffer = (if isNull Buffer then BufferHandle.Null else Buffer.Handle)
        let _BufferOffset = BufferOffset
        let _Data = Data
        let _Size = Size
        DawnRaw.wgpuQueueWriteBuffer(handle, _Buffer, _BufferOffset, _Data, _Size)
    member x.WriteTexture(Destination : option<TextureCopyView>, Data : nativeint, DataSize : unativeint, DataLayout : option<TextureDataLayout>, WriteSize : option<Extent3D>) : unit = 
        let inline _DestinationCont _Destination = 
            let _Data = Data
            let _DataSize = DataSize
            let inline _DataLayoutCont _DataLayout = 
                let inline _WriteSizeCont _WriteSize = 
                    DawnRaw.wgpuQueueWriteTexture(handle, _Destination, _Data, _DataSize, _DataLayout, _WriteSize)
                match WriteSize with
                | Some v -> v.Pin(fun n -> use ptr = fixed [|n|] in _WriteSizeCont ptr)
                | None -> _WriteSizeCont (Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt 0n)
            match DataLayout with
            | Some v -> v.Pin(fun n -> use ptr = fixed [|n|] in _DataLayoutCont ptr)
            | None -> _DataLayoutCont (Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt 0n)
        match Destination with
        | Some v -> v.Pin(fun n -> use ptr = fixed [|n|] in _DestinationCont ptr)
        | None -> _DestinationCont (Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt 0n)
[<AllowNullLiteral>]
type RenderBundle(device : Device, handle : RenderBundleHandle) = 
    member x.Device = device
    member x.Handle = handle
    member x.Reference() : unit = 
        DawnRaw.wgpuRenderBundleReference(handle)
    member x.Release() : unit = 
        DawnRaw.wgpuRenderBundleRelease(handle)
[<AllowNullLiteral>]
type RenderBundleEncoder(device : Device, handle : RenderBundleEncoderHandle) = 
    member x.Device = device
    member x.Handle = handle
    member x.Reference() : unit = 
        DawnRaw.wgpuRenderBundleEncoderReference(handle)
    member x.Release() : unit = 
        DawnRaw.wgpuRenderBundleEncoderRelease(handle)
    member x.SetPipeline(Pipeline : RenderPipeline) : unit = 
        let _Pipeline = (if isNull Pipeline then RenderPipelineHandle.Null else Pipeline.Handle)
        DawnRaw.wgpuRenderBundleEncoderSetPipeline(handle, _Pipeline)
    member x.SetBindGroup(GroupIndex : int, Group : BindGroup, DynamicOffsets : array<int>) : unit = 
        let _GroupIndex = GroupIndex
        let _Group = (if isNull Group then BindGroupHandle.Null else Group.Handle)
        use _DynamicOffsets = fixed DynamicOffsets
        let _DynamicOffsetsCount = DynamicOffsets.Length
        DawnRaw.wgpuRenderBundleEncoderSetBindGroup(handle, _GroupIndex, _Group, _DynamicOffsetsCount, _DynamicOffsets)
    member x.Draw(VertexCount : int, InstanceCount : int, FirstVertex : int, FirstInstance : int) : unit = 
        let _VertexCount = VertexCount
        let _InstanceCount = InstanceCount
        let _FirstVertex = FirstVertex
        let _FirstInstance = FirstInstance
        DawnRaw.wgpuRenderBundleEncoderDraw(handle, _VertexCount, _InstanceCount, _FirstVertex, _FirstInstance)
    member x.DrawIndexed(IndexCount : int, InstanceCount : int, FirstIndex : int, BaseVertex : int32, FirstInstance : int) : unit = 
        let _IndexCount = IndexCount
        let _InstanceCount = InstanceCount
        let _FirstIndex = FirstIndex
        let _BaseVertex = BaseVertex
        let _FirstInstance = FirstInstance
        DawnRaw.wgpuRenderBundleEncoderDrawIndexed(handle, _IndexCount, _InstanceCount, _FirstIndex, _BaseVertex, _FirstInstance)
    member x.DrawIndirect(IndirectBuffer : Buffer, IndirectOffset : uint64) : unit = 
        let _IndirectBuffer = (if isNull IndirectBuffer then BufferHandle.Null else IndirectBuffer.Handle)
        let _IndirectOffset = IndirectOffset
        DawnRaw.wgpuRenderBundleEncoderDrawIndirect(handle, _IndirectBuffer, _IndirectOffset)
    member x.DrawIndexedIndirect(IndirectBuffer : Buffer, IndirectOffset : uint64) : unit = 
        let _IndirectBuffer = (if isNull IndirectBuffer then BufferHandle.Null else IndirectBuffer.Handle)
        let _IndirectOffset = IndirectOffset
        DawnRaw.wgpuRenderBundleEncoderDrawIndexedIndirect(handle, _IndirectBuffer, _IndirectOffset)
    member x.InsertDebugMarker(MarkerLabel : string) : unit = 
        let _MarkerLabel = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi MarkerLabel
        try
            DawnRaw.wgpuRenderBundleEncoderInsertDebugMarker(handle, _MarkerLabel)
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _MarkerLabel
    member x.PopDebugGroup() : unit = 
        DawnRaw.wgpuRenderBundleEncoderPopDebugGroup(handle)
    member x.PushDebugGroup(GroupLabel : string) : unit = 
        let _GroupLabel = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi GroupLabel
        try
            DawnRaw.wgpuRenderBundleEncoderPushDebugGroup(handle, _GroupLabel)
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _GroupLabel
    member x.SetVertexBuffer(Slot : int, Buffer : Buffer, Offset : uint64, Size : uint64) : unit = 
        let _Slot = Slot
        let _Buffer = (if isNull Buffer then BufferHandle.Null else Buffer.Handle)
        let _Offset = Offset
        let _Size = Size
        DawnRaw.wgpuRenderBundleEncoderSetVertexBuffer(handle, _Slot, _Buffer, _Offset, _Size)
    member x.SetIndexBuffer(Buffer : Buffer, Offset : uint64, Size : uint64) : unit = 
        let _Buffer = (if isNull Buffer then BufferHandle.Null else Buffer.Handle)
        let _Offset = Offset
        let _Size = Size
        DawnRaw.wgpuRenderBundleEncoderSetIndexBuffer(handle, _Buffer, _Offset, _Size)
    member x.SetIndexBufferWithFormat(Buffer : Buffer, Format : IndexFormat, Offset : uint64, Size : uint64) : unit = 
        let _Buffer = (if isNull Buffer then BufferHandle.Null else Buffer.Handle)
        let _Format = Format
        let _Offset = Offset
        let _Size = Size
        DawnRaw.wgpuRenderBundleEncoderSetIndexBufferWithFormat(handle, _Buffer, _Format, _Offset, _Size)
    member x.Finish(Descriptor : RenderBundleDescriptor) : RenderBundle = 
        Descriptor.Pin (fun _DescriptorValue ->
            use _Descriptor = fixed [| _DescriptorValue |]
            RenderBundle(device, DawnRaw.wgpuRenderBundleEncoderFinish(handle, _Descriptor))
        )
[<AllowNullLiteral>]
type RenderPassEncoder(device : Device, handle : RenderPassEncoderHandle) = 
    member x.Device = device
    member x.Handle = handle
    member x.Reference() : unit = 
        DawnRaw.wgpuRenderPassEncoderReference(handle)
    member x.Release() : unit = 
        DawnRaw.wgpuRenderPassEncoderRelease(handle)
    member x.SetPipeline(Pipeline : RenderPipeline) : unit = 
        let _Pipeline = (if isNull Pipeline then RenderPipelineHandle.Null else Pipeline.Handle)
        DawnRaw.wgpuRenderPassEncoderSetPipeline(handle, _Pipeline)
    member x.SetBindGroup(GroupIndex : int, Group : BindGroup, DynamicOffsets : array<int>) : unit = 
        let _GroupIndex = GroupIndex
        let _Group = (if isNull Group then BindGroupHandle.Null else Group.Handle)
        use _DynamicOffsets = fixed DynamicOffsets
        let _DynamicOffsetsCount = DynamicOffsets.Length
        DawnRaw.wgpuRenderPassEncoderSetBindGroup(handle, _GroupIndex, _Group, _DynamicOffsetsCount, _DynamicOffsets)
    member x.Draw(VertexCount : int, InstanceCount : int, FirstVertex : int, FirstInstance : int) : unit = 
        let _VertexCount = VertexCount
        let _InstanceCount = InstanceCount
        let _FirstVertex = FirstVertex
        let _FirstInstance = FirstInstance
        DawnRaw.wgpuRenderPassEncoderDraw(handle, _VertexCount, _InstanceCount, _FirstVertex, _FirstInstance)
    member x.DrawIndexed(IndexCount : int, InstanceCount : int, FirstIndex : int, BaseVertex : int32, FirstInstance : int) : unit = 
        let _IndexCount = IndexCount
        let _InstanceCount = InstanceCount
        let _FirstIndex = FirstIndex
        let _BaseVertex = BaseVertex
        let _FirstInstance = FirstInstance
        DawnRaw.wgpuRenderPassEncoderDrawIndexed(handle, _IndexCount, _InstanceCount, _FirstIndex, _BaseVertex, _FirstInstance)
    member x.DrawIndirect(IndirectBuffer : Buffer, IndirectOffset : uint64) : unit = 
        let _IndirectBuffer = (if isNull IndirectBuffer then BufferHandle.Null else IndirectBuffer.Handle)
        let _IndirectOffset = IndirectOffset
        DawnRaw.wgpuRenderPassEncoderDrawIndirect(handle, _IndirectBuffer, _IndirectOffset)
    member x.DrawIndexedIndirect(IndirectBuffer : Buffer, IndirectOffset : uint64) : unit = 
        let _IndirectBuffer = (if isNull IndirectBuffer then BufferHandle.Null else IndirectBuffer.Handle)
        let _IndirectOffset = IndirectOffset
        DawnRaw.wgpuRenderPassEncoderDrawIndexedIndirect(handle, _IndirectBuffer, _IndirectOffset)
    member x.ExecuteBundles(Bundles : array<RenderBundle>) : unit = 
        use _Bundles = fixed (Bundles |> Array.map (fun v -> if isNull v then RenderBundleHandle.Null else v.Handle))
        let _BundlesCount = Bundles.Length
        DawnRaw.wgpuRenderPassEncoderExecuteBundles(handle, _BundlesCount, _Bundles)
    member x.InsertDebugMarker(MarkerLabel : string) : unit = 
        let _MarkerLabel = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi MarkerLabel
        try
            DawnRaw.wgpuRenderPassEncoderInsertDebugMarker(handle, _MarkerLabel)
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _MarkerLabel
    member x.PopDebugGroup() : unit = 
        DawnRaw.wgpuRenderPassEncoderPopDebugGroup(handle)
    member x.PushDebugGroup(GroupLabel : string) : unit = 
        let _GroupLabel = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi GroupLabel
        try
            DawnRaw.wgpuRenderPassEncoderPushDebugGroup(handle, _GroupLabel)
        finally
            System.Runtime.InteropServices.Marshal.FreeHGlobal _GroupLabel
    member x.SetStencilReference(Reference : int) : unit = 
        let _Reference = Reference
        DawnRaw.wgpuRenderPassEncoderSetStencilReference(handle, _Reference)
    member x.SetBlendColor(Color : option<Color>) : unit = 
        let inline _ColorCont _Color = 
            DawnRaw.wgpuRenderPassEncoderSetBlendColor(handle, _Color)
        match Color with
        | Some v -> v.Pin(fun n -> use ptr = fixed [|n|] in _ColorCont ptr)
        | None -> _ColorCont (Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt 0n)
    member x.SetViewport(X : float32, Y : float32, Width : float32, Height : float32, MinDepth : float32, MaxDepth : float32) : unit = 
        let _X = X
        let _Y = Y
        let _Width = Width
        let _Height = Height
        let _MinDepth = MinDepth
        let _MaxDepth = MaxDepth
        DawnRaw.wgpuRenderPassEncoderSetViewport(handle, _X, _Y, _Width, _Height, _MinDepth, _MaxDepth)
    member x.SetScissorRect(X : int, Y : int, Width : int, Height : int) : unit = 
        let _X = X
        let _Y = Y
        let _Width = Width
        let _Height = Height
        DawnRaw.wgpuRenderPassEncoderSetScissorRect(handle, _X, _Y, _Width, _Height)
    member x.SetVertexBuffer(Slot : int, Buffer : Buffer, Offset : uint64, Size : uint64) : unit = 
        let _Slot = Slot
        let _Buffer = (if isNull Buffer then BufferHandle.Null else Buffer.Handle)
        let _Offset = Offset
        let _Size = Size
        DawnRaw.wgpuRenderPassEncoderSetVertexBuffer(handle, _Slot, _Buffer, _Offset, _Size)
    member x.SetIndexBuffer(Buffer : Buffer, Offset : uint64, Size : uint64) : unit = 
        let _Buffer = (if isNull Buffer then BufferHandle.Null else Buffer.Handle)
        let _Offset = Offset
        let _Size = Size
        DawnRaw.wgpuRenderPassEncoderSetIndexBuffer(handle, _Buffer, _Offset, _Size)
    member x.SetIndexBufferWithFormat(Buffer : Buffer, Format : IndexFormat, Offset : uint64, Size : uint64) : unit = 
        let _Buffer = (if isNull Buffer then BufferHandle.Null else Buffer.Handle)
        let _Format = Format
        let _Offset = Offset
        let _Size = Size
        DawnRaw.wgpuRenderPassEncoderSetIndexBufferWithFormat(handle, _Buffer, _Format, _Offset, _Size)
    member x.WriteTimestamp(QuerySet : QuerySet, QueryIndex : int) : unit = 
        let _QuerySet = (if isNull QuerySet then QuerySetHandle.Null else QuerySet.Handle)
        let _QueryIndex = QueryIndex
        DawnRaw.wgpuRenderPassEncoderWriteTimestamp(handle, _QuerySet, _QueryIndex)
    member x.EndPass() : unit = 
        DawnRaw.wgpuRenderPassEncoderEndPass(handle)
[<AllowNullLiteral>]
type RenderPipeline(device : Device, handle : RenderPipelineHandle) = 
    member x.Device = device
    member x.Handle = handle
    member x.Reference() : unit = 
        DawnRaw.wgpuRenderPipelineReference(handle)
    member x.Release() : unit = 
        DawnRaw.wgpuRenderPipelineRelease(handle)
    member x.GetBindGroupLayout(GroupIndex : int) : BindGroupLayout = 
        let _GroupIndex = GroupIndex
        BindGroupLayout(device, DawnRaw.wgpuRenderPipelineGetBindGroupLayout(handle, _GroupIndex))
[<AllowNullLiteral>]
type Sampler(device : Device, handle : SamplerHandle) = 
    member x.Device = device
    member x.Handle = handle
    member x.Reference() : unit = 
        DawnRaw.wgpuSamplerReference(handle)
    member x.Release() : unit = 
        DawnRaw.wgpuSamplerRelease(handle)
[<AllowNullLiteral>]
type ShaderModule(device : Device, handle : ShaderModuleHandle) = 
    member x.Device = device
    member x.Handle = handle
    member x.Reference() : unit = 
        DawnRaw.wgpuShaderModuleReference(handle)
    member x.Release() : unit = 
        DawnRaw.wgpuShaderModuleRelease(handle)
[<AllowNullLiteral>]
type Surface(device : Device, handle : SurfaceHandle) = 
    member x.Device = device
    member x.Handle = handle
    member x.Reference() : unit = 
        DawnRaw.wgpuSurfaceReference(handle)
    member x.Release() : unit = 
        DawnRaw.wgpuSurfaceRelease(handle)
[<AllowNullLiteral>]
type SwapChain(device : Device, handle : SwapChainHandle) = 
    member x.Device = device
    member x.Handle = handle
    member x.Reference() : unit = 
        DawnRaw.wgpuSwapChainReference(handle)
    member x.Release() : unit = 
        DawnRaw.wgpuSwapChainRelease(handle)
    member x.Configure(Format : TextureFormat, AllowedUsage : TextureUsage, Width : int, Height : int) : unit = 
        let _Format = Format
        let _AllowedUsage = AllowedUsage
        let _Width = Width
        let _Height = Height
        DawnRaw.wgpuSwapChainConfigure(handle, _Format, _AllowedUsage, _Width, _Height)
    member x.GetCurrentTextureView() : TextureView = 
        TextureView(device, DawnRaw.wgpuSwapChainGetCurrentTextureView(handle))
    member x.Present() : unit = 
        DawnRaw.wgpuSwapChainPresent(handle)
[<AllowNullLiteral>]
type Texture(device : Device, handle : TextureHandle) = 
    member x.Device = device
    member x.Handle = handle
    member x.Reference() : unit = 
        DawnRaw.wgpuTextureReference(handle)
    member x.Release() : unit = 
        DawnRaw.wgpuTextureRelease(handle)
    member x.CreateView(Descriptor : TextureViewDescriptor) : TextureView = 
        Descriptor.Pin (fun _DescriptorValue ->
            use _Descriptor = fixed [| _DescriptorValue |]
            TextureView(device, DawnRaw.wgpuTextureCreateView(handle, _Descriptor))
        )
    member x.Destroy() : unit = 
        DawnRaw.wgpuTextureDestroy(handle)
[<AllowNullLiteral>]
type TextureView(device : Device, handle : TextureViewHandle) = 
    member x.Device = device
    member x.Handle = handle
    member x.Reference() : unit = 
        DawnRaw.wgpuTextureViewReference(handle)
    member x.Release() : unit = 
        DawnRaw.wgpuTextureViewRelease(handle)
