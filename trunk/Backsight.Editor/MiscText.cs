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
using Backsight.Editor.Xml;

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="15-MAY-1998" was="CeMiscText" />
    /// <summary>
    /// A miscellaneous text object
    /// </summary>
    class MiscText : TextGeometry
    {
        #region Class data

        private string m_Text;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates new miscellaneous text
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">Information about the font for the text.</param>
        /// <param name="topLeft">The position of the top-left corner of the first character of the text.</param>
        /// <param name="height">The height of the text, in meters on the ground.</param>
        /// <param name="width">The width of the text, in meters on the ground.</param>
        /// <param name="rotation">The clockwise rotation of the text, in radians from the horizontal (default=0.0).</param>
        internal MiscText(string text, PointGeometry topLeft, IFont font, double height, double width, float rotation)
            : base(topLeft, font, height, width, rotation)
        {
            m_Text = text;
        }

        /// <summary>
        /// Constructor for use during deserialization
        /// </summary>
        /// <param name="f">The feature that makes use of this geometry</param>
        /// <param name="t">The serialized version of the feature</param>
        internal MiscText(TextFeature f, MiscTextType t)
            : base(f, t)
        {
            m_Text = t.Text;
        }

        #endregion

        /// <summary>
        /// The text for this object.
        /// </summary>
        public override string Text
        {
            get { return m_Text; }
        }

        internal void SetText(string s)
        {
            m_Text = s;
        }

        /// <summary>
        /// Changes the text for this object
        /// </summary>
        /// <param name="s">The new value for this geometry</param>
        internal void SetText(TextFeature label, string s)
        {
            CadastralMapModel map = label.MapModel;
            IEditSpatialIndex index = (IEditSpatialIndex)map.Index;
            index.Remove(label);
            m_Text = s;
            index.Add(label);
        }

        /// <summary>
        /// Makes a copy of this text.
        /// </summary>
        /// <param name="where">The position for the copy. Specify null if the copy should
        /// be at the same position as this text.</param>
        /// <returns>The copy that was created.</returns>
        /*
        TextGeometry MakeText(IPosition where)
        {
	        // If a position was not specified, get the position of this text.
            IPosition pos = (where==null ? this.Position : where);

            // Create a new key text object.
            MiscText text = new MiscText(pos, m_Text);

        	// Pick up info from this object ... the constructor did it.

	        // Pick up info from the base class.
	        DefineText(text);
	        return text;
        }
         */

        /// <summary>
        /// Extracts this text primitive into another map.
        /// </summary>
        /// <param name="xref">Info about the extract.</param>
        /// <param name="exLabel">The extract label that has already been created.</param>
        /// <returns>The text that was created.</returns>
        /*
        TextGeometry Extract(ExTranslation xref, TextFeature exLabel)
        {
            throw new NotImplementedException();
            
	        // What map are we extracting into?
	        CeMap& output = xref.GetExMap();

	        // Create a new misc-text primitive in the output map.
	        CeVertex pos(GetEasting(),GetNorthing());
	        CeMiscText* pEx = new ( os_database::of(&output)
						          , os_ts<CeMiscText>::get() )
						            CeMiscText(pos,m_pString);

	        // Define base class stuff.
	        CeText::Extract(xref,*pEx);

	        // Return the address of the text we created.
	        return pEx;          
        }
        */

        /// <summary>
        /// Returns an object that represents this text, and that can be serialized using
        /// the <c>XmlSerializer</c> class.
        /// </summary>
        /// <returns>The serializable version of this text</returns>
        internal override TextType GetSerializableText()
        {
            MiscTextType t = new MiscTextType();
            SetSerializableText(t);
            t.Text = m_Text;
            return t;
        }
    }
}
