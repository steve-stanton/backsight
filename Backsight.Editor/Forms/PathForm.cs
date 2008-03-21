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
using System.Collections.Generic;
using System.Drawing;

using Backsight.Editor.Operations;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" was="CdPath"/>
    /// <summary>
    /// Dialog for a new connection path.
    /// </summary>
    partial class PathForm : Form
    {
        #region Class data

        /// <summary>
        /// The command that is running this dialog.
        /// </summary>
        readonly PathUI m_Command;

        /// <summary>
        /// The start of the path.
        /// </summary>
        PointFeature m_From;

        /// <summary>
        /// The end of the path.
        /// </summary>
        PointFeature m_To;

        /// <summary>
        /// The control that last had the focus.
        /// </summary>
        Control m_Focus;

        /// <summary>
        /// True if user pointer to obtain the start of the path.
        /// </summary>
        bool m_FromPointed;

        /// <summary>
        /// True if user pointer to obtain the end of the path.
        /// </summary>
        bool m_ToPointed;

        /// <summary>
        /// Parsed path items
        /// </summary>
        List<PathItem> m_Items;

        /// <summary>
        /// The path created from the items.
        /// </summary>
        PathOperation m_Path;

        /// <summary>
        /// True if the path has been drawn.
        /// </summary>
        bool m_DrawPath;

        /// <summary>
        /// Rotation for path (in radians)
        /// </summary>
        double m_Rotation;

        /// <summary>
        /// Scaling to apply to path distances
        /// </summary>
        double m_ScaleFactor;

        /// <summary>
        /// Current dialog for closure info.
        /// </summary>
        AdjustmentForm m_Adjustment;

        #endregion

        #region Constructors

        internal PathForm(PathUI command, PointFeature from, PointFeature to)
        {
            InitializeComponent();

            m_Command = command;
            m_From = from;
            m_To = to;

            SetZeroValues();
        }
    
        #endregion

        void SetZeroValues()
        {
	        m_Focus			= null;
	        m_FromPointed	= false;
	        m_ToPointed		= false;
	        m_Items			= null;
	        m_Path			= null;
	        m_DrawPath		= false;
	        m_Rotation		= 0.0;
	        m_ScaleFactor	= 0.0;
	        m_Adjustment	= null;
        }

        private void PathForm_Shown(object sender, EventArgs e)
        {
            // Display at top left corner of the screen.
            this.Location = new Point(0, 0);

            // If we are recalling an old operation, fill in the
            // data entry string.
            PathOperation op = null;
            if (m_Command != null)
                op = (m_Command.Recall as PathOperation);
            ShowInput(op);

            // Display the current default units.
            //defaultUnitsLabel.Text = 
        }

        /*
BOOL CdPath::OnInitDialog() 
{
	CdDistance dist;
	CStatic* pText = (CStatic*)GetDlgItem(IDC_UNITS);
	pText->SetWindowText(dist.FormatUnits());

//	Hold current entry units in a local static variable, in case
//	the above is erased.
	this->GetUnits();

//	Disable the "Preview" & "OK" buttons. They are only valid
//	after the from- and to-points have been entered, and after
//	something has been entered for the path.

	this->NoPreview();

	// If we already have a from and a to point, fill them
	// in and start in the data entry field.
	if ( m_pFrom && m_pTo ) {

		m_Focus = IDC_FROM;
		OnSelectPoint(m_pFrom);

		m_Focus = IDC_TO;
		OnSelectPoint(m_pTo);

		return FALSE;
	}
	else
		return TRUE;
}
*/

        /// <summary>
        /// Saves the path. This method is called from <see cref="AdjustmentForm"/>
        /// when the user clicks the "OK" button.
        /// </summary>
        internal void Save()
        {
        }
        /*
void CdPath::Save ( void ) {

//	There SHOULD be a path to save.
	if ( !m_pPath ) {
		AfxMessageBox ( "CdPath::Save -- nothing to save" );
		return;
	}

	// Ensure adjustment dialog has been destroyed.
	OnDestroyAdj();

//	Create persistent path.
	CeMap* pMap = CeMap::GetpMap();
	CePath* pSave = new ( os_database::of(pMap),
						  os_ts<CePath>::get() )
						  CePath(*m_pPath);

//	Tell map a save is starting.
	pMap->SaveOp(pSave);

//	Execute the operation.
	LOGICAL ok = pSave->Execute();

//	Tell the map the save has finished.
	pMap->SaveOp(pSave,ok);

//	If something went wrong, get rid of the persistent path.
	if ( !ok ) {
		delete pSave;
		return;
	}

	Finish();
	
} // end of Save
        */

        private void cancelButton_Click(object sender, EventArgs e)
        {
            // If we are showing adjustment results, get rid of them
            if (m_Adjustment!=null)
            {
                m_Adjustment.Dispose();
                m_Adjustment = null;
            }

            // Tell the command that's running this dialog that we're done
            if (m_Command!=null)
                m_Command.DialAbort(this);
        }

        /*
void CdPath::Finish ( void ) {

//	Revert from & to point to normal colour.
	this->SetNormalColour(m_pFrom);
	this->SetNormalColour(m_pTo);

	// Tell the command that's running this dialog that we're
	// finished (this will delete the memory for the CdPath object).
	if ( m_pCommand ) m_pCommand->DialFinish(this);
}
        */

        /// <summary>
        /// Performs processing upon selection of a new point (indicated by pointing at the map)
        /// </summary>
        /// <param name="point">The point that has been selected</param>
        internal void OnSelectPoint(PointFeature point)
        {
        }

        /*
// React to selection of a point on the map.

void CdPath::OnSelectPoint ( CePoint* pPoint ) {

//	Return if point is not defined.
	if ( !pPoint ) return;

//	Set colour
	SetColour(pPoint);

//	Handle the pointing, depending on what field we were
//	last in.

	switch ( m_Focus ) {

	case IDC_FROM: {

//		Ensure that any previously selected backsight reverts
//		to its normal colour and remember the new point.
		if ( pPoint!=m_pFrom ) {
			SetNormalColour(m_pFrom);
			m_pFrom = pPoint;
		}

//		Remember that the user pointed.
		m_FromPointed = TRUE;

//		Display it.
		GetDlgItem(IDC_FROM)->SetWindowText(m_pFrom->FormatKey());

//		See if preview button can be enabled.
		CheckPreview();

//		Move focus to the to-point.
		GetDlgItem(IDC_TO)->SetFocus();

		return;
	}

	case IDC_TO: {

//		Save the from point, and remember that the user pointer.
		if ( pPoint!=m_pTo ) {
			SetNormalColour(m_pTo);
			m_pTo= pPoint;
		}

		m_ToPointed = TRUE;

//		Display it.
		GetDlgItem(IDC_TO)->SetWindowText(m_pTo->FormatKey());

//		See if preview button can be enabled.
		CheckPreview();

//		Move focus to the data field, making sure that nothing
//		is initially selected.
		this->GotoPath();

		return;
	}

	default:

		return;

	} // end switch

} // end of OnSelectPoint
        */

        /// <summary>
        /// Ensures a point is drawn in its normal color.
        /// </summary>
        /// <param name="point">The point that needs to be drawn normally</param>
        void SetNormalColor(PointFeature point)
        {
            // Redraw in idle time
            if (point != null)
                m_Command.ErasePainting();
        }

        /// <summary>
        /// Sets color for a point. 
        /// </summary>
        /// <param name="point">The point to draw.</param>
        /// <param name="c">The field that the point relates to. The default is
        /// the field that currently has the focus.</param>
        void SetColor(PointFeature point, Control c)
        {
            // Return if point not specified.
            if (point==null)
                return;

            ISpatialDisplay display = m_Command.ActiveDisplay;
            Control field = (c == null ? m_Focus : c);

            if (Object.ReferenceEquals(field, fromTextBox))
                point.Draw(display, Color.DarkBlue);
            else if (Object.ReferenceEquals(field, toTextBox))
                point.Draw(display, Color.LightBlue);
        }

        /// <summary>
        /// Does any painting that this dialog does.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        internal void Render(ISpatialDisplay display)
        {
        }

        /*
void CdPath::OnDraw ( const CePoint* const pPoint ) const {

	if ( !pPoint )
		this->OnDrawAll();
	else {
		if ( pPoint==m_pFrom ) this->SetColour(m_pFrom,IDC_FROM);
		if ( pPoint==m_pTo ) this->SetColour(m_pTo,IDC_TO);
	}
}
/////////////////////////////////////////////////////////////////////////////
//
//	@mfunc Handle any redrawing. This just ensures that points
//	are drawn in the right colour.
//
//	@parm TRUE to draw. FALSE to erase.
//
/////////////////////////////////////////////////////////////////////////////

void CdPath::OnDrawAll ( const LOGICAL draw ) const {

//	There's not much to draw, so just do everything.

//	Draw any currently selected points & any direction.

	if ( draw ) {
		SetColour(m_pFrom,IDC_FROM);
		SetColour(m_pTo,IDC_TO);
		if ( m_pPath && m_DrawPath )
			m_pPath->Draw(m_Rotation,m_ScaleFactor);
	}
	else {
		SetNormalColour(m_pFrom);
		SetNormalColour(m_pTo);
	}

	return;

} // end of OnDraw

         */

        private void fromTextBox_Enter(object sender, EventArgs e)
        {
            m_Focus = fromTextBox;
        }

        private void toTextBox_Enter(object sender, EventArgs e)
        {
            m_Focus = toTextBox;
        }

        private void distanceButton_Click(object sender, EventArgs e)
        {
            // Display distance dialog. On OK, replace current selection
            // (if any) with the result of formatting the dialog.

            DistanceForm dial = new DistanceForm();
            if (dial.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show(dial.Format());
                /*
		CEdit* pEdit = (CEdit*)GetDlgItem(IDC_PATH);
		pEdit->ReplaceSel ( dial.Format() );
                 */
            }

            // Put focus back in the data entry box
            pathTextBox.Focus();
        }

        private void fromTextBox_TextChanged(object sender, EventArgs e)
        {
            // If the field is now empty, allow the user to type in an ID.
            if (fromTextBox.Text.Trim().Length == 0)
            {
                SetNormalColor(m_From);
                m_FromPointed = false;
                m_From = null;
                NoPreview();
            }
        }

        private void fromTextBox_Leave(object sender, EventArgs e)
        {

        }

        /*
void CdPath::OnKillfocusFrom ( void ) {

//	Just return if the user specified the field by pointing.
	if ( m_FromPointed ) return;

//	Return if the field is empty.
	if ( IsFieldEmpty(IDC_FROM) ) return;

//	Get address of the edit box.
	CEdit* pEdit = (CEdit*)GetDlgItem(IDC_FROM);

//	Parse the ID value.
	UINT4 idnum;
	if ( !ReadUINT4(pEdit,idnum) ) {
		AfxMessageBox("Invalid point ID");
		pEdit->SetFocus();
		return;
	}

//	Ask the map to locate the address of the specified point.
	const CeKey key(idnum);
	m_pFrom = CeMap::GetpMap()->GetpPoint(key);
	if ( !m_pFrom ) {
		AfxMessageBox("No point with specified key.");
		pEdit->SetFocus();
		return;
	}

//	Display the point in the correct colour.
	this->SetColour(m_pFrom);
	this->CheckPreview();

} // end of OnKillfocusFrom
*/

        private void toTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        /*
void CdPath::OnChangeTo ( void ) {

//	If the field is now empty, allow the user to type in an ID.
	if ( IsFieldEmpty(IDC_TO) ) {
		SetNormalColour(m_pTo);
		m_pTo = 0;
		m_ToPointed = FALSE;
		NoPreview();
	}
}
*/

        private void toTextBox_Leave(object sender, EventArgs e)
        {

        }

        /*
void CdPath::OnKillfocusTo ( void ) {

//	Just return if the user specified the field by pointing.
	if ( m_ToPointed ) return;

//	Return if the field is empty.
	if ( IsFieldEmpty(IDC_TO) ) return;

//	Get address of the edit box.
	CEdit* pEdit = (CEdit*)GetDlgItem(IDC_TO);

//	Parse the ID value.
	UINT4 idnum;
	if ( !ReadUINT4(pEdit,idnum) ) {
		AfxMessageBox("Invalid point ID");
		pEdit->SetFocus();
		return;
	}

//	Ask the map to locate the address of the specified point.
	const CeKey key(idnum);
	m_pTo = CeMap::GetpMap()->GetpPoint(key);
	if ( !m_pTo ) {
		AfxMessageBox("No point with specified key.");
		pEdit->SetFocus();
		return;
	}

//	Display the point in the correct colour.
	this->SetColour(m_pTo);
	this->CheckPreview();

} // end of OnKillfocusTo

/////////////////////////////////////////////////////////////////////////////
//
//	@mfunc Check if a field is empty. If it contains white space characters,
//	it is considered to be empty.
//
/////////////////////////////////////////////////////////////////////////////

LOGICAL CdPath::IsFieldEmpty ( const UINT idd ) const {

//	Say the field is empty if the field ID is unknown.
	CWnd* pWnd = GetDlgItem(idd);
	if ( !pWnd ) return TRUE;

//	See if field is empty.
	return ::IsFieldEmpty(pWnd);

} // end of IsFieldEmpty
*/

        private void angleButton_Click(object sender, EventArgs e)
        {
            // Display angle dialog. On OK, replace current selection
            // (if any) with the result of formatting the dialog.

            AngleForm dial = new AngleForm();
            if (dial.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show(dial.Format());
                /*
		            CEdit* pEdit = (CEdit*)GetDlgItem(IDC_PATH);
		            pEdit->ReplaceSel ( dial.Format() );
                 */
            }
            dial.Dispose();

            // Put focus back in the data entry box
            pathTextBox.Focus();
        }

        private void culDeSacButton_Click(object sender, EventArgs e)
        {

        }

        /*
void CdPath::OnCuldesac ( void ) {

//	Display cul-de-sac dialog. On OK, replace current selection
//	(if any) with the result of formatting the dialog.

	CdCuldesac dial;
	if ( dial.DoModal()==IDOK ) {
		CEdit* pEdit = (CEdit*)GetDlgItem(IDC_PATH);
		pEdit->ReplaceSel ( dial.Format() );
	}

//	Put focus back in the data entry box
	GetDlgItem(IDC_PATH)->SetFocus();
}
*/

        private void curveButton_Click(object sender, EventArgs e)
        {

        }

        /*
void CdPath::OnCurve ( void ) {

//	Display curve dialog. On OK, replace current selection
//	(if any) with the result of formatting the dialog.

	CdCurve dial;
	if ( dial.DoModal()==IDOK ) {
		CEdit* pEdit = (CEdit*)GetDlgItem(IDC_PATH);
		pEdit->ReplaceSel ( dial.Format() );
	}

//	Put focus back in the data entry box
	GetDlgItem(IDC_PATH)->SetFocus();
}
*/

        private void previewButton_Click(object sender, EventArgs e)
        {

        }
        /*
void CdPath::OnPreview ( void ) {

//	Parse the entered path
	if ( !this->ParsePath() ) return;

//	Adjust the path.
	this->Adjust();

} // end of OnPreview
*/

        private void endCurveButton_Click(object sender, EventArgs e)
        {

        }

        /*
void CdPath::OnEC ( void ) {

//	Stick an /EC at the end of the edit box.
	CEdit* pEdit = (CEdit*)GetDlgItem(IDC_PATH);
	pEdit->ReplaceSel ( ") " );
}
*/

        private void pathTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        /*
void CdPath::OnChangePath ( void ) {

//	If we previously had an adjustment dialog, cancel it.
//	This will also end up calling this->OnDestroyAdj, which
//	means the path will be erased.
	if ( m_pAdjustment ) m_pAdjustment->OnCancel();

//	Delete any path object that remains.
	delete m_pPath;
	m_pPath = 0;

//	The preview button should be enabled only if there is
//	something defined for the path.
	this->CheckPreview();
}

void CdPath::CheckPreview ( void ) const {

	if ( m_pTo && m_pFrom && !IsFieldEmpty(IDC_PATH) ) {
		TurnOn(GetDlgItem(IDC_PREVIEW));
//		TurnOn(GetDlgItem(IDOK));
	}
	else
		this->NoPreview();
}
*/
        void NoPreview()
        {
            previewButton.Enabled = false;
        }
        /*
//	Set focus to the path field, ensuring that nothing is
//	selected when the focus gets there.

void CdPath::GotoPath ( void ) const {

//	Get the current selection.
	CEdit* pEdit = (CEdit*)GetDlgItem(IDC_PATH);
	INT4 istart;
	INT4 iend;
	pEdit->GetSel(istart,iend);

//	Set selection to the character after any current selection.
	pEdit->SetSel(iend,iend);

//	Move the focus.
	pEdit->SetFocus();

	// If there's only one line in the path control, scroll
	// to the end of the line.
	if ( pEdit->GetLineCount()==1 ) {

		// Ensure nothing is selected
//		pEdit->SetSel(-1,-1);

//		CString dmsg;
//		dmsg.Format("scroll %d chars",pEdit->LineLength());
//		AfxMessageBox(dmsg);

		// Scroll to the end of the line.
//		pEdit->LineScroll(1,pEdit->LineLength());
		pEdit->LineScroll(0,iend);
	}

}

LOGICAL CdPath::ParsePath ( void ) {

//	How many characters have we got (this includes extra junk
//	on each line of the control).
	CEdit* pEdit = (CEdit*)GetDlgItem(IDC_PATH);
	INT4 nchars = pEdit->GetWindowTextLength();
	if ( nchars<=0 ) return FALSE;

//	How many lines have we got (returns 1 if NO text)
	UINT4 nline = pEdit->GetLineCount();
	CHARS line[128];
	CHARS* nl="\n";
//	string.

//	Allocate one long string for all the text. Add extra chars; one
//	for trailing null, and one for each line, to account for the
//	newline chars we stick on.
	CHARS* str = new CHARS[nchars+nline+1];
	for ( UINT4 c=0; c<nchars; str[c]='\0', c++ );

	for ( UINT4 i=0; i<nline; i++ ) {

//		Get index to the current line. Skip if line is empty.
		UINT4 index = pEdit->LineIndex(i);
		UINT4 nc = pEdit->LineLength(index);
		if ( nc==0 ) continue;

//		Pick out the text and concatenate with complete result.
		nc = pEdit->GetLine( i, line, sizeof(line)-1 );
		line[nc] = '\0';
		nc = StrLength(line);
		line[nc] = '\0';
		if ( nc==0 ) continue;

		strcat ( str, line );
		strcat ( str, nl );
	}

//	Ignore any trailing white space.
	UINT4 slen = StrLength(str);
	str[slen] = '\0';

//	Create stuff based on the string.
	LOGICAL ok = this->ParseString(str);

	delete [] str;
	return ok;

} // end of ParsePath

//	Parse the string that represents the path description.

LOGICAL CdPath::ParseString ( CHARS* str ) {

//	Initialize list of path items.
	delete [] m_Items;
	m_Items = 0;
	m_NumAlloc = 0;
	m_NumItem = 0;

//	Pick out each successive word (delimited by white space).

	CHARS* delims = " \t\n";
	CHARS* token = strtok ( str, delims );

	for ( ; token; token=strtok(NULL,delims) ) {
		if ( !ParseWord(token) ) return FALSE;
	}

//	Validate the path items
	return ValidPath();

} // end of ParseString

LOGICAL CdPath::ParseWord( CHARS* str ) {

#define WORDMAX 32		// Max size for any word.

	CHARS msg[80];		// Message buffer in case we need it.

//	Return if string is empty (could be empty if this function
//	has been called recursively from below).
	UINT4 nc = StrLength(str);
	if ( nc==0 ) return TRUE;

//	Check for excessively long words.
	if ( nc>=WORDMAX ) {
		AfxMessageBox("Path contains un-expectedly long word.");
		return FALSE;
	}

//	If we have a new default units specification, make it
//	the default. There should be whitespace after the "..."

	if ( strstr(str,"...") ) {
		const CeDistanceUnit* const pUnit = this->GetUnits(str,TRUE);
		CePathItem item(PAT_UNITS,pUnit);
		AddItem(item);
		return TRUE;
	}

//	If we have a counter-clockwise indicator, just remember it
//	and parse anything that comes after it.
	if ( nc>=2 && strnicmp(str,"cc",2)==0 ) {
		AddItem(PAT_CC);
		return ParseWord(&str[2]);
	}

//	If we have a BC, remember it & parse anything that follows.
	if ( str[0]=='(' ) {
		AddItem(PAT_BC);
		return ParseWord(&str[1]);
	}

//	If we have an EC, remember it & parse anything that follows.
	if ( str[0]==')' ) {
		AddItem(PAT_EC);
		return ParseWord(&str[1]);
	}

//	If we have a single slash character (possibly followed by
//	a digit or a decimal point), record the single slash &
//	parse anything that follows.

	if ( str[0]=='/' ) {

//		Check for a free-standing slash, or a slash that is
//		followed by a numeric digit or decimal point.

		if ( nc==1 || isdigit(str[1]) || str[1]=='.' ) {
			AddItem(PAT_SLASH);
			return ParseWord(&str[1]);
		}

//		More than one character, or what follows is not a digit.
//		So we are dealing with either a miss-connect, or an
//		omit-point. In either case, there should be whitespace
//		after that.

		if ( nc==2 ) {

			if ( str[1]=='-' ) {
				AddItem(PAT_MC);
				return TRUE;
			}

			if ( str[1]=='*' ) {
				AddItem(PAT_OP);
				return TRUE;
			}
		}

//		Allow CADCOR-style data entry

		else if ( nc==3 ) {
				
			if ( strnicmp(str,"/mc",3)==0 ) {
				AddItem(PAT_MC);
				return TRUE;
			}

			if ( strnicmp(str,"/op",3)==0 ) {
				AddItem(PAT_OP);
				return TRUE;
			}
		}

		sprintf ( msg, "Unexpected qualifier '%s'", str );
		AfxMessageBox(msg);
		return FALSE;
	}

//	If we have a multiplier, it must be immediately followed
//	by a numeric (integer) value.
	if ( str[0]=='*' ) {

		if ( nc==1 ) {
			AfxMessageBox("Unexpected '*' character");
			return FALSE;
		}

//		Pick up the repeat count.
		CHARS* pNext;
		UINT4 repeat = strtol(&str[1],&pNext,10);

//		Error if repeat count is less than 2.
		if ( repeat<2 || pNext==&str[1] ) {
			sprintf ( msg, "Unexpected repeat count in '%s'", str );
			AfxMessageBox ( msg );
			return FALSE;
		}

//		Duplicate the last item using the repeat count.
		AddRepeats(repeat);

//		Continue parsing after the repeat count.
		return ParseWord(pNext);
	}

//	If the string contains an embedded qualifier (a "*" or a "/"
//	character), process the portion of any string prior to the
//	qualifier. Note that we have just handled the cases where
//	the qualifier was at the very start of the string.

	CHARS* pQual = strpbrk(str,"*"+"/");
	if ( pQual ) {

//		Make a copy of the stuff prior to the qualifier.
		CHARS copy[WORDMAX];
		const CHARS* pStr = str;
		CHARS* pCopy = copy;
		for ( ; pStr!=pQual; pStr++, pCopy++ )
			*pCopy = *pStr;

//		Remember to null-terminate the copy.
		*pCopy = '\0';

//		Process the copy.
		if ( !ParseWord(copy) ) return FALSE;

//		Process the qualifier.
		return ParseWord(pQual);
	}

//	Process this string. We should have either a value or
//	an angle.

	if ( strchr(str,'-') || IsLastItemBC() ) {

//		If the string contains a "c" character, it's a central
//		angle; end the string there.

		PAT type = PAT_ANGLE;
		CHARS* pCA = strpbrk(str,"cC");
		if ( pCA ) {
			*pCA = '\0';
			type = PAT_CANGLE;
		}
		else {
			
			// Check if it's a deflection (if so, strip out the "d").
			CString dirstr(str);
			dirstr.MakeUpper();
			INT4 dindex = dirstr.Find('D');
			if ( dindex>=0 ) {
				CString temp(dirstr);
				dirstr = temp.Left(dindex) + temp.Mid(dindex+1);
				type = PAT_DEFLECTION;
				strcpy(str,(LPCTSTR)dirstr);
			}
		}


//		Try to parse an angular value into radians.
		FLOAT8 radval;
		if ( StrRad(radval,str) ) {
			CePathItem item(type,0,radval);
			AddItem(item);
			return TRUE;
		}

//		Bad angle.
		sprintf ( msg, "Malformed angle '%s'", str );
		AfxMessageBox(msg);
		return FALSE;
	}
	else {

//		Get the current distance units.
		const CeDistanceUnit* pUnit = GetUnits();

//		Try to parse a floating point value.
		CHARS* pNext;
		FLOAT8 val = strtod(str,&pNext);

//		If the scan stopped prior to the terminating null,
//		we may have distance units, or the ")" character
//		indicating an EC.

		if ( *pNext && *pNext!=')' ) {
			pUnit = GetUnits(pNext);
			if ( !pUnit ) {
				sprintf ( msg, "Malformed value '%s'", str );
				AfxMessageBox(msg);
				return FALSE;
			}
		}

		CePathItem item(PAT_VALUE,pUnit,val);
		AddItem(item);

		if ( *pNext==')' )
			return ParseWord(pNext);
		else
			return TRUE;
	}

} // end of ParseWord

/////////////////////////////////////////////////////////////////////////////
//
//	@mfunc	Repeat the last path item a specific number of times. The
//			thing to repeat HAS to be of type PAT_VALUE (or
//			possibly a PAT_MC that has been automatically inserted
//			by <mf CdPath::AddItem>).
//
//	@parm	The number of times the last item should appear (we
//			will append n-1 copies of the last item).
//
/////////////////////////////////////////////////////////////////////////////

void CdPath::AddRepeats ( const UINT4 repeat ) {

//	Confirm that we have something to repeat.
	if ( m_NumItem==0 ) {
		AfxMessageBox ( "Nothing to repeat" );
		return;
	}

//	If the last item was a PAT_MC, get to the value before that.
	UINT2 prev = m_NumItem-1;
	PAT type = m_Items[prev].GetType();
	if ( type==PAT_MC && prev>0 ) {
		prev--;
		type = m_Items[prev].GetType();
	}

//	It can only be a PAT_VALUE.
	if (  type != PAT_VALUE ) {
		AfxMessageBox ( "Unexpected repeat multiplier" );
		return;
	}

//	Make copies of the last value.
	CePathItem copy(m_Items[prev]);
	for ( UINT4 i=1; i<repeat; i++ )
		this->AddItem(copy);

} // end of AddRepeats

/////////////////////////////////////////////////////////////////////////////
//
//	@mfunc	Get or set the units for distance observations.
//
//	@parm	The string containing the units specification. The
//			characters up to the first white space character (or
//			"." character) must match one of the abbreviations
//			for the desired units. Pass in a null pointer (the
//			default) if you just want to get the current default
//			units.
//	@parm	TRUE if the units obtained should be regarded as
//			the new default. (default is FALSE).
//
//	@rdesc	Pointer to the corresponding units.
//
/////////////////////////////////////////////////////////////////////////////

const CeDistanceUnit* CdPath::GetUnits ( const CHARS* str
									   , const LOGICAL makedef ) const {

	static const CeDistanceUnit* pDefault=0;

//	Get pointer to the enclosing map.
	CeMap* pMap = CeMap::GetpMap();

	if ( str ) {

//		Pick up characters that represent the abbreviation for
//		the units. Break on any white space or a "." character.

		CHARS abbrev[16];
		const CHARS* pc = &str[0];
		for ( UINT4 nc=0; (nc+1)<sizeof(abbrev); pc++, nc++ ) {
			if ( isspace(*pc) ) break;
			if ( *pc == '.' ) break;
			abbrev[nc] = *pc;
		}
		abbrev[nc] = '\0';

//		Try to match the abbreviation to one of the unit
//		types known to the map.
		const CeDistanceUnit* pMatch = this->MatchUnits(abbrev);

//		Issue message if there was no match.
		if ( !pMatch ) {
			CHARS msg[80];
			sprintf ( msg, "No units with abbreviation '%s'", abbrev );
			AfxMessageBox ( msg );
		}

//		If the units should be made the new default, do it so
//		long as the units were obtained.
		if ( makedef && pMatch ) pDefault = pMatch;

//		Return pointer (if any) to the units.
		return pMatch;
	}
	else {

//		If the default was not previously defined, pick up
//		the map's current default.
		if ( !pDefault ) pDefault = &(pMap->GetEntryUnit());

		return pDefault;
	}

} // end of GetUnits

/////////////////////////////////////////////////////////////////////////////
//
//	@mfunc	Convert a string that represents a distance unit
//			abbreviation into a CeDistanceUnit pointer (one of
//			the objects known to the enclosing map).
//
//	@parm	The units abbreviation.
//
//	@rdesc	Pointer to the corresponding units, or NULL if the
//			abbreviation was not found.
//
/////////////////////////////////////////////////////////////////////////////

const CeDistanceUnit* CdPath::MatchUnits ( const CHARS* abbrev ) const {

//	How many characters do we have in the abbreviation?
	UINT4 nc = StrLength(abbrev);

//	See if the abbreviation represents metric.
	const CeDistanceUnit* pUnit;
	const CHARS* pChars;
	const CeMap* pMap = CeMap::GetpMap();

	pUnit = pMap->GetpUnit(UNIT_METRES);
	pChars = pUnit->GetAbbrev();
	if ( StrLength(pChars)==nc && stricmp(abbrev,pChars)==0 ) return pUnit;

//	Try feet
	pUnit = pMap->GetpUnit(UNIT_FEET);
	pChars = pUnit->GetAbbrev();
	if ( StrLength(pChars)==nc && stricmp(abbrev,pChars)==0 ) return pUnit;

//	Try chains
	pUnit = pMap->GetpUnit(UNIT_CHAINS);
	pChars = pUnit->GetAbbrev();
	if ( StrLength(pChars)==nc && stricmp(abbrev,pChars)==0 ) return pUnit;

//	Not a known units abbreviation.
	return 0;

} // end of FindUnits

/////////////////////////////////////////////////////////////////////////////
//
//	@mfunc	Hold on to an additional path item.
//
//	@parm	The item to add.
//
/////////////////////////////////////////////////////////////////////////////

void CdPath::AddItem ( const CePathItem& item ) {

	static LOGICAL omit=FALSE;	// True if point previously omitted.

//	If no items have been added, ensure local static has been
//	freshly initialized.
	if ( m_NumItem==0 ) omit=FALSE;

//	Ignore an attempt to add 2 miss-connects in a row (ValidPath
//	will complain).

	PAT type = item.GetType();

	if ( m_NumItem>0 &&
		 type==PAT_MC &&
		 m_Items[m_NumItem-1].GetType()==PAT_MC ) return;

//	Ensure array is big enough. If not, allocate some more.
	if ( m_NumItem+1 > m_NumAlloc ) this->SetSize(m_NumAlloc+32);

//	Copy the supplied item into the list.
	m_Items[m_NumItem] = item;
	m_NumItem++;

//	If we have just appended a PAT_VALUE, append an additional
//	miss-connect item if we previously omitted a point.
	if ( omit && type==PAT_VALUE ) {
		omit = FALSE;
		AddItem(PAT_MC);
	}

//	Remember whether we just omitted a point.
	if ( type==PAT_OP ) omit = TRUE;

} // end of AddItem

/////////////////////////////////////////////////////////////////////////////
//
//	@mfunc	Hold on to an additional path item. Good for items that
//			do not have an associated value.
//
//	@parm	The type of item to add.
//
/////////////////////////////////////////////////////////////////////////////

void CdPath::AddItem ( const PAT type ) {

//	Construct an item to add.
	CePathItem item(type);

//	Add it.
	AddItem(item);

} // end of AddItem

/////////////////////////////////////////////////////////////////////////////
//
//	@mfunc	Allocate space for a specific number of path items.
//
//	@parm	The number of items to allocate.
//
/////////////////////////////////////////////////////////////////////////////

UINT2 CdPath::SetSize ( const UINT2 newsize ) {

	UINT2 size;			// The size actually set

	if ( newsize==0 )
		size = max(1,m_NumItem);		// always get at least 1
	else if ( newsize>m_NumAlloc )
		size = newsize;					// increase
	else
		size = max(newsize,m_NumItem);	// decrease, but not too low

//	Return if the new size is identical to the current size
	if ( size == m_NumAlloc ) return size;

//	Allocate new array
	m_NumAlloc = size;
	CePathItem* newlist = new CePathItem[m_NumAlloc];

//	Copy what we have, delete the original, and then point
//	to the copy.
	if ( m_Items ) {
	  memcpy ( &newlist[0], &m_Items[0],
					m_NumItem*sizeof(CePathItem) );
	  delete [] m_Items;
	}
	m_Items = newlist;
	return m_NumAlloc;

} // end of SetSize

/////////////////////////////////////////////////////////////////////////////
//
//	@mfunc	Check if the last path item is a BC.
//
/////////////////////////////////////////////////////////////////////////////

LOGICAL CdPath::IsLastItemBC ( void ) const {

	if ( m_NumItem==0 ) return FALSE;

	return ( m_Items[m_NumItem-1].m_Item == PAT_BC );

} // IsLastItemBC

/////////////////////////////////////////////////////////////////////////////
//
//	@mfunc	Validate path items. Prior to call, the path should
//			be parsed by making a series of calls to ParseWord.
//			This generates a set of items that are generated
//			without consideration to their context. This function
//			validates the context, elaborating on the meaning of
//			PAT_ANGLE and PAT_VALUE item codes.
//
/////////////////////////////////////////////////////////////////////////////

LOGICAL CdPath::ValidPath ( void ) {

//	The path must contain at least one item.
	if ( m_NumItem==0 ) {
		AfxMessageBox("Path has not been specified.");
		return FALSE;
	}

//	All PAT_VALUE's outside of a curve definition should have
//	type PAT_DISTANCE. Within curves, it's a bit more complicated.

	UINT2 ibc = 0;			// Index of last BC
	LOGICAL curve = FALSE;	// Not in a curve to start with

	for ( UINT2 i=0; i<m_NumItem; i++ ) {

		switch ( m_Items[i].m_Item ) {

//		If we have a BC, confirm that we are not already in
//		a curve. Also confirm that there are at least 3 items
//		after the BC (enough for an angle, a radius, and an EC).

		case PAT_BC: {
			if ( curve ) {
				AfxMessageBox("Nested curve detected");
				return FALSE;
			}
			curve = TRUE;

			if ( (ibc+4) > m_NumItem ) {
				AfxMessageBox("BC not followed by angle, radius, and EC");
				return FALSE;
			}

			ibc = i;
			break;
		}

//		If we have an EC, confirm that we were in a curve.

		case PAT_EC: {
			if ( !curve ) {
				AfxMessageBox("EC was not preceded by BC");
				return FALSE;
			}
			curve = FALSE;
			break;
		}

//		If not in a curve, change all PAT_VALUE types to
//		PAT_DISTANCE types. Inside a curve, PAT_VALUES may
//		actually be angles that need to be converted into
//		radians.

		case PAT_VALUE: {

//			All values must point to the data entry units.
			if ( !m_Items[i].m_pUnit ) {
				AfxMessageBox("Value has no unit of measurement");
				return FALSE;
			}

			if ( !curve )
				m_Items[i].m_Item = PAT_DISTANCE;
			else {

//				The value immediately after the BC is always
//				an angle.

				if ( i==(ibc+1) ) {
					m_Items[i].m_Item = PAT_BANGLE;
					m_Items[i].m_Value *= DEGTORAD;
				}
				else if ( i==(ibc+2) ) {

//					Could be an angle, or a radius. If the NEXT
//					item is a value, we must have an exit angle.

					if ( m_Items[i+1].m_Item == PAT_VALUE ) {
						m_Items[i].m_Item = PAT_EANGLE;
						m_Items[i].m_Value *= DEGTORAD;
					}
					else
						m_Items[i].m_Item = PAT_RADIUS;

				}
				else if ( i==(ibc+3) )
					m_Items[i].m_Item = PAT_RADIUS;
				else
					m_Items[i].m_Item = PAT_DISTANCE;

			}
			break;
		}

//		Angles inside curve definitions have to be qualified. If
//		they appear, they MUST follow immediately after the BC.
//		For angles NOT in a curve, you can only have one angle
//		at a time.

		case PAT_DEFLECTION:
		case PAT_ANGLE: {
			if ( curve ) {

				// Can't have deflections inside a curve.
				if ( m_Items[i].m_Item == PAT_DEFLECTION ) {
					AfxMessageBox("Deflection not allowed within curve definition");
					return FALSE;
				}

				if ( i==(ibc+1) )
					m_Items[i].m_Item = PAT_BANGLE;
				else if ( i==(ibc+2) )
					m_Items[i].m_Item = PAT_EANGLE;
				else {
					AfxMessageBox("Extraneous angle inside curve definition");
					return FALSE;
				}
			}
			else {
				if ( i>0 && m_Items[i-1].m_Item == PAT_ANGLE ) {
					AfxMessageBox("More than 1 angle at the end of a straight");
					return FALSE;
				}

//				Also, it makes no sense to have an angle right
//				after an EC.
				if ( i>0 && m_Items[i-1].m_Item == PAT_EC ) {
					AfxMessageBox("Angle after EC makes no sense");
					return FALSE;
				}

			}
			break;
		}

//		A free-standing slash character is only valid within
//		a curve definition. It has to appear at a specific
//		location in the sequence.
//
//		BC -> BCAngle -> Radius -> Slash
//		BC -> BCAngle -> Radius -> CCMarker -> Slash
//		BC -> BCAngle -> ECAngle -> Radius -> CCMarker -> Slash
//
//		In other words, it can come at ibc+3 through ibc+5.

		case PAT_SLASH: {
			if ( !curve ) {
				AfxMessageBox("Extraneous '/' character");
				return FALSE;
			}
			if ( i<ibc+3 || i>ibc+5 ) {
				AfxMessageBox("Misplaced '/' character");
				return FALSE;
			}
			break;
		}

//		Counter-clockwise indicator. Similar to PAT_SLASH, it has
//		a specific range of valid positions with respect to the BC.

		case PAT_CC: {
			if ( !curve ) {
				AfxMessageBox("Counter-clockwise indicator detected outside curve definition");
				return FALSE;
			}
			if ( i<ibc+3 || i>ibc+4 ) {
				AfxMessageBox("Misplaced 'cc' characters");
				return FALSE;
			}
			break;
		}

//		A central angle is valid only within a curve definition
//		and must be immediately after the BC.

		case PAT_CANGLE: {
			if ( !curve ) {
				AfxMessageBox("Central angle detected outside curve definition");
				return FALSE;
			}
			if ( i!=ibc+1 ) {
				AfxMessageBox("Central angle does not follow immediately after BC");
				return FALSE;
			}
			break;
		}


//		Miss-connections & omit points must always follow on from
//		a PAT_DISTANCE.

		case PAT_MC:
		case PAT_OP: {
			if ( i==0 || m_Items[i-1].m_Item != PAT_DISTANCE ) {
				AfxMessageBox("Miss-Connect or Omit-Point is not preceded by a distance" );
				return FALSE;
			}
			break;
		}

//		No checks

		case PAT_UNITS:

			break;

//		All item types generated via ParseWord should have been
//		listed above, even if there is no check. If any got missed,
//		drop through to a message, but keep going.

		default: {

			CHARS msg[80];
			sprintf ( msg, "CdPath::ValidPath\nUnhandled check (%d)",
						m_Items[i].m_Item );
			AfxMessageBox(msg);

		}

		} // end switch

	} // next item

//	Error if we got to the end, and any curve was not closed.
	if ( curve ) {
		AfxMessageBox ( "Circular curve does not have an EC" );
		return FALSE;
	}

	return TRUE;

} // end of ValidPath

/////////////////////////////////////////////////////////////////////////////
//
//	@mfunc	Adjust a validated path.
//
/////////////////////////////////////////////////////////////////////////////

void CdPath::Adjust ( void ) {

//	AfxMessageBox("CdPath::Adjust");

//	Confirm that the from-point & to-point are both defined (this
//	should have been checked beforehand).
	if ( !(m_pFrom && m_pTo) ) {
		AfxMessageBox("Terminal points have not been defined");
		return;
	}

//	Assign leg numbers to each path item.
	SetLegs();

//	Get rid of any path previously created.
//	CHARS str[80];
//	sprintf ( str, "Deleting old path at %x", m_pPath );
//	AfxMessageBox(str);
	delete m_pPath;
	m_pPath = 0;

//	Create new path object
	m_pPath = new CePath(*m_pFrom,*m_pTo);
//	sprintf( str,"CdPath::Adjust created new path at %x", m_pPath);
//	AfxMessageBox(str);

//	Create the path.
	if ( !m_pPath->Create(m_Items, m_NumItem) ) return;

//	Adjust the path

	FLOAT8 de;				// Misclosure in eastings
	FLOAT8 dn;				// Misclosure in northings
	FLOAT8 prec;			// Precision
	FLOAT8 length;			// Total observed length

	m_pPath->Adjust(dn,de,prec,length,
		m_Rotation,m_ScaleFactor);

//	Draw the path with the adjustment info
	m_DrawPath = TRUE;
	m_pPath->Draw(m_Rotation,m_ScaleFactor);

//	Display the numeric results.
	m_pAdjustment = new CdAdjustment(dn,de,prec,length,this);
	m_pAdjustment->Create(IDD_ADJUSTMENT);

//	Disable the preview button (it will be re-enabled when
//	the adjustment dialog is destroyed).
	TurnOff(GetDlgItem(IDC_PREVIEW));

} // end of Adjust

/////////////////////////////////////////////////////////////////////////////
//
//	@mfunc	Associate each path item with a leg sequence number.
//
//	@rdesc	The number of legs found.
//
/////////////////////////////////////////////////////////////////////////////

UINT2 CdPath::SetLegs ( void ) {

//	AfxMessageBox("CdPath::SetLegs");
//	CHARS msg[80];

	INT2 nleg = 0;

//	Note that PAT_UNITS may not get a leg number.

	for ( UINT2 i=0; i<m_NumItem; ) {

		PAT type = m_Items[i].m_Item;

		if ( type == PAT_DISTANCE ||
			 type == PAT_ANGLE ||
			 type == PAT_DEFLECTION ||
			 type == PAT_BC ) {

//			If we have a distance or angle item, increment the leg 
//			sequence number until we hit an angle or a BC. In the case of
//			an angle, it always comes at the START of a leg.

			if ( type == PAT_DISTANCE ||
				 type == PAT_ANGLE ||
				 type == PAT_DEFLECTION ) {

				nleg++;
				m_Items[i].m_Leg = nleg;
				for ( i++; i<m_NumItem; i++ ) {
					if ( m_Items[i].m_Item == PAT_ANGLE ) break;
					if ( m_Items[i].m_Item == PAT_DEFLECTION ) break;
					if ( m_Items[i].m_Item == PAT_BC ) break;
					m_Items[i].m_Leg = nleg;
				}
			}

			else {

//				We have a BC, so increment the leg sequence number
//				& scan until we hit the EC.

				nleg++;
				for ( ; i<m_NumItem; i++ ) {
					m_Items[i].m_Leg = -nleg;	// negated
					if ( m_Items[i].m_Item == PAT_EC ) {
						i++;
						break;
					}
				}
			}
		}
		else {
			m_Items[i].m_Leg = nleg;
			i++;
		}

	} // next item

	return nleg;

} // end of SetLegs
        */

        /// <summary>
        /// Takes action when the adjustment result dialog has been cancelled (user
        /// has clicked the "Reject" button). At the time of call, the dialog
        /// is already closed, though a reference to the instance is still
        /// held by this class.
        /// </summary>
        internal void OnDestroyAdj()
        {
        }
        /*
void CdPath::OnDestroyAdj ( void ) {

//	Remember not to draw the path.
	if ( m_pPath )
		m_pPath->Erase(m_Rotation,m_ScaleFactor);

	m_DrawPath = FALSE;

//	Get rid of the dialog object.
	delete m_pAdjustment;
	m_pAdjustment = 0;

//	Re-enable the Preview button.
	TurnOn(GetDlgItem(IDC_PREVIEW));

} // end of OnDestroyAdj
        */

        /// <summary>
        /// Fills the data entry field with the stuff that was entered for
        /// a specific connection path.
        /// </summary>
        /// <param name="op">The operation to show.</param>
        void ShowInput(PathOperation op)
        {
            // Return if the operation has not been specified.
            if (op==null)
                return;

            // Get the data entry string.
            string str = op.GetString();

            // Display the data entry string (this takes care of word wrap).
            pathTextBox.Text = str;
        }
    }
}