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
using System.Diagnostics;

namespace Backsight.Editor.Xml
{
    /// <summary>
    /// Base class for any sort of serialized spatial feature.
    /// </summary>
    /// <remarks>The remainder of this class is auto-generated, and may be found
    /// in the <c>Edits.cs</c> file.</remarks>
    partial class FeatureData
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

        /// <summary>
        /// Creates an instance of <see cref="DirectPointFeature"/> using the information stored
        /// in this data instance.
        /// </summary>
        /// <param name="creator">The operation creating the feature (not null). Expected to
        /// refer to an editing session that is consistent with the session ID that is part
        /// of the feature's internal ID.</param>
        /// <param name="g">The geometry for the point (could be null, although this is only really
        /// expected during deserialization)</param>
        /// <returns></returns>
        internal DirectPointFeature CreateDirectPointFeature(Operation creator, PointGeometry g)
        {
            InternalIdValue iid = new InternalIdValue(this.Id);
            FeatureId fid = GetFeatureId(creator.MapModel);
            IEntity e = EnvironmentContainer.FindEntityById(this.Entity);
            return new DirectPointFeature(iid, fid, e, creator, g);
        }

        /// <summary>
        /// Creates an instance of <see cref="SharedPointFeature"/> using the information stored
        /// in this data instance.
        /// </summary>
        /// <param name="creator">The operation creating the feature (not null). Expected to
        /// refer to an editing session that is consistent with the session ID that is part
        /// of the feature's internal ID.</param>
        /// <param name="g">The geometry for the point (could be null, although this is only really
        /// expected during deserialization)</param>
        /// <returns></returns>
        internal SharedPointFeature CreateSharedPointFeature(Operation creator, PointFeature firstPoint)
        {
            InternalIdValue iid = new InternalIdValue(this.Id);
            FeatureId fid = GetFeatureId(creator.MapModel);
            IEntity e = EnvironmentContainer.FindEntityById(this.Entity);
            return new SharedPointFeature(iid, fid, e, creator, firstPoint);
        }

        internal ArcFeature CreateArcFeature(Operation creator, PointFeature start, PointFeature end,
                                                ArcGeometry g, bool isTopological)
        {
            IEntity e = EnvironmentContainer.FindEntityById(this.Entity);
            return CreateArcFeature(e, creator, start, end, g, isTopological);
        }

        internal ArcFeature CreateArcFeature(Operation creator, PointFeature start, PointFeature end,
                                                ArcGeometry g)
        {
            IEntity e = EnvironmentContainer.FindEntityById(this.Entity);
            return CreateArcFeature(e, creator, start, end, g, e.IsPolygonBoundaryValid);
        }

        ArcFeature CreateArcFeature(IEntity e, Operation creator, PointFeature start, PointFeature end,
                                                ArcGeometry g, bool isTopological)
        {
            InternalIdValue iid = new InternalIdValue(this.Id);
            FeatureId fid = GetFeatureId(creator.MapModel);
            return new ArcFeature(iid, fid, e, creator, start, end, g, isTopological);
        }

        internal MultiSegmentLineFeature CreateMultiSegmentLineFeature(Operation creator,
            PointFeature start, PointFeature end, MultiSegmentGeometry g, bool isTopological)
        {
            InternalIdValue iid = new InternalIdValue(this.Id);
            FeatureId fid = GetFeatureId(creator.MapModel);
            IEntity e = EnvironmentContainer.FindEntityById(this.Entity);
            return new MultiSegmentLineFeature(iid, fid, e, creator, start, end, g, isTopological);
        }

        internal SegmentLineFeature CreateSegmentLineFeature(Operation creator,
            PointFeature start, PointFeature end, bool isTopological)
        {
            IEntity e = EnvironmentContainer.FindEntityById(this.Entity);
            return CreateSegmentLineFeature(e, creator, start, end, isTopological);
        }

        internal SegmentLineFeature CreateSegmentLineFeature(Operation creator,
            PointFeature start, PointFeature end)
        {
            IEntity e = EnvironmentContainer.FindEntityById(this.Entity);
            return CreateSegmentLineFeature(e, creator, start, end, e.IsPolygonBoundaryValid);
        }

