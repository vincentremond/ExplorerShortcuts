namespace Common

open System.Diagnostics
open System.Runtime.InteropServices

type Executable = Executable of string

type StartDirectory =
    | StartDirectory of string

    static member CurrentDirectory = StartDirectory System.Environment.CurrentDirectory

module Process =
    let startAndForget (StartDirectory workingDirectory) (Executable path) arguments =
        let arguments = String.concat " " arguments

        Process.Start(ProcessStartInfo(path, arguments, WorkingDirectory = workingDirectory))
        |> ignore

    let getOutput (StartDirectory workingDirectory) (Executable path) arguments =
        let arguments = String.concat " " arguments

        let psi =
            ProcessStartInfo(
                path,
                arguments,
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            )

        let p = Process.Start(psi)
        let output = p.StandardOutput.ReadToEnd()
        let error = p.StandardError.ReadToEnd()
        p.WaitForExit()

        if p.ExitCode <> 0 || String.isNotNullOrEmpty error then
            failwithf
                $"Process exited with code %d{p.ExitCode}.\n----\nError:\n%s{error}.\n----\nOutput:\n%s{output}\n----\n"

        output

    let openUrlInBrowser (url: string) =
        ProcessStartInfo("cmd", $"/c start %s{url}", UseShellExecute = true, CreateNoWindow = true)
        |> Process.Start
        |> ignore
