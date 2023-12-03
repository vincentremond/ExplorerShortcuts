@ECHO OFF

dotnet tool restore
dotnet build -- %*

add-to-path .\ExplorerShortcuts\bin\Debug
