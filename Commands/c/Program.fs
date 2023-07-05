open System
open Common

[<EntryPoint>]
[<STAThread>]
let main _ =
    let vsCodePath = @"C:\Program Files\Microsoft VS Code\Code.exe"

    let (StartDirectory strStartDirectory) as startDirectory =
        StartDirectory.CurrentDirectory

    Process.startAndForget startDirectory (Executable vsCodePath) [| strStartDirectory |]

    0 // return an integer exit code
