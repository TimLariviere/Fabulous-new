{% include_relative _header.md %}

{% include_relative contents.md %}

Multi-page Applications and Navigation
-------
##### (topic last updated: pending)
<br /> 

Multiple pages are generated as part of the overall view. Five multi-page navigation models are shown in the `AllControls` sample:

* NavigationPage using push/pop
* NavigationPage Toolbar
* TabbedPage
* CarouselPage
* MasterDetail

### NavigationPage using push/pop

The basic principles of implementing push/pop navigation are as follows:

1. Keep some information in your model indicating the page stack (e.g. a list of page identifiers or page models)
2. Return the current visual page stack in the `pages` property of `NavigationPage`.
3. Set `HasNavigationBar` and `HasBackButton` on each sub-page according to your desire
4. Dispatch messages in order to navigate, where the corresponding `update` adjusts the page stack in the model
5. Utilize `popped` event to handle page removal

```fsharp
let view model =
    NavigationPage(pages=
        [ for page in model.PageStack do
            match page with
            | "Home" ->
                yield ContentPage(...).hasNavigationBar(true).hasBackButton(true)
            | "PageA" ->
                yield ContentPage(...).hasNavigationBar(true).hasBackButton(true)
            | "PageB" ->
                yield ContentPage(...).hasNavigationBar(true).hasBackButton(true)
        ],
        popped = NavigationPopped)
```

### NavigationPage Toolbar

A toolbar can be added to a navigation page using `.ToolbarItems([ ... ])` as follows:

```fsharp
let view model =
    ...
    View.NavigationPage([
        ContentPage(...)
            .ToolbarItems([ToolbarItem("About", ShowAbout true))
    ] )
```

### Example: Modal pages by pushing an extra page

A modal page can be achieved by yielding an additional page in the NavigationPage. For example, here is an "About" page example:

```fsharp
type Model =
    { ShowAbout: bool
      ...
    }

type Msg =
    | ...
    | ShowAbout of bool

let view model =
    ...
    let rootPage =
        ContentPage(
            "Root Page",
            Button("About",ShowAbout true)
        )

    let modalPage =
        ContentPage("About",
            VerticalStackLayout([
                Label("Fabulous!")
                Button(text = "Continue", ShowAbout false))
            ]))

    NavigationPage(
        [ yield rootPage
          if model.ShowAbout then
              yield modalPage
        ])
```

### TabbedPage navigation

Return a `TabbedPage` from your view:

```fsharp
let view model =
    TabbedPage([ ... ])
```

### CarouselPage navigation

Return a `CarouselPage` from your view:

```fsharp
let view model =
    CarouselPage([ ... ])
```

### MasterDetail Page navigation

Return a `FlyoutPage` from your view:

```fsharp
let view model =
    FlyoutPage(
        flyout = ContentPage("flyoutPage", ...), // 'title' is needed for the flyout page
        detail = ContentPage(...)        
    )
```

See also

* [The `AllControls` sample](https://github.com/fsprojects/Fabulous/blob/v1.0/Fabulous.XamarinForms/samples/AllControls/AllControls/App.fs)
