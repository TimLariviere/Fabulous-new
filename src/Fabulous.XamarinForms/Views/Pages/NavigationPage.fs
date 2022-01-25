namespace Fabulous.XamarinForms

open System.Runtime.CompilerServices
open Fabulous
open Xamarin.Forms

type INavigationPage =
    inherit IPage

/// NOTE: Would be better to have a custom diff logic for Navigation
/// because it's a Stack and not a random access collection
module NavigationPageUpdaters =
    let getItemNode (node: IViewNode) (index: int) =
        let target = node.Target :?> NavigationPage
        let pages = List.ofSeq target.Pages
        let context = (node :?> IViewNodeWithContext).TreeContext
        context.GetViewNode(box pages.[index])
    
    let insert (node: IViewNode) (index: int) (widget: Widget) =
        let target = node.Target :?> NavigationPage
        let pages = List.ofSeq target.Pages
        
        if index >= pages.Length then
            let page =
                Helpers.createViewForWidget<Page> node widget

            target.PushAsync(page) |> ignore
        else
            let temp = System.Collections.Generic.Stack<Page>()

            for i = pages.Length - 1 to index do
                temp.Push(pages.[i])
                target.PopAsync() |> ignore

            let page =
                Helpers.createViewForWidget<Page> node widget

            target.PushAsync(page, false) |> ignore

            while temp.Count > 0 do
                target.PushAsync(temp.Pop(), false)
                |> ignore
        
    let replace (node: IViewNode) (index: int) (widget: Widget) =
        let target = node.Target :?> NavigationPage
        let pages = List.ofSeq target.Pages
        
        if index = pages.Length - 1 then
            target.PopAsync() |> ignore

            let page =
                Helpers.createViewForWidget<Page> node widget

            target.PushAsync(page) |> ignore
        else
            let temp = System.Collections.Generic.Stack<Page>()

            for i = pages.Length - 1 to index do
                temp.Push(pages.[i])
                target.PopAsync() |> ignore

            let page =
                Helpers.createViewForWidget<Page> node widget

            target.PushAsync(page, false) |> ignore

            while temp.Count > 1 do
                target.PushAsync(temp.Pop(), false)
                |> ignore
        
    let remove (node: IViewNode) (index: int) =
        let target = node.Target :?> NavigationPage
        let pages = List.ofSeq target.Pages
        
        if index > pages.Length - 1 then
            () // Do nothing, page has already been popped
        elif index = pages.Length - 1 then
            target.PopAsync() |> ignore
        else
            let temp = System.Collections.Generic.Stack<Page>()

            for i = pages.Length - 1 to index do
                temp.Push(pages.[i])
                target.PopAsync() |> ignore

            while temp.Count > 1 do
                target.PushAsync(temp.Pop(), false)
                |> ignore

    let updateNode (newValueOpt: ArraySlice<Widget> voption) (node: IViewNode) =
        let navigationPage = node.Target :?> NavigationPage
        navigationPage.PopToRootAsync(false) |> ignore

        match newValueOpt with
        | ValueNone -> ()
        | ValueSome widgets ->
            for widget in ArraySlice.toSpan widgets do
                let page =
                    Helpers.createViewForWidget<Page> node widget

                navigationPage.PushAsync(page) |> ignore

module NavigationPage =
    let WidgetKey = Widgets.register<NavigationPage> ()

    let Pages =
        Attributes.defineCustomWidgetCollection
            "NavigationPage_Pages"
            NavigationPageUpdaters.getItemNode
            NavigationPageUpdaters.insert
            NavigationPageUpdaters.replace
            NavigationPageUpdaters.remove
            NavigationPageUpdaters.updateNode

    let BarBackgroundColor =
        Attributes.defineBindable<Color> NavigationPage.BarBackgroundColorProperty

    let BarTextColor =
        Attributes.defineBindable<Color> NavigationPage.BarTextColorProperty

    let HasNavigationBar =
        Attributes.defineBindable<bool> NavigationPage.HasNavigationBarProperty

    let HasBackButton =
        Attributes.defineBindable<bool> NavigationPage.HasBackButtonProperty

    let Popped =
        Attributes.defineEventWithArgs<NavigationPage, NavigationEventArgs>
            "NavigationPage_Popped"
            (fun target -> target.Popped)

[<AutoOpen>]
module NavigationPageBuilders =
    type Fabulous.XamarinForms.View with
        static member inline NavigationPage<'msg>() =
            CollectionBuilder<'msg, INavigationPage, IPage>(NavigationPage.WidgetKey, NavigationPage.Pages)

[<Extension>]
type NavigationPageModifiers =
    [<Extension>]
    static member inline barBackgroundColor(this: WidgetBuilder<'msg, #INavigationPage>, value: Color) =
        this.AddScalar(NavigationPage.BarBackgroundColor.WithValue(value))

    [<Extension>]
    static member inline barTextColor(this: WidgetBuilder<'msg, #INavigationPage>, value: Color) =
        this.AddScalar(NavigationPage.BarTextColor.WithValue(value))

    [<Extension>]
    static member inline popped(this: WidgetBuilder<'msg, #INavigationPage>, value: 'msg) =
        this.AddEvent(NavigationPage.Popped.WithValue(fun _ -> box value))

[<Extension>]
type NavigationPageAttachedModifiers =
    [<Extension>]
    static member inline hasNavigationBar(this: WidgetBuilder<'msg, #IPage>, value: bool) =
        this.AddScalar(NavigationPage.HasNavigationBar.WithValue(value))

    [<Extension>]
    static member inline hasBackButton(this: WidgetBuilder<'msg, #IPage>, value: bool) =
        this.AddScalar(NavigationPage.HasBackButton.WithValue(value))
