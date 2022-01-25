namespace Fabulous.XamarinForms

open System.Runtime.CompilerServices
open Fabulous
open Xamarin.Forms

type IApplication =
    inherit IElement

module Application =
    let WidgetKey = Widgets.register<Application> ()

    let MainPage =
        Attributes.defineWidget<Application, Page>
            "Application_MainPage"
            (fun target -> ViewNode.get target.MainPage)
            (fun target value -> target.MainPage <- value)

    let Resources =
        Attributes.defineScalar<Application, ResourceDictionary>
            "Application_Resources"
            (fun newValueOpt target ->
                let value =
                    match newValueOpt with
                    | ValueNone -> target.Resources
                    | ValueSome v -> v

                target.Resources <- value)

    let UserAppTheme =
        Attributes.defineScalar<Application, OSAppTheme>
            "Application_UserAppTheme"
            (fun newValueOpt target ->
                let value =
                    match newValueOpt with
                    | ValueNone -> OSAppTheme.Unspecified
                    | ValueSome v -> v

                target.UserAppTheme <- value)

    let RequestedThemeChanged =
        Attributes.defineEventWithArgs<Application, AppThemeChangedEventArgs>
            "Application_RequestedThemeChanged"
            (fun target -> target.RequestedThemeChanged)

    let ModalPopped =
        Attributes.defineEventWithArgs<Application, ModalPoppedEventArgs>
            "Application_ModalPopped"
            (fun target -> target.ModalPopped)

    let ModalPopping =
        Attributes.defineEventWithArgs<Application, ModalPoppingEventArgs>
            "Application_ModalPopping"
            (fun target -> target.ModalPopping)

    let ModalPushed =
        Attributes.defineEventWithArgs<Application, ModalPushedEventArgs>
            "Application_ModalPushed"
            (fun target -> target.ModalPushed)

    let ModalPushing =
        Attributes.defineEventWithArgs<Application, ModalPushingEventArgs>
            "Application_ModalPushing"
            (fun target -> target.ModalPushing)

[<AutoOpen>]
module ApplicationBuilders =
    type Fabulous.XamarinForms.View with
        static member inline Application<'msg, 'marker when 'marker :> IPage>(mainPage: WidgetBuilder<'msg, 'marker>) =
            WidgetHelpers.buildWidget<'msg, IApplication>
                Application.WidgetKey
                [| Application.MainPage.WithValue(mainPage.Compile()) |]

[<Extension>]
type ApplicationModifiers =
    [<Extension>]
    static member inline userAppTheme(this: WidgetBuilder<'msg, #IApplication>, value: OSAppTheme) =
        this.AddScalar(Application.UserAppTheme.WithValue(value))

    [<Extension>]
    static member inline resources(this: WidgetBuilder<'msg, #IApplication>, value: ResourceDictionary) =
        this.AddScalar(Application.Resources.WithValue(value))

    [<Extension>]
    static member inline onRequestedThemeChanged(this: WidgetBuilder<'msg, #IApplication>, fn: OSAppTheme -> 'msg) =
        this.AddEvent(Application.RequestedThemeChanged.WithValue(fun args -> fn args.RequestedTheme |> box))

    [<Extension>]
    static member inline onModalPopped(this: WidgetBuilder<'msg, #IApplication>, fn: ModalPoppedEventArgs -> 'msg) =
        this.AddEvent(Application.ModalPopped.WithValue(fn >> box))

    [<Extension>]
    static member inline onModalPopping(this: WidgetBuilder<'msg, #IApplication>, fn: ModalPoppingEventArgs -> 'msg) =
        this.AddEvent(Application.ModalPopping.WithValue(fn >> box))

    [<Extension>]
    static member inline onModalPushed(this: WidgetBuilder<'msg, #IApplication>, fn: ModalPushedEventArgs -> 'msg) =
        this.AddEvent(Application.ModalPushed.WithValue(fn >> box))

    [<Extension>]
    static member inline onModalPushing(this: WidgetBuilder<'msg, #IApplication>, fn: ModalPushingEventArgs -> 'msg) =
        this.AddEvent(Application.ModalPushing.WithValue(fn >> box))
