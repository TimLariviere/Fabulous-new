namespace Fabulous

open System
open Fabulous
open Fabulous.MemoryPool
open Fabulous.StackAllocatedCollections

module Reconciler =

    module private MemPool =
        let widgetChanges = Pool<WidgetChange>()
        let scalarChanges = Pool<ScalarChange>()
        let widgetColChanges = Pool<WidgetCollectionChange>()
        let widgetColItemChange = Pool<WidgetCollectionItemChange>()


    let inline mapToSlice (col: 'v []) (f: 'v -> 'u) (pool: Pool<'u>) =
        let arr = pool.allocate col.Length

        for i = 0 to col.Length - 1 do
            arr.[i] <- f col.[i]

        ArraySlice(uint16 col.Length, arr)

    let inline mapSlice (slice: ArraySlice<'v>) (f: 'v -> 'u) (pool: Pool<'u>) =
        let struct (size, _) = slice
        let len = int size
        let arr = pool.allocate len

        let source = ArraySlice.toSpan slice
        let destination = Span(arr, 0, len)

        for i = 0 to len - 1 do
            destination.[i] <- f source.[i]

        ArraySlice(size, arr)


    /// Let's imagine that we have the following situation
    /// prev = [|1,2,6,7|] note that it is sorted
    /// next = [|8,5,6,2|] unsorted
    /// In reality we have Key and Value, but let's pretend that we only care about keys for now
    ///
    /// then the desired outcome is this
    /// added = [5, 8]
    /// removed = [1, 7]
    ///
    /// Approach
    /// 1. we sort both arrays
    /// prev = [|1,2,6,7|]
    /// next = [|2,5,6,8|]
    ///
    /// 2. Starting from index 0 for both of the arrays
    ///     if prevItem < nextItem then
    ///         inc prevIndex, add prevItem to removed, goto 2
    ///     if prevItem > nextItem
    ///         inc nextIndex, add nextItem to added, goto 2
    ///     else (meaning equals)
    ///         compare values
    ///
    /// break when we reached both ends of the arrays
    let rec diffScalarAttributes
        (prev: ArraySlice<ScalarAttribute> voption)
        (next: ArraySlice<ScalarAttribute> voption)
        : ArraySlice<ScalarChange> voption =
        match (prev, next) with
        | ValueNone, ValueNone -> ValueNone

        // all were deleted
        | ValueSome prev, ValueNone ->
            let res =
                mapSlice prev ScalarChange.Removed MemPool.scalarChanges

            BuildersMemPool.scalars.recycleSlice prev
            res |> ValueSome


        | ValueNone, ValueSome next ->
            mapSlice next ScalarChange.Added MemPool.scalarChanges
            |> ValueSome

        | ValueSome prev, ValueSome next ->

            let mutable result = DiffBuilder.create()

            let mutable prevIndex = 0
            let mutable nextIndex = 0

            let struct (prevLength, prevSpan) = prev
            let struct (nextLength, nextSpan) = next

            let nextLength = int nextLength
            let prevLength = int prevLength

            while not(prevIndex >= prevLength && nextIndex >= nextLength) do
                if prevIndex = prevLength then
                    // that means we are done with the prev and only need to add next's tail to added
                    //result <- StackArray3.add(&result, (ScalarChange.Added next.[nextIndex]))
                    DiffBuilder.addOpMut &result DiffBuilder.Add (uint16 nextIndex)
                    nextIndex <- nextIndex + 1

                elif nextIndex = nextLength then
                    // that means that we are done with new items and only need prev's tail to removed
                    // result <- StackArray3.add(&result, ScalarChange.Removed prev.[prevIndex])
                    DiffBuilder.addOpMut &result DiffBuilder.Remove (uint16 prevIndex)
                    prevIndex <- prevIndex + 1

                else
                    // we haven't reached either of the ends
                    let prevAttr = prevSpan.[prevIndex]
                    let nextAttr = nextSpan.[nextIndex]

                    let prevKey = prevAttr.Key
                    let nextKey = nextAttr.Key

                    match prevKey.CompareTo nextKey with
                    | c when c < 0 ->
                        // prev key is less than next -> remove prev key
                        DiffBuilder.addOpMut &result DiffBuilder.Remove (uint16 prevIndex)
                        //                        result <- StackArray3.add(&result, ScalarChange.Removed prevAttr)
                        prevIndex <- prevIndex + 1

                    | c when c > 0 ->
                        // prev key is more than next -> add next item
                        // result <- StackArray3.add(&result, ScalarChange.Added nextAttr)
                        DiffBuilder.addOpMut &result DiffBuilder.Add (uint16 nextIndex)
                        nextIndex <- nextIndex + 1

                    | _ ->
                        // means that we are targeting the same attribute

                        let definition =
                            AttributeDefinitionStore.get prevAttr.Key :?> IScalarAttributeDefinition

                        match definition.CompareBoxed(prevAttr.Value, nextAttr.Value) with
                        // Previous and next values are identical, we don't need to do anything
                        | ScalarAttributeComparison.Identical -> ()

                        // New value completely replaces the old value
                        | ScalarAttributeComparison.Different value ->
                            DiffBuilder.addOpMut &result DiffBuilder.Change (uint16 nextIndex)

                        // move both pointers
                        prevIndex <- prevIndex + 1
                        nextIndex <- nextIndex + 1


            // TODO recycle diff memory
            let res =
                match DiffBuilder.lenght &result with
                | 0 -> ValueNone
                | _ ->
                    ValueSome(
                        DiffBuilder.toArraySlice
                            &result
                            (fun op ->
                                match op with
                                | DiffBuilder.Added i -> ScalarChange.Added nextSpan.[int i]
                                | DiffBuilder.Removed i -> ScalarChange.Removed prevSpan.[int i]
                                | DiffBuilder.Changed i -> ScalarChange.Updated nextSpan.[int i])
                            MemPool.scalarChanges
                    )

            BuildersMemPool.scalars.recycleSlice prev
            res

    and diffWidgetAttributes
        (canReuseView: Widget -> Widget -> bool)
        (prev: WidgetAttribute [] option)
        (next: WidgetAttribute [] option)
        : ArraySlice<WidgetChange> voption =

        match (prev, next) with
        | None, None -> ValueNone

        // all were deleted
        | Some prev, None ->
            mapToSlice prev WidgetChange.Removed MemPool.widgetChanges
            |> ValueSome


        | None, Some next ->
            mapToSlice next WidgetChange.Added MemPool.widgetChanges
            |> ValueSome

        | Some prev, Some next ->

            let mutable result = MutStackArray1.Empty

            let mutable prevIndex = 0
            let mutable nextIndex = 0

            let prevLength = prev.Length
            let nextLength = next.Length

            while not(prevIndex >= prevLength && nextIndex >= nextLength) do
                if prevIndex = prevLength then
                    // that means we are done with the prev and only need to add next's tail to added
                    result <- MutStackArray1.addMut(&result, WidgetChange.Added next.[nextIndex], MemPool.widgetChanges)
                    nextIndex <- nextIndex + 1

                elif nextIndex = nextLength then
                    // that means that we are done with new items and only need prev's tail to removed
                    result <-
                        MutStackArray1.addMut(&result, WidgetChange.Removed prev.[prevIndex], MemPool.widgetChanges)

                    prevIndex <- prevIndex + 1

                else
                    // we haven't reached either of the ends
                    let prevAttr = prev.[prevIndex]
                    let nextAttr = next.[nextIndex]

                    let prevKey = prevAttr.Key
                    let nextKey = nextAttr.Key
                    let prevWidget = prevAttr.Value
                    let nextWidget = nextAttr.Value

                    match prevKey.CompareTo nextKey with
                    | c when c < 0 ->
                        // prev key is less than next -> remove prev key
                        result <- MutStackArray1.addMut(&result, WidgetChange.Removed prevAttr, MemPool.widgetChanges)
                        prevIndex <- prevIndex + 1

                    | c when c > 0 ->
                        // prev key is more than next -> add next item
                        result <- MutStackArray1.addMut(&result, WidgetChange.Added nextAttr, MemPool.widgetChanges)
                        nextIndex <- nextIndex + 1

                    | _ ->
                        // means that we are targeting the same attribute

                        // move both pointers
                        prevIndex <- prevIndex + 1
                        nextIndex <- nextIndex + 1

                        let changeOpt =
                            if prevWidget = nextWidget then
                                ValueNone
                            elif canReuseView prevWidget nextWidget then
                                match diffWidget canReuseView (ValueSome prevWidget) nextWidget with
                                | ValueNone -> ValueNone
                                | ValueSome diffs -> ValueSome(WidgetChange.Updated struct (nextAttr, diffs))
                            else
                                ValueSome(WidgetChange.ReplacedBy nextAttr)

                        match changeOpt with
                        | ValueNone -> ()
                        | ValueSome change -> result <- MutStackArray1.addMut(&result, change, MemPool.widgetChanges)

            MutStackArray1.toArraySlice(&result, MemPool.widgetChanges)


    and diffWidgetCollectionAttributes
        (canReuseView: Widget -> Widget -> bool)
        (prev: WidgetCollectionAttribute [] option)
        (next: WidgetCollectionAttribute [] option)
        : ArraySlice<WidgetCollectionChange> voption =

        match (prev, next) with
        | None, None -> ValueNone

        // all were deleted
        | Some prev, None ->
            mapToSlice prev WidgetCollectionChange.Removed MemPool.widgetColChanges
            |> ValueSome

        | None, Some next ->
            mapToSlice next WidgetCollectionChange.Added MemPool.widgetColChanges
            |> ValueSome

        | Some prev, Some next ->

            let mutable result = MutStackArray1.Empty


            let mutable prevIndex = 0
            let mutable nextIndex = 0

            let prevLength = prev.Length
            let nextLength = next.Length

            while not(prevIndex >= prevLength && nextIndex >= nextLength) do
                if prevIndex = prevLength then
                    // that means we are done with the prev and only need to add next's tail to added
                    // DiffBuilder.addOpMut &result DiffBuilder.Add (uint16 nextIndex)
                    result <-
                        MutStackArray1.addMut(
                            &result,
                            WidgetCollectionChange.Added next.[nextIndex],
                            MemPool.widgetColChanges
                        )


                    nextIndex <- nextIndex + 1

                elif nextIndex = nextLength then
                    // that means that we are done with new items and only need prev's tail to removed
                    // DiffBuilder.addOpMut &result DiffBuilder.Remove (uint16 prevIndex)
                    result <-
                        MutStackArray1.addMut(
                            &result,
                            WidgetCollectionChange.Removed prev.[prevIndex],
                            MemPool.widgetColChanges
                        )


                    prevIndex <- prevIndex + 1

                else
                    // we haven't reached either of the ends
                    let prevAttr = prev.[prevIndex]
                    let nextAttr = next.[nextIndex]

                    let prevKey = prevAttr.Key
                    let nextKey = nextAttr.Key
                    let prevWidgetColl = prevAttr.Value
                    let nextWidgetColl = nextAttr.Value

                    match prevKey.CompareTo nextKey with
                    | c when c < 0 ->
                        // prev key is less than next -> remove prev key

                        result <-
                            MutStackArray1.addMut(
                                &result,
                                WidgetCollectionChange.Removed prevAttr,
                                MemPool.widgetColChanges
                            )

                        prevIndex <- prevIndex + 1

                    | c when c > 0 ->
                        // prev key is more than next -> add next item
                        result <-
                            MutStackArray1.addMut(
                                &result,
                                WidgetCollectionChange.Added nextAttr,
                                MemPool.widgetColChanges
                            )

                        nextIndex <- nextIndex + 1

                    | _ ->
                        // means that we are targeting the same attribute

                        // move both pointers
                        prevIndex <- prevIndex + 1
                        nextIndex <- nextIndex + 1

                        let diff =
                            diffWidgetCollections canReuseView prevWidgetColl nextWidgetColl

                        match diff with
                        | ValueNone -> ()
                        | ValueSome slice ->
                            let change =
                                WidgetCollectionChange.Updated struct (nextAttr, slice)

                            result <- MutStackArray1.addMut(&result, change, MemPool.widgetColChanges)

            MutStackArray1.toArraySlice(&result, MemPool.widgetColChanges)

    and diffWidgetCollections
        (canReuseView: Widget -> Widget -> bool)
        (prev: ArraySlice<Widget>)
        (next: ArraySlice<Widget>)
        : ArraySlice<WidgetCollectionItemChange> voption =
        let mutable result = MutStackArray1.Empty

        let prevSpan = ArraySlice.toSpan prev
        let nextSpan = ArraySlice.toSpan next

        if prevSpan.Length > nextSpan.Length then
            for i = nextSpan.Length to prevSpan.Length - 1 do
                result <-
                    MutStackArray1.addMut(&result, WidgetCollectionItemChange.Remove i, MemPool.widgetColItemChange)

        for i = 0 to nextSpan.Length - 1 do
            let currItem = nextSpan.[i]
            //            index < 0 || index >= array.Length ? (FSharpOption<T>) null : FSharpOption<T>.Some(array[index]);
            let prevItemOpt =
                if (i >= prevSpan.Length) then
                    ValueNone
                else
                    ValueSome prevSpan.[i]

            let changeOpt =
                match prevItemOpt with
                | ValueNone -> ValueSome(WidgetCollectionItemChange.Insert struct (i, currItem))

                | ValueSome prevItem when canReuseView prevItem currItem ->

                    match diffWidget canReuseView (ValueSome prevItem) currItem with
                    | ValueNone -> ValueNone
                    | ValueSome diffs -> ValueSome(WidgetCollectionItemChange.Update struct (i, diffs))

                | ValueSome _ -> ValueSome(WidgetCollectionItemChange.Replace struct (i, currItem))

            match changeOpt with
            | ValueNone -> ()
            | ValueSome change -> result <- MutStackArray1.addMut(&result, change, MemPool.widgetColItemChange)

        // NOTE here we can recycle memory from the prev widgets
        BuildersMemPool.widgets.recycleSlice prev

        MutStackArray1.toArraySlice(&result, MemPool.widgetColItemChange)

    and diffWidget
        (canReuseView: Widget -> Widget -> bool)
        (prevOpt: Widget voption)
        (next: Widget)
        : WidgetDiff voption =
        let prevScalarAttributes =
            match prevOpt with
            | ValueNone -> ValueNone
            | ValueSome widget -> widget.ScalarAttributes

        let prevWidgetAttributes =
            match prevOpt with
            | ValueNone -> None
            | ValueSome widget -> widget.WidgetAttributes

        let prevWidgetCollectionAttributes =
            match prevOpt with
            | ValueNone -> None
            | ValueSome widget -> widget.WidgetCollectionAttributes

        let scalarDiffs =
            diffScalarAttributes prevScalarAttributes next.ScalarAttributes

        let widgetDiffs =
            diffWidgetAttributes canReuseView prevWidgetAttributes next.WidgetAttributes

        let collectionDiffs =
            diffWidgetCollectionAttributes canReuseView prevWidgetCollectionAttributes next.WidgetCollectionAttributes


        match (scalarDiffs, widgetDiffs, collectionDiffs) with
        | ValueNone, ValueNone, ValueNone -> ValueNone
        | _ ->
            ValueSome
                {
                    ScalarChanges = scalarDiffs
                    WidgetChanges = widgetDiffs
                    WidgetCollectionChanges = collectionDiffs
                }

    let rec private recycleDiffMemory (diff: WidgetDiff) : unit =
        match diff.ScalarChanges with
        | ValueSome changes -> MemPool.scalarChanges.recycleSlice changes
        | ValueNone -> ()

        match diff.WidgetChanges with
        | ValueSome slice ->
            for change in ArraySlice.toSpan slice do
                match change with
                | WidgetChange.Updated (struct (_, widgetDiff)) -> recycleDiffMemory widgetDiff
                | _ -> ()

            MemPool.widgetChanges.recycleSlice slice
        | ValueNone -> ()

        match diff.WidgetCollectionChanges with
        | ValueSome slice ->
            for change in ArraySlice.toSpan slice do
                match change with
                | WidgetCollectionChange.Updated (struct (_, itemChangesSlice)) ->
                    for change in ArraySlice.toSpan itemChangesSlice do
                        match change with
                        | WidgetCollectionItemChange.Update (struct (_, widgetDiff)) -> recycleDiffMemory widgetDiff
                        | _ -> ()

                    MemPool.widgetColItemChange.recycleSlice itemChangesSlice
                | _ -> ()

            MemPool.widgetColChanges.recycleSlice slice

        | ValueNone -> ()

    /// Diffs changes and applies them on the target
    let update
        (canReuseView: Widget -> Widget -> bool)
        (prevOpt: Widget voption)
        (next: Widget)
        (node: IViewNode)
        : unit =
        match diffWidget canReuseView prevOpt next with
        | ValueNone -> ()
        | ValueSome diff ->
            match diff.ScalarChanges with
            | ValueSome changes -> node.ApplyScalarDiffs(changes)
            | ValueNone -> ()

            match diff.WidgetChanges with
            | ValueSome slice -> node.ApplyWidgetDiffs(slice)
            | ValueNone -> ()

            match diff.WidgetCollectionChanges with
            | ValueSome slice -> node.ApplyWidgetCollectionDiffs(slice)
            | ValueNone -> ()

            recycleDiffMemory(diff)
