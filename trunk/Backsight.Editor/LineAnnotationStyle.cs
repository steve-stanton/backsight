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

namespace Backsight.Editor
{
    [Serializable]
    class LineAnnotationStyle
    {
        #region Class data

        /// <summary>
        /// Scale denominator at which line annotation will be drawn.
        /// </summary>
        double m_Scale;

        /// <summary>
        /// Height of line annotation text, in meters on the ground.
        /// </summary>
        double m_Height;

        /// <summary>
        /// Flags indicating what item(s) appear in line annotations.
        /// </summary>
        LineAnnotationOptions m_Options;

        #endregion

        #region Constructors

        internal LineAnnotationStyle()
        {
            m_Scale = 2000.0;
            m_Height = 5.0;
            m_Options = 0;
        }

        #endregion

        public double ShowScale
        {
            get { return m_Scale; }
            set { m_Scale = value; }
        }

        public double Height
        {
            get { return m_Height; }
            set { m_Height = value; }
        }

        public bool ShowAdjustedLengths
        {
            get { return (m_Options & LineAnnotationOptions.ShowAdjustedLengths)!=0; }
            set { SetOption(LineAnnotationOptions.ShowAdjustedLengths, value); }
        }

        public bool ShowObservedLengths
        {
            get { return (m_Options & LineAnnotationOptions.ShowObservedLengths)!=0; }
            set { SetOption(LineAnnotationOptions.ShowObservedLengths, value); }
        }

        public bool ShowObservedAngles
        {
            get { return (m_Options & LineAnnotationOptions.ShowObservedAngles)!=0; }
            set { SetOption(LineAnnotationOptions.ShowObservedAngles, value); }
        }

        /// <summary>
        /// Sets option bit(s)
        /// </summary>
        /// <param name="option">The option bit(s) to set</param>
        /// <param name="setting">True to set, false to clear</param>
        void SetOption(LineAnnotationOptions option, bool setting)
        {
            if (setting)
                m_Options |= option;
            else
                m_Options &= (~option);
        }
    }
}
