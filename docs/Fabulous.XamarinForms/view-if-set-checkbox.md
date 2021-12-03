{% include_relative _header.md %}

{% include_relative contents.md %}

CheckBox
--------
##### `topic last updated: v1.0 - 24.04.2021 - 11:47pm`

### [back to interface objects](view-interface-objects.html#interface-objects)

<br />

### Basic example


```fsharp 
CheckBox(true, CheckedChanged)
```

<img src="images/view/CheckBox-adr-basic.png" width="300">

<br /> <br /> 

### Basic example with styling

```fsharp 
CheckBox(true)
    .horizontalOptions(style.Position)
    .verticalOptions(style.Position)
    .backgroundColor(style.LayoutColor)
```


<img src="images/view/CheckBox-adr-styled.png" width="300">

<br /> <br /> 

See also:

* [`Xamarin.Forms.CheckBox`](https://docs.microsoft.com/en-us/dotnet/api/Xamarin.Forms.CheckBox)

<br /> 

### More examples

```fsharp 

CheckBox(true, CheckedChanged)
```
