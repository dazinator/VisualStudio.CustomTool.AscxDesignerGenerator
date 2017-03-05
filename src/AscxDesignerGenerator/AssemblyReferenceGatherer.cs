using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;

namespace AscxDesignerGenerator
{
    public class AssemblyReferenceGatherer
    {

        public IEnumerable<ITaskItem> GetAssemblyReferences(Microsoft.Build.Construction.ProjectRootElement projectRootElement)
        {
            ConsoleLogger logger = new ConsoleLogger(LoggerVerbosity.Normal);
            BuildManager manager = BuildManager.DefaultBuildManager;

            ProjectInstance projectInstance = new ProjectInstance(projectRootElement);
            var result = manager.Build(
                new BuildParameters()
                {
                    DetailedSummary = true,
                    Loggers = new List<ILogger>() { logger }
                },
                new BuildRequestData(projectInstance, new string[]
                {
                    "ResolveProjectReferences",
                    "ResolveAssemblyReferences"
                }));

            var projectReferences = GetAssemblyReferencesList(result, "ResolveProjectReferences");
            var assemblyReferences = GetAssemblyReferencesList(result, "ResolveAssemblyReferences");
            var results = projectReferences.Union(assemblyReferences);
            return results;
        }

        private IList<ITaskItem> GetAssemblyReferencesList(BuildResult result, string targetName)
        {
            var buildResult = result.ResultsByTarget[targetName];
            var buildResultItems = buildResult.Items;

            var results = new List<ITaskItem>();
            if (buildResultItems.Length == 0)
            {
                Console.WriteLine("No refereces detected in target {0}.", targetName);
                return results;
            }

            foreach (var item in buildResultItems)
            {
                results.Add(item);
            }

            return results;
        }
    }
}
