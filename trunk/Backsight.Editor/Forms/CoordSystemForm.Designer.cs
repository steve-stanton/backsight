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
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.projectionLabel = new System.Windows.Forms.Label();
            this.ellipsoidLabel = new System.Windows.Forms.Label();
            this.zoneUpDown = new System.Windows.Forms.NumericUpDown();
            this.meanElevationTextBox = new System.Windows.Forms.TextBox();
            this.geoidSeparationTextBox = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.zoneUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Map Projection";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(75, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Ellipsoid";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(96, 134);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "Zone";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(35, 169);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 16);
            this.label4.TabIndex = 3;
            this.label4.Text = "Mean elevation";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(23, 204);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(112, 16);
            this.label5.TabIndex = 4;
            this.label5.Text = "Geoid separation";
            // 
            // projectionLabel
            // 
            this.projectionLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.projectionLabel.Location = new System.Drawing.Point(155, 33);
            this.projectionLabel.Name = "projectionLabel";
            this.projectionLabel.Size = new System.Drawing.Size(100, 17);
            this.projectionLabel.TabIndex = 5;
            this.projectionLabel.Text = "UTM";
            // 
            // ellipsoidLabel
            // 
            this.ellipsoidLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ellipsoidLabel.Location = new System.Drawing.Point(155, 70);
            this.ellipsoidLabel.Name = "ellipsoidLabel";
            this.ellipsoidLabel.Size = new System.Drawing.Size(100, 17);
            this.ellipsoidLabel.TabIndex = 6;
            this.ellipsoidLabel.Text = "NAD83";
            // 
            // zoneUpDown
            // 
            this.zoneUpDown.Location = new System.Drawing.Point(155, 132);
            this.zoneUpDown.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.zoneUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.zoneUpDown.Name = "zoneUpDown";
            this.zoneUpDown.Size = new System.Drawing.Size(100, 22);
            this.zoneUpDown.TabIndex = 8;
            this.zoneUpDown.TabStop = false;
            this.zoneUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // meanElevationTextBox
            // 
            this.meanElevationTextBox.Location = new System.Drawing.Point(155, 166);
            this.meanElevationTextBox.Name = "meanElevationTextBox";
            this.meanElevationTextBox.Size = new System.Drawing.Size(100, 22);
            this.meanElevationTextBox.TabIndex = 9;
            this.meanElevationTextBox.TabStop = false;
            // 
            // geoidSeparationTextBox
            // 
            this.geoidSeparationTextBox.Location = new System.Drawing.Point(155, 201);
            this.geoidSeparationTextBox.Name = "geoidSeparationTextBox";
            this.geoidSeparationTextBox.Size = new System.Drawing.Size(100, 22);
            this.geoidSeparationTextBox.TabIndex = 10;
            this.geoidSeparationTextBox.TabStop = false;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(155, 262);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(100, 30);
            this.okButton.TabIndex = 11;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // CoordSystemForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(372, 328);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.geoidSeparationTextBox);
            this.Controls.Add(this.meanElevationTextBox);
            this.Controls.Add(this.zoneUpDown);
            this.Controls.Add(this.ellipsoidLabel);
            this.Controls.Add(this.projectionLabel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "CoordSystemForm";
            this.Text = "Coordinate System";
            this.Shown += new System.EventHandler(this.CoordSystemForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.zoneUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label projectionLabel;
        private System.Windows.Forms.Label ellipsoidLabel;
        private System.Windows.Forms.NumericUpDown zoneUpDown;
        private System.Windows.Forms.TextBox meanElevationTextBox;
        private System.Windows.Forms.TextBox geoidSeparationTextBox;
        private System.Windows.Forms.Button okButton;
    }
}