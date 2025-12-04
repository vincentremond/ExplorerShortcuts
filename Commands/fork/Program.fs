open System
open ExplorerShortcuts.Common
open Fargo
open Pinicola.FSharp.SpectreConsole
open Pinicola.FSharp.IO
open Pinicola.FSharp.Fargo

let cliParser =
    fargo {
        let! verbose = flag "verbose" "v" "Verbose output"
        return {| Verbose = verbose |}
    }

FargoCmdLine.run
    "fork"
    cliParser
    (fun args ->

        let startDirectory = StartDirectory.CurrentDirectory

        if args.Verbose then
            AnsiConsole.markupLineInterpolated $"Starting Fork from [bold]{startDirectory.Value}[/]"

        let appDataDirectory =
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)

        if args.Verbose then
            AnsiConsole.markupLineInterpolated $"AppData directory is [bold]{appDataDirectory}[/]"

        let possibleLocations = [
            appDataDirectory </> "Fork" </> "current"
            appDataDirectory </> "Fork"
        ]

        let location = possibleLocations |> Path.tryFindFileInLocations "Fork.exe"

        let forkPath =
            match location with
            | Some path -> path
            | None -> failwith $"Fork.exe not found in {possibleLocations}"

        if args.Verbose then
            AnsiConsole.markup "Starting Fork... "

        Process.startAndForget startDirectory (Executable forkPath) [| startDirectory.Value |]

        if args.Verbose then
            AnsiConsole.markupLine " [green]✔[/] done."
    )
