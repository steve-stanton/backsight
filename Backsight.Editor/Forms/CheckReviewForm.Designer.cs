namespace Backsight.Editor.Forms
{
    partial class CheckReviewForm
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
            this.statusGroupBox = new System.Windows.Forms.GroupBox();
            this.explanationLabel = new System.Windows.Forms.Label();
            this.previousButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.statusGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusGroupBox
            // 
            this.statusGroupBox.Controls.Add(this.explanationLabel);
            this.statusGroupBox.Location = new System.Drawing.Point(14, 13);
            this.statusGroupBox.Name = "statusGroupBox";
            this.statusGroupBox.Size = new System.Drawing.Size(269, 94);
            this.statusGroupBox.TabIndex = 0;
            this.statusGroupBox.TabStop = false;
            // 
            // explanationLabel
            // 
            this.explanationLabel.AutoSize = true;
            this.explanationLabel.Location = new System.Drawing.Point(22, 31);
            this.explanationLabel.Name = "explanationLabel";
            this.explanationLabel.Size = new System.Drawing.Size(101, 13);
            this.explanationLabel.TabIndex = 0;
            this.explanationLabel.Text = "Check in progress...";
            // 
            // previousButton
            // 
            this.previousButton.Location = new System.Drawing.Point(62, 120);
            this.previousButton.Name = "previousButton";
            this.previousButton.Size = new System.Drawing.Size(75, 23);
            this.previousButton.TabIndex = 1;
            this.previousButton.Text = "&Back";
            this.previousButton.UseVisualStyleBackColor = true;
            this.previousButton.Click += new System.EventHandler(this.previousButton_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(161, 120);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "&First";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // CheckReviewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(295, 155);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.previousButton);
            this.Controls.Add(this.statusGroupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "CheckReviewForm";
            this.Text = "File Check Review";
            this.Shown += new System.EventHandler(this.CheckReviewForm_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CheckReviewForm_FormClosing);
            this.statusGroupBox.ResumeLayout(false);
            this.statusGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox statusGroupBox;
        private System.Windows.Forms.Label explanationLabel;
        private System.Windows.Forms.Button previousButton;
        private System.Windows.Forms.Button okButton;
    }
}