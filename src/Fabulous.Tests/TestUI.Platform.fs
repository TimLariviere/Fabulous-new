module Tests.Platform

open System.Collections.Generic


type TestViewElement() =
    member val AutomationId: string = "" with get, set
    member val PropertyBag = Dictionary<string, obj>()

type IText =
    abstract member Text: string with get, set
    abstract member TextColor: string with get, set

type IContainer =
    abstract member Children: List<TestViewElement>

type ButtonHandler = unit -> unit

type IButton =
    abstract AddPressListener: ButtonHandler -> int
    abstract RemovePressListener: int -> unit

type LabelChangeList =
    | TextSet of string
    | ColorSet of string


type TestLabel() =
    inherit TestViewElement()

    let mutable text = ""
    let mutable textColor = ""
    // TODO: Fix unit tests for Memo
    // Record is actually dependent on the sort order of Array.sortInPlace(attr.Key).
    // By making AttributeDefinition lazy for allowing deletion of unused code by linker,
    // the value of attr.Key is not guaranteed and completely dependent on usage.
    // This was making ViewNodeInstanceCanBeReused.Test fail because Text was before Record in the sorted array.
    member val record = false with get, set

    member val changeList = [] with get, set

    interface IText with
        member x.Text
            with get () = text
            and set (value) =
                //if x.record then
                x.changeList <- List.append x.changeList [ TextSet value ]

                text <- value

        member x.TextColor
            with get () = textColor
            and set (value) =
                //if x.record then
                x.changeList <- List.append x.changeList [ ColorSet value ]

                textColor <- value

type TestStack() =
    inherit TestViewElement()

    interface IContainer with
        member val Children = List<TestViewElement>()


type TestButton() =
    inherit TestViewElement()
    let mutable counter: int = 1
    let handlers = Dictionary<int, ButtonHandler>()

    member _.Press() =
        for handler in Array.ofSeq (handlers.Values) do
            handler ()

    interface IText with
        member val Text = "" with get, set
        member val TextColor = "" with get, set

    interface IButton with
        member this.AddPressListener(handler) =
            handlers.Add(counter, handler)
            counter <- counter + 1
            counter - 1

        member this.RemovePressListener(id) = handlers.Remove(id) |> ignore
