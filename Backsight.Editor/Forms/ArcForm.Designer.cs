namespace Backsight.Editor.Forms
{
    partial class ArcForm
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
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.counterClockwiseRadioButton = new System.Windows.Forms.RadioButton();
            this.clockwiseRadioButton = new System.Windows.Forms.RadioButton();
            this.radiusTextBox = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.bcAngleTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ecAngleTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(105, 90);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 16);
            this.label2.TabIndex = 24;
            this.label2.Text = "Radius";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 30);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 16);
            this.label1.TabIndex = 23;
            this.label1.Text = "Angle to center (BC)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.counterClockwiseRadioButton);
            this.groupBox1.Controls.Add(this.clockwiseRadioButton);
            this.groupBox1.Location = new System.Drawing.Point(26, 132);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(319, 123);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Which way does the curve go?";
            // 
            // counterClockwiseRadioButton
            // 
            this.counterClockwiseRadioButton.AutoSize = true;
            this.counterClockwiseRadioButton.Location = new System.Drawing.Point(33, 73);
            this.counterClockwiseRadioButton.Margin = new System.Windows.Forms.Padding(4);
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
            this.clockwiseRadioButton.Location = new System.Drawing.Point(33, 41);
            this.clockwiseRadioButton.Margin = new System.Windows.Forms.Padding(4);
            this.clockwiseRadioButton.Name = "clockwiseRadioButton";
            this.clockwiseRadioButton.Size = new System.Drawing.Size(87, 20);
            this.clockwiseRadioButton.TabIndex = 10;
            this.clockwiseRadioButton.Text = "Clock&wise";
            this.clockwiseRadioButton.UseVisualStyleBackColor = true;
            this.clockwiseRadioButton.CheckedChanged += new System.EventHandler(this.clockwiseRadioButton_CheckedChanged);
            // 
            // radiusTextBox
            // 
            this.radiusTextBox.Location = new System.Drawing.Point(164, 87);
            this.radiusTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.radiusTextBox.Name = "radiusTextBox";
            this.radiusTextBox.Size = new System.Drawing.Size(99, 22);
            this.radiusTextBox.TabIndex = 19;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(189, 280);
            this.okButton.Margin = new System.Windows.Forms.Padding(4);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(100, 25);
            this.okButton.TabIndex = 21;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(72, 280);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(4);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(100, 25);
            this.cancelButton.TabIndex = 20;
            this.cancelButton.TabStop = false;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // bcAngleTextBox
            // 
            this.bcAngleTextBox.Location = new System.Drawing.Point(164, 27);
            this.bcAngleTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.bcAngleTextBox.Name = "bcAngleTextBox";
            this.bcAngleTextBox.Size = new System.Drawing.Size(99, 22);
            this.bcAngleTextBox.TabIndex = 18;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(30, 60);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(126, 16);
            this.label3.TabIndex = 26;
            this.label3.Text = "Angle to center (EC)";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ecAngleTextBox
            // 
            this.ecAngleTextBox.Location = new System.Drawing.Point(164, 57);
            this.ecAngleTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.ecAngleTextBox.Name = "ecAngleTextBox";
            this.ecAngleTextBox.Size = new System.Drawing.Size(99, 22);
            this.ecAngleTextBox.TabIndex = 25;
            this.ecAngleTextBox.Leave += new System.EventHandler(this.ecAngleTextBox_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(266, 60);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 16);
            this.label4.TabIndex = 27;
            this.label4.Text = "(optional)";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ArcForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(381, 327);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ecAngleTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.radiusTextBox);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.bcAngleTextBox);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ArcForm";
            this.Text = "Circular Arc";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.ArcForm_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton counterClockwiseRadioButton;
        private System.Windows.Forms.RadioButton clockwiseRadioButton;
        private System.Windows.Forms.TextBox radiusTextBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox bcAngleTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox ecAngleTextBox;
        private System.Windows.Forms.Label label4;
    }
}