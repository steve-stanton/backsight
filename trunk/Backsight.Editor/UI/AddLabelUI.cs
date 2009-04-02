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
using System.Windows.Forms;
using System.Drawing;

using Backsight.Editor.Forms;
using Backsight.Forms;
using Backsight.Environment;

namespace Backsight.Editor.UI
{
    /// <written by="Steve Stanton" on="03-JAN-1999" was="CuiAddLabel"/>
    /// <summary>
    /// User interface for adding a new polygon label.
    /// </summary>
    abstract class AddLabelUI : SimpleCommandUI
    {
        #region Static

        /// <summary>
        /// A multiplying factor to apply to the text height (expressed as a percentage)
        /// </summary>
        //static double s_SizeFactor = 1.0;
        static uint s_SizeFactor = 100;

        #endregion

        #region Class data

        /// <summary>
        /// The entity type for the labels.
        /// </summary>
        IEntity m_Entity;

        /// <summary>
        /// Width of text (meters on the ground).
        /// </summary>
        double m_Width;

        /// <summary>
        /// Height of text (meters on the ground).
        /// </summary>
        double m_Height;

        /// <summary>
        /// The position last used for generating an outline of the current label.
        /// </summary>
        IPosition m_LastPos;

        /// <summary>
        /// Rotation angle for labels, in radians. This should be used only if the user
        /// is positioning the text "manually" (if the <see cref="IsAutoPosition"/> property
        /// is true, text is always added horizontally).
        /// </summary>
        double m_Rotation;

        /// <summary>
        /// The text that was last used to derive m_Width and m_Height
        /// </summary>
        string m_Text;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>AddLabelUI</c>
        /// </summary>
        /// <param name="cc">The container for any dialogs</param>
        /// <param name="action">The action that initiated this command</param>
        protected AddLabelUI(IControlContainer cc, IUserAction action)
            : base(cc, action)
        {
            m_Entity = null;
            m_Width = 0.0;
            m_Height = 0.0;
            m_LastPos = null;
            m_Rotation = CadastralMapModel.Current.DefaultTextRotation;
        }

        #endregion

        abstract internal bool IsAutoPosition { get; }

        abstract internal TextFeature AddNewLabel(IPosition posn);

        abstract internal bool UpdateLabel(TextFeature label);

        /// <summary>
        /// The entity type for the labels.
        /// </summary>
        protected IEntity Entity
        {
            get { return m_Entity; }
            set { m_Entity = value; }
        }

        /// <summary>
        /// Draws the text that is currently being added, together with a rectangle representing
        /// the outline of the label.
        /// </summary>
        /// <param name="refpos">Reference position for the label</param>
        protected void DrawText(IPosition refpos)
        {
            if (refpos==null)
                return;

            // Draw the outline of the label.
            IPosition[] outline = GetOutline(refpos);
            IDrawStyle style = new DrawStyle(); // black
            style.Render(ActiveDisplay, outline);

            // Draw the text
            PointGeometry p = PointGeometry.Create(refpos);
            IFont font = (m_Entity==null ? null : m_Entity.Font);
            MiscText text = new MiscText(m_Text, p, font, m_Height, m_Width, (float)m_Rotation);
            style.Render(ActiveDisplay, text);

            // If doing auto-angle stuff, draw an additional line
            // at the position used to search for candidate lines
            // ... perhaps (the auto-angle stuff needs to move to
            // this class from CuiNewLabel)

            // Remember the specified position as the "last" position.
            m_LastPos = refpos;
        }

        /// <summary>
        /// Erases the the outline of the label that is currently being added.
        /// </summary>
        protected void EraseRect()
        {
            if (m_LastPos!=null)
            {
                m_LastPos = null;
                ErasePainting();
            }
        }

