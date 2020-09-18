#r "System.Collections.Immutable.dll"
#r @"../../packages/Newtonsoft.Json/lib/netstandard2.0/Newtonsoft.Json.dll"

open System
open Newtonsoft.Json
open Newtonsoft.Json.Linq
open System.IO

let dawnDir = Path.Combine(__SOURCE_DIRECTORY__, "..", "..", "packages", "build", "dawn")

type Parameter =
    {
        name        : string
        typ         : string
        annotation  : option<string>
        def         : option<string>
    }

type Method =
    {
        name    : string
        ret     : option<string>
        args    : list<Parameter>
    }

type Entry =
    | Struct of name : string * extensible : bool * members : list<Parameter>
    | Enum of name : string * flags : bool * values : list<string * int>
    | Object of name : string * methods : list<Method>
    | Native of name : string
    | Callback of name : string * args : list<Parameter>

    member x.Name =
        match x with
        | Struct(name,_,_) -> name
        | Enum(name,_,_) -> name
        | Object(name,_) -> name
        | Native(name) -> name
        | Callback(name,_) -> name

[<AutoOpen>]
module Helpers =
    
    let prop (v : JToken) (name : string) : option<'a> =
        if isNull v then None
        else
            match v with
            | :? JObject as o -> 
                let p = o.Property name
                if isNull p then None
                else 
                    if typeof<JToken>.IsAssignableFrom typeof<'a> then
                        match p.Value :> obj with
                        | :? 'a as a -> Some a
                        | _ -> None
                    else
                        p.Value.ToObject<'a>() |> Some
            | _ ->
                None

    let cleanName (name : string) =
        let c0 = name.[0] 
        let name =
            if c0 >= '0' && c0 <= '9' then "d " + name
            else name

        name.Split([| ' '; '\t' |], StringSplitOptions.RemoveEmptyEntries)
        |> Seq.map (fun str ->
            if str.Length > 0 then 
                str.Substring(0, 1).ToUpper() + str.Substring(1)
            else
                str
        )
        |> String.concat ""


    let otherChars (a : string) (b : string) =
        let a = cleanName a
        let b = cleanName b
        let mutable cnt = 0 
        while cnt < a.Length && cnt < b.Length && a.[cnt] = b.[cnt] do
            cnt <- cnt + 1

        let res = min a.Length b.Length - cnt
        System.Console.WriteLine(sprintf "%A %A: %d" a b res)

        res

module Method =
    let tryParse (v : JToken) =
        match prop v "name" with
        | Some (name : string) ->
            let ret : option<string> = prop v "returns"
            let args =
                match prop v "args" with
                | Some (arr : JArray) ->
                    arr |> Seq.choose (fun a ->
                        match prop a "name", prop a "type" with
                        | Some (name : string), Some(typ : string) ->
                            let defaultValue = 
                                match name.ToLower().Trim() with
                                | "userdata" | "user data" -> Some "0" 
                                | "label" -> Some "null" 
                                | _ -> prop a "default"
                            Some { name = name; typ = typ; annotation = prop a "annotation"; def = defaultValue }
                        | _ ->
                            None
                    )
                | None ->
                    Seq.empty

            Some { name = name; ret = ret; args = Seq.toList args }
        | None ->
            None
            
module Callback =
    let tryParse (v : JToken) =
        match prop v "args" with
        | Some (arr : JArray) ->
            arr |> Seq.toList |> List.choose (fun a ->
                match prop a "name", prop a "type" with
                | Some (name : string), Some(typ : string) ->
                    Some { name = name; typ = typ; annotation = prop a "annotation"; def = prop a "default" }
                | _ ->
                    None
            ) |> Some
        | None ->
            None

module Entry =

    let prop (v : JToken) (name : string) : option<'a> =
        if isNull v then None
        else
            match v with
            | :? JObject as o -> 
                let p = o.Property name
                if isNull p then None
                else p.Value.ToObject<'a>() |> Some
            | _ ->
                None

    let tryParse (o : JProperty) =
        match o.Value with
        | :? JObject as value -> 
            let c = value.Property("category")
            if isNull c then
                None
            else
                let cat = c.Value.ToString().Trim()
                match cat with
                | "object" ->
                    let methods =
                        let p = value.Property("methods")
                        if isNull p then []
                        else
                            match p.Value with
                            | :? JArray as methods ->
                                methods |> Seq.choose Method.tryParse |> Seq.toList
                            | _ ->
                                []

                    
                    let methods =
                        { name = "reference"; ret = None; args = [] } ::
                        { name = "release"; ret = None; args = [] } ::
                        methods

                    Object(o.Name, methods) |> Some

                | "structure" ->
                    let ext = 
                        match prop value "extensible" with
                        | Some v -> v
                        | None -> false

                    let fields =
                        let p = value.Property("members")
                        if isNull p then []
                        else
                            match p.Value with
                            | :? JArray as members ->
                                members |> Seq.toList |> List.choose (fun m ->  
                                    match prop m "type", prop m "name" with
                                    | Some (typ : string), Some (name : string) ->
                                        let defaultValue = 
                                            match name.ToLower().Trim() with
                                            | "userdata" | "user data" -> Some "0" 
                                            | "label" -> Some "null" 
                                            | _ -> prop m "default"
                                        Some { name = name; typ = typ; annotation = prop m "annotation"; def = defaultValue }
                                    | _ ->
                                        None
                                )
                            | _->
                                []

                    Struct(o.Name, ext, fields) |> Some

                | "enum" | "bitmask" ->
                    let isFlag = cat = "bitmask"

                    let values =
                        let p = value.Property("values")
                        if isNull p then []
                        else
                            match p.Value with
                            | :? JArray as members ->
                                members |> Seq.toList |> List.choose (fun m ->  
                                    match prop m "value", prop m "name" with
                                    | Some (value : int), Some (name : string) ->
                                        Some (name, value)
                                    | _ ->
                                        None
                                )
                            | _->
                                []
                        

                    Enum(o.Name, isFlag, values) |> Some

                | "native" ->   
                    Native( o.Name.Trim()) |> Some

                | "callback" ->
                    match Callback.tryParse value with
                    | Some cb -> Callback(o.Name, cb) |> Some
                    | None -> printfn "bad %s: %s" cat o.Name; None

                | cat ->
                    printfn "bad category: %A" cat
                    None
        | _ ->
            None

