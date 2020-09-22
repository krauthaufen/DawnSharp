module ListBuilderTests

open FSharp.Data.Adaptive
open Armadillo
open Expecto


[<Tests>]
let ListBuilder =
    testList "ListBuilder" [
        property "append" <| fun (a : list<obj>) ->
            let b = ListBuilder()
            for e in a do b.Append e


            Expect.equal b.Count (List.length a) "builder count wrong"
            let res = b.ToList()
            Expect.equal res a "builder result broken"
            
        property "appendMany" <| fun (a : list<obj>) ->
            let b = ListBuilder<obj>()
            b.Append a
            
            Expect.equal b.Count (List.length a) "builder count wrong"
            let res = b.ToList()
            Expect.equal res a "builder result broken"
            
        property "prepend" <| fun (a : list<obj>) ->
            let b = ListBuilder()
            for e in a do b.Prepend e
            
            Expect.equal b.Count (List.length a) "builder count wrong"
            let res = b.ToList() |> List.rev
            Expect.equal res a "builder result broken"
            
        property "clear" <| fun (a : list<obj>) ->
            let b = ListBuilder<obj>()
            b.Append a
            
            Expect.equal b.Count (List.length a) "builder count wrong"
            b.Clear()
            
            Expect.equal b.Count 0 "builder count wrong"
            let res = b.ToList()
            Expect.isEmpty res "builder result broken"
            
        property "reuse" <| fun (a : list<obj>) ->
            let b = ListBuilder()
            for e in a do b.Append e
            Expect.equal b.Count (List.length a) "builder count wrong"
            let res = b.ToList()
            Expect.equal res a "builder result broken"
            
            Expect.equal b.Count 0 "builder count wrong"
            
            for e in a do b.Append e
            Expect.equal b.Count (List.length a) "builder count wrong"
            let res = b.ToList()
            Expect.equal res a "builder result broken"
            Expect.equal b.Count 0 "builder count wrong"

            
    ]