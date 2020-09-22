module HeapTests

open FSharp.Data.Adaptive
open Armadillo
open Expecto
open FsCheck

[<Tests>]
let Heap =
    testList "Heap" [
        property "order" <| fun (values : Map<int, NormalFloat>) ->
            let h = Heap<int, float>()
            for (k, v) in Map.toSeq values do
                h.EnqueueOrUpdate(k, v.Get)

            let res =
                [
                    while h.Count > 0 do
                        yield h.Dequeue()
                ]
                
            let sorted = values |> Map.toList |> List.map (fun (_,v) -> v.Get) |> List.sort
            Expect.equal res sorted "list not sorted"

        property "decrease key" <| fun (values : Map<int, NormalFloat>, k0 : int, v0 : NormalFloat) ->
            let values = Map.add k0 v0 values

            let h = Heap<int, float>()
            for (k, v) in Map.toSeq values do
                h.EnqueueOrUpdate(k, v.Get)

            h.EnqueueOrDecrease(k0, System.Double.NegativeInfinity)

            let res =
                [
                    while h.Count > 0 do
                        yield h.Dequeue()
                ]

            let sorted = 
                Map.add k0 (NormalFloat System.Double.NegativeInfinity) values 
                |> Map.toList 
                |> List.map (fun (_,v) -> v.Get) 
                |> List.sort

            Expect.equal res sorted "list not sorted"

            Expect.equal (List.head res) System.Double.NegativeInfinity "decrease wrong"
            
        property "increase key" <| fun (values : Map<int, NormalFloat>, k0 : int, v0 : NormalFloat) ->
            let values = Map.add k0 v0 values

            let h = Heap<int, float>()
            for (k, v) in Map.toSeq values do
                h.EnqueueOrUpdate(k, v.Get)

            h.EnqueueOrIncrease(k0, System.Double.PositiveInfinity)

            let res =
                [
                    while h.Count > 0 do
                        yield h.Dequeue()
                ]
                
            let sorted = 
                Map.add k0 (NormalFloat System.Double.PositiveInfinity) values 
                |> Map.toList 
                |> List.map (fun (_,v) -> v.Get) 
                |> List.sort

            Expect.equal res sorted "list not sorted"

            Expect.equal (List.last res) System.Double.PositiveInfinity "increase wrong"
  
        property "remove" <| fun (values : Map<int, NormalFloat>, k0 : int, v0 : NormalFloat) ->
            let values = Map.add k0 v0 values

            let h = Heap<int, float>()
            for (k, v) in Map.toSeq values do
                h.EnqueueOrUpdate(k, v.Get)

            Expect.isTrue (h.Remove k0) "remove returned false"

            let res =
                [
                    while h.Count > 0 do
                        yield h.Dequeue()
                ]
                
            let sorted = 
                Map.remove k0 values 
                |> Map.toList 
                |> List.map (fun (_,v) -> v.Get) 
                |> List.sort

            Expect.equal res sorted "list not sorted"

    ]