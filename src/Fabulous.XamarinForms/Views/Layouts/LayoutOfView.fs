namespace Fabulous.XamarinForms

open Fabulous
open Xamarin.Forms

module LayoutOfView =
    let Children =
        Attributes.defineWidgetCollection<Xamarin.Forms.Layout<View>, View>
            "LayoutOfWidget_Children"
            (fun target -> target.Children)
