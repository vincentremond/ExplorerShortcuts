namespace ExplorerShortcuts.Common

open System.IO

[<RequireQualifiedAccess>]
module File =
    let writeAllLines (path: string) (lines: string seq) =
        File.WriteAllLines(path, lines)

    let writeAllText (path: string) (contents: string) =
        File.WriteAllText(path, contents)
        
    let hide (path: string) =
        let currentAttributes = File.GetAttributes(path)
        let newFileAttributes = FileAttributes.Hidden ||| currentAttributes
        File.SetAttributes(path, newFileAttributes)
