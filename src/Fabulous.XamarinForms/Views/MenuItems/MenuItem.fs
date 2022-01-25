namespace Fabulous.XamarinForms

open Fabulous
open Fabulous.StackList
open Xamarin.Forms

type IMenuItem =
    inherit IElement

module MenuItem =
    let WidgetKey = Widgets.register<MenuItem> ()

    let Text =
        Attributes.defineBindable<string> MenuItem.TextProperty

    let Clicked =
        Attributes.defineEventNoArg<MenuItem> "MenuItem_Clicked" (fun target -> target.Clicked)

[<AutoOpen>]
module MenuItemBuilders =
    type Fabulous.XamarinForms.View with
        static member inline MenuItem<'msg>(text: string, onClicked: 'msg) =
            WidgetBuilder<'msg, IMenuItem>(
                MenuItem.WidgetKey,
                AttributesBundle(
                    StackList.one (MenuItem.Text.WithValue(text)),
                    ValueSome [| MenuItem.Clicked.WithValue(onClicked) |],
                    ValueNone,
                    ValueNone
                )
            )
