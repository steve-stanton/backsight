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

using Backsight.Forms;

namespace Backsight.Editor
{
    /// <summary>
    /// Information about current options for an editing job.
    /// </summary>
    /// <remarks>Implemented by <see cref="JobFile"/></remarks>
    interface IJobInfo
    {
        /// <summary>
        /// The container for the job data.
        /// </summary>
        IJobContainer Container { get; }

        /// <summary>
        /// The user-perceived name of the job.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// An internal ID for the job (0 if the job is only local).
        /// </summary>
        uint JobId { get; }

        /// <summary>
        /// Identifies a map layer associated with the job.
        /// </summary>
        /// <remarks>Possibly obsolete. Return 0 in the meantime.</remarks>
        int LayerId { get; }

        /// <summary>
        /// Information about the area that was last drawn.
        /// </summary>
        DrawInfo LastDraw { get; set; }

        /// <summary>
        /// Current display units
        /// </summary>
        DistanceUnitType DisplayUnitType { get; set; }

        /// <summary>
        /// Current data entry units
        /// </summary>
        DistanceUnitType EntryUnitType { get; set; }

        /// <summary>
        /// Height of point symbols, in meters on the ground.
        /// </summary>
        double PointHeight { get; set; }

        /// <summary>
        /// Scale denominator at which labels (text) will start to be drawn.
        /// </summary>
        double ShowLabelScale { get; set; }

        /// <summary>
        /// Scale denominator at which points will start to be drawn.
        /// </summary>
        double ShowPointScale { get; set; }

        /// <summary>
        /// Should feature IDs be assigned automatically? (false if the user must specify).
        /// </summary>
        bool IsAutoNumber { get; set; }

        /// <summary>
        /// The ID of the default entity type for points (0 if undefined)
        /// </summary>
        int DefaultPointType { get; set; }

        /// <summary>
        /// The ID of the default entity type for lines (0 if undefined)
        /// </summary>
        int DefaultLineType { get; set; }

        /// <summary>
        /// The ID of the default entity type for polygons (0 if undefined)
        /// </summary>
        int DefaultPolygonType { get; set; }

        /// <summary>
        /// The ID of the default entity type for text (0 if undefined)
        /// </summary>
        int DefaultTextType { get; set; }

        /// <summary>
        /// The nominal map scale, for use in converting the size of fonts.
        /// </summary>
        uint NominalMapScale { get; set; }

        /// <summary>
        /// The style for annotating lines with distances (and angles)
        /// </summary>
        LineAnnotationStyle LineAnnotation { get; set; }

        /// <summary>
        /// Should intersection points be drawn? Relevant only if points
        /// are drawn at the current display scale (see the <see cref="ShowPointScale"/> property).
        /// </summary>
        bool AreIntersectionsDrawn { get; set; }

        string SplashIncrement { get; set; }

        string SplashPercents { get; set; }

        /// <summary>
        /// Has modified job information been saved?
        /// </summary>
        bool IsSaved { get; }

        /// <summary>
        /// Saves the job info as part of a persistent storage area.
        /// </summary>
        void Save();

        /// <summary>
        /// Loads a map model with the content of this job.
        /// </summary>
        /// <param name="mapModel">The model to load</param>
        void LoadModel(CadastralMapModel mapModel);
    }
}
