namespace Backsight.Environment.Editor
{
    partial class PropertyListForm
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.propertyGrid = new System.Windows.Forms.DataGridView();
            this.updateButton = new System.Windows.Forms.Button();
            this.newButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.dgcName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.propertyGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.propertyGrid);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.closeButton);
            this.splitContainer1.Panel2.Controls.Add(this.updateButton);
            this.splitContainer1.Panel2.Controls.Add(this.newButton);
            this.splitContainer1.Panel2.Controls.Add(this.deleteButton);
            this.splitContainer1.Size = new System.Drawing.Size(538, 358);
            this.splitContainer1.SplitterDistance = 415;
            this.splitContainer1.TabIndex = 0;
            // 
            // propertyGrid
            // 
            this.propertyGrid.AllowUserToAddRows = false;
            this.propertyGrid.AllowUserToDeleteRows = false;
            this.propertyGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.propertyGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgcName,
            this.dgcValue});
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid.MultiSelect = false;
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.ReadOnly = true;
            this.propertyGrid.RowHeadersVisible = false;
            this.propertyGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.propertyGrid.Size = new System.Drawing.Size(415, 358);
            this.propertyGrid.TabIndex = 0;
            this.propertyGrid.DoubleClick += new System.EventHandler(this.propertyGrid_DoubleClick);
            // 
            // updateButton
            // 
            this.updateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.updateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.updateButton.Location = new System.Drawing.Point(18, 59);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(75, 23);
            this.updateButton.TabIndex = 12;
            this.updateButton.Text = "&Update";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // newButton
            // 
            this.newButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.newButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newButton.Location = new System.Drawing.Point(18, 30);
            this.newButton.Name = "newButton";
            this.newButton.Size = new System.Drawing.Size(75, 23);
            this.newButton.TabIndex = 10;
            this.newButton.Text = "&New";
            this.newButton.UseVisualStyleBackColor = true;
            this.newButton.Click += new System.EventHandler(this.newButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deleteButton.Location = new System.Drawing.Point(18, 88);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(75, 23);
            this.deleteButton.TabIndex = 11;
            this.deleteButton.Text = "&Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closeButton.Location = new System.Drawing.Point(18, 311);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 13;
            this.closeButton.Text = "&Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // dgcName
            // 
            this.dgcName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgcName.DataPropertyName = "Name";
            this.dgcName.FillWeight = 30F;
            this.dgcName.HeaderText = "Property";
            this.dgcName.Name = "dgcName";
            this.dgcName.ReadOnly = true;
            // 
            // dgcValue
            // 
            this.dgcValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgcValue.DataPropertyName = "Value";
            this.dgcValue.FillWeight = 70F;
            this.dgcValue.HeaderText = "Value";
            this.dgcValue.Name = "dgcValue";
            this.dgcValue.ReadOnly = true;
            // 
            // PropertyListForm
            // 
            this.AcceptButton = this.closeButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(538, 358);
            this.Controls.Add(this.splitContainer1);
            this.Name = "PropertyListForm";
            this.Text = "Properties";
            this.Shown += new System.EventHandler(this.PropertyListForm_Shown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.propertyGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView propertyGrid;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.Button newButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcValue;
    }
}