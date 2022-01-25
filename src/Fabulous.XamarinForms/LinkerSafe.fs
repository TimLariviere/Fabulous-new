namespace Fabulous.XamarinForms

type LinkerSafeAttribute () =
    inherit System.Attribute()

/// Mark Fabulous.XamarinForms as safe for linking
/// This means users selecting "Link framework SDKs only" will get a fully tree-shaken Fabulous.XamarinForms.dll
[<assembly:LinkerSafe>]
do()