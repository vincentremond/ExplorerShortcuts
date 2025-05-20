module Program

open System
open System.IO
open ExplorerShortcuts.Common

try

    // "C:\Users\remond\AppData\Local\Programs\cursor\Cursor.exe"
    let possiblePaths =
        [
            @"C:\Program Files"
            (Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
             </> @"Programs")
        ]
        |> List.map (fun x -> x </> @"cursor\cursor.exe")

    let cursorPath =
        possiblePaths
        |> List.tryFind File.Exists
        |> Option.defaultWith (fun _ -> failwith $"Visual Studio Code not found (%A{possiblePaths})")

    let startDirectory = StartDirectory.CurrentDirectory

    let workspaceFile =
        Directory.GetFiles(startDirectory.Value, "*.code-workspace")
        |> Array.tryExactlyOne

    let arg =
        match workspaceFile with
        | Some file -> file
        | None -> startDirectory.Value

    Process.startAndForget startDirectory (Executable cursorPath) [| arg |]
with e ->
    MessageBox.show "error" (e.ToString())
    File.WriteAllText($"explorershortcuts.cursor.error.{DateTimeOffset.Now:yyyyMMddHHmmssfff}.txt", e.ToString())
    reraise ()
