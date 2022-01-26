namespace Fabulous.XamarinForms

open Fabulous
open Xamarin.Forms

[<Struct>]
type AppThemeValues<'T> = { Light: 'T; Dark: 'T voption }

module Attributes =
    /// Define an attribute storing a Widget for a bindable property
    let inline defineBindableWidget (bindableProperty: BindableProperty) =
        Attributes.defineWidget<BindableObject, obj>
            bindableProperty.PropertyName
            (fun target -> ViewNode.get (target.GetValue(bindableProperty)))
            (fun target value ->
                if value = null then
                    target.ClearValue(bindableProperty)
                else
                    target.SetValue(bindableProperty, value))
                    
    let bindableUpdateNode<'a> (newValueOpt: 'a voption) (node: IViewNode) (bindableProperty: Xamarin.Forms.BindableProperty) =
        let target = node.Target :?> BindableObject
        match newValueOpt with
        | ValueNone -> target.ClearValue(bindableProperty)
        | ValueSome v -> target.SetValue(bindableProperty, v)

    let inline defineBindableWithComparer<'inputType, 'modelType, 'valueType>
        key
        (bindableProperty: BindableProperty)
        (convert: 'inputType -> 'modelType)
        (convertValue: 'modelType -> 'valueType)
        (compare: 'modelType -> 'modelType -> bool)
        =
        Attributes.defineCustomScalar<'inputType, 'modelType, 'valueType, BindableProperty>
            key
            bindableProperty
            convert
            convertValue
            compare
            bindableUpdateNode
                
    let equalCompare<'T when 'T : equality> (x: 'T) (y: 'T) = x = y

    let inline defineBindable<'T when 'T: equality> key bindableProperty =
        defineBindableWithComparer<'T, 'T, 'T> key bindableProperty id id equalCompare

    let inline defineAppThemeBindable<'T when 'T: equality> (bindableProperty: BindableProperty) =
        Attributes.defineCustomScalar<AppThemeValues<'T>, AppThemeValues<'T>, AppThemeValues<'T>, BindableProperty>
            bindableProperty.PropertyName
            bindableProperty
            id
            id
            (=)
            (fun newValueOpt node bindableProperty ->
                let target = node.Target :?> BindableObject
                match newValueOpt with
                | ValueNone -> target.ClearValue(bindableProperty)
                | ValueSome { Light = light; Dark = ValueNone } -> target.SetValue(bindableProperty, light)
                | ValueSome { Light = light; Dark = ValueSome dark } ->
                    target.SetOnAppTheme(bindableProperty, light, dark))
