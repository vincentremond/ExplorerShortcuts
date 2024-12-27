namespace ExplorerShortcuts.Common

open System.IO

[<AutoOpen>]
module Path =
    let (</>) a b = Path.Combine(a, b)

    // let (<//>) a b = b |> List.map (fun b -> a </> b)
    let (<//>) a b = a |> List.map (fun a -> a </> b)

    let (<///>) a b =
        List.allPairs a b |> List.map (fun (a, b) -> (a </> b))

    let relativePath relativeTo path = Path.GetRelativePath(relativeTo, path)

// let (<|/>) a b = a |> List.map (fun a -> a </> b)
