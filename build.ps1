$ErrorActionPreference = "Stop"

dotnet tool restore
dotnet build

AddToPath .\ExplorerShortcuts\bin\Debug
