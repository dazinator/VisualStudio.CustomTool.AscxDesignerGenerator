namespace AscxDesignerGenerator
{
    /// <summary>
    /// The parsed equivalent of a &lt;add&gt; (web.config) or &lt;%Register%&gt; (markup) directive.
    /// This is a "dumb" class, containing only fields; all the intelligence of managing it is in
    /// other classes.
    /// </summary>
    public class TagRegistration
    {
        /// <summary>
        /// What kind of tag registration this is.
        /// </summary>
        public TagRegistrationKind Kind;

        /// <summary>
        /// The name of this tag in the markup (if appropriate).
        /// </summary>
        public string TagName;

        /// <summary>
        /// The prefix that will be used before instances of this tag(s) in the markup.
        /// </summary>
        public string TagPrefix;

        /// <summary>
        /// The assembly the code for this tag can be found in.
        /// </summary>
        public string AssemblyFilename;

        /// <summary>
        /// The namespace inside the given assembly where this tag prefix's code can be found.
        /// </summary>
        public string Namespace;

        /// <summary>
        /// The .ascx file containing the markup for this registered user control.
        /// </summary>
        public string SourceFilename;

        /// <summary>
        /// The C# full type name that this .ascx file says it inherits.
        /// </summary>
        public string Typename;
    }
}
