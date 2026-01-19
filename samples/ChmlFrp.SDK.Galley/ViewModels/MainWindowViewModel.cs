using ChmlFrp.SDK.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ChmlFrp.SDK.Galley.ViewModels;

public partial class MainWindowViewModel(UserData user) : ObservableObject
{
    [ObservableProperty] private string _username = user.Username!;
}