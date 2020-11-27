namespace Armadillo


type State<'s, 'a>() =
    abstract member Run : byref<'s> -> 'a
    abstract member RunUnit : byref<'s> -> unit

    default x.Run(s) = x.RunUnit(&s); Unchecked.defaultof<'a>
    default x.RunUnit(s) = x.Run(&s) |> ignore

module State =
                
    let inline value (value : 'a) =
        { new State<'s, 'a>() with
            member x.Run(_) = value
        }

    let inline get<'s> =
        { new State<'s, 's>() with
            member x.Run(s) = s
        }
                    
    let inline put (state : 's) =
        { new State<'s, unit>() with
            member x.RunUnit(s) = s <- state
        }
                        
    let inline modify (mapping : 's -> 's) =
        { new State<'s, unit>() with
            member x.RunUnit(s) = s <- mapping s
        }
                    
    let inline custom (mapping : 's -> 's * 'a) =
        { new State<'s, 'a>() with
            member x.Run(s) = 
                let (ns, v) = mapping s
                s <- ns
                v
        }

    let inline maps (mapping : 's -> 'a -> 'b) (state : State<'s, 'a>) =
        { new State<'s, 'b>() with
            member x.Run(s) =
                let a = state.Run(&s)
                mapping s a
        }

    let inline map (mapping : 'a -> 'b) (state : State<'s, 'a>) =
        { new State<'s, 'b>() with
            member x.Run(s) =
                state.Run(&s) |> mapping
        }
                    
    let inline binds (mapping : 's -> 'a -> State<'s, 'b>) (state : State<'s, 'a>) =
        { new State<'s, 'b>() with
            member x.Run(s) =
                let a = state.Run(&s)
                (mapping s a).Run(&s)
        }

    let inline bind (mapping : 'a -> State<'s, 'b>) (state : State<'s, 'a>) =
        { new State<'s, 'b>() with
            member x.Run(s) =
                let a = state.Run(&s)
                (mapping a).Run(&s)
        }

    let inline ignore (state : State<'s, 'a>) =
        { new State<'s, unit>() with
            member x.RunUnit(s) = state.Run(&s) |> Operators.ignore
        }

    let inline delay (action : unit -> State<'s, 'a>) =
        { new State<'s, 'a>() with
            member x.Run(s) = action().Run(&s)
        }

    let inline combine (l : State<'s, unit>) (r : State<'s, 'a>) =
        { new State<'s, 'a>() with
            member x.Run(s) = l.RunUnit(&s); r.Run(&s)
        }

    let inline tryWith (inner : State<'s, 'a>) (handler : exn -> State<'s, 'a>) =
        { new State<'s, 'a>() with
            member x.Run(s) =
                try 
                    let mutable innerState = s
                    let res = inner.Run(&innerState)
                    s <- innerState
                    res
                with e ->
                    handler(e).Run(&s)
        }

    let inline tryFinally (inner : State<'s, 'a>) (fin : State<'s, unit>) =
        { new State<'s, 'a>() with
            member x.Run(s) =
                try
                    let mutable innerState = s
                    let res = inner.Run(&innerState)
                    s <- innerState
                    res
                finally
                    fin.RunUnit(&s)
        }

    type StateBuilder() =
        member x.Return v = value v
        member x.ReturnFrom (v : State<_, _>) = v
        member x.Bind(m, mapping) = bind mapping m
        member x.Delay action = delay action
        member x.Run (s : State<_,_>) = s
        member x.Combine(l, r) = combine l r
        member x.TryWith(a, b) = tryWith a b
        member x.TryFinally(a,b) = tryFinally a b
        member x.Zero() = value()

[<AutoOpen>]
module ``State Builder`` =

    let state = State.StateBuilder()

    module Option =
        let mapS (mapping : 'a -> State<'s, 'b>) (value : option<'a>) =
            { new State<'s, option<'b>>() with
                member x.Run(s) =
                    match value with
                    | Some v ->
                        mapping(v).Run(&s) |> Some
                    | None ->
                        None
            }
        

    module List =
        let mapS (mapping : 'a -> State<'s, 'b>) (list : list<'a>) =
            { new State<'s, list<'b>>() with
                member x.Run(s) =
                    let mutable state = s
                    let res = list |> List.map (fun a -> mapping(a).Run(&state))
                    s <- state
                    res
            }
