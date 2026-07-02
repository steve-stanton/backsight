// <remarks>
// Copyright 2008 - Steve Stanton. This file is part of Backsight
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

using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Backsight.Editor.Forms;

/// <remarks>
/// See http://www.codeproject.com/KB/cs/prettygoodsplashscreen.aspx.
/// The photo image comes from http://www.old-picture.com/old-west/Surveying-Corps.htm
/// </remarks>
public class SplashScreen : System.Windows.Forms.Form
{
	/// <summary>
	/// Information about the project being opened while splash screen displayed
	/// </summary>
	static string ms_Increment = null;
	static string ms_Percents = null;

	// Threading
	static SplashScreen ms_frmSplash = null;
	static Thread ms_oThread = null;

	// Fade in and out.
	private double m_dblOpacityIncrement = .05;
	private double m_dblOpacityDecrement = .08;
	private const int TIMER_INTERVAL = 50;

	// Status and progress bar
	static string ms_sStatus;
	private double m_dblCompletionFraction = 0;
	private Rectangle m_rProgress;

	// Progress smoothing
	private double m_dblLastCompletionFraction = 0.0;
	private double m_dblPBIncrementPerTimerInterval = .015;

	// Self-calibration support
	private bool m_bFirstLaunch = false;
	private DateTime m_dtStart;
	private bool m_bDTSet = false;
	private int m_iIndex = 1;
	private int m_iActualTicks = 0;
	private ArrayList m_alPreviousCompletionFraction;
	private ArrayList m_alActualTimes = new ArrayList();
	//private const string REG_KEY_INITIALIZATION = "Initialization";
	public const string REGVALUE_PB_MILISECOND_INCREMENT = "Increment";
	public const string REGVALUE_PB_PERCENTS = "Percents";

	private System.Windows.Forms.Label lblStatus;
	private System.Windows.Forms.Label lblTimeRemaining;
	private System.Windows.Forms.Timer timer1;
	private System.Windows.Forms.Panel pnlStatus;
	private Label label2;
	private Label label1;
	private PictureBox pictureBox;
	private Panel panel1;
	private Label label3;
	private Panel panel2;
	private System.ComponentModel.IContainer components;

	/// <summary>
	/// Constructor
	/// </summary>
	public SplashScreen()
	{
		InitializeComponent();
		this.Opacity = .00;
		timer1.Interval = TIMER_INTERVAL;
		timer1.Start();
		//this.ClientSize = this.BackgroundImage.Size;
	}

	/// <summary>
	/// Clean up any resources being used.
	/// </summary>
	protected override void Dispose( bool disposing )
	{
		if( disposing )
		{
			if(components != null)
			{
				components.Dispose();
			}
		}
		base.Dispose( disposing );
	}

