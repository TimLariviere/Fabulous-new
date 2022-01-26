namespace Fabulous.XamarinForms

open System.Runtime.CompilerServices
open Fabulous
open Xamarin.Forms

type IPage =
    inherit IElement

module Page =
    let BackgroundImageSource =
        Attributes.defineBindable<ImageSource> Page.BackgroundImageSourceProperty

    let IconImageSource =
        Attributes.defineBindable<ImageSource> Page.IconImageSourceProperty

    let IsBusy =
        Attributes.defineBindable<bool> Page.IsBusyProperty

    let Padding =
        Attributes.defineBindable<Thickness> Page.PaddingProperty

    let Title =
        Attributes.defineBindable<string> Page.TitleProperty

    let ToolbarItems =
        Attributes.defineWidgetCollection<Page, ToolbarItem>
            "Page_ToolbarItems"
            (fun target -> target.ToolbarItems)

    let Appearing =
        Attributes.defineEventNoArg<Page> "Page_Appearing" (fun target -> target.Appearing)

    let Disappearing =
        Attributes.defineEventNoArg<Page> "Page_Disappearing" (fun target -> target.Disappearing)

    let LayoutChanged =
        Attributes.defineEventNoArg<Page> "Page_LayoutChanged" (fun target -> target.LayoutChanged)

[<Extension>]
type PageModifiers =
    [<Extension>]
    static member inline title(this: WidgetBuilder<'msg, #IPage>, value: string) =
        this.AddScalar(Page.Title.WithValue(value))

    [<Extension>]
    static member inline fileIcon(this: WidgetBuilder<'msg, #IPage>, value: string) =
        this.AddScalar(Page.IconImageSource.WithValue(ImageSource.FromFile(value)))

    [<Extension>]
    static member inline onAppearing(this: WidgetBuilder<'msg, #IPage>, value: 'msg) =
        this.AddEvent(Page.Appearing.WithValue(value))

    [<Extension>]
    static member inline onDisappearing(this: WidgetBuilder<'msg, #IPage>, value: 'msg) =
        this.AddEvent(Page.Disappearing.WithValue(value))

    [<Extension>]
    static member inline onLayoutChanged(this: WidgetBuilder<'msg, #IPage>, value: 'msg) =
        this.AddEvent(Page.LayoutChanged.WithValue(value))

    [<Extension>]
    static member inline toolbarItems<'msg, 'marker when 'marker :> IPage>(this: WidgetBuilder<'msg, 'marker>) =
        WidgetHelpers.buildAttributeCollection<'msg, 'marker, IToolbarItem> Page.ToolbarItems this
