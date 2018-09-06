﻿// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

using System;

using Xunit;

namespace Microsoft.PSharp.TestingServices.Tests.Unit
{
    public class ReceivingExternalEventTest : BaseTest
    {
        class E : Event
        {
            public int Value;

            public E(int value)
            {
                this.Value = value;
            }
        }

        class Engine
        {
            public static void Send(PSharpRuntime runtime, MachineId target)
            {
                runtime.SendEvent(target, new E(2));
            }
        }

        class M : Machine
        {
            [Start]
            [OnEntry(nameof(InitOnEntry))]
            [OnEventDoAction(typeof(E), nameof(HandleEvent))]
            class Init : MachineState { }

            void InitOnEntry()
            {
                Engine.Send(this.Runtime, this.Id);
            }

            void HandleEvent()
            {
                this.Assert((this.ReceivedEvent as E).Value == 2);
            }
        }

        [Fact]
        public void TestReceivingExternalEvents()
        {
            var test = new Action<PSharpRuntime>((r) => {
                r.CreateMachine(typeof(M));
            });

            base.AssertSucceeded(test);
        }
    }
}
