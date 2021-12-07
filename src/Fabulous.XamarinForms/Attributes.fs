namespace Fabulous.XamarinForms

open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms

type [<Struct>] AppThemeValues<'T> =
    { Light: 'T
      Dark: 'T voption }

module Attributes =
    /// Define an attribute storing a Widget for a bindable property
    let defineBindableWidget (bindableProperty: BindableProperty) =
        Attributes.defineWidget
            ViewNode.getViewNode
            bindableProperty.PropertyName
            (fun parent ->
                let p = (parent :?> BindableObject)
                let x = p.GetValue(bindableProperty)
                x
            )
            (fun target value ->
                let bindableObject = target :?> BindableObject
                if value = null then
                    bindableObject.ClearValue(bindableProperty)
                else
                    bindableObject.SetValue(bindableProperty, value)
            )

    let defineBindableWithComparer<'inputType, 'modelType> (bindableProperty: Xamarin.Forms.BindableProperty) (convert: 'inputType -> 'modelType) (compare: ('modelType * 'modelType) -> ScalarAttributeComparison) =
        Attributes.defineScalarWithConverter<'inputType, 'modelType>
            bindableProperty.PropertyName
            convert
            compare
            (fun (newValueOpt, target) ->
                match newValueOpt with
                | ValueNone -> (target :?> BindableObject).ClearValue(bindableProperty)
                | ValueSome v -> (target :?> BindableObject).SetValue(bindableProperty, v)
            )

    let inline defineBindable<'T when 'T: equality> bindableProperty =
        defineBindableWithComparer<'T, 'T> bindableProperty id ScalarAttributeComparers.equalityCompare

    let inline defineAppThemeBindable<'T when 'T: equality> (bindableProperty: Xamarin.Forms.BindableProperty) =
        Attributes.defineScalarWithConverter<AppThemeValues<'T>, AppThemeValues<'T>>
            bindableProperty.PropertyName
            id
            ScalarAttributeComparers.equalityCompare
            (fun (newValueOpt, target) ->
                match newValueOpt with
                | ValueNone -> (target :?> BindableObject).ClearValue(bindableProperty)
                | ValueSome { Light = light; Dark = ValueNone } -> (target :?> BindableObject).SetValue(bindableProperty, light)
                | ValueSome { Light = light; Dark = ValueSome dark } -> (target :?> BindableObject).SetOnAppTheme(bindableProperty, light, dark)
            )

    let defineStyleSetter<'T when 'T: equality> (bindableProperty: Xamarin.Forms.BindableProperty) =
        Attributes.defineScalarWithConverter<'T, 'T>
            bindableProperty.PropertyName
            id
            ScalarAttributeComparers.equalityCompare
            (fun (newValueOpt, target) ->
                let style = (target :?> BindableStyle).FormsStyle
                let setterOpt = style.Setters |> Seq.tryFind (fun s -> s.Property = bindableProperty)
                match struct (newValueOpt, setterOpt) with
                | ValueNone, None -> ()
                | ValueNone, Some setter -> style.Setters.Remove(setter) |> ignore
                | ValueSome value, None ->
                    style.Setters.Add(Xamarin.Forms.Setter(Property = bindableProperty, Value = value))
                    ()
                | ValueSome value, Some setter -> setter.Value <- value
            )