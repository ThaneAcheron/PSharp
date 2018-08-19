﻿//-----------------------------------------------------------------------
// <copyright file="EventWaitHandler.cs">
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

namespace Microsoft.PSharp.Runtime
{
    /// <summary>
    /// Defines an event wait handler.
    /// </summary>
    internal class EventWaitHandler
    {
        /// <summary>
        /// Type of the event to handle.
        /// </summary>
        internal readonly Type EventType;

        /// <summary>
        /// Handle the event only if the
        /// predicate evaluates to true.
        /// </summary>
        internal readonly Func<Event, bool> Predicate;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="eventType">Event type</param>
        internal EventWaitHandler(Type eventType)
        {
            this.EventType = eventType;
            this.Predicate = (Event e) => true;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="predicate">Predicate</param>
        internal EventWaitHandler(Type eventType, Func<Event, bool> predicate)
        {
            this.EventType = eventType;
            this.Predicate = predicate;
        }
    }
}
