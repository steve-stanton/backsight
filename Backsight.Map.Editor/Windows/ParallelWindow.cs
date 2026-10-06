using Backsight.Map.Editor.Tools;
using Backsight.Model;

namespace Backsight.Map.Editor.Windows;

public class ParallelWindow : WizardWindow
{
    internal ParallelWindow(ParallelTool tool, LineFeature referenceLine) :
        base(new ParallelViewModel(tool, referenceLine))
    {
        Title = "Parallel Line";
        Pages.Add(new ParallelOffsetPage(Model.Offset));
        Pages.Add(new ParallelTerminalPage(Model.Terminal1));
        Pages.Add(new ParallelTerminalPage(Model.Terminal2));
    }
    
    internal ParallelViewModel Model => (ParallelViewModel)ViewModel; 
}