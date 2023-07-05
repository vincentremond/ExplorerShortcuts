open System
open System.IO
open Common
open FSharpPlus
open ExplorerShortcuts.Common

let displayPrompt () =
    Console.Title <- " >>< TMP ><<"
    Console.BackgroundColor <- ConsoleColor.DarkBlue
    Console.ForegroundColor <- ConsoleColor.White
    Console.Clear()

    [
        ""
        ""
        "                ████████╗███╗   ███╗██████╗ "
        "                ╚══██╔══╝████╗ ████║██╔══██╗"
        "                   ██║   ██╔████╔██║██████╔╝"
        "                   ██║   ██║╚██╔╝██║██╔═══╝ "
        "                   ██║   ██║ ╚═╝ ██║██║     "
        "                   ╚═╝   ╚═╝     ╚═╝╚═╝     "
        ""
        ""

    ]
    |> List.iter (printfn "%s")

    printf "                <name>: "


let fixedName n =
    n |> Regex.replace @"[^\w\d]" "-" |> Regex.replace "-+" "-"

displayPrompt ()
let name = Console.ReadLine()
let today = DateTime.Now.ToString("yyyy-MM-dd")
let folderName = $"{today}--{fixedName name}"
let folder = @"D:\TMP\" </> folderName
let directoryInfo = Directory.CreateDirectory(folder)
let notes = folder </> "notes.md"
[ $"# {name}"; ""; $"_{today}_"; ""; ""; "" ] |> (File.writeAllLines notes)

Process.startAndForget (StartDirectory folder) (Executable "cmd.exe") [
    "/c"
    $"code.cmd \"{folder}\" --goto notes.md:5:0"
]
