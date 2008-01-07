namespace Backsight.Editor.Forms
{
    partial class NewCircularArcContextMenu
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
            this.ctxCancel = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxSpecifyId = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ctxShortArc = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxLongArc = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.SuspendLayout();
            // 
            // ctxCancel
            // 
            this.ctxCancel.Name = "ctxCancel";
            this.ctxCancel.Size = new System.Drawing.Size(133, 22);
            this.ctxCancel.Text = "Cancel";
            // 
            // ctxSpecifyId
            // 
            this.ctxSpecifyId.Name = "ctxSpecifyId";
            this.ctxSpecifyId.Size = new System.Drawing.Size(133, 22);
            this.ctxSpecifyId.Text = "Specify ID...";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(130, 6);
            // 
            // ctxShortArc
            // 
            this.ctxShortArc.Name = "ctxShortArc";
            this.ctxShortArc.Size = new System.Drawing.Size(133, 22);
            this.ctxShortArc.Text = "Short arc";
            // 
            // ctxLongArc
            // 
            this.ctxLongArc.Name = "ctxLongArc";
            this.ctxLongArc.Size = new System.Drawing.Size(133, 22);
            this.ctxLongArc.Text = "Long arc";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 6);
            // 
            // NewCircularArcContextMenu
            // 
            this.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxSpecifyId,
            this.toolStripSeparator1,
            this.ctxShortArc,
            this.ctxLongArc,
            this.toolStripSeparator2,
            this.ctxCancel});
            this.ShowImageMargin = false;
            this.Size = new System.Drawing.Size(134, 98);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripMenuItem ctxCancel;
        private System.Windows.Forms.ToolStripMenuItem ctxSpecifyId;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem ctxShortArc;
        private System.Windows.Forms.ToolStripMenuItem ctxLongArc;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}