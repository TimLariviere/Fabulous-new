namespace Fabulous

open System
open Microsoft.FSharp.Core

// TODO try to use it to optimize ArraySlice<'t> voption type
// that is, potentially save on padding + optimize readability
//
//[<Struct; RequireQualifiedAccess; NoComparison>]
//type PartialArray<'v> =
//    | Empty
//    | Filled of ArraySlice<'v>
//
//module PartialArray =
//    let inline isEmpty (arr: PartialArray<'v>) : bool =
//        match arr with
//        | PartialArray.Empty -> true
//        | PartialArray.Filled (used, _) -> used > 0us
//
//    let inline fromArray (arr: 'v []) : PartialArray<'v> =
//        match arr.Length with
//        | 0 -> PartialArray.Empty
//        | len -> PartialArray.Filled(uint16 len, arr)


module Array =
    let inline appendOne (v: 'v) (arr: 'v array) =
        let res = Array.zeroCreate(arr.Length + 1)
        Array.blit arr 0 res 0 arr.Length
        res.[arr.Length] <- v
        res

    /// This is insertion sort that is O(n*n) but it performs better
    /// 1. if the array is partially sorted (second sort is cheap)
    /// 2. there are few elements, we expect to have only a handful of them per widget
    /// 3. stable, which is handy for duplicate attributes, e.g. we can choose which one to pick
    /// https://en.wikipedia.org/wiki/Insertion_sort
    let inline sortInPlace<'T, 'V when 'V: comparison> ([<InlineIfLambda>] getKey: 'T -> 'V) (values: 'T []) : 'T [] =
        let N = values.Length

        for i in [ 1 .. N - 1 ] do
            for j = i downto 1 do
                let key = getKey values.[j]
                let prevKey = getKey(values.[j - 1])

                if key < prevKey then
                    let temp = values.[j]
                    values.[j] <- values.[j - 1]
                    values.[j - 1] <- temp

        values


    let inline sortSpanInPlace<'T, 'V when 'V: comparison>
        ([<InlineIfLambda>] getKey: 'T -> 'V)
        (values: Span<'T>)
        : unit =
        let N = values.Length

        for i in [ 1 .. N - 1 ] do
            for j = i downto 1 do
                let key = getKey values.[j]
                let prevKey = getKey(values.[j - 1])

                if key < prevKey then
                    let temp = values.[j]
                    values.[j] <- values.[j - 1]
                    values.[j - 1] <- temp
