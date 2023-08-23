namespace ExplorerShortcuts.Common

open System.Text.RegularExpressions

[<RequireQualifiedAccess>]
module Regex =
    let replace (pattern: string) (replacement: string) (input: string) =
        Regex.Replace(input, pattern, replacement)

    let replaceAll (replacements: (string * string) seq) (input: string) =
        replacements
        |> Seq.fold (fun acc (pattern, replacement) -> replace pattern replacement acc) input
