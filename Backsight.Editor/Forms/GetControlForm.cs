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
using System.IO;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" on="10-FEB-1998" was="CdGetControl" />
    /// <summary>
    /// Dialog for getting a list of control points
    /// </summary>
    partial class GetControlForm : Form
    {
        #region Class data

        /// <summary>
        /// The command driving this dialog.
        /// </summary>
        readonly GetControlUI m_Cmd;

        /// <summary>
        /// Control ranges (one for each line in the dialog).
        /// </summary>
        List<ControlRange> m_Ranges;

        /// <summary>
        /// True if the map initially has an undefined extent.
        /// </summary>
        bool m_NewMap;

    	// uint m_CurrRange; // used in methods for CCF file input

        #endregion

        #region Constructors

        internal GetControlForm(GetControlUI cmd)
        {
            InitializeComponent();

            m_Cmd = cmd;
            m_Ranges = null;
            m_NewMap = false;
        }

        #endregion

        private void GetControlForm_Shown(object sender, EventArgs e)
        {
            // Remember whether the map starts out empty. We do this here because
            // CadastralMapModel.IsEmpty works by checking whether the map's
            // window is defined. Since we may also set the extent, we could not
            // subsequently get a correct answer as to whether the map already
            // contains data.
            m_NewMap = CadastralMapModel.Current.IsEmpty;

            // Initialize the file spec of the control file, based on the corresponding registry entry.
            // (formerly environment variable CED$ControlFile)
            string cfile = GlobalUserSetting.Read("ControlFile");
            if (!String.IsNullOrEmpty(cfile) && File.Exists(cfile))
            {
                controlFileTextBox.Text = cfile;
                controlTextBox.Focus();
            }
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dial = new OpenFileDialog();
            dial.Filter = "Control files (*.utm)|*.utm|Text files (*.txt)|*.txt|All Files (*.*)|*.*";
            dial.DefaultExt = "utm";
            dial.Title = "Pick Control File";

            // If the user picked a file, display it, and set focus
            // to the list of control points.
            if (dial.ShowDialog() == DialogResult.OK)
            {
                controlFileTextBox.Text = dial.FileName;
                controlTextBox.Focus();
                GlobalUserSetting.Write("ControlFile", dial.FileName);
            }
        }

        private void getDataButton_Click(object sender, EventArgs e)
        {
            /*
//	Get pointer to the list of required control	points.
	CEdit* pEdit = (CEdit*)GetDlgItem(IDC_CONTROL);

//	Get rid of any control list previously loaded.
	KillRanges();

//	Go through each line in the edit box, to confirm
//	that the control ranges are valid. While at it, get a count
//	of the number of ranges.

	UINT4 nrange=0;			// Number of ranges
	UINT4 minid;			// Min ID in range
	UINT4 maxid;			// Max ID in range

//	How many lines have we got (returns 1 if NO text)
	UINT4 nline = pEdit->GetLineCount();
	CHARS line[128];

	for ( UINT4 i=0; i<nline; i++ ) {

//		Get index to the current line. Skip if line is empty.
		UINT4 index = pEdit->LineIndex(i);
		UINT4 nc = pEdit->LineLength(index);
		if ( nc==0 ) continue;

//		Pick out the text and confirm that it is valid. If
//		not, select the current line, issue message, & return.
		nc = pEdit->GetLine( i, line, sizeof(line)-1 );
		line[nc] = '\0';
		nc = StrLength(line);
		line[nc] = '\0';
		if ( nc==0 ) continue;

		if ( !GetRange(line,minid,maxid) ) {
			int schar = int(index);
			int echar = int(index+nc);
			pEdit->SetSel(schar,echar);
			CHARS msg[128];
			sprintf ( msg, "Bad range '%s'.", line);
			AfxMessageBox(msg);
			return;
		}

//		Increment the number of valid ranges.
		nrange++;

	} // next line

//	If no control has been specified, see if we can do things
//	using the current display window.
	if ( nrange==0 ) {
		if ( m_NewMap )
			AfxMessageBox("No control points have been specified.");
		else
			GetInsideWindow();
		return;
	}

//	Create array of the desired ranges.
	m_Ranges = new CeControlRange[nrange];

//	If someone enters a stupidly big range in order to try to
//	get everything, we may have ran out of memory.
	if ( !m_Ranges ) {
		AfxMessageBox("Ran out of memory. Try smaller ranges.");
		return;
	}
	m_NumRange = nrange;
	m_NumAlloc = nrange;

//	Define each range (same sort of loop as above).

	for ( i=0; i<nline; i++ ) {

//		Get index to the current line. Skip if line is empty.
		UINT4 index = pEdit->LineIndex(i);
		UINT4 nc = pEdit->LineLength(index);
		if ( nc==0 ) continue;

//		Pick out the text and confirm that it is valid. If
//		not, select the current line, issue message, & return.
		nc = pEdit->GetLine( i, line, sizeof(line)-1 );
		line[nc] = '\0';
		nc = StrLength(line);
		line[nc] = '\0';
		if ( nc==0 ) continue;

//		Define the range.
		GetRange(line,minid,maxid);
		if ( !m_Ranges[i].SetRange(minid,maxid) ) {
			KillRanges();
			return;
		}

	} // next line

//	Load array of control data.
	CeWindow win;
	if ( !LoadControl(win) ) return;

//	Display the results.
	ShowRanges(win);
             */
        }

        /*

//	Display all ranges.

void CdGetControl::ShowRanges ( const CeWindow& win ) {

//	Get pointer to the list of control points. Then erase
//	whatever's in the list.
	CEdit* pEdit = (CEdit*)GetDlgItem(IDC_CONTROL);
	ClearEdit(pEdit);

//	Return if there are no ranges.
	if ( m_NumRange==0 ) return;

//	We need to ensure that the map has a defined extent. If not,
//	set it to match the extent of the control data we have
//	loaded, and tell the draw to initialize draw parameters.

	if ( m_NewMap ) {
		CeMap::GetpMap()->SetExtent(win);
		CeDraw* pDraw = GetpDraw();
		pDraw->Setup();

		// Tell the view to remember the extent.
		GetpView()->AddExtent();

//		Tell the user the draw scale that has been defined.
		FLOAT8 scale = pDraw->GetDrawScale();
		CHARS scalemsg[80];
		sprintf ( scalemsg, "Draw scale has been set to 1:%d",
					UINT4(scale) );
		AfxMessageBox(scalemsg);
	}

	CString text;			// Complete text for the list
	CHARS output[132];		// Single line in the list
	UINT4 minkey=0;			// Min key in range
	UINT4 maxkey=0;			// Max key in range

//	Only show the first 100 ranges (any more, and the string
//	might get too long to display).

	for ( UINT4 i=0; i<min(100,m_NumRange); i++ ) {

//		Get the range
		minkey = m_Ranges[i].GetMin();
		maxkey = m_Ranges[i].GetMax();

//		Form status string
		UINT4 nfound = m_Ranges[i].GetNumDefined();
		UINT4 ncontr = m_Ranges[i].GetNumControl();
		CHARS status[80];

		if ( nfound==ncontr ) {
			if ( nfound==1 )
				strcpy ( status, "found" );
			else
				strcpy ( status, "all found" );
		}
		else
			sprintf ( status, "found %d out of %d", nfound, ncontr );

		if ( minkey==maxkey )
			sprintf ( output, "%d (%s)\r\n", minkey, status );
		else
			sprintf ( output, "%d-%d (%s)\r\n", minkey, maxkey, status );

//		pEdit->SetWindowText(output);
		text += output;
	}

	pEdit->SetWindowText(text);

//	Draw the control points too.
	this->OnDraw();

//	Message if all the ranges could not be shown.
	if ( m_NumRange>100 ) {
		CHARS msg[132];
		sprintf ( msg, "Only the first 100 ranges (of %d) have been listed.",
						m_NumRange );
		AfxMessageBox(msg);
	}

} // end of ShowRanges
         */

        /*
//	Parse a string that defines a range of IDs.
//
//	@rdesc	The number of IDs in the range (0 if range is invalid).

UINT4 CdGetControl::GetRange ( const CHARS* string
							 , UINT4& minid
							 , UINT4& maxid ) const {

//	Get the number of characters in the string, excluding any
//	trailing white space, and express as a pointer (to the
//	first trailing white space character).
	UINT4 slen = StrLength(string);
	if ( slen==0 ) return 0;
	const CHARS* const pEnd = &string[slen];

//	Do we have a range, or just a single value?
	const CHARS* const pDash = strchr(string,'-');

//	Invalid if the dash is at the start.
	if ( pDash==string ) return 0;

//	If no dash, we should have just one integer value. Otherwise
//	we should have two integer values.

	CHARS* eptr;

	if ( !pDash ) {

//		Parse the value.
		minid = strtoul(string,&eptr,10);
		if ( minid==0 || eptr!=pEnd ) return 0;

//		The end of range is the same as the start.
		maxid = minid;
		return 1;
	}
	else {

//		Parse the first value. It should be the dash that
//		stops the scan (or possibly white space prior to
//		the dash).
		minid = strtoul(string,&eptr,10);
		if ( minid==0 || (eptr!=pDash && !isspace(*eptr)) ) return 0;

		maxid = strtoul(&pDash[1],&eptr,10);
		if ( maxid==0 || eptr!=pEnd ) return 0;

//		The max ID must be greater than the min. If not, assume
//		the user has done something like 7436-43, meaning 7436-7443.

		if ( maxid<minid ) {
			INT4 nc = eptr-pDash-1;	// Number of chars
			if ( nc>0 ) {
				if ( nc==1 )
					maxid += (minid/10 * 10);
				else {
					UINT4 factor=1;
					for ( INT4 i=0; i<(nc-1); factor*=10, i++ );
					maxid += (minid/factor * factor);
				}
			}
		}

		if ( maxid<minid ) return 0;
		return (maxid-minid+1);
	}

} // end of GetRange
         */

        private void addToMapButton_Click(object sender, EventArgs e)
        {
            /*
//	Return if there is nothing to add.
	if ( !m_Ranges ) {
		AfxMessageBox("There is nothing to add.");
		return;
	}

//	Get the currently active theme. If there isn't one, ask
//	for one and make it the default.
	CeMap* pMap = CeMap::GetpMap();
	const CeTheme* pTheme = pMap->GetpTheme();
	if ( !pTheme ) {
		pTheme = GetpView()->SetTheme();
		if ( !pTheme ) return;
	}

	// Do we have an entity type for control points?
	CHARS cent[256];
	GetEnvironmentVariable("CED$ControlEntity",cent,sizeof(cent));

	// If so, get its address in the map.
	CeAttributeStructure* pAtt = pMap->GetpDatabase();
	CeEntity* pDefEnt = pAtt->GetEntityPtr(os_string(cent));

//	Get the desired entity type.
	CdGetEntity dial(pTheme,VERTEX,pDefEnt);
	dial.DoModal();
	const CeEntity* const pEnt = dial.GetpEntity();

	// Save the control.
	this->Save(*pEnt);

//	Eliminate memory for the control points (this will cause
//	them to be erased from the screen).
	this->KillRanges();

//	If points are not currently displayed, issue a warning message.
//	Otherwise invalidate the entire area of the view's client area
//	to force a redraw.

	CeView* pView = GetpView();
	if ( pView ) {
		if ( !pView->ArePointsDrawn() )
			AfxMessageBox("Points will not be drawn at the current scale." );
		else
			pView->InvalidateRect(0);
	}

//	Finish the dialog (this deletes the memory for the dialog).
//	CDialog::OnOK();
	pView->OnFinishControl();
             */
        }

        /*

INT4 CdGetControl::Save ( const CeEntity& ent ) const {

	CWaitCursor wait;		// Start hourglass

	// Create import operation.
	CeMap* pMap = CeMap::GetpMap();
	CeGetControl* pSave = new ( os_database::of(pMap)
							  , os_ts<CeGetControl>::get() ) CeGetControl();

	// Tell the map a save is starting.
	pMap->SaveOp(pSave);

	// Tell each range to add itself to the map.
	for ( UINT4 i=0; i<m_NumRange; i++ )
		m_Ranges[i].Save(*pSave,ent);


	// Execute the op.
	LOGICAL ok = pSave->Execute();

	// Tell map the save has finished.
	pMap->SaveOp(pSave,ok);

	if ( ok )
		return pSave->GetCount();
	else {
		delete pSave;
		return -1;
	}

} // end of Save
         */

        /*
//	Load control data from external file.

LOGICAL CdGetControl::LoadControl ( CeWindow& win ) {

//	Get the name of the control file.
	CString fspec;
	GetDlgItem(IDC_CONTROL_FILE)->GetWindowText(fspec);

//	Initialize the window.
	win.Reset();

//	Open the control file.
//	CFile cfile();
//	if ( !cfile.Open((LPCTSTR)fspec,CFile::modeRead) ) {
	FILE* fp;
	if ( (fp=fopen((LPCTSTR)fspec,"r"))==0 ) {
		CHARS msg[256];
		sprintf ( msg, "Cannot access '%s'", fspec );
		AfxMessageBox(msg);
		return FALSE;
	}

//	Scan though the control file. For each line, try to form
//	a control object. If successful, scan the array of control
//	ranges we have to try to find a match.

	CHARS str[132];			// Input buffer
	UINT4 ncontrol=0;		// Number of valid control points read.
	UINT4 nfound=0;			// Number of control points found.
	CWaitCursor wait;		// Start hourglass (stops when it
							// goes out of scope).

//	while ( cfile.Read(str,sizeof(str)) ) {
	while ( fgets(str,sizeof(str),fp) ) {
		CeControl control(str);
		if ( control.IsDefined() ) {
			ncontrol++;
//			if ( ncontrol>5660 ) {
//				CHARS deb[132];
//				sprintf ( deb, "%d = %s", ncontrol, str );
//				AfxMessageBox(deb);
//			}
			for ( UINT4 i=0; i<m_NumRange; i++ ) {
				if ( control.IsInRange(m_Ranges[i]) ) {
					m_Ranges[i].Insert(control);
					win.Expand(control.GetEasting(),
							   control.GetNorthing());
					nfound++;
					break;
				}
			}
		}
	}

//	Close the control file.
//	cfile.Close();
	fclose(fp);

	return TRUE;

} // end of LoadControl
         */

        /*
//	Get rid of any previously loaded ranges.

void CdGetControl::KillRanges ( void ) {

	if ( m_Ranges ) {
		for ( UINT4 i=0; i<m_NumRange; i++ )
			m_Ranges[i].Erase();
		delete [] m_Ranges;
		m_Ranges = 0;
	}

	m_NumRange = 0;
	m_NumAlloc = 0;
	m_CurrRange = 0;

} // end of KillRanges
         */

        /*
//	Redraw any control points.

void CdGetControl::OnDraw ( void ) const {

//	Return if no ranges.
	if ( !m_Ranges ) return;

//	Draw each range.
	for ( UINT4 i=0; i<m_NumRange; i++ )
		m_Ranges[i].Draw();

} // end of OnDraw
         */

        private void cancelButton_Click(object sender, EventArgs e)
        {
            // If the map initially had an undefined extent, set it back that way.
            //if (m_NewMap)
            //    CadastralMapModel.Current.Extent = 

            m_Cmd.DialAbort(this);
        }

        /*

LOGICAL CdGetControl::GetInsideWindow ( void ) {

//	Ask the user whether they want the data inside the current
//	draw window.

	if ( AfxMessageBox(
			"Load control inside current map window?",
			MB_YESNO )!=IDYES ) return FALSE;

//	Get the current display window.
	CeMap* pMap = CeMap::GetpMap();
	const CeWindow& drawin = pMap->GetDrawWin();

//	Get the name of the control file.
	CString fspec;
	GetDlgItem(IDC_CONTROL_FILE)->GetWindowText(fspec);

//	Open the control file.
	FILE* fp;
	if ( (fp=fopen((LPCTSTR)fspec,"r"))==0 ) {
		CHARS msg[256];
		sprintf ( msg, "Cannot access '%s'", fspec );
		AfxMessageBox(msg);
		return FALSE;
	}

//	Scan though the control file. For each line, try to form
//	a control object. If successful, see if it falls within
//	the current draw window.

	CHARS str[132];				// Input buffer
	UINT4 ncontrol=0;			// Number of valid control points read.
	UINT4 nfound=0;				// Number of control points found.
	CeControlRange* pRange=0;	// Current control range
	CWaitCursor wait;			// Start hourglass (stops when it
								// goes out of scope).

	while ( fgets(str,sizeof(str),fp) ) {
		CeControl control(str);
		if ( control.IsDefined() ) {
			ncontrol++;
			CeVertex pos(control.GetEasting(),
					     control.GetNorthing());
			if ( drawin.IsOverlap(pos) ) {
				nfound++;

//				If a control range is currently defined, but the
//				control point cannot be appended, close the range.
				if ( pRange && !pRange->CanAppend(control) )
					pRange = 0;

//				If there is no control range, create a new one.
				if ( !pRange ) pRange = AddRange();

//				Append the control point to the current range.
				pRange->Append(control);
			}
		}
	}

//	Close the control file.
	fclose(fp);

//	Show the results
	ShowRanges(drawin);
	if ( nfound==0 ) AfxMessageBox("No control in current window");

	return TRUE;

} // end of GetInsideWindow

         */

        /*
//	Get another control range, re-allocating if necessary.

CeControlRange*	CdGetControl::AddRange ( void ) {

//	If the current allocation is insufficient for an extra range,
//	re-allocate to a bigger size.

	if ( m_NumRange == m_NumAlloc ) {

		m_NumAlloc += 32;
		CeControlRange* pNew = new CeControlRange[m_NumAlloc];

//		Copy over all the ranges (if any) that we currently have.
		for ( UINT4 i=0; i<m_NumRange; i++ )
			pNew[i] = m_Ranges[i];

//		Get rid of the old ranges & replace with the new stuff.
		delete [] m_Ranges;
		m_Ranges = pNew;
	}

//	Increment the number of active ranges.
	m_NumRange++;

//	Return pointer to the last active range.
	return &m_Ranges[m_NumRange-1];

} // end of AddRange
         */
    }
}