        /// <summary>
        /// Returns the corners of the current label (as a closed outline)
        /// </summary>
        /// <param name="refpos">The reference position for the label.</param>
        /// <returns>Array of 5 positions. The first and last positions refer to the top-left corner
        /// of the first character of the text string. The rest appear in clockwise order.</returns>
        protected IPosition[] GetOutline(IPosition refpos) // was GetCorners, which returned only 4 positions
        {
            IPosition[] result = new IPosition[5];
            result[0] = refpos;
            result[4] = refpos;

            // If the text is rotated ...
            if (Rotation > MathConstants.TINY)
            {
                // Get vertical and horizontal bearings for the label.
                double vbearing = VBearing;
                double hbearing = HBearing;

                // Project to the top-right corner.
                result[1] = Geom.Polar(result[0], hbearing, Width);

                // Project from the 2 top points to the points underneath.
                result[2] = Geom.Polar(result[1], vbearing + Math.PI, Height);
                result[3] = Geom.Polar(result[0], vbearing + Math.PI, Height);
            }
            else
            {
                // Horizontal text

                // Get the position of the reference position.
                double xo = refpos.X;
                double yo = refpos.Y;

                result[1] = new Position(xo+Width, yo);
                result[2] = new Position(xo+Width, yo-Height);
                result[3] = new Position(xo, yo-Height);
            }

            return result;
        }

        /// <summary>
        /// The vertical bearing of the text string (i.e. the bearing from the
        /// bottom of the characters to the top).
        /// </summary>
        double VBearing
        {
            get { return Rotation; }
        }

        /// <summary>
        /// The horizontal bearing of the text string (i.e. the bearing from the
        /// first to last char).
        /// </summary>
        double HBearing
        {
            get
            {
                double r = Rotation;

                if (r < MathConstants.TINY)
                    return MathConstants.PIDIV2;
                else if (r > MathConstants.PIMUL1P5)
                    return r - MathConstants.PIMUL1P5;
                else
                    return r + MathConstants.PIDIV2;
            }
        }

        /// <summary>
        /// Defines the dimensions of the text that's being added. For this to
        /// work, the <see cref="Entity"/> property must be defined beforehand.
        /// </summary>
        /// <param name="str">The text involved</param>
        /// <returns>True if dimensions set ok. False if the entity type
        /// was not defined, or it does not have an associated font.</returns>
        protected bool SetDimensions(string str)
        {
            // Confirm the entity type has been defined
            if (m_Entity==null)
            {
                MessageBox.Show("AddLabelUI.SetDimensions - Text type not specified.");
                DialFinish(null);
                return false;
            }

            // Remember the text that the dimensions correspond to
            m_Text = str;

            // The entity type provides the height of the text (in point units)
            IFont fontInfo = m_Entity.Font;
            if (fontInfo==null)
                fontInfo = FontInfo.Default;

            // Figure out the ground height at the map's nominal scale
            float fontSize = fontInfo.PointSize;
            uint nominalScale = EditingController.Current.JobFile.Data.NominalMapScale;
            double ht = (double)fontSize * (double)nominalScale * MathConstants.POINTSIZE_TO_METERS;

            // Convert into pixels on the active display
            float htPixels = ActiveDisplay.LengthToDisplay(ht);
            Font font = new Font(fontInfo.TypeFace, htPixels, fontInfo.Modifiers, GraphicsUnit.Pixel);

            // Get the size of the text
            Size proposedSize = new Size(int.MaxValue, int.MaxValue);
            Size size = TextRenderer.MeasureText(str, font, proposedSize,
                (TextFormatFlags.NoPadding | TextFormatFlags.NoClipping | TextFormatFlags.NoPrefix));

            // Remember the width and height of the text, in ground units (if
            // we stored logical units, the meaning of the values might change
            // if the scale changed during label positioning).
            ISpatialDisplay display = ActiveDisplay;
            m_Width = display.DisplayToLength((float)size.Width);
            m_Height = display.DisplayToLength((float)size.Height);

            // Reset the position of the last text outline that was drawn.
            m_LastPos = null;
            return true;
        }

        /// <summary>
        /// Nulls the position used for drawing the outline of new text
        /// </summary>
        protected void ResetLastPos()
        {
            m_LastPos = null;
        }

        /// <summary>
        /// Resets info when derived class has just added a label.
        /// </summary>
        protected void OnLabelAdd()
        {
            m_LastPos = null;

            // Ensure the newly added label is part of the display buffer (this
            // normally gets called only when a command completes - however, the
            // commands for adding text may perform several editing operations
            // before completing).
            Controller.RefreshAllDisplays();
        }

        /// <summary>
        /// The rotation of the text, in radians clockwise from the horizontal (+x) axis 
        /// </summary>
        protected double Rotation
        {
            get
            {
                if (IsAutoPosition)
                    return 0.0;
                else
                    return m_Rotation;
            }
        }

