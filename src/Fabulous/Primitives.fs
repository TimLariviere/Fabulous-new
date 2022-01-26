namespace Fabulous

open System

type IViewNode =
    abstract member Target: obj
    abstract member Parent: IViewNode voption

type IScalarAttributeDefinition =
    abstract member Key: string
    abstract member CompareBoxed: a: obj -> b: obj -> bool
    abstract member UpdateNode: obj voption -> IViewNode -> unit
    
type ScalarAttributeDefinition<'inputType, 'modelType, 'valueType, 'additionalData> =
    { Key: string
      AdditionalData: 'additionalData
      Convert: 'inputType -> 'modelType
      ConvertValue: 'modelType -> 'valueType
      Compare: 'modelType -> 'modelType -> bool
      UpdateNode: 'valueType voption -> IViewNode -> 'additionalData -> unit }

    interface IScalarAttributeDefinition with
        member x.Key = x.Key
        
        member x.CompareBoxed a b =
            x.Compare (unbox<'modelType> a) (unbox<'modelType> b)

        member x.UpdateNode newValueOpt node =
            let newValueOpt =
                match newValueOpt with
                | ValueNone -> ValueNone
                | ValueSome v -> ValueSome(x.ConvertValue(unbox v))

            x.UpdateNode newValueOpt node x.AdditionalData

type IEventAttributeDefinition =
    abstract member Key: string
    abstract member CompareBoxed: a: obj -> b: obj -> bool
    abstract member AddHandler: obj voption -> IViewNodeWithEvents -> unit
    abstract member RemoveHandler: IViewNodeWithEvents -> unit
    
and IViewNodeWithEvents =
    inherit IViewNode
    abstract member TryGetHandler<'T> : IEventAttributeDefinition -> 'T voption
    abstract member SetHandler<'T> : IEventAttributeDefinition * 'T voption -> unit
    abstract member Dispatch: obj -> unit

type EventAttributeDefinition<'inputType, 'eventHandler, 'args, 'targetType when 'eventHandler :> Delegate and 'eventHandler : delegate<'args, unit>> =
    { Key: string
      Convert: 'inputType -> 'args -> obj
      ConvertValue: ('args -> unit) -> 'eventHandler
      Compare: 'inputType -> 'inputType -> bool
      GetEvent: 'targetType -> IEvent<'eventHandler, 'args> }

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
                let dispatchFn = msgFn >> node.Dispatch
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
      Insert: IViewNode -> int -> Widget -> unit
      Replace: IViewNode -> int -> Widget -> unit
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
    type ScalarAttributeDefinition<'inputType, 'modelType, 'valueType, 'additionalData> with
        member inline x.WithValue(value) : ScalarAttribute =
            { Definition = x
              Value = x.Convert(value) }
            
    type EventAttributeDefinition<'inputType, 'eventHandler, 'args, 'targetType when 'eventHandler :> Delegate and 'eventHandler : delegate<'args, unit>> with
        member inline x.WithValue(value: obj) : EventAttribute =
            { Definition = x
              Value = value }
            
        member inline x.WithValue(value: 'args -> obj) : EventAttribute =
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
