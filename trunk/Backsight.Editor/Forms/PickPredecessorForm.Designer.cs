namespace Backsight.Editor.Forms
{
    partial class PickPredecessorForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PickPredecessorForm));
            this.okButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.smallImageList = new System.Windows.Forms.ImageList(this.components);
            this.lineGrid = new System.Windows.Forms.DataGridView();
            this.imageColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.opNumberColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.operationColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.precisionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.createdColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.editorColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.lineGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(501, 53);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Location = new System.Drawing.Point(501, 92);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 2;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            // 
            // smallImageList
            // 
            this.smallImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("smallImageList.ImageStream")));
            this.smallImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.smallImageList.Images.SetKeyName(0, "NoEntryIcon.ico");
            this.smallImageList.Images.SetKeyName(1, "CircleIcon.ico");
            this.smallImageList.Images.SetKeyName(2, "GrayCircleIcon.ico");
            this.smallImageList.Images.SetKeyName(3, "LineIcon.ico");
            // 
            // lineGrid
            // 
            this.lineGrid.AllowUserToAddRows = false;
            this.lineGrid.AllowUserToDeleteRows = false;
            this.lineGrid.AllowUserToResizeRows = false;
            this.lineGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lineGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.lineGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.imageColumn,
            this.opNumberColumn,
            this.operationColumn,
            this.precisionColumn,
            this.createdColumn,
            this.editorColumn});
            this.lineGrid.Location = new System.Drawing.Point(12, 12);
            this.lineGrid.Name = "lineGrid";
            this.lineGrid.ReadOnly = true;
            this.lineGrid.RowHeadersVisible = false;
            this.lineGrid.Size = new System.Drawing.Size(465, 151);
            this.lineGrid.TabIndex = 3;
            // 
            // imageColumn
            // 
            this.imageColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.imageColumn.FillWeight = 50F;
            this.imageColumn.HeaderText = "";
            this.imageColumn.Name = "imageColumn";
            this.imageColumn.ReadOnly = true;
            // 
            // opNumberColumn
            // 
            this.opNumberColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.opNumberColumn.HeaderText = "Op#";
            this.opNumberColumn.Name = "opNumberColumn";
            this.opNumberColumn.ReadOnly = true;
            // 
            // operationColumn
            // 
            this.operationColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.operationColumn.FillWeight = 150F;
            this.operationColumn.HeaderText = "Operation";
            this.operationColumn.Name = "operationColumn";
            this.operationColumn.ReadOnly = true;
            // 
            // precisionColumn
            // 
            this.precisionColumn.FillWeight = 90F;
            this.precisionColumn.HeaderText = "Precision";
            this.precisionColumn.Name = "precisionColumn";
            this.precisionColumn.ReadOnly = true;
            // 
            // createdColumn
            // 
            this.createdColumn.FillWeight = 120F;
            this.createdColumn.HeaderText = "Created";
            this.createdColumn.Name = "createdColumn";
            this.createdColumn.ReadOnly = true;
            // 
            // editorColumn
            // 
            this.editorColumn.FillWeight = 145F;
            this.editorColumn.HeaderText = "Editor";
            this.editorColumn.Name = "editorColumn";
            this.editorColumn.ReadOnly = true;
            // 
            // PickPredecessorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(598, 175);
            this.Controls.Add(this.lineGrid);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "PickPredecessorForm";
            this.Text = "Predecessor Operations";
            ((System.ComponentModel.ISupportInitialize)(this.lineGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.ImageList smallImageList;
        private System.Windows.Forms.DataGridView lineGrid;
        private System.Windows.Forms.DataGridViewImageColumn imageColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn opNumberColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn operationColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn precisionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn createdColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn editorColumn;
    }
}