namespace ExplorerShortcuts.Common

[<RequireQualifiedAccess>]
module File =
    let writeAllLines (path: string) (lines: string seq) =
        System.IO.File.WriteAllLines(path, lines)

    let writeAllText (path: string) (contents: string) =
        System.IO.File.WriteAllText(path, contents)
