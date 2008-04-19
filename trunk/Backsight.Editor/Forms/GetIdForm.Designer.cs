namespace Backsight.Editor.Forms
{
    partial class GetIdForm
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
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.pointIdComboBox = new System.Windows.Forms.ComboBox();
            this.autoNumberCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(120, 91);
            this.okButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(100, 28);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(12, 91);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(100, 28);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.TabStop = false;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // pointIdComboBox
            // 
            this.pointIdComboBox.FormattingEnabled = true;
            this.pointIdComboBox.Location = new System.Drawing.Point(37, 12);
            this.pointIdComboBox.Name = "pointIdComboBox";
            this.pointIdComboBox.Size = new System.Drawing.Size(159, 24);
            this.pointIdComboBox.TabIndex = 4;
            this.pointIdComboBox.SelectedValueChanged += new System.EventHandler(this.pointIdComboBox_SelectedValueChanged);
            // 
            // autoNumberCheckBox
            // 
            this.autoNumberCheckBox.AutoSize = true;
            this.autoNumberCheckBox.Location = new System.Drawing.Point(61, 51);
            this.autoNumberCheckBox.Name = "autoNumberCheckBox";
            this.autoNumberCheckBox.Size = new System.Drawing.Size(120, 20);
            this.autoNumberCheckBox.TabIndex = 5;
            this.autoNumberCheckBox.Text = "Don\'t ask again";
            this.autoNumberCheckBox.UseVisualStyleBackColor = true;
            // 
            // GetIdForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(235, 136);
            this.Controls.Add(this.autoNumberCheckBox);
            this.Controls.Add(this.pointIdComboBox);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "GetIdForm";
            this.Text = "Specify ID";
            this.Shown += new System.EventHandler(this.GetIdForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ComboBox pointIdComboBox;
        private System.Windows.Forms.CheckBox autoNumberCheckBox;
    }
}