namespace Backsight.Editor.Forms
{
    partial class ListOperationsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListOperationsForm));
            this.closeButton = new System.Windows.Forms.Button();
            this.smallImageList = new System.Windows.Forms.ImageList(this.components);
            this.grid = new System.Windows.Forms.DataGridView();
            this.opIdColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.operationColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.precisionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.createdColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.editorColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Location = new System.Drawing.Point(596, 137);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 2;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // smallImageList
            // 
            this.smallImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("smallImageList.ImageStream")));
            this.smallImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.smallImageList.Images.SetKeyName(0, "NoEntry");
            this.smallImageList.Images.SetKeyName(1, "Circle");
            this.smallImageList.Images.SetKeyName(2, "GrayCircle");
            this.smallImageList.Images.SetKeyName(3, "Line");
            // 
            // grid
            // 
            this.grid.AllowUserToAddRows = false;
            this.grid.AllowUserToDeleteRows = false;
            this.grid.AllowUserToResizeRows = false;
            this.grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.opIdColumn,
            this.operationColumn,
            this.precisionColumn,
            this.createdColumn,
            this.editorColumn});
            this.grid.Location = new System.Drawing.Point(12, 12);
            this.grid.Name = "grid";
            this.grid.ReadOnly = true;
            this.grid.RowHeadersVisible = false;
            this.grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grid.Size = new System.Drawing.Size(565, 147);
            this.grid.TabIndex = 3;
            // 
            // opIdColumn
            // 
            this.opIdColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.opIdColumn.HeaderText = "Op#";
            this.opIdColumn.Name = "opIdColumn";
            this.opIdColumn.ReadOnly = true;
            // 
            // operationColumn
            // 
            this.operationColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.operationColumn.FillWeight = 200F;
            this.operationColumn.HeaderText = "Operation";
            this.operationColumn.Name = "operationColumn";
            this.operationColumn.ReadOnly = true;
            // 
            // precisionColumn
            // 
            this.precisionColumn.FillWeight = 70F;
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
            // ListOperationsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 171);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.closeButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ListOperationsForm";
            this.Text = "Dependent Operations";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.ListOperationsForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.ImageList smallImageList;
        private System.Windows.Forms.DataGridView grid;
        private System.Windows.Forms.DataGridViewTextBoxColumn opIdColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn operationColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn precisionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn createdColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn editorColumn;
    }
}