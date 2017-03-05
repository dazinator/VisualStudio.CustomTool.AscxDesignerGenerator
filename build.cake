//////////////////////////////////////////////////////////////////////
// TOOLS
//////////////////////////////////////////////////////////////////////
#tool "nuget:?package=GitVersion.CommandLine"
#tool "nuget:?package=GitReleaseNotes"
#tool "nuget:?package=GithubReleaseCreator" 
#tool "nuget:?package=xunit.runners&version=1.9.2"
#addin nuget:?package=Cake.Git
#addin "Cake.ExtendedNuGet"
#addin "nuget:?package=NuGet.Core&version=2.8.6"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////
var artifactsDir = "./artifacts";
var globalAssemblyFile = "./src/GlobalAssemblyInfo.cs";
var repoBranchName = "master";
var isContinuousIntegrationBuild = !BuildSystem.IsLocalBuild;

var gitVersionInfo = GitVersion(new GitVersionSettings {
   OutputType = GitVersionOutput.Json
});

var nugetVersion = isContinuousIntegrationBuild ? gitVersionInfo.NuGetVersion : "0.0.0";

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////
Setup(context =>
{
    Information("Building v{0}", nugetVersion);    
});

Teardown(context =>
{
    Information("Finished running tasks.");
});

//////////////////////////////////////////////////////////////////////
//  PRIVATE TASKS
//////////////////////////////////////////////////////////////////////

Task("__Default")    
    .IsDependentOn("__SetAppVeyorBuildNumber")
    .IsDependentOn("__Clean")
    .IsDependentOn("__Restore")
    .IsDependentOn("__UpdateAssemblyVersionInformation")
    .IsDependentOn("__Build")
  //.IsDependentOn("__Test")
    .IsDependentOn("__GenerateReleaseNotes")
    .IsDependentOn("__Pack")  
    .IsDependentOn("__PublishNuGetPackages");

Task("__Clean")
    .Does(() =>
{
    CleanDirectory(artifactsDir);
    CleanDirectories("./src/**/bin");
    CleanDirectories("./src/**/obj");
});

Task("__SetAppVeyorBuildNumber")
    .Does(() =>
{
    if (BuildSystem.AppVeyor.IsRunningOnAppVeyor)
    {
        var appVeyorBuildNumber = EnvironmentVariable("APPVEYOR_BUILD_NUMBER");
        var appVeyorBuildVersion = $"{nugetVersion}+{appVeyorBuildNumber}";
        repoBranchName = EnvironmentVariable("APPVEYOR_REPO_BRANCH");
        Information("AppVeyor branch name is " + repoBranchName);
        Information("AppVeyor build version is " + appVeyorBuildVersion);
        BuildSystem.AppVeyor.UpdateBuildVersion(appVeyorBuildVersion);
    }
    else
    {
        Information("Not running on AppVeyor");
    }    
});

Task("__Restore")
    .Does(() => {

    var solutions = GetFiles("./**/*.sln");
    // Restore all NuGet packages.
    foreach(var solution in solutions)
    {
        Information("Restoring {0}", solution);
        NuGetRestore(solution, new NuGetRestoreSettings { NoCache = true });
    }

    });

Task("__UpdateAssemblyVersionInformation")
    .WithCriteria(isContinuousIntegrationBuild)
    .Does(() =>
{
     GitVersion(new GitVersionSettings {
        UpdateAssemblyInfo = true,
        UpdateAssemblyInfoFilePath = globalAssemblyFile
    });

    Information("AssemblyVersion -> {0}", gitVersionInfo.AssemblySemVer);
    Information("AssemblyFileVersion -> {0}", $"{gitVersionInfo.MajorMinorPatch}.0");
    Information("AssemblyInformationalVersion -> {0}", gitVersionInfo.InformationalVersion);
});

Task("__Build")
    .Does(() =>
{
    GetFiles("**/*.sln")
        .ToList()
        .ForEach(slnFile => 
        {     

             MSBuild(slnFile.ToString(), new MSBuildSettings {
    Verbosity = Verbosity.Minimal,
    ToolVersion = MSBuildToolVersion.VS2015,
    Configuration = configuration,
    PlatformTarget = PlatformTarget.MSIL
    });
           
        });  
  
});

Task("__Test")
    .Does(() =>
{

    var testAssemblies = GetFiles($"./src/**/bin/{configuration}/*.Tests.dll");
XUnit(testAssemblies,
     new XUnitSettings {
        HtmlReport = true,
        OutputDirectory = artifactsDir
    });   
   
});

Task("__GenerateReleaseNotes")
    .Does(() =>
{               
    GitReleaseNotes($"{artifactsDir}/ReleaseNotes.md", new GitReleaseNotesSettings {
    WorkingDirectory         = ".",
    Verbose                  = true,       
    RepoBranch               = repoBranchName,    
    Version                  = nugetVersion,
    AllLabels                = true
    });
});

Task("__Pack")
    .Does(() =>
{

    var nuGetPackSettings   = new NuGetPackSettings {
                                Version                 = nugetVersion,                              
                                Symbols                 = true,                                              
                                OutputDirectory         = artifactsDir
                            };
            
   GetFiles("**/*.nuspec")
        .ToList()
        .ForEach(nuspecFile => 
        {           
            var projectDir = nuspecFile.GetDirectory();
            if(!projectDir.FullPath.Contains("Tests"))
            {
                Information("Packing {0}", nuspecFile.FullPath);
                NuGetPack($"{nuspecFile.FullPath}", nuGetPackSettings);                          
            }            
        });    
              
});




Task("__PublishNuGetPackages")
    .Does(() =>
{              

           if(isContinuousIntegrationBuild)
           {

                var feed = new
                {
                    Name = "NuGetOrgFeed",
                    Source = EnvironmentVariable("PUBLIC_NUGET_FEED_SOURCE")
                };
            
              

                var apiKey = EnvironmentVariable("NuGetOrgApiKey");

                 GetFiles($"{artifactsDir}/*.{nugetVersion}.nupkg")
                .ToList()
                .ForEach(nugetPackageToPublish => 
                     {        
                       Information("Pushing {0}",nugetPackageToPublish.FullPath);   
                        //if(!nugetPackageToPublish.FullPath.Contains(excludeProjectFromPublish))
                        //{
                         // Push the package. NOTE: this also pushes the symbols package alongside.
                        NuGetPush($"{nugetPackageToPublish.FullPath}", new NuGetPushSettings {
                        Source = feed.Source,
                        ApiKey = apiKey
                        });                     
                     //}
                     });                     
            }

            });


          


//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////
Task("Default")
    .IsDependentOn("__Default");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////
RunTarget(target);