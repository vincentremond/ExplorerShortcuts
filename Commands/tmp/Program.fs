open System
open System.IO
open Common

open ExplorerShortcuts.Common

let fixedName n =
    n
    |> Regex.replace @"[^\w\d]" "-"
    |> Regex.replace "-+" "-"

printf "Name : "
let name = Console.ReadLine()
let today = DateTime.Now.ToString("yyyy-MM-dd")
let folderName = $"{today}--{fixedName name}"
let folder = @"D:\TMP\" </> folderName
let directoryInfo = Directory.CreateDirectory(folder)
let notes = folder </> "notes.md"
let notesContent = $"# {name}\r\n\r\n_{today}_\r\n\r\n"
File.WriteAllText(notes, notesContent)
Process.startAndForget (StartDirectory folder) (Executable "cmd.exe") [ "/c"; "\"--goto notes.md:5:0\"" ]
