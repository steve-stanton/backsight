// <remarks>
// Copyright 2007 - Steve Stanton. This file is part of Backsight
//
// Backsight is free software; you can redistribute it and/or modify it under the terms
// of the GNU Lesser General Public License as published by the Free Software Foundation;
// either version 3 of the License, or (at your option) any later version.
//
// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
// </remarks>

using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;

using Microsoft.SqlServer.Management.Smo;

using Backsight.Data;
using Backsight.SqlServer;

namespace Backsight.Environment.Editor
{
    /// <summary>
    /// Main dialog for working with Backsight environment settings.
    /// </summary>
    public partial class MainForm : Form
    {
        private const string NO_NAME = "(Untitled)";

        #region Class data

        /// <summary>
        /// The container that holds the environment settings.
        /// </summary>
        EnvironmentDatabase m_Data;

        #endregion

        public MainForm()
        {
            InitializeComponent();

            //helpProvider.SetHelpNavigator(this, HelpNavigator.TableOfContents);
            //helpProvider.SetHelpNavigator(this, HelpNavigator.KeywordIndex);//HelpNavigator.Topic);
            //helpProvider.SetHelpKeyword(this, "Thecedxfile");

            //helpProvider.SetHelpNavigator(this, HelpNavigator.TopicId);
            //helpProvider.SetHelpNavigator(this, HelpNavigator.TopicId);
            //helpProvider.SetHelpKeyword(this, "DatabaseTables.htm");
            //SendKeys.Send(Keys.F1);
        }

