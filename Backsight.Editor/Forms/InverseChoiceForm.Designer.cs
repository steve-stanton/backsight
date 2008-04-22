namespace Backsight.Editor.Forms
{
    partial class InverseChoiceForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.distanceAngleButton = new System.Windows.Forms.Button();
            this.distanceBearingButton = new System.Windows.Forms.Button();
            this.distanceButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.arcDistanceBearingButton = new System.Windows.Forms.Button();
            this.arcDistanceButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.distanceAngleButton);
            this.groupBox1.Controls.Add(this.distanceBearingButton);
            this.groupBox1.Controls.Add(this.distanceButton);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(25, 25);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 157);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Point to Point";
            // 
            // distanceAngleButton
            // 
            this.distanceAngleButton.Location = new System.Drawing.Point(29, 113);
            this.distanceAngleButton.Name = "distanceAngleButton";
            this.distanceAngleButton.Size = new System.Drawing.Size(145, 25);
            this.distanceAngleButton.TabIndex = 2;
            this.distanceAngleButton.TabStop = false;
            this.distanceAngleButton.Text = "Distance - &Angle...";
            this.distanceAngleButton.UseVisualStyleBackColor = true;
            this.distanceAngleButton.Click += new System.EventHandler(this.distanceAngleButton_Click);
            // 
            // distanceBearingButton
            // 
            this.distanceBearingButton.Location = new System.Drawing.Point(29, 71);
            this.distanceBearingButton.Name = "distanceBearingButton";
            this.distanceBearingButton.Size = new System.Drawing.Size(145, 25);
            this.distanceBearingButton.TabIndex = 1;
            this.distanceBearingButton.TabStop = false;
            this.distanceBearingButton.Text = "Distance - &Bearing...";
            this.distanceBearingButton.UseVisualStyleBackColor = true;
            this.distanceBearingButton.Click += new System.EventHandler(this.distanceBearingButton_Click);
            // 
            // distanceButton
            // 
            this.distanceButton.Location = new System.Drawing.Point(29, 27);
            this.distanceButton.Name = "distanceButton";
            this.distanceButton.Size = new System.Drawing.Size(145, 25);
            this.distanceButton.TabIndex = 0;
            this.distanceButton.TabStop = false;
            this.distanceButton.Text = "&Distance...";
            this.distanceButton.UseVisualStyleBackColor = true;
            this.distanceButton.Click += new System.EventHandler(this.distanceButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.arcDistanceBearingButton);
            this.groupBox2.Controls.Add(this.arcDistanceButton);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(256, 25);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(268, 114);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Points on Circular Arcs";
            // 
            // arcDistanceBearingButton
            // 
            this.arcDistanceBearingButton.Location = new System.Drawing.Point(21, 71);
            this.arcDistanceBearingButton.Name = "arcDistanceBearingButton";
            this.arcDistanceBearingButton.Size = new System.Drawing.Size(230, 23);
            this.arcDistanceBearingButton.TabIndex = 4;
            this.arcDistanceBearingButton.TabStop = false;
            this.arcDistanceBearingButton.Text = "Arc Distance - &Centre Bearing...";
            this.arcDistanceBearingButton.UseVisualStyleBackColor = true;
            this.arcDistanceBearingButton.Click += new System.EventHandler(this.arcDistanceBearingButton_Click);
            // 
            // arcDistanceButton
            // 
            this.arcDistanceButton.Location = new System.Drawing.Point(21, 27);
            this.arcDistanceButton.Name = "arcDistanceButton";
            this.arcDistanceButton.Size = new System.Drawing.Size(230, 25);
            this.arcDistanceButton.TabIndex = 3;
            this.arcDistanceButton.TabStop = false;
            this.arcDistanceButton.Text = "A&rc Distance...";
            this.arcDistanceButton.UseVisualStyleBackColor = true;
            this.arcDistanceButton.Click += new System.EventHandler(this.arcDistanceButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.Location = new System.Drawing.Point(352, 159);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.TabStop = false;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // InverseChoiceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 214);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "InverseChoiceForm";
            this.Text = "Inverse Calculator";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button distanceBearingButton;
        private System.Windows.Forms.Button distanceButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button distanceAngleButton;
        private System.Windows.Forms.Button arcDistanceBearingButton;
        private System.Windows.Forms.Button arcDistanceButton;
        private System.Windows.Forms.Button cancelButton;
    }
}