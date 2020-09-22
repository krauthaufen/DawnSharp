namespace Armadillo

open System.Reflection
open Aardvark.Base
open Aardvark.Base.IL

/// Utility for setting a list's tail via mutation.
type private TailSetter<'a> private() =
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

/// ListBuilder allows to build F#-lists by Appending/Prepending efficiently
type ListBuilder<'a>() =
    let mutable start = []
    let mutable current = start
    let mutable count = 0

    member x.Count = count

    /// Clears the content of the ListBuilder.
    member x.Clear() =
        start <- []
        current <- start
        count <- 0

    /// Prepends a value to the ListBuilder.
    member x.Prepend(value : 'a) =
        match start with
        | [] ->
            start <- [value]
            current <- start
        | _ -> 
            start <- value :: start
        count <- count + 1
        
    /// Appends a value to the ListBuilder.
    member x.Append(value : 'a) =
        match start with
        | [] ->
            start <- [value]
            current <- start
        | _ ->
            let m = [value]
            TailSetter<'a>.SetTail(current, m)
            current <- m
        count <- count + 1
                
    /// Appends multiple values to the ListBuilder.
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
                TailSetter<'a>.SetTail(current, m)
                current <- m
            count <- count + 1

            for e in t do 
                let m = [e]
                TailSetter<'a>.SetTail(current, m)
                current <- m
                count <- count + 1

    /// Returns the current state as a list (and resets the ListBuilder).
    member x.ToList() =
        let res = start
        start <- []
        current <- start
        count <- 0
        res
