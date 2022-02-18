namespace Fabulous

module MapMsg =
    let MapMsg =
        Attributes.defineScalarWithConverter<obj -> obj, _, _>
            "Fabulous_MapMsg"
            id
            id
            ScalarAttributeComparers.noCompare
            (fun _ value node ->
                match value with
                | ValueNone -> node.MapMsg <- None
                | ValueSome fn -> node.MapMsg <- Some fn)
