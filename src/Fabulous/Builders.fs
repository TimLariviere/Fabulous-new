namespace Fabulous

open System.ComponentModel
open Fabulous
open Fabulous.StackList
open Microsoft.FSharp.Core


type AttributesBundle =
    (struct (StackList<ScalarAttribute> * EventAttribute [] voption * WidgetAttribute [] voption * WidgetCollectionAttribute [] voption))

[<Struct; NoComparison; NoEquality>]
type WidgetBuilder<'msg, 'marker> =
    struct
        val Definition: WidgetDefinition
        val Attributes: AttributesBundle

        new(definition: WidgetDefinition, attributes: AttributesBundle) =
            { Definition = definition
              Attributes = attributes }

        new(definition: WidgetDefinition, scalar: ScalarAttribute) =
            { Definition = definition
              Attributes = AttributesBundle(StackList.one scalar, ValueNone, ValueNone, ValueNone) }

        new(definition: WidgetDefinition, scalarA: ScalarAttribute, scalarB: ScalarAttribute) =
            { Definition = definition
              Attributes = AttributesBundle(StackList.two (scalarA, scalarB), ValueNone, ValueNone, ValueNone) }

        new(definition: WidgetDefinition, scalar1: ScalarAttribute, scalar2: ScalarAttribute, scalar3: ScalarAttribute) =
            { Definition = definition
              Attributes = AttributesBundle(StackList.three (scalar1, scalar2, scalar3), ValueNone, ValueNone, ValueNone) }

        [<EditorBrowsable(EditorBrowsableState.Never)>]
        member x.Compile() : Widget =
            let struct (scalarAttributes, eventAttributes, widgetAttributes, widgetCollectionAttributes) = x.Attributes

            { Definition = x.Definition
              Data =
                  { ScalarAttributes =
                      match StackList.length &scalarAttributes with
                      | 0us -> ValueNone
                      | _ -> ValueSome(Array.sortInPlace (fun a -> a.Definition.Key) (StackList.toArray &scalarAttributes))
                      
                    EventAttributes = ValueOption.map (Array.sortInPlace (fun a -> a.Definition.Key)) eventAttributes
                      
                    WidgetAttributes = ValueOption.map (Array.sortInPlace (fun a -> a.Definition.Key)) widgetAttributes
                    
                    WidgetCollectionAttributes =
                        widgetCollectionAttributes
                        |> ValueOption.map (Array.sortInPlace (fun a -> a.Definition.Key)) } }

        [<EditorBrowsable(EditorBrowsableState.Never)>]
        member inline x.AddScalar(attr: ScalarAttribute) =
            let struct (scalarAttributes, eventAttributes, widgetAttributes, widgetCollectionAttributes) = x.Attributes

            WidgetBuilder<'msg, 'marker>(
                x.Definition,
                struct (StackList.add (&scalarAttributes, attr), eventAttributes, widgetAttributes, widgetCollectionAttributes)
            )
            
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        member inline x.AddEvent(attr: EventAttribute) =
            let struct (scalarAttributes, eventAttributes, widgetAttributes, widgetCollectionAttributes) = x.Attributes
            let attribs = eventAttributes

            let res =
                match attribs with
                | ValueNone -> [| attr |]
                | ValueSome attribs ->
                    let attribs2 = Array.zeroCreate (attribs.Length + 1)
                    Array.blit attribs 0 attribs2 0 attribs.Length
                    attribs2.[attribs.Length] <- attr
                    attribs2

            WidgetBuilder<'msg, 'marker>(
                x.Definition,
                struct (scalarAttributes, ValueSome res, widgetAttributes, widgetCollectionAttributes)
            )

        [<EditorBrowsable(EditorBrowsableState.Never)>]
        member x.AddWidget(attr: WidgetAttribute) =
            let struct (scalarAttributes, eventAttributes, widgetAttributes, widgetCollectionAttributes) = x.Attributes
            let attribs = widgetAttributes

            let res =
                match attribs with
                | ValueNone -> [| attr |]
                | ValueSome attribs ->
                    let attribs2 = Array.zeroCreate (attribs.Length + 1)
                    Array.blit attribs 0 attribs2 0 attribs.Length
                    attribs2.[attribs.Length] <- attr
                    attribs2

            WidgetBuilder<'msg, 'marker>(x.Definition, struct (scalarAttributes, eventAttributes, ValueSome res, widgetCollectionAttributes))

        [<EditorBrowsable(EditorBrowsableState.Never)>]
        member x.AddWidgetCollection(attr: WidgetCollectionAttribute) =
            let struct (scalarAttributes, eventAttributes, widgetAttributes, widgetCollectionAttributes) = x.Attributes
            let attribs = widgetCollectionAttributes

            let res =
                match attribs with
                | ValueNone -> [| attr |]
                | ValueSome attribs ->
                    let attribs2 = Array.zeroCreate (attribs.Length + 1)
                    Array.blit attribs 0 attribs2 0 attribs.Length
                    attribs2.[attribs.Length] <- attr
                    attribs2

            WidgetBuilder<'msg, 'marker>(x.Definition, struct (scalarAttributes, eventAttributes, widgetAttributes, ValueSome res))
    end



