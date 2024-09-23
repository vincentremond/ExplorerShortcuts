open System
open System.IO
open ExplorerShortcuts.Common

[<EntryPoint>]
[<STAThread>]
let main _args =

    let startDirectory = StartDirectory.CurrentDirectory

    printfn $"Starting Fork from %s{startDirectory.Value}"

    let appDataDirectory =
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)

    printfn $"AppData directory is %s{appDataDirectory}"

    let forkSubPath = @"Fork\Fork.exe"
    let forkPath = Path.Combine(appDataDirectory, forkSubPath)
    let forkPathInfo = forkPath |> FileInfo

    printfn $"Fork path is %s{forkPath} (Exists: %b{forkPathInfo.Exists})"
    printf "Starting Fork..."

    Process.startAndForget startDirectory (Executable forkPath) [| $@"""%s{startDirectory.Value}""" |]

    printfn " done."
    0 // return an integer exit code
