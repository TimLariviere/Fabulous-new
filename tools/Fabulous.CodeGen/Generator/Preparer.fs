// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.CodeGen.Generator

open Fabulous.CodeGen
open Fabulous.CodeGen.Binder.Models
open Fabulous.CodeGen.Generator.Models

module Preparer =
    let extractAttributes (boundTypes: BoundType array) : AttributeData array =
        (seq {
            for boundType in boundTypes do
                for e in boundType.Events do
                    if e.IsInherited = false && e.CustomAttributeKey.IsNone then
                        yield { UniqueName = e.UniqueName; Name = e.Name; ModelType = e.ModelType }
                for p in boundType.Properties do
                    if p.IsInherited = false then
                        if p.CustomAttributeKey.IsNone then
                            yield { UniqueName = p.UniqueName; Name = p.Name; ModelType = p.ModelType }
                   
                        match p.CollectionData with
                        | None -> ()
                        | Some cd ->
                            for ap in cd.AttachedProperties do
                                if ap.CustomAttributeKey.IsNone then
                                    yield { UniqueName = ap.UniqueName; Name = ap.Name; ModelType = ap.ModelType } } : seq<AttributeData>)
        |> Seq.distinctBy (fun a -> a.UniqueName)
        |> Seq.toArray

    let toBuildData (boundType: BoundType) =
        let toBuildMember (m: IBoundConstructorMember) : BuildMember =
            { Name = m.ShortName
              UniqueName = m.UniqueName
              CustomAttributeKey = m.CustomAttributeKey
              InputType = m.InputType
              ConvertInputToModel = m.ConvertInputToModel
              IsInherited = m.IsInherited }
        
        let properties = boundType.Properties |> Array.map toBuildMember
        let events = boundType.Events |> Array.map toBuildMember
        let members = Array.concat [ properties; events ]
        
        { Name = boundType.Name
          BaseName = boundType.BaseTypeName
          Members = members }

    let toCreateData (boundType: BoundType) =
        { Name = boundType.Name
          FullName = boundType.FullName
          TypeToInstantiate = boundType.TypeToInstantiate
          CreateCode = boundType.CreateCode }

    let toUpdateAttachedPropertiesData (boundType: BoundType) =
        let immediatePropertiesWithAttachedProperties = boundType.Properties |> Array.filter (fun p -> not p.IsInherited && p.CollectionData.IsSome && p.CollectionData.Value.AttachedProperties.Length > 0)
        
        let updatePropertiesWithAttachedProperties =
            immediatePropertiesWithAttachedProperties
            |> Array.map (fun p ->
                { UniqueName = p.UniqueName
                  CustomAttributeKey = p.CustomAttributeKey
                  CollectionDataElementType = p.CollectionData |> Option.map (fun c -> c.ElementType)
                  AttachedProperties =
                     p.CollectionData
                     |> Option.map (fun cd ->
                           cd.AttachedProperties
                           |> Array.map (fun ap ->
                               { Name = ap.Name
                                 UniqueName = ap.UniqueName
                                 CustomAttributeKey = ap.CustomAttributeKey
                                 DefaultValue = ap.DefaultValue
                                 OriginalType = ap.OriginalType
                                 ModelType = ap.ModelType
                                 ConvertModelToValue = ap.ConvertModelToValue
                                 UpdateCode = ap.UpdateCode }))
                     |> Option.defaultValue [||] })

        { Name = boundType.Name
          FullName = boundType.FullName
          BaseName = boundType.BaseTypeName
          PropertiesWithAttachedProperties = updatePropertiesWithAttachedProperties }

    let toUpdateData (boundType: BoundType) =
        let immediateEvents = boundType.Events |> Array.filter (fun e -> not e.IsInherited && e.CanBeUpdated)
        let immediateProperties = boundType.Properties |> Array.filter (fun p -> not p.IsInherited && p.CanBeUpdated)
        
        let eventMembers = immediateEvents |> Array.map (fun e -> { UniqueName = e.UniqueName; CustomAttributeKey = e.CustomAttributeKey; ModelType = e.ModelType })
        let propertyMembers = immediateProperties |> Array.map (fun p -> { UniqueName = p.UniqueName; CustomAttributeKey = p.CustomAttributeKey; ModelType = p.ModelType })
        let immediateMembers = Array.concat [ eventMembers; propertyMembers ]
        
        let updateEvents = immediateEvents |> Array.map (fun e ->
            let relatedProperties = e.RelatedProperties |> Array.choose (fun rp ->
                boundType.Properties
                |> Array.tryFind (fun p -> p.Name = rp)
                |> Option.map (fun p -> p.UniqueName))
            
            { Name = e.Name
              UniqueName = e.UniqueName
              RelatedProperties = relatedProperties }
        )

        let updateProperties =
            immediateProperties
            |> Array.filter (fun p -> not p.HasPriority)
            |> Array.map (fun p ->
                { Name = p.Name
                  UniqueName = p.UniqueName
                  CustomAttributeKey = p.CustomAttributeKey
                  DefaultValue = p.DefaultValue
                  OriginalType = p.OriginalType
                  ModelType = p.ModelType
                  ConvertModelToValue = p.ConvertModelToValue
                  UpdateCode = p.UpdateCode
                  CollectionDataElementType = p.CollectionData |> Option.map (fun c -> c.ElementType) })
            
        let updatePriorityProperties =
            immediateProperties
            |> Array.filter (fun p -> p.HasPriority)
            |> Array.map (fun p ->
                { Name = p.Name
                  UniqueName = p.UniqueName
                  CustomAttributeKey = p.CustomAttributeKey
                  DefaultValue = p.DefaultValue
                  OriginalType = p.OriginalType
                  ModelType = p.ModelType
                  ConvertModelToValue = p.ConvertModelToValue
                  UpdateCode = p.UpdateCode
                  CollectionDataElementType = p.CollectionData |> Option.map (fun c -> c.ElementType) })
        
        { Name = boundType.Name
          FullName = boundType.FullName
          BaseName = boundType.BaseTypeName
          ImmediateMembers = immediateMembers
          Events = updateEvents
          Properties = updateProperties
          PriorityProperties = updatePriorityProperties }

    let toConstructData (boundType: BoundType) : ConstructData =
        let properties = boundType.Properties |> Array.map (fun p -> { Name = p.ShortName; InputType = p.InputType } : ConstructType)
        let events = boundType.Events |> Array.map (fun e -> { Name = e.ShortName; InputType = e.InputType } : ConstructType)
        let members = Array.concat [ properties; events ]
        
        { Name = boundType.Name
          FullName = boundType.FullName
          Members = members }
    
    let toBuilderData (boundType: BoundType) =
        { Build = toBuildData boundType
          Create = if boundType.CanBeInstantiated then Some (toCreateData boundType) else None
          UpdateAttachedProperties = toUpdateAttachedPropertiesData boundType
          Update = toUpdateData boundType
          Construct = if boundType.CanBeInstantiated then Some (toConstructData boundType) else None }

    let toViewerData (allTypes: BoundType array) (boundType: BoundType) : ViewerData =
        let properties = boundType.Properties |> Array.filter (fun p -> not p.IsInherited) |> Array.map (fun p -> { Name = p.Name; UniqueName = p.UniqueName; CustomAttributeKey = p.CustomAttributeKey })
        let events = boundType.Events |> Array.filter (fun e -> not e.IsInherited) |> Array.map (fun e -> { Name = e.Name; UniqueName = e.UniqueName; CustomAttributeKey = e.CustomAttributeKey })
        let members = Array.concat [ properties; events ]
            
        let rec tryGetInheritedInstantiableType (typ: BoundType) =
            match typ.BaseTypeName with
            | None -> None
            | Some name ->
                match allTypes |> Array.tryFind (fun t -> t.Name = name) with
                | None -> None
                | Some x when x.CanBeInstantiated = false -> tryGetInheritedInstantiableType x
                | Some _ -> Some name
            
        { Name = boundType.Name
          FullName = boundType.FullName
          ViewerName = $"%s{boundType.Name}Viewer"
          GenericConstraint = boundType.GenericConstraint
          InheritedViewerName = tryGetInheritedInstantiableType boundType |> Option.map (sprintf "%sViewer")
          InheritedGenericConstraint = boundType.BaseGenericConstraint
          Members = members }
    
    type TempConstructorMember = { Name: string; ShortName: string; InputType: string }
    
    let toConstructorData (boundType: BoundType) =
        let properties = boundType.Properties |> Array.map (fun p -> { Name = p.Name; ShortName = p.ShortName; InputType = p.InputType })
        let events = boundType.Events |> Array.map (fun e -> { Name = e.Name; ShortName = e.ShortName; InputType = e.InputType })
        let allMembers = Array.concat [ properties; events ]
        
        let primaryMembers =
            match boundType.PrimaryConstructorMembers with
            | None -> [||]
            | Some memberNames -> memberNames |> Array.choose (fun n -> allMembers |> Array.tryFind (fun m -> m.Name = n))
            
        let otherMembers =
            allMembers
            |> Array.except primaryMembers
            |> Array.sortBy (fun m -> m.ShortName)
        
        let members =
            [ primaryMembers; otherMembers ]
            |> Array.concat
            |> Array.map (fun p -> { Name = p.ShortName; InputType = p.InputType })
        
        { Name = boundType.Name
          FullName = boundType.FullName
          Members = members }

    let getViewExtensionsData (types: BoundType array) =
        let toViewExtensionsMember (m: IBoundMember) =
            { LowerUniqueName = Text.toLowerPascalCase m.UniqueName
              UniqueName = m.UniqueName
              CustomAttributeKey = m.CustomAttributeKey
              InputType = m.InputType
              ConvertInputToModel = m.ConvertInputToModel }
            
        [|
            for typ in types do
                for e in typ.Events do
                    if e.IsInherited = false then
                        yield toViewExtensionsMember e
                        
                for p in typ.Properties do
                    if p.IsInherited = false then
                        yield toViewExtensionsMember p
                   
                        match p.CollectionData with
                        | None -> ()
                        | Some cd ->
                            for a in cd.AttachedProperties do
                                yield toViewExtensionsMember a
        |]
        |> Array.groupBy (fun y -> y.UniqueName)
        |> Array.map (fun (_, members) -> members |> Array.head)
    
    let prepareData (boundModel: BoundModel) =
        let typesToGenerate = boundModel.Types |> Array.filter (fun t -> t.ShouldGenerateBinding)
        
        { Namespace = boundModel.OutputNamespace
          AdditionalNamespaces = boundModel.AdditionalNamespaces
          Attributes = extractAttributes typesToGenerate
          Builders = typesToGenerate |> Array.map toBuilderData
          Viewers = typesToGenerate |> Array.map (toViewerData boundModel.Types)
          Constructors = typesToGenerate |> Array.filter (fun t -> t.CanBeInstantiated) |> Array.map toConstructorData
          ViewExtensions = typesToGenerate |> getViewExtensionsData }

