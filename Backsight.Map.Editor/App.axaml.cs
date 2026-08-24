using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Backsight.Database;
using Backsight.Map.Editor.Mapping;
using Backsight.Map.Editor.Models;
using Backsight.Map.Editor.Windows;
using Backsight.Model;
using Microsoft.Extensions.DependencyInjection;
using RepoDb;

namespace Backsight.Map.Editor;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    // ReSharper disable once AsyncVoidMethod
    public override async void OnFrameworkInitializationCompleted()
    {
        // Use SQLite for the environment database
        GlobalConfiguration.Setup().UseSqlite();

        var collection = new ServiceCollection();
        collection.AddSingleton<IEnvironmentRepository, EnvironmentRepository>();
        collection.AddSingleton<IMapEditorModel, MapEditorModel>();
        collection.AddSingleton<IMapEditorViewModel, MapEditorViewModel>();
        collection.AddSingleton<IMapRepository, MapsDirectory>();
        collection.AddSingleton<IMapControlRenderer, Renderer>();
        collection.AddSingleton<MapEditorWindow>();
        collection.AddSingleton<IDialogService, DialogService>();  
        
        var services = collection.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Set up for map display (requiring it here ensures that an instance actually gets created)
            services.GetRequiredService<Mapping.IMapControlRenderer>();
            
            // For more on splash screens, see https://github.com/AvaloniaUI/Avalonia/discussions/11083
            var splashVm = new SplashViewModel();
            var splash = new SplashWindow { DataContext = splashVm };
            desktop.MainWindow = splash;
            splash.Show();
            await splashVm.InitializeAsync();

            var editorWindow = services.GetRequiredService<MapEditorWindow>();
            editorWindow.DataContext = services.GetRequiredService<IMapEditorViewModel>();
            desktop.MainWindow = editorWindow;
            editorWindow.Show();
            splash.Close();
        }

        base.OnFrameworkInitializationCompleted();
    }
}