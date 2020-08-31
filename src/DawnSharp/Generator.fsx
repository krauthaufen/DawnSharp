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
                            Some { name = name; typ = typ; annotation = prop a "annotation"; def = prop a "default" }
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
                                        Some { name = name; typ = typ; annotation = prop m "annotation"; def = None }
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

type TypeMode =
    | Extern
    | Internal
    | Default

let indent (str : string) =
    str.Split([|"\r\n"|], StringSplitOptions.None) |> Array.map (sprintf "    %s") |> String.concat "\r\n"

let run() = 
    if not (Directory.Exists dawnDir) then failwith "please run build-script"
    let text = File.ReadAllText (Path.Combine(dawnDir, "dawn.json"))
    let root = JObject.Parse text


    let all = root.Properties() |> Seq.choose Entry.tryParse |> Seq.map (fun e -> e.Name, e) |> Map.ofSeq

    
    let typeName (mode : TypeMode) (name : string) (ann : option<string>) =
        match Map.tryFind name all with
        | Some def ->
            let ann = ann |> Option.map (fun s -> s.Replace("const", "").Trim())

            let ptr =
                match ann with
                | Some "*" -> true
                | Some other -> failwithf "bad annotation: %A" other
                | None -> false

            let wrap =
                if ptr then 
                    match mode with
                    | Extern -> sprintf "%s*"
                    | Internal -> sprintf "nativeptr<%s>"
                    | Default -> sprintf "%s[]"
                else id

            match def with
            | Native "uint32_t" -> wrap "uint32"
            | Native "int32_t" -> wrap "int"
            | Native "uint64_t" -> wrap "uint64"
            | Native "bool" -> wrap "int"
            | Native "float" -> wrap "float32"
            | Native "char" -> 
                match mode with
                | Default when ptr -> "string"
                | _ -> wrap "byte"
            | Native "size_t" -> wrap "unativeint"
            | Native "void" -> 
                if ptr then "nativeint"
                else "unit"
            | Native "void *" | Native "const void *" | Native "void const *"-> 
                wrap "nativeint"
            | Native o ->   
                failwithf "bad native type: %A" o
                
            | Callback _ when mode = Internal || mode = Extern ->
                "nativeint" |> wrap

            | Object(name,_) ->
                match mode with
                | Internal | Extern ->
                    let name = cleanName name + "Handle"
                    wrap name
                | _ ->
                    if ptr then sprintf "%s[]" (cleanName name)
                    else cleanName name |> wrap
            | Struct(name, _, _) ->
                match mode with
                | Internal | Extern ->
                    let name = "WGPU" + cleanName name
                    wrap name
                | _ ->
                    if ptr then sprintf "%s[]" (cleanName name)
                    else cleanName name |> wrap
            | _ -> 
                cleanName def.Name |> wrap
       
        | None ->
            printfn "bad type: %A" name
            cleanName name


    let b = System.Text.StringBuilder()
    let printfn fmt = Printf.kprintf (fun str -> b.AppendLine str |> ignore) fmt

    printfn "namespace rec WebGPU"
    printfn "open System"
    printfn ""
    printfn "#nowarn \"49\""
    printfn "#nowarn \"9\""
    printfn ""
    for (_, e) in Map.toSeq all do
        match e with
        | Enum(name, flags, values) ->
            if flags then printfn "[<Flags>]"
            printfn "type %s =" (cleanName name)
            for (name, value) in values do
                printfn "    | %s = %d" (cleanName name) value

        | Struct(name, extensible, fields) ->
            printfn "type WGPU%s =" (cleanName name)
            printfn "    struct"
            if extensible then
                printfn "        val mutable public pNext : nativeint"
            for p in fields do
                printfn "        val mutable public %s : %s" (cleanName p.name) (typeName Internal p.typ p.annotation)
                
            printfn "    end"


            match fields with
            | [] ->
                printfn "type %s private () =" (cleanName name)
                printfn "    static let instance = %s()" (cleanName name)
                printfn "    static member Instance = instance"

            | _ -> 
                printfn "[<Struct>]"
                printfn "type %s =" (cleanName name)
                printfn "    {"
                //if extensible then
                //    printfn "        Next : nativeint"

                let rec run (fields : list<Parameter>) =
                    match fields with
                    | [] -> ()
                    | p :: fields ->
                        if typeName Internal p.typ p.annotation = "uint32" &&  p.name.EndsWith " count" then
                            match fields with
                            | pn :: fields when Option.isSome pn.annotation && otherChars p.name pn.name < 4 ->
                                printfn "        %s : %s" (cleanName pn.name) (typeName Default pn.typ pn.annotation)
                                run fields
                            | _ ->
                                printfn "        %s : %s" (cleanName p.name) (typeName Default p.typ p.annotation)
                                run fields
                        else
                            printfn "        %s : %s" (cleanName p.name) (typeName Default p.typ p.annotation)
                            run fields
                        
                run fields
                //for p in fields do
                //    printfn "        %s : %s" (cleanName p.name) (typeName Default p.typ p.annotation)
                printfn "    }"
            printfn "    member x.Pin<'a>(action : WGPU%s -> 'a) : 'a =" (cleanName name)
            printfn "        let x = x"
            printfn "        let mutable native = Unchecked.defaultof<WGPU%s>" (cleanName name)
            //if extensible then
            //    printfn "        native.pNext <- x.Next"

            let body = 
                String.concat "\r\n" [
                    if extensible then yield "native.pNext <- 0n"
                    yield "action native"
                ]

            let rec wrap (fields : list<Parameter>) =
                match fields with
                | [] -> 
                    body
                | f :: rest ->
                    let doit(fields : list<_>) = 
                        match fields with
                        | f :: fields -> 
                            let ptr = f.annotation |> Option.isSome
                            match all.[f.typ] with
                            | Struct _ as typ ->
                                if ptr then
                                    String.concat "\r\n" [
                                        sprintf "let rec pin%s (a : array<%s>) (p : array<_>) (i : int) ="  (cleanName f.name) (typeName Default f.typ None)
                                        sprintf "    if i >= a.Length then"
                                        sprintf "        use p = fixed p"
                                        sprintf "        native.%s <- p"  (cleanName f.name)
                                        sprintf "%s" (indent (indent (wrap fields)))
                                        sprintf "    else"
                                        sprintf "        a.[i].Pin(fun ai -> p.[i] <- ai; pin%s a p (i+1))"(cleanName f.name)
                                        sprintf "pin%s x.%s (Array.zeroCreate x.%s.Length) 0" (cleanName f.name) (cleanName f.name) (cleanName f.name)
                                    ]
                                else
                                    String.concat "\r\n" [
                                        sprintf "x.%s.Pin(fun _%s ->" (cleanName f.name) (cleanName f.name) 
                                        sprintf "    native.%s <- _%s" (cleanName f.name) (cleanName f.name) 
                                        indent (wrap fields)
                                        sprintf ")"
                                    ]
                    
                            | Object _ ->
                                if ptr then
                                    String.concat "\r\n" [
                                        sprintf "let _%s = x.%s |> Array.map (fun a -> a.Handle)" (cleanName f.name) (cleanName f.name)
                                        sprintf "use _%s = fixed _%s" (cleanName f.name) (cleanName f.name)
                                        sprintf "native.%s <- _%s" (cleanName f.name) (cleanName f.name) 
                                        wrap fields
                                    ]
                                else
                                    String.concat "\r\n" [
                                        sprintf "native.%s <- x.%s.Handle" (cleanName f.name) (cleanName f.name) 
                                        wrap fields
                                    ]
                
                            | Native "char" when ptr ->
                                String.concat "\r\n" [
                                    sprintf "let p%s = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.%s" (cleanName f.name) (cleanName f.name)
                                    sprintf "try"
                                    sprintf "    native.%s <- Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt p%s" (cleanName f.name) (cleanName f.name)
                                    indent (wrap fields)
                                    sprintf "finally"
                                    sprintf "    System.Runtime.InteropServices.Marshal.FreeHGlobal p%s" (cleanName f.name)
                                ]

                            | t ->
                                if ptr then
                                    match typeName Default f.typ None with
                                    | "unit" ->
                                        String.concat "\r\n" [
                                            sprintf "native.%s <- x.%s" (cleanName f.name) (cleanName f.name) 
                                            wrap fields
                                        ]
                                    | _ -> 
                                        String.concat "\r\n" [
                                            sprintf "use _%s = fixed x.%s" (cleanName f.name) (cleanName f.name) 
                                            sprintf "native.%s <- _%s" (cleanName f.name) (cleanName f.name) 
                                            wrap fields
                                        ]

                                else 
                                    String.concat "\r\n" [
                                        sprintf "native.%s <- x.%s" (cleanName f.name) (cleanName f.name) 
                                        wrap fields
                                    ]
                        | _ ->
                            body 

                    if typeName Internal f.typ f.annotation = "uint32" && f.name.EndsWith " count" then
                        match rest with
                        | pn :: rest when Option.isSome pn.annotation && otherChars f.name pn.name < 4 ->
                            String.concat "\r\n" [
                                sprintf "native.%s <- (if isNull x.%s then 0u else uint32 x.%s.Length)" (cleanName f.name) (cleanName pn.name) (cleanName pn.name)
                                doit (pn :: rest)
                            ]
                        | _ ->
                            doit (f :: rest)
                    else
                        doit (f :: rest)

            let body = wrap fields

            //for f in List.rev fields do
            //    let ptr = f.annotation |> Option.isSome
            //    match all.[f.typ] with
            //    | Struct _ as typ ->
            //        if ptr then
            //            body <-
            //                String.concat "\r\n" [
            //                    sprintf "let rec pin%s (a : array<%s>) (p : array<_>) (i : int) ="  (cleanName f.name) (typeName Default f.typ None)
            //                    sprintf "    if i >= a.Length then"
            //                    sprintf "        use p = fixed p"
            //                    sprintf "        native.%s <- p"  (cleanName f.name)
            //                    sprintf "%s" (indent (indent body))
            //                    sprintf "    else"
            //                    sprintf "        a.[i].Pin(fun ai -> p.[i] <- ai; pin%s a p (i+1))"(cleanName f.name)
            //                    sprintf "pin%s x.%s (Array.zeroCreate x.%s.Length) 0" (cleanName f.name) (cleanName f.name) (cleanName f.name)
            //                ]
            //        else
            //            body <-
            //                String.concat "\r\n" [
            //                    sprintf "x.%s.Pin(fun _%s ->" (cleanName f.name) (cleanName f.name) 
            //                    sprintf "    native.%s <- _%s" (cleanName f.name) (cleanName f.name) 
            //                    indent body
            //                    sprintf ")"
            //                ]
                    
            //    | Object _ ->
            //        if ptr then
            //            body <-
            //                String.concat "\r\n" [
            //                    sprintf "let _%s = x.%s |> Array.map (fun a -> a.Handle)" (cleanName f.name) (cleanName f.name)
            //                    sprintf "use _%s = fixed _%s" (cleanName f.name) (cleanName f.name)
            //                    sprintf "native.%s <- _%s" (cleanName f.name) (cleanName f.name) 
            //                    body
            //                ]
            //        else
            //            body <-
            //                String.concat "\r\n" [
            //                    sprintf "native.%s <- x.%s.Handle" (cleanName f.name) (cleanName f.name) 
            //                    body
            //                ]
                
            //    | Native "char" when ptr ->
            //        body <-
            //            String.concat "\r\n" [
            //                sprintf "let p%s = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi x.%s" (cleanName f.name) (cleanName f.name)
            //                sprintf "try"
            //                sprintf "    native.%s <- Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt p%s" (cleanName f.name) (cleanName f.name)
            //                indent body
            //                sprintf "finally"
            //                sprintf "    System.Runtime.InteropServices.Marshal.FreeHGlobal p%s" (cleanName f.name)
            //            ]

            //    | t ->
            //        if ptr then
            //            match typeName Default f.typ None with
            //            | "unit" ->
                        
            //                body <- 
            //                    String.concat "\r\n" [
            //                        sprintf "native.%s <- x.%s" (cleanName f.name) (cleanName f.name) 
            //                        body
            //                    ]
            //            | _ -> 
            //                body <- 
            //                    String.concat "\r\n" [
            //                        sprintf "use _%s = fixed x.%s" (cleanName f.name) (cleanName f.name) 
            //                        sprintf "native.%s <- _%s" (cleanName f.name) (cleanName f.name) 
            //                        body
            //                    ]

            //        else 
            //            body <- 
            //                String.concat "\r\n" [
            //                    sprintf "native.%s <- x.%s" (cleanName f.name) (cleanName f.name) 
            //                    body
            //                ]

            printfn "%s" (indent (indent body))

        | Callback(name, args) ->
            let args = 
                match args with
                | [] -> "unit"
                | _ -> args |> List.map (fun a -> typeName Internal a.typ a.annotation) |> String.concat " * "
            printfn "type %s = delegate of %s -> unit" (cleanName name) args
        | Object _ ->
            ()
        | Native _ ->
            ()

    printfn "module DawnRaw ="
    printfn "    open System.Runtime.InteropServices"
    printfn "    open System.Security"
    printfn ""
    for (_, e) in Map.toSeq all do
        match e with
        | Object(name, methods) ->


            for m in methods do
                let fullName = "wgpu" + cleanName (name + " " + m.name)
                let ret = m.ret |> Option.map (fun t -> typeName Extern t None) |> Option.defaultValue "void"
                let args = ({ name = "self"; typ = name; annotation = None; def = None } :: m.args) |> Seq.map (fun a -> sprintf "%s %s" (typeName Extern a.typ a.annotation) (cleanName a.name)) |> String.concat ", "
                printfn "    [<DllImport(\"dawn\"); SuppressUnmanagedCodeSecurity>]"
                printfn "    extern %s %s(%s)" ret fullName args


        | _ ->
            ()


    //let printfn fmt = Printf.kprintf ignore fmt
    for (_, e) in Map.toSeq all do
        match e with
        | Object(name, methods) ->
            ()
            printfn "type %sHandle = struct val mutable public Handle : nativeint end" (cleanName name)
            

            let selfName = cleanName name

            printfn "[<Struct>]"
            
            if selfName <> "Device" then
                printfn "type %s(device : Device, handle : %sHandle) =" (cleanName name) (cleanName name)
            else 
                printfn "type %s(handle : %sHandle) =" (cleanName name) (cleanName name)
            printfn "    member x.Handle : %sHandle = handle" (cleanName name)
            if selfName <> "Device" then
                printfn "    member x.Device : Device = device" 


            for meth in methods do

                let clean = cleanName meth.name
                let mutable isCallback = false

                let rec run (args : list<Parameter>) : list<string> * list<string> * (string -> list<string>) =

                    let doit (args : list<Parameter>) =
                        match args with
                        | arg :: args ->
                            let (a,b,c) = run args
                            match Map.tryFind arg.typ all with
                            | Some typ ->
                                let ptr = Option.isSome arg.annotation 
                                let argName = cleanName arg.name
                                match typ with
                                | Callback(objName,args) ->
                                    let objName = cleanName objName
                                    let argDef = args |> List.map (fun a -> cleanName a.name) |> String.concat " "
                                    let argUse = args |> List.map (fun a -> cleanName a.name) |> String.concat ", "
                                    if clean.EndsWith "Callback" then
                                        isCallback <- true
                                        sprintf "%s : %s" argName objName :: a, sprintf "System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate %s" argName :: b, fun str -> [
                                            yield sprintf "let _%sGC = System.Runtime.InteropServices.GCHandle.Alloc(%s)" argName argName
                                            yield! c str
                                            yield sprintf "{ new System.IDisposable with member x.Dispose() = _%sGC.Free() }" argName
                                        ]
                                    else
                                        sprintf "%s : %s" argName objName :: a, sprintf "System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate _%s" argName :: b, fun str -> [
                                            yield sprintf "let mutable _%sGC = Unchecked.defaultof<System.Runtime.InteropServices.GCHandle>" argName
                                            yield sprintf "let _%s = %s(fun %s -> %s.Invoke(%s); _%sGC.Free())" argName objName argDef argName argUse argName
                                            yield sprintf "_%sGC <- System.Runtime.InteropServices.GCHandle.Alloc(_%s)" argName argName
                                            yield! c str
                                        ]

                                
                                | Object(objName, _) ->
                                    let objName = cleanName objName

                                    if ptr then
                                        sprintf "%s : %s[]" argName objName :: a, sprintf "_%s" argName :: b, fun str -> [
                                            yield sprintf "let _%s = %s |> Array.map (fun s -> s.Handle)" argName argName
                                            yield sprintf "use _%s = fixed _%s" argName argName
                                            yield! c str
                                        ]
                                    else
                                        sprintf "%s : %s" argName objName :: a, sprintf "%s.Handle" argName :: b, c
                            
                                | Struct(objName, _, _) ->
                                    let objName = cleanName objName
                                    if argName.EndsWith "Descriptor" then
                                        sprintf "%s : %s" argName objName :: a, sprintf "_%s" argName :: b, fun str -> [
                                            sprintf "%s.Pin(fun _%s ->" argName argName 
                                            sprintf "    use _%s = fixed [| _%s |]" argName argName
                                            indent (String.concat "\r\n" (c str))
                                            sprintf ")"
                                        ]
                                    elif ptr then
                                        sprintf "%s : %s[]" argName objName :: a, sprintf "_%s" argName :: b, fun str -> [
                                        
                                            sprintf "let inline %sCont(_%s) =" (cleanName argName) (cleanName argName)
                                            sprintf "%s" (indent (String.concat "\r\n" (c str)))
                                            ""
                                            sprintf "let rec pin%s (a : array<%s>) (p : array<_>) (i : int) ="  (cleanName argName) (typeName Default arg.typ None)
                                            sprintf "    if i >= a.Length then"
                                            sprintf "        use _%s = fixed p" (cleanName argName)
                                            sprintf "        %sCont(_%s)" (cleanName argName) (cleanName argName)
                                            sprintf "    else"
                                            sprintf "        a.[i].Pin(fun ai -> p.[i] <- ai; pin%s a p (i+1))"(cleanName argName)
                                            
                                            sprintf "if isNull %s then %sCont(Microsoft.FSharp.NativeInterop.NativePtr.ofNativeInt 0n)" (cleanName argName) (cleanName argName)
                                            sprintf "else pin%s %s (Array.zeroCreate %s.Length) 0" (cleanName argName) (cleanName argName) (cleanName argName)
                                        ]
                                    else
                                        sprintf "%s : %s" argName objName :: a, sprintf "_%s" argName :: b, fun str -> [
                                            sprintf "%s.Pin(fun _%s ->" argName argName 
                                            sprintf "    use _%s = fixed [| _%s |]" argName argName
                                            indent (String.concat "\r\n" (c str))
                                            sprintf ")"
                                        ]
                                        //sprintf "%s : %s" argName objName :: a, sprintf "%s.ToNative()" argName :: b, fun str -> c str
                                

                                | _ ->
                              
                                    let typName = typeName Default typ.Name None
                                
                                    if ptr then 
                                        if typName = "unit" then 
                                            sprintf "%s : nativeint" argName :: a, argName :: b, c
                                        elif argName.EndsWith "Descriptor" then
                                            sprintf "%s : %s" argName typName :: a, sprintf "_%s" argName :: b, fun str ->[
                                                yield sprintf "use _%s = fixed [| %s |]" argName argName
                                                yield! c str
                                            ]
                                        else
                                            sprintf "%s : %s[]" argName typName :: a, sprintf "_%s" argName :: b, fun str -> [
                                                yield sprintf "use _%s = fixed %s" argName argName
                                                yield! c str
                                            ]
                                    else
                                        argName :: a, argName :: b, fun str -> c str
                            | None ->
                                failwith "bad type"

                        | [] ->
                            [], [], List.singleton

                    match args with
                    | a :: args ->
                        if typeName Internal a.typ a.annotation = "uint32" &&  a.name.EndsWith " count" then
                            match args with
                            | pn :: args when Option.isSome pn.annotation && otherChars a.name pn.name < 4 ->
                                let (a,b,c) = doit (pn :: args)
                                a, (sprintf "(if isNull %s then 0u else uint32 %s.Length)" (cleanName pn.name) (cleanName pn.name) :: b), c


                                //[], [], fun str -> [str]

                            | pn :: args when Option.isSome pn.annotation ->
                                doit ({ a with annotation = None } :: { pn with annotation = None } :: args)

                            | _ ->
                                doit ({ a with annotation = None } :: args)
                        else
                            match args with
                            | pn :: args when Option.isSome pn.annotation ->
                                doit ({ a with annotation = None } :: { pn with annotation = None } :: args)
                            | _ ->
                                doit ({ a with annotation = None } :: args)
                            
                    | [] ->
                        doit []
                            


                let (argDecl, argUse, wrapBody) = run meth.args
                    //meth.args |> List.map (fun a -> 
                    //    match Map.tryFind a.typ all with
                    //    | Some typ ->
                    //        let ptr = Option.isSome a.annotation 
                    //        let argName = cleanName a.name
                    //        match typ with
                    //        | Callback(objName,args) ->
                    //            let objName = cleanName objName
                    //            let argDef = args |> List.map (fun a -> cleanName a.name) |> String.concat " "
                    //            let argUse = args |> List.map (fun a -> cleanName a.name) |> String.concat ", "
                    //            if clean.EndsWith "Callback" then
                    //                isCallback <- true
                    //                sprintf "%s : %s" argName objName, sprintf "System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate %s" argName, fun str -> [
                    //                    yield sprintf "let _%sGC = System.Runtime.InteropServices.GCHandle.Alloc(%s)" argName argName
                    //                    yield str
                    //                    yield sprintf "{ new System.IDisposable with member x.Dispose() = _%sGC.Free() }" argName
                    //                ]
                    //            else
                    //                sprintf "%s : %s" argName objName, sprintf "System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate _%s" argName, fun str -> [
                    //                    yield sprintf "let mutable _%sGC = Unchecked.defaultof<System.Runtime.InteropServices.GCHandle>" argName
                    //                    yield sprintf "let _%s = %s(fun %s -> %s.Invoke(%s); _%sGC.Free())" argName objName argDef argName argUse argName
                    //                    yield sprintf "_%sGC <- System.Runtime.InteropServices.GCHandle.Alloc(_%s)" argName argName
                    //                    yield str
                    //                ]

                                
                    //        | Object(objName, _) ->
                    //            let objName = cleanName objName

                    //            if ptr then
                    //                sprintf "%s : %s[]" argName objName, sprintf "_%s" argName, fun str -> [
                    //                    sprintf "let _%s = %s |> Array.map (fun s -> s.Handle)" argName argName
                    //                    sprintf "use _%s = fixed _%s" argName argName
                    //                    str
                    //                ]
                    //            else
                    //                sprintf "%s : %s" argName objName, sprintf "%s.Handle" argName, fun str -> [str]
                            
                    //        | Struct(objName, _, _) ->
                    //            let objName = cleanName objName
                    //            if argName.EndsWith "Descriptor" then
                    //                sprintf "%s : %s" argName objName, sprintf "_%s" argName, fun str -> [
                    //                    sprintf "%s.Pin(fun _%s ->" argName argName 
                    //                    sprintf "    use _%s = fixed [| _%s |]" argName argName
                    //                    indent str
                    //                    sprintf ")"
                    //                ]
                    //            elif ptr then
                    //                sprintf "%s : %s[]" argName objName, sprintf "_%s" argName, fun str -> [
                                        
                    //                    sprintf "let rec pin%s (a : array<%s>) (p : array<_>) (i : int) ="  (cleanName argName) (typeName Default a.typ None)
                    //                    sprintf "    if i >= a.Length then"
                    //                    sprintf "        use _%s = fixed p" (cleanName argName)
                    //                    sprintf "%s" (indent (indent str))
                    //                    sprintf "    else"
                    //                    sprintf "        a.[i].Pin(fun ai -> p.[i] <- ai; pin%s a p (i+1))"(cleanName argName)
                    //                    sprintf "pin%s %s (Array.zeroCreate %s.Length) 0" (cleanName argName) (cleanName argName) (cleanName argName)
                    //                ]
                    //            else
                    //                sprintf "%s : %s" argName objName, sprintf "%s.ToNative()" argName, fun str -> [str]
                                

                    //        | _ ->
                              
                    //            let typName = typeName Default typ.Name None
                                
                    //            if ptr then 
                    //                if typName = "unit" then 
                    //                    sprintf "%s : nativeint" argName, argName, fun str -> [str]
                    //                elif argName.EndsWith "Descriptor" then
                    //                    sprintf "%s : %s" argName typName, sprintf "_%s" argName, fun str ->[
                    //                        sprintf "use _%s = fixed [| %s |]" argName argName
                    //                        str
                    //                    ]
                    //                else
                    //                    sprintf "%s : %s[]" argName typName, sprintf "_%s" argName, fun str -> [
                    //                        sprintf "use _%s = fixed %s" argName argName
                    //                        str
                    //                    ]
                    //            else
                    //                argName, argName, fun str -> [str]
                    //    | None ->
                    //        failwith "bad type"
                    //)

                let argDecl = argDecl |> String.concat ", "
                let argUse = argUse |> List.append ["handle"] |> String.concat ", "


                let wrap, ret =
                    if isCallback then
                        id, "System.IDisposable"
                    else
                        let ret =  meth.ret |> Option.bind (fun r -> Map.tryFind r all)
                        match ret with
                        | Some (Object(name,_)) ->
                            let n = typeName Default name None
                            if n <> "Device" then
                                sprintf "%s(device, %s)" n, n
                            else
                                sprintf "%s(%s)" n, n
                        | Some o ->
                            let n = typeName Default o.Name None
                            id, n
                        | None ->
                            id, "unit"
                        
                let fullName = "DawnRaw.wgpu" + cleanName (name + " " + meth.name)

                let mutable body = wrap (sprintf "%s(%s)" fullName argUse)
                body <- wrapBody body |> String.concat "\r\n"

                printfn "    member x.%s(%s) : %s =" (cleanName meth.name) argDecl ret
                if selfName = "Device" then printfn "        let device = x"
                else printfn "        let device = device"
                printfn "        let handle = handle"
                printfn "%s" (indent (indent body))
                
                ()

        | _ ->
            ()
    
    let output = Path.Combine(__SOURCE_DIRECTORY__, "WebGPU.fs")
    File.WriteAllText(output, b.ToString())