using Avalonia.Controls.Notifications;
using ChmlFrp.SDK.Galley.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChmlFrp.SDK.Galley.ViewModels;

public partial class LoginWindowViewModel(WindowNotificationManager manager,LoginWindow window) : ObservableObject
{
    [ObservableProperty] private string? _username;
    [ObservableProperty] private string? _password;

    [RelayCommand]
    private async Task Login()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            manager.Show(new Notification("登录失败。", "用户名密码不能为空。", NotificationType.Error));
            return;
        }

        var dataResponse = await App.ChmlFrpClient.LoginAsync(Username,Password);

        if (!dataResponse!.State)
        {
            manager.Show(new Notification("登录失败。", dataResponse.Message, NotificationType.Error));
        }
        else
        {
            manager.Show(new Notification("登录成功。", $"Hi,{dataResponse.Data!.Username} 欢迎回来。", NotificationType.Success)
            {
                OnClose = () =>
                {
                    new MainWindow { DataContext = new MainWindowViewModel(dataResponse.Data) }.Show();
                    window.Close();
                }
            });
        }
    }
}