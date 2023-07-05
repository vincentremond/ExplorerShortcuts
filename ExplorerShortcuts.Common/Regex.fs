namespace ExplorerShortcuts.Common

open System.Text.RegularExpressions

[<RequireQualifiedAccess>]
module Regex =
    let replace (pattern: string) (replacement: string) (input: string) =
        Regex.Replace(input, pattern, replacement)
