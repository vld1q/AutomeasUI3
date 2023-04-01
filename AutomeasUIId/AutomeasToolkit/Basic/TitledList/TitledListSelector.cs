using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AutomeasUII.AutomeasToolkit.Basic.TitledList;

public class TitledListSelector : DataTemplateSelector
{
    private readonly Dictionary<string, string> avalibleUiElements = new()
    {
        { "check", "TitledCheckboxListDataTemplate" },
        { "combo", "TitledComboListDataTemplate" }
    };

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        var element = container as FrameworkElement;
        var cs = item as TitledList;
        if (element != null && item != null && cs != null)
            return element.FindResource(avalibleUiElements[cs.Type]) as DataTemplate;

        return null;
    }
}