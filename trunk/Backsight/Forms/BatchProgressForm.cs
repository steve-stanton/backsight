using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Firefly.Utility
{
    public partial class BatchProgressForm : Form
    {
        private List<String> m_messages = new List<String>();

        private String m_workingDir;
        private String m_command;
        private String m_arguments;
        private String m_startMessage = "";
        private String m_endMessage = "";

        private String HeaderText { get { return this.Text; } set { this.Text = value; } }

        public BatchProgressForm() : this ("", true) {}

        public BatchProgressForm(String headerText) : this(headerText, true) {}

        // 081106 EDS #478 Add show save button option
        public BatchProgressForm(String headerText, Boolean showSaveButton)
        {
            InitializeComponent();
            this.Text = headerText;
            this.cmdSaveLog.Visible = showSaveButton;
        }

        private void BatchProgressForm_Load(object sender, EventArgs e)
        {
            try
            {
            }
            catch (FileNotFoundException fnfExc)
            {
                MessageBox.Show(String.Format("{0}: {1} {2}", fnfExc.TargetSite, fnfExc.Message, fnfExc.FileName));
            }
            catch (Exception exc)
            {
                MessageBox.Show(String.Format("{0}: {1}", exc.TargetSite, exc.Message));
            }
        }

        private void stdOutWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Run((ProcessStartInfo)e.Argument);
        }

        private void stdOutWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void stdOutWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            cmdClose.Enabled = true; 
            cmdSaveLog.Visible = true;
            rtfBatchProgress.ScrollBars = RichTextBoxScrollBars.Vertical;
        }

        private void stdErrWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Process p = (Process)e.Argument;
                String errLine;

                StreamReader sr = p.StandardError;
                while ((errLine = sr.ReadLine()) != null)
                {
                    ReportMessage(errLine);
                }
            }
            catch (FileNotFoundException fnfExc)
            {
                MessageBox.Show(String.Format("{0}: {1} {2}", fnfExc.TargetSite, fnfExc.Message, fnfExc.FileName));
            }
            catch (Exception exc)
            {
                MessageBox.Show(String.Format("{0}: {1}", exc.TargetSite, exc.Message));
            }
        }

        private void cmdSaveLog_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog dial = new SaveFileDialog();
                if (dial.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllLines(dial.FileName, m_messages.ToArray());
                }
                dial.Dispose();
            }
            catch (FileNotFoundException fnfExc)
            {
                MessageBox.Show(String.Format("{0}: {1} {2}", fnfExc.TargetSite, fnfExc.Message, fnfExc.FileName));
            }
            catch (Exception exc)
            {
                MessageBox.Show(String.Format("{0}: {1}", exc.TargetSite, exc.Message));
            }
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void ClearMessages()
        {
            rtfBatchProgress.Clear();
        }

        public void Run(String workingDir, String command, String arguments)
        {
            Run(workingDir, command, arguments, "", "");
        }

        public void Run(String workingDir, String command, String arguments, String startMessage, String endMessage)
        {
            try
            {
                m_workingDir = workingDir;
                m_command = command;
                m_arguments = arguments;

                m_startMessage = startMessage;
                m_endMessage = endMessage;

                cmdClose.Enabled = false;
                rtfBatchProgress.ScrollBars = RichTextBoxScrollBars.None;

                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.WorkingDirectory = m_workingDir;
                //processStartInfo.FileName = m_workingDir + Path.DirectorySeparatorChar + m_command;
                processStartInfo.FileName = m_command;
                processStartInfo.Arguments = m_arguments;

                processStartInfo.UseShellExecute = false;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.RedirectStandardError = true;
                processStartInfo.CreateNoWindow = true;

                ReportMessage(startMessage);
                stdOutWorker.RunWorkerAsync(processStartInfo);
            }
            catch (FileNotFoundException fnfExc)
            {
                MessageBox.Show(String.Format("{0}: {1} {2}", fnfExc.TargetSite, fnfExc.Message, fnfExc.FileName));
            }
            catch (Exception exc)
            {
                MessageBox.Show(String.Format("{0}: {1}", exc.TargetSite, exc.Message));
            }
        }

        private void Run(ProcessStartInfo psi)
        {
            try
            {
                ReportMessage(String.Format("Working Directory : {0}", psi.WorkingDirectory));
                ReportMessage(String.Format("Filename : {0}", psi.FileName));
                ReportMessage(String.Format("Arguments : {0}", psi.Arguments));

                Process p = Process.Start(psi);

                // Handle stderr in a further background thread (we're picking up both stdout &
                // stderr, and help suggests that deadlocks can arise if both are read from
                // the same thread)
                stdErrWorker.RunWorkerAsync(p);

                String outLine;
                StreamReader sr = p.StandardOutput;
                while ((outLine = sr.ReadLine()) != null)
                {
                    // Ignore blank lines
                    if (outLine.Trim().Length == 0)
                    {
                        continue;
                    }
                    ReportMessage(outLine);
                }

                p.WaitForExit();
                ReportMessage(m_endMessage);
            }
            catch (FileNotFoundException fnfExc)
            {
                MessageBox.Show(String.Format("{0}: {1} {2}", fnfExc.TargetSite, fnfExc.Message, fnfExc.FileName));
            }
            catch (Exception exc)
            {
                MessageBox.Show(String.Format("{0}: {1}", exc.TargetSite, exc.Message));
            }
        }

        delegate void DoSomething();

        public void ReportMessage(string message)
        {
            try
            {
                DoSomething d = delegate
                {
                    // A List isn't threadsafe, and this stuff may be getting called by one of
                    // two threads (I think, but the d.Invoke call complicates things -- anyway,
                    // it shouldn't hurt to lock just in case)
                    lock (m_messages)
                    {
                        m_messages.Add(message);
                        rtfBatchProgress.Lines = m_messages.ToArray();
                    }

                    // Scroll to the bottom of the output
                    rtfBatchProgress.SelectionStart = rtfBatchProgress.TextLength;
                    rtfBatchProgress.ScrollToCaret();
                };
                this.Invoke(d);
            }
            catch (FileNotFoundException fnfExc)
            {
                MessageBox.Show(String.Format("{0}: {1} {2}", fnfExc.TargetSite, fnfExc.Message, fnfExc.FileName));
            }
            catch (Exception exc)
            {
                MessageBox.Show(String.Format("{0}: {1}", exc.TargetSite, exc.Message));
            }
        }
    }
}