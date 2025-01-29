open System.Diagnostics
open System.IO
open System.Xml.Linq

let inkscape = @"C:\Program Files\Inkscape\bin\inkscape.exe"
let magick = "magick.exe"
let (</>) a b = Path.Combine(a, b)

let sizes = [
    16
    24
    32
    48
    64
    128
    256
    512
]

let dir d =
    Directory.CreateDirectory(d) |> _.FullName

let tempDir = __SOURCE_DIRECTORY__ </> ".." </> "temp-icons"
let targetDirSvg = tempDir </> "svg" |> dir
let targetDirPng = tempDir </> "png" |> dir

let pngExport svgPath pngPath size =
    let arguments = [
        $"--export-width={size}"
        $"--export-height={size}"
        "--export-filename"
        pngPath
        svgPath
    ]

    let processStartInfo =
        ProcessStartInfo(inkscape, arguments, UseShellExecute = false, CreateNoWindow = true)

    let process_ = Process.Start(processStartInfo)
    process_.WaitForExit()

let icoExport (pngPaths: string list) (icoPath: string) : unit =
    let arguments = [
        "convert"
        for pngPath in pngPaths do
            pngPath
        icoPath
    ]

    let processStartInfo =
        ProcessStartInfo(magick, arguments, UseShellExecute = false, CreateNoWindow = true)

    let process_ = Process.Start(processStartInfo)
    process_.WaitForExit()

let svgNamespace = XNamespace.Get("http://www.w3.org/2000/svg")

let (!) n =
    XName.Get(n, svgNamespace.NamespaceName)

let rect width height fill =
    string (
        XDocument(
            XElement(
                !"svg",
                [
                    XAttribute("width", width)
                    XAttribute("height", height)
                ],
                [
                    XElement(
                        !"rect",
                        [
                            XAttribute("x", 0)
                            XAttribute("y", 0)
                            XAttribute("width", width)
                            XAttribute("height", height)
                            XAttribute("fill", fill)
                        ]
                    )
                ]
            )
        )
    )

let svg icon fontFamily xFix yFix =

    string (
        XDocument(
            XElement(
                !"svg",
                [
                    XAttribute("width", 512)
                    XAttribute("height", 512)
                ],
                [
                    XElement(
                        !"rect",
                        [
                            XAttribute("width", 512)
                            XAttribute("height", 512)
                            XAttribute("rx", 100)
                            XAttribute("ry", 100)
                            XAttribute("fill", "#072448")
                        ]
                    )
                    XElement(
                        !"text",
                        [
                            XAttribute("x", (256 + 5 * xFix).ToString())
                            XAttribute("y", (282 + 5 * yFix).ToString())
                            XAttribute("text-anchor", "middle")
                            XAttribute("dominant-baseline", "middle")
                            XAttribute("fill", "white")
                            XAttribute("font-size", 300)
                            XAttribute("font-family", fontFamily)
                        ],
                        [ XText($"{icon}") ]
                    )
                ]
            )
        )
    )

type IconSet =
    | Solid
    | Brand

let icons = [
    Solid, '\uf121', +0, +0, "c"
    Solid, '\uf246', +0, +0, "csr"
    Solid, '\uf07c', +0, +0, "e"
    Solid, '\ue13b', +0, +0, "fork"
    Solid, '\uf1d3', +0, +0, "repo"
    Solid, '\uf6f0', +0, +0, "rider"
    Solid, '\uf0e7', +0, +0, "tmp"
    Solid, '\uf121', +0, +0, "vs"
]

for set, icon, xFix, yFix, name in icons do

    let pathSvg = targetDirSvg </> $"{name}.svg"

    printfn $"Generating %s{pathSvg}"

    let fontFamily =
        match set with
        | Solid -> "Font Awesome 6 Free"
        | Brand -> "Font Awesome 6 Brands"

    let rendered = svg icon fontFamily xFix yFix

    File.WriteAllText(pathSvg, rendered)

    let pngPaths =
        sizes
        |> List.map (fun size ->
            let pathPng = targetDirPng </> $"%s{name}-%03i{size}.png"
            printfn $"Exporting %s{pathPng}"
            pngExport pathSvg pathPng size
            pathPng
        )

    let icoPath =
        __SOURCE_DIRECTORY__ </> ".." </> "Commands" </> name </> $"%s{name}.ico"

    printfn $"Exporting %s{icoPath}"
    icoExport pngPaths icoPath

printfn "Done"
