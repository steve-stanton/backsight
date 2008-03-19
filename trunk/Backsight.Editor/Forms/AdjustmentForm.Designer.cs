namespace Backsight.Editor.Forms
{
    partial class AdjustmentForm
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
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.mRadioButton = new System.Windows.Forms.RadioButton();
            this.cRadioButton = new System.Windows.Forms.RadioButton();
            this.fRadioButton = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lengthLabel = new System.Windows.Forms.Label();
            this.deltaNorthingLabel = new System.Windows.Forms.Label();
            this.deltaEastingLabel = new System.Windows.Forms.Label();
            this.precisionLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.Location = new System.Drawing.Point(17, 226);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cancelButton.Size = new System.Drawing.Size(100, 30);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Reject";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.okButton.Location = new System.Drawing.Point(124, 226);
            this.okButton.Name = "okButton";
            this.okButton.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.okButton.Size = new System.Drawing.Size(100, 30);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // mRadioButton
            // 
            this.mRadioButton.AutoSize = true;
            this.mRadioButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mRadioButton.Location = new System.Drawing.Point(21, 21);
            this.mRadioButton.Name = "mRadioButton";
            this.mRadioButton.Size = new System.Drawing.Size(67, 20);
            this.mRadioButton.TabIndex = 2;
            this.mRadioButton.Text = "Meters";
            this.mRadioButton.UseVisualStyleBackColor = true;
            this.mRadioButton.CheckedChanged += new System.EventHandler(this.mRadioButton_CheckedChanged);
            // 
            // cRadioButton
            // 
            this.cRadioButton.AutoSize = true;
            this.cRadioButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cRadioButton.Location = new System.Drawing.Point(157, 21);
            this.cRadioButton.Name = "cRadioButton";
            this.cRadioButton.Size = new System.Drawing.Size(67, 20);
            this.cRadioButton.TabIndex = 3;
            this.cRadioButton.Text = "Chains";
            this.cRadioButton.UseVisualStyleBackColor = true;
            this.cRadioButton.CheckedChanged += new System.EventHandler(this.cRadioButton_CheckedChanged);
            // 
            // fRadioButton
            // 
            this.fRadioButton.AutoSize = true;
            this.fRadioButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fRadioButton.Location = new System.Drawing.Point(95, 21);
            this.fRadioButton.Name = "fRadioButton";
            this.fRadioButton.Size = new System.Drawing.Size(53, 20);
            this.fRadioButton.TabIndex = 4;
            this.fRadioButton.Text = "Feet";
            this.fRadioButton.UseVisualStyleBackColor = true;
            this.fRadioButton.CheckedChanged += new System.EventHandler(this.fRadioButton_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(18, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 32);
            this.label1.TabIndex = 5;
            this.label1.Text = "Observed length";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(92, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 16);
            this.label2.TabIndex = 6;
            this.label2.Text = "dN";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(92, 138);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 16);
            this.label3.TabIndex = 7;
            this.label3.Text = "dE";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(18, 165);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 32);
            this.label4.TabIndex = 8;
            this.label4.Text = "Precision";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lengthLabel
            // 
            this.lengthLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lengthLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lengthLabel.Location = new System.Drawing.Point(124, 65);
            this.lengthLabel.Name = "lengthLabel";
            this.lengthLabel.Size = new System.Drawing.Size(100, 23);
            this.lengthLabel.TabIndex = 9;
            this.lengthLabel.Text = "0.0";
            this.lengthLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // deltaNorthingLabel
            // 
            this.deltaNorthingLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.deltaNorthingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deltaNorthingLabel.Location = new System.Drawing.Point(124, 100);
            this.deltaNorthingLabel.Name = "deltaNorthingLabel";
            this.deltaNorthingLabel.Size = new System.Drawing.Size(100, 23);
            this.deltaNorthingLabel.TabIndex = 10;
            this.deltaNorthingLabel.Text = "0.0";
            this.deltaNorthingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // deltaEastingLabel
            // 
            this.deltaEastingLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.deltaEastingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deltaEastingLabel.Location = new System.Drawing.Point(124, 135);
            this.deltaEastingLabel.Name = "deltaEastingLabel";
            this.deltaEastingLabel.Size = new System.Drawing.Size(100, 23);
            this.deltaEastingLabel.TabIndex = 11;
            this.deltaEastingLabel.Text = "0.0";
            this.deltaEastingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // precisionLabel
            // 
            this.precisionLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.precisionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.precisionLabel.Location = new System.Drawing.Point(124, 170);
            this.precisionLabel.Name = "precisionLabel";
            this.precisionLabel.Size = new System.Drawing.Size(100, 23);
            this.precisionLabel.TabIndex = 12;
            this.precisionLabel.Text = "1:0";
            this.precisionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // AdjustmentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(254, 268);
            this.Controls.Add(this.precisionLabel);
            this.Controls.Add(this.deltaEastingLabel);
            this.Controls.Add(this.deltaNorthingLabel);
            this.Controls.Add(this.lengthLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.fRadioButton);
            this.Controls.Add(this.cRadioButton);
            this.Controls.Add(this.mRadioButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AdjustmentForm";
            this.Text = "Adjustment Results";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.AdjustmentForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.RadioButton mRadioButton;
        private System.Windows.Forms.RadioButton cRadioButton;
        private System.Windows.Forms.RadioButton fRadioButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lengthLabel;
        private System.Windows.Forms.Label deltaNorthingLabel;
        private System.Windows.Forms.Label deltaEastingLabel;
        private System.Windows.Forms.Label precisionLabel;
    }
}