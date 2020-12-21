open System
open System.Diagnostics
open System.Windows.Forms

[<EntryPoint>]
[<STAThread>]
let main argv =
    try
        let proc =
            new Process(StartInfo =
                ProcessStartInfo
                    (FileName = @"C:\Program Files\Microsoft VS Code\Code.exe",
                     UseShellExecute = false,
                     Arguments = $"""{Environment.CurrentDirectory}""",
                     WorkingDirectory = Environment.CurrentDirectory,
                     WindowStyle = ProcessWindowStyle.Normal,
                     LoadUserProfile = true,
                     CreateNoWindow = false))

        proc.Start() |> ignore
    with
    | ex -> MessageBox.Show(ex.ToString()) |> ignore
    0
