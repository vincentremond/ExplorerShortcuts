module Program

open System
open ExplorerShortcuts.Common

[<EntryPoint>]
let main _ =

    let possiblePaths =
        [
            @"C:\Program Files"
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            </> @"Programs"
        ]
        |> List.map (fun x -> x </> @"Microsoft VS Code\Code.exe")

    VsCodeBasedEditorOpener.main "c" "Visual Studio Code" possiblePaths
