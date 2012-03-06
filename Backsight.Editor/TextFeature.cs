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
using System.Drawing;
using System.Diagnostics;

using Backsight.Environment;
using Backsight.Geometry;
using Backsight.Editor.Operations;
using Backsight.Forms;

namespace Backsight.Editor
{
    class TextFeature : Feature
    {
        #region Static

        /// <summary>
        /// Should polygon label reference points be rendered during draws?
        /// </summary>
        static bool s_DrawReferencePoints = false;

        /// <summary>
        /// Should polygon label reference points be rendered during draws? This is
        /// currently turned on while the File-Check command is running, since it helps
        /// the user to appreciate a situation where a polygon label falls slightly outside
        /// a thin polygon.
        /// </summary>
        internal static bool AreReferencePointsDrawn
        {
            get { return s_DrawReferencePoints; }
            set { s_DrawReferencePoints = value; }
        }

        #endregion

        #region Class data

        /// <summary>
        /// The annotation & style (font)
        /// </summary>
        TextGeometry m_Geom; // readonly

        /// <summary>
        /// The reference position that should be used when attempting to locate
        /// an enclosing polygon. Null if the top-left corner of the text should
        /// be used. Applies only to text features that are flagged as topological.
        /// </summary>
        PointGeometry m_PolygonPosition;

