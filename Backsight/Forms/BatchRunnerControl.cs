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
    /// A control that executes some sort of batch script, with output
    /// (stdout and stderr) displayed as part of the control. The command
    /// script will be executed asynchronously (a sub-process spawned by
    /// a background thread). However, only one script can be executed at
    /// a time.
    /// </summary>
    public partial class BatchRunnerControl : UserControl
    {
        #region Class data

        /// <summary>
        /// The object that invoked a call to <see cref="RunCommand"/> that
        /// still has to complete (or fail). Null if nothing is currently
        /// running.
        /// </summary>
        IBatchRunner m_Runner;

        /// <summary>
        /// The name of the command passed to <see cref-"RunCommand"/> (this
        /// remains defined after the current run completes, since completion
        /// handling may need to make use of it).
        /// </summary>
        string m_CommandFile;

        /// <summary>
        /// The output messages that have been intercepted and displayed
        /// on the face of this control.
        /// </summary>
        readonly List<string> m_Messages;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor creates a control with no messages, and
        /// nothing currently running.
        /// </summary>
        public BatchRunnerControl()
        {
            InitializeComponent();
            m_Messages = new List<string>();
            m_Runner = null;
            m_CommandFile = null;
        }

        #endregion

        /// <summary>
        /// Is a command currently running?
        /// </summary>
        public bool IsBusy
        {
            get { return (m_Runner != null); }
        }

        /// <summary>
        /// Executes a command script that requires no command line arguments.
        /// </summary>
        /// <param name="runner">The object invoking the command (will be notified
        /// on completion or failure)</param>
        /// <param name="cmdFile">The command file to run</param>
        /// <exception cref="InvalidAsynchronousStateException">If a previous 
        /// run has not yet completed</exception>
        /// <exception cref="ArgumentNullException">If the supplied <paramref name="runner"/>
        /// is null</exception>
        public void RunCommand(IBatchRunner runner, string cmdFile)
        {
            RunCommand(runner, cmdFile, String.Empty);
        }

        /// <summary>
        /// Executes a command script.
        /// </summary>
        /// <param name="runner">The object invoking the command (will be notified
        /// on completion or failure)</param>
        /// <param name="cmdFile">The command file to run</param>
        /// <param name="args">Any command line arguments for the script</param>
        /// <exception cref="ArgumentNullException">If the supplied <paramref name="runner"/>
        /// is null</exception>
        /// <exception cref="InvalidAsynchronousStateException">If a previous 
        /// run has not yet completed</exception>
        public void RunCommand(IBatchRunner runner, string cmdFile, string args)
        {
            // Disallow an attempt to run something if a prior call is still executing.
            if (IsBusy)
                throw new InvalidAsynchronousStateException();

            if (runner == null)
                throw new ArgumentNullException();

            // I don't see how anything here could lead to an exception,
            // but who knows...

            try
            {
                m_Runner = runner;
                m_CommandFile = cmdFile;
                richTextBox.ScrollBars = RichTextBoxScrollBars.None;

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

            catch (Exception ex)
            {
                m_Runner = null;
                runner.RunCompleted(ex);
            }
        }

        /// <summary>
        /// The name of the command file that was supplied on the last call to
        /// <see cref="RunCommand"/> (null if never called). This remains defined
        /// after the command has completed (or failed).
        /// </summary>
        public string LastCommand
        {
            get { return m_CommandFile; }
        }

        private void stdoutWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Run((ProcessStartInfo)e.Argument);
        }

        private void stdoutWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Debug.Assert(m_Runner != null);
            richTextBox.ScrollBars = RichTextBoxScrollBars.Vertical;
            m_Runner.RunCompleted(e.Error);
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

        public void ReportMessage(string message)
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
