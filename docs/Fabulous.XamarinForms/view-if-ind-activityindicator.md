{% include_relative _header.md %}

{% include_relative contents.md %}

ActivityIndicator
--------
##### `topic last updated: v1.0 - 24.04.2021 - 11:47pm`

### [back to interface objects](view-interface-objects.html#interface-objects)

<br />


### Basic example


```fsharp 
View.ActivityIndicator
(isRunning = true)
```

<img src="images/view/ActivityIndicator-adr-basic.png" width="300">

<br /> <br /> 

### Basic example with styling

```fsharp 
View.ActivityIndicator
(
    horizontalOptions = style.Position,
    verticalOptions = style.Position,
    backgroundColor = style.LayoutColor,
    isRunning = true
)
```


<img src="images/view/ActivityIndicator-adr-styled.png" width="300">

<br /> <br /> 

See also:

* [ActivityIndicator in Xamarin Forms](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/ActivityIndicator)
* [`Xamarin.Forms.ActivityIndicator`](https://docs.microsoft.com/en-us/dotnet/api/Xamarin.Forms.ActivityIndicator)

<br /> 

### More examples

A simple `ActivityIndicator` is as follows:

```fsharp
View.ActivityIndicator(isRunning = (count > 0))
```

<img src="https://user-images.githubusercontent.com/52166903/60177355-9c424c00-9810-11e9-8275-bd8c2ebcf3c8.png" width="400">