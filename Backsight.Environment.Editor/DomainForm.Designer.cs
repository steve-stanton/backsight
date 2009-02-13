namespace Backsight.Environment.Editor
{
    partial class DomainForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.tableNameComboBox = new System.Windows.Forms.ComboBox();
            this.shortValueColumnNameComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.longValueColumnNameComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.alreadyAddedLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 28);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Domain table";
            // 
            // tableNameComboBox
            // 
            this.tableNameComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableNameComboBox.FormattingEnabled = true;
            this.tableNameComboBox.Location = new System.Drawing.Point(171, 25);
            this.tableNameComboBox.Name = "tableNameComboBox";
            this.tableNameComboBox.Size = new System.Drawing.Size(320, 24);
            this.tableNameComboBox.TabIndex = 1;
            this.tableNameComboBox.SelectedIndexChanged += new System.EventHandler(this.tableNameComboBox_SelectedIndexChanged);
            // 
            // shortValueColumnNameComboBox
            // 
            this.shortValueColumnNameComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.shortValueColumnNameComboBox.FormattingEnabled = true;
            this.shortValueColumnNameComboBox.Location = new System.Drawing.Point(171, 55);
            this.shortValueColumnNameComboBox.Name = "shortValueColumnNameComboBox";
            this.shortValueColumnNameComboBox.Size = new System.Drawing.Size(320, 24);
            this.shortValueColumnNameComboBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 58);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Lookup column";
            // 
            // longValueColumnNameComboBox
            // 
            this.longValueColumnNameComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.longValueColumnNameComboBox.FormattingEnabled = true;
            this.longValueColumnNameComboBox.Location = new System.Drawing.Point(171, 85);
            this.longValueColumnNameComboBox.Name = "longValueColumnNameComboBox";
            this.longValueColumnNameComboBox.Size = new System.Drawing.Size(320, 24);
            this.longValueColumnNameComboBox.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 88);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(152, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Expanded value column";
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(249, 142);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(339, 142);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 7;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // alreadyAddedLabel
            // 
            this.alreadyAddedLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.alreadyAddedLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.alreadyAddedLabel.Location = new System.Drawing.Point(157, 123);
            this.alreadyAddedLabel.Name = "alreadyAddedLabel";
            this.alreadyAddedLabel.Size = new System.Drawing.Size(354, 16);
            this.alreadyAddedLabel.TabIndex = 25;
            this.alreadyAddedLabel.Text = "Domain has already been added to Backsight catalog";
            this.alreadyAddedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DomainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(523, 190);
            this.Controls.Add(this.alreadyAddedLabel);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.longValueColumnNameComboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.shortValueColumnNameComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tableNameComboBox);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "DomainForm";
            this.Text = "Domains";
            this.Shown += new System.EventHandler(this.DomainForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox tableNameComboBox;
        private System.Windows.Forms.ComboBox shortValueColumnNameComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox longValueColumnNameComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label alreadyAddedLabel;
    }
}