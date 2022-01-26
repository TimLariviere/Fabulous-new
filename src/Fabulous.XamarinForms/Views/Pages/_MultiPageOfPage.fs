namespace Fabulous.XamarinForms

open Fabulous
open Xamarin.Forms

module MultiPageOfPage =
    let Children =
        Attributes.defineWidgetCollection<MultiPage<Page>, Page> "MultiPageOfPage" (fun target -> target.Children)
