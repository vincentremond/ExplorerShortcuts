open System
open System.IO
open Common

[<EntryPoint>]
[<STAThread>]
let main _args =

    let StartDirectory strStartDirectory as startDirectory =
        StartDirectory.CurrentDirectory

    printfn $"Starting Fork from %s{strStartDirectory}"

    let appDataDirectory =
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)

    printfn $"AppData directory is %s{appDataDirectory}"

    let forkSubPath = @"Fork\Fork.exe"
    let forkPath = Path.Combine(appDataDirectory, forkSubPath)
    let forkPathInfo = forkPath |> FileInfo

    printfn $"Fork path is %s{forkPath} (Exists: %b{forkPathInfo.Exists})"
    printf "Starting Fork..."

    Process.startAndForget startDirectory (Executable forkPath) [| strStartDirectory |]

    printfn " done."
    0 // return an integer exit code
