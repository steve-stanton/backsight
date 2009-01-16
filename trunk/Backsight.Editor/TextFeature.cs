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

using Backsight.Environment;
using Backsight.Geometry;
using Backsight.Editor.Operations;

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
        /// The polygon that encloses this text feature. Null if this feature isn't a polygon
        /// label, or the enclosing polygon has yet to be determined (in the latter case,
        /// the <c>IsBuilt</c> property will return false).
        /// </summary>
        /// <remarks>
        /// A polygon can only refer back to one label. If more than one label gets found,
        /// the polygon will continue to refer to the initially found label.
        /// </remarks>
        Polygon m_Container;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new text feature
        /// </summary>
        /// <param name="text">The text geometry (including the text string itself)</param>
        /// <param name="ent">The entity type for the string.</param>
        /// <param name="creator">The operation creating the text</param>
        /// </param>
        public TextFeature(TextGeometry text, IEntity ent, Operation creator) : base(ent, creator)
        {
            m_Geom = text;
            m_Container = null;
        }

        /// <summary>
        /// Default constructor (for serialization)
        /// </summary>
        public TextFeature()
        {
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

            if (s_DrawReferencePoints)
                style.RenderPlus(display, m_Geom.Position);
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

            // If there is no alternate position, just use the regular position of the text
            if (!HasDependents)
                return m_Geom.Position;

            foreach (IFeatureDependent d in Dependents)
            {
                if (d is MoveTextOperation)
                {
                    MoveTextOperation mto = (d as MoveTextOperation);
                    if (mto.MovedText == this)
                        return mto.OldPosition;
                }
            }

            return m_Geom.Position;
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
            // out the reference (the polygon may not point back).
            if (m_Container!=null && m_Container.IsDeleted)
                OnPolygonDelete();

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
        /// Writes the content of this class. This is called by
        /// <see cref="XmlContentWriter.WriteElement"/>
        /// after the element name and class type (xsi:type) have been written.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteContent(XmlContentWriter writer)
        {
            base.WriteContent(writer);

            if (!IsTopological)
                writer.WriteString("Flags", "N");

            writer.WriteElement("Geometry", m_Geom);
        }

        /// <summary>
        /// Loads the content of this class. This is called by
        /// <see cref="XmlContentReader"/> during deserialization from XML (just
        /// after the default constructor has been invoked).
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public override void ReadContent(XmlContentReader reader)
        {
            base.ReadContent(reader);

            string flags = reader.ReadString("Flags");
            if (flags == null)
                flags = String.Empty;

            if (flags.Contains("N"))
                SetTopology(false);
            else
                SetTopology(true);

            m_Geom = reader.ReadElement<TextGeometry>("Geometry");

            // KeyText refers back to this feature (which provides the key)
            if (m_Geom is KeyTextGeometry)
                (m_Geom as KeyTextGeometry).Label = this;
        }
    }
}
