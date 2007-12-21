namespace Backsight.Editor.Forms
{
    partial class GetDistanceControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.distanceGroupBox = new System.Windows.Forms.GroupBox();
            this.distanceTextBox = new System.Windows.Forms.TextBox();
            this.fromPointTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lineTypeComboBox = new Backsight.Editor.Forms.EntityTypeComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.distanceGroupBox.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // distanceGroupBox
            // 
            this.distanceGroupBox.Controls.Add(this.distanceTextBox);
            this.distanceGroupBox.Controls.Add(this.fromPointTextBox);
            this.distanceGroupBox.Controls.Add(this.label2);
            this.distanceGroupBox.Controls.Add(this.label1);
            this.distanceGroupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.distanceGroupBox.Location = new System.Drawing.Point(26, 27);
            this.distanceGroupBox.Name = "distanceGroupBox";
            this.distanceGroupBox.Size = new System.Drawing.Size(235, 126);
            this.distanceGroupBox.TabIndex = 0;
            this.distanceGroupBox.TabStop = false;
            this.distanceGroupBox.Text = "Distance";
            // 
            // distanceTextBox
            // 
            this.distanceTextBox.Location = new System.Drawing.Point(101, 73);
            this.distanceTextBox.Name = "distanceTextBox";
            this.distanceTextBox.Size = new System.Drawing.Size(100, 22);
            this.distanceTextBox.TabIndex = 3;
            this.distanceTextBox.Enter += new System.EventHandler(this.distanceTextBox_Enter);
            this.distanceTextBox.Leave += new System.EventHandler(this.distanceTextBox_Leave);
            this.distanceTextBox.TextChanged += new System.EventHandler(this.distanceTextBox_TextChanged);
            // 
            // fromPointTextBox
            // 
            this.fromPointTextBox.Location = new System.Drawing.Point(101, 31);
            this.fromPointTextBox.Name = "fromPointTextBox";
            this.fromPointTextBox.Size = new System.Drawing.Size(100, 22);
            this.fromPointTextBox.TabIndex = 2;
            this.fromPointTextBox.Enter += new System.EventHandler(this.fromPointTextBox_Enter);
            this.fromPointTextBox.Leave += new System.EventHandler(this.fromPointTextBox_Leave);
            this.fromPointTextBox.TextChanged += new System.EventHandler(this.fromPointTextBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Distance";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "From point";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lineTypeComboBox);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(291, 27);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(247, 126);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Line type";
            // 
            // lineTypeComboBox
            // 
            this.lineTypeComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lineTypeComboBox.FormattingEnabled = true;
            this.lineTypeComboBox.Location = new System.Drawing.Point(15, 29);
            this.lineTypeComboBox.Name = "lineTypeComboBox";
            this.lineTypeComboBox.Size = new System.Drawing.Size(213, 24);
            this.lineTypeComboBox.TabIndex = 3;
            this.lineTypeComboBox.SelectedValueChanged += new System.EventHandler(this.lineTypeComboBox_SelectedValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(114, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "to the intersection.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(152, 16);
            this.label4.TabIndex = 0;
            this.label4.Text = "Specify if you want a line";
            // 
            // GetDistanceControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.distanceGroupBox);
            this.Name = "GetDistanceControl";
            this.Size = new System.Drawing.Size(568, 175);
            this.distanceGroupBox.ResumeLayout(false);
            this.distanceGroupBox.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox distanceGroupBox;
        private System.Windows.Forms.TextBox distanceTextBox;
        private System.Windows.Forms.TextBox fromPointTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private EntityTypeComboBox lineTypeComboBox;
    }
}
