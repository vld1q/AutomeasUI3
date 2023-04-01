using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AutomeasUII.AutomeasToolkit.Basic.TitledList;
using AutomeasUII.Core;
using AutomeasUII.DevConfig;
using AutomeasUII.ViewModel;

namespace AutomeasUII;
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
    
}