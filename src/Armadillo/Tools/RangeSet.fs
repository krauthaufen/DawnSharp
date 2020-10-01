namespace Armadillo

open Aardvark.Base
open System.Collections.Generic

[<AutoOpen>]
module private RangeSetHelpers = 
    [<Struct>]
    type HalfRangeKind =
        | Left
        | Right 


    type RangeSetEnumerator(e : IEnumerator<KeyValuePair<int, HalfRangeKind>>) =
        
        let mutable a = Unchecked.defaultof<_>
        let mutable b = Unchecked.defaultof<_>

        member x.MoveNext() =
            if e.MoveNext() then
                a <- e.Current
                if e.MoveNext() then
                    b <- e.Current
                    true
                else
                    failwithf "impossible"
            else
                false
            
        member x.Reset() =
            e.Reset()
            a <- Unchecked.defaultof<_>
            b <- Unchecked.defaultof<_>

        member x.Current =
            assert (a.Value = HalfRangeKind.Left && b.Value = HalfRangeKind.Right)
            Range1i(a.Key, b.Key - 1)

        member x.Dispose() =
            e.Dispose()
            a <- Unchecked.defaultof<_>
            b <- Unchecked.defaultof<_>

        interface System.Collections.IEnumerator with
            member x.MoveNext() = x.MoveNext()
            member x.Current = x.Current :> obj
            member x.Reset() = x.Reset()

        interface System.Collections.Generic.IEnumerator<Range1i> with
            member x.Dispose() = x.Dispose()
            member x.Current = x.Current

    type RangeSetValueEnumerator(e : IEnumerator<KeyValuePair<int, HalfRangeKind>>) =
    
        let mutable a = -1
        let mutable b = -1
        let mutable current = -1
    

        let nextRange() =
            if e.MoveNext() then
                a <- e.Current.Key
                if e.MoveNext() then
                    b <- e.Current.Key
                    true
                else
                    failwithf "impossible"
            else
                false
            
        member x.MoveNext() =
            if a >= b then
                if nextRange() then x.MoveNext()
                else false
            else
                current <- a
                a <- a + 1
                true

        member x.Current = current

        member x.Reset() =
            e.Reset()
            a <- -1
            b <- -1
            current <- -1

        member x.Dispose() =
            e.Dispose()
            a <- -1
            b <- -1
            current <- -1
        
        interface System.Collections.IEnumerator with
            member x.MoveNext() = x.MoveNext()
            member x.Current = x.Current :> obj
            member x.Reset() = x.Reset()

        interface System.Collections.Generic.IEnumerator<int> with
            member x.Dispose() = x.Dispose()
            member x.Current = x.Current

    type RangeSetValueEnumerable(store : MapExt<int, HalfRangeKind>) =
    
        interface System.Collections.IEnumerable with
            member x.GetEnumerator() = new RangeSetValueEnumerator((store :> seq<_>).GetEnumerator()) :> _
    
        interface IEnumerable<int> with
            member x.GetEnumerator() = new RangeSetValueEnumerator((store :> seq<_>).GetEnumerator()) :> _

    type SortedBinaryEnumerator(a : IEnumerator<KeyValuePair<int, HalfRangeKind>>, b : IEnumerator<KeyValuePair<int, HalfRangeKind>>) =
        
        static let cmp (l : KeyValuePair<int, HalfRangeKind>) (r : KeyValuePair<int, HalfRangeKind>) =
            compare l.Key r.Key

        let mutable current = Unchecked.defaultof<_>
        let mutable stackSide = 0
        let mutable stack = ValueNone

        member x.MoveNext() =
            match stack with
            | ValueSome s ->
                if stackSide = 0 then
                    // s is from a
                    if b.MoveNext() then
                        let c = cmp s b.Current
                        if c < 0 then
                            current <- s
                            stack <- ValueSome b.Current
                            stackSide <- 1
                            true
                        else
                            current <- b.Current
                            true
                    else
                        current <- s
                        stack <- ValueNone
                        true
                else
                    // s is from r
                    if a.MoveNext() then
                        let c = cmp s a.Current
                        if c < 0 then
                            current <- s
                            stack <- ValueSome a.Current
                            stackSide <- 0
                            true
                        else
                            current <- a.Current
                            true
                    else
                        current <- s
                        stack <- ValueNone
                        true
            | ValueNone -> 
                if a.MoveNext() then
                    if b.MoveNext() then
                        let c = cmp a.Current b.Current
                        if c < 0 then
                            current <- a.Current
                            stack <- ValueSome b.Current
                            stackSide <- 1
                            true
                        else
                            current <- b.Current
                            stack <- ValueSome a.Current
                            stackSide <- 0
                            true
                    
                    else
                        current <- a.Current
                        true
                else
                    if b.MoveNext() then
                        current <- b.Current
                        true
                    else
                        false
                    
        member x.Current = current
        
        member x.Reset() =
            a.Reset()
            b.Reset()
            stack <- ValueNone
            stackSide <- 0
            current <- Unchecked.defaultof<_>

        member x.Dispose() =
            a.Dispose()
            b.Dispose()
            stack <- ValueNone
            stackSide <- 0
            current <- Unchecked.defaultof<_>
            
        interface System.Collections.IEnumerator with
            member x.MoveNext() = x.MoveNext()
            member x.Current = x.Current :> obj
            member x.Reset() = x.Reset()

        interface System.Collections.Generic.IEnumerator<KeyValuePair<int, HalfRangeKind>> with
            member x.Dispose() = x.Dispose()
            member x.Current = x.Current

                


