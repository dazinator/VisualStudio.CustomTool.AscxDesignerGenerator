using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Build.Framework;

namespace AscxDesignerGenerator
{
    public class ReferencedAssembliesContext
    {

        /// <summary>
        /// The complete set of loaded assemblies, organized by assembly name.
        /// </summary>
        private readonly Dictionary<string, Assembly> _loadedAssemblies = new Dictionary<string, Assembly>();

        private readonly List<ITaskItem> _assemblyItems;

        public ReferencedAssembliesContext(List<ITaskItem> assemblyItems, string mainOutputAssemblyPath)
        {
            _assemblyItems = assemblyItems;
            MianOutputAssemblyPath = mainOutputAssemblyPath;
            OutputDirectory = System.IO.Path.GetDirectoryName(mainOutputAssemblyPath);


           // AppDomain domain = AppDomain.CreateDomain("CompilationDomain");



           // AppDomain.Unload(domain);
            // var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            // should perhaps load into seperate app domain, and dispose through idisposable.
            foreach (var item in assemblyItems)
            {
                Assembly assembly;


                if (AllowedAssemblies.IsAllowed(item.ItemSpec))
                {
                    try
                    {
                        assembly = Assembly.ReflectionOnlyLoadFrom(item.ItemSpec);
                    }
                    catch (FileLoadException e)
                    {

                        //todo: check if assembly is loaded.
                        continue;

                        //throw;
                    }

                    var name = assembly.GetName();

                    _loadedAssemblies.Add(name.FullName, assembly);

                    if (item.ItemSpec == mainOutputAssemblyPath)
                    {
                        ProjectOutputAssembly = assembly;
                    }

                    if (name.Name == "System.Web")
                    {
                        SystemWebAssembly = assembly;
                    }
                }
               
            }

            LoadStandardTagRegistrations(SystemWebAssembly);

            if (ProjectOutputAssembly == null && System.IO.File.Exists(mainOutputAssemblyPath))
            {
                var assembly = Assembly.ReflectionOnlyLoadFrom(mainOutputAssemblyPath);
                ProjectOutputAssembly = assembly;
            }
        }

        private void LoadStandardTagRegistrations(Assembly systemWebAssembly)
        {
            if (systemWebAssembly == null)
            {
                StandardTagRegistrations = new List<TagRegistration>();
            }
            else
            {
                StandardTagRegistrations = new List<TagRegistration>
            {
                new TagRegistration
                    {
                        Kind = TagRegistrationKind.Namespace,
                        AssemblyFilename = systemWebAssembly.FullName,
                        Namespace = "System.Web.UI.WebControls",
                        TagPrefix = "asp",
                    }
            };
            }

        }

        public Assembly GetAssembly(string assemblyName)
        {
            if (_loadedAssemblies.ContainsKey(assemblyName))
            {
                return _loadedAssemblies[assemblyName];
            }

            return null;
        }

        public Assembly ProjectOutputAssembly { get; private set; }

        public Assembly SystemWebAssembly { get; private set; }

        public string MianOutputAssemblyPath { get; private set; }

        public string OutputDirectory { get; set; }

        /// <summary>
        /// The standard tag registrations that are always included, regardless of whether they
        /// are mentioned in the "web.config" or markup.
        /// 
        /// Note that HTML server controls are handled specially and are not included here.
        /// </summary>
        public IEnumerable<TagRegistration> StandardTagRegistrations { get; set; }

    }

    public static class AllowedAssemblies
    {

        public static string[] BlackList = new[] {"mscorib"};


        public static bool IsAllowed(string assemblyFilePath)
        {
            var assemblyFileName = System.IO.Path.GetFileNameWithoutExtension(assemblyFilePath);
            return !BlackList.Contains(assemblyFileName);
        }

    }
}