        void OnIdle(object sender, EventArgs args)
        {
            string name = (m_Data==null ? String.Empty : m_Data.Name);

            if (String.IsNullOrEmpty(name))
            {
                this.Text = NO_NAME;
                fileSaveMenuItem.Enabled = false;
            }
            else
            {
                this.Text = name;
                fileSaveMenuItem.Enabled = m_Data.IsModified;
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            bool doClose = false;

            // If a database connection isn't defined, see if a database called 'Backsight' exists
            // on the local server. If not, ask the user to locate a database.

            string lastConn = LastDatabase.ConnectionString;
            bool lookedForDefault = false;

            if (String.IsNullOrEmpty(lastConn))
            {
                lastConn = TableFactory.GetDefaultConnection();
                lookedForDefault = true;

                if (!String.IsNullOrEmpty(lastConn))
                    LastDatabase.ConnectionString = lastConn;
            }

            if (String.IsNullOrEmpty(lastConn) && lookedForDefault)
            {
                string msg = String.Empty;
                msg += ("The Environment Editor doesn't have a record of the database" + System.Environment.NewLine);
                msg += ("where information should be stored. Please pick an existing" + System.Environment.NewLine);
                msg += ("database, or click Cancel to exit.");

                if (MessageBox.Show(msg, "Database unknown", MessageBoxButtons.OKCancel)==DialogResult.Cancel)
                    doClose = true;
                else if (OpenDatabase()==null)
                    doClose = true;
            }
            else
            {
                if (OpenDatabase(lastConn) == null)
                {
                    string msg = String.Empty;
                    msg += (String.Format("Unable to access database '{0}'", lastConn) + System.Environment.NewLine);
                    msg += ("Please pick the database you want to access, or click Cancel to exit.");

                    if (MessageBox.Show(msg, "Cannot access database", MessageBoxButtons.OKCancel)==DialogResult.Cancel)
                        doClose = true;
                    else
                    {
                        LastDatabase.ConnectionString = String.Empty;
                        if (OpenDatabase()==null)
                            doClose = true;
                    }
                }
            }

            if (doClose)
                Close();
            else
            {
                Application.Idle += OnIdle;
                tabControl.SelectedTab = entityTypesPage;
                RefreshList();
            }
        }

        bool CheckSave()
        {
            if (m_Data==null)
                return true;

            if (m_Data.IsEmpty)
                return true;

            if (!m_Data.IsModified)
                return true;

            string name = m_Data.Name;
            if (String.IsNullOrEmpty(name))
                name = NO_NAME;

            string msg = String.Format("Save changes to {0}?", name);
            DialogResult res = MessageBox.Show(msg, "Unsaved Changes", MessageBoxButtons.YesNoCancel);
            if (res==DialogResult.Cancel)
                return false;

            if (res==DialogResult.Yes && !SaveData())
                    return false;

            return true;
        }

        private void fileOpenMenuItem_Click(object sender, EventArgs e)
        {
            OpenDatabase();
            RefreshList();
        }

        IEnvironmentContainer OpenDatabase()
        {
            ConnectionForm dial = new ConnectionForm();
            if (dial.ShowDialog() == DialogResult.OK)
            {
                Database db = dial.Database;
                TableFactory tf = new TableFactory(db);
                if (!ConfirmTablesExist(tf))
                    return null;

                return OpenDatabase(tf.ConnectionString);
            }

            dial.Dispose();
            return null;
        }

        IEnvironmentContainer OpenDatabase(string connectionString)
        {
            try
            {
                TableFactory tf = new TableFactory(connectionString);
                if (!ConfirmTablesExist(tf))
                    throw new Exception("Cannot load Backsight system tables");

                m_Data = new EnvironmentDatabase(connectionString);
                EnvironmentContainer.Current = m_Data;
                LastDatabase.ConnectionString = connectionString;
                return m_Data;
            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return null;
        }

        /// <summary>
        /// Confirms that Backsight system tables exist. This checks whether a database schema
        /// called "ced" has been created.
        /// </summary>
        /// <param name="tf">The factory that can be used to create database tables in an
        /// associated database.</param>
        bool ConfirmTablesExist(TableFactory tf)
        {
            if (tf==null)
                return false;

            if (tf.DoTablesExist())
                return true;

            CreateTablesForm ctf = new CreateTablesForm(tf);
            bool isCreatedOk = (ctf.ShowDialog()==DialogResult.OK);
            ctf.Dispose();
            return isCreatedOk;
        }

        private void fileSaveMenuItem_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        bool SaveData()
        {
            try
            {
                m_Data.Write();
                return true;
            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return false;
        }

        private void fileExportMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dial = new SaveFileDialog();
            dial.Filter = "Backsight environment files (*.xml)|*.xml|All files (*.*)|*.*";
            dial.DefaultExt = ".xml";

            if (dial.ShowDialog() == DialogResult.OK)
                WriteExportFile(dial.FileName);

            dial.Dispose();
        }

        void WriteExportFile(string fileName)
        {
            // A by-product of the following is that the database name gets re-assigned
            // to the supplied filename, so we'll need to fix it up. Should really handle
            // names a bit better.
            string dbName = m_Data.Name;

            try
            {
                EnvironmentFile ef = new EnvironmentFile(fileName, m_Data);
                ef.Write();
            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            finally
            {
                m_Data.Name = dbName;
                MessageBox.Show("Done");
            }
        }

        private void fileImportMenuItem_Click(object sender, EventArgs e)
        {
            // Confirm that everything currently in the database will be blown away
            string msg = String.Empty;
            msg += ("Importing will replace the content of current database." + System.Environment.NewLine);
            msg += ("Are you sure that's what you want to do?");
            if (MessageBox.Show(msg, "Confirm Import", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                return;

            OpenFileDialog dial = new OpenFileDialog();
            dial.Title = "Locate file containing the new environment";
            dial.Filter = "Backsight environment files (*.xml)|*.xml|All files (*.*)|*.*";

            if (dial.ShowDialog() == DialogResult.OK)
            {
                // Load the file into its own dataset
                EnvironmentFile ef = new EnvironmentFile(dial.FileName);

                // Get rid of the content of the current database (including empty rows)
                m_Data.Replace(ef);
                RefreshList();
                MessageBox.Show("Done");
            }

            dial.Dispose();
        }

        private void fileExitMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CheckSave())
            {
                e.Cancel = true;
                return;
            }

            Application.Idle -= OnIdle;
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            IDisplayControl display = GetCurrentDisplay();
            if (display != null)
                display.NewItem();
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            IDisplayControl display = GetCurrentDisplay();
            if (display != null)
                display.UpdateSelectedItem();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            IDisplayControl display = GetCurrentDisplay();
            if (display != null)
                display.DeleteSelectedItem();
        }

        private void RefreshList()
        {
            IDisplayControl display = GetCurrentDisplay();
            if (display != null)
                display.RefreshList();
        }

        /// <summary>
        /// Reacts to the selection of a specific tab page by ensuring that
        /// a display has been attached.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl_Selected(object sender, TabControlEventArgs e)
        {
            IDisplayControl display = (IDisplayControl)e.TabPage.Tag;
            if (display == null)
            {
                if (e.TabPage == domainsPage)
                    AttachListData<DomainListData>(domainsPage);
                else if (e.TabPage == entityTypesPage)
                    AttachListData<EntityListData>(entityTypesPage);
                else if (e.TabPage == fontsPage)
                    AttachListData<FontListData>(fontsPage);
                else if (e.TabPage == idGroupsPage)
                    AttachListData<IdGroupListData>(idGroupsPage);
                else if (e.TabPage == layersPage)
                    AttachListData<LayerListData>(layersPage);
                else if (e.TabPage == propertiesPage)
                    AttachDisplay<PropertyGridControl>(propertiesPage);
                else if (e.TabPage == tablesPage)
                    AttachListData<TableListData>(tablesPage);
                else if (e.TabPage == templatesPage)
                    AttachListData<TemplateListData>(templatesPage);
                else if (e.TabPage == themesPage)
                    AttachListData<ThemeListData>(themesPage);
                else if (e.TabPage == zonesPage)
                    AttachListData<ZoneListData>(zonesPage);
                else
                    throw new Exception("No display for tab page");

                display = (IDisplayControl)e.TabPage.Tag;
            }

            Debug.Assert(display != null);

            // Ensure the display is up to date. This is meant to cover the
            // fact that items on one page may have been removed via changes
            // on other pages.
            display.RefreshList();
        }

        /// <summary>
        /// Attaches a display to a tab page. For a given tab page, this should
        /// be done only once while the application is running.
        /// </summary>
        /// <typeparam name="T">The type of display to attach</typeparam>
        /// <param name="page">The page to add the display to</param>
        void AttachDisplay<T>(TabPage page) where T : UserControl, IDisplayControl, new()
        {
            AttachDisplay<T>(page, new T());
        }

        /// <summary>
        /// Attaches a display to a tab page. For a given tab page, this should
        /// be done only once while the application is running.
        /// </summary>
        /// <typeparam name="T">The type of display to attach</typeparam>
        /// <param name="page">The page to add the display to</param>
        /// <param name="display">The display control to add</param>
        void AttachDisplay<T>(TabPage page, T display) where T : UserControl, IDisplayControl
        {
            display.Dock = DockStyle.Fill;
            page.Tag = display;
            page.Controls.Add(display);
            display.RefreshList();
        }

        /// <summary>
        /// Attaches an instance of <see cref="SimpleListControl"/> to a tab page, using
        /// a specific data provider. For a given tab page, this should be done only once
        /// while the application is running.
        /// </summary>
        /// <typeparam name="T">The object that provides data for the display</typeparam>
        /// <param name="page">The page to add the display to</param>
        void AttachListData<T>(TabPage page) where T : ISimpleListData, new()
        {
            T listData = new T();
            SimpleListControl display = new SimpleListControl(listData);
            AttachDisplay<SimpleListControl>(page, display);
        }

        /// <summary>
        /// Obtains the display associated with the currently selected tab page.
        /// </summary>
        /// <returns>The selected display (null if no tabs are selected, or
        /// a display is not attached to the tab).</returns>
        IDisplayControl GetCurrentDisplay()
        {
            TabPage page = tabControl.SelectedTab;
            if (page == null)
                return null;

            return (page.Tag as IDisplayControl);
        }
    }
}