        SegmentLineFeature CreateSegmentLineFeature(IEntity e, Operation creator,
            PointFeature start, PointFeature end, bool isTopological)
        {
            InternalIdValue iid = new InternalIdValue(this.Id);
            FeatureId fid = GetFeatureId(creator.MapModel);
            return new SegmentLineFeature(iid, fid, e, creator, start, end, isTopological);
        }

        internal SectionLineFeature CreateSectionLineFeature(Operation creator,
            LineFeature baseLine, PointFeature start, PointFeature end, bool isTopological)
        {
            InternalIdValue iid = new InternalIdValue(this.Id);
            FeatureId fid = GetFeatureId(creator.MapModel);
            IEntity e = EnvironmentContainer.FindEntityById(this.Entity);
            return new SectionLineFeature(iid, fid, e, creator, baseLine, start, end, isTopological);
        }

        internal MiscTextFeature CreateMiscTextFeature(Operation creator, MiscTextGeometry geom,
            bool isTopological, PointGeometry polPosition)
        {
            InternalIdValue iid = new InternalIdValue(this.Id);
            FeatureId fid = GetFeatureId(creator.MapModel);
            IEntity e = EnvironmentContainer.FindEntityById(this.Entity);
            return new MiscTextFeature(iid, fid, e, creator, geom, isTopological, polPosition);
        }

        internal KeyTextFeature CreateKeyTextFeature(Operation creator, KeyTextGeometry geom,
            bool isTopological, PointGeometry polPosition)
        {
            InternalIdValue iid = new InternalIdValue(this.Id);
            FeatureId fid = GetFeatureId(creator.MapModel);
            IEntity e = EnvironmentContainer.FindEntityById(this.Entity);
            KeyTextFeature result = new KeyTextFeature(iid, fid, e, creator, geom, isTopological, polPosition);
            geom.Label = result;
            return result;
        }

        internal RowTextFeature CreateRowTextFeature(Operation creator, RowTextContent geom,
            bool isTopological, PointGeometry polPosition)
        {
            InternalIdValue iid = new InternalIdValue(this.Id);
            FeatureId fid = GetFeatureId(creator.MapModel);
            IEntity e = EnvironmentContainer.FindEntityById(this.Entity);
            RowTextFeature result = new RowTextFeature(iid, fid, e, creator, geom, isTopological, polPosition);
            return result;
        }

