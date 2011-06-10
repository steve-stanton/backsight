// <remarks>
// Copyright 2007 - Steve Stanton. This file is part of Backsight
//
// Backsight is free software; you can redistribute it and/or modify it under the terms
// of the GNU Lesser General Public License as published by the Free Software Foundation;
// either version 3 of the License, or (at your option) any later version.
//
// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
// </remarks>

namespace Backsight.Editor.Forms
{
    partial class RadialControl
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
            Backsight.Editor.EditingHelpProvider editingHelpProvider;
            this.backsightTextBox = new System.Windows.Forms.TextBox();
            this.angleTextBox = new System.Windows.Forms.TextBox();
            this.lengthTextBox = new System.Windows.Forms.TextBox();
            this.centreOfCurveCheckBox = new System.Windows.Forms.CheckBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.clockwiseRadioButton = new System.Windows.Forms.RadioButton();
            this.counterClockwiseRadioButton = new System.Windows.Forms.RadioButton();
            this.offsetButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.addLineCheckBox = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.pointIdComboBox = new System.Windows.Forms.ComboBox();
            this.entityTypeComboBox = new Backsight.Editor.Forms.EntityTypeComboBox();
            editingHelpProvider = new Backsight.Editor.EditingHelpProvider();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // backsightTextBox
            // 
            editingHelpProvider.SetHelpKeyword(this.backsightTextBox, "");
            editingHelpProvider.SetHelpString(this.backsightTextBox, "");
            this.backsightTextBox.Location = new System.Drawing.Point(76, 26);
            this.backsightTextBox.Name = "backsightTextBox";
            editingHelpProvider.SetShowHelp(this.backsightTextBox, true);
            this.backsightTextBox.Size = new System.Drawing.Size(100, 20);
            this.backsightTextBox.TabIndex = 0;
            this.backsightTextBox.TextChanged += new System.EventHandler(this.backsightTextBox_TextChanged);
            this.backsightTextBox.Enter += new System.EventHandler(this.backsightTextBox_Enter);
            // 
            // angleTextBox
            // 
            editingHelpProvider.SetHelpKeyword(this.angleTextBox, "angleTextBox");
            editingHelpProvider.SetHelpNavigator(this.angleTextBox, System.Windows.Forms.HelpNavigator.TopicId);
            this.angleTextBox.Location = new System.Drawing.Point(76, 72);
            this.angleTextBox.Name = "angleTextBox";
            editingHelpProvider.SetShowHelp(this.angleTextBox, true);
            this.angleTextBox.Size = new System.Drawing.Size(100, 20);
            this.angleTextBox.TabIndex = 1;
            this.angleTextBox.TextChanged += new System.EventHandler(this.angleTextBox_TextChanged);
            this.angleTextBox.Enter += new System.EventHandler(this.angleTextBox_Enter);
            // 
            // lengthTextBox
            // 
            editingHelpProvider.SetHelpKeyword(this.lengthTextBox, "lengthTextBox");
            this.lengthTextBox.Location = new System.Drawing.Point(76, 117);
            this.lengthTextBox.Name = "lengthTextBox";
            editingHelpProvider.SetShowHelp(this.lengthTextBox, true);
            this.lengthTextBox.Size = new System.Drawing.Size(100, 20);
            this.lengthTextBox.TabIndex = 2;
            this.lengthTextBox.TextChanged += new System.EventHandler(this.lengthTextBox_TextChanged);
            this.lengthTextBox.Enter += new System.EventHandler(this.lengthTextBox_Enter);
            // 
            // centreOfCurveCheckBox
            // 
            this.centreOfCurveCheckBox.AutoSize = true;
            editingHelpProvider.SetHelpKeyword(this.centreOfCurveCheckBox, "centreOfCurveCheckBox");
            editingHelpProvider.SetHelpNavigator(this.centreOfCurveCheckBox, System.Windows.Forms.HelpNavigator.TopicId);
            this.centreOfCurveCheckBox.Location = new System.Drawing.Point(192, 27);
            this.centreOfCurveCheckBox.Name = "centreOfCurveCheckBox";
            editingHelpProvider.SetShowHelp(this.centreOfCurveCheckBox, true);
            this.centreOfCurveCheckBox.Size = new System.Drawing.Size(122, 17);
            this.centreOfCurveCheckBox.TabIndex = 9;
            this.centreOfCurveCheckBox.TabStop = false;
            this.centreOfCurveCheckBox.Text = "&Use Center of Curve";
            this.centreOfCurveCheckBox.UseVisualStyleBackColor = true;
            this.centreOfCurveCheckBox.CheckedChanged += new System.EventHandler(this.centreOfCurveCheckBox_CheckedChanged);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(450, 120);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.TabStop = false;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Backsight";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "(optional)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Angle";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 120);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Length";
            // 
            // clockwiseRadioButton
            // 
            this.clockwiseRadioButton.AutoSize = true;
            this.clockwiseRadioButton.Location = new System.Drawing.Point(192, 69);
            this.clockwiseRadioButton.Name = "clockwiseRadioButton";
            this.clockwiseRadioButton.Size = new System.Drawing.Size(73, 17);
            this.clockwiseRadioButton.TabIndex = 10;
            this.clockwiseRadioButton.Text = "Clock&wise";
            this.clockwiseRadioButton.UseVisualStyleBackColor = true;
            this.clockwiseRadioButton.CheckedChanged += new System.EventHandler(this.clockwiseRadioButton_CheckedChanged);
            // 
            // counterClockwiseRadioButton
            // 
            this.counterClockwiseRadioButton.AutoSize = true;
            this.counterClockwiseRadioButton.Location = new System.Drawing.Point(192, 84);
            this.counterClockwiseRadioButton.Name = "counterClockwiseRadioButton";
            this.counterClockwiseRadioButton.Size = new System.Drawing.Size(112, 17);
            this.counterClockwiseRadioButton.TabIndex = 11;
            this.counterClockwiseRadioButton.Text = "&Counter-clockwise";
            this.counterClockwiseRadioButton.UseVisualStyleBackColor = true;
            this.counterClockwiseRadioButton.CheckedChanged += new System.EventHandler(this.counterClockwiseRadioButton_CheckedChanged);
            // 
            // offsetButton
            // 
            this.offsetButton.Location = new System.Drawing.Point(211, 119);
            this.offsetButton.Name = "offsetButton";
            this.offsetButton.Size = new System.Drawing.Size(75, 23);
            this.offsetButton.TabIndex = 12;
            this.offsetButton.TabStop = false;
            this.offsetButton.Text = "&Offset...";
            this.offsetButton.UseVisualStyleBackColor = true;
            this.offsetButton.Click += new System.EventHandler(this.offsetButton_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(531, 120);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 5;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.entityTypeComboBox);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.addLineCheckBox);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.pointIdComboBox);
            this.groupBox1.Location = new System.Drawing.Point(345, 16);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(363, 97);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "New Sideshot Point";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 26);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(31, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Type";
            // 
            // addLineCheckBox
            // 
            this.addLineCheckBox.AutoSize = true;
            this.addLineCheckBox.Location = new System.Drawing.Point(246, 60);
            this.addLineCheckBox.Name = "addLineCheckBox";
            this.addLineCheckBox.Size = new System.Drawing.Size(95, 17);
            this.addLineCheckBox.TabIndex = 10;
            this.addLineCheckBox.TabStop = false;
            this.addLineCheckBox.Text = "Add a &Line too";
            this.addLineCheckBox.UseVisualStyleBackColor = true;
            this.addLineCheckBox.CheckedChanged += new System.EventHandler(this.addLineCheckBox_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 61);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(18, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "ID";
            // 
            // pointIdComboBox
            // 
            this.pointIdComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.pointIdComboBox.FormattingEnabled = true;
            this.pointIdComboBox.Location = new System.Drawing.Point(58, 58);
            this.pointIdComboBox.Name = "pointIdComboBox";
            this.pointIdComboBox.Size = new System.Drawing.Size(173, 21);
            this.pointIdComboBox.TabIndex = 4;
            this.pointIdComboBox.TabStop = false;
            this.pointIdComboBox.SelectedValueChanged += new System.EventHandler(this.pointIdComboBox_SelectedValueChanged);
            // 
            // entityTypeComboBox
            // 
            this.entityTypeComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.entityTypeComboBox.FormattingEnabled = true;
            this.entityTypeComboBox.Location = new System.Drawing.Point(59, 19);
            this.entityTypeComboBox.Name = "entityTypeComboBox";
            this.entityTypeComboBox.ShowBlankEntityType = false;
            this.entityTypeComboBox.Size = new System.Drawing.Size(282, 24);
            this.entityTypeComboBox.TabIndex = 16;
            this.entityTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.entityTypeComboBox_SelectedValueChanged);
            // 
            // editingHelpProvider
            // 
            editingHelpProvider.HelpNamespace = "C:\\Users\\sstanton\\Code\\Files\\Backsight.chm";
            // 
            // RadialControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.offsetButton);
            this.Controls.Add(this.counterClockwiseRadioButton);
            this.Controls.Add(this.clockwiseRadioButton);
            this.Controls.Add(this.centreOfCurveCheckBox);
            this.Controls.Add(this.lengthTextBox);
            this.Controls.Add(this.angleTextBox);
            this.Controls.Add(this.backsightTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cancelButton);
            editingHelpProvider.SetHelpKeyword(this, "Sideshot");
            editingHelpProvider.SetHelpNavigator(this, System.Windows.Forms.HelpNavigator.TopicId);
            this.Name = "RadialControl";
            editingHelpProvider.SetShowHelp(this, true);
            this.Size = new System.Drawing.Size(724, 163);
            this.Load += new System.EventHandler(this.RadialControl_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox backsightTextBox;
        private System.Windows.Forms.TextBox angleTextBox;
        private System.Windows.Forms.TextBox lengthTextBox;
        private System.Windows.Forms.CheckBox centreOfCurveCheckBox;
        private System.Windows.Forms.RadioButton clockwiseRadioButton;
        private System.Windows.Forms.RadioButton counterClockwiseRadioButton;
        private System.Windows.Forms.Button offsetButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox pointIdComboBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox addLineCheckBox;
        private EntityTypeComboBox entityTypeComboBox;
    }
}
