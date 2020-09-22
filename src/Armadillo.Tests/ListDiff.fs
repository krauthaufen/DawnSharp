module ListDiffTests

open FSharp.Data.Adaptive
open Armadillo
open Expecto


[<Tests>]
let ListDiffing =
    testList "ListDiffing" [
        property "general" <| fun (a : list<obj>, b : list<obj>) ->
            let l = IndexList.ofList a
            let ops = IndexList.computeDeltaToList l b
            let (tl, ed) = IndexList.applyDelta l ops
            let t = tl |> IndexList.toList

            Expect.equal t b "list broken"
            Expect.equal ed ops "delta broken"

        property "nop" <| fun (a : list<obj>) ->
            let l = IndexList.ofList a
            let ops = IndexList.computeDeltaToList l a
            Expect.isTrue ops.IsEmpty
            
        property "clear" <| fun (a : list<obj>) ->
            let l = IndexList.ofList a
            let ops = IndexList.computeDeltaToList l []
            Expect.equal ops.Count (List.length a) "not all removed"
            
            let (tl, _) = IndexList.applyDelta l ops
            let t = tl |> IndexList.toList
            Expect.isEmpty t "list not cleared"
            
        property "init" <| fun (a : list<obj>) ->
            let l = IndexList.empty
            let ops = IndexList.computeDeltaToList l a
            Expect.equal ops.Count (List.length a) "not all added"
            
            let (tl, _) = IndexList.applyDelta l ops
            let t = tl |> IndexList.toList
            Expect.equal t a "list broken"

    ]
