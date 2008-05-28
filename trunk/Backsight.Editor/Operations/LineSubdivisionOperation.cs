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
using Backsight.Geometry;

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="23-OCT-1997" />
    /// <change by="Steve Stanton" on="27-MAR-2002" why="Significant changes to accommodate secondary faces." />
    /// <summary>
    /// Operation to subdivide a line.
    /// </summary>
    /// <remarks>
    ///	When you subdivide a line that has the same base layer as the active editing layer,
    ///	sections get created for each observed distance, and the original line gets de-activated.
    ///
    ///	Things get more complicated if the editing layer is derived from the line's base layer.
    ///	For example, suppose the line that's getting subdivided is on layers 1 through 5 (where
    ///	layer 1 is the base theme), but the editing layer is #2. In that case, the resultant
    ///	sections relate only to layers 2-5. The original line is modified to refer only to
    /// layer 1 (i.e. the portion of the theme hierarchy that did not get subdivided). Like this:
    ///
    ///	-------	Original line relates to 1-5
    ///     
    ///	Edit on layer 2 produces:
    ///
    ///  ------- Original line now relates only to layer 1
    ///  ---X--- Two sections relating to layers 2-5
    ///
    ///	If you then change the editing layer to #1, you will only see the line that relates to
    ///	layer#1, and you are at liberty to subdivide that too. In that scenario, the subdivision
    ///	relates only to layer#1. The fact that the original line got subdivided on a derived
    ///	layer is irrelevant.
    ///
    ///	Now suppose you change the editing layer to #5. The line on layer 1 will be invisible,
    ///	but you will see the two sections that are shared by layers 2-5. At that stage, you decide
    ///	to add a second face (which you can do via the update dialog). When you do that, additional
    ///	sections get created that relate only to layer 5. Unlike the initially created sections,
    ///	sections on a secondary face are marked as non-topological. The sections are only produced
    ///	because concrete features must exist in order to annotate them with the associated distance
    ///	(the sections are also assigned a "void" status, which means they are not meant to get written
    ///	upon export to something like an AutoCad file). So what you have now looks like this:
    ///
    ///  ------- Line relating to layer 1
    ///	---X--- Two sections relating to layer 2-5 (primary face)
    ///  -X---X- Additional face for layer 5
    ///
    ///	Now change the editing layer to layer 3. Since the additional face was created on layer 5,
    ///	it will not be apparent. So you can go on to define a secondary face once more. In that case,
    ///	it might be acceptable to reference the secondary sections to layers 3-5. However, since
    ///	layer 5 already has an invisible second face, it seems more sensible to restrict them to layers
    ///	3-4. The end result will therefore consist of the following:
    ///
    ///  ------- Line relating to layer 1
    ///	---X--- Two sections relating to layers 2-5 (primary face)
    ///  --X---- Additional face for layers 3-4
    ///  -X---X- Additional face for layer 5
    /// </remarks>
    [Serializable]
    class LineSubdivisionOperation : Operation
    {
        #region Class data

        /// <summary>
        /// The line that was subdivided.
        /// </summary>
        readonly LineFeature m_Line;

        /// <summary>
        /// The face(s) for the subdivided line. It is anticipated that there will usually be
        /// only one face. You need two faces when dealing with "staggered" property lots,
        /// so long as the 2nd face refers to the same editing layers as the primary face.
        /// You need more than two faces if you have a "complex" staggered property lot,
        /// where the staggering is different on different layers within a mapping theme.
        /// </summary>
        IPossibleList<LineSubdivisionFace> m_Faces;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>LineSubdivision</c> for the supplied line.
        /// </summary>
        /// <param name="line">The line that is being subdivided.</param>
        internal LineSubdivisionOperation(LineFeature line)
            : base()
        {
            m_Line = line;
            m_Faces = null;
        }

        #endregion

        /// <summary>
        /// The line that was subdivided.
        /// </summary>
        internal LineFeature Parent
        {
            get { return m_Line; }
        }

        internal bool IsMultiFace
        {
            get { return (m_Faces.Count>1); }
        }

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return "Line subdivision"; }
        }

        /// <summary>
        /// Adds the distance observations for one face of the subdivided line. If any
        /// distances are already defined, the distances relate to an additional face.
        /// <para/>
        /// This method should be called only ONCE prior to a call to <c>Execute</c>.
        /// Additional faces are defined via the update mechanism, which ends up calling
        /// <c>Rollforward</c>
        /// </summary>
        /// <param name="distances">The distance observations</param>
        /// <returns>
        /// True if distances added. False if the number of distances is less than 2.
        /// </returns>
        internal bool AddDistances(List<Distance> distances)
        {
            // Must have at least two distances
            if (distances==null)
                throw new ArgumentNullException();

            if (distances.Count<2)
                throw new ArgumentException();

            LineSubdivisionFace face = new LineSubdivisionFace(distances);
            m_Faces = (m_Faces==null ? face : m_Faces.Add(face));
            return true;
        }

        /// <summary>
        /// Execute line subdivision. This should be called only to process the initial
        /// subdivision of a line. Any subsequent faces are handled via <c>Rollfoward</c>
        /// </summary>
        internal void Execute()
        {
            // Must only be one face
            if (NumFace>1)
                throw new InvalidOperationException("Attempt to add multiple faces during initial line subdivision.");

            // There has to be at least 2 distances
            if (NumFace==0)
                throw new InvalidOperationException("Distance observation have not been assigned.");

            LineSubdivisionFace face = m_Faces[0];
            if (face.Sections.Length<2)
                throw new InvalidOperationException("Line subdivision needs at least 2 observed distances.");

            // Adjust the observed distances
            double[] adjray = Adjust(face);

            // If the active editing layer is derived from the subdivided line's base layer,...
	        //CeSubTheme* pSubTheme = GetSubTheme(*m_pArc);

            // Create line sections
            MeasuredLineFeature[] sections = face.Sections;
	        double edist=0.0;		// Distance to end of section.
            PointFeature start = m_Line.StartPoint;

        	for (int i=0; i<adjray.Length; i++)
	        {
		        edist += adjray[i];
                sections[i].Line = MakeSection(start, edist);
                start = sections[i].Line.EndPoint;
	        }

            // De-activate the parent line
            m_Line.IsInactive = true;

            // Peform standard completion steps
            Complete();
        }

        int NumFace
        {
            get { return (m_Faces==null ? 0 : m_Faces.Count); }
        }

        /// <summary>
        /// Adjusts the observed distances for a line subdivision face.
        /// </summary>
        /// <param name="face">The face to adjust</param>
        /// <returns>The adjusted distances (in meters) for each observed distance</returns>
        double[] Adjust(LineSubdivisionFace face)
        {
            return face.GetAdjustedLengths(m_Line.Length);
        }

        /// <summary>
        /// Finds the observed length of a line that was created by this operation.
        /// </summary>
        /// <param name="line">The line to find</param>
        /// <returns>The corresponding distance (null if not found)</returns>
        internal override Distance GetDistance(LineFeature line)
        {
            if (line==null || m_Faces==null)
                return null;

            foreach (LineSubdivisionFace face in m_Faces)
            {
                foreach (MeasuredLineFeature mf in face.Sections)
                {
                    if (Object.ReferenceEquals(mf.Line, line))
                        return mf.ObservedLength;
                }
            }

            return null;
        }

        /// <summary>
        /// The features that were created by this operation.
        /// </summary>
        internal override Feature[] Features
        {
            get
            {
                if (m_Faces==null)
                    return new Feature[0];

                List<Feature> result = new List<Feature>(100);

                foreach (LineSubdivisionFace face in m_Faces)
                {
                    foreach (MeasuredLineFeature mf in face.Sections)
                    {
                        result.Add(mf.Line);

                        // If the point feature at the end of the section was created
                        // by this op, append that too. Take care to avoid any
                        // duplicates in situations where more than one face is involved.
                        PointFeature pf = mf.Line.EndPoint;
                        if (pf.Creator == this && !result.Contains(pf))
                            result.Add(pf);
                    }
                }

                return result.ToArray();
            }
        }

        internal override EditingActionId EditId
        {
            get { return EditingActionId.LineSubdivision; }
        }

        public override void AddReferences()
        {
            m_Line.AddOp(this);

            foreach (LineSubdivisionFace face in m_Faces)
                face.AddReferences(this);
        }

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        /// <returns>True if operation was rolled back ok</returns>
        internal override bool Undo()
        {
            base.OnRollback();

            m_Line.CutOp(this);

            // Process each face. Do it in reverse order, just in case
            // subsequently created faces have some sort of dependency
            // on the earlier faces.
            int nFace = m_Faces.Count;
            for (int i=nFace-1; i>=0; i--)
            {
                LineSubdivisionFace face = m_Faces[i];
                face.Undo(this);
            }

            // Restore the original line
            m_Line.Restore();
            return true;
        }

        /// <summary>
        /// Rollforward this edit in response to some sort of update.
        /// </summary>
        /// <returns>True if operation has been re-executed successfully</returns>
        internal override bool Rollforward()
        {
            // Return if this operation has not been marked as changed.
            if (!IsChanged)
                return base.OnRollforward();

            foreach (LineSubdivisionFace face in m_Faces)
                face.Rollforward(this);

            // Rollforward the base class.
            return base.OnRollforward();
        }

        internal bool CanCorrect
        {
            get { return true; }
        }

        /// <summary>
        /// Creates a section for this arc subdivision op.
        /// </summary>
        /// <param name="start">The point at the start of the section</param>
        /// <param name="edist">The distance to the end of the section.</param>
        /// <returns>The created section</returns>
        LineFeature MakeSection(PointFeature start, double edist)
        {
            SectionGeometry section = AddSection(start, edist);
            LineFeature newLine = m_Line.MakeSubSection(section, this);
            //MapModel.EditingIndex.Add(newLine);
            return newLine;
        }

        /// <summary>
        /// Adds a line section to the map. This adds the geometry for the section,
        /// together with terminal points, but NOT the line feature.
        /// 
        /// The caller is responsible for associating the operation with the section,
        /// and the parent line with the operation.
        /// </summary>
        /// <param name="start">The point at the start of the section</param>
        /// <param name="edist">Distance from the start of the parent line to the end
        /// of the section.</param>
        /// <returns>The new section.</returns>
        SectionGeometry AddSection(PointFeature start, double edist)
        {
            CadastralMapModel map = CadastralMapModel.Current;

            // Get the position for the end point.
            LineGeometry parent = m_Line.LineGeometry;
            IPosition end;
            parent.GetPosition(new Length(edist), out end);

            // Add points at these positions (with no ID & default entity). If they
            // did not previously exist, reference them to THIS operation.

            PointFeature ept = (end as PointFeature);
            if (ept==null)
                ept = (map.Index.QueryClosest(end, Length.Zero, SpatialType.Point) as PointFeature);

            if (ept==null)
            {
                ept = map.AddPoint(end, map.DefaultPointType, this);
                ept.SetNextId();
            }

            SectionGeometry section = new SectionGeometry(m_Line, start, ept);
            return section;
        }

        /// <summary>
        /// The first face
        /// </summary>
        internal LineSubdivisionFace FirstFace
        {
            get { return m_Faces[0]; }
        }

        /// <summary>
        /// Writes the content of this class. This is called by
        /// <see cref="XmlContentWriter.WriteElement"/>
        /// after the element name and class type (xsi:type) have been written.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteContent(XmlContentWriter writer)
        {
        }
    }
}
