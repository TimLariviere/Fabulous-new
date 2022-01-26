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

    let inline defineBindableWithComparer<'inputType, 'modelType, 'valueType>
        (bindableProperty: BindableProperty)
        (convert: 'inputType -> 'modelType)
        (convertValue: 'modelType -> 'valueType)
        (compare: 'modelType -> 'modelType -> bool)
        =
        Attributes.defineCustomScalar<'inputType, 'modelType, 'valueType, BindableProperty>
            bindableProperty.PropertyName
            bindableProperty
            convert
            convertValue
            compare
            (fun newValueOpt node bindableProperty ->
                let target = node.Target :?> BindableObject
                match newValueOpt with
                | ValueNone -> target.ClearValue(bindableProperty)
                | ValueSome v -> target.SetValue(bindableProperty, v))

    let inline defineBindable<'T when 'T: equality> bindableProperty =
        defineBindableWithComparer<'T, 'T, 'T> bindableProperty id id (=)

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
