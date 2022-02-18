﻿namespace Fabulous.XamarinForms

open System
open System.Collections.Generic
open System.Runtime.CompilerServices
open Fabulous
open Fabulous.StackAllocatedCollections.StackList
open Fabulous.StackAllocatedCollections
open Microsoft.FSharp.Core

[<AbstractClass; Sealed>]
type View =
    class
    end

module Widgets =
    let registerWithAdditionalSetup<'T when 'T :> Xamarin.Forms.BindableObject and 'T: (new: unit -> 'T)>
        (additionalSetup: 'T -> IViewNode -> unit)
        =
        let key = WidgetDefinitionStore.getNextKey ()

        let definition =
            { Key = key
              Name = typeof<'T>.Name
              TargetType = typeof<'T>
              CreateView =
                  fun (widget, treeContext, parentNode) ->
                      let name = typeof<'T>.Name
                      printfn $"Creating view for {name}"

                      let view = new 'T()
                      let weakReference = WeakReference(view)

                      let node =
                          ViewNode(parentNode, treeContext, weakReference)

                      ViewNode.set node view

                      additionalSetup view node

                      Reconciler.update treeContext.CanReuseView ValueNone widget node
                      struct (node :> IViewNode, box view) }

        WidgetDefinitionStore.set key definition
        key

    let register<'T when 'T :> Xamarin.Forms.BindableObject and 'T: (new: unit -> 'T)> () =
        registerWithAdditionalSetup<'T> (fun _ _ -> ())

module WidgetHelpers =
    let inline compileSeq (items: seq<WidgetBuilder<'msg, 'marker>>) =
        items
        |> Seq.map (fun item -> item.Compile())
        |> Seq.toArray

    let inline buildWidgets<'msg, 'marker> (key: WidgetKey) (attrs: WidgetAttribute []) =
        WidgetBuilder<'msg, 'marker>(key, struct (StackList.empty (), ValueSome attrs, ValueNone))

    let inline buildAttributeCollection<'msg, 'marker, 'item>
        (collectionAttributeDefinition: WidgetCollectionAttributeDefinition)
        (widget: WidgetBuilder<'msg, 'marker>)
        =
        AttributeCollectionBuilder<'msg, 'marker, 'item>(widget, collectionAttributeDefinition)

    let buildItems<'msg, 'marker, 'itemData, 'itemMarker>
        key
        (attrDef: ScalarAttributeDefinition<WidgetItems<'itemData>, WidgetItems<'itemData>, IEnumerable<Widget>>)
        (items: seq<'itemData>)
        (itemTemplate: 'itemData -> WidgetBuilder<'msg, 'itemMarker>)
        =
        let template (item: obj) =
            let item = unbox<'itemData> item
            (itemTemplate item).Compile()

        let data: WidgetItems<'itemData> =
            { OriginalItems = items
              Template = template }

        WidgetBuilder<'msg, 'marker>(key, attrDef.WithValue(data))

    let buildItemsWithScalars<'msg, 'marker, 'itemData, 'itemMarker>
        key
        (scalarA: ScalarAttribute)
        (attrDef: ScalarAttributeDefinition<WidgetItems<'itemData>, WidgetItems<'itemData>, IEnumerable<Widget>>)
        (items: seq<'itemData>)
        (itemTemplate: 'itemData -> WidgetBuilder<'msg, 'itemMarker>)
        =
        let template (item: obj) =
            let item = unbox<'itemData> item
            (itemTemplate item).Compile()

        let data: WidgetItems<'itemData> =
            { OriginalItems = items
              Template = template }

        WidgetBuilder<'msg, 'marker>(key, scalarA, attrDef.WithValue(data))

    let buildGroupItems<'msg, 'marker, 'groupData, 'itemData, 'groupMarker, 'itemMarker when 'groupData :> seq<'itemData>>
        key
        (attrDef: ScalarAttributeDefinition<GroupedWidgetItems<'groupData>, GroupedWidgetItems<'groupData>, IEnumerable<GroupItem>>)
        (items: seq<'groupData>)
        (groupHeaderTemplate: 'groupData -> WidgetBuilder<'msg, 'groupMarker>)
        (itemTemplate: 'itemData -> WidgetBuilder<'msg, 'itemMarker>)
        (groupFooterTemplate: 'groupData -> WidgetBuilder<'msg, 'groupMarker>)
        =
        let template (group: obj) =
            let group = unbox<'groupData> group
            let header = (groupHeaderTemplate group).Compile()
            let footer = (groupFooterTemplate group).Compile()

            let items =
                group
                |> Seq.map (fun item -> (itemTemplate item).Compile())

            GroupItem(header, footer, items)

        let data: GroupedWidgetItems<'groupData> =
            { OriginalItems = items
              Template = template }

        WidgetBuilder<'msg, 'marker>(key, attrDef.WithValue(data))

    let buildGroupItemsNoFooter<'msg, 'marker, 'groupData, 'itemData, 'groupMarker, 'itemMarker when 'groupData :> seq<'itemData>>
        key
        attrDef
        items
        groupHeaderTemplate
        itemTemplate
        =
        buildGroupItems<'msg, 'marker, 'groupData, 'itemData, 'groupMarker, 'itemMarker>
            key
            attrDef
            items
            groupHeaderTemplate
            itemTemplate
            (fun _ -> Unchecked.defaultof<_>)
