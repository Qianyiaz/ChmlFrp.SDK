using System.Collections;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ChmlFrp.SDK.Galley.Converters;

public class CollectionToBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not IEnumerable collection) return false;
        return !collection.Cast<object>().Any();
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}