[<Struct>]
type Content<'msg> = { Widgets: MutStackArray1.T<Widget> }

[<Struct; NoComparison; NoEquality>]
type CollectionBuilder<'msg, 'marker, 'itemMarker> =
    struct
        val Definition: WidgetDefinition
        val Scalars: StackList<ScalarAttribute>
        val Attr: WidgetCollectionAttributeDefinition

        new(definition: WidgetDefinition, scalars: StackList<ScalarAttribute>, attr: WidgetCollectionAttributeDefinition) =
            { Definition = definition
              Scalars = scalars
              Attr = attr }

        new(definition: WidgetDefinition, attr: WidgetCollectionAttributeDefinition) =
            { Definition = definition
              Scalars = StackList.empty ()
              Attr = attr }

        new(definition: WidgetDefinition, attr: WidgetCollectionAttributeDefinition, scalar: ScalarAttribute) =
            { Definition = definition
              Scalars = StackList.one scalar
              Attr = attr }

        new(definition: WidgetDefinition,
            attr: WidgetCollectionAttributeDefinition,
            scalarA: ScalarAttribute,
            scalarB: ScalarAttribute) =
            { Definition = definition
              Scalars = StackList.two (scalarA, scalarB)
              Attr = attr }

        member inline x.Run(c: Content<'msg>) =
            let attrValue =
                match MutStackArray1.toArraySlice &c.Widgets with
                | ValueNone -> ArraySlice.emptyWithNull ()
                | ValueSome slice -> slice

            WidgetBuilder<'msg, 'marker>(
                x.Definition,
                AttributesBundle(x.Scalars, ValueNone, ValueNone, ValueSome [| x.Attr.WithValue(attrValue) |])
            )

        member inline _.Combine(a: Content<'msg>, b: Content<'msg>) : Content<'msg> =
            let res =
                MutStackArray1.combineMut (&a.Widgets, b.Widgets)

            { Widgets = res }

        member inline _.Zero() : Content<'msg> = { Widgets = MutStackArray1.Empty }

        member inline _.Delay([<InlineIfLambda>] f) : Content<'msg> = f ()

        member inline x.For<'t>(sequence: 't seq, f: 't -> Content<'msg>) : Content<'msg> =
            let mutable res: Content<'msg> = x.Zero()

            // this is essentially Fold, not sure what is more optimal
            // handwritten version of via Seq.Fold
            for t in sequence do
                res <- x.Combine(res, f t)

            res
    end

[<Struct>]
type AttributeCollectionBuilder<'msg, 'marker, 'itemMarker> =
    struct
        val Widget: WidgetBuilder<'msg, 'marker>
        val Attr: WidgetCollectionAttributeDefinition

        new(widget: WidgetBuilder<'msg, 'marker>, attr: WidgetCollectionAttributeDefinition) =
            { Widget = widget; Attr = attr }

        member inline x.Run(c: Content<'msg>) =
            let attrValue =
                match MutStackArray1.toArraySlice &c.Widgets with
                | ValueNone -> ArraySlice.emptyWithNull ()
                | ValueSome slice -> slice

            x.Widget.AddWidgetCollection(x.Attr.WithValue(attrValue))

        member inline _.Combine(a: Content<'msg>, b: Content<'msg>) : Content<'msg> =
            { Widgets = MutStackArray1.combineMut (&a.Widgets, b.Widgets) }

        member inline _.Zero() : Content<'msg> = { Widgets = MutStackArray1.Empty }

        member inline _.Delay([<InlineIfLambda>] f) : Content<'msg> = f ()

        member inline x.For<'t>(sequence: 't seq, [<InlineIfLambda>] f: 't -> Content<'msg>) : Content<'msg> =
            let mutable res: Content<'msg> = x.Zero()

            // this is essentially Fold, not sure what is more optimal
            // handwritten version of via Seq.Fold
            for t in sequence do
                res <- x.Combine(res, f t)

            res
    end
