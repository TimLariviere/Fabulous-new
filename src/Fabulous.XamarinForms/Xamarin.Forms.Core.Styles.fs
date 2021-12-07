namespace Fabulous.XamarinForms

open Fabulous.Attributes
open Fabulous.XamarinForms
open System.Runtime.CompilerServices
open Fabulous

module XFStyleAttributes =
    module VisualElement =
        let BackgroundColor = Attributes.defineStyleSetter<Xamarin.Forms.Color> Xamarin.Forms.VisualElement.BackgroundColorProperty
    
    module Label =
        let TextColor = Attributes.defineStyleSetter<Xamarin.Forms.Color> Xamarin.Forms.Label.TextColorProperty
        let FontSize = Attributes.defineStyleSetter<float> Xamarin.Forms.Label.FontSizeProperty
        
    module Button =
        let TextColor = Attributes.defineStyleSetter<Xamarin.Forms.Color> Xamarin.Forms.Button.TextColorProperty
        
type [<Struct>] LabelStyle<'msg> (attrs: AttributesBuilder) =
    static let key = Widgets.registerStyle<Xamarin.Forms.Label> ()
    member _.Builder = attrs

    static member inline Create() =
        LabelStyle<'msg>(
            AttributesBuilder([||], [||], [||])
        )

    interface IViewStyleWidgetBuilder<'msg> with
        member x.Compile() = attrs.Build(key)
        
type [<Struct>] ButtonStyle<'msg> (attrs: AttributesBuilder) =
    static let key = Widgets.registerStyle<Xamarin.Forms.Button> ()
    member _.Builder = attrs

    static member inline Create() =
        ButtonStyle<'msg>(
            AttributesBuilder([||], [||], [||])
        )

    interface IViewStyleWidgetBuilder<'msg> with
        member x.Compile() = attrs.Build(key)
        
[<Extension>]
type StyleExtensions () =
    [<Extension>]
    static member inline backgroundColor(this: #IViewStyleWidgetBuilder<_>, value: Xamarin.Forms.Color) =
        this.AddScalarAttribute(XFStyleAttributes.VisualElement.BackgroundColor.WithValue(value))
    [<Extension>]
    static member inline textColor(this: LabelStyle<_>, value: Xamarin.Forms.Color) =
        this.AddScalarAttribute(XFStyleAttributes.Label.TextColor.WithValue(value))
    [<Extension>]
    static member inline fontSize(this: LabelStyle<_>, value: float) =
        this.AddScalarAttribute(XFStyleAttributes.Label.FontSize.WithValue(value))
    [<Extension>]
    static member inline textColor(this: ButtonStyle<_>, value: Xamarin.Forms.Color) =
        this.AddScalarAttribute(XFStyleAttributes.Button.TextColor.WithValue(value))
        
[<AutoOpen>]
module Styles =
    type Fabulous.XamarinForms.View with
        static member inline LabelStyle<'msg>() = LabelStyle<'msg>.Create()
        static member inline ButtonStyle<'msg>() = ButtonStyle<'msg>.Create()
        