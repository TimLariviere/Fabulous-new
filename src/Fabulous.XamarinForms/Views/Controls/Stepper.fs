namespace Fabulous.XamarinForms

open System.Runtime.CompilerServices
open Fabulous
open Fabulous.StackList
open Xamarin.Forms

type IStepper =
    inherit IView

module StepperUpdaters =
    let updateMinMax (newValueOpt: struct (float * float) voption) (stepper: Stepper) =
        match newValueOpt with
        | ValueNone ->
            stepper.ClearValue(Stepper.MinimumProperty)
            stepper.ClearValue(Stepper.MaximumProperty)
        | ValueSome (min, max) ->
            let currMax =
                stepper.GetValue(Stepper.MaximumProperty) :?> float

            if min > currMax then
                stepper.SetValue(Stepper.MaximumProperty, max)
                stepper.SetValue(Stepper.MinimumProperty, min)
            else
                stepper.SetValue(Stepper.MinimumProperty, min)
                stepper.SetValue(Stepper.MaximumProperty, max)

module Stepper =
    let WidgetKey = Widgets.register<Stepper> ()

    let Increment =
        Attributes.defineBindable<float> Stepper.IncrementProperty

    let MinimumMaximum =
        Attributes.defineScalar<Stepper, struct (float * float)> "Stepper_MinimumMaximum" StepperUpdaters.updateMinMax

    let Value =
        Attributes.defineBindable<float> Stepper.ValueProperty

    let ValueChanged =
        Attributes.defineEventWithArgs<Stepper, ValueChangedEventArgs>
            "Stepper_ValueChanged"
            (fun target -> target.ValueChanged)

[<AutoOpen>]
module StepperBuilders =
    type Fabulous.XamarinForms.View with
        static member inline Stepper<'msg>(min: float, max: float, value: float, onValueChanged: float -> 'msg) =
            WidgetBuilder<'msg, IStepper>(
                Stepper.WidgetKey,
                AttributesBundle(
                    StackList.two(Stepper.Value.WithValue(value), Stepper.MinimumMaximum.WithValue(struct (min, max))),
                    ValueSome [| Stepper.ValueChanged.WithValue(fun args -> onValueChanged args.NewValue |> box) |],
                    ValueNone,
                    ValueNone
                )
            )

[<Extension>]
type StepperModifiers =
    [<Extension>]
    static member inline increment(this: WidgetBuilder<'msg, #IStepper>, value: float) =
        this.AddScalar(Stepper.Increment.WithValue(value))
