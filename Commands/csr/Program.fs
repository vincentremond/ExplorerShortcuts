module Program

open System
open ExplorerShortcuts.Common

[<EntryPoint>]
let main _ =

    let possiblePaths =
        [
            @"C:\Program Files"
            (Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
             </> @"Programs")
        ]
        |> List.map (fun x -> x </> @"cursor\cursor.exe")

    VsCodeBasedEditorOpener.main "csr" "Cursor" possiblePaths
