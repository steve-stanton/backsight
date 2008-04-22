namespace Backsight.Editor.Forms
{
    partial class InverseDistanceAngleForm
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.distance2TextBox = new System.Windows.Forms.TextBox();
            this.cRadioButton = new System.Windows.Forms.RadioButton();
            this.fRadioButton = new System.Windows.Forms.RadioButton();
            this.mRadioButton = new System.Windows.Forms.RadioButton();
            this.distance1TextBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.point3TextBox = new System.Windows.Forms.TextBox();
            this.color3Button = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.point2TextBox = new System.Windows.Forms.TextBox();
            this.color2Button = new System.Windows.Forms.Button();
            this.point1TextBox = new System.Windows.Forms.TextBox();
            this.color1Button = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.ccwRadioButton = new System.Windows.Forms.RadioButton();
            this.cwRadioButton = new System.Windows.Forms.RadioButton();
            this.angleTextBox = new System.Windows.Forms.TextBox();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.Location = new System.Drawing.Point(557, 153);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.distance2TextBox);
            this.groupBox2.Controls.Add(this.cRadioButton);
            this.groupBox2.Controls.Add(this.fRadioButton);
            this.groupBox2.Controls.Add(this.mRadioButton);
            this.groupBox2.Controls.Add(this.distance1TextBox);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(232, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(239, 173);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Distance";
            // 
            // distance2TextBox
            // 
            this.distance2TextBox.Location = new System.Drawing.Point(20, 63);
            this.distance2TextBox.Name = "distance2TextBox";
            this.distance2TextBox.ReadOnly = true;
            this.distance2TextBox.Size = new System.Drawing.Size(198, 22);
            this.distance2TextBox.TabIndex = 5;
            this.distance2TextBox.TabStop = false;
            // 
            // cRadioButton
            // 
            this.cRadioButton.AutoSize = true;
            this.cRadioButton.Location = new System.Drawing.Point(20, 141);
            this.cRadioButton.Name = "cRadioButton";
            this.cRadioButton.Size = new System.Drawing.Size(67, 20);
            this.cRadioButton.TabIndex = 4;
            this.cRadioButton.Text = "&Chains";
            this.cRadioButton.UseVisualStyleBackColor = true;
            this.cRadioButton.CheckedChanged += new System.EventHandler(this.cRadioButton_CheckedChanged);
            // 
            // fRadioButton
            // 
            this.fRadioButton.AutoSize = true;
            this.fRadioButton.Location = new System.Drawing.Point(20, 117);
            this.fRadioButton.Name = "fRadioButton";
            this.fRadioButton.Size = new System.Drawing.Size(53, 20);
            this.fRadioButton.TabIndex = 3;
            this.fRadioButton.Text = "&Feet";
            this.fRadioButton.UseVisualStyleBackColor = true;
            this.fRadioButton.CheckedChanged += new System.EventHandler(this.fRadioButton_CheckedChanged);
            // 
            // mRadioButton
            // 
            this.mRadioButton.AutoSize = true;
            this.mRadioButton.Location = new System.Drawing.Point(20, 93);
            this.mRadioButton.Name = "mRadioButton";
            this.mRadioButton.Size = new System.Drawing.Size(67, 20);
            this.mRadioButton.TabIndex = 2;
            this.mRadioButton.Text = "&Meters";
            this.mRadioButton.UseVisualStyleBackColor = true;
            this.mRadioButton.CheckedChanged += new System.EventHandler(this.mRadioButton_CheckedChanged);
            // 
            // distance1TextBox
            // 
            this.distance1TextBox.Location = new System.Drawing.Point(20, 35);
            this.distance1TextBox.Name = "distance1TextBox";
            this.distance1TextBox.ReadOnly = true;
            this.distance1TextBox.Size = new System.Drawing.Size(198, 22);
            this.distance1TextBox.TabIndex = 1;
            this.distance1TextBox.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.point3TextBox);
            this.groupBox1.Controls.Add(this.color3Button);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.point2TextBox);
            this.groupBox1.Controls.Add(this.color2Button);
            this.groupBox1.Controls.Add(this.point1TextBox);
            this.groupBox1.Controls.Add(this.color1Button);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 173);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Points";
            // 
            // point3TextBox
            // 
            this.point3TextBox.Location = new System.Drawing.Point(57, 91);
            this.point3TextBox.Name = "point3TextBox";
            this.point3TextBox.ReadOnly = true;
            this.point3TextBox.Size = new System.Drawing.Size(100, 22);
            this.point3TextBox.TabIndex = 6;
            this.point3TextBox.TabStop = false;
            // 
            // color3Button
            // 
            this.color3Button.Enabled = false;
            this.color3Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.color3Button.Location = new System.Drawing.Point(22, 91);
            this.color3Button.Name = "color3Button";
            this.color3Button.Size = new System.Drawing.Size(20, 22);
            this.color3Button.TabIndex = 5;
            this.color3Button.TabStop = false;
            this.color3Button.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(66, 121);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "Point at map";
            // 
            // point2TextBox
            // 
            this.point2TextBox.Location = new System.Drawing.Point(57, 63);
            this.point2TextBox.Name = "point2TextBox";
            this.point2TextBox.ReadOnly = true;
            this.point2TextBox.Size = new System.Drawing.Size(100, 22);
            this.point2TextBox.TabIndex = 3;
            this.point2TextBox.TabStop = false;
            // 
            // color2Button
            // 
            this.color2Button.Enabled = false;
            this.color2Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.color2Button.Location = new System.Drawing.Point(22, 63);
            this.color2Button.Name = "color2Button";
            this.color2Button.Size = new System.Drawing.Size(20, 22);
            this.color2Button.TabIndex = 2;
            this.color2Button.TabStop = false;
            this.color2Button.UseVisualStyleBackColor = true;
            // 
            // point1TextBox
            // 
            this.point1TextBox.Location = new System.Drawing.Point(57, 35);
            this.point1TextBox.Name = "point1TextBox";
            this.point1TextBox.ReadOnly = true;
            this.point1TextBox.Size = new System.Drawing.Size(100, 22);
            this.point1TextBox.TabIndex = 1;
            this.point1TextBox.TabStop = false;
            // 
            // color1Button
            // 
            this.color1Button.Enabled = false;
            this.color1Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.color1Button.Location = new System.Drawing.Point(22, 35);
            this.color1Button.Name = "color1Button";
            this.color1Button.Size = new System.Drawing.Size(20, 22);
            this.color1Button.TabIndex = 0;
            this.color1Button.TabStop = false;
            this.color1Button.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.ccwRadioButton);
            this.groupBox3.Controls.Add(this.cwRadioButton);
            this.groupBox3.Controls.Add(this.angleTextBox);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(490, 9);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(200, 128);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Angle";
            // 
            // ccwRadioButton
            // 
            this.ccwRadioButton.AutoSize = true;
            this.ccwRadioButton.Location = new System.Drawing.Point(19, 92);
            this.ccwRadioButton.Name = "ccwRadioButton";
            this.ccwRadioButton.Size = new System.Drawing.Size(135, 20);
            this.ccwRadioButton.TabIndex = 5;
            this.ccwRadioButton.Text = "&Counter-clockwise";
            this.ccwRadioButton.UseVisualStyleBackColor = true;
            this.ccwRadioButton.CheckedChanged += new System.EventHandler(this.ccwRadioButton_CheckedChanged);
            // 
            // cwRadioButton
            // 
            this.cwRadioButton.AutoSize = true;
            this.cwRadioButton.Checked = true;
            this.cwRadioButton.Location = new System.Drawing.Point(19, 68);
            this.cwRadioButton.Name = "cwRadioButton";
            this.cwRadioButton.Size = new System.Drawing.Size(87, 20);
            this.cwRadioButton.TabIndex = 4;
            this.cwRadioButton.TabStop = true;
            this.cwRadioButton.Text = "Clock&wise";
            this.cwRadioButton.UseVisualStyleBackColor = true;
            this.cwRadioButton.CheckedChanged += new System.EventHandler(this.cwRadioButton_CheckedChanged);
            // 
            // angleTextBox
            // 
            this.angleTextBox.Location = new System.Drawing.Point(19, 38);
            this.angleTextBox.Name = "angleTextBox";
            this.angleTextBox.ReadOnly = true;
            this.angleTextBox.Size = new System.Drawing.Size(143, 22);
            this.angleTextBox.TabIndex = 2;
            this.angleTextBox.TabStop = false;
            // 
            // InverseDistanceAngleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(710, 202);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "InverseDistanceAngleForm";
            this.Text = "Inverse Distance & Angle";
            this.TopMost = true;
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox distance2TextBox;
        private System.Windows.Forms.RadioButton cRadioButton;
        private System.Windows.Forms.RadioButton fRadioButton;
        private System.Windows.Forms.RadioButton mRadioButton;
        private System.Windows.Forms.TextBox distance1TextBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox point3TextBox;
        private System.Windows.Forms.Button color3Button;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox point2TextBox;
        private System.Windows.Forms.Button color2Button;
        private System.Windows.Forms.TextBox point1TextBox;
        private System.Windows.Forms.Button color1Button;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton ccwRadioButton;
        private System.Windows.Forms.RadioButton cwRadioButton;
        private System.Windows.Forms.TextBox angleTextBox;
    }
}