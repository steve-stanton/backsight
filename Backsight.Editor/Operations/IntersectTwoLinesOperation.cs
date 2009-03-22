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

using Backsight.Environment;

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="03-DEC-1998" was="CeIntersectLine" />
    /// <summary>
    /// Operation to intersect 2 lines.
    /// </summary>
    class IntersectTwoLinesOperation : IntersectOperation
    {
        #region Class data

        /// <summary>
        /// The 1st line to intersect.
        /// </summary>
        LineFeature m_Line1;

        /// <summary>
        /// The 2nd line to intersect.
        /// </summary>
        LineFeature m_Line2;

        /// <summary>
        /// The point closest to the intersection (usually defaulted to one of
        /// the end points for the 2 lines).
        /// </summary>
        PointFeature m_CloseTo;

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
        /// True if the 1st line needs to be split at the intersection.
        /// </summary>
        bool m_IsSplit1;

        /// <summary>
        /// The portion of m_Line2 prior to the intersection (null if m_IsSplit2==false).
        /// </summary>
        LineFeature m_Line2a;

        /// <summary>
        /// The portion of m_Line2 after the intersection (null if m_IsSplit2==false).
        /// </summary>
        LineFeature m_Line2b;

        /// <summary>
        /// True if the 2nd line needs to be split at the intersection.
        /// </summary>
        bool m_IsSplit2;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IntersectTwoLinesOperation"/> class
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        internal IntersectTwoLinesOperation(Session s)
            : base(s)
        {
            m_Line1 = null;
            m_Line2 = null;
            m_CloseTo = null;
            m_Intersection = null;
            m_Line1a = null;
            m_Line1b = null;
            m_Line2a = null;
            m_Line2b = null;
            m_IsSplit1 = false;
            m_IsSplit2 = false;
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
        /// Adds references to existing features referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        public override void AddReferences()
        {
            m_Line1.AddOp(this);
            m_Line2.AddOp(this);
            m_CloseTo.AddOp(this);
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
        /// Rollforward this edit in response to some sort of update.
        /// </summary>
        /// <returns>True if operation has been re-executed successfully</returns>
        internal override bool Rollforward()
        {
            // Return if this operation has not been marked as changed.
            if (!IsChanged)
                return base.OnRollforward();

            // Re-calculate the position of the point of intersection.
            IPosition xsect;
            PointFeature closest;
            if (!m_Line1.Intersect(m_Line2, m_CloseTo, out xsect, out closest))
                throw new RollforwardException(this, "Cannot re-calculate intersection point.");

            // Update the intersection point to the new position.
            m_Intersection.Move(xsect);

            // Rollforward the base class.
            return base.OnRollforward();
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
        /// <param name="line1">The 1st line to intersect.</param>
        /// <param name="line2">The 2nd line to intersect.</param>
        /// <param name="closeTo">The point the intersection has to be close to. Used if
        /// there is more than one intersection to choose from. If null is specified, a
        /// default point will be selected.</param>
        /// <param name="wantsplit1">True if 1st line should be split at the intersection.</param>
        /// <param name="wantsplit2">True if 2nd line should be split at the intersection.</param>
        /// <param name="pointId">The key and entity type to assign to the intersection point.</param>
        internal void Execute(LineFeature line1, LineFeature line2, PointFeature closeTo,
                                bool wantsplit1, bool wantsplit2, IdHandle pointId)
        {
            // Calculate the position of the point of intersection.
            IPosition xsect;
            PointFeature closest;
            if (!line1.Intersect(line2, closeTo, out xsect, out closest))
                throw new Exception("Cannot calculate intersection point");

            // Add the intersection point
            m_Intersection = AddIntersection(xsect, pointId);

            // Remember input
            m_Line1 = line1;
            m_Line2 = line2;

            // If a close-to point was not specified, use the one we picked.
            if (closeTo == null)
                m_CloseTo = closest;
            else
                m_CloseTo = closeTo;

            // Are we splitting the input lines? If so, do it.
            m_IsSplit1 = wantsplit1;
            if (m_IsSplit1)
                SplitLine(m_Intersection, m_Line1, out m_Line1a, out m_Line1b);

            m_IsSplit2 = wantsplit2;
            if (m_IsSplit2)
                SplitLine(m_Intersection, m_Line2, out m_Line2a, out m_Line2b);

            // Peform standard completion steps
            Complete();
        }

        /// <summary>
        /// Executes this operation.
        /// </summary>
        /// <param name="line1">The 1st line to intersect.</param>
        /// <param name="line2">The 2nd line to intersect.</param>
        /// <param name="closeTo">The point the intersection has to be close to. Used if
        /// there is more than one intersection to choose from. If null is specified, a
        /// default point will be selected.</param>
        /// <param name="wantsplit1">True if 1st line should be split at the intersection.</param>
        /// <param name="wantsplit2">True if 2nd line should be split at the intersection.</param>
        /// <param name="pointType">The entity type to assign to the intersection point.</param>
        internal void Execute(LineFeature line1, LineFeature line2, PointFeature closeTo,
                                bool wantsplit1, bool wantsplit2, IEntity pointType)
        {
            // Calculate the position of the point of intersection.
            IPosition xsect;
            PointFeature closest;
            if (!line1.Intersect(line2, closeTo, out xsect, out closest))
                throw new Exception("Cannot calculate intersection point");

            // Add the intersection point
            m_Intersection = AddIntersection(xsect, pointType);

            // Remember input
            m_Line1 = line1;
            m_Line2 = line2;

            // If a close-to point was not specified, use the one we picked.
            if (closeTo == null)
                m_CloseTo = closest;
            else
                m_CloseTo = closeTo;

            // Are we splitting the input lines? If so, do it.
            m_IsSplit1 = wantsplit1;
            if (m_IsSplit1)
                SplitLine(m_Intersection, m_Line1, out m_Line1a, out m_Line1b);

            m_IsSplit2 = wantsplit2;
            if (m_IsSplit2)
                SplitLine(m_Intersection, m_Line2, out m_Line2a, out m_Line2b);

            // Peform standard completion steps
            Complete();
        }

        /// <summary>
        /// Updates this operation.
        /// </summary>
        /// <param name="line1">The 1st line to intersect.</param>
        /// <param name="line2">The 2nd line to intersect.</param>
        /// <param name="closeTo">The point the intersection has to be close to. Used if
        /// there is more than one intersection to choose from. If null is specified, a
        /// default point will be selected.</param>
        /// <param name="wantsplit1">True if 1st line should be split at the intersection.</param>
        /// <param name="wantsplit2">True if 2nd line should be split at the intersection.</param>
        /// <returns>True if operation updated ok.</returns>
        internal bool Correct(LineFeature line1, LineFeature line2, PointFeature closeTo,
                                bool wantsplit1, bool wantsplit2)
        {
            // Disallow attempts to change the split status

            if (wantsplit1 != m_IsSplit1 || wantsplit2 != m_IsSplit2)
                throw new Exception("You cannot make line splits via update.");

            // If the lines have changed, cut references to this
            // operation from the old lines, and change it so the
            // operation is referenced from the new lines.

            if (!Object.ReferenceEquals(m_Line1, line1))
            {
                m_Line1.CutOp(this);
                m_Line1 = line1;
                m_Line1.AddOp(this);
            }

            if (!Object.ReferenceEquals(m_Line2, line2))
            {
                m_Line2.CutOp(this);
                m_Line2 = line2;
                m_Line2.AddOp(this);
            }

            if (!Object.ReferenceEquals(m_CloseTo, closeTo))
            {
                if (m_CloseTo != null)
                    m_CloseTo.CutOp(this);

                m_CloseTo = closeTo;

                if (m_CloseTo != null)
                    m_CloseTo.AddOp(this);
            }

            return true;
        }

        /// <summary>
        /// Writes the attributes of this class.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteAttributes(XmlContentWriter writer)
        {
            base.WriteAttributes(writer);

            writer.WriteFeatureReference("Line1", m_Line1);
            writer.WriteFeatureReference("Line2", m_Line2);
            writer.WriteFeatureReference("CloseTo", m_CloseTo);
            writer.WriteBool("IsSplit1", m_IsSplit1);
            writer.WriteBool("IsSplit2", m_IsSplit2);
        }

        /// <summary>
        /// Writes any child elements of this class. This will be called after
        /// all attributes have been written via <see cref="WriteAttributes"/>.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteChildElements(XmlContentWriter writer)
        {
            base.WriteChildElements(writer);

            writer.WriteCalculatedPoint("To", m_Intersection);
            if (m_IsSplit1)
            {
                writer.WriteElement("Line1a", m_Line1a);
                writer.WriteElement("Line1b", m_Line1b);
            }
            if (m_IsSplit2)
            {
                writer.WriteElement("Line2a", m_Line2a);
                writer.WriteElement("Line2b", m_Line2b);
            }
        }

        /// <summary>
        /// Defines the attributes of this content
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public override void ReadAttributes(XmlContentReader reader)
        {
            base.ReadAttributes(reader);

            m_Line1 = reader.ReadFeatureByReference<LineFeature>("Line1");
            m_Line2 = reader.ReadFeatureByReference<LineFeature>("Line2");
            m_CloseTo = reader.ReadFeatureByReference<PointFeature>("CloseTo");
            m_IsSplit1 = reader.ReadBool("IsSplit1");
            m_IsSplit2 = reader.ReadBool("IsSplit2");
        }

        /// <summary>
        /// Defines any child content related to this instance. This will be called after
        /// all attributes have been defined via <see cref="ReadAttributes"/>.
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public override void ReadChildElements(XmlContentReader reader)
        {
            base.ReadChildElements(reader);

            //IPosition p = Calculate();
            //m_Intersection = reader.ReadCalculatedPoint("To", p);
            m_Intersection = reader.ReadPoint("To");
            if (m_IsSplit1)
            {
                m_Line1a = reader.ReadElement<LineFeature>("Line1a");
                m_Line1b = reader.ReadElement<LineFeature>("Line1b");
            }
            if (m_IsSplit2)
            {
                m_Line2a = reader.ReadElement<LineFeature>("Line2a");
                m_Line2b = reader.ReadElement<LineFeature>("Line2b");
            }
        }

        /// <summary>
        /// Calculates the geometry for any features created by this edit.
        /// </summary>
        public override void CalculateGeometry()
        {
            IPosition p = Calculate();
            PointGeometry pg = PointGeometry.Create(p);
            m_Intersection.PointGeometry = pg;
        }
    }
}
