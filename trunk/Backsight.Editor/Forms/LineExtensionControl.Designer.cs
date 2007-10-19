namespace Backsight.Editor.Forms
{
    partial class LineExtensionControl
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lengthTextBox = new System.Windows.Forms.TextBox();
            this.otherEndButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pointTypeComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.idComboBox = new System.Windows.Forms.ComboBox();
            this.wantLineCheckBox = new System.Windows.Forms.CheckBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.otherEndButton);
            this.groupBox1.Controls.Add(this.lengthTextBox);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(18, 20);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(193, 118);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Specify Length of Extension";
            // 
            // lengthTextBox
            // 
            this.lengthTextBox.Location = new System.Drawing.Point(16, 37);
            this.lengthTextBox.Name = "lengthTextBox";
            this.lengthTextBox.Size = new System.Drawing.Size(159, 22);
            this.lengthTextBox.TabIndex = 0;
            this.lengthTextBox.TextChanged += new System.EventHandler(this.lengthTextBox_TextChanged);
            // 
            // otherEndButton
            // 
            this.otherEndButton.Location = new System.Drawing.Point(15, 74);
            this.otherEndButton.Name = "otherEndButton";
            this.otherEndButton.Size = new System.Drawing.Size(160, 27);
            this.otherEndButton.TabIndex = 1;
            this.otherEndButton.TabStop = false;
            this.otherEndButton.Text = "Extend from &Other End";
            this.otherEndButton.UseVisualStyleBackColor = true;
            this.otherEndButton.Click += new System.EventHandler(this.otherEndButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.wantLineCheckBox);
            this.groupBox2.Controls.Add(this.idComboBox);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.pointTypeComboBox);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(228, 20);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(343, 118);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "New Point at the end of the Extension";
            // 
            // pointTypeComboBox
            // 
            this.pointTypeComboBox.FormattingEnabled = true;
            this.pointTypeComboBox.Location = new System.Drawing.Point(17, 35);
            this.pointTypeComboBox.Name = "pointTypeComboBox";
            this.pointTypeComboBox.Size = new System.Drawing.Size(307, 24);
            this.pointTypeComboBox.TabIndex = 0;
            this.pointTypeComboBox.SelectedValueChanged += new System.EventHandler(this.pointTypeComboBox_SelectedValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(21, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "ID";
            // 
            // idComboBox
            // 
            this.idComboBox.FormattingEnabled = true;
            this.idComboBox.Location = new System.Drawing.Point(41, 74);
            this.idComboBox.Name = "idComboBox";
            this.idComboBox.Size = new System.Drawing.Size(144, 24);
            this.idComboBox.TabIndex = 2;
            this.idComboBox.SelectedValueChanged += new System.EventHandler(this.idComboBox_SelectedValueChanged);
            // 
            // wantLineCheckBox
            // 
            this.wantLineCheckBox.AutoSize = true;
            this.wantLineCheckBox.Location = new System.Drawing.Point(211, 78);
            this.wantLineCheckBox.Name = "wantLineCheckBox";
            this.wantLineCheckBox.Size = new System.Drawing.Size(113, 20);
            this.wantLineCheckBox.TabIndex = 3;
            this.wantLineCheckBox.Text = "Add a &Line too";
            this.wantLineCheckBox.UseVisualStyleBackColor = true;
            this.wantLineCheckBox.CheckedChanged += new System.EventHandler(this.wantLineCheckBox_CheckedChanged);
            // 
            // okButton
            // 
            this.okButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.okButton.Location = new System.Drawing.Point(606, 55);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.Location = new System.Drawing.Point(606, 94);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.TabStop = false;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // LineExtensionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "LineExtensionControl";
            this.Size = new System.Drawing.Size(700, 157);
            this.Load += new System.EventHandler(this.LineExtensionControl_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button otherEndButton;
        private System.Windows.Forms.TextBox lengthTextBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox wantLineCheckBox;
        private System.Windows.Forms.ComboBox idComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox pointTypeComboBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}
