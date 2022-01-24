namespace Fabulous

open System
open Fabulous

type IViewNode =
    abstract member Target: obj
    abstract member Parent: IViewNode voption

type IScalarAttributeDefinition =
    abstract member Key: string
    abstract member CompareBoxed: a: obj -> b: obj -> bool
    abstract member UpdateNode: obj voption -> obj -> unit
    
type ScalarAttributeDefinition<'inputType, 'modelType, 'valueType, 'targetType> =
    { Key: string
      Convert: 'inputType -> 'modelType
      ConvertValue: 'modelType -> 'valueType
      Compare: 'modelType -> 'modelType -> bool
      UpdateNode: 'valueType voption -> 'targetType -> unit }

    interface IScalarAttributeDefinition with
        member x.Key = x.Key
        
        member x.CompareBoxed a b =
            x.Compare (unbox<'modelType> a) (unbox<'modelType> b)

        member x.UpdateNode newValueOpt target =
            let newValueOpt =
                match newValueOpt with
                | ValueNone -> ValueNone
                | ValueSome v -> ValueSome(x.ConvertValue(unbox v))

            x.UpdateNode newValueOpt (unbox target)

type IEventAttributeDefinition =
    abstract member Key: string
    abstract member CompareBoxed: a: obj -> b: obj -> bool
    abstract member AddHandler: obj voption -> IViewNodeWithEvents -> unit
    abstract member RemoveHandler: IViewNodeWithEvents -> unit
    
and IViewNodeWithEvents =
    inherit IViewNode
    abstract member MapMsg: (obj -> obj) voption with get, set
    abstract member TryGetHandler<'T> : IEventAttributeDefinition -> 'T voption
    abstract member SetHandler<'T> : IEventAttributeDefinition * 'T voption -> unit

type EventAttributeDefinition<'inputType, 'eventHandler, 'args, 'targetType when 'eventHandler :> Delegate and 'eventHandler : delegate<'args, unit>> =
    { Key: string
      Convert: 'inputType -> 'args -> obj
      ConvertValue: ('args -> unit) -> 'eventHandler
      Compare: 'inputType -> 'inputType -> bool
      GetEvent: 'targetType -> IEvent<'eventHandler, 'args> }
    
    member x.DispatchMsg (node: IViewNodeWithEvents) =
        fun msg ->
            let mutable parentOpt = node.Parent
            let mutable mapMsg =
                match node.MapMsg with
                | ValueNone -> id
                | ValueSome fn -> fn

            while parentOpt.IsSome do
                let parent = parentOpt.Value :?> IViewNodeWithEvents
                parentOpt <- parent.Parent

                mapMsg <-
                    match parent.MapMsg with
                    | ValueNone -> mapMsg
                    | ValueSome fn -> mapMsg >> fn

            let newMsg = mapMsg msg
            //node.TreeContext.Dispatch(newMsg)
            ()

    interface IEventAttributeDefinition with
        member x.Key = x.Key
        
        member x.CompareBoxed a b =
            x.Compare (unbox<'inputType> a) (unbox<'inputType> b)
        
        member x.RemoveHandler node =
            match node.TryGetHandler(x) with
            | ValueNone -> ()
            | ValueSome handler ->
                let event = x.GetEvent(unbox node.Target)
                event.RemoveHandler(handler)
            
        member x.AddHandler newValueOpt node =
            match newValueOpt with
            | ValueNone ->
                node.SetHandler(x, ValueNone)
            | ValueSome v ->
                let msgFn = x.Convert(unbox v)
                let dispatchFn = msgFn >> x.DispatchMsg node
                let handler = x.ConvertValue(dispatchFn)
                let event = x.GetEvent(unbox node.Target)
                node.SetHandler(x, ValueSome handler)
                event.AddHandler(handler)

[<Struct>]
type ScalarAttribute =
    { Definition: IScalarAttributeDefinition
      Value: obj }

[<Struct>]
type EventAttribute =
    { Definition: IEventAttributeDefinition
      Value: obj }

type [<ReferenceEquality>] WidgetAttributeDefinition =
    { Key: string
      GetChildNode: IViewNode -> IViewNode
      CreateNode: Widget -> IViewNode -> unit
      RemoveNode: IViewNode -> unit }

and [<ReferenceEquality>] WidgetCollectionAttributeDefinition =
    { Key: string
      GetItemNode: IViewNode -> int -> IViewNode
      Insert: IViewNode -> int -> obj -> unit
      Replace: IViewNode -> int -> obj -> unit
      Remove: IViewNode -> int -> unit
      UpdateNode: ArraySlice<Widget> voption -> IViewNode -> unit }

and [<Struct>] WidgetAttribute =
    { Definition: WidgetAttributeDefinition
      Value: Widget }
    
and [<Struct>] WidgetCollectionAttribute =
    { Definition: WidgetCollectionAttributeDefinition
      Value: ArraySlice<Widget> }

and [<Struct>] WidgetData =
    { ScalarAttributes: ScalarAttribute[] voption
      EventAttributes: EventAttribute[] voption
      WidgetAttributes: WidgetAttribute[] voption
      WidgetCollectionAttributes: WidgetCollectionAttribute[] voption }

and [<ReferenceEquality>] WidgetDefinition =
    { Key: string
      TargetType: Type
      CreateView: WidgetData -> ViewTreeContext -> IViewNode voption -> struct (IViewNode * obj) }

and [<Struct>] Widget =
    { Definition: WidgetDefinition
      Data: WidgetData }
    
and ViewTreeContext =
    { CanReuseView: Widget -> Widget -> bool
      GetViewNode: obj -> IViewNode
      Dispatch: obj -> unit }
    
and IViewNodeWithContext =
    inherit IViewNode
    abstract member TreeContext: ViewTreeContext
        
[<AutoOpen>]
module WithValue =
    type ScalarAttributeDefinition<'inputType, 'modelType, 'valueType, 'targetType> with
        member inline x.WithValue(value) : ScalarAttribute =
            { Definition = x
              Value = x.Convert(value) }
            
    type EventAttributeDefinition<'inputType, 'eventHandler, 'args, 'targetType when 'eventHandler :> Delegate and 'eventHandler : delegate<'args, unit>> with
        member inline x.WithValue(value) : EventAttribute =
            { Definition = x
              Value = value }
            
    type WidgetAttributeDefinition with
        member inline x.WithValue(value) : WidgetAttribute =
            { Definition = x
              Value = value }
    
    type WidgetCollectionAttributeDefinition with        
        member inline x.WithValue(value) : WidgetCollectionAttribute =
            { Definition = x
              Value = value }
