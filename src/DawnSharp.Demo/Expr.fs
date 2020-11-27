namespace Armadillo

open System
open System.Reflection
open Microsoft.FSharp.Reflection
open FSharp.Data.Adaptive

type SgVar(name : string, typ : Type, isMutable : bool) =
    member x.Name = name
    member x.Type = typ
    member x.IsMutable = isMutable
    
    override x.ToString() = name

    new(name : string, typ : Type) = SgVar(name, typ, false)

type SgExpr =
    | Group of SgExpr
    | Yield of SgExpr

    | Draw of info : SgExpr
    | SetShader of shader : SgExpr * child : SgExpr
    | SetVertexAttribute of name : SgExpr * value : SgExpr * child : SgExpr
    | SetInstanceAttribute of name : SgExpr * value : SgExpr * child : SgExpr
    | SetUniform of name : SgExpr * value : SgExpr * child : SgExpr

    | Range of min : SgExpr * step : SgExpr * max : SgExpr

    | AddressOf of SgExpr
    | Application of lambda : SgExpr * argument : SgExpr
    | Call of target : option<SgExpr> * method : MethodInfo * args : list<SgExpr>
    | Coerce of SgExpr * Type
    | DefaultValue of Type
    | FieldGet of option<SgExpr> * FieldInfo
    | FieldSet of option<SgExpr> * FieldInfo * SgExpr
    | For of var : SgVar * elems : SgExpr * body : SgExpr
    | IfThenElse of condition : SgExpr * ifTrue : SgExpr * ifFalse : SgExpr
    | Lambdas of vars : list<list<SgVar>> * body : SgExpr
    | Let of var : SgVar * expr : SgExpr * body : SgExpr
    | NewArray of elementType : Type * length : list<SgExpr>
    | NewDelegate of delegateType : Type * vars : list<SgVar> * body : SgExpr
    | NewObject of ctor : ConstructorInfo * args : list<SgExpr>
    | NewRecord of record : Type * args : list<SgExpr>
    | NewTuple of args : list<SgExpr>
    | NewUnionCase of case : UnionCaseInfo * args : list<SgExpr>
    | PropertyGet of target : option<SgExpr> * prop : PropertyInfo * indices : list<SgExpr>
    | PropertySet of target : option<SgExpr> * prop : PropertyInfo * indices : list<SgExpr> * value : SgExpr
    | QuoteRaw of SgExpr
    | QuoteTyped of SgExpr
    | Sequential of l : SgExpr * r : SgExpr
    | TryFinally of SgExpr * SgExpr
    | TryWith of SgExpr * SgVar * SgExpr * SgVar * SgExpr
    | TupleGet of SgExpr * int
    | TypeTest of SgExpr * Type
    | UnionCaseTest of SgExpr * UnionCaseInfo
    | Value of value : obj * typ : Type
    | Unit
    | VarSet of SgVar * SgExpr
    | Var of var : SgVar
    | WhileLoop of guard : SgExpr * body : SgExpr

    member x.Type =
        match x with
        | Group _ -> typeof<unit>
        | Yield _ -> typeof<unit>
        | Draw _ -> typeof<unit>
        | SetShader(_, e) -> e.Type 
        | SetVertexAttribute(_, _, e) -> e.Type
        | SetInstanceAttribute(_, _, e) -> e.Type
        | SetUniform(_, _, e) -> e.Type

        | Range(min,_,_) -> typedefof<seq<_>>.MakeGenericType [| min.Type |]

        | AddressOf e -> typeof<nativeint>
        | Application(l, a) -> 
            let (ta, r) = FSharpType.GetFunctionElements l.Type
            if ta <> a.Type then failwithf "bad application: %A vs %A" ta a.Type
            r

        | Call(_, mi, _) ->
            mi.ReturnType

        | Coerce(_, t) -> t
        | DefaultValue t -> t 
        | FieldGet(_, f) -> f.FieldType
        | FieldSet _ -> typeof<unit>
        | For _ -> typeof<unit>
        | IfThenElse(_, i, _) -> i.Type
        | Lambdas(vars, body) ->
            let rec build (ts : list<Type>) =
                match ts with
                | [] -> body.Type
                | t0 :: ts ->
                    FSharpType.MakeFunctionType(t0, build ts)

            vars |> List.map (function
                | [a] -> a.Type
                | args -> args |> List.map (fun v -> v.Type) |> List.toArray |> FSharpType.MakeTupleType
            )
            |> build

        | Let(_, _, b) -> b.Type
        | NewArray(elementType, lens) ->
            elementType.MakeArrayType(List.length lens)

        | NewDelegate(delegateType, _, _) -> delegateType
        | NewObject(ctor, _) -> ctor.DeclaringType
        | NewRecord(record, _) -> record
        | NewTuple(args) ->
            args |> List.map (fun e -> e.Type) |> List.toArray |> FSharpType.MakeTupleType
        | NewUnionCase(case, _) -> case.DeclaringType

        | PropertyGet(_, prop, _) -> prop.PropertyType
        | PropertySet _ -> typeof<unit>
        | QuoteRaw _ -> typeof<Quotations.Expr>
        | QuoteTyped e -> typedefof<Quotations.Expr<_>>.MakeGenericType [| e.Type |]
        | Sequential(_, r) -> r.Type
        | TryFinally(b, _) -> b.Type
        | TryWith(b, _, _, _, _) -> b.Type
        | TupleGet(e, i) ->
            let ts = FSharpType.GetTupleElements e.Type
            ts.[i]
        | TypeTest _ -> typeof<bool>
        | UnionCaseTest _ -> typeof<bool>
        | Value(_, t) -> t
        | Unit -> typeof<unit>
        | VarSet _ -> typeof<unit>
        | Var v -> v.Type
        | WhileLoop _ -> typeof<unit>



