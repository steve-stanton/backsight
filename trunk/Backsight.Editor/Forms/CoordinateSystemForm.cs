using System;
using System.IO;
using System.Windows.Forms;

using CSLib;

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Dialog for selecting a coordinate system. Depends on an environment variable
    /// called CS_MAP_DIR (referring to the CSMap folder containing dictionary files).
    /// </summary>
    public partial class CoordinateSystemForm : Form
    {
        CoordinateSystemDef m_SelectedSystem;

        public CoordinateSystemForm()
        {
            InitializeComponent();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            string csFolder = System.Environment.GetEnvironmentVariable("CS_MAP_DIR");
            CoordinateSystemCatalog cat = new CoordinateSystemCatalog(csFolder);
            cat.Load();
            catListBox.Items.AddRange(cat.Categories);
        }

        private void catListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            csListBox.Items.Clear();
            csPropertyGrid.SelectedObject = null;

            CategoryDef cat = (catListBox.SelectedItem as CategoryDef);
            if (cat != null)
                csListBox.Items.AddRange(cat.Systems);
        }

        private void csListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            CoordinateSystemDef csDef = (csListBox.SelectedItem as CoordinateSystemDef);
            csPropertyGrid.SelectedObject = csDef;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            CoordinateSystemDef cs = (csListBox.SelectedItem as CoordinateSystemDef);
            if (cs == null)
            {
                MessageBox.Show("You haven't selected a coordinate system");
                return;
            }

            m_SelectedSystem = cs;
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        internal CoordinateSystemDef SelectedSystem
        {
            get { return m_SelectedSystem; }
        }
    }
}
