﻿// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

using System;

using Xunit;

namespace Microsoft.PSharp.TestingServices.Tests.Unit
{
    public class GotoStateExitFailTest : BaseTest
    {
        class Program : Machine
        {
            [Start]
            [OnEntry(nameof(EntryInit))]
            [OnExit(nameof(ExitInit))]
            class Init : MachineState { }

            void EntryInit()
            {
                this.Goto<Done>();
            }

            void ExitInit()
            {
                // This assert is reachable.
                this.Assert(false, "Bug found.");
            }

            class Done : MachineState { }
        }

        [Fact]
        public void TestGotoStateExitFail()
        {
            var test = new Action<PSharpRuntime>((r) => {
                r.CreateMachine(typeof(Program));
            });

            base.AssertFailed(test, 1, true);
        }
    }
}
