namespace Fabulous

open System
open System.Collections.Generic
open Fabulous

/// Widget definition to create a control
type WidgetDefinition =
    { Key: WidgetKey
      Name: string
      TargetType: Type
      CreateView: Widget * ViewTreeContext * IViewNode voption -> struct (IViewNode * obj) }

module WidgetDefinitionStore =
    let private _keys =
        Dictionary<Type, WidgetKey>()
    let private _widgets =
        Dictionary<WidgetKey, WidgetDefinition>()

    let mutable private _nextKey = 0

    let get key = _widgets.[key]
    let set key value = _widgets.[key] <- value
    
    let getKeyForType<'T> (): WidgetKey =
        match _keys.TryGetValue(typeof<'T>) with
        | true, key -> key
        | false, _ ->
            let key = _nextKey
            _nextKey <- _nextKey + 1
            _keys.[typeof<'T>] <- key
            key
            
    let contains (key: AttributeKey) =
        _widgets.ContainsKey(key)
        
module Widgets =
    let inline withType<'T> ([<InlineIfLambda>] fn: WidgetKey -> string -> WidgetDefinition) =
        let key = WidgetDefinitionStore.getKeyForType<'T>()
        if not (WidgetDefinitionStore.contains key) then
            let definition = fn key typeof<'T>.Name
            WidgetDefinitionStore.set key definition
        key