[<StructuredFormatDisplay("{AsString}"); Struct; CustomEquality; CustomComparison>]
type RangeSet private(store : MapExt<int, HalfRangeKind>) =
    static let empty = RangeSet(MapExt.empty)

    static member Empty = empty

    static member OfSeq(s : seq<Range1i>) =
        let arr = s |> Seq.toArray
        if arr.Length = 0 then
            empty
        elif arr.Length = 1 then
            let r = arr.[0]
            RangeSet(MapExt.ofList [r.Min, HalfRangeKind.Left; r.Max + 1, HalfRangeKind.Right ])
        else
            // TODO: better impl possible (sort array and traverse)
            arr |> Array.fold (fun s r -> s.Add r) empty
            
    member private x.Store = store

    member x.Add(r : Range1i) =

        if r.Max < r.Min then 
            x 
        else
            let min = r.Min
            let max = r.Max + 1

            let lm, _, inner = MapExt.split min store
            let inner, _, rm = MapExt.split max inner

            let before = MapExt.tryMax lm |> Option.map (fun mk -> mk, lm.[mk])
            let after = MapExt.tryMin rm |> Option.map (fun mk -> mk, rm.[mk])

            let newStore = 
                match before, after with
                    | None, None ->
                        MapExt.ofList [ min, HalfRangeKind.Left; max, HalfRangeKind.Right]

                    | Some(bk, HalfRangeKind.Right), None ->
                        lm 
                        |> MapExt.add min HalfRangeKind.Left
                        |> MapExt.add max HalfRangeKind.Right

                    | Some(bk, HalfRangeKind.Left), None ->
                        lm 
                        |> MapExt.add max HalfRangeKind.Right

                    | None, Some(ak, HalfRangeKind.Left) ->
                        rm
                        |> MapExt.add min HalfRangeKind.Left
                        |> MapExt.add max HalfRangeKind.Right

                    | None, Some(ak, HalfRangeKind.Right) ->
                        rm
                        |> MapExt.add min HalfRangeKind.Left

                    | Some(bk, HalfRangeKind.Right), Some(ak, HalfRangeKind.Left) ->
                        let self = MapExt.ofList [ min, HalfRangeKind.Left; max, HalfRangeKind.Right]
                        MapExt.union (MapExt.union lm self) rm
                        
                    | Some(bk, HalfRangeKind.Left), Some(ak, HalfRangeKind.Left) ->
                        let self = MapExt.ofList [ max, HalfRangeKind.Right]
                        MapExt.union (MapExt.union lm self) rm

                    | Some(bk, HalfRangeKind.Right), Some(ak, HalfRangeKind.Right) ->
                        let self = MapExt.ofList [ min, HalfRangeKind.Left ]
                        MapExt.union (MapExt.union lm self) rm

                    | Some(bk, HalfRangeKind.Left), Some(ak, HalfRangeKind.Right) ->
                        MapExt.union lm rm

                    //| _ ->
                    //    failwithf "impossible"
            assert (newStore.Count % 2 = 0)
            RangeSet(newStore)

    member x.Remove(r : Range1i) =
        if r.Max < r.Min then
            x
        else
            let min = r.Min
            let max = r.Max + 1

            let lm, _, inner = MapExt.split min store
            let inner, _, rm = MapExt.split max inner

            let before = MapExt.tryMax lm |> Option.map (fun mk -> mk, lm.[mk])
            let after = MapExt.tryMin rm |> Option.map (fun mk -> mk, rm.[mk])

            let newStore = 
                match before, after with
                    | None, None ->
                        MapExt.empty

                    | Some(bk, HalfRangeKind.Right), None ->
                        lm

                    | Some(bk, HalfRangeKind.Left), None ->
                        lm 
                        |> MapExt.add min HalfRangeKind.Right

                    | None, Some(ak, HalfRangeKind.Left) ->
                        rm

                    | None, Some(ak, HalfRangeKind.Right) ->
                        rm
                        |> MapExt.add max HalfRangeKind.Left

                    | Some(bk, HalfRangeKind.Right), Some(ak, HalfRangeKind.Left) ->
                        MapExt.union lm rm
                        
                    | Some(bk, HalfRangeKind.Left), Some(ak, HalfRangeKind.Left) ->
                        let self = MapExt.ofList [ min, HalfRangeKind.Right]
                        MapExt.union (MapExt.union lm self) rm

                    | Some(bk, HalfRangeKind.Right), Some(ak, HalfRangeKind.Right) ->
                        let self = MapExt.ofList [ max, HalfRangeKind.Left ]
                        MapExt.union (MapExt.union lm self) rm

                    | Some(bk, HalfRangeKind.Left), Some(ak, HalfRangeKind.Right) ->
                        let self = MapExt.ofList [ min, HalfRangeKind.Right; max, HalfRangeKind.Left]
                        MapExt.union (MapExt.union lm self) rm


            RangeSet(newStore)

    member x.Contains(v : int) =
        let l, s, _ = MapExt.neighbours v store
        match s with
            | Some(_,k) -> 
                k = HalfRangeKind.Left
            | _ ->
                match l with
                    | Some(_,HalfRangeKind.Left) -> true
                    | _ -> false

    member x.Contains(r : Range1i) =
        if r.Max < r.Min then
            false
        else
            let min = r.Min 
            let max = r.Max + 1

            let lm, lv, inner = MapExt.split min store
            let inner, rv, rm = MapExt.split max inner

            if MapExt.isEmpty inner then
                ( 
                    match lv with
                    | Some HalfRangeKind.Left -> true
                    | Some _ -> false
                    | None ->
                        match lm.TryMaxValueV with
                        | ValueSome HalfRangeKind.Left -> true
                        | _ -> false
                ) &&
                (
                    match rv with
                    | Some HalfRangeKind.Right -> true
                    | Some _ -> false
                    | None ->
                        match lm.TryMaxValueV with
                        | ValueSome HalfRangeKind.Right -> true
                        | _ -> false
                )
                
            else
                false

    static member Union(l : RangeSet, r : RangeSet) : RangeSet =
        if l.Count = 0 then 
            r

        elif r.Count = 0 then
            l

        // TODO: better implementation??
        elif l.Count < r.Count then
            let mutable r = r
            for range in l do
                r <- r.Add range

            r
        else
            let mutable l = l
            for range in r do
                l <- l.Add range
            l

    static member Difference(l : RangeSet, r : RangeSet) : RangeSet =
        if l.Count = 0 then 
            l

        elif r.Count = 0 then
            l

        else
            let mutable l = l
            for range in r do
                l <- l.Remove range
            l

    static member Intersection(l : RangeSet, r : RangeSet) =
        use e = new SortedBinaryEnumerator((l.Store :> seq<_>).GetEnumerator(), (r.Store :> seq<_>).GetEnumerator())

        let mutable result = MapExt.empty
        let mutable cnt = 0
        let mutable start = 0
        while e.MoveNext() do   
            let kvp = e.Current
            let k = kvp.Key
            let op = kvp.Value
            let oldCnt = cnt
            match op with
            | HalfRangeKind.Left -> cnt <- cnt + 1
            | HalfRangeKind.Right -> cnt <- cnt - 1

            if oldCnt = 1 && cnt = 2 then
                // start of intersection
                start <- k
            elif oldCnt = 2 && cnt = 1 then
                // end of intersection
                if start < k then
                    result <-
                        result
                        |> MapExt.add start HalfRangeKind.Left
                        |> MapExt.add k HalfRangeKind.Right

        if result.Count % 2 = 1 then printfn "asdasdasd"
        RangeSet(result)


    member x.Count = 
        assert (store.Count &&& 1 = 0)
        store.Count / 2

    member private x.AsString = x.ToString()

    member x.ToArray() =
        let arr = Array.zeroCreate (store.Count / 2)
        let rec write (i : int) (l : list<int * HalfRangeKind>) =
            match l with
                | (lKey, HalfRangeKind.Left) :: (rKey, HalfRangeKind.Right) :: rest ->
                    arr.[i] <- Range1i(lKey, rKey - 1)
                    write (i + 1) rest
                | [] -> ()
                | _ -> failwith "bad RangeSet"

        store |> MapExt.toList |> write 0
        arr

    member x.ToList() =
        let rec run (b : ListBuilder<_>) (l : list<struct(int * HalfRangeKind)>) =
            match l with
            | struct(lKey, HalfRangeKind.Left) :: struct(rKey, HalfRangeKind.Right) :: rest ->
                b.Append (Range1i(lKey, rKey - 1)) 
                run b rest

            | [] -> b.ToList()

            | _ -> failwith "bad RangeSet"


        let b = ListBuilder()
        store |> MapExt.toListV |> run b
             
    member x.ToSeq() =
        x :> seq<_>       

    member x.ToValueSeq() =
        RangeSetValueEnumerable(store) :> seq<_>

    member x.ToValueList() =
        let b = ListBuilder()
        for r in x do
            for v in r.Min .. r.Max do
                b.Append v

        b.ToList()
      
    member x.ToValueArray() =
        let mutable res = Array.zeroCreate 32
        let mutable cnt = 0
        for r in x do
            let s = cnt + int (r.Max - r.Min + 1)
            if s > res.Length then
                System.Array.Resize(&res, Fun.NextPowerOfTwo s)
            for v in r.Min .. r.Max do
                res.[cnt] <- v
                cnt <- cnt + 1

        if res.Length > cnt then System.Array.Resize(&res, cnt)
        res

    member x.TryDequeue() =
        match MapExt.tryRemoveMin store with
        | Some (l, HalfRangeKind.Left, withoutL) ->
            match MapExt.tryRemoveMin withoutL with
            | Some (r, HalfRangeKind.Right, rest) -> 
                let id = l
                let l = l + 1

                let newStore = 
                    if l < r then
                        withoutL
                        |> MapExt.add l HalfRangeKind.Left
                    else    
                        rest
                Some (id, RangeSet newStore)
            | _ ->
                None
        | _ ->
            None

    

    override x.ToString() =
        let rec ranges (l : list<int * HalfRangeKind>) =
            match l with
                | (kMin, vMin) :: (kMax, vMax) :: rest ->
                    sprintf "[%d,%d)" kMin kMax ::
                    ranges rest

                | [(k,v)] ->
                    [ sprintf "ERROR: %d %A" k v ]

                | [] ->
                    []
                
        store |> MapExt.toList |> ranges |> String.concat ", " |> sprintf "ranges [ %s ]"
        
    override x.GetHashCode() =  
        store.GetHashCode()
        
    override x.Equals(o : obj) =  
        match o with
        | :? RangeSet as o -> o.Store = store
        | _ -> false
        
    interface System.IComparable<RangeSet> with
        member x.CompareTo(o : RangeSet) =
            compare store o.Store

    interface System.IComparable with
        member x.CompareTo(o : obj) =
            let o = o :?> RangeSet
            compare store o.Store

    interface System.Collections.IEnumerable with
        member x.GetEnumerator() = new RangeSetEnumerator((store :> seq<_>).GetEnumerator()) :> _
            
    interface System.Collections.Generic.IEnumerable<Range1i> with
        member x.GetEnumerator() = new RangeSetEnumerator((store :> seq<_>).GetEnumerator()) :> _

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module RangeSet =
    let empty = RangeSet.Empty

    let inline ofSeq (s : seq<Range1i>) = RangeSet.OfSeq s
    let inline ofList (s : list<Range1i>) = RangeSet.OfSeq s
    let inline ofArray (s : Range1i[]) = RangeSet.OfSeq s

    let inline add (r : Range1i) (s : RangeSet) = s.Add r
    let inline remove (r : Range1i) (s : RangeSet) = s.Remove r
    let inline contains (v : int) (s : RangeSet) = s.Contains v
    let inline containsRange (v : Range1i) (s : RangeSet) = s.Contains v
    let inline count (s : RangeSet) = s.Count

    let inline tryDequeue (s : RangeSet) = s.TryDequeue()
    let inline union (l : RangeSet) (r : RangeSet) = RangeSet.Union(l,r)
    let unionMany (l : #seq<RangeSet>) =
        use e = l.GetEnumerator()
        if e.MoveNext() then
            let mutable set = e.Current
            while e.MoveNext() do
                set <- union set e.Current
            set
        else
            empty
    
    let inline intersect (l : RangeSet) (r : RangeSet) = RangeSet.Intersection(l,r)
    let intersectMany (l : seq<RangeSet>) = 
        use e = l.GetEnumerator()
        if e.MoveNext() then
            let mutable set = e.Current
            while e.MoveNext() do
                set <- intersect set e.Current
            set
        else
            empty

    let inline difference (l : RangeSet) (r : RangeSet) = RangeSet.Difference(l,r)


    let inline toSeq (s : RangeSet) = s :> seq<_>
    let inline toList (s : RangeSet) = s.ToList()
    let inline toArray (s : RangeSet) = s.ToArray()

    let inline toValueSeq (s : RangeSet) = s.ToValueSeq()
    let inline toValueList (s : RangeSet) = s.ToValueList()
    let inline toValueArray (s : RangeSet) = s.ToValueArray()

