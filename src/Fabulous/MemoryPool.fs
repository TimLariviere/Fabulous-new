module Fabulous.MemoryPool

open System.Buffers

[<Struct>]
type private Cell<'v> =
    | Empty
    | Stored of struct (int * 'v array)

type Pool<'v when 'v: struct>() =

//    // TODO consider a different data structure, maybe a List of arrays or something
    let mutable items: Cell<'v> array = Array.create 100 Empty
    let mutable availableCount = 0

    member x.allocate(size: int) : 'v array =
        let mutable i = 0
        let mutable res = None
        let mutable leftToCheck = availableCount

        while leftToCheck > 0  && i >= 0 && (i < items.Length - 1) do
            match items.[i] with
            | Stored struct (available, arr) ->
                if available >= size then
                    items.[i] <- Empty // cleanup
                    res <- Some arr
                    availableCount <- availableCount - 1 
                    i <- -1 // break loop
                else
                    leftToCheck <- leftToCheck - 1
                    i <- i + 1
             
            | Empty-> i <- i + 1

        match res with
        | Some arr -> arr
        | None -> Array.zeroCreate size

      
    member x.recycleSlice(struct (size, arr) : ArraySlice<'v>) : unit =
        match size with
        | 0us -> ()
        | _ -> x.recycle arr 
            
    member x.recycle(arr: 'v array) : unit =
        availableCount <- availableCount + 1
        
        let mutable i = 0
        let mutable filled = false
        

        let length = arr.Length

        for i = 0 to length - 1 do
            // cleanup items in the array
            arr.[i] <- Unchecked.defaultof<'v>

        while (not filled) && (i < items.Length - 1) do
            match items.[i] with
            | Empty ->
                items.[i] <- Stored(length, arr)
                filled <- true
            | _ -> i <- i + 1

        if not filled then
            items <- items |> Array.appendOne(Stored(length, arr))


   // -------------
//    let arrPool = ArrayPool.Create()
//        
//    member x.allocate(size: int) : 'v array = arrPool.Rent size
//    member x.recycle(arr: 'v array) : unit =
//            arrPool.Return arr
   // -------------
      