module Program

open System
open System.Diagnostics
open System.IO
open ExplorerShortcuts.Common
open Newtonsoft.Json
open Pinicola.FSharp
open Pinicola.FSharp.SpectreConsole
open Spectre.Console

type Maybe<'a> =
    | Yes of 'a
    | No

type DotnetProjectLanguage =
    | FSharp
    | CSharp

    member this.asString =
        match this with
        | FSharp -> "fsharp"
        | CSharp -> "csharp"

type Location =
    | Default
    | Personal

type Editor =
    | VisualStudioCode
    | Cursor

type PromptResult = {
    Subject: string
    CreateDotnetProject: Maybe<DotnetProjectLanguage * bool>
    Location: Location
    Editor: Editor
}

let userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
let preferredPersonalLocations = [ userProfile </> "Perso" </> "tmp" ]

let preferredDefaultLocations =

    list {
        yield @"G:\My Drive\TMP"
        yield (userProfile </> "TMP")
        yield @"D:\TMP\"

        yield!
            (userProfile
             |> Directory.getDirectories "OneDrive*"
             |> Seq.map (fun onedrive -> onedrive </> "TMP"))
    }

let select title (choices: 'a seq) =
    let selectionPrompt = SelectionPrompt()
    selectionPrompt.Title <- title
    selectionPrompt.AddChoices(choices) |> ignore
    let choice = AnsiConsole.Prompt(selectionPrompt)
    AnsiConsole.MarkupLineInterpolated($"[grey]{title}[/] [blue]{choice}[/]")
    choice

let displayPrompt () =
    Console.Title <- "TMP"

    AnsiConsole.Background <- Color.DarkBlue
    AnsiConsole.Foreground <- Color.White
    AnsiConsole.Clear()
    let figletText = FigletText(SpectreConsole.FigletFont.Lean, "TMP")
    figletText.Color <- Color.Yellow
    figletText.Justification <- Justify.Center
    let panel = Panel(figletText)
    panel.BorderStyle <- Style.Parse("grey")

    AnsiConsole.Write(panel)

    let subject = AnsiConsole.Ask<string>("What are you working on today?")

    let createDotnetProject =
        select "Create a new .NET project ?" [
            No
            (Yes FSharp)
            (Yes CSharp)
        ]

    let createDotnetProject =
        match createDotnetProject with
        | No -> No
        | Yes lang ->
            let withUnitTests =
                select "With unit tests ?" [
                    false
                    true
                ]

            Yes(lang, withUnitTests)

    let location =
        select "Where do you want to create your notes ?" [
            Location.Default
            Location.Personal
        ]

    let editor =
        select "Which editor do you want to use ?" [
            Editor.Cursor
            Editor.VisualStudioCode
        ]

    {
        Subject = subject
        CreateDotnetProject = createDotnetProject
        Location = location
        Editor = editor
    }

[<EntryPoint>]
let main _ =

    let promptResult = displayPrompt ()

    let locationSource =
        match promptResult.Location with
        | Default -> preferredDefaultLocations
        | Personal -> preferredPersonalLocations

    let tmpLocation =
        locationSource
        |> List.tryPick Directory.tryPath
        |> Option.defaultWith (fun _ -> failwith $"Could not find preferred location %A{locationSource}")
        |> _.FullName

    let thisMonth = DateTimeOffset.Now.ToString("yyyy-MM")
    let today = DateTimeOffset.Now.ToString("yyyy-MM-dd")

    let fixedName =
        promptResult.Subject |> Regex.replace @"[^\w\d]" "-" |> Regex.replace "-+" "-"

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
        $"# {promptResult.Subject}"
        ""
        $"Date: _{today}_"
        ""
        "**your notes here**"
    ]
    |> (File.writeAllLines notesFilePath)

    let workspaceContents =
        {|
            folders = [ {| path = "." |} ]
            settings = {| ``window.title`` = promptResult.Subject |}
        |}
        |> (fun x -> JsonConvert.SerializeObject(x, Formatting.Indented))

    let workspaceFile = newTmpFolder </> $"_{fixedName}_.code-workspace"

    File.writeAllText workspaceFile workspaceContents
    File.hide workspaceFile

    let editorExecutable =
        match promptResult.Editor with
        | VisualStudioCode -> "c.exe"
        | Cursor -> "csr.exe"

    let startInfo =
        ProcessStartInfo(
            editorExecutable,
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

    match promptResult.CreateDotnetProject with
    | No -> ()
    | Yes(projectLanguage, withUnitTests) ->

        Rule() |> Rule.withTitle "Create a new .NET project" |> AnsiConsole.write

        let targetFolder = newTmpFolder </> fixedName

        Directory.CreateDirectory(targetFolder) |> ignore

        let dotnetStartInfo =
            ProcessStartInfo(
                "InitProject.exe",
                [
                    if not withUnitTests then
                        "--no-test-project"
                    "--lang"
                    projectLanguage.asString
                ]
            )

        dotnetStartInfo.WorkingDirectory <- targetFolder

        let _dotnetProcess = Process.Start(dotnetStartInfo)
        let _dotnetExited = _dotnetProcess.WaitForExit()

        AnsiConsole.Confirm("Press enter to exit ?", defaultValue = true) |> ignore

    0
