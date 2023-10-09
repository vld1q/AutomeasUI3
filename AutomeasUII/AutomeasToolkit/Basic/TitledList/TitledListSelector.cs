using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AutomeasUII.AutomeasToolkit.Basic.TitledList;

public class TitledListSelector : DataTemplateSelector{
    private readonly Dictionary<string, string> _availableUiElements = new(){
        { "check", "TitledCheckboxListDataTemplate" },
        { "combo", "TitledComboListDataTemplate" }
    };

    public override DataTemplate? SelectTemplate(object item, DependencyObject container){
        if (container is FrameworkElement element && item is TitledList cs)
            return element.FindResource(_availableUiElements[cs.Type]) as DataTemplate;

        return null;
    }
}