namespace ExplorerShortcuts.Common

open System

[<AutoOpen>]
module StringActivePatterns =
    let (|IsNullOrEmpty|_|) (s: string) =
        if System.String.IsNullOrEmpty(s) then Some() else None

    let (|ContainsCI|_|) (sub: string) (s: string) =
        if s.Contains(sub, StringComparison.InvariantCultureIgnoreCase) then
            Some()
        else
            None

[<RequireQualifiedAccess>]
module String =
    let isNullOrEmpty (s: string) = System.String.IsNullOrEmpty(s)

    let isNotNullOrEmpty = isNullOrEmpty >> not

    let trim (s: string) = s.Trim()

    let replaceAll (replacements: seq<string * string>) (s: string) : string =
        replacements |> Seq.fold (fun s -> s.Replace) s
