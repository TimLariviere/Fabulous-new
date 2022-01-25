namespace Fabulous.XamarinForms

open Fabulous
open Fabulous.StackList
open Xamarin.Forms

type ITapGestureRecognizer =
    inherit Fabulous.XamarinForms.IGestureRecognizer

module TapGestureRecognizer =
    let WidgetKey =
        Widgets.register<TapGestureRecognizer> ()

    let Tapped =
        Attributes.defineEventNoArg<TapGestureRecognizer>
            "TapGestureRecognizer_Tapped"
            (fun target -> target.Tapped)

[<AutoOpen>]
module TapGestureRecognizerBuilders =
    type Fabulous.XamarinForms.View with
        static member inline TapGestureRecognizer<'msg>(onTapped: 'msg) =
            WidgetBuilder<'msg, ITapGestureRecognizer>(
                TapGestureRecognizer.WidgetKey,
                AttributesBundle(StackList.empty (), ValueSome [| TapGestureRecognizer.Tapped.WithValue(onTapped) |], ValueNone, ValueNone)
            )
