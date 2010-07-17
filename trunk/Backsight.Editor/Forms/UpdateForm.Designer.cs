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
    partial class UpdateForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.infoTextBox = new System.Windows.Forms.TextBox();
            this.dependenciesButton = new System.Windows.Forms.Button();
            this.predecessorsButton = new System.Windows.Forms.Button();
            this.numUpdateLabel = new System.Windows.Forms.Label();
            this.updateButton = new System.Windows.Forms.Button();
            this.finishButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.lightBox = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lightBox)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.infoTextBox);
            this.groupBox1.Controls.Add(this.dependenciesButton);
            this.groupBox1.Controls.Add(this.predecessorsButton);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(347, 138);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // infoTextBox
            // 
            this.infoTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.infoTextBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.infoTextBox.Location = new System.Drawing.Point(17, 22);
            this.infoTextBox.Multiline = true;
            this.infoTextBox.Name = "infoTextBox";
            this.infoTextBox.ReadOnly = true;
            this.infoTextBox.Size = new System.Drawing.Size(310, 63);
            this.infoTextBox.TabIndex = 7;
            this.infoTextBox.TabStop = false;
            this.infoTextBox.Text = "Nothing selected for update";
            this.infoTextBox.WordWrap = false;
            // 
            // dependenciesButton
            // 
            this.dependenciesButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.dependenciesButton.Location = new System.Drawing.Point(188, 98);
            this.dependenciesButton.Name = "dependenciesButton";
            this.dependenciesButton.Size = new System.Drawing.Size(114, 23);
            this.dependenciesButton.TabIndex = 1;
            this.dependenciesButton.TabStop = false;
            this.dependenciesButton.Text = "&Dependencies...";
            this.dependenciesButton.UseVisualStyleBackColor = true;
            this.dependenciesButton.Click += new System.EventHandler(this.dependenciesButton_Click);
            // 
            // predecessorsButton
            // 
            this.predecessorsButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.predecessorsButton.Location = new System.Drawing.Point(53, 98);
            this.predecessorsButton.Name = "predecessorsButton";
            this.predecessorsButton.Size = new System.Drawing.Size(114, 23);
            this.predecessorsButton.TabIndex = 0;
            this.predecessorsButton.TabStop = false;
            this.predecessorsButton.Text = "P&redecessors...";
            this.predecessorsButton.UseVisualStyleBackColor = true;
            this.predecessorsButton.Click += new System.EventHandler(this.predecessorsButton_Click);
            // 
            // numUpdateLabel
            // 
            this.numUpdateLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numUpdateLabel.Location = new System.Drawing.Point(12, 159);
            this.numUpdateLabel.Name = "numUpdateLabel";
            this.numUpdateLabel.Size = new System.Drawing.Size(348, 25);
            this.numUpdateLabel.TabIndex = 1;
            this.numUpdateLabel.Text = "Update count";
            this.numUpdateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.numUpdateLabel.Visible = false;
            // 
            // updateButton
            // 
            this.updateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.updateButton.Location = new System.Drawing.Point(376, 41);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(75, 23);
            this.updateButton.TabIndex = 2;
            this.updateButton.Text = "&Update...";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // finishButton
            // 
            this.finishButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.finishButton.Location = new System.Drawing.Point(376, 75);
            this.finishButton.Name = "finishButton";
            this.finishButton.Size = new System.Drawing.Size(75, 23);
            this.finishButton.TabIndex = 3;
            this.finishButton.Text = "&Finish";
            this.finishButton.UseVisualStyleBackColor = true;
            this.finishButton.Click += new System.EventHandler(this.finishButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(376, 111);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.TabStop = false;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // lightBox
            // 
            this.lightBox.Image = ((System.Drawing.Image)(resources.GetObject("lightBox.Image")));
            this.lightBox.Location = new System.Drawing.Point(463, 37);
            this.lightBox.Name = "lightBox";
            this.lightBox.Size = new System.Drawing.Size(32, 32);
            this.lightBox.TabIndex = 5;
            this.lightBox.TabStop = false;
            // 
            // UpdateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(510, 193);
            this.Controls.Add(this.lightBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.finishButton);
            this.Controls.Add(this.updateButton);
            this.Controls.Add(this.numUpdateLabel);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "UpdateForm";
            this.Text = "Update";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.UpdateForm_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UpdateForm_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lightBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button dependenciesButton;
        private System.Windows.Forms.Button predecessorsButton;
        private System.Windows.Forms.Label numUpdateLabel;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.Button finishButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.PictureBox lightBox;
        private System.Windows.Forms.TextBox infoTextBox;
    }
}
