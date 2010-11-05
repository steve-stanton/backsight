namespace Backsight.Editor.Forms
{
    partial class LineSubdivisionUpdateForm
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
            this.listBox = new System.Windows.Forms.ListBox();
            this.updateButton = new System.Windows.Forms.Button();
            this.flipDistButton = new System.Windows.Forms.Button();
            this.newFaceButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBox
            // 
            this.listBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox.DisplayMember = "ObservedLength";
            this.listBox.FormattingEnabled = true;
            this.listBox.ItemHeight = 16;
            this.listBox.Location = new System.Drawing.Point(12, 12);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(138, 244);
            this.listBox.TabIndex = 0;
            this.listBox.DoubleClick += new System.EventHandler(this.listBox_DoubleClick);
            this.listBox.SelectedValueChanged += new System.EventHandler(this.listBox_SelectedValueChanged);
            // 
            // updateButton
            // 
            this.updateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.updateButton.Location = new System.Drawing.Point(167, 13);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(111, 28);
            this.updateButton.TabIndex = 1;
            this.updateButton.TabStop = false;
            this.updateButton.Text = "&Update...";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // flipDistButton
            // 
            this.flipDistButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flipDistButton.Location = new System.Drawing.Point(167, 47);
            this.flipDistButton.Name = "flipDistButton";
            this.flipDistButton.Size = new System.Drawing.Size(111, 28);
            this.flipDistButton.TabIndex = 2;
            this.flipDistButton.TabStop = false;
            this.flipDistButton.Text = "Flip Ann&otations";
            this.flipDistButton.UseVisualStyleBackColor = true;
            this.flipDistButton.Click += new System.EventHandler(this.flipDistButton_Click);
            // 
            // newFaceButton
            // 
            this.newFaceButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.newFaceButton.Enabled = false;
            this.newFaceButton.Location = new System.Drawing.Point(167, 81);
            this.newFaceButton.Name = "newFaceButton";
            this.newFaceButton.Size = new System.Drawing.Size(111, 28);
            this.newFaceButton.TabIndex = 3;
            this.newFaceButton.TabStop = false;
            this.newFaceButton.Text = "New Fac&e";
            this.newFaceButton.UseVisualStyleBackColor = true;
            this.newFaceButton.Click += new System.EventHandler(this.newFaceButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(167, 194);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(111, 28);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(167, 228);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(111, 28);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.TabStop = false;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // LineSubdivisionUpdateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(294, 266);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.newFaceButton);
            this.Controls.Add(this.flipDistButton);
            this.Controls.Add(this.updateButton);
            this.Controls.Add(this.listBox);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "LineSubdivisionUpdateForm";
            this.Text = "Update Line Subdivision";
            this.Shown += new System.EventHandler(this.LineSubdivisionUpdateForm_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LineSubdivisionUpdateForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.Button flipDistButton;
        private System.Windows.Forms.Button newFaceButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}