open System
open System.IO
open ExplorerShortcuts.Common
open FSharpPlus

[<EntryPoint>]
[<STAThread>]
let main _ =

    let possiblePaths =
        [
            @"C:\Program Files"
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) </> @"Programs"
        ]
        |> List.map (fun x ->
            x
            </> @"Microsoft VS Code\Code.exe"
        )

    let vsCodePath =
        possiblePaths
        |> List.tryFind File.Exists
        |> Option.defaultWith (fun _ -> failwith $"Visual Studio Code not found (%A{possiblePaths})")

    let StartDirectory strStartDirectory as startDirectory =
        StartDirectory.CurrentDirectory

    let arg =
        strStartDirectory
        |> String.replace "\"" "\\\""
        |> sprintf "\"%s\""

    Process.startAndForget startDirectory (Executable vsCodePath) [| arg |]

    0 // return an integer exit code
