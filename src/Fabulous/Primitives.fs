namespace Fabulous

open Fabulous

type AttributeKey = int
type WidgetKey = int
type StateKey = int
type ViewAdapterKey = int

[<Struct>]
type ScalarAttribute =
    { Key: AttributeKey
#if DEBUG
      DebugName: string
#endif
      Value: obj }

and [<Struct>] WidgetAttribute =
    { Key: AttributeKey
#if DEBUG
      DebugName: string
#endif
      Value: Widget }

and [<Struct>] WidgetCollectionAttribute =
    { Key: AttributeKey
#if DEBUG
      DebugName: string
#endif
      Value: ArraySlice<Widget> }

and [<Struct>] Widget =
    { Key: WidgetKey
#if DEBUG
      DebugName: string
#endif
      ScalarAttributes: ScalarAttribute [] voption
      WidgetAttributes: WidgetAttribute [] voption
      WidgetCollectionAttributes: WidgetCollectionAttribute [] voption }
