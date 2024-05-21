open System
open ExplorerShortcuts.Common
open System.IO

[<EntryPoint>]
[<STAThread>]
let main args =

    let folder =
        match args with
        | [| "." |] -> Directory.GetCurrentDirectory()
        | [| path |] -> path
        | [||] -> Directory.GetCurrentDirectory()
        | _ -> failwith "Invalid arguments"

    Process.startAndForget (StartDirectory folder) (Executable "explorer.exe") [| folder |]

    0 // return an integer exit code
