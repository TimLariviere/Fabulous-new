{% include_relative _header.md %}

{% include_relative contents.md %}

Switch
--------
##### `topic last updated: v1.0 - 24.04.2021 - 11:47pm`

### [back to interface objects](view-interface-objects.html#interface-objects)

<br />

### Basic example


```fsharp 
Switch()
```

<img src="images/view/Switch-adr-basic.png" width="300">

<br /> <br /> 

### Basic example with styling

```fsharp 
Switch(false, TimerToggled)
    .horizontalOptions(style.Position)
    .verticalOptions(style.Position)
    .backgroundColor(style.ViewColor)
```


<img src="images/view/Switch-adr-styled.png" width="300">

<br /> <br /> 

See also:

* [`Xamarin.Forms.Switch`](https://docs.microsoft.com/en-us/dotnet/api/Xamarin.Forms.Switch)

<br /> 

### More examples

`Switch` is a horizontal toggle button that can be manipulated by the user to toggle between on and off states, which are represented by a boolean value. 

```fsharp 
Switch(false, SwitchToggled)
```