        /// <summary>
        /// Changes the rotation angle.
        /// </summary>
        /// <param name="rot">The new rotation angle (in radians).</param>
        protected void SetRotation(double rot)
        {
            // Erase any rectangle that is currently drawn
            EraseRect();

            // Remember the new rotation.
            m_Rotation = rot;

            // Make the new rotation the map's default.
            CadastralMapModel.Current.DefaultTextRotation = rot;

            // Redraw the rectangle at the last position.
            DrawText(m_LastPos);
        }

        /// <summary>
        /// Defines the <see cref="SizeFactor"/> property using the factor in the
        /// supplied action (which is expected to be an instance of <see cref="TextSizeAction"/>).
        /// </summary>
        /// <remarks>This method is used by the context menus created by derived command
        /// classes (e.g. <see cref="NewTextContextMenu"/>)</remarks>
        /// <param name="action">The action for defining a new size factor</param>
        internal void SetSizeFactor(IUserAction action)
        {
            TextSizeAction tsa = (TextSizeAction)action;
            SetTextSize(tsa.SizeFactor);
        }

        /// <summary>
        /// The multiplying factor to apply to the text height (expressed as a percentage)
        /// </summary>
        internal uint SizeFactor
        {
            get { return s_SizeFactor; }
        }

        /// <summary>
        /// Adjusts text size.
        /// </summary>
        /// <param name="pc">The percentage of the current size that the text should be set to.</param>
        void SetTextSize(uint pc)
        {
            // Erase any rectangle that is currently drawn
            EraseRect();

            // Remember the new height factor.
            // A factor is used so that the same factor can be remembered
            // for each label that's added during a single editing command.
            //s_SizeFactor = (double)pc / 100.0;
            s_SizeFactor = pc;

            // Redraw the rectangle at the last position.
            //DrawRect(m_LastPos);

            // Derived classes may need to know that size has changed
            OnSizeFactorChange();
        }

        /// <summary>
        /// Performs any processing when the text magnification factor has been changed. This
        /// implementation does nothing. The <see cref="NewLabelUI"/> class overrides.
        /// </summary>
        internal virtual void OnSizeFactorChange()
        {
        }

        /// <summary>
        /// The width of the text, in meters on the ground (scaled by the current magnification factor)
        /// </summary>
        protected double Width
        {
            get { return (m_Width * (double)s_SizeFactor/100.0); }
        }

        /// <summary>
        /// The height of the text, in meters on the ground (scaled by the current magnification factor)
        /// </summary>
        protected double Height
        {
            get { return (m_Height * (double)s_SizeFactor/100.0); }
        }

        /// <summary>
        /// Are text labels currently drawn to the active display?
        /// </summary>
        protected bool IsTextDrawn
        {
            get
            {
                EditingController ec = EditingController.Current;
                ISpatialDisplay display = ActiveDisplay;
                return (display!=null && display.MapScale <= ec.JobFile.Data.ShowLabelScale);
            }
        }

        /// <summary>
        /// Reacts to action that concludes the command dialog.
        /// </summary>
        /// <param name="wnd">The dialog window where the action originated (not used)</param>
        /// <returns>True if command finished ok. This implementation returns the
        /// result of a call to <see cref="FinishCommand"/>.</returns>
        internal override bool DialFinish(Control wnd)
        {
            RestoreTextRotation();
            return FinishCommand();
        }

        /// <summary>
        /// Restores the default text rotation that was defined when this command was started.
        /// This implementation does nothing. The <see cref="NewTextUI"/> class overrides.
        /// </summary>
        internal virtual void RestoreTextRotation()
        {
        }

        /// <summary>
        /// Override indicates that this command performs painting. This means that the
        /// controller will periodically call the <see cref="Paint"/> method (probably during
        /// idle time).
        /// </summary>
        internal override bool PerformsPainting
        {
            get { return true; }
        }

        /// <summary>
        /// Do any command-specific drawing.
        /// </summary>
        /// <param name="point">The specific point (if any) that the parent window has drawn.</param>
        internal override void Paint(PointFeature point)
        {
            DrawText(m_LastPos);
        }

        /// <summary>
        /// Reacts to a situation where the user presses the ESC key.
        /// </summary>
        internal override void Escape()
        {
            DialFinish(null);
        }
    }
}