let indent (str : string) =
    str.Split([|"\r\n"|], StringSplitOptions.None) |> Array.map (sprintf "    %s") |> String.concat "\r\n"

module rec Ast =
    type Field =
        {
            name            : string
            fieldType       : Lazy<TypeDef>
            defaultValue    : option<string>
        }

        member x.Defaultable =
            match x.defaultValue with
            | Some _ -> true
            | None ->
                match x.fieldType.Value with
                | Struct(_,_,fields) | ByRef(Struct(_,_,fields)) ->
                    fields |> List.forall (fun f -> f.Defaultable)
                | _ ->
                    false

    type Method =
        {
            name            : string
            parameters      : list<Field>
            returnType      : Lazy<TypeDef>
        }

    type TypeDef =
        | Unit
        | Object of name : string * methods : list<Method>
        | Struct of name : string * extensible : bool * fields : list<Field>
        | PersistentCallback of name : string * args : list<Field>
        | CompletionCallback of name : string * args : list<Field>
        | Enum of name : string * flags : bool * values : list<string * int>
        | Option of TypeDef
        | Array of TypeDef
        | Ptr of TypeDef
        | ByRef of TypeDef
        | NativeInt of signed : bool
        | Int of signed : bool * bits : int 
        | Float of bits : int 
        | Bool
        | String

        member x.Flatten =  
            match x with
            | Option t | Array t | Ptr t | ByRef t -> t
            | _ -> x


        member x.UsedTypes =
            match x with
            | Object(_, methods) -> 
                methods 
                |> Seq.collect (fun m -> m.returnType.Value :: (m.parameters |> List.map (fun p -> p.fieldType.Value)))
                |> Seq.map (fun t -> t.Flatten)
            | Struct(_, _, fields) ->
                fields 
                |> Seq.map (fun p -> p.fieldType.Value)
                |> Seq.map (fun t -> t.Flatten)
            | PersistentCallback(_, args) | CompletionCallback(_, args) ->
                args 
                |> Seq.map (fun p -> p.fieldType.Value)
                |> Seq.map (fun t -> t.Flatten)
            | Option t | Array t | Ptr t | ByRef t ->
                Seq.append (Seq.singleton t) t.UsedTypes
                |> Seq.map (fun t -> t.Flatten)
            | _ ->
                Seq.empty

    let rec nativeName (t : TypeDef) =
        match t with
        | Int(false, 8) -> "uint8"
        | Int(false, 16) -> "uint16"
        | Int(false, 32) -> "int"
        | Int(false, 64) -> "uint64"
        | Int(true, 8) -> "int8"
        | Int(true, 16) -> "int16"
        | Int(true, 32) -> "int32"
        | Int(true, 64) -> "int64"
        | Int(s, b) -> failwithf "bad int: %A %A" s b
        | Float(32) -> "float32"
        | Float(64) -> "float"
        | NativeInt true -> "nativeint"
        | NativeInt false -> "unativeint"
        | Float _ -> failwith "bad int"
        | String -> "nativeint"
        | Bool -> "int"
        | Array Unit | Option Unit | Ptr Unit | ByRef Unit ->
            "nativeint"
        | Array t | Option t | Ptr t | ByRef t ->
            sprintf "nativeptr<%s>" (nativeName t)
        | PersistentCallback _ | CompletionCallback _ ->
            "nativeint"
        //| Function _ ->
        //    failwithf "functions cannot be native"
        | Enum(name,_,_) ->
            name
        | Struct(name,_,_) ->
            "DawnRaw.WGPU" + name
        | Object(name,_) ->
            name + "Handle"
        | Unit ->
            "unit"
                
    let rec externName (t : TypeDef) =
        match t with
        | Int(false, 8) -> "uint8"
        | Int(false, 16) -> "uint16"
        | Int(false, 32) -> "int"
        | Int(false, 64) -> "uint64"
        | Int(true, 8) -> "int8"
        | Int(true, 16) -> "int16"
        | Int(true, 32) -> "int32"
        | Int(true, 64) -> "int64"
        | Int(s, b) -> failwithf "bad int: %A %A" s b
        | Float(32) -> "float32"
        | Float(64) -> "float"
        | NativeInt true -> "nativeint"
        | NativeInt false -> "unativeint"
        | Float _ -> failwith "bad int"
        | String -> "nativeint"
        | Bool -> "int"

        | Array t | Option t | Ptr t | ByRef t ->
            sprintf "%s*" (externName t)
        | PersistentCallback _ | CompletionCallback _ ->
            "nativeint"
        //| Function _ ->
        //    failwithf "functions cannot be native"
        | Enum(name,_,_) ->
            name
        | Struct(name,_,_) ->
            "WGPU" + name
        | Object(name,_) ->
            name + "Handle"
        | Unit ->
            "void"

    let rec frontendName (t : TypeDef) =
        match t with
        | Int _ | Float _ | NativeInt _ -> nativeName t
        | String -> "string"
        | Bool -> "bool"
        | Array t ->
            sprintf "array<%s>" (frontendName t)
        | Option t ->
            sprintf "option<%s>" (frontendName t)
        | Ptr t ->
            sprintf "nativeptr<%s>" (frontendName t)
        | ByRef t ->
            frontendName t
        | PersistentCallback(name, _) | CompletionCallback(name,_) ->
            name
        | Enum(name,_,_) ->
            name
        | Struct(name,_,_) ->
            name
        | Object(name,_) ->
            name
        | Unit ->
            "unit"
        //| Function(_, args, ret) ->
        //    match args with
        //    | [] -> sprintf "unit -> %s" (frontendName ret)
        //    | args -> 
        //        let argTypes = args |> List.map frontendName |> String.concat " -> "
        //        sprintf "%s -> %s" argTypes (frontendName ret)
    
    module private TypeDef =
        let cache = System.Collections.Concurrent.ConcurrentDictionary<string, Lazy<TypeDef>>()

        let cleanName (name : string) =
            let c0 = name.[0] 
            let name =
                if c0 >= '0' && c0 <= '9' then "d " + name
                else name

            name.Split([| ' '; '\t' |], StringSplitOptions.RemoveEmptyEntries)
            |> Seq.map (fun str ->
                if str.Length > 0 then 
                    str.Substring(0, 1).ToUpper() + str.Substring(1)
                else
                    str
            )
            |> String.concat ""
            
        let otherChars (a : string) (b : string) =
            let a = cleanName a
            let b = cleanName b
            let mutable cnt = 0 
            while cnt < a.Length && cnt < b.Length && a.[cnt] = b.[cnt] do
                cnt <- cnt + 1

            let res = min a.Length b.Length - cnt
            System.Console.WriteLine(sprintf "%A %A: %d" a b res)

            res



        let rec parameterType (context : Map<string, Entry>) (p : Parameter) =
            let baseType = 
                match Map.tryFind p.typ context with
                | Some entry -> ofEntry context entry
                | None -> failwithf "bad parameter type: %A" p.typ
            if Option.isNone p.annotation then 
                baseType
            else 
                lazy (  
                    match baseType.Value with
                    | Int(false, 8) -> String
                    | Unit -> NativeInt true
                    | t -> Option t
                )

        and private valueString (typ : TypeDef) (value : string) =
            match typ with
            | Int(signed, bits) ->

                let inline opt (a : bool, value : 'a) = if a then Some value else None

                let parsers =
                    [
                        System.Numerics.BigInteger.TryParse >> opt
                        
                        fun v -> 
                            if v.StartsWith "0x" then
                                System.Numerics.BigInteger.TryParse(v.Substring 2, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture) |> opt
                            else
                                None
                    ]

                let result = parsers |> List.tryPick (fun f -> f value)

                match result with
                | Some value ->
                    if signed then
                        match bits with
                        | 8 -> sprintf "%dy" (int8 value)
                        | 16 -> sprintf "%ds" (int16 value)
                        | 32 -> sprintf "%d" (int32 value)
                        | 64 -> sprintf "%dL" (int64 value)
                        | _ -> failwithf "bad interger type: %A" typ
                    else
                        match bits with
                        | 8 -> sprintf "%duy" (uint8 value)
                        | 16 -> sprintf "%dus" (uint16 value)
                        | 32 -> sprintf "%d" (int value)
                        | 64 -> sprintf "%dUL" (uint64 value)
                        | _ -> failwithf "bad interger type: %A" typ
                | _ ->
                    failwithf "bad integer value: %A" value
            | NativeInt signed ->
                match System.Int64.TryParse value with
                | (true, value) ->
                    if signed then sprintf "%dn" value
                    else sprintf "%dun" (uint64 value)
                | _ ->
                    failwithf "bad integer value: %A" value
            | Float bits ->
                let value = if value.EndsWith "f" then value.Substring(0, value.Length - 1) else value

                match System.Double.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture) with
                | (true, value) ->
                    match bits with
                    | 32 -> sprintf "%ff" value
                    | 64 -> sprintf "%f" value
                    | _ -> failwithf "bad float type: %A" typ
                | _ ->
                    failwithf "bad float value: %A" value
            | Bool ->
                match System.Boolean.TryParse value with
                | (true, value) ->
                    if value then "true" else "false"
                | _ ->
                    failwithf "bad boolean value: %A" value

            | Enum(_, _, values) ->
                let value = cleanName value
                let existing = values |> List.exists (fun (n,_) -> n = value)
                if existing then
                    sprintf "%s.%s" (frontendName typ) value
                else
                    failwithf "unknown enum-value: %s.%s" (frontendName typ) value

            | Object _ ->
                match value with
                | "undefined" | "null" | "" ->
                    "null"
                | _ ->
                    failwithf "bad object value: %A" value

            | Option _ ->
                match value with
                | "undefined" | "null" | "" ->
                    "None"
                | _ ->
                    failwithf "bad object value: %A" value

            | String ->
                match value with
                | "null" -> "null"
                | _ -> sprintf "\"%s\"" value

            | Unit ->
                "()"

            | ByRef _ | Array _ | Ptr _ | Struct _ | PersistentCallback _ | CompletionCallback _  ->
                failwithf "cannot print default value for type: %A" typ

        and private ofParameters (persistent : bool) (meth : bool) (context : Map<string, Entry>) (pars : list<Parameter>) =
            match pars with
            | [] ->
                []
            | p0 :: pars ->
                let n0 = cleanName p0.name
                match (parameterType context p0).Value with
                | Int(false, 32) ->
                    match pars with
                    | p1 :: pars when Option.isSome p1.annotation && otherChars p0.name p1.name < 4 ->
                        let n1 = cleanName p1.name
                        let typ = 
                            match (parameterType context p1).Value with
                            | Array t -> lazy (Array t)
                            | Option t -> lazy (Array t)
                            | Ptr t -> lazy (Array t)
                            | t -> lazy t

                        let def = p1.def |> Option.map (valueString typ.Value)

                        { name = n1; fieldType = typ; defaultValue = def } :: ofParameters persistent meth context pars
                    | _ ->
                        let def = p0.def |> Option.map (valueString (Int(false, 32)))

                        { name = n0; fieldType = lazy Int(false, 32); defaultValue = def } :: ofParameters persistent meth context pars
                | Option elementType when meth -> //(n0.EndsWith "Descriptor" || n0 = "CopySize" || (frontendName elementType).EndsWith "CopyView") ->
                    { name = n0; fieldType = lazy (ByRef elementType); defaultValue = None } :: ofParameters persistent meth context pars
                | PersistentCallback(name, args) ->
                    if persistent then { name = n0; fieldType = lazy (PersistentCallback(name, args)); defaultValue = None } :: ofParameters persistent meth context pars
                    else { name = n0; fieldType = lazy (CompletionCallback(name, args)); defaultValue = None } :: ofParameters persistent meth context pars
                | t0 ->
                    let def = p0.def |> Option.map (valueString t0)
                    { name = n0; fieldType = lazy t0; defaultValue = def } :: ofParameters persistent meth context pars

        let rec ofEntry (context : Map<string, Entry>) (entry : Entry) = 
            cache.GetOrAdd(entry.Name, fun _ ->
                lazy 
                (
                    match entry with
                    | Entry.Native "uint32_t" -> Int(false, 32)
                    | Entry.Native "int32_t" -> Int(true, 32)
                    | Entry.Native "uint64_t" -> Int(false, 64)
                    | Entry.Native "bool" -> Bool
                    | Entry.Native "float" -> Float 32
                    | Entry.Native "char" -> Int(false, 8)
                    | Entry.Native "size_t" -> NativeInt false
                    | Entry.Native "void" -> Unit
                    | Entry.Native "void *" | Entry.Native "const void *" | Entry.Native "void const *"-> NativeInt true
                    | Entry.Native o ->  failwithf "bad native type: %A" o
                    | Entry.Object(name, meths) ->  
                        let objectName = cleanName name

                        let methods =
                            meths |> List.map (fun m ->
                                let returnType =
                                    match m.ret with
                                    | Some retName ->
                                        match Map.tryFind retName context with
                                        | Some retEntry ->
                                            ofEntry context retEntry
                                        | None ->
                                            failwithf "bad return type: %A" retName
                                    | None ->
                                        lazy Unit

                                let parameters = ofParameters (m.name.EndsWith "callback") true context m.args
                                //System.Console.WriteLine(sprintf "%s: %A" m.name (m.name.EndsWith "callback"))
                                {
                                    name = cleanName m.name
                                    parameters = parameters
                                    returnType = returnType
                                }
                            )

                        Object(objectName, methods)

                    | Entry.Enum(name, flags, values) ->
                        let values = values |> List.map (fun (n, v) -> cleanName n, v)
                        Enum(cleanName name, flags, values)
                    | Entry.Struct(name, ext, fields) ->
                        let fields = ofParameters false false context fields
                        Struct(cleanName name, ext, fields)

                    | Entry.Callback(name, args) ->
                        let args = ofParameters false false context args
                        PersistentCallback(cleanName name, args)

                )
            )

        and ofType (context : Map<string, Entry>) (typ : string) (annotation : option<string>) =
            let baseType = 
                match Map.tryFind typ context with
                | Some e -> ofEntry context e
                | None -> failwithf "undefined type: %A" typ
            let isPtr = Option.isSome annotation
            lazy (
                let baseType = baseType.Value
                match baseType with
                | Unit when isPtr -> NativeInt true
                | Int(false, 8) when isPtr -> String
                | _ ->
                    if isPtr then Array baseType
                    else baseType

            )
          
    let typeDefs (context : Map<string, Entry>) =
        context |> Map.toList |> List.choose (fun (_, e) ->
            match e with
            | Native _ -> None
            | _ -> (TypeDef.ofEntry context e).Value |> Some
        )

    let run2() =
        let b = System.Text.StringBuilder()
        let printfn fmt = Printf.kprintf (fun str -> b.AppendLine str |> ignore) fmt

        if not (Directory.Exists dawnDir) then failwith "please run build-script"
        let text = File.ReadAllText (Path.Combine(dawnDir, "dawn.json"))
        let root = JObject.Parse text


        let all = root.Properties() |> Seq.choose Entry.tryParse |> Seq.map (fun e -> e.Name, e) |> Map.ofSeq

        let indent (str : string) =
            str.Split([|"\r\n"|], StringSplitOptions.None) |> Array.map (fun l -> "    " + l) |> String.concat "\r\n"

                
        let readValue (access : string -> string) (field : Field) (inner : string) =
            let name = field.name
            let typ = field.fieldType.Value
            match typ with
            | String ->
                String.concat "\r\n" [
                    sprintf "let _%s = System.Runtime.InteropServices.Marshal.PtrToStringAnsi %s" name (access name)
                    inner
                ]
            | _ ->
                String.concat "\r\n" [
                    sprintf "let _%s = %s" name (access name)
                    inner
                ]
            
        let rec pinField (access : string -> string) (field : Field) (inner : string) =
            let name = field.name
            let typ = field.fieldType.Value
            match typ with
            | String ->
                String.concat "\r\n" [

                    sprintf "let inline _%sCont (_%s) = " name name
                    indent inner
                    sprintf "if not (isNull %s) then" (access name)
                    sprintf "    let _%sLen = System.Text.Encoding.UTF8.GetByteCount %s" name (access name)
                    sprintf "    let _%sSize = _%sLen + 1" name name
                    sprintf "    let _%sPtr = NativePtr.stackalloc<byte> _%sSize" name name
                    sprintf "    System.Text.Encoding.UTF8.GetBytes(%s.AsSpan(), Span<byte>(NativePtr.toVoidPtr _%sPtr, _%sSize)) |> ignore" (access name) name name
                    sprintf "    NativePtr.set _%sPtr _%sLen 0uy" name name
                    sprintf "    _%sCont (NativePtr.toNativeInt _%sPtr)" name name
                    sprintf "else"
                    sprintf "    _%sCont 0n" name

                    //sprintf "let _%s = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi %s" name (access name)
                    //sprintf "try"
                    //indent inner
                    //sprintf "finally"
                    //sprintf "    System.Runtime.InteropServices.Marshal.FreeHGlobal _%s" name
                ]
            | Bool ->
                String.concat "\r\n" [
                    sprintf "let _%s = (if %s then 1 else 0)" name (access name)
                    inner
                ]

            | Float _ | Int _ | NativeInt _ | Enum _ | Ptr _ ->
                String.concat "\r\n" [
                    sprintf "let _%s = %s" name (access name)
                    inner
                ]
                
            | ByRef (Struct (_, ext, fields) as element) ->
                let nativeName = nativeName element
                pinStruct nativeName (access name) (sprintf "%sValue" name) ext fields [
                    sprintf "let _%s = NativePtr.stackalloc 1" name
                    sprintf "NativePtr.write _%s _%sValue" name name
                    inner
                ]
                //String.concat "\r\n" [
                //    sprintf "%s.Pin (fun _%sValue ->" (access name) name
                //    sprintf "    let _%s = NativePtr.stackalloc 1" name
                //    sprintf "    NativePtr.write _%s _%sValue" name name
                //    //sprintf "    use _%s = fixed [| _%sValue |]" name name
                //    indent inner
                //    sprintf ")"
                //]
            | ByRef (Enum _ | Float _ | Int _ | NativeInt _ | Ptr _) ->
                String.concat "\r\n" [
                    sprintf "let _%s = NativePtr.stackalloc 1" name
                    sprintf "NativePtr.write _%s %s" name (access name)
                    //sprintf "use _%s = fixed [| %s |]" name (access name)
                    inner
                ]
            | ByRef typ ->
                failwithf "unexpected: %A" typ
                
            | Array (Object _ as element) ->
                let elementName = nativeName element
                String.concat "\r\n" [
                    sprintf "let _%sCount = %s.Length" name (access name)
                    sprintf "let _%s = NativePtr.stackalloc _%sCount" name name
                    sprintf "for i in 0 .. _%sCount-1 do" name
                    sprintf "    if isNull %s.[i] then NativePtr.set _%s i %s.Null" (access name) name elementName
                    sprintf "    else NativePtr.set _%s i %s.[i].Handle" name (access name)
                    //// TODO bad array alloc
                    //sprintf "use _%s = fixed (%s |> Array.map (fun v -> if isNull v then %s.Null else v.Handle))" name (access name) elementName
                    inner
                ]
                 
            | Array (Struct(_,ext, fields) as element) ->
                let frontendElement = frontendName element
                let nativeElement = nativeName element

                String.concat "\r\n" [
                    sprintf "let _%sCount = if isNull %s then 0 else %s.Length" name (access name) (access name)
                    sprintf "let rec _%sCont (_%sinputs : array<%s>) (_%soutputs : array<%s>) (_%si : int) =" name name frontendElement name nativeElement name
                    sprintf "    if _%si >= _%sCount then" name name
                    sprintf "        use _%s = fixed _%soutputs" name name
                    sprintf "%s" (indent (indent inner))
                    sprintf "    else"
                    indent (indent (pinStruct nativeElement (sprintf "_%sinputs.[_%si]" name name) "n" ext fields [
                        sprintf "_%soutputs.[_%si] <- _n" name name
                        sprintf "_%sCont _%sinputs _%soutputs (_%si + 1)" name name name name
                    ]))
                    //sprintf "        inputs.[i].Pin(fun n -> outputs.[i] <- n; _%sCont inputs outputs (i + 1))" name
                    sprintf "_%sCont %s (if _%sCount > 0 then Array.zeroCreate _%sCount else null) 0" name (access name) name name

                ]

            | Array _ ->
                String.concat "\r\n" [
                    sprintf "use _%s = fixed %s" name (access name)
                    sprintf "let _%sCount = %s.Length" name (access name)
                    inner
                ]

            | Option (Object _ as element) ->
                let elementName = nativeName element
                String.concat "\r\n" [
                    sprintf "let inline _%sCont _%s =" name name
                    indent inner

                    sprintf "match %s with" (access name)
                    sprintf "| Some o ->"
                    sprintf "    let _%s = NativePtr.stackalloc 1" name
                    sprintf "    if isNull o then NativePtr.write _%s %s.Null" name elementName
                    sprintf "    else NativePtr.write _%s o.Handle" name
                    sprintf "    _%sCont _%s" name name
                    sprintf "| _ ->"
                    sprintf "    _%sCont (NativePtr.ofNativeInt 0n)" name
                ]
                
            | Option (Struct(_, ext, fields) as element) ->
                let nativeName = nativeName element
                String.concat "\r\n" [
                    sprintf "let inline _%sCont _%s = " name name
                    indent inner
                    sprintf "match %s with" (access name)
                    sprintf "| Some v ->"
                    indent (
                        pinStruct nativeName "v" "n" ext fields [
                            sprintf "let ptr = NativePtr.stackalloc 1"
                            sprintf "NativePtr.write ptr _n"
                            sprintf "_%sCont ptr" name
                            
                        ]
                    )
                    //sprintf "    v.Pin(fun n -> "
                    //sprintf "    )" 
                    sprintf "| None -> _%sCont (NativePtr.ofNativeInt 0n)" name
                ]
                
            | Option _ ->
                String.concat "\r\n" [
                    sprintf "let inline _%sCont _%s =" name name
                    indent inner

                    sprintf "match %s with" (access name)
                    sprintf "| Some o ->"
                    sprintf "    let _%s = NativePtr.stackalloc 1" name
                    sprintf "    NativePtr.write _%s o" name
                    sprintf "    _%sCont _%s" name name
                    sprintf "| _ ->"
                    sprintf "    _%sCont (NativePtr.ofNativeInt 0n)" name
                ]

            
            //| Function(PersistentCallback _ as cbType, _, _) ->
            //    String.concat "\r\n" [
            //        sprintf "let _%sDel = %s(%s)" name (frontendName cbType) name
            //        sprintf "let _%sGC = System.Runtime.InteropServices.GCHandle.Alloc(_%sDel)" name (access name)
            //        sprintf "let _%s = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(_%sDel)" name (access name)
            //        inner
            //    ]
                
            
            //| Function(CompletionCallback(_, args) as cbType, _, _) ->
            //    let argDef = args |> List.map (fun a -> a.name) |> String.concat " "
            //    let argUse = args |> List.map (fun a -> a.name) |> String.concat " "
            //    String.concat "\r\n" [
            //        sprintf "let mutable _%sGC = Unchecked.defaultof<System.Runtime.InteropServices.GCHandle>" name
            //        sprintf "let _%sCB = %s(fun %s -> %s %s; _%sGC.Free())" name (frontendName cbType) argDef (access name) argUse name
            //        sprintf "let _%sGC = System.Runtime.InteropServices.GCHandle.Alloc(_%sCB)" name name
            //        sprintf "let _%s = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(_%sGC)" name name
            //        inner
            //    ]

            //| Function _ ->
            //    failwith "impossible"

            | PersistentCallback(_, args) ->
                let argDef = args |> List.map (fun a -> sprintf "(%s : %s)" a.name (nativeName a.fieldType.Value)) |> String.concat " "

                let rec readArgs (a : list<Field>) =
                    match a with
                    | [] ->
                        let all = args |> List.map (fun f -> sprintf "_%s" f.name) |> String.concat ", "
                        sprintf "%s.Invoke(%s)" name all
                    | a0 :: rest ->
                        readValue id a0 (readArgs rest)



                String.concat "\r\n" [
                    sprintf "let _%sFunction %s = " name argDef
                    indent (readArgs args)
                    sprintf "let _%sDel = WGPU%s(_%sFunction)" name (frontendName typ) name
                    sprintf "let _%sGC = System.Runtime.InteropServices.GCHandle.Alloc(_%sDel)" name (access name)
                    sprintf "let _%s = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(_%sDel)" name (access name)
                    inner
                ]

            | CompletionCallback(_, args) ->
                let argDef = args |> List.map (fun a -> sprintf "(%s : %s)" a.name (nativeName a.fieldType.Value)) |> String.concat " "

                let rec readArgs (a : list<Field>) =
                    match a with
                    | [] ->
                        let all = args |> List.map (fun f -> sprintf "_%s" f.name) |> String.concat ", "
                        String.concat "\r\n"[
                            sprintf "if _%sGC.IsAllocated then _%sGC.Free()" name name
                            sprintf "%s.Invoke(%s)" name all
                        ]
                    | a0 :: rest ->
                        readValue id a0 (readArgs rest)



                String.concat "\r\n" [
                    sprintf "let mutable _%sGC = Unchecked.defaultof<System.Runtime.InteropServices.GCHandle>" name
                    sprintf "let _%sFunction %s = " name argDef
                    indent (readArgs args)
                    sprintf "let _%sDel = WGPU%s(_%sFunction)" name (frontendName typ) name
                    sprintf "_%sGC <- System.Runtime.InteropServices.GCHandle.Alloc(_%sDel)" name (access name)
                    sprintf "let _%s = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(_%sDel)" name (access name)
                    inner
                ]
                //let argDef = args |> List.map (fun a -> a.name) |> String.concat " "
                //let argUse = args |> List.map (fun a -> a.name) |> String.concat ", "
                //String.concat "\r\n" [
                //    sprintf "let mutable _%sGC = Unchecked.defaultof<System.Runtime.InteropServices.GCHandle>" name
                //    sprintf "let _%sCB = %s(fun %s -> %s.Invoke(%s); _%sGC.Free())" name (frontendName typ) argDef (access name) argUse name
                //    sprintf "let _%sGC = System.Runtime.InteropServices.GCHandle.Alloc(_%sCB)" name name
                //    sprintf "let _%s = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(_%sGC)" name name
                //    inner
                //]

            | Object(_, _meths) ->
                let nativeName = nativeName typ
                String.concat "\r\n" [
                    sprintf "let _%s = (if isNull %s then %s.Null else %s.Handle)" name (access name) nativeName (access name)
                    inner
                ]
            | Struct(_, ext, fields)  ->
                pinStruct (nativeName typ) (access name) name ext fields [
                    inner
                ]
                //String.concat "\r\n" [
                //    sprintf "%s.Pin (fun _%s ->" (access name) name
                //    indent inner
                //    sprintf ")"
                //]
            | Unit ->
                inner

        and pinStruct (nativeName : string) (access : string) (varName : string) (ext : bool) (fields : list<Field>) (inner : list<string>) =
            let rec pinCode (f : list<Field>) =
                match f with
                | [] ->
                    String.concat "\r\n" [
                        yield sprintf "let mutable _%s = Unchecked.defaultof<%s>" varName nativeName
                        if ext then yield sprintf "_%s.Next <- 0n" varName
                        for f in fields do
                            match f.fieldType.Value with
                            | Array _ -> yield sprintf "_%s.%sCount <- _%sCount" varName f.name f.name
                            | _ -> ()
                            yield sprintf "_%s.%s <- _%s" varName f.name f.name
                        yield sprintf "let _%s = _%s" varName varName
                        yield! inner
                    ]
                | f0 :: rest ->
                    let rest = pinCode rest
                    pinField (sprintf "%s.%s" access) f0 rest
                
            pinCode fields

        let defs = typeDefs all

        let equalType (a : TypeDef) (b : TypeDef) =
            frontendName a = frontendName b

        let graph =
            defs |> List.map (fun d ->
                let used = d.UsedTypes |> Seq.filter (fun t -> defs |> List.exists (fun ti -> equalType ti t)) |> Seq.toList |> List.distinct

                System.Console.WriteLine(sprintf "%s: [%s]" (frontendName d) (used |> List.map frontendName |> String.concat "; "))

                d, used
            )

        let rec tops (graph : list<TypeDef * list<TypeDef>>) =
            let noDeps, deps = graph |> List.partition (fun (_,ds) -> List.isEmpty ds)
            let noDeps = List.map fst noDeps
            if List.isEmpty deps then
                noDeps
            elif List.isEmpty noDeps then
                failwithf "cycle in [%s]" (graph |> List.map (fun (a,_) -> frontendName a) |> String.concat "; ")
            else
                let filtered = deps |> List.map (fun (a, bs) -> a, bs |> List.filter (fun b -> not (List.exists (equalType b) noDeps)))
                noDeps @ tops filtered

   
        let defs = tops graph
            



        printfn "namespace rec WebGPU"
        printfn "open System"
        printfn "open System.Security"
        printfn "open System.Runtime.InteropServices"
        printfn "open Microsoft.FSharp.NativeInterop"
        printfn "#nowarn \"9\""
        printfn "#nowarn \"49\""
        printfn ""
        
        // all handles
        for e in defs do
            match e with
            | Object _ ->
                let nativeName = nativeName e
                printfn "[<StructLayout(LayoutKind.Sequential)>]"
                printfn "type %s = " nativeName
                printfn "    struct"
                printfn "        val mutable public Handle : nativeint"
                printfn "        new(handle : nativeint) = { Handle = handle }"
                printfn "        static member Null = %s(0n)" nativeName
                printfn "    end"

            | _ ->
                ()

        // all callbacks  
        for e in defs do
            match e with
            | PersistentCallback(_, args) | CompletionCallback(_, args) ->
                let argTypes = args |> List.map (fun a -> nativeName a.fieldType.Value) |> String.concat " * "
                let selfName = frontendName e
                printfn "type WGPU%s = delegate of %s -> unit" selfName argTypes
            | _ ->
                ()
                
        for e in defs do
            match e with
            | PersistentCallback(_, args) | CompletionCallback(_, args) ->
                let argTypes = args |> List.map (fun a -> frontendName a.fieldType.Value) |> String.concat " * "
                let selfName = frontendName e
                printfn "type %s = delegate of %s -> unit" selfName argTypes
            | _ ->
                ()

        printfn ""
        printfn ""

        // all enums
        for e in defs do
            match e with
            | Enum(name, isFlags, values) ->
                if isFlags then printfn "[<Flags>]"
                printfn "type %s = " name
                for (name, value) in values do
                printfn "| %s = %d" name value
            | _ ->
                ()
                
        printfn ""
        printfn ""

        printfn "module DawnRaw ="
        // all structs
        for e in defs do
            match e with
            | Struct(name, ext, fields) ->
                printfn "    [<StructLayout(LayoutKind.Sequential)>]"
                printfn "    type WGPU%s =" name
                printfn "        struct"
                if ext then printfn "            val mutable public Next : nativeint"
                for f in fields do
                    let typ = f.fieldType.Value
                    let typeName = nativeName typ
                    match typ with
                    | Array _ -> 
                        let cntType = nativeName (Int(false, 32))
                        printfn "            val mutable public %sCount : %s" f.name cntType
                    | _ -> ()
                    printfn "            val mutable public %s : %s" f.name typeName
                printfn "        end"
            | _ ->
                ()

        // all functions
        for e in defs do
            match e with
            | Object(name, meths) ->
                for meth in meths do
                    let functionName = sprintf "wgpu%s%s" name meth.name

                    let args =
                        { name = "self"; fieldType = lazy e; defaultValue = None } :: meth.parameters 
                        |> List.collect (fun f ->  
                            let arg = sprintf "%s %s" (externName f.fieldType.Value) f.name
                            match f.fieldType.Value with
                            | Array _ -> 
                                [ sprintf "%s %sCount" (externName (Int(false, 32))) f.name; arg]
                            | _ -> 
                                [ arg ]
                        )
                        |> String.concat ", "

                    printfn "    [<DllImport(\"dawn\"); SuppressUnmanagedCodeSecurity>]"
                    printfn "    extern %s %s(%s)" (externName meth.returnType.Value) functionName args
            | _ ->
                ()

        printfn ""
        printfn ""


        // frontend structs
        for e in defs do
            match e with
            | Struct(name, ext, []) ->
                
                printfn "[<Struct>]"
                printfn "type %s = " name
                printfn "    member inline x.Pin<'a>(callback : %s -> 'a) : 'a = " (nativeName e)
                printfn "        let mutable native = Unchecked.defaultof<%s>" (nativeName e)
                if ext then printfn "        native.Next <- 0n"
                printfn "        callback native"
                ()
            | Struct(name, ext, fields) ->
                printfn "[<Struct>]"
                printfn "type %s =" name
                printfn "    {"
                for f in fields do
                    let typ = f.fieldType.Value
                    let typeName = frontendName typ
                    printfn "        %s : %s" f.name typeName
                printfn "    }"

                
                let defFields, otherFields = fields |> List.partition (fun f -> f.Defaultable)
                if not (List.isEmpty defFields) then
                    let args = 
                        match otherFields with
                        | [] -> 
                            ""
                        | other ->
                            other |> List.map (fun f -> sprintf "%s: %s" f.name (frontendName f.fieldType.Value)) |> String.concat ", " |> sprintf "(%s)"

                    let argSet = otherFields |> List.map (fun f -> f.name) |> Set.ofList

                    printfn "    static member Default%s : %s =" args name
                    printfn "        {"
                    for f in fields do
                        let fieldType = f.fieldType.Value |> frontendName
                        match f.defaultValue with
                        | Some v ->
                            printfn "            %s = %s" f.name v
                        | None ->
                            if Set.contains f.name argSet then
                                printfn "            %s = %s" f.name f.name
                            else
                                printfn "            %s = %s.Default" f.name fieldType
                    printfn "        }"

                printfn ""

                printfn "    member inline x.Pin<'a>(callback : %s -> 'a) : 'a = " (nativeName e)
                printfn "        let x = x"
                let rec pinCode (f : list<Field>) =
                    match f with
                    | [] ->
                        String.concat "\r\n" [
                            yield sprintf "let mutable native = Unchecked.defaultof<%s>" (nativeName e)
                            if ext then yield "native.Next <- 0n"
                            for f in fields do
                                match f.fieldType.Value with
                                | Array _ -> yield sprintf "native.%sCount <- _%sCount" f.name f.name
                                | _ -> ()
                                yield sprintf "native.%s <- _%s" f.name f.name
                            yield sprintf "callback native"
                        ]
                    | f0 :: rest ->
                        let rest = pinCode rest
                        pinField (sprintf "x.%s") f0 rest
                
                let code = pinCode fields
                printfn "%s" (indent (indent code))


            | _ ->
                ()

        printfn ""
        printfn ""
        
        for e in defs do
            match e with
            | Object(name, meths) ->
                let meNative = nativeName e
                let device = if name = "Device" then "x" else "x.Device"
                let ctorArgs = 
                    String.concat ", " [
                        if name <> "Device" then yield "device : Device"
                        yield sprintf "handle : %s" meNative
                    ]

                let ctorArgsWithRefCount = 
                    String.concat ", " [
                        if name <> "Device" then yield "device : Device"
                        yield sprintf "handle : %s" meNative
                        yield sprintf "refCount : ref<int>"
                    ]

                    

                let ctorArgsUseWithRefCount = 
                    String.concat ", " [
                        if name <> "Device" then yield "device"
                        yield sprintf "handle"
                        yield sprintf "refCount"
                    ]

                printfn "[<AllowNullLiteral>]"
                printfn "type %s(%s) = " name ctorArgsWithRefCount
                printfn "    let mutable isDisposed = false"
                if name <> "Device" then
                    printfn "    member x.Device = device"

                printfn "    member x.ReferenceCount = !refCount"
                printfn "    member x.Handle = handle"
                printfn "    member x.IsDisposed = isDisposed"

                printfn "    member private x.Dispose(disposing : bool) ="
                printfn "        if not isDisposed then "
                printfn "            refCount := !refCount - 1"
                printfn "            isDisposed <- true"
                printfn "            if disposing then System.GC.SuppressFinalize x"
                printfn "            DawnRaw.wgpu%sRelease(handle)" name

                printfn "    member x.Dispose() = x.Dispose(true)"
                printfn "    override x.Finalize() = x.Dispose(false)"
                printfn "    member x.Clone() = "
                printfn "        if isDisposed then raise <| System.ObjectDisposedException(\"%s\")" name
                printfn "        refCount := !refCount + 1"
                printfn "        DawnRaw.wgpu%sReference(handle)" name
                printfn "        new %s(%s)" name ctorArgsUseWithRefCount
                printfn "    interface System.IDisposable with"
                printfn "        member x.Dispose() = x.Dispose()"
                
                let ctorArgUse =
                    String.concat ", " [
                        if name <> "Device" then yield "device"
                        yield sprintf "handle"
                        yield "ref 1"
                    ]
                printfn "    new(%s) = new %s(%s)" ctorArgs name ctorArgUse

                for meth in meths do
                    if meth.name <> "Reference" && meth.name <> "Release" then
                        let overloads (meth : Method) =
                            let rec traverse (a : list<Field>) =
                                match a with
                                | [] -> [[]]
                                | f :: rest ->
                                    if f.Defaultable then
                                        let take = traverse rest |> List.map (fun a -> Choice1Of2 { f with defaultValue = None } :: a)
                                        let restArgs = 
                                            (f :: rest) |> List.map (fun f -> 
                                                match f.defaultValue with
                                                | Some v -> Choice2Of2 (f.name, v, f.fieldType.Value)
                                                | None -> Choice2Of2 (f.name, sprintf "%s.Default" (frontendName f.fieldType.Value), f.fieldType.Value)
                                            )
                                        restArgs :: take
                                    else
                                        traverse rest |> List.map (fun fs ->
                                            Choice1Of2 f :: fs
                                        )

                            traverse meth.parameters |> List.map (fun args ->
                                let pars = args |> List.choose (function Choice1Of2 f -> Some f | _ -> None)
                                { meth with parameters = pars }, args
                            )

                        for meth, args in overloads meth do

                            let ret = meth.returnType.Value |> frontendName
                            let argDecl = 
                                meth.parameters |> List.map (fun p ->
                                    sprintf "%s : %s" p.name (frontendName p.fieldType.Value)
                                ) |> String.concat ", "
                            printfn "    member inline x.%s(%s) : %s = " meth.name argDecl ret

                            //for p in meth.parameters do
                            //    if p.Defaultable then
                            //        match p.defaultValue with
                            //        | Some value -> printfn "        let %s = defaultArg %s %s" p.name p.name value
                            //        | None -> printfn "        let %s = defaultArg %s %s.Default" p.name p.name (frontendName p.fieldType.Value)


                            let nativeFunctionName = sprintf "wgpu%s%s" name meth.name

                            let rec pinCode (args : list<string>) (f : list<Choice<Field, string * string * TypeDef>>) =
                                match f with
                                | [] ->
                                    let retName = frontendName meth.returnType.Value
                                    let wrap = 
                                        match meth.returnType.Value with
                                        | Object _ -> 
                                            if retName = "Device" then sprintf "new %s(%s)" retName
                                            else sprintf "new %s(%s, %s)" retName device
                                        | Bool ->
                                            sprintf "%s <> 0"
                                        | Unit | Float _ | Int _ | NativeInt _  ->
                                            id
                                        | t ->
                                            failwithf "bad return type: %A" t

                                    String.concat "\r\n" [
                                        sprintf "DawnRaw.%s(%s)" nativeFunctionName ("x.Handle" :: (List.rev args) |> String.concat ", ") |> wrap
                                    ]

                        

                                | f0 :: rest ->
                                    match f0 with
                                    | Choice1Of2 f0 -> 
                                        match f0.fieldType.Value with
                                        | Array _ ->
                                            let rest = pinCode (sprintf "_%s" f0.name :: sprintf "_%sCount" f0.name :: args) rest
                                            pinField id f0 rest
                                
                                        | _ -> 
                                            let rest = pinCode (sprintf "_%s" f0.name :: args) rest
                                            pinField id f0 rest
                                    | Choice2Of2 ((name, value, typ)) ->
                                        let rest = pinCode (sprintf "_%s" name :: args) rest
                                        pinField (fun _ -> value) { name = name; fieldType = lazy typ; defaultValue = None } rest
                
                            let code = pinCode [] args
                            printfn "%s" (indent (indent code))


            | _ ->
                ()

        let output = Path.Combine(__SOURCE_DIRECTORY__, "WebGPU.fs")
        File.WriteAllText(output, b.ToString())

Ast.run2()