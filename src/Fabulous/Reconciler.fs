namespace Fabulous

open Fabulous

type IViewNodeWithDiff =
    inherit IViewNode
    abstract member ApplyDiff: WidgetDiff inref -> unit
    
module Reconciler =
    let update
        (canReuseView: Widget -> Widget -> bool)
        (prevOpt: WidgetData voption)
        (next: WidgetData)
        (node: IViewNodeWithDiff)
        : unit =

        let diff =
            WidgetDiff.create (prevOpt, next, canReuseView)

        node.ApplyDiff(&diff)
