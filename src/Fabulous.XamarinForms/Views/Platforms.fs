namespace Fabulous.XamarinForms

open System.Runtime.CompilerServices
open Fabulous
open Xamarin.Forms.PlatformConfiguration
open Xamarin.Forms.PlatformConfiguration.iOSSpecific
open Xamarin.Forms.PlatformConfiguration.AndroidSpecific

module iOS =
    let UseSafeArea =
        Attributes.defineScalar<Xamarin.Forms.Page, bool>
            "Page_UseSafeArea"
            (fun newValueOpt target ->
                let value =
                    match newValueOpt with
                    | ValueNone -> false
                    | ValueSome v -> v

                target.On<iOS>().SetUseSafeArea(value) |> ignore)

module Android =
    let ToolbarPlacement =
        Attributes.defineScalar<Xamarin.Forms.TabbedPage, ToolbarPlacement>
            "TabbedPage_ToolbarPlacement"
            (fun newValueOpt target ->
                let value =
                    match newValueOpt with
                    | ValueNone -> ToolbarPlacement.Default
                    | ValueSome v -> v

                target
                    .On<Android>()
                    .SetToolbarPlacement(value)
                |> ignore)

[<Extension>]
type PlatformModifiers =
    [<Extension>]
    static member inline ignoreSafeArea(this: WidgetBuilder<'msg, #IPage>) =
        this.AddScalar(iOS.UseSafeArea.WithValue(false))

    [<Extension>]
    static member inline androidToolbarPlacement(this: WidgetBuilder<'msg, #ITabbedPage>, value: ToolbarPlacement) =
        this.AddScalar(Android.ToolbarPlacement.WithValue(value))
