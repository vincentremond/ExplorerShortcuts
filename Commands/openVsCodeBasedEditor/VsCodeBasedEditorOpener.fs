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
    | OpenCurrentDirectory

type CommandLineOptions =
    | OpenFile of FileName: string * Line: int32 * Column: int32 option * Directory: string option
    | OpenCurrentDirectory

let commandLineParser editorName =

    let tryParseInt (str: string) =
        match Int32.TryParse str with
        | true, i -> Ok i
        | false, _ -> Error "Failed to parse int"

    fargo {
        match!
            (cmd "open" "o" $"Open a file in {editorName}" |>> CliOptions.OpenFile)
            <|> (ret CliOptions.OpenCurrentDirectory)
        with
        | CliOptions.OpenFile ->
            let! line =
                opt "line" "l" "line" "The line number to open the file at"
                |> optParse tryParseInt
                |> defaultValue 0

            let! column =
                opt "column" "c" "column" "The column number to open the file at"
                |> optParse tryParseInt

            let! directory = opt "directory" "d" "directory" "The directory to open the file in"

            let! file = opt "file" "f" "file" "The file to open" |> reqOpt
            return OpenFile(file, line, column, directory)
        | CliOptions.OpenCurrentDirectory -> return OpenCurrentDirectory

    }

let main shortCutName editorName possiblePaths =

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

                | OpenCurrentDirectory ->

                    let startDirectory = StartDirectory.CurrentDirectory

                    let workspaceFile =
                        Directory.GetFiles(startDirectory.Value, "*.code-workspace")
                        |> Array.tryExactlyOne

                    let arg =
                        match workspaceFile with
                        | Some file -> file
                        | None -> startDirectory.Value

                    Process.startAndForget startDirectory (Executable editorExePath) [| arg |]
            with e ->
                MessageBox.show "error" (e.ToString())

                File.WriteAllText(
                    $"explorershortcuts.{shortCutName}.error.{DateTimeOffset.Now:yyyyMMddHHmmssfff}.txt",
                    e.ToString()
                )

                reraise ()
        )

    0
