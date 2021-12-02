{% include_relative _header.md %}

{% include_relative contents-view.md %}

Views
------
##### `topic last updated: v 1.0  - 04.04.2021 - 02:29pm,`

The `view` function is a function returning your view elements based on the current model. For example:

```fsharp
let view model =
    ContentPage("Pocket Piggy Bank",
        Label(text = sprintf "Hello world!")
    )
```

The view function is normal F# code that returns elements created using the `View.*` method calls.

View functions are particularly useful when the existence, characteristics and layout of the view depends on information
in the model. Differential update is used to efficiently update the Xamarin.Forms display based on the previous
and current view descriptions.

Here is a larger example:
```fsharp 
type Model =
    { Balance : decimal
        CurrencySymbol : string
        User: string option }

type Msg =
    | Spend of decimal
    | Add of decimal
    | Login of string option

let init() = 
    { Balance = 2m
        CurrencySymbol = "$"
        User = Some "user"
    }, Cmd.none    

let update msg model =
    match msg with
    | Spend x -> { model with Balance = model.Balance - x }, Cmd.none
    | Add x -> { model with Balance = model.Balance + x }, Cmd.none
    | Login user -> { model with User = user }, Cmd.none

let view model =
    ContentPage("Pocket Piggy Bank",
        VerticalStackLayout([
            match model.User with
            | Some user ->
                yield Label(sprintf "Logged in as : %s" user)
                yield Label(sprintf "Balance: %s%.2f" model.CurrencySymbol model.Balance)
                yield Button("Withdraw", (Spend 10.0m)).isEnabled(model.Balance > 10m)
                yield Button("Deposit", (Add 10.0m))
                yield Button("Logout", (Login None))
            | None ->
                Button("Login", Login (Some "user"))
            ]))
```
The four main control groups used to create the user interface of a Xamarin.Forms application are: 
* [Pages](view-pages.html)
* [Layouts](view-layouts.html)
* [Interface objects](view-interface-objects.html)
* [Cells](view-cells.html)

See also:

* [Views and Performance](view-a-performance.html)
* [Styling](view-a-styling.html)
