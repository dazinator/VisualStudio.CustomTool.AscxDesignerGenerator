using System;
using EnvDTE;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Build.Framework;
using System.Text;
using DnnProjectSystem.Logging;
using AscxDesignerGenerator;

namespace DnnProjectSystem.Generator.Ascx
{

    public class AscxDesignerCompiler : ICompileContext
    {
        private IActivityLogger _logger;

        private int _filenameCount;

        /// <summary>
        /// How many hyphens to add before verbose messages.
        /// </summary>
        public int VerboseNesting { get; set; }

        public AscxDesignerCompiler(IActivityLogger logger)
        {
            _logger = logger;
        }

        public async System.Threading.Tasks.Task<byte[]> GenerateDesignerFile(ProjectItem projectItem, string contents, string outputAssemblyPath)
        {
            // get list of referenced assemblies.
            var project = projectItem.ContainingProject;
            var unconfiguredProject = project.GetUnconfiguredProject();
            var lockService = unconfiguredProject.ProjectService.Services.ProjectLockService;

            var configuredProject = await unconfiguredProject.GetSuggestedConfiguredProjectAsync();

            List<ITaskItem> assemblyReferenceItems;
            // need to gather the referenced assemblies.
            using (var access = await lockService.WriteLockAsync())
            {
                Microsoft.Build.Evaluation.Project msBuildProject = await access.GetProjectAsync(configuredProject);

                // party on it, respecting the type of lock you've acquired. 
                var assemblyReferenceGatherer = new AssemblyReferenceGatherer();
                assemblyReferenceItems = assemblyReferenceGatherer.GetAssemblyReferences(msBuildProject.Xml).ToList();
            }

            var projDir = project.GetProjectPath();

            var referencedAssemblies = new ReferencedAssembliesContext(assemblyReferenceItems, outputAssemblyPath);


            string currentDir = System.IO.Path.GetDirectoryName(project.FullName);
            string websiteRootPath = currentDir; // todo - this could point to a dnn website we are targeting?      

            List<TagRegistration> tagRegistrations = referencedAssemblies.StandardTagRegistrations.ToList();
            UserControlTagRegistrationResolver.ResolveUserControls(this, tagRegistrations, referencedAssemblies, websiteRootPath, currentDir);

            Verbose("Begin processing \"{0}\"...", projectItem.Name);
            Verbose("");
            VerboseNesting++;
            BeginFile(projectItem.Name);

            //bool succeeded = GenerateDesignerForFilename(document.FullName, tagRegistrations, referencedAssemblies, websiteRootPath);

            bool succeeded = false;
            // var filename = document.FullName;

            string designer = null;
            // string designerFilename = filename + ".designer.cs";

            // Load the markup from the .aspx or .ascx file.
            MarkupReader markup = new MarkupReader();
            MarkupInfo markupInfo = null;
            try
            {
                markupInfo = markup.LoadMarkup(this, tagRegistrations, referencedAssemblies, websiteRootPath, contents, projectItem.Name);
            }
            catch (Exception e)
            {
                Error("{0}: Failed to load markup file:\r\n{1}", projectItem.Name, e.Message);
                Verbose("Stopping file processing due to exception.  Stack trace:\r\n{0}", e.StackTrace);
                succeeded = false;
                return null;
            }

            // If we're not inheriting a real class, there's no reason for a designer file to exist.
            if (string.IsNullOrWhiteSpace(markupInfo.InheritsClassName))
            {
                Verbose("Skipping generating designer file because markup does not have an Inherits=\"...\" attribute.", projectItem.Name);
                succeeded = true;
                return null;
            }

            // Generate the output text for the new .designer.cs file.
            try
            {
                DesignerWriter designerWriter = new DesignerWriter();
                designer = designerWriter.CreateDesigner(this, markupInfo);
            }
            catch (Exception e)
            {
                Error("{0}: Cannot regenerate designer file:\r\n{1}", projectItem.Name, e.Message);
                Verbose("Stopping file processing due to exception.  Stack trace:\r\n{0}", e.StackTrace);
                succeeded = false;
                return null;
            }

            // Save the output .designer.cs file to disk.

            var utfBytes = Encoding.UTF8.GetBytes(designer);
            EndFile(projectItem.Name, succeeded);
            VerboseNesting--;
            Verbose("");
            Verbose("End processing \"{0}\".", projectItem.Name);

            return utfBytes;

        }


        //protected virtual bool IsHandled(Document document)
        //{
        //    var extension = System.IO.Path.GetExtension(document.FullName);
        //    var isAscxFile = extension.ToLowerInvariant() == ".ascx";

        //    if (isAscxFile)
        //    {
        //        var project = document.ProjectItem.ContainingProject;
        //        var vsHierarchy = project.ToHierarchy();
        //        bool isCpsProject = PackageUtilities.IsCapabilityMatch(vsHierarchy, MyUnconfiguredProject.UniqueCapability);
        //        return isCpsProject;
        //    }

        //    // files is either not ascx, or is is an ascx, but the project does not have DnnProjectSystem capability.
        //    return false;
        //}

        #region ICompileContext

        public void BeginTask(int filenameCount)
        {
            _filenameCount = filenameCount;
        }

        public void BeginFile(string filename)
        {
            if (_filenameCount > 1)
            {
                _logger.LogInfo(filename + "...").Wait();
            }

            return;
        }

        public void EndFile(string filename, bool succeeded)
        {
            if (!succeeded)
            {
                _logger.LogInfo(string.Empty).Wait();
            }
        }

        public void Verbose(string format, params object[] args)
        {
            if (string.IsNullOrEmpty(format))
            {
                _logger.LogInfo(string.Empty);
            }
            else
            {
                var prependedMessage = (string.Format("{0} {1}", RepeatString("-", VerboseNesting + 1), format));
                _logger.LogInfo(prependedMessage, args);
            }
        }

        public void Warning(string format, params object[] args)
        {
            _logger.LogWarning(format, args);
        }

        public void Error(string format, params object[] args)
        {
            _logger.LogError(format, args);
        }

        /// <summary>
        /// Given a string, return another string where the original is repeated 'count' times.
        /// </summary>
        public static string RepeatString(string str, int count)
        {
            switch (count)
            {
                case 0: return string.Empty;
                case 1: return str;
                case 2: return str + str;
                case 3: return str + str + str;

                default:
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < count; i++)
                    {
                        stringBuilder.Append(str);
                    }
                    return stringBuilder.ToString();
            }
        }

        #endregion
    }
    
    



}
