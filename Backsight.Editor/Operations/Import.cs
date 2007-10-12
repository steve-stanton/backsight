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

namespace Backsight.Editor.Operations
{
    [Serializable]
    class Import : Operation
    {
        /// <summary>
        /// The features that were imported.
        /// </summary>
        private readonly Feature[] m_Data;

        /*
        public static Import New(string fileName)
        {
            IFeature[] features = LoadFeatures(fileName);
            return RegisterNew(new Import(features, null));
        }

        private static IFeature[] LoadFeatures(string fileName)
        {
            // Assume shape file containing lines for now
            ShapefileReader sfr = new ShapefileReader(fileName);

            ShapefileHeader hdr = sfr.Header;
            if (hdr.ShapeType != ShapeGeometryTypes.LineString)
                throw new Exception("Unexpected shape type: "+hdr.ShapeType.ToString());

            //Envelope ex = hdr.Bounds;
            //m_Extent = new Window(ex.MinX, ex.MinY, ex.MaxX, ex.MaxY);
            GeometryCollection all = sfr.ReadAll();
            Geometry[] gs = all.Geometries;
            List<IFeature> output = new List<IFeature>(gs.Length);

            foreach (Geometry geom in gs)
            {
                IFeature f = LoadFeature(geom);
                output.Add(f);
            }

            return output.ToArray();
        }

        private static IFeature LoadFeature(Geometry g)
        {
            Coordinate[] cos = g.Coordinates;
            Position[] pts = new Position[cos.Length];
            for (int i=0; i<cos.Length; i++)
            {
                Coordinate c = cos[i];
                pts[i] = new Position(c.X, c.Y);
            }

            return new LineFeature(pts);
        }
        */
        public Import(FileImportSource source) : base()
        {
            m_Data = source.Load(this);
        }

        internal override Feature[] Features
        {
            get { return m_Data; }
        }

        /// <summary>
        /// Handles any intersections created by this operation.
        /// </summary>
        internal override void Intersect()
        {
            SetMoved();
        }

        /// <summary>
        /// Marks all topological arcs as moved.
        /// </summary>
        /// <returns>The number of lines that were marked.</returns>
        uint SetMoved()
        {
	        // Loop through the features that were created, marking
	        // them as moved so that the map will intersect them.

            uint nMarked = 0;

            foreach(Feature f in m_Data)
            {
                if (f.IsTopological)
                {
                    f.IsMoved = true;
                    nMarked++;
                }
            }

        	return nMarked;
        }

        internal override EditingActionId EditId
        {
            get { return EditingActionId.DataImport; }
        }

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return "Data import"; }
        }

        public override void AddReferences()
        {
            // No direct references
        }

        internal override Distance GetDistance(LineFeature line)
        {
            return null; // nothing to do
        }

        internal override bool Undo()
        {
            base.OnRollback();

        	// Get rid of the features that were created.
            foreach (Feature f in m_Data)
                Rollback(f);

            return true;
        }

        internal override bool Rollforward()
        {
            // Return if this operation has not been marked as changed.
            if (!IsChanged)
                return base.OnRollforward();

            // Nothing to do

            return true;
        }
    }
}
