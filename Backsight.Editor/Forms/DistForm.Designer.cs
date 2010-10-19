namespace Backsight.Editor.Forms
{
    partial class DistForm
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.wantLineCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(156, 20);
            this.textBox1.TabIndex = 0;
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(12, 61);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.TabStop = false;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(93, 61);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // wantLineCheckBox
            // 
            this.wantLineCheckBox.AutoSize = true;
            this.wantLineCheckBox.Enabled = false;
            this.wantLineCheckBox.Location = new System.Drawing.Point(48, 38);
            this.wantLineCheckBox.Name = "wantLineCheckBox";
            this.wantLineCheckBox.Size = new System.Drawing.Size(95, 17);
            this.wantLineCheckBox.TabIndex = 3;
            this.wantLineCheckBox.TabStop = false;
            this.wantLineCheckBox.Text = "Add a &Line too";
            this.wantLineCheckBox.UseVisualStyleBackColor = true;
            this.wantLineCheckBox.Visible = false;
            // 
            // DistForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(182, 103);
            this.Controls.Add(this.wantLineCheckBox);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.textBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "DistForm";
            this.Text = "Specify Distance";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.CheckBox wantLineCheckBox;
    }
}