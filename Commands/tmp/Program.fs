module Program

open System
open System.Diagnostics
open System.IO
open ExplorerShortcuts.Common
open Newtonsoft.Json
open Pinicola.FSharp.SpectreConsole
open Spectre.Console

[<RequireQualifiedAccess>]
type CreateDotnetProject =
    | No
    | Yes of Type: string

let preferredLocations =
    let userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)

    seq {
        yield "G:\My Drive\TMP"
        yield (userProfile </> "TMP")
        yield @"D:\TMP\"

        yield!
            (userProfile
             |> Directory.getDirectories "OneDrive*"
             |> Seq.map (fun onedrive -> onedrive </> "TMP"))
    }
    |> List.ofSeq

let tmpLocation =
    preferredLocations
    |> List.tryPick Directory.tryPath
    |> Option.defaultWith (fun _ -> failwith $"Could not find preferred location %A{preferredLocations}")
    |> _.FullName

let displayPrompt () =
    Console.Title <- " ... TMP ... "

    AnsiConsole.Background <- Color.DarkBlue
    AnsiConsole.Foreground <- Color.White
    AnsiConsole.Clear()
    AnsiConsole.Write(FigletText(SpectreConsole.FigletFont.AnsiShadow, "TMP"))

    let project = AnsiConsole.Ask<string>("What are you working on today?")

    // let createDotnetProject =
    //     AnsiConsole.Confirm("Create a new .NET project?", defaultValue = false)

    let selectionPrompt = SelectionPrompt()
    selectionPrompt.Title <- "Create a new .NET project?"

    selectionPrompt.AddChoices(
        [
            CreateDotnetProject.No
            (CreateDotnetProject.Yes "fsharp")
            (CreateDotnetProject.Yes "csharp")
        ]
    )
    |> ignore

    let createDotnetProject = AnsiConsole.Prompt(selectionPrompt)

    (project, createDotnetProject)

let name, createDotnetProject = displayPrompt ()
let thisMonth = DateTime.Now.ToString("yyyy-MM")
let today = DateTime.Now.ToString("yyyy-MM-dd")
let fixedName = name |> Regex.replace @"[^\w\d]" "-" |> Regex.replace "-+" "-"
let newFolderName = $"{today}--{fixedName}"

// Create month folder if it doesn't exist
let monthFolder = tmpLocation </> thisMonth

if not (Directory.Exists monthFolder) then
    Directory.CreateDirectory(monthFolder) |> ignore

let newTmpFolder = tmpLocation </> newFolderName
Directory.CreateDirectory(newTmpFolder) |> ignore


let notesFileName = "notes.md"
let notesFilePath = newTmpFolder </> notesFileName

[
    $"# {name}"
    ""
    $"Date: _{today}_"
    ""
    "**your notes here**"
]
|> (File.writeAllLines notesFilePath)

let workspaceContents =
    {|
        folders = [ {| path = "." |} ]
        settings = {| ``window.title`` = name |}
    |}
    |> (fun x -> JsonConvert.SerializeObject(x, Formatting.Indented))

let workspaceFile = newTmpFolder </> $"_{fixedName}_.code-workspace"

File.writeAllText workspaceFile workspaceContents
File.hide workspaceFile

let startInfo =
    ProcessStartInfo(
        "c.exe",
        [
            "open"
            "--file"
            notesFilePath
            "--line"
            "5"
            "--column"
            "0"
        ]
    )

startInfo.WorkingDirectory <- newTmpFolder
startInfo.WindowStyle <- ProcessWindowStyle.Hidden
startInfo.UseShellExecute <- true
let _process = Process.Start(startInfo)
let _exited = _process.WaitForExit(TimeSpan.FromSeconds(5.))

match createDotnetProject with
| CreateDotnetProject.No -> ()
| CreateDotnetProject.Yes ``type`` ->

    Rule() |> Rule.withTitle "Create a new .NET project" |> AnsiConsole.write

    let targetFolder = newTmpFolder </> fixedName

    Directory.CreateDirectory(targetFolder) |> ignore

    let dotnetStartInfo =
        ProcessStartInfo(
            "InitProject.exe",
            [
                "--lang"
                ``type``
            ]
        )

    dotnetStartInfo.WorkingDirectory <- targetFolder

    let _dotnetProcess = Process.Start(dotnetStartInfo)
    let _dotnetExited = _dotnetProcess.WaitForExit()

    AnsiConsole.Confirm("Press enter to exit ?", defaultValue = true) |> ignore

()
