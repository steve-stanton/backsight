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

using Backsight.Environment;

namespace Backsight.Editor;

/// <written by="Steve Stanton" on="15-MAY-1998" was="CeMiscText" />
/// <summary>
/// A miscellaneous text object
/// </summary>
class MiscTextGeometry : TextGeometry
{
    private string m_Text;

    /// <summary>
    /// Creates new miscellaneous text
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="font">Information about the font for the text.</param>
    /// <param name="topLeft">The position of the top-left corner of the first character of the text.</param>
    /// <param name="height">The height of the text, in meters on the ground.</param>
    /// <param name="width">The width of the text, in meters on the ground.</param>
    /// <param name="rotation">The clockwise rotation of the text, in radians from the horizontal (default=0.0).</param>
    internal MiscTextGeometry(string text, PointGeometry topLeft, IFont font, double height, double width, float rotation)
        : base(topLeft, font, height, width, rotation)
    {
        m_Text = text;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiscTextGeometry"/> class
    /// using the data read from persistent storage.
    /// </summary>
    /// <param name="editDeserializer">The mechanism for reading back content.</param>
    internal MiscTextGeometry(EditDeserializer editDeserializer)
        : base(editDeserializer)
    {
        m_Text = editDeserializer.ReadString(DataField.Text);
    }

    /// <summary>
    /// The text for this object.
    /// </summary>
    public override string Text => m_Text;

    /// <summary>
    /// Changes the text for this object
    /// </summary>
    /// <param name="s">The new value for this geometry</param>
    internal void SetText(TextFeature label, string s)
    {
        CadastralMapModel map = label.MapModel;
        EditingIndex index = map.EditingIndex;
        index.RemoveFeature(label);
        m_Text = s;
        index.AddFeature(label);
    }

    /// <summary>
    /// Writes the content of this instance to a persistent storage area.
    /// </summary>
    /// <param name="editSerializer">The mechanism for storing content.</param>
    public override void WriteData(EditSerializer editSerializer)
    {
        base.WriteData(editSerializer);
        editSerializer.WriteString(DataField.Text, m_Text);
    }
}