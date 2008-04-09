/// <remarks>
/// Copyright 2007 - Steve Stanton. This file is part of Backsight
///
/// Backsight is free software; you can redistribute it and/or modify it under the terms
/// of the GNU Lesser General Public License as published by the Free Software Foundation;
/// either version 3 of the License, or (at your option) any later version.
///
/// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
/// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU Lesser General Public License for more details.
///
/// You should have received a copy of the GNU Lesser General Public License
/// along with this program. If not, see <http://www.gnu.org/licenses/>.
/// </remarks>

namespace Backsight.Environment.Editor
{
    partial class IdFormatForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.leadingCharsTextBox = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numDigitTextBox = new System.Windows.Forms.TextBox();
            this.keepLeadingZeroesCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.trailingCharsTextBox = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.checkDigitCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.leadingCharsTextBox);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(10, 17);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(154, 104);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Leading characters";
            // 
            // leadingCharsTextBox
            // 
            this.leadingCharsTextBox.Location = new System.Drawing.Point(43, 38);
            this.leadingCharsTextBox.Name = "leadingCharsTextBox";
            this.leadingCharsTextBox.Size = new System.Drawing.Size(73, 22);
            this.leadingCharsTextBox.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numDigitTextBox);
            this.groupBox2.Controls.Add(this.keepLeadingZeroesCheckBox);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(178, 17);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(293, 104);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Numeric portion";
            // 
            // numDigitTextBox
            // 
            this.numDigitTextBox.Location = new System.Drawing.Point(123, 34);
            this.numDigitTextBox.Name = "numDigitTextBox";
            this.numDigitTextBox.Size = new System.Drawing.Size(73, 22);
            this.numDigitTextBox.TabIndex = 1;
            this.numDigitTextBox.TextChanged += new System.EventHandler(this.numDigitTextBox_TextChanged);
            // 
            // keepLeadingZeroesCheckBox
            // 
            this.keepLeadingZeroesCheckBox.AutoSize = true;
            this.keepLeadingZeroesCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.keepLeadingZeroesCheckBox.Location = new System.Drawing.Point(123, 62);
            this.keepLeadingZeroesCheckBox.Name = "keepLeadingZeroesCheckBox";
            this.keepLeadingZeroesCheckBox.Size = new System.Drawing.Size(151, 20);
            this.keepLeadingZeroesCheckBox.TabIndex = 10;
            this.keepLeadingZeroesCheckBox.TabStop = false;
            this.keepLeadingZeroesCheckBox.Text = "Keep leading zeroes";
            this.keepLeadingZeroesCheckBox.UseVisualStyleBackColor = true;
            this.keepLeadingZeroesCheckBox.CheckedChanged += new System.EventHandler(this.keepLeadingZeroesCheckBox_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Number of digits";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.trailingCharsTextBox);
            this.groupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(484, 17);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(149, 104);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Trailing characters";
            // 
            // trailingCharsTextBox
            // 
            this.trailingCharsTextBox.Location = new System.Drawing.Point(23, 34);
            this.trailingCharsTextBox.Name = "trailingCharsTextBox";
            this.trailingCharsTextBox.Size = new System.Drawing.Size(73, 22);
            this.trailingCharsTextBox.TabIndex = 2;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(342, 160);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(252, 160);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 7;
            this.cancelButton.TabStop = false;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // checkDigitCheckBox
            // 
            this.checkDigitCheckBox.AutoSize = true;
            this.checkDigitCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkDigitCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkDigitCheckBox.Location = new System.Drawing.Point(491, 127);
            this.checkDigitCheckBox.Name = "checkDigitCheckBox";
            this.checkDigitCheckBox.Size = new System.Drawing.Size(142, 20);
            this.checkDigitCheckBox.TabIndex = 9;
            this.checkDigitCheckBox.TabStop = false;
            this.checkDigitCheckBox.Text = "Append check digit";
            this.checkDigitCheckBox.UseVisualStyleBackColor = true;
            this.checkDigitCheckBox.CheckedChanged += new System.EventHandler(this.checkDigitCheckBox_CheckedChanged);
            // 
            // IdFormatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(651, 197);
            this.Controls.Add(this.checkDigitCheckBox);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "IdFormatForm";
            this.Text = "Format for ID Group";
            this.Load += new System.EventHandler(this.IdFormatForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox leadingCharsTextBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox trailingCharsTextBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox checkDigitCheckBox;
        private System.Windows.Forms.CheckBox keepLeadingZeroesCheckBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox numDigitTextBox;
    }
}
