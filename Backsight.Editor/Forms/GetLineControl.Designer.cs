namespace Backsight.Editor.Forms
{
    partial class GetLineControl
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.gotLineCheckBox = new System.Windows.Forms.CheckBox();
            this.questionGroupBox = new System.Windows.Forms.GroupBox();
            this.noCheckBox = new System.Windows.Forms.CheckBox();
            this.yesCheckBox = new System.Windows.Forms.CheckBox();
            this.noSplitButton = new System.Windows.Forms.Button();
            this.wantSplitButton = new System.Windows.Forms.Button();
            this.polBoundaryLabel = new System.Windows.Forms.Label();
            this.deletedLabel = new System.Windows.Forms.Label();
            this.questionGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(101, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(222, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Point at the line you want to intersect.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(56, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(267, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "If you point at the wrong line, just point again.";
            // 
            // gotLineCheckBox
            // 
            this.gotLineCheckBox.AutoSize = true;
            this.gotLineCheckBox.Location = new System.Drawing.Point(328, 19);
            this.gotLineCheckBox.Name = "gotLineCheckBox";
            this.gotLineCheckBox.Size = new System.Drawing.Size(15, 14);
            this.gotLineCheckBox.TabIndex = 2;
            this.gotLineCheckBox.UseVisualStyleBackColor = true;
            // 
            // questionGroupBox
            // 
            this.questionGroupBox.Controls.Add(this.noCheckBox);
            this.questionGroupBox.Controls.Add(this.yesCheckBox);
            this.questionGroupBox.Controls.Add(this.noSplitButton);
            this.questionGroupBox.Controls.Add(this.wantSplitButton);
            this.questionGroupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.questionGroupBox.Location = new System.Drawing.Point(59, 57);
            this.questionGroupBox.Name = "questionGroupBox";
            this.questionGroupBox.Size = new System.Drawing.Size(308, 74);
            this.questionGroupBox.TabIndex = 3;
            this.questionGroupBox.TabStop = false;
            this.questionGroupBox.Text = "Split selected line at the intersection?";
            this.questionGroupBox.Visible = false;
            // 
            // noCheckBox
            // 
            this.noCheckBox.AutoSize = true;
            this.noCheckBox.Enabled = false;
            this.noCheckBox.Location = new System.Drawing.Point(220, 37);
            this.noCheckBox.Name = "noCheckBox";
            this.noCheckBox.Size = new System.Drawing.Size(15, 14);
            this.noCheckBox.TabIndex = 4;
            this.noCheckBox.UseVisualStyleBackColor = true;
            // 
            // yesCheckBox
            // 
            this.yesCheckBox.AutoSize = true;
            this.yesCheckBox.Enabled = false;
            this.yesCheckBox.Location = new System.Drawing.Point(122, 37);
            this.yesCheckBox.Name = "yesCheckBox";
            this.yesCheckBox.Size = new System.Drawing.Size(15, 14);
            this.yesCheckBox.TabIndex = 3;
            this.yesCheckBox.UseVisualStyleBackColor = true;
            // 
            // noSplitButton
            // 
            this.noSplitButton.Location = new System.Drawing.Point(169, 32);
            this.noSplitButton.Name = "noSplitButton";
            this.noSplitButton.Size = new System.Drawing.Size(75, 23);
            this.noSplitButton.TabIndex = 1;
            this.noSplitButton.Text = "&No";
            this.noSplitButton.UseVisualStyleBackColor = true;
            this.noSplitButton.Click += new System.EventHandler(this.noSplitButton_Click);
            // 
            // wantSplitButton
            // 
            this.wantSplitButton.Location = new System.Drawing.Point(68, 32);
            this.wantSplitButton.Name = "wantSplitButton";
            this.wantSplitButton.Size = new System.Drawing.Size(75, 23);
            this.wantSplitButton.TabIndex = 0;
            this.wantSplitButton.Text = "&Yes";
            this.wantSplitButton.UseVisualStyleBackColor = true;
            this.wantSplitButton.Click += new System.EventHandler(this.wantSplitButton_Click);
            // 
            // polBoundaryLabel
            // 
            this.polBoundaryLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.polBoundaryLabel.Location = new System.Drawing.Point(59, 134);
            this.polBoundaryLabel.Name = "polBoundaryLabel";
            this.polBoundaryLabel.Size = new System.Drawing.Size(308, 16);
            this.polBoundaryLabel.TabIndex = 4;
            this.polBoundaryLabel.Text = "line is a polygon boundary";
            this.polBoundaryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.polBoundaryLabel.Visible = false;
            // 
            // deletedLabel
            // 
            this.deletedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deletedLabel.Location = new System.Drawing.Point(59, 150);
            this.deletedLabel.Name = "deletedLabel";
            this.deletedLabel.Size = new System.Drawing.Size(308, 16);
            this.deletedLabel.TabIndex = 5;
            this.deletedLabel.Text = "line has been de-activated via a deletion";
            this.deletedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.deletedLabel.Visible = false;
            // 
            // GetLineControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.deletedLabel);
            this.Controls.Add(this.polBoundaryLabel);
            this.Controls.Add(this.questionGroupBox);
            this.Controls.Add(this.gotLineCheckBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "GetLineControl";
            this.Size = new System.Drawing.Size(424, 179);
            this.questionGroupBox.ResumeLayout(false);
            this.questionGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox gotLineCheckBox;
        private System.Windows.Forms.GroupBox questionGroupBox;
        private System.Windows.Forms.Button noSplitButton;
        private System.Windows.Forms.Button wantSplitButton;
        private System.Windows.Forms.Label polBoundaryLabel;
        private System.Windows.Forms.Label deletedLabel;
        private System.Windows.Forms.CheckBox noCheckBox;
        private System.Windows.Forms.CheckBox yesCheckBox;
    }
}
