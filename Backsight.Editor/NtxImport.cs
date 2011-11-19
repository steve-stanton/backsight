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
using System.Collections.Generic;
using System.Diagnostics;

using Backsight.Editor.Operations;
using Backsight.Environment;
using Backsight.Index;
using Backsight.Geometry;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="16-FEB-1999" was="CeNtx" />
    /// <summary>
    /// Importer for CARIS-NTX files.
    /// </summary>
    class NtxImport : FileImportSource
    {
        #region Class data

        /// <summary>
        /// Tool for converting feature codes into entity types.
        /// </summary>
        readonly ITranslate m_Translator;

        /// <summary>
        /// The features created by a call to <c>LoadFeatures</c>
        /// </summary>
        readonly List<Feature> m_Result;

        /// <summary>
        /// Temporary spatial index (used for points and circles)
        /// </summary>
        readonly SpatialIndex m_Index;

        /// <summary>
        /// The IDs of features created during this import
        /// </summary>
        readonly Dictionary<string, ForeignId> m_KeyIndex;

        #endregion

        #region Constructors

        internal NtxImport(string fileName, ITranslate t) : base(fileName)
        {
            m_Translator = t;
            m_Result = new List<Feature>(1000);
            m_Index = new SpatialIndex();
            m_KeyIndex = new Dictionary<string, ForeignId>(1000);
        }

        #endregion

        internal override Feature[] LoadFeatures(string fileName, Operation creator)
        {
            // First load all point features
            Trace.Write("Loading points");
            List<PointFeature> points = LoadPoints(fileName, creator);
            m_Result.AddRange(points.ToArray());

            // Create a spatial index for the points
            foreach (PointFeature p in points)
                m_Index.Add(p);

            // Load circular arcs
            Trace.Write("Loading circular arcs");
            List<ArcFeature> arcs = LoadArcs(fileName, creator);
            m_Result.AddRange(arcs.ToArray());

            // Now load everything except points
            Trace.Write("Loading data");
            Ntx.File file = new Ntx.File();

            try
            {
                file.Open(fileName);

                // Define line end point tolerance
                ILength tol = GetPointMatchTolerance(file);

                while (file.GetMore())
                {
                    Feature f = null;

                    if (file.DataType == (int)Ntx.DataType.Line && !file.Line.IsCurve)
                        f = ImportLine(tol, file.Line, creator);
                    else if (file.DataType == (int)Ntx.DataType.Name)
                        f = ImportName(file.Name, creator);

                    if (f!=null)
                        m_Result.Add(f);
                }

                // Mark all features as moved, so they will be intersected against the map model
                //SetMoved(m_Result);

                return m_Result.ToArray();
            }

            finally
            {
                file.Close();
            }
        }

        /// <summary>
        /// Loads all point features (symbols) from an NTX file.
        /// </summary>
        /// <param name="fileName">The name of the NTX file to read from</param>
        /// <param name="creator">The edit that's being used to perform the import</param>
        /// <returns>The loaded points</returns>
        internal List<PointFeature> LoadPoints(string fileName, Operation creator)
        {
            Ntx.File file = new Ntx.File();

            try
            {
                file.Open(fileName);
                List<PointFeature> result = new List<PointFeature>(1000);
                while (file.GetMore())
                {
                    if (file.DataType == (int)Ntx.DataType.Symbol)
                    {
                        Feature f = ImportSymbol(file.Symbol, creator);
                        if (f!=null)
                            result.Add((PointFeature)f);
                    }
                }
                return result;
            }

            finally
            {
                file.Close();
            }
        }

        /// <summary>
        /// Loads all circular arcs from an NTX file.
        /// </summary>
        /// <param name="fileName">The name of the NTX file to read from</param>
        /// <param name="creator">The edit that's being used to perform the import</param>
        /// <returns>The loaded features</returns>
        internal List<ArcFeature> LoadArcs(string fileName, Operation creator)
        {
            Ntx.File file = new Ntx.File();

            try
            {
                file.Open(fileName);
                List<ArcFeature> result = new List<ArcFeature>(1000);
                ILength tol = GetPointMatchTolerance(file);

                while (file.GetMore())
                {
                    if (file.DataType == (int)Ntx.DataType.Line && file.Line.IsCurve)
                    {
                        ArcFeature f = ImportArc(file.Line, creator, tol);
                        if (f!=null)
                            result.Add((ArcFeature)f);
                    }
                }
                return result;
            }

            finally
            {
                file.Close();
            }
        }

        private ArcFeature ImportArc(Ntx.Line line, Operation creator, ILength tol)
        {
            Debug.Assert(line.IsCurve);
            IEntity what = GetEntityType(line, SpatialType.Line);

            // Get positions defining the arc
            PointGeometry[] pts = GetPositions(line);

            // Ignore zero-length lines
            if (HasZeroLength(pts))
                return null;

            // Add a point at the center of the circle
            Ntx.Position pos = line.Center;
            PointGeometry pc = new PointGeometry(pos.Easting, pos.Northing);
            PointFeature center = EnsurePointExists(pc, tol, creator);

            // Calculate exact positions for the arc endpoints
            double radius = line.Radius;
            ICircleGeometry cg = new CircleGeometry(pc, radius);
            IPosition bc = CircleGeometry.GetClosestPosition(cg, pts[0]);
            IPosition ec = CircleGeometry.GetClosestPosition(cg, pts[pts.Length-1]);

            // Round off to nearest micron
            PointGeometry bcg = PointGeometry.Create(bc);
            PointGeometry ecg = PointGeometry.Create(ec);

            // Ensure point features exist at both ends of the line.
            PointFeature ps = GetArcEndPoint(bcg, tol, creator);
            PointFeature pe = GetArcEndPoint(ecg, tol, creator);

            // Try to find a circle that's already been added by this import.
            Circle c = EnsureCircleExists(center, radius, tol, creator);

            // Determine which way the arc is directed
            bool iscw = LineStringGeometry.IsClockwise(pts, center);

            uint ss = Session.ReserveNextItem();
            ArcFeature arc = new ArcFeature(creator, ss, what, c, ps, pe, iscw);

            if (line.IsTopologicalArc)
                arc.SetTopology(true);

            #if DEBUG
            // Confirm the NTX data was valid (ensure it's consistent with what we've imported)...

            double readRad = c.Radius;
            double calcRad = BasicGeom.Distance(c.Center, ps);
            Debug.Assert(Math.Abs(readRad-calcRad) < tol.Meters);

            foreach (IPointGeometry pg in pts)
            {
                ILength check = arc.Geometry.Distance(pg);
                Debug.Assert(check.Meters < tol.Meters);
            }
            #endif

            return arc;
        }

        PointFeature GetArcEndPoint(PointGeometry g, ILength tol, Operation creator)
        {
            // Ensure we've got a point at the required position
            PointFeature pt = EnsurePointExists(g, tol, creator);

            // If it's not exactly coincident, we've picked up a previously loaded point
            // that happens to be within tolerance. If it's not already connected to any
            // lines, shift it to where we want it.
            if (!pt.IsCoincident(g))
            {
                if (!pt.HasDependents)
                {
                    m_Index.Remove(pt);
                    PointFeature[] pts = pt.Node.Points;
                    pt.SetNode(new Node(pts, g));
                    m_Index.Add(pt);
                }
            }

            return pt;
        }

        ILength GetPointMatchTolerance(Ntx.File file)
        {
            // Define line end point tolerance that matches the resolution of the input NTX file.
            // ...in the sample NTX file I've got, the resolution is supposedly 0.0001m, but
            // things like circle center points are frequently different by 0.0002 or so.
            // So be a bit more permissive as far as search tolerance is concerned.
            double res = file.Header.XYResolution;
            //ILength tol = new MicronValue(res);
            ILength tol = new MicronValue(res*10);
            return tol;
        }

        private IEntity GetEntityType(Ntx.Feature f, SpatialType type)
        {
            if (m_Translator==null)
                return null;

            string fc = f.FeatureCode;
            return m_Translator.FindEntityTypeByExternalName(fc, type);
        }

        private Feature ImportLine(ILength tol, Ntx.Line line, Operation creator)
        {
            // Circular arcs are handled elsewhere
            if (line.IsCurve)
                return null;

            IEntity what = GetEntityType(line, SpatialType.Line);

            PointGeometry[] pts = GetPositions(line);

            // Ignore zero-length lines
            if (HasZeroLength(pts))
                return null;

            // Ensure point features exist at both ends of the line.
            PointFeature ps = EnsurePointExists(pts[0], tol, creator);
            PointFeature pe = EnsurePointExists(pts[pts.Length-1], tol, creator);

            // Force end positions to match
            pts[0] = ps.PointGeometry;
            pts[pts.Length-1] = pe.PointGeometry;

            // It's possible we've now produced a zero-length line
            if (Object.ReferenceEquals(ps, pe) && HasZeroLength(pts))
                return null;

            // If we're dealing with a multi-segment, I have occasionally seen tiny glitches
            // at the end of the incoming lines (whether this is a real data problem, or an
            // imperfection in the import software is unknown). So double check now.

            // In the longer term, the import software should also check for more complex
            // issues, like missing intersections. In the meantime, I assume that incoming
            // topological data is generally clean.

            if (pts.Length > 2 && line.IsTopologicalArc)
                pts = CheckMultiSegmentEnds(pts);

            LineFeature result;
            uint ss = Session.ReserveNextItem();

            if (pts.Length==2)
                result = new LineFeature(creator, ss, what, ps, pe);
            else
                result = new LineFeature(creator, ss, what, ps, pe, pts);

            if (line.IsTopologicalArc)
                result.SetTopology(true);

            return result;
        }

        /// <summary>
        /// Checks whether the supplied positions define a zero-length line
        /// </summary>
        /// <param name="pts">The positions for a line</param>
        /// <returns>True if the supplied array contains fewer than 2 points, or all the points
        /// are exactly coincident.</returns>
        private bool HasZeroLength(PointGeometry[] pts)
        {
            if (pts.Length<2)
                return true;

            PointGeometry start = pts[0];
            for (int i=1; i<pts.Length; i++)
            {
                if (!pts[i].IsCoincident(start))
                    return false;
            }

            return true;
        }

        private PointGeometry[] CheckMultiSegmentEnds(PointGeometry[] pts)
        {
            if (pts.Length<=2)
                return pts;

            //double tol = (Constants.XYRES * Constants.XYRES);
            double tol = (0.001 * 0.001);
            PointGeometry[] res = pts;
            bool doCheck = true;

            while (doCheck && res.Length>2)
            {
                doCheck = false;

                // If the start position coincides with the second segment, strip out
                // the second position.
                if (BasicGeom.DistanceSquared(res[0].X, res[0].Y, res[1].X, res[1].Y, res[2].X, res[2].Y) < tol)
                {
                    PointGeometry[] tmp = new PointGeometry[res.Length-1];
                    tmp[0] = res[0];
                    Array.Copy(res, 2, tmp, 1, res.Length-2);
                    res = tmp;
                    doCheck = true;
                }
            }

            // If the end position coincides with the second last segment, strip out
            // the second last position.

            doCheck = true;

            while (doCheck && res.Length>2)
            {
                doCheck = false;

                int last = res.Length-1;
                if (BasicGeom.DistanceSquared(res[last].X, res[last].Y, res[last-1].X, res[last-1].Y, res[last-2].X, res[last-2].Y) < tol)
                {
                    PointGeometry[] tmp = new PointGeometry[res.Length-1];
                    Array.Copy(res, 0, tmp, 0, res.Length-2);
                    tmp[tmp.Length-1] = res[last];
                    res = tmp;
                    doCheck = true;
                }
            }

            return res;
        }

        private PointFeature EnsurePointExists(PointGeometry p, ILength tol, Operation creator)
        {
            PointFeature result = (PointFeature)m_Index.QueryClosest(p, tol, SpatialType.Point);
            if (result==null)
            {
                IEntity e = creator.MapModel.DefaultPointType;
                uint ss = Session.ReserveNextItem();
                result = new PointFeature(creator, ss, e, p);
                m_Index.Add(result);
                m_Result.Add(result);
            }
            return result;
        }

        private Circle EnsureCircleExists(PointFeature center, double radius, ILength tol, Operation creator)
        {
            // The index refers to the data loaded from the current NTX file. It holds only
            // information for points & circles, so we should only find circles at this stage.

            Position p = new Position(center.X, center.Y+radius);
            ISpatialObject so = m_Index.QueryClosest(p, tol, SpatialType.Line);
            if (so==null)
            {
                so = new Circle(center, radius);
                m_Index.Add(so);
            }

            Debug.Assert(so is Circle);
            return (Circle)so;
        }

        private PointGeometry[] GetPositions(Ntx.Line line)
        {
            PointGeometry[] pts = new PointGeometry[line.NumPosition];
            for (int i=0; i<line.NumPosition; i++)
            {
                Ntx.Position xp = line.Position(i);
                pts[i] = new PointGeometry(xp.Easting, xp.Northing);
            }

            return pts;
        }

        private Feature ImportName(Ntx.Name name, Operation creator)
        {
            /*
	// Get pointer to the applicable map theme
	CeTheme theme(Name.GetTheme());
	CeTheme* pTheme = theme.AddTheme();

	// Get pointer to the entity type.
	GRAPHICSTYPE geom = ANNOTATION;
	if ( Name.IsLabel() ) geom = POLYGON;
	CeEntity* pEntity = AddEntity(Name.GetpFeatureCode(),pTheme,geom);
             */
            IEntity entity = GetEntityType(name, SpatialType.Text);

            // Get the text string
            string text = name.Text;

	        // Get the position of the centre of the 1st character
	        Ntx.Position pos = name.Position(0);
            IPosition vcentre = new Position(pos.Easting, pos.Northing);

            // Get text metrics
            float height = name.Height;
            float spacing = name.Spacing;
            float rotation = name.Rotation;

            // Calculate the top left corner of the first character using
            // the text metrics we just got ...

            // Get the width of the first character. For names that contain 
            // only one character, the spacing we have will be zero, so in
            // that case, deduce the width of the character via the covering
            // rectangle.

            float charwidth = spacing;
            if (charwidth < Constants.TINY)
            {
                // Get the covering rectangle.
                Ntx.Position nw = name.NorthWest;
                Ntx.Position se = name.SouthEast;

                // And get the dimensions.
                double dx = se.Easting - nw.Easting;
                double dy = nw.Northing - se.Northing;

                // If the cover is screwed up, assume the width is 80% of the text height.
                if (dy < Constants.TINY)
                    charwidth = (float)(height * 0.8);
                else
                    charwidth = (float)(height * (dx/dy));
            }

            // Define the bearing from bottom to top of the text.
            double vbear = (double)rotation;

            // Get position directly above the centre of the 1st char.
            IPosition above = Geom.Polar(vcentre, vbear, 0.5 * (double)height);

            // Define the bearing from the point we just got to the
            // start of the text string.
            double hbear = vbear - Constants.PIDIV2;

            // Back up half a character to get the initial corner.
            PointGeometry topleft = new PointGeometry(Geom.Polar(above, hbear, 0.5 * (double)charwidth));

            IFont font = null;
            double width = (double)text.Length * charwidth;
            TextFeature result = null;

            if (name.IsLabel)
            {
                // Create key text
                string keystr = name.Text;
                KeyTextGeometry kt = new KeyTextGeometry(topleft, font, height, width, rotation);
                uint ss = Session.ReserveNextItem();
                result = new TextFeature(creator, ss, entity, kt);
                kt.Label = result;
                result.SetTopology(true);

                // Define the label's foreign ID and form a two-way association
                ForeignId fid = GetFeatureId(keystr);
                Debug.Assert(fid != null);
                fid.Add(result);

                // Remember the reference position of the label.
                Ntx.Position xp = name.RefPosition;
                IPointGeometry pp = new PointGeometry(xp.Easting, xp.Northing);
                result.SetPolPosition(pp);
            }
            else
            {
                // Create a miscellaneous text label.
                MiscTextGeometry mt = new MiscTextGeometry(text, topleft, font, height, width, rotation);
                uint ss = Session.ReserveNextItem();
                result = new MiscTextFeature(creator, ss, entity, mt);
                result.SetTopology(false);
            }

            return result;
        }

        private Feature ImportSymbol(Ntx.Symbol symbol, Operation creator)
        {
            IEntity what = GetEntityType(symbol, SpatialType.Point);

            // Get the position
	        Ntx.Position pos = symbol.Position;
            PointGeometry g = new PointGeometry(pos.Easting, pos.Northing);

            // Ignore positions at 0,0!
            if (g.Easting.Microns==0 && g.Northing.Microns==0)
                return null;

            uint ss = Session.ReserveNextItem();
            PointFeature p = new PointFeature(creator, ss, what, g);
            /*

	static LOGICAL warned=FALSE;	// debug

	// Get pointer to the map theme
	CeTheme theme(Symbol.GetTheme());
	const CeTheme* const pTheme = theme.AddTheme();

	// Get pointer to the entity
	CeEntity* pEntity = AddEntity(Symbol.GetpFeatureCode(),pTheme,VERTEX);

	// Get the position
	const CxPosition& pos = Symbol.GetPosition();

	// For the time being ...
	if ( !warned && pos.Is3D() ) {
		AfxMessageBox("Elevation data is being stripped.");
		warned = TRUE;
	}

	// Add the position of the symbol.
	CeVertex vtx(pos.GetEasting(),pos.GetNorthing()) ;
	const CeLocation* pLoc = pMap->AddLocation(vtx);

	// If the location does not already have an associated point
	// feature, add one now (the location may have been previously
	// added via the import of a line).

	// Note that this version of AddPoint will always add a duplicate
	// point at the specified location.

	CePoint* pPoint = pMap->AddPoint((CeLocation* const)pLoc,pEntity);
            */

            // Define foreign ID (if any) ...
            string keystr = symbol.Key;
            ForeignId fid = GetFeatureId(keystr);
            if (fid != null)
                fid.Add(p);

            return p;
        }

        /// <summary>
        /// Obtains the ID object that corresponds to the supplied formatted key.
        /// </summary>
        /// <param name="keystr">The formatted key obtained during the import</param>
        /// <returns>The ID corresponding to the supplied key (null if the supplied key is null
        /// or blank)</returns>
        ForeignId GetFeatureId(string keystr)
        {
            if (String.IsNullOrEmpty(keystr))
                return null;

            // First check the current map model to see if the supplied key matches a
            // previously created ID
            CadastralMapModel mapModel = CadastralMapModel.Current;
            ForeignId result = mapModel.FindForeignId(keystr);
            if (result != null)
                return result;

            // Check the index of keys that have been created during this import (they
            // won't be added to the map model until the loading phase has been completed).
            if (m_KeyIndex.TryGetValue(keystr, out result))
                return result;

            // Remember a new foreign key as part of this import (it will be added to the model
            // after the loading phase has been completed)
            result = new ForeignId(keystr);
            m_KeyIndex.Add(keystr, result);
            return result;
        }
    }

}
