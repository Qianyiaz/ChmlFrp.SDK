using System.Globalization;
using Avalonia.Data.Converters;

namespace ChmlFrp.SDK.Galley.Converters;

public class TunnelStateToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is true ? "Green" : "Gray";

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}