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
    partial class EntityForm
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
            this.idGroupComboBox = new System.Windows.Forms.ComboBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.lblStatic1 = new System.Windows.Forms.Label();
            this.lblStatic2 = new System.Windows.Forms.Label();
            this.entityNameTextBox = new System.Windows.Forms.TextBox();
            this.grpStatic3 = new System.Windows.Forms.GroupBox();
            this.boundaryCheckbox = new System.Windows.Forms.CheckBox();
            this.labelCheckbox = new System.Windows.Forms.CheckBox();
            this.lineCheckbox = new System.Windows.Forms.CheckBox();
            this.pointCheckbox = new System.Windows.Forms.CheckBox();
            this.textCheckbox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.layerComboBox = new System.Windows.Forms.ComboBox();
            this.grpStatic3.SuspendLayout();
            this.SuspendLayout();
            // 
            // idGroupComboBox
            // 
            this.idGroupComboBox.FormattingEnabled = true;
            this.idGroupComboBox.Location = new System.Drawing.Point(120, 66);
            this.idGroupComboBox.Name = "idGroupComboBox";
            this.idGroupComboBox.Size = new System.Drawing.Size(380, 21);
            this.idGroupComboBox.TabIndex = 23;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(416, 247);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 19;
            this.cancelButton.TabStop = false;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(416, 215);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 18;
            this.okButton.Text = "OK";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // lblStatic1
            // 
            this.lblStatic1.Location = new System.Drawing.Point(12, 24);
            this.lblStatic1.Name = "lblStatic1";
            this.lblStatic1.Size = new System.Drawing.Size(68, 17);
            this.lblStatic1.TabIndex = 20;
            this.lblStatic1.Text = "Entity Name";
            this.lblStatic1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStatic2
            // 
            this.lblStatic2.Location = new System.Drawing.Point(12, 67);
            this.lblStatic2.Name = "lblStatic2";
            this.lblStatic2.Size = new System.Drawing.Size(68, 17);
            this.lblStatic2.TabIndex = 21;
            this.lblStatic2.Text = "ID Group";
            this.lblStatic2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // entityNameTextBox
            // 
            this.entityNameTextBox.Location = new System.Drawing.Point(120, 23);
            this.entityNameTextBox.Name = "entityNameTextBox";
            this.entityNameTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.entityNameTextBox.Size = new System.Drawing.Size(380, 20);
            this.entityNameTextBox.TabIndex = 17;
            // 
            // grpStatic3
            // 
            this.grpStatic3.Controls.Add(this.boundaryCheckbox);
            this.grpStatic3.Controls.Add(this.labelCheckbox);
            this.grpStatic3.Controls.Add(this.lineCheckbox);
            this.grpStatic3.Controls.Add(this.pointCheckbox);
            this.grpStatic3.Controls.Add(this.textCheckbox);
            this.grpStatic3.Location = new System.Drawing.Point(120, 163);
            this.grpStatic3.Name = "grpStatic3";
            this.grpStatic3.Size = new System.Drawing.Size(242, 117);
            this.grpStatic3.TabIndex = 22;
            this.grpStatic3.TabStop = false;
            this.grpStatic3.Text = "Valid Geometric Types";
            // 
            // boundaryCheckbox
            // 
            this.boundaryCheckbox.Location = new System.Drawing.Point(101, 52);
            this.boundaryCheckbox.Name = "boundaryCheckbox";
            this.boundaryCheckbox.Size = new System.Drawing.Size(118, 22);
            this.boundaryCheckbox.TabIndex = 14;
            this.boundaryCheckbox.TabStop = false;
            this.boundaryCheckbox.Text = "Polygon &Boundary";
            // 
            // labelCheckbox
            // 
            this.labelCheckbox.Location = new System.Drawing.Point(101, 79);
            this.labelCheckbox.Name = "labelCheckbox";
            this.labelCheckbox.Size = new System.Drawing.Size(118, 28);
            this.labelCheckbox.TabIndex = 13;
            this.labelCheckbox.TabStop = false;
            this.labelCheckbox.Text = "Polygon L&abel";
            // 
            // lineCheckbox
            // 
            this.lineCheckbox.Location = new System.Drawing.Point(11, 52);
            this.lineCheckbox.Name = "lineCheckbox";
            this.lineCheckbox.Size = new System.Drawing.Size(64, 22);
            this.lineCheckbox.TabIndex = 11;
            this.lineCheckbox.TabStop = false;
            this.lineCheckbox.Text = "&Line";
            this.lineCheckbox.CheckedChanged += new System.EventHandler(this.lineCheckbox_CheckedChanged);
            // 
            // pointCheckbox
            // 
            this.pointCheckbox.Location = new System.Drawing.Point(11, 25);
            this.pointCheckbox.Name = "pointCheckbox";
            this.pointCheckbox.Size = new System.Drawing.Size(96, 21);
            this.pointCheckbox.TabIndex = 10;
            this.pointCheckbox.TabStop = false;
            this.pointCheckbox.Text = "&Point";
            // 
            // textCheckbox
            // 
            this.textCheckbox.Location = new System.Drawing.Point(11, 79);
            this.textCheckbox.Name = "textCheckbox";
            this.textCheckbox.Size = new System.Drawing.Size(64, 28);
            this.textCheckbox.TabIndex = 12;
            this.textCheckbox.TabStop = false;
            this.textCheckbox.Text = "&Text";
            this.textCheckbox.CheckedChanged += new System.EventHandler(this.textCheckbox_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 110);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 17);
            this.label1.TabIndex = 24;
            this.label1.Text = "Layer (blank for all)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // layerComboBox
            // 
            this.layerComboBox.FormattingEnabled = true;
            this.layerComboBox.Location = new System.Drawing.Point(120, 109);
            this.layerComboBox.Name = "layerComboBox";
            this.layerComboBox.Size = new System.Drawing.Size(380, 21);
            this.layerComboBox.TabIndex = 25;
            // 
            // EntityForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(528, 311);
            this.Controls.Add(this.layerComboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.idGroupComboBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.lblStatic1);
            this.Controls.Add(this.lblStatic2);
            this.Controls.Add(this.entityNameTextBox);
            this.Controls.Add(this.grpStatic3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "EntityForm";
            this.Text = "Entity Type";
            this.Shown += new System.EventHandler(this.EntityForm_Shown);
            this.grpStatic3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox idGroupComboBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label lblStatic1;
        private System.Windows.Forms.Label lblStatic2;
        private System.Windows.Forms.TextBox entityNameTextBox;
        private System.Windows.Forms.GroupBox grpStatic3;
        private System.Windows.Forms.CheckBox boundaryCheckbox;
        private System.Windows.Forms.CheckBox labelCheckbox;
        private System.Windows.Forms.CheckBox lineCheckbox;
        private System.Windows.Forms.CheckBox pointCheckbox;
        private System.Windows.Forms.CheckBox textCheckbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox layerComboBox;
    }
}
