using DnnProjectSystem.Generator.Ascx;
using DnnProjectSystem.Logging;
using Microsoft.VisualStudio.Shell;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace AscxDesignerGenerator
{
    /// <summary>
    /// Summary description for ResourceKeysGenerator
    /// </summary>   
    [ComVisible(true)]
    [Guid(AscxDesignerGeneratorGuid)]
    public class AscxDesignerClassGenerator : BaseCodeGeneratorWithSite
    {

        /// <summary>
        /// The GUID for this package.
        /// </summary>
        public const string AscxDesignerGeneratorGuid = "3f003da2-e4da-40aa-9e6f-9e5d9c1b57cc";

        protected override byte[] GenerateCode(string fileContents)
        {

            var project = GetVSProject().Project;
            var activeConfig = project.ConfigurationManager.ActiveConfiguration;

            var outputPath = activeConfig.Properties.Item("OutputPath").Value.ToString();
            var projDir = project.GetProjectPath();

            //   string outputPath = outputPathProp.Value.ToString();
            string outputDir = Path.Combine(projDir, outputPath);
            var outputFileNameProp = project.Properties.Item("OutputFileName");
            string outputFileName = outputFileNameProp.Value.ToString();
            var outputFilepath = Path.Combine(outputDir, outputFileName);
           
            //return assemblyPath;





            try
            {

                var results = ThreadHelper.JoinableTaskFactory.Run(async delegate
                 {
                     //  await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                     var serviceProvider = this.SiteServiceProvider;
                     var logger = new ActivityLogger(serviceProvider, "AscxDesignerClassGenerator");
                     var ascxCompiler = new AscxDesignerCompiler(logger);

                     var projectItem = this.GetProjectItem();
                     var task = ascxCompiler.GenerateDesignerFile(projectItem, fileContents, outputFilepath);

                     var result = await task;
                     //ThreadHelper.JoinableTaskFactory.Run(_=>task)

                     //= task.Result;
                     return result;
                     // You're now on the UI thread.
                 });

                return results;
            }
            catch (Exception e)
            {
                return null;
            }

            //var codeProvider = GetCodeProvider();
            // codeProvider
            // return null;

        }


        protected override string GetDefaultExtension()
        {
            return ".ascx.designer.cs";
        }

    }
}
