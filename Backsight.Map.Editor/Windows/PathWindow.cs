using Backsight.Map.Editor.Tools;
using Backsight.Model.Operations;

namespace Backsight.Map.Editor.Windows;

public class PathWindow : WizardWindow
{
    internal PathWindow(ConnectionPathTool tool, PathOperation? recall) :
        base(new PathViewModel(tool, recall))
    {
        Title = "Connection Path";
        Pages.Add(new PickPointPage(Model.Start));
        Pages.Add(new PickPointPage(Model.End));
        Pages.Add(new PathEntryPage(Model.Entry));
    }
    
    internal PathViewModel Model => (PathViewModel)ViewModel;
}