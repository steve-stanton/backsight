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
            this.openLastButton = new System.Windows.Forms.Button();
            this.openFileButton = new System.Windows.Forms.Button();
            this.newProjectButton = new System.Windows.Forms.Button();
            this.exitButton = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lastDatabaseLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.databaseButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.openLastButton);
            this.groupBox1.Controls.Add(this.openFileButton);
            this.groupBox1.Controls.Add(this.newProjectButton);
            this.groupBox1.Controls.Add(this.exitButton);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(314, 210);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "What do you want to do?";
            // 
            // openLastButton
            // 
            this.openLastButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.openLastButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.openLastButton.Location = new System.Drawing.Point(32, 37);
            this.openLastButton.Name = "openLastButton";
            this.openLastButton.Size = new System.Drawing.Size(256, 25);
            this.openLastButton.TabIndex = 0;
            this.openLastButton.Text = "&Open last project";
            this.toolTip.SetToolTip(this.openLastButton, "Re-opens the project with the name shown");
            this.openLastButton.UseVisualStyleBackColor = false;
            this.openLastButton.Click += new System.EventHandler(this.openLastButton_Click);
            // 
            // openFileButton
            // 
            this.openFileButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.openFileButton.Location = new System.Drawing.Point(32, 79);
            this.openFileButton.Name = "openFileButton";
            this.openFileButton.Size = new System.Drawing.Size(256, 25);
            this.openFileButton.TabIndex = 7;
            this.openFileButton.TabStop = false;
            this.openFileButton.Text = "Open anothe&r project";
            this.toolTip.SetToolTip(this.openFileButton, "You will be asked to pick an existing project");
            this.openFileButton.UseVisualStyleBackColor = true;
            this.openFileButton.Click += new System.EventHandler(this.openFileButton_Click);
            // 
            // newProjectButton
            // 
            this.newProjectButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.newProjectButton.Location = new System.Drawing.Point(32, 121);
            this.newProjectButton.Name = "newProjectButton";
            this.newProjectButton.Size = new System.Drawing.Size(256, 25);
            this.newProjectButton.TabIndex = 6;
            this.newProjectButton.TabStop = false;
            this.newProjectButton.Text = "Create a &new project";
            this.toolTip.SetToolTip(this.newProjectButton, "Create a brand new project");
            this.newProjectButton.UseVisualStyleBackColor = true;
            this.newProjectButton.Click += new System.EventHandler(this.newProjectButton_Click);
            // 
            // exitButton
            // 
            this.exitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exitButton.Location = new System.Drawing.Point(32, 163);
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
            this.statusStrip1.Location = new System.Drawing.Point(0, 239);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(344, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lastDatabaseLabel
            // 
            this.lastDatabaseLabel.Name = "lastDatabaseLabel";
            this.lastDatabaseLabel.Size = new System.Drawing.Size(99, 17);
            this.lastDatabaseLabel.Text = "Database unknown";
            // 
            // databaseButton
            // 
            this.databaseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.databaseButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.databaseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.databaseButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.databaseButton.Location = new System.Drawing.Point(281, 239);
            this.databaseButton.Name = "databaseButton";
            this.databaseButton.Size = new System.Drawing.Size(63, 22);
            this.databaseButton.TabIndex = 6;
            this.databaseButton.TabStop = false;
            this.databaseButton.Text = "Change...";
            this.databaseButton.UseVisualStyleBackColor = true;
            this.databaseButton.Click += new System.EventHandler(this.databaseButton_Click);
            // 
            // StartupForm
            // 
            this.AcceptButton = this.openLastButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 261);
            this.Controls.Add(this.databaseButton);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "StartupForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cadastral Editor";
            this.Load += new System.EventHandler(this.StartupForm_Load);
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
        private System.Windows.Forms.Button newProjectButton;
        private System.Windows.Forms.Button openFileButton;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lastDatabaseLabel;
        private System.Windows.Forms.Button databaseButton;
        private System.Windows.Forms.Button openLastButton;
    }
}