module RangeSetTests

open FSharp.Data.Adaptive
open Aardvark.Base
open Armadillo
open Expecto
open FsUnit

type SimpleRangeSet(values : Set<int>) =
    


    static let allValues(ranges : seq<Range1i>) =
        
        let mutable set = Set.empty
        for r in ranges do
            for v in r.Min .. r.Max do
                set <- Set.add v set
        set

    static member Union(l : SimpleRangeSet, r : SimpleRangeSet) =
        SimpleRangeSet(Set.union l.Values r.Values)
        
    static member Difference(l : SimpleRangeSet, r : SimpleRangeSet) =
        SimpleRangeSet(Set.difference l.Values r.Values)
        
    static member Intersection(l : SimpleRangeSet, r : SimpleRangeSet) =
        SimpleRangeSet(Set.intersect l.Values r.Values)

    member x.Values = values

    member x.ToList() =
        use e = (values :> seq<_>).GetEnumerator()

        let res = ListBuilder()
        if e.MoveNext() then
            let mutable start = e.Current
            let mutable current = start
            while e.MoveNext() do
                if e.Current = current + 1 then
                    current <- e.Current
                else
                    // range done
                    res.Append(Range1i(start, current))
                    start <- e.Current
                    current <- start

            if current >= start then
                res.Append(Range1i(start, current))

            res.ToList()
        else
            []

    member x.ToValueList() =
        Set.toList values

    member x.Add(range : Range1i) =
        let mutable set = values
        for v in range.Min .. range.Max do
            set <- Set.add v set
        SimpleRangeSet(set)

    member x.Remove(range : Range1i) =
        let mutable set = values
        for v in range.Min .. range.Max do
            set <- Set.remove v set
        SimpleRangeSet(set)

    member x.Check(other : RangeSet) =
        let a = x.ToList()
        let b = other.ToList()
        let d = List.computeDelta a b
        match d with
        | [] ->
            ()
        | deltas ->
            failwithf "toList: \r\n%A\r\n%A\r\n%A" a b deltas

            
        let a = x.ToValueList()
        let b = other.ToValueList()
        let d = List.computeDelta a b
        match d with
        | [] ->
            ()
        | deltas ->
            failwithf "ToValueList: \r\n%A\r\n%A\r\n%A" a b deltas

            
        let b = other.ToValueSeq() |> Seq.toList
        let d = List.computeDelta a b
        match d with
        | [] ->
            ()
        | deltas ->
            failwithf "ToValueSeq: \r\n%A\r\n%A\r\n%A" a b deltas

        let b = other.ToValueArray() |> Array.toList
        let d = List.computeDelta a b 
        match d with
        | [] ->
            ()
        | deltas ->
            failwithf "ToValueArray: \r\n%A\r\n%A\r\n%A" a b deltas


        let rec run (s : RangeSet) =
            match s.TryDequeue() with
            | Some (v, rest) ->
                v :: run rest
            | None ->
                []
        let b = run other
        let d = List.computeDelta a b 
        match d with
        | [] ->
            ()
        | deltas ->
            failwithf "TryDequeue: \r\n%A\r\n%A\r\n%A" a b deltas

        for v in values do
            if not (RangeSet.contains v other) then
                failwithf "does not contain %A %d" other v

        for r in x.ToList() do
            if not (RangeSet.containsRange r other) then
                failwithf "does not contain %A %A" other r
            
        ()


    new(ranges : seq<Range1i>) =
        SimpleRangeSet(allValues ranges)

[<Tests>]
let RangeSet =
    testList "RangeSet" [
        property "ofSeq" <| fun (a : list<Range1i>) ->
            let set = RangeSet.ofSeq a
            let simple = SimpleRangeSet a

            simple.Check set

            
        property "add" <| fun (a : list<Range1i>) (other : Range1i) ->
            let set = RangeSet.ofSeq a
            let simple = SimpleRangeSet a

            let set = RangeSet.add other set
            let simple = simple.Add other

            simple.Check set
            
        property "remove" <| fun (a : list<Range1i>) (other : Range1i) ->
            let set = RangeSet.ofSeq a
            let simple = SimpleRangeSet a

            let set = RangeSet.remove other set
            let simple = simple.Remove other

            simple.Check set

            
        property "union" <| fun (a : list<Range1i>) (b : list<Range1i>)  ->
            let sa = RangeSet.ofSeq a
            let ta = SimpleRangeSet a
            let sb = RangeSet.ofSeq b
            let tb = SimpleRangeSet b


            let s = RangeSet.union sa sb
            let t = SimpleRangeSet.Union(ta, tb)

            t.Check s
            
        property "difference" <| fun (a : list<Range1i>) (b : list<Range1i>)  ->
            let sa = RangeSet.ofSeq a
            let ta = SimpleRangeSet a
            let sb = RangeSet.ofSeq b
            let tb = SimpleRangeSet b


            let s = RangeSet.difference sa sb
            let t = SimpleRangeSet.Difference(ta, tb)

            t.Check s

            
        property "intersect" <| fun (a : list<Range1i>) (b : list<Range1i>)  ->
            let sa = RangeSet.ofSeq a
            let ta = SimpleRangeSet a
            let sb = RangeSet.ofSeq b
            let tb = SimpleRangeSet b


            let s = RangeSet.intersect sa sb
            let t = SimpleRangeSet.Intersection(ta, tb)

            t.Check s




    ]