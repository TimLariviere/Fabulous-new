namespace Fabulous.XamarinForms

open System.Runtime.CompilerServices
open Fabulous
open Fabulous.StackList
open Xamarin.Forms

type IButton =
    inherit IView

module Button =
    let WidgetKey = Widgets.register<Button> ()

    let Text =
        Attributes.defineBindable<string> Button.TextProperty

    let Clicked =
        Attributes.defineEventNoArg<Button> "Button_Clicked" (fun target -> target.Clicked)

    let TextColor =
        Attributes.defineAppThemeBindable<Color> Button.TextColorProperty

    let FontSize =
        Attributes.defineBindable<double> Button.FontSizeProperty

[<AutoOpen>]
module ButtonBuilders =
    type Fabulous.XamarinForms.View with
        static member inline Button<'msg>(text: string, onClicked: 'msg) =
            WidgetBuilder<'msg, IButton>(
                Button.WidgetKey,
                AttributesBundle(
                    StackList.one (Button.Text.WithValue(text)),
                    ValueSome [| Button.Clicked.WithValue(onClicked) |],
                    ValueNone,
                    ValueNone
                )
            )

[<Extension>]
type ButtonModifiers =
    [<Extension>]
    static member inline textColor(this: WidgetBuilder<'msg, #IButton>, light: Color, ?dark: Color) =
        this.AddScalar(
            Button.TextColor.WithValue(
                { Light = light
                  Dark =
                      match dark with
                      | None -> ValueNone
                      | Some v -> ValueSome v }
            )
        )

    [<Extension>]
    static member inline font(this: WidgetBuilder<'msg, #IButton>, value: double) =
        this.AddScalar(Button.FontSize.WithValue(value))

    [<Extension>]
    static member inline font(this: WidgetBuilder<'msg, #IButton>, value: NamedSize) =
        this.AddScalar(Button.FontSize.WithValue(Device.GetNamedSize(value, typeof<Button>)))
