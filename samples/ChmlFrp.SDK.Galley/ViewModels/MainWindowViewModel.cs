using ChmlFrp.SDK.Models;

namespace ChmlFrp.SDK.Galley.ViewModels;

public class MainWindowViewModel(UserData user)
{
    public string Username { get; set; } = user.Username!;
}