using System;
using System.Collections.Generic;
using System.Linq;

namespace AscxDesignerGenerator
{
    public static class UserControlTagRegistrationResolver
    {

        /// <summary>
        /// Given a set of tag registrations for user controls, attempt to connect those tag registrations to actual
        /// .NET class types in the main website assembly.  This will update the Typename field for each TagRegistration
        /// where a matching class type is found; or if no matching class type is found, this will throw an exception.
        /// </summary>
        /// <param name="compileContext">The context in which errors should be reported.</param>
        /// <param name="tagRegistrations">The set of user-control registrations to resolve to real class types.</param>
        /// <param name="assemblies">The full set of preloaded assemblies.</param>
        /// <param name="assemblyDirectory">The directory where the main website DLL can be found.</param>
        /// <param name="rootPath">The real disk path to the root of the website's virtual directory.</param>
        /// <param name="currentDirectory">The current directory (for resolving relative paths).</param>
        public static void ResolveUserControls(ICompileContext compileContext, IEnumerable<TagRegistration> tagRegistrations, ReferencedAssembliesContext assemblies, string rootPath, string currentDirectory)
        {
            foreach (TagRegistration tagRegistration in tagRegistrations.Where(t => t.Kind == TagRegistrationKind.SingleUserControl))
            {
                compileContext.Verbose("Registering user control <{0}:{1}> as \"{2}\".", tagRegistration.TagPrefix, tagRegistration.TagName, tagRegistration.SourceFilename);

                compileContext.VerboseNesting++;

                string filename = VistualPathUtils.ResolveWebsitePath(compileContext, tagRegistration.SourceFilename, rootPath, currentDirectory);

                MarkupReader userControlReader = new MarkupReader();
               
                // todo: how to handle references to user controls not found within rootpath?
                Tag userControlMainDirective = userControlReader.ReadMainDirective(compileContext, filename, assemblies, rootPath);

                if (string.IsNullOrEmpty(userControlMainDirective.TagName)
                  && string.Compare(userControlMainDirective.TagName, "control", StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    throw new RedesignerException("Cannot register user control \"{0}\":  Its main <% ... %> directive does not start with the \"Control\" keyword.  Is this actually a user control?", tagRegistration.SourceFilename);
                }

                string inheritsAttribute = userControlMainDirective["inherits"];
                if (string.IsNullOrEmpty(inheritsAttribute))
                {
                    throw new RedesignerException("Cannot register user control \"{0}\":  Its main <% Control ... %> directive is missing the required Inherits=\"...\" attribute.", tagRegistration.SourceFilename);
                }

                tagRegistration.Typename = inheritsAttribute;

                compileContext.Verbose("User control registered as type \"{0}\".", inheritsAttribute);
                compileContext.VerboseNesting--;
            }
        }
    }
}
