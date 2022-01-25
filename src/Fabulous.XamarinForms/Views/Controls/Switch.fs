namespace Fabulous.XamarinForms

open Fabulous
open Fabulous.StackList
open Xamarin.Forms

type ISwitch =
    inherit IView

module Switch =
    let WidgetKey = Widgets.register<Switch> ()

    let IsToggled =
        Attributes.defineBindable<bool> Switch.IsToggledProperty

    let Toggled =
        Attributes.defineEventWithArgs<Switch, ToggledEventArgs> "Switch_Toggled" (fun target -> target.Toggled)

[<AutoOpen>]
module SwitchBuilders =
    type Fabulous.XamarinForms.View with
        static member inline Switch<'msg>(isToggled: bool, onToggled: bool -> 'msg) =
            WidgetBuilder<'msg, ISwitch>(
                Switch.WidgetKey,
                AttributesBundle(
                    StackList.one (Switch.IsToggled.WithValue(isToggled)),
                    ValueSome [| Switch.Toggled.WithValue(fun args -> onToggled args.Value |> box) |],
                    ValueNone,
                    ValueNone
                )
            )
