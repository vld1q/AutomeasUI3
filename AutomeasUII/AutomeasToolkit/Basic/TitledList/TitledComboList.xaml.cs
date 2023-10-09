using System.Windows;
using System.Windows.Controls;

namespace AutomeasUII.AutomeasToolkit.Basic.TitledList;

public partial class TitledComboList : UserControl{
    public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(nameof(Items),
        typeof(Combobox), typeof(TitledComboList), new PropertyMetadata(default(Combobox)));

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string),
        typeof(TitledComboList), new PropertyMetadata(default(string)));

    public TitledComboList(){
        InitializeComponent();
    }

    public Combobox Items{
        get => (Combobox)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public string Title{
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
}