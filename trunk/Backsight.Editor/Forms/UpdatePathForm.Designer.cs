namespace Backsight.Editor.Forms
{
    partial class UpdatePathForm
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
            this.secondFaceLabel = new System.Windows.Forms.Label();
            this.previousButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.precisionLabel = new System.Windows.Forms.Label();
            this.updateButton = new System.Windows.Forms.Button();
            this.insertButton = new System.Windows.Forms.Button();
            this.angleButton = new System.Windows.Forms.Button();
            this.breakButton = new System.Windows.Forms.Button();
            this.curveButton = new System.Windows.Forms.Button();
            this.newFaceButton = new System.Windows.Forms.Button();
            this.flipDistButton = new System.Windows.Forms.Button();
            this.insBeforeRadioButton = new System.Windows.Forms.RadioButton();
            this.insAfterRadioButton = new System.Windows.Forms.RadioButton();
            this.brkAfterRadioButton = new System.Windows.Forms.RadioButton();
            this.brkBeforeRadioButton = new System.Windows.Forms.RadioButton();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.distancesListBox = new System.Windows.Forms.ListBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // secondFaceLabel
            // 
            this.secondFaceLabel.Location = new System.Drawing.Point(17, 334);
            this.secondFaceLabel.Name = "secondFaceLabel";
            this.secondFaceLabel.Size = new System.Drawing.Size(158, 20);
            this.secondFaceLabel.TabIndex = 19;
            this.secondFaceLabel.Text = "(second face)";
            this.secondFaceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // previousButton
            // 
            this.previousButton.Location = new System.Drawing.Point(17, 372);
            this.previousButton.Name = "previousButton";
            this.previousButton.Size = new System.Drawing.Size(82, 28);
            this.previousButton.TabIndex = 1;
            this.previousButton.Text = "&Back";
            this.previousButton.UseVisualStyleBackColor = true;
            this.previousButton.Click += new System.EventHandler(this.previousButton_Click);
            // 
            // nextButton
            // 
            this.nextButton.Location = new System.Drawing.Point(105, 372);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(70, 28);
            this.nextButton.TabIndex = 2;
            this.nextButton.Text = "&Next";
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // precisionLabel
            // 
            this.precisionLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.precisionLabel.Location = new System.Drawing.Point(17, 416);
            this.precisionLabel.Name = "precisionLabel";
            this.precisionLabel.Size = new System.Drawing.Size(158, 28);
            this.precisionLabel.TabIndex = 22;
            this.precisionLabel.Text = "Precision";
            this.precisionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // updateButton
            // 
            this.updateButton.Location = new System.Drawing.Point(269, 18);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(111, 28);
            this.updateButton.TabIndex = 23;
            this.updateButton.TabStop = false;
            this.updateButton.Text = "&Update...";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // insertButton
            // 
            this.insertButton.Location = new System.Drawing.Point(269, 65);
            this.insertButton.Name = "insertButton";
            this.insertButton.Size = new System.Drawing.Size(111, 28);
            this.insertButton.TabIndex = 24;
            this.insertButton.TabStop = false;
            this.insertButton.Text = "&Insert...";
            this.insertButton.UseVisualStyleBackColor = true;
            this.insertButton.Click += new System.EventHandler(this.insertButton_Click);
            // 
            // angleButton
            // 
            this.angleButton.Location = new System.Drawing.Point(269, 112);
            this.angleButton.Name = "angleButton";
            this.angleButton.Size = new System.Drawing.Size(111, 28);
            this.angleButton.TabIndex = 25;
            this.angleButton.TabStop = false;
            this.angleButton.Text = "&Angle...";
            this.angleButton.UseVisualStyleBackColor = true;
            this.angleButton.Click += new System.EventHandler(this.angleButton_Click);
            // 
            // breakButton
            // 
            this.breakButton.Location = new System.Drawing.Point(269, 159);
            this.breakButton.Name = "breakButton";
            this.breakButton.Size = new System.Drawing.Size(111, 28);
            this.breakButton.TabIndex = 26;
            this.breakButton.TabStop = false;
            this.breakButton.Text = "Brea&k";
            this.breakButton.UseVisualStyleBackColor = true;
            this.breakButton.Click += new System.EventHandler(this.breakButton_Click);
            // 
            // curveButton
            // 
            this.curveButton.Location = new System.Drawing.Point(269, 206);
            this.curveButton.Name = "curveButton";
            this.curveButton.Size = new System.Drawing.Size(111, 28);
            this.curveButton.TabIndex = 29;
            this.curveButton.TabStop = false;
            this.curveButton.Text = "&Circular arc...";
            this.curveButton.UseVisualStyleBackColor = true;
            this.curveButton.Click += new System.EventHandler(this.curveButton_Click);
            // 
            // newFaceButton
            // 
            this.newFaceButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.newFaceButton.Location = new System.Drawing.Point(269, 301);
            this.newFaceButton.Name = "newFaceButton";
            this.newFaceButton.Size = new System.Drawing.Size(111, 28);
            this.newFaceButton.TabIndex = 31;
            this.newFaceButton.TabStop = false;
            this.newFaceButton.Text = "New Fac&e";
            this.newFaceButton.UseVisualStyleBackColor = true;
            this.newFaceButton.Click += new System.EventHandler(this.newFaceButton_Click);
            // 
            // flipDistButton
            // 
            this.flipDistButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.flipDistButton.Location = new System.Drawing.Point(269, 254);
            this.flipDistButton.Name = "flipDistButton";
            this.flipDistButton.Size = new System.Drawing.Size(111, 28);
            this.flipDistButton.TabIndex = 30;
            this.flipDistButton.TabStop = false;
            this.flipDistButton.Text = "Flip Ann&otations";
            this.flipDistButton.UseVisualStyleBackColor = true;
            this.flipDistButton.Click += new System.EventHandler(this.flipDistButton_Click);
            // 
            // insBeforeRadioButton
            // 
            this.insBeforeRadioButton.AutoSize = true;
            this.insBeforeRadioButton.Location = new System.Drawing.Point(10, 15);
            this.insBeforeRadioButton.Name = "insBeforeRadioButton";
            this.insBeforeRadioButton.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.insBeforeRadioButton.Size = new System.Drawing.Size(66, 20);
            this.insBeforeRadioButton.TabIndex = 32;
            this.insBeforeRadioButton.Text = "B&efore";
            this.insBeforeRadioButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.insBeforeRadioButton.UseVisualStyleBackColor = true;
            // 
            // insAfterRadioButton
            // 
            this.insAfterRadioButton.AutoSize = true;
            this.insAfterRadioButton.Location = new System.Drawing.Point(23, 35);
            this.insAfterRadioButton.Name = "insAfterRadioButton";
            this.insAfterRadioButton.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.insAfterRadioButton.Size = new System.Drawing.Size(53, 20);
            this.insAfterRadioButton.TabIndex = 33;
            this.insAfterRadioButton.Text = "Af&ter";
            this.insAfterRadioButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.insAfterRadioButton.UseVisualStyleBackColor = true;
            // 
            // brkAfterRadioButton
            // 
            this.brkAfterRadioButton.AutoSize = true;
            this.brkAfterRadioButton.Location = new System.Drawing.Point(23, 34);
            this.brkAfterRadioButton.Name = "brkAfterRadioButton";
            this.brkAfterRadioButton.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.brkAfterRadioButton.Size = new System.Drawing.Size(53, 20);
            this.brkAfterRadioButton.TabIndex = 35;
            this.brkAfterRadioButton.Text = "Afte&r";
            this.brkAfterRadioButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.brkAfterRadioButton.UseVisualStyleBackColor = true;
            // 
            // brkBeforeRadioButton
            // 
            this.brkBeforeRadioButton.AutoSize = true;
            this.brkBeforeRadioButton.Location = new System.Drawing.Point(10, 15);
            this.brkBeforeRadioButton.Name = "brkBeforeRadioButton";
            this.brkBeforeRadioButton.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.brkBeforeRadioButton.Size = new System.Drawing.Size(66, 20);
            this.brkBeforeRadioButton.TabIndex = 34;
            this.brkBeforeRadioButton.Text = "Bef&ore";
            this.brkBeforeRadioButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.brkBeforeRadioButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.okButton.Location = new System.Drawing.Point(269, 372);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(111, 28);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "&Finish";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cancelButton.Location = new System.Drawing.Point(269, 416);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(111, 28);
            this.cancelButton.TabIndex = 36;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.brkBeforeRadioButton);
            this.panel1.Controls.Add(this.brkAfterRadioButton);
            this.panel1.Location = new System.Drawing.Point(183, 140);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(80, 70);
            this.panel1.TabIndex = 37;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.insAfterRadioButton);
            this.panel2.Controls.Add(this.insBeforeRadioButton);
            this.panel2.Location = new System.Drawing.Point(183, 43);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(80, 75);
            this.panel2.TabIndex = 38;
            // 
            // distancesListBox
            // 
            this.distancesListBox.FormattingEnabled = true;
            this.distancesListBox.ItemHeight = 16;
            this.distancesListBox.Location = new System.Drawing.Point(22, 23);
            this.distancesListBox.Name = "distancesListBox";
            this.distancesListBox.Size = new System.Drawing.Size(155, 308);
            this.distancesListBox.TabIndex = 39;
            this.distancesListBox.SelectedValueChanged += new System.EventHandler(this.distancesListBox_SelectedValueChanged);
            this.distancesListBox.DoubleClick += new System.EventHandler(this.distancesListBox_DoubleClick);
            // 
            // UpdatePathForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(406, 466);
            this.Controls.Add(this.distancesListBox);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.newFaceButton);
            this.Controls.Add(this.flipDistButton);
            this.Controls.Add(this.curveButton);
            this.Controls.Add(this.breakButton);
            this.Controls.Add(this.angleButton);
            this.Controls.Add(this.insertButton);
            this.Controls.Add(this.updateButton);
            this.Controls.Add(this.precisionLabel);
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.previousButton);
            this.Controls.Add(this.secondFaceLabel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "UpdatePathForm";
            this.Text = "Connection Path Leg";
            this.Shown += new System.EventHandler(this.UpdatePathForm_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label secondFaceLabel;
        private System.Windows.Forms.Button previousButton;
        private System.Windows.Forms.Button nextButton;
        private System.Windows.Forms.Label precisionLabel;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.Button insertButton;
        private System.Windows.Forms.Button angleButton;
        private System.Windows.Forms.Button breakButton;
        private System.Windows.Forms.Button curveButton;
        private System.Windows.Forms.Button newFaceButton;
        private System.Windows.Forms.Button flipDistButton;
        private System.Windows.Forms.RadioButton insBeforeRadioButton;
        private System.Windows.Forms.RadioButton insAfterRadioButton;
        private System.Windows.Forms.RadioButton brkAfterRadioButton;
        private System.Windows.Forms.RadioButton brkBeforeRadioButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ListBox distancesListBox;
    }
}