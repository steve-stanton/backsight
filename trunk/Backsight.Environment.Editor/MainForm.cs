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

using Microsoft.SqlServer.Management.Smo;

using Backsight.Data;
using Backsight.SqlServer;
using System.Reflection;

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
        /// The type of info that's currently displayed 
        /// </summary>
        ItemType m_CurrentType = ItemType.None;

        /// <summary>
        /// The container that holds the environment settings.
        /// </summary>
        EnvironmentDatabase m_Data;

        #endregion

        public MainForm()
        {
            InitializeComponent();
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
            m_CurrentType = ItemType.Entity;

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
            Form dial = null;
            if (m_CurrentType == ItemType.Entity)
                dial = new EntityForm();
            else if (m_CurrentType == ItemType.Font)
                dial = new FontForm();
            else if (m_CurrentType == ItemType.IdGroup)
                dial = new IdGroupForm();
            else if (m_CurrentType == ItemType.Layer)
                dial = new LayerForm();
            else if (m_CurrentType == ItemType.Schema)
                dial = new TableForm();
            else if (m_CurrentType == ItemType.Theme)
                dial = new ThemeForm();
            else if (m_CurrentType == ItemType.Template)
                dial = new TemplateForm();
            else if (m_CurrentType == ItemType.Zone)
                dial = new ZoneForm();

            if (dial==null)
            {
                MessageBox.Show("Unexpected item type: "+m_CurrentType);
                return;
            }

            bool ok = (dial.ShowDialog() == DialogResult.OK);
            dial.Dispose();

            if (ok)
                RefreshList();
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            IEnvironmentItem item = (IEnvironmentItem)listBox.SelectedItem;
            if (item==null)
            {
                MessageBox.Show("You must first select an item from the list");
                return;
            }

            Update(item);
        }

        private void listBox_DoubleClick(object sender, EventArgs e)
        {
            IEnvironmentItem item = (IEnvironmentItem)listBox.SelectedItem;
            if (item!=null)
                Update(item);
        }

        void Update(IEnvironmentItem item)
        {
            Form dial =null;

            if (m_CurrentType == ItemType.Entity)
                dial = new EntityForm((IEditEntity)item);
            else if (m_CurrentType == ItemType.Font)
                dial = new FontForm((IEditFont)item);
            else if (m_CurrentType == ItemType.IdGroup)
                dial = new IdGroupForm((IEditIdGroup)item);
            else if (m_CurrentType == ItemType.Layer)
                dial = new LayerForm((IEditLayer)item);
            else if (m_CurrentType == ItemType.Schema)
                dial = new TableForm((IEditTable)item);
            else if (m_CurrentType == ItemType.Theme)
                dial = new ThemeForm((IEditTheme)item);
            else if (m_CurrentType == ItemType.Template)
                dial = new TemplateForm((IEditTemplate)item);
            else if (m_CurrentType == ItemType.Zone)
                dial = new ZoneForm((IEditZone)item);

            if (dial==null)
            {
                MessageBox.Show("Unexpected item type: "+m_CurrentType);
                return;
            }

            bool ok = (dial.ShowDialog() == DialogResult.OK);
            dial.Dispose();

            if (ok)
                RefreshList();

        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            IEnvironmentItem item = (IEnvironmentItem)listBox.SelectedItem;
            if (item==null)
            {
                MessageBox.Show("You must first select an item from the list");
                return;
            }

            // Deletions should be disallowed if the environment has been "published"

            if (item is IEditControl)
            {
                (item as IEditControl).Delete();
                RefreshList();
            }
            else
                throw new NotSupportedException();
        }

        void RefreshList()
        {
            string typeName = String.Empty;

            switch (m_CurrentType)
            {
                case ItemType.Entity:
                {
                    typeName = "entity type";
                    RefreshList(m_CurrentType, m_Data.EntityTypes);
                    break;
                }

                case ItemType.Font:
                {
                    typeName = "font";
                    RefreshList(m_CurrentType, m_Data.Fonts);
                    break;
                }

                case ItemType.IdGroup:
                {
                    typeName = "ID group";
                    RefreshList(m_CurrentType, m_Data.IdGroups);
                    break;
                }

                case ItemType.Layer:
                {
                    typeName = "layer";
                    RefreshList(m_CurrentType, m_Data.Layers);
                    break;
                }

                case ItemType.Schema:
                {
                    typeName = "table";
                    RefreshList(m_CurrentType, m_Data.Tables);
                    break;
                }

                case ItemType.Template:
                {
                    typeName = "template";
                    RefreshList(m_CurrentType, m_Data.Templates);
                    break;
                }

                case ItemType.Theme:
                {
                    typeName = "theme";
                    RefreshList(m_CurrentType, m_Data.Themes);
                    break;
                }

                case ItemType.Zone:
                {
                    typeName = "zone";
                    RefreshList(m_CurrentType, m_Data.Zones);
                    break;
                }

                default:
                {
                    MessageBox.Show("Unexpected item type: "+m_CurrentType);
                    break;
                }
            }

            int nRows = listBox.Items.Count;
            if (nRows>1)
                typeName += "s";

            countLabel.Text = String.Format("{0} {1}", nRows, typeName);
        }

        private void viewEntityTypesMenuItem_Click(object sender, EventArgs e)
        {
            RefreshList(ItemType.Entity);
        }

        private void viewFontsMenuItem_Click(object sender, EventArgs e)
        {
            RefreshList(ItemType.Font);
        }

        private void viewIdGroupsMenuItem_Click(object sender, EventArgs e)
        {
            RefreshList(ItemType.IdGroup);
        }

        private void viewLayersMenuItem_Click(object sender, EventArgs e)
        {
            RefreshList(ItemType.Layer);
        }

        private void viewTablesMenuItem_Click(object sender, EventArgs e)
        {
            RefreshList(ItemType.Schema);
        }

        private void viewTemplateMenuItem_Click(object sender, EventArgs e)
        {
            RefreshList(ItemType.Template);
        }

        private void viewThemesMenuItem_Click(object sender, EventArgs e)
        {
            RefreshList(ItemType.Theme);
        }

        private void viewZonesMenuItem_Click(object sender, EventArgs e)
        {
            RefreshList(ItemType.Zone);
        }

        void RefreshList(ItemType t)
        {
            m_CurrentType = t;
            RefreshList();
        }

        void RefreshList(ItemType itemType, IEnvironmentItem[] items)
        {
            m_CurrentType = itemType;

            listBox.Items.Clear();
            listBox.Items.AddRange(items);

            // The Update button isn't relevant when dealing with associated tables
            updateButton.Enabled = (itemType != ItemType.Schema);

            // If the first item is blank, remove it (all "real" items should have
            // a defined name, blanks refer to rows that exist only to accommodate
            // foreign key constraints)
            if (items.Length>0 && listBox.Items[0].ToString().Length==0)
                listBox.Items.RemoveAt(0);
        }

        private void viewPropertiesMenuItem_Click(object sender, EventArgs e)
        {
            PropertyListForm dial = new PropertyListForm();
            dial.ShowDialog();
            dial.Dispose();
        }
    }
}
