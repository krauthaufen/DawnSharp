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
                            Some { name = name; typ = typ; annotation = prop a "annotation" }
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
                    Some { name = name; typ = typ; annotation = prop a "annotation" }
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
                                        Some { name = name; typ = typ; annotation = prop m "annotation" }
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

let run() = 
    if not (Directory.Exists dawnDir) then failwith "please run build-script"
    let text = File.ReadAllText (Path.Combine(dawnDir, "dawn.json"))
    let root = JObject.Parse text


    let all = root.Properties() |> Seq.choose Entry.tryParse |> Seq.map (fun e -> e.Name, e) |> Map.ofSeq


    let cleanName (name : string) =
        let c0 = name.[0] 
        let name =
            if c0 >= '0' && c0 <= '9' then "d " + name
            else name

        name.Split([| ' '; '\t' |], StringSplitOptions.RemoveEmptyEntries)
        |> Seq.map (fun str ->
            if str.Length > 0 then 
                str.Substring(0, 1).ToUpper() + str.Substring(1).ToLower()
            else
                str
        )
        |> String.concat ""

    let typeName (name : string) (ann : option<string>) =
        match Map.tryFind name all with
        | Some def ->
            let ann = ann |> Option.map (fun s -> s.Replace("const", "").Trim())

            let ptr =
                match ann with
                | Some "*" -> true
                | Some other -> failwithf "bad annotation: %A" other
                | None -> false

            let wrap =
                if ptr then sprintf "nativeptr<%s>"
                else id

            match def with
            | Native "uint32_t" -> wrap "uint32"
            | Native "int32_t" -> wrap "int"
            | Native "uint64_t" -> wrap "uint64"
            | Native "bool" -> wrap "int"
            | Native "float" -> wrap "float32"
            | Native "char" -> wrap "char"
            | Native "size_t" -> wrap "unativeint"
            | Native "void" -> 
                if ptr then "nativeint"
                else "unit"
            | Native "void *" | Native "const void *" | Native "void const *"-> 
                wrap "nativeint"
            | Native o ->   
                failwithf "bad native type: %A" o
                
            | _ -> 
                cleanName def.Name |> wrap
       
        | None ->
            printfn "bad type: %A" name
            cleanName name


    //let printfn fmt = Printf.kprintf ignore fmt
    for (_, e) in Map.toSeq all do
        match e with
        | Enum(name, flags, values) ->
            if flags then printfn "[<Flags>]"
            printfn "type %s =" (cleanName name)
            for (name, value) in values do
                printfn "    | %s = %d" (cleanName name) value

        | Struct(name, extensible, fields) ->
            printfn "[<Struct>]"
            printfn "type %s =" (cleanName name)
            printfn "    {"
            if extensible then
                printfn "        pNext : nativeint"
            for p in fields do
                printfn "        %s : %s" (cleanName p.name) (typeName p.typ p.annotation)
                
            printfn "    }"

        | Object(name, methods) ->
            printfn "type %sHandle = struct val mutable private Handle : nativeint end" (cleanName name)

            printfn "type %s(handle : %sHandle) =" (cleanName name) (cleanName name)

            for meth in methods do
                let args = meth.args |> List.map (fun a -> sprintf "%s: %s" (cleanName a.name) (typeName a.typ a.annotation)) |> String.concat ", "
                printfn "    member x.%s(%s) : %s =" (cleanName meth.name) args (match meth.ret with | Some ret -> typeName ret None | None -> "unit")
                printfn "        failwith \"\""
                
                ()

        | _ ->
            ()
    //for a in all do
    //    printfn "%A" a
    printfn "%A" all.Count