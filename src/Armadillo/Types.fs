namespace Armadillo

open Aardvark.Base
open WebGPU

[<Struct>]
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
    
[<Struct>]
type DepthTestMode =
    {
        compare     : CompareFunction
        write       : bool
    }
    static member Default = { compare = CompareFunction.Always; write = true }

[<Struct>]
type StencilMode =
    {
        readMask    : uint32
        writeMask   : uint32
        front       : StencilStateFaceDescriptor
        back        : StencilStateFaceDescriptor
    }

    static member Default = { readMask = 0u; writeMask = 0u; front = StencilStateFaceDescriptor.Default; back = StencilStateFaceDescriptor.Default }


type BufferDescriptor =
    | Data of Data
    | Buffer of fmt : VertexFormat * slotCnt : int * buffer : Buffer * offset : nativeint * size : nativeint

    member x.Stride =
        VertexFormat.size x.Format // TODO!!!!

    //member x.Offset = 
    //    match x with
    //    | Data _ -> 0UL
    //    | Buffer(_,_,o,_) -> uint64 o

    //member x.Size =
    //    match x with
    //    | Data d -> d.Size
    //    | Buffer(_, _, _, s) -> s

    member x.Format =
        match x with
        | Data d -> d.Format
        | Buffer(f,_,_,_,_) -> f
        
    member x.SlotCount =
        match x with
        | Data d -> d.SlotCount
        | Buffer(_,c,_,_,_) -> c
