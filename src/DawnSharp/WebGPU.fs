namespace rec WebGPU
open System
open System.Threading
open System.Security
open System.Runtime.InteropServices
open Microsoft.FSharp.NativeInterop
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
type CommandBufferHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = CommandBufferHandle(0n)
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
type RenderBundleHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = RenderBundleHandle(0n)
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
type TextureViewHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = TextureViewHandle(0n)
    end
[<StructLayout(LayoutKind.Sequential)>]
type ComputePipelineHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = ComputePipelineHandle(0n)
    end
[<StructLayout(LayoutKind.Sequential)>]
type InstanceHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = InstanceHandle(0n)
    end
[<StructLayout(LayoutKind.Sequential)>]
type RenderPipelineHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = RenderPipelineHandle(0n)
    end
[<StructLayout(LayoutKind.Sequential)>]
type SwapChainHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = SwapChainHandle(0n)
    end
[<StructLayout(LayoutKind.Sequential)>]
type BufferHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = BufferHandle(0n)
    end
[<StructLayout(LayoutKind.Sequential)>]
type FenceHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = FenceHandle(0n)
    end
[<StructLayout(LayoutKind.Sequential)>]
type TextureHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = TextureHandle(0n)
    end
[<StructLayout(LayoutKind.Sequential)>]
type ComputePassEncoderHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = ComputePassEncoderHandle(0n)
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
type CommandEncoderHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = CommandEncoderHandle(0n)
    end
[<StructLayout(LayoutKind.Sequential)>]
type QueueHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = QueueHandle(0n)
    end
[<StructLayout(LayoutKind.Sequential)>]
type DeviceHandle = 
    struct
        val mutable public Handle : nativeint
        new(handle : nativeint) = { Handle = handle }
        static member Null = DeviceHandle(0n)
    end
