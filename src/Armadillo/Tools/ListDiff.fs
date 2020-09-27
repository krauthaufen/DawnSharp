namespace Armadillo


// TODO: documentation

open System.Collections.Generic
open Aardvark.Base
open FSharp.Data.Adaptive

type UpdateStatus =
    | Nop = 0
    | Success = 1
    | Error = 2


type ListDiffing<'a, 'b> =
    abstract member Create : 'b -> 'a
    abstract member Destroy : 'a -> unit
    abstract member Equals : 'a * 'b -> bool
    abstract member TryUpdate : 'a * 'b -> UpdateStatus

module ListDiffing =
    
    type private IdentityDiffing<'a> private() =
        static let instance = IdentityDiffing<'a>()

        static member Instance = instance :> ListDiffing<'a, 'a>

        interface ListDiffing<'a, 'a> with
            member x.Create a = a
            member x.Destroy _ = ()
            member x.Equals(a, b) = Unchecked.equals a b
            member x.TryUpdate(a, b) = if Unchecked.equals a b then UpdateStatus.Nop else UpdateStatus.Error
            
    type private UsingDiffing<'a, 'b when 'a :> System.IDisposable>(inner : ListDiffing<'a, 'b>) =
        interface ListDiffing<'a, 'b> with
            member x.Create a = inner.Create a
            member x.Destroy a = inner.Destroy a; (a :> System.IDisposable).Dispose()
            member x.Equals(a, b) = inner.Equals(a,b)
            member x.TryUpdate(a, b) = inner.TryUpdate(a,b)

    let simple<'a> = IdentityDiffing<'a>.Instance

    let using<'a, 'b when 'a :> System.IDisposable> (d : ListDiffing<'a, 'b>) = 
        UsingDiffing(d) :> ListDiffing<_,_>

    let custom (equals : 'a -> 'b -> bool) (create : 'b -> 'a) (destroy : 'a -> unit) (tryUpdate : 'a -> 'b -> UpdateStatus)  = 
        let tryUpdate = OptimizedClosures.FSharpFunc<'a, 'b, UpdateStatus>.Adapt tryUpdate
        let equals = OptimizedClosures.FSharpFunc<'a, 'b, bool>.Adapt equals
        { new ListDiffing<'a, 'b> with 
            member x.Create(b) = create b
            member x.TryUpdate(a, b) = tryUpdate.Invoke(a, b)
            member x.Equals(a, b) = equals.Invoke(a, b)
            member x.Destroy(a) = destroy a
        }



[<AutoOpen>]
module AStar =
    [<AutoOpen>]
    module private Implementation = 
        type Operation =
            | Insert = 1
            | Remove = 2
            | Update = 3

        type OperationList private(values : list<uint64>, used : int) =
    
            static member Empty = OperationList([], 0)
          
            member x.Count = used

            member x.Append(op : Operation) =
                if used = 0 then
                    OperationList([uint64 op], 1)
                else
                    let usedInLast = used &&& 31
                    if usedInLast <> 0 then
                        match values with
                        | h :: t ->
                            OperationList(((h <<< 2) ||| (uint64 op)) :: t, used + 1)
                        | _ ->
                            failwith "bad list"
                    else
                        OperationList((uint64 op) :: values, used + 1)

            member x.Iter (action : Operation -> unit) =
                let rec traverse (action : Operation -> unit) (l : list<uint64>) =
                    match l with
                    | [] -> 
                        ()
                    | h :: t ->
                        traverse action t

                        if System.Runtime.Intrinsics.X86.Bmi1.X64.IsSupported then
                            let mutable sb = 62uy
                            for i in 0 .. 31 do
                                let op = System.Runtime.Intrinsics.X86.Bmi1.X64.BitFieldExtract(h, sb, 2uy) |> int |> unbox<Operation>
                                action op
                                sb <- sb - 2uy
                        else
                            let mutable h = h
                            for i in 0 .. 31 do
                                let op = ((h &&& 0xC000000000000000UL) >>> 62) |> int |> unbox<Operation>
                                action op
                                h <- h <<< 2
                    
                match values with
                | h :: t ->
                    let rem = (used &&& 31)
                    if rem = 0 then
                        traverse action values
                    else
                        traverse action t
                        if System.Runtime.Intrinsics.X86.Bmi1.X64.IsSupported then
                            let mutable sb = (byte rem <<< 1) - 2uy
                            for i in 0 .. rem-1 do
                                let op = System.Runtime.Intrinsics.X86.Bmi1.X64.BitFieldExtract(h, sb, 2uy) |> int |> unbox<Operation>
                                action op
                                sb <- sb - 2uy
                        else
                            let mutable h = h
                            let v = 64 - (used <<< 1)
                            h <- h <<< v
                            for i in 0 .. rem - 1 do
                                let op = ((h &&& 0xC000000000000000UL) >>> 62) |> int |> unbox<Operation>
                                action op
                                h <- h <<< 2

                | [] ->
                    ()

            member x.ToList() =
                let b = ListBuilder()
                x.Iter b.Append
                b.ToList()

            member x.AsArray =
                let mutable index = 0
                let arr = Array.zeroCreate used
                x.Iter (fun v ->
                    arr.[index] <- v
                    index <- index + 1
                )
                arr

        type Path<'a, 'b> =
            { 
                length      : float
                position    : V2i
                weight      : float
                path        : OperationList
                left        : list<'a>
                right       : list<'b>
            }

        type PathComparer<'a, 'b> private() =
            static let floatCmp = System.Collections.Generic.Comparer<float>.Default
            static let instance = PathComparer<'a, 'b>()

            member x.Compare(l, r) =
                floatCmp.Compare(l.weight, r.weight)

            interface IComparer<Path<'a, 'b>> with
                member x.Compare(l, r) =
                    floatCmp.Compare(l.weight, r.weight)

            static member Instance = instance


        let stepTup (diffing : ListDiffing<'a, 'b>) (target : V2i) (queue : Heap<V2i, Path<'x * 'a, 'b>>) =
            let best = queue.Dequeue()
                
            match best.left with
            | [] -> 
                // left is empty
                match best.right with
                | [] ->
                    // both are empty -> done
                    ValueSome best
                | _h :: t ->
                    // insert h -> re-enqueue
                    let pos = best.position + V2i(0,1)
                    let len = best.length + 1.0
                    queue.EnqueueOrDecrease(
                        pos,
                        { 
                            position = pos
                            length = len
                            path = best.path.Append Operation.Insert
                            weight = Vec.distance pos target + len
                            left = []
                            right = t
                        }
                    )
                    ValueNone
            | (_, l0) :: l1 -> 
                    
                // left is non-empty
                //let li0 = best.left.MinIndex
                //let (l0, l1) = best.left |> IndexList.tryRemove li0 |> Option.get

                match best.right with
                | [] ->
                    // right is empty -> remove
                    let pos = best.position + V2i(1,0)
                    let len = best.length + 1.0
                    queue.EnqueueOrDecrease(
                        pos,
                        { 
                            position = pos
                            length = len
                            path = best.path.Append Operation.Remove
                            weight = Vec.distance pos target + len
                            left = l1
                            right = []
                        }
                    )
                    ValueNone
                | r0 :: r1 ->
                    if diffing.Equals(l0, r0) then
                        // equal heads
                        let inline cont (path : OperationList) (pos : V2i) (len : float) (l : list<'x * 'a>) (r : list<'b>) =
                            queue.EnqueueOrDecrease(
                                pos,
                                { 
                                    position = pos
                                    length = len
                                    weight = Vec.distance pos target + len
                                    path = path
                                    left = l
                                    right = r
                                }
                            )

                        let rec skipEqualPrefix (path : OperationList) (pos : V2i) (len : float) (l : list<'x * 'a>) (r : list<'b>) =
                            match l with
                            | [] ->
                                cont path pos len l r
                            | (_,l0) :: l1 ->
                                match r with
                                | r0 :: r1 ->
                                    //let li0 = l.MinIndex
                                    //let (l0, l1) = l.TryRemove li0 |> Option.get

                                    if diffing.Equals(l0, r0) then
                                        let pp = path.Append Operation.Update
                                        skipEqualPrefix pp (pos + V2i.II) (len + Constant.Sqrt2) l1 r1
                                    else
                                        cont path pos len l r
                                | [] ->
                                    cont path pos len l r

                        skipEqualPrefix (best.path.Append Operation.Update) (best.position + V2i(1,1)) (best.length + Constant.Sqrt2) l1 r1

                    else
                        do // Remove
                            let len = best.length + 1.0
                            let pos = best.position + V2i(1,0)
                            queue.EnqueueOrDecrease(
                                pos,
                                { 
                                    position = pos
                                    length = len
                                    path = best.path.Append Operation.Remove
                                    weight = Vec.distance pos target + len
                                    left = l1
                                    right = best.right
                                }
                            )

                        do // Add
                            let len = best.length + 1.0
                            let pos = best.position + V2i(0,1)
                            queue.EnqueueOrDecrease(
                                pos,
                                { 
                                    position = pos
                                    length = len
                                    path = best.path.Append Operation.Insert
                                    weight = Vec.distance pos target + len
                                    left = best.left
                                    right = r1
                                }
                            )


                    ValueNone
    
        let astarIndexList (differ : ListDiffing<'a, 'b>) (l : IndexList<'a>) (rCount : int) (r : list<'b>) =
            if l.Count = 0 then
                if rCount = 0 then
                    struct(UpdateStatus.Nop, IndexListDelta.empty)
                else
                    // all new
                    let mutable i = Index.zero
                    let mutable delta = IndexListDelta.empty
                    for ri in r do
                        let v = differ.Create ri
                        let id = Index.after i
                        delta <- IndexListDelta.add id (Set v) delta
                        i <- id

                    struct(UpdateStatus.Error, delta)
            elif rCount = 0 then
                // all removed
                let mutable delta = IndexListDelta.empty
                for (id, v) in IndexList.toSeqIndexed l do  
                    differ.Destroy v
                    delta <- IndexListDelta.add id Remove delta
                struct(UpdateStatus.Error, delta)
            else    
                let target = V2i(l.Count, rCount)

                let l = IndexList.toListIndexed l

                let p0 =
                    {
                        left = l
                        right = r
                        length = 0.0
                        position = V2i.OO
                        path = OperationList.Empty
                        weight = Vec.distance V2i.OO target
                    }

                let paths = Heap<V2i, Path<Index * 'a, 'b>>(PathComparer.Instance)
                paths.EnqueueOrDecrease(p0.position, p0)

                let mutable delta = Unchecked.defaultof<_>
                let mutable fin = false

                let mutable steps = 0
                while not fin && paths.Count > 0 do
                    match stepTup differ target paths with
                    | ValueSome best ->
                        let path = best.path.ToList()
                        let rec run (status : UpdateStatus) (delta : IndexListDelta<'a>) (lastIndex : Index) (ops : list<Operation>) (l : list<Index * 'a>) (r : list<'b>) =
                            match ops with
                            | [] ->
                                struct(status, delta)

                            | Operation.Update :: ops ->
                                match l with
                                | (lid,l0) :: l1 ->
                                    match r with
                                    | r0 :: r1 -> 
                                        let s = differ.TryUpdate(l0, r0)
                                        if s <> UpdateStatus.Error then
                                            run (max status s) delta lid ops l1 r1
                                        else
                                            differ.Destroy l0
                                            run UpdateStatus.Error (IndexListDelta.add lid (Set (differ.Create r0)) delta) lid ops l1 r1
                                    | [] ->
                                        failwith "empty list"
                                | [] ->
                                    failwith "empty list"

                            | Operation.Remove :: Operation.Insert :: ops 
                            | Operation.Insert :: Operation.Remove :: ops ->
                                match l with
                                | (id, l0) :: l1 ->
                                    match r with
                                    | r0 :: r1 ->
                                        let s = differ.TryUpdate(l0, r0)
                                        if s <> UpdateStatus.Error then
                                            run (max status s) delta id ops l1 r1
                                        else
                                            differ.Destroy l0
                                            run UpdateStatus.Error (IndexListDelta.add id (ElementOperation.Set (differ.Create r0)) delta) id ops
                                                l1 r1
                                    | [] ->
                                        failwith "bad"
                                | [] ->
                                    failwith "bad"

                            | Operation.Remove :: ops ->
                                match l with
                                | (id, l0) :: l1 ->
                                    differ.Destroy l0
                                    run UpdateStatus.Error (IndexListDelta.add id ElementOperation.Remove delta) id ops
                                        l1 r
                                | [] ->
                                    failwith "bad"
                                    
                            | Operation.Insert :: ops ->
                                match r with
                                | r0 :: r1 -> 
                                    let id = 
                                        match l with
                                        | [] -> Index.after lastIndex
                                        | (n,_) :: _ -> Index.between lastIndex n
                                    run UpdateStatus.Error (IndexListDelta.add id (ElementOperation.Set (differ.Create r0)) delta) id ops
                                        l r1
                                | [] ->
                                    failwith "bad"
                                
                            | op :: _ ->
                                failwithf "bad op: %A" op

                        delta <- run UpdateStatus.Nop IndexListDelta.empty Index.zero path l r
                        fin <- true
                    | ValueNone ->
                        ()
                    steps <- steps + 1
            
                delta

    module IndexList =
        let computeDeltaToList (l : IndexList<'a>) (r : list<'a>) =
            let struct(_, a) = astarIndexList ListDiffing.simple l (List.length r) r
            a

    type IndexList<'a> with
        static member ComputeDelta(l : IndexList<'a>, r : list<'a>) =
            let struct(_, a) = astarIndexList ListDiffing.simple l (List.length r) r
            a

        static member ComputeDelta(l : IndexList<'a>, r : list<'a>, rCount : int) =
            let struct(_, a) = astarIndexList ListDiffing.simple l rCount r
            a

    type ListDiffing<'a, 'b> with
        member x.Update(l : IndexList<'a>, r : list<'b>, rCount : int) =
            astarIndexList x l rCount r

        member x.Update(l : IndexList<'a>, r : list<'b>) =
            astarIndexList x l (List.length r) r


