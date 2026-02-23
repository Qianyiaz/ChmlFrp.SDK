using System.Globalization;
using Avalonia.Data.Converters;

namespace ChmlFrp.SDK.Galley.Converters;

public class NodeStateToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is true ? "Green" : "Red";

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}