type WGPUDeviceLostCallback = delegate of nativeint * nativeint -> unit
type WGPUBufferMapCallback = delegate of BufferMapAsyncStatus * nativeint -> unit
type WGPUErrorCallback = delegate of ErrorType * nativeint * nativeint -> unit
type WGPUFenceOnCompletionCallback = delegate of FenceCompletionStatus * nativeint -> unit
type DeviceLostCallback = delegate of string * nativeint -> unit
type BufferMapCallback = delegate of BufferMapAsyncStatus * nativeint -> unit
type ErrorCallback = delegate of ErrorType * string * nativeint -> unit
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
| BC6HRGBFloat = 51
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
    type WGPUColor =
        struct
            val mutable public R : float32
            val mutable public G : float32
            val mutable public B : float32
            val mutable public A : float32
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
    type WGPURenderBundleDescriptor =
        struct
            val mutable public Next : nativeint
            val mutable public Label : nativeint
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
    type WGPUTextureDataLayout =
        struct
            val mutable public Next : nativeint
            val mutable public Offset : uint64
            val mutable public BytesPerRow : int
            val mutable public RowsPerImage : int
        end
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
    type WGPUBufferDescriptor =
        struct
            val mutable public Next : nativeint
            val mutable public Label : nativeint
            val mutable public Usage : BufferUsage
            val mutable public Size : uint64
            val mutable public MappedAtCreation : int
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
    type WGPUStencilStateFaceDescriptor =
        struct
            val mutable public Compare : CompareFunction
            val mutable public FailOp : StencilOperation
            val mutable public DepthFailOp : StencilOperation
            val mutable public PassOp : StencilOperation
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
    type WGPUBindGroupLayoutDescriptor =
        struct
            val mutable public Next : nativeint
            val mutable public Label : nativeint
            val mutable public EntriesCount : int
            val mutable public Entries : nativeptr<DawnRaw.WGPUBindGroupLayoutEntry>
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
    type WGPURenderPipelineDescriptorDummyExtension =
        struct
            val mutable public DummyStage : DawnRaw.WGPUProgrammableStageDescriptor
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
    type WGPUBufferCopyView =
        struct
            val mutable public Next : nativeint
            val mutable public Layout : DawnRaw.WGPUTextureDataLayout
            val mutable public Buffer : BufferHandle
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
    type WGPUVertexStateDescriptor =
        struct
            val mutable public Next : nativeint
            val mutable public IndexFormat : IndexFormat
            val mutable public VertexBuffersCount : int
            val mutable public VertexBuffers : nativeptr<DawnRaw.WGPUVertexBufferLayoutDescriptor>
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
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuBindGroupReference(BindGroupHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuBindGroupRelease(BindGroupHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuBindGroupLayoutReference(BindGroupLayoutHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuBindGroupLayoutRelease(BindGroupLayoutHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuCommandBufferReference(CommandBufferHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuCommandBufferRelease(CommandBufferHandle self)
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
    extern void wgpuRenderBundleReference(RenderBundleHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderBundleRelease(RenderBundleHandle self)
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
    extern void wgpuTextureViewReference(TextureViewHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuTextureViewRelease(TextureViewHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuComputePipelineReference(ComputePipelineHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuComputePipelineRelease(ComputePipelineHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern BindGroupLayoutHandle wgpuComputePipelineGetBindGroupLayout(ComputePipelineHandle self, int GroupIndex)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuInstanceReference(InstanceHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuInstanceRelease(InstanceHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern SurfaceHandle wgpuInstanceCreateSurface(InstanceHandle self, WGPUSurfaceDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPipelineReference(RenderPipelineHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuRenderPipelineRelease(RenderPipelineHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern BindGroupLayoutHandle wgpuRenderPipelineGetBindGroupLayout(RenderPipelineHandle self, int GroupIndex)
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
    extern void wgpuFenceReference(FenceHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuFenceRelease(FenceHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern uint64 wgpuFenceGetCompletedValue(FenceHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuFenceOnCompletion(FenceHandle self, uint64 Value, nativeint Callback, nativeint Userdata)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuTextureReference(TextureHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuTextureRelease(TextureHandle self)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern TextureViewHandle wgpuTextureCreateView(TextureHandle self, WGPUTextureViewDescriptor* Descriptor)
    [<DllImport("dawn"); SuppressUnmanagedCodeSecurity>]
    extern void wgpuTextureDestroy(TextureHandle self)
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


[<Struct>]
type Color =
    {
        R : float32
        G : float32
        B : float32
        A : float32
    }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUColor -> 'a) : 'a = 
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
type CommandBufferDescriptor =
    {
        Label : string
    }
    static member Default : CommandBufferDescriptor =
        {
            Label = null
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUCommandBufferDescriptor -> 'a) : 'a = 
        let x = x
        let inline _LabelCont (_Label) = 
            let mutable native = Unchecked.defaultof<DawnRaw.WGPUCommandBufferDescriptor>
            native.Next <- 0n
            native.Label <- _Label
            callback native
        if not (isNull x.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount x.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(x.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
[<Struct>]
type CommandEncoderDescriptor =
    {
        Label : string
    }
    static member Default : CommandEncoderDescriptor =
        {
            Label = null
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUCommandEncoderDescriptor -> 'a) : 'a = 
        let x = x
        let inline _LabelCont (_Label) = 
            let mutable native = Unchecked.defaultof<DawnRaw.WGPUCommandEncoderDescriptor>
            native.Next <- 0n
            native.Label <- _Label
            callback native
        if not (isNull x.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount x.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(x.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
[<Struct>]
type ComputePassDescriptor =
    {
        Label : string
    }
    static member Default : ComputePassDescriptor =
        {
            Label = null
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUComputePassDescriptor -> 'a) : 'a = 
        let x = x
        let inline _LabelCont (_Label) = 
            let mutable native = Unchecked.defaultof<DawnRaw.WGPUComputePassDescriptor>
            native.Next <- 0n
            native.Label <- _Label
            callback native
        if not (isNull x.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount x.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(x.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
[<Struct>]
type DeviceProperties =
    {
        TextureCompressionBC : bool
        ShaderFloat16 : bool
        PipelineStatisticsQuery : bool
        TimestampQuery : bool
    }
    static member Default : DeviceProperties =
        {
            TextureCompressionBC = false
            ShaderFloat16 = false
            PipelineStatisticsQuery = false
            TimestampQuery = false
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUDeviceProperties -> 'a) : 'a = 
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

    member inline x.Pin<'a>(callback : DawnRaw.WGPUExtent3D -> 'a) : 'a = 
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
    static member Default : FenceDescriptor =
        {
            Label = null
            InitialValue = 0UL
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUFenceDescriptor -> 'a) : 'a = 
        let x = x
        let inline _LabelCont (_Label) = 
            let _InitialValue = x.InitialValue
            let mutable native = Unchecked.defaultof<DawnRaw.WGPUFenceDescriptor>
            native.Next <- 0n
            native.Label <- _Label
            native.InitialValue <- _InitialValue
            callback native
        if not (isNull x.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount x.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(x.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
[<Struct>]
type InstanceDescriptor = 
    member inline x.Pin<'a>(callback : DawnRaw.WGPUInstanceDescriptor -> 'a) : 'a = 
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
    static member Default : Origin3D =
        {
            X = 0
            Y = 0
            Z = 0
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUOrigin3D -> 'a) : 'a = 
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
type RenderBundleDescriptor =
    {
        Label : string
    }
    static member Default : RenderBundleDescriptor =
        {
            Label = null
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPURenderBundleDescriptor -> 'a) : 'a = 
        let x = x
        let inline _LabelCont (_Label) = 
            let mutable native = Unchecked.defaultof<DawnRaw.WGPURenderBundleDescriptor>
            native.Next <- 0n
            native.Label <- _Label
            callback native
        if not (isNull x.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount x.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(x.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
[<Struct>]
type SamplerDescriptorDummyAnisotropicFiltering =
    {
        MaxAnisotropy : float32
    }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUSamplerDescriptorDummyAnisotropicFiltering -> 'a) : 'a = 
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

    member inline x.Pin<'a>(callback : DawnRaw.WGPUShaderModuleSPIRVDescriptor -> 'a) : 'a = 
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

    member inline x.Pin<'a>(callback : DawnRaw.WGPUShaderModuleWGSLDescriptor -> 'a) : 'a = 
        let x = x
        let inline _SourceCont (_Source) = 
            let mutable native = Unchecked.defaultof<DawnRaw.WGPUShaderModuleWGSLDescriptor>
            native.Source <- _Source
            callback native
        if not (isNull x.Source) then
            let _SourceLen = System.Text.Encoding.UTF8.GetByteCount x.Source
            let _SourceSize = _SourceLen + 1
            let _SourcePtr = NativePtr.stackalloc<byte> _SourceSize
            System.Text.Encoding.UTF8.GetBytes(x.Source.AsSpan(), Span<byte>(NativePtr.toVoidPtr _SourcePtr, _SourceSize)) |> ignore
            NativePtr.set _SourcePtr _SourceLen 0uy
            _SourceCont (NativePtr.toNativeInt _SourcePtr)
        else
            _SourceCont 0n
[<Struct>]
type ShaderModuleDescriptor =
    {
        Label : string
    }
    static member Default : ShaderModuleDescriptor =
        {
            Label = null
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUShaderModuleDescriptor -> 'a) : 'a = 
        let x = x
        let inline _LabelCont (_Label) = 
            let mutable native = Unchecked.defaultof<DawnRaw.WGPUShaderModuleDescriptor>
            native.Next <- 0n
            native.Label <- _Label
            callback native
        if not (isNull x.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount x.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(x.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
[<Struct>]
type SurfaceDescriptor =
    {
        Label : string
    }
    static member Default : SurfaceDescriptor =
        {
            Label = null
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUSurfaceDescriptor -> 'a) : 'a = 
        let x = x
        let inline _LabelCont (_Label) = 
            let mutable native = Unchecked.defaultof<DawnRaw.WGPUSurfaceDescriptor>
            native.Next <- 0n
            native.Label <- _Label
            callback native
        if not (isNull x.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount x.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(x.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
[<Struct>]
type SurfaceDescriptorFromCanvasHTMLSelector =
    {
        Selector : string
    }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUSurfaceDescriptorFromCanvasHTMLSelector -> 'a) : 'a = 
        let x = x
        let inline _SelectorCont (_Selector) = 
            let mutable native = Unchecked.defaultof<DawnRaw.WGPUSurfaceDescriptorFromCanvasHTMLSelector>
            native.Selector <- _Selector
            callback native
        if not (isNull x.Selector) then
            let _SelectorLen = System.Text.Encoding.UTF8.GetByteCount x.Selector
            let _SelectorSize = _SelectorLen + 1
            let _SelectorPtr = NativePtr.stackalloc<byte> _SelectorSize
            System.Text.Encoding.UTF8.GetBytes(x.Selector.AsSpan(), Span<byte>(NativePtr.toVoidPtr _SelectorPtr, _SelectorSize)) |> ignore
            NativePtr.set _SelectorPtr _SelectorLen 0uy
            _SelectorCont (NativePtr.toNativeInt _SelectorPtr)
        else
            _SelectorCont 0n
[<Struct>]
type SurfaceDescriptorFromMetalLayer =
    {
        Layer : nativeint
    }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUSurfaceDescriptorFromMetalLayer -> 'a) : 'a = 
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

    member inline x.Pin<'a>(callback : DawnRaw.WGPUSurfaceDescriptorFromWindowsHWND -> 'a) : 'a = 
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

    member inline x.Pin<'a>(callback : DawnRaw.WGPUSurfaceDescriptorFromXlib -> 'a) : 'a = 
        let x = x
        let _Display = x.Display
        let _Window = x.Window
        let mutable native = Unchecked.defaultof<DawnRaw.WGPUSurfaceDescriptorFromXlib>
        native.Display <- _Display
        native.Window <- _Window
        callback native
[<Struct>]
type TextureDataLayout =
    {
        Offset : uint64
        BytesPerRow : int
        RowsPerImage : int
    }
    static member Default(BytesPerRow: int) : TextureDataLayout =
        {
            Offset = 0UL
            BytesPerRow = BytesPerRow
            RowsPerImage = 0
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUTextureDataLayout -> 'a) : 'a = 
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
type AdapterProperties =
    {
        DeviceID : int
        VendorID : int
        Name : string
        AdapterType : AdapterType
        BackendType : BackendType
    }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUAdapterProperties -> 'a) : 'a = 
        let x = x
        let _DeviceID = x.DeviceID
        let _VendorID = x.VendorID
        let inline _NameCont (_Name) = 
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
        if not (isNull x.Name) then
            let _NameLen = System.Text.Encoding.UTF8.GetByteCount x.Name
            let _NameSize = _NameLen + 1
            let _NamePtr = NativePtr.stackalloc<byte> _NameSize
            System.Text.Encoding.UTF8.GetBytes(x.Name.AsSpan(), Span<byte>(NativePtr.toVoidPtr _NamePtr, _NameSize)) |> ignore
            NativePtr.set _NamePtr _NameLen 0uy
            _NameCont (NativePtr.toNativeInt _NamePtr)
        else
            _NameCont 0n
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
    static member Default(Binding: int, Visibility: ShaderStage, Type: BindingType) : BindGroupLayoutEntry =
        {
            Binding = Binding
            Visibility = Visibility
            Type = Type
            HasDynamicOffset = false
            MinBufferBindingSize = 0UL
            Multisampled = false
            ViewDimension = TextureViewDimension.Undefined
            TextureComponentType = TextureComponentType.Float
            StorageTextureFormat = TextureFormat.Undefined
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUBindGroupLayoutEntry -> 'a) : 'a = 
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
    static member Default : BlendDescriptor =
        {
            Operation = BlendOperation.Add
            SrcFactor = BlendFactor.One
            DstFactor = BlendFactor.Zero
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUBlendDescriptor -> 'a) : 'a = 
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
type BufferDescriptor =
    {
        Label : string
        Usage : BufferUsage
        Size : uint64
        MappedAtCreation : bool
    }
    static member Default(Usage: BufferUsage, Size: uint64) : BufferDescriptor =
        {
            Label = null
            Usage = Usage
            Size = Size
            MappedAtCreation = false
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUBufferDescriptor -> 'a) : 'a = 
        let x = x
        let inline _LabelCont (_Label) = 
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
        if not (isNull x.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount x.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(x.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
[<Struct>]
type PipelineLayoutDescriptor =
    {
        Label : string
        BindGroupLayouts : array<BindGroupLayout>
    }
    static member Default(BindGroupLayouts: array<BindGroupLayout>) : PipelineLayoutDescriptor =
        {
            Label = null
            BindGroupLayouts = BindGroupLayouts
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUPipelineLayoutDescriptor -> 'a) : 'a = 
        let x = x
        let inline _LabelCont (_Label) = 
            let _BindGroupLayoutsCount = x.BindGroupLayouts.Length
            let _BindGroupLayouts = NativePtr.stackalloc _BindGroupLayoutsCount
            for i in 0 .. _BindGroupLayoutsCount-1 do
                if isNull x.BindGroupLayouts.[i] then NativePtr.set _BindGroupLayouts i BindGroupLayoutHandle.Null
                else NativePtr.set _BindGroupLayouts i x.BindGroupLayouts.[i].Handle
            let mutable native = Unchecked.defaultof<DawnRaw.WGPUPipelineLayoutDescriptor>
            native.Next <- 0n
            native.Label <- _Label
            native.BindGroupLayoutsCount <- _BindGroupLayoutsCount
            native.BindGroupLayouts <- _BindGroupLayouts
            callback native
        if not (isNull x.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount x.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(x.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
[<Struct>]
type ProgrammableStageDescriptor =
    {
        Module : ShaderModule
        EntryPoint : string
    }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUProgrammableStageDescriptor -> 'a) : 'a = 
        let x = x
        let _Module = (if isNull x.Module then ShaderModuleHandle.Null else x.Module.Handle)
        let inline _EntryPointCont (_EntryPoint) = 
            let mutable native = Unchecked.defaultof<DawnRaw.WGPUProgrammableStageDescriptor>
            native.Next <- 0n
            native.Module <- _Module
            native.EntryPoint <- _EntryPoint
            callback native
        if not (isNull x.EntryPoint) then
            let _EntryPointLen = System.Text.Encoding.UTF8.GetByteCount x.EntryPoint
            let _EntryPointSize = _EntryPointLen + 1
            let _EntryPointPtr = NativePtr.stackalloc<byte> _EntryPointSize
            System.Text.Encoding.UTF8.GetBytes(x.EntryPoint.AsSpan(), Span<byte>(NativePtr.toVoidPtr _EntryPointPtr, _EntryPointSize)) |> ignore
            NativePtr.set _EntryPointPtr _EntryPointLen 0uy
            _EntryPointCont (NativePtr.toNativeInt _EntryPointPtr)
        else
            _EntryPointCont 0n
[<Struct>]
type QuerySetDescriptor =
    {
        Label : string
        Type : QueryType
        Count : int
        PipelineStatistics : option<PipelineStatisticName>
        PipelineStatisticsCount : int
    }
    static member Default(Type: QueryType, Count: int, PipelineStatistics: option<PipelineStatisticName>) : QuerySetDescriptor =
        {
            Label = null
            Type = Type
            Count = Count
            PipelineStatistics = PipelineStatistics
            PipelineStatisticsCount = 0
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUQuerySetDescriptor -> 'a) : 'a = 
        let x = x
        let inline _LabelCont (_Label) = 
            let _Type = x.Type
            let _Count = x.Count
            let inline _PipelineStatisticsCont _PipelineStatistics =
                let _PipelineStatisticsCount = x.PipelineStatisticsCount
                let mutable native = Unchecked.defaultof<DawnRaw.WGPUQuerySetDescriptor>
                native.Next <- 0n
                native.Label <- _Label
                native.Type <- _Type
                native.Count <- _Count
                native.PipelineStatistics <- _PipelineStatistics
                native.PipelineStatisticsCount <- _PipelineStatisticsCount
                callback native
            match x.PipelineStatistics with
            | Some o ->
                let _PipelineStatistics = NativePtr.stackalloc 1
                NativePtr.write _PipelineStatistics o
                _PipelineStatisticsCont _PipelineStatistics
            | _ ->
                _PipelineStatisticsCont (NativePtr.ofNativeInt 0n)
        if not (isNull x.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount x.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(x.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
[<Struct>]
type RasterizationStateDescriptor =
    {
        FrontFace : FrontFace
        CullMode : CullMode
        DepthBias : int32
        DepthBiasSlopeScale : float32
        DepthBiasClamp : float32
    }
    static member Default : RasterizationStateDescriptor =
        {
            FrontFace = FrontFace.CCW
            CullMode = CullMode.None
            DepthBias = 0
            DepthBiasSlopeScale = 0.000000f
            DepthBiasClamp = 0.000000f
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPURasterizationStateDescriptor -> 'a) : 'a = 
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
type RenderBundleEncoderDescriptor =
    {
        Label : string
        ColorFormats : array<TextureFormat>
        DepthStencilFormat : TextureFormat
        SampleCount : int
    }
    static member Default(ColorFormats: array<TextureFormat>) : RenderBundleEncoderDescriptor =
        {
            Label = null
            ColorFormats = ColorFormats
            DepthStencilFormat = TextureFormat.Undefined
            SampleCount = 1
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPURenderBundleEncoderDescriptor -> 'a) : 'a = 
        let x = x
        let inline _LabelCont (_Label) = 
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
        if not (isNull x.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount x.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(x.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
[<Struct>]
type RenderPassColorAttachmentDescriptor =
    {
        Attachment : TextureView
        ResolveTarget : TextureView
        LoadOp : LoadOp
        StoreOp : StoreOp
        ClearColor : Color
    }

    member inline x.Pin<'a>(callback : DawnRaw.WGPURenderPassColorAttachmentDescriptor -> 'a) : 'a = 
        let x = x
        let _Attachment = (if isNull x.Attachment then TextureViewHandle.Null else x.Attachment.Handle)
        let _ResolveTarget = (if isNull x.ResolveTarget then TextureViewHandle.Null else x.ResolveTarget.Handle)
        let _LoadOp = x.LoadOp
        let _StoreOp = x.StoreOp
        let _R = x.ClearColor.R
        let _G = x.ClearColor.G
        let _B = x.ClearColor.B
        let _A = x.ClearColor.A
        let mutable _ClearColor = Unchecked.defaultof<DawnRaw.WGPUColor>
        _ClearColor.R <- _R
        _ClearColor.G <- _G
        _ClearColor.B <- _B
        _ClearColor.A <- _A
        let _ClearColor = _ClearColor
        let mutable native = Unchecked.defaultof<DawnRaw.WGPURenderPassColorAttachmentDescriptor>
        native.Attachment <- _Attachment
        native.ResolveTarget <- _ResolveTarget
        native.LoadOp <- _LoadOp
        native.StoreOp <- _StoreOp
        native.ClearColor <- _ClearColor
        callback native
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
    static member Default(Attachment: TextureView, DepthLoadOp: LoadOp, DepthStoreOp: StoreOp, ClearDepth: float32, StencilLoadOp: LoadOp, StencilStoreOp: StoreOp) : RenderPassDepthStencilAttachmentDescriptor =
        {
            Attachment = Attachment
            DepthLoadOp = DepthLoadOp
            DepthStoreOp = DepthStoreOp
            ClearDepth = ClearDepth
            DepthReadOnly = false
            StencilLoadOp = StencilLoadOp
            StencilStoreOp = StencilStoreOp
            ClearStencil = 0
            StencilReadOnly = false
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPURenderPassDepthStencilAttachmentDescriptor -> 'a) : 'a = 
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
    static member Default : SamplerDescriptor =
        {
            Label = null
            AddressModeU = AddressMode.ClampToEdge
            AddressModeV = AddressMode.ClampToEdge
            AddressModeW = AddressMode.ClampToEdge
            MagFilter = FilterMode.Nearest
            MinFilter = FilterMode.Nearest
            MipmapFilter = FilterMode.Nearest
            LodMinClamp = 0.000000f
            LodMaxClamp = 1000.000000f
            Compare = CompareFunction.Undefined
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUSamplerDescriptor -> 'a) : 'a = 
        let x = x
        let inline _LabelCont (_Label) = 
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
        if not (isNull x.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount x.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(x.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
[<Struct>]
type StencilStateFaceDescriptor =
    {
        Compare : CompareFunction
        FailOp : StencilOperation
        DepthFailOp : StencilOperation
        PassOp : StencilOperation
    }
    static member Default : StencilStateFaceDescriptor =
        {
            Compare = CompareFunction.Always
            FailOp = StencilOperation.Keep
            DepthFailOp = StencilOperation.Keep
            PassOp = StencilOperation.Keep
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUStencilStateFaceDescriptor -> 'a) : 'a = 
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
    static member Default(Usage: TextureUsage, Format: TextureFormat, Width: int, Height: int, PresentMode: PresentMode) : SwapChainDescriptor =
        {
            Label = null
            Usage = Usage
            Format = Format
            Width = Width
            Height = Height
            PresentMode = PresentMode
            Implementation = 0UL
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUSwapChainDescriptor -> 'a) : 'a = 
        let x = x
        let inline _LabelCont (_Label) = 
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
        if not (isNull x.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount x.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(x.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
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
    static member Default(Usage: TextureUsage, Size: Extent3D, Format: TextureFormat) : TextureDescriptor =
        {
            Label = null
            Usage = Usage
            Dimension = TextureDimension.D2D
            Size = Size
            Format = Format
            MipLevelCount = 1
            SampleCount = 1
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUTextureDescriptor -> 'a) : 'a = 
        let x = x
        let inline _LabelCont (_Label) = 
            let _Usage = x.Usage
            let _Dimension = x.Dimension
            let _Width = x.Size.Width
            let _Height = x.Size.Height
            let _Depth = x.Size.Depth
            let mutable _Size = Unchecked.defaultof<DawnRaw.WGPUExtent3D>
            _Size.Width <- _Width
            _Size.Height <- _Height
            _Size.Depth <- _Depth
            let _Size = _Size
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
        if not (isNull x.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount x.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(x.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
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
    static member Default : TextureViewDescriptor =
        {
            Label = null
            Format = TextureFormat.Undefined
            Dimension = TextureViewDimension.Undefined
            BaseMipLevel = 0
            MipLevelCount = 0
            BaseArrayLayer = 0
            ArrayLayerCount = 0
            Aspect = TextureAspect.All
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUTextureViewDescriptor -> 'a) : 'a = 
        let x = x
        let inline _LabelCont (_Label) = 
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
        if not (isNull x.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount x.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(x.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
[<Struct>]
type VertexAttributeDescriptor =
    {
        Format : VertexFormat
        Offset : uint64
        ShaderLocation : int
    }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUVertexAttributeDescriptor -> 'a) : 'a = 
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
type BindGroupLayoutDescriptor =
    {
        Label : string
        Entries : array<BindGroupLayoutEntry>
    }
    static member Default(Entries: array<BindGroupLayoutEntry>) : BindGroupLayoutDescriptor =
        {
            Label = null
            Entries = Entries
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUBindGroupLayoutDescriptor -> 'a) : 'a = 
        let x = x
        let inline _LabelCont (_Label) = 
            let _EntriesCount = if isNull x.Entries then 0 else x.Entries.Length
            let rec _EntriesCont (_Entriesinputs : array<BindGroupLayoutEntry>) (_Entriesoutputs : array<DawnRaw.WGPUBindGroupLayoutEntry>) (_Entriesi : int) =
                if _Entriesi >= _EntriesCount then
                    use _Entries = fixed _Entriesoutputs
                    let mutable native = Unchecked.defaultof<DawnRaw.WGPUBindGroupLayoutDescriptor>
                    native.Next <- 0n
                    native.Label <- _Label
                    native.EntriesCount <- _EntriesCount
                    native.Entries <- _Entries
                    callback native
                else
                    let _Binding = _Entriesinputs.[_Entriesi].Binding
                    let _Visibility = _Entriesinputs.[_Entriesi].Visibility
                    let _Type = _Entriesinputs.[_Entriesi].Type
                    let _HasDynamicOffset = (if _Entriesinputs.[_Entriesi].HasDynamicOffset then 1 else 0)
                    let _MinBufferBindingSize = _Entriesinputs.[_Entriesi].MinBufferBindingSize
                    let _Multisampled = (if _Entriesinputs.[_Entriesi].Multisampled then 1 else 0)
                    let _ViewDimension = _Entriesinputs.[_Entriesi].ViewDimension
                    let _TextureComponentType = _Entriesinputs.[_Entriesi].TextureComponentType
                    let _StorageTextureFormat = _Entriesinputs.[_Entriesi].StorageTextureFormat
                    let mutable _n = Unchecked.defaultof<DawnRaw.WGPUBindGroupLayoutEntry>
                    _n.Binding <- _Binding
                    _n.Visibility <- _Visibility
                    _n.Type <- _Type
                    _n.HasDynamicOffset <- _HasDynamicOffset
                    _n.MinBufferBindingSize <- _MinBufferBindingSize
                    _n.Multisampled <- _Multisampled
                    _n.ViewDimension <- _ViewDimension
                    _n.TextureComponentType <- _TextureComponentType
                    _n.StorageTextureFormat <- _StorageTextureFormat
                    let _n = _n
                    _Entriesoutputs.[_Entriesi] <- _n
                    _EntriesCont _Entriesinputs _Entriesoutputs (_Entriesi + 1)
            _EntriesCont x.Entries (if _EntriesCount > 0 then Array.zeroCreate _EntriesCount else null) 0
        if not (isNull x.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount x.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(x.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
[<Struct>]
type ColorStateDescriptor =
    {
        Format : TextureFormat
        AlphaBlend : BlendDescriptor
        ColorBlend : BlendDescriptor
        WriteMask : ColorWriteMask
    }
    static member Default(Format: TextureFormat) : ColorStateDescriptor =
        {
            Format = Format
            AlphaBlend = BlendDescriptor.Default
            ColorBlend = BlendDescriptor.Default
            WriteMask = ColorWriteMask.All
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUColorStateDescriptor -> 'a) : 'a = 
        let x = x
        let _Format = x.Format
        let _Operation = x.AlphaBlend.Operation
        let _SrcFactor = x.AlphaBlend.SrcFactor
        let _DstFactor = x.AlphaBlend.DstFactor
        let mutable _AlphaBlend = Unchecked.defaultof<DawnRaw.WGPUBlendDescriptor>
        _AlphaBlend.Operation <- _Operation
        _AlphaBlend.SrcFactor <- _SrcFactor
        _AlphaBlend.DstFactor <- _DstFactor
        let _AlphaBlend = _AlphaBlend
        let _Operation = x.ColorBlend.Operation
        let _SrcFactor = x.ColorBlend.SrcFactor
        let _DstFactor = x.ColorBlend.DstFactor
        let mutable _ColorBlend = Unchecked.defaultof<DawnRaw.WGPUBlendDescriptor>
        _ColorBlend.Operation <- _Operation
        _ColorBlend.SrcFactor <- _SrcFactor
        _ColorBlend.DstFactor <- _DstFactor
        let _ColorBlend = _ColorBlend
        let _WriteMask = x.WriteMask
        let mutable native = Unchecked.defaultof<DawnRaw.WGPUColorStateDescriptor>
        native.Next <- 0n
        native.Format <- _Format
        native.AlphaBlend <- _AlphaBlend
        native.ColorBlend <- _ColorBlend
        native.WriteMask <- _WriteMask
        callback native
[<Struct>]
type ComputePipelineDescriptor =
    {
        Label : string
        Layout : PipelineLayout
        ComputeStage : ProgrammableStageDescriptor
    }
    static member Default(Layout: PipelineLayout, ComputeStage: ProgrammableStageDescriptor) : ComputePipelineDescriptor =
        {
            Label = null
            Layout = Layout
            ComputeStage = ComputeStage
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUComputePipelineDescriptor -> 'a) : 'a = 
        let x = x
        let inline _LabelCont (_Label) = 
            let _Layout = (if isNull x.Layout then PipelineLayoutHandle.Null else x.Layout.Handle)
            let _Module = (if isNull x.ComputeStage.Module then ShaderModuleHandle.Null else x.ComputeStage.Module.Handle)
            let inline _EntryPointCont (_EntryPoint) = 
                let mutable _ComputeStage = Unchecked.defaultof<DawnRaw.WGPUProgrammableStageDescriptor>
                _ComputeStage.Next <- 0n
                _ComputeStage.Module <- _Module
                _ComputeStage.EntryPoint <- _EntryPoint
                let _ComputeStage = _ComputeStage
                let mutable native = Unchecked.defaultof<DawnRaw.WGPUComputePipelineDescriptor>
                native.Next <- 0n
                native.Label <- _Label
                native.Layout <- _Layout
                native.ComputeStage <- _ComputeStage
                callback native
            if not (isNull x.ComputeStage.EntryPoint) then
                let _EntryPointLen = System.Text.Encoding.UTF8.GetByteCount x.ComputeStage.EntryPoint
                let _EntryPointSize = _EntryPointLen + 1
                let _EntryPointPtr = NativePtr.stackalloc<byte> _EntryPointSize
                System.Text.Encoding.UTF8.GetBytes(x.ComputeStage.EntryPoint.AsSpan(), Span<byte>(NativePtr.toVoidPtr _EntryPointPtr, _EntryPointSize)) |> ignore
                NativePtr.set _EntryPointPtr _EntryPointLen 0uy
                _EntryPointCont (NativePtr.toNativeInt _EntryPointPtr)
            else
                _EntryPointCont 0n
        if not (isNull x.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount x.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(x.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
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
    static member Default(Format: TextureFormat) : DepthStencilStateDescriptor =
        {
            Format = Format
            DepthWriteEnabled = false
            DepthCompare = CompareFunction.Always
            StencilFront = StencilStateFaceDescriptor.Default
            StencilBack = StencilStateFaceDescriptor.Default
            StencilReadMask = -1
            StencilWriteMask = -1
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUDepthStencilStateDescriptor -> 'a) : 'a = 
        let x = x
        let _Format = x.Format
        let _DepthWriteEnabled = (if x.DepthWriteEnabled then 1 else 0)
        let _DepthCompare = x.DepthCompare
        let _Compare = x.StencilFront.Compare
        let _FailOp = x.StencilFront.FailOp
        let _DepthFailOp = x.StencilFront.DepthFailOp
        let _PassOp = x.StencilFront.PassOp
        let mutable _StencilFront = Unchecked.defaultof<DawnRaw.WGPUStencilStateFaceDescriptor>
        _StencilFront.Compare <- _Compare
        _StencilFront.FailOp <- _FailOp
        _StencilFront.DepthFailOp <- _DepthFailOp
        _StencilFront.PassOp <- _PassOp
        let _StencilFront = _StencilFront
        let _Compare = x.StencilBack.Compare
        let _FailOp = x.StencilBack.FailOp
        let _DepthFailOp = x.StencilBack.DepthFailOp
        let _PassOp = x.StencilBack.PassOp
        let mutable _StencilBack = Unchecked.defaultof<DawnRaw.WGPUStencilStateFaceDescriptor>
        _StencilBack.Compare <- _Compare
        _StencilBack.FailOp <- _FailOp
        _StencilBack.DepthFailOp <- _DepthFailOp
        _StencilBack.PassOp <- _PassOp
        let _StencilBack = _StencilBack
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
[<Struct>]
type RenderPassDescriptor =
    {
        Label : string
        ColorAttachments : array<RenderPassColorAttachmentDescriptor>
        DepthStencilAttachment : option<RenderPassDepthStencilAttachmentDescriptor>
        OcclusionQuerySet : QuerySet
    }
    static member Default(ColorAttachments: array<RenderPassColorAttachmentDescriptor>, DepthStencilAttachment: option<RenderPassDepthStencilAttachmentDescriptor>, OcclusionQuerySet: QuerySet) : RenderPassDescriptor =
        {
            Label = null
            ColorAttachments = ColorAttachments
            DepthStencilAttachment = DepthStencilAttachment
            OcclusionQuerySet = OcclusionQuerySet
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPURenderPassDescriptor -> 'a) : 'a = 
        let x = x
        let inline _LabelCont (_Label) = 
            let _ColorAttachmentsCount = if isNull x.ColorAttachments then 0 else x.ColorAttachments.Length
            let rec _ColorAttachmentsCont (_ColorAttachmentsinputs : array<RenderPassColorAttachmentDescriptor>) (_ColorAttachmentsoutputs : array<DawnRaw.WGPURenderPassColorAttachmentDescriptor>) (_ColorAttachmentsi : int) =
                if _ColorAttachmentsi >= _ColorAttachmentsCount then
                    use _ColorAttachments = fixed _ColorAttachmentsoutputs
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
                    | Some v ->
                        let _Attachment = (if isNull v.Attachment then TextureViewHandle.Null else v.Attachment.Handle)
                        let _DepthLoadOp = v.DepthLoadOp
                        let _DepthStoreOp = v.DepthStoreOp
                        let _ClearDepth = v.ClearDepth
                        let _DepthReadOnly = (if v.DepthReadOnly then 1 else 0)
                        let _StencilLoadOp = v.StencilLoadOp
                        let _StencilStoreOp = v.StencilStoreOp
                        let _ClearStencil = v.ClearStencil
                        let _StencilReadOnly = (if v.StencilReadOnly then 1 else 0)
                        let mutable _n = Unchecked.defaultof<DawnRaw.WGPURenderPassDepthStencilAttachmentDescriptor>
                        _n.Attachment <- _Attachment
                        _n.DepthLoadOp <- _DepthLoadOp
                        _n.DepthStoreOp <- _DepthStoreOp
                        _n.ClearDepth <- _ClearDepth
                        _n.DepthReadOnly <- _DepthReadOnly
                        _n.StencilLoadOp <- _StencilLoadOp
                        _n.StencilStoreOp <- _StencilStoreOp
                        _n.ClearStencil <- _ClearStencil
                        _n.StencilReadOnly <- _StencilReadOnly
                        let _n = _n
                        let ptr = NativePtr.stackalloc 1
                        NativePtr.write ptr _n
                        _DepthStencilAttachmentCont ptr
                    | None -> _DepthStencilAttachmentCont (NativePtr.ofNativeInt 0n)
                else
                    let _Attachment = (if isNull _ColorAttachmentsinputs.[_ColorAttachmentsi].Attachment then TextureViewHandle.Null else _ColorAttachmentsinputs.[_ColorAttachmentsi].Attachment.Handle)
                    let _ResolveTarget = (if isNull _ColorAttachmentsinputs.[_ColorAttachmentsi].ResolveTarget then TextureViewHandle.Null else _ColorAttachmentsinputs.[_ColorAttachmentsi].ResolveTarget.Handle)
                    let _LoadOp = _ColorAttachmentsinputs.[_ColorAttachmentsi].LoadOp
                    let _StoreOp = _ColorAttachmentsinputs.[_ColorAttachmentsi].StoreOp
                    let _R = _ColorAttachmentsinputs.[_ColorAttachmentsi].ClearColor.R
                    let _G = _ColorAttachmentsinputs.[_ColorAttachmentsi].ClearColor.G
                    let _B = _ColorAttachmentsinputs.[_ColorAttachmentsi].ClearColor.B
                    let _A = _ColorAttachmentsinputs.[_ColorAttachmentsi].ClearColor.A
                    let mutable _ClearColor = Unchecked.defaultof<DawnRaw.WGPUColor>
                    _ClearColor.R <- _R
                    _ClearColor.G <- _G
                    _ClearColor.B <- _B
                    _ClearColor.A <- _A
                    let _ClearColor = _ClearColor
                    let mutable _n = Unchecked.defaultof<DawnRaw.WGPURenderPassColorAttachmentDescriptor>
                    _n.Attachment <- _Attachment
                    _n.ResolveTarget <- _ResolveTarget
                    _n.LoadOp <- _LoadOp
                    _n.StoreOp <- _StoreOp
                    _n.ClearColor <- _ClearColor
                    let _n = _n
                    _ColorAttachmentsoutputs.[_ColorAttachmentsi] <- _n
                    _ColorAttachmentsCont _ColorAttachmentsinputs _ColorAttachmentsoutputs (_ColorAttachmentsi + 1)
            _ColorAttachmentsCont x.ColorAttachments (if _ColorAttachmentsCount > 0 then Array.zeroCreate _ColorAttachmentsCount else null) 0
        if not (isNull x.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount x.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(x.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
[<Struct>]
type RenderPipelineDescriptorDummyExtension =
    {
        DummyStage : ProgrammableStageDescriptor
    }

    member inline x.Pin<'a>(callback : DawnRaw.WGPURenderPipelineDescriptorDummyExtension -> 'a) : 'a = 
        let x = x
        let _Module = (if isNull x.DummyStage.Module then ShaderModuleHandle.Null else x.DummyStage.Module.Handle)
        let inline _EntryPointCont (_EntryPoint) = 
            let mutable _DummyStage = Unchecked.defaultof<DawnRaw.WGPUProgrammableStageDescriptor>
            _DummyStage.Next <- 0n
            _DummyStage.Module <- _Module
            _DummyStage.EntryPoint <- _EntryPoint
            let _DummyStage = _DummyStage
            let mutable native = Unchecked.defaultof<DawnRaw.WGPURenderPipelineDescriptorDummyExtension>
            native.DummyStage <- _DummyStage
            callback native
        if not (isNull x.DummyStage.EntryPoint) then
            let _EntryPointLen = System.Text.Encoding.UTF8.GetByteCount x.DummyStage.EntryPoint
            let _EntryPointSize = _EntryPointLen + 1
            let _EntryPointPtr = NativePtr.stackalloc<byte> _EntryPointSize
            System.Text.Encoding.UTF8.GetBytes(x.DummyStage.EntryPoint.AsSpan(), Span<byte>(NativePtr.toVoidPtr _EntryPointPtr, _EntryPointSize)) |> ignore
            NativePtr.set _EntryPointPtr _EntryPointLen 0uy
            _EntryPointCont (NativePtr.toNativeInt _EntryPointPtr)
        else
            _EntryPointCont 0n
[<Struct>]
type VertexBufferLayoutDescriptor =
    {
        ArrayStride : uint64
        StepMode : InputStepMode
        Attributes : array<VertexAttributeDescriptor>
    }
    static member Default(ArrayStride: uint64, Attributes: array<VertexAttributeDescriptor>) : VertexBufferLayoutDescriptor =
        {
            ArrayStride = ArrayStride
            StepMode = InputStepMode.Vertex
            Attributes = Attributes
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUVertexBufferLayoutDescriptor -> 'a) : 'a = 
        let x = x
        let _ArrayStride = x.ArrayStride
        let _StepMode = x.StepMode
        let _AttributesCount = if isNull x.Attributes then 0 else x.Attributes.Length
        let rec _AttributesCont (_Attributesinputs : array<VertexAttributeDescriptor>) (_Attributesoutputs : array<DawnRaw.WGPUVertexAttributeDescriptor>) (_Attributesi : int) =
            if _Attributesi >= _AttributesCount then
                use _Attributes = fixed _Attributesoutputs
                let mutable native = Unchecked.defaultof<DawnRaw.WGPUVertexBufferLayoutDescriptor>
                native.ArrayStride <- _ArrayStride
                native.StepMode <- _StepMode
                native.AttributesCount <- _AttributesCount
                native.Attributes <- _Attributes
                callback native
            else
                let _Format = _Attributesinputs.[_Attributesi].Format
                let _Offset = _Attributesinputs.[_Attributesi].Offset
                let _ShaderLocation = _Attributesinputs.[_Attributesi].ShaderLocation
                let mutable _n = Unchecked.defaultof<DawnRaw.WGPUVertexAttributeDescriptor>
                _n.Format <- _Format
                _n.Offset <- _Offset
                _n.ShaderLocation <- _ShaderLocation
                let _n = _n
                _Attributesoutputs.[_Attributesi] <- _n
                _AttributesCont _Attributesinputs _Attributesoutputs (_Attributesi + 1)
        _AttributesCont x.Attributes (if _AttributesCount > 0 then Array.zeroCreate _AttributesCount else null) 0
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
    static member Default(Binding: int, Buffer: Buffer, Size: uint64, Sampler: Sampler, TextureView: TextureView) : BindGroupEntry =
        {
            Binding = Binding
            Buffer = Buffer
            Offset = 0UL
            Size = Size
            Sampler = Sampler
            TextureView = TextureView
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUBindGroupEntry -> 'a) : 'a = 
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
type BufferCopyView =
    {
        Layout : TextureDataLayout
        Buffer : Buffer
    }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUBufferCopyView -> 'a) : 'a = 
        let x = x
        let _Offset = x.Layout.Offset
        let _BytesPerRow = x.Layout.BytesPerRow
        let _RowsPerImage = x.Layout.RowsPerImage
        let mutable _Layout = Unchecked.defaultof<DawnRaw.WGPUTextureDataLayout>
        _Layout.Next <- 0n
        _Layout.Offset <- _Offset
        _Layout.BytesPerRow <- _BytesPerRow
        _Layout.RowsPerImage <- _RowsPerImage
        let _Layout = _Layout
        let _Buffer = (if isNull x.Buffer then BufferHandle.Null else x.Buffer.Handle)
        let mutable native = Unchecked.defaultof<DawnRaw.WGPUBufferCopyView>
        native.Next <- 0n
        native.Layout <- _Layout
        native.Buffer <- _Buffer
        callback native
[<Struct>]
type TextureCopyView =
    {
        Texture : Texture
        MipLevel : int
        Origin : Origin3D
        Aspect : TextureAspect
    }
    static member Default(Texture: Texture) : TextureCopyView =
        {
            Texture = Texture
            MipLevel = 0
            Origin = Origin3D.Default
            Aspect = TextureAspect.All
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUTextureCopyView -> 'a) : 'a = 
        let x = x
        let _Texture = (if isNull x.Texture then TextureHandle.Null else x.Texture.Handle)
        let _MipLevel = x.MipLevel
        let _X = x.Origin.X
        let _Y = x.Origin.Y
        let _Z = x.Origin.Z
        let mutable _Origin = Unchecked.defaultof<DawnRaw.WGPUOrigin3D>
        _Origin.X <- _X
        _Origin.Y <- _Y
        _Origin.Z <- _Z
        let _Origin = _Origin
        let _Aspect = x.Aspect
        let mutable native = Unchecked.defaultof<DawnRaw.WGPUTextureCopyView>
        native.Next <- 0n
        native.Texture <- _Texture
        native.MipLevel <- _MipLevel
        native.Origin <- _Origin
        native.Aspect <- _Aspect
        callback native
[<Struct>]
type VertexStateDescriptor =
    {
        IndexFormat : IndexFormat
        VertexBuffers : array<VertexBufferLayoutDescriptor>
    }
    static member Default(VertexBuffers: array<VertexBufferLayoutDescriptor>) : VertexStateDescriptor =
        {
            IndexFormat = IndexFormat.Undefined
            VertexBuffers = VertexBuffers
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUVertexStateDescriptor -> 'a) : 'a = 
        let x = x
        let _IndexFormat = x.IndexFormat
        let _VertexBuffersCount = if isNull x.VertexBuffers then 0 else x.VertexBuffers.Length
        let rec _VertexBuffersCont (_VertexBuffersinputs : array<VertexBufferLayoutDescriptor>) (_VertexBuffersoutputs : array<DawnRaw.WGPUVertexBufferLayoutDescriptor>) (_VertexBuffersi : int) =
            if _VertexBuffersi >= _VertexBuffersCount then
                use _VertexBuffers = fixed _VertexBuffersoutputs
                let mutable native = Unchecked.defaultof<DawnRaw.WGPUVertexStateDescriptor>
                native.Next <- 0n
                native.IndexFormat <- _IndexFormat
                native.VertexBuffersCount <- _VertexBuffersCount
                native.VertexBuffers <- _VertexBuffers
                callback native
            else
                let _ArrayStride = _VertexBuffersinputs.[_VertexBuffersi].ArrayStride
                let _StepMode = _VertexBuffersinputs.[_VertexBuffersi].StepMode
                let _AttributesCount = if isNull _VertexBuffersinputs.[_VertexBuffersi].Attributes then 0 else _VertexBuffersinputs.[_VertexBuffersi].Attributes.Length
                let rec _AttributesCont (_Attributesinputs : array<VertexAttributeDescriptor>) (_Attributesoutputs : array<DawnRaw.WGPUVertexAttributeDescriptor>) (_Attributesi : int) =
                    if _Attributesi >= _AttributesCount then
                        use _Attributes = fixed _Attributesoutputs
                        let mutable _n = Unchecked.defaultof<DawnRaw.WGPUVertexBufferLayoutDescriptor>
                        _n.ArrayStride <- _ArrayStride
                        _n.StepMode <- _StepMode
                        _n.AttributesCount <- _AttributesCount
                        _n.Attributes <- _Attributes
                        let _n = _n
                        _VertexBuffersoutputs.[_VertexBuffersi] <- _n
                        _VertexBuffersCont _VertexBuffersinputs _VertexBuffersoutputs (_VertexBuffersi + 1)
                    else
                        let _Format = _Attributesinputs.[_Attributesi].Format
                        let _Offset = _Attributesinputs.[_Attributesi].Offset
                        let _ShaderLocation = _Attributesinputs.[_Attributesi].ShaderLocation
                        let mutable _n = Unchecked.defaultof<DawnRaw.WGPUVertexAttributeDescriptor>
                        _n.Format <- _Format
                        _n.Offset <- _Offset
                        _n.ShaderLocation <- _ShaderLocation
                        let _n = _n
                        _Attributesoutputs.[_Attributesi] <- _n
                        _AttributesCont _Attributesinputs _Attributesoutputs (_Attributesi + 1)
                _AttributesCont _VertexBuffersinputs.[_VertexBuffersi].Attributes (if _AttributesCount > 0 then Array.zeroCreate _AttributesCount else null) 0
        _VertexBuffersCont x.VertexBuffers (if _VertexBuffersCount > 0 then Array.zeroCreate _VertexBuffersCount else null) 0
[<Struct>]
type BindGroupDescriptor =
    {
        Label : string
        Layout : BindGroupLayout
        Entries : array<BindGroupEntry>
    }
    static member Default(Layout: BindGroupLayout, Entries: array<BindGroupEntry>) : BindGroupDescriptor =
        {
            Label = null
            Layout = Layout
            Entries = Entries
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPUBindGroupDescriptor -> 'a) : 'a = 
        let x = x
        let inline _LabelCont (_Label) = 
            let _Layout = (if isNull x.Layout then BindGroupLayoutHandle.Null else x.Layout.Handle)
            let _EntriesCount = if isNull x.Entries then 0 else x.Entries.Length
            let rec _EntriesCont (_Entriesinputs : array<BindGroupEntry>) (_Entriesoutputs : array<DawnRaw.WGPUBindGroupEntry>) (_Entriesi : int) =
                if _Entriesi >= _EntriesCount then
                    use _Entries = fixed _Entriesoutputs
                    let mutable native = Unchecked.defaultof<DawnRaw.WGPUBindGroupDescriptor>
                    native.Next <- 0n
                    native.Label <- _Label
                    native.Layout <- _Layout
                    native.EntriesCount <- _EntriesCount
                    native.Entries <- _Entries
                    callback native
                else
                    let _Binding = _Entriesinputs.[_Entriesi].Binding
                    let _Buffer = (if isNull _Entriesinputs.[_Entriesi].Buffer then BufferHandle.Null else _Entriesinputs.[_Entriesi].Buffer.Handle)
                    let _Offset = _Entriesinputs.[_Entriesi].Offset
                    let _Size = _Entriesinputs.[_Entriesi].Size
                    let _Sampler = (if isNull _Entriesinputs.[_Entriesi].Sampler then SamplerHandle.Null else _Entriesinputs.[_Entriesi].Sampler.Handle)
                    let _TextureView = (if isNull _Entriesinputs.[_Entriesi].TextureView then TextureViewHandle.Null else _Entriesinputs.[_Entriesi].TextureView.Handle)
                    let mutable _n = Unchecked.defaultof<DawnRaw.WGPUBindGroupEntry>
                    _n.Binding <- _Binding
                    _n.Buffer <- _Buffer
                    _n.Offset <- _Offset
                    _n.Size <- _Size
                    _n.Sampler <- _Sampler
                    _n.TextureView <- _TextureView
                    let _n = _n
                    _Entriesoutputs.[_Entriesi] <- _n
                    _EntriesCont _Entriesinputs _Entriesoutputs (_Entriesi + 1)
            _EntriesCont x.Entries (if _EntriesCount > 0 then Array.zeroCreate _EntriesCount else null) 0
        if not (isNull x.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount x.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(x.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
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
    static member Default(Layout: PipelineLayout, VertexStage: ProgrammableStageDescriptor, FragmentStage: option<ProgrammableStageDescriptor>, VertexState: option<VertexStateDescriptor>, PrimitiveTopology: PrimitiveTopology, RasterizationState: option<RasterizationStateDescriptor>, DepthStencilState: option<DepthStencilStateDescriptor>, ColorStates: array<ColorStateDescriptor>) : RenderPipelineDescriptor =
        {
            Label = null
            Layout = Layout
            VertexStage = VertexStage
            FragmentStage = FragmentStage
            VertexState = VertexState
            PrimitiveTopology = PrimitiveTopology
            RasterizationState = RasterizationState
            SampleCount = 1
            DepthStencilState = DepthStencilState
            ColorStates = ColorStates
            SampleMask = -1
            AlphaToCoverageEnabled = false
        }

    member inline x.Pin<'a>(callback : DawnRaw.WGPURenderPipelineDescriptor -> 'a) : 'a = 
        let x = x
        let inline _LabelCont (_Label) = 
            let _Layout = (if isNull x.Layout then PipelineLayoutHandle.Null else x.Layout.Handle)
            let _Module = (if isNull x.VertexStage.Module then ShaderModuleHandle.Null else x.VertexStage.Module.Handle)
            let inline _EntryPointCont (_EntryPoint) = 
                let mutable _VertexStage = Unchecked.defaultof<DawnRaw.WGPUProgrammableStageDescriptor>
                _VertexStage.Next <- 0n
                _VertexStage.Module <- _Module
                _VertexStage.EntryPoint <- _EntryPoint
                let _VertexStage = _VertexStage
                let inline _FragmentStageCont _FragmentStage = 
                    let inline _VertexStateCont _VertexState = 
                        let _PrimitiveTopology = x.PrimitiveTopology
                        let inline _RasterizationStateCont _RasterizationState = 
                            let _SampleCount = x.SampleCount
                            let inline _DepthStencilStateCont _DepthStencilState = 
                                let _ColorStatesCount = if isNull x.ColorStates then 0 else x.ColorStates.Length
                                let rec _ColorStatesCont (_ColorStatesinputs : array<ColorStateDescriptor>) (_ColorStatesoutputs : array<DawnRaw.WGPUColorStateDescriptor>) (_ColorStatesi : int) =
                                    if _ColorStatesi >= _ColorStatesCount then
                                        use _ColorStates = fixed _ColorStatesoutputs
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
                                        let _Format = _ColorStatesinputs.[_ColorStatesi].Format
                                        let _Operation = _ColorStatesinputs.[_ColorStatesi].AlphaBlend.Operation
                                        let _SrcFactor = _ColorStatesinputs.[_ColorStatesi].AlphaBlend.SrcFactor
                                        let _DstFactor = _ColorStatesinputs.[_ColorStatesi].AlphaBlend.DstFactor
                                        let mutable _AlphaBlend = Unchecked.defaultof<DawnRaw.WGPUBlendDescriptor>
                                        _AlphaBlend.Operation <- _Operation
                                        _AlphaBlend.SrcFactor <- _SrcFactor
                                        _AlphaBlend.DstFactor <- _DstFactor
                                        let _AlphaBlend = _AlphaBlend
                                        let _Operation = _ColorStatesinputs.[_ColorStatesi].ColorBlend.Operation
                                        let _SrcFactor = _ColorStatesinputs.[_ColorStatesi].ColorBlend.SrcFactor
                                        let _DstFactor = _ColorStatesinputs.[_ColorStatesi].ColorBlend.DstFactor
                                        let mutable _ColorBlend = Unchecked.defaultof<DawnRaw.WGPUBlendDescriptor>
                                        _ColorBlend.Operation <- _Operation
                                        _ColorBlend.SrcFactor <- _SrcFactor
                                        _ColorBlend.DstFactor <- _DstFactor
                                        let _ColorBlend = _ColorBlend
                                        let _WriteMask = _ColorStatesinputs.[_ColorStatesi].WriteMask
                                        let mutable _n = Unchecked.defaultof<DawnRaw.WGPUColorStateDescriptor>
                                        _n.Next <- 0n
                                        _n.Format <- _Format
                                        _n.AlphaBlend <- _AlphaBlend
                                        _n.ColorBlend <- _ColorBlend
                                        _n.WriteMask <- _WriteMask
                                        let _n = _n
                                        _ColorStatesoutputs.[_ColorStatesi] <- _n
                                        _ColorStatesCont _ColorStatesinputs _ColorStatesoutputs (_ColorStatesi + 1)
                                _ColorStatesCont x.ColorStates (if _ColorStatesCount > 0 then Array.zeroCreate _ColorStatesCount else null) 0
                            match x.DepthStencilState with
                            | Some v ->
                                let _Format = v.Format
                                let _DepthWriteEnabled = (if v.DepthWriteEnabled then 1 else 0)
                                let _DepthCompare = v.DepthCompare
                                let _Compare = v.StencilFront.Compare
                                let _FailOp = v.StencilFront.FailOp
                                let _DepthFailOp = v.StencilFront.DepthFailOp
                                let _PassOp = v.StencilFront.PassOp
                                let mutable _StencilFront = Unchecked.defaultof<DawnRaw.WGPUStencilStateFaceDescriptor>
                                _StencilFront.Compare <- _Compare
                                _StencilFront.FailOp <- _FailOp
                                _StencilFront.DepthFailOp <- _DepthFailOp
                                _StencilFront.PassOp <- _PassOp
                                let _StencilFront = _StencilFront
                                let _Compare = v.StencilBack.Compare
                                let _FailOp = v.StencilBack.FailOp
                                let _DepthFailOp = v.StencilBack.DepthFailOp
                                let _PassOp = v.StencilBack.PassOp
                                let mutable _StencilBack = Unchecked.defaultof<DawnRaw.WGPUStencilStateFaceDescriptor>
                                _StencilBack.Compare <- _Compare
                                _StencilBack.FailOp <- _FailOp
                                _StencilBack.DepthFailOp <- _DepthFailOp
                                _StencilBack.PassOp <- _PassOp
                                let _StencilBack = _StencilBack
                                let _StencilReadMask = v.StencilReadMask
                                let _StencilWriteMask = v.StencilWriteMask
                                let mutable _n = Unchecked.defaultof<DawnRaw.WGPUDepthStencilStateDescriptor>
                                _n.Next <- 0n
                                _n.Format <- _Format
                                _n.DepthWriteEnabled <- _DepthWriteEnabled
                                _n.DepthCompare <- _DepthCompare
                                _n.StencilFront <- _StencilFront
                                _n.StencilBack <- _StencilBack
                                _n.StencilReadMask <- _StencilReadMask
                                _n.StencilWriteMask <- _StencilWriteMask
                                let _n = _n
                                let ptr = NativePtr.stackalloc 1
                                NativePtr.write ptr _n
                                _DepthStencilStateCont ptr
                            | None -> _DepthStencilStateCont (NativePtr.ofNativeInt 0n)
                        match x.RasterizationState with
                        | Some v ->
                            let _FrontFace = v.FrontFace
                            let _CullMode = v.CullMode
                            let _DepthBias = v.DepthBias
                            let _DepthBiasSlopeScale = v.DepthBiasSlopeScale
                            let _DepthBiasClamp = v.DepthBiasClamp
                            let mutable _n = Unchecked.defaultof<DawnRaw.WGPURasterizationStateDescriptor>
                            _n.Next <- 0n
                            _n.FrontFace <- _FrontFace
                            _n.CullMode <- _CullMode
                            _n.DepthBias <- _DepthBias
                            _n.DepthBiasSlopeScale <- _DepthBiasSlopeScale
                            _n.DepthBiasClamp <- _DepthBiasClamp
                            let _n = _n
                            let ptr = NativePtr.stackalloc 1
                            NativePtr.write ptr _n
                            _RasterizationStateCont ptr
                        | None -> _RasterizationStateCont (NativePtr.ofNativeInt 0n)
                    match x.VertexState with
                    | Some v ->
                        let _IndexFormat = v.IndexFormat
                        let _VertexBuffersCount = if isNull v.VertexBuffers then 0 else v.VertexBuffers.Length
                        let rec _VertexBuffersCont (_VertexBuffersinputs : array<VertexBufferLayoutDescriptor>) (_VertexBuffersoutputs : array<DawnRaw.WGPUVertexBufferLayoutDescriptor>) (_VertexBuffersi : int) =
                            if _VertexBuffersi >= _VertexBuffersCount then
                                use _VertexBuffers = fixed _VertexBuffersoutputs
                                let mutable _n = Unchecked.defaultof<DawnRaw.WGPUVertexStateDescriptor>
                                _n.Next <- 0n
                                _n.IndexFormat <- _IndexFormat
                                _n.VertexBuffersCount <- _VertexBuffersCount
                                _n.VertexBuffers <- _VertexBuffers
                                let _n = _n
                                let ptr = NativePtr.stackalloc 1
                                NativePtr.write ptr _n
                                _VertexStateCont ptr
                            else
                                let _ArrayStride = _VertexBuffersinputs.[_VertexBuffersi].ArrayStride
                                let _StepMode = _VertexBuffersinputs.[_VertexBuffersi].StepMode
                                let _AttributesCount = if isNull _VertexBuffersinputs.[_VertexBuffersi].Attributes then 0 else _VertexBuffersinputs.[_VertexBuffersi].Attributes.Length
                                let rec _AttributesCont (_Attributesinputs : array<VertexAttributeDescriptor>) (_Attributesoutputs : array<DawnRaw.WGPUVertexAttributeDescriptor>) (_Attributesi : int) =
                                    if _Attributesi >= _AttributesCount then
                                        use _Attributes = fixed _Attributesoutputs
                                        let mutable _n = Unchecked.defaultof<DawnRaw.WGPUVertexBufferLayoutDescriptor>
                                        _n.ArrayStride <- _ArrayStride
                                        _n.StepMode <- _StepMode
                                        _n.AttributesCount <- _AttributesCount
                                        _n.Attributes <- _Attributes
                                        let _n = _n
                                        _VertexBuffersoutputs.[_VertexBuffersi] <- _n
                                        _VertexBuffersCont _VertexBuffersinputs _VertexBuffersoutputs (_VertexBuffersi + 1)
                                    else
                                        let _Format = _Attributesinputs.[_Attributesi].Format
                                        let _Offset = _Attributesinputs.[_Attributesi].Offset
                                        let _ShaderLocation = _Attributesinputs.[_Attributesi].ShaderLocation
                                        let mutable _n = Unchecked.defaultof<DawnRaw.WGPUVertexAttributeDescriptor>
                                        _n.Format <- _Format
                                        _n.Offset <- _Offset
                                        _n.ShaderLocation <- _ShaderLocation
                                        let _n = _n
                                        _Attributesoutputs.[_Attributesi] <- _n
                                        _AttributesCont _Attributesinputs _Attributesoutputs (_Attributesi + 1)
                                _AttributesCont _VertexBuffersinputs.[_VertexBuffersi].Attributes (if _AttributesCount > 0 then Array.zeroCreate _AttributesCount else null) 0
                        _VertexBuffersCont v.VertexBuffers (if _VertexBuffersCount > 0 then Array.zeroCreate _VertexBuffersCount else null) 0
                    | None -> _VertexStateCont (NativePtr.ofNativeInt 0n)
                match x.FragmentStage with
                | Some v ->
                    let _Module = (if isNull v.Module then ShaderModuleHandle.Null else v.Module.Handle)
                    let inline _EntryPointCont (_EntryPoint) = 
                        let mutable _n = Unchecked.defaultof<DawnRaw.WGPUProgrammableStageDescriptor>
                        _n.Next <- 0n
                        _n.Module <- _Module
                        _n.EntryPoint <- _EntryPoint
                        let _n = _n
                        let ptr = NativePtr.stackalloc 1
                        NativePtr.write ptr _n
                        _FragmentStageCont ptr
                    if not (isNull v.EntryPoint) then
                        let _EntryPointLen = System.Text.Encoding.UTF8.GetByteCount v.EntryPoint
                        let _EntryPointSize = _EntryPointLen + 1
                        let _EntryPointPtr = NativePtr.stackalloc<byte> _EntryPointSize
                        System.Text.Encoding.UTF8.GetBytes(v.EntryPoint.AsSpan(), Span<byte>(NativePtr.toVoidPtr _EntryPointPtr, _EntryPointSize)) |> ignore
                        NativePtr.set _EntryPointPtr _EntryPointLen 0uy
                        _EntryPointCont (NativePtr.toNativeInt _EntryPointPtr)
                    else
                        _EntryPointCont 0n
                | None -> _FragmentStageCont (NativePtr.ofNativeInt 0n)
            if not (isNull x.VertexStage.EntryPoint) then
                let _EntryPointLen = System.Text.Encoding.UTF8.GetByteCount x.VertexStage.EntryPoint
                let _EntryPointSize = _EntryPointLen + 1
                let _EntryPointPtr = NativePtr.stackalloc<byte> _EntryPointSize
                System.Text.Encoding.UTF8.GetBytes(x.VertexStage.EntryPoint.AsSpan(), Span<byte>(NativePtr.toVoidPtr _EntryPointPtr, _EntryPointSize)) |> ignore
                NativePtr.set _EntryPointPtr _EntryPointLen 0uy
                _EntryPointCont (NativePtr.toNativeInt _EntryPointPtr)
            else
                _EntryPointCont 0n
        if not (isNull x.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount x.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(x.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n


[<AllowNullLiteral>]
type BindGroup(device : Device, handle : BindGroupHandle, refCount : ref<int>) = 
    let mutable isDisposed = false
    member x.Device = device
    member x.ReferenceCount = !refCount
    member x.Handle = handle
    member x.IsDisposed = isDisposed
    member private x.Dispose(disposing : bool) =
        if not isDisposed then 
            let r = Interlocked.Decrement(&refCount.contents)
            isDisposed <- true
            if disposing then System.GC.SuppressFinalize x
            DawnRaw.wgpuBindGroupRelease(handle)
    member x.Dispose() = x.Dispose(true)
    override x.Finalize() = x.Dispose(false)
    member x.Clone() = 
        if isDisposed || Interlocked.Increment(&refCount.contents) = 1 then raise <| System.ObjectDisposedException("BindGroup")
        DawnRaw.wgpuBindGroupReference(handle)
        new BindGroup(device, handle, refCount)
    interface System.IDisposable with
        member x.Dispose() = x.Dispose()
    new(device : Device, handle : BindGroupHandle) = new BindGroup(device, handle, ref 1)
[<AllowNullLiteral>]
type BindGroupLayout(device : Device, handle : BindGroupLayoutHandle, refCount : ref<int>) = 
    let mutable isDisposed = false
    member x.Device = device
    member x.ReferenceCount = !refCount
    member x.Handle = handle
    member x.IsDisposed = isDisposed
    member private x.Dispose(disposing : bool) =
        if not isDisposed then 
            let r = Interlocked.Decrement(&refCount.contents)
            isDisposed <- true
            if disposing then System.GC.SuppressFinalize x
            DawnRaw.wgpuBindGroupLayoutRelease(handle)
    member x.Dispose() = x.Dispose(true)
    override x.Finalize() = x.Dispose(false)
    member x.Clone() = 
        if isDisposed || Interlocked.Increment(&refCount.contents) = 1 then raise <| System.ObjectDisposedException("BindGroupLayout")
        DawnRaw.wgpuBindGroupLayoutReference(handle)
        new BindGroupLayout(device, handle, refCount)
    interface System.IDisposable with
        member x.Dispose() = x.Dispose()
    new(device : Device, handle : BindGroupLayoutHandle) = new BindGroupLayout(device, handle, ref 1)
[<AllowNullLiteral>]
type CommandBuffer(device : Device, handle : CommandBufferHandle, refCount : ref<int>) = 
    let mutable isDisposed = false
    member x.Device = device
    member x.ReferenceCount = !refCount
    member x.Handle = handle
    member x.IsDisposed = isDisposed
    member private x.Dispose(disposing : bool) =
        if not isDisposed then 
            let r = Interlocked.Decrement(&refCount.contents)
            isDisposed <- true
            if disposing then System.GC.SuppressFinalize x
            DawnRaw.wgpuCommandBufferRelease(handle)
    member x.Dispose() = x.Dispose(true)
    override x.Finalize() = x.Dispose(false)
    member x.Clone() = 
        if isDisposed || Interlocked.Increment(&refCount.contents) = 1 then raise <| System.ObjectDisposedException("CommandBuffer")
        DawnRaw.wgpuCommandBufferReference(handle)
        new CommandBuffer(device, handle, refCount)
    interface System.IDisposable with
        member x.Dispose() = x.Dispose()
    new(device : Device, handle : CommandBufferHandle) = new CommandBuffer(device, handle, ref 1)
[<AllowNullLiteral>]
type PipelineLayout(device : Device, handle : PipelineLayoutHandle, refCount : ref<int>) = 
    let mutable isDisposed = false
    member x.Device = device
    member x.ReferenceCount = !refCount
    member x.Handle = handle
    member x.IsDisposed = isDisposed
    member private x.Dispose(disposing : bool) =
        if not isDisposed then 
            let r = Interlocked.Decrement(&refCount.contents)
            isDisposed <- true
            if disposing then System.GC.SuppressFinalize x
            DawnRaw.wgpuPipelineLayoutRelease(handle)
    member x.Dispose() = x.Dispose(true)
    override x.Finalize() = x.Dispose(false)
    member x.Clone() = 
        if isDisposed || Interlocked.Increment(&refCount.contents) = 1 then raise <| System.ObjectDisposedException("PipelineLayout")
        DawnRaw.wgpuPipelineLayoutReference(handle)
        new PipelineLayout(device, handle, refCount)
    interface System.IDisposable with
        member x.Dispose() = x.Dispose()
    new(device : Device, handle : PipelineLayoutHandle) = new PipelineLayout(device, handle, ref 1)
[<AllowNullLiteral>]
type QuerySet(device : Device, handle : QuerySetHandle, refCount : ref<int>) = 
    let mutable isDisposed = false
    member x.Device = device
    member x.ReferenceCount = !refCount
    member x.Handle = handle
    member x.IsDisposed = isDisposed
    member private x.Dispose(disposing : bool) =
        if not isDisposed then 
            let r = Interlocked.Decrement(&refCount.contents)
            isDisposed <- true
            if disposing then System.GC.SuppressFinalize x
            if r = 0 then DawnRaw.wgpuQuerySetDestroy(handle)
            DawnRaw.wgpuQuerySetRelease(handle)
    member x.Dispose() = x.Dispose(true)
    override x.Finalize() = x.Dispose(false)
    member x.Clone() = 
        if isDisposed || Interlocked.Increment(&refCount.contents) = 1 then raise <| System.ObjectDisposedException("QuerySet")
        DawnRaw.wgpuQuerySetReference(handle)
        new QuerySet(device, handle, refCount)
    interface System.IDisposable with
        member x.Dispose() = x.Dispose()
    new(device : Device, handle : QuerySetHandle) = new QuerySet(device, handle, ref 1)
[<AllowNullLiteral>]
type RenderBundle(device : Device, handle : RenderBundleHandle, refCount : ref<int>) = 
    let mutable isDisposed = false
    member x.Device = device
    member x.ReferenceCount = !refCount
    member x.Handle = handle
    member x.IsDisposed = isDisposed
    member private x.Dispose(disposing : bool) =
        if not isDisposed then 
            let r = Interlocked.Decrement(&refCount.contents)
            isDisposed <- true
            if disposing then System.GC.SuppressFinalize x
            DawnRaw.wgpuRenderBundleRelease(handle)
    member x.Dispose() = x.Dispose(true)
    override x.Finalize() = x.Dispose(false)
    member x.Clone() = 
        if isDisposed || Interlocked.Increment(&refCount.contents) = 1 then raise <| System.ObjectDisposedException("RenderBundle")
        DawnRaw.wgpuRenderBundleReference(handle)
        new RenderBundle(device, handle, refCount)
    interface System.IDisposable with
        member x.Dispose() = x.Dispose()
    new(device : Device, handle : RenderBundleHandle) = new RenderBundle(device, handle, ref 1)
[<AllowNullLiteral>]
type Sampler(device : Device, handle : SamplerHandle, refCount : ref<int>) = 
    let mutable isDisposed = false
    member x.Device = device
    member x.ReferenceCount = !refCount
    member x.Handle = handle
    member x.IsDisposed = isDisposed
    member private x.Dispose(disposing : bool) =
        if not isDisposed then 
            let r = Interlocked.Decrement(&refCount.contents)
            isDisposed <- true
            if disposing then System.GC.SuppressFinalize x
            DawnRaw.wgpuSamplerRelease(handle)
    member x.Dispose() = x.Dispose(true)
    override x.Finalize() = x.Dispose(false)
    member x.Clone() = 
        if isDisposed || Interlocked.Increment(&refCount.contents) = 1 then raise <| System.ObjectDisposedException("Sampler")
        DawnRaw.wgpuSamplerReference(handle)
        new Sampler(device, handle, refCount)
    interface System.IDisposable with
        member x.Dispose() = x.Dispose()
    new(device : Device, handle : SamplerHandle) = new Sampler(device, handle, ref 1)
[<AllowNullLiteral>]
type ShaderModule(device : Device, handle : ShaderModuleHandle, refCount : ref<int>) = 
    let mutable isDisposed = false
    member x.Device = device
    member x.ReferenceCount = !refCount
    member x.Handle = handle
    member x.IsDisposed = isDisposed
    member private x.Dispose(disposing : bool) =
        if not isDisposed then 
            let r = Interlocked.Decrement(&refCount.contents)
            isDisposed <- true
            if disposing then System.GC.SuppressFinalize x
            DawnRaw.wgpuShaderModuleRelease(handle)
    member x.Dispose() = x.Dispose(true)
    override x.Finalize() = x.Dispose(false)
    member x.Clone() = 
        if isDisposed || Interlocked.Increment(&refCount.contents) = 1 then raise <| System.ObjectDisposedException("ShaderModule")
        DawnRaw.wgpuShaderModuleReference(handle)
        new ShaderModule(device, handle, refCount)
    interface System.IDisposable with
        member x.Dispose() = x.Dispose()
    new(device : Device, handle : ShaderModuleHandle) = new ShaderModule(device, handle, ref 1)
[<AllowNullLiteral>]
type Surface(device : Device, handle : SurfaceHandle, refCount : ref<int>) = 
    let mutable isDisposed = false
    member x.Device = device
    member x.ReferenceCount = !refCount
    member x.Handle = handle
    member x.IsDisposed = isDisposed
    member private x.Dispose(disposing : bool) =
        if not isDisposed then 
            let r = Interlocked.Decrement(&refCount.contents)
            isDisposed <- true
            if disposing then System.GC.SuppressFinalize x
            DawnRaw.wgpuSurfaceRelease(handle)
    member x.Dispose() = x.Dispose(true)
    override x.Finalize() = x.Dispose(false)
    member x.Clone() = 
        if isDisposed || Interlocked.Increment(&refCount.contents) = 1 then raise <| System.ObjectDisposedException("Surface")
        DawnRaw.wgpuSurfaceReference(handle)
        new Surface(device, handle, refCount)
    interface System.IDisposable with
        member x.Dispose() = x.Dispose()
    new(device : Device, handle : SurfaceHandle) = new Surface(device, handle, ref 1)
[<AllowNullLiteral>]
type TextureView(device : Device, handle : TextureViewHandle, refCount : ref<int>) = 
    let mutable isDisposed = false
    member x.Device = device
    member x.ReferenceCount = !refCount
    member x.Handle = handle
    member x.IsDisposed = isDisposed
    member private x.Dispose(disposing : bool) =
        if not isDisposed then 
            let r = Interlocked.Decrement(&refCount.contents)
            isDisposed <- true
            if disposing then System.GC.SuppressFinalize x
            DawnRaw.wgpuTextureViewRelease(handle)
    member x.Dispose() = x.Dispose(true)
    override x.Finalize() = x.Dispose(false)
    member x.Clone() = 
        if isDisposed || Interlocked.Increment(&refCount.contents) = 1 then raise <| System.ObjectDisposedException("TextureView")
        DawnRaw.wgpuTextureViewReference(handle)
        new TextureView(device, handle, refCount)
    interface System.IDisposable with
        member x.Dispose() = x.Dispose()
    new(device : Device, handle : TextureViewHandle) = new TextureView(device, handle, ref 1)
[<AllowNullLiteral>]
type ComputePipeline(device : Device, handle : ComputePipelineHandle, refCount : ref<int>) = 
    let mutable isDisposed = false
    member x.Device = device
    member x.ReferenceCount = !refCount
    member x.Handle = handle
    member x.IsDisposed = isDisposed
    member private x.Dispose(disposing : bool) =
        if not isDisposed then 
            let r = Interlocked.Decrement(&refCount.contents)
            isDisposed <- true
            if disposing then System.GC.SuppressFinalize x
            DawnRaw.wgpuComputePipelineRelease(handle)
    member x.Dispose() = x.Dispose(true)
    override x.Finalize() = x.Dispose(false)
    member x.Clone() = 
        if isDisposed || Interlocked.Increment(&refCount.contents) = 1 then raise <| System.ObjectDisposedException("ComputePipeline")
        DawnRaw.wgpuComputePipelineReference(handle)
        new ComputePipeline(device, handle, refCount)
    interface System.IDisposable with
        member x.Dispose() = x.Dispose()
    new(device : Device, handle : ComputePipelineHandle) = new ComputePipeline(device, handle, ref 1)
    member inline x.GetBindGroupLayout(GroupIndex : int) : BindGroupLayout = 
        let _GroupIndex = GroupIndex
        new BindGroupLayout(x.Device, DawnRaw.wgpuComputePipelineGetBindGroupLayout(x.Handle, _GroupIndex))
[<AllowNullLiteral>]
type Instance(device : Device, handle : InstanceHandle, refCount : ref<int>) = 
    let mutable isDisposed = false
    member x.Device = device
    member x.ReferenceCount = !refCount
    member x.Handle = handle
    member x.IsDisposed = isDisposed
    member private x.Dispose(disposing : bool) =
        if not isDisposed then 
            let r = Interlocked.Decrement(&refCount.contents)
            isDisposed <- true
            if disposing then System.GC.SuppressFinalize x
            DawnRaw.wgpuInstanceRelease(handle)
    member x.Dispose() = x.Dispose(true)
    override x.Finalize() = x.Dispose(false)
    member x.Clone() = 
        if isDisposed || Interlocked.Increment(&refCount.contents) = 1 then raise <| System.ObjectDisposedException("Instance")
        DawnRaw.wgpuInstanceReference(handle)
        new Instance(device, handle, refCount)
    interface System.IDisposable with
        member x.Dispose() = x.Dispose()
    new(device : Device, handle : InstanceHandle) = new Instance(device, handle, ref 1)
    member inline x.CreateSurface() : Surface = 
        let inline _LabelCont (_Label) = 
            let mutable _DescriptorValue = Unchecked.defaultof<DawnRaw.WGPUSurfaceDescriptor>
            _DescriptorValue.Next <- 0n
            _DescriptorValue.Label <- _Label
            let _DescriptorValue = _DescriptorValue
            let _Descriptor = NativePtr.stackalloc 1
            NativePtr.write _Descriptor _DescriptorValue
            new Surface(x.Device, DawnRaw.wgpuInstanceCreateSurface(x.Handle, _Descriptor))
        if not (isNull SurfaceDescriptor.Default.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount SurfaceDescriptor.Default.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(SurfaceDescriptor.Default.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
    member inline x.CreateSurface(Descriptor : SurfaceDescriptor) : Surface = 
        let inline _LabelCont (_Label) = 
            let mutable _DescriptorValue = Unchecked.defaultof<DawnRaw.WGPUSurfaceDescriptor>
            _DescriptorValue.Next <- 0n
            _DescriptorValue.Label <- _Label
            let _DescriptorValue = _DescriptorValue
            let _Descriptor = NativePtr.stackalloc 1
            NativePtr.write _Descriptor _DescriptorValue
            new Surface(x.Device, DawnRaw.wgpuInstanceCreateSurface(x.Handle, _Descriptor))
        if not (isNull Descriptor.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount Descriptor.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(Descriptor.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
[<AllowNullLiteral>]
type RenderPipeline(device : Device, handle : RenderPipelineHandle, refCount : ref<int>) = 
    let mutable isDisposed = false
    member x.Device = device
    member x.ReferenceCount = !refCount
    member x.Handle = handle
    member x.IsDisposed = isDisposed
    member private x.Dispose(disposing : bool) =
        if not isDisposed then 
            let r = Interlocked.Decrement(&refCount.contents)
            isDisposed <- true
            if disposing then System.GC.SuppressFinalize x
            DawnRaw.wgpuRenderPipelineRelease(handle)
    member x.Dispose() = x.Dispose(true)
    override x.Finalize() = x.Dispose(false)
    member x.Clone() = 
        if isDisposed || Interlocked.Increment(&refCount.contents) = 1 then raise <| System.ObjectDisposedException("RenderPipeline")
        DawnRaw.wgpuRenderPipelineReference(handle)
        new RenderPipeline(device, handle, refCount)
    interface System.IDisposable with
        member x.Dispose() = x.Dispose()
    new(device : Device, handle : RenderPipelineHandle) = new RenderPipeline(device, handle, ref 1)
    member inline x.GetBindGroupLayout(GroupIndex : int) : BindGroupLayout = 
        let _GroupIndex = GroupIndex
        new BindGroupLayout(x.Device, DawnRaw.wgpuRenderPipelineGetBindGroupLayout(x.Handle, _GroupIndex))
[<AllowNullLiteral>]
type SwapChain(device : Device, handle : SwapChainHandle, refCount : ref<int>) = 
    let mutable isDisposed = false
    member x.Device = device
    member x.ReferenceCount = !refCount
    member x.Handle = handle
    member x.IsDisposed = isDisposed
    member private x.Dispose(disposing : bool) =
        if not isDisposed then 
            let r = Interlocked.Decrement(&refCount.contents)
            isDisposed <- true
            if disposing then System.GC.SuppressFinalize x
            DawnRaw.wgpuSwapChainRelease(handle)
    member x.Dispose() = x.Dispose(true)
    override x.Finalize() = x.Dispose(false)
    member x.Clone() = 
        if isDisposed || Interlocked.Increment(&refCount.contents) = 1 then raise <| System.ObjectDisposedException("SwapChain")
        DawnRaw.wgpuSwapChainReference(handle)
        new SwapChain(device, handle, refCount)
    interface System.IDisposable with
        member x.Dispose() = x.Dispose()
    new(device : Device, handle : SwapChainHandle) = new SwapChain(device, handle, ref 1)
    member inline x.Configure(Format : TextureFormat, AllowedUsage : TextureUsage, Width : int, Height : int) : unit = 
        let _Format = Format
        let _AllowedUsage = AllowedUsage
        let _Width = Width
        let _Height = Height
        DawnRaw.wgpuSwapChainConfigure(x.Handle, _Format, _AllowedUsage, _Width, _Height)
    member inline x.GetCurrentTextureView() : TextureView = 
        new TextureView(x.Device, DawnRaw.wgpuSwapChainGetCurrentTextureView(x.Handle))
    member inline x.Present() : unit = 
        DawnRaw.wgpuSwapChainPresent(x.Handle)
[<AllowNullLiteral>]
type Buffer(device : Device, handle : BufferHandle, refCount : ref<int>) = 
    let mutable isDisposed = false
    member x.Device = device
    member x.ReferenceCount = !refCount
    member x.Handle = handle
    member x.IsDisposed = isDisposed
    member private x.Dispose(disposing : bool) =
        if not isDisposed then 
            let r = Interlocked.Decrement(&refCount.contents)
            isDisposed <- true
            if disposing then System.GC.SuppressFinalize x
            if r = 0 then DawnRaw.wgpuBufferDestroy(handle)
            DawnRaw.wgpuBufferRelease(handle)
    member x.Dispose() = x.Dispose(true)
    override x.Finalize() = x.Dispose(false)
    member x.Clone() = 
        if isDisposed || Interlocked.Increment(&refCount.contents) = 1 then raise <| System.ObjectDisposedException("Buffer")
        DawnRaw.wgpuBufferReference(handle)
        new Buffer(device, handle, refCount)
    interface System.IDisposable with
        member x.Dispose() = x.Dispose()
    new(device : Device, handle : BufferHandle) = new Buffer(device, handle, ref 1)
    member inline x.MapAsync(Mode : MapMode, Offset : unativeint, Size : unativeint, Callback : BufferMapCallback) : unit = 
        let _Mode = Mode
        let _Offset = Offset
        let _Size = Size
        let mutable _CallbackGC = Unchecked.defaultof<System.Runtime.InteropServices.GCHandle>
        let _CallbackFunction (Status : BufferMapAsyncStatus) (Userdata : nativeint) = 
            let _Status = Status
            let _Userdata = Userdata
            if _CallbackGC.IsAllocated then _CallbackGC.Free()
            Callback.Invoke(_Status, _Userdata)
        let _CallbackDel = WGPUBufferMapCallback(_CallbackFunction)
        _CallbackGC <- System.Runtime.InteropServices.GCHandle.Alloc(_CallbackDel)
        let _Callback = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(_CallbackDel)
        let _Userdata = 0n
        DawnRaw.wgpuBufferMapAsync(x.Handle, _Mode, _Offset, _Size, _Callback, _Userdata)
    member inline x.MapAsync(Mode : MapMode, Offset : unativeint, Size : unativeint, Callback : BufferMapCallback, Userdata : nativeint) : unit = 
        let _Mode = Mode
        let _Offset = Offset
        let _Size = Size
        let mutable _CallbackGC = Unchecked.defaultof<System.Runtime.InteropServices.GCHandle>
        let _CallbackFunction (Status : BufferMapAsyncStatus) (Userdata : nativeint) = 
            let _Status = Status
            let _Userdata = Userdata
            if _CallbackGC.IsAllocated then _CallbackGC.Free()
            Callback.Invoke(_Status, _Userdata)
        let _CallbackDel = WGPUBufferMapCallback(_CallbackFunction)
        _CallbackGC <- System.Runtime.InteropServices.GCHandle.Alloc(_CallbackDel)
        let _Callback = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(_CallbackDel)
        let _Userdata = Userdata
        DawnRaw.wgpuBufferMapAsync(x.Handle, _Mode, _Offset, _Size, _Callback, _Userdata)
    member inline x.GetMappedRange() : nativeint = 
        let _Offset = 0un
        let _Size = 0un
        DawnRaw.wgpuBufferGetMappedRange(x.Handle, _Offset, _Size)
    member inline x.GetMappedRange(Offset : unativeint) : nativeint = 
        let _Offset = Offset
        let _Size = 0un
        DawnRaw.wgpuBufferGetMappedRange(x.Handle, _Offset, _Size)
    member inline x.GetMappedRange(Offset : unativeint, Size : unativeint) : nativeint = 
        let _Offset = Offset
        let _Size = Size
        DawnRaw.wgpuBufferGetMappedRange(x.Handle, _Offset, _Size)
    member inline x.GetConstMappedRange() : nativeint = 
        let _Offset = 0un
        let _Size = 0un
        DawnRaw.wgpuBufferGetConstMappedRange(x.Handle, _Offset, _Size)
    member inline x.GetConstMappedRange(Offset : unativeint) : nativeint = 
        let _Offset = Offset
        let _Size = 0un
        DawnRaw.wgpuBufferGetConstMappedRange(x.Handle, _Offset, _Size)
    member inline x.GetConstMappedRange(Offset : unativeint, Size : unativeint) : nativeint = 
        let _Offset = Offset
        let _Size = Size
        DawnRaw.wgpuBufferGetConstMappedRange(x.Handle, _Offset, _Size)
    member inline x.Unmap() : unit = 
        DawnRaw.wgpuBufferUnmap(x.Handle)
[<AllowNullLiteral>]
type Fence(device : Device, handle : FenceHandle, refCount : ref<int>) = 
    let mutable isDisposed = false
    member x.Device = device
    member x.ReferenceCount = !refCount
    member x.Handle = handle
    member x.IsDisposed = isDisposed
    member private x.Dispose(disposing : bool) =
        if not isDisposed then 
            let r = Interlocked.Decrement(&refCount.contents)
            isDisposed <- true
            if disposing then System.GC.SuppressFinalize x
            DawnRaw.wgpuFenceRelease(handle)
    member x.Dispose() = x.Dispose(true)
    override x.Finalize() = x.Dispose(false)
    member x.Clone() = 
        if isDisposed || Interlocked.Increment(&refCount.contents) = 1 then raise <| System.ObjectDisposedException("Fence")
        DawnRaw.wgpuFenceReference(handle)
        new Fence(device, handle, refCount)
    interface System.IDisposable with
        member x.Dispose() = x.Dispose()
    new(device : Device, handle : FenceHandle) = new Fence(device, handle, ref 1)
    member inline x.GetCompletedValue() : uint64 = 
        DawnRaw.wgpuFenceGetCompletedValue(x.Handle)
    member inline x.OnCompletion(Value : uint64, Callback : FenceOnCompletionCallback) : unit = 
        let _Value = Value
        let mutable _CallbackGC = Unchecked.defaultof<System.Runtime.InteropServices.GCHandle>
        let _CallbackFunction (Status : FenceCompletionStatus) (Userdata : nativeint) = 
            let _Status = Status
            let _Userdata = Userdata
            if _CallbackGC.IsAllocated then _CallbackGC.Free()
            Callback.Invoke(_Status, _Userdata)
        let _CallbackDel = WGPUFenceOnCompletionCallback(_CallbackFunction)
        _CallbackGC <- System.Runtime.InteropServices.GCHandle.Alloc(_CallbackDel)
        let _Callback = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(_CallbackDel)
        let _Userdata = 0n
        DawnRaw.wgpuFenceOnCompletion(x.Handle, _Value, _Callback, _Userdata)
    member inline x.OnCompletion(Value : uint64, Callback : FenceOnCompletionCallback, Userdata : nativeint) : unit = 
        let _Value = Value
        let mutable _CallbackGC = Unchecked.defaultof<System.Runtime.InteropServices.GCHandle>
        let _CallbackFunction (Status : FenceCompletionStatus) (Userdata : nativeint) = 
            let _Status = Status
            let _Userdata = Userdata
            if _CallbackGC.IsAllocated then _CallbackGC.Free()
            Callback.Invoke(_Status, _Userdata)
        let _CallbackDel = WGPUFenceOnCompletionCallback(_CallbackFunction)
        _CallbackGC <- System.Runtime.InteropServices.GCHandle.Alloc(_CallbackDel)
        let _Callback = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(_CallbackDel)
        let _Userdata = Userdata
        DawnRaw.wgpuFenceOnCompletion(x.Handle, _Value, _Callback, _Userdata)
[<AllowNullLiteral>]
type Texture(device : Device, handle : TextureHandle, refCount : ref<int>) = 
    let mutable isDisposed = false
    member x.Device = device
    member x.ReferenceCount = !refCount
    member x.Handle = handle
    member x.IsDisposed = isDisposed
    member private x.Dispose(disposing : bool) =
        if not isDisposed then 
            let r = Interlocked.Decrement(&refCount.contents)
            isDisposed <- true
            if disposing then System.GC.SuppressFinalize x
            if r = 0 then DawnRaw.wgpuTextureDestroy(handle)
            DawnRaw.wgpuTextureRelease(handle)
    member x.Dispose() = x.Dispose(true)
    override x.Finalize() = x.Dispose(false)
    member x.Clone() = 
        if isDisposed || Interlocked.Increment(&refCount.contents) = 1 then raise <| System.ObjectDisposedException("Texture")
        DawnRaw.wgpuTextureReference(handle)
        new Texture(device, handle, refCount)
    interface System.IDisposable with
        member x.Dispose() = x.Dispose()
    new(device : Device, handle : TextureHandle) = new Texture(device, handle, ref 1)
    member inline x.CreateView() : TextureView = 
        let inline _LabelCont (_Label) = 
            let _Format = TextureViewDescriptor.Default.Format
            let _Dimension = TextureViewDescriptor.Default.Dimension
            let _BaseMipLevel = TextureViewDescriptor.Default.BaseMipLevel
            let _MipLevelCount = TextureViewDescriptor.Default.MipLevelCount
            let _BaseArrayLayer = TextureViewDescriptor.Default.BaseArrayLayer
            let _ArrayLayerCount = TextureViewDescriptor.Default.ArrayLayerCount
            let _Aspect = TextureViewDescriptor.Default.Aspect
            let mutable _DescriptorValue = Unchecked.defaultof<DawnRaw.WGPUTextureViewDescriptor>
            _DescriptorValue.Next <- 0n
            _DescriptorValue.Label <- _Label
            _DescriptorValue.Format <- _Format
            _DescriptorValue.Dimension <- _Dimension
            _DescriptorValue.BaseMipLevel <- _BaseMipLevel
            _DescriptorValue.MipLevelCount <- _MipLevelCount
            _DescriptorValue.BaseArrayLayer <- _BaseArrayLayer
            _DescriptorValue.ArrayLayerCount <- _ArrayLayerCount
            _DescriptorValue.Aspect <- _Aspect
            let _DescriptorValue = _DescriptorValue
            let _Descriptor = NativePtr.stackalloc 1
            NativePtr.write _Descriptor _DescriptorValue
            new TextureView(x.Device, DawnRaw.wgpuTextureCreateView(x.Handle, _Descriptor))
        if not (isNull TextureViewDescriptor.Default.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount TextureViewDescriptor.Default.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(TextureViewDescriptor.Default.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
    member inline x.CreateView(Descriptor : TextureViewDescriptor) : TextureView = 
        let inline _LabelCont (_Label) = 
            let _Format = Descriptor.Format
            let _Dimension = Descriptor.Dimension
            let _BaseMipLevel = Descriptor.BaseMipLevel
            let _MipLevelCount = Descriptor.MipLevelCount
            let _BaseArrayLayer = Descriptor.BaseArrayLayer
            let _ArrayLayerCount = Descriptor.ArrayLayerCount
            let _Aspect = Descriptor.Aspect
            let mutable _DescriptorValue = Unchecked.defaultof<DawnRaw.WGPUTextureViewDescriptor>
            _DescriptorValue.Next <- 0n
            _DescriptorValue.Label <- _Label
            _DescriptorValue.Format <- _Format
            _DescriptorValue.Dimension <- _Dimension
            _DescriptorValue.BaseMipLevel <- _BaseMipLevel
            _DescriptorValue.MipLevelCount <- _MipLevelCount
            _DescriptorValue.BaseArrayLayer <- _BaseArrayLayer
            _DescriptorValue.ArrayLayerCount <- _ArrayLayerCount
            _DescriptorValue.Aspect <- _Aspect
            let _DescriptorValue = _DescriptorValue
            let _Descriptor = NativePtr.stackalloc 1
            NativePtr.write _Descriptor _DescriptorValue
            new TextureView(x.Device, DawnRaw.wgpuTextureCreateView(x.Handle, _Descriptor))
        if not (isNull Descriptor.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount Descriptor.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(Descriptor.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
[<AllowNullLiteral>]
type ComputePassEncoder(device : Device, handle : ComputePassEncoderHandle, refCount : ref<int>) = 
    let mutable isDisposed = false
    member x.Device = device
    member x.ReferenceCount = !refCount
    member x.Handle = handle
    member x.IsDisposed = isDisposed
    member private x.Dispose(disposing : bool) =
        if not isDisposed then 
            let r = Interlocked.Decrement(&refCount.contents)
            isDisposed <- true
            if disposing then System.GC.SuppressFinalize x
            DawnRaw.wgpuComputePassEncoderRelease(handle)
    member x.Dispose() = x.Dispose(true)
    override x.Finalize() = x.Dispose(false)
    member x.Clone() = 
        if isDisposed || Interlocked.Increment(&refCount.contents) = 1 then raise <| System.ObjectDisposedException("ComputePassEncoder")
        DawnRaw.wgpuComputePassEncoderReference(handle)
        new ComputePassEncoder(device, handle, refCount)
    interface System.IDisposable with
        member x.Dispose() = x.Dispose()
    new(device : Device, handle : ComputePassEncoderHandle) = new ComputePassEncoder(device, handle, ref 1)
    member inline x.InsertDebugMarker(MarkerLabel : string) : unit = 
        let inline _MarkerLabelCont (_MarkerLabel) = 
            DawnRaw.wgpuComputePassEncoderInsertDebugMarker(x.Handle, _MarkerLabel)
        if not (isNull MarkerLabel) then
            let _MarkerLabelLen = System.Text.Encoding.UTF8.GetByteCount MarkerLabel
            let _MarkerLabelSize = _MarkerLabelLen + 1
            let _MarkerLabelPtr = NativePtr.stackalloc<byte> _MarkerLabelSize
            System.Text.Encoding.UTF8.GetBytes(MarkerLabel.AsSpan(), Span<byte>(NativePtr.toVoidPtr _MarkerLabelPtr, _MarkerLabelSize)) |> ignore
            NativePtr.set _MarkerLabelPtr _MarkerLabelLen 0uy
            _MarkerLabelCont (NativePtr.toNativeInt _MarkerLabelPtr)
        else
            _MarkerLabelCont 0n
    member inline x.PopDebugGroup() : unit = 
        DawnRaw.wgpuComputePassEncoderPopDebugGroup(x.Handle)
    member inline x.PushDebugGroup(GroupLabel : string) : unit = 
        let inline _GroupLabelCont (_GroupLabel) = 
            DawnRaw.wgpuComputePassEncoderPushDebugGroup(x.Handle, _GroupLabel)
        if not (isNull GroupLabel) then
            let _GroupLabelLen = System.Text.Encoding.UTF8.GetByteCount GroupLabel
            let _GroupLabelSize = _GroupLabelLen + 1
            let _GroupLabelPtr = NativePtr.stackalloc<byte> _GroupLabelSize
            System.Text.Encoding.UTF8.GetBytes(GroupLabel.AsSpan(), Span<byte>(NativePtr.toVoidPtr _GroupLabelPtr, _GroupLabelSize)) |> ignore
            NativePtr.set _GroupLabelPtr _GroupLabelLen 0uy
            _GroupLabelCont (NativePtr.toNativeInt _GroupLabelPtr)
        else
            _GroupLabelCont 0n
    member inline x.SetPipeline(Pipeline : ComputePipeline) : unit = 
        let _Pipeline = (if isNull Pipeline then ComputePipelineHandle.Null else Pipeline.Handle)
        DawnRaw.wgpuComputePassEncoderSetPipeline(x.Handle, _Pipeline)
    member inline x.SetBindGroup(GroupIndex : int, Group : BindGroup, DynamicOffsets : array<int>) : unit = 
        let _GroupIndex = GroupIndex
        let _Group = (if isNull Group then BindGroupHandle.Null else Group.Handle)
        use _DynamicOffsets = fixed DynamicOffsets
        let _DynamicOffsetsCount = DynamicOffsets.Length
        DawnRaw.wgpuComputePassEncoderSetBindGroup(x.Handle, _GroupIndex, _Group, _DynamicOffsetsCount, _DynamicOffsets)
    member inline x.WriteTimestamp(QuerySet : QuerySet, QueryIndex : int) : unit = 
        let _QuerySet = (if isNull QuerySet then QuerySetHandle.Null else QuerySet.Handle)
        let _QueryIndex = QueryIndex
        DawnRaw.wgpuComputePassEncoderWriteTimestamp(x.Handle, _QuerySet, _QueryIndex)
    member inline x.Dispatch(X : int) : unit = 
        let _X = X
        let _Y = 1
        let _Z = 1
        DawnRaw.wgpuComputePassEncoderDispatch(x.Handle, _X, _Y, _Z)
    member inline x.Dispatch(X : int, Y : int) : unit = 
        let _X = X
        let _Y = Y
        let _Z = 1
        DawnRaw.wgpuComputePassEncoderDispatch(x.Handle, _X, _Y, _Z)
    member inline x.Dispatch(X : int, Y : int, Z : int) : unit = 
        let _X = X
        let _Y = Y
        let _Z = Z
        DawnRaw.wgpuComputePassEncoderDispatch(x.Handle, _X, _Y, _Z)
    member inline x.DispatchIndirect(IndirectBuffer : Buffer, IndirectOffset : uint64) : unit = 
        let _IndirectBuffer = (if isNull IndirectBuffer then BufferHandle.Null else IndirectBuffer.Handle)
        let _IndirectOffset = IndirectOffset
        DawnRaw.wgpuComputePassEncoderDispatchIndirect(x.Handle, _IndirectBuffer, _IndirectOffset)
    member inline x.EndPass() : unit = 
        DawnRaw.wgpuComputePassEncoderEndPass(x.Handle)
[<AllowNullLiteral>]
type RenderBundleEncoder(device : Device, handle : RenderBundleEncoderHandle, refCount : ref<int>) = 
    let mutable isDisposed = false
    member x.Device = device
    member x.ReferenceCount = !refCount
    member x.Handle = handle
    member x.IsDisposed = isDisposed
    member private x.Dispose(disposing : bool) =
        if not isDisposed then 
            let r = Interlocked.Decrement(&refCount.contents)
            isDisposed <- true
            if disposing then System.GC.SuppressFinalize x
            DawnRaw.wgpuRenderBundleEncoderRelease(handle)
    member x.Dispose() = x.Dispose(true)
    override x.Finalize() = x.Dispose(false)
    member x.Clone() = 
        if isDisposed || Interlocked.Increment(&refCount.contents) = 1 then raise <| System.ObjectDisposedException("RenderBundleEncoder")
        DawnRaw.wgpuRenderBundleEncoderReference(handle)
        new RenderBundleEncoder(device, handle, refCount)
    interface System.IDisposable with
        member x.Dispose() = x.Dispose()
    new(device : Device, handle : RenderBundleEncoderHandle) = new RenderBundleEncoder(device, handle, ref 1)
    member inline x.SetPipeline(Pipeline : RenderPipeline) : unit = 
        let _Pipeline = (if isNull Pipeline then RenderPipelineHandle.Null else Pipeline.Handle)
        DawnRaw.wgpuRenderBundleEncoderSetPipeline(x.Handle, _Pipeline)
    member inline x.SetBindGroup(GroupIndex : int, Group : BindGroup, DynamicOffsets : array<int>) : unit = 
        let _GroupIndex = GroupIndex
        let _Group = (if isNull Group then BindGroupHandle.Null else Group.Handle)
        use _DynamicOffsets = fixed DynamicOffsets
        let _DynamicOffsetsCount = DynamicOffsets.Length
        DawnRaw.wgpuRenderBundleEncoderSetBindGroup(x.Handle, _GroupIndex, _Group, _DynamicOffsetsCount, _DynamicOffsets)
    member inline x.Draw(VertexCount : int) : unit = 
        let _VertexCount = VertexCount
        let _InstanceCount = 1
        let _FirstVertex = 0
        let _FirstInstance = 0
        DawnRaw.wgpuRenderBundleEncoderDraw(x.Handle, _VertexCount, _InstanceCount, _FirstVertex, _FirstInstance)
    member inline x.Draw(VertexCount : int, InstanceCount : int) : unit = 
        let _VertexCount = VertexCount
        let _InstanceCount = InstanceCount
        let _FirstVertex = 0
        let _FirstInstance = 0
        DawnRaw.wgpuRenderBundleEncoderDraw(x.Handle, _VertexCount, _InstanceCount, _FirstVertex, _FirstInstance)
    member inline x.Draw(VertexCount : int, InstanceCount : int, FirstVertex : int) : unit = 
        let _VertexCount = VertexCount
        let _InstanceCount = InstanceCount
        let _FirstVertex = FirstVertex
        let _FirstInstance = 0
        DawnRaw.wgpuRenderBundleEncoderDraw(x.Handle, _VertexCount, _InstanceCount, _FirstVertex, _FirstInstance)
    member inline x.Draw(VertexCount : int, InstanceCount : int, FirstVertex : int, FirstInstance : int) : unit = 
        let _VertexCount = VertexCount
        let _InstanceCount = InstanceCount
        let _FirstVertex = FirstVertex
        let _FirstInstance = FirstInstance
        DawnRaw.wgpuRenderBundleEncoderDraw(x.Handle, _VertexCount, _InstanceCount, _FirstVertex, _FirstInstance)
    member inline x.DrawIndexed(IndexCount : int) : unit = 
        let _IndexCount = IndexCount
        let _InstanceCount = 1
        let _FirstIndex = 0
        let _BaseVertex = 0
        let _FirstInstance = 0
        DawnRaw.wgpuRenderBundleEncoderDrawIndexed(x.Handle, _IndexCount, _InstanceCount, _FirstIndex, _BaseVertex, _FirstInstance)
    member inline x.DrawIndexed(IndexCount : int, InstanceCount : int) : unit = 
        let _IndexCount = IndexCount
        let _InstanceCount = InstanceCount
        let _FirstIndex = 0
        let _BaseVertex = 0
        let _FirstInstance = 0
        DawnRaw.wgpuRenderBundleEncoderDrawIndexed(x.Handle, _IndexCount, _InstanceCount, _FirstIndex, _BaseVertex, _FirstInstance)
    member inline x.DrawIndexed(IndexCount : int, InstanceCount : int, FirstIndex : int) : unit = 
        let _IndexCount = IndexCount
        let _InstanceCount = InstanceCount
        let _FirstIndex = FirstIndex
        let _BaseVertex = 0
        let _FirstInstance = 0
        DawnRaw.wgpuRenderBundleEncoderDrawIndexed(x.Handle, _IndexCount, _InstanceCount, _FirstIndex, _BaseVertex, _FirstInstance)
    member inline x.DrawIndexed(IndexCount : int, InstanceCount : int, FirstIndex : int, BaseVertex : int32) : unit = 
        let _IndexCount = IndexCount
        let _InstanceCount = InstanceCount
        let _FirstIndex = FirstIndex
        let _BaseVertex = BaseVertex
        let _FirstInstance = 0
        DawnRaw.wgpuRenderBundleEncoderDrawIndexed(x.Handle, _IndexCount, _InstanceCount, _FirstIndex, _BaseVertex, _FirstInstance)
    member inline x.DrawIndexed(IndexCount : int, InstanceCount : int, FirstIndex : int, BaseVertex : int32, FirstInstance : int) : unit = 
        let _IndexCount = IndexCount
        let _InstanceCount = InstanceCount
        let _FirstIndex = FirstIndex
        let _BaseVertex = BaseVertex
        let _FirstInstance = FirstInstance
        DawnRaw.wgpuRenderBundleEncoderDrawIndexed(x.Handle, _IndexCount, _InstanceCount, _FirstIndex, _BaseVertex, _FirstInstance)
    member inline x.DrawIndirect(IndirectBuffer : Buffer, IndirectOffset : uint64) : unit = 
        let _IndirectBuffer = (if isNull IndirectBuffer then BufferHandle.Null else IndirectBuffer.Handle)
        let _IndirectOffset = IndirectOffset
        DawnRaw.wgpuRenderBundleEncoderDrawIndirect(x.Handle, _IndirectBuffer, _IndirectOffset)
    member inline x.DrawIndexedIndirect(IndirectBuffer : Buffer, IndirectOffset : uint64) : unit = 
        let _IndirectBuffer = (if isNull IndirectBuffer then BufferHandle.Null else IndirectBuffer.Handle)
        let _IndirectOffset = IndirectOffset
        DawnRaw.wgpuRenderBundleEncoderDrawIndexedIndirect(x.Handle, _IndirectBuffer, _IndirectOffset)
    member inline x.InsertDebugMarker(MarkerLabel : string) : unit = 
        let inline _MarkerLabelCont (_MarkerLabel) = 
            DawnRaw.wgpuRenderBundleEncoderInsertDebugMarker(x.Handle, _MarkerLabel)
        if not (isNull MarkerLabel) then
            let _MarkerLabelLen = System.Text.Encoding.UTF8.GetByteCount MarkerLabel
            let _MarkerLabelSize = _MarkerLabelLen + 1
            let _MarkerLabelPtr = NativePtr.stackalloc<byte> _MarkerLabelSize
            System.Text.Encoding.UTF8.GetBytes(MarkerLabel.AsSpan(), Span<byte>(NativePtr.toVoidPtr _MarkerLabelPtr, _MarkerLabelSize)) |> ignore
            NativePtr.set _MarkerLabelPtr _MarkerLabelLen 0uy
            _MarkerLabelCont (NativePtr.toNativeInt _MarkerLabelPtr)
        else
            _MarkerLabelCont 0n
    member inline x.PopDebugGroup() : unit = 
        DawnRaw.wgpuRenderBundleEncoderPopDebugGroup(x.Handle)
    member inline x.PushDebugGroup(GroupLabel : string) : unit = 
        let inline _GroupLabelCont (_GroupLabel) = 
            DawnRaw.wgpuRenderBundleEncoderPushDebugGroup(x.Handle, _GroupLabel)
        if not (isNull GroupLabel) then
            let _GroupLabelLen = System.Text.Encoding.UTF8.GetByteCount GroupLabel
            let _GroupLabelSize = _GroupLabelLen + 1
            let _GroupLabelPtr = NativePtr.stackalloc<byte> _GroupLabelSize
            System.Text.Encoding.UTF8.GetBytes(GroupLabel.AsSpan(), Span<byte>(NativePtr.toVoidPtr _GroupLabelPtr, _GroupLabelSize)) |> ignore
            NativePtr.set _GroupLabelPtr _GroupLabelLen 0uy
            _GroupLabelCont (NativePtr.toNativeInt _GroupLabelPtr)
        else
            _GroupLabelCont 0n
    member inline x.SetVertexBuffer(Slot : int, Buffer : Buffer) : unit = 
        let _Slot = Slot
        let _Buffer = (if isNull Buffer then BufferHandle.Null else Buffer.Handle)
        let _Offset = 0UL
        let _Size = 0UL
        DawnRaw.wgpuRenderBundleEncoderSetVertexBuffer(x.Handle, _Slot, _Buffer, _Offset, _Size)
    member inline x.SetVertexBuffer(Slot : int, Buffer : Buffer, Offset : uint64) : unit = 
        let _Slot = Slot
        let _Buffer = (if isNull Buffer then BufferHandle.Null else Buffer.Handle)
        let _Offset = Offset
        let _Size = 0UL
        DawnRaw.wgpuRenderBundleEncoderSetVertexBuffer(x.Handle, _Slot, _Buffer, _Offset, _Size)
    member inline x.SetVertexBuffer(Slot : int, Buffer : Buffer, Offset : uint64, Size : uint64) : unit = 
        let _Slot = Slot
        let _Buffer = (if isNull Buffer then BufferHandle.Null else Buffer.Handle)
        let _Offset = Offset
        let _Size = Size
        DawnRaw.wgpuRenderBundleEncoderSetVertexBuffer(x.Handle, _Slot, _Buffer, _Offset, _Size)
    member inline x.SetIndexBuffer(Buffer : Buffer) : unit = 
        let _Buffer = (if isNull Buffer then BufferHandle.Null else Buffer.Handle)
        let _Offset = 0UL
        let _Size = 0UL
        DawnRaw.wgpuRenderBundleEncoderSetIndexBuffer(x.Handle, _Buffer, _Offset, _Size)
    member inline x.SetIndexBuffer(Buffer : Buffer, Offset : uint64) : unit = 
        let _Buffer = (if isNull Buffer then BufferHandle.Null else Buffer.Handle)
        let _Offset = Offset
        let _Size = 0UL
        DawnRaw.wgpuRenderBundleEncoderSetIndexBuffer(x.Handle, _Buffer, _Offset, _Size)
    member inline x.SetIndexBuffer(Buffer : Buffer, Offset : uint64, Size : uint64) : unit = 
        let _Buffer = (if isNull Buffer then BufferHandle.Null else Buffer.Handle)
        let _Offset = Offset
        let _Size = Size
        DawnRaw.wgpuRenderBundleEncoderSetIndexBuffer(x.Handle, _Buffer, _Offset, _Size)
    member inline x.SetIndexBufferWithFormat(Buffer : Buffer, Format : IndexFormat) : unit = 
        let _Buffer = (if isNull Buffer then BufferHandle.Null else Buffer.Handle)
        let _Format = Format
        let _Offset = 0UL
        let _Size = 0UL
        DawnRaw.wgpuRenderBundleEncoderSetIndexBufferWithFormat(x.Handle, _Buffer, _Format, _Offset, _Size)
    member inline x.SetIndexBufferWithFormat(Buffer : Buffer, Format : IndexFormat, Offset : uint64) : unit = 
        let _Buffer = (if isNull Buffer then BufferHandle.Null else Buffer.Handle)
        let _Format = Format
        let _Offset = Offset
        let _Size = 0UL
        DawnRaw.wgpuRenderBundleEncoderSetIndexBufferWithFormat(x.Handle, _Buffer, _Format, _Offset, _Size)
    member inline x.SetIndexBufferWithFormat(Buffer : Buffer, Format : IndexFormat, Offset : uint64, Size : uint64) : unit = 
        let _Buffer = (if isNull Buffer then BufferHandle.Null else Buffer.Handle)
        let _Format = Format
        let _Offset = Offset
        let _Size = Size
        DawnRaw.wgpuRenderBundleEncoderSetIndexBufferWithFormat(x.Handle, _Buffer, _Format, _Offset, _Size)
    member inline x.Finish() : RenderBundle = 
        let inline _LabelCont (_Label) = 
            let mutable _DescriptorValue = Unchecked.defaultof<DawnRaw.WGPURenderBundleDescriptor>
            _DescriptorValue.Next <- 0n
            _DescriptorValue.Label <- _Label
            let _DescriptorValue = _DescriptorValue
            let _Descriptor = NativePtr.stackalloc 1
            NativePtr.write _Descriptor _DescriptorValue
            new RenderBundle(x.Device, DawnRaw.wgpuRenderBundleEncoderFinish(x.Handle, _Descriptor))
        if not (isNull RenderBundleDescriptor.Default.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount RenderBundleDescriptor.Default.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(RenderBundleDescriptor.Default.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
    member inline x.Finish(Descriptor : RenderBundleDescriptor) : RenderBundle = 
        let inline _LabelCont (_Label) = 
            let mutable _DescriptorValue = Unchecked.defaultof<DawnRaw.WGPURenderBundleDescriptor>
            _DescriptorValue.Next <- 0n
            _DescriptorValue.Label <- _Label
            let _DescriptorValue = _DescriptorValue
            let _Descriptor = NativePtr.stackalloc 1
            NativePtr.write _Descriptor _DescriptorValue
            new RenderBundle(x.Device, DawnRaw.wgpuRenderBundleEncoderFinish(x.Handle, _Descriptor))
        if not (isNull Descriptor.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount Descriptor.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(Descriptor.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
[<AllowNullLiteral>]
type RenderPassEncoder(device : Device, handle : RenderPassEncoderHandle, refCount : ref<int>) = 
    let mutable isDisposed = false
    member x.Device = device
    member x.ReferenceCount = !refCount
    member x.Handle = handle
    member x.IsDisposed = isDisposed
    member private x.Dispose(disposing : bool) =
        if not isDisposed then 
            let r = Interlocked.Decrement(&refCount.contents)
            isDisposed <- true
            if disposing then System.GC.SuppressFinalize x
            DawnRaw.wgpuRenderPassEncoderRelease(handle)
    member x.Dispose() = x.Dispose(true)
    override x.Finalize() = x.Dispose(false)
    member x.Clone() = 
        if isDisposed || Interlocked.Increment(&refCount.contents) = 1 then raise <| System.ObjectDisposedException("RenderPassEncoder")
        DawnRaw.wgpuRenderPassEncoderReference(handle)
        new RenderPassEncoder(device, handle, refCount)
    interface System.IDisposable with
        member x.Dispose() = x.Dispose()
    new(device : Device, handle : RenderPassEncoderHandle) = new RenderPassEncoder(device, handle, ref 1)
    member inline x.SetPipeline(Pipeline : RenderPipeline) : unit = 
        let _Pipeline = (if isNull Pipeline then RenderPipelineHandle.Null else Pipeline.Handle)
        DawnRaw.wgpuRenderPassEncoderSetPipeline(x.Handle, _Pipeline)
    member inline x.SetBindGroup(GroupIndex : int, Group : BindGroup, DynamicOffsets : array<int>) : unit = 
        let _GroupIndex = GroupIndex
        let _Group = (if isNull Group then BindGroupHandle.Null else Group.Handle)
        use _DynamicOffsets = fixed DynamicOffsets
        let _DynamicOffsetsCount = DynamicOffsets.Length
        DawnRaw.wgpuRenderPassEncoderSetBindGroup(x.Handle, _GroupIndex, _Group, _DynamicOffsetsCount, _DynamicOffsets)
    member inline x.Draw(VertexCount : int) : unit = 
        let _VertexCount = VertexCount
        let _InstanceCount = 1
        let _FirstVertex = 0
        let _FirstInstance = 0
        DawnRaw.wgpuRenderPassEncoderDraw(x.Handle, _VertexCount, _InstanceCount, _FirstVertex, _FirstInstance)
    member inline x.Draw(VertexCount : int, InstanceCount : int) : unit = 
        let _VertexCount = VertexCount
        let _InstanceCount = InstanceCount
        let _FirstVertex = 0
        let _FirstInstance = 0
        DawnRaw.wgpuRenderPassEncoderDraw(x.Handle, _VertexCount, _InstanceCount, _FirstVertex, _FirstInstance)
    member inline x.Draw(VertexCount : int, InstanceCount : int, FirstVertex : int) : unit = 
        let _VertexCount = VertexCount
        let _InstanceCount = InstanceCount
        let _FirstVertex = FirstVertex
        let _FirstInstance = 0
        DawnRaw.wgpuRenderPassEncoderDraw(x.Handle, _VertexCount, _InstanceCount, _FirstVertex, _FirstInstance)
    member inline x.Draw(VertexCount : int, InstanceCount : int, FirstVertex : int, FirstInstance : int) : unit = 
        let _VertexCount = VertexCount
        let _InstanceCount = InstanceCount
        let _FirstVertex = FirstVertex
        let _FirstInstance = FirstInstance
        DawnRaw.wgpuRenderPassEncoderDraw(x.Handle, _VertexCount, _InstanceCount, _FirstVertex, _FirstInstance)
    member inline x.DrawIndexed(IndexCount : int) : unit = 
        let _IndexCount = IndexCount
        let _InstanceCount = 1
        let _FirstIndex = 0
        let _BaseVertex = 0
        let _FirstInstance = 0
        DawnRaw.wgpuRenderPassEncoderDrawIndexed(x.Handle, _IndexCount, _InstanceCount, _FirstIndex, _BaseVertex, _FirstInstance)
    member inline x.DrawIndexed(IndexCount : int, InstanceCount : int) : unit = 
        let _IndexCount = IndexCount
        let _InstanceCount = InstanceCount
        let _FirstIndex = 0
        let _BaseVertex = 0
        let _FirstInstance = 0
        DawnRaw.wgpuRenderPassEncoderDrawIndexed(x.Handle, _IndexCount, _InstanceCount, _FirstIndex, _BaseVertex, _FirstInstance)
    member inline x.DrawIndexed(IndexCount : int, InstanceCount : int, FirstIndex : int) : unit = 
        let _IndexCount = IndexCount
        let _InstanceCount = InstanceCount
        let _FirstIndex = FirstIndex
        let _BaseVertex = 0
        let _FirstInstance = 0
        DawnRaw.wgpuRenderPassEncoderDrawIndexed(x.Handle, _IndexCount, _InstanceCount, _FirstIndex, _BaseVertex, _FirstInstance)
    member inline x.DrawIndexed(IndexCount : int, InstanceCount : int, FirstIndex : int, BaseVertex : int32) : unit = 
        let _IndexCount = IndexCount
        let _InstanceCount = InstanceCount
        let _FirstIndex = FirstIndex
        let _BaseVertex = BaseVertex
        let _FirstInstance = 0
        DawnRaw.wgpuRenderPassEncoderDrawIndexed(x.Handle, _IndexCount, _InstanceCount, _FirstIndex, _BaseVertex, _FirstInstance)
    member inline x.DrawIndexed(IndexCount : int, InstanceCount : int, FirstIndex : int, BaseVertex : int32, FirstInstance : int) : unit = 
        let _IndexCount = IndexCount
        let _InstanceCount = InstanceCount
        let _FirstIndex = FirstIndex
        let _BaseVertex = BaseVertex
        let _FirstInstance = FirstInstance
        DawnRaw.wgpuRenderPassEncoderDrawIndexed(x.Handle, _IndexCount, _InstanceCount, _FirstIndex, _BaseVertex, _FirstInstance)
    member inline x.DrawIndirect(IndirectBuffer : Buffer, IndirectOffset : uint64) : unit = 
        let _IndirectBuffer = (if isNull IndirectBuffer then BufferHandle.Null else IndirectBuffer.Handle)
        let _IndirectOffset = IndirectOffset
        DawnRaw.wgpuRenderPassEncoderDrawIndirect(x.Handle, _IndirectBuffer, _IndirectOffset)
    member inline x.DrawIndexedIndirect(IndirectBuffer : Buffer, IndirectOffset : uint64) : unit = 
        let _IndirectBuffer = (if isNull IndirectBuffer then BufferHandle.Null else IndirectBuffer.Handle)
        let _IndirectOffset = IndirectOffset
        DawnRaw.wgpuRenderPassEncoderDrawIndexedIndirect(x.Handle, _IndirectBuffer, _IndirectOffset)
    member inline x.ExecuteBundles(Bundles : array<RenderBundle>) : unit = 
        let _BundlesCount = Bundles.Length
        let _Bundles = NativePtr.stackalloc _BundlesCount
        for i in 0 .. _BundlesCount-1 do
            if isNull Bundles.[i] then NativePtr.set _Bundles i RenderBundleHandle.Null
            else NativePtr.set _Bundles i Bundles.[i].Handle
        DawnRaw.wgpuRenderPassEncoderExecuteBundles(x.Handle, _BundlesCount, _Bundles)
    member inline x.InsertDebugMarker(MarkerLabel : string) : unit = 
        let inline _MarkerLabelCont (_MarkerLabel) = 
            DawnRaw.wgpuRenderPassEncoderInsertDebugMarker(x.Handle, _MarkerLabel)
        if not (isNull MarkerLabel) then
            let _MarkerLabelLen = System.Text.Encoding.UTF8.GetByteCount MarkerLabel
            let _MarkerLabelSize = _MarkerLabelLen + 1
            let _MarkerLabelPtr = NativePtr.stackalloc<byte> _MarkerLabelSize
            System.Text.Encoding.UTF8.GetBytes(MarkerLabel.AsSpan(), Span<byte>(NativePtr.toVoidPtr _MarkerLabelPtr, _MarkerLabelSize)) |> ignore
            NativePtr.set _MarkerLabelPtr _MarkerLabelLen 0uy
            _MarkerLabelCont (NativePtr.toNativeInt _MarkerLabelPtr)
        else
            _MarkerLabelCont 0n
    member inline x.PopDebugGroup() : unit = 
        DawnRaw.wgpuRenderPassEncoderPopDebugGroup(x.Handle)
    member inline x.PushDebugGroup(GroupLabel : string) : unit = 
        let inline _GroupLabelCont (_GroupLabel) = 
            DawnRaw.wgpuRenderPassEncoderPushDebugGroup(x.Handle, _GroupLabel)
        if not (isNull GroupLabel) then
            let _GroupLabelLen = System.Text.Encoding.UTF8.GetByteCount GroupLabel
            let _GroupLabelSize = _GroupLabelLen + 1
            let _GroupLabelPtr = NativePtr.stackalloc<byte> _GroupLabelSize
            System.Text.Encoding.UTF8.GetBytes(GroupLabel.AsSpan(), Span<byte>(NativePtr.toVoidPtr _GroupLabelPtr, _GroupLabelSize)) |> ignore
            NativePtr.set _GroupLabelPtr _GroupLabelLen 0uy
            _GroupLabelCont (NativePtr.toNativeInt _GroupLabelPtr)
        else
            _GroupLabelCont 0n
    member inline x.SetStencilReference(Reference : int) : unit = 
        let _Reference = Reference
        DawnRaw.wgpuRenderPassEncoderSetStencilReference(x.Handle, _Reference)
    member inline x.SetBlendColor(Color : Color) : unit = 
        let _R = Color.R
        let _G = Color.G
        let _B = Color.B
        let _A = Color.A
        let mutable _ColorValue = Unchecked.defaultof<DawnRaw.WGPUColor>
        _ColorValue.R <- _R
        _ColorValue.G <- _G
        _ColorValue.B <- _B
        _ColorValue.A <- _A
        let _ColorValue = _ColorValue
        let _Color = NativePtr.stackalloc 1
        NativePtr.write _Color _ColorValue
        DawnRaw.wgpuRenderPassEncoderSetBlendColor(x.Handle, _Color)
    member inline x.SetViewport(X : float32, Y : float32, Width : float32, Height : float32, MinDepth : float32, MaxDepth : float32) : unit = 
        let _X = X
        let _Y = Y
        let _Width = Width
        let _Height = Height
        let _MinDepth = MinDepth
        let _MaxDepth = MaxDepth
        DawnRaw.wgpuRenderPassEncoderSetViewport(x.Handle, _X, _Y, _Width, _Height, _MinDepth, _MaxDepth)
    member inline x.SetScissorRect(X : int, Y : int, Width : int, Height : int) : unit = 
        let _X = X
        let _Y = Y
        let _Width = Width
        let _Height = Height
        DawnRaw.wgpuRenderPassEncoderSetScissorRect(x.Handle, _X, _Y, _Width, _Height)
    member inline x.SetVertexBuffer(Slot : int, Buffer : Buffer) : unit = 
        let _Slot = Slot
        let _Buffer = (if isNull Buffer then BufferHandle.Null else Buffer.Handle)
        let _Offset = 0UL
        let _Size = 0UL
        DawnRaw.wgpuRenderPassEncoderSetVertexBuffer(x.Handle, _Slot, _Buffer, _Offset, _Size)
    member inline x.SetVertexBuffer(Slot : int, Buffer : Buffer, Offset : uint64) : unit = 
        let _Slot = Slot
        let _Buffer = (if isNull Buffer then BufferHandle.Null else Buffer.Handle)
        let _Offset = Offset
        let _Size = 0UL
        DawnRaw.wgpuRenderPassEncoderSetVertexBuffer(x.Handle, _Slot, _Buffer, _Offset, _Size)
    member inline x.SetVertexBuffer(Slot : int, Buffer : Buffer, Offset : uint64, Size : uint64) : unit = 
        let _Slot = Slot
        let _Buffer = (if isNull Buffer then BufferHandle.Null else Buffer.Handle)
        let _Offset = Offset
        let _Size = Size
        DawnRaw.wgpuRenderPassEncoderSetVertexBuffer(x.Handle, _Slot, _Buffer, _Offset, _Size)
    member inline x.SetIndexBuffer(Buffer : Buffer) : unit = 
        let _Buffer = (if isNull Buffer then BufferHandle.Null else Buffer.Handle)
        let _Offset = 0UL
        let _Size = 0UL
        DawnRaw.wgpuRenderPassEncoderSetIndexBuffer(x.Handle, _Buffer, _Offset, _Size)
    member inline x.SetIndexBuffer(Buffer : Buffer, Offset : uint64) : unit = 
        let _Buffer = (if isNull Buffer then BufferHandle.Null else Buffer.Handle)
        let _Offset = Offset
        let _Size = 0UL
        DawnRaw.wgpuRenderPassEncoderSetIndexBuffer(x.Handle, _Buffer, _Offset, _Size)
    member inline x.SetIndexBuffer(Buffer : Buffer, Offset : uint64, Size : uint64) : unit = 
        let _Buffer = (if isNull Buffer then BufferHandle.Null else Buffer.Handle)
        let _Offset = Offset
        let _Size = Size
        DawnRaw.wgpuRenderPassEncoderSetIndexBuffer(x.Handle, _Buffer, _Offset, _Size)
    member inline x.SetIndexBufferWithFormat(Buffer : Buffer, Format : IndexFormat) : unit = 
        let _Buffer = (if isNull Buffer then BufferHandle.Null else Buffer.Handle)
        let _Format = Format
        let _Offset = 0UL
        let _Size = 0UL
        DawnRaw.wgpuRenderPassEncoderSetIndexBufferWithFormat(x.Handle, _Buffer, _Format, _Offset, _Size)
    member inline x.SetIndexBufferWithFormat(Buffer : Buffer, Format : IndexFormat, Offset : uint64) : unit = 
        let _Buffer = (if isNull Buffer then BufferHandle.Null else Buffer.Handle)
        let _Format = Format
        let _Offset = Offset
        let _Size = 0UL
        DawnRaw.wgpuRenderPassEncoderSetIndexBufferWithFormat(x.Handle, _Buffer, _Format, _Offset, _Size)
    member inline x.SetIndexBufferWithFormat(Buffer : Buffer, Format : IndexFormat, Offset : uint64, Size : uint64) : unit = 
        let _Buffer = (if isNull Buffer then BufferHandle.Null else Buffer.Handle)
        let _Format = Format
        let _Offset = Offset
        let _Size = Size
        DawnRaw.wgpuRenderPassEncoderSetIndexBufferWithFormat(x.Handle, _Buffer, _Format, _Offset, _Size)
    member inline x.WriteTimestamp(QuerySet : QuerySet, QueryIndex : int) : unit = 
        let _QuerySet = (if isNull QuerySet then QuerySetHandle.Null else QuerySet.Handle)
        let _QueryIndex = QueryIndex
        DawnRaw.wgpuRenderPassEncoderWriteTimestamp(x.Handle, _QuerySet, _QueryIndex)
    member inline x.EndPass() : unit = 
        DawnRaw.wgpuRenderPassEncoderEndPass(x.Handle)
[<AllowNullLiteral>]
type CommandEncoder(device : Device, handle : CommandEncoderHandle, refCount : ref<int>) = 
    let mutable isDisposed = false
    member x.Device = device
    member x.ReferenceCount = !refCount
    member x.Handle = handle
    member x.IsDisposed = isDisposed
    member private x.Dispose(disposing : bool) =
        if not isDisposed then 
            let r = Interlocked.Decrement(&refCount.contents)
            isDisposed <- true
            if disposing then System.GC.SuppressFinalize x
            DawnRaw.wgpuCommandEncoderRelease(handle)
    member x.Dispose() = x.Dispose(true)
    override x.Finalize() = x.Dispose(false)
    member x.Clone() = 
        if isDisposed || Interlocked.Increment(&refCount.contents) = 1 then raise <| System.ObjectDisposedException("CommandEncoder")
        DawnRaw.wgpuCommandEncoderReference(handle)
        new CommandEncoder(device, handle, refCount)
    interface System.IDisposable with
        member x.Dispose() = x.Dispose()
    new(device : Device, handle : CommandEncoderHandle) = new CommandEncoder(device, handle, ref 1)
    member inline x.Finish() : CommandBuffer = 
        let inline _LabelCont (_Label) = 
            let mutable _DescriptorValue = Unchecked.defaultof<DawnRaw.WGPUCommandBufferDescriptor>
            _DescriptorValue.Next <- 0n
            _DescriptorValue.Label <- _Label
            let _DescriptorValue = _DescriptorValue
            let _Descriptor = NativePtr.stackalloc 1
            NativePtr.write _Descriptor _DescriptorValue
            new CommandBuffer(x.Device, DawnRaw.wgpuCommandEncoderFinish(x.Handle, _Descriptor))
        if not (isNull CommandBufferDescriptor.Default.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount CommandBufferDescriptor.Default.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(CommandBufferDescriptor.Default.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
    member inline x.Finish(Descriptor : CommandBufferDescriptor) : CommandBuffer = 
        let inline _LabelCont (_Label) = 
            let mutable _DescriptorValue = Unchecked.defaultof<DawnRaw.WGPUCommandBufferDescriptor>
            _DescriptorValue.Next <- 0n
            _DescriptorValue.Label <- _Label
            let _DescriptorValue = _DescriptorValue
            let _Descriptor = NativePtr.stackalloc 1
            NativePtr.write _Descriptor _DescriptorValue
            new CommandBuffer(x.Device, DawnRaw.wgpuCommandEncoderFinish(x.Handle, _Descriptor))
        if not (isNull Descriptor.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount Descriptor.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(Descriptor.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
    member inline x.BeginComputePass() : ComputePassEncoder = 
        let inline _LabelCont (_Label) = 
            let mutable _DescriptorValue = Unchecked.defaultof<DawnRaw.WGPUComputePassDescriptor>
            _DescriptorValue.Next <- 0n
            _DescriptorValue.Label <- _Label
            let _DescriptorValue = _DescriptorValue
            let _Descriptor = NativePtr.stackalloc 1
            NativePtr.write _Descriptor _DescriptorValue
            new ComputePassEncoder(x.Device, DawnRaw.wgpuCommandEncoderBeginComputePass(x.Handle, _Descriptor))
        if not (isNull ComputePassDescriptor.Default.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount ComputePassDescriptor.Default.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(ComputePassDescriptor.Default.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
    member inline x.BeginComputePass(Descriptor : ComputePassDescriptor) : ComputePassEncoder = 
        let inline _LabelCont (_Label) = 
            let mutable _DescriptorValue = Unchecked.defaultof<DawnRaw.WGPUComputePassDescriptor>
            _DescriptorValue.Next <- 0n
            _DescriptorValue.Label <- _Label
            let _DescriptorValue = _DescriptorValue
            let _Descriptor = NativePtr.stackalloc 1
            NativePtr.write _Descriptor _DescriptorValue
            new ComputePassEncoder(x.Device, DawnRaw.wgpuCommandEncoderBeginComputePass(x.Handle, _Descriptor))
        if not (isNull Descriptor.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount Descriptor.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(Descriptor.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
    member inline x.BeginRenderPass(Descriptor : RenderPassDescriptor) : RenderPassEncoder = 
        let inline _LabelCont (_Label) = 
            let _ColorAttachmentsCount = if isNull Descriptor.ColorAttachments then 0 else Descriptor.ColorAttachments.Length
            let rec _ColorAttachmentsCont (_ColorAttachmentsinputs : array<RenderPassColorAttachmentDescriptor>) (_ColorAttachmentsoutputs : array<DawnRaw.WGPURenderPassColorAttachmentDescriptor>) (_ColorAttachmentsi : int) =
                if _ColorAttachmentsi >= _ColorAttachmentsCount then
                    use _ColorAttachments = fixed _ColorAttachmentsoutputs
                    let inline _DepthStencilAttachmentCont _DepthStencilAttachment = 
                        let _OcclusionQuerySet = (if isNull Descriptor.OcclusionQuerySet then QuerySetHandle.Null else Descriptor.OcclusionQuerySet.Handle)
                        let mutable _DescriptorValue = Unchecked.defaultof<DawnRaw.WGPURenderPassDescriptor>
                        _DescriptorValue.Next <- 0n
                        _DescriptorValue.Label <- _Label
                        _DescriptorValue.ColorAttachmentsCount <- _ColorAttachmentsCount
                        _DescriptorValue.ColorAttachments <- _ColorAttachments
                        _DescriptorValue.DepthStencilAttachment <- _DepthStencilAttachment
                        _DescriptorValue.OcclusionQuerySet <- _OcclusionQuerySet
                        let _DescriptorValue = _DescriptorValue
                        let _Descriptor = NativePtr.stackalloc 1
                        NativePtr.write _Descriptor _DescriptorValue
                        new RenderPassEncoder(x.Device, DawnRaw.wgpuCommandEncoderBeginRenderPass(x.Handle, _Descriptor))
                    match Descriptor.DepthStencilAttachment with
                    | Some v ->
                        let _Attachment = (if isNull v.Attachment then TextureViewHandle.Null else v.Attachment.Handle)
                        let _DepthLoadOp = v.DepthLoadOp
                        let _DepthStoreOp = v.DepthStoreOp
                        let _ClearDepth = v.ClearDepth
                        let _DepthReadOnly = (if v.DepthReadOnly then 1 else 0)
                        let _StencilLoadOp = v.StencilLoadOp
                        let _StencilStoreOp = v.StencilStoreOp
                        let _ClearStencil = v.ClearStencil
                        let _StencilReadOnly = (if v.StencilReadOnly then 1 else 0)
                        let mutable _n = Unchecked.defaultof<DawnRaw.WGPURenderPassDepthStencilAttachmentDescriptor>
                        _n.Attachment <- _Attachment
                        _n.DepthLoadOp <- _DepthLoadOp
                        _n.DepthStoreOp <- _DepthStoreOp
                        _n.ClearDepth <- _ClearDepth
                        _n.DepthReadOnly <- _DepthReadOnly
                        _n.StencilLoadOp <- _StencilLoadOp
                        _n.StencilStoreOp <- _StencilStoreOp
                        _n.ClearStencil <- _ClearStencil
                        _n.StencilReadOnly <- _StencilReadOnly
                        let _n = _n
                        let ptr = NativePtr.stackalloc 1
                        NativePtr.write ptr _n
                        _DepthStencilAttachmentCont ptr
                    | None -> _DepthStencilAttachmentCont (NativePtr.ofNativeInt 0n)
                else
                    let _Attachment = (if isNull _ColorAttachmentsinputs.[_ColorAttachmentsi].Attachment then TextureViewHandle.Null else _ColorAttachmentsinputs.[_ColorAttachmentsi].Attachment.Handle)
                    let _ResolveTarget = (if isNull _ColorAttachmentsinputs.[_ColorAttachmentsi].ResolveTarget then TextureViewHandle.Null else _ColorAttachmentsinputs.[_ColorAttachmentsi].ResolveTarget.Handle)
                    let _LoadOp = _ColorAttachmentsinputs.[_ColorAttachmentsi].LoadOp
                    let _StoreOp = _ColorAttachmentsinputs.[_ColorAttachmentsi].StoreOp
                    let _R = _ColorAttachmentsinputs.[_ColorAttachmentsi].ClearColor.R
                    let _G = _ColorAttachmentsinputs.[_ColorAttachmentsi].ClearColor.G
                    let _B = _ColorAttachmentsinputs.[_ColorAttachmentsi].ClearColor.B
                    let _A = _ColorAttachmentsinputs.[_ColorAttachmentsi].ClearColor.A
                    let mutable _ClearColor = Unchecked.defaultof<DawnRaw.WGPUColor>
                    _ClearColor.R <- _R
                    _ClearColor.G <- _G
                    _ClearColor.B <- _B
                    _ClearColor.A <- _A
                    let _ClearColor = _ClearColor
                    let mutable _n = Unchecked.defaultof<DawnRaw.WGPURenderPassColorAttachmentDescriptor>
                    _n.Attachment <- _Attachment
                    _n.ResolveTarget <- _ResolveTarget
                    _n.LoadOp <- _LoadOp
                    _n.StoreOp <- _StoreOp
                    _n.ClearColor <- _ClearColor
                    let _n = _n
                    _ColorAttachmentsoutputs.[_ColorAttachmentsi] <- _n
                    _ColorAttachmentsCont _ColorAttachmentsinputs _ColorAttachmentsoutputs (_ColorAttachmentsi + 1)
            _ColorAttachmentsCont Descriptor.ColorAttachments (if _ColorAttachmentsCount > 0 then Array.zeroCreate _ColorAttachmentsCount else null) 0
        if not (isNull Descriptor.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount Descriptor.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(Descriptor.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
    member inline x.CopyBufferToBuffer(Source : Buffer, SourceOffset : uint64, Destination : Buffer, DestinationOffset : uint64, Size : uint64) : unit = 
        let _Source = (if isNull Source then BufferHandle.Null else Source.Handle)
        let _SourceOffset = SourceOffset
        let _Destination = (if isNull Destination then BufferHandle.Null else Destination.Handle)
        let _DestinationOffset = DestinationOffset
        let _Size = Size
        DawnRaw.wgpuCommandEncoderCopyBufferToBuffer(x.Handle, _Source, _SourceOffset, _Destination, _DestinationOffset, _Size)
    member inline x.CopyBufferToTexture(Source : BufferCopyView, Destination : TextureCopyView, CopySize : Extent3D) : unit = 
        let _Offset = Source.Layout.Offset
        let _BytesPerRow = Source.Layout.BytesPerRow
        let _RowsPerImage = Source.Layout.RowsPerImage
        let mutable _Layout = Unchecked.defaultof<DawnRaw.WGPUTextureDataLayout>
        _Layout.Next <- 0n
        _Layout.Offset <- _Offset
        _Layout.BytesPerRow <- _BytesPerRow
        _Layout.RowsPerImage <- _RowsPerImage
        let _Layout = _Layout
        let _Buffer = (if isNull Source.Buffer then BufferHandle.Null else Source.Buffer.Handle)
        let mutable _SourceValue = Unchecked.defaultof<DawnRaw.WGPUBufferCopyView>
        _SourceValue.Next <- 0n
        _SourceValue.Layout <- _Layout
        _SourceValue.Buffer <- _Buffer
        let _SourceValue = _SourceValue
        let _Source = NativePtr.stackalloc 1
        NativePtr.write _Source _SourceValue
        let _Texture = (if isNull Destination.Texture then TextureHandle.Null else Destination.Texture.Handle)
        let _MipLevel = Destination.MipLevel
        let _X = Destination.Origin.X
        let _Y = Destination.Origin.Y
        let _Z = Destination.Origin.Z
        let mutable _Origin = Unchecked.defaultof<DawnRaw.WGPUOrigin3D>
        _Origin.X <- _X
        _Origin.Y <- _Y
        _Origin.Z <- _Z
        let _Origin = _Origin
        let _Aspect = Destination.Aspect
        let mutable _DestinationValue = Unchecked.defaultof<DawnRaw.WGPUTextureCopyView>
        _DestinationValue.Next <- 0n
        _DestinationValue.Texture <- _Texture
        _DestinationValue.MipLevel <- _MipLevel
        _DestinationValue.Origin <- _Origin
        _DestinationValue.Aspect <- _Aspect
        let _DestinationValue = _DestinationValue
        let _Destination = NativePtr.stackalloc 1
        NativePtr.write _Destination _DestinationValue
        let _Width = CopySize.Width
        let _Height = CopySize.Height
        let _Depth = CopySize.Depth
        let mutable _CopySizeValue = Unchecked.defaultof<DawnRaw.WGPUExtent3D>
        _CopySizeValue.Width <- _Width
        _CopySizeValue.Height <- _Height
        _CopySizeValue.Depth <- _Depth
        let _CopySizeValue = _CopySizeValue
        let _CopySize = NativePtr.stackalloc 1
        NativePtr.write _CopySize _CopySizeValue
        DawnRaw.wgpuCommandEncoderCopyBufferToTexture(x.Handle, _Source, _Destination, _CopySize)
    member inline x.CopyTextureToBuffer(Source : TextureCopyView, Destination : BufferCopyView, CopySize : Extent3D) : unit = 
        let _Texture = (if isNull Source.Texture then TextureHandle.Null else Source.Texture.Handle)
        let _MipLevel = Source.MipLevel
        let _X = Source.Origin.X
        let _Y = Source.Origin.Y
        let _Z = Source.Origin.Z
        let mutable _Origin = Unchecked.defaultof<DawnRaw.WGPUOrigin3D>
        _Origin.X <- _X
        _Origin.Y <- _Y
        _Origin.Z <- _Z
        let _Origin = _Origin
        let _Aspect = Source.Aspect
        let mutable _SourceValue = Unchecked.defaultof<DawnRaw.WGPUTextureCopyView>
        _SourceValue.Next <- 0n
        _SourceValue.Texture <- _Texture
        _SourceValue.MipLevel <- _MipLevel
        _SourceValue.Origin <- _Origin
        _SourceValue.Aspect <- _Aspect
        let _SourceValue = _SourceValue
        let _Source = NativePtr.stackalloc 1
        NativePtr.write _Source _SourceValue
        let _Offset = Destination.Layout.Offset
        let _BytesPerRow = Destination.Layout.BytesPerRow
        let _RowsPerImage = Destination.Layout.RowsPerImage
        let mutable _Layout = Unchecked.defaultof<DawnRaw.WGPUTextureDataLayout>
        _Layout.Next <- 0n
        _Layout.Offset <- _Offset
        _Layout.BytesPerRow <- _BytesPerRow
        _Layout.RowsPerImage <- _RowsPerImage
        let _Layout = _Layout
        let _Buffer = (if isNull Destination.Buffer then BufferHandle.Null else Destination.Buffer.Handle)
        let mutable _DestinationValue = Unchecked.defaultof<DawnRaw.WGPUBufferCopyView>
        _DestinationValue.Next <- 0n
        _DestinationValue.Layout <- _Layout
        _DestinationValue.Buffer <- _Buffer
        let _DestinationValue = _DestinationValue
        let _Destination = NativePtr.stackalloc 1
        NativePtr.write _Destination _DestinationValue
        let _Width = CopySize.Width
        let _Height = CopySize.Height
        let _Depth = CopySize.Depth
        let mutable _CopySizeValue = Unchecked.defaultof<DawnRaw.WGPUExtent3D>
        _CopySizeValue.Width <- _Width
        _CopySizeValue.Height <- _Height
        _CopySizeValue.Depth <- _Depth
        let _CopySizeValue = _CopySizeValue
        let _CopySize = NativePtr.stackalloc 1
        NativePtr.write _CopySize _CopySizeValue
        DawnRaw.wgpuCommandEncoderCopyTextureToBuffer(x.Handle, _Source, _Destination, _CopySize)
    member inline x.CopyTextureToTexture(Source : TextureCopyView, Destination : TextureCopyView, CopySize : Extent3D) : unit = 
        let _Texture = (if isNull Source.Texture then TextureHandle.Null else Source.Texture.Handle)
        let _MipLevel = Source.MipLevel
        let _X = Source.Origin.X
        let _Y = Source.Origin.Y
        let _Z = Source.Origin.Z
        let mutable _Origin = Unchecked.defaultof<DawnRaw.WGPUOrigin3D>
        _Origin.X <- _X
        _Origin.Y <- _Y
        _Origin.Z <- _Z
        let _Origin = _Origin
        let _Aspect = Source.Aspect
        let mutable _SourceValue = Unchecked.defaultof<DawnRaw.WGPUTextureCopyView>
        _SourceValue.Next <- 0n
        _SourceValue.Texture <- _Texture
        _SourceValue.MipLevel <- _MipLevel
        _SourceValue.Origin <- _Origin
        _SourceValue.Aspect <- _Aspect
        let _SourceValue = _SourceValue
        let _Source = NativePtr.stackalloc 1
        NativePtr.write _Source _SourceValue
        let _Texture = (if isNull Destination.Texture then TextureHandle.Null else Destination.Texture.Handle)
        let _MipLevel = Destination.MipLevel
        let _X = Destination.Origin.X
        let _Y = Destination.Origin.Y
        let _Z = Destination.Origin.Z
        let mutable _Origin = Unchecked.defaultof<DawnRaw.WGPUOrigin3D>
        _Origin.X <- _X
        _Origin.Y <- _Y
        _Origin.Z <- _Z
        let _Origin = _Origin
        let _Aspect = Destination.Aspect
        let mutable _DestinationValue = Unchecked.defaultof<DawnRaw.WGPUTextureCopyView>
        _DestinationValue.Next <- 0n
        _DestinationValue.Texture <- _Texture
        _DestinationValue.MipLevel <- _MipLevel
        _DestinationValue.Origin <- _Origin
        _DestinationValue.Aspect <- _Aspect
        let _DestinationValue = _DestinationValue
        let _Destination = NativePtr.stackalloc 1
        NativePtr.write _Destination _DestinationValue
        let _Width = CopySize.Width
        let _Height = CopySize.Height
        let _Depth = CopySize.Depth
        let mutable _CopySizeValue = Unchecked.defaultof<DawnRaw.WGPUExtent3D>
        _CopySizeValue.Width <- _Width
        _CopySizeValue.Height <- _Height
        _CopySizeValue.Depth <- _Depth
        let _CopySizeValue = _CopySizeValue
        let _CopySize = NativePtr.stackalloc 1
        NativePtr.write _CopySize _CopySizeValue
        DawnRaw.wgpuCommandEncoderCopyTextureToTexture(x.Handle, _Source, _Destination, _CopySize)
    member inline x.InsertDebugMarker(MarkerLabel : string) : unit = 
        let inline _MarkerLabelCont (_MarkerLabel) = 
            DawnRaw.wgpuCommandEncoderInsertDebugMarker(x.Handle, _MarkerLabel)
        if not (isNull MarkerLabel) then
            let _MarkerLabelLen = System.Text.Encoding.UTF8.GetByteCount MarkerLabel
            let _MarkerLabelSize = _MarkerLabelLen + 1
            let _MarkerLabelPtr = NativePtr.stackalloc<byte> _MarkerLabelSize
            System.Text.Encoding.UTF8.GetBytes(MarkerLabel.AsSpan(), Span<byte>(NativePtr.toVoidPtr _MarkerLabelPtr, _MarkerLabelSize)) |> ignore
            NativePtr.set _MarkerLabelPtr _MarkerLabelLen 0uy
            _MarkerLabelCont (NativePtr.toNativeInt _MarkerLabelPtr)
        else
            _MarkerLabelCont 0n
    member inline x.PopDebugGroup() : unit = 
        DawnRaw.wgpuCommandEncoderPopDebugGroup(x.Handle)
    member inline x.PushDebugGroup(GroupLabel : string) : unit = 
        let inline _GroupLabelCont (_GroupLabel) = 
            DawnRaw.wgpuCommandEncoderPushDebugGroup(x.Handle, _GroupLabel)
        if not (isNull GroupLabel) then
            let _GroupLabelLen = System.Text.Encoding.UTF8.GetByteCount GroupLabel
            let _GroupLabelSize = _GroupLabelLen + 1
            let _GroupLabelPtr = NativePtr.stackalloc<byte> _GroupLabelSize
            System.Text.Encoding.UTF8.GetBytes(GroupLabel.AsSpan(), Span<byte>(NativePtr.toVoidPtr _GroupLabelPtr, _GroupLabelSize)) |> ignore
            NativePtr.set _GroupLabelPtr _GroupLabelLen 0uy
            _GroupLabelCont (NativePtr.toNativeInt _GroupLabelPtr)
        else
            _GroupLabelCont 0n
    member inline x.ResolveQuerySet(QuerySet : QuerySet, FirstQuery : int, QueryCount : int, Destination : Buffer, DestinationOffset : uint64) : unit = 
        let _QuerySet = (if isNull QuerySet then QuerySetHandle.Null else QuerySet.Handle)
        let _FirstQuery = FirstQuery
        let _QueryCount = QueryCount
        let _Destination = (if isNull Destination then BufferHandle.Null else Destination.Handle)
        let _DestinationOffset = DestinationOffset
        DawnRaw.wgpuCommandEncoderResolveQuerySet(x.Handle, _QuerySet, _FirstQuery, _QueryCount, _Destination, _DestinationOffset)
    member inline x.WriteTimestamp(QuerySet : QuerySet, QueryIndex : int) : unit = 
        let _QuerySet = (if isNull QuerySet then QuerySetHandle.Null else QuerySet.Handle)
        let _QueryIndex = QueryIndex
        DawnRaw.wgpuCommandEncoderWriteTimestamp(x.Handle, _QuerySet, _QueryIndex)
[<AllowNullLiteral>]
type Queue(device : Device, handle : QueueHandle, refCount : ref<int>) = 
    let mutable isDisposed = false
    member x.Device = device
    member x.ReferenceCount = !refCount
    member x.Handle = handle
    member x.IsDisposed = isDisposed
    member private x.Dispose(disposing : bool) =
        if not isDisposed then 
            let r = Interlocked.Decrement(&refCount.contents)
            isDisposed <- true
            if disposing then System.GC.SuppressFinalize x
            DawnRaw.wgpuQueueRelease(handle)
    member x.Dispose() = x.Dispose(true)
    override x.Finalize() = x.Dispose(false)
    member x.Clone() = 
        if isDisposed || Interlocked.Increment(&refCount.contents) = 1 then raise <| System.ObjectDisposedException("Queue")
        DawnRaw.wgpuQueueReference(handle)
        new Queue(device, handle, refCount)
    interface System.IDisposable with
        member x.Dispose() = x.Dispose()
    new(device : Device, handle : QueueHandle) = new Queue(device, handle, ref 1)
    member inline x.Submit(Commands : array<CommandBuffer>) : unit = 
        let _CommandsCount = Commands.Length
        let _Commands = NativePtr.stackalloc _CommandsCount
        for i in 0 .. _CommandsCount-1 do
            if isNull Commands.[i] then NativePtr.set _Commands i CommandBufferHandle.Null
            else NativePtr.set _Commands i Commands.[i].Handle
        DawnRaw.wgpuQueueSubmit(x.Handle, _CommandsCount, _Commands)
    member inline x.Signal(Fence : Fence, SignalValue : uint64) : unit = 
        let _Fence = (if isNull Fence then FenceHandle.Null else Fence.Handle)
        let _SignalValue = SignalValue
        DawnRaw.wgpuQueueSignal(x.Handle, _Fence, _SignalValue)
    member inline x.CreateFence() : Fence = 
        let inline _LabelCont (_Label) = 
            let _InitialValue = FenceDescriptor.Default.InitialValue
            let mutable _DescriptorValue = Unchecked.defaultof<DawnRaw.WGPUFenceDescriptor>
            _DescriptorValue.Next <- 0n
            _DescriptorValue.Label <- _Label
            _DescriptorValue.InitialValue <- _InitialValue
            let _DescriptorValue = _DescriptorValue
            let _Descriptor = NativePtr.stackalloc 1
            NativePtr.write _Descriptor _DescriptorValue
            new Fence(x.Device, DawnRaw.wgpuQueueCreateFence(x.Handle, _Descriptor))
        if not (isNull FenceDescriptor.Default.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount FenceDescriptor.Default.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(FenceDescriptor.Default.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
    member inline x.CreateFence(Descriptor : FenceDescriptor) : Fence = 
        let inline _LabelCont (_Label) = 
            let _InitialValue = Descriptor.InitialValue
            let mutable _DescriptorValue = Unchecked.defaultof<DawnRaw.WGPUFenceDescriptor>
            _DescriptorValue.Next <- 0n
            _DescriptorValue.Label <- _Label
            _DescriptorValue.InitialValue <- _InitialValue
            let _DescriptorValue = _DescriptorValue
            let _Descriptor = NativePtr.stackalloc 1
            NativePtr.write _Descriptor _DescriptorValue
            new Fence(x.Device, DawnRaw.wgpuQueueCreateFence(x.Handle, _Descriptor))
        if not (isNull Descriptor.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount Descriptor.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(Descriptor.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
    member inline x.WriteBuffer(Buffer : Buffer, BufferOffset : uint64, Data : nativeint, Size : unativeint) : unit = 
        let _Buffer = (if isNull Buffer then BufferHandle.Null else Buffer.Handle)
        let _BufferOffset = BufferOffset
        let _Data = Data
        let _Size = Size
        DawnRaw.wgpuQueueWriteBuffer(x.Handle, _Buffer, _BufferOffset, _Data, _Size)
    member inline x.WriteTexture(Destination : TextureCopyView, Data : nativeint, DataSize : unativeint, DataLayout : TextureDataLayout, WriteSize : Extent3D) : unit = 
        let _Texture = (if isNull Destination.Texture then TextureHandle.Null else Destination.Texture.Handle)
        let _MipLevel = Destination.MipLevel
        let _X = Destination.Origin.X
        let _Y = Destination.Origin.Y
        let _Z = Destination.Origin.Z
        let mutable _Origin = Unchecked.defaultof<DawnRaw.WGPUOrigin3D>
        _Origin.X <- _X
        _Origin.Y <- _Y
        _Origin.Z <- _Z
        let _Origin = _Origin
        let _Aspect = Destination.Aspect
        let mutable _DestinationValue = Unchecked.defaultof<DawnRaw.WGPUTextureCopyView>
        _DestinationValue.Next <- 0n
        _DestinationValue.Texture <- _Texture
        _DestinationValue.MipLevel <- _MipLevel
        _DestinationValue.Origin <- _Origin
        _DestinationValue.Aspect <- _Aspect
        let _DestinationValue = _DestinationValue
        let _Destination = NativePtr.stackalloc 1
        NativePtr.write _Destination _DestinationValue
        let _Data = Data
        let _DataSize = DataSize
        let _Offset = DataLayout.Offset
        let _BytesPerRow = DataLayout.BytesPerRow
        let _RowsPerImage = DataLayout.RowsPerImage
        let mutable _DataLayoutValue = Unchecked.defaultof<DawnRaw.WGPUTextureDataLayout>
        _DataLayoutValue.Next <- 0n
        _DataLayoutValue.Offset <- _Offset
        _DataLayoutValue.BytesPerRow <- _BytesPerRow
        _DataLayoutValue.RowsPerImage <- _RowsPerImage
        let _DataLayoutValue = _DataLayoutValue
        let _DataLayout = NativePtr.stackalloc 1
        NativePtr.write _DataLayout _DataLayoutValue
        let _Width = WriteSize.Width
        let _Height = WriteSize.Height
        let _Depth = WriteSize.Depth
        let mutable _WriteSizeValue = Unchecked.defaultof<DawnRaw.WGPUExtent3D>
        _WriteSizeValue.Width <- _Width
        _WriteSizeValue.Height <- _Height
        _WriteSizeValue.Depth <- _Depth
        let _WriteSizeValue = _WriteSizeValue
        let _WriteSize = NativePtr.stackalloc 1
        NativePtr.write _WriteSize _WriteSizeValue
        DawnRaw.wgpuQueueWriteTexture(x.Handle, _Destination, _Data, _DataSize, _DataLayout, _WriteSize)
[<AllowNullLiteral>]
type Device(handle : DeviceHandle, refCount : ref<int>) = 
    let mutable isDisposed = false
    member x.ReferenceCount = !refCount
    member x.Handle = handle
    member x.IsDisposed = isDisposed
    member private x.Dispose(disposing : bool) =
        if not isDisposed then 
            let r = Interlocked.Decrement(&refCount.contents)
            isDisposed <- true
            if disposing then System.GC.SuppressFinalize x
            DawnRaw.wgpuDeviceRelease(handle)
    member x.Dispose() = x.Dispose(true)
    override x.Finalize() = x.Dispose(false)
    member x.Clone() = 
        if isDisposed || Interlocked.Increment(&refCount.contents) = 1 then raise <| System.ObjectDisposedException("Device")
        DawnRaw.wgpuDeviceReference(handle)
        new Device(handle, refCount)
    interface System.IDisposable with
        member x.Dispose() = x.Dispose()
    new(handle : DeviceHandle) = new Device(handle, ref 1)
    member inline x.CreateBindGroup(Descriptor : BindGroupDescriptor) : BindGroup = 
        let inline _LabelCont (_Label) = 
            let _Layout = (if isNull Descriptor.Layout then BindGroupLayoutHandle.Null else Descriptor.Layout.Handle)
            let _EntriesCount = if isNull Descriptor.Entries then 0 else Descriptor.Entries.Length
            let rec _EntriesCont (_Entriesinputs : array<BindGroupEntry>) (_Entriesoutputs : array<DawnRaw.WGPUBindGroupEntry>) (_Entriesi : int) =
                if _Entriesi >= _EntriesCount then
                    use _Entries = fixed _Entriesoutputs
                    let mutable _DescriptorValue = Unchecked.defaultof<DawnRaw.WGPUBindGroupDescriptor>
                    _DescriptorValue.Next <- 0n
                    _DescriptorValue.Label <- _Label
                    _DescriptorValue.Layout <- _Layout
                    _DescriptorValue.EntriesCount <- _EntriesCount
                    _DescriptorValue.Entries <- _Entries
                    let _DescriptorValue = _DescriptorValue
                    let _Descriptor = NativePtr.stackalloc 1
                    NativePtr.write _Descriptor _DescriptorValue
                    new BindGroup(x, DawnRaw.wgpuDeviceCreateBindGroup(x.Handle, _Descriptor))
                else
                    let _Binding = _Entriesinputs.[_Entriesi].Binding
                    let _Buffer = (if isNull _Entriesinputs.[_Entriesi].Buffer then BufferHandle.Null else _Entriesinputs.[_Entriesi].Buffer.Handle)
                    let _Offset = _Entriesinputs.[_Entriesi].Offset
                    let _Size = _Entriesinputs.[_Entriesi].Size
                    let _Sampler = (if isNull _Entriesinputs.[_Entriesi].Sampler then SamplerHandle.Null else _Entriesinputs.[_Entriesi].Sampler.Handle)
                    let _TextureView = (if isNull _Entriesinputs.[_Entriesi].TextureView then TextureViewHandle.Null else _Entriesinputs.[_Entriesi].TextureView.Handle)
                    let mutable _n = Unchecked.defaultof<DawnRaw.WGPUBindGroupEntry>
                    _n.Binding <- _Binding
                    _n.Buffer <- _Buffer
                    _n.Offset <- _Offset
                    _n.Size <- _Size
                    _n.Sampler <- _Sampler
                    _n.TextureView <- _TextureView
                    let _n = _n
                    _Entriesoutputs.[_Entriesi] <- _n
                    _EntriesCont _Entriesinputs _Entriesoutputs (_Entriesi + 1)
            _EntriesCont Descriptor.Entries (if _EntriesCount > 0 then Array.zeroCreate _EntriesCount else null) 0
        if not (isNull Descriptor.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount Descriptor.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(Descriptor.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
    member inline x.CreateBindGroupLayout(Descriptor : BindGroupLayoutDescriptor) : BindGroupLayout = 
        let inline _LabelCont (_Label) = 
            let _EntriesCount = if isNull Descriptor.Entries then 0 else Descriptor.Entries.Length
            let rec _EntriesCont (_Entriesinputs : array<BindGroupLayoutEntry>) (_Entriesoutputs : array<DawnRaw.WGPUBindGroupLayoutEntry>) (_Entriesi : int) =
                if _Entriesi >= _EntriesCount then
                    use _Entries = fixed _Entriesoutputs
                    let mutable _DescriptorValue = Unchecked.defaultof<DawnRaw.WGPUBindGroupLayoutDescriptor>
                    _DescriptorValue.Next <- 0n
                    _DescriptorValue.Label <- _Label
                    _DescriptorValue.EntriesCount <- _EntriesCount
                    _DescriptorValue.Entries <- _Entries
                    let _DescriptorValue = _DescriptorValue
                    let _Descriptor = NativePtr.stackalloc 1
                    NativePtr.write _Descriptor _DescriptorValue
                    new BindGroupLayout(x, DawnRaw.wgpuDeviceCreateBindGroupLayout(x.Handle, _Descriptor))
                else
                    let _Binding = _Entriesinputs.[_Entriesi].Binding
                    let _Visibility = _Entriesinputs.[_Entriesi].Visibility
                    let _Type = _Entriesinputs.[_Entriesi].Type
                    let _HasDynamicOffset = (if _Entriesinputs.[_Entriesi].HasDynamicOffset then 1 else 0)
                    let _MinBufferBindingSize = _Entriesinputs.[_Entriesi].MinBufferBindingSize
                    let _Multisampled = (if _Entriesinputs.[_Entriesi].Multisampled then 1 else 0)
                    let _ViewDimension = _Entriesinputs.[_Entriesi].ViewDimension
                    let _TextureComponentType = _Entriesinputs.[_Entriesi].TextureComponentType
                    let _StorageTextureFormat = _Entriesinputs.[_Entriesi].StorageTextureFormat
                    let mutable _n = Unchecked.defaultof<DawnRaw.WGPUBindGroupLayoutEntry>
                    _n.Binding <- _Binding
                    _n.Visibility <- _Visibility
                    _n.Type <- _Type
                    _n.HasDynamicOffset <- _HasDynamicOffset
                    _n.MinBufferBindingSize <- _MinBufferBindingSize
                    _n.Multisampled <- _Multisampled
                    _n.ViewDimension <- _ViewDimension
                    _n.TextureComponentType <- _TextureComponentType
                    _n.StorageTextureFormat <- _StorageTextureFormat
                    let _n = _n
                    _Entriesoutputs.[_Entriesi] <- _n
                    _EntriesCont _Entriesinputs _Entriesoutputs (_Entriesi + 1)
            _EntriesCont Descriptor.Entries (if _EntriesCount > 0 then Array.zeroCreate _EntriesCount else null) 0
        if not (isNull Descriptor.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount Descriptor.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(Descriptor.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
    member inline x.CreateBuffer(Descriptor : BufferDescriptor) : Buffer = 
        let inline _LabelCont (_Label) = 
            let _Usage = Descriptor.Usage
            let _Size = Descriptor.Size
            let _MappedAtCreation = (if Descriptor.MappedAtCreation then 1 else 0)
            let mutable _DescriptorValue = Unchecked.defaultof<DawnRaw.WGPUBufferDescriptor>
            _DescriptorValue.Next <- 0n
            _DescriptorValue.Label <- _Label
            _DescriptorValue.Usage <- _Usage
            _DescriptorValue.Size <- _Size
            _DescriptorValue.MappedAtCreation <- _MappedAtCreation
            let _DescriptorValue = _DescriptorValue
            let _Descriptor = NativePtr.stackalloc 1
            NativePtr.write _Descriptor _DescriptorValue
            new Buffer(x, DawnRaw.wgpuDeviceCreateBuffer(x.Handle, _Descriptor))
        if not (isNull Descriptor.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount Descriptor.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(Descriptor.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
    member inline x.CreateErrorBuffer() : Buffer = 
        new Buffer(x, DawnRaw.wgpuDeviceCreateErrorBuffer(x.Handle))
    member inline x.CreateCommandEncoder() : CommandEncoder = 
        let inline _LabelCont (_Label) = 
            let mutable _DescriptorValue = Unchecked.defaultof<DawnRaw.WGPUCommandEncoderDescriptor>
            _DescriptorValue.Next <- 0n
            _DescriptorValue.Label <- _Label
            let _DescriptorValue = _DescriptorValue
            let _Descriptor = NativePtr.stackalloc 1
            NativePtr.write _Descriptor _DescriptorValue
            new CommandEncoder(x, DawnRaw.wgpuDeviceCreateCommandEncoder(x.Handle, _Descriptor))
        if not (isNull CommandEncoderDescriptor.Default.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount CommandEncoderDescriptor.Default.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(CommandEncoderDescriptor.Default.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
    member inline x.CreateCommandEncoder(Descriptor : CommandEncoderDescriptor) : CommandEncoder = 
        let inline _LabelCont (_Label) = 
            let mutable _DescriptorValue = Unchecked.defaultof<DawnRaw.WGPUCommandEncoderDescriptor>
            _DescriptorValue.Next <- 0n
            _DescriptorValue.Label <- _Label
            let _DescriptorValue = _DescriptorValue
            let _Descriptor = NativePtr.stackalloc 1
            NativePtr.write _Descriptor _DescriptorValue
            new CommandEncoder(x, DawnRaw.wgpuDeviceCreateCommandEncoder(x.Handle, _Descriptor))
        if not (isNull Descriptor.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount Descriptor.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(Descriptor.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
    member inline x.CreateComputePipeline(Descriptor : ComputePipelineDescriptor) : ComputePipeline = 
        let inline _LabelCont (_Label) = 
            let _Layout = (if isNull Descriptor.Layout then PipelineLayoutHandle.Null else Descriptor.Layout.Handle)
            let _Module = (if isNull Descriptor.ComputeStage.Module then ShaderModuleHandle.Null else Descriptor.ComputeStage.Module.Handle)
            let inline _EntryPointCont (_EntryPoint) = 
                let mutable _ComputeStage = Unchecked.defaultof<DawnRaw.WGPUProgrammableStageDescriptor>
                _ComputeStage.Next <- 0n
                _ComputeStage.Module <- _Module
                _ComputeStage.EntryPoint <- _EntryPoint
                let _ComputeStage = _ComputeStage
                let mutable _DescriptorValue = Unchecked.defaultof<DawnRaw.WGPUComputePipelineDescriptor>
                _DescriptorValue.Next <- 0n
                _DescriptorValue.Label <- _Label
                _DescriptorValue.Layout <- _Layout
                _DescriptorValue.ComputeStage <- _ComputeStage
                let _DescriptorValue = _DescriptorValue
                let _Descriptor = NativePtr.stackalloc 1
                NativePtr.write _Descriptor _DescriptorValue
                new ComputePipeline(x, DawnRaw.wgpuDeviceCreateComputePipeline(x.Handle, _Descriptor))
            if not (isNull Descriptor.ComputeStage.EntryPoint) then
                let _EntryPointLen = System.Text.Encoding.UTF8.GetByteCount Descriptor.ComputeStage.EntryPoint
                let _EntryPointSize = _EntryPointLen + 1
                let _EntryPointPtr = NativePtr.stackalloc<byte> _EntryPointSize
                System.Text.Encoding.UTF8.GetBytes(Descriptor.ComputeStage.EntryPoint.AsSpan(), Span<byte>(NativePtr.toVoidPtr _EntryPointPtr, _EntryPointSize)) |> ignore
                NativePtr.set _EntryPointPtr _EntryPointLen 0uy
                _EntryPointCont (NativePtr.toNativeInt _EntryPointPtr)
            else
                _EntryPointCont 0n
        if not (isNull Descriptor.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount Descriptor.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(Descriptor.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
    member inline x.CreatePipelineLayout(Descriptor : PipelineLayoutDescriptor) : PipelineLayout = 
        let inline _LabelCont (_Label) = 
            let _BindGroupLayoutsCount = Descriptor.BindGroupLayouts.Length
            let _BindGroupLayouts = NativePtr.stackalloc _BindGroupLayoutsCount
            for i in 0 .. _BindGroupLayoutsCount-1 do
                if isNull Descriptor.BindGroupLayouts.[i] then NativePtr.set _BindGroupLayouts i BindGroupLayoutHandle.Null
                else NativePtr.set _BindGroupLayouts i Descriptor.BindGroupLayouts.[i].Handle
            let mutable _DescriptorValue = Unchecked.defaultof<DawnRaw.WGPUPipelineLayoutDescriptor>
            _DescriptorValue.Next <- 0n
            _DescriptorValue.Label <- _Label
            _DescriptorValue.BindGroupLayoutsCount <- _BindGroupLayoutsCount
            _DescriptorValue.BindGroupLayouts <- _BindGroupLayouts
            let _DescriptorValue = _DescriptorValue
            let _Descriptor = NativePtr.stackalloc 1
            NativePtr.write _Descriptor _DescriptorValue
            new PipelineLayout(x, DawnRaw.wgpuDeviceCreatePipelineLayout(x.Handle, _Descriptor))
        if not (isNull Descriptor.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount Descriptor.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(Descriptor.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
    member inline x.CreateQuerySet(Descriptor : QuerySetDescriptor) : QuerySet = 
        let inline _LabelCont (_Label) = 
            let _Type = Descriptor.Type
            let _Count = Descriptor.Count
            let inline _PipelineStatisticsCont _PipelineStatistics =
                let _PipelineStatisticsCount = Descriptor.PipelineStatisticsCount
                let mutable _DescriptorValue = Unchecked.defaultof<DawnRaw.WGPUQuerySetDescriptor>
                _DescriptorValue.Next <- 0n
                _DescriptorValue.Label <- _Label
                _DescriptorValue.Type <- _Type
                _DescriptorValue.Count <- _Count
                _DescriptorValue.PipelineStatistics <- _PipelineStatistics
                _DescriptorValue.PipelineStatisticsCount <- _PipelineStatisticsCount
                let _DescriptorValue = _DescriptorValue
                let _Descriptor = NativePtr.stackalloc 1
                NativePtr.write _Descriptor _DescriptorValue
                new QuerySet(x, DawnRaw.wgpuDeviceCreateQuerySet(x.Handle, _Descriptor))
            match Descriptor.PipelineStatistics with
            | Some o ->
                let _PipelineStatistics = NativePtr.stackalloc 1
                NativePtr.write _PipelineStatistics o
                _PipelineStatisticsCont _PipelineStatistics
            | _ ->
                _PipelineStatisticsCont (NativePtr.ofNativeInt 0n)
        if not (isNull Descriptor.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount Descriptor.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(Descriptor.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
    member inline x.CreateRenderBundleEncoder(Descriptor : RenderBundleEncoderDescriptor) : RenderBundleEncoder = 
        let inline _LabelCont (_Label) = 
            use _ColorFormats = fixed Descriptor.ColorFormats
            let _ColorFormatsCount = Descriptor.ColorFormats.Length
            let _DepthStencilFormat = Descriptor.DepthStencilFormat
            let _SampleCount = Descriptor.SampleCount
            let mutable _DescriptorValue = Unchecked.defaultof<DawnRaw.WGPURenderBundleEncoderDescriptor>
            _DescriptorValue.Next <- 0n
            _DescriptorValue.Label <- _Label
            _DescriptorValue.ColorFormatsCount <- _ColorFormatsCount
            _DescriptorValue.ColorFormats <- _ColorFormats
            _DescriptorValue.DepthStencilFormat <- _DepthStencilFormat
            _DescriptorValue.SampleCount <- _SampleCount
            let _DescriptorValue = _DescriptorValue
            let _Descriptor = NativePtr.stackalloc 1
            NativePtr.write _Descriptor _DescriptorValue
            new RenderBundleEncoder(x, DawnRaw.wgpuDeviceCreateRenderBundleEncoder(x.Handle, _Descriptor))
        if not (isNull Descriptor.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount Descriptor.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(Descriptor.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
    member inline x.CreateRenderPipeline(Descriptor : RenderPipelineDescriptor) : RenderPipeline = 
        let inline _LabelCont (_Label) = 
            let _Layout = (if isNull Descriptor.Layout then PipelineLayoutHandle.Null else Descriptor.Layout.Handle)
            let _Module = (if isNull Descriptor.VertexStage.Module then ShaderModuleHandle.Null else Descriptor.VertexStage.Module.Handle)
            let inline _EntryPointCont (_EntryPoint) = 
                let mutable _VertexStage = Unchecked.defaultof<DawnRaw.WGPUProgrammableStageDescriptor>
                _VertexStage.Next <- 0n
                _VertexStage.Module <- _Module
                _VertexStage.EntryPoint <- _EntryPoint
                let _VertexStage = _VertexStage
                let inline _FragmentStageCont _FragmentStage = 
                    let inline _VertexStateCont _VertexState = 
                        let _PrimitiveTopology = Descriptor.PrimitiveTopology
                        let inline _RasterizationStateCont _RasterizationState = 
                            let _SampleCount = Descriptor.SampleCount
                            let inline _DepthStencilStateCont _DepthStencilState = 
                                let _ColorStatesCount = if isNull Descriptor.ColorStates then 0 else Descriptor.ColorStates.Length
                                let rec _ColorStatesCont (_ColorStatesinputs : array<ColorStateDescriptor>) (_ColorStatesoutputs : array<DawnRaw.WGPUColorStateDescriptor>) (_ColorStatesi : int) =
                                    if _ColorStatesi >= _ColorStatesCount then
                                        use _ColorStates = fixed _ColorStatesoutputs
                                        let _SampleMask = Descriptor.SampleMask
                                        let _AlphaToCoverageEnabled = (if Descriptor.AlphaToCoverageEnabled then 1 else 0)
                                        let mutable _DescriptorValue = Unchecked.defaultof<DawnRaw.WGPURenderPipelineDescriptor>
                                        _DescriptorValue.Next <- 0n
                                        _DescriptorValue.Label <- _Label
                                        _DescriptorValue.Layout <- _Layout
                                        _DescriptorValue.VertexStage <- _VertexStage
                                        _DescriptorValue.FragmentStage <- _FragmentStage
                                        _DescriptorValue.VertexState <- _VertexState
                                        _DescriptorValue.PrimitiveTopology <- _PrimitiveTopology
                                        _DescriptorValue.RasterizationState <- _RasterizationState
                                        _DescriptorValue.SampleCount <- _SampleCount
                                        _DescriptorValue.DepthStencilState <- _DepthStencilState
                                        _DescriptorValue.ColorStatesCount <- _ColorStatesCount
                                        _DescriptorValue.ColorStates <- _ColorStates
                                        _DescriptorValue.SampleMask <- _SampleMask
                                        _DescriptorValue.AlphaToCoverageEnabled <- _AlphaToCoverageEnabled
                                        let _DescriptorValue = _DescriptorValue
                                        let _Descriptor = NativePtr.stackalloc 1
                                        NativePtr.write _Descriptor _DescriptorValue
                                        new RenderPipeline(x, DawnRaw.wgpuDeviceCreateRenderPipeline(x.Handle, _Descriptor))
                                    else
                                        let _Format = _ColorStatesinputs.[_ColorStatesi].Format
                                        let _Operation = _ColorStatesinputs.[_ColorStatesi].AlphaBlend.Operation
                                        let _SrcFactor = _ColorStatesinputs.[_ColorStatesi].AlphaBlend.SrcFactor
                                        let _DstFactor = _ColorStatesinputs.[_ColorStatesi].AlphaBlend.DstFactor
                                        let mutable _AlphaBlend = Unchecked.defaultof<DawnRaw.WGPUBlendDescriptor>
                                        _AlphaBlend.Operation <- _Operation
                                        _AlphaBlend.SrcFactor <- _SrcFactor
                                        _AlphaBlend.DstFactor <- _DstFactor
                                        let _AlphaBlend = _AlphaBlend
                                        let _Operation = _ColorStatesinputs.[_ColorStatesi].ColorBlend.Operation
                                        let _SrcFactor = _ColorStatesinputs.[_ColorStatesi].ColorBlend.SrcFactor
                                        let _DstFactor = _ColorStatesinputs.[_ColorStatesi].ColorBlend.DstFactor
                                        let mutable _ColorBlend = Unchecked.defaultof<DawnRaw.WGPUBlendDescriptor>
                                        _ColorBlend.Operation <- _Operation
                                        _ColorBlend.SrcFactor <- _SrcFactor
                                        _ColorBlend.DstFactor <- _DstFactor
                                        let _ColorBlend = _ColorBlend
                                        let _WriteMask = _ColorStatesinputs.[_ColorStatesi].WriteMask
                                        let mutable _n = Unchecked.defaultof<DawnRaw.WGPUColorStateDescriptor>
                                        _n.Next <- 0n
                                        _n.Format <- _Format
                                        _n.AlphaBlend <- _AlphaBlend
                                        _n.ColorBlend <- _ColorBlend
                                        _n.WriteMask <- _WriteMask
                                        let _n = _n
                                        _ColorStatesoutputs.[_ColorStatesi] <- _n
                                        _ColorStatesCont _ColorStatesinputs _ColorStatesoutputs (_ColorStatesi + 1)
                                _ColorStatesCont Descriptor.ColorStates (if _ColorStatesCount > 0 then Array.zeroCreate _ColorStatesCount else null) 0
                            match Descriptor.DepthStencilState with
                            | Some v ->
                                let _Format = v.Format
                                let _DepthWriteEnabled = (if v.DepthWriteEnabled then 1 else 0)
                                let _DepthCompare = v.DepthCompare
                                let _Compare = v.StencilFront.Compare
                                let _FailOp = v.StencilFront.FailOp
                                let _DepthFailOp = v.StencilFront.DepthFailOp
                                let _PassOp = v.StencilFront.PassOp
                                let mutable _StencilFront = Unchecked.defaultof<DawnRaw.WGPUStencilStateFaceDescriptor>
                                _StencilFront.Compare <- _Compare
                                _StencilFront.FailOp <- _FailOp
                                _StencilFront.DepthFailOp <- _DepthFailOp
                                _StencilFront.PassOp <- _PassOp
                                let _StencilFront = _StencilFront
                                let _Compare = v.StencilBack.Compare
                                let _FailOp = v.StencilBack.FailOp
                                let _DepthFailOp = v.StencilBack.DepthFailOp
                                let _PassOp = v.StencilBack.PassOp
                                let mutable _StencilBack = Unchecked.defaultof<DawnRaw.WGPUStencilStateFaceDescriptor>
                                _StencilBack.Compare <- _Compare
                                _StencilBack.FailOp <- _FailOp
                                _StencilBack.DepthFailOp <- _DepthFailOp
                                _StencilBack.PassOp <- _PassOp
                                let _StencilBack = _StencilBack
                                let _StencilReadMask = v.StencilReadMask
                                let _StencilWriteMask = v.StencilWriteMask
                                let mutable _n = Unchecked.defaultof<DawnRaw.WGPUDepthStencilStateDescriptor>
                                _n.Next <- 0n
                                _n.Format <- _Format
                                _n.DepthWriteEnabled <- _DepthWriteEnabled
                                _n.DepthCompare <- _DepthCompare
                                _n.StencilFront <- _StencilFront
                                _n.StencilBack <- _StencilBack
                                _n.StencilReadMask <- _StencilReadMask
                                _n.StencilWriteMask <- _StencilWriteMask
                                let _n = _n
                                let ptr = NativePtr.stackalloc 1
                                NativePtr.write ptr _n
                                _DepthStencilStateCont ptr
                            | None -> _DepthStencilStateCont (NativePtr.ofNativeInt 0n)
                        match Descriptor.RasterizationState with
                        | Some v ->
                            let _FrontFace = v.FrontFace
                            let _CullMode = v.CullMode
                            let _DepthBias = v.DepthBias
                            let _DepthBiasSlopeScale = v.DepthBiasSlopeScale
                            let _DepthBiasClamp = v.DepthBiasClamp
                            let mutable _n = Unchecked.defaultof<DawnRaw.WGPURasterizationStateDescriptor>
                            _n.Next <- 0n
                            _n.FrontFace <- _FrontFace
                            _n.CullMode <- _CullMode
                            _n.DepthBias <- _DepthBias
                            _n.DepthBiasSlopeScale <- _DepthBiasSlopeScale
                            _n.DepthBiasClamp <- _DepthBiasClamp
                            let _n = _n
                            let ptr = NativePtr.stackalloc 1
                            NativePtr.write ptr _n
                            _RasterizationStateCont ptr
                        | None -> _RasterizationStateCont (NativePtr.ofNativeInt 0n)
                    match Descriptor.VertexState with
                    | Some v ->
                        let _IndexFormat = v.IndexFormat
                        let _VertexBuffersCount = if isNull v.VertexBuffers then 0 else v.VertexBuffers.Length
                        let rec _VertexBuffersCont (_VertexBuffersinputs : array<VertexBufferLayoutDescriptor>) (_VertexBuffersoutputs : array<DawnRaw.WGPUVertexBufferLayoutDescriptor>) (_VertexBuffersi : int) =
                            if _VertexBuffersi >= _VertexBuffersCount then
                                use _VertexBuffers = fixed _VertexBuffersoutputs
                                let mutable _n = Unchecked.defaultof<DawnRaw.WGPUVertexStateDescriptor>
                                _n.Next <- 0n
                                _n.IndexFormat <- _IndexFormat
                                _n.VertexBuffersCount <- _VertexBuffersCount
                                _n.VertexBuffers <- _VertexBuffers
                                let _n = _n
                                let ptr = NativePtr.stackalloc 1
                                NativePtr.write ptr _n
                                _VertexStateCont ptr
                            else
                                let _ArrayStride = _VertexBuffersinputs.[_VertexBuffersi].ArrayStride
                                let _StepMode = _VertexBuffersinputs.[_VertexBuffersi].StepMode
                                let _AttributesCount = if isNull _VertexBuffersinputs.[_VertexBuffersi].Attributes then 0 else _VertexBuffersinputs.[_VertexBuffersi].Attributes.Length
                                let rec _AttributesCont (_Attributesinputs : array<VertexAttributeDescriptor>) (_Attributesoutputs : array<DawnRaw.WGPUVertexAttributeDescriptor>) (_Attributesi : int) =
                                    if _Attributesi >= _AttributesCount then
                                        use _Attributes = fixed _Attributesoutputs
                                        let mutable _n = Unchecked.defaultof<DawnRaw.WGPUVertexBufferLayoutDescriptor>
                                        _n.ArrayStride <- _ArrayStride
                                        _n.StepMode <- _StepMode
                                        _n.AttributesCount <- _AttributesCount
                                        _n.Attributes <- _Attributes
                                        let _n = _n
                                        _VertexBuffersoutputs.[_VertexBuffersi] <- _n
                                        _VertexBuffersCont _VertexBuffersinputs _VertexBuffersoutputs (_VertexBuffersi + 1)
                                    else
                                        let _Format = _Attributesinputs.[_Attributesi].Format
                                        let _Offset = _Attributesinputs.[_Attributesi].Offset
                                        let _ShaderLocation = _Attributesinputs.[_Attributesi].ShaderLocation
                                        let mutable _n = Unchecked.defaultof<DawnRaw.WGPUVertexAttributeDescriptor>
                                        _n.Format <- _Format
                                        _n.Offset <- _Offset
                                        _n.ShaderLocation <- _ShaderLocation
                                        let _n = _n
                                        _Attributesoutputs.[_Attributesi] <- _n
                                        _AttributesCont _Attributesinputs _Attributesoutputs (_Attributesi + 1)
                                _AttributesCont _VertexBuffersinputs.[_VertexBuffersi].Attributes (if _AttributesCount > 0 then Array.zeroCreate _AttributesCount else null) 0
                        _VertexBuffersCont v.VertexBuffers (if _VertexBuffersCount > 0 then Array.zeroCreate _VertexBuffersCount else null) 0
                    | None -> _VertexStateCont (NativePtr.ofNativeInt 0n)
                match Descriptor.FragmentStage with
                | Some v ->
                    let _Module = (if isNull v.Module then ShaderModuleHandle.Null else v.Module.Handle)
                    let inline _EntryPointCont (_EntryPoint) = 
                        let mutable _n = Unchecked.defaultof<DawnRaw.WGPUProgrammableStageDescriptor>
                        _n.Next <- 0n
                        _n.Module <- _Module
                        _n.EntryPoint <- _EntryPoint
                        let _n = _n
                        let ptr = NativePtr.stackalloc 1
                        NativePtr.write ptr _n
                        _FragmentStageCont ptr
                    if not (isNull v.EntryPoint) then
                        let _EntryPointLen = System.Text.Encoding.UTF8.GetByteCount v.EntryPoint
                        let _EntryPointSize = _EntryPointLen + 1
                        let _EntryPointPtr = NativePtr.stackalloc<byte> _EntryPointSize
                        System.Text.Encoding.UTF8.GetBytes(v.EntryPoint.AsSpan(), Span<byte>(NativePtr.toVoidPtr _EntryPointPtr, _EntryPointSize)) |> ignore
                        NativePtr.set _EntryPointPtr _EntryPointLen 0uy
                        _EntryPointCont (NativePtr.toNativeInt _EntryPointPtr)
                    else
                        _EntryPointCont 0n
                | None -> _FragmentStageCont (NativePtr.ofNativeInt 0n)
            if not (isNull Descriptor.VertexStage.EntryPoint) then
                let _EntryPointLen = System.Text.Encoding.UTF8.GetByteCount Descriptor.VertexStage.EntryPoint
                let _EntryPointSize = _EntryPointLen + 1
                let _EntryPointPtr = NativePtr.stackalloc<byte> _EntryPointSize
                System.Text.Encoding.UTF8.GetBytes(Descriptor.VertexStage.EntryPoint.AsSpan(), Span<byte>(NativePtr.toVoidPtr _EntryPointPtr, _EntryPointSize)) |> ignore
                NativePtr.set _EntryPointPtr _EntryPointLen 0uy
                _EntryPointCont (NativePtr.toNativeInt _EntryPointPtr)
            else
                _EntryPointCont 0n
        if not (isNull Descriptor.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount Descriptor.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(Descriptor.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
    member inline x.CreateSampler() : Sampler = 
        let inline _LabelCont (_Label) = 
            let _AddressModeU = SamplerDescriptor.Default.AddressModeU
            let _AddressModeV = SamplerDescriptor.Default.AddressModeV
            let _AddressModeW = SamplerDescriptor.Default.AddressModeW
            let _MagFilter = SamplerDescriptor.Default.MagFilter
            let _MinFilter = SamplerDescriptor.Default.MinFilter
            let _MipmapFilter = SamplerDescriptor.Default.MipmapFilter
            let _LodMinClamp = SamplerDescriptor.Default.LodMinClamp
            let _LodMaxClamp = SamplerDescriptor.Default.LodMaxClamp
            let _Compare = SamplerDescriptor.Default.Compare
            let mutable _DescriptorValue = Unchecked.defaultof<DawnRaw.WGPUSamplerDescriptor>
            _DescriptorValue.Next <- 0n
            _DescriptorValue.Label <- _Label
            _DescriptorValue.AddressModeU <- _AddressModeU
            _DescriptorValue.AddressModeV <- _AddressModeV
            _DescriptorValue.AddressModeW <- _AddressModeW
            _DescriptorValue.MagFilter <- _MagFilter
            _DescriptorValue.MinFilter <- _MinFilter
            _DescriptorValue.MipmapFilter <- _MipmapFilter
            _DescriptorValue.LodMinClamp <- _LodMinClamp
            _DescriptorValue.LodMaxClamp <- _LodMaxClamp
            _DescriptorValue.Compare <- _Compare
            let _DescriptorValue = _DescriptorValue
            let _Descriptor = NativePtr.stackalloc 1
            NativePtr.write _Descriptor _DescriptorValue
            new Sampler(x, DawnRaw.wgpuDeviceCreateSampler(x.Handle, _Descriptor))
        if not (isNull SamplerDescriptor.Default.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount SamplerDescriptor.Default.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(SamplerDescriptor.Default.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
    member inline x.CreateSampler(Descriptor : SamplerDescriptor) : Sampler = 
        let inline _LabelCont (_Label) = 
            let _AddressModeU = Descriptor.AddressModeU
            let _AddressModeV = Descriptor.AddressModeV
            let _AddressModeW = Descriptor.AddressModeW
            let _MagFilter = Descriptor.MagFilter
            let _MinFilter = Descriptor.MinFilter
            let _MipmapFilter = Descriptor.MipmapFilter
            let _LodMinClamp = Descriptor.LodMinClamp
            let _LodMaxClamp = Descriptor.LodMaxClamp
            let _Compare = Descriptor.Compare
            let mutable _DescriptorValue = Unchecked.defaultof<DawnRaw.WGPUSamplerDescriptor>
            _DescriptorValue.Next <- 0n
            _DescriptorValue.Label <- _Label
            _DescriptorValue.AddressModeU <- _AddressModeU
            _DescriptorValue.AddressModeV <- _AddressModeV
            _DescriptorValue.AddressModeW <- _AddressModeW
            _DescriptorValue.MagFilter <- _MagFilter
            _DescriptorValue.MinFilter <- _MinFilter
            _DescriptorValue.MipmapFilter <- _MipmapFilter
            _DescriptorValue.LodMinClamp <- _LodMinClamp
            _DescriptorValue.LodMaxClamp <- _LodMaxClamp
            _DescriptorValue.Compare <- _Compare
            let _DescriptorValue = _DescriptorValue
            let _Descriptor = NativePtr.stackalloc 1
            NativePtr.write _Descriptor _DescriptorValue
            new Sampler(x, DawnRaw.wgpuDeviceCreateSampler(x.Handle, _Descriptor))
        if not (isNull Descriptor.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount Descriptor.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(Descriptor.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
    member inline x.CreateShaderModule() : ShaderModule = 
        let inline _LabelCont (_Label) = 
            let mutable _DescriptorValue = Unchecked.defaultof<DawnRaw.WGPUShaderModuleDescriptor>
            _DescriptorValue.Next <- 0n
            _DescriptorValue.Label <- _Label
            let _DescriptorValue = _DescriptorValue
            let _Descriptor = NativePtr.stackalloc 1
            NativePtr.write _Descriptor _DescriptorValue
            new ShaderModule(x, DawnRaw.wgpuDeviceCreateShaderModule(x.Handle, _Descriptor))
        if not (isNull ShaderModuleDescriptor.Default.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount ShaderModuleDescriptor.Default.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(ShaderModuleDescriptor.Default.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
    member inline x.CreateShaderModule(Descriptor : ShaderModuleDescriptor) : ShaderModule = 
        let inline _LabelCont (_Label) = 
            let mutable _DescriptorValue = Unchecked.defaultof<DawnRaw.WGPUShaderModuleDescriptor>
            _DescriptorValue.Next <- 0n
            _DescriptorValue.Label <- _Label
            let _DescriptorValue = _DescriptorValue
            let _Descriptor = NativePtr.stackalloc 1
            NativePtr.write _Descriptor _DescriptorValue
            new ShaderModule(x, DawnRaw.wgpuDeviceCreateShaderModule(x.Handle, _Descriptor))
        if not (isNull Descriptor.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount Descriptor.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(Descriptor.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
    member inline x.CreateSwapChain(Surface : Surface, Descriptor : SwapChainDescriptor) : SwapChain = 
        let _Surface = (if isNull Surface then SurfaceHandle.Null else Surface.Handle)
        let inline _LabelCont (_Label) = 
            let _Usage = Descriptor.Usage
            let _Format = Descriptor.Format
            let _Width = Descriptor.Width
            let _Height = Descriptor.Height
            let _PresentMode = Descriptor.PresentMode
            let _Implementation = Descriptor.Implementation
            let mutable _DescriptorValue = Unchecked.defaultof<DawnRaw.WGPUSwapChainDescriptor>
            _DescriptorValue.Next <- 0n
            _DescriptorValue.Label <- _Label
            _DescriptorValue.Usage <- _Usage
            _DescriptorValue.Format <- _Format
            _DescriptorValue.Width <- _Width
            _DescriptorValue.Height <- _Height
            _DescriptorValue.PresentMode <- _PresentMode
            _DescriptorValue.Implementation <- _Implementation
            let _DescriptorValue = _DescriptorValue
            let _Descriptor = NativePtr.stackalloc 1
            NativePtr.write _Descriptor _DescriptorValue
            new SwapChain(x, DawnRaw.wgpuDeviceCreateSwapChain(x.Handle, _Surface, _Descriptor))
        if not (isNull Descriptor.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount Descriptor.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(Descriptor.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
    member inline x.CreateTexture(Descriptor : TextureDescriptor) : Texture = 
        let inline _LabelCont (_Label) = 
            let _Usage = Descriptor.Usage
            let _Dimension = Descriptor.Dimension
            let _Width = Descriptor.Size.Width
            let _Height = Descriptor.Size.Height
            let _Depth = Descriptor.Size.Depth
            let mutable _Size = Unchecked.defaultof<DawnRaw.WGPUExtent3D>
            _Size.Width <- _Width
            _Size.Height <- _Height
            _Size.Depth <- _Depth
            let _Size = _Size
            let _Format = Descriptor.Format
            let _MipLevelCount = Descriptor.MipLevelCount
            let _SampleCount = Descriptor.SampleCount
            let mutable _DescriptorValue = Unchecked.defaultof<DawnRaw.WGPUTextureDescriptor>
            _DescriptorValue.Next <- 0n
            _DescriptorValue.Label <- _Label
            _DescriptorValue.Usage <- _Usage
            _DescriptorValue.Dimension <- _Dimension
            _DescriptorValue.Size <- _Size
            _DescriptorValue.Format <- _Format
            _DescriptorValue.MipLevelCount <- _MipLevelCount
            _DescriptorValue.SampleCount <- _SampleCount
            let _DescriptorValue = _DescriptorValue
            let _Descriptor = NativePtr.stackalloc 1
            NativePtr.write _Descriptor _DescriptorValue
            new Texture(x, DawnRaw.wgpuDeviceCreateTexture(x.Handle, _Descriptor))
        if not (isNull Descriptor.Label) then
            let _LabelLen = System.Text.Encoding.UTF8.GetByteCount Descriptor.Label
            let _LabelSize = _LabelLen + 1
            let _LabelPtr = NativePtr.stackalloc<byte> _LabelSize
            System.Text.Encoding.UTF8.GetBytes(Descriptor.Label.AsSpan(), Span<byte>(NativePtr.toVoidPtr _LabelPtr, _LabelSize)) |> ignore
            NativePtr.set _LabelPtr _LabelLen 0uy
            _LabelCont (NativePtr.toNativeInt _LabelPtr)
        else
            _LabelCont 0n
    member inline x.GetDefaultQueue() : Queue = 
        new Queue(x, DawnRaw.wgpuDeviceGetDefaultQueue(x.Handle))
    member inline x.InjectError(Type : ErrorType, Message : string) : unit = 
        let _Type = Type
        let inline _MessageCont (_Message) = 
            DawnRaw.wgpuDeviceInjectError(x.Handle, _Type, _Message)
        if not (isNull Message) then
            let _MessageLen = System.Text.Encoding.UTF8.GetByteCount Message
            let _MessageSize = _MessageLen + 1
            let _MessagePtr = NativePtr.stackalloc<byte> _MessageSize
            System.Text.Encoding.UTF8.GetBytes(Message.AsSpan(), Span<byte>(NativePtr.toVoidPtr _MessagePtr, _MessageSize)) |> ignore
            NativePtr.set _MessagePtr _MessageLen 0uy
            _MessageCont (NativePtr.toNativeInt _MessagePtr)
        else
            _MessageCont 0n
    member inline x.LoseForTesting() : unit = 
        DawnRaw.wgpuDeviceLoseForTesting(x.Handle)
    member inline x.Tick() : unit = 
        DawnRaw.wgpuDeviceTick(x.Handle)
    member inline x.SetUncapturedErrorCallback(Callback : ErrorCallback) : unit = 
        let _CallbackFunction (Type : ErrorType) (Message : nativeint) (Userdata : nativeint) = 
            let _Type = Type
            let _Message = System.Runtime.InteropServices.Marshal.PtrToStringAnsi Message
            let _Userdata = Userdata
            Callback.Invoke(_Type, _Message, _Userdata)
        let _CallbackDel = WGPUErrorCallback(_CallbackFunction)
        let _CallbackGC = System.Runtime.InteropServices.GCHandle.Alloc(_CallbackDel)
        let _Callback = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(_CallbackDel)
        let _Userdata = 0n
        DawnRaw.wgpuDeviceSetUncapturedErrorCallback(x.Handle, _Callback, _Userdata)
    member inline x.SetUncapturedErrorCallback(Callback : ErrorCallback, Userdata : nativeint) : unit = 
        let _CallbackFunction (Type : ErrorType) (Message : nativeint) (Userdata : nativeint) = 
            let _Type = Type
            let _Message = System.Runtime.InteropServices.Marshal.PtrToStringAnsi Message
            let _Userdata = Userdata
            Callback.Invoke(_Type, _Message, _Userdata)
        let _CallbackDel = WGPUErrorCallback(_CallbackFunction)
        let _CallbackGC = System.Runtime.InteropServices.GCHandle.Alloc(_CallbackDel)
        let _Callback = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(_CallbackDel)
        let _Userdata = Userdata
        DawnRaw.wgpuDeviceSetUncapturedErrorCallback(x.Handle, _Callback, _Userdata)
    member inline x.SetDeviceLostCallback(Callback : DeviceLostCallback) : unit = 
        let _CallbackFunction (Message : nativeint) (Userdata : nativeint) = 
            let _Message = System.Runtime.InteropServices.Marshal.PtrToStringAnsi Message
            let _Userdata = Userdata
            Callback.Invoke(_Message, _Userdata)
        let _CallbackDel = WGPUDeviceLostCallback(_CallbackFunction)
        let _CallbackGC = System.Runtime.InteropServices.GCHandle.Alloc(_CallbackDel)
        let _Callback = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(_CallbackDel)
        let _Userdata = 0n
        DawnRaw.wgpuDeviceSetDeviceLostCallback(x.Handle, _Callback, _Userdata)
    member inline x.SetDeviceLostCallback(Callback : DeviceLostCallback, Userdata : nativeint) : unit = 
        let _CallbackFunction (Message : nativeint) (Userdata : nativeint) = 
            let _Message = System.Runtime.InteropServices.Marshal.PtrToStringAnsi Message
            let _Userdata = Userdata
            Callback.Invoke(_Message, _Userdata)
        let _CallbackDel = WGPUDeviceLostCallback(_CallbackFunction)
        let _CallbackGC = System.Runtime.InteropServices.GCHandle.Alloc(_CallbackDel)
        let _Callback = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(_CallbackDel)
        let _Userdata = Userdata
        DawnRaw.wgpuDeviceSetDeviceLostCallback(x.Handle, _Callback, _Userdata)
    member inline x.PushErrorScope(Filter : ErrorFilter) : unit = 
        let _Filter = Filter
        DawnRaw.wgpuDevicePushErrorScope(x.Handle, _Filter)
    member inline x.PopErrorScope(Callback : ErrorCallback) : bool = 
        let mutable _CallbackGC = Unchecked.defaultof<System.Runtime.InteropServices.GCHandle>
        let _CallbackFunction (Type : ErrorType) (Message : nativeint) (Userdata : nativeint) = 
            let _Type = Type
            let _Message = System.Runtime.InteropServices.Marshal.PtrToStringAnsi Message
            let _Userdata = Userdata
            if _CallbackGC.IsAllocated then _CallbackGC.Free()
            Callback.Invoke(_Type, _Message, _Userdata)
        let _CallbackDel = WGPUErrorCallback(_CallbackFunction)
        _CallbackGC <- System.Runtime.InteropServices.GCHandle.Alloc(_CallbackDel)
        let _Callback = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(_CallbackDel)
        let _Userdata = 0n
        DawnRaw.wgpuDevicePopErrorScope(x.Handle, _Callback, _Userdata) <> 0
    member inline x.PopErrorScope(Callback : ErrorCallback, Userdata : nativeint) : bool = 
        let mutable _CallbackGC = Unchecked.defaultof<System.Runtime.InteropServices.GCHandle>
        let _CallbackFunction (Type : ErrorType) (Message : nativeint) (Userdata : nativeint) = 
            let _Type = Type
            let _Message = System.Runtime.InteropServices.Marshal.PtrToStringAnsi Message
            let _Userdata = Userdata
            if _CallbackGC.IsAllocated then _CallbackGC.Free()
            Callback.Invoke(_Type, _Message, _Userdata)
        let _CallbackDel = WGPUErrorCallback(_CallbackFunction)
        _CallbackGC <- System.Runtime.InteropServices.GCHandle.Alloc(_CallbackDel)
        let _Callback = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(_CallbackDel)
        let _Userdata = Userdata
        DawnRaw.wgpuDevicePopErrorScope(x.Handle, _Callback, _Userdata) <> 0
