namespace Backsight.Editor.Forms
{
    partial class CoordSystemForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.epsgNumberLabel = new System.Windows.Forms.Label();
            this.meanElevationTextBox = new System.Windows.Forms.TextBox();
            this.geoidSeparationTextBox = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.systemNameLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(45, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "System name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(39, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "EPSG Number";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(38, 121);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 16);
            this.label4.TabIndex = 3;
            this.label4.Text = "Mean elevation";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(26, 156);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(112, 16);
            this.label5.TabIndex = 4;
            this.label5.Text = "Geoid separation";
            // 
            // epsgNumberLabel
            // 
            this.epsgNumberLabel.AutoSize = true;
            this.epsgNumberLabel.Location = new System.Drawing.Point(155, 60);
            this.epsgNumberLabel.Name = "epsgNumberLabel";
            this.epsgNumberLabel.Size = new System.Drawing.Size(60, 16);
            this.epsgNumberLabel.TabIndex = 6;
            this.epsgNumberLabel.Text = "unknown";
            // 
            // meanElevationTextBox
            // 
            this.meanElevationTextBox.Location = new System.Drawing.Point(158, 118);
            this.meanElevationTextBox.Name = "meanElevationTextBox";
            this.meanElevationTextBox.Size = new System.Drawing.Size(100, 22);
            this.meanElevationTextBox.TabIndex = 9;
            this.meanElevationTextBox.TabStop = false;
            // 
            // geoidSeparationTextBox
            // 
            this.geoidSeparationTextBox.Location = new System.Drawing.Point(158, 153);
            this.geoidSeparationTextBox.Name = "geoidSeparationTextBox";
            this.geoidSeparationTextBox.Size = new System.Drawing.Size(100, 22);
            this.geoidSeparationTextBox.TabIndex = 10;
            this.geoidSeparationTextBox.TabStop = false;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(169, 211);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(80, 27);
            this.okButton.TabIndex = 11;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // systemNameLabel
            // 
            this.systemNameLabel.AutoSize = true;
            this.systemNameLabel.Location = new System.Drawing.Point(155, 34);
            this.systemNameLabel.Name = "systemNameLabel";
            this.systemNameLabel.Size = new System.Drawing.Size(60, 16);
            this.systemNameLabel.TabIndex = 12;
            this.systemNameLabel.Text = "unknown";
            // 
            // CoordSystemForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 269);
            this.Controls.Add(this.systemNameLabel);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.geoidSeparationTextBox);
            this.Controls.Add(this.meanElevationTextBox);
            this.Controls.Add(this.epsgNumberLabel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "CoordSystemForm";
            this.Text = "Coordinate System";
            this.Shown += new System.EventHandler(this.CoordSystemForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label epsgNumberLabel;
        private System.Windows.Forms.TextBox meanElevationTextBox;
        private System.Windows.Forms.TextBox geoidSeparationTextBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label systemNameLabel;
    }
}