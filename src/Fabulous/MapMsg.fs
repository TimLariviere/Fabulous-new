namespace Fabulous

module MapMsg =
    let MapMsg =
        Attributes.defineCustomScalar<obj -> obj, _, _>
            "Fabulous_MapMsg"
            id
            id
            (fun _ _ -> false)
            (fun value node ->
                let mappedNode = node :?> IViewNodeWithEvents
                match value with
                | ValueNone -> mappedNode.MapMsg <- ValueNone
                | ValueSome fn -> mappedNode.MapMsg <- ValueSome fn)
