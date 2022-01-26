namespace Fabulous.XamarinForms

open Fabulous
open Xamarin.Forms

type IContentView =
    inherit Fabulous.XamarinForms.ILayout

module ContentView =
    let WidgetKey = Widgets.register<ContentView> ()

    let Content =
        Attributes.defineBindableWidget ContentView.ContentProperty

[<AutoOpen>]
module ContentViewBuilders =
    type Fabulous.XamarinForms.View with
        static member inline ContentView<'msg, 'marker when 'marker :> IView>(content: WidgetBuilder<'msg, 'marker>) =
            WidgetHelpers.buildWidget<'msg, IContentView>
                ContentView.WidgetKey
                [| ContentView.Content.WithValue(content.Compile()) |]
