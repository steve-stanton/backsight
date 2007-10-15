namespace Backsight.Editor.Forms
{
    partial class InverseDistanceForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.point2TextBox = new System.Windows.Forms.TextBox();
            this.color2Button = new System.Windows.Forms.Button();
            this.point1TextBox = new System.Windows.Forms.TextBox();
            this.color1Button = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cRadioButton = new System.Windows.Forms.RadioButton();
            this.fRadioButton = new System.Windows.Forms.RadioButton();
            this.mRadioButton = new System.Windows.Forms.RadioButton();
            this.distanceTextBox = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.point2TextBox);
            this.groupBox1.Controls.Add(this.color2Button);
            this.groupBox1.Controls.Add(this.point1TextBox);
            this.groupBox1.Controls.Add(this.color1Button);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 148);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Points";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(66, 98);
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
            // 
            // color2Button
            // 
            this.color2Button.Enabled = false;
            this.color2Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.color2Button.Location = new System.Drawing.Point(22, 63);
            this.color2Button.Name = "color2Button";
            this.color2Button.Size = new System.Drawing.Size(20, 22);
            this.color2Button.TabIndex = 2;
            this.color2Button.UseVisualStyleBackColor = true;
            // 
            // point1TextBox
            // 
            this.point1TextBox.Location = new System.Drawing.Point(57, 35);
            this.point1TextBox.Name = "point1TextBox";
            this.point1TextBox.ReadOnly = true;
            this.point1TextBox.Size = new System.Drawing.Size(100, 22);
            this.point1TextBox.TabIndex = 1;
            // 
            // color1Button
            // 
            this.color1Button.Enabled = false;
            this.color1Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.color1Button.Location = new System.Drawing.Point(22, 35);
            this.color1Button.Name = "color1Button";
            this.color1Button.Size = new System.Drawing.Size(20, 22);
            this.color1Button.TabIndex = 0;
            this.color1Button.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cRadioButton);
            this.groupBox2.Controls.Add(this.fRadioButton);
            this.groupBox2.Controls.Add(this.mRadioButton);
            this.groupBox2.Controls.Add(this.distanceTextBox);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(232, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(239, 148);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Distance";
            // 
            // cRadioButton
            // 
            this.cRadioButton.AutoSize = true;
            this.cRadioButton.Location = new System.Drawing.Point(20, 117);
            this.cRadioButton.Name = "cRadioButton";
            this.cRadioButton.Size = new System.Drawing.Size(67, 20);
            this.cRadioButton.TabIndex = 4;
            this.cRadioButton.TabStop = true;
            this.cRadioButton.Text = "&Chains";
            this.cRadioButton.UseVisualStyleBackColor = true;
            this.cRadioButton.CheckedChanged += new System.EventHandler(this.cRadioButton_CheckedChanged);
            // 
            // fRadioButton
            // 
            this.fRadioButton.AutoSize = true;
            this.fRadioButton.Location = new System.Drawing.Point(20, 91);
            this.fRadioButton.Name = "fRadioButton";
            this.fRadioButton.Size = new System.Drawing.Size(53, 20);
            this.fRadioButton.TabIndex = 3;
            this.fRadioButton.TabStop = true;
            this.fRadioButton.Text = "&Feet";
            this.fRadioButton.UseVisualStyleBackColor = true;
            this.fRadioButton.CheckedChanged += new System.EventHandler(this.fRadioButton_CheckedChanged);
            // 
            // mRadioButton
            // 
            this.mRadioButton.AutoSize = true;
            this.mRadioButton.Location = new System.Drawing.Point(20, 65);
            this.mRadioButton.Name = "mRadioButton";
            this.mRadioButton.Size = new System.Drawing.Size(67, 20);
            this.mRadioButton.TabIndex = 2;
            this.mRadioButton.TabStop = true;
            this.mRadioButton.Text = "&Meters";
            this.mRadioButton.UseVisualStyleBackColor = true;
            this.mRadioButton.CheckedChanged += new System.EventHandler(this.mRadioButton_CheckedChanged);
            // 
            // distanceTextBox
            // 
            this.distanceTextBox.Location = new System.Drawing.Point(20, 35);
            this.distanceTextBox.Name = "distanceTextBox";
            this.distanceTextBox.ReadOnly = true;
            this.distanceTextBox.Size = new System.Drawing.Size(198, 22);
            this.distanceTextBox.TabIndex = 1;
            // 
            // cancelButton
            // 
            this.cancelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.Location = new System.Drawing.Point(500, 126);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // InverseDistanceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(599, 173);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "InverseDistanceForm";
            this.Text = "Inverse Distance";
            this.TopMost = true;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox point1TextBox;
        private System.Windows.Forms.Button color1Button;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox point2TextBox;
        private System.Windows.Forms.Button color2Button;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox distanceTextBox;
        private System.Windows.Forms.RadioButton cRadioButton;
        private System.Windows.Forms.RadioButton fRadioButton;
        private System.Windows.Forms.RadioButton mRadioButton;
        private System.Windows.Forms.Button cancelButton;
    }
}
