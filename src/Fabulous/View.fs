namespace Fabulous

module ViewHelpers =
    let canReuseView (prevWidget: Widget) (currWidget: Widget) =
        let prevKey = prevWidget.Definition.Key

        if not (prevKey = currWidget.Definition.Key) then
            false
        else if (prevKey = Memo.widgetKey) then
            Memo.canReuseMemoizedWidget prevWidget.Data currWidget.Data
        else
            true

module View =
    let lazy'<'msg, 'key, 'marker when 'key: equality>
        (fn: 'key -> WidgetBuilder<'msg, 'marker>)
        (key: 'key)
        : WidgetBuilder<'msg, Memo.Memoized<'marker>> =

        let memo: Memo.MemoData =
            { KeyData = box key
              KeyComparer = fun (prev: obj) (next: obj) -> unbox<'key> prev = unbox<'key> next
              CreateWidget = fun k -> (fn (unbox<'key> k)).Compile()
              KeyType = typeof<'key>
              MarkerType = typeof<'marker> }

        WidgetBuilder<'msg, Memo.Memoized<'marker>>(Memo.MemoWidgetDefinition, Memo.MemoAttribute.WithValue(memo))

    let inline map (fn: 'oldMsg -> 'newMsg) (x: WidgetBuilder<'oldMsg, 'marker>) : WidgetBuilder<'newMsg, 'marker> =
        let fnWithBoxing =
            fun (msg: obj) -> msg |> unbox<'oldMsg> |> fn |> box

        let builder =
            x.AddScalar(MapMsg.MapMsg.WithValue fnWithBoxing)

        WidgetBuilder<'newMsg, 'marker>(builder.Definition, builder.Attributes)

    let inline lazyMap (mapFn: 'oldMsg -> 'newMsg) (viewFn: 'key -> WidgetBuilder<'oldMsg, 'marker>) (model: 'key) =
        lazy' (viewFn >> map mapFn) model
