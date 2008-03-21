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
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.counterClockwiseRadioButton = new System.Windows.Forms.RadioButton();
            this.clockwiseRadioButton = new System.Windows.Forms.RadioButton();
            this.angleTextBox = new System.Windows.Forms.TextBox();
            this.radiusTextBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(156, 228);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(100, 30);
            this.okButton.TabIndex = 13;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(40, 228);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(100, 30);
            this.cancelButton.TabIndex = 12;
            this.cancelButton.TabStop = false;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // counterClockwiseRadioButton
            // 
            this.counterClockwiseRadioButton.AutoSize = true;
            this.counterClockwiseRadioButton.Location = new System.Drawing.Point(25, 59);
            this.counterClockwiseRadioButton.Name = "counterClockwiseRadioButton";
            this.counterClockwiseRadioButton.Size = new System.Drawing.Size(135, 20);
            this.counterClockwiseRadioButton.TabIndex = 11;
            this.counterClockwiseRadioButton.Text = "&Counter-clockwise";
            this.counterClockwiseRadioButton.UseVisualStyleBackColor = true;
            this.counterClockwiseRadioButton.CheckedChanged += new System.EventHandler(this.counterClockwiseRadioButton_CheckedChanged);
            // 
            // clockwiseRadioButton
            // 
            this.clockwiseRadioButton.AutoSize = true;
            this.clockwiseRadioButton.Location = new System.Drawing.Point(25, 33);
            this.clockwiseRadioButton.Name = "clockwiseRadioButton";
            this.clockwiseRadioButton.Size = new System.Drawing.Size(87, 20);
            this.clockwiseRadioButton.TabIndex = 10;
            this.clockwiseRadioButton.Text = "Clock&wise";
            this.clockwiseRadioButton.UseVisualStyleBackColor = true;
            this.clockwiseRadioButton.CheckedChanged += new System.EventHandler(this.clockwiseRadioButton_CheckedChanged);
            // 
            // angleTextBox
            // 
            this.angleTextBox.Location = new System.Drawing.Point(125, 27);
            this.angleTextBox.Name = "angleTextBox";
            this.angleTextBox.Size = new System.Drawing.Size(105, 22);
            this.angleTextBox.TabIndex = 0;
            // 
            // radiusTextBox
            // 
            this.radiusTextBox.Location = new System.Drawing.Point(125, 65);
            this.radiusTextBox.Name = "radiusTextBox";
            this.radiusTextBox.Size = new System.Drawing.Size(105, 22);
            this.radiusTextBox.TabIndex = 1;
            this.radiusTextBox.TextChanged += new System.EventHandler(this.radiusTextBox_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.counterClockwiseRadioButton);
            this.groupBox1.Controls.Add(this.clockwiseRadioButton);
            this.groupBox1.Location = new System.Drawing.Point(28, 108);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(239, 100);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Which way does the curve go?";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 16);
            this.label1.TabIndex = 16;
            this.label1.Text = "Central Angle";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(68, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 16);
            this.label2.TabIndex = 17;
            this.label2.Text = "Radius";
            // 
            // CulDeSacForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 286);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.radiusTextBox);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.angleTextBox);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "CulDeSacForm";
            this.Text = "Cul de sac";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.CulDeSacForm_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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