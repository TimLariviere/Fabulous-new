{% include_relative _header.md %}

{% include_relative contents.md %}

BoxView
--------
##### `topic last updated: v1.0 - 24.04.2021 - 11:47pm`

### [back to interface objects](view-interface-objects.html#interface-objects)

<br />

### Basic example


```fsharp 
BoxView(Color.Black)
```

<img src="images/view/BoxView-adr-basic.png" width="300">

<br /> <br /> 

### Basic example with styling

```fsharp 
BoxView(Color.Black)
```


<img src="images/view/BoxView-adr-styled.png" width="300">

<br /> <br /> 

See also:

* [BoxView in Xamarin Forms](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/BoxView)
* [`Xamarin.Forms.BoxView`](https://docs.microsoft.com/en-us/dotnet/api/Xamarin.Forms.BoxView)

<br /> 

### More examples

An example `BoxView` is as follows:
```fsharp 
BoxView(Color.CornflowerBlue)
    .cornerRadius(10.)
    .horizontalOptions(LayoutOptions.Center)
```
<img src="https://user-images.githubusercontent.com/6429007/60753625-c1377b80-9fd5-11e9-91cc-eaef04a372cf.png" width="400">
