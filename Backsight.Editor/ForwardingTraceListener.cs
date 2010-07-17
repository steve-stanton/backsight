// <remarks>
// Copyright 2007 - Steve Stanton. This file is part of Backsight
//
// Backsight is free software; you can redistribute it and/or modify it under the terms
// of the GNU Lesser General Public License as published by the Free Software Foundation;
// either version 3 of the License, or (at your option) any later version.
//
// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
// </remarks>

using System;
using System.Diagnostics;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="01-NOV-2007" />
    /// <summary>
    /// A <c>TraceListener</c> that forwards trace output to a delegate.
    /// </summary>
    public class ForwardingTraceListener : TraceListener
    {
        /// <summary>
        /// Delegate for accepting trace output.
        /// </summary>
        /// <param name="message"></param>
        public delegate void MessageReceiver (string message);

        #region Class data

        /// <summary>
        /// The delegate that trace output should be forwarded to (not null)
        /// </summary>
        readonly MessageReceiver m_Receiver;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>ForwardingTraceListener</c> that forwards to the
        /// specified receiver.
        /// </summary>
        /// <param name="receiver">The method that accepts forwarded trace output (not null)</param>
        /// <exception cref="ArgumentNullException">If <paramref name="receiver"/> is null</exception>
        public ForwardingTraceListener(MessageReceiver receiver)
        {
            if (receiver==null)
                throw new ArgumentNullException();

            m_Receiver = receiver;
        }

        #endregion

        /// <summary>
        /// Forwards a trace message
        /// </summary>
        /// <param name="message">The message to forward</param>
        public override void Write(string message)
        {
            m_Receiver(message);
        }

        /// <summary>
        /// Forwards a trace message
        /// </summary>
        /// <param name="message">The message to forward</param>
        public override void WriteLine(string message)
        {
            m_Receiver(message);
        }
    }
}
