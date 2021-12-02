{% include_relative _header.md %}

{% include_relative contents.md %}

Using Maps
------
##### (topic last updated: pending)
<br /> 

The nuget [`Fabulous.XamarinForms.Maps`](https://www.nuget.org/packages/Fabulous.XamarinForms.Maps) implements an [extension](view-a-extensions.html) for the types [Map](https://docs.microsoft.com/dotnet/api/xamarin.forms.maps.map?view=xamarin-forms]) and
[Pin](https://docs.microsoft.com/en-gb/dotnet/api/xamarin.forms.maps.pin?view=xamarin-forms).

[![Maps example from Microsoft](https://user-images.githubusercontent.com/7204669/42186154-60437d42-7e43-11e8-805b-7200282f3b98.png)](https://user-images.githubusercontent.com/7204669/42186154-60437d42-7e43-11e8-805b-7200282f3b98.png)

To use `Fabulous.XamarinForms.Maps`, you must

1. Add a reference to `Fabulous.XamarinForms.Maps` across your whole solution.
2. Additionally [follow the instructions to initialize Xamarin.Forms Maps](https://docs.microsoft.com/xamarin/xamarin-forms/user-interface/map#Maps_Initialization). For example, on Android you must enable Google Play servies, add a call to `Xamarin.FormsMaps.Init(this, bundle)` to `MainActivity.fs` and add both and API key and `uses-permission` to `AndroidManifest.xml`.

After these steps you can use maps in your `view` function as follows:

```fsharp
open Xamarin.Forms.Maps
open Fabulous.XamarinForms

Map()
 .hasZoomEnabled(true)
 .hasScrollEnabled(true)
```

Next, a map with requested region around Timbuktu:

```fsharp
let timbuktu = Position(16.7666, -3.0026)
Map(MapSpan.FromCenterAndRadius(timbuktu, Distance.FromKilometers(1.0)))
    .hasZoomEnabled(true)
    .hasScrollEnabled(true)
```

Next, a map with two pins for Paris and London:

```fsharp
let paris = Position(48.8566, 2.3522)
let london = Position(51.5074, -0.1278)
let calais = Position(50.9513, 1.8587)
Map(MapSpan.FromCenterAndRadius(calais, Distance.FromKilometers(300.0))
    [ Pin(paris, label="Paris", pinType = PinType.Place)
      Pin(london, label="London", pinType = PinType.Place) ])
        .hasZoomEnabled(true)
        .hasScrollEnabled(true)
```

See also:

* [Interface Objects](view-interface-objects.html).
* [Working with Maps](https://developer.xamarin.com/samples/xamarin-forms/WorkingWithMaps).
* [Source for the Maps extension](https://github.com/fsprojects/Fabulous/blob/v1.0/Fabulous.XamarinForms/extensions/Maps/Xamarin.Forms.Maps.fs)
* [Defining Extensions](view-a-extensions.html)
