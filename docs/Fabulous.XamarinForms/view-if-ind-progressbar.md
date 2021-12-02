{% include_relative _header.md %}

{% include_relative contents.md %}

ProgressBar
--------
##### `topic last updated: v1.0 - 24.04.2021 - 11:47pm`

### [back to interface objects](view-interface-objects.html#interface-objects)

<br />

The Progress Bar represents progress as a horizontal bar, that is filled to a percentage, represented by a float value. 

<br /> 

### Basic example


```fsharp 
ProgressBar(0.5)
```

<img src="images/view/ProgressBar-adr-basic.png" width="300">

<br /> <br /> 

### Basic example with styling

```fsharp 
View.ProgressBar(0.5)
    .horizontalOptions(style.Position)
    .verticalOptions(style.Position)
    .backgroundColor(style.LayoutColor)
```


<img src="images/view/ProgressBar-adr-styled.png" width="300">

<br /> <br /> 

See also:

* [ProgressBar in Xamarin Forms](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/ProgressBar)
* [`Xamarin.Forms.ProgressBar`](https://docs.microsoft.com/en-us/dotnet/api/Xamarin.Forms.ProgressBar)

<br /> 

### More examples

`to-do`
