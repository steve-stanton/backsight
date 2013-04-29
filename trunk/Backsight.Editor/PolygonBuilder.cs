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
    /// <written by="Steve Stanton" on="01-AUG-2007" />
    /// <summary>
    /// Updates polygon topology within a map model. This looks at all topological line
    /// features referenced by the map's index, ensuring that neighbouring polygons
    /// are defined for them. Lines that already refer to a polygon are left untouched.
    /// </summary>
    class PolygonBuilder
    {
        #region Class data

        /// <summary>
        /// The model for the polygons.
        /// </summary>
        private readonly CadastralMapModel m_Model;

        /// <summary>
        /// The window of newly created polygons.
        /// </summary>        
        private readonly Window m_NewPolygonExtent;

        /// <summary>
        /// The spatial index to update during the build.
        /// </summary>
        private readonly EditingIndex m_Index;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>PolygonBuilder</c> for the supplied model. Make a subsequent
        /// call to <c>Build</c> to create polygons.
        /// </summary>
        /// <param name="model">The model the polygons should be created within.</param>
        internal PolygonBuilder(CadastralMapModel model)
        {
            m_Model = model;
            m_NewPolygonExtent = new Window();
            m_Index = model.EditingIndex;
        }

        #endregion

        /// <summary>
        /// Builds polygon topology
        /// </summary>
        internal void Build()
        {
            //return; // debug

            Trace.Write("Building network");
            m_Model.Index.QueryWindow(null, SpatialType.Line, ProcessLine);

            Trace.Write("Processing island polygons");
            BuildIslands();

            Trace.Write("Associating polygon labels");
            BuildLabels();

            /*
            using (System.IO.StreamWriter output = System.IO.File.CreateText(@"C:\Temp\Build.txt"))
            {
                Stopwatch sw = Stopwatch.StartNew();

                Trace.Write("Building network");
                m_Model.Index.QueryWindow(null, SpatialType.Line, ProcessLine);
                sw.Stop();
                output.WriteLine("Build network: "+sw.ElapsedMilliseconds);

                sw.Reset();
                sw.Start();
                Trace.Write("Processing island polygons");
                BuildIslands();
                sw.Stop();
                output.WriteLine("Build islands: "+sw.ElapsedMilliseconds);

                sw.Reset();
                sw.Start();
                Trace.Write("Associating polygon labels");
                BuildLabels();
                sw.Stop();
                output.WriteLine("Build labels: "+sw.ElapsedMilliseconds);
                //(m_Model.Index as Backsight.Index.SpatialIndex).DumpStats();
            }
             */
        }

        /// <summary>
        /// Delegate called to process every line feature in the model.
        /// Called by <c>Build</c>
        /// </summary>
        /// <param name="o">An item found in the spatial index.</param>
        /// <returns>True (always), indicating that the spatial query should keep going.</returns>
        /// <exception cref="Exception">If an error occurred building a polygon.</exception>
        bool ProcessLine(ISpatialObject o)
        {
            Debug.Assert(o is LineFeature);
            LineFeature line = (LineFeature)o;

            if (line.InternalId.ItemSequence == 149)
            {
                int junk = 0;
            }

            try
            {
                line.BuildPolygons(m_NewPolygonExtent, m_Index);
                return true;
            }

            catch (Exception e)
            {
                string msg = String.Format("Error building polygon starting at line {0} ({1})",
                                            line.ToString(), e.Message);
                throw new Exception(msg);
            }
        }

        /// <summary>
        /// Associates islands with their enclosing polygons (if any). It is assumed that
        /// the polygon topology is up to date.
        /// </summary>
        void BuildIslands()
        {
            // Return if we didn't create any new polygons
            if (m_NewPolygonExtent.IsEmpty)
                return;

            // Try using the new build window (20070823)
            //m_Model.Index.QueryWindow(null, SpatialType.Polygon, ProcessPolygon);
            m_Model.Index.QueryWindow(m_NewPolygonExtent, SpatialType.Polygon, ProcessPolygon);
        }

        /// <summary>
        /// Delegate called to process each polygon found within the build window.
        /// Called by <c>BuildIslands</c>
        /// </summary>
        /// <param name="o">An item found in the spatial index.</param>
        /// <returns>True (always), indicating that the spatial query should keep going.</returns>
        bool ProcessPolygon(ISpatialObject o)
        {
            if (!(o is Island))
                return true;

            // Just return if the enclosing polygon is already known
            Island pol = (Island)o;
            if (pol.Container!=null)
                return true;

            // If the island is marked as floating, and it has a window
            // that encloses the build window, mark it as not floating.
            // This covers situations where a previously floating island
            // is now enclosed by something.
            if (pol.IsFloating && pol.Extent.IsOverlap(m_NewPolygonExtent))
                pol.IsFloating = false;

    		pol.SetContainer();
            return true;
        }

        /// <summary>
        /// Associates labels with their enclosing polygons (if any). It is assumed
        /// that the polygon topology is up to date.
        /// </summary>
        void BuildLabels()
        {
            // Scan unlabelled polygons to find enclosed labels. This is significantly
            // faster than scanning the labels to find enclosing polygons (which is
            // what we'll do below). However, it won't be faster when dealing with
            // more complicated polygons, so it purposefully ignored polygons that
            // contain any islands. That's why we do the alternate scan below. Taking
            // this approach improved performance between 10x and 20x.
            m_Model.Index.QueryWindow(null, SpatialType.Polygon, FindLabelForPolygon);

            // Not sure if restricting it to the new polygon extent is valid (what if
            // the user has just added labels, but no lines).
            //m_Model.Index.QueryWindow(m_NewPolygonExtent, SpatialType.Text, ProcessLabel);
            m_Model.Index.QueryWindow(null, SpatialType.Text, ProcessLabel);
        }

        /// <summary>
        /// Delegate called to process each text feature found within the build window.
        /// Called by <see cref="BuildLabels"/>.
        /// </summary>
        /// <param name="o">An item found in the spatial index.</param>
        /// <returns>True (always), indicating that the spatial query should keep going.</returns>
        bool ProcessLabel(ISpatialObject o)
        {
            Debug.Assert(o is TextFeature);
            TextFeature f = (TextFeature)o;
            f.SetPolygon();
            return true;
        }

        /// <summary>
        /// Delegate called to process every polygon in the map.
        /// Called by <see cref="BuildLabels"/>.
        /// </summary>
        /// <param name="o">A polygon ring in the spatial index</param>
        /// <returns>True (always), to force examination of the entire spatial index</returns>
        bool FindLabelForPolygon(ISpatialObject o)
        {
            Debug.Assert(o is Ring);

            if (o is Polygon)
            {
                // Only process polygons that don't already have a label, and which
                // don't contain any islands (polygons with islands tend to cover
                // much larget areas, so trying to find the label this way will
                // probably not be very efficient).

                Polygon p = (Polygon)o;
                if (p.Label == null && !p.HasAnyIslands)
                {
                    TextFeature label = new FindPolygonLabelQuery(m_Model.Index, p).Result;
                    if (label != null)
                    {
                        p.ClaimLabel(label);
                        label.SetBuilt(true);
                    }
                }
            }

            return true;
        }
	}
}
