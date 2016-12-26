#r "tools/FAKE/tools/FakeLib.dll"
open Fake
open Fake.AssemblyInfoFile

// Properties

let buildDir = "./build"
let nugetDir = "./nuget"
let releaseNotesFile = "./ReleaseNotes.md"
let projectName = "ReactiveBluetooth"
let solutionAssemblyInfo = "./AssemblyInfo.cs"
let nuspecFile = "./ReactiveBluetooth.nuspec"

let releaseNotes =
    ReadFile releaseNotesFile
    |> ReleaseNotesHelper.parseReleaseNotes

// Target
Target "Clean" (fun _ ->
    CleanDir buildDir
)

Target "RestorePackages" (fun _ ->
    RestorePackages()
)

Target "BuildRelease" (fun _ ->
    MSBuildRelease buildDir "Build" !! ("ReactiveBluetooth*/*.csproj")
        |> Log "ReleaseBuild:"
)

Target "AssemblyInfo" (fun _ -> 
    CreateCSharpAssemblyInfo solutionAssemblyInfo
            [Attribute.Product projectName
             Attribute.Version releaseNotes.AssemblyVersion
             Attribute.FileVersion releaseNotes.AssemblyVersion]
)

Target "CreateNugetPackages" (fun _ -> 
    CopyFiles nugetDir !! (buildDir + "/ReactiveBluetooth*.dll")
    NuGet (fun p -> 
        {p with
            Project = projectName
            Version = releaseNotes.NugetVersion
            Publish = false
            Files= [
                    ("ReactiveBluetooth.*", Some "lib\\portable-net45+netcore45+wpa81\\", Some "**\\*.pdb;*iOS*;*Android*" )
                    ("ReactiveBluetooth.*", Some "lib\\Xamarin.iOS1.0\\", Some "**\\*.pdb;*Android*" )
                    ("ReactiveBluetooth.*", Some "lib\\MonoAndroid1.0\\", Some "**\\*.pdb;*iOS*" )
            ]
        }) nuspecFile

)

// Dependencies
"Clean"
    ==> "RestorePackages"
    ==> "AssemblyInfo"
    ==> "BuildRelease"
    ==> "CreateNugetPackages"
    
RunTargetOrDefault "CreateNugetPackages"