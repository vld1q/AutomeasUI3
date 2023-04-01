using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using AutomeasAsyncCommunication;
using AutomeasToolkit.Basic.TitledList;
using AutomeasUI.DevConfig;
using AutomeasUI.ViewModel;

namespace AutomeasUI;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public ObservableCollection<TitledList> Test { get; set; } = ConfigBar.Collumns["TypRuchuLeft"];

    public Tuple<ObservableCollection<TitledList>, ObservableCollection<TitledList>> TypRuchu { get; set; } = new
        (ConfigBar.Collumns["TypRuchuLeft"], ConfigBar.Collumns["TypRuchuRight"]);

    public DashboardViewModel DashboardViewModel { get; set; }= new();
    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        Task.Run(() =>
        {
            Mcu mcu = new("COM10", 9600);
            var res = Program.MakeMeasurement(mcu);
            mcu.Port.Close();
        });
    }
}