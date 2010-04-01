// <remarks>
// Copyright 2009 - Steve Stanton. This file is part of Backsight
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

namespace Backsight.Editor.Xml
{
    /// <summary>
    /// Base class for any sort of serialized spatial feature.
    /// </summary>
    /// <remarks>The remainder of this class is auto-generated, and may be found
    /// in the <c>Edits.cs</c> file.</remarks>
    public partial class FeatureData
    {
        /// <summary>
        /// Loads this feature as part of an editing operation. Derived types
        /// must implement this method, otherwise you will get an exception on
        /// deserialization from the database.
        /// </summary>
        /// <param name="op">The editing operation creating the feature</param>
        /// <returns>The spatial feature that was loaded</returns>
        //internal abstract Feature LoadFeature(Operation op);
        internal virtual Feature LoadFeature(Operation op)
        {
            throw new NotImplementedException("LoadFeature not implemented by: " + GetType().Name);
        }

        /*
        internal string ForeignKey
        {
            get
            {
                if (ItemElementName == Item2ChoiceType.)
                    return Item;
                else
                    return null;
            }

            set
            {
                Item = value;
                ItemElementName = ItemChoiceType.FirstArc;
            }
        }
        */
    }

    /// <summary>
    /// A serialized line feature with geometry defined by a circular arc.
    /// </summary>
    public partial class ArcData
    {
        /// <summary>
        /// Loads this feature as part of an editing operation
        /// </summary>
        /// <param name="op">The editing operation creating the feature</param>
        /// <returns>The spatial feature that was loaded</returns>
        internal override Feature LoadFeature(Operation op)
        {
            return new ArcFeature(op, this);
        }

        internal string FirstArc
        {
            get
            {
                if (ItemElementName == ItemChoiceType.FirstArc)
                    return Item;
                else
                    return null;
            }

            set
            {
                Item = value;
                ItemElementName = ItemChoiceType.FirstArc;
            }
        }

        internal string Center
        {
            get
            {
                if (ItemElementName == ItemChoiceType.Center)
                    return Item;
                else
                    return null;
            }

            set
            {
                Item = value;
                ItemElementName = ItemChoiceType.Center;
            }
        }
    }

    /// <summary>
    /// A serialized line feature with geometry defined by an array of positions.
    /// </summary>
    public partial class MultiSegmentData
    {
        /// <summary>
        /// Loads this feature as part of an editing operation
        /// </summary>
        /// <param name="op">The editing operation creating the feature</param>
        /// <returns>The spatial feature that was loaded</returns>
        internal override Feature LoadFeature(Operation op)
        {
            return new LineFeature(op, this);
        }
    }

    /// <summary>
    /// A serialized point feature.
    /// </summary>
    public partial class PointData
    {
        /// <summary>
        /// Loads this feature as part of an editing operation
        /// </summary>
        /// <param name="op">The editing operation creating the feature</param>
        /// <returns>The spatial feature that was loaded</returns>
        internal override Feature LoadFeature(Operation op)
        {
            return new PointFeature(op, this);
        }
    }

    /// <summary>
    /// A serialized line feature with geometry defined as a section of another line.
    /// </summary>
    public partial class SectionData
    {
        /// <summary>
        /// Loads this feature as part of an editing operation
        /// </summary>
        /// <param name="op">The editing operation creating the feature</param>
        /// <returns>The spatial feature that was loaded</returns>
        internal override Feature LoadFeature(Operation op)
        {
            return new LineFeature(op, this);
        }
    }

    /// <summary>
    /// A serialized line feature with geometry defined by two positions.
    /// </summary>
    public partial class SegmentData
    {
        /// <summary>
        /// Loads this feature as part of an editing operation
        /// </summary>
        /// <param name="op">The editing operation creating the feature</param>
        /// <returns>The spatial feature that was loaded</returns>
        internal override Feature LoadFeature(Operation op)
        {
            return new LineFeature(op, this);
        }
    }

    /// <summary>
    /// Serialized version of a point feature that shares geometry with
    /// another point feature.
    /// </summary>
    public partial class SharedPointData
    {
        /// <summary>
        /// Loads this feature as part of an editing operation
        /// </summary>
        /// <param name="op">The editing operation creating the feature</param>
        /// <returns>The spatial feature that was loaded</returns>
        internal override Feature LoadFeature(Operation op)
        {
            return new PointFeature(op, this);
        }
    }

    public partial class CalculatedFeatureData
    {
        public CalculatedFeatureData()
        {
        }

        internal CalculatedFeatureData(Feature f)
            : this(f, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalculatedFeatureData"/> class.
        /// </summary>
        /// <param name="f">The feature that is being serialized</param>
        /// <param name="allFields">Should all attributes be serialized? Specify <c>false</c> only
        /// if the feature actually represents something that previously existed at the calculated
        /// position.</param>
        internal CalculatedFeatureData(Feature f, bool allFields)
        {
            this.Id = f.DataId;

            if (allFields)
            {
                this.Entity = f.EntityType.Id;

                FeatureId fid = f.Id;
                if (fid != null)
                {
                    if (fid is NativeId)
                    {
                        this.Key = fid.RawId;
                        this.KeySpecified = true;
                    }
                    else
                        this.ForeignKey = fid.FormattedKey;
                }
            }
        }
    }

    /// <summary>
    /// A serialized text feature.
    /// </summary>
    public partial class RowTextData
    {
        /// <summary>
        /// Loads this feature as part of an editing operation
        /// </summary>
        /// <param name="op">The editing operation creating the feature</param>
        /// <returns>The spatial feature that was loaded</returns>
        internal override Feature LoadFeature(Operation op)
        {
            return new TextFeature(op, this);
        }
    }

    /// <summary>
    /// A serialized text feature.
    /// </summary>
    public partial class MiscTextData
    {
        /// <summary>
        /// Loads this feature as part of an editing operation
        /// </summary>
        /// <param name="op">The editing operation creating the feature</param>
        /// <returns>The spatial feature that was loaded</returns>
        internal override Feature LoadFeature(Operation op)
        {
            return new TextFeature(op, this);
        }
    }

    /// <summary>
    /// A serialized text feature that represents the user-perceived key for a spatial feature.
    /// </summary>
    public partial class KeyTextData
    {
        /// <summary>
        /// Loads this feature as part of an editing operation
        /// </summary>
        /// <param name="op">The editing operation creating the feature</param>
        /// <returns>The spatial feature that was loaded</returns>
        internal override Feature LoadFeature(Operation op)
        {
            return new TextFeature(op, this);
        }
    }
}
