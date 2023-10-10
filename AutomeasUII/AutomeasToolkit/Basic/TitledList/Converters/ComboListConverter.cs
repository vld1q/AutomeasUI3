using System;
using System.Globalization;
using System.Windows.Data;

namespace AutomeasUII.AutomeasToolkit.Basic.TitledList.Converters;

public class ComboListConverter : IValueConverter{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture){
        var element = value as Combobox;
        var list = value as TitledList;
        if (element != null && list != null){
            element.Name = list.Name;
            return element;
        }

        throw new NotSupportedException("Type conversion failed");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture){
        throw new NotImplementedException();
    }
}