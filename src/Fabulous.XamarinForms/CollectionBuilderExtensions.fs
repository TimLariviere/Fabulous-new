namespace Fabulous.XamarinForms

open System.Runtime.CompilerServices
open Fabulous

[<Extension>]
type CollectionBuilderExtensions =
    [<Extension>]
    static member inline Yield<'msg, 'marker, 'itemType when 'itemType :> IPage>
        (
            _: CollectionBuilder<'msg, 'marker, IPage>,
            x: WidgetBuilder<'msg, 'itemType>
        ) : Content<'msg> =
        { Widgets = MutStackArray1.One(x.Compile()) }

    [<Extension>]
    static member inline Yield<'msg, 'marker, 'itemType when 'itemType :> IView>
        (
            _: CollectionBuilder<'msg, 'marker, IView>,
            x: WidgetBuilder<'msg, 'itemType>
        ) : Content<'msg> =
        { Widgets = MutStackArray1.One(x.Compile()) }

    [<Extension>]
    static member inline Yield<'msg, 'marker, 'itemType when 'itemType :> IGestureRecognizer>
        (
            _: AttributeCollectionBuilder<'msg, 'marker, IGestureRecognizer>,
            x: WidgetBuilder<'msg, 'itemType>
        ) : Content<'msg> =
        { Widgets = MutStackArray1.One(x.Compile()) }

    [<Extension>]
    static member inline Yield<'msg, 'marker, 'itemType when 'itemType :> IToolbarItem>
        (
            _: AttributeCollectionBuilder<'msg, 'marker, IToolbarItem>,
            x: WidgetBuilder<'msg, 'itemType>
        ) : Content<'msg> =
        { Widgets = MutStackArray1.One(x.Compile()) }



    [<Extension>]
    static member inline Yield<'msg, 'marker, 'itemType when 'itemType :> IPage>
        (
            _: CollectionBuilder<'msg, 'marker, IPage>,
            x: WidgetBuilder<'msg, Memo.Memoized<'itemType>>
        ) : Content<'msg> =
        { Widgets = MutStackArray1.One(x.Compile()) }

    [<Extension>]
    static member inline Yield<'msg, 'marker, 'itemType when 'itemType :> IView>
        (
            _: CollectionBuilder<'msg, 'marker, IView>,
            x: WidgetBuilder<'msg, Memo.Memoized<'itemType>>
        ) : Content<'msg> =
        { Widgets = MutStackArray1.One(x.Compile()) }

    [<Extension>]
    static member inline Yield<'msg, 'marker, 'itemType when 'itemType :> IGestureRecognizer>
        (
            _: AttributeCollectionBuilder<'msg, 'marker, IGestureRecognizer>,
            x: WidgetBuilder<'msg, Memo.Memoized<'itemType>>
        ) : Content<'msg> =
        { Widgets = MutStackArray1.One(x.Compile()) }

    [<Extension>]
    static member inline Yield<'msg, 'marker, 'itemType when 'itemType :> IToolbarItem>
        (
            _: AttributeCollectionBuilder<'msg, 'marker, IToolbarItem>,
            x: WidgetBuilder<'msg, Memo.Memoized<'itemType>>
        ) : Content<'msg> =
        { Widgets = MutStackArray1.One(x.Compile()) }
