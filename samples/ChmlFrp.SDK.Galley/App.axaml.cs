using Avalonia;
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
        var loginWindow = new LoginWindow();
        loginWindow.Show();

        var autoLoginAsync = await ChmlFrpClient.AutoLoginAsync();
        if (!autoLoginAsync!.State) return;
        new MainWindow { DataContext = new MainWindowViewModel(autoLoginAsync.Data!) }.Show();
        loginWindow.Close();
    }
}