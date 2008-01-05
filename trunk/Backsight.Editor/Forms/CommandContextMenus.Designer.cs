namespace Backsight.Editor.Forms
{
    partial class CommandContextMenus
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
            this.components = new System.ComponentModel.Container();
            this.newArcContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxNewArcSpecifyId = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator33 = new System.Windows.Forms.ToolStripSeparator();
            this.ctxNewArcShortArc = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxNewArcLongArc = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator34 = new System.Windows.Forms.ToolStripSeparator();
            this.ctxNewArcCancel = new System.Windows.Forms.ToolStripMenuItem();
            this.newLineContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxNewLineSpecifyId = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator32 = new System.Windows.Forms.ToolStripSeparator();
            this.ctxNewLineCancel = new System.Windows.Forms.ToolStripMenuItem();
            this.newArcContextMenu.SuspendLayout();
            this.newLineContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // newArcContextMenu
            // 
            this.newArcContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxNewArcSpecifyId,
            this.toolStripSeparator33,
            this.ctxNewArcShortArc,
            this.ctxNewArcLongArc,
            this.toolStripSeparator34,
            this.ctxNewArcCancel});
            this.newArcContextMenu.Name = "newArcContextMenu";
            this.newArcContextMenu.ShowImageMargin = false;
            this.newArcContextMenu.Size = new System.Drawing.Size(108, 104);
            // 
            // ctxNewArcSpecifyId
            // 
            this.ctxNewArcSpecifyId.Name = "ctxNewArcSpecifyId";
            this.ctxNewArcSpecifyId.Size = new System.Drawing.Size(127, 22);
            this.ctxNewArcSpecifyId.Text = "Specify ID...";
            // 
            // toolStripSeparator33
            // 
            this.toolStripSeparator33.Name = "toolStripSeparator33";
            this.toolStripSeparator33.Size = new System.Drawing.Size(124, 6);
            // 
            // ctxNewArcShortArc
            // 
            this.ctxNewArcShortArc.Name = "ctxNewArcShortArc";
            this.ctxNewArcShortArc.Size = new System.Drawing.Size(127, 22);
            this.ctxNewArcShortArc.Text = "Short arc";
            // 
            // ctxNewArcLongArc
            // 
            this.ctxNewArcLongArc.Name = "ctxNewArcLongArc";
            this.ctxNewArcLongArc.Size = new System.Drawing.Size(127, 22);
            this.ctxNewArcLongArc.Text = "Long arc";
            // 
            // toolStripSeparator34
            // 
            this.toolStripSeparator34.Name = "toolStripSeparator34";
            this.toolStripSeparator34.Size = new System.Drawing.Size(124, 6);
            // 
            // ctxNewArcCancel
            // 
            this.ctxNewArcCancel.Name = "ctxNewArcCancel";
            this.ctxNewArcCancel.Size = new System.Drawing.Size(127, 22);
            this.ctxNewArcCancel.Text = "Cancel";
            // 
            // newLineContextMenu
            // 
            this.newLineContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxNewLineSpecifyId,
            this.toolStripSeparator32,
            this.ctxNewLineCancel});
            this.newLineContextMenu.Name = "newLineContextMenu";
            this.newLineContextMenu.ShowImageMargin = false;
            this.newLineContextMenu.Size = new System.Drawing.Size(128, 76);
            // 
            // ctxNewLineSpecifyId
            // 
            this.ctxNewLineSpecifyId.Name = "ctxNewLineSpecifyId";
            this.ctxNewLineSpecifyId.Size = new System.Drawing.Size(127, 22);
            this.ctxNewLineSpecifyId.Text = "Specify ID...";
            // 
            // toolStripSeparator32
            // 
            this.toolStripSeparator32.Name = "toolStripSeparator32";
            this.toolStripSeparator32.Size = new System.Drawing.Size(124, 6);
            // 
            // ctxNewLineCancel
            // 
            this.ctxNewLineCancel.Name = "ctxNewLineCancel";
            this.ctxNewLineCancel.Size = new System.Drawing.Size(127, 22);
            this.ctxNewLineCancel.Text = "Cancel";
            // 
            // CommandContextMenus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "CommandContextMenus";
            this.newArcContextMenu.ResumeLayout(false);
            this.newLineContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip newArcContextMenu;
        private System.Windows.Forms.ToolStripMenuItem ctxNewArcSpecifyId;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator33;
        private System.Windows.Forms.ToolStripMenuItem ctxNewArcShortArc;
        private System.Windows.Forms.ToolStripMenuItem ctxNewArcLongArc;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator34;
        private System.Windows.Forms.ToolStripMenuItem ctxNewArcCancel;
        private System.Windows.Forms.ContextMenuStrip newLineContextMenu;
        private System.Windows.Forms.ToolStripMenuItem ctxNewLineSpecifyId;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator32;
        private System.Windows.Forms.ToolStripMenuItem ctxNewLineCancel;
    }
}
