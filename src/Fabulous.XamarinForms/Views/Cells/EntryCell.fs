namespace Fabulous.XamarinForms

open System.Runtime.CompilerServices
open Fabulous
open Xamarin.Forms

type IEntryCell =
    inherit ICell

module EntryCell =
    let WidgetKey = Widgets.register<CustomEntryCell> ()

    let Text =
        Attributes.defineBindable<string> EntryCell.TextProperty

    let Label =
        Attributes.defineBindable<string> EntryCell.LabelProperty

    let LabelColor =
        Attributes.defineAppThemeBindable<Color> EntryCell.LabelColorProperty

    let Placeholder =
        Attributes.defineBindable<string> EntryCell.PlaceholderProperty

    let HorizontalTextAlignment =
        Attributes.defineBindable<TextAlignment> EntryCell.HorizontalTextAlignmentProperty

    let VerticalTextAlignment =
        Attributes.defineBindable<TextAlignment> EntryCell.VerticalTextAlignmentProperty

    let Keyboard =
        Attributes.defineBindable<Keyboard> EntryCell.KeyboardProperty

    let TextChanged =
        Attributes.defineEvent<TextChangedEventArgs>
            "EntryCell_TextChanged"
            (fun target -> (target :?> CustomEntryCell).TextChanged)

    let OnCompleted =
        Attributes.defineEventNoArg "EntryCell_Completed" (fun target -> (target :?> EntryCell).Completed)

[<AutoOpen>]
module EntryCellBuilders =

    type Fabulous.XamarinForms.View with
        static member inline EntryCell<'msg>(label: string, text: string, onTextChanged: string -> 'msg) =
            WidgetBuilder<'msg, IEntryCell>(
                EntryCell.WidgetKey,
                EntryCell.Label.WithValue(label),
                EntryCell.Text.WithValue(text),
                EntryCell.TextChanged.WithValue(fun args -> onTextChanged args.NewTextValue |> box)
            )

[<Extension>]
type EntryCellModifiers =
    /// <summary>Set the color of the label</summary>
    /// <param name="light">The color of the label in the light theme.</param>
    /// <param name="dark">The color of the label in the dark theme.</param>
    [<Extension>]
    static member inline labelColor(this: WidgetBuilder<'msg, #IEntryCell>, light: Color, ?dark: Color) =
        this.AddScalar(EntryCell.LabelColor.WithValue(AppTheme.create light dark))

    /// <summary>Set the horizontal text alignment</summary>
    /// param name="alignment">The horizontal text alignment</summary>
    [<Extension>]
    static member inline horizontalTextAlignment(this: WidgetBuilder<'msg, #IEntryCell>, alignment: TextAlignment) =
        this.AddScalar(EntryCell.HorizontalTextAlignment.WithValue(alignment))

    /// <summary>Set the vertical text alignment</summary>
    /// param name="alignment">The vertical text alignment</summary>
    [<Extension>]
    static member inline verticalTextAlignment(this: WidgetBuilder<'msg, #IEntryCell>, alignment: TextAlignment) =
        this.AddScalar(EntryCell.VerticalTextAlignment.WithValue(alignment))

    /// <summary>Set the keyboard</summary>
    /// param name="keyboard">The keyboard type</summary>
    [<Extension>]
    static member inline keyboard(this: WidgetBuilder<'msg, #IEntryCell>, keyboard: Keyboard) =
        this.AddScalar(EntryCell.Keyboard.WithValue(keyboard))

    /// <summary>Set the placeholder text</summary>
    /// param name="placeholder">The placeholder</summary>
    [<Extension>]
    static member inline placeholder(this: WidgetBuilder<'msg, #IEntryCell>, placeholder: string) =
        this.AddScalar(EntryCell.Placeholder.WithValue(placeholder))

    [<Extension>]
    static member inline onCompleted(this: WidgetBuilder<'msg, #IEntryCell>, onCompleted: 'msg) =
        this.AddScalar(EntryCell.OnCompleted.WithValue(onCompleted))

    /// <summary>Link a ViewRef to access the direct EntryCell control instance</summary>
    [<Extension>]
    static member inline reference(this: WidgetBuilder<'msg, IEntryCell>, value: ViewRef<EntryCell>) =
        this.AddScalar(ViewRefAttributes.ViewRef.WithValue(value.Unbox))
