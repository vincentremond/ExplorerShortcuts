open System
open System.Diagnostics
open ExplorerShortcuts.Common
open ExplorerShortcuts.Common.SpectreConsole
open Spectre.Console

[<EntryPoint>]
[<STAThread>]
let main _args =

    let possibleLocations =
        [
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
        ]
        <//> "Microsoft Visual Studio"
        <///> [
            "2019"
            "2022"
        ]
        <///> [
            "Community"
            "Professional"
            "Enterprise"
        ]
        <//> @"Common7\IDE\devenv.exe"

    let exeLocations =
        possibleLocations
        |> Seq.choose File.tryPath
        |> Seq.map (_.FullName)
        |> Seq.sort
        |> Seq.toArray

    let solutionFiles =
        [|
            "*.sln"
            "*.slnx"
        |]
        |> Array.collect (fun ext -> Directory.getAllFiles ext Environment.CurrentDirectory)

    AnsiConsole.Write(FigletText(SpectreConsole.FigletFont.AnsiShadow, "Visual Studio"))
    AnsiConsole.WriteLine()

    let path =
        match exeLocations with
        | [||] -> failwithf $"No Visual Studio installation found in\n%A{possibleLocations}"
        | [| path |] -> path
        | _ ->
            AnsiConsole.Prompt(
                SelectionPrompt<string>()
                |> SelectionPrompt.setTitle "Version ?"
                |> SelectionPrompt.pageSize 10
                |> SelectionPrompt.addChoices exeLocations
            )

    AnsiConsole.WriteLine()
    AnsiConsole.WriteLine($"Using Visual Studio located : '{path}'")

    let solutionFile =
        match solutionFiles with
        | [||] -> failwithf $"No solution file found in %A{Environment.CurrentDirectory}"
        | [| x |] -> x
        | _ ->
            AnsiConsole.Prompt(
                SelectionPrompt<string>()
                |> SelectionPrompt.setTitle "Solution ?"
                |> SelectionPrompt.pageSize 10
                |> SelectionPrompt.addChoices solutionFiles
            )

    AnsiConsole.WriteLine()
    AnsiConsole.WriteLine($"Opening solution file '{solutionFile}'")

    let args = [| solutionFile |] |> Array.map (sprintf "\"%s\"") |> String.concat " "

    let psi =
        ProcessStartInfo(path, args, UseShellExecute = true, CreateNoWindow = false)

    Process.Start(psi) |> ignore

    0 // return an integer exit code
