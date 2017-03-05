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

namespace AscxDesignerGenerator
{
    /// <summary>
    /// Metadata about a control that needs to be generated in the .designer file.  This is a simple
    /// "dumb" class; all intelligence needed to manage and maintain instances of this class is elsewhere.
    /// </summary>
    public class OutputControl
    {
        #region Fields

        /// <summary>
        /// The name given to this control in the markup.
        /// </summary>
        public string Name;

        /// <summary>
        /// The matching .NET class type for this control (and all suitable associated metadata).
        /// </summary>
        public ReflectedControl ReflectedControl;

        #endregion

        #region Methods

        /// <summary>
        /// Convert this control declaration to a string for easy debugging.
        /// </summary>
        /// <returns>A stringified version of this control declaration.</returns>
        public override string ToString()
        {
            return string.Format("{0} {1}", ReflectedControl.ControlType.FullName, string.IsNullOrEmpty(Name) ? "<unnamed>" : Name);
        }

        #endregion
    }
}
