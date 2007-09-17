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

using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

using Microsoft.SqlServer.Management.Smo;

using Backsight.Data;
using Edit=Backsight.Environment.Editor.Properties;
using Backsight.SqlServer;
using Backsight.Data.BacksightDataSetTableAdapters;

namespace Backsight.Environment.Editor
{
    public partial class MainForm : Form
    {
        private const string NO_NAME = "(Untitled)";

        /// <summary>
        /// The type of info that's currently displayed 
        /// </summary>
        ItemType m_CurrentType = ItemType.None;

        IEnvironmentContainer m_Data;

        public MainForm()
        {
            InitializeComponent();
            CreateNew();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Application.Idle += OnIdle;
        }

        void OnIdle(object sender, EventArgs args)
        {
            //string name = (m_Data as EnvironmentFile).Path;
            string name = m_Data.Name;

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
            m_CurrentType = ItemType.Entity;

            string lastConn = Edit.Settings.Default.LastConnection;
            if (String.IsNullOrEmpty(lastConn))
                return;

            if (File.Exists(lastConn))
                OpenFile(lastConn);
            else
                OpenDatabase(lastConn);

            RefreshList();
        }

        private void fileNewMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckSave())
                CreateNew();
        }

        void CreateNew()
        {
            m_Data = new EnvironmentFile();
            EnvironmentContainer.Current = m_Data;
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

            if (res==DialogResult.Yes && !Save())
                    return false;

            return true;
        }

        private void fileOpenFileMenuItem_Click(object sender, EventArgs e)
        {
            if (!CheckSave())
                return;

            OpenFileDialog dial = new OpenFileDialog();
            dial.Filter = "Backsight environment files (*.bse)|*.bse|All files (*.*)|*.*";

            string lastConn = Edit.Settings.Default.LastConnection;
            if (!String.IsNullOrEmpty(lastConn))
            {
                string lastDir = Path.GetDirectoryName(lastConn);
                if (Directory.Exists(lastDir))
                    dial.InitialDirectory = lastDir;
            }

            if (dial.ShowDialog() == DialogResult.OK)
                OpenFile(dial.FileName);

            dial.Dispose();
        }

        private void fileOpenDatabaseMenuItem_Click(object sender, EventArgs e)
        {
            ConnectionForm dial = new ConnectionForm();
            if (dial.ShowDialog() == DialogResult.OK)
            {
                Database db = dial.Database;
                TableFactory tf = new TableFactory(db);
                OpenDatabase(tf.ConnectionString);
                RefreshList();
            }
            dial.Dispose();
        }

        void OpenFile(string fileName)
        {
            try
            {
                m_Data = new EnvironmentFile(fileName);
                EnvironmentContainer.Current = m_Data;
                Edit.Settings.Default.LastConnection = fileName;
                Edit.Settings.Default.Save();
            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        void OpenDatabase(string connectionString)
        {
            try
            {
                m_Data = new EnvironmentDatabase(connectionString);
                EnvironmentContainer.Current = m_Data;
                Edit.Settings.Default.LastConnection = connectionString;
                Edit.Settings.Default.Save();
            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void fileSaveMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        bool Save()
        {
            if (String.IsNullOrEmpty(m_Data.Name))
                return SaveAs();
            else
                return SaveData();
        }

        private void fileSaveAsFileMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        private void fileSaveAsDatabaseMenuItem_Click(object sender, EventArgs e)
        {

        }

        bool SaveAs()
        {
            SaveFileDialog dial = new SaveFileDialog();
            dial.Filter = "Backsight environment files (*.bse)|*.bse|All files (*.*)|*.*";
            dial.DefaultExt = ".bse";

            string lastConn = Edit.Settings.Default.LastConnection;
            if (!String.IsNullOrEmpty(lastConn) && File.Exists(lastConn))
            {
                string lastDir = Path.GetDirectoryName(lastConn);
                if (Directory.Exists(lastDir))
                    dial.InitialDirectory = lastDir;
            }

            bool result = false;
            if (dial.ShowDialog() == DialogResult.OK)
                result = SaveAsFile(dial.FileName);

            dial.Dispose();
            return result;
        }

        bool SaveAsFile(string fileName)
        {
            try
            {
                // EnvironmentData is the base class for both EnvironmentFile & EnvironmentDatabase
                if (!(m_Data is EnvironmentData))
                    throw new NotSupportedException("Unexpected environment container");

                EnvironmentFile ef = new EnvironmentFile(fileName, (EnvironmentData)m_Data);
                ef.Write();
                OpenFile(fileName);
                return true;
            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return false;
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
            else if (m_CurrentType == ItemType.IdGroup)
                dial = new IdGroupForm();
            else if (m_CurrentType == ItemType.Layer)
                dial = new LayerForm();
            else if (m_CurrentType == ItemType.Theme)
                dial = new ThemeForm();

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
            else if (m_CurrentType == ItemType.IdGroup)
                dial = new IdGroupForm((IEditIdGroup)item);
            else if (m_CurrentType == ItemType.Layer)
                dial = new LayerForm((IEditLayer)item);
            else if (m_CurrentType == ItemType.Theme)
                dial = new ThemeForm((IEditTheme)item);

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

                case ItemType.Theme:
                {
                    typeName = "theme";
                    RefreshList(m_CurrentType, m_Data.Themes);
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

        private void viewIdGroupsMenuItem_Click(object sender, EventArgs e)
        {
            RefreshList(ItemType.IdGroup);
        }

        private void viewLayersMenuItem_Click(object sender, EventArgs e)
        {
            RefreshList(ItemType.Layer);
        }

        private void viewThemesMenuItem_Click(object sender, EventArgs e)
        {
            RefreshList(ItemType.Theme);
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

            // If the first item is blank, remove it (all "real" items should have
            // a defined name, blanks refer to rows that exist only to accommodate
            // foreign key constraints)
            if (items.Length>0 && listBox.Items[0].ToString().Length==0)
                listBox.Items.RemoveAt(0);
        }

        private void fileDatabaseMenuItem_Click(object sender, EventArgs e)
        {
            ConnectionForm dial = new ConnectionForm();
            if (dial.ShowDialog() == DialogResult.OK)
            {
                Database db = dial.Database;
                TableFactory tf = new TableFactory(db);
                tf.CreateTables(OnTableCreate);
                IEnvironmentContainer ec = EnvironmentContainer.Current;
                if (ec is EnvironmentData)
                {
                    BacksightDataSet ds = (ec as EnvironmentData).Data;
                    ds.Save(tf.ConnectionString);
                }
                tf.CreateForeignKeyConstraints();
            }
            dial.Dispose();
        }

        void OnTableCreate(string tableName)
        {

        }
    }
}
