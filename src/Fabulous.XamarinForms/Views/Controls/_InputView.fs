namespace Fabulous.XamarinForms

open Fabulous
open Xamarin.Forms

type IInputView =
    inherit IView

module InputView =
    let TextChanged =
        Attributes.defineEventWithArgs<InputView, TextChangedEventArgs>
            "InputView_TextChanged"
            (fun target -> target.TextChanged)