module SgExpr =
    module Translation =
        open Microsoft.FSharp.Quotations
    
        module Reflection =
            open Microsoft.FSharp.Quotations.Patterns
            open Microsoft.FSharp.Quotations.ExprShape

            let rec private tryGetMethod (e : Expr) =
                match e with
                | Call(_, mi, _) -> 
                    Some mi
                | ShapeLambda(_, b) ->
                    tryGetMethod b
                | ShapeCombination(_, args) ->
                    args |> List.tryPick tryGetMethod
                | ShapeVar _ ->
                    None
                
            let rec private tryGetProperty (e : Expr) =
                match e with
                | PropertyGet(_, mi, _) -> 
                    Some mi
                | ShapeLambda(_, b) ->
                    tryGetProperty b
                | ShapeCombination(_, args) ->
                    args |> List.tryPick tryGetProperty
                | ShapeVar _ ->
                    None

            let method (e : Expr<'a>) =
                match tryGetMethod e with
                | Some m -> m
                | None -> failwithf "no method: %A" e
            
            let property (e : Expr<'a>) =
                match tryGetProperty e with
                | Some m -> m
                | None -> failwithf "no property: %A" e

            let tryGetReflectedDefinition (e : Expr<'a>) =
                match tryGetMethod e with
                | Some mi ->
                    try Expr.TryGetReflectedDefinition mi
                    with _ -> None
                | None ->
                    None

        [<AutoOpen>]
        module SgExtensions =
            type Sg = Node<TraversalState>

            let private groupRun = Reflection.method <@ Sg.bgroup.Run @> 
            let private groupCombine = Reflection.method <@ Sg.bgroup.Combine @> 
            let private groupDelay = Reflection.method <@ Sg.bgroup.Delay @> 
            let private groupYieldSg = Reflection.method <@ Sg.bgroup.Yield : Node<_> -> unit @> 
            let private groupYieldUniform = Reflection.method <@ Sg.bgroup.Yield : SgAttribute.Uniform -> unit @> 
            let private groupYieldShader = Reflection.method <@ Sg.bgroup.Yield : SgAttribute.Shader -> unit @> 
            let private groupYieldVertexAttr = Reflection.method <@ Sg.bgroup.Yield : SgAttribute.VertexAttribute -> unit @> 
            let private groupYieldInstanceAttr = Reflection.method <@ Sg.bgroup.Yield : SgAttribute.InstanceAttribute -> unit @> 
            let private groupFor = (Reflection.method <@ Sg.bgroup.For @>).GetGenericMethodDefinition()

            let private uniformMeth = (Reflection.method <@ Sg.uniform @>).GetGenericMethodDefinition()
            let private vertexDataMeth = Reflection.method <@ Sg.vertexData @>
            let private effectMeth = Reflection.method <@ Sg.effect @>
            let private drawProp = Reflection.property <@ Sg.draw @>

            let private bgroupExpr = <@ Sg.bgroup @>

            type Expr with
                static member SgRun(e : Expr) =
                    Expr.Call(bgroupExpr, groupRun, [e])

                static member SgDelay(e : Expr) =
                    let unitVar = Var("unitVar", typeof<unit>)
                    Expr.Call(bgroupExpr, groupDelay, [Expr.Lambda(unitVar, e)])

                static member SgCombine(l : Expr, r : Expr) =
                    Expr.Call(bgroupExpr, groupCombine, [l;r])

                static member SgYield(e : Expr) =
                    if e.Type = typeof<Sg> then
                        Expr.Call(bgroupExpr, groupYieldSg, [e])
                    elif e.Type = typeof<SgAttribute.Uniform> then
                        Expr.Call(bgroupExpr, groupYieldUniform, [e])
                    elif e.Type = typeof<SgAttribute.VertexAttribute> then
                        Expr.Call(bgroupExpr, groupYieldVertexAttr, [e])
                    elif e.Type = typeof<SgAttribute.InstanceAttribute> then
                        Expr.Call(bgroupExpr, groupYieldInstanceAttr, [e])
                    elif e.Type = typeof<SgAttribute.Shader> then
                        Expr.Call(bgroupExpr, groupYieldShader, [e])
                    else
                        failwithf "cannot yield %A" e

                static member SgFor(v : Var, elems : Expr, body : Expr) =
                    Expr.Call(bgroupExpr, groupFor.MakeGenericMethod [| v.Type |], [elems; Expr.Lambda(v, body)])

                static member SgDraw(info : Expr) =
                    Expr.Application(Expr.PropertyGet drawProp, info)


                static member SgUniform(name : Expr, value : Expr) =
                    Expr.Call(uniformMeth.MakeGenericMethod [| value.Type |], [name; value])

                static member SgVertexData(name : Expr, value : Expr) =
                    Expr.Call(vertexDataMeth, [name; value])
                
                static member SgEffect(value : Expr) =
                    Expr.Call(effectMeth, [value])

            module Patterns =
            
                let (|SgRun|_|) (e : Expr) =
                    match e with
                    | Patterns.Call(Some _, r, [b]) when r = groupRun ->
                        Some b
                    | _ ->
                        None

                let (|SgDelay|_|) (e : Expr) =
                    match e with
                    | Patterns.Call(Some _, d, [Patterns.Lambda(v, body)]) when d = groupDelay && v.Type = typeof<unit> ->
                        Some body
                    | _ ->
                        None

                let (|SgCombine|_|) (e : Expr) =  
                    match e with
                    | Patterns.Call(Some _, c, [l;r]) when groupCombine = c ->
                        Some (l, r)
                    | _ ->
                        None

                let (|SgYield|_|) (e : Expr) =
                    match e with
                    | Patterns.Call(Some _, y, [v]) ->
                        if y = groupYieldSg || y = groupYieldShader || y = groupYieldVertexAttr || y = groupYieldInstanceAttr || y = groupYieldUniform then
                            Some v
                        else
                            None
                    | _ ->
                        None
                
                let (|SgFor|_|) (e : Expr) =
                    match e with
                    | Patterns.Call(Some _, f, [elems; Patterns.Lambda(v, b)]) when f.IsGenericMethod && f.GetGenericMethodDefinition() = groupFor ->
                        Some(v, elems, b)
                    | _ ->
                        None
                
                let (|SgDraw|_|) (e : Expr) =
                    match e with
                    | Patterns.Application(Patterns.PropertyGet(None, draw, []), arg) when draw = drawProp ->
                        Some arg
                    | _ ->
                        None


                let (|SgUniform|_|) (e : Expr) =
                    if e.Type = typeof<SgAttribute.Uniform> then
                        match e with
                        | Patterns.Call(None, uf, [name; value]) when uf.IsGenericMethod && uf.GetGenericMethodDefinition() = uniformMeth ->
                            Some(name, value)
                        | _ ->
                            None
                    else
                        None

                let (|SgVertexData|_|) (e : Expr) =
                    if e.Type = typeof<SgAttribute.VertexAttribute> then
                        match e with
                        | Patterns.Call(None, att, [name; value]) when att = vertexDataMeth ->
                            Some(name, value)
                        | _ ->
                            None
                    else
                        None
                
                let (|SgEffect|_|) (e : Expr) =
                    if e.Type = typeof<SgAttribute.Shader> then
                        match e with
                        | Patterns.Call(None, eff, [shader]) when eff = effectMeth ->
                            Some shader
                        | _ ->
                            None
                    else
                        None
                
        let getVar (v : Var) : State<Map<Var, SgVar>, SgVar> =
            State.custom (fun s ->
                match Map.tryFind v s with
                | Some r -> s, r
                | None ->
                    let r = SgVar(v.Name, v.Type, v.IsMutable)
                    Map.add v r s, r
            )

        let rec ofExprS (e : Expr) : State<Map<Var, SgVar>, SgExpr> =
            state {
                match e with
                | Patterns.Application(Patterns.Lambda(v, b), e) ->
                    if v.Type = typeof<Sg.GroupBuilder> then
                        let builder = Expr.Value(Sg.bgroup)
                        return!
                            b.Substitute (fun vi -> if vi = v then Some builder else None)
                            |> ofExprS
                    else
                        return! ofExprS (Expr.Let(v, e, b))

                
                
                | Patterns.Sequential(Patterns.Sequential(a,b),c)
                | Patterns.SgCombine(Patterns.Sequential(a,b),c) 
                | Patterns.Sequential(Patterns.SgCombine(a,b),c) 
                | Patterns.SgCombine(Patterns.SgCombine(a,b),c) ->
                    return! ofExprS (Expr.Sequential(a, Expr.Sequential(b,c)))
                
                | Patterns.Sequential(Patterns.SgYield thing, (Patterns.SgDelay r | r)) 
                | Patterns.SgCombine(Patterns.SgYield thing, (Patterns.SgDelay r | r))  ->
                    let! rest = ofExprS r
                    match thing with
                    | Patterns.SgEffect e ->
                        let! e = ofExprS e
                        return SgExpr.SetShader(e, rest)
                    | Patterns.SgUniform(name, value) ->
                        let! name = ofExprS name
                        let! value = ofExprS value
                        return SgExpr.SetUniform(name, value, rest)
                    | Patterns.SgVertexData(name, value) ->
                        let! name = ofExprS name
                        let! value = ofExprS value
                        return SgExpr.SetVertexAttribute(name, value, rest)
                    | v when v.Type = typeof<Sg> ->
                        let! v = ofExprS v
                        return SgExpr.Sequential(SgExpr.Yield v, rest)
                    | _ ->  
                        return failwithf "bad yield: %A" thing

                | Patterns.SgYield v when v.Type = typeof<Sg> ->
                    let! v = ofExprS v
                    return SgExpr.Yield v

                | Patterns.SgRun(Patterns.SgDelay b | b) ->
                    let! b = ofExprS b
                    return SgExpr.Group b

                | Patterns.SgCombine(l, (Patterns.SgDelay r | r)) | Patterns.Sequential(l, (Patterns.SgDelay r | r)) ->
                    let! l = ofExprS l
                    let! r = ofExprS r
                    return SgExpr.Sequential(l, r)

                | Patterns.SgDraw info ->
                    let! info = ofExprS info
                    return SgExpr.Draw info
                
                | Patterns.SgDelay r ->
                    return! ofExprS r

                | Patterns.SgFor(v, e, b) ->
                    let! e = ofExprS e
                    let! v = getVar v
                    let! b = ofExprS b
                    return SgExpr.For(v, e, b)

                | Patterns.Call(None, mi, [a;b]) when mi.Name = "op_Range" ->
                    let t = e.Type
                    let! a = ofExprS a
                    let! b = ofExprS b

                    let one =
                        if t = typeof<int8> then SgExpr.Value(1y, typeof<int8>)
                        elif t = typeof<int16> then SgExpr.Value(1s, typeof<int16>)
                        elif t = typeof<int32> then SgExpr.Value(1, typeof<int32>)
                        elif t = typeof<int64> then SgExpr.Value(1L, typeof<int64>)
                        elif t = typeof<uint8> then SgExpr.Value(1uy, typeof<uint8>)
                        elif t = typeof<uint16> then SgExpr.Value(1us, typeof<uint16>)
                        elif t = typeof<uint32> then SgExpr.Value(1u, typeof<uint32>)
                        elif t = typeof<uint64> then SgExpr.Value(1UL, typeof<uint64>)
                        elif t = typeof<float32> then SgExpr.Value(1.0f, typeof<float32>)
                        elif t = typeof<float> then SgExpr.Value(1.0, typeof<float>)
                        elif t = typeof<decimal> then SgExpr.Value(1.0m, typeof<decimal>)
                        else
                            let one = t.GetProperty("One")
                            if isNull one || one.PropertyType <> t then failwithf "cannot create range for type: %A" t
                            else SgExpr.Value(one.GetValue(null), one.PropertyType)

                    return SgExpr.Range(a, one, b)



                | Patterns.AddressOf e ->
                    let! e = ofExprS e
                    return SgExpr.AddressOf e

                | Patterns.Application(l, a) ->
                    let! l = ofExprS l
                    let! a = ofExprS a
                    return SgExpr.Application(l, a)

                | Patterns.Call(target, method, args) ->
                    let! target = target |> Option.mapS ofExprS
                    let! args = args |> List.mapS ofExprS
                    return SgExpr.Call(target, method, args)
                | Patterns.Coerce(e, t) ->
                    let! e = ofExprS e
                    return SgExpr.Coerce(e, t)

                | Patterns.DefaultValue t ->
                    return SgExpr.DefaultValue t

                | Patterns.FieldGet(target, field) ->
                    let! target = Option.mapS ofExprS target
                    return SgExpr.FieldGet(target, field)

                | Patterns.FieldSet(target, field, value) ->
                    let! target = Option.mapS ofExprS target
                    let! value = ofExprS value
                    return SgExpr.FieldSet(target, field, value)

                | Patterns.IfThenElse(c, i, e) ->
                    let! c = ofExprS c
                    let! i = ofExprS i
                    let! e = ofExprS e
                    return SgExpr.IfThenElse(c, i, e)

                | DerivedPatterns.Lambdas(vars, body) ->
                    let! vs = vars |> List.mapS (List.mapS getVar)
                    let! body = ofExprS body
                    return SgExpr.Lambdas(vs, body)

                | Patterns.Let(v, e, b) ->
                    let! e = ofExprS e
                    let! v = getVar v
                    let! b = ofExprS b
                    return SgExpr.Let(v, e, b)

                | Patterns.NewArray(t, l) ->
                    let! l = List.mapS ofExprS l
                    return SgExpr.NewArray(t, l)

                | Patterns.NewDelegate(typ, vars, body) ->
                    let! vars = vars |> List.mapS getVar
                    let! body = ofExprS body
                    return SgExpr.NewDelegate(typ, vars, body)

                | Patterns.NewObject(ctor, args) ->
                    let! args = args |> List.mapS ofExprS
                    return SgExpr.NewObject(ctor, args)

                | Patterns.NewRecord(typ, args) ->
                    let! args = args |> List.mapS ofExprS
                    return SgExpr.NewRecord(typ, args)

                | Patterns.NewTuple(args) ->
                    let! args = args |> List.mapS ofExprS
                    return SgExpr.NewTuple args

                | Patterns.NewUnionCase(ci, args) ->
                    let! args = args |> List.mapS ofExprS
                    return SgExpr.NewUnionCase(ci, args)

                | Patterns.PropertyGet(target, prop, indices) ->
                    let! target = target |> Option.mapS ofExprS
                    let! indices = indices |> List.mapS ofExprS
                    return SgExpr.PropertyGet(target, prop, indices)
                | Patterns.PropertySet(target, prop, indices, value) ->
                    let! target = target |> Option.mapS ofExprS
                    let! indices = indices |> List.mapS ofExprS
                    let! value = ofExprS value
                    return SgExpr.PropertySet(target, prop, indices, value)

                | Patterns.QuoteRaw e ->
                    let! e = ofExprS e
                    return SgExpr.QuoteRaw e
                
                | Patterns.QuoteTyped e ->
                    let! e = ofExprS e
                    return SgExpr.QuoteTyped e

                | Patterns.Sequential(l, r) ->
                    let! l = ofExprS l
                    let! r = ofExprS r
                    return SgExpr.Sequential(l, r)
                
                | Patterns.TryFinally(b, f) ->
                    let! b = ofExprS b
                    let! f = ofExprS f
                    return SgExpr.TryFinally(b, f)

                | Patterns.TryWith(a, v0, b, v1, c) ->
                    let! v0 = getVar v0
                    let! v1 = getVar v1
                    let! a = ofExprS a
                    let! b = ofExprS b
                    let! c = ofExprS c
                    return SgExpr.TryWith(a, v0, b, v1, c)

                | Patterns.TupleGet(t, i) ->
                    let! t = ofExprS t
                    return SgExpr.TupleGet(t, i)

                | Patterns.TypeTest(t, typ) ->
                    let! t = ofExprS t
                    return SgExpr.TypeTest(t, typ)

                | Patterns.UnionCaseTest(t, ci) ->
                    let! t = ofExprS t
                    return SgExpr.UnionCaseTest(t, ci)

                | Patterns.Value(value, typ) ->
                    if typ = typeof<unit> then return SgExpr.Unit
                    else return SgExpr.Value(value, typ)

                | Patterns.VarSet(v, e) ->
                    let! v = getVar v
                    let! e = ofExprS e
                    return SgExpr.VarSet(v, e)

                | Patterns.Var(v) ->
                    let! v = getVar v
                    return SgExpr.Var v

                | Patterns.WhileLoop(guard, body) ->
                    let! guard = ofExprS guard
                    let! body = ofExprS body
                    return SgExpr.WhileLoop(guard, body)

                | _ ->
                    return failwithf "unknown expression: %A" e
            }

    module Print =
        open System
        open System.Text
        open System.Text.RegularExpressions

        let private lb = Regex @"\r?\n"
        let private suffix = Regex @"`[0-9]+$"

        let multiLine (str : string) =
            lb.IsMatch(str)
                
        let indent (str : string) =
            let b = StringBuilder()
            for l in lb.Split str do
                b.AppendLine(String.Format("    {0}", l)) |> ignore
            b.ToString()

        let rec typeName (t : Type) =
            if t.IsArray then
                let commas = 
                    let rank = t.GetArrayRank()
                    if rank > 1 then Array.create (t.GetArrayRank() - 1) "," |> String.concat ""
                    else ""

                let e = t.GetElementType() |> typeName
                sprintf "%s[%s]" e commas

            elif t.IsByRef then
                sprintf "byref<%s>" (typeName (t.GetElementType()))

            elif t.IsGenericType then
                let name = suffix.Replace(t.Name, "")

                let def = t.GetGenericTypeDefinition()
                let name = 
                    if def = typedefof<list<_>> then "list"
                    elif def = typedefof<seq<_>> then "seq"
                    elif def = typedefof<Set<_>> then "Set"
                    elif def = typedefof<Map<_,_>> then "Map"
                    else name

                let pars = t.GetGenericArguments() |> Array.map typeName |> String.concat ", "
                sprintf "%s<%s>" name pars
            else
                if t = typeof<int8> then "int8"
                elif t = typeof<int16> then "int16"
                elif t = typeof<int32> then "int"
                elif t = typeof<int64> then "int64"
                elif t = typeof<uint8> then "uint8"
                elif t = typeof<uint16> then "uint16"
                elif t = typeof<uint32> then "uint32"
                elif t = typeof<uint64> then "uint64"
                elif t = typeof<float32> then "float32"
                elif t = typeof<float> then "float"
                elif t = typeof<decimal> then "decimal"
                elif t = typeof<bool> then "bool"
                elif t = typeof<char> then "char"
                elif t = typeof<string> then "string"

                else t.Name // TODO

        type Precedence =
            | Default = 0
            | Comma = 3
            | Application = 9
            | Prefix = 10
            | Call = 11
            | Dot = 12



        let rec print (scopePrecedence : Precedence) (e : SgExpr) =
            let inline wrap (mine : Precedence) (str : string) =
                if mine < scopePrecedence then 
                    if multiLine str then
                        sprintf "(\r\n%s\r\n)" (indent str)
                    else
                        "(" + str + ")"
                else str

            match e with
            | Group e ->
                sprintf "Sg.bgroup {\r\n%s\r\n}" (indent (print Precedence.Default e))
                |> wrap Precedence.Default

            | AddressOf e ->
                sprintf "&&%s" (print Precedence.Prefix e)
                |> wrap Precedence.Prefix

            | Application(a, b) ->
                let a = print Precedence.Application a
                let b = print Precedence.Application b
                sprintf "(%s) (%s)" a b
                |> wrap Precedence.Application

            | Call(None, mi, args) ->
                let args = args |> List.map (print Precedence.Default) |> String.concat ", "
                sprintf "%s.%s(%s)" (typeName mi.DeclaringType) mi.Name args
                |> wrap Precedence.Call
                
            | Call(Some t, mi, args) ->
                let t = print Precedence.Dot t
                let args = args |> List.map (print Precedence.Comma) |> String.concat ", "
                sprintf "%s.%s(%s)" t mi.Name args
                |> wrap Precedence.Call

            | Range(min, step, max) ->
                let min = print Precedence.Comma min
                let step = print Precedence.Comma step
                let max = print Precedence.Comma max
                sprintf "%s .. %s .. %s" min step max
                |> wrap Precedence.Comma

            | Coerce(e, t) ->
                sprintf "%s :> %s" (print Precedence.Default e) (typeName t)
                |> wrap Precedence.Comma

            | DefaultValue t ->
                sprintf "Unchecked.defaultof<%s>" (typeName t)

            | Draw info ->
                sprintf "draw %s" (print Precedence.Application info)
                |> wrap Precedence.Call

            | FieldGet(t, f) ->
                let t =
                    match t with
                    | None -> typeName f.DeclaringType
                    | Some t -> print Precedence.Dot t
                sprintf "%s.%s" t f.Name
                |> wrap Precedence.Dot
                
            | FieldSet(t, f, v) ->
                let t =
                    match t with
                    | None -> typeName f.DeclaringType
                    | Some t -> print Precedence.Dot t
                sprintf "%s.%s <- %s" t f.Name (print Precedence.Default v)
                |> wrap Precedence.Dot

            | For(v, elems, body) ->
                sprintf "for %s in %s do\r\n%s" v.Name (print Precedence.Default elems) (indent (print Precedence.Default body))
                |> wrap Precedence.Default

            | IfThenElse(c, i, Unit) ->
                let i = print Precedence.Default i 
                if multiLine i then
                    sprintf "if %s then\r\n%s" (print Precedence.Comma c) (indent i)
                    |> wrap Precedence.Default
                else
                    sprintf "if %s then %s" (print Precedence.Comma c) i
                    |> wrap Precedence.Default
                
            | IfThenElse(c, i, e) ->
                let i = print Precedence.Default i 
                let e = print Precedence.Default e

                if multiLine i || multiLine e then

                    sprintf "if %s then\r\n%s\r\nelse\r\n%s" (print Precedence.Comma c) (indent i) (indent e)
                    |> wrap Precedence.Default
                else
                    sprintf "if %s then %s else %s" (print Precedence.Comma c) i e
                    |> wrap Precedence.Default
                    

            | Lambdas(vars, body) ->
                let pars = vars |> List.map (List.map (fun v -> sprintf "%s : %s" v.Name (typeName v.Type)) >> String.concat ", " >> sprintf "(%s)") |> String.concat " "
                sprintf "fun %s ->\r\n%s" pars (indent (print Precedence.Default body))
                |> wrap Precedence.Default

            | Let(v, e, b) ->
                let e = print Precedence.Default e

                if multiLine e then
                    if v.IsMutable then
                        sprintf "let mutable %s =\r\n%s\r\n%s" v.Name (indent e) (print Precedence.Default b)
                        |> wrap Precedence.Default
                    else
                        sprintf "let %s =\r\n%s\r\n%s" v.Name (indent e) (print Precedence.Default b)
                        |> wrap Precedence.Default
                else
                    if v.IsMutable then
                        sprintf "let mutable %s = %s\r\n%s" v.Name e  (print Precedence.Default b)
                        |> wrap Precedence.Default
                    else
                        sprintf "let %s = %s\r\n%s" v.Name e (print Precedence.Default b)
                        |> wrap Precedence.Default

            | NewArray(t, [len]) ->
                sprintf "Array.zeroCreate<%s>(%s)" (typeName t) (print Precedence.Comma len)
                |> wrap Precedence.Dot
            | NewArray _ ->
                failwith "not implemented"

            | NewDelegate(t, vars, body) ->
                sprintf "%s(fun %s ->\r\n%s\r\n)" (typeName t) (vars |> Seq.map (fun v -> sprintf "%s : %s" v.Name (typeName v.Type)) |> String.concat " ") (indent (print Precedence.Default body))
                |> wrap Precedence.Call

            | NewObject(ctor, args) ->
                let args = args |> List.map (print Precedence.Comma) |> String.concat ", "
                sprintf "new %s(%s)" (typeName ctor.DeclaringType) args
                |> wrap Precedence.Call

            | NewRecord(t, args) ->
                let fields = FSharpType.GetRecordFields t |> Array.toList

                let fields = 
                    (fields, args) 
                    ||> List.map2 (fun f a -> sprintf "    %s = %s" f.Name (print Precedence.Comma a))
                    |> String.concat "\r\n"

                sprintf "{\r\n%s\r\n}" fields
                |> wrap Precedence.Dot

            | NewTuple(args) ->
                args
                |> List.map (print Precedence.Comma)
                |> String.concat ", "
                |> sprintf "(%s)"

            | NewUnionCase(ci, [h;t]) when ci.DeclaringType.IsGenericType && ci.DeclaringType.GetGenericTypeDefinition() = typedefof<list<_>> ->
                let rec all (t : SgExpr) =
                    match t with
                    | NewUnionCase(ci, [h;t]) when ci.DeclaringType.IsGenericType && ci.DeclaringType.GetGenericTypeDefinition() = typedefof<list<_>> ->
                        let known, rest = all t
                        h :: known, rest
                    | NewUnionCase(ci,[]) when ci.DeclaringType.IsGenericType && ci.DeclaringType.GetGenericTypeDefinition() = typedefof<list<_>> ->
                        [], None
                    | _ ->
                        [], Some t

                let known, rest = all t
                let known = h :: known
                match rest with
                | None ->
                    known |> List.map (print Precedence.Comma) |> String.concat "; " |> sprintf "[%s]"
                    |> wrap Precedence.Dot
                | Some r ->
                    let a = known |> List.map (print Precedence.Comma) |> String.concat "::"
                    sprintf "%s::%s" a (print Precedence.Comma r)
                    |> wrap Precedence.Call

            | NewUnionCase(ci, args) ->
                args
                |> List.map (print Precedence.Comma)
                |> String.concat ", "
                |> sprintf "%s(%s)" ci.Name
                |> wrap Precedence.Call
                
            | PropertyGet(t, f, idx) ->
                let t =
                    match t with
                    | None -> typeName f.DeclaringType
                    | Some t -> print Precedence.Dot t

                let t =
                    match idx with
                    | [] -> t
                    | idx -> idx |> Seq.map (print Precedence.Comma) |> String.concat ", " |> sprintf "%s.[%s]" t

                sprintf "%s.%s" t f.Name
                |> wrap Precedence.Dot

                
            | PropertySet(t, f, idx, v) ->
                let t =
                    match t with
                    | None -> typeName f.DeclaringType
                    | Some t -> print Precedence.Dot t

                let t =
                    match idx with
                    | [] -> t
                    | idx -> idx |> Seq.map (print Precedence.Comma) |> String.concat ", " |> sprintf "%s.[%s]" t

                sprintf "%s.%s <- %s" t f.Name (print Precedence.Default v)
                |> wrap Precedence.Dot

            | QuoteRaw(e) ->
                print Precedence.Default e |> indent |> sprintf "<@@\r\n%s\r\n@@>"
                |> wrap Precedence.Dot
                
            | QuoteTyped(e) ->
                print Precedence.Default e |> indent |> sprintf "<@\r\n%s\r\n@>"
                |> wrap Precedence.Dot

            | Sequential(l, r) ->
                print scopePrecedence l + "\r\n" + print scopePrecedence r
                |> wrap Precedence.Comma

            | SetInstanceAttribute(name, value, rest) ->
                sprintf "instanceData %s %s\r\n%s" (print Precedence.Application name) (print Precedence.Application value) (print Precedence.Default rest)
                |> wrap Precedence.Comma
                
            | SetVertexAttribute(name, value, rest) ->
                sprintf "vertexData %s %s\r\n%s" (print Precedence.Application name) (print Precedence.Application value) (print Precedence.Default rest)
                |> wrap Precedence.Comma
                
            | SetUniform(name, value, rest) ->
                sprintf "uniform %s %s\r\n%s" (print Precedence.Application name) (print Precedence.Application value) (print Precedence.Default rest)
                |> wrap Precedence.Comma
                
            | SetShader(shader, rest) ->
                sprintf "shader %s\r\n%s" (print Precedence.Application shader) (print Precedence.Default rest)
                |> wrap Precedence.Comma

            | TryFinally(a, b) ->
                sprintf "try\r\n%s\r\nfinally\r\n%s" (indent (print Precedence.Default a)) (indent (print Precedence.Default  b))
                |> wrap Precedence.Default

            | TryWith _ ->
                failwith "implement me"

            | TupleGet(t, i) ->
                sprintf "%s.Item%d" (print Precedence.Dot t) (i + 1)
                |> wrap Precedence.Dot

            | TypeTest(e, t) ->
                sprintf "(match %s with :? %s -> true | _ -> false)" (print Precedence.Comma e) (typeName t)

            | UnionCaseTest _ ->
                failwith "implement me"

            | Unit ->
                "()"

            | Value(v, t) ->
                let str = sprintf "%A" v
                let str =
                    if str.Length > 14 then str.Substring(0, 11) + "..."
                    else str
                sprintf "(%s : %s)" str  (typeName t)

            | Var v ->
                v.Name
                
            | VarSet(v, e) ->
                sprintf "%s <- %s" v.Name (print Precedence.Comma e)

            | WhileLoop(guard, body) ->
                sprintf "while %s do\r\n%s" (print Precedence.Comma guard) (indent (print Precedence.Default body))

            | Yield a ->
                print Precedence.Default a

    module ConstantFolding =

        module Purity = 
            open Aardvark.Base.IL

            let private purity = System.Collections.Generic.Dictionary<MethodBase, ref<bool>>()


            let rec isPure (mi : MethodBase) =
                lock purity (fun () ->
                    match purity.TryGetValue mi with
                    | (true, r) -> !r
                    | _ ->
                        let r = ref true
                        purity.[mi] <- r

                        if mi.DeclaringType.Namespace <> "Microsoft.FSharp.Core" then
                            let def = Disassembler.disassemble mi
                            for i in def.Body do
                                match i with
                                | Instruction.CallIndirect -> r := false
                                | Instruction.Call m ->
                                    if not (isPure m) then r := false
                                | Instruction.Stfld _ ->
                                    r := false
                                | _ ->
                                    ()
                        
                        !r
                )



        let rec (|AllValues|_|) (l : list<SgExpr>) =
            match l with
            | [] -> Some []
            | Value(v0, t0) :: AllValues rest -> Some ((v0, t0) :: rest)
            | _ -> None

        let rec evaluateConstantsS (e : SgExpr) =
            state {
                match e with
                | Group e -> 
                    let! e = evaluateConstantsS e
                    return Group e

                | AddressOf e ->
                    let! e = evaluateConstantsS e
                    return AddressOf e
                    
                | Application(a, b) ->
                    let! a = evaluateConstantsS a
                    let! b = evaluateConstantsS b
                    match a, b with
                    | Value(va, ta), Value(vb, _) ->
                        try
                            let (arg, ret) = FSharpType.GetFunctionElements ta
                            let invoke = ta.GetMethod("Invoke", [| arg |])
                            let res = invoke.Invoke(va, [|vb|])
                            return Value(res, ret)
                        with _ ->
                            return Application(a,b)
                    | _ ->
                        return Application(a,b)

                | Call(None, mi, args) ->
                    let! args = args |> List.mapS evaluateConstantsS
                    let constant = args |> List.forall (function Value _ -> true | _ -> false)

                    let pure = Purity.isPure mi

                    if constant && pure then
                        try
                            let args = args |> List.map (function Value(v,_) -> v | _ -> failwith "impossible") |> List.toArray
                            return Value(mi.Invoke(null, args), mi.ReturnType)
                        with _ ->
                            return Call(None, mi, args)
                    else
                        return Call(None, mi, args)
                        
                | Call(Some t, mi, args) ->
                    let! t = evaluateConstantsS t
                    let! args = args |> List.mapS evaluateConstantsS

                    match t with
                    | Value(tv, _) -> 
                        //match args with
                        //| AllValues values when mi.ReturnType <> typeof<unit> && mi.ReturnType <> typeof<System.Void> ->
                        //    try
                        //        let args = values |> List.map fst |> List.toArray
                        //        return Value(mi.Invoke(tv, args), mi.ReturnType)
                        //    with _ ->
                        //        return Call(Some t, mi, args)
                        //| args ->
                            return Call(Some t, mi, args)
                    | _ ->
                        return Call(Some t, mi, args)

                | Range(min, step, max) ->
                    let! min = evaluateConstantsS min
                    let! step = evaluateConstantsS step
                    let! max = evaluateConstantsS max
                    // TODO: completely constant
                    return Range(min, step, max)

                | Coerce(e, t) ->
                    match! evaluateConstantsS e with
                    | Value(ve,_) -> return Value(ve, t)
                    | e -> return Coerce(e, t)

                | DefaultValue t ->
                    if t.IsValueType then return Value(Activator.CreateInstance t, t)
                    else return Value(null, t)

                | Draw e ->
                    let! e = evaluateConstantsS e
                    return Draw e

                | FieldGet(None, f) ->
                    return Value(f.GetValue null, f.FieldType)
                    
                | FieldGet(Some t, f) ->
                    match! evaluateConstantsS t with
                    | Value(vt, _) ->
                        return Value(f.GetValue vt, f.FieldType)
                    | e ->
                        return FieldGet(Some e, f)

                | FieldSet(t, f, value) ->
                    let! t = Option.mapS evaluateConstantsS t
                    let! value = evaluateConstantsS value
                    return FieldSet(t, f, value)

                | For(v, elems, body) ->
                    let! elems = evaluateConstantsS elems
                    let! body = evaluateConstantsS body
                    return For(v, elems, body)

                | IfThenElse(c, i, e) ->
                    match! evaluateConstantsS c with
                    | Value(cond, _) ->
                        if unbox cond then
                            return! evaluateConstantsS i
                        else
                            return! evaluateConstantsS e
                    | c ->
                        let! i = evaluateConstantsS i
                        let! e = evaluateConstantsS e
                        return IfThenElse(c, i, e)

                | Lambdas(vars, body) ->
                    let! body = evaluateConstantsS body
                    return Lambdas(vars, body)

                | NewArray(t, lens) ->
                    let! lens = lens |> List.mapS evaluateConstantsS
                    return NewArray(t, lens)
                    
                | NewDelegate(t, vars, body) ->
                    let! body = evaluateConstantsS body
                    return NewDelegate(t, vars, body)

                | NewObject(ctor, args) ->
                    match! args |> List.mapS evaluateConstantsS with
                    | AllValues values ->
                        try
                            let vs = values |> List.map fst |> List.toArray
                            return Value(ctor.Invoke(vs), ctor.DeclaringType)
                        with _ ->
                            return NewObject(ctor, List.map Value values)
                    | args ->
                        return NewObject(ctor, args)
                        
                | NewRecord(t, args) ->
                    match! args |> List.mapS evaluateConstantsS with
                    | AllValues values ->
                        try
                            let vs = values |> List.map fst |> List.toArray
                            return Value(FSharpValue.MakeRecord(t, vs), t)
                        with _ ->
                            return NewRecord(t, List.map Value values)
                    | args ->
                        return  NewRecord(t, args)
                    
                | NewTuple(args) ->
                    match! args |> List.mapS evaluateConstantsS with
                    | AllValues values ->
                        let typ = 
                            values |> List.map snd |> List.toArray |> FSharpType.MakeTupleType
                        try
                            let vs = values |> List.map fst |> List.toArray
                            return Value(FSharpValue.MakeTuple(vs, typ), typ)
                        with _ ->
                            return NewTuple(List.map Value values)
                    | args ->
                        return NewTuple(args)
                    
                    
                | NewUnionCase(ctor, args) ->
                    match! args |> List.mapS evaluateConstantsS with
                    | AllValues values ->
                        try
                            let vs = values |> List.map fst |> List.toArray
                            return Value(FSharpValue.MakeUnion(ctor, vs), ctor.DeclaringType)
                        with _ ->
                            return NewUnionCase(ctor, List.map Value values)
                    | args ->
                        return NewUnionCase(ctor, args)
                        
                | PropertyGet(None, f, idx) ->
                    match! List.mapS evaluateConstantsS idx with
                    | AllValues values -> 
                        let idx = values |> List.map fst |> List.toArray
                        return Value(f.GetValue(null, idx), f.PropertyType)
                    | args ->
                        return PropertyGet(None, f, args)
                    
                | PropertyGet(Some t, f, idx) ->
                    match! evaluateConstantsS t with
                    | Value(vt, _) as t ->
                        let! idx = List.mapS evaluateConstantsS idx
                        match idx with
                        | AllValues values -> 
                            let idx = values |> List.map fst |> List.toArray
                            return Value(f.GetValue(vt, idx), f.PropertyType)
                        | args ->
                            return PropertyGet(Some t, f, args)
                    | e ->
                        let! idx = List.mapS evaluateConstantsS idx
                        return PropertyGet(Some e, f, idx)
                        
                | PropertySet(t, f, idx, value) ->
                    let! t = t |> Option.mapS evaluateConstantsS
                    let! idx = idx |> List.mapS evaluateConstantsS
                    let! value = evaluateConstantsS value
                    return PropertySet(t, f, idx, value)

                | QuoteRaw e ->
                    let! e = evaluateConstantsS e
                    return QuoteRaw e
                    
                | QuoteTyped e ->
                    let! e = evaluateConstantsS e
                    return QuoteTyped e

                | Sequential(l, r) ->
                    let! l = evaluateConstantsS l
                    let! r = evaluateConstantsS r
                    match l with
                    | Unit | Value _ ->
                        return r
                    | _ ->
                        return Sequential(l, r)

                | SetInstanceAttribute(name, value, rest) ->
                    let! name = evaluateConstantsS name
                    let! value = evaluateConstantsS value
                    let! rest = evaluateConstantsS rest
                    return SetInstanceAttribute(name, value, rest)

                | SetVertexAttribute(name, value, rest) ->
                    let! name = evaluateConstantsS name
                    let! value = evaluateConstantsS value
                    let! rest = evaluateConstantsS rest
                    return SetVertexAttribute(name, value, rest)
                    
                | SetUniform(name, value, rest) ->
                    let! name = evaluateConstantsS name
                    let! value = evaluateConstantsS value
                    let! rest = evaluateConstantsS rest
                    return SetUniform(name, value, rest)
                    
                | SetShader(shader, rest) ->
                    let! shader = evaluateConstantsS shader
                    let! rest = evaluateConstantsS rest
                    return SetShader(shader, rest)

                | TryFinally(a,b) ->
                    let! a = evaluateConstantsS a
                    let! b = evaluateConstantsS b
                    return TryFinally(a, b)

                | TryWith _ ->
                    return failwith "implement me"

                | TupleGet(t, i) ->
                    match! evaluateConstantsS t with
                    | Value(vt, tt) ->
                        return Value(FSharpValue.GetTupleField(vt, i), FSharpType.GetTupleElements(tt).[i])
                    | t ->
                        return TupleGet(t, i)

                | TypeTest(e, t) ->
                    let! e = evaluateConstantsS e
                    // TODO: constants
                    return TypeTest(e, t)
                    
                | UnionCaseTest _ ->
                    return failwith "implement me"

                | Unit ->
                    return Unit

                | Value _ as e ->
                    return e

                | VarSet(v, e) ->
                    let! e = evaluateConstantsS e
                    return VarSet(v, e)

                | WhileLoop(guard, body) ->
                    let! guard = evaluateConstantsS guard
                    let! body = evaluateConstantsS body
                    return WhileLoop(guard, body)

                | Yield a ->
                    let! a = evaluateConstantsS a
                    return a

                | Let(v, e, b) ->
                    let! e = evaluateConstantsS e
                    match e with
                    | Value(o, t) when not v.IsMutable ->
                        do! State.modify (fun s -> HashMap.add v (o,t) s)
                        return! evaluateConstantsS b
                    | _ ->
                        let! b = evaluateConstantsS b
                        return Let(v, e, b)

                | Var v ->
                    let! s = State.get
                    match HashMap.tryFind v s with
                    | Some (o,t) ->
                        return Value(o, t)
                    | None ->
                        return Var v

            }
            
    let evaluateConstants (e : SgExpr) =
        let mutable state = HashMap.empty
        ConstantFolding.evaluateConstantsS(e).Run(&state)

    let ofExpr (e : Microsoft.FSharp.Quotations.Expr) =
        let mutable state = Map.empty
        Translation.ofExprS(e).Run(&state)

    let rec substitute (replace : SgExpr -> option<SgExpr>) (e : SgExpr) =
        match replace e with
        | Some r ->
            r
        | None ->
            match e with
            | Group e -> Group (substitute replace e)
            | Draw e -> Draw (substitute replace e)
            | SetVertexAttribute(name, value, rest) -> SetVertexAttribute(substitute replace name, substitute replace value, substitute replace rest)
            | SetInstanceAttribute(name, value, rest) -> SetInstanceAttribute(substitute replace name, substitute replace value, substitute replace rest)
            | SetUniform(name, value, rest) -> SetUniform(substitute replace name, substitute replace value, substitute replace rest)
            | SetShader(shader, rest) -> SetShader(substitute replace shader, substitute replace rest)
            | Yield e -> Yield (substitute replace e)

            | AddressOf e -> AddressOf (substitute replace e) 
            | Application(a, b) -> Application (substitute replace a, substitute replace b) 
            | Call(target, mi, args) -> Call (Option.map (substitute replace) target, mi, List.map (substitute replace) args) 
            | Coerce(e, t) -> Coerce(substitute replace e, t) 
            | DefaultValue t -> DefaultValue t
            | FieldGet(t, f) -> FieldGet(Option.map (substitute replace) t, f)
            | FieldSet(t, f, v) -> FieldSet(Option.map (substitute replace) t, f, v)
            | For(v, e, b) -> For(v, substitute replace e, substitute replace b)
            | IfThenElse(c, i, e) -> IfThenElse(substitute replace c, substitute replace i, substitute replace e)
            | Lambdas(vars, body) -> Lambdas(vars, substitute replace body)
            | Let(v, e, b) -> Let(v, substitute replace e, substitute replace b)
            | NewArray(t, lens) -> NewArray(t, lens |> List.map (substitute replace))
            | NewDelegate(t, vars, body) -> NewDelegate(t, vars, substitute replace body)
            | NewObject(ctor, args) -> NewObject(ctor, List.map (substitute replace) args)
            | NewRecord(t, args) -> NewRecord(t, List.map (substitute replace) args)
            | NewTuple(args) -> NewTuple(List.map (substitute replace) args)
            | NewUnionCase(ci, args) -> NewUnionCase(ci, List.map (substitute replace) args)
            | PropertyGet(t, p, idx) -> PropertyGet(Option.map (substitute replace) t, p, List.map (substitute replace) idx)
            | PropertySet(t, p, idx, v) -> PropertySet(Option.map (substitute replace) t, p, List.map (substitute replace) idx, substitute replace v)
            | QuoteRaw e -> QuoteRaw (substitute replace e)
            | QuoteTyped e -> QuoteTyped (substitute replace e)
            | Sequential(l, r) -> Sequential (substitute replace l, substitute replace r)
            | TryFinally(a,b) -> TryFinally(substitute replace a, substitute replace b)
            | TryWith(a,v0,b,v1,c) -> TryWith(substitute replace a, v0, substitute replace b, v1, substitute replace c)
            | TupleGet(a,b) -> TupleGet(substitute replace a, b)
            | TypeTest(a,b) -> TypeTest(substitute replace a, b)
            | UnionCaseTest(a,b) -> UnionCaseTest(substitute replace a, b)
            | Value _ | Var _ | Unit -> e
            | VarSet(v, e) -> VarSet(v, substitute replace e)
            | WhileLoop(g, e) -> WhileLoop(substitute replace g, substitute replace e)
            | Range(a,b,c) -> Range(substitute replace a, substitute replace b, substitute replace c)

    let substituteVar (replace : SgVar -> option<SgExpr>) (e : SgExpr) =
        e |> substitute (function
            | Var v -> replace v
            | _ -> None
        )

    let toString (e : SgExpr) =
        Print.print Print.Precedence.Default e

    
