using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace AutomeasUII.AutomeasToolkit.Basic.ExpandableSection;

public partial class TitledListCollumn : UserControl{
    public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(nameof(Items),
        typeof(ObservableCollection<TitledList.TitledList>), typeof(TitledListCollumn),
        new PropertyMetadata(default(ObservableCollection<TitledList.TitledList>)));

    public TitledListCollumn(){
        InitializeComponent();
    }

    public ObservableCollection<TitledList.TitledList> Items{
        get => (ObservableCollection<TitledList.TitledList>)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }
}