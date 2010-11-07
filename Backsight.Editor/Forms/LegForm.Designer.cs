namespace Backsight.Editor.Forms
{
    partial class LegForm
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
            this.distancesTextBox = new System.Windows.Forms.TextBox();
            this.lengthLeftTextBox = new System.Windows.Forms.TextBox();
            this.totalEnteredTextBox = new System.Windows.Forms.TextBox();
            this.lengthTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chainsRadioButton = new System.Windows.Forms.RadioButton();
            this.feetRadioButton = new System.Windows.Forms.RadioButton();
            this.metersRadioButton = new System.Windows.Forms.RadioButton();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // distancesTextBox
            // 
            this.distancesTextBox.AcceptsReturn = true;
            this.distancesTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.distancesTextBox.Location = new System.Drawing.Point(16, 16);
            this.distancesTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.distancesTextBox.Multiline = true;
            this.distancesTextBox.Name = "distancesTextBox";
            this.distancesTextBox.Size = new System.Drawing.Size(111, 159);
            this.distancesTextBox.TabIndex = 17;
            this.distancesTextBox.WordWrap = false;
            this.distancesTextBox.TextChanged += new System.EventHandler(this.distancesTextBox_TextChanged);
            // 
            // lengthLeftTextBox
            // 
            this.lengthLeftTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lengthLeftTextBox.Location = new System.Drawing.Point(150, 116);
            this.lengthLeftTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.lengthLeftTextBox.Name = "lengthLeftTextBox";
            this.lengthLeftTextBox.ReadOnly = true;
            this.lengthLeftTextBox.Size = new System.Drawing.Size(99, 22);
            this.lengthLeftTextBox.TabIndex = 23;
            this.lengthLeftTextBox.TabStop = false;
            // 
            // totalEnteredTextBox
            // 
            this.totalEnteredTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.totalEnteredTextBox.Location = new System.Drawing.Point(150, 86);
            this.totalEnteredTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.totalEnteredTextBox.Name = "totalEnteredTextBox";
            this.totalEnteredTextBox.ReadOnly = true;
            this.totalEnteredTextBox.Size = new System.Drawing.Size(99, 22);
            this.totalEnteredTextBox.TabIndex = 22;
            this.totalEnteredTextBox.TabStop = false;
            // 
            // lengthTextBox
            // 
            this.lengthTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lengthTextBox.Location = new System.Drawing.Point(150, 56);
            this.lengthTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.lengthTextBox.Name = "lengthTextBox";
            this.lengthTextBox.ReadOnly = true;
            this.lengthTextBox.Size = new System.Drawing.Size(99, 22);
            this.lengthTextBox.TabIndex = 21;
            this.lengthTextBox.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(17, 118);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 16);
            this.label3.TabIndex = 20;
            this.label3.Text = "Length left to enter";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(44, 88);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 16);
            this.label2.TabIndex = 19;
            this.label2.Text = "Total entered";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(48, 58);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 16);
            this.label1.TabIndex = 18;
            this.label1.Text = "Length of leg";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.chainsRadioButton);
            this.groupBox1.Controls.Add(this.feetRadioButton);
            this.groupBox1.Controls.Add(this.metersRadioButton);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.lengthTextBox);
            this.groupBox1.Controls.Add(this.lengthLeftTextBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.totalEnteredTextBox);
            this.groupBox1.Location = new System.Drawing.Point(158, 16);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(267, 159);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            // 
            // chainsRadioButton
            // 
            this.chainsRadioButton.AutoSize = true;
            this.chainsRadioButton.Location = new System.Drawing.Point(173, 22);
            this.chainsRadioButton.Name = "chainsRadioButton";
            this.chainsRadioButton.Size = new System.Drawing.Size(67, 20);
            this.chainsRadioButton.TabIndex = 2;
            this.chainsRadioButton.TabStop = true;
            this.chainsRadioButton.Text = "Chains";
            this.chainsRadioButton.UseVisualStyleBackColor = true;
            this.chainsRadioButton.CheckedChanged += new System.EventHandler(this.chainsRadioButton_CheckedChanged);
            // 
            // feetRadioButton
            // 
            this.feetRadioButton.AutoSize = true;
            this.feetRadioButton.Location = new System.Drawing.Point(102, 22);
            this.feetRadioButton.Name = "feetRadioButton";
            this.feetRadioButton.Size = new System.Drawing.Size(53, 20);
            this.feetRadioButton.TabIndex = 1;
            this.feetRadioButton.TabStop = true;
            this.feetRadioButton.Text = "Feet";
            this.feetRadioButton.UseVisualStyleBackColor = true;
            this.feetRadioButton.CheckedChanged += new System.EventHandler(this.feetRadioButton_CheckedChanged);
            // 
            // metersRadioButton
            // 
            this.metersRadioButton.AutoSize = true;
            this.metersRadioButton.Location = new System.Drawing.Point(20, 22);
            this.metersRadioButton.Name = "metersRadioButton";
            this.metersRadioButton.Size = new System.Drawing.Size(67, 20);
            this.metersRadioButton.TabIndex = 0;
            this.metersRadioButton.TabStop = true;
            this.metersRadioButton.Text = "Meters";
            this.metersRadioButton.UseVisualStyleBackColor = true;
            this.metersRadioButton.CheckedChanged += new System.EventHandler(this.metersRadioButton_CheckedChanged);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(308, 194);
            this.okButton.Margin = new System.Windows.Forms.Padding(4);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(100, 28);
            this.okButton.TabIndex = 25;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(190, 194);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(4);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(100, 28);
            this.cancelButton.TabIndex = 26;
            this.cancelButton.TabStop = false;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(13, 194);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(153, 56);
            this.label4.TabIndex = 27;
            this.label4.Text = "Specify one distance per line, followed by Enter";
            // 
            // LegForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(452, 246);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.distancesTextBox);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "LegForm";
            this.Text = "Specify distances for the new face ...";
            this.Shown += new System.EventHandler(this.LegForm_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox distancesTextBox;
        private System.Windows.Forms.TextBox lengthLeftTextBox;
        private System.Windows.Forms.TextBox totalEnteredTextBox;
        private System.Windows.Forms.TextBox lengthTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton chainsRadioButton;
        private System.Windows.Forms.RadioButton feetRadioButton;
        private System.Windows.Forms.RadioButton metersRadioButton;
    }
}