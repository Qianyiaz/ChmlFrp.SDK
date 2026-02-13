using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using ChmlFrp.SDK.Content;
using ChmlFrp.SDK.Galley.Views;
using ChmlFrp.SDK.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChmlFrp.SDK.Galley.ViewModels;

public partial class LoginWindowViewModel(WindowNotificationManager manager, Window window) : ObservableObject
{
    [ObservableProperty] private bool _isRememberMe = true;
    [ObservableProperty] private bool _isUseToken;
    [ObservableProperty] private string? _password;
    [ObservableProperty] private string? _username;
    [ObservableProperty] private string? _usertoken;

    [RelayCommand]
    private async Task Login()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            manager.Show(new Notification("登录失败。", "用户名密码不能为空。", NotificationType.Error));
            return;
        }

        DataResponse<UserData>? dataResponse;
        try
        {
            if (IsUseToken)
                dataResponse = await App.ChmlFrpClient.LoginByTokenAsync(Usertoken!, IsRememberMe);
            else
                dataResponse = await App.ChmlFrpClient.LoginAsync(Username, Password, IsRememberMe);
        }
        catch (Exception e)
        {
            manager.Show(new Notification("登录失败。", e.Message, NotificationType.Error));
            return;
        }
        
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