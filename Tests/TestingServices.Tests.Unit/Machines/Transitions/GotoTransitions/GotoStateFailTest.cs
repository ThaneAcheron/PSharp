﻿// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

using System;

using Xunit;

namespace Microsoft.PSharp.TestingServices.Tests.Unit
{
    public class GotoStateFailTest : BaseTest
    {
        class Program : Machine
        {
            [Start]
            [OnEntry(nameof(EntryInit))]
            class Init : MachineState { }

            void EntryInit()
            {
                // This line no longer builds after converting from Goto(typeof(T)) to Goto<T>()
                // due to the "where T: MachineState" constraint on Goto<T>().
                //this.Goto<object>();

                // Added a different failure mode here; try to Goto a state from another machine.
                this.Goto<Program2.Done>();
            }

            class Done : MachineState { }
        }

        class Program2 : Machine
        {
            [Start]
            [OnEntry(nameof(EntryInit))]
            class Init : MachineState { }

            void EntryInit()
            {
            }

            internal class Done : MachineState { }
        }

        [Fact]
        public void TestGotoStateFail()
        {
            var test = new Action<PSharpRuntime>((r) => {
                r.CreateMachine(typeof(Program));
            });

            base.AssertFailed(test, 1, true);
        }
    }
}
