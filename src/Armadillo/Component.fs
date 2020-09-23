namespace Armadillo


open Aardvark.Base
open FSharp.Data.Adaptive
open System.Runtime.CompilerServices
open System
open System.Threading

module rec New =
    
    type Environment =
        abstract member MarkDirty : unit -> unit

    type IComponentConstructor =
        abstract member ComponentName : string

    [<AbstractClass>]
    type Component(e : Environment) =
        member x.Environment = e

        abstract member Mount : unit -> unit
        abstract member Unmount : unit -> unit
        abstract member Render : unit -> list<Node>
        abstract member ReceivedValue : unit -> unit

        default x.Mount() = ()
        default x.Unmount() = ()
        default x.ReceivedValue() = ()


    [<AbstractClass>]
    type Component<'a> private(e : Environment, ctor : ComponentConstructor<'a>, state : ValueOption<'a>) =
        inherit Component(e)
        let mutable state = state

        member x.Constructor = ctor

        member x.State
            with get() =
                match state with
                | ValueSome s -> s
                | ValueNone ->
                    failwith "[Component] State being accessed before initialized"
            and internal set v = 
                state <- ValueSome v
            
        member x.SetState(value : 'a, forceRender : bool) =
            match state with
                | ValueSome s when not forceRender && not (x.ShouldUpdateInternal(value, s))  ->
                    // no change
                    false
                | _ ->
                    state <- ValueSome value
                    x.ReceivedValue()
                    e.MarkDirty()
                    true

        member x.SetState(value : 'a) =
            x.SetState(value, false)

        member x.ForceRender() =
            e.MarkDirty()
            


        abstract member ShouldUpdate : 'a * 'a -> bool
        default x.ShouldUpdate(_,_) = true

        member x.ShouldUpdateInternal(a : 'a, b : 'a) =
            if ShallowEqualityComparer<'a>.ShallowEquals(a,b) then  
                false
            else
                x.ShouldUpdate(a, b)


        new(e : Environment, ctor : ComponentConstructor<'a>) = Component<'a>(e, ctor, ValueNone)
        new(e : Environment, ctor : ComponentConstructor<'a>, v : 'a) = Component<'a>(e, ctor, ValueSome v)

    type ComponentConstructor<'a> =
        {
            ComponentName   : string
            Creator         : Environment -> 'a -> Component<'a> 
        }

    [<AbstractClass>]
    type Node() =
        abstract member CreateComponent : Environment -> Component
        abstract member TryUpdate : Component -> voption<bool>

    type Node<'a>(ctor : ComponentConstructor<'a>, value : 'a) =
        inherit Node()

        member x.Constructor = ctor
        member x.Value = value

        override x.GetHashCode() =
            HashCode.Combine(Unchecked.hash ctor, ShallowEqualityComparer.ShallowHashCode value)

        override x.Equals o =
            match o with
            | :? Node<'a> as o ->
                Unchecked.equals ctor o.Constructor && ShallowEqualityComparer.ShallowEquals(o.Value, value)
            | _ ->
                false

        override x.ToString() =
            sprintf "%s(%A)" ctor.ComponentName value

        override x.CreateComponent(env) =
            let c = ctor.Creator env value :> Component
            c.ReceivedValue()
            c

        override x.TryUpdate(comp) =
            match comp with
            | :? Component<'a> as comp when Unchecked.equals comp.Constructor ctor ->
                ValueSome (comp.SetState value)
            | _ ->
                ValueNone


    type Runner() =
        let queue = Heap<Reconciler, int>()

        member x.RunUntilEmpty() =
            while queue.Count > 0 do
                let e = queue.DequeueKey()
                e.Run()

        member x.Add(r : Reconciler) =
            queue.Enqueue(r, r.Level, false, true)
    
        member x.Remove(r : Reconciler) =
            queue.Remove(r) |> ignore

        member x.Update(r : Reconciler, value : Node) =
            r.Node <- value
            x.RunUntilEmpty()

    type DirtyState =
        | UpToDate = 0
        | Dirty = 1
        | ForceRender = 2

    type Reconciler(env : Runner, level : int, node : Node) as this =
        let mutable comp = node.CreateComponent this
        let mutable node = node
        let mutable children = IndexList.empty<Reconciler>
        let mutable dirty = DirtyState.Dirty
        let mutable mounted = false

        let diff =
            ListDiffing.custom
                (fun (r : Reconciler) (value : Node) -> r.Node = value)
                (fun (value : Node) -> Reconciler(env, level + 1, value))
                (fun (r : Reconciler) -> r.Destroy())
                (fun (r : Reconciler) (value : Node) ->
                    r.Node <- value
                    UpdateStatus.Success
                )
                
        do env.Add this

        interface Environment with
            member x.MarkDirty() = 
                dirty <- DirtyState.ForceRender
                env.Add x

        member x.Level = level

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

        member x.Node
            with get() = node
            and set v =  
                if node <> v then
                    node <- v
                    dirty <- DirtyState.Dirty
                    env.Add x

        member x.Run() =
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
                    let newChildren = comp.Render()

                    // update children
                    let struct(_, ops) = diff.Update(children, newChildren)
                    children <- IndexList.applyDelta children ops |> fst



    module Test =
        
        type ListComponent(e : Environment, children : list<Node>) =
            inherit Component<list<Node>>(e, ListComponent.Constructor, children)

            static let constructor =
                {
                    ComponentName = "List"
                    Creator = fun e i -> ListComponent(e, i) :> _
                }

            static member Constructor = constructor


            override x.Mount() =
                Log.line "mount list"
            
            override x.Unmount() =
                Log.line "unmount List"

            override x.ReceivedValue() =
                Log.line "list received %A" x.State

            override x.Render() =
                Log.line "render list %A" x.State
                x.State

        type LeafComponent(e : Environment, info : int) =
            inherit Component<int>(e, LeafComponent.Constructor, info)

            static let constructor =
                {
                    ComponentName = "Leaf"
                    Creator = fun e i -> LeafComponent(e, i) :> _
                }

            static member Constructor = constructor


            override x.Mount() =
                Log.line "mount leaf"
            
            override x.Unmount() =
                Log.line "unmount leaf"

            //override x.ShouldUpdate(o, n) =
            //    o % 2 <> n % 2

            override x.ReceivedValue() =
                Log.line "leaf received %d %d" x.State (x.State % 2)

            override x.Render() =
                Log.line "render leaf %d %d" x.State (x.State % 2)
                []

        type AdaptiveLeafComponent(e : Environment, info : aval<int>) =
            inherit Component<aval<int>>(e, AdaptiveLeafComponent.Constructor, info)
            let mutable s = { new IDisposable with member x.Dispose() = () }

            static let constructor =
                {
                    ComponentName = "AdaptiveLeaf"
                    Creator = fun e i -> AdaptiveLeafComponent(e, i) :> _
                }

            static member Constructor = constructor



            override x.Mount() =
                s <- info.AddMarkingCallback (fun () -> 
                    Log.line "mark leaf"
                    x.ForceRender()
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

            override x.Render() =
                Log.line "render aleaf %d" (AVal.force x.State)
                []


        let list a = Node<_>(ListComponent.Constructor, a) :> Node
        let leaf a = Node<_>(LeafComponent.Constructor, a) :> Node
        let aleaf a = Node<_>(AdaptiveLeafComponent.Constructor, a) :> Node


        let run() =
            Log.start "run 1"
            let graph =
                list [
                    leaf 1
                    leaf 2
                    leaf 5
                ]

            let runner = Runner()
            let r = Reconciler(runner, 0, graph)
            runner.Update(r, graph)
            Log.stop()

            Log.start "run 2"
            let graph =
                list [
                    leaf 1
                    leaf 2
                    leaf 3
                ]
            runner.Update(r, graph)
            Log.stop()


            
            Log.start "run 3"
            let graph =
                list []
                
            runner.Update(r, graph)
            Log.stop()

            let c = cval 10

            
            Log.start "run 4"
            let graph =
                list [
                    aleaf c
                ]
            runner.Update(r, graph)
            Log.stop()

            Log.start "run 5"
            transact(fun () -> c.Value <- 1000)
            runner.RunUntilEmpty()
            Log.stop()



            ()








type IRunnerJob =
    abstract member Level : int
    abstract member Run : unit -> unit

type IRunner =
    abstract member Level : int
    abstract member Dirty : unit -> unit


[<AbstractClass>]
type Component<'s>() =
    let mutable runner : voption<IRunner> = ValueNone

    member internal x.Runner
        with get() = runner
        and set v = runner <- v

    member x.MarkDirty() =  
        match runner with
        | ValueSome r ->
            r.Dirty()
        | ValueNone ->
            Log.warn "Component marked without active runner"

    abstract member Mount : unit -> unit
    abstract member Unmount : unit -> unit
    abstract member Render : 's -> struct('s * list<Node<'s>>)
    
    default x.Mount() = ()
    default x.Unmount() = ()

and [<AbstractClass>] Component<'s, 'a>(props : 'a) =
    inherit Component<'s>()
    abstract member Update : 'a -> UpdateStatus
  
and [<AbstractClass>] Node<'s>() =
    abstract member Create : unit -> Component<'s>
    abstract member Update : Component<'s> -> UpdateStatus

and private Node<'s, 'a>(create : 'a -> Component<'s, 'a>, value : 'a) =
    inherit Node<'s>()

    member x.Creator = create
    member x.Value = value

    override x.GetHashCode() =
        HashCode.Combine(Unchecked.hash create, Unchecked.hash value)

    override x.Equals(o : obj) =
        match o with
        | :? Node<'s, 'a> as o -> Unchecked.equals create o.Creator && Unchecked.equals value o.Value
        | _ -> false

    override x.Create() = create value :> Component<'s>
    override x.Update (c : Component<'s>) =
        match c with
        | :? Component<'s, 'a> as c -> c.Update(value)
        | _ -> UpdateStatus.Error     

module Node =
    let create (create : 'a -> Component<'s, 'a>) (value : 'a) =
        Node<'s, 'a>(create, value) :> Node<'s>

type RunnerState() =
    let mutable delete = System.Collections.Generic.HashSet<System.IDisposable>()
    let dirty = Heap<IRunnerJob, int>(System.Collections.Generic.Comparer<int>.Default, 16)

    member x.UpdateDirty() =
        lock dirty (fun () ->
            while dirty.Count = 0 do
                Monitor.Wait dirty |> ignore

            while dirty.Count > 0 do
                let r = dirty.DequeueKey()
                r.Run()
        )
        
    member x.Enqueue(job : IRunnerJob) =
        lock dirty (fun () ->
            dirty.Enqueue(job, job.Level, false, false)
            Monitor.PulseAll dirty
        )
    member x.Remove(job : IRunnerJob) =
        lock dirty (fun () ->
            dirty.Remove job |> ignore
            Monitor.PulseAll dirty
        )

    member x.Delete(value : 'a) = 
        delete.Add (value :> System.IDisposable) |> ignore

    member x.Cleanup() =
        let o = delete
        delete <- System.Collections.Generic.HashSet<System.IDisposable>()
        for e in o do e.Dispose()

type Runner<'s>(state : RunnerState, value : Node<'s>) as this =

    [<ThreadStatic; DefaultValue>]
    static val mutable private _CurrentState : 's

    [<ThreadStatic; DefaultValue>]
    static val mutable private _CurrentLevel : int
    
    static member internal CurrentState
        with get() = Runner<'s>._CurrentState
        and set v = Runner<'s>._CurrentState <- v

    static member internal CurrentLevel
        with get() = Runner<'s>._CurrentLevel
        and set v = Runner<'s>._CurrentLevel <- v

    let mutable parent : voption<Runner<'s>> = ValueNone
    let mutable latestState = Runner<'s>.CurrentState //Unchecked.defaultof<'s>
    let mutable latest = value
    let mutable cache = None
    let mutable childRunners : IndexList<Runner<'s>> = IndexList.empty
    let level = Runner<'s>.CurrentLevel

    let job =
        { new IRunnerJob with
            member _.Level = level
            member _.Run() =
                this.Run(latestState)
        }

    let value = ()

    let runnerDiffing =
        ListDiffing.custom
            (fun (r : Runner<'s>) (value : Node<'s>) -> r.Value = value)
            (fun (value : Node<'s>) -> Runner<'s>(state, value))
            (fun (r : Runner<'s>) -> r.Destroy())
            (fun (r : Runner<'s>) (value : Node<'s>) ->
                r.Value <- value
                //state.Enqueue job
                UpdateStatus.Success
                //match r.Component with
                //| Some c -> value.Update c
                //| None -> UpdateStatus.Error
            )
        
    member x.Parent
        with get() = parent
        and set v = parent <- v
        
    member x.Value
        with get() = latest
        and set v = latest <- v

    member x.Component : option<Component<'s>> = cache

    member x.Destroy() =
        state.Remove job
        parent <- ValueNone
        for c in childRunners do c.Destroy()
        childRunners <- IndexList.empty

        match cache with
        | Some c ->
            c.Unmount()
            cache <- None
        | None ->
            ()

    member x.Run(state : 's) = 
        let o = Runner<'s>.CurrentLevel
        let os = Runner<'s>.CurrentState
        Runner<'s>.CurrentLevel <- level + 1
        Runner<'s>.CurrentState <- state
        try
            latestState <- state
            let comp =
                match cache with
                | Some c ->
                    let s = latest.Update c
                    match s with
                    | UpdateStatus.Success | UpdateStatus.Nop ->
                        c
                    | _ -> 
                        c.Unmount()
                        let c = latest.Create()
                        c.Runner <- ValueSome (x :> IRunner)
                        c.Mount()
                        cache <- Some c
                        c
                | None ->
                    let c = latest.Create()
                    c.Runner <- ValueSome (x :> IRunner)
                    c.Mount()
                    cache <- Some c
                    c
            let struct(newState, children) = comp.Render(state)
            let struct(_status, ops) = runnerDiffing.Update(childRunners, children)

            childRunners <- IndexList.applyDelta childRunners ops |> fst

            for c in childRunners do
                c.Parent <- ValueSome x
                c.Run(newState)
        finally
            Runner<'s>.CurrentState <- os
            Runner<'s>.CurrentLevel <- o
                


    interface IRunner with
        member x.Level = level
        member x.Dirty() = state.Enqueue job


module Gubble =

    type Handle = nativeint
    type Ref =
        {
            acquire : unit -> unit
            release : unit -> unit
            ptr : ref<Handle>
        }

    type TraversalState =
        {
            stack : list<ref<int>>
            buffers : Map<int, Ref>
        }

    type Sg = Node<TraversalState>

    type RenderComponent(fvc : aval<int>) =
        inherit Component<TraversalState, aval<int>>(fvc)
        let mutable fvc = fvc
        let mutable s = { new IDisposable with member x.Dispose() = () }


        override x.Unmount() =
            Log.line "unmount Render"
            
        override x.Mount() =
            
            Log.line "mount Render"

        override x.Update(nc : aval<int>) =
            //fvc = nc
            if fvc <> nc then
                s.Dispose()
                s <- fvc.AddMarkingCallback (fun () -> Log.line "cb"; x.MarkDirty())
                Log.line "update Render: %A -> %A" (AVal.force fvc) (AVal.force nc)
                fvc <- nc
                UpdateStatus.Success
            else
                AVal.force nc |> ignore
                UpdateStatus.Nop

        override x.Render(state : TraversalState) =
            Log.line "render Render %d (%A)" (AVal.force fvc) state
            struct(state, [])
  

    type GroupComponent(children : list<Node<TraversalState>>) =
        inherit Component<TraversalState, list<Node<TraversalState>>>(children)
        let mutable children = children
        
        override x.Unmount() =
            Log.line "unmount Group"
            
        override x.Mount() =
            Log.line "mount Group"

        override x.Update(nc : list<Sg>) =
            if children <> nc then
                Log.line "update Group: %A" (List.length nc)
                children <- nc
                UpdateStatus.Success
            else
                UpdateStatus.Success

        override x.Render(state : TraversalState) =
            Log.line "render Group"
            let newState = { state with stack = ref 1 :: state.stack }
            struct(newState, children)
   
    [<MethodImpl(MethodImplOptions.NoInlining ||| MethodImplOptions.NoOptimization)>]
    let idd (a : 'a) = a
    let groupComp = idd (fun cs -> GroupComponent(cs) :> Component<_,_>)
    let renderComp = idd (fun cs -> RenderComponent(cs) :> Component<_,_>)

    let group = Node.create groupComp
    let render = Node.create renderComp





    type Applicator( tup ) =
        inherit Component<TraversalState, (int * Sg)>(tup)
        let mutable currentValue = ref (fst tup)
        let mutable currentChild = snd tup

        override x.Unmount() =
            Log.line "unmount Applicator"
            
        override x.Mount() =
            Log.line "mount Applicator"

        override x.Update((value, child) as tup) =
            if child = currentChild then
                Log.line "fress"
                currentValue := value
                UpdateStatus.Nop
            else
                currentValue := value
                currentChild <- child
                UpdateStatus.Success

        override x.Render(state : TraversalState) =
            Log.line "render Applicator %d (%A)" !currentValue currentChild
            let newState = { state with stack = currentValue :: state.stack }
            struct(newState, [currentChild])
  
    let appComp = idd (fun cs -> Applicator(cs) :> Component<_,_>)

    let apply v c : Sg = Node.create appComp (v,c)




    let test() =    
        New.Test.run()
        exit 0

        let s = RunnerState()

        let thread = 
            startThread <| fun () ->
                while true do
                    Log.start "update"
                    s.UpdateDirty()
                    Log.stop()


        let empty = { stack = [ ]; buffers = Map.empty }

        Log.start "run 1"
        let sg =
            group [
                group [
                    render (AVal.constant 1)
                    render (AVal.constant 5)
                ]
            ]

        let r = Runner(s, sg)
        r.Value <- sg
        r.Run(empty)
        Log.stop()

        let c = cval 10
        let sg2 =   
            group [ 
                group [
                    render (AVal.constant 1)
                    render (AVal.constant 5)
                    render c
                ]
            ]
        Log.start "run 2"
        r.Value <- sg2
        r.Run(empty)

        Log.stop()
      
        Console.ReadLine() |> ignore
        Log.line "change"

        transact (fun () ->
            c.Value <- 100
        )
        Console.ReadLine() |> ignore


        //Log.start "run 2"
        //let sg2 =   
        //    group [
        //    ]
        //r.Run(empty, sg2, true)
        //Log.stop()
    


//[<AbstractClass>]
//type Component<'s, 'a>() =
//    inherit Component<'s>()
//    abstract member Render : 's * 'a -> unit

//type ComponentFactory<'s, 'a> =
//    abstract member Name : string
//    abstract member Create : unit -> Component<'s, 'a>

//type NodeVisitor<'r> =
//    abstract member Visit : Node<'s, 'a> -> 'r

//and Node<'s> =
//    abstract member Visit : NodeVisitor<'r> -> 'r
//    //abstract member Update : 's * Component<'s> -> bool
//    //abstract member CreateComponent : unit -> Component<'s>

//and Node<'s, 'a>(creator : ComponentFactory<'s, 'a>, value : 'a) =
//    member x.CreateComponent() = creator.Create()
//    member x.Value = value

//    interface Node<'s> with
//        member x.Visit v =
//            v.Visit x
//        //member x.CreateComponent() = x.CreateComponent() :> _
//        //member x.Update(state, target) =    
//        //    match target with
//        //    | :? Component<'s, 'a> as target ->
//        //        target.Update(state, value)
//        //        true
//        //    | _ ->
//        //        false


//module Gabll = 

//    //let inline factory< ^a when ^a : (static member Create : unit -> ^a) and ^a :> Component and ^a : (static member Name : string) > =
//    //    {
//    //        new ComponentFactory<'s
//    //    }

//    type RenderComponent() =
//        inherit Component<unit, int>()

//        static let factory =
//            {
//                new ComponentFactory<unit, int> with
//                    member x.Name = "Render"
//                    member x.Create() = RenderComponent() :> _
//            }

//        let mutable fvc = 0

//        static member Factory = factory


//        override x.Update((), value) =
//            fvc <- value

//        override x.Render(()) =
//            Log.line "draw %d" fvc
            
//    type GroupComponent() =
//        inherit Component<unit, list<Node<unit>>>()

//        static let factory =
//            {
//                new ComponentFactory<unit, list<Node<unit>>> with
//                    member x.Name = "Group"
//                    member x.Create() = GroupComponent() :> _
//            }

//        let mutable children = []

//        static member Factory = factory

//        override x.Update((), cs) =
//            children <- 
//                cs |> List.map (fun c ->
//                    c.Visit {
//                        new NodeVisitor<_> with
//                            member x.Visit (n : Node<unit, 'a>) =
//                                n.CreateComponent() :> Component<_>
//                    }
//                    //c, c.CreateComponent()
//                )

//        override x.Render(()) =
//            Log.start "group"
//            for (n, c) in children do
//                c.Render(())
//            Log.stop()

//    let render fvc = Node(RenderComponent.Factory, fvc) :> Node<unit>
//    let group cs = Node(GroupComponent.Factory, cs) :> Node<unit>


//    let test =
//        group [
//            render 10
//            render 100
//        ]

