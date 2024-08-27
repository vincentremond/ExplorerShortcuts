open System
open System.Diagnostics
open System.IO
open ExplorerShortcuts.Common
open Newtonsoft.Json
open Pinicola.FSharp.SpectreConsole
open Spectre.Console

let preferredLocations =
    let userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)

    seq {
        yield (userProfile </> "TMP")
        yield @"D:\TMP\"

        yield!
            (userProfile
             |> Directory.getDirectories "OneDrive*"
             |> Seq.map (fun onedrive -> onedrive </> "TMP"))
    }
    |> List.ofSeq

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

    let project = AnsiConsole.Ask<string>("What are you working on today?")

    let createDotnetProject =
        AnsiConsole.Confirm("Create a new .NET project?", defaultValue = false)

    (project, createDotnetProject)

let fixName n =
    n |> Regex.replace @"[^\w\d]" "-" |> Regex.replace "-+" "-"

let name, createDotnetProject = displayPrompt ()
let today = DateTime.Now.ToString("yyyy-MM-dd")
let fixedName = fixName name
let folderName = $"{today}--{fixedName}"

let folder = location.FullName </> folderName

Directory.CreateDirectory(folder) |> ignore

let notes = folder </> "notes.md"

[
    $"# {name}"
    ""
    $"Date: _{today}_"
    ""
    "**your notes here**"
]
|> (File.writeAllLines notes)

let workspaceContents =
    {|
        folders = [ {| path = "." |} ]
        settings = {| ``window.title`` = name |}
    |}
    |> (fun x -> JsonConvert.SerializeObject(x, Formatting.Indented))

let workspaceFile = folder </> $"_{fixedName}_.code-workspace"

File.writeAllText workspaceFile workspaceContents
File.hide workspaceFile

let startInfo =
    ProcessStartInfo("cmd.exe", $"/c code.cmd \"{workspaceFile}\" --goto notes.md:5:0")

startInfo.WorkingDirectory <- folder
startInfo.WindowStyle <- ProcessWindowStyle.Hidden
startInfo.UseShellExecute <- true
let _process = Process.Start(startInfo)
let _exited = _process.WaitForExit(TimeSpan.FromSeconds(5.))

if createDotnetProject then

    Rule() |> Rule.withTitle "Create a new .NET project" |> AnsiConsole.write

    let targetFolder = folder </> fixedName

    Directory.CreateDirectory(targetFolder) |> ignore

    let dotnetStartInfo = ProcessStartInfo("InitProject.exe")
    dotnetStartInfo.WorkingDirectory <- targetFolder

    let _dotnetProcess = Process.Start(dotnetStartInfo)
    let _dotnetExited = _dotnetProcess.WaitForExit()

    AnsiConsole.Confirm("Press enter to exit ?", defaultValue = true) |> ignore

()
