namespace ExplorerShortcuts.Common

[<AutoOpen>]
module Path =
    let (</>) a b = System.IO.Path.Combine(a, b)
