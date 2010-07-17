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
    partial class TerminalControl
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
            this.okButton = new System.Windows.Forms.Button();
            this.otherWayButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.noTerminalCheckBox = new System.Windows.Forms.CheckBox();
            this.messageLabel1 = new System.Windows.Forms.Label();
            this.messageLabel2 = new System.Windows.Forms.Label();
            this.messageLabel3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(388, 99);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 14;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // otherWayButton
            // 
            this.otherWayButton.Location = new System.Drawing.Point(388, 36);
            this.otherWayButton.Name = "otherWayButton";
            this.otherWayButton.Size = new System.Drawing.Size(75, 23);
            this.otherWayButton.TabIndex = 15;
            this.otherWayButton.TabStop = false;
            this.otherWayButton.Text = "&Other Way";
            this.otherWayButton.UseVisualStyleBackColor = true;
            this.otherWayButton.Click += new System.EventHandler(this.otherWayButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(307, 99);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 13;
            this.cancelButton.TabStop = false;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // noTerminalCheckBox
            // 
            this.noTerminalCheckBox.AutoSize = true;
            this.noTerminalCheckBox.Location = new System.Drawing.Point(28, 103);
            this.noTerminalCheckBox.Name = "noTerminalCheckBox";
            this.noTerminalCheckBox.Size = new System.Drawing.Size(151, 17);
            this.noTerminalCheckBox.TabIndex = 16;
            this.noTerminalCheckBox.Text = "&Don\'t terminate on any line";
            this.noTerminalCheckBox.UseVisualStyleBackColor = true;
            this.noTerminalCheckBox.CheckedChanged += new System.EventHandler(this.noTerminalCheckBox_CheckedChanged);
            // 
            // messageLabel1
            // 
            this.messageLabel1.AutoSize = true;
            this.messageLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.messageLabel1.Location = new System.Drawing.Point(25, 27);
            this.messageLabel1.Name = "messageLabel1";
            this.messageLabel1.Size = new System.Drawing.Size(312, 16);
            this.messageLabel1.TabIndex = 17;
            this.messageLabel1.Text = "The parallel will terminate on the line that is currently";
            // 
            // messageLabel2
            // 
            this.messageLabel2.AutoSize = true;
            this.messageLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.messageLabel2.Location = new System.Drawing.Point(25, 43);
            this.messageLabel2.Name = "messageLabel2";
            this.messageLabel2.Size = new System.Drawing.Size(301, 16);
            this.messageLabel2.TabIndex = 18;
            this.messageLabel2.Text = "highlighted. Click on a different line to terminate on";
            // 
            // messageLabel3
            // 
            this.messageLabel3.AutoSize = true;
            this.messageLabel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.messageLabel3.Location = new System.Drawing.Point(25, 59);
            this.messageLabel3.Name = "messageLabel3";
            this.messageLabel3.Size = new System.Drawing.Size(291, 16);
            this.messageLabel3.TabIndex = 19;
            this.messageLabel3.Text = "something else. Or click on the checkbox below.";
            // 
            // TerminalControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.messageLabel3);
            this.Controls.Add(this.messageLabel2);
            this.Controls.Add(this.messageLabel1);
            this.Controls.Add(this.noTerminalCheckBox);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.otherWayButton);
            this.Controls.Add(this.cancelButton);
            this.Name = "TerminalControl";
            this.Size = new System.Drawing.Size(530, 150);
            this.Load += new System.EventHandler(this.TerminalControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button otherWayButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox noTerminalCheckBox;
        private System.Windows.Forms.Label messageLabel1;
        private System.Windows.Forms.Label messageLabel2;
        private System.Windows.Forms.Label messageLabel3;
    }
}
