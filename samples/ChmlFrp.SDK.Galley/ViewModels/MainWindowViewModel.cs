using ChmlFrp.SDK.Models;

namespace ChmlFrp.SDK.Galley.ViewModels;

public class MainWindowViewModel(UserData user)
{
    public string Username => user.Username!;
    public string ImageUrl => user.AvatarUrl!;
}