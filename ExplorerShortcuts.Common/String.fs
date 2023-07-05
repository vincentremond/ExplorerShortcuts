namespace Common

[<RequireQualifiedAccess>]
module String =
    let isNullOrEmpty (s: string) = System.String.IsNullOrEmpty(s)

    let isNotNullOrEmpty = isNullOrEmpty >> not


    let trim (s: string) = s.Trim()
