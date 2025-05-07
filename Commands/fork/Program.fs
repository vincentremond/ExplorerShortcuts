open System
open ExplorerShortcuts.Common
open Pinicola.FSharp.SpectreConsole
open Pinicola.FSharp.IO

let startDirectory = StartDirectory.CurrentDirectory

AnsiConsole.markupLineInterpolated $"Starting Fork from [bold]{startDirectory.Value}[/]"

let appDataDirectory =
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)

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

AnsiConsole.markup "Starting Fork... "

Process.startAndForget startDirectory (Executable forkPath) [| startDirectory.Value |]

AnsiConsole.markupLine " [green]✔[/] done."
