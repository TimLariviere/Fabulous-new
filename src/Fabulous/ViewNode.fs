﻿namespace Fabulous

open Fabulous

/// Define the logic to apply diffs and store event handlers of its target control
type ViewNode(parentNode: IViewNode voption, treeContext: ViewTreeContext, targetRef: System.WeakReference) =

    // TODO consider combine handlers mapMsg and property bag
    // also we can probably use just Dictionary instead of Map because
    // ViewNode is supposed to be mutable, stateful and persistent object
    let mutable _handlers: Map<AttributeKey, obj> = Map.empty

    interface IViewNode with
        member _.Target = targetRef.Target
        member _.TreeContext = treeContext
        member _.Parent = parentNode
        member val MapMsg: (obj -> obj) voption = ValueNone with get, set
        member val MemoizedWidget: Widget option = None with get, set

        member _.TryGetHandler<'T>(key: AttributeKey) =
            match Map.tryFind key _handlers with
            | None -> ValueNone
            | Some v -> ValueSome(unbox<'T> v)

        member _.SetHandler<'T>(key: AttributeKey, handlerOpt: 'T voption) =
            _handlers <-
                _handlers
                |> Map.change
                    key
                    (fun _ ->
                        match handlerOpt with
                        | ValueNone -> None
                        | ValueSome h -> Some(box h))

        member this.ApplyScalarDiffs(diffs) =
            if not targetRef.IsAlive then
                ()
            else
                for diff in diffs do
                    match diff with
                    | ScalarChange.Added added ->
                        let definition =
                            AttributeDefinitionStore.get<IScalarAttributeDefinition> added.Key 

                        definition.UpdateNode(ValueSome added.Value, this)

                    | ScalarChange.Removed removed ->
                        let definition =
                            AttributeDefinitionStore.get<IScalarAttributeDefinition> removed.Key

                        definition.UpdateNode(ValueNone, this)

                    | ScalarChange.Updated newAttr ->
                        let definition =
                            AttributeDefinitionStore.get<IScalarAttributeDefinition> newAttr.Key

                        definition.UpdateNode(ValueSome newAttr.Value, this)

        member this.ApplyWidgetDiffs(diffs) =
            if not targetRef.IsAlive then
                ()
            else
                for diff in diffs do
                    match diff with
                    | WidgetChange.Added newWidget
                    | WidgetChange.ReplacedBy newWidget ->
                        let definition =
                            AttributeDefinitionStore.get<WidgetAttributeDefinition> newWidget.Key 

                        definition.UpdateNode(ValueSome newWidget.Value, this :> IViewNode)

                    | WidgetChange.Removed removed ->
                        let definition =
                            AttributeDefinitionStore.get<WidgetAttributeDefinition> removed.Key

                        definition.UpdateNode(ValueNone, this :> IViewNode)

                    | WidgetChange.Updated struct (newAttr, diffs) ->
                        let definition =
                            AttributeDefinitionStore.get<WidgetAttributeDefinition> newAttr.Key

                        definition.ApplyDiff(diffs, this :> IViewNode)

        member this.ApplyWidgetCollectionDiffs(diffs) =
            if not targetRef.IsAlive then
                ()
            else
                for diff in diffs do
                    match diff with
                    | WidgetCollectionChange.Added added ->
                        let definition =
                            AttributeDefinitionStore.get<WidgetCollectionAttributeDefinition> added.Key

                        definition.UpdateNode(ValueSome added.Value, this :> IViewNode)

                    | WidgetCollectionChange.Removed removed ->
                        let definition =
                            AttributeDefinitionStore.get<WidgetCollectionAttributeDefinition> removed.Key

                        definition.UpdateNode(ValueNone, this :> IViewNode)

                    | WidgetCollectionChange.Updated struct (newAttr, diffs) ->
                        let definition =
                            AttributeDefinitionStore.get<WidgetCollectionAttributeDefinition> newAttr.Key

                        definition.ApplyDiff(diffs, this :> IViewNode)
