namespace Backsight.Editor.Forms
{
    partial class StartupForm
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
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.switchButton = new System.Windows.Forms.Button();
            this.openFileButton = new System.Windows.Forms.Button();
            this.newJobButton = new System.Windows.Forms.Button();
            this.exitButton = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lastDatabaseLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.switchButton);
            this.groupBox1.Controls.Add(this.openFileButton);
            this.groupBox1.Controls.Add(this.newJobButton);
            this.groupBox1.Controls.Add(this.exitButton);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(314, 211);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "What do you want to do?";
            // 
            // switchButton
            // 
            this.switchButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.switchButton.Location = new System.Drawing.Point(32, 120);
            this.switchButton.Name = "switchButton";
            this.switchButton.Size = new System.Drawing.Size(256, 25);
            this.switchButton.TabIndex = 8;
            this.switchButton.TabStop = false;
            this.switchButton.Text = "Switch to another &database";
            this.toolTip.SetToolTip(this.switchButton, "Specify connection settings for another database");
            this.switchButton.UseVisualStyleBackColor = true;
            this.switchButton.Click += new System.EventHandler(this.switchButton_Click);
            // 
            // openFileButton
            // 
            this.openFileButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.openFileButton.Location = new System.Drawing.Point(32, 36);
            this.openFileButton.Name = "openFileButton";
            this.openFileButton.Size = new System.Drawing.Size(256, 25);
            this.openFileButton.TabIndex = 7;
            this.openFileButton.TabStop = false;
            this.openFileButton.Text = "&Open an existing job file";
            this.toolTip.SetToolTip(this.openFileButton, "You will be asked to pick an existing cedx file");
            this.openFileButton.UseVisualStyleBackColor = true;
            this.openFileButton.Click += new System.EventHandler(this.openFileButton_Click);
            // 
            // newJobButton
            // 
            this.newJobButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.newJobButton.Location = new System.Drawing.Point(32, 78);
            this.newJobButton.Name = "newJobButton";
            this.newJobButton.Size = new System.Drawing.Size(256, 25);
            this.newJobButton.TabIndex = 6;
            this.newJobButton.TabStop = false;
            this.newJobButton.Text = "List &all registered jobs";
            this.toolTip.SetToolTip(this.newJobButton, "Look at all jobs that have been registered in the database (with option to create" +
        " new jobs)");
            this.newJobButton.UseVisualStyleBackColor = true;
            this.newJobButton.Click += new System.EventHandler(this.newJobButton_Click);
            // 
            // exitButton
            // 
            this.exitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exitButton.Location = new System.Drawing.Point(32, 162);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(256, 25);
            this.exitButton.TabIndex = 5;
            this.exitButton.TabStop = false;
            this.exitButton.Text = "E&xit";
            this.toolTip.SetToolTip(this.exitButton, "Close the Cadastral Editor application");
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lastDatabaseLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 264);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(344, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lastDatabaseLabel
            // 
            this.lastDatabaseLabel.Name = "lastDatabaseLabel";
            this.lastDatabaseLabel.Size = new System.Drawing.Size(116, 17);
            this.lastDatabaseLabel.Text = "Database unknown";
            // 
            // StartupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 286);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "StartupForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cadastral Editor";
            this.groupBox1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.Button newJobButton;
        private System.Windows.Forms.Button openFileButton;
        private System.Windows.Forms.Button switchButton;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lastDatabaseLabel;
    }
}