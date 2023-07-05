open System
open System.IO
open Common

[<EntryPoint>]
[<STAThread>]
let main _ =

    let (StartDirectory strStartDirectory) as startDirectory =
        StartDirectory.CurrentDirectory

    let appDataDirectory =
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)

    let forkSubPath = @"Fork\Fork.exe"
    let forkPath = Path.Combine(appDataDirectory, forkSubPath)

    Process.startAndForget startDirectory (Executable forkPath) [| strStartDirectory |]

    0 // return an integer exit code
