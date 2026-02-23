using ChmlFrp.SDK.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChmlFrp.SDK.Galley.ViewModels;

public partial class MainWindowViewModel(UserData user) : ObservableObject
{
    [ObservableProperty] private UserData _user = user;

    [RelayCommand]
    private async Task Refresh()
    {
        try
        {
            var result = await App.ChmlFrpClient.RefreshAsync();
            if (result!.State)
                User = result.Data!;
        }
        catch
        {
            // ignored
        }
    }
}