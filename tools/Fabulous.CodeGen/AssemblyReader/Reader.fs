// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.CodeGen.AssemblyReader

open System.Reflection
open Fabulous.CodeGen
open Fabulous.CodeGen.AssemblyReader.Models
open Mono.Cecil

module Reader =
    let readEventsFromType
            (convertTypeName: string -> string)
            (``type``: TypeDefinition) =
        
        Resolver.getAllEventsForType ``type``
        |> Array.map (fun edef ->
            let eventArgsType =
                  match edef.EventType with
                  | :? GenericInstanceType as git -> Some (git.GenericArguments.[0].FullName |> Text.removeDotNetGenericNotation)
                  | _ -> None
                  
            let eventHandlerType =
                match edef.EventType with
                | :? GenericInstanceType as git -> git.FullName |> Text.removeDotNetGenericNotation
                | _ -> "System.EventHandler"
            
            { Name = edef.Name
              EventArgsType = eventArgsType |> Option.map convertTypeName |> Option.defaultValue "unit"
              EventHandlerType = convertTypeName eventHandlerType }    
        )
        
    let readAttachedPropertiesFromType
            (convertTypeName: string -> string)
            (tryGetStringRepresentationOfDefaultValue: obj -> string option)
            (tryGetProperty: string * string -> ReflectionAttachedProperty option)
            (propertyBaseType: string)
            (``type``: TypeDefinition) =
       
        Resolver.getAllAttachedPropertiesForType propertyBaseType ``type``
        |> Array.map (fun fdef ->
            match tryGetProperty (``type``.FullName, fdef.Name.Replace("Property", "")) with
            | None -> None
            | Some data ->
                let attachedPropertyType = data.Type |> Text.removeDotNetGenericNotation |> convertTypeName
                let defaultValue =
                    data.DefaultValue
                    |> Option.bind tryGetStringRepresentationOfDefaultValue 
                    |> Option.defaultValue $"Unchecked.defaultof<%s{attachedPropertyType}>"

                Some
                    ({ Name = data.Name
                       Type = attachedPropertyType
                       DefaultValue = defaultValue } : AssemblyTypeAttachedProperty)
        )
        |> Array.choose id

    let readPropertiesFromType
            (convertTypeName: string -> string)
            (tryGetStringRepresentationOfDefaultValue: obj -> string option)
            (tryGetProperty: string * string -> ReflectionAttachedProperty option)
            (``type``: TypeDefinition) =

        let getDefaultValueAsString typeName (value: obj) =
            tryGetStringRepresentationOfDefaultValue value
            |> Option.defaultValue $"Unchecked.defaultof<%s{typeName}>"

        let settableProperties =
            Resolver.getAllPropertiesForType ``type``
            |> Array.map (fun tdef ->
                match tryGetProperty (``type``.FullName, tdef.Name) with
                | None -> None
                | Some data ->
                    let propertyType = data.Type |> Text.removeDotNetGenericNotation |> convertTypeName
                    let defaultValue =
                        match data.DefaultValue with
                        | None -> ""
                        | Some defaultValue -> getDefaultValueAsString propertyType defaultValue
                        
                    Some
                        ({ Name = data.Name
                           Type = propertyType
                           CollectionElementType = Resolver.getElementTypeForType ``type``
                           DefaultValue = defaultValue } : AssemblyTypeProperty)
            )
            |> Array.choose id

        let listPropertiesWithNoSetter =
            Resolver.getAllListPropertiesWithNoSetterForType ``type``
            |> Array.map (fun pdef ->
                let propertyType = pdef.PropertyType.FullName |> Text.removeDotNetGenericNotation |> convertTypeName

                { Name = pdef.Name
                  Type = propertyType
                  CollectionElementType = Resolver.getElementTypeForPropertyType pdef.PropertyType
                  DefaultValue = getDefaultValueAsString propertyType null } : AssemblyTypeProperty
            )

        Array.concat [ settableProperties; listPropertiesWithNoSetter ]

    let readType
            (convertTypeName: string -> string)
            (tryGetStringRepresentationOfDefaultValue: obj -> string option)
            (tryGetProperty: string * string -> ReflectionAttachedProperty option)
            (propertyBaseType: string)
            (baseTypeName: string)
            (tdef: TypeDefinition) =
        let ctor =
            tdef.Methods
             |> Seq.tryFind (fun x -> x.IsConstructor && x.IsPublic && x.Parameters.Count = 0)
        
        { FullName = tdef.FullName
          AssemblyName = tdef.Module.Assembly.Name.Name
          CanBeInstantiated = not tdef.IsAbstract && ctor.IsSome
          InheritanceHierarchy = Resolver.getHierarchyForType baseTypeName tdef 
          Events = readEventsFromType convertTypeName tdef
          AttachedProperties = readAttachedPropertiesFromType convertTypeName tryGetStringRepresentationOfDefaultValue tryGetProperty propertyBaseType tdef
          Properties = readPropertiesFromType convertTypeName tryGetStringRepresentationOfDefaultValue tryGetProperty tdef }
        
    let readAssemblies
            (loadAllAssembliesByReflection: seq<string> -> Assembly array)
            (tryGetAttachedPropertyByReflection: Assembly array -> string * string -> ReflectionAttachedProperty option)
            (isTypeResolvable: string -> bool)
            (convertTypeName: string -> string)
            (tryGetStringRepresentationOfDefaultValue: obj -> string option)
            (propertyBaseType: string)
            (baseTypeName: string)
            assemblies : WorkflowResult<AssemblyType array> =
        
        use resolver = new AssemblyResolver.RegistrableResolver()
        let cecilAssemblies = AssemblyResolver.loadAllAssemblies resolver assemblies
        let assemblies = loadAllAssembliesByReflection assemblies
        
        let allTypes = Resolver.getAllTypesFromAssemblies cecilAssemblies
        let allTypesDerivingFromBaseType = Resolver.getAllTypesDerivingFromBaseType isTypeResolvable allTypes baseTypeName
        
        let tryGetProperty = tryGetAttachedPropertyByReflection assemblies
        
        let data =
            allTypesDerivingFromBaseType
            |> Array.map
                   (readType
                        convertTypeName
                        tryGetStringRepresentationOfDefaultValue
                        tryGetProperty
                        propertyBaseType
                        baseTypeName)
        
        WorkflowResult.ok data

