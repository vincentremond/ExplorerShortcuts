namespace ExplorerShortcuts.Common

open System.IO

[<RequireQualifiedAccess>]
module Directory =
    let getDirectories filter path = Directory.GetDirectories(path, filter)

    let getAllFiles pattern path =
        Directory.GetFiles(path, pattern, SearchOption.AllDirectories)

    let tryPath path =
        let directoryInfo = DirectoryInfo path

        match directoryInfo.Exists with
        | true -> Some directoryInfo
        | false -> None
