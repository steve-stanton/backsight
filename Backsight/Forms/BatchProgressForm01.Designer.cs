namespace CommonInstaller
{
    partial class BatchProgressForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.stdOutWorker = new System.ComponentModel.BackgroundWorker();
            this.cmdSaveLog = new System.Windows.Forms.Button();
            this.cmdClose = new System.Windows.Forms.Button();
            this.stdErrWorker = new System.ComponentModel.BackgroundWorker();
            this.rtfBatchProgress = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // stdOutWorker
            // 
            this.stdOutWorker.WorkerReportsProgress = true;
            this.stdOutWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.stdOutWorker_DoWork);
            this.stdOutWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.stdOutWorker_RunWorkerCompleted);
            this.stdOutWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.stdOutWorker_ProgressChanged);
            // 
            // cmdSaveLog
            // 
            this.cmdSaveLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdSaveLog.Location = new System.Drawing.Point(616, 462);
            this.cmdSaveLog.Name = "cmdSaveLog";
            this.cmdSaveLog.Size = new System.Drawing.Size(75, 23);
            this.cmdSaveLog.TabIndex = 2;
            this.cmdSaveLog.Text = "&Save Log...";
            this.cmdSaveLog.UseVisualStyleBackColor = true;
            this.cmdSaveLog.Visible = false;
            this.cmdSaveLog.Click += new System.EventHandler(this.cmdSaveLog_Click);
            // 
            // cmdClose
            // 
            this.cmdClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdClose.Location = new System.Drawing.Point(697, 462);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(75, 23);
            this.cmdClose.TabIndex = 3;
            this.cmdClose.Text = "&Close";
            this.cmdClose.UseVisualStyleBackColor = true;
            this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
            // 
            // stdErrWorker
            // 
            this.stdErrWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.stdErrWorker_DoWork);
            // 
            // rtfBatchProgress
            // 
            this.rtfBatchProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtfBatchProgress.Location = new System.Drawing.Point(-1, -2);
            this.rtfBatchProgress.Name = "rtfBatchProgress";
            this.rtfBatchProgress.ReadOnly = true;
            this.rtfBatchProgress.Size = new System.Drawing.Size(786, 458);
            this.rtfBatchProgress.TabIndex = 1;
            this.rtfBatchProgress.Text = "";
            // 
            // BatchProgressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 497);
            this.Controls.Add(this.rtfBatchProgress);
            this.Controls.Add(this.cmdClose);
            this.Controls.Add(this.cmdSaveLog);
            this.Name = "BatchProgressForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BatchProgressForm";
            this.Load += new System.EventHandler(this.BatchProgressForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.ComponentModel.BackgroundWorker stdOutWorker;
        private System.Windows.Forms.Button cmdSaveLog;
        private System.Windows.Forms.Button cmdClose;
        private System.ComponentModel.BackgroundWorker stdErrWorker;
        private System.Windows.Forms.RichTextBox rtfBatchProgress;
    }
}