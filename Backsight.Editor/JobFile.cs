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
using System.IO;
using Backsight.Forms;
using Backsight.Editor.FileStore;

namespace Backsight.Editor
{
    /// <summary>
    /// The file holding job information (with the "cedx" file extension).
    /// </summary>
    class JobFile : IJobInfo
    {
        #region Static

        /// <summary>
        /// Writes a new job file
        /// </summary>
        /// <param name="fileName">The name of the job file to create</param>
        /// <param name="info">The information to write out</param>
        /// <returns>An object representing the resultant file</returns>
        internal static JobFile SaveJobFile(string fileName, JobFileInfo info)
        {
            // Write out the info, then read it back in
            info.WriteXML(fileName);
            return new JobFile(fileName);
        }

        #endregion

        #region Class data

        /// <summary>
        /// The full name of the job file (including directory)
        /// </summary>
        string m_FileName;

        /// <summary>
        /// The information read from the file.
        /// </summary>
        JobFileInfo m_Info;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>JobFile</c> that refers to an existing file
        /// </summary>
        /// <param name="fileName">The name of the job file to load</param>
        internal JobFile(string fileName)
        {
            m_FileName = fileName;
            m_Info = JobFileInfo.CreateInstance(m_FileName);
        }

        #endregion

        /// <summary>
        /// The information read from the file.
        /// </summary>
        internal JobFileInfo Data
        {
            get { return m_Info; }
        }

        /// <summary>
        /// The container for the job data.
        /// </summary>
        IJobContainer IJobInfo.Container
        {
            get { return new JobCollectionFolder(); }
        }

        /// <summary>
        /// The user-perceived name of the job.
        /// </summary>
        string IJobInfo.Name
        {
            get { return Path.GetFileNameWithoutExtension(m_FileName); }
        }

        /// <summary>
        /// Information about the area that was last drawn.
        /// </summary>
        DrawInfo IJobInfo.LastDraw
        {
            get { return m_Info.LastDraw; }
            set { m_Info.LastDraw = value; }
        }

        /// <summary>
        /// Current display units
        /// </summary>
        DistanceUnitType IJobInfo.DisplayUnitType
        {
            get { return m_Info.DisplayUnitType; }
            set { m_Info.DisplayUnitType = value; }
        }

        /// <summary>
        /// Current data entry units
        /// </summary>
        DistanceUnitType IJobInfo.EntryUnitType
        {
            get { return m_Info.EntryUnitType; }
            set { m_Info.EntryUnitType = value; }
        }

        /// <summary>
        /// Height of point symbols, in meters on the ground.
        /// </summary>
        double IJobInfo.PointHeight
        {
            get { return m_Info.PointHeight; }
            set { m_Info.PointHeight = value; }
        }

        /// <summary>
        /// Scale denominator at which labels (text) will start to be drawn.
        /// </summary>
        double IJobInfo.ShowLabelScale
        {
            get { return m_Info.ShowLabelScale; }
            set { m_Info.ShowLabelScale = value; }
        }

        /// <summary>
        /// Scale denominator at which points will start to be drawn.
        /// </summary>
        double IJobInfo.ShowPointScale
        {
            get { return m_Info.ShowPointScale; }
            set { m_Info.ShowPointScale = value; }
        }

        /// <summary>
        /// Should feature IDs be assigned automatically? (false if the user must specify).
        /// </summary>
        bool IJobInfo.IsAutoNumber
        {
            get { return m_Info.IsAutoNumber; }
            set { m_Info.IsAutoNumber = value; }
        }

        /// <summary>
        /// Has modified job information been saved?
        /// </summary>
        bool IJobInfo.IsSaved
        {
            get { return m_Info.IsSaved; }
        }

        /// <summary>
        /// Saves the job info as part of a persistent storage area.
        /// </summary>
        void IJobInfo.Save()
        {
            m_Info.WriteXML(m_FileName);
        }

        /// <summary>
        /// The ID of the default entity type for points (0 if undefined)
        /// </summary>
        int IJobInfo.DefaultPointType
        {
            get { return m_Info.DefaultPointType; }
            set { m_Info.DefaultPointType = value; }
        }

        /// <summary>
        /// The ID of the default entity type for lines (0 if undefined)
        /// </summary>
        int IJobInfo.DefaultLineType
        {
            get { return m_Info.DefaultLineType; }
            set { m_Info.DefaultLineType = value; }
        }

        /// <summary>
        /// The ID of the default entity type for polygons (0 if undefined)
        /// </summary>
        int IJobInfo.DefaultPolygonType
        {
            get { return m_Info.DefaultPolygonType; }
            set { m_Info.DefaultPolygonType = value; }
        }

        /// <summary>
        /// The ID of the default entity type for text (0 if undefined)
        /// </summary>
        int IJobInfo.DefaultTextType
        {
            get { return m_Info.DefaultTextType; }
            set { m_Info.DefaultTextType = value; }
        }

        /// <summary>
        /// The nominal map scale, for use in converting the size of fonts.
        /// </summary>
        uint IJobInfo.NominalMapScale
        {
            get { return m_Info.NominalMapScale; }
            set { m_Info.NominalMapScale = value; }
        }

        /// <summary>
        /// The style for annotating lines with distances (and angles)
        /// </summary>
        LineAnnotationStyle IJobInfo.LineAnnotation
        {
            get { return m_Info.LineAnnotation; }
            set { m_Info.LineAnnotation = value; }
        }

        /// <summary>
        /// Should intersection points be drawn? Relevant only if points
        /// are drawn at the current display scale (see the <see cref="ShowPointScale"/> property).
        /// </summary>
        bool IJobInfo.AreIntersectionsDrawn
        {
            get { return m_Info.AreIntersectionsDrawn; }
            set { m_Info.AreIntersectionsDrawn = value; }
        }

        string IJobInfo.SplashIncrement
        {
            get { return m_Info.SplashIncrement; }
            set { m_Info.SplashIncrement = value; }
        }

        string IJobInfo.SplashPercents
        {
            get { return m_Info.SplashPercents; }
            set { m_Info.SplashPercents = value; }
        }

        /// <summary>
        /// An internal ID for the job (0 if the job is only local).
        /// </summary>
        uint IJobInfo.JobId
        {
            get { return m_Info.JobId; }
        }

        /// <summary>
        /// Identifies a map layer associated with the job.
        /// </summary>
        /// <value>Always 0</value>
        int IJobInfo.LayerId
        {
            get { return 0; }
        }

        /// <summary>
        /// Loads a map model with the content of this job.
        /// </summary>
        /// <param name="mapModel">The model to load</param>
        void IJobInfo.LoadModel(CadastralMapModel mapModel)
        {
            throw new NotImplementedException();
        }
    }
}
