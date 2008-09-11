// <remarks>
// Copyright 2008 - Steve Stanton. This file is part of Backsight
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

namespace Backsight.Forms
{
    /// <summary>
    /// Something that makes use of the <see cref="BatchRunnerControl"/> class.
    /// </summary>
    public interface IBatchRunner
    {
        /// <summary>
        /// Handler that will be called upon completion of batch command (initiated
        /// via a call to <see cref="BatchRunnerControl.RunCommand"/>)
        /// </summary>
        /// <param name="e">Any exception denoting a failure (null if the command
        /// completed successfully)</param>
        void RunCompleted(Exception e);
    }
}
