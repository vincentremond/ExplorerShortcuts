module Program

open System
open ExplorerShortcuts.Common
open LibGit2Sharp
open Spectre.Console

let dir = StartDirectory.CurrentDirectory.Value |> Repository.Discover

AnsiConsole.MarkupLineInterpolated($"Found git repository at [green]{dir}[/]")

// get remote url
let repo = new Repository(dir)

let currentBranch = repo.Head

let remoteUri =
    repo.Network.Remotes |> Seq.head |> _.Url |> Regex.replaceAll [ @"\.git$", "/" ]

let remoteUriHost = remoteUri |> Uri |> (_.Host)

let urlToOpen =
    match remoteUriHost with
    | ContainsCI "gitlab" ->
        AnsiConsole.MarkupLineInterpolated($"Found gitlab host : [green]{remoteUriHost}[/]")
        $"%s{string remoteUri}-/tree/%s{currentBranch.FriendlyName}"
    | ContainsCI "github" ->
        AnsiConsole.MarkupLineInterpolated($"Found github host : [green]{remoteUriHost}[/]")
        $"%s{string remoteUri}tree/%s{currentBranch.FriendlyName}"
    | ContainsCI "visualstudio.com" ->
        AnsiConsole.MarkupLineInterpolated($"Found azure devops url : [green]{remoteUriHost}[/]")
        $"%s{string remoteUri}?version=GB{currentBranch.FriendlyName}"
    | _ ->
        AnsiConsole.MarkupLineInterpolated($"Found unknown url host : [yellow]{remoteUriHost}[/]")
        $"%s{string remoteUri}"

AnsiConsole.MarkupLineInterpolated($"Opening [green]{urlToOpen}[/]")

// open in browser
Process.openUrlInBrowser urlToOpen
