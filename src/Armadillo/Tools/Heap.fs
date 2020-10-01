namespace Armadillo

open System
open Aardvark.Base
open System.Collections.Generic

[<AllowNullLiteral>]
type private HeapEntry<'k, 'v> =
    val mutable public Key : 'k
    val mutable public Value : 'v
    val mutable public Index : int

    new(k,v) = { Key = k; Value = v; Index = -1 }

/// Represents a min-heap of values that are uniquely identified by their key and
/// allows for removal/update of enqueued values.
type Heap<'k, 'v when 'k : equality>(cmp : IComparer<'v>, capacity : int) =
    let heap = List<HeapEntry<'k, 'v>>(capacity)
    let dict = Dict<'k, HeapEntry<'k, 'v>>(capacity)

    let rec bubbleUp (i : int) (e : HeapEntry<'k, 'v>) =
        if i > 0 then
            let p = (i-1) / 2
            let pe = heap.[p]
            let c = cmp.Compare(pe.Value, e.Value)
            if c > 0 then   
                pe.Index <- i
                heap.[i] <- pe
                bubbleUp p e
            else
                e.Index <- i
                heap.[i] <- e
        else
            e.Index <- 0
            heap.[0] <- e

    let rec pushDown (i : int) (e : HeapEntry<'k, 'v>) =
        let i0 = 2 * i + 1
        let i1 = i0 + 1

        if i1 < heap.Count then    
            let e0 = heap.[i0]
            let e1 = heap.[i1]

            let c0 = cmp.Compare(e.Value, e0.Value)
            let c1 = cmp.Compare(e.Value, e1.Value)

            if c0 > 0 && c1 > 0 then
                let c01 = cmp.Compare(e0.Value, e1.Value)
                if c01 < 0 then
                    e0.Index <- i
                    heap.[i] <- e0
                    pushDown i0 e
                else
                    e1.Index <- i
                    heap.[i] <- e1
                    pushDown i1 e
            elif c0 > 0 then
                e0.Index <- i
                heap.[i] <- e0
                pushDown i0 e
            elif c1 > 0 then
                e1.Index <- i
                heap.[i] <- e1
                pushDown i1 e
            else
                e.Index <- i
                heap.[i] <- e
                    
        elif i0 < heap.Count then
            let e0 = heap.[i0]
            let c0 = cmp.Compare(e.Value, e0.Value)
            if c0 > 0 then
                e0.Index <- i
                heap.[i] <- e0
                e.Index <- i0
                heap.[i0] <- e
            else
                e.Index <- i
                heap.[i] <- e
        else
            e.Index <- i 
            heap.[i] <- e
 
    /// Total number of entries currently in the Heap.
    member x.Count = heap.Count

    /// Enqueues (or updates) an entry. 
    /// * if decrease is true the heap will be updated whenever the new value is smaller than the old one.
    /// * if increase is true the heap will be updated whenever the new value is greater than the old one.
    member x.Enqueue(key : 'k, value : 'v, decrease : bool, increase : bool) =
        let isNew = ref false
        let e = 
            dict.GetOrCreate(key, fun key ->
                let idx = heap.Count
                let e = HeapEntry(key, value)
                heap.Add(e)
                bubbleUp idx e
                isNew := true
                e
            )
        if not !isNew then
            let c = cmp.Compare(value, e.Value)
            if decrease && c < 0 then
                e.Value <- value
                bubbleUp e.Index e
            elif increase && c > 0 then 
                e.Value <- value
                pushDown e.Index e
                
    /// Enqueues (or decreases) an entry. 
    member x.EnqueueOrDecrease(key : 'k, value : 'v) = x.Enqueue(key, value, true, false)

    /// Enqueues (or increases) an entry. 
    member x.EnqueueOrIncrease(key : 'k, value : 'v) = x.Enqueue(key, value, false, true)

    /// Enqueues (or updates) an entry. 
    member x.EnqueueOrUpdate(key : 'k, value : 'v) = x.Enqueue(key, value, true, true)
    
    /// Dequeues the smallest element.
    member x.Dequeue() =
        if heap.Count = 0 then raise <| IndexOutOfRangeException "heap empty"

        let last = heap.Count - 1
        let res = heap.[0]
        let le = heap.[last]
        heap.RemoveAt last
        dict.Remove res.Key |> ignore
        if last > 0 then
            heap.[0] <- le
            le.Index <- 0
            pushDown 0 le

        res.Value

    /// Dequeues the smallest element.
    member x.DequeueKey() =
        if heap.Count = 0 then raise <| IndexOutOfRangeException "heap empty"

        let last = heap.Count - 1
        let res = heap.[0]
        let le = heap.[last]
        heap.RemoveAt last
        dict.Remove res.Key |> ignore
        if last > 0 then
            heap.[0] <- le
            le.Index <- 0
            pushDown 0 le

        res.Key

    /// Removes the element associated with key from the heap and returns true when it was successfully removed.
    member x.Remove(key : 'k) =
        match dict.TryRemove key with
        | (true, e) ->
            if heap.Count = 1 then
                heap.Clear()
            else
                let last = heap.Count - 1
                let l = heap.[last]
                heap.RemoveAt last

                if e.Index <> last then
                    l.Index <- e.Index
                    heap.[e.Index] <- l

                    let c = cmp.Compare(l.Value, e.Value)
                    if c > 0 then pushDown l.Index l
                    elif c < 0 then bubbleUp l.Index l

            true
        | _ ->
            false
            
    /// Removes the element associated with key from the heap and returns its (optional) value when it was successfully removed.
    member x.TryRemove(key : 'k) =
        match dict.TryRemove key with
        | (true, e) ->
            if heap.Count = 1 then
                heap.Clear()
            else
                let last = heap.Count - 1
                let l = heap.[last]
                heap.RemoveAt last

                l.Index <- e.Index
                heap.[e.Index] <- l

                pushDown l.Index l

            Some e.Value
        | _ ->
            None

    new(cmp : IComparer<'v>) = Heap<'k, 'v>(cmp, 32)
    new(capacity : int) = Heap<'k, 'v>(Comparer<'v>.Default, capacity)
    new() = Heap<'k, 'v>(Comparer<'v>.Default, 32)


    