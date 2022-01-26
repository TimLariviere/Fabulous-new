namespace Fabulous.XamarinForms

open Fabulous
open Fabulous.StackList
open Xamarin.Forms

type ISlider =
    inherit IView
    
module SliderUpdaters =
    let updateMinMax (newValueOpt: struct (float * float) voption) (slider: Slider) =
        match newValueOpt with
        | ValueNone ->
            slider.ClearValue(Slider.MinimumProperty)
            slider.ClearValue(Slider.MaximumProperty)
        | ValueSome (min, max) ->
            let currMax =
                slider.GetValue(Slider.MaximumProperty) :?> float

            if min > currMax then
                slider.SetValue(Slider.MaximumProperty, max)
                slider.SetValue(Slider.MinimumProperty, min)
            else
                slider.SetValue(Slider.MinimumProperty, min)
                slider.SetValue(Slider.MaximumProperty, max)

module Slider =
    let WidgetKey = Widgets.register<Slider> ()

    let MinimumMaximum =
        Attributes.defineScalar<Slider, struct (float * float)> "Slider_MinimumMaximum" SliderUpdaters.updateMinMax

    let Value =
        Attributes.defineBindable<float> Slider.ValueProperty

    let ValueChanged =
        Attributes.defineEventWithArgs<Slider, ValueChangedEventArgs>
            "Slider_ValueChanged"
            (fun target -> target.ValueChanged)

[<AutoOpen>]
module SliderBuilders =
    type Fabulous.XamarinForms.View with
        static member inline Slider<'msg>(min: float, max: float, value: float, onValueChanged: float -> 'msg) =
            WidgetBuilder<'msg, ISlider>(
                Slider.WidgetKey,
                AttributesBundle(
                    StackList.two(Slider.Value.WithValue(value), Slider.MinimumMaximum.WithValue(struct (min, max))),
                    ValueSome [| Slider.ValueChanged.WithValue(fun args -> onValueChanged args.NewValue |> box) |],
                    ValueNone,
                    ValueNone
                )
            )
