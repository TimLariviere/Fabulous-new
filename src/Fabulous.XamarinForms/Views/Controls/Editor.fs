namespace Fabulous.XamarinForms

open Fabulous
open Fabulous.StackList
open Xamarin.Forms

type IEditor =
    inherit IInputView

module Editor =
    let WidgetKey = Widgets.register<Editor> ()

    let Text =
        Attributes.defineBindable<string> Editor.TextProperty

[<AutoOpen>]
module EditorBuilders =
    type Fabulous.XamarinForms.View with
        static member inline Editor<'msg>(text: string, onTextChanged: string -> 'msg) =
            WidgetBuilder<'msg, IEditor>(
                Editor.WidgetKey,
                AttributesBundle(
                    StackList.one (Editor.Text.WithValue(text)),
                    ValueSome [| InputView.TextChanged.WithValue(fun args -> onTextChanged args.NewTextValue |> box) |],
                    ValueNone,
                    ValueNone
                )
            )
