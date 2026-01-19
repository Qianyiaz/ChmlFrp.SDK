using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChmlFrp.SDK.Galley.ViewModels;

public partial class MainWindowViewModel(WindowNotificationManager manager) : ObservableObject
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

        manager.Show(dataResponse!.State
            ? new Notification("登录成功。", $"Hi,{dataResponse.Data!.Username} 欢迎回来。", NotificationType.Success)
            : new Notification("登录失败。", dataResponse.Message, NotificationType.Error));
    }
}