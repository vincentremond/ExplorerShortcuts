open System
open System.IO
open Common

[<EntryPoint>]
[<STAThread>]
let main _ =

    let currentDirectory = Directory.GetCurrentDirectory()

    let appDataDirectory =
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)

    let forkSubPath = @"Fork\Fork.exe"
    let forkPath = Path.Combine(appDataDirectory, forkSubPath)

    Process.start (StartDirectory currentDirectory) (Executable forkPath) [| currentDirectory |]

    0 // return an integer exit code
