namespace Fabulous

type IMappedViewNode =
    inherit IViewNode
    abstract member MapMsg: (obj -> obj) voption with get, set

module MapMsg =
    let MapMsg =
        Attributes.defineCustomScalar<obj -> obj, _, _, obj>
            "Fabulous_MapMsg"
            null
            id
            id
            (fun _ _ -> false)
            (fun value node _ ->
                let mappedNode = node :?> IMappedViewNode
                match value with
                | ValueNone -> mappedNode.MapMsg <- ValueNone
                | ValueSome fn -> mappedNode.MapMsg <- ValueSome fn)
