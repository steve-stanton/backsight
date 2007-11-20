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
using System.Collections.Generic;
using System.Diagnostics;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="19-NOV-2007" />
    /// <summary>
    /// Query spatial index as part of file check.
    /// </summary>
    class FileCheckQuery
    {
        /// <summary>
        /// Delegate that may be called during file check.
        /// </summary>
        /// <param name="numChecked">The number of items that have been checked so far</param>
        internal delegate void OnCheckItem(int numChecked);

        #region Class data

        /// <summary>
        /// Delegate to call on checking each item in the file.
        /// </summary>
        readonly OnCheckItem m_OnCheckItem;

        /// <summary>
        /// Flag bits indicating the checks that are being performed.
        /// </summary>
        readonly CheckType m_Options;

        /// <summary>
        /// The total number of items checked.
        /// </summary>
        int m_NumCheck;

        /// <summary>
        /// The result of the check.
        /// </summary>
        readonly List<CheckItem> m_Result;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>FileCheckQuery</c> and executes it.
        /// </summary>
        /// <param name="model">The model to check</param>
        /// <param name="checks">Flag bits indicating the checks of interest</param>
        /// <param name="onCheckItem">Delegate to call whenever a further item is checked (null if no
        /// callback is required)</param>
        internal FileCheckQuery(CadastralMapModel model, CheckType checks, OnCheckItem onCheckItem)
        {
            if (model==null)
                throw new ArgumentNullException();

            m_OnCheckItem = onCheckItem;
            m_Options = checks;
            m_NumCheck = 0;
            m_Result = new List<CheckItem>(100);

            ISpatialIndex index = model.Index;
            index.QueryWindow(null, SpatialType.Line, CheckLine);
            index.QueryWindow(null, SpatialType.Text, CheckText);
            index.QueryWindow(null, SpatialType.Polygon, CheckPolygon);

            // Do any post-processing.
            PostCheck(m_Result, true);

            m_Result.TrimExcess();
        }

        #endregion

        /// <summary>
        /// The result of the check (not null, but may be empty)
        /// </summary>
        internal List<CheckItem> Result
        {
            get { return m_Result; }
        }

        /// <summary>
        /// The total number of items checked.
        /// </summary>
        internal int NumChecked
        {
            get { return m_NumCheck; }
        }

        /// <summary>
        /// Delegate that's called whenever the index finds a line.
        /// </summary>
        /// <param name="item">The item to process</param>
        /// <returns>True (always), indicating that the query should continue.</returns>
        private bool CheckLine(ISpatialObject item)
        {
            Debug.Assert(item is LineFeature);
            LineFeature line = (LineFeature)item;

            // Return if the line is non-topological
            if (!line.IsTopological)
                return true;

            Topology t = line.Topology;
            if (t!=null)
            {
                foreach (IDivider d in t)
                {
                    CheckType types = (m_Options & DividerCheck.Check(d));
                    if (types != CheckType.Null)
                    {
                        DividerCheck check = new DividerCheck(d, types);
                        m_Result.Add(check);
                    }
                }
            }

            return OnCheck();
        }

        /// <summary>
        /// Delegate that's called whenever the index finds an item of text.
        /// </summary>
        /// <param name="item">The item to process</param>
        /// <returns>True (always), indicating that the query should continue.</returns>
        private bool CheckText(ISpatialObject item)
        {
            Debug.Assert(item is TextFeature);
            TextFeature label = (TextFeature)item;

            // Return if the label is non-topological
            if (!label.IsTopological)
                return true;

            // Check the label & restrict to requested types.
            CheckType types = TextCheck.CheckLabel(label);
            types &= m_Options;

            if (types!=CheckType.Null)
            {
                TextCheck check = new TextCheck(label, types);
                m_Result.Add(check);
            }

            return OnCheck();
        }

        /// <summary>
        /// Delegate that's called whenever the index finds a polygon ring.
        /// </summary>
        /// <param name="item">The item to process</param>
        /// <returns>True (always), indicating that the query should continue.</returns>
        private bool CheckPolygon(ISpatialObject item)
        {
            Debug.Assert(item is Ring);
            Ring ring = (Ring)item;

            // Check the ring & restrict to requested types.
            CheckType types = RingCheck.CheckRing(ring);
            types &= m_Options;

            if (types!=CheckType.Null)
            {
                RingCheck check = new RingCheck(ring, types);
                m_Result.Add(check);
            }

            return OnCheck();
        }

        /// <summary>
        /// Performs any processing whenever an additional item has been checked.
        /// </summary>
        /// <returns>True (always)</returns>
        bool OnCheck()
        {
            m_NumCheck++;

            if (m_OnCheckItem!=null)
                m_OnCheckItem(m_NumCheck);

            return true;
        }

        /// <summary>
        /// Performs post-processing of the supplied list of check results
        /// </summary>
        /// <param name="items"></param>
        /// <param name="canRemove">True to remove stuff. False just to mark it as fixed.</param>
        /// <remarks>This method is static because it is also used by <c>FileCheckUI</c>
        /// to massage results whenever the user completes an edit</remarks>
        internal static void PostCheck(List<CheckItem> items, bool canRemove)
        {
            // If we only have ONE instance of "no polygon encloses this
            // polygon", it refers to the outside polygon, which you could
            // expect to get on every single check. If that is the case,
            // get rid of it.
            CheckItem rem = null;

            foreach (CheckItem check in items)
            {
                // If we've found a relevant problem, and we've already found the same
                // type of problem previously, just break from loop (ensuring we won't
                // attempt to remove the initially found problem)

                if ((check.Types & CheckType.NotEnclosed)!=0)
                {
                    if (rem!=null)
                    {
                        rem = null;
                        break;
                    }

                    rem = check;
                }
            }

            // If we got something, clear the problem type. If the
            // polygon has no other problems, remove it from the results
            // and get rid of it.
            if (rem!=null)
            {
                CheckType types = rem.Types;
                types &= (~CheckType.NotEnclosed);
                if (types != CheckType.Null)
                    rem.Types = types;
                else
                {
                    if (canRemove)
                        items.Remove(rem);
                    else
                        rem.Types = CheckType.Null;
                }
            }
        }
    }
}
