open System
open System.Diagnostics
open ExplorerShortcuts.Common
open ExplorerShortcuts.Common.SpectreConsole
open Spectre.Console

[<RequireQualifiedAccess>]
module VersionParser =

    open FParsec

    let parse =
        run (
            ((pint32 .>> skipChar '.')
             .>>. (pint32 .>> skipChar '.')
             .>>. (pint32 .>> skipString ".RD-")
             .>>. (pint32 .>> skipChar '.')
             .>>. (pint32 .>> skipChar '.')
             .>>. (pint32 .>> eof))
            |>> fun (((((major1, minor1), fix), major2), minor2), build) -> (major2, minor2, build)
        )
        >> (function
        | Success(result, _, _) -> Result.Ok result
        | Failure(msg, _, _) -> Result.Error msg)

[<EntryPoint>]
[<STAThread>]
let main _ =

    let getProductVersion (path: string) =
        let versionInfo = FileVersionInfo.GetVersionInfo path
        versionInfo.ProductVersion

    let parseProductVersion (version: string) =
        match VersionParser.parse version with
        | Ok(major, minor, build) -> (major, minor, build)
        | Error _ -> failwithf $"Failed to parse version: %s{version}"

    let possibleLocations =
        [
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
        ]
        |> Seq.map (fun path -> path </> "JetBrains")

    let installFolders =
        possibleLocations
        |> Seq.collect (Directory.getDirectories "JetBrains Rider *")
        |> Seq.map (fun installPath -> installPath </> "bin" </> "rider64.exe")
        |> Seq.map (fun path -> (path, getProductVersion path))
        |> Seq.map (fun (path, version) -> (path, parseProductVersion version))
        |> Seq.sortByDescending snd
        |> Seq.toArray

    let slnPath = Environment.CurrentDirectory |> Directory.getAllFiles "*.sln"

    AnsiConsole.Write(FigletText(SpectreConsole.FigletFont.AnsiShadow, "JetBrains Rider"))
    AnsiConsole.WriteLine()

    let (path, (major, minor, build)) =
        match installFolders with
        | [||] -> failwithf $"No JetBrains Rider installation found in %A{possibleLocations}"
        | [| x |] -> x
        | _ ->
            AnsiConsole.Prompt(
                SelectionPrompt<string * (int * int * int)>()
                |> SelectionPrompt.setTitle "Version ?"
                |> SelectionPrompt.pageSize (10)
                |> SelectionPrompt.addChoices installFolders
            )

    AnsiConsole.WriteLine()
    AnsiConsole.WriteLine($"Using JetBrains Rider {major}.{minor}.{build} located : '{path}'")

    let solutionFile =
        match slnPath with
        | [||] -> failwithf $"No solution file found in %A{Environment.CurrentDirectory}"
        | [| x |] -> x
        | _ ->
            AnsiConsole.Prompt(
                SelectionPrompt<string>()
                |> SelectionPrompt.setTitle "Solution ?"
                |> SelectionPrompt.pageSize (10)
                |> SelectionPrompt.addChoices slnPath
            )

    AnsiConsole.WriteLine()
    AnsiConsole.WriteLine($"Opening solution file '{solutionFile}'")

    let args = [| solutionFile |] |> Array.map (sprintf "\"%s\"") |> String.concat " "

    let psi =
        ProcessStartInfo(path, args, UseShellExecute = true, CreateNoWindow = false)

    Process.Start(psi) |> ignore

    0 // return an integer exit code
