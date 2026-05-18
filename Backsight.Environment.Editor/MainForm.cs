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

using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.SqlServer.Management.Smo;
using Backsight.Data;
using Backsight.Database;
using Backsight.SqlServer;
using RepoDb;

namespace Backsight.Environment.Editor;

/// <summary>
/// Main dialog for working with Backsight environment settings.
/// </summary>
public partial class MainForm : Form
{
    private const string NO_NAME = "(Untitled)";

    /// <summary>
    /// The container that holds the environment settings.
    /// </summary>
    EnvironmentDatabase m_Data;

    public MainForm()
    {
        InitializeComponent();
    }

    void OnIdle(object sender, EventArgs args)
    {
        string name = EnvironmentRepository.Current.Name;

        if (String.IsNullOrEmpty(name))
        {
            this.Text = NO_NAME;
            fileSaveMenuItem.Enabled = false;
        }
        else
        {
            this.Text = name;
            fileSaveMenuItem.Enabled = true; //m_Data.IsModified;
        }
    }

    private void MainForm_Shown(object sender, EventArgs e)
    {
        GlobalConfiguration.Setup().UseSqlite();
        var repo = new EnvironmentRepository(@"Data Source=C:\ProgramData\Backsight\Manitoba.db;Mode=ReadWrite");
        repo.Load();

        Application.Idle += OnIdle;
        tabControl.SelectedTab = entityTypesPage;
        RefreshList();
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