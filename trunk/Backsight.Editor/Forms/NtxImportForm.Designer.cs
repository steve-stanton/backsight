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
    partial class NtxImportForm
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
            this.closeButton = new System.Windows.Forms.Button();
            this.loadButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ntxFileTextBox = new System.Windows.Forms.TextBox();
            this.browseNtxButton = new System.Windows.Forms.Button();
            this.browseFeatureTTbutton = new System.Windows.Forms.Button();
            this.translationTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.loadProgressLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(486, 89);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 0;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // loadButton
            // 
            this.loadButton.Location = new System.Drawing.Point(243, 89);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(75, 23);
            this.loadButton.TabIndex = 1;
            this.loadButton.Text = "Load";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Input file";
            // 
            // ntxFileTextBox
            // 
            this.ntxFileTextBox.Location = new System.Drawing.Point(96, 15);
            this.ntxFileTextBox.Name = "ntxFileTextBox";
            this.ntxFileTextBox.Size = new System.Drawing.Size(371, 20);
            this.ntxFileTextBox.TabIndex = 0;
            // 
            // browseNtxButton
            // 
            this.browseNtxButton.Location = new System.Drawing.Point(486, 13);
            this.browseNtxButton.Name = "browseNtxButton";
            this.browseNtxButton.Size = new System.Drawing.Size(75, 23);
            this.browseNtxButton.TabIndex = 4;
            this.browseNtxButton.TabStop = false;
            this.browseNtxButton.Text = "&Browse...";
            this.browseNtxButton.UseVisualStyleBackColor = true;
            this.browseNtxButton.Click += new System.EventHandler(this.browseNtxButton_Click);
            // 
            // browseFeatureTTbutton
            // 
            this.browseFeatureTTbutton.Location = new System.Drawing.Point(486, 49);
            this.browseFeatureTTbutton.Name = "browseFeatureTTbutton";
            this.browseFeatureTTbutton.Size = new System.Drawing.Size(75, 23);
            this.browseFeatureTTbutton.TabIndex = 7;
            this.browseFeatureTTbutton.TabStop = false;
            this.browseFeatureTTbutton.Text = "&Browse...";
            this.browseFeatureTTbutton.UseVisualStyleBackColor = true;
            this.browseFeatureTTbutton.Click += new System.EventHandler(this.browseFeatureTTbutton_Click);
            // 
            // translationTextBox
            // 
            this.translationTextBox.Location = new System.Drawing.Point(96, 51);
            this.translationTextBox.Name = "translationTextBox";
            this.translationTextBox.Size = new System.Drawing.Size(371, 20);
            this.translationTextBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Feature code";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "translation file";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadProgressLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 123);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(587, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 10;
            this.statusStrip.Text = "statusStrip1";
            // 
            // loadProgressLabel
            // 
            this.loadProgressLabel.Name = "loadProgressLabel";
            this.loadProgressLabel.Size = new System.Drawing.Size(52, 17);
            this.loadProgressLabel.Text = "Loading";
            // 
            // NtxImportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(587, 145);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.browseFeatureTTbutton);
            this.Controls.Add(this.translationTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.browseNtxButton);
            this.Controls.Add(this.ntxFileTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.loadButton);
            this.Controls.Add(this.closeButton);
            this.Name = "NtxImportForm";
            this.Text = "Import NTX";
            this.Shown += new System.EventHandler(this.NtxImportForm_Shown);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ntxFileTextBox;
        private System.Windows.Forms.Button browseNtxButton;
        private System.Windows.Forms.Button browseFeatureTTbutton;
        private System.Windows.Forms.TextBox translationTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel loadProgressLabel;
    }
}
