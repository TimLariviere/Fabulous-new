namespace Fabulous.XamarinForms.Samples.FabulousWeather.iOS

open System
open UIKit
open Foundation
open Xamarin.Essentials
open Xamarin.Forms
open Xamarin.Forms.Platform.iOS
open Fabulous.XamarinForms.Samples.FabulousWeather
open Fabulous.XamarinForms

[<Register ("AppDelegate")>]
type AppDelegate () =
    inherit FormsApplicationDelegate ()

    override this.FinishedLaunching (app, options) =
        UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, true)
        Forms.Init()
        this.LoadApplication(App.application)
        base.FinishedLaunching(app, options)

module Main =
    [<EntryPoint>]
    let main args =
        UIApplication.Main(args, null, typeof<AppDelegate>)
        0