namespace Backsight.Editor.Forms
{
    partial class InverseArcDistanceBearingForm
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.bearing2TextBox = new System.Windows.Forms.TextBox();
            this.bearing1TextBox = new System.Windows.Forms.TextBox();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.bearing2TextBox);
            this.groupBox3.Controls.Add(this.bearing1TextBox);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(491, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(160, 100);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Bearings To Center";
            // 
            // bearing2TextBox
            // 
            this.bearing2TextBox.Location = new System.Drawing.Point(19, 63);
            this.bearing2TextBox.Name = "bearing2TextBox";
            this.bearing2TextBox.ReadOnly = true;
            this.bearing2TextBox.Size = new System.Drawing.Size(119, 22);
            this.bearing2TextBox.TabIndex = 3;
            // 
            // bearing1TextBox
            // 
            this.bearing1TextBox.Location = new System.Drawing.Point(19, 35);
            this.bearing1TextBox.Name = "bearing1TextBox";
            this.bearing1TextBox.ReadOnly = true;
            this.bearing1TextBox.Size = new System.Drawing.Size(119, 22);
            this.bearing1TextBox.TabIndex = 2;
            // 
            // InverseArcDistanceBearingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(667, 173);
            this.Controls.Add(this.groupBox3);
            this.Name = "InverseArcDistanceBearingForm";
            this.Text = "Arc Distance & Bearings to Center";
            this.Controls.SetChildIndex(this.groupBox3, 0);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox bearing2TextBox;
        private System.Windows.Forms.TextBox bearing1TextBox;
    }
}