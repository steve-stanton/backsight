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

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="23-OCT-1997" />
    /// <summary>
    /// Any sort of survey observation.
    /// </summary>
    abstract class Observation : IXmlContent
    {
        abstract internal bool HasReference(Feature feature);
        abstract internal void OnRollback(Operation op);

        protected Observation() : base()
        {
        }

        protected Observation(Observation copy)
        {
        }

        /// <summary>
        /// Attempts to make a distance out of this observation and a from-point.
        /// </summary>
        /// <param name="from">The point the distance was measured from.</param>
        /// <returns>
        /// The distance, in metres on the ground. If a distance cannot be deduced,
        /// this will be 0.0. NOTE: may actually be a distance on the mapping plane if this
        /// observation is an offset point. The caller needs to check.</returns>
        /// <devnote>
        /// This function was written to assist in the implementation of the
        /// Intersect Direction & Distance command, which allows a distance to be specified
        /// using an offset point. Since offsets and distance object do not inherit from some
        /// place where we could define a pure virtual, this function exists to make explicit
        /// checks on the sort of distance we have (I don't want to define stubs for all the
        /// other objects which won't be able to return a distance). Re-arranging the class
        /// hierarchy would be better.
        /// </devnote>
        public ILength GetDistance(PointFeature from)
        {
            // It's easy if the observation is a distance object.
            Distance dist = (this as Distance);
            if (dist!=null)
                return dist;

            // Can't do anything if the from point is undefined.
	        if (from==null)
                return Length.Zero;

            // See if we have an offset point. If so, the distance is the
            // distance from the from-point to the offset-point.
            OffsetPoint offset = (OffsetPoint)this;
            if (offset==null)
                return Length.Zero;

            return new Length(Geom.Distance(offset.Point, from));
        }

        /// <summary>
        /// Returns the edit that is currently being saved. All derived classes should
        /// call this function when they are saving themselves, to confirm that the observation
        /// is associated with an edit.
        /// </summary>
        /*
        protected Operation SaveOp
        {
            get
            {
                Operation edit = EditingController.Current.CurrentEdit;
                if (edit==null)
                    throw new ArgumentNullException("Observation has no operation.");
                return edit;
            }
        }
         */

        /// <summary>
        /// Adds references to existing features referenced by this observation.
        /// 
        /// This should be called by implementations of <c>Operation.AddReferences</c>
        /// (for each instance of Observation that the operation involves). This ensures
        /// that the features utilized by observations are cross-referenced to the editing
        /// operations that depend on them.
        /// </summary>
        /// <param name="op">The operation that makes use of this observation</param>
        internal abstract void AddReferences(Operation op);

        #region IXmlContent Members

        /// <summary>
        /// Writes the content of this class. This is called by
        /// <see cref="XmlContentWriter.WriteElement"/>
        /// after the element name and class type (xsi:type) have been written.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        abstract public void WriteContent(XmlContentWriter writer);

        /// <summary>
        /// Loads the content of this class. This is called by
        /// <see cref="XmlContentReader"/> during deserialization from XML (just
        /// after the default constructor has been invoked).
        /// </summary>
        /// <param name="reader">The reading tool</param>
        abstract public void ReadContent(XmlContentReader reader);

        public void WriteContent(ContentWriter writer)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
