namespace svg2xxx

open System.IO
open ExplorerShortcuts.Common
open Fargo
open Pinicola.FSharp.Fargo
open Pinicola.FSharp.SpectreConsole

module Convert =

    let commandLineParser targetExt =
        fargo {
            let! input = opt "input" "i" "input" "The input .SVG file" |> reqOpt
            and! output = opt "output" "o" "output" $"The output {targetExt} file"
            return (input, output)
        }

    let run commandName (ext: string) =
        AnsiConsole.markupLineInterpolated $"[green]{commandName}[/]"

        FargoCmdLine.run
            commandName
            (commandLineParser (ext.ToUpper()))
            (fun (svgPath, output) ->
                let outputPath =
                    match output with
                    | Some o -> o
                    | None -> Path.ChangeExtension(svgPath, ext)

                let inkscapePossibleLocations = [ @"C:\Program Files\Inkscape\bin" ]

                let inkscape =
                    match Executable.search "inkscape.exe" inkscapePossibleLocations with
                    | None -> failwith $"Inkscape not found in {inkscapePossibleLocations}"
                    | Some i -> i

                AnsiConsole.markupLineInterpolated $"Converting [green]{svgPath}[/] to [green]{outputPath}[/]"

                let arguments = [
                    "--export-filename"
                    outputPath
                    svgPath
                ]

                let exitCode =
                    Process.startAndWait StartDirectory.CurrentDirectory inkscape arguments

                if exitCode <> 0 then
                    failwith $"Inkscape failed with exit code {exitCode}"

                if not (File.Exists outputPath) then
                    failwith $"Output file {outputPath} not found"
                else
                    AnsiConsole.markupLineInterpolated $"[green]{outputPath}[/] created successfully"

                ()
            )

    let rec svg2pdf () = run (nameof svg2pdf) ".pdf"
    let rec svg2png () = run (nameof svg2png) ".png"
    let rec svg2dxf () = run (nameof svg2dxf) ".dxf"
