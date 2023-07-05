open System
open Common

[<EntryPoint>]
[<STAThread>]
let main _ =

    let remoteUrl =
        Process.getOutput StartDirectory.CurrentDirectory (Executable "git") [|
            "config"
            "--get"
            "remote.origin.url"
        |]
        |> String.trim

    printfn $"Opening %s{remoteUrl}"

    // open in browser
    Process.openUrlInBrowser remoteUrl

    0 // return an integer exit code
