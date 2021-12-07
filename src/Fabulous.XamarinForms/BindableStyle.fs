namespace Fabulous.XamarinForms

[<AbstractClass>]
type BindableStyle () =
    inherit Xamarin.Forms.BindableObject()
    abstract member FormsStyle: Xamarin.Forms.Style
    
type BindableStyle<'T> () =
    inherit BindableStyle()
    let _formsStyle = Xamarin.Forms.Style(typeof<'T>)
    override this.FormsStyle = _formsStyle