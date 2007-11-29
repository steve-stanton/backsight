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

using Backsight.Environment;
using Backsight.Editor.Operations;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="10-FEB-1998" was="CeControlRange" />
    /// <summary>
    /// A range of control points.
    /// </summary>
    class ControlRange
    {
        #region Class data

        /// <summary>
        /// The low end of the range.
        /// </summary>
        uint m_MinId;

        /// <summary>
        /// The high end of the range.
        /// </summary>
        uint m_MaxId;

        /// <summary>
        /// Control data (one for each control point in the range). May contain
        /// null references if a control point object hasn't been attached.
        /// </summary>
        ControlPoint[] m_Control;

        /// <summary>
        /// Number of control points that have been defined (the number of elements
        /// in m_Control that have not-null references).
        /// </summary>
        uint m_NumDefined;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor sets everything to zero or null.
        /// </summary>
        internal ControlRange()
        {
            Zero();
        }

        #endregion

        /// <summary>
        /// Initializes this instance will zeros and nulls.
        /// </summary>
        void Zero()
        {
            m_MinId = 0;
            m_MaxId = 0;
            m_Control = null;
            m_NumDefined = 0;
        }

        /// <summary>
        /// Does this range enclose the specified ID.
        /// </summary>
        /// <param name="id">The ID of interest</param>
        /// <returns>True if <paramref name="id"/> falls within the limits of this range</returns>
        internal bool IsEnclosing(uint id)
        {
            return (id>=m_MinId && id<=m_MaxId);
        }

        /// <summary>
        /// The number of control points in the range (zero if the range hasn't been defined)
        /// </summary>
        int NumControl
        {
            get { return (m_Control==null ? 0 : m_Control.Length); }
        }

        /// <summary>
        /// Number of control points that have been defined within the range.
        /// </summary>
        uint NumDefined
        {
            get { return m_NumDefined; }
        }

        /// <summary>
        /// The low end of the range.
        /// </summary>
        uint Min
        {
            get { return m_MinId; }
        }

        /// <summary>
        /// The high end of the range.
        /// </summary>
        uint Max
        {
            get { return m_MaxId; }
        }

        /// <summary>
        /// Allocates space for a range of control points.
        /// </summary>
        /// <param name="minid"></param>
        /// <param name="maxid"></param>
        /// <exception cref="ArgumentException">If <paramref name="minid"/> is
        /// greater than <paramref name="maxid"/></exception>
        void SetRange(uint minid, uint maxid)
        {
            // If we previously had a control array, get rid of it.
            Zero();

            // Confirm the range is valid.
            if (minid>maxid)
                throw new ArgumentException("ControlRange.SetRange - Bad range");

            // Remember the range.
            m_MinId = minid;
            m_MaxId = maxid;

            // Allocate array of undefined control point objects.
            int numControl = (int)(m_MaxId-m_MinId+1);
            m_Control = new ControlPoint[numControl];
        }

        /// <summary>
        /// Inserts a control point into this range (if applicable). Prior to call the
        /// limits of the range must be defined via a call to <see cref="SetRange"/>.
        /// </summary>
        /// <param name="control">The control point to insert.</param>
        /// <returns>True if control point was inserted. False if the control point does
        /// not belong to this range.</returns>
        internal bool Insert(ControlPoint control)
        {
            if (control==null)
                throw new ArgumentNullException();

            // Return if the control point does not belong to this range.
            if (!control.IsInRange(this))
                return false;

            // Confirm range has been defined.
            if (NumControl==0)
                throw new Exception("ControlRange.Insert - Range was not defined");

            // Return if this range is already completely defined.
            if (m_NumDefined==NumControl)
                return true;

            // Get array index for the control point.
            int index = (int)(control.ControlId - m_MinId);

            // If the control point was not previously stored, store
            // the supplied control point.

            if (m_Control[index]==null)
            {
                m_Control[index] = control;
                m_NumDefined++;
            }

            return true;
        }

        /// <summary>
        /// Saves this control range in the map.
        /// </summary>
        /// <param name="op">The operation doing the save.</param>
        /// <param name="ent">The entity type to assign to the control.</param>
        internal void Save(GetControlOperation op, IEntity ent)
        {
            // Return if there is nothing to save.
            if (m_Control==null)
                return;

            // Save each control point.
            foreach (ControlPoint cp in m_Control)
            {
                if (cp!=null)
                    op.Import(cp, ent);
            }
        }

        /// <summary>
        /// Draws this control range.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The style for the drawing</param>
        internal void Render(ISpatialDisplay display, IDrawStyle style)
        {
            // Return if there is nothing to draw.
            if (m_Control==null)
                return;

            // Draw each control point.
            foreach (ControlPoint cp in m_Control)
            {
                if (cp!=null)
                    cp.Render(display, style);
            }
        }

        /// <summary>
        /// Confirms that a control point can be appended to this control range. This
        /// should be called before making a call to <see cref="Append"/>
        /// </summary>
        /// <param name="control">The control point you want to append.</param>
        /// <returns>True if the control point can be appended.</returns>
        internal bool CanAppend(ControlPoint control)
        {
            // We can append if the range is undefined, or the specified
            // control point has a key that is consecutive with the last one.
            return (m_MaxId==0 || control.ControlId==(m_MaxId+1));
        }

        /// <summary>
        /// Appends a control point to this range. Before calling this function, you should
        /// make a call to <see cref="CanAppend"/>, to see whether the control point can really
        /// be appended. If you don't, the control point will simply be appended, even though
        /// it may not be consecutive with the rest of the range.
        /// </summary>
        /// <param name="control">The control point to append (not null)</param>
        internal void Append(ControlPoint control)
        {
            if (control==null)
                throw new ArgumentNullException();

            // If there is insufficient allocation for an extra control
            // point, re-allocate some more.
            if (m_Control==null)
                m_Control = new ControlPoint[1];
            else
            {
                ControlPoint[] newArray = new ControlPoint[m_Control.Length+1];
                Array.Copy(m_Control, newArray, m_Control.Length);
                m_Control = newArray;
            }

            // If this is the very first control point to be added to this
            // range, initialize the min & max IDs.
            if (m_Control.Length==1)
                m_MinId = m_MaxId = control.ControlId;
            else
                m_MaxId = control.ControlId;

            // Save the control point and Increment the number of defined control points
            m_Control[m_Control.Length-1] = control;
            m_NumDefined++;
        }
    }
}
