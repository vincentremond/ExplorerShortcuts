open System
open System.Diagnostics
open System.IO
open ExplorerShortcuts.Common
open Spectre.Console

let preferredLocations = [
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
    </> "TMP"
    @"D:\TMP\"
]

let location =
    preferredLocations
    |> List.tryPick Directory.tryPath
    |> Option.defaultWith (fun _ -> failwith $"Could not find preferred location %A{preferredLocations}")

let displayPrompt () =
    Console.Title <- " ... TMP ... "

    AnsiConsole.Background <- Color.DarkBlue
    AnsiConsole.Foreground <- Color.White
    AnsiConsole.Clear()
    AnsiConsole.Write(FigletText(SpectreConsole.FigletFont.AnsiShadow, "TMP"))

    AnsiConsole.Ask<string>("What are you working on today?")

let fixName n =
    n
    |> Regex.replace @"[^\w\d]" "-"
    |> Regex.replace "-+" "-"

let name = displayPrompt ()
let today = DateTime.Now.ToString("yyyy-MM-dd")
let fixedName = fixName name
let folderName = $"{today}--{fixedName}"

let folder =
    location.FullName
    </> folderName

Directory.CreateDirectory(folder)
|> ignore

let notes = folder </> "notes.md"

[
    $"# {name}"
    ""
    $"_{today}_"
    ""
    ""
    ""
]
|> (File.writeAllLines notes)

let workspaceContents = """
{
	"folders": [
		{
			"path": "."
		}
	],
	"settings": {}
}
"""
    
let workspaceFile = folder </> $"_{fixedName}..code-workspace"
File.writeAllText workspaceFile workspaceContents 

let startInfo =
    ProcessStartInfo("cmd.exe", $"/c code.cmd \"{workspaceFile}\" --goto notes.md:5:0")

startInfo.WorkingDirectory <- folder
startInfo.WindowStyle <- ProcessWindowStyle.Hidden
startInfo.UseShellExecute <- false
let _process = Process.Start(startInfo)
()
