namespace Backsight.Editor.Forms
{
    partial class DistanceForm
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
            this.distanceTextBox = new System.Windows.Forms.TextBox();
            this.wantLineCheckBox = new System.Windows.Forms.CheckBox();
            this.wantPointCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.newDefaultCheckBox = new System.Windows.Forms.CheckBox();
            this.cRadioButton = new System.Windows.Forms.RadioButton();
            this.fRadioButton = new System.Windows.Forms.RadioButton();
            this.mRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.x3Button = new System.Windows.Forms.Button();
            this.x2Button = new System.Windows.Forms.Button();
            this.x1Button = new System.Windows.Forms.Button();
            this.x6Button = new System.Windows.Forms.Button();
            this.x5Button = new System.Windows.Forms.Button();
            this.x4Button = new System.Windows.Forms.Button();
            this.x9Button = new System.Windows.Forms.Button();
            this.x8Button = new System.Windows.Forms.Button();
            this.x7Button = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // distanceTextBox
            // 
            this.distanceTextBox.Location = new System.Drawing.Point(20, 19);
            this.distanceTextBox.Name = "distanceTextBox";
            this.distanceTextBox.Size = new System.Drawing.Size(114, 22);
            this.distanceTextBox.TabIndex = 0;
            this.distanceTextBox.TextChanged += new System.EventHandler(this.distanceTextBox_TextChanged);
            // 
            // wantLineCheckBox
            // 
            this.wantLineCheckBox.AutoSize = true;
            this.wantLineCheckBox.Location = new System.Drawing.Point(186, 19);
            this.wantLineCheckBox.Name = "wantLineCheckBox";
            this.wantLineCheckBox.Size = new System.Drawing.Size(91, 20);
            this.wantLineCheckBox.TabIndex = 1;
            this.wantLineCheckBox.TabStop = false;
            this.wantLineCheckBox.Text = "Create line";
            this.wantLineCheckBox.UseVisualStyleBackColor = true;
            this.wantLineCheckBox.CheckedChanged += new System.EventHandler(this.wantLineCheckBox_CheckedChanged);
            // 
            // wantPointCheckBox
            // 
            this.wantPointCheckBox.AutoSize = true;
            this.wantPointCheckBox.Location = new System.Drawing.Point(291, 19);
            this.wantPointCheckBox.Name = "wantPointCheckBox";
            this.wantPointCheckBox.Size = new System.Drawing.Size(99, 20);
            this.wantPointCheckBox.TabIndex = 2;
            this.wantPointCheckBox.TabStop = false;
            this.wantPointCheckBox.Text = "Create point";
            this.wantPointCheckBox.UseVisualStyleBackColor = true;
            this.wantPointCheckBox.CheckedChanged += new System.EventHandler(this.wantPointCheckBox_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.newDefaultCheckBox);
            this.groupBox1.Controls.Add(this.cRadioButton);
            this.groupBox1.Controls.Add(this.fRadioButton);
            this.groupBox1.Controls.Add(this.mRadioButton);
            this.groupBox1.Location = new System.Drawing.Point(21, 62);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(169, 172);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Units";
            // 
            // newDefaultCheckBox
            // 
            this.newDefaultCheckBox.AutoSize = true;
            this.newDefaultCheckBox.Location = new System.Drawing.Point(22, 122);
            this.newDefaultCheckBox.Name = "newDefaultCheckBox";
            this.newDefaultCheckBox.Size = new System.Drawing.Size(131, 20);
            this.newDefaultCheckBox.TabIndex = 3;
            this.newDefaultCheckBox.TabStop = false;
            this.newDefaultCheckBox.Text = "Make new default";
            this.newDefaultCheckBox.UseVisualStyleBackColor = true;
            this.newDefaultCheckBox.CheckedChanged += new System.EventHandler(this.newDefaultCheckBox_CheckedChanged);
            // 
            // cRadioButton
            // 
            this.cRadioButton.AutoSize = true;
            this.cRadioButton.Location = new System.Drawing.Point(22, 84);
            this.cRadioButton.Name = "cRadioButton";
            this.cRadioButton.Size = new System.Drawing.Size(67, 20);
            this.cRadioButton.TabIndex = 2;
            this.cRadioButton.Text = "Chains";
            this.cRadioButton.UseVisualStyleBackColor = true;
            this.cRadioButton.CheckedChanged += new System.EventHandler(this.cRadioButton_CheckedChanged);
            // 
            // fRadioButton
            // 
            this.fRadioButton.AutoSize = true;
            this.fRadioButton.Location = new System.Drawing.Point(22, 58);
            this.fRadioButton.Name = "fRadioButton";
            this.fRadioButton.Size = new System.Drawing.Size(53, 20);
            this.fRadioButton.TabIndex = 1;
            this.fRadioButton.Text = "Feet";
            this.fRadioButton.UseVisualStyleBackColor = true;
            this.fRadioButton.CheckedChanged += new System.EventHandler(this.fRadioButton_CheckedChanged);
            // 
            // mRadioButton
            // 
            this.mRadioButton.AutoSize = true;
            this.mRadioButton.Location = new System.Drawing.Point(22, 32);
            this.mRadioButton.Name = "mRadioButton";
            this.mRadioButton.Size = new System.Drawing.Size(67, 20);
            this.mRadioButton.TabIndex = 0;
            this.mRadioButton.Text = "Meters";
            this.mRadioButton.UseVisualStyleBackColor = true;
            this.mRadioButton.CheckedChanged += new System.EventHandler(this.mRadioButton_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.x3Button);
            this.groupBox2.Controls.Add(this.x2Button);
            this.groupBox2.Controls.Add(this.x1Button);
            this.groupBox2.Controls.Add(this.x6Button);
            this.groupBox2.Controls.Add(this.x5Button);
            this.groupBox2.Controls.Add(this.x4Button);
            this.groupBox2.Controls.Add(this.x9Button);
            this.groupBox2.Controls.Add(this.x8Button);
            this.groupBox2.Controls.Add(this.x7Button);
            this.groupBox2.Location = new System.Drawing.Point(216, 62);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(174, 172);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Repeat";
            // 
            // x3Button
            // 
            this.x3Button.Location = new System.Drawing.Point(113, 114);
            this.x3Button.Name = "x3Button";
            this.x3Button.Size = new System.Drawing.Size(40, 40);
            this.x3Button.TabIndex = 8;
            this.x3Button.TabStop = false;
            this.x3Button.Text = "*3";
            this.x3Button.UseVisualStyleBackColor = true;
            this.x3Button.Click += new System.EventHandler(this.x3Button_Click);
            // 
            // x2Button
            // 
            this.x2Button.Location = new System.Drawing.Point(67, 114);
            this.x2Button.Name = "x2Button";
            this.x2Button.Size = new System.Drawing.Size(40, 40);
            this.x2Button.TabIndex = 7;
            this.x2Button.TabStop = false;
            this.x2Button.Text = "*2";
            this.x2Button.UseVisualStyleBackColor = true;
            this.x2Button.Click += new System.EventHandler(this.x2Button_Click);
            // 
            // x1Button
            // 
            this.x1Button.Location = new System.Drawing.Point(21, 114);
            this.x1Button.Name = "x1Button";
            this.x1Button.Size = new System.Drawing.Size(40, 40);
            this.x1Button.TabIndex = 6;
            this.x1Button.TabStop = false;
            this.x1Button.Text = "*1";
            this.x1Button.UseVisualStyleBackColor = true;
            this.x1Button.Click += new System.EventHandler(this.x1Button_Click);
            // 
            // x6Button
            // 
            this.x6Button.Location = new System.Drawing.Point(113, 67);
            this.x6Button.Name = "x6Button";
            this.x6Button.Size = new System.Drawing.Size(40, 40);
            this.x6Button.TabIndex = 5;
            this.x6Button.TabStop = false;
            this.x6Button.Text = "*6";
            this.x6Button.UseVisualStyleBackColor = true;
            this.x6Button.Click += new System.EventHandler(this.x6Button_Click);
            // 
            // x5Button
            // 
            this.x5Button.Location = new System.Drawing.Point(67, 68);
            this.x5Button.Name = "x5Button";
            this.x5Button.Size = new System.Drawing.Size(40, 40);
            this.x5Button.TabIndex = 4;
            this.x5Button.TabStop = false;
            this.x5Button.Text = "*5";
            this.x5Button.UseVisualStyleBackColor = true;
            this.x5Button.Click += new System.EventHandler(this.x5Button_Click);
            // 
            // x4Button
            // 
            this.x4Button.Location = new System.Drawing.Point(21, 68);
            this.x4Button.Name = "x4Button";
            this.x4Button.Size = new System.Drawing.Size(40, 40);
            this.x4Button.TabIndex = 3;
            this.x4Button.TabStop = false;
            this.x4Button.Text = "*4";
            this.x4Button.UseVisualStyleBackColor = true;
            this.x4Button.Click += new System.EventHandler(this.x4Button_Click);
            // 
            // x9Button
            // 
            this.x9Button.Location = new System.Drawing.Point(113, 21);
            this.x9Button.Name = "x9Button";
            this.x9Button.Size = new System.Drawing.Size(40, 40);
            this.x9Button.TabIndex = 2;
            this.x9Button.TabStop = false;
            this.x9Button.Text = "*9";
            this.x9Button.UseVisualStyleBackColor = true;
            this.x9Button.Click += new System.EventHandler(this.x9Button_Click);
            // 
            // x8Button
            // 
            this.x8Button.Location = new System.Drawing.Point(67, 22);
            this.x8Button.Name = "x8Button";
            this.x8Button.Size = new System.Drawing.Size(40, 40);
            this.x8Button.TabIndex = 1;
            this.x8Button.TabStop = false;
            this.x8Button.Text = "*8";
            this.x8Button.UseVisualStyleBackColor = true;
            this.x8Button.Click += new System.EventHandler(this.x8Button_Click);
            // 
            // x7Button
            // 
            this.x7Button.Location = new System.Drawing.Point(21, 22);
            this.x7Button.Name = "x7Button";
            this.x7Button.Size = new System.Drawing.Size(40, 40);
            this.x7Button.TabIndex = 0;
            this.x7Button.TabStop = false;
            this.x7Button.Text = "*7";
            this.x7Button.UseVisualStyleBackColor = true;
            this.x7Button.Click += new System.EventHandler(this.x7Button_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(90, 260);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(100, 30);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.TabStop = false;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(216, 260);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(100, 30);
            this.okButton.TabIndex = 6;
            this.okButton.TabStop = false;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // DistanceForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(417, 310);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.wantPointCheckBox);
            this.Controls.Add(this.wantLineCheckBox);
            this.Controls.Add(this.distanceTextBox);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "DistanceForm";
            this.Text = "Enter distance";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.DistanceForm_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox distanceTextBox;
        private System.Windows.Forms.CheckBox wantLineCheckBox;
        private System.Windows.Forms.CheckBox wantPointCheckBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox newDefaultCheckBox;
        private System.Windows.Forms.RadioButton cRadioButton;
        private System.Windows.Forms.RadioButton fRadioButton;
        private System.Windows.Forms.RadioButton mRadioButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button x3Button;
        private System.Windows.Forms.Button x2Button;
        private System.Windows.Forms.Button x1Button;
        private System.Windows.Forms.Button x6Button;
        private System.Windows.Forms.Button x5Button;
        private System.Windows.Forms.Button x4Button;
        private System.Windows.Forms.Button x9Button;
        private System.Windows.Forms.Button x8Button;
        private System.Windows.Forms.Button x7Button;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
    }
}