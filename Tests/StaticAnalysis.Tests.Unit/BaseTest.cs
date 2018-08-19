﻿//-----------------------------------------------------------------------
// <copyright file="BaseTest.cs">
//      Copyright (c) Microsoft Corporation. All rights reserved.
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

using Microsoft.PSharp.IO;
using Microsoft.PSharp.LanguageServices.Compilation;
using Microsoft.PSharp.LanguageServices.Parsing;

using Xunit;

namespace Microsoft.PSharp.StaticAnalysis.Tests.Unit
{
    public abstract class BaseTest
    {
        #region successful tests

        protected void AssertSucceeded(string test, bool isPSharpProgram = true)
        {
            var configuration = GetConfiguration();
            AssertSucceeded(configuration, test, isPSharpProgram);
        }

        protected void AssertSucceeded(Configuration configuration, string test, bool isPSharpProgram = true)
        {
            InMemoryLogger logger = new InMemoryLogger();

            try
            {
                var context = CompileTest(configuration, test, isPSharpProgram);
                var engine = StaticAnalysisEngine.Create(context, logger).Run();
                var numErrors = engine.ErrorReporter.ErrorCount;
                var numWarnings = engine.ErrorReporter.WarningCount;
                Assert.Equal(0, numErrors);
                Assert.Equal(0, numWarnings);
            }
            finally
            {
                logger.Dispose();
            }
        }

        #endregion

        #region failed tests

        protected void AssertFailed(string test, int numExpectedErrors, bool isPSharpProgram = true)
        {
            var configuration = GetConfiguration();
            AssertFailed(configuration, test, numExpectedErrors, isPSharpProgram);
        }

        protected void AssertFailed(string test, int numExpectedErrors, string expectedOutput, bool isPSharpProgram = true)
        {
            var configuration = GetConfiguration();
            AssertFailed(configuration, test, numExpectedErrors, expectedOutput, isPSharpProgram);
        }

        protected void AssertFailed(Configuration configuration, string test, int numExpectedErrors, bool isPSharpProgram = true)
        {
            AssertFailed(configuration, test, numExpectedErrors, String.Empty, isPSharpProgram);
        }

        protected void AssertFailed(Configuration configuration, string test, int numExpectedErrors, string expectedOutput, bool isPSharpProgram = true)
        {
            InMemoryLogger logger = new InMemoryLogger();

            try
            {
                var context = CompileTest(configuration, test, isPSharpProgram);
                var engine = StaticAnalysisEngine.Create(context, logger).Run();

                var numErrors = engine.ErrorReporter.ErrorCount;
                Assert.Equal(numExpectedErrors, numErrors);

                if (!string.IsNullOrEmpty(expectedOutput))
                {
                    var actual = logger.ToString();
                    Assert.Equal(expectedOutput.Replace(Environment.NewLine, String.Empty),
                       actual.Substring(0, actual.IndexOf(Environment.NewLine)));
                }
            }
            finally
            {
                logger.Dispose();
            }
        }

        #endregion

        #region warning tests

        protected void AssertWarning(string test, int numExpectedWarnings, bool isPSharpProgram = true)
        {
            var configuration = GetConfiguration();
            AssertWarning(configuration, test, numExpectedWarnings, isPSharpProgram);
        }

        protected void AssertWarning(string test, int numExpectedWarnings, string expectedOutput, bool isPSharpProgram = true)
        {
            var configuration = GetConfiguration();
            AssertWarning(configuration, test, numExpectedWarnings, expectedOutput, isPSharpProgram);
        }

        protected void AssertWarning(Configuration configuration, string test, int numExpectedWarnings, bool isPSharpProgram = true)
        {
            AssertWarning(configuration, test, numExpectedWarnings, String.Empty, isPSharpProgram);
        }

        protected void AssertWarning(Configuration configuration, string test, int numExpectedWarnings, string expectedOutput, bool isPSharpProgram = true)
        {
            AssertFailedAndWarning(configuration, test, 0, numExpectedWarnings, expectedOutput, isPSharpProgram);
        }

        #endregion

        #region failed with warning tests

        protected void AssertFailedAndWarning(string test, int numExpectedErrors, int numExpectedWarnings, bool isPSharpProgram = true)
        {
            var configuration = GetConfiguration();
            AssertFailedAndWarning(configuration, test, numExpectedErrors, numExpectedWarnings, isPSharpProgram);
        }

        protected void AssertFailedAndWarning(string test, int numExpectedErrors, int numExpectedWarnings, string expectedOutput, bool isPSharpProgram = true)
        {
            var configuration = GetConfiguration();
            AssertFailedAndWarning(configuration, test, numExpectedErrors, numExpectedWarnings, expectedOutput, isPSharpProgram);
        }

        protected void AssertFailedAndWarning(Configuration configuration, string test, int numExpectedErrors, int numExpectedWarnings, bool isPSharpProgram = true)
        {
            AssertFailedAndWarning(configuration, test, numExpectedErrors, numExpectedWarnings, String.Empty, isPSharpProgram);
        }

        protected void AssertFailedAndWarning(Configuration configuration, string test, int numExpectedErrors, int numExpectedWarnings, string expectedOutput, bool isPSharpProgram = true)
        {
            InMemoryLogger logger = new InMemoryLogger();
            configuration.ShowWarnings = true;

            try
            {
                var context = CompileTest(configuration, test, isPSharpProgram);
                var engine = StaticAnalysisEngine.Create(context, logger).Run();

                var numErrors = engine.ErrorReporter.ErrorCount;
                var numWarnings = engine.ErrorReporter.WarningCount;
                Assert.Equal(numExpectedErrors, numErrors);
                Assert.Equal(numExpectedWarnings, numWarnings);

                if (!string.IsNullOrEmpty(expectedOutput))
                {
                    var actual = logger.ToString();
                    Assert.Equal(expectedOutput.Replace(Environment.NewLine, String.Empty),
                       actual.Replace(Environment.NewLine, String.Empty));
                }
            }
            finally
            {
                logger.Dispose();
            }
        }

        #endregion

        #region utilities

        protected Configuration GetConfiguration()
        {
            var configuration = Configuration.Create();
            configuration.ProjectName = "Test";
            configuration.ThrowInternalExceptions = true;
            configuration.Verbose = 2;
            configuration.AnalyzeDataFlow = true;
            configuration.AnalyzeDataRaces = true;
            return configuration;
        }

        private CompilationContext CompileTest(Configuration configuration, string test, bool isPSharpProgram)
        {
            var context = CompilationContext.Create(configuration).LoadSolution(test, isPSharpProgram ? "psharp" : "cs");
            ParsingEngine.Create(context).Run();
            RewritingEngine.Create(context).Run();
            return context;
        }

        #endregion
    }
}
