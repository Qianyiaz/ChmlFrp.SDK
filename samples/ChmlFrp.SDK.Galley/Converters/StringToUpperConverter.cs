using System.Globalization;
using Avalonia.Data.Converters;

namespace ChmlFrp.SDK.Galley.Converters;

public class StringToUpperConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value?.ToString()?.ToUpperInvariant();

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}