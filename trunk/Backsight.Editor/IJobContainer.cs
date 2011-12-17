// <remarks>
// Copyright 2011 - Steve Stanton. This file is part of Backsight
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

using Backsight.Environment;
namespace Backsight.Editor
{
    /// <summary>
    /// Something that holds one or more editing jobs.
    /// </summary>
    interface IJobContainer
    {
        /// <summary>
        /// Creates a brand new job.
        /// </summary>
        /// <param name="jobName">The user-perceived name for the job.</param>
        /// <param name="layer">The map layer the job is for.</param>
        /// <returns>Information describing the state of the job.</returns>
        IJobInfo CreateJob(string jobName, ILayer layer);

        /// <summary>
        /// Opens an editing job that was previously created.
        /// </summary>
        /// <param name="jobName">The name of the job</param>
        /// <returns>Information describing the state of the job (null if it could not be found).</returns>
        IJobInfo OpenJob(string jobName);

        /// <summary>
        /// Obtains a list of all previously created editing jobs.
        /// </summary>
        /// <returns>The names of all editing jobs in this container.</returns>
        string[] FindAllJobNames();
    }
}
