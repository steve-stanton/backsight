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
using System.Reflection;
using Backsight.Database;
using RepoDb;

namespace Backsight.Environment.Editor;

/// <summary>
/// Main dialog for working with Backsight environment settings.
/// </summary>
public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
    }

    private void MainForm_Shown(object sender, EventArgs e)
    {
        try
        {
            GlobalConfiguration.Setup().UseSqlite();
            var connectionString = GetConnectionString("Manitoba.db");
            Console.WriteLine($"Loading environment database from {connectionString}");
            var repo = new EnvironmentRepository(connectionString);
            repo.Load();

            Text = repo.Name;
            tabControl.SelectedTab = entityTypesPage;
            RefreshList();
        }
        catch
        {
            MessageBox.Show("Error loading environment database");
            Close();
        }
    }

    private static string GetConnectionString(string dbName)
    {
        var appData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData);
        var dbFolder = Path.Combine(appData, "Backsight");

        // If we don't already have a dedicated folder under ProgramData, use the location of the entry assembly
        if (!Directory.Exists(dbFolder))
            dbFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location ?? throw new ApplicationException());

        var dbPath = Path.Combine(dbFolder, dbName);
        
        if (!File.Exists(dbPath))
            throw new FileNotFoundException("Database file not found: ", dbPath);

        return $"Data Source={dbPath};Mode=ReadWrite";
    }

    private void fileExitMenuItem_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void newButton_Click(object sender, EventArgs e)
    {
        GetCurrentDisplay()?.NewItem();
    }

    private void updateButton_Click(object sender, EventArgs e)
    {
        GetCurrentDisplay()?.UpdateSelectedItem();
    }

    private void deleteButton_Click(object sender, EventArgs e)
    {
        GetCurrentDisplay()?.DeleteSelectedItem();
    }

    private void RefreshList()
    {
        GetCurrentDisplay()?.RefreshList();
    }

    /// <summary>
    /// Reacts to the selection of a specific tab page by ensuring that
    /// a display has been attached.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tabControl_Selected(object sender, TabControlEventArgs e)
    {
        var display = (IDisplayControl?)e.TabPage.Tag;
        if (display is null)
        {
            if (e.TabPage == domainsPage)
                AttachListData<DomainListData, IDomainTable>(domainsPage);
            else if (e.TabPage == entityTypesPage)
                AttachListData<EntityListData, IEntity>(entityTypesPage);
            else if (e.TabPage == fontsPage)
                AttachListData<FontListData, IFont>(fontsPage);
            else if (e.TabPage == idGroupsPage)
                AttachListData<IdGroupListData, IIdGroup>(idGroupsPage);
            else if (e.TabPage == layersPage)
                AttachListData<LayerListData, ILayer>(layersPage);
            else if (e.TabPage == propertiesPage)
                AttachDisplay<PropertyGridControl>(propertiesPage);
            else if (e.TabPage == tablesPage)
                AttachListData<TableListData, ITable>(tablesPage);
            else if (e.TabPage == templatesPage)
                AttachListData<TemplateListData, ITemplate>(templatesPage);
            else if (e.TabPage == themesPage)
                AttachListData<ThemeListData, ITheme>(themesPage);
            else
                throw new Exception("No display for tab page");

            display = (IDisplayControl?)e.TabPage.Tag;
        }

        Debug.Assert(display is not null);

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
    void AttachListData<TData, TItem>(TabPage page)
        where TData : ISimpleListData<TItem>, new()
        where TItem : class, IEnvironmentItem
    {
        TData listData = new TData();
        var display = new SimpleListControl<TItem>(listData);
        AttachDisplay(page, display);
    }

    /// <summary>
    /// Obtains the display associated with the currently selected tab page.
    /// </summary>
    /// <returns>The selected display (null if no tabs are selected, or
    /// a display is not attached to the tab).</returns>
    IDisplayControl? GetCurrentDisplay()
    {
        TabPage? page = tabControl.SelectedTab;
        if (page is null)
            return null;

        return page.Tag as IDisplayControl;
    }
}