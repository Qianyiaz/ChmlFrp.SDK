using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using ChmlFrp.SDK.Galley.ViewModels;
using ChmlFrp.SDK.Galley.Views;
using ChmlFrp.SDK.Service;

namespace ChmlFrp.SDK.Galley;

public class App : Application
{
    public static readonly ChmlFrpClient ChmlFrpClient = new();

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override async void OnFrameworkInitializationCompleted()
    {
        foreach (var plugin in BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray())
            BindingPlugins.DataValidators.Remove(plugin);

        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktopLifetime) return;

        var loginWindow = new LoginWindow();

        desktopLifetime.MainWindow = loginWindow;
        desktopLifetime.Exit += (_, _) => ChmlFrpClient.Dispose();

        try
        {
            var autoLoginAsync = await ChmlFrpClient.AutoLoginAsync();
            if (!autoLoginAsync!.State) return;
            new MainWindow { DataContext = new MainWindowViewModel(autoLoginAsync.Data!) }.Show();
            loginWindow.Close();
        }
        catch
        {
            // ignored
        }
    }
}