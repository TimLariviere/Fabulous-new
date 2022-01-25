namespace Fabulous.XamarinForms

open Fabulous
open Fabulous.StackList
open Xamarin.Forms

type IRefreshView =
    inherit IContentView

module RefreshView =
    let WidgetKey = Widgets.register<RefreshView> ()

    let IsRefreshing =
        Attributes.defineBindable<bool> RefreshView.IsRefreshingProperty

    let Refreshing =
        Attributes.defineEventNoArg<RefreshView> "RefreshView_Refreshing" (fun target -> target.Refreshing)

[<AutoOpen>]
module RefreshViewBuilders =
    type Fabulous.XamarinForms.View with
        static member inline RefreshView<'msg, 'marker when 'marker :> IView>
            (
                isRefreshing: bool,
                onRefreshing: 'msg,
                content: WidgetBuilder<'msg, 'marker>
            ) =
            WidgetBuilder<'msg, IRefreshView>(
                RefreshView.WidgetKey,
                AttributesBundle(
                    StackList.one (RefreshView.IsRefreshing.WithValue(isRefreshing)),
                    ValueSome [| RefreshView.Refreshing.WithValue(onRefreshing) |],
                    ValueSome [| ContentView.Content.WithValue(content.Compile()) |],
                    ValueNone
                )
            )
