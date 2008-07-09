namespace Firefly.Utility
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BatchProgressForm));
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
            resources.ApplyResources(this.cmdSaveLog, "cmdSaveLog");
            this.cmdSaveLog.Name = "cmdSaveLog";
            this.cmdSaveLog.UseVisualStyleBackColor = true;
            this.cmdSaveLog.Click += new System.EventHandler(this.cmdSaveLog_Click);
            // 
            // cmdClose
            // 
            resources.ApplyResources(this.cmdClose, "cmdClose");
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.UseVisualStyleBackColor = true;
            this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
            // 
            // stdErrWorker
            // 
            this.stdErrWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.stdErrWorker_DoWork);
            // 
            // rtfBatchProgress
            // 
            resources.ApplyResources(this.rtfBatchProgress, "rtfBatchProgress");
            this.rtfBatchProgress.Name = "rtfBatchProgress";
            // 
            // BatchProgressForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rtfBatchProgress);
            this.Controls.Add(this.cmdClose);
            this.Controls.Add(this.cmdSaveLog);
            this.Name = "BatchProgressForm";
            this.Load += new System.EventHandler(this.BatchProgressForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.ComponentModel.BackgroundWorker stdOutWorker;
        private System.Windows.Forms.Button cmdSaveLog;
        private System.Windows.Forms.Button cmdClose;
        private System.ComponentModel.BackgroundWorker stdErrWorker;
        protected System.Windows.Forms.RichTextBox rtfBatchProgress;
    }
}