using System.Windows.Input;

namespace Backsight.Map.Editor.ViewModels;

public record RecentMap(string MapName, ICommand OpenCommand);
