// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.CodeGen.Binder

module Models =
    type IBoundConstructorMember =
        abstract ShortName: string
        abstract UniqueName: string
        abstract InputType: string
        abstract ConvertInputToModel: string
        abstract IsInherited: bool
        abstract CustomAttributeKey: string option
        
    type IBoundMember =
        abstract UniqueName: string
        abstract InputType: string
        abstract ConvertInputToModel: string
        abstract CustomAttributeKey: string option
    
    type BoundEvent =
        { Name: string
          ShortName: string
          UniqueName: string
          CanBeUpdated: bool
          CustomAttributeKey: string option
          EventArgsType: string
          InputType: string
          ModelType: string
          ConvertInputToModel: string
          RelatedProperties: string array
          IsInherited: bool }
        interface IBoundConstructorMember with
            member this.ShortName = this.ShortName
            member this.UniqueName = this.UniqueName
            member this.InputType = this.InputType
            member this.ConvertInputToModel = this.ConvertInputToModel
            member this.IsInherited = this.IsInherited
            member this.CustomAttributeKey = this.CustomAttributeKey
        interface IBoundMember with
            member this.UniqueName = this.UniqueName
            member this.InputType = this.InputType
            member this.ConvertInputToModel = this.ConvertInputToModel
            member this.CustomAttributeKey = this.CustomAttributeKey
            
    type BoundAttachedProperty =
        { Name: string
          UniqueName: string
          DefaultValue: string
          CanBeUpdated: bool
          CustomAttributeKey: string option
          OriginalType: string
          InputType: string
          ModelType: string
          ConvertInputToModel: string
          ConvertModelToValue: string
          UpdateCode: string }
        interface IBoundMember with
            member this.UniqueName = this.UniqueName
            member this.InputType = this.InputType
            member this.ConvertInputToModel = this.ConvertInputToModel
            member this.CustomAttributeKey = this.CustomAttributeKey
    
    type BoundPropertyCollectionData =
        { ElementType: string
          AttachedProperties: BoundAttachedProperty array }
    
    type BoundProperty =
        { Name: string
          ShortName: string
          UniqueName: string
          CanBeUpdated: bool
          CustomAttributeKey: string option
          DefaultValue: string
          OriginalType: string
          InputType: string
          ModelType: string
          ConvertInputToModel: string
          ConvertModelToValue: string
          UpdateCode: string
          CollectionData: BoundPropertyCollectionData option
          HasPriority: bool
          IsInherited: bool }
        interface IBoundConstructorMember with
            member this.ShortName = this.ShortName
            member this.UniqueName = this.UniqueName
            member this.InputType = this.InputType
            member this.ConvertInputToModel = this.ConvertInputToModel
            member this.IsInherited = this.IsInherited
            member this.CustomAttributeKey = this.CustomAttributeKey
        interface IBoundMember with
            member this.UniqueName = this.UniqueName
            member this.InputType = this.InputType
            member this.ConvertInputToModel = this.ConvertInputToModel
            member this.CustomAttributeKey = this.CustomAttributeKey
    
    type BoundType =
        { Id: string
          FullName: string
          ShouldGenerateBinding: bool
          GenericConstraint: string option
          CanBeInstantiated: bool
          CreateCode: string option
          TypeToInstantiate: string
          BaseTypeName: string option
          BaseGenericConstraint: string option
          Name: string
          Events: BoundEvent array
          Properties: BoundProperty array
          PrimaryConstructorMembers: string array option }
    
    type BoundModel =
        { Assemblies: string array
          OutputNamespace: string
          AdditionalNamespaces: string array
          Types: BoundType array }