namespace Fabulous

type LinkerSafeAttribute () =
    inherit System.Attribute()

/// Mark Fabulous as safe for linking
/// This means users selecting "Link framework SDKs only" will get a fully tree-shaken Fabulous.dll
[<assembly:LinkerSafe>]
do()