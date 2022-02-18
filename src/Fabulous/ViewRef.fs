namespace Fabulous

open System

type ViewRef(onAttached, onDetached) =
    let handle = System.WeakReference<obj>(null)

    /// Check if the new target is the same than the previous one
    /// This is done to avoid triggering change events when nothing changes
    member private __.IsSameTarget(target) =
        match handle.TryGetTarget() with
        | true, res when res = target -> true
        | _ -> false

    member x.Set(target: obj) : unit =
        if not (x.IsSameTarget(target)) then
            handle.SetTarget(target)
            onAttached x target

    member x.Unset() : unit =
        if not (x.IsSameTarget(null)) then
            handle.SetTarget(null)
            onDetached x ()

    member _.TryValue =
        match handle.TryGetTarget() with
        | true, null -> None
        | true, res -> Some res
        | _ -> None

type ViewRef<'T when 'T : not struct>() =
    let attached = Event<EventHandler<'T>, 'T>()
    let detached = Event<EventHandler, EventArgs>()

    let onAttached sender target = attached.Trigger(sender, unbox target)
    let onDetached sender () = detached.Trigger(sender, EventArgs())

    let handle = ViewRef(onAttached, onDetached)

    [<CLIEvent>] member __.Attached = attached.Publish
    [<CLIEvent>] member __.Detached = detached.Publish

    member _.Value : 'T =
        match handle.TryValue with
        | Some res -> unbox res
        | None -> failwith "view reference target has been collected or was not set"

    member _.TryValue : 'T option =
        match handle.TryValue with
        | Some res -> Some (unbox res)
        | None -> None

    member _.Unbox = handle
    
module ViewRef =
    let ViewRef =
        Attributes.defineScalarWithConverter<ViewRef, _, _>
            "Fabulous_ViewRef"
            id
            id
            ScalarAttributeComparers.equalityCompare
            (fun prevValue currValue node ->
                match prevValue with
                | ValueNone -> ()
                | ValueSome viewRef -> viewRef.Unset()
                
                match currValue with
                | ValueNone -> ()
                | ValueSome viewRef -> viewRef.Set(node.Target))