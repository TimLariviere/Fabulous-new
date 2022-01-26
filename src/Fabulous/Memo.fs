namespace Fabulous

open System
open Fabulous

type ILazyViewNode =
    inherit IViewNode
    abstract member MemoizedWidget: Widget option with get, set
    
module Memo =
    type internal MemoData =
        {
          /// Captures data that memoization depends on
          KeyData: obj

          // comparer that remembers KeyType internally
          KeyComparer: obj -> obj -> bool

          /// wrapped untyped lambda that users provide
          CreateWidget: obj -> Widget

          /// Captures type of data that memoization depends on
          KeyType: Type

          /// Captures type of the marker memoized function produces
          MarkerType: Type }

    type Memoized<'t> = { phantom: 't }

    let inline private getMemoData (widgetData: WidgetData) : MemoData =
        match widgetData.ScalarAttributes with
        | ValueSome attrs when attrs.Length = 1 -> attrs.[0].Value :?> MemoData
        | _ -> failwith "Memo widget cannot have extra attributes"

    let internal canReuseMemoizedWidget prev next =
        (getMemoData prev).MarkerType = (getMemoData next).MarkerType

    let private compareAttributes (prev: MemoData) (next: MemoData) : bool =
        match (prev.KeyType = next.KeyType, prev.MarkerType = next.MarkerType) with
        | true, true -> next.KeyComparer next.KeyData prev.KeyData
        | _ -> false

    let private updateNode (data: MemoData voption) (node: IViewNode) _ : unit =
        match data with
        | ValueSome memoData ->
            let lazyNode = node :?> ILazyViewNode
            let memoizedWidget = memoData.CreateWidget memoData.KeyData

            let prevWidget =
                match lazyNode.MemoizedWidget with
                | Some widget -> ValueSome(widget.Data)
                | _ -> ValueNone

            lazyNode.MemoizedWidget <- Some memoizedWidget

            Reconciler.update (node :?> IViewNodeWithContext).TreeContext.CanReuseView prevWidget memoizedWidget.Data (node :?> IViewNodeWithDiff)

        | ValueNone -> ()

    let internal MemoAttribute =
        { Key = "MemoAttribute"
          AdditionalData = null
          Convert = id
          ConvertValue = id
          Compare = compareAttributes
          UpdateNode = updateNode }

    [<Literal>]
    let widgetKey = "Memo"
    
    // Memo isn't allowed in lists, TargetType will never get called,
    // so Unchecked.defaultof is an acceptable value
    let internal MemoWidgetDefinition: WidgetDefinition =
        { Key = widgetKey
          TargetType = Unchecked.defaultof<_>
          CreateView =
              fun widgetData context parentNode ->

                  let memoData = getMemoData widgetData

                  let memoizedWidget = memoData.CreateWidget memoData.KeyData
                  
                  let struct (node, view) =
                      memoizedWidget.Definition.CreateView memoizedWidget.Data context parentNode

                  // store widget that was used to produce this node
                  // to pass it to reconciler later on
                  (node :?> ILazyViewNode).MemoizedWidget <- Some memoizedWidget
                  struct (node, view) }
