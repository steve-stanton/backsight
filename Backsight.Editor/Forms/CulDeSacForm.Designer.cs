namespace Backsight.Editor.Forms
{
    partial class CulDeSacForm
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
            okButton = new System.Windows.Forms.Button();
            cancelButton = new System.Windows.Forms.Button();
            counterClockwiseRadioButton = new System.Windows.Forms.RadioButton();
            clockwiseRadioButton = new System.Windows.Forms.RadioButton();
            angleTextBox = new System.Windows.Forms.TextBox();
            radiusTextBox = new System.Windows.Forms.TextBox();
            groupBox1 = new System.Windows.Forms.GroupBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // okButton
            // 
            okButton.Location = new System.Drawing.Point(156, 228);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(100, 25);
            okButton.TabIndex = 13;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += okButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.Location = new System.Drawing.Point(40, 228);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(100, 25);
            cancelButton.TabIndex = 12;
            cancelButton.TabStop = false;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // counterClockwiseRadioButton
            // 
            counterClockwiseRadioButton.AutoSize = true;
            counterClockwiseRadioButton.Location = new System.Drawing.Point(25, 59);
            counterClockwiseRadioButton.Name = "counterClockwiseRadioButton";
            counterClockwiseRadioButton.Size = new System.Drawing.Size(168, 24);
            counterClockwiseRadioButton.TabIndex = 11;
            counterClockwiseRadioButton.Text = "&Counter-clockwise";
            counterClockwiseRadioButton.UseVisualStyleBackColor = true;
            counterClockwiseRadioButton.CheckedChanged += counterClockwiseRadioButton_CheckedChanged;
            // 
            // clockwiseRadioButton
            // 
            clockwiseRadioButton.AutoSize = true;
            clockwiseRadioButton.Location = new System.Drawing.Point(25, 33);
            clockwiseRadioButton.Name = "clockwiseRadioButton";
            clockwiseRadioButton.Size = new System.Drawing.Size(106, 24);
            clockwiseRadioButton.TabIndex = 10;
            clockwiseRadioButton.Text = "Clock&wise";
            clockwiseRadioButton.UseVisualStyleBackColor = true;
            clockwiseRadioButton.CheckedChanged += clockwiseRadioButton_CheckedChanged;
            // 
            // angleTextBox
            // 
            angleTextBox.Location = new System.Drawing.Point(125, 27);
            angleTextBox.Name = "angleTextBox";
            angleTextBox.Size = new System.Drawing.Size(105, 26);
            angleTextBox.TabIndex = 0;
            // 
            // radiusTextBox
            // 
            radiusTextBox.Location = new System.Drawing.Point(125, 65);
            radiusTextBox.Name = "radiusTextBox";
            radiusTextBox.Size = new System.Drawing.Size(105, 26);
            radiusTextBox.TabIndex = 1;
            radiusTextBox.TextChanged += radiusTextBox_TextChanged;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(counterClockwiseRadioButton);
            groupBox1.Controls.Add(clockwiseRadioButton);
            groupBox1.Location = new System.Drawing.Point(28, 108);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(267, 100);
            groupBox1.TabIndex = 15;
            groupBox1.TabStop = false;
            groupBox1.Text = "Which way does the curve go?";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(31, 30);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(110, 20);
            label1.TabIndex = 16;
            label1.Text = "Central Angle";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(68, 68);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(61, 20);
            label2.TabIndex = 17;
            label2.Text = "Radius";
            // 
            // CulDeSacForm
            // 
            AcceptButton = okButton;
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(314, 274);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(groupBox1);
            Controls.Add(radiusTextBox);
            Controls.Add(okButton);
            Controls.Add(cancelButton);
            Controls.Add(angleTextBox);
            Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)0));
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Margin = new System.Windows.Forms.Padding(4);
            Text = "Cul de sac";
            TopMost = true;
            Shown += CulDeSacForm_Shown;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.RadioButton counterClockwiseRadioButton;
        private System.Windows.Forms.RadioButton clockwiseRadioButton;
        private System.Windows.Forms.TextBox angleTextBox;
        private System.Windows.Forms.TextBox radiusTextBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}