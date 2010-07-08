namespace Backsight.Editor.Forms
{
    partial class GetDirectionControl
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
            this.lineTypeGroupBox = new System.Windows.Forms.GroupBox();
            this.lineTypeComboBox = new Backsight.Editor.Forms.EntityTypeComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.directionGroupBox = new System.Windows.Forms.GroupBox();
            this.counterClockwiseRadioButton = new System.Windows.Forms.RadioButton();
            this.clockwiseRadioButton = new System.Windows.Forms.RadioButton();
            this.useCenterCheckBox = new System.Windows.Forms.CheckBox();
            this.directionTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.backsightTextBox = new System.Windows.Forms.TextBox();
            this.fromPointTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.setDefaultOffsetButton = new System.Windows.Forms.Button();
            this.rightRadioButton = new System.Windows.Forms.RadioButton();
            this.leftRadioButton = new System.Windows.Forms.RadioButton();
            this.offsetTextBox = new System.Windows.Forms.TextBox();
            this.lineTypeGroupBox.SuspendLayout();
            this.directionGroupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lineTypeGroupBox
            // 
            this.lineTypeGroupBox.Controls.Add(this.lineTypeComboBox);
            this.lineTypeGroupBox.Controls.Add(this.label3);
            this.lineTypeGroupBox.Controls.Add(this.label4);
            this.lineTypeGroupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lineTypeGroupBox.Location = new System.Drawing.Point(513, 15);
            this.lineTypeGroupBox.Name = "lineTypeGroupBox";
            this.lineTypeGroupBox.Size = new System.Drawing.Size(223, 160);
            this.lineTypeGroupBox.TabIndex = 3;
            this.lineTypeGroupBox.TabStop = false;
            this.lineTypeGroupBox.Text = "Line type";
            // 
            // lineTypeComboBox
            // 
            this.lineTypeComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lineTypeComboBox.FormattingEnabled = true;
            this.lineTypeComboBox.Location = new System.Drawing.Point(15, 29);
            this.lineTypeComboBox.Name = "lineTypeComboBox";
            this.lineTypeComboBox.ShowBlankEntityType = true;
            this.lineTypeComboBox.Size = new System.Drawing.Size(194, 24);
            this.lineTypeComboBox.TabIndex = 4;
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
            // directionGroupBox
            // 
            this.directionGroupBox.Controls.Add(this.counterClockwiseRadioButton);
            this.directionGroupBox.Controls.Add(this.clockwiseRadioButton);
            this.directionGroupBox.Controls.Add(this.useCenterCheckBox);
            this.directionGroupBox.Controls.Add(this.directionTextBox);
            this.directionGroupBox.Controls.Add(this.label6);
            this.directionGroupBox.Controls.Add(this.label5);
            this.directionGroupBox.Controls.Add(this.backsightTextBox);
            this.directionGroupBox.Controls.Add(this.fromPointTextBox);
            this.directionGroupBox.Controls.Add(this.label2);
            this.directionGroupBox.Controls.Add(this.label1);
            this.directionGroupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.directionGroupBox.Location = new System.Drawing.Point(18, 15);
            this.directionGroupBox.Name = "directionGroupBox";
            this.directionGroupBox.Size = new System.Drawing.Size(322, 160);
            this.directionGroupBox.TabIndex = 2;
            this.directionGroupBox.TabStop = false;
            this.directionGroupBox.Text = "Direction";
            // 
            // counterClockwiseRadioButton
            // 
            this.counterClockwiseRadioButton.AutoSize = true;
            this.counterClockwiseRadioButton.Location = new System.Drawing.Point(167, 123);
            this.counterClockwiseRadioButton.Name = "counterClockwiseRadioButton";
            this.counterClockwiseRadioButton.Size = new System.Drawing.Size(135, 20);
            this.counterClockwiseRadioButton.TabIndex = 9;
            this.counterClockwiseRadioButton.Text = "&Counter-clockwise";
            this.counterClockwiseRadioButton.UseVisualStyleBackColor = true;
            this.counterClockwiseRadioButton.CheckedChanged += new System.EventHandler(this.counterClockwiseRadioButton_CheckedChanged);
            // 
            // clockwiseRadioButton
            // 
            this.clockwiseRadioButton.AutoSize = true;
            this.clockwiseRadioButton.Location = new System.Drawing.Point(167, 106);
            this.clockwiseRadioButton.Name = "clockwiseRadioButton";
            this.clockwiseRadioButton.Size = new System.Drawing.Size(87, 20);
            this.clockwiseRadioButton.TabIndex = 8;
            this.clockwiseRadioButton.Text = "Clock&wise";
            this.clockwiseRadioButton.UseVisualStyleBackColor = true;
            this.clockwiseRadioButton.CheckedChanged += new System.EventHandler(this.clockwiseRadioButton_CheckedChanged);
            // 
            // useCenterCheckBox
            // 
            this.useCenterCheckBox.AutoSize = true;
            this.useCenterCheckBox.Location = new System.Drawing.Point(167, 73);
            this.useCenterCheckBox.Name = "useCenterCheckBox";
            this.useCenterCheckBox.Size = new System.Drawing.Size(146, 20);
            this.useCenterCheckBox.TabIndex = 7;
            this.useCenterCheckBox.TabStop = false;
            this.useCenterCheckBox.Text = "&Use Center of Curve";
            this.useCenterCheckBox.UseVisualStyleBackColor = true;
            this.useCenterCheckBox.CheckedChanged += new System.EventHandler(this.useCenterCheckBox_CheckedChanged);
            // 
            // directionTextBox
            // 
            this.directionTextBox.Location = new System.Drawing.Point(87, 111);
            this.directionTextBox.Name = "directionTextBox";
            this.directionTextBox.Size = new System.Drawing.Size(74, 22);
            this.directionTextBox.TabIndex = 2;
            this.directionTextBox.TextChanged += new System.EventHandler(this.directionTextBox_TextChanged);
            this.directionTextBox.Leave += new System.EventHandler(this.directionTextBox_Leave);
            this.directionTextBox.Enter += new System.EventHandler(this.directionTextBox_Enter);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 114);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 16);
            this.label6.TabIndex = 5;
            this.label6.Text = "Angle";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 83);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 16);
            this.label5.TabIndex = 4;
            this.label5.Text = "(optional)";
            // 
            // backsightTextBox
            // 
            this.backsightTextBox.Location = new System.Drawing.Point(87, 71);
            this.backsightTextBox.Name = "backsightTextBox";
            this.backsightTextBox.Size = new System.Drawing.Size(74, 22);
            this.backsightTextBox.TabIndex = 1;
            this.backsightTextBox.TextChanged += new System.EventHandler(this.backsightTextBox_TextChanged);
            this.backsightTextBox.Leave += new System.EventHandler(this.backsightTextBox_Leave);
            this.backsightTextBox.Enter += new System.EventHandler(this.backsightTextBox_Enter);
            // 
            // fromPointTextBox
            // 
            this.fromPointTextBox.Location = new System.Drawing.Point(87, 29);
            this.fromPointTextBox.Name = "fromPointTextBox";
            this.fromPointTextBox.Size = new System.Drawing.Size(74, 22);
            this.fromPointTextBox.TabIndex = 0;
            this.fromPointTextBox.TextChanged += new System.EventHandler(this.fromPointTextBox_TextChanged);
            this.fromPointTextBox.Leave += new System.EventHandler(this.fromPointTextBox_Leave);
            this.fromPointTextBox.Enter += new System.EventHandler(this.fromPointTextBox_Enter);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Backsight";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "From point";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.setDefaultOffsetButton);
            this.groupBox1.Controls.Add(this.rightRadioButton);
            this.groupBox1.Controls.Add(this.leftRadioButton);
            this.groupBox1.Controls.Add(this.offsetTextBox);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(356, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(141, 160);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Offset";
            // 
            // setDefaultOffsetButton
            // 
            this.setDefaultOffsetButton.Location = new System.Drawing.Point(16, 112);
            this.setDefaultOffsetButton.Name = "setDefaultOffsetButton";
            this.setDefaultOffsetButton.Size = new System.Drawing.Size(110, 23);
            this.setDefaultOffsetButton.TabIndex = 11;
            this.setDefaultOffsetButton.TabStop = false;
            this.setDefaultOffsetButton.Text = "Set As Default";
            this.setDefaultOffsetButton.UseVisualStyleBackColor = true;
            this.setDefaultOffsetButton.Click += new System.EventHandler(this.setDefaultOffsetButton_Click);
            // 
            // rightRadioButton
            // 
            this.rightRadioButton.AutoSize = true;
            this.rightRadioButton.Location = new System.Drawing.Point(69, 60);
            this.rightRadioButton.Name = "rightRadioButton";
            this.rightRadioButton.Size = new System.Drawing.Size(57, 20);
            this.rightRadioButton.TabIndex = 10;
            this.rightRadioButton.Text = "&Right";
            this.rightRadioButton.UseVisualStyleBackColor = true;
            this.rightRadioButton.CheckedChanged += new System.EventHandler(this.rightRadioButton_CheckedChanged);
            // 
            // leftRadioButton
            // 
            this.leftRadioButton.AutoSize = true;
            this.leftRadioButton.Location = new System.Drawing.Point(16, 60);
            this.leftRadioButton.Name = "leftRadioButton";
            this.leftRadioButton.Size = new System.Drawing.Size(47, 20);
            this.leftRadioButton.TabIndex = 9;
            this.leftRadioButton.Text = "&Left";
            this.leftRadioButton.UseVisualStyleBackColor = true;
            this.leftRadioButton.CheckedChanged += new System.EventHandler(this.leftRadioButton_CheckedChanged);
            // 
            // offsetTextBox
            // 
            this.offsetTextBox.Location = new System.Drawing.Point(16, 29);
            this.offsetTextBox.Name = "offsetTextBox";
            this.offsetTextBox.Size = new System.Drawing.Size(110, 22);
            this.offsetTextBox.TabIndex = 3;
            this.offsetTextBox.TextChanged += new System.EventHandler(this.offsetTextBox_TextChanged);
            this.offsetTextBox.Leave += new System.EventHandler(this.offsetTextBox_Leave);
            this.offsetTextBox.Enter += new System.EventHandler(this.offsetTextBox_Enter);
            // 
            // GetDirectionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lineTypeGroupBox);
            this.Controls.Add(this.directionGroupBox);
            this.Name = "GetDirectionControl";
            this.Size = new System.Drawing.Size(752, 191);
            this.lineTypeGroupBox.ResumeLayout(false);
            this.lineTypeGroupBox.PerformLayout();
            this.directionGroupBox.ResumeLayout(false);
            this.directionGroupBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox lineTypeGroupBox;
        private EntityTypeComboBox lineTypeComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox directionGroupBox;
        private System.Windows.Forms.RadioButton counterClockwiseRadioButton;
        private System.Windows.Forms.RadioButton clockwiseRadioButton;
        private System.Windows.Forms.CheckBox useCenterCheckBox;
        private System.Windows.Forms.TextBox directionTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox backsightTextBox;
        private System.Windows.Forms.TextBox fromPointTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button setDefaultOffsetButton;
        private System.Windows.Forms.RadioButton rightRadioButton;
        private System.Windows.Forms.RadioButton leftRadioButton;
        private System.Windows.Forms.TextBox offsetTextBox;
    }
}
