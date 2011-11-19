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

using Backsight.Environment;

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="03-DEC-1998" was="CeIntersectLine" />
    /// <summary>
    /// Operation to intersect 2 lines.
    /// </summary>
    class IntersectTwoLinesOperation : IntersectOperation, IRecallable
    {
        #region Class data

        /// <summary>
        /// The 1st line to intersect.
        /// </summary>
        readonly LineFeature m_Line1;

        /// <summary>
        /// True if the 1st line needs to be split at the intersection.
        /// </summary>
        readonly bool m_IsSplit1;

        /// <summary>
        /// The 2nd line to intersect.
        /// </summary>
        readonly LineFeature m_Line2;

        /// <summary>
        /// True if the 2nd line needs to be split at the intersection.
        /// </summary>
        readonly bool m_IsSplit2;

        /// <summary>
        /// The point closest to the intersection (usually defaulted to one of
        /// the end points for the 2 lines).
        /// </summary>
        readonly PointFeature m_CloseTo;

        // Creations ...

        /// <summary>
        /// The created intersection point (if any). May have existed previously.
        /// </summary>
        PointFeature m_Intersection;

        // Relating to line splits ...

        /// <summary>
        /// The portion of m_Line1 prior to the intersection (null if m_IsSplit1==false).
        /// </summary>
        LineFeature m_Line1a;

        /// <summary>
        /// The portion of m_Line1 after the intersection (null if m_IsSplit1==false).
        /// </summary>
        LineFeature m_Line1b;

        /// <summary>
        /// The portion of m_Line2 prior to the intersection (null if m_IsSplit2==false).
        /// </summary>
        LineFeature m_Line2a;

        /// <summary>
        /// The portion of m_Line2 after the intersection (null if m_IsSplit2==false).
        /// </summary>
        LineFeature m_Line2b;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IntersectTwoLinesOperation"/> class
        /// </summary>
        /// <param name="session">The session the new instance should be added to</param>
        /// <param name="sequence">The sequence number of the edit within the session (specify 0 if
        /// a new sequence number should be reserved). A non-zero value is specified during
        /// deserialization from the database.</param>
        /// <param name="line1">The 1st line to intersect.</param>
        /// <param name="wantsplit1">True if 1st line should be split at the intersection.</param>
        /// <param name="line2">The 2nd line to intersect.</param>
        /// <param name="wantsplit2">True if 2nd line should be split at the intersection.</param>
        /// <param name="closeTo">The point the intersection has to be close to. Used if
        /// there is more than one intersection to choose from. If null is specified, a
        /// default point will be selected.</param>
        internal IntersectTwoLinesOperation(Session session, uint sequence, LineFeature line1, bool wantSplit1,
                                            LineFeature line2, bool wantSplit2, PointFeature closeTo)
            : base(session, sequence)
        {
            if (line1==null || line2==null || closeTo==null)
                throw new ArgumentNullException();

            m_Line1 = line1;
            m_IsSplit1 = wantSplit1;
            m_Line2 = line2;
            m_IsSplit2 = wantSplit2;
            m_CloseTo = closeTo;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntersectTwoLinesOperation"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal IntersectTwoLinesOperation(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            FeatureStub to;
            string idLine1a, idLine1b, idLine2a, idLine2b;
            ReadData(editDeserializer, out m_Line1, out m_Line2, out m_CloseTo,
                            out to, out idLine1a, out idLine1b, out idLine2a, out idLine2b);

            m_IsSplit1 = (idLine1a != null && idLine1b != null);
            m_IsSplit2 = (idLine2a != null && idLine2b != null);

            DeserializationFactory dff = new DeserializationFactory(this);
            dff.AddFeatureStub(DataField.To, to);
            dff.AddLineSplit(m_Line1, DataField.SplitBefore1, idLine1a);
            dff.AddLineSplit(m_Line1, DataField.SplitAfter1, idLine1b);
            dff.AddLineSplit(m_Line2, DataField.SplitBefore2, idLine2a);
            dff.AddLineSplit(m_Line2, DataField.SplitAfter2, idLine2b);
            ProcessFeatures(dff);
        }

        #endregion

        /// <summary>
        /// The 1st line to intersect.
        /// </summary>
        internal LineFeature Line1 // was GetpArc1
        {
            get { return m_Line1; }
        }

        /// <summary>
        /// True if the 1st line needs to be split at the intersection.
        /// </summary>
        internal bool IsSplit1
        {
            get { return m_IsSplit1; }
            //set { m_IsSplit1 = value; }
        }

        /// <summary>
        /// The 2nd line to intersect.
        /// </summary>
        internal LineFeature Line2 // was GetpArc2
        {
            get { return m_Line2; }
        }

        /// <summary>
        /// True if the 2nd line needs to be split at the intersection.
        /// </summary>
        internal bool IsSplit2
        {
            get { return m_IsSplit2; }
            //set { m_IsSplit2 = value; }
        }

        /// <summary>
        /// The point feature at the intersection created by this edit.
        /// </summary>
        internal override PointFeature IntersectionPoint
        {
            get { return m_Intersection; }
        }

        /// <summary>
        /// Was the intersection created at it's default position? Always true.
        /// </summary>
        internal override bool IsDefault
        {
            get { return true; }
        }

        /// <summary>
        /// The point closest to the intersection (usually defaulted to one of
        /// the end points for the 2 lines).
        /// For use when relocating the intersection as part of rollforward processing).
        /// </summary>
        internal override PointFeature ClosePoint
        {
            get { return m_CloseTo; }
        }

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return "Intersect two lines"; }
        }

        /// <summary>
        /// The features created by this editing operation.
        /// </summary>
        internal override Feature[] Features
        {
            get
            {
                List<Feature> result = new List<Feature>(5);

                // The intersection point MIGHT have existed previously.
                if (m_Intersection!=null && m_Intersection.Creator==this)
                    result.Add(m_Intersection);

                if (m_Line1a!=null)
                    result.Add(m_Line1a);

                if (m_Line1b!=null)
                    result.Add(m_Line1b);

                if (m_Line2a!=null)
                    result.Add(m_Line2a);

                if (m_Line2b!=null)
                    result.Add(m_Line2b);

                return result.ToArray();
            }
        }

        /// <summary>
        /// The unique identifier for this edit.
        /// </summary>
        internal override EditingActionId EditId
        {
            get { return EditingActionId.LineIntersect; }
        }

        /// <summary>
        /// Obtains the features that are referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        /// <returns>The referenced features (never null, but may be an empty array).</returns>
        public override Feature[] GetRequiredFeatures()
        {
            return new Feature[]
            {
                m_Line1,
                m_Line2,
                m_CloseTo
            };
        }

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        /// <returns>True if operation was rolled back ok</returns>
        internal override bool Undo()
        {
            base.OnRollback();

            // Cut direct refs made by this operation.
            if (m_Line1!=null)
                m_Line1.CutOp(this);

            if (m_Line2!=null)
                m_Line2.CutOp(this);

            if (m_CloseTo!=null)
                m_CloseTo.CutOp(this);

            // Undo the intersect point and any lines
            Rollback(m_Intersection);
            Rollback(m_Line1a);
            Rollback(m_Line1b);
            Rollback(m_Line2a);
            Rollback(m_Line2b);

            // If we actually did splits, re-activate the original line.
            if (m_Line1a!=null || m_Line1b!=null)
            {
                m_Line1a = null;
                m_Line1b = null;
                m_Line1.Restore();
            }

            if (m_Line2a!=null || m_Line2b!=null)
            {
                m_Line2a = null;
                m_Line2b = null;
                m_Line2.Restore();
            }

            return true;
        }

        /// <summary>
        /// Checks whether this operation makes reference to a specific feature.
        /// </summary>
        /// <param name="feat">The feature to check for.</param>
        /// <returns>True if this edit depends on (contains a reference to) the supplied feature</returns>
        bool HasReference(Feature feat)
        {
            return (Object.ReferenceEquals(m_Line1, feat) ||
                    Object.ReferenceEquals(m_Line2, feat) ||
                    Object.ReferenceEquals(m_CloseTo, feat));
        }

        /// <summary>
        /// Attempts to locate a superseded (inactive) line that was the parent of
        /// a specific line.
        /// </summary>
        /// <param name="line">The line of interest</param>
        /// <returns>The superseded line that the line of interest was derived from. Null if
        /// this edit did not create the line of interest.</returns>
        internal override LineFeature GetPredecessor(LineFeature line)
        {
            if (Object.ReferenceEquals(m_Line1a, line) ||
                Object.ReferenceEquals(m_Line1b, line))
                return m_Line1;

            if (Object.ReferenceEquals(m_Line2a, line) ||
                Object.ReferenceEquals(m_Line2b, line))
                return m_Line2;

            return null;
        }

        /// <summary>
        /// Calculates the position of the intersection (if any).
        /// </summary>
        /// <returns>The position of the intersection (null if it cannot be calculated).</returns>
        IPosition Calculate()
        {
            IPosition xsect;
            PointFeature closest;
            if (m_Line1.Intersect(m_Line2, m_CloseTo, out xsect, out closest))
                return xsect;
            else
                return null;
        }

        /// <summary>
        /// Executes this operation.
        /// </summary>
        /// <param name="pointId">The key and entity type to assign to the intersection point.</param>
        internal void Execute(IdHandle pointId)
        {
            FeatureFactory ff = new FeatureFactory(this);

            FeatureId fid = pointId.CreateId();
            IFeature x = new FeatureStub(this, pointId.Entity, fid);
            ff.AddFeatureDescription(DataField.To, x);

            if (m_IsSplit1)
            {
                // See FeatureFactory.MakeSection - the only thing that really matters is the
                // session sequence number that will get picked up by the FeatureStub constructor.
                ff.AddFeatureDescription(DataField.SplitBefore1, new FeatureStub(this, m_Line1.EntityType, null));
                ff.AddFeatureDescription(DataField.SplitAfter1, new FeatureStub(this, m_Line1.EntityType, null));
            }

            if (m_IsSplit2)
            {
                ff.AddFeatureDescription(DataField.SplitBefore2, new FeatureStub(this, m_Line2.EntityType, null));
                ff.AddFeatureDescription(DataField.SplitAfter2, new FeatureStub(this, m_Line2.EntityType, null));
            }

            base.Execute(ff);

            //////////
            /*
            // Calculate the position of the point of intersection.
            IPosition xsect;
            PointFeature closest;
            if (!m_Line1.Intersect(m_Line2, m_CloseTo, out xsect, out closest))
                throw new Exception("Cannot calculate intersection point");

            // Add the intersection point
            m_Intersection = AddIntersection(xsect, pointId);
            
            // Are we splitting the input lines? If so, do it.
            m_IsSplit1 = wantsplit1;
            if (m_IsSplit1)
                SplitLine(m_Intersection, m_Line1, out m_Line1a, out m_Line1b);

            m_IsSplit2 = wantsplit2;
            if (m_IsSplit2)
                SplitLine(m_Intersection, m_Line2, out m_Line2a, out m_Line2b);

            // Peform standard completion steps
            Complete();
             */
        }

        /// <summary>
        /// Creates any new spatial features (without any geometry)
        /// </summary>
        /// <param name="ff">The factory class for generating spatial features</param>
        internal override void ProcessFeatures(FeatureFactory ff)
        {
            m_Intersection = ff.CreatePointFeature(DataField.To);

            if (m_IsSplit1)
            {
                SectionLineFeature lineBefore1, lineAfter1;
                ff.MakeSections(m_Line1, DataField.SplitBefore1, m_Intersection, DataField.SplitAfter1,
                                    out lineBefore1, out lineAfter1);
                m_Line1a = lineBefore1;
                m_Line1b = lineAfter1;
            }

            if (m_IsSplit2)
            {
                SectionLineFeature lineBefore2, lineAfter2;
                ff.MakeSections(m_Line2, DataField.SplitBefore2, m_Intersection, DataField.SplitAfter2,
                                    out lineBefore2, out lineAfter2);
                m_Line2a = lineBefore2;
                m_Line2b = lineAfter2;
            }
        }

        /// <summary>
        /// Performs the data processing associated with this editing operation.
        /// </summary>
        /// <param name="ctx">The context in which the geometry is being calculated.</param>
        internal override void CalculateGeometry(EditingContext ctx)
        {
            IPosition p = Calculate();
            PointGeometry pg = PointGeometry.Create(p);
            m_Intersection.ApplyPointGeometry(ctx, pg);
        }

        /// <summary>
        /// The portion of m_Line1 prior to the intersection (null if m_IsSplit1==false).
        /// </summary>
        internal LineFeature Line1BeforeSplit
        {
            get { return m_Line1a; }
        }

        /// <summary>
        /// The portion of m_Line1 after the intersection (null if m_IsSplit1==false).
        /// </summary>
        internal LineFeature Line1AfterSplit
        {
            get { return m_Line1b; }
        }

        /// <summary>
        /// The portion of m_Line2 prior to the intersection (null if m_IsSplit2==false).
        /// </summary>
        internal LineFeature Line2BeforeSplit
        {
            get { return m_Line2a; }
        }

        /// <summary>
        /// The portion of m_Line2 after the intersection (null if m_IsSplit2==false).
        /// </summary>
        internal LineFeature Line2AfterSplit
        {
            get { return m_Line2b; }
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public override void WriteData(EditSerializer editSerializer)
        {
            base.WriteData(editSerializer);

            editSerializer.WriteFeatureRef<LineFeature>(DataField.Line1, m_Line1);
            editSerializer.WriteFeatureRef<LineFeature>(DataField.Line2, m_Line2);
            editSerializer.WriteFeatureRef<PointFeature>(DataField.CloseTo, m_CloseTo);
            editSerializer.WritePersistent<FeatureStub>(DataField.To, new FeatureStub(m_Intersection));

            if (m_Line1a != null)
                editSerializer.WriteString(DataField.SplitBefore1, m_Line1a.DataId);

            if (m_Line1b != null)
                editSerializer.WriteString(DataField.SplitAfter1, m_Line1b.DataId);

            if (m_Line2a != null)
                editSerializer.WriteString(DataField.SplitBefore2, m_Line2a.DataId);

            if (m_Line2b != null)
                editSerializer.WriteString(DataField.SplitAfter2, m_Line2b.DataId);
        }

        /// <summary>
        /// Reads data that was previously written using <see cref="WriteData"/>
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        /// <param name="line1">The 1st line to intersect.</param>
        /// <param name="line2">The 2nd line to intersect.</param>
        /// <param name="closeTo">The point closest to the intersection.</param>
        /// <param name="to">The created intersection point (if any). May have existed previously.</param>
        /// <param name="idLine1a">The ID of the portion of the first line prior to the intersection (null if no split).</param>
        /// <param name="idLine1b">The ID of the portion of the first line after the intersection (null if no split).</param>
        /// <param name="idLine2a">The ID of the portion of the second line prior to the intersection (null if no split).</param>
        /// <param name="idLine2b">The ID of the portion of the second line after the intersection (null if no split).</param>
        static void ReadData(EditDeserializer editDeserializer, out LineFeature line1, out LineFeature line2, out PointFeature closeTo,
                                out FeatureStub to, out string idLine1a, out string idLine1b, out string idLine2a, out string idLine2b)
        {
            line1 = editDeserializer.ReadFeatureRef<LineFeature>(DataField.Line1);
            line2 = editDeserializer.ReadFeatureRef<LineFeature>(DataField.Line2);
            closeTo = editDeserializer.ReadFeatureRef<PointFeature>(DataField.CloseTo);
            to = editDeserializer.ReadPersistent<FeatureStub>(DataField.To);
            idLine1a = (editDeserializer.IsNextField(DataField.SplitBefore1) ? editDeserializer.ReadString(DataField.SplitBefore1) : null);
            idLine1b = (editDeserializer.IsNextField(DataField.SplitAfter1) ? editDeserializer.ReadString(DataField.SplitAfter1) : null);
            idLine2a = (editDeserializer.IsNextField(DataField.SplitBefore2) ? editDeserializer.ReadString(DataField.SplitBefore2) : null);
            idLine2b = (editDeserializer.IsNextField(DataField.SplitAfter2) ? editDeserializer.ReadString(DataField.SplitAfter2) : null);
        }
    }
}
