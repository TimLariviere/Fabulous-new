{% include_relative _header.md %}

{% include_relative contents.md %}

Label
--------
##### `topic last updated: v1.0 - 24.04.2021 - 11:47pm`

### [back to interface objects](view-interface-objects.html#interface-objects)

<br />

### Basic example


```fsharp 
View.Label("Label")
```

<img src="images/view/Label-adr-basic.png" width="300">

<br /> <br /> 

### Basic example with styling

```fsharp 
Label("Label")
    .horizontalOptions(style.Position)
    .verticalOptions(style.Position)
    .backgroundColor(style.ViewColor)
    .padding(style.Padding)
```


<img src="images/view/Label-adr-styled.png" width="300">

<br /> <br /> 

See also:

* [Label in Xamarin Forms](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/text/Label)
* [`Xamarin.Forms.Label`](https://docs.microsoft.com/en-us/dotnet/api/Xamarin.Forms.Label)

<br /> 

### More examples

The Label view is used for displaying text, both single and multi-line. Labels can have text decorations, colored text, and use custom fonts (families, sizes, and options).

```fsharp 
Label("this is a label")
```
