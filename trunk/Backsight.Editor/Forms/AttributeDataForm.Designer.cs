namespace Backsight.Editor.Forms
{
    partial class AttributeDataForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.domainGrid = new System.Windows.Forms.DataGridView();
            this.grid = new System.Windows.Forms.DataGridView();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.dgcColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridContainer = new System.Windows.Forms.SplitContainer();
            this.dgcShortValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcLongValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.domainGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.gridContainer.Panel1.SuspendLayout();
            this.gridContainer.Panel2.SuspendLayout();
            this.gridContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.gridContainer);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.okButton);
            this.splitContainer.Panel2.Controls.Add(this.cancelButton);
            this.splitContainer.Size = new System.Drawing.Size(645, 579);
            this.splitContainer.SplitterDistance = 507;
            this.splitContainer.TabIndex = 0;
            // 
            // domainGrid
            // 
            this.domainGrid.AllowUserToAddRows = false;
            this.domainGrid.AllowUserToDeleteRows = false;
            this.domainGrid.AllowUserToResizeRows = false;
            this.domainGrid.BackgroundColor = System.Drawing.Color.Moccasin;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.Moccasin;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.domainGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.domainGrid.ColumnHeadersHeight = 24;
            this.domainGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgcShortValue,
            this.dgcLongValue});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.Moccasin;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.Coral;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.domainGrid.DefaultCellStyle = dataGridViewCellStyle3;
            this.domainGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.domainGrid.Location = new System.Drawing.Point(0, 0);
            this.domainGrid.Margin = new System.Windows.Forms.Padding(10);
            this.domainGrid.MultiSelect = false;
            this.domainGrid.Name = "domainGrid";
            this.domainGrid.ReadOnly = true;
            this.domainGrid.RowHeadersVisible = false;
            this.domainGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.domainGrid.Size = new System.Drawing.Size(297, 507);
            this.domainGrid.TabIndex = 2;
            this.domainGrid.SelectionChanged += new System.EventHandler(this.domainGrid_SelectionChanged);
            // 
            // grid
            // 
            this.grid.AllowUserToAddRows = false;
            this.grid.AllowUserToDeleteRows = false;
            this.grid.AllowUserToResizeRows = false;
            this.grid.ColumnHeadersHeight = 24;
            this.grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgcColumnName,
            this.dgcValue});
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.Location = new System.Drawing.Point(0, 0);
            this.grid.MultiSelect = false;
            this.grid.Name = "grid";
            this.grid.RowHeadersVisible = false;
            this.grid.Size = new System.Drawing.Size(344, 507);
            this.grid.TabIndex = 1;
            this.grid.SelectionChanged += new System.EventHandler(this.grid_SelectionChanged);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(558, 23);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(468, 23);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.TabStop = false;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // dgcColumnName
            // 
            this.dgcColumnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgcColumnName.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgcColumnName.HeaderText = "Column Name";
            this.dgcColumnName.Name = "dgcColumnName";
            this.dgcColumnName.ReadOnly = true;
            // 
            // dgcValue
            // 
            this.dgcValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgcValue.HeaderText = "Value";
            this.dgcValue.Name = "dgcValue";
            // 
            // gridContainer
            // 
            this.gridContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridContainer.Location = new System.Drawing.Point(0, 0);
            this.gridContainer.Margin = new System.Windows.Forms.Padding(10);
            this.gridContainer.Name = "gridContainer";
            // 
            // gridContainer.Panel1
            // 
            this.gridContainer.Panel1.Controls.Add(this.grid);
            // 
            // gridContainer.Panel2
            // 
            this.gridContainer.Panel2.Controls.Add(this.domainGrid);
            this.gridContainer.Size = new System.Drawing.Size(645, 507);
            this.gridContainer.SplitterDistance = 344;
            this.gridContainer.TabIndex = 3;
            // 
            // dgcShortValue
            // 
            this.dgcShortValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgcShortValue.FillWeight = 30F;
            this.dgcShortValue.HeaderText = "Value";
            this.dgcShortValue.Name = "dgcShortValue";
            this.dgcShortValue.ReadOnly = true;
            // 
            // dgcLongValue
            // 
            this.dgcLongValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgcLongValue.HeaderText = "Meaning";
            this.dgcLongValue.Name = "dgcLongValue";
            this.dgcLongValue.ReadOnly = true;
            // 
            // AttributeDataForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(645, 579);
            this.Controls.Add(this.splitContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "AttributeDataForm";
            this.Text = "Attribute Data";
            this.Shown += new System.EventHandler(this.AttributeDataForm_Shown);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.domainGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.gridContainer.Panel1.ResumeLayout(false);
            this.gridContainer.Panel2.ResumeLayout(false);
            this.gridContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.DataGridView grid;
        private System.Windows.Forms.DataGridView domainGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcColumnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcValue;
        private System.Windows.Forms.SplitContainer gridContainer;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcShortValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcLongValue;
    }
}