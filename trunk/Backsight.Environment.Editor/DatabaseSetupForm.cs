using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Reflection;


namespace CommonInstaller
{
    public enum NavServerSetupTask { Create, Upgrade };

    public partial class DatabaseSetupForm : Form
    {
        private String m_serverName = "";
        private String m_databaseName = "";
        private String m_instanceName = "";
        private String m_instance = "";
        private String m_ecpLocation = "";
        private String m_dbLocation = "";
        private String m_backupLocation = "";

        private String m_userName = "";
        private String m_password = "";

        private String m_dbSetupFileName;
        private String m_sdeSetupFileName = String.Empty;
        private String m_dbScriptPath;

        private NavServerType m_type;
        private NavServerSetupTask m_task;
        private String m_appName;

        public DatabaseSetupForm(NavServerType type, NavServerSetupTask task, String dbSetupFileName, String sdeSetupFileName, String appName)
        {
            InitializeComponent();
            m_dbSetupFileName = dbSetupFileName;
            m_sdeSetupFileName = sdeSetupFileName;
            m_type = type;
            m_task = task;
            m_appName = appName;

        }

        private void DatabaseSetupForm_Load(object sender, EventArgs e)
        {
            try
            {
                SetupControls();
                ManageControls();
            }
            catch (Exception exc)
            {
                InstallUtility.ExceptionHandler(exc, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
        }

        // 170507 EDS #892 Only lookup setup files in CreateNewProject. Removed redundant code
        private void cmdContinue_Click(object sender, EventArgs e)
        {
            m_serverName = txtServerName.Text;
            m_databaseName = txtDatabaseName.Text;
            m_instanceName = txtInstanceName.Text;
            m_instance = txtInstance.Text;
            m_ecpLocation = txtLicenseFile.Text;
            m_dbLocation = txtLocation.Text;
            m_backupLocation = txtLocationBackup.Text;

            m_userName = txtUsername.Text;
            m_password = txtPassword.Text;


            try
            {
                switch (m_task)
                {
                    case NavServerSetupTask.Create:
                        CreateNewProject();
                        break;
                    case NavServerSetupTask.Upgrade:
                        StatusCheck.Perform(m_serverName, m_databaseName, rdoUseSQLServerAuthentication.Checked, m_userName, m_password, m_dbLocation, m_backupLocation, m_instance, m_instanceName, m_sdeSetupFileName);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception exc)
            {
                InstallUtility.ExceptionHandler(exc, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
            DialogResult = DialogResult.OK;
            this.Close();
        }

        // 170507 EDS #892 Only lookup setup files in CreateNewProject
        private void CreateNewProject()
        {
            try
            {
                String scriptLocation = FileLocator.FindFile(m_dbSetupFileName, Path.GetDirectoryName(Application.ExecutablePath), false);
                m_dbScriptPath = Path.GetDirectoryName(scriptLocation);
                if (CreateSQLDatabase())
                {
                    if (CreateSDEDatabase())
                    {
                        String finishedString = String.Format(Strings.Value((m_task == NavServerSetupTask.Create) ? "strDBCreationComplete" : "strDBUpgradeComplete"),
                                                                m_databaseName, m_serverName);
                        MessageBox.Show(Strings.Value(finishedString, Assembly.GetExecutingAssembly()));
                    }
                }
            }
            catch (Exception exc)
            {
                InstallUtility.ExceptionHandler(exc, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private Boolean CreateSQLDatabase()
        {
            Boolean finished = false;
            try
            {
                StringBuilder m_shortPath = new StringBuilder(1000);
                InstallUtility.GetShortPathName(m_dbLocation, m_shortPath, m_shortPath.Capacity);

                StringBuilder m_shortBackupPath = new StringBuilder(1000);
                InstallUtility.GetShortPathName(m_backupLocation, m_shortBackupPath, m_shortBackupPath.Capacity);

                String arguments;
                String typeStr = (m_type == NavServerType.Central) ? "CNS" : "BNS";

                if (rdoUseSQLServerAuthentication.Checked)
                {
                    arguments = String.Format("{0} {1} SQL {2} {3} {4} {5} {6}", m_serverName, m_databaseName, typeStr, m_shortPath.ToString(), m_shortBackupPath.ToString(), m_userName, m_password);
                }
                else
                {
                    arguments = String.Format("{0} {1} WIN {2} {3} {4}", m_serverName, m_databaseName, typeStr, m_shortPath.ToString(), m_shortBackupPath.ToString());
                }
                BatchProgressForm dbFrm = new BatchProgressForm(m_dbScriptPath, m_dbSetupFileName, arguments);
                if (dbFrm.ShowDialog() == DialogResult.OK)
                {
                    finished = true;
                }
            }
            catch (Exception exc)
            {
                InstallUtility.ExceptionHandler(exc, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
            return finished;
        }

        private Boolean CreateSDEDatabase()
        {
            Boolean finished = false;
            try
            {
                if ((m_type == NavServerType.Base) && (m_sdeSetupFileName != String.Empty))
                {
                    StringBuilder m_shortPath = new StringBuilder(1000);
                    InstallUtility.GetShortPathName(m_ecpLocation, m_shortPath, m_shortPath.Capacity);

                    String arguments = String.Format("{0} {1} {2} {3} {4}", m_serverName, m_databaseName, m_instanceName, m_instance, m_shortPath.ToString());

                    BatchProgressForm dbFrm = new BatchProgressForm(m_dbScriptPath, m_sdeSetupFileName, arguments);
                    if (dbFrm.ShowDialog() == DialogResult.OK)
                    {
                        finished = true;
                    }
                }
            }
            catch (Exception exc)
            {
                InstallUtility.ExceptionHandler(exc, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
            return finished;
        }

        private void cmdBrowseLicense_Click(object sender, EventArgs e)
        {
            
            if (dlgOpen.ShowDialog(this) == DialogResult.OK)
            {
                txtLicenseFile.Text = dlgOpen.FileName;
            }
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void cmdBrowseLocation_Click(object sender, EventArgs e)
        {
            if (System.IO.Directory.Exists(txtLocation.Text))
            {
                dlgBrowse.SelectedPath = txtLocation.Text;
            }
            if (dlgBrowse.ShowDialog(this) == DialogResult.OK)
            {
                txtLocation.Text = dlgBrowse.SelectedPath;
            }
        }

        /// <summary>
        /// SetupControls : hides controls and resizes display depending on nav server type and installation task
        /// </summary>
        // 271006 EDS #416 Initialise the server name text box to the machine name
        private void SetupControls()
        {
            try
            {
                txtServerName.Text = System.Environment.MachineName;
                rdoUseWindowsAuthentication.Checked = true;

                grpSDESettings.Visible = ((m_task == NavServerSetupTask.Create) && (m_type == NavServerType.Base));
                grpLocations.Visible = (m_task == NavServerSetupTask.Create);               

                if (grpSDESettings.Visible == false)
                {
                    this.Height -= grpSDESettings.Height;
                }
                if (grpLocations.Visible == false)
                {
                    this.Height -= grpLocations.Height;
                }
            }
            catch (Exception exc)
            {
                InstallUtility.ExceptionHandler(exc, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
        }

        /// <summary>
        /// ManageControls : enables controls based on user settings
        /// </summary>
        private void ManageControls()
        {
            lblUsername.Enabled = rdoUseSQLServerAuthentication.Checked;
            lblPassword.Enabled = rdoUseSQLServerAuthentication.Checked;
            txtUsername.Enabled = rdoUseSQLServerAuthentication.Checked;
            txtPassword.Enabled = rdoUseSQLServerAuthentication.Checked;
        }

        private void chk_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                ManageControls();
            }
            catch (Exception exc)
            {
                InstallUtility.ExceptionHandler(exc, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
        }

        private void cmdBrowseBackup_Click(object sender, EventArgs e)
        {
            if (System.IO.Directory.Exists(txtLocationBackup.Text))
            {
                dlgBrowse.SelectedPath = txtLocationBackup.Text;
            }
            if (dlgBrowse.ShowDialog(this) == DialogResult.OK)
            {
                txtLocationBackup.Text = dlgBrowse.SelectedPath;
            }
        }

        private void rdoAuthentication_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                ManageControls();
            }
            catch (Exception exc)
            {
                InstallUtility.ExceptionHandler(exc, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
        }
    }
}

