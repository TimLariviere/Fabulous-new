{% include_relative _header.md %}

{% include_relative contents.md %}

Path
--------
##### `topic last updated: v1.0 - 24.04.2021 - 11:47pm`

### [back to interface objects](view-interface-objects.html#interface-objects)

<br />

### Basic example


```fsharp 
Path(stroke = View.SolidColorBrush(Color.Black), data = Content.fromString "M 10,100 C 100,0 200,200 300,100")
```

<img src="images/view/Path-adr-basic.png" width="300">

<br /> <br /> 

### Basic example with styling

```fsharp 
Path
    (                           
        stroke = View.SolidColorBrush(Color.Black),
        data = Content.fromString "M 10,100 C 100,0 200,200 300,100"
    ).horizontalOptions(style.Position)
     .verticalOptions(style.Position)
     .backgroundColor(style.ViewColor)
```


<img src="images/view/Path-adr-styled.png" width="300">

<br /> <br /> 

See also:

* [Path in Xamarin Forms](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/shapes/Path)
* [`Xamarin.Forms.Path`](https://docs.microsoft.com/en-us/dotnet/api/xamarin.forms.shapes.polygon?view=xamarin-forms)

<br /> 

### More examples

 `Path` can be used to draw curves and complex shapes. These curves and shapes are often described using Geometry objects. 

```fsharp 
Label(text = "Path")
Path(
    stroke = View.SolidColorBrush(Color.Black),
    aspect = Stretch.Uniform,
    horizontalOptions = LayoutOptions.Center,
    data = Content.fromString "M 10,50 L 200,70"
)

Label(text = "Cubic Bezier Path")
Path(
    stroke = View.SolidColorBrush(Color.Black),
    aspect = Stretch.Uniform,
    horizontalOptions = LayoutOptions.Center,
    data = Content.fromString "M 10,100 C 100,0 200,200 300,100"
)
```
