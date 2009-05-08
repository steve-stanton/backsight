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
using System.Collections.Generic;
using System.Drawing;

using Backsight.Forms;
using Backsight;
using BNS.Properties;

namespace BNS
{
    /// <summary>
    /// The default drawing style for the BNS viewer application
    /// </summary>
    class ViewStyle : DrawStyle
    {
        #region Class data

        /// <summary>
        /// The icons that should be used when drawing survey points
        /// </summary>
        readonly Dictionary<PointStatus, Icon> m_PointIcons;

        /// <summary>
        /// The default icon to use for points with unexpected status values
        /// </summary>
        readonly Icon m_DefaultIcon;

        #endregion

        #region Constructors

        internal ViewStyle()
        {
            m_DefaultIcon = Resources.DefaultSurveyPoint;
            m_PointIcons = new Dictionary<PointStatus, Icon>();
            m_PointIcons.Add(PointStatus.None, GetIcon(Resources.Receiver_Default));
            m_PointIcons.Add(PointStatus.Killed, GetIcon(Resources.Receiver_Killed));
            m_PointIcons.Add(PointStatus.Laid, GetIcon(Resources.Receiver_Laid));
            m_PointIcons.Add(PointStatus.PickedUp, GetIcon(Resources.Receiver_PickedUp));
            m_PointIcons.Add(PointStatus.ReadyToShoot, GetIcon(Resources.Source_ReadyToShoot));
            m_PointIcons.Add(PointStatus.ReadyToLoad, GetIcon(Resources.Source_ReadyToLoad));

            PointHeight = new Length(100.0);
        }

        #endregion

        /// <summary>
        /// Draws a point
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="position">The position of the center of the point</param>
        public override void Render(ISpatialDisplay display, IPosition position)
        {
            if (position is SurveyPoint)
            {
                SurveyPoint sp = (SurveyPoint)position;

                if (display.MapScale >50000)
                    base.Render(display, position);
                else
                {
                    Icon icon;
                    if (!m_PointIcons.TryGetValue(sp.Status, out icon))
                        icon = m_DefaultIcon;

                    Render(display, position, icon);
                }
            }
            else
            {
                base.Render(display, position);
            }
        }

        Icon GetIcon(Bitmap bmp)
        {
            IntPtr handle = bmp.GetHicon();
            return Icon.FromHandle(handle);
        }
    }
}
