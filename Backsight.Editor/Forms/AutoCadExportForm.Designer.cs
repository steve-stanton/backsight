namespace Backsight.Editor.Forms
{
    partial class AutoCadExportForm
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
            this.browseButton = new System.Windows.Forms.Button();
            this.entFileTextBox = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.versionComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.arcPolylineCheckBox = new System.Windows.Forms.CheckBox();
            this.othDataRadioButton = new System.Windows.Forms.RadioButton();
            this.polDataRadioButton = new System.Windows.Forms.RadioButton();
            this.themeLabel = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.browseButton);
            this.groupBox1.Controls.Add(this.entFileTextBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 22);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(562, 80);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Entity Type to Layer Translation (optional)";
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(441, 36);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(75, 23);
            this.browseButton.TabIndex = 1;
            this.browseButton.Text = "&Browse...";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // entFileTextBox
            // 
            this.entFileTextBox.Location = new System.Drawing.Point(22, 36);
            this.entFileTextBox.Name = "entFileTextBox";
            this.entFileTextBox.Size = new System.Drawing.Size(395, 22);
            this.entFileTextBox.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.versionComboBox);
            this.groupBox2.Location = new System.Drawing.Point(13, 130);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(162, 159);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "AutoCad Version";
            // 
            // versionComboBox
            // 
            this.versionComboBox.FormattingEnabled = true;
            this.versionComboBox.Items.AddRange(new object[] {
            "2007",
            "2004",
            "2000",
            "12"});
            this.versionComboBox.Location = new System.Drawing.Point(21, 31);
            this.versionComboBox.Name = "versionComboBox";
            this.versionComboBox.Size = new System.Drawing.Size(121, 24);
            this.versionComboBox.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.arcPolylineCheckBox);
            this.groupBox3.Controls.Add(this.othDataRadioButton);
            this.groupBox3.Controls.Add(this.polDataRadioButton);
            this.groupBox3.Controls.Add(this.themeLabel);
            this.groupBox3.Location = new System.Drawing.Point(202, 130);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(372, 159);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Output Filter";
            // 
            // arcPolylineCheckBox
            // 
            this.arcPolylineCheckBox.AutoSize = true;
            this.arcPolylineCheckBox.Location = new System.Drawing.Point(195, 86);
            this.arcPolylineCheckBox.Name = "arcPolylineCheckBox";
            this.arcPolylineCheckBox.Size = new System.Drawing.Size(162, 20);
            this.arcPolylineCheckBox.TabIndex = 3;
            this.arcPolylineCheckBox.Text = "Write &arcs as polylines";
            this.arcPolylineCheckBox.UseVisualStyleBackColor = true;
            // 
            // othDataRadioButton
            // 
            this.othDataRadioButton.AutoSize = true;
            this.othDataRadioButton.Location = new System.Drawing.Point(32, 100);
            this.othDataRadioButton.Name = "othDataRadioButton";
            this.othDataRadioButton.Size = new System.Drawing.Size(114, 20);
            this.othDataRadioButton.TabIndex = 2;
            this.othDataRadioButton.TabStop = true;
            this.othDataRadioButton.Text = "&Other Features";
            this.othDataRadioButton.UseVisualStyleBackColor = true;
            // 
            // polDataRadioButton
            // 
            this.polDataRadioButton.AutoSize = true;
            this.polDataRadioButton.Location = new System.Drawing.Point(32, 74);
            this.polDataRadioButton.Name = "polDataRadioButton";
            this.polDataRadioButton.Size = new System.Drawing.Size(132, 20);
            this.polDataRadioButton.TabIndex = 1;
            this.polDataRadioButton.TabStop = true;
            this.polDataRadioButton.Text = "&Polygon Features";
            this.polDataRadioButton.UseVisualStyleBackColor = true;
            // 
            // themeLabel
            // 
            this.themeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.themeLabel.Location = new System.Drawing.Point(29, 31);
            this.themeLabel.Name = "themeLabel";
            this.themeLabel.Size = new System.Drawing.Size(315, 24);
            this.themeLabel.TabIndex = 0;
            this.themeLabel.Text = "Currently drawn theme";
            this.themeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(204, 316);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(308, 316);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "&Save";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // AutoCadExportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(603, 368);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "AutoCadExportForm";
            this.Text = "Export to AutoCad";
            this.Load += new System.EventHandler(this.AutoCadExportForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.TextBox entFileTextBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox versionComboBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox arcPolylineCheckBox;
        private System.Windows.Forms.RadioButton othDataRadioButton;
        private System.Windows.Forms.RadioButton polDataRadioButton;
        private System.Windows.Forms.Label themeLabel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
    }
}