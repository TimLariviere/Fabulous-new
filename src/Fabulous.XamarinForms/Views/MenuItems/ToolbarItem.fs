namespace Fabulous.XamarinForms

open System.Runtime.CompilerServices
open Fabulous
open Fabulous.StackList
open Xamarin.Forms

type IToolbarItem =
    inherit IMenuItem

module ToolbarItem =
    let WidgetKey = Widgets.register<ToolbarItem> ()

    let Order =
        Attributes.defineScalar<ToolbarItem, ToolbarItemOrder>
            "ToolbarItem_Order"
            (fun newValueOpt target ->
                match newValueOpt with
                | ValueNone -> target.Order <- ToolbarItemOrder.Default
                | ValueSome order -> target.Order <- order)

[<AutoOpen>]
module ToolbarItemBuilders =
    type Fabulous.XamarinForms.View with
        static member inline ToolbarItem<'msg>(text: string, onClicked: 'msg) =
            WidgetBuilder<'msg, IToolbarItem>(
                ToolbarItem.WidgetKey,
                AttributesBundle(
                    StackList.one (MenuItem.Text.WithValue(text)),
                    ValueSome [| MenuItem.Clicked.WithValue(onClicked) |],
                    ValueNone,
                    ValueNone
                )
            )

[<Extension>]
type ToolbarItemModifiers =
    [<Extension>]
    static member inline order(this: WidgetBuilder<'msg, #IToolbarItem>, value: ToolbarItemOrder) =
        this.AddScalar(ToolbarItem.Order.WithValue(value))
