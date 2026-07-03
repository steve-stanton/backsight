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
            distancesTextBox = new System.Windows.Forms.TextBox();
            lengthLeftTextBox = new System.Windows.Forms.TextBox();
            totalEnteredTextBox = new System.Windows.Forms.TextBox();
            lengthTextBox = new System.Windows.Forms.TextBox();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            groupBox1 = new System.Windows.Forms.GroupBox();
            chainsRadioButton = new System.Windows.Forms.RadioButton();
            feetRadioButton = new System.Windows.Forms.RadioButton();
            metersRadioButton = new System.Windows.Forms.RadioButton();
            okButton = new System.Windows.Forms.Button();
            cancelButton = new System.Windows.Forms.Button();
            label4 = new System.Windows.Forms.Label();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // distancesTextBox
            // 
            distancesTextBox.AcceptsReturn = true;
            distancesTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            distancesTextBox.Location = new System.Drawing.Point(16, 16);
            distancesTextBox.Margin = new System.Windows.Forms.Padding(4);
            distancesTextBox.Multiline = true;
            distancesTextBox.Name = "distancesTextBox";
            distancesTextBox.Size = new System.Drawing.Size(188, 159);
            distancesTextBox.TabIndex = 17;
            distancesTextBox.WordWrap = false;
            distancesTextBox.TextChanged += distancesTextBox_TextChanged;
            // 
            // lengthLeftTextBox
            // 
            lengthLeftTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lengthLeftTextBox.Location = new System.Drawing.Point(175, 116);
            lengthLeftTextBox.Margin = new System.Windows.Forms.Padding(4);
            lengthLeftTextBox.Name = "lengthLeftTextBox";
            lengthLeftTextBox.ReadOnly = true;
            lengthLeftTextBox.Size = new System.Drawing.Size(99, 26);
            lengthLeftTextBox.TabIndex = 23;
            lengthLeftTextBox.TabStop = false;
            // 
            // totalEnteredTextBox
            // 
            totalEnteredTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            totalEnteredTextBox.Location = new System.Drawing.Point(175, 86);
            totalEnteredTextBox.Margin = new System.Windows.Forms.Padding(4);
            totalEnteredTextBox.Name = "totalEnteredTextBox";
            totalEnteredTextBox.ReadOnly = true;
            totalEnteredTextBox.Size = new System.Drawing.Size(99, 26);
            totalEnteredTextBox.TabIndex = 22;
            totalEnteredTextBox.TabStop = false;
            // 
            // lengthTextBox
            // 
            lengthTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lengthTextBox.Location = new System.Drawing.Point(175, 56);
            lengthTextBox.Margin = new System.Windows.Forms.Padding(4);
            lengthTextBox.Name = "lengthTextBox";
            lengthTextBox.ReadOnly = true;
            lengthTextBox.Size = new System.Drawing.Size(99, 26);
            lengthTextBox.TabIndex = 21;
            lengthTextBox.TabStop = false;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)0));
            label3.Location = new System.Drawing.Point(17, 118);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(150, 20);
            label3.TabIndex = 20;
            label3.Text = "Length left to enter";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)0));
            label2.Location = new System.Drawing.Point(60, 88);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(107, 20);
            label2.TabIndex = 19;
            label2.Text = "Total entered";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)0));
            label1.Location = new System.Drawing.Point(60, 58);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(106, 20);
            label1.TabIndex = 18;
            label1.Text = "Length of leg";
            // 
            // groupBox1
            // 
            groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Right));
            groupBox1.Controls.Add(chainsRadioButton);
            groupBox1.Controls.Add(feetRadioButton);
            groupBox1.Controls.Add(metersRadioButton);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(lengthTextBox);
            groupBox1.Controls.Add(lengthLeftTextBox);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(totalEnteredTextBox);
            groupBox1.Location = new System.Drawing.Point(240, 16);
            groupBox1.Margin = new System.Windows.Forms.Padding(4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(4);
            groupBox1.Size = new System.Drawing.Size(308, 159);
            groupBox1.TabIndex = 24;
            groupBox1.TabStop = false;
            // 
            // chainsRadioButton
            // 
            chainsRadioButton.AutoSize = true;
            chainsRadioButton.Location = new System.Drawing.Point(213, 22);
            chainsRadioButton.Name = "chainsRadioButton";
            chainsRadioButton.Size = new System.Drawing.Size(82, 24);
            chainsRadioButton.TabIndex = 2;
            chainsRadioButton.TabStop = true;
            chainsRadioButton.Text = "Chains";
            chainsRadioButton.UseVisualStyleBackColor = true;
            chainsRadioButton.CheckedChanged += chainsRadioButton_CheckedChanged;
            // 
            // feetRadioButton
            // 
            feetRadioButton.AutoSize = true;
            feetRadioButton.Location = new System.Drawing.Point(130, 22);
            feetRadioButton.Name = "feetRadioButton";
            feetRadioButton.Size = new System.Drawing.Size(63, 24);
            feetRadioButton.TabIndex = 1;
            feetRadioButton.TabStop = true;
            feetRadioButton.Text = "Feet";
            feetRadioButton.UseVisualStyleBackColor = true;
            feetRadioButton.CheckedChanged += feetRadioButton_CheckedChanged;
            // 
            // metersRadioButton
            // 
            metersRadioButton.AutoSize = true;
            metersRadioButton.Location = new System.Drawing.Point(32, 22);
            metersRadioButton.Name = "metersRadioButton";
            metersRadioButton.Size = new System.Drawing.Size(82, 24);
            metersRadioButton.TabIndex = 0;
            metersRadioButton.TabStop = true;
            metersRadioButton.Text = "Meters";
            metersRadioButton.UseVisualStyleBackColor = true;
            metersRadioButton.CheckedChanged += metersRadioButton_CheckedChanged;
            // 
            // okButton
            // 
            okButton.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
            okButton.Location = new System.Drawing.Point(390, 194);
            okButton.Margin = new System.Windows.Forms.Padding(4);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(100, 28);
            okButton.TabIndex = 25;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += okButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
            cancelButton.Location = new System.Drawing.Point(272, 194);
            cancelButton.Margin = new System.Windows.Forms.Padding(4);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(100, 28);
            cancelButton.TabIndex = 26;
            cancelButton.TabStop = false;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // label4
            // 
            label4.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)0));
            label4.Location = new System.Drawing.Point(13, 194);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(153, 56);
            label4.TabIndex = 27;
            label4.Text = "Specify one distance per line, followed by Enter";
            // 
            // LegForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(571, 246);
            Controls.Add(label4);
            Controls.Add(okButton);
            Controls.Add(cancelButton);
            Controls.Add(groupBox1);
            Controls.Add(distancesTextBox);
            Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)0));
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            Margin = new System.Windows.Forms.Padding(4);
            Text = "Specify distances for the new face ...";
            TopMost = true;
            Shown += LegForm_Shown;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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