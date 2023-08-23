open System
open ExplorerShortcuts.Common
open FSharpPlus

[<EntryPoint>]
[<STAThread>]
let main _ =
    let vsCodePath = @"C:\Program Files\Microsoft VS Code\Code.exe"

    let (StartDirectory strStartDirectory) as startDirectory =
        StartDirectory.CurrentDirectory

    let arg =
        strStartDirectory
        |> String.replace "\"" "\\\""
        |> sprintf "\"%s\""

    Process.startAndForget startDirectory (Executable vsCodePath) [| arg |]

    0 // return an integer exit code
