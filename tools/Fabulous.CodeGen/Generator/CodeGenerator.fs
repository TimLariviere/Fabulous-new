// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.CodeGen.Generator

open Fabulous.CodeGen
open Fabulous.CodeGen.Binder.Models
open Fabulous.CodeGen.Text
open Fabulous.CodeGen.Generator.Models
open System.IO

module CodeGenerator =
    let getAttributeKeyName uniqueName =
        $"%s{uniqueName}AttribKey"
    
    let getAttributeKey customAttributeKey uniqueName =
        match customAttributeKey with
        | None -> $"ViewAttributes.%s{getAttributeKeyName uniqueName}"
        | Some attributeKey -> attributeKey
    
    let generateNamespace (namespaceOfGeneratedCode: string) (additionalNamespaces: string array) (w: StringWriter) = 
        w.printfn "// Copyright 2018-2022 Fabulous contributors. See LICENSE.md for license."
        w.printfn $"namespace %s{namespaceOfGeneratedCode}"
        w.printfn ""
        w.printfn "open Fabulous"
        w.printfn "open Fabulous.XamarinForms"
        
        for additionalNamespace in additionalNamespaces do
            w.printfn $"open %s{additionalNamespace}"
        
        w.printfn ""
        w

    let generateAttributes (members: AttributeData array) (w: StringWriter) =
