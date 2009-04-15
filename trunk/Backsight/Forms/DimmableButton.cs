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
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Backsight.Forms
{
    /// <summary>
    /// A button that may be dimmed when it is disabled (for use with buttons that
    /// show some sort of background image). Not really meant for buttons that just
    /// contain plain text, since the basic <see cref="Button"/> class already
    /// provides dimming for plain text.
    /// </summary>
    public partial class DimmableButton : Button
    {
        #region Class data

        /// <summary>
        /// The normal background image for this button (used when the button is enabled).
        /// May be null for buttons that just contain plain text.
        /// </summary>
        Image m_NormalImage;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DimmableButton"/> class.
        /// </summary>
        public DimmableButton()
        {
            InitializeComponent();
        }

        #endregion

        /// <summary>
        /// Reacts to the <see cref="E:EnabledChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnEnabledChanged(EventArgs e)
        {
            if (this.Enabled)
            {
                if (m_NormalImage!=null)
                    BackgroundImage = m_NormalImage;
            }
            else
            {
                m_NormalImage = BackgroundImage;

                if (m_NormalImage!=null)
                {
                    Bitmap bm = new Bitmap(m_NormalImage);
                    BackgroundImage = ConvertToGrayscale(bm);
                }
            }
        }

        /// <summary>
        /// Converts the supplied bitmap into a gray-scale image.
        /// See http://www.bobpowell.net/grayscale.htm - this is the simple method, perhaps
        /// not the slickest.
        /// </summary>
        /// <param name="source">The bitmap to convert (not null)</param>
        /// <returns>The gray-scale version of the supplied image</returns>
        Bitmap ConvertToGrayscale(Bitmap source)
        {
            Bitmap bm = new Bitmap(source.Width, source.Height);

            for (int y=0; y<bm.Height; y++)
            {
                for (int x=0; x<bm.Width; x++)
                {
                    Color c = source.GetPixel(x, y);
                    int luma = (int)(c.R*0.3 + c.G*0.59+ c.B*0.11);
                    bm.SetPixel(x, y, Color.FromArgb(luma, luma, luma));
                }
            }

            return bm;
        }
    }
}
