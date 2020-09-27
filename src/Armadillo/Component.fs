namespace Armadillo


open Aardvark.Base
open FSharp.Data.Adaptive
open System.Runtime.CompilerServices
open System
open System.Threading

type DirtyState =
    | UpToDate = 0
    | Dirty = 1
    | ForceUpdate = 2

type Environment =
    abstract member MarkDirty : DirtyState -> unit

[<AbstractClass>]
type Component(e : Environment) =
    member x.Environment = e
    abstract member Mount : unit -> unit
    abstract member Unmount : unit -> unit
    abstract member ReceivedValue : unit -> unit
        
    default x.Mount() = ()
    default x.Unmount() = ()
    default x.ReceivedValue() = ()

type [<AbstractClass>] Component<'s>(e : Environment) =
    inherit Component(e)
    abstract member Update : 's -> struct('s * list<Node<'s>>)

and [<AbstractClass>] Node<'s>() =
    abstract member CreateComponent : Environment -> Component<'s>
    abstract member TryUpdate : Component<'s> -> voption<bool>

[<AbstractClass>]
type Component<'s, 'a> private(e : Environment, state : ValueOption<'a>) as this =
    inherit Component<'s>(e)
    let mutable state = state
    let selfType = this.GetType()


    member x.SelfType = selfType

    member x.State
        with get() =
            match state with
            | ValueSome s -> s
            | ValueNone ->
                failwith "[Component] State being accessed before initialized"
        and internal set v = 
            state <- ValueSome v
            
    member x.SetState(value : 'a, forceUpdate : bool) =
        match state with
            | ValueSome s when not forceUpdate && not (x.ShouldUpdateInternal(value, s))  ->
                // no change
                false
            | _ ->
                state <- ValueSome value
                x.ReceivedValue()
                e.MarkDirty (if forceUpdate then DirtyState.ForceUpdate else DirtyState.Dirty)
                true

    member x.SetState(value : 'a) =
        x.SetState(value, false)

    member x.ForceUpdate() =
        e.MarkDirty DirtyState.ForceUpdate
 
    abstract member ShouldUpdate : 'a * 'a -> bool
    default x.ShouldUpdate(_,_) = true

    member internal x.ShouldUpdateInternal(a : 'a, b : 'a) =
        if ShallowEqualityComparer<'a>.ShallowEquals(a,b) then  
            false
        else
            x.ShouldUpdate(a, b)


    new(e : Environment) = Component<'s, 'a>(e, ValueNone)
    new(e : Environment, v : 'a) = Component<'s, 'a>(e, ValueSome v)

type ComponentConstructor<'s, 'a> =
    {
        ComponentType   : Type
        ComponentName   : string
        Creator         : Environment -> 'a -> Component<'s, 'a> 
    }

module ComponentConstructor =

    let private cache = System.Collections.Concurrent.ConcurrentDictionary<struct(Type * Type * Type), obj>()

    let getConstructor<'c, 's, 'a when 'c :> Component<'s, 'a>> (creator : Environment * 'a -> 'c) =
        cache.GetOrAdd(struct(typeof<'c>, typeof<'s>, typeof<'a>), fun struct(compType, _, _) ->
            let name =
                let n = compType.Name
                if n.EndsWith "Component" then n.Substring(0, n.Length - 9)
                else n

            {
                ComponentType = compType
                ComponentName = name
                Creator = fun t v -> creator(t, v) :> Component<'s, 'a>
            } :> obj
        ) |> unbox<ComponentConstructor<'s, 'a>>
        
type Node<'s, 'a>(ctor : ComponentConstructor<'s, 'a>, value : 'a) =
    inherit Node<'s>()

    member x.Constructor = ctor
    member x.Value = value

    override x.GetHashCode() =
        HashCode.Combine(Unchecked.hash ctor, ShallowEqualityComparer.ShallowHashCode value)

    override x.Equals o =
        match o with
        | :? Node<'s, 'a> as o ->
            Unchecked.equals ctor o.Constructor && ShallowEqualityComparer.ShallowEquals(o.Value, value)
        | _ ->
            false

    override x.ToString() =
        sprintf "%s(%A)" ctor.ComponentName value

    override x.CreateComponent(env) =
        let c = ctor.Creator env value :> Component<'s>
        c.ReceivedValue()
        c

    override x.TryUpdate(comp) =
        match comp with
        | :? Component<'s, 'a> as comp when comp.SelfType = ctor.ComponentType ->
            ValueSome (comp.SetState value)
        | _ ->
            ValueNone

module Node = 
    let inline create<'c, 's, 'a when 'c :> Component<'s, 'a>> (action : Environment * 'a -> 'c) =
        let ctor = ComponentConstructor.getConstructor action
        fun value -> Node<'s, 'a>(ctor, value) :> Node<_>

[<AbstractClass>]
type ReconcilerNode() =
    abstract member Level : int
    abstract member Run : unit -> unit

type Reconciler() =
    let queue = Heap<ReconcilerNode, int>()

    member x.RunUntilEmpty() =
        while queue.Count > 0 do
            let e = queue.DequeueKey()
            e.Run()

    member x.Add(r : ReconcilerNode) =
        queue.Enqueue(r, r.Level, false, true)
    
    member x.Remove(r : ReconcilerNode) =
        queue.Remove(r) |> ignore


type ReconcilerNode<'s>(env : Reconciler, level : int, node : Node<'s>, traversalState : 's) as this =
    inherit ReconcilerNode()
    let mutable comp = node.CreateComponent this
    let mutable node = node
    let mutable traversalState = traversalState
    let mutable children = IndexList.empty<ReconcilerNode<'s>>
    let mutable dirty = DirtyState.Dirty
    let mutable mounted = false

    let diff (s : 's) =
        ListDiffing.custom
            (fun (r : ReconcilerNode<'s>) (value : Node<'s>) -> r.Node = value)
            (fun (value : Node<'s>) -> ReconcilerNode<'s>(env, level + 1, value, s))
            (fun (r : ReconcilerNode<'s>) -> r.Destroy())
            (fun (r : ReconcilerNode<'s>) (value : Node<'s>) ->
                r.Node <- value
                r.TraversalState <- s
                UpdateStatus.Success
            )
                
    do env.Add this

    interface Environment with
        member x.MarkDirty(s) = 
            if s <> DirtyState.UpToDate then
                dirty <- s
                env.Add x

    override x.Level = level

    member x.Destroy() =
        env.Remove x
        for c in children do c.Destroy()
        if mounted then comp.Unmount()

        children <- IndexList.empty
        comp <- Unchecked.defaultof<_>
        node <- Unchecked.defaultof<_>
        dirty <- DirtyState.UpToDate
        mounted <- false

    member x.Component = 
        comp

    member x.TraversalState
        with get() = traversalState
        and set v = 
            if not (Unchecked.equals traversalState v) then
                traversalState <- v
                dirty <- DirtyState.ForceUpdate
                env.Add x

    member x.Node
        with get() = node
        and set v =  
            if node <> v then
                node <- v
                dirty <- DirtyState.Dirty
                env.Add x

    override x.Run() =
        if dirty <> DirtyState.UpToDate then
            let tryUpdate = mounted && dirty = DirtyState.Dirty
            dirty <- DirtyState.UpToDate

            let mutable changed = true
            if tryUpdate then
                match node.TryUpdate comp with
                | ValueNone ->
                    // unable to update existing component

                    // destroy all children (TODO: necessary???)
                    for c in children do c.Destroy()
                    children <- IndexList.empty

                    // unmount the old component
                    if mounted then
                        mounted <- false
                        comp.Unmount()

                    // create a new component
                    comp <- node.CreateComponent x
                | ValueSome c ->
                    changed <- c

            if changed then
                // mount if necessary
                if not mounted then
                    mounted <- true
                    comp.Mount()

                // render
                let struct (newState, newChildren) = comp.Update traversalState

                // update children
                let struct(_, ops) = (diff newState).Update(children, newChildren)
                children <- IndexList.applyDelta children ops |> fst

[<AutoOpen>]
module ReconcilerExtensions =
    type Reconciler with
        member x.Update(r : ReconcilerNode<'s>, value : Node<'s>) =
            r.Node <- value
            x.RunUntilEmpty()


module ComponentTest =
    type TraversalState =
        {
            stack : list<int>
        }       

    type Node = Node<TraversalState>

    type ListComponent(e : Environment, children : list<Node>) =
        inherit Component<TraversalState, list<Node>>(e, children)


        override x.Mount() =
            Log.line "mount list"
            
        override x.Unmount() =
            Log.line "unmount List"

        override x.ReceivedValue() =
            Log.line "list received %A" x.State

        override x.Update(state) =
            Log.line "update list %A" x.State
            //{ state with stack = 1 :: state.stack }, x.State
            struct(state, x.State)

    type ApplicatorComponent(e : Environment, value : (int * Node)) =
        inherit Component<TraversalState, int * Node>(e, value)


        override x.Mount() =
            Log.line "mount applicator"
            
        override x.Unmount() =
            Log.line "unmount applicator"

        override x.ReceivedValue() =
            Log.line "applicator received %A" x.State

        override x.Update(state) =
            let (value, child) = x.State
            Log.line "update applicator %A %A" value child
            struct({ state with stack = (value % 2) :: state.stack }, [child])
            //state, [child]
    type LeafComponent(e : Environment, info : int) =
        inherit Component<TraversalState, int>(e, info)


        override x.Mount() =
            Log.line "mount leaf"
            
        override x.Unmount() =
            Log.line "unmount leaf"

        //override x.ShouldUpdate(o, n) =
        //    o % 2 <> n % 2

        override x.ReceivedValue() =
            Log.line "leaf received %d" x.State

        override x.Update(s) =
            Log.line "update leaf %d (%A)" x.State s
            struct(s, [])

    type AdaptiveLeafComponent(e : Environment, info : aval<int>) =
        inherit Component<TraversalState, aval<int>>(e, info)
        let mutable s = { new IDisposable with member x.Dispose() = () }

        override x.Mount() =
            s <- info.AddMarkingCallback (fun () -> 
                Log.line "mark leaf"
                x.ForceUpdate()
            )
            Log.line "mount aleaf"
            
        override x.Unmount() =
            s.Dispose()
            s <- { new IDisposable with member x.Dispose() = () }
            Log.line "unmount aleaf"

        //override x.ShouldUpdate(o, n) =
        //    o % 2 <> n % 2

        override x.ReceivedValue() =
            Log.line "aleaf received %A" x.State

        override x.Update(s) =
            Log.line "update aleaf %d (%A)" (AVal.force x.State) s
            struct(s, [])

    module Sg = 
        let list = Node.create ListComponent
        let leaf = Node.create LeafComponent
        let apply = Node.create ApplicatorComponent |> curry
        let aleaf = Node.create AdaptiveLeafComponent


    let run() =
        
        let c = cval 10
        Log.start "run 1"
        let graph =
            Sg.list [
                Sg.leaf 1
                Sg.aleaf c
            ]
            |> Sg.apply 5

        let runner = Reconciler()
        let r = ReconcilerNode<_>(runner, 0, graph, { stack = [] })
        runner.Update(r, graph)
        Log.stop()

        Log.start "run 2"
        let graph =
            Sg.list [
                Sg.leaf 1
                Sg.aleaf c
            ]
            |> Sg.apply 4
        runner.Update(r, graph)
        Log.stop()

        Log.start "run 3"
        let graph =
            Sg.list [
                Sg.leaf 1
                Sg.aleaf c
            ]
            |> Sg.apply 6
        runner.Update(r, graph)
        Log.stop()
            
        Log.start "run 4"
        transact (fun () -> c.Value <- 1000)
        runner.Update(r, graph)
        Log.stop()
            
        Log.start "run 5"
        let graph =
            Sg.list [
                Sg.leaf 1
            ]
            |> Sg.apply 6
        runner.Update(r, graph)
        Log.stop()
            
        Log.start "run 6"
        transact (fun () -> c.Value <- 666)
        runner.Update(r, graph)
        Log.stop()

            
        Log.start "run 7"
        let graph =
            Sg.list [
                Sg.leaf 1
            ]
            |> Sg.apply 5
        runner.Update(r, graph)
        Log.stop()

        ()
