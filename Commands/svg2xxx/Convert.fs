namespace svg2xxx

open System
open System.IO
open System.Threading.Tasks
open ExplorerShortcuts.Common
open Fargo
open Pinicola.FSharp.Fargo
open Pinicola.FSharp.SpectreConsole
open Pinicola.FSharp.IO

module Convert =

    type Target =
        | Pdf
        | Png
        | Dxf
        | Ico

    let commandLineParser targetExt =
        fargo {
            let! input = opt "input" "i" "input" "The input .SVG file" |> reqOpt
            and! output = opt "output" "o" "output" $"The output {targetExt} file"
            return (input, output)
        }

    let inkscapeExport inkscapePath (outputPath: string) (svgPath: string) size =

        let arguments = [
            match size with
            | Some s ->
                $"--export-width=%i{s}"
                $"--export-height=%i{s}"
            | None -> ()
            "--export-filename"
            outputPath
            svgPath
        ]

        let exitCode =
            Process.startAndWait StartDirectory.CurrentDirectory inkscapePath arguments

        if exitCode <> 0 then
            failwith $"Inkscape failed with exit code {exitCode}"

    let icoExport inkscapePath (outputPath: string) (svgPath: string) =

        let magick = Path.findInPathEnvVar "magick.exe" |> Executable

        let sizes = [
            16
            24
            32
            48
            64
            128
            256
            512
        ]

        let tempFolder = Path.GetTempPath()
        let uniqueIdentifier = $"{DateTimeOffset.Now:yyyyMMssHHmmss}_{Guid.NewGuid():N}"

        let pngPaths =
            Progress.init ()
            |> Progress.withColumns [
                Progress.Columns.spinner()
                Progress.Columns.taskDescription()
                Progress.Columns.elapsedTime()
            ]
            |> Progress.withHideCompleted false
            |> Progress.withAutoClear false
            |> Progress.withAutoRefresh true
            |> Progress.runTasks
                sizes
                (fun size -> $"Exporting {size}x{size}")
                (fun size ->
                    let tempFileName = $"{uniqueIdentifier}_{size}.png"
                    let tempFilePath = tempFolder </> tempFileName

                    inkscapeExport inkscapePath tempFilePath svgPath (Some size)
                    tempFilePath
                )

        let magickArguments = [
            "convert"
            for pngPath in pngPaths do
                pngPath
            outputPath
        ]

        let exitCode =
            Process.startAndWait StartDirectory.CurrentDirectory magick magickArguments

        if exitCode <> 0 then
            failwith $"Magick failed with exit code {exitCode}"

        pngPaths |> Seq.iter File.Delete

    let run commandName target =
        AnsiConsole.markupLineInterpolated $"[green]{commandName}[/]"

        let ext =
            match target with
            | Pdf -> ".pdf"
            | Png -> ".png"
            | Dxf -> ".dxf"
            | Ico -> ".ico"

        FargoCmdLine.run
            commandName
            (commandLineParser (ext.ToUpper()))
            (fun (svgPath, output) ->
                let outputPath =
                    match output with
                    | Some o -> o
                    | None -> Path.ChangeExtension(svgPath, ext)

                let inkscapePossibleLocations = [ @"C:\Program Files\Inkscape\bin" ]

                let inkscapePath =
                    match Executable.search "inkscape.exe" inkscapePossibleLocations with
                    | None -> failwith $"Inkscape not found in {inkscapePossibleLocations}"
                    | Some i -> i

                match target with
                | Dxf
                | Png
                | Pdf -> inkscapeExport inkscapePath outputPath svgPath None
                | Ico -> icoExport inkscapePath outputPath svgPath

                AnsiConsole.markupLineInterpolated $"Converting [green]{svgPath}[/] to [green]{outputPath}[/]"

                if not (File.Exists outputPath) then
                    failwith $"Output file {outputPath} not found"
                else
                    AnsiConsole.markupLineInterpolated $"[green]{outputPath}[/] created successfully"

                ()
            )

    let rec svg2pdf () = run (nameof svg2pdf) Pdf
    let rec svg2png () = run (nameof svg2png) Png
    let rec svg2dxf () = run (nameof svg2dxf) Dxf
    let rec svg2ico () = run (nameof svg2ico) Ico
