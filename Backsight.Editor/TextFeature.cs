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
    abstract class TextFeature : Feature
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
        /// <param name="ent">The entity type for the string.</param>
        /// <param name="creator">The operation creating the text</param>
        /// <param name="text">The text geometry (including the text string itself)</param>
        /// </param>
        protected TextFeature(IEntity ent, Operation creator, TextGeometry text)
            : base(ent, creator)
        {
            m_Geom = text;
            m_Container = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextFeature"/> class, and records it
        /// as part of the map model.
        /// </summary>
        /// <param name="iid">The internal ID for the feature.</param>
        /// <param name="fid">The (optional) user-perceived ID for the feature. If not null,
        /// this will be modified by cross-referencing it to the newly created feature.</param>
        /// <param name="ent">The entity type for the feature (not null)</param>
        /// <param name="creator">The operation creating the feature (not null). Expected to
        /// refer to an editing session that is consistent with the session ID that is part
        /// of the feature's internal ID.</param>
        /// <param name="geom">The metrics for the text (including the text itself).</param>
        /// <param name="isTopological">Is the new feature expected to act as a polygon label?</param>
        /// <param name="polPosition">The position of the polygon reference position (specify null
        /// if the feature is not a polygon label)</param>
        /// <exception cref="ArgumentNullException">If either <paramref name="ent"/> or
        /// <paramref name="creator"/> is null.</exception>
        protected TextFeature(InternalIdValue iid, FeatureId fid, IEntity ent, Operation creator,
            TextGeometry geom, bool isTopological, PointGeometry polPosition)
            : base(iid, fid, ent, creator)
        {
            m_Geom = geom;
            SetTopology(isTopological);
            m_PolygonPosition = polPosition;
        }

        #endregion

        /*
        public static TextFeature New(ITextGeometry g, IEntity ent)
        {
            //if (ent==null)
            //    throw new ArgumentNullException("Entity type not specified");

            // Create the text primitive.         
            MiscText miscText = new MiscText(topLeft, text, height, spacing, rotation);

            // Define the text metrics.
            miscText.Spacing = spacing;
            miscText.Rotation = rotation;
            miscText.Height = height;

            // If an entity type has not been given, get the default entity type for text.
	        IEntity newEnt = ent;
	        if (newEnt==null) newEnt = CeMap::GetpEntity(ANNOTATION);
	        if (newEnt==null)
                throw new Exception("CeMap::AddMiscLabel\nUnspecified entity type.");

            // Do standard stuff for adding a label.
            TextFeature label = new TextFeature(g, ent);
            return label;

            // Cross-reference the text to the label.
	        //miscText.AddObject(label);

            // If a height has been explicitly given, use that. If
            // no height, but we have a default font, use the height
            // of that font. Otherwise fall back on the height of
            // line annotations.

            ISimpleFont font = ent.DefaultFont;

            if (height < Constants.TINY)
            {
                // If we can, use default font (and height). Otherwise
                // the text gets the height of line annotation, but
                // does NOT get a font.

                if (font != null) // entity has font assigned, use that
                {
                    miscText.Height = font.Height;
                    miscText.DefaultFont = font;
                }              
                else if (CeMap::m_pFont)
                {
                    text.SetHeight(m_pFont->GetHeight());
                    text.SetFont(*m_pFont);
                }
                else
                    text.Height = (float)(CeMap::m_LineAnnoHeight);
            }
            else
            {
                // If we have a default font, initialize with that.
                if (font != null)
                    miscText.Font = font;
                else if (m_pFont)
                    text.SetFont(*m_pFont);

                // Use the height supplied. If this height is not
                // consistent with the font (if any), this may cause
                // a new font to be added to the map.
                miscText.Height = height;
            }

            // And add to the spatial index.
            // no... during imports, we do this AFTER
            if (label!=null)
                m_Space.Add(label);

            return label;
        }
         */
    
        public override SpatialType SpatialType
        {
            get { return SpatialType.Text; }
        }

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

        public override IWindow Extent
        {
            get { return m_Geom.Extent; }
        }

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
            OnPreMove(this);
            m_Geom.Position = to;
            m_PolygonPosition = null;
            OnPostMove(this);

            return true;
        }

    }
}
