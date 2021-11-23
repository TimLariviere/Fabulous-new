namespace Fabulous.XamarinForms.Samples.CounterApp.Android

open Android.App
open Android.Content.PM
open Android.Runtime
open Xamarin.Forms.Platform.Android
open Fabulous.XamarinForms.Samples.CounterApp

[<Activity (Label = "CounterApp.Android", MainLauncher = true, Theme = "@style/MainTheme", Icon = "@mipmap/icon", ConfigurationChanges = (ConfigChanges.ScreenSize ||| ConfigChanges.Orientation ||| ConfigChanges.UiMode ||| ConfigChanges.ScreenLayout ||| ConfigChanges.SmallestScreenSize))>]
type MainActivity () =
    inherit FormsAppCompatActivity ()
    
    override this.OnCreate (bundle) =
        base.OnCreate(bundle)
        Xamarin.Essentials.Platform.Init(this, bundle)
        Xamarin.Forms.Forms.Init(this, bundle)
        this.LoadApplication(App.application)
            
    override this.OnRequestPermissionsResult(requestCode: int, permissions: string[], [<GeneratedEnum>] grantResults: Android.Content.PM.Permission[]) =
        Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults)
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults)