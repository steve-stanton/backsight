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
using Backsight.Environment;

namespace Backsight.Editor.Xml
{
    /// <summary>
    /// Base class for any sort of serialized spatial feature.
    /// </summary>
    /// <remarks>The remainder of this class is auto-generated, and may be found
    /// in the <c>Edits.cs</c> file.</remarks>
    public partial class FeatureData
    {
        internal FeatureData(Feature f)
            : this(f, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureData"/> class.
        /// </summary>
        /// <param name="f">The feature that is being serialized</param>
        /// <param name="allFields">Should all attributes be serialized? Specify <c>false</c> only
        /// if the feature actually represents something that previously existed at the calculated
        /// position.</param>
        internal FeatureData(Feature f, bool allFields)
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

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureData"/> class.
        /// </summary>
        /// <param name="f">The feature that is being serialized</param>
        /// <param name="defaultEntityId">The ID of the default entity type. If the feature's
        /// entity type matches, it will not be recorded as part of this instance.</param>
        internal FeatureData(Feature f, int defaultEntityId)
        {
            this.Id = f.DataId;

            if (f.EntityType.Id != defaultEntityId)
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
        //public ArcData()
        //{
        //}

        internal ArcData(ArcFeature arc)
            : base(arc)
        {
            ArcGeometry geom = arc.Geometry;
            this.Clockwise = geom.IsClockwise;

            // If this is the first arc associated with the circle, write out
            // the ID of the point at the center of the circle.

            Circle c = (Circle)geom.Circle;
            ArcFeature firstArc = c.FirstArc;
            if (Object.ReferenceEquals(firstArc, arc))
                this.Center = c.CenterPoint.DataId;
            else
                this.FirstArc = firstArc.DataId;
        }

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
        public MultiSegmentData()
        {
        }

        internal MultiSegmentData(MultiSegmentLineFeature f)
            : base(f)
        {
            MultiSegmentGeometry g = (MultiSegmentGeometry)f.LineGeometry;

            // Write out array of expanded positions (there aren't that many
            // multi-segments in a cadastral database).
            PointGeometry[] data = g.GetUnpackedData();
            PointGeometryData[] points = new PointGeometryData[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                PointGeometry pg = data[i];
                PointGeometryData pt = new PointGeometryData();
                pt.X = pg.Easting.Microns;
                pt.Y = pg.Northing.Microns;
                points[i] = pt;
            }

            this.Point = points;
        }

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
        public PointData()
        {
        }

        internal PointData(PointFeature f)
            : base(f)
        {
            IPointGeometry geom = f.Geometry;

            this.X = geom.Easting.Microns;
            this.Y = geom.Northing.Microns;            
        }

        /// <summary>
        /// Loads this feature as part of an editing operation
        /// </summary>
        /// <param name="op">The editing operation creating the feature</param>
        /// <returns>The spatial feature that was loaded</returns>
        internal override Feature LoadFeature(Operation op)
        {
            //IEntity e = EnvironmentContainer.FindEntityById(this.Entity);
            //PointGeometry pg = new PointGeometry(this.X, this.Y);
            //return new DirectPointFeature(e, op, pg);

            return new DirectPointFeature(op, this);
        }

        /// <summary>
        /// Loads this point as part of an editing operation
        /// </summary>
        /// <param name="op">The editing operation creating the feature</param>
        /// <returns>The point that was loaded</returns>
        internal PointFeature LoadPoint(Operation op)
        {
            return new DirectPointFeature(op, this);
        }
    }

    /// <summary>
    /// A serialized line feature with geometry defined as a section of another line.
    /// </summary>
    public partial class SectionData
    {
        public SectionData()
        {
        }

        internal SectionData(SectionLineFeature f)
            : base(f)
        {
            this.Base = f.BaseLine.DataId;
        }

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
        public SegmentData()
        {
        }

        internal SegmentData(LineFeature line)
            : base(line)
        {
        }

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
        public SharedPointData()
        {
        }

        internal SharedPointData(SharedPointFeature f)
            : base(f)
        {
            this.FirstPoint = f.FirstPoint.DataId;
        }

        /// <summary>
        /// Loads this feature as part of an editing operation
        /// </summary>
        /// <param name="op">The editing operation creating the feature</param>
        /// <returns>The spatial feature that was loaded</returns>
        internal override Feature LoadFeature(Operation op)
        {
            return new SharedPointFeature(op, this);
        }
    }

    /// <summary>
    /// A serialized text feature.
    /// </summary>
    public partial class RowTextData
    {
        public RowTextData()
        {
        }

        internal RowTextData(RowTextFeature t)
            : base(t)
        {
            RowTextGeometry g = (RowTextGeometry)t.TextGeometry;
            this.Table = (uint)g.Row.Table.Id;
            this.Template = (uint)g.Template.Id;
        }

        /// <summary>
        /// Loads this feature as part of an editing operation
        /// </summary>
        /// <param name="op">The editing operation creating the feature</param>
        /// <returns>The spatial feature that was loaded</returns>
        internal override Feature LoadFeature(Operation op)
        {
            IEntity e = EnvironmentContainer.FindEntityById(this.Entity);
            RowTextFeature f = new RowTextFeature(e, op, null);
            f.TextGeometry = new RowTextContent(f, this);
            return f;
        }
    }

    /// <summary>
    /// A serialized text feature that represents the user-perceived key for a spatial feature.
    /// </summary>
    public partial class KeyTextData
    {
        public KeyTextData()
        {
        }

        internal KeyTextData(KeyTextFeature t)
            : base(t)
        {
            // nothing to do
        }

        /// <summary>
        /// Loads this feature as part of an editing operation
        /// </summary>
        /// <param name="op">The editing operation creating the feature</param>
        /// <returns>The spatial feature that was loaded</returns>
        internal override Feature LoadFeature(Operation op)
        {
            IEntity e = EnvironmentContainer.FindEntityById(this.Entity);
            KeyTextFeature f = new KeyTextFeature(e, op, null);
            f.TextGeometry = new KeyTextGeometry(f, this);
            return f;
        }
    }

    public partial class LineData
    {
        //public LineData()
        //{
        //}

        // should be protected
        internal LineData(LineFeature line)
            : base(line)
        {
            this.From = line.StartPoint.DataId;
            this.To = line.EndPoint.DataId;
            this.Topological = line.IsTopological;
        }
    }

    /// <summary>
    /// A serialized text feature.
    /// </summary>
    public partial class MiscTextData
    {
        public MiscTextData()
        {
        }

        internal MiscTextData(MiscTextFeature t)
            : base(t)
        {
            MiscTextGeometry mt = (MiscTextGeometry)t.TextGeometry;
            this.Text = mt.Text;
        }

        /// <summary>
        /// Loads this feature as part of an editing operation
        /// </summary>
        /// <param name="op">The editing operation creating the feature</param>
        /// <returns>The spatial feature that was loaded</returns>
        internal override Feature LoadFeature(Operation op)
        {
            IEntity e = EnvironmentContainer.FindEntityById(this.Entity);
            MiscTextFeature f = new MiscTextFeature(e, op, null);
            f.TextGeometry = new MiscTextGeometry(f, this);
            return f;
        }
    }

    /// <summary>
    /// A serialized text feature.
    /// </summary>
    public partial class TextData
    {
        //public TextData()
        //{
        //}

        // should be protected
        internal TextData(TextFeature t)
            : base(t)
        {
            this.Topological = t.IsTopological;

            IPointGeometry p = t.GetPolPosition();
            if (p != null)
            {
                this.PolygonX = p.Easting.Microns;
                this.PolygonY = p.Northing.Microns;

                this.polygonXFieldSpecified = this.PolygonYSpecified = true;
            }

            p = t.Position;
            this.X = p.Easting.Microns;
            this.Y = p.Northing.Microns;

            TextGeometry tg = t.TextGeometry;
            this.Height = Math.Round((double)tg.Height, 2);
            this.Width = Math.Round((double)tg.Width, 2);
            this.Font = (tg.Font == null ? 0 : tg.Font.Id);

            // TODO: May want to cover indirect rotations
            this.Rotation = RadianValue.AsString(tg.Rotation.Radians);
        }
    }
}
