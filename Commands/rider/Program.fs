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
let main args =

    let verbose =
        match args with
        | [| _; "--verbose" |] -> true
        | _ -> false

    let getProductVersion (path: string) =
        let versionInfo = FileVersionInfo.GetVersionInfo path
        versionInfo.ProductVersion

    let parseProductVersion (version: string) =
        match VersionParser.parse version with
        | Ok(major, minor, build) -> (major, minor, build)
        | Error _ -> failwithf $"Failed to parse version: %s{version}"

    let possibleLocations = [
        (Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
         </> "JetBrains",
         "JetBrains Rider *")
        (Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
         </> "JetBrains",
         "JetBrains Rider *")
        (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
         </> @"AppData\Local\Programs",
         "Rider")
    ]

    let installFolders =
        possibleLocations
        |> List.collect (fun (path, installDirectoryPattern) ->
            match Directory.tryPath path with
            | None -> []
            | Some directory ->
                directory.GetDirectories(installDirectoryPattern)
                |> Seq.map (fun dir -> dir.FullName)
                |> Seq.toList
        )
        |> List.map (fun installPath -> installPath </> "bin" </> "rider64.exe")
        |> List.map (fun path -> (path, getProductVersion path))
        |> List.map (fun (path, version) -> (path, parseProductVersion version))
        |> List.sortByDescending snd
        |> List.toArray

    let slnPath = Environment.CurrentDirectory |> Directory.getAllFiles "*.sln"

    let figletText =
        FigletText(SpectreConsole.FigletFont.AnsiShadow, "JetBrains Rider").Centered()

    figletText.Pad <- false
    let panel = Panel(figletText).PadTop(1).PadBottom(0).PadLeft(1).PadRight(1)
    panel.Expand <- false
    panel.Width <- 110
    AnsiConsole.Write(panel)

    let (path, (major, minor, build)) =
        match installFolders with
        | [||] ->
            failwithf
                "No JetBrains Rider installation found in %A"
                (possibleLocations |> List.map (fun (a, b) -> $"{a}\\{b}"))
        | [| x |] -> x
        | _ ->
            AnsiConsole.Prompt(
                SelectionPrompt<string * (int * int * int)>()
                |> SelectionPrompt.setTitle "Version ?"
                |> SelectionPrompt.pageSize (10)
                |> SelectionPrompt.addChoices installFolders
            )

    AnsiConsole.MarkupLineInterpolated(
        $"Using JetBrains Rider [blue]{major}.{minor}.{build}[/] located in : '[bold]{path}[/]'"
    )

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

    AnsiConsole.MarkupLineInterpolated($"Opening solution file '[bold]{solutionFile}[/]'")

    let psi =
        ProcessStartInfo(path, [| solutionFile |], UseShellExecute = true, CreateNoWindow = false)

    Process.Start(psi) |> ignore

    0 // return an integer exit code
