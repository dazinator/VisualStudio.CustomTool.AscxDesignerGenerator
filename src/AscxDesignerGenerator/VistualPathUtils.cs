//-------------------------------------------------------------------------------------------------
//
//  Redesigner
//
//  Copyright (c) 2012-3 by Sean Werkema
//  All rights reserved.
//
//  This software is released under the terms of the "New BSD License," as follows:
//
//  Redistribution and use in source and binary forms, with or without modification, are permitted
//  provided that the following conditions are met:
//
//   * Redistributions of source code must retain the above copyright notice, this list of
//     conditions and the following disclaimer.
//
//   * Redistributions in binary form must reproduce the above copyright notice, this list of
//     conditions and the following disclaimer in the documentation and/or other materials
//     provided with the distribution.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
//  IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
//  AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
//  CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
//  CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//  SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
//  THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
//  OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
//  POSSIBILITY OF SUCH DAMAGE.
//
//-------------------------------------------------------------------------------------------------

using System.IO;

namespace AscxDesignerGenerator
{
    class VistualPathUtils
    {

        /// <summary>
        /// Given a virtual filename (possibly either rooted or relative), and a root path to the website,
        /// and the current disk directory, return a complete disk path for that virtual filename.
        /// </summary>
        /// <param name="compileContext">The context in which this is being resolved (for error-reporting).</param>
        /// <param name="virtualFilename">The virtual filename to resolve to a disk path to a real file.</param>
        /// <param name="rootPath">The root disk path to the website.</param>
        /// <param name="currentDirectory">The current directory (for evaluating relative paths).</param>
        /// <returns>The fully-resolved disk path to the given file.</returns>
        public static string ResolveWebsitePath(ICompileContext compileContext, string virtualFilename, string rootPath, string currentDirectory)
        {
            string filename;

            if (virtualFilename.StartsWith("~/") || virtualFilename.StartsWith(@"~\"))
            {
                // Rooted virtual path.
                filename = virtualFilename.Substring(2).Replace('/', '\\');
                return Path.Combine(rootPath, filename);
            }

            if (virtualFilename.StartsWith("/") || virtualFilename.StartsWith(@"\"))
                throw new RedesignerException("Illegal virtual path \"{0}\".  Virtual paths should be relative to the current path, or should start with \"~/\".", virtualFilename);

            // Relative virtual path.
            filename = virtualFilename.Replace('/', '\\');
            return Path.Combine(currentDirectory, filename);
        }

    }
}
