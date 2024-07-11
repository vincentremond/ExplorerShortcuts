open System
open ExplorerShortcuts.Common
open LibGit2Sharp

let dir = StartDirectory.CurrentDirectory.Value |> Repository.Discover

printfn $"Found git repository at %s{dir}"

// get remote url
let repo = new Repository(dir)

let currentBranch = repo.Head

let remoteUri =
    repo.Network.Remotes
    |> Seq.head
    |> fun r -> r.Url
    |> Regex.replaceAll [ @"\.git$", "/" ]
    |> Uri

let urlToOpen =
    match remoteUri.Host with
    | ContainsCI "gitlab" -> $"%s{string remoteUri}-/tree/%s{currentBranch.FriendlyName}"
    | ContainsCI "github" -> $"%s{string remoteUri}tree/%s{currentBranch.FriendlyName}"
    | _ -> $"%s{string remoteUri}"

printfn $"Opening %s{urlToOpen}"

// open in browser
Process.openUrlInBrowser urlToOpen
