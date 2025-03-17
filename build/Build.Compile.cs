using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.MSBuild;
using Serilog;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;

internal partial class Build
{
    private Target Compile => _ => _
         .TriggeredBy(Cleaning)
         .Executes(() =>
         {
             var configurations = GetConfigurations(BuildConfiguration, InstallerConfiguration);
             configurations.ForEach(configuration =>
             {
                 Log.Debug("Building configuration: {configuration}", configuration);
                 Log.Information("Building configuration: {configuration}", configuration);
                 try
                 {
                     MSBuild(s => s
                         .SetTargets("Rebuild")
                         .SetProcessToolPath(MsBuildPath.Value)
                         .SetConfiguration(configuration)
                         .SetVerbosity(MSBuildVerbosity.Diagnostic) // Increased verbosity
                         .DisableNodeReuse()
                         .EnableRestore());
                 }
                 catch (ProcessException ex)
                 {
                     Log.Error("MSBuild failed with an exception");
                     Log.Error(ex, ex.Message);
                     throw;
                 }
             });
         });
}
