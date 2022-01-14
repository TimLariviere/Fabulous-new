namespace Fabulous

open System
open Fabulous

module Helpers =
    let canReuse<'T when 'T: equality> (prev: 'T) (curr: 'T) = prev = curr

    let inline createViewForWidget (parent: IViewNode) (widget: Widget) =
        let widgetDefinition = WidgetDefinitionStore.get widget.Key

        let struct (_node, view) =
            widgetDefinition.CreateView(widget, parent.TreeContext, ValueSome parent)

        view

module ScalarAttributeComparers =
    let noCompare (_, _) = ScalarAttributeComparison.Different

    let equalityCompare (a, b) =
        if a = b then
            ScalarAttributeComparison.Identical
        else
            ScalarAttributeComparison.Different

module Attributes =
    let inline withName<'T when 'T :> IAttributeDefinition> (name: string) ([<InlineIfLambda>] fn: AttributeKey -> string -> 'T): 'T =
        let key = AttributeDefinitionStore.getKeyForName name
        match AttributeDefinitionStore.tryGet<'T> key with
        | ValueSome definition -> definition
        | ValueNone ->
            let definition = fn key name
            AttributeDefinitionStore.set key definition
            definition
    
    /// Define a custom attribute storing any value
    let defineScalarWithConverter<'input, 'model, 'value> name convert convertValue compare updateNode =
        withName name
            (fun key name ->
                { Key = key
                  Name = name
                  Convert = convert
                  ConvertValue = convertValue
                  Compare = compare
                  UpdateNode = updateNode } : ScalarAttributeDefinition<'input, 'model, 'value>)

    /// Define a custom attribute storing a widget
    let defineWidgetWithConverter name applyDiff updateNode =
        withName name
            (fun key name ->
                { Key = key
                  Name = name
                  ApplyDiff = applyDiff
                  UpdateNode = updateNode } : WidgetAttributeDefinition)
            
    /// Define a custom attribute storing a widget collection
    let defineWidgetCollectionWithConverter name applyDiff updateNode =
        withName name
            (fun key name ->
                { Key = key
                  Name = name
                  ApplyDiff = applyDiff
                  UpdateNode = updateNode } : WidgetCollectionAttributeDefinition)

    /// Define an attribute storing a Widget for a CLR property
    let defineWidget<'T when 'T: null> (name: string) (get: obj -> IViewNode) (set: obj -> 'T -> unit) =
        let applyDiff (diff: WidgetDiff, node: IViewNode) =
            let childNode = get node.Target

            match diff.ScalarChanges with
            | ValueSome changes -> childNode.ApplyScalarDiffs(changes)
            | ValueNone -> ()

            match diff.WidgetChanges with
            | ValueSome slice -> childNode.ApplyWidgetDiffs(ArraySlice.toSpan slice)
            | ValueNone -> ()

            match diff.WidgetCollectionChanges with
            | ValueSome slice -> childNode.ApplyWidgetCollectionDiffs(ArraySlice.toSpan slice)
            | ValueNone -> ()

        let updateNode (newValueOpt: Widget voption, node: IViewNode) =
            match newValueOpt with
            | ValueNone -> set node.Target null
            | ValueSome widget ->
                let view =
                    Helpers.createViewForWidget node widget |> unbox

                set node.Target view

        defineWidgetWithConverter name applyDiff updateNode

    /// Define an attribute storing a collection of Widget
    let defineWidgetCollection<'itemType> name (getCollection: obj -> System.Collections.Generic.IList<'itemType>) =
        let applyDiff (diffs: ArraySlice<WidgetCollectionItemChange>, node: IViewNode) =
            let targetColl = getCollection node.Target

            for diff in ArraySlice.toSpan diffs do
                match diff with
                | WidgetCollectionItemChange.Remove index -> targetColl.RemoveAt(index)
                | _ -> ()

            for diff in ArraySlice.toSpan diffs do
                match diff with
                | WidgetCollectionItemChange.Insert (index, widget) ->
                    let view = Helpers.createViewForWidget node widget
                    targetColl.Insert(index, unbox view)

                | WidgetCollectionItemChange.Update (index, widgetDiff) ->
                    let childNode =
                        node.TreeContext.GetViewNode(box targetColl.[index])

                    match widgetDiff.ScalarChanges with
                    | ValueSome changes -> childNode.ApplyScalarDiffs(changes)
                    | ValueNone -> ()

                    match widgetDiff.WidgetChanges with
                    | ValueSome slice -> childNode.ApplyWidgetDiffs(ArraySlice.toSpan slice)
                    | ValueNone -> ()

                    match widgetDiff.WidgetCollectionChanges with
                    | ValueSome slice -> childNode.ApplyWidgetCollectionDiffs(ArraySlice.toSpan slice)
                    | ValueNone -> ()

                | WidgetCollectionItemChange.Replace (index, widget) ->
                    let view = Helpers.createViewForWidget node widget
                    targetColl.[index] <- unbox view

                | _ -> ()

        let updateNode (newValueOpt: ArraySlice<Widget> voption, node: IViewNode) =
            let targetColl = getCollection node.Target
            targetColl.Clear()

            match newValueOpt with
            | ValueNone -> ()
            | ValueSome widgets ->
                for widget in ArraySlice.toSpan widgets do
                    let view = Helpers.createViewForWidget node widget
                    targetColl.Add(unbox view)

        defineWidgetCollectionWithConverter name applyDiff updateNode

    let inline define<'T when 'T: equality> name (updateTarget: 'T voption * IViewNode -> unit) =
        defineScalarWithConverter name id id ScalarAttributeComparers.equalityCompare updateTarget

    let dispatchMsgOnViewNode (node: IViewNode) msg =
        let mutable parentOpt = node.Parent

        let mutable mapMsg =
            match node.MapMsg with
            | ValueNone -> id
            | ValueSome fn -> fn

        while parentOpt.IsSome do
            let parent = parentOpt.Value
            parentOpt <- parent.Parent

            mapMsg <-
                match parent.MapMsg with
                | ValueNone -> mapMsg
                | ValueSome fn -> mapMsg >> fn

        let newMsg = mapMsg msg
        node.TreeContext.Dispatch(newMsg)

    let defineEventNoArg name (getEvent: obj -> IEvent<EventHandler, EventArgs>) =
        withName name
            (fun key name ->
                { Key = key
                  Name = name
                  Convert = id
                  ConvertValue = id
                  Compare = ScalarAttributeComparers.noCompare
                  UpdateNode =
                      fun (newValueOpt, node) ->
                          let event = getEvent node.Target

                          match node.TryGetHandler(key) with
                          | ValueNone -> ()
                          | ValueSome handler -> event.RemoveHandler handler

                          match newValueOpt with
                          | ValueNone -> node.SetHandler(key, ValueNone)

                          | ValueSome msg ->
                              let handler =
                                  EventHandler(fun _ _ -> dispatchMsgOnViewNode node msg)

                              event.AddHandler handler
                              node.SetHandler(key, ValueSome handler) } : ScalarAttributeDefinition<obj, obj, obj>)

    let defineEvent<'args> name (getEvent: obj -> IEvent<EventHandler<'args>, 'args>) =
        withName name
            (fun key name ->
                { Key = key
                  Name = name
                  Convert = id
                  ConvertValue = id
                  Compare = ScalarAttributeComparers.noCompare
                  UpdateNode =
                      fun (newValueOpt: ('args -> obj) voption, node: IViewNode) ->
                          let event = getEvent node.Target
                
                          match node.TryGetHandler(key) with
                          | ValueNone -> printfn $"No old handler for {name}"
                          | ValueSome handler ->
                              printfn $"Removed old handler for {name}"
                              event.RemoveHandler handler
                
                          match newValueOpt with
                          | ValueNone -> node.SetHandler(key, ValueNone)
                
                          | ValueSome fn ->
                              let handler =
                                  EventHandler<'args>
                                      (fun _ args ->
                                          printfn $"Handler for {name} triggered"
                                          let r = fn args
                                          dispatchMsgOnViewNode node r)
                
                              node.SetHandler(key, ValueSome handler)
                              event.AddHandler handler
                              printfn $"Added new handler for {name}" } : ScalarAttributeDefinition<_,_,_>)
