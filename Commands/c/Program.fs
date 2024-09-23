module Program

open System
open System.IO
open Fargo
open Fargo.Operators
open ExplorerShortcuts.Common
open Pinicola.FSharp

[<RequireQualifiedAccess>]
type CliOptions =
    | OpenFile
    | OpenCurrentDirectory

type CommandLineOptions =
    | OpenFile of FileName: string * Line: int32 * Column: int32 option
    | OpenCurrentDirectory

let commandLineParser =

    let tryParseInt (str: string) =
        match Int32.TryParse str with
        | true, i -> Ok i
        | false, _ -> Error "Failed to parse int"

    fargo {
        match!
            (cmd "open" "o" "Open a file in Visual Studio Code" |>> CliOptions.OpenFile)
            <|> (ret CliOptions.OpenCurrentDirectory)
        with
        | CliOptions.OpenFile ->
            let! line =
                opt "line" "l" "line" "The line number to open the file at"
                |> optParse tryParseInt
                |> reqOpt

            let! column =
                opt "column" "c" "column" "The column number to open the file at"
                |> optParse tryParseInt

            let! file = opt "file" "f" "file" "The file to open" |> reqOpt
            return OpenFile(file, line, column)
        | CliOptions.OpenCurrentDirectory -> return OpenCurrentDirectory

    }

[<EntryPoint>]
let main _ =

    FargoCmdLine.run
        "c"
        commandLineParser
        (fun options ->
            try

                let possiblePaths =
                    [
                        @"C:\Program Files"
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                        </> @"Programs"
                    ]
                    |> List.map (fun x -> x </> @"Microsoft VS Code\Code.exe")

                let vsCodePath =
                    possiblePaths
                    |> List.tryFind File.Exists
                    |> Option.defaultWith (fun _ -> failwith $"Visual Studio Code not found (%A{possiblePaths})")

                match options with
                | OpenFile(fileName, line, column) ->

                    let directory = Path.GetDirectoryName(fileName)

                    let gotoArgValue =
                        match column with
                        | Some column -> $"%s{fileName}:%d{line}:%d{column}"
                        | None -> sprintf $"%s{fileName}:%d{line}"

                    let startDirectory = StartDirectory.With directory

                    Process.startAndForget startDirectory (Executable vsCodePath) [|
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

                    Process.startAndForget startDirectory (Executable vsCodePath) [| arg |]
            with e ->
                MessageBox.show "error" (e.ToString())
                File.WriteAllText($"c.error.{DateTimeOffset.Now:yyyyMMddHHmmssfff}.txt", e.ToString())
                reraise ()
        )

    0
