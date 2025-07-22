module ExplorerShortcuts.Common.SpectreConsole

[<RequireQualifiedAccess>]
module SelectionPrompt =
    open Spectre.Console

    let setTitle title (prompt: SelectionPrompt<'a>) =
        prompt.Title <- title
        prompt

    let pageSize pageSize (prompt: SelectionPrompt<'a>) =
        prompt.PageSize <- pageSize
        prompt

    let addChoices choices (prompt: SelectionPrompt<'a>) = prompt.AddChoices(choices)

[<RequireQualifiedAccess>]
module FigletFont =
    open Spectre.Console
    open FSharp.Data.LiteralProviders

    [<RequireQualifiedAccess>]
    module FontsSources =
        let AnsiShadow = TextFile.``ANSI-Shadow.flf``.Text
        let Lean = TextFile.``Lean.flf``.Text

    let AnsiShadow =

        FigletFont.Parse(FontsSources.AnsiShadow)

    let Lean = FigletFont.Parse(FontsSources.Lean)
