using System;
using System.Globalization;
using System.Windows.Data;

namespace AutomeasToolkit.Basic.TitledList.Converters;

public class CheckboxListConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var element = value as CheckboxCollection;
        var list = value as TitledList;
        if (element != null && list != null)
        {
            element.Name = list.Name;
            return element;
        }

        throw new NotSupportedException("Type conversion failed");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}