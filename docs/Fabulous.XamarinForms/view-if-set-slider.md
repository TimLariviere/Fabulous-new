{% include_relative _header.md %}

{% include_relative contents.md %}

Slider
--------
##### `topic last updated: v1.0 - 24.04.2021 - 11:47pm`

### [back to interface objects](view-interface-objects.html#interface-objects)

<br />

### Basic example


```fsharp 
Slider( 5.0 )
```

<img src="images/view/Slider-adr-basic.png" width="300">

<br /> <br /> 

### Basic example with styling

```fsharp 
 Slider(min = 0., max = 10., value = float model.Step)
    .backgroundColor(style.ViewColor)
```


<img src="images/view/Slider-adr-styled.png" width="300">

<br /> <br /> 

See also:

* [`Xamarin.Forms.Slider`](https://docs.microsoft.com/en-us/dotnet/api/Xamarin.Forms.Slider)

<br /> 

### More examples

```fsharp
 Slider(min = 0., max = 10., value = float model.Step, onValueChanged = StepChanged)
```

<img src="https://user-images.githubusercontent.com/52166903/60177363-9d737900-9810-11e9-8842-aeb904e7d739.png" width="400">
