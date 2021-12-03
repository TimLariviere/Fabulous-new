{% include_relative _header.md %}

{% include_relative contents.md %}

TimePicker
--------
##### `topic last updated: v1.0 - 24.04.2021 - 11:47pm`

### [back to interface objects](view-interface-objects.html#interface-objects)

<br />

### Basic example


```fsharp 
TimePicker()
```

<img src="images/view/TimePicker-adr-basic.png" width="300">

<br /> <br /> 

### Basic example with styling

```fsharp 
TimePicker()
    .horizontalOptions(style.Position)
     .verticalOptions(style.Position)
     .backgroundColor(style.ViewColor)
```


<img src="images/view/TimePicker-adr-styled.png" width="300">

<br /> <br /> 

See also:

* [`Xamarin.Forms.TimePicker`](https://docs.microsoft.com/en-us/dotnet/api/Xamarin.Forms.TimePicker)

<br /> 

### More examples

### TimePicker
```fsharp 
TimePicker(TimeSpan (12, 22, 0))
```
