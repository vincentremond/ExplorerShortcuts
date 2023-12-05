namespace ExplorerShortcuts.Common

open System.IO

[<AutoOpen>]
module Path =
    let (</>) a b = Path.Combine(a, b)

[<RequireQualifiedAccess>]
module Directory =
    let tryPath path =
        let directoryInfo = DirectoryInfo path

        match directoryInfo.Exists with
        | true -> Some directoryInfo
        | false -> None
