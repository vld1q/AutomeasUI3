using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace AutomeasUII.AutomeasToolkit.Basic.TitledList;

public partial class TitledCheckboxList : UserControl{
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string),
        typeof(TitledCheckboxList), new PropertyMetadata(default(string)));

    public static readonly DependencyProperty CheckboxesProperty = DependencyProperty.Register(nameof(Checkboxes),
        typeof(ObservableCollection<Checkbox>), typeof(TitledCheckboxList),
        new PropertyMetadata(default(ObservableCollection<Checkbox>)));

    public TitledCheckboxList(){
        InitializeComponent();
    }

    public string Title{
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public ObservableCollection<Checkbox> Checkboxes{
        get => (ObservableCollection<Checkbox>)GetValue(CheckboxesProperty);
        set => SetValue(CheckboxesProperty, value);
    }
}