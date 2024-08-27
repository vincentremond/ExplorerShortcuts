module rider.Tests

open NUnit.Framework
open Program
open FsUnit

[<SetUp>]
let Setup () = ()

[<Test>]
[<TestCase("2021.1.3.RD-211.7142.1", 211, 7142, 1, 0)>]
[<TestCase("242.20224.431.0-RD", 242, 20224, 431, 0)>]
let ``VersionParser should be able to parse valid formats``
    version
    expectedMajor
    expectedMinor
    expectedFix
    expectedBuild
    =
    let result = VersionParser.parse version

    match result with
    | Ok(major, minor, fix, build) ->
        major |> should equal expectedMajor
        minor |> should equal expectedMinor
        fix |> should equal expectedFix
        build |> should equal expectedBuild

    | Error errorValue -> Assert.Fail(errorValue)
