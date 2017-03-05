using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Diagnostics;
using System.Xml;
using Microsoft.VisualStudio;

namespace AscxDesignerGenerator
{
    public static class EnvDteExtensions
    {

        public static IVsHierarchy ToHierarchy(this EnvDTE.Project project)
        {
            if (project == null) throw new ArgumentNullException("project"); string projectGuid = null;        // DTE does not expose the project GUID that exists at in the msbuild project file.        // Cannot use MSBuild object model because it uses a static instance of the Engine,         // and using the Project will cause it to be unloaded from the engine when the         // GC collects the variable that we declare.       
            using (XmlReader projectReader = XmlReader.Create(project.FileName))
            {
                projectReader.MoveToContent();
                object nodeName = projectReader.NameTable.Add("ProjectGuid");
                while (projectReader.Read())
                {
                    if (Object.Equals(projectReader.LocalName, nodeName))
                    {
                        projectGuid = (String)projectReader.ReadElementContentAsString(); break;
                    }
                }
            }
            Debug.Assert(!String.IsNullOrEmpty(projectGuid));
            IServiceProvider serviceProvider = new ServiceProvider(project.DTE as Microsoft.VisualStudio.OLE.Interop.IServiceProvider); return VsShellUtilities.GetHierarchy(serviceProvider, new Guid(projectGuid));
        }

        public static IVsProject ToVsProject(this EnvDTE.Project project)
        {
            if (project == null) throw new ArgumentNullException("project");

            ThreadHelper.ThrowIfNotOnUIThread();
            IVsProject vsProject = ToHierarchy(project) as IVsProject;
            if (vsProject == null)
            {
                throw new ArgumentException("Project is not a VS project.");
            }
            return vsProject;
        }

        public static EnvDTE.Project ToDteProject(this IVsHierarchy hierarchy)
        {
            if (hierarchy == null) throw new ArgumentNullException("hierarchy");

            ThreadHelper.ThrowIfNotOnUIThread();
            object prjObject = null;
            if (hierarchy.GetProperty(0xfffffffe, -2027, out prjObject) >= 0)
            {
                return (EnvDTE.Project)prjObject;
            }
            else
            {
                throw new ArgumentException("Hierarchy is not a project.");
            }
        }

        public static EnvDTE.Project ToDteProject(this IVsProject project)
        {
            if (project == null) throw new ArgumentNullException("project");
            return ToDteProject(project as IVsHierarchy);
        }

        public static UnconfiguredProject GetUnconfiguredProject(this EnvDTE.Project project)
        {
            IVsBrowseObjectContext context = project as IVsBrowseObjectContext;
            if (context == null && project != null)
            { // VC implements this on their DTE.Project.Object
                context = project.Object as IVsBrowseObjectContext;
            }

            return context != null ? context.UnconfiguredProject : null;
        }


        public static UnconfiguredProject GetUnconfiguredProject(this IVsProject project)
        {
            IVsBrowseObjectContext context = project as IVsBrowseObjectContext;
            if (context == null)
            { // VC implements this on their DTE.Project.Object
                IVsHierarchy hierarchy = project as IVsHierarchy;
                if (hierarchy != null)
                {
                    object extObject;
                    if (ErrorHandler.Succeeded(hierarchy.GetProperty((uint)VSConstants.VSITEMID.Root, (int)__VSHPROPID.VSHPROPID_ExtObject, out extObject)))
                    {
                        EnvDTE.Project dteProject = extObject as EnvDTE.Project;
                        if (dteProject != null)
                        {
                            context = dteProject.Object as IVsBrowseObjectContext;
                        }
                    }
                }
            }

            return context != null ? context.UnconfiguredProject : null;
        }

        public static string GetProjectPath(this EnvDTE.Project project)
        {
            var fullPathProp = project.Properties.Item("FullPath");
            if (fullPathProp != null)
            {
                string fullPath = fullPathProp.Value.ToString();
                return fullPath;

            }

            return "";
        }



    }
}
