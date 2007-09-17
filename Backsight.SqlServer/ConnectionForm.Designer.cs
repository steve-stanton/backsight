/// <remarks>
/// Copyright 2007 - Steve Stanton. This file is part of Backsight
///
/// Backsight is free software; you can redistribute it and/or modify it under the terms
/// of the GNU Lesser General Public License as published by the Free Software Foundation;
/// either version 3 of the License, or (at your option) any later version.
///
/// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
/// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU Lesser General Public License for more details.
///
/// You should have received a copy of the GNU Lesser General Public License
/// along with this program. If not, see <http://www.gnu.org/licenses/>.
/// </remarks>

namespace Backsight.SqlServer
{
    partial class ConnectionForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.serverTextBox = new System.Windows.Forms.TextBox();
            this.browseServerButton = new System.Windows.Forms.Button();
            this.connectButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.dbBrowseButton = new System.Windows.Forms.Button();
            this.dbTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SecurityPanel = new System.Windows.Forms.Panel();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.userAndPasswordRadioButton = new System.Windows.Forms.RadioButton();
            this.userNameTextBox = new System.Windows.Forms.TextBox();
            this.PasswordLabel = new System.Windows.Forms.Label();
            this.windowsAuthenticationRadioButton = new System.Windows.Forms.RadioButton();
            this.UserNameLabel = new System.Windows.Forms.Label();
            this.SecurityPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server";
            // 
            // serverTextBox
            // 
            this.serverTextBox.Location = new System.Drawing.Point(89, 18);
            this.serverTextBox.Name = "serverTextBox";
            this.serverTextBox.Size = new System.Drawing.Size(319, 20);
            this.serverTextBox.TabIndex = 0;
            // 
            // browseServerButton
            // 
            this.browseServerButton.Location = new System.Drawing.Point(423, 16);
            this.browseServerButton.Name = "browseServerButton";
            this.browseServerButton.Size = new System.Drawing.Size(75, 23);
            this.browseServerButton.TabIndex = 2;
            this.browseServerButton.TabStop = false;
            this.browseServerButton.Text = "Brow&se...";
            this.browseServerButton.UseVisualStyleBackColor = true;
            this.browseServerButton.Click += new System.EventHandler(this.browseServerButton_Click);
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(262, 254);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(75, 23);
            this.connectButton.TabIndex = 3;
            this.connectButton.Text = "&Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(181, 254);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.TabStop = false;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // dbBrowseButton
            // 
            this.dbBrowseButton.Location = new System.Drawing.Point(423, 200);
            this.dbBrowseButton.Name = "dbBrowseButton";
            this.dbBrowseButton.Size = new System.Drawing.Size(75, 23);
            this.dbBrowseButton.TabIndex = 7;
            this.dbBrowseButton.TabStop = false;
            this.dbBrowseButton.Text = "&Browse...";
            this.dbBrowseButton.UseVisualStyleBackColor = true;
            this.dbBrowseButton.Click += new System.EventHandler(this.dbBrowseButton_Click);
            // 
            // dbTextBox
            // 
            this.dbTextBox.Location = new System.Drawing.Point(89, 202);
            this.dbTextBox.Name = "dbTextBox";
            this.dbTextBox.Size = new System.Drawing.Size(319, 20);
            this.dbTextBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 205);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Database";
            // 
            // SecurityPanel
            // 
            this.SecurityPanel.Controls.Add(this.passwordTextBox);
            this.SecurityPanel.Controls.Add(this.userAndPasswordRadioButton);
            this.SecurityPanel.Controls.Add(this.userNameTextBox);
            this.SecurityPanel.Controls.Add(this.PasswordLabel);
            this.SecurityPanel.Controls.Add(this.windowsAuthenticationRadioButton);
            this.SecurityPanel.Controls.Add(this.UserNameLabel);
            this.SecurityPanel.Location = new System.Drawing.Point(89, 59);
            this.SecurityPanel.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.SecurityPanel.Name = "SecurityPanel";
            this.SecurityPanel.Size = new System.Drawing.Size(319, 126);
            this.SecurityPanel.TabIndex = 8;
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Location = new System.Drawing.Point(92, 91);
            this.passwordTextBox.Margin = new System.Windows.Forms.Padding(1, 3, 3, 3);
            this.passwordTextBox.MaxLength = 128;
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.Size = new System.Drawing.Size(180, 20);
            this.passwordTextBox.TabIndex = 12;
            this.passwordTextBox.UseSystemPasswordChar = true;
            // 
            // userAndPasswordRadioButton
            // 
            this.userAndPasswordRadioButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.userAndPasswordRadioButton.Location = new System.Drawing.Point(14, 31);
            this.userAndPasswordRadioButton.Margin = new System.Windows.Forms.Padding(3, 1, 3, 3);
            this.userAndPasswordRadioButton.Name = "userAndPasswordRadioButton";
            this.userAndPasswordRadioButton.Size = new System.Drawing.Size(223, 24);
            this.userAndPasswordRadioButton.TabIndex = 1;
            this.userAndPasswordRadioButton.Text = "Use a specific user &ID and password:";
            // 
            // userNameTextBox
            // 
            this.userNameTextBox.Location = new System.Drawing.Point(92, 61);
            this.userNameTextBox.Margin = new System.Windows.Forms.Padding(1, 3, 3, 3);
            this.userNameTextBox.MaxLength = 128;
            this.userNameTextBox.Name = "userNameTextBox";
            this.userNameTextBox.Size = new System.Drawing.Size(180, 20);
            this.userNameTextBox.TabIndex = 10;
            // 
            // PasswordLabel
            // 
            this.PasswordLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.PasswordLabel.Location = new System.Drawing.Point(32, 94);
            this.PasswordLabel.Margin = new System.Windows.Forms.Padding(3, 3, 1, 3);
            this.PasswordLabel.Name = "PasswordLabel";
            this.PasswordLabel.Size = new System.Drawing.Size(80, 14);
            this.PasswordLabel.TabIndex = 11;
            this.PasswordLabel.Text = "&Password:";
            // 
            // windowsAuthenticationRadioButton
            // 
            this.windowsAuthenticationRadioButton.Checked = true;
            this.windowsAuthenticationRadioButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.windowsAuthenticationRadioButton.Location = new System.Drawing.Point(14, 3);
            this.windowsAuthenticationRadioButton.Margin = new System.Windows.Forms.Padding(3, 3, 3, 2);
            this.windowsAuthenticationRadioButton.Name = "windowsAuthenticationRadioButton";
            this.windowsAuthenticationRadioButton.Size = new System.Drawing.Size(234, 24);
            this.windowsAuthenticationRadioButton.TabIndex = 0;
            this.windowsAuthenticationRadioButton.TabStop = true;
            this.windowsAuthenticationRadioButton.Text = "Use &Windows Authentication";
            this.windowsAuthenticationRadioButton.CheckedChanged += new System.EventHandler(this.windowsAuthenticationRadioButton_CheckedChanged);
            // 
            // UserNameLabel
            // 
            this.UserNameLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.UserNameLabel.Location = new System.Drawing.Point(32, 64);
            this.UserNameLabel.Margin = new System.Windows.Forms.Padding(3, 2, 1, 3);
            this.UserNameLabel.Name = "UserNameLabel";
            this.UserNameLabel.Size = new System.Drawing.Size(80, 17);
            this.UserNameLabel.TabIndex = 9;
            this.UserNameLabel.Text = "&User name:";
            // 
            // ConnectionForm
            // 
            this.AcceptButton = this.connectButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(510, 298);
            this.Controls.Add(this.SecurityPanel);
            this.Controls.Add(this.dbBrowseButton);
            this.Controls.Add(this.dbTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.browseServerButton);
            this.Controls.Add(this.serverTextBox);
            this.Controls.Add(this.label1);
            this.Name = "ConnectionForm";
            this.Text = "Database Connection";
            this.Shown += new System.EventHandler(this.ConnectionForm_Shown);
            this.SecurityPanel.ResumeLayout(false);
            this.SecurityPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox serverTextBox;
        private System.Windows.Forms.Button browseServerButton;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button dbBrowseButton;
        private System.Windows.Forms.TextBox dbTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel SecurityPanel;
        private System.Windows.Forms.RadioButton userAndPasswordRadioButton;
        private System.Windows.Forms.RadioButton windowsAuthenticationRadioButton;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.TextBox userNameTextBox;
        private System.Windows.Forms.Label PasswordLabel;
        private System.Windows.Forms.Label UserNameLabel;
    }
}
