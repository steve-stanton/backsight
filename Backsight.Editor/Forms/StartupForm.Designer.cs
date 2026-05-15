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
            components = new System.ComponentModel.Container();
            groupBox1 = new System.Windows.Forms.GroupBox();
            openLastButton = new System.Windows.Forms.Button();
            openFileButton = new System.Windows.Forms.Button();
            newProjectButton = new System.Windows.Forms.Button();
            exitButton = new System.Windows.Forms.Button();
            toolTip = new System.Windows.Forms.ToolTip(components);
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            lastDatabaseLabel = new System.Windows.Forms.ToolStripStatusLabel();
            groupBox1.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(openLastButton);
            groupBox1.Controls.Add(openFileButton);
            groupBox1.Controls.Add(newProjectButton);
            groupBox1.Controls.Add(exitButton);
            groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)0));
            groupBox1.Location = new System.Drawing.Point(14, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(353, 210);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "What do you want to do?";
            // 
            // openLastButton
            // 
            openLastButton.BackColor = System.Drawing.Color.FromArgb(((int)((byte)255)), ((int)((byte)192)), ((int)((byte)128)));
            openLastButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            openLastButton.Location = new System.Drawing.Point(36, 37);
            openLastButton.Name = "openLastButton";
            openLastButton.Size = new System.Drawing.Size(288, 32);
            openLastButton.TabIndex = 0;
            openLastButton.Text = "&Open last project";
            toolTip.SetToolTip(openLastButton, "Re-opens the project with the name shown");
            openLastButton.UseVisualStyleBackColor = false;
            openLastButton.Click += openLastButton_Click;
            // 
            // openFileButton
            // 
            openFileButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            openFileButton.Location = new System.Drawing.Point(36, 79);
            openFileButton.Name = "openFileButton";
            openFileButton.Size = new System.Drawing.Size(288, 32);
            openFileButton.TabIndex = 7;
            openFileButton.TabStop = false;
            openFileButton.Text = "Open anothe&r project";
            toolTip.SetToolTip(openFileButton, "You will be asked to pick an existing project");
            openFileButton.UseVisualStyleBackColor = true;
            openFileButton.Click += openFileButton_Click;
            // 
            // newProjectButton
            // 
            newProjectButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            newProjectButton.Location = new System.Drawing.Point(36, 121);
            newProjectButton.Name = "newProjectButton";
            newProjectButton.Size = new System.Drawing.Size(288, 32);
            newProjectButton.TabIndex = 6;
            newProjectButton.TabStop = false;
            newProjectButton.Text = "Create a &new project";
            toolTip.SetToolTip(newProjectButton, "Create a brand new project");
            newProjectButton.UseVisualStyleBackColor = true;
            newProjectButton.Click += newProjectButton_Click;
            // 
            // exitButton
            // 
            exitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            exitButton.Location = new System.Drawing.Point(36, 163);
            exitButton.Name = "exitButton";
            exitButton.Size = new System.Drawing.Size(288, 32);
            exitButton.TabIndex = 5;
            exitButton.TabStop = false;
            exitButton.Text = "E&xit";
            toolTip.SetToolTip(exitButton, "Close the Cadastral Editor application");
            exitButton.UseVisualStyleBackColor = true;
            exitButton.Click += exitButton_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { lastDatabaseLabel });
            statusStrip1.Location = new System.Drawing.Point(0, 235);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            statusStrip1.Size = new System.Drawing.Size(387, 26);
            statusStrip1.SizingGrip = false;
            statusStrip1.TabIndex = 5;
            statusStrip1.Text = "statusStrip1";
            // 
            // lastDatabaseLabel
            // 
            lastDatabaseLabel.Name = "lastDatabaseLabel";
            lastDatabaseLabel.Size = new System.Drawing.Size(135, 20);
            lastDatabaseLabel.Text = "Database unknown";
            // 
            // StartupForm
            // 
            AcceptButton = openLastButton;
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(387, 261);
            Controls.Add(statusStrip1);
            Controls.Add(groupBox1);
            Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)0));
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Margin = new System.Windows.Forms.Padding(4);
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Cadastral Editor";
            Load += StartupForm_Load;
            groupBox1.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.Button newProjectButton;
        private System.Windows.Forms.Button openFileButton;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lastDatabaseLabel;
        private System.Windows.Forms.Button openLastButton;
    }
}