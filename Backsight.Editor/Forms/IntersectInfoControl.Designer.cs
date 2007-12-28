namespace Backsight.Editor.Forms
{
    partial class IntersectInfoControl
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
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.pointIdComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.eastingLabel = new System.Windows.Forms.Label();
            this.northingLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.otherButton = new System.Windows.Forms.Button();
            this.pointTypeComboBox = new Backsight.Editor.Forms.EntityTypeComboBox();
            this.closestPointExplanationLabel = new System.Windows.Forms.Label();
            this.closestPointTrimLabel = new System.Windows.Forms.Label();
            this.closestPointValueLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(456, 36);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(111, 16);
            this.label5.TabIndex = 18;
            this.label5.Text = "at the intersection";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(456, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(149, 16);
            this.label4.TabIndex = 17;
            this.label4.Text = "Specify the type of entity";
            // 
            // pointIdComboBox
            // 
            this.pointIdComboBox.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pointIdComboBox.FormattingEnabled = true;
            this.pointIdComboBox.Location = new System.Drawing.Point(295, 50);
            this.pointIdComboBox.Name = "pointIdComboBox";
            this.pointIdComboBox.Size = new System.Drawing.Size(133, 24);
            this.pointIdComboBox.TabIndex = 16;
            this.pointIdComboBox.SelectedValueChanged += new System.EventHandler(this.pointIdComboBox_SelectedValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(269, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 16);
            this.label3.TabIndex = 15;
            this.label3.Text = "ID";
            // 
            // eastingLabel
            // 
            this.eastingLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.eastingLabel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.eastingLabel.Location = new System.Drawing.Point(109, 51);
            this.eastingLabel.Name = "eastingLabel";
            this.eastingLabel.Size = new System.Drawing.Size(137, 23);
            this.eastingLabel.TabIndex = 13;
            this.eastingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // northingLabel
            // 
            this.northingLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.northingLabel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.northingLabel.Location = new System.Drawing.Point(109, 18);
            this.northingLabel.Name = "northingLabel";
            this.northingLabel.Size = new System.Drawing.Size(137, 23);
            this.northingLabel.TabIndex = 12;
            this.northingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(48, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 16);
            this.label2.TabIndex = 11;
            this.label2.Text = "Easting";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(41, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 16);
            this.label1.TabIndex = 10;
            this.label1.Text = "Northing";
            // 
            // otherButton
            // 
            this.otherButton.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.otherButton.Location = new System.Drawing.Point(109, 88);
            this.otherButton.Name = "otherButton";
            this.otherButton.Size = new System.Drawing.Size(137, 23);
            this.otherButton.TabIndex = 19;
            this.otherButton.Text = "&Other Intersection";
            this.otherButton.UseVisualStyleBackColor = true;
            this.otherButton.Click += new System.EventHandler(this.otherButton_Click);
            // 
            // pointTypeComboBox
            // 
            this.pointTypeComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pointTypeComboBox.FormattingEnabled = true;
            this.pointTypeComboBox.Location = new System.Drawing.Point(272, 17);
            this.pointTypeComboBox.Name = "pointTypeComboBox";
            this.pointTypeComboBox.Size = new System.Drawing.Size(178, 24);
            this.pointTypeComboBox.TabIndex = 20;
            this.pointTypeComboBox.SelectedValueChanged += new System.EventHandler(this.pointTypeComboBox_SelectedValueChanged);
            // 
            // closestPointExplanationLabel
            // 
            this.closestPointExplanationLabel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closestPointExplanationLabel.Location = new System.Drawing.Point(269, 130);
            this.closestPointExplanationLabel.Name = "closestPointExplanationLabel";
            this.closestPointExplanationLabel.Size = new System.Drawing.Size(353, 59);
            this.closestPointExplanationLabel.TabIndex = 21;
            this.closestPointExplanationLabel.Text = "The closest point (in red) is used to relocate the intersection in the event that" +
    " the intersection moves due to subsequent edits. Specify an alternative point if" +
    " desired.";
            this.closestPointExplanationLabel.Visible = false;
            // 
            // closestPointTrimLabel
            // 
            this.closestPointTrimLabel.AutoSize = true;
            this.closestPointTrimLabel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closestPointTrimLabel.Location = new System.Drawing.Point(16, 130);
            this.closestPointTrimLabel.Name = "closestPointTrimLabel";
            this.closestPointTrimLabel.Size = new System.Drawing.Size(81, 16);
            this.closestPointTrimLabel.TabIndex = 22;
            this.closestPointTrimLabel.Text = "Closest Point";
            this.closestPointTrimLabel.Visible = false;
            // 
            // closestPointValueLabel
            // 
            this.closestPointValueLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.closestPointValueLabel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closestPointValueLabel.Location = new System.Drawing.Point(109, 127);
            this.closestPointValueLabel.Name = "closestPointValueLabel";
            this.closestPointValueLabel.Size = new System.Drawing.Size(137, 23);
            this.closestPointValueLabel.TabIndex = 23;
            this.closestPointValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.closestPointValueLabel.Visible = false;
            // 
            // IntersectInfoControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.closestPointValueLabel);
            this.Controls.Add(this.closestPointTrimLabel);
            this.Controls.Add(this.closestPointExplanationLabel);
            this.Controls.Add(this.pointTypeComboBox);
            this.Controls.Add(this.otherButton);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pointIdComboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.eastingLabel);
            this.Controls.Add(this.northingLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "IntersectInfoControl";
            this.Size = new System.Drawing.Size(628, 196);
            this.VisibleChanged += new System.EventHandler(this.IntersectInfoControl_VisibleChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox pointIdComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label eastingLabel;
        private System.Windows.Forms.Label northingLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button otherButton;
        private EntityTypeComboBox pointTypeComboBox;
        private System.Windows.Forms.Label closestPointExplanationLabel;
        private System.Windows.Forms.Label closestPointTrimLabel;
        private System.Windows.Forms.Label closestPointValueLabel;
    }
}
