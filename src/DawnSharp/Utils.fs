namespace WebGPU

open System.Runtime.InteropServices
open System.Reflection
open Aardvark.Base
open Aardvark.Base.IL
open System
open Microsoft.FSharp.NativeInterop
open WebGPU

#nowarn "9"

type private SetTail<'a> private() =
    static let tailProperty =
        let tailField = typeof<list<'a>>.GetField("tail", BindingFlags.NonPublic ||| BindingFlags.Instance)
        tailField

    static let setTail : list<'a> -> list<'a> -> unit =
        cil {
            do! IL.ldarg 0
            do! IL.ldarg 1
            do! IL.stfld tailProperty
            do! IL.ret
        }


    static member SetTail(l : list<'a>, tail : list<'a>) =
        setTail l tail

type ListBuilder<'a>() =
    let mutable start = []
    let mutable current = start

    member x.Clear() =
        start <- []
        current <- start

    member x.Prepend(value : 'a) =
        match start with
        | [] ->
            start <- [value]
            current <- start
        | _ -> 
            start <- value :: start

    member x.Append(value : 'a) =
        match start with
        | [] ->
            start <- [value]
            current <- start
        | _ ->
            let m = [value]
            SetTail<'a>.SetTail(current, m)
            current <- m
                
    member x.Append(value : list<'a>) =
        match value with
        | [] -> ()
        | h :: t ->
            match start with
            | [] ->
                start <- [h]
                current <- start
            | _ ->
                let m = [h]
                SetTail<'a>.SetTail(current, m)
                current <- m

            for e in t do 
                let m = [e]
                SetTail<'a>.SetTail(current, m)
                current <- m

    member x.ToList() =
        let res = start
        start <- []
        current <- start
        res

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module VertexFormat = 

    let private elementSizes =
        [|
            2  // | UChar2 = 0
            4  // | UChar4 = 1
            2  // | Char2 = 2
            4  // | Char4 = 3
            2  // | UChar2Norm = 4
            4  // | UChar4Norm = 5
            2  // | Char2Norm = 6
            4  // | Char4Norm = 7
            4  // | UShort2 = 8
            8  // | UShort4 = 9
            4  // | Short2 = 10
            8  // | Short4 = 11
            4  // | UShort2Norm = 12
            8  // | UShort4Norm = 13
            4  // | Short2Norm = 14
            8  // | Short4Norm = 15
            4  // | Half2 = 16
            8  // | Half4 = 17
            4  // | Float = 18
            8  // | Float2 = 19
            12 // | Float3 = 20
            16 // | Float4 = 21
            4  // | UInt = 22
            8  // | UInt2 = 23
            12 // | UInt3 = 24
            16 // | UInt4 = 25
            4  // | Int = 26
            8  // | Int2 = 27
            12 // | Int3 = 28
            16 // | Int4 = 29
        |]

    let private formats =
        Dict.ofArray [|
            typeof<C4b>, VertexFormat.UChar4Norm

            typeof<float32>, VertexFormat.Float
            typeof<V2f>, VertexFormat.Float2
            typeof<V3f>, VertexFormat.Float3
            typeof<V4f>, VertexFormat.Float4
            typeof<int>, VertexFormat.Int
            typeof<V2i>, VertexFormat.Int2
            typeof<V3i>, VertexFormat.Int3
            typeof<V4i>, VertexFormat.Int4

            typeof<C4us>, VertexFormat.UShort4Norm

            typeof<uint32>, VertexFormat.UInt
            typeof<C3ui>, VertexFormat.UInt2
            typeof<C4ui>, VertexFormat.UInt3
        |]
        
    let ofType (t : Type) =
        match formats.TryGetValue t with
        | (true, fmt) -> fmt
        | _ -> failwithf "[VertexFormat] unsupported type: %A" t.FullName

    let size (fmt : VertexFormat) =
        let id = int fmt
        if id < 0 || id >= elementSizes.Length then 
            failwithf "[VertexFormat] unsupported format: %A" fmt
        elementSizes.[id]


[<AbstractClass>]
type Data() = 
    abstract member Size : nativeint
    abstract member CopyTo : dst : nativeint -> unit
    abstract member CopyTo<'a when 'a : unmanaged> : dst : Span<'a> -> unit
    abstract member Format : VertexFormat

    member x.ElementSize = VertexFormat.size x.Format
    member x.Count = x.Size / nativeint x.ElementSize |> int

    member x.CopyTo(array : 'a[], index : int) = 
        let cnt = x.Size / nativeint sizeof<'a> |> int
        x.CopyTo(Span(array, index, cnt))
        
    member x.CopyTo(array : 'a[]) = 
        let cnt = x.Size / nativeint sizeof<'a> |> int
        x.CopyTo(Span(array, 0, cnt))

    member x.CopyTo(ptr : nativeptr<'a>) =
        x.CopyTo(NativePtr.toNativeInt ptr)

    static member Create (array : 'a[]) =   
        Data<'a>(VertexFormat.ofType typeof<'a>, Memory array) :> Data
        
    static member Create (memory : Memory<'a>) =   
        Data<'a>(VertexFormat.ofType typeof<'a>, memory) :> Data

    static member Create (array : 'a[], offset : int, count : int) =   
        Data<'a>(VertexFormat.ofType typeof<'a>, Memory(array, offset, count)) :> Data

    static member Create (array : 'a[], offset : int) =   
        Data<'a>(VertexFormat.ofType typeof<'a>, Memory(array, offset, array.Length - offset)) :> Data
        
    static member Create (fmt : VertexFormat, array : 'a[]) =   
        Data<'a>(fmt, Memory array) :> Data
        
    static member Create (fmt : VertexFormat, memory : Memory<'a>) =   
        Data<'a>(fmt, memory) :> Data

    static member Create (fmt : VertexFormat, array : 'a[], offset : int, count : int) =   
        Data<'a>(fmt, Memory(array, offset, count)) :> Data

    static member Create (fmt : VertexFormat, array : 'a[], offset : int) =   
        Data<'a>(fmt, Memory(array, offset, array.Length - offset)) :> Data
        
    static member Create (fmt : VertexFormat, ptr : nativeint, size : nativeint) =   
        NativeData(fmt, ptr, size) :> Data

and private Data<'a when 'a : unmanaged>(fmt : VertexFormat, memory : Memory<'a>) =
    inherit Data()
    static let sa = nativeint sizeof<'a>

    //let fmt = VertexFormat.ofType typeof<'a>
    let size = nativeint memory.Length * sa

    override x.Format =
        fmt

    override x.Size = 
        size

    override x.CopyTo (dst : nativeint) =
        let dstSpan = Span<'a>(NativePtr.toVoidPtr (NativePtr.ofNativeInt<'a> dst), memory.Length )
        memory.Span.CopyTo(dstSpan)
        
    override x.CopyTo (dst : Span<'x>) =
        let cnt = nativeint (memory.Length * sizeof<'a>) / nativeint sizeof<'x> |> int
        use h = memory.Pin()
        let src = Span<'x>(NativePtr.toVoidPtr (NativePtr.ofVoidPtr<'x> h.Pointer), cnt)
        src.CopyTo(dst)

and private NativeData(fmt : VertexFormat, ptr : nativeint, size : nativeint) =
    inherit Data()

    override x.Format =
        fmt

    override x.Size = 
        size

    override x.CopyTo (dst : nativeint) =
        Marshal.Copy(ptr, dst, size)
        
    override x.CopyTo (dst : Span<'x>) =    
        let cnt = size / nativeint sizeof<'x> |> int
        let src = Span<'x>(NativePtr.toVoidPtr (NativePtr.ofNativeInt<'x> ptr), cnt)
        src.CopyTo dst
