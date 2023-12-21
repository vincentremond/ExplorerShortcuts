namespace ExplorerShortcuts.Common

open System.IO

[<AutoOpen>]
module Path =
    let (</>) a b = Path.Combine(a, b)
