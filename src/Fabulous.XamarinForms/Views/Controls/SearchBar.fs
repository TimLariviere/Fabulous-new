namespace Fabulous.XamarinForms

open System.Runtime.CompilerServices
open Fabulous
open Fabulous.StackList
open Xamarin.Forms

type ISearchBar =
    inherit IInputView

module SearchBar =
    let WidgetKey = Widgets.register<SearchBar> ()

    let Text =
        Attributes.defineBindable<string> SearchBar.TextProperty

    let CancelButtonColor =
        Attributes.defineBindable<Color> SearchBar.CancelButtonColorProperty

    let SearchButtonPressed =
        Attributes.defineEventNoArg<SearchBar>
            "SearchBar_SearchButtonPressed"
            (fun target -> target.SearchButtonPressed)

[<AutoOpen>]
module SearchBarBuilders =
    type Fabulous.XamarinForms.View with
        static member inline SearchBar<'msg>(text: string, onTextChanged: string -> 'msg, onSearchButtonPressed: 'msg) =
            WidgetBuilder<'msg, ISearchBar>(
                SearchBar.WidgetKey,
                AttributesBundle(
                    StackList.one (SearchBar.Text.WithValue(text)),
                    ValueSome [|
                        InputView.TextChanged.WithValue(fun args -> onTextChanged args.NewTextValue |> box)
                        SearchBar.SearchButtonPressed.WithValue(onSearchButtonPressed)
                    |],
                    ValueNone,
                    ValueNone
                )
            )

[<Extension>]
type SearchBarModifiers =
    [<Extension>]
    static member inline cancelButtonColor(this: WidgetBuilder<'msg, #ISearchBar>, value: Color) =
        this.AddScalar(SearchBar.CancelButtonColor.WithValue(value))
