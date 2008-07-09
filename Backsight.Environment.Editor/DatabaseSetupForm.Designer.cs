namespace CommonInstaller
{
    partial class DatabaseSetupForm
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
            this.cmdContinue = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
            this.dlgBrowse = new System.Windows.Forms.FolderBrowserDialog();
            this.grpSQLAuthentication = new System.Windows.Forms.GroupBox();
            this.rdoUseSQLServerAuthentication = new System.Windows.Forms.RadioButton();
            this.rdoUseWindowsAuthentication = new System.Windows.Forms.RadioButton();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblUsername = new System.Windows.Forms.Label();
            this.grpSDESettings = new System.Windows.Forms.GroupBox();
            this.txtInstanceName = new System.Windows.Forms.TextBox();
            this.lblInstanceName = new System.Windows.Forms.Label();
            this.txtInstance = new System.Windows.Forms.TextBox();
            this.lblInstance = new System.Windows.Forms.Label();
            this.cmdBrowseLicense = new System.Windows.Forms.Button();
            this.txtLicenseFile = new System.Windows.Forms.TextBox();
            this.lblLicenseFile = new System.Windows.Forms.Label();
            this.grpDatabaseSettings = new System.Windows.Forms.GroupBox();
            this.lblDatabaseName = new System.Windows.Forms.Label();
            this.txtDatabaseName = new System.Windows.Forms.TextBox();
            this.txtServerName = new System.Windows.Forms.TextBox();
            this.lblServerName = new System.Windows.Forms.Label();
            this.grpLocations = new System.Windows.Forms.GroupBox();
            this.cmdBrowseBackup = new System.Windows.Forms.Button();
            this.txtLocationBackup = new System.Windows.Forms.TextBox();
            this.lblLocationBackup = new System.Windows.Forms.Label();
            this.cmdBrowseLocation = new System.Windows.Forms.Button();
            this.txtLocation = new System.Windows.Forms.TextBox();
            this.lblLocation = new System.Windows.Forms.Label();
            this.grpSQLAuthentication.SuspendLayout();
            this.grpSDESettings.SuspendLayout();
            this.grpDatabaseSettings.SuspendLayout();
            this.grpLocations.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdContinue
            // 
            this.cmdContinue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdContinue.Location = new System.Drawing.Point(255, 444);
            this.cmdContinue.Name = "cmdContinue";
            this.cmdContinue.Size = new System.Drawing.Size(75, 23);
            this.cmdContinue.TabIndex = 4;
            this.cmdContinue.Text = "C&ontinue";
            this.cmdContinue.UseVisualStyleBackColor = true;
            this.cmdContinue.Click += new System.EventHandler(this.cmdContinue_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(336, 444);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 5;
            this.cmdCancel.Text = "&Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // dlgOpen
            // 
            this.dlgOpen.DefaultExt = "ecp";
            this.dlgOpen.Filter = "ESRI License file (*.ecp)|*.ecp";
            // 
            // grpSQLAuthentication
            // 
            this.grpSQLAuthentication.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSQLAuthentication.Controls.Add(this.rdoUseSQLServerAuthentication);
            this.grpSQLAuthentication.Controls.Add(this.rdoUseWindowsAuthentication);
            this.grpSQLAuthentication.Controls.Add(this.txtPassword);
            this.grpSQLAuthentication.Controls.Add(this.txtUsername);
            this.grpSQLAuthentication.Controls.Add(this.lblPassword);
            this.grpSQLAuthentication.Controls.Add(this.lblUsername);
            this.grpSQLAuthentication.Location = new System.Drawing.Point(15, 90);
            this.grpSQLAuthentication.Name = "grpSQLAuthentication";
            this.grpSQLAuthentication.Size = new System.Drawing.Size(396, 134);
            this.grpSQLAuthentication.TabIndex = 1;
            this.grpSQLAuthentication.TabStop = false;
            this.grpSQLAuthentication.Text = "SQL Server Authentication";
            // 
            // rdoUseSQLServerAuthentication
            // 
            this.rdoUseSQLServerAuthentication.AutoSize = true;
            this.rdoUseSQLServerAuthentication.Location = new System.Drawing.Point(6, 42);
            this.rdoUseSQLServerAuthentication.Name = "rdoUseSQLServerAuthentication";
            this.rdoUseSQLServerAuthentication.Size = new System.Drawing.Size(173, 17);
            this.rdoUseSQLServerAuthentication.TabIndex = 1;
            this.rdoUseSQLServerAuthentication.TabStop = true;
            this.rdoUseSQLServerAuthentication.Text = "Use S&QL Server Authentication";
            this.rdoUseSQLServerAuthentication.UseVisualStyleBackColor = true;
            this.rdoUseSQLServerAuthentication.CheckedChanged += new System.EventHandler(this.rdoAuthentication_CheckedChanged);
            // 
            // rdoUseWindowsAuthentication
            // 
            this.rdoUseWindowsAuthentication.AutoSize = true;
            this.rdoUseWindowsAuthentication.Location = new System.Drawing.Point(6, 19);
            this.rdoUseWindowsAuthentication.Name = "rdoUseWindowsAuthentication";
            this.rdoUseWindowsAuthentication.Size = new System.Drawing.Size(162, 17);
            this.rdoUseWindowsAuthentication.TabIndex = 0;
            this.rdoUseWindowsAuthentication.TabStop = true;
            this.rdoUseWindowsAuthentication.Text = "Use &Windows Authentication";
            this.rdoUseWindowsAuthentication.UseVisualStyleBackColor = true;
            this.rdoUseWindowsAuthentication.CheckedChanged += new System.EventHandler(this.rdoAuthentication_CheckedChanged);
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Location = new System.Drawing.Point(155, 96);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(202, 20);
            this.txtPassword.TabIndex = 5;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // txtUsername
            // 
            this.txtUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUsername.Location = new System.Drawing.Point(155, 70);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(202, 20);
            this.txtUsername.TabIndex = 3;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(6, 99);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(53, 13);
            this.lblPassword.TabIndex = 4;
            this.lblPassword.Text = "&Password";
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(6, 73);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(60, 13);
            this.lblUsername.TabIndex = 2;
            this.lblUsername.Text = "&User Name";
            // 
            // grpSDESettings
            // 
            this.grpSDESettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSDESettings.Controls.Add(this.txtInstanceName);
            this.grpSDESettings.Controls.Add(this.lblInstanceName);
            this.grpSDESettings.Controls.Add(this.txtInstance);
            this.grpSDESettings.Controls.Add(this.lblInstance);
            this.grpSDESettings.Controls.Add(this.cmdBrowseLicense);
            this.grpSDESettings.Controls.Add(this.txtLicenseFile);
            this.grpSDESettings.Controls.Add(this.lblLicenseFile);
            this.grpSDESettings.Location = new System.Drawing.Point(15, 318);
            this.grpSDESettings.Name = "grpSDESettings";
            this.grpSDESettings.Size = new System.Drawing.Size(396, 114);
            this.grpSDESettings.TabIndex = 3;
            this.grpSDESettings.TabStop = false;
            this.grpSDESettings.Text = "SDE Settings";
            // 
            // txtInstanceName
            // 
            this.txtInstanceName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInstanceName.Location = new System.Drawing.Point(155, 19);
            this.txtInstanceName.Name = "txtInstanceName";
            this.txtInstanceName.Size = new System.Drawing.Size(202, 20);
            this.txtInstanceName.TabIndex = 1;
            // 
            // lblInstanceName
            // 
            this.lblInstanceName.AutoSize = true;
            this.lblInstanceName.Location = new System.Drawing.Point(6, 22);
            this.lblInstanceName.Name = "lblInstanceName";
            this.lblInstanceName.Size = new System.Drawing.Size(104, 13);
            this.lblInstanceName.TabIndex = 0;
            this.lblInstanceName.Text = "SDE Instance &Name";
            // 
            // txtInstance
            // 
            this.txtInstance.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInstance.Location = new System.Drawing.Point(155, 45);
            this.txtInstance.Name = "txtInstance";
            this.txtInstance.Size = new System.Drawing.Size(202, 20);
            this.txtInstance.TabIndex = 3;
            // 
            // lblInstance
            // 
            this.lblInstance.AutoSize = true;
            this.lblInstance.Location = new System.Drawing.Point(6, 48);
            this.lblInstance.Name = "lblInstance";
            this.lblInstance.Size = new System.Drawing.Size(73, 13);
            this.lblInstance.TabIndex = 2;
            this.lblInstance.Text = "SDE &Instance";
            // 
            // cmdBrowseLicense
            // 
            this.cmdBrowseLicense.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdBrowseLicense.Location = new System.Drawing.Point(365, 69);
            this.cmdBrowseLicense.Name = "cmdBrowseLicense";
            this.cmdBrowseLicense.Size = new System.Drawing.Size(24, 23);
            this.cmdBrowseLicense.TabIndex = 6;
            this.cmdBrowseLicense.Text = "...";
            this.cmdBrowseLicense.UseVisualStyleBackColor = true;
            this.cmdBrowseLicense.Click += new System.EventHandler(this.cmdBrowseLicense_Click);
            // 
            // txtLicenseFile
            // 
            this.txtLicenseFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLicenseFile.Location = new System.Drawing.Point(155, 71);
            this.txtLicenseFile.Name = "txtLicenseFile";
            this.txtLicenseFile.Size = new System.Drawing.Size(202, 20);
            this.txtLicenseFile.TabIndex = 5;
            // 
            // lblLicenseFile
            // 
            this.lblLicenseFile.AutoSize = true;
            this.lblLicenseFile.Location = new System.Drawing.Point(6, 74);
            this.lblLicenseFile.Name = "lblLicenseFile";
            this.lblLicenseFile.Size = new System.Drawing.Size(88, 13);
            this.lblLicenseFile.TabIndex = 4;
            this.lblLicenseFile.Text = "SDE &License File";
            // 
            // grpDatabaseSettings
            // 
            this.grpDatabaseSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDatabaseSettings.Controls.Add(this.lblDatabaseName);
            this.grpDatabaseSettings.Controls.Add(this.txtDatabaseName);
            this.grpDatabaseSettings.Controls.Add(this.txtServerName);
            this.grpDatabaseSettings.Controls.Add(this.lblServerName);
            this.grpDatabaseSettings.Location = new System.Drawing.Point(15, 12);
            this.grpDatabaseSettings.Name = "grpDatabaseSettings";
            this.grpDatabaseSettings.Size = new System.Drawing.Size(396, 72);
            this.grpDatabaseSettings.TabIndex = 0;
            this.grpDatabaseSettings.TabStop = false;
            this.grpDatabaseSettings.Text = "Database Settings";
            // 
            // lblDatabaseName
            // 
            this.lblDatabaseName.AutoSize = true;
            this.lblDatabaseName.Location = new System.Drawing.Point(6, 42);
            this.lblDatabaseName.Name = "lblDatabaseName";
            this.lblDatabaseName.Size = new System.Drawing.Size(84, 13);
            this.lblDatabaseName.TabIndex = 2;
            this.lblDatabaseName.Text = "&Database Name";
            // 
            // txtDatabaseName
            // 
            this.txtDatabaseName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDatabaseName.Location = new System.Drawing.Point(155, 39);
            this.txtDatabaseName.Name = "txtDatabaseName";
            this.txtDatabaseName.Size = new System.Drawing.Size(202, 20);
            this.txtDatabaseName.TabIndex = 3;
            // 
            // txtServerName
            // 
            this.txtServerName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtServerName.Location = new System.Drawing.Point(155, 13);
            this.txtServerName.Name = "txtServerName";
            this.txtServerName.Size = new System.Drawing.Size(202, 20);
            this.txtServerName.TabIndex = 1;
            // 
            // lblServerName
            // 
            this.lblServerName.AutoSize = true;
            this.lblServerName.Location = new System.Drawing.Point(6, 16);
            this.lblServerName.Name = "lblServerName";
            this.lblServerName.Size = new System.Drawing.Size(69, 13);
            this.lblServerName.TabIndex = 0;
            this.lblServerName.Text = "&Server Name";
            // 
            // grpLocations
            // 
            this.grpLocations.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLocations.Controls.Add(this.cmdBrowseBackup);
            this.grpLocations.Controls.Add(this.txtLocationBackup);
            this.grpLocations.Controls.Add(this.lblLocationBackup);
            this.grpLocations.Controls.Add(this.cmdBrowseLocation);
            this.grpLocations.Controls.Add(this.txtLocation);
            this.grpLocations.Controls.Add(this.lblLocation);
            this.grpLocations.Location = new System.Drawing.Point(15, 230);
            this.grpLocations.Name = "grpLocations";
            this.grpLocations.Size = new System.Drawing.Size(396, 82);
            this.grpLocations.TabIndex = 2;
            this.grpLocations.TabStop = false;
            this.grpLocations.Text = "File Locations";
            // 
            // cmdBrowseBackup
            // 
            this.cmdBrowseBackup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdBrowseBackup.Location = new System.Drawing.Point(365, 43);
            this.cmdBrowseBackup.Name = "cmdBrowseBackup";
            this.cmdBrowseBackup.Size = new System.Drawing.Size(24, 23);
            this.cmdBrowseBackup.TabIndex = 5;
            this.cmdBrowseBackup.Text = "...";
            this.cmdBrowseBackup.UseVisualStyleBackColor = true;
            this.cmdBrowseBackup.Click += new System.EventHandler(this.cmdBrowseBackup_Click);
            // 
            // txtLocationBackup
            // 
            this.txtLocationBackup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLocationBackup.Location = new System.Drawing.Point(155, 45);
            this.txtLocationBackup.Name = "txtLocationBackup";
            this.txtLocationBackup.Size = new System.Drawing.Size(202, 20);
            this.txtLocationBackup.TabIndex = 4;
            // 
            // lblLocationBackup
            // 
            this.lblLocationBackup.AutoSize = true;
            this.lblLocationBackup.Location = new System.Drawing.Point(6, 48);
            this.lblLocationBackup.Name = "lblLocationBackup";
            this.lblLocationBackup.Size = new System.Drawing.Size(124, 13);
            this.lblLocationBackup.TabIndex = 3;
            this.lblLocationBackup.Text = "Location for &Backup files";
            // 
            // cmdBrowseLocation
            // 
            this.cmdBrowseLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdBrowseLocation.Location = new System.Drawing.Point(365, 17);
            this.cmdBrowseLocation.Name = "cmdBrowseLocation";
            this.cmdBrowseLocation.Size = new System.Drawing.Size(24, 23);
            this.cmdBrowseLocation.TabIndex = 2;
            this.cmdBrowseLocation.Text = "...";
            this.cmdBrowseLocation.UseVisualStyleBackColor = true;
            this.cmdBrowseLocation.Click += new System.EventHandler(this.cmdBrowseLocation_Click);
            // 
            // txtLocation
            // 
            this.txtLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLocation.Location = new System.Drawing.Point(155, 19);
            this.txtLocation.Name = "txtLocation";
            this.txtLocation.Size = new System.Drawing.Size(202, 20);
            this.txtLocation.TabIndex = 1;
            // 
            // lblLocation
            // 
            this.lblLocation.AutoSize = true;
            this.lblLocation.Location = new System.Drawing.Point(6, 22);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(133, 13);
            this.lblLocation.TabIndex = 0;
            this.lblLocation.Text = "Location for D&atabase files";
            // 
            // DatabaseSetupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 479);
            this.Controls.Add(this.grpLocations);
            this.Controls.Add(this.grpDatabaseSettings);
            this.Controls.Add(this.grpSDESettings);
            this.Controls.Add(this.grpSQLAuthentication);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdContinue);
            this.Name = "DatabaseSetupForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Database Setup Form";
            this.Load += new System.EventHandler(this.DatabaseSetupForm_Load);
            this.grpSQLAuthentication.ResumeLayout(false);
            this.grpSQLAuthentication.PerformLayout();
            this.grpSDESettings.ResumeLayout(false);
            this.grpSDESettings.PerformLayout();
            this.grpDatabaseSettings.ResumeLayout(false);
            this.grpDatabaseSettings.PerformLayout();
            this.grpLocations.ResumeLayout(false);
            this.grpLocations.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdContinue;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.OpenFileDialog dlgOpen;
        private System.Windows.Forms.FolderBrowserDialog dlgBrowse;
        private System.Windows.Forms.GroupBox grpSQLAuthentication;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.GroupBox grpSDESettings;
        private System.Windows.Forms.TextBox txtInstance;
        private System.Windows.Forms.Label lblInstance;
        private System.Windows.Forms.Button cmdBrowseLicense;
        private System.Windows.Forms.TextBox txtLicenseFile;
        private System.Windows.Forms.Label lblLicenseFile;
        private System.Windows.Forms.GroupBox grpDatabaseSettings;
        private System.Windows.Forms.Label lblDatabaseName;
        private System.Windows.Forms.TextBox txtDatabaseName;
        private System.Windows.Forms.TextBox txtServerName;
        private System.Windows.Forms.Label lblServerName;
        private System.Windows.Forms.TextBox txtInstanceName;
        private System.Windows.Forms.Label lblInstanceName;
        private System.Windows.Forms.RadioButton rdoUseSQLServerAuthentication;
        private System.Windows.Forms.RadioButton rdoUseWindowsAuthentication;
        private System.Windows.Forms.GroupBox grpLocations;
        private System.Windows.Forms.Button cmdBrowseBackup;
        private System.Windows.Forms.TextBox txtLocationBackup;
        private System.Windows.Forms.Label lblLocationBackup;
        private System.Windows.Forms.Button cmdBrowseLocation;
        private System.Windows.Forms.TextBox txtLocation;
        private System.Windows.Forms.Label lblLocation;
    }
}