using System.Windows;
using System.Windows.Controls;

namespace AutomeasUII.AutomeasToolkit.Basic.TitledList;

public partial class TitledPList : UserControl{
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string),
        typeof(TitledPList), new PropertyMetadata(default(string)));

    public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(nameof(Items),
        typeof(TitledList), typeof(TitledPList), new PropertyMetadata(default(TitledList)));

    public TitledPList(){
        InitializeComponent();
    }

    public string Title{
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public TitledList Items{
        get => (TitledList)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }
}