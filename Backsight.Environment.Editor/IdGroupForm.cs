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
using Backsight.Database;

namespace Backsight.Environment.Editor;

public partial class IdGroupForm : Form
{
    /// <summary>
    /// The ID group that's being updated.
    /// </summary>
    private readonly IIdGroup m_Item;
    
    /// <summary>
    /// The current details for the ID group.
    /// </summary>
    private IdGroupDetail m_Details;

    /// <summary>
    /// The entity types that are currently associated with the ID group (null
    /// if the user hasn't yet displayed entity types).
    /// </summary>
    private IEntity[]? m_EntityTypes;

    internal IdGroupForm(IIdGroup? item)
    {
        InitializeComponent();

        m_Item = item ?? EnvironmentRepository.Current.CreateNewItem<IIdGroup>();
        m_Details = new IdGroupDetail(m_Item);
        m_EntityTypes = null;
    }

    private void IdGroupForm_Shown(object sender, EventArgs e)
    {
        groupNameTextBox.Text = m_Details.Name;
        minTextBox.Text = m_Details.LowestId.ToString();
        maxTextBox.Text = m_Details.HighestId.ToString();
        packetSizeTextBox.Text = m_Details.PacketSize.ToString();
    }

    private void cancelButton_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    /// <summary>
    /// Creates the group details based on the current form values.
    /// </summary>
    /// <returns>The current details (not necessarily valid).</returns>
    IdGroupDetail GetCurrentDetails()
    {
        string name = groupNameTextBox.Text.Trim();
        int minId = GetInt(minTextBox);
        int maxId = GetInt(maxTextBox);
        int psize = GetInt(packetSizeTextBox);
        
        // The HasCheckDigit and KeyFormat properties may have been changed via IdFormatForm 
        return new IdGroupDetail(name, minId, maxId, m_Details.HasCheckDigit, psize, m_Details.KeyFormat);
    }
    
    bool Validate(IdGroupDetail detail)
    {
        if (detail.Name.Length==0)
        {
            MessageBox.Show("A name must be supplied for the ID group");
            groupNameTextBox.Focus();
            return false;
        }

        if (detail.LowestId > detail.HighestId)
        {
            MessageBox.Show("Low end of range is bigger than the max");
            minTextBox.Focus();
            return false;
        }

        var numId = detail.HighestId - detail.LowestId + 1;
        if (detail.PacketSize > numId)
        {
            MessageBox.Show("Packet size exceeds the number of IDs in the group");
            packetSizeTextBox.Focus();
            return false;
        }
        
        return true;
    }

    private void okButton_Click(object sender, EventArgs e)
    {
        var details = GetCurrentDetails();
        if (!Validate(details))
            return;

        var repo = EnvironmentRepository.Current;
        var set = repo.GetSetter<IIdGroup, ISetIdGroup>(m_Item);
        
        set.Name = details.Name;
        set.LowestId = details.LowestId;
        set.HighestId = details.HighestId;
        set.PacketSize = details.PacketSize;
        set.KeyFormat = details.KeyFormat;
        set.HasCheckDigit = details.HasCheckDigit;

        repo.SaveChanges(m_Item, set);

        // Save any changes to the entity types associated with the ID group
        if (m_EntityTypes is not null)
            repo.SaveAssociatedEntities(m_Item, m_EntityTypes);

        DialogResult = DialogResult.OK;
        Close();
    }

    private void formatButton_Click(object sender, EventArgs e)
    {
        // Validate the current details
        var details = GetCurrentDetails();
        if (!Validate(details))
            return;

        var dial = new IdFormatForm(details);
        if (dial.ShowDialog() == DialogResult.OK)
        {
            m_Details = details with
            {
                HasCheckDigit = dial.HasCheckDigit,
                KeyFormat = dial.KeyFormat
            };
        }
        dial.Dispose();
    }

    IEntity[] GetEntityTypes()
    {
        if (m_EntityTypes is null)
            m_EntityTypes = m_Item.EntityTypes;

        return m_EntityTypes;
    }

    private void entitiesButton_Click(object sender, EventArgs e)
    {
        IEntity[] entities = EnvironmentRepository.Current.EntityTypes.OrderBy(x => x.Name).ToArray();
        IEntity[] selection = GetEntityTypes();
        var dial = new ChecklistForm<IEntity>(entities, selection);

        if (dial.ShowDialog() == DialogResult.OK)
            m_EntityTypes = dial.Selection;

        dial.Dispose();
    }

    int GetInt(TextBox tb)
    {
        string s = tb.Text.Trim();
        return s.Length==0 ? 0 : Int32.Parse(s);
    }
}