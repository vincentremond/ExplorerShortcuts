module ExplorerShortcuts.Common.SpectreConsole

[<RequireQualifiedAccess>]
module FigletFont =
    open Spectre.Console
    open FSharp.Data.LiteralProviders

    [<RequireQualifiedAccess>]
    module FontsSources =
        let AnsiShadow = TextFile.``ANSI-Shadow.flf``.Text

    let AnsiShadow =

        FigletFont.Parse(FontsSources.AnsiShadow)
