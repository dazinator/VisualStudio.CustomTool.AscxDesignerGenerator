﻿//-------------------------------------------------------------------------------------------------
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

using System.Collections.Generic;

namespace AscxDesignerGenerator
{
    /// <summary>
    /// The complete result of having parsed a .aspx or .ascx file.
    /// </summary>
    public class MarkupInfo
    {
        /// <summary>
        /// The main &lt;% Page %&gt; or &lt;% Control %&gt; directive for this file.
        /// </summary>
        public Tag MainDirective { get; set; }

        ///// <summary>
        ///// The .NET class type for the code-behind of this file.
        ///// </summary>
        //public Type ClassType { get; set; }

        /// <summary>
        /// The complete list of controls that should appear in a .designer file for this markup.
        /// </summary>
        public List<OutputControl> OutputControls { get; set; }

        /// <summary>
        /// The complete list of assemblies used or referenced by this page or control (or their associated web.config file).
        /// </summary>
        public ReferencedAssembliesContext Assemblies { get; set; }

        public string InheritsClassName { get; set; }

        public string InheritsClassNamespace { get; set; }


    }
}
