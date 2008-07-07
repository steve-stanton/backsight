namespace Backsight.Editor.Forms
{
    partial class SessionForm
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
            this.closeButton = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.grid = new System.Windows.Forms.DataGridView();
            this.EditSequence = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EditType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FeatureCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.detailsButton = new System.Windows.Forms.Button();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Location = new System.Drawing.Point(373, 11);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 0;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.grid);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.detailsButton);
            this.splitContainer1.Panel2.Controls.Add(this.closeButton);
            this.splitContainer1.Size = new System.Drawing.Size(460, 262);
            this.splitContainer1.SplitterDistance = 212;
            this.splitContainer1.TabIndex = 1;
            // 
            // grid
            // 
            this.grid.AllowUserToAddRows = false;
            this.grid.AllowUserToDeleteRows = false;
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.EditSequence,
            this.EditType,
            this.FeatureCount});
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.Location = new System.Drawing.Point(0, 0);
            this.grid.Name = "grid";
            this.grid.RowHeadersVisible = false;
            this.grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grid.Size = new System.Drawing.Size(460, 212);
            this.grid.TabIndex = 0;
            this.grid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_CellDoubleClick);
            this.grid.SelectionChanged += new System.EventHandler(this.grid_SelectionChanged);
            // 
            // EditSequence
            // 
            this.EditSequence.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.EditSequence.DataPropertyName = "EditSequence";
            this.EditSequence.FillWeight = 50F;
            this.EditSequence.HeaderText = "Edit Sequence";
            this.EditSequence.Name = "EditSequence";
            this.EditSequence.ReadOnly = true;
            // 
            // EditType
            // 
            this.EditType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.EditType.DataPropertyName = "Name";
            this.EditType.HeaderText = "Edit Type";
            this.EditType.Name = "EditType";
            this.EditType.ReadOnly = true;
            // 
            // FeatureCount
            // 
            this.FeatureCount.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.FeatureCount.DataPropertyName = "FeatureCount";
            this.FeatureCount.FillWeight = 50F;
            this.FeatureCount.HeaderText = "Feature Count";
            this.FeatureCount.Name = "FeatureCount";
            this.FeatureCount.ReadOnly = true;
            // 
            // detailsButton
            // 
            this.detailsButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.detailsButton.Location = new System.Drawing.Point(12, 11);
            this.detailsButton.Name = "detailsButton";
            this.detailsButton.Size = new System.Drawing.Size(75, 23);
            this.detailsButton.TabIndex = 1;
            this.detailsButton.Text = "&Details";
            this.detailsButton.UseVisualStyleBackColor = true;
            this.detailsButton.Click += new System.EventHandler(this.detailsButton_Click);
            // 
            // SessionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 262);
            this.Controls.Add(this.splitContainer1);
            this.Name = "SessionForm";
            this.Text = "Session History";
            this.Shown += new System.EventHandler(this.SessionForm_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SessionForm_FormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView grid;
        private System.Windows.Forms.DataGridViewTextBoxColumn EditSequence;
        private System.Windows.Forms.DataGridViewTextBoxColumn EditType;
        private System.Windows.Forms.DataGridViewTextBoxColumn FeatureCount;
        private System.Windows.Forms.Button detailsButton;
    }
}