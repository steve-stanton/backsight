namespace Backsight.Editor
{
    partial class TestForm
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
            this.testButton1 = new Backsight.Forms.TestButton();
            this.testButton2 = new Backsight.Forms.TestButton();
            this.SuspendLayout();
            // 
            // testButton1
            // 
            this.testButton1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.testButton1.Location = new System.Drawing.Point(70, 79);
            this.testButton1.Name = "testButton1";
            this.testButton1.Size = new System.Drawing.Size(124, 53);
            this.testButton1.TabIndex = 0;
            this.testButton1.Text = "testButton1";
            this.testButton1.UseVisualStyleBackColor = true;
            this.testButton1.Click += new System.EventHandler(this.testButton1_Click);
            // 
            // testButton2
            // 
            this.testButton2.BackgroundImage = global::Backsight.Editor.Properties.Resources.ConnectionPath;
            this.testButton2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.testButton2.Location = new System.Drawing.Point(104, 171);
            this.testButton2.Name = "testButton2";
            this.testButton2.Size = new System.Drawing.Size(60, 59);
            this.testButton2.TabIndex = 1;
            this.testButton2.UseVisualStyleBackColor = true;
            this.testButton2.Click += new System.EventHandler(this.testButton2_Click);
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(638, 429);
            this.Controls.Add(this.testButton2);
            this.Controls.Add(this.testButton1);
            this.Name = "TestForm";
            this.Text = "TestForm";
            this.Shown += new System.EventHandler(this.TestForm_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TestForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private Backsight.Forms.TestButton testButton1;
        private Backsight.Forms.TestButton testButton2;
    }
}