/// <remarks>
/// Copyright 2008 - Steve Stanton. This file is part of Backsight
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
using System.Diagnostics;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;

namespace Backsight.Forms
{
    /// <summary>
    /// A control that executes some sort of batch script, with output to a displayed list
    /// </summary>
    public partial class BatchRunnerControl : UserControl
    {
        #region Events

        /// <summary>
        /// Event fired when the command script passed to <see cref="RunCommand"/>
        /// has completed.
        /// </summary>
        public event EventHandler Completed;

        #endregion

        #region Class data

        private readonly List<string> m_Messages;
        //public event Completed;

        #endregion

        #region Constructors

        public BatchRunnerControl()
        {
            InitializeComponent();
            m_Messages = new List<string>();
        }

        #endregion

        /// <summary>
        /// Executes a command script. On completion, the <c>Completed</c> event
        /// will be fired.
        /// </summary>
        /// <param name="cmdFile">The file containing the script to run</param>
        /// <param name="args">Any command line arguments for the script</param>
        /// <exception cref="InvalidAsynchronousStateException">If the script passed
        /// via a previous call has not yet completed</exception>
        public void RunCommand(string cmdFile, string args)
        {
            // Disallow an attempt to run something if a prior call is still executing.
            if (stdoutWorker.IsBusy || stderrWorker.IsBusy)
                throw new InvalidAsynchronousStateException();

            richTextBox.ScrollBars = RichTextBoxScrollBars.None;

            string workingDir = Path.GetTempPath();

            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.WorkingDirectory = Path.GetDirectoryName(cmdFile);
            processStartInfo.FileName = cmdFile;
            processStartInfo.Arguments = args;

            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.CreateNoWindow = true;

            stdoutWorker.RunWorkerAsync(processStartInfo);
        }

        private void stdoutWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Run((ProcessStartInfo)e.Argument);
        }

        private void stdoutWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            richTextBox.ScrollBars = RichTextBoxScrollBars.Vertical;
            if (Completed != null)
                Completed(this, e);
        }

        private void stderrWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Process p = (Process)e.Argument;
            String errLine;

            StreamReader sr = p.StandardError;
            while ((errLine = sr.ReadLine()) != null)
                ReportMessage(errLine);
        }

        private void Run(ProcessStartInfo psi)
        {
            Process p = Process.Start(psi);

            // Handle stderr in a further background thread (we're picking up both stdout &
            // stderr, and help suggests that deadlocks can arise if both are read from
            // the same thread)
            stderrWorker.RunWorkerAsync(p);

            String outLine;
            StreamReader sr = p.StandardOutput;
            while ((outLine = sr.ReadLine()) != null)
                ReportMessage(outLine);

            p.WaitForExit();
        }

        delegate void DoSomething();

        void ReportMessage(string message)
        {
            DoSomething d = delegate
            {
                // A List isn't threadsafe, and this stuff may be getting called by one of
                // two threads (I think, but the d.Invoke call complicates things -- anyway,
                // it shouldn't hurt to lock just in case)
                lock (m_Messages)
                {
                    m_Messages.Add(message);
                    richTextBox.Lines = m_Messages.ToArray();
                }

                // Scroll to the bottom of the output
                richTextBox.SelectionStart = richTextBox.TextLength;
                richTextBox.ScrollToCaret();
            };
            this.Invoke(d);
        }

        /// <summary>
        /// Clears the output of any previous call to <see cref="RunCommand"/>
        /// </summary>
        public void ClearMessages()
        {
            richTextBox.Clear();
            m_Messages.Clear();
        }

    }
}
