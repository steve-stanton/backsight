using Backsight.Map.Editor.Tools;
using Backsight.Model;
using Backsight.Model.Operations;

namespace Backsight.Map.Editor.Windows;

public class ParallelWindow : WizardWindow
{
    internal ParallelWindow(ParallelTool tool, LineFeature referenceLine, ParallelLineOperation? recall) :
        base(new ParallelViewModel(tool, referenceLine, recall))
    {
        Title = "Parallel Line";
        Pages.Add(new ParallelOffsetPage(Model.Offset));
        Pages.Add(new ParallelTerminalPage(Model.Terminal1));
        Pages.Add(new ParallelTerminalPage(Model.Terminal2));
    }
    
    internal ParallelViewModel Model => (ParallelViewModel)ViewModel; 
}