        /// <summary>
        /// The polygon that encloses this text feature. Null if this feature isn't a polygon
        /// label, or the enclosing polygon has yet to be determined (in the latter case,
        /// the <c>IsBuilt</c> property will return false).
        /// </summary>
        Polygon m_Container;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new text feature
        /// </summary>
        /// <param name="creator">The operation creating the text</param>
        /// <param name="id">The internal ID of this feature within the
        /// project that created it.</param>
        /// <param name="ent">The entity type for the string.</param>
        /// <param name="text">The text geometry (including the text string itself)</param>
        /// </param>
        internal TextFeature(Operation creator, InternalIdValue id, IEntity ent, TextGeometry text)
            : base(creator, id, ent, null)
        {
            m_Geom = text;
            m_Container = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextFeature"/> class, and records it
        /// as part of the map model.
        /// </summary>
        /// <param name="f">Basic information about the feature (not null).</param>
        /// <param name="geom">The metrics for the text (including the text itself).</param>
        /// <param name="isTopological">Is the new feature expected to act as a polygon label?</param>
        /// <param name="polPosition">The position of the polygon reference position (specify null
        /// if the feature is not a polygon label)</param>
        /// <exception cref="ArgumentNullException">If <paramref name="f"/> is null.</exception>
        protected TextFeature(IFeature f, TextGeometry geom, bool isTopological, PointGeometry polPosition)
            : base(f)
        {
            m_Geom = geom;
            SetTopology(isTopological);
            m_PolygonPosition = polPosition;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextFeature"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal TextFeature(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            bool isTopological;
            ReadData(editDeserializer, out isTopological, out m_PolygonPosition, out m_Geom);
            SetTopology(isTopological);

            if (m_Geom is KeyTextGeometry)
                (m_Geom as KeyTextGeometry).Label = this;
        }

        #endregion

        public override SpatialType SpatialType
        {
            get { return SpatialType.Text; }
        }

        /// <summary>
        /// Draws this object on the specified display
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The drawing style</param>
        public override void Render(ISpatialDisplay display, IDrawStyle style)
        {
            m_Geom.Render(display, style);

            if (s_DrawReferencePoints || style is HighlightStyle)
            {
                IPointGeometry p = GetPolPosition();
                if (p!=null)
                {
                    Color c = style.LineColor;
                    style.LineColor = Color.Gray;
                    style.RenderPlus(display, p);
                    style.LineColor = c;
                }
            }
        }

        /// <summary>
        /// The covering rectangle that encloses this feature.
        /// </summary>
        public override IWindow Extent
        {
            get { return m_Geom.Extent; }
        }

        /// <summary>
        /// The shortest distance between this object and the specified position.
        /// </summary>
        /// <param name="point">The position of interest</param>
        /// <returns>The shortest distance between the specified position and this object</returns>
        public override ILength Distance(IPosition point)
        {
            return m_Geom.Distance(point);
        }

        /// <summary>
        /// The "geometry" for this text.
        /// </summary>
        internal TextGeometry TextGeometry
        {
            get { return m_Geom; }
            set { m_Geom = value; }
        }

        /// <summary>
        /// Ensure that this label knows it's enclosing polygon.
        /// </summary>
        /// <returns>True if label successfully associated with polygon.</returns>
        internal void SetPolygon()
        {
            // Return if this label is already associated with a polygon (or the
            // label was previously found to be inside nothing)
            if (IsBuilt)
                return;

            // If this is a non-topological label, mark is as "built"
            // and return (it should have been previously marked as built).
            if (IsTopological)
            {
                // Get the label's reference position.
                IPointGeometry posn = GetPolPosition();

                // Try to find enclosing polygon
                ISpatialIndex index = CadastralMapModel.Current.Index;
                Polygon enc = new FindPointContainerQuery(index, posn).Result;
                if (enc!=null)
                    enc.ClaimLabel(this);
            }

            SetBuilt(true);
        }

        /// <summary>
        /// Returns the reference position for this label.
        /// </summary>
        /// <returns>The reference position (null if this feature isn't a polygon label)</returns>
        internal IPointGeometry GetPolPosition()
        {
            // Return if this is not a topological label.
            if (!IsTopological)
                return null;

            if (m_PolygonPosition != null)
                return m_PolygonPosition;
            else
                return m_Geom.Position;
        }

        /// <summary>
        /// Remembers a new polygon reference position for this label.
        /// </summary>
        /// <param name="p">The reference position. Specify null if the default reference
        /// position should be used (the position associated with the text geometry).</param>
        internal void SetPolPosition(IPointGeometry p)
        {
            if (p == null)
                m_PolygonPosition = null;
            else
                m_PolygonPosition = PointGeometry.Create(p);

            // If the label was previously "built", re-calculate the relationship to any
            // enclosing polygon
            RecalculateEnclosingPolygon();
        }

        /// <summary>
        /// Re-calculates the polygon that encloses this label. This should be called whenever
        /// the polygon reference position gets changed (if a polygon reference position is
        /// not explicit, this will arise when the text itself is moved). Does nothing if the
        /// label is not currently marked as "built".
        /// </summary>
        internal void RecalculateEnclosingPolygon()
        {
            if (!IsBuilt)
                return;

            if (!IsTopological)
                return;

            // If a container was previously defined, first check whether it's still the
            // enclosing polygon (if so, we're done).
            IPointGeometry p = GetPolPosition();
            if (m_Container!=null && m_Container.IsEnclosing(p))
                return;

            // If a container was previously defined, break the association with this label
            if (m_Container!=null)
            {
                m_Container.ReleaseLabel(this);
                Debug.Assert(m_Container==null);
            }

            // Figure out which polygon now encloses this label (if any)
            ISpatialIndex index = CadastralMapModel.Current.Index;
            Polygon enc = new FindPointContainerQuery(index, p).Result;
            if (enc!=null)
                enc.ClaimLabel(this);
        }

        /// <summary>
        /// The polygon that encloses this text feature. Null if this feature isn't a polygon
        /// label, or the enclosing polygon has yet to be determined (in the latter case,
        /// the <c>IsBuilt</c> property will return false).
        /// </summary>
        internal Polygon Container
        {
            get { return m_Container; }
            set { m_Container = value; }
        }

        /// <summary>
        /// Releases the association of this label with a polygon that is being removed.
        /// </summary>
        internal void OnPolygonDelete()
        {
            m_Container = null;
            SetBuilt(false);
        }

        public string ContainerId
        {
            get { return (m_Container==null ? String.Empty : m_Container.ToString()); }
        }

        /// <summary>
        /// Ensures this feature is clean after some sort of edit. If this is a polygon label, but
        /// the polygon has been marked for deletion, this will clear the reference to the polygon,
        /// then calls <see cref="Feature.Clean"/>
        /// </summary>
        internal override void Clean()
        {
            // If this label refers to a polygon that has been marked for deletion, null
            // out the reference (the polygon may not point back). If the label is now inactive,
            // but the polygon isn't, clear the polygon->label association.
            if (m_Container != null)
            {
                if (m_Container.IsDeleted)
                    OnPolygonDelete();
                else if (this.IsInactive)
                    m_Container.ReleaseLabel(this);
            }

            base.Clean();
        }

        /// <summary>
        /// The position of the top left corner of the text
        /// </summary>
        internal IPointGeometry Position
        {
            get { return m_Geom.Position; }
        }

        /// <summary>
        /// Moves this text to a new position
        /// </summary>
        /// <param name="to">The new position for the text</param>
        /// <returns>True if the text was moved. False if the specified position is
        /// coincident with the current position.</returns>
        internal bool Move(PointGeometry to)
        {
            // Just return if the new location is at the same position
            // as the old location.
            if (m_Geom.Position.IsCoincident(to))
                return false;

            // Remove this feature from spatial index, move the text (and null the
            // polygon reference position), add back into the spatial index
            RemoveIndex();
            m_Geom.Position = to;
            m_PolygonPosition = null;
            AddToIndex();

            return true;
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public override void WriteData(EditSerializer editSerializer)
        {
            base.WriteData(editSerializer);

            editSerializer.WriteBool(DataField.Topological, IsTopological);

            IPointGeometry tp = Position;
            IPointGeometry pp = GetPolPosition();
            if (pp != null)
            {
                if (pp.Easting.Microns != tp.Easting.Microns || pp.Northing.Microns != tp.Northing.Microns)
                {
                    editSerializer.WriteInt64(DataField.PolygonX, pp.Easting.Microns);
                    editSerializer.WriteInt64(DataField.PolygonY, pp.Northing.Microns);
                }
            }

            // RowText is problematic on deserialization because the database rows might not
            // be there. To cover that possibility, use a proxy object.
            if (m_Geom is RowTextGeometry)
                editSerializer.WritePersistent<TextGeometry>(DataField.Type, new RowTextContent((RowTextGeometry)m_Geom));
            else
                editSerializer.WritePersistent<TextGeometry>(DataField.Type, m_Geom);
        }

        /// <summary>
        /// Reads data that was previously written using <see cref="WriteData"/>
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        /// <param name="isTopological">Are we dealing with a polygon label</param>
        /// <param name="polPos">The label reference point (usually applies onlt to polygon labels). Null if it's
        /// identical to the position recorded via the geometry object.</param>
        /// <param name="geom">The geometry for the text.</param>
        static void ReadData(EditDeserializer editDeserializer, out bool isTopological, out PointGeometry polPos, out TextGeometry geom)
        {
            isTopological = editDeserializer.ReadBool(DataField.Topological);

            if (editDeserializer.IsNextField(DataField.PolygonX))
                polPos = editDeserializer.ReadPointGeometry(DataField.PolygonX, DataField.PolygonY);
            else
                polPos = null;

            geom = editDeserializer.ReadPersistent<TextGeometry>(DataField.Type);
        }
    }
}
