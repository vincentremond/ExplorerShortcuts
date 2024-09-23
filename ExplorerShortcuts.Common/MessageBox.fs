namespace ExplorerShortcuts.Common

open Vanara.PInvoke

[<RequireQualifiedAccess>]
module MessageBox =

    let private expect expected actual =
        if expected <> actual then
            failwithf $"Expected %A{expected} but got %A{actual}"

    let show (caption: string) (message: string) =
        User32.MessageBox(HWND.NULL, message, caption, User32.MB_FLAGS.MB_OK)
        |> expect User32.MB_RESULT.IDOK
