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

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="16-MAY-1998" was="CeKeyText"/>
    /// <summary>
    /// A text object that represents the key of a feature.
    /// </summary>
    class KeyTextGeometry : TextGeometry
    {
        #region Class data

        /// The label that makes use of this geometry. This should be defined as soon as the
        /// label has been created (which should happen immediately after instantiation of
        /// this geometry).
        TextFeature m_Feature;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>KeyTextGeometry</c> that isn't associated with a text feature. There is a chicken and egg
        /// problem here - an instance of KeyTextGeometry is expected to refer to a TextFeature, but the feature cannot
        /// be created until the geometry has been created. So after creating the KeyTextGeometry, you are expected to
        /// create the corresponding feature, then assign the feature to this geometry using the <see cref="Label"/>
        /// property.
        /// </summary>
        /// <param name="pos">Position of the text's reference point (always the top left corner of the string).</param>
        /// <param name="font">The text style (defines the type-face and the height of the text).</param>
        /// <param name="height">The height of the text, in meters on the ground.</param>
        /// <param name="width">The total width of the text, in meters on the ground.</param>
        /// <param name="rotation">Clockwise rotation from horizontal</param>
        internal KeyTextGeometry(PointGeometry pos, IFont font, double height, double width, float rotation)
            : base(pos, font, height, width, rotation)
        {
            m_Feature = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyTextGeometry"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal KeyTextGeometry(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            // Nothing to do - the TextFeature constructor must define the Label property after
            // this geometry object has been created.
        }

        #endregion

        /// <summary>
        /// The label that makes use of this geometry. This should be defined as soon as the
        /// label has been created (which should happen immediately after instantiation of
        /// this geometry).
        /// </summary>
        internal TextFeature Label
        {
            get { return m_Feature; }
            set { m_Feature = value; }
        }

        public override string Text
        {
            get
            {
                FeatureId id = (m_Feature==null ? null : m_Feature.FeatureId);
                return (id==null ? "?" : id.FormattedKey);
            }
        }
    }
}
