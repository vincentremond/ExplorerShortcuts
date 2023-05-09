open System
open Common

[<EntryPoint>]
[<STAThread>]
let main _ =
    let vsCodePath = @"C:\Program Files\Microsoft VS Code\Code.exe"
    let currentDirectory = Environment.CurrentDirectory

    Process.start (StartDirectory currentDirectory) (Executable vsCodePath) [| currentDirectory |]

    0 // return an integer exit code
