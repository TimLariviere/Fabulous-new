module Tests.TestUI_Attributes

open Fabulous
open Tests.Platform

module Attributes =

    let definePressable name =
        Attributes.withName name
            (fun key name ->
              { Key = key
                Name = name
                Convert = id
                ConvertValue = id
                Compare = ScalarAttributeComparers.noCompare
                UpdateNode =
                    fun (newValueOpt, node) ->

                        let btn = node.Target :?> IButton

                        match node.TryGetHandler<int>(key) with
                        | ValueNone -> ()
                        | ValueSome handlerId -> btn.RemovePressListener handlerId

                        match newValueOpt with
                        | ValueNone -> node.SetHandler(key, ValueNone)

                        | ValueSome msg ->
                            let handler () =
                                Attributes.dispatchMsgOnViewNode node msg

                            let handlerId = btn.AddPressListener handler
                            node.SetHandler<int>(key, ValueSome handlerId) } : ScalarAttributeDefinition<obj, obj, obj>)

    // --------------- Actual Properties ---------------
    //    open Fabulous.Attributes

    [<AbstractClass; Sealed>]
    type Text =
        static member Record =
            Attributes.define<bool> "Record" TestUI_ViewUpdaters.updateRecord

        static member Text =
            Attributes.define<string> "Text" TestUI_ViewUpdaters.updateText

    [<AbstractClass; Sealed>]
    type TextStyle =
        static member TextColor =
            Attributes.define<string> "TextColor" TestUI_ViewUpdaters.updateTextColor

    [<AbstractClass; Sealed>]
    type Container =
        static member Children =
            Attributes.defineWidgetCollection<TestViewElement> "Container_Children" (fun target -> (target :?> IContainer).Children :> System.Collections.Generic.IList<_>)


    [<AbstractClass; Sealed>]
    type Button =
        static member Pressed = definePressable "Button_Pressed"


    [<AbstractClass; Sealed>]
    type Automation =
        static member AutomationId =
            Attributes.define<string> "AutomationId" TestUI_ViewUpdaters.updateAutomationId
