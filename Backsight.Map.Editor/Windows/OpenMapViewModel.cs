using System.Linq;
using Backsight.Model;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Backsight.Map.Editor.Windows;

public partial class OpenMapViewModel : DialogViewModel
{
    public string[] MapNames { get; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OkCommand))]
    private string? _selectedMapName;

    public OpenMapViewModel(IMapRepository mapRepository)
    {
        MapNames = mapRepository.FindAllMapNames().ToArray();
    }
    
    protected override bool CanExecuteOk()
    {
        return SelectedMapName is not null;
    }
}
