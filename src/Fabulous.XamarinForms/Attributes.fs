namespace Fabulous.XamarinForms

open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms

[<Struct>]
type AppThemeValues<'T> = { Light: 'T; Dark: 'T voption }

module Attributes =
    /// Define an attribute storing a Widget for a bindable property
    let defineBindableWidget name (bindableProperty: BindableProperty) =
        Attributes.defineWidget
            name
            (fun target ->
                let childTarget =
                    (target :?> BindableObject)
                        .GetValue(bindableProperty)

                ViewNode.get childTarget)
            (fun target value ->
                let bindableObject = target :?> BindableObject

                if value = null then
                    bindableObject.ClearValue(bindableProperty)
                else
                    bindableObject.SetValue(bindableProperty, value))

    let inline defineBindableWithComparer<'input, 'model, 'value>
        name
        (bindableProperty: BindableProperty)
        convert
        convertValue
        compare
        =
        Attributes.defineScalarWithConverter<'input, 'model, 'value>
            name
            convert
            convertValue
            compare
            (fun (newValueOpt, node) ->
                let target = node.Target :?> BindableObject

                match newValueOpt with
                | ValueNone -> target.ClearValue(bindableProperty)
                | ValueSome v -> target.SetValue(bindableProperty, v))

    let inline defineBindable<'T when 'T : equality> name bindableProperty =
        defineBindableWithComparer<'T, 'T, 'T> name bindableProperty id id ScalarAttributeComparers.equalityCompare

    let inline defineAppThemeBindable<'T when 'T: equality> name (bindableProperty: BindableProperty) =
        Attributes.defineScalarWithConverter<AppThemeValues<'T>, AppThemeValues<'T>, AppThemeValues<'T>>
            name
            id
            id
            ScalarAttributeComparers.equalityCompare
            (fun (newValueOpt, node) ->
                let target = node.Target :?> BindableObject

                match newValueOpt with
                | ValueNone -> target.ClearValue(bindableProperty)
                | ValueSome { Light = light; Dark = ValueNone } -> target.SetValue(bindableProperty, light)
                | ValueSome { Light = light; Dark = ValueSome dark } ->
                    target.SetOnAppTheme(bindableProperty, light, dark))
