/// <remarks>
/// Copyright 2007 - Steve Stanton. This file is part of Backsight
///
/// Backsight is free software; you can redistribute it and/or modify it under the terms
/// of the GNU Lesser General Public License as published by the Free Software Foundation;
/// either version 3 of the License, or (at your option) any later version.
///
/// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
/// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU Lesser General Public License for more details.
///
/// You should have received a copy of the GNU Lesser General Public License
/// along with this program. If not, see <http://www.gnu.org/licenses/>.
/// </remarks>

using System;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="12-JUN-07" />
    /// <summary>
    /// Something that is dependent on instance(s) of <c>Feature</c>. Each
    /// feature involved should be cross-referenced to the dependent feature.
    /// </summary>
    interface IFeatureDependent
    {
        /// <summary>
        /// Performs any processing that needs to be done just before the position of
        /// a referenced feature is changed.
        /// </summary>
        /// <param name="f">The feature that is about to be changed (a feature that
        /// the <c>IFeatureDependent</c> is dependent on)</param>
        void OnPreMove(Feature f);

        /// <summary>
        /// Performs any processing that needs to be done after the position of
        /// a referenced feature has been changed.
        /// </summary>
        /// <param name="f">The feature that has just been changed (a feature that
        /// the <c>IFeatureDependent</c> is dependent on)</param>
        void OnPostMove(Feature f);

        /// <summary>
        /// Adds references to the features that this dependent is dependent on.
        /// </summary>
        void AddReferences();
    }
}