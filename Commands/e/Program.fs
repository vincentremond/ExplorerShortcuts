module Program

open System
open ExplorerShortcuts.Common
open System.IO

[<EntryPoint>]
[<STAThread>]
let main args =

    let folder =
        StartDirectory.With
        <| match args with
           | [| "." |] -> Directory.GetCurrentDirectory()
           | [| path |] -> path
           | [||] -> Directory.GetCurrentDirectory()
           | _ -> failwith "Invalid arguments"

    Process.startAndForget (folder) (Executable "explorer.exe") [| folder.Value |]

    0 // return an integer exit code
