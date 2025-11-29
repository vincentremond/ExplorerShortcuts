module VsCodeBasedEditorOpener

open System
open System.IO
open Fargo
open Fargo.Operators
open ExplorerShortcuts.Common
open Pinicola.FSharp.Fargo
open Pinicola.FSharp.SpectreConsole

[<RequireQualifiedAccess>]
type CliOptions =
    | OpenFile
    | OpenDirectory
    | OpenCurrentDirectory

type CommandLineOptions =
    | OpenFile of FilePath: string * Line: int32 * Column: int32 option * Directory: string option
    | OpenDirectory of FolderPath: string
    | OpenCurrentDirectory

let commandLineParser editorName =

    let tryParseInt (str: string) =
        match Int32.TryParse str with
        | true, i -> Ok i
        | false, _ -> Error "Failed to parse int"

    fargo {
        match!
            (cmd "open" "o" $"Open a file in {editorName}" |>> CliOptions.OpenFile)
            <|> (cmd "opendir" null $"Open a directory in {editorName}"
                 |>> CliOptions.OpenDirectory)
            <|> (ret CliOptions.OpenCurrentDirectory)
        with
        | CliOptions.OpenFile ->
            let! file = opt "file" "f" "file" "The file to open" |> reqOpt
            and! directory = opt "directory" "d" "directory" "The directory to open the file in"

            and! line =
                opt "line" "l" "line" "The line number to open the file at"
                |> optParse tryParseInt
                |> defaultValue 0

            and! column =
                opt "column" "c" "column" "The column number to open the file at"
                |> optParse tryParseInt

            return OpenFile(file, line, column, directory)
        | CliOptions.OpenDirectory ->
            let! folderPath = opt "directory" "d" "directory" "The directory to open" |> reqOpt
            return OpenDirectory(folderPath)
        | CliOptions.OpenCurrentDirectory -> return OpenCurrentDirectory

    }

let main shortCutName editorName possiblePaths =

    let openFile editorExePath (fileName: string) directory line column =
        let directory =
            match directory with
            | Some directory -> directory
            | None -> Path.GetDirectoryName(fileName)

        let gotoArgValue =
            match column with
            | Some column -> $"%s{fileName}:%d{line}:%d{column}"
            | None -> sprintf $"%s{fileName}:%d{line}"

        let startDirectory = StartDirectory.With directory

        Process.startAndForget startDirectory (Executable editorExePath) [|
            startDirectory.Value
            "--goto"
            gotoArgValue
        |]

    let openDirectory editorExePath (startDirectory: StartDirectory) =

        let workspaceFile =
            Directory.GetFiles(startDirectory.Value, "*.code-workspace")
            |> Array.tryExactlyOne

        let arg =
            match workspaceFile with
            | Some file -> file
            | None -> startDirectory.Value

        Process.startAndForget startDirectory (Executable editorExePath) [| arg |]

    FargoCmdLine.run
        shortCutName
        (commandLineParser editorName)
        (fun options ->
            try

                let editorExePath =
                    possiblePaths
                    |> List.tryFind File.Exists
                    |> Option.defaultWith (fun _ -> failwith $"{editorName} not found (%A{possiblePaths})")

                AnsiConsole.markupLineInterpolated $"Opening {editorName} at [bold]{editorExePath}[/]"

                match options with
                | OpenFile(fileName, line, column, directory) ->

                    openFile editorExePath fileName directory line column

                | OpenDirectory folderPath ->
                    let startDirectory = StartDirectory.With folderPath
                    openDirectory editorExePath startDirectory
                | OpenCurrentDirectory ->
                    let startDirectory = StartDirectory.CurrentDirectory
                    openDirectory editorExePath startDirectory

            with e ->
                MessageBox.show "error" (e.ToString())

                File.WriteAllText(
                    $"explorershortcuts.{shortCutName}.error.{DateTimeOffset.Now:yyyyMMddHHmmssfff}.txt",
                    e.ToString()
                )

                reraise ()
        )

    0
