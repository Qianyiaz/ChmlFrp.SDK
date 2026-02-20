using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Primitives;
using ChmlFrp.SDK.Galley.ViewModels;

namespace ChmlFrp.SDK.Galley.Views;

public partial class LoginWindow : Window
{
    private WindowNotificationManager _manager = null!;

    public LoginWindow() => InitializeComponent();

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _manager = new WindowNotificationManager(this) { MaxItems = 3, Position = NotificationPosition.BottomCenter };
        DataContext = new LoginWindowViewModel(_manager, this);
    }
}