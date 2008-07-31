namespace Backsight.Forms
{
    partial class BatchRunnerControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.stdoutWorker = new System.ComponentModel.BackgroundWorker();
            this.stderrWorker = new System.ComponentModel.BackgroundWorker();
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // stdoutWorker
            // 
            this.stdoutWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.stdoutWorker_DoWork);
            this.stdoutWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.stdoutWorker_RunWorkerCompleted);
            // 
            // stderrWorker
            // 
            this.stderrWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.stderrWorker_DoWork);
            // 
            // richTextBox
            // 
            this.richTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.richTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox.Location = new System.Drawing.Point(0, 0);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.ReadOnly = true;
            this.richTextBox.Size = new System.Drawing.Size(449, 199);
            this.richTextBox.TabIndex = 0;
            this.richTextBox.Text = "";
            // 
            // BatchRunnerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.richTextBox);
            this.Name = "BatchRunnerControl";
            this.Size = new System.Drawing.Size(449, 199);
            this.ResumeLayout(false);

        }

        #endregion

        private System.ComponentModel.BackgroundWorker stdoutWorker;
        private System.ComponentModel.BackgroundWorker stderrWorker;
        private System.Windows.Forms.RichTextBox richTextBox;
    }
}
