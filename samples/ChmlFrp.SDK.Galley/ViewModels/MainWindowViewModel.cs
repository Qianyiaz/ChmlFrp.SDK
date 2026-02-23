using Avalonia.Collections;
using ChmlFrp.SDK.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChmlFrp.SDK.Galley.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] private UserData _user;

    [RelayCommand]
    private async Task Refresh()
    {
        try
        {
            var result = await App.ChmlFrpClient.RefreshAsync();
            if (result!.State)
                User = result.Data!;
            await Task.WhenAll(LoadTunnelsAsync(), LoadNodesAsync());
        }
        catch
        {
            // ignored
        }
    }

    public AvaloniaList<TunnelData> Tunnels { get; } = [];

    [RelayCommand]
    private async Task LoadTunnelsAsync()
    {
        try
        {
            var result = await App.ChmlFrpClient.GetTunnelResponseAsync();
            Tunnels.Clear();
            if (result?.State == true)
                Tunnels.AddRange(result.Data!);
        }
        catch
        {
            // ignored
        }
    }
    
    public AvaloniaList<NodeData> Nodes { get; } = [];
    
    [RelayCommand]
    private async Task LoadNodesAsync()
    {
        try
        {
            var result = await App.ChmlFrpClient.GetNodeResponseAsync();
            Nodes.Clear();
            if (result?.State == true)
                Nodes.AddRange(result.Data!);
        }
        catch
        {
            // ignored
        }
    }

    public MainWindowViewModel(UserData user)
    {
        _user = user;
        _ = Task.WhenAll(LoadTunnelsAsync(), LoadNodesAsync());
    }
}