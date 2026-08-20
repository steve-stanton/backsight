using System;
using System.Threading.Tasks;

namespace Backsight.Map.Editor.Windows;

public class SplashViewModel : ViewModelBase
{
    public async Task InitializeAsync()
    {
        await Task.Delay(TimeSpan.FromSeconds(1));
    }
    
}