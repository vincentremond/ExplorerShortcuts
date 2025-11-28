open System
open System.Diagnostics
open ExplorerShortcuts.Common
open ExplorerShortcuts.Common.SpectreConsole
open Spectre.Console

[<RequireQualifiedAccess>]
module VersionParser =

    open FParsec

    let private (?<|>) left right = (attempt left) <|> right
    let private (<|>?) left right = (left) <|> (attempt right)

    let private publicMajor =
        pint32
        >>= (fun major ->
            if major > 2020 && major < 2100 then
                preturn major
            else
                fail "Major version out of range"
        )

    let private publicMinor = pint32
    let private publicFix = pint32
    let private major = pint32
    let private minor = pint32
    let private fix = pint32
    let private build = pint32

    let oldFormat =
        ((publicMajor .>> skipChar '.')
         >>. (publicMinor .>> skipChar '.')
         >>. (publicFix .>> skipString ".RD-")
         >>. (major .>> skipChar '.')
         .>>. (minor .>> skipChar '.')
         .>>. (fix .>> eof))
        |>> fun ((major, minor), fix) -> (major, minor, fix, 0)

    let newFormat =
        (major .>> skipChar '.')
        .>>. (minor .>> skipChar '.')
        .>>. (fix .>> skipChar '.')
        .>>. (build .>> skipString "-RD")
        |>> fun (((major, minor), fix), build) -> (major, minor, fix, build)

    let parse =
        run (
            choice [
                attempt oldFormat
                newFormat
            ]
        )
        >> (function
        | Success(result, _, _) -> Result.Ok result
        | Failure(msg, _, _) -> Result.Error msg)

[<RequireQualifiedAccess>]
module RiderLocator =

    let getPossibleLocations () = [
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

    let getProductVersion (path: string) =
        let versionInfo = FileVersionInfo.GetVersionInfo path
        versionInfo.ProductVersion

    let parseProductVersion (version: string) =
        match VersionParser.parse version with
        | Ok(major, minor, build, fix) -> (major, minor, build)
        | Error _ -> failwithf $"Failed to parse version: %s{version}"

    let getInstallFolders possibleLocations =
        possibleLocations
        |> List.collect (fun (path, installDirectoryPattern) ->
            match Directory.tryPath path with
            | None -> []
            | Some directory ->
                directory.GetDirectories(installDirectoryPattern)
                |> Seq.map _.FullName
                |> Seq.toList
        )
        |> List.map (fun installPath -> installPath </> "bin" </> "rider64.exe")
        |> List.map (fun path -> (path, getProductVersion path))
        |> List.map (fun (path, version) -> (path, parseProductVersion version))
        |> List.sortByDescending snd
        |> List.toArray

    let getRiderPath () =

        let possibleLocations = getPossibleLocations ()
        let installFolders = getInstallFolders possibleLocations

        match installFolders with
        | [||] -> failwithf $"No JetBrains Rider installation found in %A{possibleLocations}"
        | [| x |] -> x
        | _ ->
            AnsiConsole.Prompt(
                SelectionPrompt<string * (int * int * int)>()
                |> SelectionPrompt.setTitle "Version ?"
                |> SelectionPrompt.pageSize 10
                |> SelectionPrompt.addChoices installFolders
            )

[<RequireQualifiedAccess>]
module SolutionLocator =
    let get dir =
        [|
            "*.sln"
            "*.slnx"
        |]
        |> Array.collect (fun ext -> Directory.getAllFiles ext dir)
        |> Array.filter (fun file ->
            let relativePath = Path.relativePath dir file

            let isPinicolaSubModulePath = relativePath |> String.startsWith "Pinicola.FSharp"

            not isPinicolaSubModulePath

        )

[<EntryPoint>]
let main args =

    let logo = FSharp.Data.LiteralProviders.TextFile.``logo.txt``.Text

    let target =

        match args with
        | [| solutionFile |] -> solutionFile
        | [||] ->
            let currentDirectory = Environment.CurrentDirectory
            let solutionFiles = SolutionLocator.get currentDirectory

            match solutionFiles with
            | [||] ->
                AnsiConsole.MarkupLineInterpolated($"No solution files found in the current directory.")

                match AnsiConsole.Confirm("Do you want to open the current directory in Rider ?") with
                | true -> Environment.CurrentDirectory
                | false -> failwith "No solution files found and user did not confirm to open the current directory."

            | [| x |] -> x
            | _ ->
                AnsiConsole.Prompt(
                    SelectionPrompt<string>()
                    |> SelectionPrompt.setTitle "Solution ?"
                    |> SelectionPrompt.pageSize (10)
                    |> SelectionPrompt.addChoices solutionFiles
                )
        | _ -> failwithf $"Unsupported args {args}"

    AnsiConsole.WriteLine(logo)
    let (path, (major, minor, build)) = RiderLocator.getRiderPath ()

    AnsiConsole.MarkupLineInterpolated($"Using JetBrains Rider [blue]{major}.{minor}.{build}[/] located in : '[bold]{path}[/]'")
    AnsiConsole.MarkupLineInterpolated($"Opening : '[bold]{target}[/]'")

    let psi =
        ProcessStartInfo(path, [| target |], UseShellExecute = true, CreateNoWindow = false)

    Process.Start(psi) |> ignore

    0 // return an integer exit code