//        w.printfn "module ViewAttributes ="
//        for m in members do
//            let typeName =
//                match m.Name with
//                | "Created" -> "(obj -> unit)"
//                | _ -> m.ModelType
//                
//            w.printfn $"    let %s{getAttributeKeyName m.UniqueName} : AttributeKey<_> = AttributeKey<%s{typeName}>(\"%s{m.UniqueName}\")"
        w.printfn ""
        w

    let generateBuildFunction (data: BuildData) (w: StringWriter) =
        let memberNewLine = "\n                              " + String.replicate data.Name.Length " " + " "
        let members =
            data.Members
            |> Array.map (fun m -> $",%s{memberNewLine}?%s{m.Name}: %s{m.InputType}")
            |> Array.fold (+) ""

        let immediateMembers =
            data.Members
            |> Array.filter (fun m -> not m.IsInherited)

        w.printfn $"    /// Builds the attributes for a %s{data.Name} in the view"
        w.printfn $"    static member inline Build%s{data.Name}(attribCount: int%s{members}) = "

        if immediateMembers.Length > 0 then
            w.printfn ""
            for m in immediateMembers do
                w.printfn $"        let attribCount = match %s{m.Name} with Some _ -> attribCount + 1 | None -> attribCount"
            w.printfn ""

        match data.BaseName with 
        | None ->
            w.printfn "        let attribBuilder = AttributesBuilder(attribCount)"
        | Some nameOfBaseCreator ->
            let baseMemberNewLine = "\n                                              " + String.replicate nameOfBaseCreator.Length " " + " "
            let baseMembers =
                data.Members
                |> Array.filter (fun m -> m.IsInherited)
                |> Array.mapi (fun index m -> sprintf ", %s?%s=%s" (if index > 0 && index % 5 = 0 then baseMemberNewLine else "") m.Name m.Name)
                |> Array.fold (+) ""
            w.printfn $"        let attribBuilder = ViewBuilders.Build%s{nameOfBaseCreator}(attribCount%s{baseMembers})"

        for m in immediateMembers do
            let attributeKey = getAttributeKey m.CustomAttributeKey m.UniqueName                
            w.printfn $"        match %s{m.Name} with None -> () | Some v -> attribBuilder.Add(%s{attributeKey}, %s{m.ConvertInputToModel}(v)) " 

        w.printfn "        attribBuilder"
        w.printfn ""
        w

    let generateCreateFunction (data: CreateData option) (w: StringWriter) =
        match data with
        | None -> w
        | Some data ->

            w.printfn $"    static member Create%s{data.Name} () : %s{data.FullName} ="
            
            let createCode =
                match data.CreateCode with
                | Some createCode -> createCode
                | _ -> $"%s{data.TypeToInstantiate}()"

            if data.TypeToInstantiate = data.FullName then
                w.printfn $"        %s{createCode}"
            else
                w.printfn $"        upcast (%s{createCode})"
            
            w.printfn ""
            w

    let generateUpdateAttachedPropertiesFunction (data: UpdateAttachedPropertiesData) (w: StringWriter) =
        let printUpdateBaseIfNeeded (space: string) (isUnitRequired: bool) =
            match data.BaseName with
            | None when isUnitRequired = false ->
                ()
            | None ->
                w.printfn $"%s{space}()"
            | Some baseName ->
                w.printfn $"%s{space}ViewBuilders.Update%s{baseName}AttachedProperties(propertyKey, prevOpt, curr, target)"

        w.printfn $"    static member Update%s{data.Name}AttachedProperties (propertyKey: int, prevOpt: ViewElement voption, curr: ViewElement, target: obj) = "
        if data.PropertiesWithAttachedProperties.Length = 0 then
            printUpdateBaseIfNeeded "        " true
        else
            w.printfn "        match propertyKey with"
            for p in data.PropertiesWithAttachedProperties do
                w.printfn $"        | key when key = %s{getAttributeKey p.CustomAttributeKey p.UniqueName}.KeyValue ->"

                for ap in p.AttachedProperties do
                    let hasApply = not (System.String.IsNullOrWhiteSpace(ap.ConvertModelToValue)) || not (System.String.IsNullOrWhiteSpace(ap.UpdateCode))
                    let attributeKey = getAttributeKey ap.CustomAttributeKey ap.UniqueName

                    w.printfn $"            let prev%s{ap.UniqueName}Opt = match prevOpt with ValueNone -> ValueNone | ValueSome prevChild -> prevChild.TryGetAttributeKeyed<%s{ap.ModelType}>(%s{attributeKey})"
                    w.printfn $"            let curr%s{ap.UniqueName}Opt = curr.TryGetAttributeKeyed<%s{ap.ModelType}>(%s{attributeKey})"
                    w.printfn "            let target = target :?> %s" (Option.defaultValue "MISSING_COLLECTION_ELEMENT_TYPE" p.CollectionDataElementType)

                    if ap.ModelType = "ViewElement" && not hasApply then
                        w.printfn $"            match struct (prev%s{ap.UniqueName}Opt, curr%s{ap.UniqueName}Opt) with"
                        w.printfn "            // For structured objects, dependsOn on reference equality"
                        w.printfn "            | struct (ValueSome prevValue, ValueSome newValue) when identical prevValue newValue -> ()"
                        w.printfn "            | struct (ValueSome prevValue, ValueSome newValue) when canReuseView prevValue newValue ->"
                        w.printfn $"                newValue.UpdateIncremental(prevValue, (%s{data.FullName}.Get%s{ap.Name}(target)))"
                        w.printfn "            | struct (_, ValueSome newValue) ->"
                        w.printfn $"                %s{data.FullName}.Set%s{ap.Name}(target, (newValue.Create() :?> %s{ap.OriginalType}))"
                        w.printfn "            | struct (ValueSome _, ValueNone) ->"
                        w.printfn $"                %s{data.FullName}.Set%s{ap.Name}(target, null)"
                        w.printfn "            | struct (ValueNone, ValueNone) -> ()"
                        
                    elif not (System.String.IsNullOrWhiteSpace(ap.UpdateCode)) then
                        w.printfn $"            %s{ap.UpdateCode} prev%s{ap.UniqueName}Opt curr%s{ap.UniqueName}Opt targetChild"
                        
                    else
                        w.printfn $"            match struct (prev%s{ap.UniqueName}Opt, curr%s{ap.UniqueName}Opt) with"
                        w.printfn "            | struct (ValueSome prevValue, ValueSome currValue) when prevValue = currValue -> ()"
                        w.printfn $"            | struct (_, ValueSome currValue) -> target.SetValue(%s{data.FullName}.%s{ap.Name}Property, %s{ap.ConvertModelToValue} currValue)"
                        w.printfn $"            | struct (ValueSome _, ValueNone) -> target.ClearValue(%s{data.FullName}.%s{ap.Name}Property)"
                        w.printfn "            | _ -> ()"
                        
                printUpdateBaseIfNeeded "            " false

            w.printfn "        | _ ->"
            printUpdateBaseIfNeeded "            " true
        
        w.printfn ""
        w

        
    let generateUpdateFunction (data: UpdateData) (w: StringWriter) =
        let generateProperties (properties: UpdateProperty array) =
            for p in properties do
                let hasApply = not (System.String.IsNullOrWhiteSpace(p.ConvertModelToValue)) || not (System.String.IsNullOrWhiteSpace(p.UpdateCode))
                let attributeKey = getAttributeKey p.CustomAttributeKey p.UniqueName

                // Check if the property is a collection
                match p.CollectionDataElementType with 
                | Some collectionDataElementType when not hasApply ->
                    w.printfn $"        Collections.updateChildren prev%s{p.UniqueName}Opt curr%s{p.UniqueName}Opt target.%s{p.Name}"
                    w.printfn $"            (fun x -> x.Create() :?> %s{collectionDataElementType})"
                    w.printfn "            Collections.updateChild"
                    w.printfn $"            (fun prevChildOpt currChild targetChild -> curr.UpdateAttachedPropertiesForAttribute(%s{attributeKey}, prevChildOpt, currChild, targetChild))"
                    
                | Some _ when hasApply ->
                    w.printfn $"        %s{p.UpdateCode} prev%s{p.UniqueName}Opt curr%s{p.UniqueName}Opt target"
                    w.printfn $"            (fun prevChildOpt currChild targetChild -> curr.UpdateAttachedPropertiesForAttribute(%s{attributeKey}, prevChildOpt, currChild, targetChild))"

                | _ ->
                    // If the type is ViewElement, then it's a type from the model
                    // Issue recursive calls to "Create" and "UpdateIncremental"
                    if p.ModelType = "ViewElement" && not hasApply then
                        w.printfn $"        match struct (prev%s{p.UniqueName}Opt, curr%s{p.UniqueName}Opt) with"
                        w.printfn "        // For structured objects, dependsOn on reference equality"
                        w.printfn "        | struct (ValueSome prevValue, ValueSome newValue) when identical prevValue newValue -> ()"
                        w.printfn "        | struct (ValueSome prevValue, ValueSome newValue) when canReuseView prevValue newValue ->"
                        w.printfn $"            newValue.UpdateIncremental(prevValue, target.%s{p.Name})"
                        w.printfn "        | struct (_, ValueSome newValue) ->"
                        w.printfn $"            target.%s{p.Name} <- (newValue.Create() :?> %s{p.OriginalType})"
                        w.printfn "        | struct (ValueSome _, ValueNone) ->"
                        w.printfn $"            target.%s{p.Name} <- null"
                        w.printfn "        | struct (ValueNone, ValueNone) -> ()"

                    // Explicit update code
                    elif not (System.String.IsNullOrWhiteSpace(p.UpdateCode)) then
                        w.printfn $"        %s{p.UpdateCode} prev%s{p.UniqueName}Opt curr%s{p.UniqueName}Opt target"

                    else
                        w.printfn $"        match struct (prev%s{p.UniqueName}Opt, curr%s{p.UniqueName}Opt) with"
                        w.printfn "        | struct (ValueSome prevValue, ValueSome currValue) when prevValue = currValue -> ()"
                        w.printfn $"        | struct (_, ValueSome currValue) -> target.%s{p.Name} <- %s{p.ConvertModelToValue} currValue"
                        if p.DefaultValue = "" then
                            w.printfn $"        | struct (ValueSome _, ValueNone) -> target.ClearValue %s{data.FullName}.%s{p.Name}Property"
                        else
                            w.printfn $"        | struct (ValueSome _, ValueNone) -> target.%s{p.Name} <- %s{p.DefaultValue}"
                        w.printfn "        | struct (ValueNone, ValueNone) -> ()"
        
        w.printfn $"    static member Update%s{data.Name} (prevOpt: ViewElement voption, curr: ViewElement, target: %s{data.FullName}) = "

        if data.ImmediateMembers.Length = 0 then
            if data.BaseName.IsNone then
                w.printfn "        ()"
            else
                w.printfn $"        ViewBuilders.Update%s{data.BaseName.Value}(prevOpt, curr, target)"
        else
            if data.ImmediateMembers.Length > 0 then
                for m in data.ImmediateMembers do
                    w.printfn $"        let mutable prev%s{m.UniqueName}Opt = ValueNone"
                    w.printfn $"        let mutable curr%s{m.UniqueName}Opt = ValueNone"
                w.printfn "        for kvp in curr.AttributesKeyed do"
                for m in data.ImmediateMembers do
                    let attributeKey = getAttributeKey m.CustomAttributeKey m.UniqueName
                    w.printfn $"            if kvp.Key = %s{attributeKey}.KeyValue then "
                    w.printfn $"                curr%s{m.UniqueName}Opt <- ValueSome (kvp.Value :?> %s{m.ModelType})"
                w.printfn "        match prevOpt with"
                w.printfn "        | ValueNone -> ()"
                w.printfn "        | ValueSome prev ->"
                w.printfn "            for kvp in prev.AttributesKeyed do"
                for m in data.ImmediateMembers do
                    let attributeKey = getAttributeKey m.CustomAttributeKey m.UniqueName
                    w.printfn $"                if kvp.Key = %s{attributeKey}.KeyValue then "
                    w.printfn $"                    prev%s{m.UniqueName}Opt <- ValueSome (kvp.Value :?> %s{m.ModelType})"
            
            // Unsubscribe previous event handlers
            if data.Events.Length > 0 then
                w.printfn "        // Unsubscribe previous event handlers"
                for e in data.Events do
                    let relatedProperties =
                        e.RelatedProperties
                        |> Array.map (fun p -> $"(identicalVOption prev%s{p}Opt curr%s{p}Opt)")
                        |> Array.fold (fun a b -> a + " && " + b) ""

                    w.printfn $"        let shouldUpdate%s{e.UniqueName} = not ((identicalVOption prev%s{e.UniqueName}Opt curr%s{e.UniqueName}Opt)%s{relatedProperties})"
                    w.printfn $"        if shouldUpdate%s{e.UniqueName} then"
                    w.printfn $"            match prev%s{e.UniqueName}Opt with"
                    w.printfn $"            | ValueSome prevValue -> target.%s{e.Name}.RemoveHandler(prevValue)"
                    w.printfn "            | ValueNone -> ()"

            // Update priority properties
            if data.PriorityProperties.Length > 0 then
                w.printfn "        // Update priority properties"
                generateProperties data.PriorityProperties

            // Update inherited members
            if data.BaseName.IsSome then
                w.printfn "        // Update inherited members"
                w.printfn $"        ViewBuilders.Update%s{data.BaseName.Value}(prevOpt, curr, target)"

            // Update properties
            if data.Properties.Length > 0 then
                w.printfn "        // Update properties"
                generateProperties data.Properties
            
            // Subscribe event handlers
            if data.Events.Length > 0 then
                w.printfn "        // Subscribe new event handlers"
                for e in data.Events do
                    w.printfn $"        if shouldUpdate%s{e.UniqueName} then"
                    w.printfn $"            match curr%s{e.UniqueName}Opt with"
                    w.printfn $"            | ValueSome currValue -> target.%s{e.Name}.AddHandler(currValue)"
                    w.printfn "            | ValueNone -> ()"
                
        w.printfn ""
        w

    let generateConstruct (data: ConstructData option) (w: StringWriter) =
        match data with
        | None -> ()
        | Some data ->
            let memberNewLine = "\n                                  " + String.replicate data.Name.Length " " + " "
            let space = "\n                               "
            let membersForConstructor =
                data.Members
                |> Array.mapi (fun i m ->
                    let commaSpace = if i = 0 then "" else "," + memberNewLine
                    match m.Name with
                    | "created" -> $"%s{commaSpace}?%s{m.Name}: (%s{data.FullName} -> unit)"
                    | "ref" ->     $"%s{commaSpace}?%s{m.Name}: ViewRef<%s{data.FullName}>"
                    | _ ->         $"%s{commaSpace}?%s{m.Name}: %s{m.InputType}")
                |> Array.fold (+) ""
            let membersForBuild =
                data.Members
                |> Array.map (fun m ->
                    match m.Name with
                    | "created" -> $",%s{space}?%s{m.Name}=(match %s{m.Name} with None -> None | Some createdFunc -> Some (fun (target: obj) ->  createdFunc (unbox<%s{data.FullName}> target)))"
                    | "ref" ->     $",%s{space}?%s{m.Name}=(match %s{m.Name} with None -> None | Some (ref: ViewRef<%s{data.FullName}>) -> Some ref.Unbox)"
                    | _ ->         $",%s{space}?%s{m.Name}=%s{m.Name}")
                |> Array.fold (+) ""

            w.printfn $"    static member inline Construct%s{data.Name}(%s{membersForConstructor}) = "
            w.printfn ""
            w.printfn $"        let attribBuilder = ViewBuilders.Build%s{data.Name}(0%s{membersForBuild})"
            w.printfn ""
            w.printfn $"        ViewElement.Create<%s{data.FullName}>(ViewBuilders.Create%s{data.Name}, (fun prev curr target -> ViewBuilders.Update%s{data.Name}(prev, curr, target)), (fun key prev curr target -> ViewBuilders.Update%s{data.Name}AttachedProperties(key, prev, curr, target)), attribBuilder)"
            w.printfn ""


    let generateBuilders (data: BuilderData array) (w: StringWriter) =
        w.printfn "type ViewBuilders() ="
        for typ in data do
            w
            |> generateBuildFunction typ.Build
            |> generateCreateFunction typ.Create
            |> generateUpdateAttachedPropertiesFunction typ.UpdateAttachedProperties
            |> generateUpdateFunction typ.Update
            |> generateConstruct typ.Construct
        w

    let generateViewers (data: ViewerData array) (w: StringWriter) =
        for typ in data do
            let genericConstraint =
                match typ.GenericConstraint with
                | None -> ""
                | Some constr -> $"<%s{constr}>"
            
            w.printfn $"/// Viewer that allows to read the properties of a ViewElement representing a %s{typ.Name}"
            w.printfn $"type %s{typ.ViewerName}%s{genericConstraint}(element: ViewElement) ="

            match typ.InheritedViewerName with
            | None -> ()
            | Some inheritedViewerName ->
                let inheritedGenericConstraint =
                    match typ.InheritedGenericConstraint with
                    | None -> ""
                    | Some constr -> $"<%s{constr}>"
                
                w.printfn $"    inherit %s{inheritedViewerName}%s{inheritedGenericConstraint}(element)"

            w.printfn "    do if not ((typeof<%s>).IsAssignableFrom(element.TargetType)) then failwithf \"A ViewElement assignable to type '%s' is expected, but '%%s' was provided.\" element.TargetType.FullName" typ.FullName typ.FullName
            for m in typ.Members do
                match m.Name with
                | "Created" | "Ref" -> ()
                | _ ->
                    let attributeKey = getAttributeKey m.CustomAttributeKey m.UniqueName
                    w.printfn $"    /// Get the value of the %s{m.Name} member"
                    w.printfn $"    member this.%s{m.Name} = element.GetAttributeKeyed(%s{attributeKey})"
            w.printfn ""
        w

    let generateConstructors (data: ConstructorData array) (w: StringWriter) =
        w.printfn "[<AbstractClass; Sealed>]"
        w.printfn "type View private () ="

        for d in data do
            let memberNewLine = "\n                         " + String.replicate d.Name.Length " " + " "
            let space = "\n                               "
            let membersForConstructor =
                d.Members
                |> Array.mapi (fun i m ->
                    let commaSpace = if i = 0 then "" else "," + memberNewLine
                    match m.Name with
                    | "created" -> $"%s{commaSpace}?%s{m.Name}: (%s{d.FullName} -> unit)"
                    | "ref" ->     $"%s{commaSpace}?%s{m.Name}: ViewRef<%s{d.FullName}>"
                    | _ ->         $"%s{commaSpace}?%s{m.Name}: %s{m.InputType}")
                |> Array.fold (+) ""
            let membersForConstruct =
                d.Members
                |> Array.mapi (fun i m ->
                    let commaSpace = if i = 0 then "" else "," + space
                    $"%s{commaSpace}?%s{m.Name}=%s{m.Name}")
                |> Array.fold (+) ""

            w.printfn $"    /// Describes a %s{d.Name} in the view"
            w.printfn $"    static member inline %s{d.Name}(%s{membersForConstructor}) ="
            w.printfn ""
            w.printfn $"        ViewBuilders.Construct%s{d.Name}(%s{membersForConstruct})"
            w.printfn ""
        w.printfn ""
        w
    
    let generateViewExtensions (data: ViewExtensionsData array) (w: StringWriter) : StringWriter =
        let newLine = "\n                                       "

        w.printfn "[<AutoOpen>]"
        w.printfn "module ViewElementExtensions = "
        w.printfn ""
        w.printfn "    type ViewElement with"

        for m in data do
            match m.UniqueName with
            | "Created" | "Ref" -> ()
            | _ ->
                let attributeKey = getAttributeKey m.CustomAttributeKey m.UniqueName
                w.printfn ""
                w.printfn $"        /// Adjusts the %s{m.UniqueName} property in the visual element"
                w.printfn $"        member x.%s{m.UniqueName}(value: %s{m.InputType}) = x.WithAttribute(%s{attributeKey}, %s{m.ConvertInputToModel}(value))"

        let members =
            data
            |> Array.filter (fun m -> m.UniqueName <> "Created" && m.UniqueName <> "Ref")
            |> Array.mapi (fun index m -> sprintf "%s%s?%s: %s" (if index > 0 then ", " else "") (if index > 0 && index % 5 = 0 then newLine else "") m.LowerUniqueName m.InputType)
            |> Array.fold (+) ""

        w.printfn ""
        w.printfn $"        member inline viewElement.With(%s{members}) ="
        for m in data do
            match m.UniqueName with
            | "Created" | "Ref" -> ()
            | _ -> w.printfn $"            let viewElement = match %s{m.LowerUniqueName} with None -> viewElement | Some opt -> viewElement.%s{m.UniqueName}(opt)"
        w.printfn "            viewElement"
        w.printfn ""

        for m in data do
            match m.UniqueName with
            | "Created" | "Ref" -> ()
            | _ ->
                w.printfn $"    /// Adjusts the %s{m.UniqueName} property in the visual element"
                w.printfn $"    let %s{m.LowerUniqueName} (value: %s{m.InputType}) (x: ViewElement) = x.%s{m.UniqueName}(value)"
        w
        
    let generateCodeForAttributes data =
        let toString (w: StringWriter) = w.ToString()
        use writer = new StringWriter()
        
        writer
        |> generateNamespace data.Namespace data.AdditionalNamespaces
        |> generateAttributes data.Attributes
        |> toString
        
    let generateCodeForBuilders data =
        let toString (w: StringWriter) = w.ToString()
        use writer = new StringWriter()
        
        writer
//        |> generateNamespace data.Namespace data.AdditionalNamespaces
//        |> generateBuilders data.Builders
//        |> generateViewers data.Viewers
//        |> generateConstructors data.Constructors
//        |> generateViewExtensions data.ViewExtensions
        |> toString

    let generateCode
        (prepareData: BoundModel -> GeneratorData)
        (generateForAttributes: GeneratorData -> string)
        (generateForBuilders: GeneratorData -> string)
        (bindings: BoundModel) : WorkflowResult<string * string> =
        
        bindings
        |> prepareData
        |> (fun data ->
            let attributes = generateForAttributes data
            let builders = generateForBuilders data
            (attributes, builders)
        )
        |> WorkflowResult.ok
