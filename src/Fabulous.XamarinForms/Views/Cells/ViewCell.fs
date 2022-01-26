namespace Fabulous.XamarinForms

open Fabulous
open Xamarin.Forms

type IViewCell =
    inherit ICell

module ViewCell =
    let WidgetKey = Widgets.register<ViewCell> ()

    let View =
        Attributes.defineWidget<ViewCell, View>
            "ViewCell_View"
            (fun target -> ViewNode.get target.View)
            (fun target value -> target.View <- value)

[<AutoOpen>]
module ViewCellBuilders =
    type Fabulous.XamarinForms.View with
        static member inline ViewCell<'msg, 'marker when 'marker :> IView>(view: WidgetBuilder<'msg, 'marker>) =
            WidgetHelpers.buildWidget<'msg, IViewCell> ViewCell.WidgetKey [| ViewCell.View.WithValue(view.Compile()) |]
