param(
    [Parameter(Mandatory=$true)]
    [string]$Name
)

$CommandsFolder = "Commands"
$FsProjDir = "$CommandsFolder/$Name"
$FsProjPath = "$FsProjDir/$Name.fsproj"
$ProgramPath = "$FsProjDir/Program.fs"
$PaketReferencesPath = "$FsProjDir/paket.references"

New-Item -Path $FsProjDir -ItemType Directory

@"
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <OutputType>WinExe</OutputType>
        <TargetFramework>net8.0</TargetFramework>
    </PropertyGroup>

    <ItemGroup>
        <None Include="paket.references" />
        <Compile Include="Program.fs" />
    </ItemGroup>

    <ItemGroup>
      <ProjectReference Include="..\..\ExplorerShortcuts.Common\ExplorerShortcuts.Common.fsproj" />
    </ItemGroup>

</Project>
"@ | Out-File $FsProjPath

"FS.Core" | Out-File $PaketReferencesPath

$ProgramContents = @"
open System
open ExplorerShortcuts.Common

[<EntryPoint>]
[<STAThread>]
let main _ =
    
    // TODO: Add your code here.

    0 // return an integer exit code
"@ | Out-File $ProgramPath

dotnet sln add $FsProjPath --solution-folder $CommandsFolder

dotnet add .\ExplorerShortcuts\ExplorerShortcuts.fsproj reference $FsProjPath