	#region Windows Form Designer generated code

	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{
		components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashScreen));
		lblStatus = new System.Windows.Forms.Label();
		pnlStatus = new System.Windows.Forms.Panel();
		lblTimeRemaining = new System.Windows.Forms.Label();
		timer1 = new System.Windows.Forms.Timer(components);
		label2 = new System.Windows.Forms.Label();
		label1 = new System.Windows.Forms.Label();
		pictureBox = new System.Windows.Forms.PictureBox();
		panel1 = new System.Windows.Forms.Panel();
		label3 = new System.Windows.Forms.Label();
		panel2 = new System.Windows.Forms.Panel();
		((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
		panel1.SuspendLayout();
		SuspendLayout();
		// 
		// lblStatus
		// 
		lblStatus.BackColor = System.Drawing.Color.Transparent;
		lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)0));
		lblStatus.Location = new System.Drawing.Point(24, 531);
		lblStatus.Name = "lblStatus";
		lblStatus.Size = new System.Drawing.Size(449, 21);
		lblStatus.TabIndex = 0;
		lblStatus.DoubleClick += SplashScreen_DoubleClick;
		// 
		// pnlStatus
		// 
		pnlStatus.BackColor = System.Drawing.Color.Transparent;
		pnlStatus.Location = new System.Drawing.Point(24, 557);
		pnlStatus.Name = "pnlStatus";
		pnlStatus.Size = new System.Drawing.Size(449, 37);
		pnlStatus.TabIndex = 1;
		pnlStatus.Paint += pnlStatus_Paint;
		pnlStatus.DoubleClick += SplashScreen_DoubleClick;
		// 
		// lblTimeRemaining
		// 
		lblTimeRemaining.BackColor = System.Drawing.Color.Transparent;
		lblTimeRemaining.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)0));
		lblTimeRemaining.Location = new System.Drawing.Point(20, 598);
		lblTimeRemaining.Name = "lblTimeRemaining";
		lblTimeRemaining.Size = new System.Drawing.Size(453, 25);
		lblTimeRemaining.TabIndex = 2;
		lblTimeRemaining.Text = "Time remaining";
		lblTimeRemaining.Visible = false;
		lblTimeRemaining.DoubleClick += SplashScreen_DoubleClick;
		// 
		// timer1
		// 
		timer1.Tick += timer1_Tick;
		// 
		// label2
		// 
		label2.AutoSize = true;
		label2.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)0));
		label2.ForeColor = System.Drawing.Color.Black;
		label2.Location = new System.Drawing.Point(18, 60);
		label2.Name = "label2";
		label2.Size = new System.Drawing.Size(189, 28);
		label2.TabIndex = 4;
		label2.Text = "Cadastral Editor";
		// 
		// label1
		// 
		label1.AutoSize = true;
		label1.Font = new System.Drawing.Font("Arial Black", 9.75F, ((System.Drawing.FontStyle)(System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic)), System.Drawing.GraphicsUnit.Point, ((byte)0));
		label1.ForeColor = System.Drawing.Color.Firebrick;
		label1.Location = new System.Drawing.Point(20, 32);
		label1.Name = "label1";
		label1.Size = new System.Drawing.Size(102, 24);
		label1.TabIndex = 3;
		label1.Text = "Backsight";
		// 
		// pictureBox
		// 
		pictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
		pictureBox.Image = ((System.Drawing.Image)resources.GetObject("pictureBox.Image"));
		pictureBox.Location = new System.Drawing.Point(24, 128);
		pictureBox.Name = "pictureBox";
		pictureBox.Size = new System.Drawing.Size(393, 309);
		pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		pictureBox.TabIndex = 6;
		pictureBox.TabStop = false;
		// 
		// panel1
		// 
		panel1.BackColor = System.Drawing.Color.FromArgb(((int)((byte)192)), ((int)((byte)64)), ((int)((byte)0)));
		panel1.Controls.Add(label3);
		panel1.Location = new System.Drawing.Point(74, 197);
		panel1.Name = "panel1";
		panel1.Size = new System.Drawing.Size(399, 314);
		panel1.TabIndex = 7;
		// 
		// label3
		// 
		label3.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
		label3.AutoSize = true;
		label3.BackColor = System.Drawing.Color.FromArgb(((int)((byte)192)), ((int)((byte)64)), ((int)((byte)0)));
		label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)0));
		label3.ForeColor = System.Drawing.Color.DarkKhaki;
		label3.Location = new System.Drawing.Point(227, 266);
		label3.Name = "label3";
		label3.Size = new System.Drawing.Size(135, 20);
		label3.TabIndex = 3;
		label3.Text = "by Steve Stanton";
		// 
		// panel2
		// 
		panel2.BackColor = System.Drawing.Color.DimGray;
		panel2.Location = new System.Drawing.Point(24, 100);
		panel2.Name = "panel2";
		panel2.Size = new System.Drawing.Size(455, 8);
		panel2.TabIndex = 8;
		// 
		// SplashScreen
		// 
		AutoScaleBaseSize = new System.Drawing.Size(7, 20);
		BackColor = System.Drawing.Color.DarkKhaki;
		ClientSize = new System.Drawing.Size(506, 552);
		Controls.Add(panel2);
		Controls.Add(pictureBox);
		Controls.Add(label2);
		Controls.Add(label1);
		Controls.Add(lblTimeRemaining);
		Controls.Add(pnlStatus);
		Controls.Add(lblStatus);
		Controls.Add(panel1);
		FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
		ShowInTaskbar = false;
		StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		Text = "SplashScreen";
		DoubleClick += SplashScreen_DoubleClick;
		((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
		panel1.ResumeLayout(false);
		panel1.PerformLayout();
		ResumeLayout(false);
		PerformLayout();
	}
	#endregion

	// ************* Static Methods *************** //

	// A static method to create the thread and 
	// launch the SplashScreen.
	static public void ShowSplashScreen(string increment, string percents)
	{
		// Remember information about the project that's being loaded
		if (String.IsNullOrEmpty(increment))
			ms_Increment = "0.0015";
		else
			ms_Increment = increment;

		if (String.IsNullOrEmpty(percents))
			ms_Percents = String.Empty;
		else
			ms_Percents = percents;

		// Make sure it's only launched once.
		if( ms_frmSplash != null )
			return;

		// SS: Create the form in the calling thread?
		//ms_frmSplash = new SplashScreen();

		ms_oThread = new Thread( new ThreadStart(SplashScreen.ShowForm));
		ms_oThread.IsBackground = true;
		//ms_oThread.ApartmentState = ApartmentState.STA;
		ms_oThread.SetApartmentState(ApartmentState.STA);
		ms_oThread.Start();
	}

	// A property returning the splash screen instance
	static public SplashScreen SplashForm 
	{
		get
		{
			return ms_frmSplash;
		} 
	}

	// A private entry point for the thread.
	static private void ShowForm()
	{
		ms_frmSplash = new SplashScreen();
		//ms_frmSplash.Owner = ms_ParentForm;
		Application.Run(ms_frmSplash);
		//ms_frmSplash.Show();
		//Application.DoEvents();
	}

	// A static method to close the SplashScreen
	static public void CloseForm()
	{
		if( ms_frmSplash != null && ms_frmSplash.IsDisposed == false )
		{
			// Make it start going away.
			ms_frmSplash.m_dblOpacityIncrement = - ms_frmSplash.m_dblOpacityDecrement;
		}

		// SS: Ensure the correct main form comes to the fore when the splash screen
		// finally fades away. Do it here to avoid cross-threading error. I tried doing
		// this on creation of the instance, but for some reason, the splash screen
		// doesn't show up in that case. Doing it here just works.
		//ms_frmSplash.Owner = ms_ParentForm;

		ms_oThread = null;	// we don't need these any more.
		ms_frmSplash = null;
	}

	// A static method to set the status and update the reference.
	static public void SetStatus(string newStatus)
	{
		SetStatus(newStatus, true);
	}

	// A static method to set the status and optionally update the reference.
	// This is useful if you are in a section of code that has a variable
	// set of status string updates.  In that case, don't set the reference.
	static public void SetStatus(string newStatus, bool setReference)
	{
		ms_sStatus = newStatus;
		if( ms_frmSplash == null )
			return;
		if( setReference )
			ms_frmSplash.SetReferenceInternal();
	}

	// Static method called from the initializing application to 
	// give the splash screen reference points.  Not needed if
	// you are using a lot of status strings.
	static public void SetReferencePoint()
	{
		if( ms_frmSplash == null )
			return;
		ms_frmSplash.SetReferenceInternal();

	}

	// ************ Private methods ************

	// Internal method for setting reference points.
	private void SetReferenceInternal()
	{
		if( m_bDTSet == false )
		{
			m_bDTSet = true;
			m_dtStart = DateTime.Now;
			ReadIncrements();
		}
		double dblMilliseconds = ElapsedMilliSeconds();
		m_alActualTimes.Add(dblMilliseconds);
		m_dblLastCompletionFraction = m_dblCompletionFraction;
		if( m_alPreviousCompletionFraction != null && m_iIndex < m_alPreviousCompletionFraction.Count )
			m_dblCompletionFraction = (double)m_alPreviousCompletionFraction[m_iIndex++];
		else
			m_dblCompletionFraction = ( m_iIndex > 0 )? 1: 0;
	}

	// Utility function to return elapsed Milliseconds since the 
	// SplashScreen was launched.
	private double ElapsedMilliSeconds()
	{
		TimeSpan ts = DateTime.Now - m_dtStart;
		return ts.TotalMilliseconds;
	}

	// Function to read the checkpoint intervals from the previous invocation of the
	// splashscreen from the registry.
	private void ReadIncrements()
	{
		string sPBIncrementPerTimerInterval = ms_Increment;
		double dblResult;

		if( Double.TryParse(sPBIncrementPerTimerInterval, System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo, out dblResult) == true )
			m_dblPBIncrementPerTimerInterval = dblResult;
		else
			m_dblPBIncrementPerTimerInterval = .0015;

		string sPBPreviousPctComplete = ms_Percents;

		if( sPBPreviousPctComplete != "" )
		{
			string [] aTimes = sPBPreviousPctComplete.Split(null);
			m_alPreviousCompletionFraction = new ArrayList();

			for(int i = 0; i < aTimes.Length; i++ )
			{
				double dblVal;
				if( Double.TryParse(aTimes[i], System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo, out dblVal) )
					m_alPreviousCompletionFraction.Add(dblVal);
				else
					m_alPreviousCompletionFraction.Add(1.0);
			}
		}
		else
		{
			m_bFirstLaunch = true;
			//lblTimeRemaining.Text = "";
		}
	}

	// Method to store the intervals (in percent complete) from the current invocation of
	// the splash screen to the registry.
	/*
	private void StoreIncrements()
	{
		string sPercent = "";
		double dblElapsedMilliseconds = ElapsedMilliSeconds();
		for( int i = 0; i < m_alActualTimes.Count; i++ )
			sPercent += ((double)m_alActualTimes[i]/dblElapsedMilliseconds).ToString("0.####", System.Globalization.NumberFormatInfo.InvariantInfo) + " ";

	    SetPercents(sPercent);

		m_dblPBIncrementPerTimerInterval = 1.0/(double)m_iActualTicks;
	    string sIncrement = m_dblPBIncrementPerTimerInterval.ToString("#.000000", System.Globalization.NumberFormatInfo.InvariantInfo);
	    SetIncrement(sIncrement);
	}
	*/

	//********* Event Handlers ************

	// Tick Event handler for the Timer control.  Handle fade in and fade out.  Also
	// handle the smoothed progress bar.
	private void timer1_Tick(object sender, System.EventArgs e)
	{
		lblStatus.Text = ms_sStatus;

		if( m_dblOpacityIncrement > 0 )
		{
			m_iActualTicks++;
			if( this.Opacity < 1 )
				this.Opacity += m_dblOpacityIncrement;
		}
		else
		{
			if( this.Opacity > 0 )
				this.Opacity += m_dblOpacityIncrement;
			else
			{
				//StoreIncrements();
				this.Close();

				// SS: Avoid cross-thread call
				//Debug.WriteLine("Called this.Close()");
			}
		}
		if( m_bFirstLaunch == false && m_dblLastCompletionFraction < m_dblCompletionFraction )
		{
			m_dblLastCompletionFraction += m_dblPBIncrementPerTimerInterval;
			int width = (int)Math.Floor(pnlStatus.ClientRectangle.Width * m_dblLastCompletionFraction);
			int height = pnlStatus.ClientRectangle.Height;
			int x = pnlStatus.ClientRectangle.X;
			int y = pnlStatus.ClientRectangle.Y;
			if( width > 0 && height > 0 )
			{
				m_rProgress = new Rectangle( x, y, width, height);
				pnlStatus.Invalidate(m_rProgress);
				int iSecondsLeft = 1 + (int)(TIMER_INTERVAL * ((1.0 - m_dblLastCompletionFraction)/m_dblPBIncrementPerTimerInterval)) / 1000;

				if (iSecondsLeft > 1 && !lblTimeRemaining.Visible)
					lblTimeRemaining.Visible = true;

				if( iSecondsLeft == 1 )
					lblTimeRemaining.Text = string.Format( "1 second remaining");
				else
					lblTimeRemaining.Text = string.Format( "{0} seconds remaining", iSecondsLeft);
			}
		}
	}

	// Paint the portion of the panel invalidated during the tick event.
	private void pnlStatus_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
	{
		if (m_rProgress.Width == 0 && m_rProgress.Height == 0)
			return;

		if( m_bFirstLaunch == false && e.ClipRectangle.Width > 0 && m_iActualTicks > 1 )
		{
			// SS: Originally a bluish gradient, but greeny-yellow works better with my background
			Color dark = Color.FromArgb(84, 155, 66); //(58, 96, 151);
			Color light = Color.FromArgb(232, 254, 107); //(181, 237, 254);
			LinearGradientBrush brBackground = new LinearGradientBrush(m_rProgress, dark, light, LinearGradientMode.Horizontal);
			e.Graphics.FillRectangle(brBackground, m_rProgress);
		}
	}

	// Close the form if they double click on it.
	private void SplashScreen_DoubleClick(object sender, System.EventArgs e)
	{
		CloseForm();
	}

	internal string GetIncrement()
	{
		m_dblPBIncrementPerTimerInterval = 1.0/(double)m_iActualTicks;
		return m_dblPBIncrementPerTimerInterval.ToString("#.000000", System.Globalization.NumberFormatInfo.InvariantInfo);
	}

	internal string GetPercents()
	{
		string sPercent = "";
		double dblElapsedMilliseconds = ElapsedMilliSeconds();
		for (int i = 0; i < m_alActualTimes.Count; i++)
			sPercent += ((double)m_alActualTimes[i]/dblElapsedMilliseconds).ToString("0.####", System.Globalization.NumberFormatInfo.InvariantInfo) + " ";

		return sPercent;
	}
}