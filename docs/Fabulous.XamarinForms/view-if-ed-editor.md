{% include_relative _header.md %}

{% include_relative contents.md %}

Editor
--------
##### `topic last updated: v1.0 - 24.04.2021 - 11:47pm`

### [back to interface objects](view-interface-objects.html#interface-objects)

<br />

### Basic example


```fsharp 
Editor("Editor")
```

<img src="images/view/Editor-adr-basic.png" width="300">

<br /> <br /> 

### Basic example with styling

```fsharp 
Editor("Editor")
    .horizontalOptions(style.Position)
    .verticalOptions(style.Position)
    .backgroundColor(style.LayoutColor)
```


<img src="images/view/Editor-adr-styled.png" width="300">

<br /> <br /> 

See also:

* [Editor in Xamarin Forms](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/text/Editor)
* [`Xamarin.Forms.Editor`](https://docs.microsoft.com/en-us/dotnet/api/Xamarin.Forms.Editor)

<br /> 

### More examples

An example `Editor` is as follows:

```fsharp
Editor(editorText, TextChanged(args.OldTextValue, args.NewTextValue), EditorEditCompleted(text))
```

<img src="https://user-images.githubusercontent.com/52166903/60175558-d2c99800-980b-11e9-9755-860cc9a60dcf.png" width="400">
