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
        |> List.collect (fun x ->
            [
                x </> @"Antigravity IDE\Antigravity IDE.exe"
                x </> @"Antigravity\Antigravity.exe"
                x </> @"Antigravity\Antigravity IDE.exe"
            ]
        )

    VsCodeBasedEditorOpener.main "a" "Antigravity IDE" possiblePaths
