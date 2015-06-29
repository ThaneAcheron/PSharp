﻿//-----------------------------------------------------------------------
// <copyright file="CSharpParser.cs">
//      Copyright (c) 2015 Pantazis Deligiannis (p.deligiannis@imperial.ac.uk)
// 
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//      EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//      MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//      IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
//      CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
//      TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
//      SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

using Microsoft.PSharp.LanguageServices.Parsing.Framework;
using Microsoft.PSharp.Tooling;

namespace Microsoft.PSharp.LanguageServices.Parsing
{
    /// <summary>
    /// The C# parser.
    /// </summary>
    public sealed class CSharpParser : BaseParser
    {
        #region fields

        /// <summary>
        /// The error log.
        /// </summary>
        private Dictionary<SyntaxToken, string> ErrorLog;

        #endregion

        #region public API

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CSharpParser()
            : base()
        {
            this.ErrorLog = new Dictionary<SyntaxToken, string>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="project">PSharpProject</param>
        /// <param name="tree">SyntaxTree</param>
        /// <param name="exitAtError">Exits at error</param>
        internal CSharpParser(PSharpProject project, SyntaxTree tree, bool exitAtError = true)
            : base(project, tree, exitAtError)
        {
            this.ErrorLog = new Dictionary<SyntaxToken, string>();
        }

        /// <summary>
        /// Returns a P# program.
        /// </summary>
        /// <returns>P# program</returns>
        public IPSharpProgram Parse()
        {
            this.Program = this.CreateNewProgram();
            this.ParseSyntaxTree();

            if (this.ErrorLog.Count > 0)
            {
                this.ReportParsingErrors();
            }

            return this.Program;
        }

        #endregion

        #region protected API

        /// <summary>
        /// Returns a new C# program.
        /// </summary>
        /// <returns>P# program</returns>
        protected override IPSharpProgram CreateNewProgram()
        {
            return new CSharpProgram(base.Project, base.SyntaxTree);
        }

        #endregion

        #region private API

        /// <summary>
        /// Parses the syntax tree for errors.
        /// </summary>
        private void ParseSyntaxTree()
        {
            new MachineDeclarationParser(base.Project, this.ErrorLog).Parse(base.SyntaxTree);
        }

        /// <summary>
        /// Reports the parsing errors. Only works if the parser is
        /// running internally.
        /// </summary>
        private void ReportParsingErrors()
        {
            if (!base.IsRunningInternally)
            {
                return;
            }

            foreach (var error in this.ErrorLog)
            {
                var report = error.Value;
                var errorLine = base.SyntaxTree.GetLineSpan(error.Key.Span).StartLinePosition.Line + 1;
                
                var root = base.SyntaxTree.GetRoot();
                var lines = System.Text.RegularExpressions.Regex.Split(root.ToFullString(), "\r\n|\r|\n");

                report += "\nIn " + this.SyntaxTree.FilePath + " (line " + errorLine + "):\n";
                report += " " + lines[errorLine - 1];

                ErrorReporter.Report(report);
            }

            ErrorReporter.WriteLineAndExit("Found {0} parsing error{1}.", this.ErrorLog.Count,
                this.ErrorLog.Count == 1 ? "" : "s");
        }

        #endregion
    }
}