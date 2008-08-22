namespace Backsight.Editor.Forms
{
    partial class PublishForm
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
            this.publishButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lastRevisionLabel = new System.Windows.Forms.Label();
            this.numEditLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(178, 166);
            this.closeButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(100, 28);
            this.closeButton.TabIndex = 0;
            this.closeButton.TabStop = false;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // publishButton
            // 
            this.publishButton.Enabled = false;
            this.publishButton.Location = new System.Drawing.Point(329, 107);
            this.publishButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.publishButton.Name = "publishButton";
            this.publishButton.Size = new System.Drawing.Size(100, 28);
            this.publishButton.TabIndex = 1;
            this.publishButton.Text = "&Publish";
            this.publishButton.UseVisualStyleBackColor = true;
            this.publishButton.Click += new System.EventHandler(this.publishButton_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 23);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(348, 69);
            this.label1.TabIndex = 2;
            this.label1.Text = "Publishing makes your recent edits visible to other people and other jobs";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 70);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(215, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Revision number of last publication";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 112);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(178, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Number of unpublished edits";
            // 
            // lastRevisionLabel
            // 
            this.lastRevisionLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lastRevisionLabel.Location = new System.Drawing.Point(238, 64);
            this.lastRevisionLabel.Name = "lastRevisionLabel";
            this.lastRevisionLabel.Size = new System.Drawing.Size(69, 28);
            this.lastRevisionLabel.TabIndex = 5;
            this.lastRevisionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numEditLabel
            // 
            this.numEditLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numEditLabel.Location = new System.Drawing.Point(238, 106);
            this.numEditLabel.Name = "numEditLabel";
            this.numEditLabel.Size = new System.Drawing.Size(69, 28);
            this.numEditLabel.TabIndex = 6;
            this.numEditLabel.Text = "0";
            this.numEditLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PublishForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(466, 215);
            this.Controls.Add(this.numEditLabel);
            this.Controls.Add(this.lastRevisionLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.publishButton);
            this.Controls.Add(this.closeButton);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "PublishForm";
            this.Text = "Publish Changes";
            this.Shown += new System.EventHandler(this.PublishForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button publishButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lastRevisionLabel;
        private System.Windows.Forms.Label numEditLabel;
    }
}