        /// <summary>
        /// Deserializes the user-perceived ID of a feature
        /// </summary>
        /// <param name="mapModel">The model containing this feature</param>
        /// <param name="t">The serialized version of this feature</param>
        /// <returns>The corresponding ID (null if this feature does not have
        /// a user-perceived ID).</returns>
        FeatureId GetFeatureId(CadastralMapModel mapModel)
        {
            uint nativeKey = this.Key;
            if (nativeKey > 0)
            {
                NativeId nid = mapModel.FindNativeId(nativeKey);
                if (nid == null)
                    return mapModel.AddNativeId(nativeKey);
                else
                    return nid;
            }

            string key = this.ForeignKey;
            if (key != null)
            {
                ForeignId fid = mapModel.FindForeignId(key);
                if (fid == null)
                    return mapModel.AddForeignId(key);
            }

            return null;
        }
    }

    partial class FeatureStubData
    {
        public FeatureStubData()
        {
        }

        internal FeatureStubData(FeatureGeometry g)
        {
            this.Geometry = (uint)g;
        }

        internal FeatureStubData(Feature f)
            : this(f, true)
        {
        }

        internal FeatureStubData(Feature f, bool allFields)
            : base(f, allFields)
        {
            this.Geometry = (uint)f.Representation;
        }

        internal FeatureStubData(Feature f, int defaultEntityId)
            : base(f, defaultEntityId)
        {
            this.Geometry = (uint)f.Representation;
        }
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
            return CreateArcFeature(op);
        }

        internal ArcFeature CreateArcFeature(Operation op)
        {
            if (this.Center==null)
            {
                ArcFeature firstArc = op.MapModel.Find<ArcFeature>(this.FirstArc);
                Debug.Assert(firstArc != null);
                return base.CreateArcFeature(op, this.Clockwise, firstArc.Circle);
            }
            else
            {
                PointFeature center = op.MapModel.Find<PointFeature>(this.Center);
                return base.CreateArcFeature(op, this.Clockwise, center);
            }
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
            return CreateMultiSegmentLineFeature(op);
        }

        internal MultiSegmentLineFeature CreateMultiSegmentLineFeature(Operation op)
        {
            PointGeometryData[] pts = this.Point;
            IPointGeometry[] pgs = new IPointGeometry[pts.Length];
            for (int i=0; i<pts.Length; i++)
            {
                PointGeometryData pt = pts[i];
                pgs[i] = new PointGeometry(pt.X, pt.Y);
            }

            return base.CreateMultiSegmentLineFeature(op, pgs);
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
            return CreateDirectPointFeature(op);
        }

        /// <summary>
        /// Defines a new spatial feature based on this data instance.
        /// </summary>
        /// <param name="op">The editing operation the feature is part of</param>
        /// <returns>The created feature</returns>
        internal DirectPointFeature CreateDirectPointFeature(Operation op)
        {
            PointGeometry g = new PointGeometry(this.X, this.Y);
            return base.CreateDirectPointFeature(op, g);
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
            return CreateSectionLineFeature(op);
        }

        internal SectionLineFeature CreateSectionLineFeature(Operation op)
        {
            LineFeature baseLine = op.MapModel.Find<LineFeature>(this.Base);
            return base.CreateSectionLineFeature(op, baseLine);
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

        internal SegmentData(SegmentLineFeature line)
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
            return CreateSegmentLineFeature(op);
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
            return CreateSharedPointFeature(op);
        }

        /// <summary>
        /// Defines a new spatial feature based on this data instance.
        /// </summary>
        /// <param name="op">The editing operation the feature is part of</param>
        /// <returns>The created feature</returns>
        internal SharedPointFeature CreateSharedPointFeature(Operation op)
        {
            PointFeature firstPoint = op.MapModel.Find<PointFeature>(this.FirstPoint);
            return base.CreateSharedPointFeature(op, firstPoint);
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
            return CreateRowTextFeature(op);
        }

        internal RowTextFeature CreateRowTextFeature(Operation op)
        {
            ITemplate t = EnvironmentContainer.FindTemplateById((int)this.Template);
            return base.CreateRowTextFeature(op, (int)this.Table, t);
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
            return CreateKeyTextFeature(op);
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

        /// <summary>
        /// Creates an <see cref="ArcFeature"/> that coincides with a previously created circle.
        /// </summary>
        /// <param name="op">The editing operation creating the feature</param>
        /// <param name="isClockwise">Is the arc directed clockwise around the circle?</param>
        /// <param name="circle">The circle that the arc coincides with</param>
        /// <returns>The created arc</returns>
        internal ArcFeature CreateArcFeature(Operation op, bool isClockwise, Circle circle)
        {
            PointFeature from = op.MapModel.Find<PointFeature>(this.From);
            PointFeature to = op.MapModel.Find<PointFeature>(this.To);
            ArcGeometry geom = new ArcGeometry(circle, from, to, isClockwise);
            return base.CreateArcFeature(op, from, to, geom, this.Topological);
        }

        /// <summary>
        /// Creates an <see cref="ArcFeature"/> on a new circle.
        /// </summary>
        /// <param name="op">The editing operation creating the feature</param>
        /// <param name="isClockwise">Is the arc directed clockwise around the circle?</param>
        /// <param name="center">The point at the center of the circle</param>
        /// <returns>The created arc</returns>
        internal ArcFeature CreateArcFeature(Operation op, bool isClockwise, PointFeature center)
        {
            // The arc is the first arc attached to the circle. However, we may be
            // unable to calculate the radius (whereas the geometry will be available
            // if the data comes from an import, it will be undefined if the geometry
            // is calculated).

            PointFeature from = op.MapModel.Find<PointFeature>(this.From);
            PointFeature to = op.MapModel.Find<PointFeature>(this.To);

            double radius = 0.0;
            if (center.PointGeometry != null && from.PointGeometry != null)
                radius = Geom.Distance(center.PointGeometry, from.PointGeometry);

            Circle c = new Circle(center, radius);
            center.AddReference(c);
            ArcGeometry geom = new ArcGeometry(c, from, to, isClockwise);
            return base.CreateArcFeature(op, from, to, geom, this.Topological);
        }

        internal MultiSegmentLineFeature CreateMultiSegmentLineFeature(Operation op, IPointGeometry[] pgs)
        {
            PointFeature from = op.MapModel.Find<PointFeature>(this.From);
            PointFeature to = op.MapModel.Find<PointFeature>(this.To);
            MultiSegmentGeometry geom = new MultiSegmentGeometry(from, to, pgs);
            return base.CreateMultiSegmentLineFeature(op, from, to, geom, this.Topological);
        }

        internal SegmentLineFeature CreateSegmentLineFeature(Operation op)
        {
            PointFeature from = op.MapModel.Find<PointFeature>(this.From);
            PointFeature to = op.MapModel.Find<PointFeature>(this.To);
            return base.CreateSegmentLineFeature(op, from, to, this.Topological);
        }

        internal SectionLineFeature CreateSectionLineFeature(Operation op, LineFeature baseLine)
        {
            PointFeature from = op.MapModel.Find<PointFeature>(this.From);
            PointFeature to = op.MapModel.Find<PointFeature>(this.To);
            return base.CreateSectionLineFeature(op, baseLine, from, to, this.Topological);
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
            return CreateMiscTextFeature(op);
        }

        internal MiscTextFeature CreateMiscTextFeature(Operation op)
        {
            return base.CreateMiscTextFeature(op, this.Text);
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

            IPointGeometry tp = t.Position;
            this.X = tp.Easting.Microns;
            this.Y = tp.Northing.Microns;

            IPointGeometry pp = t.GetPolPosition();
            if (pp != null)
            {
                if (pp.Easting.Microns != tp.Easting.Microns || pp.Northing.Microns != tp.Northing.Microns)
                {
                    this.PolygonX = pp.Easting.Microns;
                    this.PolygonY = pp.Northing.Microns;

                    this.polygonXFieldSpecified = this.PolygonYSpecified = true;
                }
            }

            TextGeometry tg = t.TextGeometry;
            this.Height = Math.Round((double)tg.Height, 2);
            this.Width = Math.Round((double)tg.Width, 2);
            this.Font = (tg.Font == null ? 0 : tg.Font.Id);

            // TODO: May want to cover indirect rotations
            this.Rotation = RadianValue.AsString(tg.Rotation.Radians);
        }

        internal MiscTextFeature CreateMiscTextFeature(Operation op, string text)
        {
            IFont font = EnvironmentContainer.FindFontById(this.Font);
            PointGeometry topLeft = new PointGeometry(this.X, this.Y);
            double rot = RadianValue.Parse(this.Rotation);
            MiscTextGeometry geom = new MiscTextGeometry(text, topLeft, font,
                                            this.Height, this.Width, (float)rot);

            PointGeometry polPosition = null;
            if (this.PolygonXSpecified && this.PolygonYSpecified)
                polPosition = new PointGeometry(this.PolygonX, this.PolygonY);

            return base.CreateMiscTextFeature(op, geom, this.Topological, polPosition);
        }

        internal KeyTextFeature CreateKeyTextFeature(Operation op)
        {
            IFont font = EnvironmentContainer.FindFontById(this.Font);
            PointGeometry topLeft = new PointGeometry(this.X, this.Y);
            double rot = RadianValue.Parse(this.Rotation);
            KeyTextGeometry geom = new KeyTextGeometry(topLeft, font, this.Height, this.Width, (float)rot);

            PointGeometry polPosition = null;
            if (this.PolygonXSpecified && this.PolygonYSpecified)
                polPosition = new PointGeometry(this.PolygonX, this.PolygonY);

            return base.CreateKeyTextFeature(op, geom, this.Topological, polPosition);
        }

        internal RowTextFeature CreateRowTextFeature(Operation op, int tableId, ITemplate template)
        {
            IFont font = EnvironmentContainer.FindFontById(this.Font);
            PointGeometry topLeft = new PointGeometry(this.X, this.Y);
            double rot = RadianValue.Parse(this.Rotation);
            RowTextContent geom = new RowTextContent(tableId, template, topLeft, font,
                                        this.Height, this.Width, (float)rot);

            PointGeometry polPosition = null;
            if (this.PolygonXSpecified && this.PolygonYSpecified)
                polPosition = new PointGeometry(this.PolygonX, this.PolygonY);

            return base.CreateRowTextFeature(op, geom, this.Topological, polPosition);
        }
    }
}
