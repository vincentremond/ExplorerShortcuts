namespace Common

open System.Diagnostics

type Executable = | Executable of string
type StartDirectory = | StartDirectory of string

module Process =
    let start (StartDirectory workingDirectory) (Executable path) arguments =
        let arguments = String.concat " " arguments
        Process.Start(ProcessStartInfo(path, arguments, WorkingDirectory = workingDirectory)) |> ignore
        