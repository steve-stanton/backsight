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
using System.Windows.Forms;

using Backsight.Editor.Forms;
using Backsight.Forms;
using Backsight.Environment;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="03-JAN-1999" was="CuiAddLabel"/>
    /// <summary>
    /// User interface for adding a new polygon label.
    /// </summary>
    abstract class AddLabelUI : SimpleCommandUI
    {
        #region Static

        /// <summary>
        /// A multiplying factor to apply to the text height 
        /// </summary>
        static double s_SizeFactor = 1.0;

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
        /// Rotation angle for labels, in radians.
        /// </summary>
        double m_Rotation;

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

        // Default for oldLabel was null
        abstract internal TextFeature AddNewLabel(IPosition posn, TextFeature oldLabel);

        abstract internal bool Update(TextFeature label);

        /// <summary>
        /// The entity type for the labels.
        /// </summary>
        protected IEntity Entity
        {
            get { return m_Entity; }
            set { m_Entity = value; }
        }

        /// <summary>
        /// Draws a rectangle representing the outline of the label that is currently being added.
        /// </summary>
        /// <param name="refpos">Reference position for the label</param>
        protected void DrawRect(IPosition refpos)
        {
            if (refpos==null)
                return;

            // Draw the outline of the label.
            IPosition[] outline = GetOutline(refpos);
            IDrawStyle style = new DrawStyle(); // black
            style.Render(ActiveDisplay, outline);

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
            if (m_Rotation > MathConstants.TINY)
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
            get { return m_Rotation; }
        }

        /// <summary>
        /// The horizontal bearing of the text string (i.e. the bearing from the
        /// first to last char).
        /// </summary>
        double HBearing
        {
            get
            {
                if (m_Rotation < MathConstants.TINY)
                    return MathConstants.PIDIV2;
                else if (m_Rotation > MathConstants.PIMUL1P5)
                    return m_Rotation - MathConstants.PIMUL1P5;
                else
                    return m_Rotation + MathConstants.PIDIV2;
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

            // Get the font for the entity type (it SHOULD be defined).
            throw new NotImplementedException("AddLabelUI.SetDimensions");
        }
        /*
	const CeFont* const pCeFont = m_pEntity->GetpFont();
	if ( !pCeFont ) {
		AfxMessageBox("CuiAddLabel::GetLabelInfo\nText type has undefined font");
		DialFinish(0);
		return FALSE;
	}

	// Define the MFC font object, assuming horizontal.
	CFont font;
	LOGFONT logfont;
	pCeFont->SetLogFont(logfont);
	font.CreateFontIndirect(&logfont);

	// Get the dimensions of the annotation, in logical units.
	CeDraw* pDraw = GetpWnd();
	CClientDC dc(pDraw);
	dc.SetTextAlign(TA_TOP|TA_LEFT);
	dc.SelectObject((CFont*)&font);
	CSize size = dc.GetTextExtent(str);

	// All done with the font object.
	dc.SelectObject((CFont*)0);
	font.DeleteObject();

	// Remember the width and height of the text, in ground units (if
	// we stored logical units, the meaning of the values might change
	// if the scale changed during label positioning).
	m_Width  = pDraw->LPToLength(FLOAT8(size.cx));
	m_Height = pDraw->LPToLength(FLOAT8(size.cy));

	// Reset the position of the last text outline that was drawn.
	m_LastPos.Reset();

	return TRUE;

} // end of SetDimensions
        */

        /// <summary>
        /// Resets info when derived class has just added a label.
        /// </summary>
        protected void OnLabelAdd()
        {
            m_LastPos = null;
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
            DrawRect(m_LastPos);
        }

        /// <summary>
        /// Returns the command ID that corresponds to the current size factor
        /// applied to text dimensions.
        /// </summary>
        /// <returns>The command ID corresponding to the size factor.</returns>
        protected int GetSizeId()
        {
            throw new NotImplementedException("AddLabelUI.GetSizeId");
        }
        /*
protected INT4 CuiAddLabel::GetSizeId ( void ) const {

	if ( fabs(m_SizeFactor-5.00)<TINY ) return ID_TEXT_500;
	if ( fabs(m_SizeFactor-2.00)<TINY ) return ID_TEXT_200;
	if ( fabs(m_SizeFactor-1.50)<TINY ) return ID_TEXT_150;
	if ( fabs(m_SizeFactor-1.00)<TINY ) return ID_TEXT_100;
	if ( fabs(m_SizeFactor-0.75)<TINY ) return ID_TEXT_75;
	if ( fabs(m_SizeFactor-0.50)<TINY ) return ID_TEXT_50;
	if ( fabs(m_SizeFactor-0.25)<TINY ) return ID_TEXT_25;

	return 0;

} // end of GetSizeId
        */

        /// <summary>
        /// Defines the command ID that corresponds to the current size factor
        /// applied to text dimensions.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected bool SetSizeId(int id)
        {
            throw new NotImplementedException("AddLabelUI.SetSizeId");
        }

        /*
protected LOGICAL CuiAddLabel::SetSizeId ( const INT4 id ) {

	switch ( id ) {

	case ID_TEXT_500:	SetTextSize(500);	return TRUE;
	case ID_TEXT_200:	SetTextSize(200);	return TRUE;
	case ID_TEXT_150:	SetTextSize(150);	return TRUE;
	case ID_TEXT_100:	SetTextSize(100);	return TRUE;
	case ID_TEXT_75:	SetTextSize(75);	return TRUE;
	case ID_TEXT_50:	SetTextSize(50);	return TRUE;
	case ID_TEXT_25:	SetTextSize(25);	return TRUE;
	}

	return FALSE;

} // end of SetSizeId
        */

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
            s_SizeFactor = (double)pc / 100.0;

            // Redraw the rectangle at the last position.
            DrawRect(m_LastPos);
        }

        /// <summary>
        /// The width of the text, in meters on the ground (scaled by the current magnification factor)
        /// </summary>
        protected double Width
        {
            get { return (m_Width * s_SizeFactor); }
        }

        /// <summary>
        /// The height of the text, in meters on the ground (scaled by the current magnification factor)
        /// </summary>
        protected double Height
        {
            get { return (m_Height * s_SizeFactor); }
        }

        /// <summary>
        /// Are text labels currently drawn to the active display?
        /// </summary>
        protected bool IsTextDrawn
        {
            get
            {
                CadastralMapModel cmm = CadastralMapModel.Current;
                ISpatialDisplay display = ActiveDisplay;
                return (display!=null && display.MapScale <= cmm.ShowLabelScale);
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
            return FinishCommand();
        }
    }
}
