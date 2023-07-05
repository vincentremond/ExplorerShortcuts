namespace ExplorerShortcuts.Common

open System.Text.RegularExpressions

[<RequireQualifiedAccess>]
module File =
    let writeAllLines (path: string) (lines: string seq) =
        System.IO.File.WriteAllLines(path, lines)
