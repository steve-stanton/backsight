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

namespace Ntx
{
    public class Name : Feature
    {
        #region Class data

        /// <summary>
        /// Reference position
        /// </summary>
        Position m_RefPosition;

        /// <summary>
        /// The name (max in ntx is 87)
        /// </summary>
	    string m_Name;

        /// <summary>
        /// Is this a polygon label
        /// </summary>
	    bool m_IsLabel;

        /// <summary>
        /// Positions of each character (one for each character in the name).
        /// </summary>
	    Position[] m_Positions;

        /// <summary>
        /// Height of the text, in meters on the ground.
        /// </summary>
	    float m_Height;

        /// <summary>
        /// Rotation of the FIRST character in the name, measured in minutes
        /// from the horizontal.
        /// </summary>
	    int m_Rotation;

        #endregion

        #region Constructors

        internal Name()
        {
            m_Name = String.Empty;
            m_IsLabel = false;
            m_Positions = null;
            m_Height = 0.0F;
            m_Rotation = 0;
        }

        #endregion

        internal Position RefPosition
        {
            get { return m_RefPosition; }
            set { m_RefPosition = value; }
        }

        public string Text
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public bool IsLabel
        {
            get { return m_IsLabel; }
            set { m_IsLabel = value; }
        }

        public float Height
        {
            get { return m_Height; }
            set { m_Height = value; }
        }

        internal int Count
        {
            get { return m_Name.Length; }
        }

        public Position Position(int index)
        {
            return m_Positions[index];
        }

        /// <summary>
        /// The average spacing between each character in this name (in meters on the ground).
        /// For names that consist of only 1 character, the spacing is always zero.
        /// </summary>
        public float Spacing
        {
            get
            {
                // If this name consists of only 1 character, return no spacing.
                int nc = this.Count;
                if (nc<=1)
                    return 0.0F;

                // Sum the distance between each character.
	            double totlen = 0.0;
	            double xs = m_Positions[0].Easting;
	            double ys = m_Positions[0].Northing;
	            double xe;
	            double ye;

	            for (int i=1; i<nc; i++, xs=xe, ys=ye)
                {
		            xe = m_Positions[i].Easting;
		            ye = m_Positions[i].Northing;
		            double dx = xe-xs;
		            double dy = ye-ys;
		            double len = Math.Sqrt(dx*dx + dy*dy);
		            if ( len < Double.Epsilon )
                        len = 0.0;
		            totlen += len;
	            }

                // Return the average distance between each character.
	            double spacing = totlen/(double)(nc-1);
	            return (float)spacing;
            }
        }

        /// <summary>
        /// The clockwise rotation (if any) of this name, measured in radians from the horizontal.
        /// This is the rotation of the overall name, not the rotation of individual characters.
        /// </summary>
        public float Rotation
        {
            get
            {
                // Get the number of characters in the name. If only
                // one character, return the first rotation.
	            int nc = this.Count;
	            if (nc<=1)
                    return this.FirstRotation;

                // Get the positions of the first and last characters.
	            double xs = m_Positions[0].Easting;
	            double ys = m_Positions[0].Northing;
	            double xe = m_Positions[nc-1].Easting;
	            double ye = m_Positions[nc-1].Northing;

                // Figure out the rotation based on these positions.
	            double dx = xe-xs;
	            double dy = ye-ys;

                // Check for horizontal text.
	            if (Math.Abs(dy) < Double.Epsilon)
                    return (float)(dx>0.0 ? 0.0 : (float)Math.PI);


                // Check for vertical text.
	            if (Math.Abs(dx) < Double.Epsilon)
                    return (dy>0.0 ? (float)(Math.PI * 1.5) : (float)(Math.PI / 2.0));

                //	Figure out the rotation.

	            double rot;

	            if (dx>0.0)
                    rot = (dy>0.0 ? Math.PI * 1.5 + Math.Atan(dx/dy) :
                                    Math.Atan(Math.Abs(dy)/dx));
	            else
                    rot = (dy>0.0 ? Math.PI + Math.Atan(dy/Math.Abs(dx)) :
                                    Math.PI * 0.5 + Math.Atan(Math.Abs(dx)/Math.Abs(dy)));

	            return (float)rot;
            }
        }

        /// <summary>
        /// The clockwise rotation of the first character, measured in radians from the horizontal.
        /// </summary>
        float FirstRotation
        {
            get
            {
                // Convert rotation from minutes of arc to radians.
                const double DEGTORAD = (Math.PI * 2.0)/360.0;
	            return (float)(DEGTORAD * m_Rotation/60.0F);
            }
        }

        internal Position[] Positions
        {
            set { m_Positions = value; }
        }
    }
}
