namespace Fabulous

open System
open Fabulous

module Helpers =
    let inline createViewForWidget<'T> (parent: IViewNode) (widget: Widget) =
        let struct (_node, view) =
            widget.Definition.CreateView widget.Data (parent :?> IViewNodeWithContext).TreeContext (ValueSome parent)
        unbox<'T> view

module Attributes =
    /// Define a custom attribute storing any value
    let inline defineCustomScalar<'inputType, 'modelType, 'valueType>
        key
        (convert: 'inputType -> 'modelType)
        (convertValue: 'modelType -> 'valueType)
        (compare: 'modelType -> 'modelType -> bool)
        (updateNode: 'valueType voption -> IViewNode -> unit)
        =
        { Key = key
          Convert = convert
          ConvertValue = convertValue
          Compare = compare
          UpdateNode = updateNode }
        
    let inline defineCustomEvent<'inputType, 'args, 'eventHandler, 'targetType when 'eventHandler :> Delegate and 'eventHandler : delegate<'args, unit>>
        key
        (convert: 'inputType -> 'args -> obj)
        (convertValue: ('args -> unit) -> 'eventHandler)
        (compare: 'inputType -> 'inputType -> bool)
        (getEvent: 'targetType -> IEvent<'eventHandler, 'args>) =
        { Key = key
          Convert = convert
          ConvertValue = convertValue
          Compare = compare
          GetEvent = getEvent }

    /// Define a custom attribute storing a widget
    let inline defineCustomWidget
        key
        (getChildNode: IViewNode -> IViewNode)
        (createNode: Widget -> IViewNode -> unit)
        (removeNode: IViewNode -> unit)
        =
        { Key = key
          GetChildNode = getChildNode
          CreateNode = createNode
          RemoveNode = removeNode }

    /// Define a custom attribute storing a widget collection
    let inline defineCustomWidgetCollection
        key
        (getItemNode: IViewNode -> int -> IViewNode)
        (insert: IViewNode -> int -> Widget -> unit)
        (replace: IViewNode -> int -> Widget -> unit)
        (remove: IViewNode -> int -> unit)
        (updateNode: ArraySlice<Widget> voption -> IViewNode -> unit)
        =
        { Key = key
          GetItemNode = getItemNode
          Insert = insert
          Replace = replace
          Remove = remove
          UpdateNode = updateNode }

    /// Define an attribute storing a Widget for a CLR property
    let inline defineWidget<'controlType, 'attributeType when 'attributeType: null> (key: string) (get: 'controlType -> IViewNode) (set: 'controlType -> 'attributeType -> unit) =
        defineCustomWidget
            key
            (fun parentNode -> get (unbox parentNode.Target))
            (fun widget parentNode -> Helpers.createViewForWidget parentNode widget |> set (unbox parentNode.Target))
            (fun parentNode -> set (unbox parentNode.Target) null)

    /// Define an attribute storing a collection of Widget
    let defineWidgetCollection<'targetType, 'itemType> key (getCollection: 'targetType -> System.Collections.Generic.IList<'itemType>) =
        let getCollection = unbox >> getCollection
        
        let updateNode (newValueOpt: ArraySlice<Widget> voption) (node: IViewNode) =
            let targetColl = getCollection node.Target
            targetColl.Clear()

            match newValueOpt with
            | ValueNone -> ()
            | ValueSome widgets ->
                for widget in ArraySlice.toSpan widgets do
                    let view = Helpers.createViewForWidget node widget
                    targetColl.Add(unbox view)

        defineCustomWidgetCollection
            key
            (fun node index -> (node :?> IViewNodeWithContext).TreeContext.GetViewNode (unbox (getCollection node.Target).[index]))
            (fun node index value -> (getCollection node.Target).Insert(index, unbox value))
            (fun node index value -> (getCollection node.Target).[index] <- unbox value)
            (fun node index -> (getCollection node.Target).RemoveAt(index))
            updateNode

    let inline defineScalar<'targetType, 'T when 'T: equality> name updateTarget =
        defineCustomScalar<'T, 'T, 'T> name id id (=) (fun newValueOpt node -> updateTarget newValueOpt (unbox<'targetType> node.Target))

    let inline defineEventNoArg<'targetType> key (getEvent: 'targetType -> IEvent<EventHandler, EventArgs>) =
        defineCustomEvent<obj, EventArgs, EventHandler, 'targetType>
            key
            (fun msg -> fun _ -> msg)
            (fun fn -> EventHandler(fun _ -> fn))
            (=)
            getEvent

    let inline defineEventWithArgs<'targetType, 'args> key (getEvent: 'targetType -> IEvent<EventHandler<'args>, 'args>) =
        defineCustomEvent<'args -> obj, 'args, EventHandler<'args>, 'targetType>
            key
            id
            (fun fn -> EventHandler<'args>(fun _ -> fn))
            (fun _ _ -> false)
            getEvent
