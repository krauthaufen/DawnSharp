namespace WebGPU

open Aardvark.Base

type ITypeVisitor<'r> =
    abstract member Accept<'a when 'a : unmanaged> : unit -> 'r

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module GLSLType =
    open FShade
    open FShade.GLSL
    
    let private lookupTable (s : seq<'a * 'b>) =
        let d = Aardvark.Base.Dict()
        for (k, v) in s do d.[k] <- v
        fun k ->
            match d.TryGetValue(k) with
            | (true, v) -> v
            | _ -> failwithf "no value for: %A" k

    let toType =
        lookupTable [
            Bool, typeof<int>
            Void, typeof<unit>
            
            Int(true, 8), typeof<sbyte>
            Int(true, 16), typeof<int16>
            Int(true, 32), typeof<int32>
            Int(true, 64), typeof<int64>

            Int(false, 8), typeof<byte>
            Int(false, 16), typeof<uint16>
            Int(false, 32), typeof<uint32>
            Int(false, 64), typeof<uint64>

            Float(16), typeof<float16>
            Float(32), typeof<float32>
            Float(64), typeof<float32>

            Vec(2, Int(true, 32)), typeof<V2i>
            Vec(3, Int(true, 32)), typeof<V3i>
            Vec(4, Int(true, 32)), typeof<V4i>

            Vec(3, Int(false, 32)), typeof<C3ui>
            Vec(4, Int(false, 32)), typeof<C4ui>
            
            Vec(2, Float(32)), typeof<V2f>
            Vec(3, Float(32)), typeof<V3f>
            Vec(4, Float(32)), typeof<V4f>

            Vec(2, Float(64)), typeof<V2f>
            Vec(3, Float(64)), typeof<V3f>
            Vec(4, Float(64)), typeof<V4f>

            Mat(2,2,Int(true,32)), typeof<M22i>
            Mat(2,3,Int(true,32)), typeof<M23i>
            Mat(3,3,Int(true,32)), typeof<M34i>
            Mat(3,4,Int(true,32)), typeof<M34i>
            Mat(4,4,Int(true,32)), typeof<M44i>

            Mat(2,2,Float(32)), typeof<M22f>
            Mat(2,3,Float(32)), typeof<M23f>
            Mat(3,3,Float(32)), typeof<M34f>
            Mat(3,4,Float(32)), typeof<M34f>
            Mat(4,4,Float(32)), typeof<M44f>

            Mat(2,2,Float(64)), typeof<M22f>
            Mat(2,3,Float(64)), typeof<M23f>
            Mat(3,3,Float(64)), typeof<M34f>
            Mat(3,4,Float(64)), typeof<M34f>
            Mat(4,4,Float(64)), typeof<M44f>
            
        ]
    
    let rec sizeof (t : GLSLType) =
        match t with
            | Bool -> 4
            | Int(_,b) -> b / 8
            | Float(64 | 32) -> 4
            | Float(w) -> w / 4
            | Vec(d,e) -> d * sizeof e
            | Mat(r,c,e) -> r * c * sizeof e
            | Array(len, e, stride) -> len * stride
            | Struct(_,f,size) -> size
            | Void -> failwith "[UniformWriter] void does not have a size"
            | Image _ -> failwith "[UniformWriter] image does not have a size"
            | Sampler _ -> failwith "[UniformWriter] sampler does not have a size"
            | DynamicArray _ -> failwith "[UniformWriter] dynamic arrays do not have a size"
            | Intrinsic _ -> failwith "[UniformWriter] dynamic arrays do not have a size"

    let visit (tv : ITypeVisitor<'r>) (t : GLSLType) =
        match t with
        | Bool -> tv.Accept<int> ()

        | Int(true, 8) -> tv.Accept<int8> ()
        | Int(true, 16) -> tv.Accept<int16> ()
        | Int(true, 32) -> tv.Accept<int32> ()
        | Int(true, 64) -> tv.Accept<int64> ()
        | Int(false, 8) -> tv.Accept<uint8> ()
        | Int(false, 16) -> tv.Accept<uint16> ()
        | Int(false, 32) -> tv.Accept<uint32> ()
        | Int(false, 64) -> tv.Accept<uint64> ()
        | Int _ -> failwith "bad integer"

        | Float 16 -> tv.Accept<float16> ()
        | Float _ -> tv.Accept<float32> ()
        
        | Vec(2, Int(true, 32)) -> tv.Accept<V2i> ()
        | Vec(3, Int(true, 32)) -> tv.Accept<V3i> ()
        | Vec(4, Int(true, 32)) -> tv.Accept<V4i> ()
        
        | Vec(2, Int(true, 64)) -> tv.Accept<V2l> ()
        | Vec(3, Int(true, 64)) -> tv.Accept<V3l> ()
        | Vec(4, Int(true, 64)) -> tv.Accept<V4l> ()

        | Vec(2, Float _) -> tv.Accept<V2f> ()
        | Vec(3, Float _) -> tv.Accept<V3f> ()
        | Vec(4, Float _) -> tv.Accept<V4f> ()
        
        | Vec(3, Int(false, 8)) -> tv.Accept<C3b> ()
        | Vec(4, Int(false, 8)) -> tv.Accept<C4b> ()
        | Vec(3, Int(false, 16)) -> tv.Accept<C3us> ()
        | Vec(4, Int(false, 16)) -> tv.Accept<C4us> ()
        | Vec(3, Int(false, 32)) -> tv.Accept<C3ui> ()
        | Vec(4, Int(false, 32)) -> tv.Accept<C4ui> ()
        | Vec _ -> failwith "bad vec"

        | Mat(2, 2, Float _) -> tv.Accept<M22f> ()
        | Mat(2, 2, Int(true, 32)) -> tv.Accept<M22i> ()
        | Mat(2, 3, Float _) -> tv.Accept<M23f> ()
        | Mat(2, 3, Int(true, 32)) -> tv.Accept<M23i> ()
        | Mat(3, 3, Float _) -> tv.Accept<M33f> ()
        | Mat(3, 3, Int(true, 32)) -> tv.Accept<M33i> ()
        | Mat(3, 4, Float _) -> tv.Accept<M34f> ()
        | Mat(3, 4, Int(true, 32)) -> tv.Accept<M34i> ()
        | Mat(4, 4, Float _) -> tv.Accept<M44f> ()
        | Mat(4, 4, Int(true, 32)) -> tv.Accept<M44i> ()
        | Mat _ -> failwith "bad mat"

        | Array _ | Struct _ -> failwith ""
        | Void -> failwith "[UniformWriter] void does not have a size"
        | Image _ -> failwith "[UniformWriter] image does not have a size"
        | Sampler _ -> failwith "[UniformWriter] sampler does not have a size"
        | DynamicArray _ -> failwith "[UniformWriter] dynamic arrays do not have a size"
        | Intrinsic _ -> failwith "[UniformWriter] dynamic arrays do not have a size"

        

