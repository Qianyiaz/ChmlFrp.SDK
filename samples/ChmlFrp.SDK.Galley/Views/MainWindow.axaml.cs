using Avalonia.Controls;
using ChmlFrp.SDK.Models;
using DialogHostAvalonia;

namespace ChmlFrp.SDK.Galley.Views;

public partial class MainWindow : Window
{
    public MainWindow() => InitializeComponent();

    private void OnTunnelsSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not ListBox listBox) return;
        if (listBox.SelectedItem is not TunnelData tunnelData) return;
        
        listBox.SelectedItem = null;
        DialogHost.Show(tunnelData);
    }

    private async void OnNodelsSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not ListBox listBox) return;
        if (listBox.SelectedItem is not NodeData nodeData) return;

        listBox.SelectedItem = null;
        try
        {
            var nodeInfoResponse = await App.ChmlFrpClient.GetNodeInfoResponseAsync(nodeData);

            if (nodeInfoResponse?.State == true)
                await DialogHost.Show(nodeInfoResponse.Data);
        }
        catch
        {
            // ignored
        }
    }
}