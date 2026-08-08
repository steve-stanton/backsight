using System;
using System.Threading.Tasks;

namespace Backsight.Map.Editor.ViewModels;

public class SplashViewModel : ViewModelBase
{
    public async Task InitializeAsync()
    {
        await Task.Delay(TimeSpan.FromSeconds(2));
    }
    
}