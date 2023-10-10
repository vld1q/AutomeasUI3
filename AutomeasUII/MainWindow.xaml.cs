using System;
using System.Collections.ObjectModel;
using System.Windows;
using AutomeasUII.AutomeasToolkit.Basic.TitledList;
using AutomeasUII.DevConfig;
using AutomeasUII.ViewModel;

namespace AutomeasUII;

public partial class MainWindow : Window{
    public MainWindow(){
        InitializeComponent();
    }

    public ObservableCollection<TitledList> Test{ get; set; } = ConfigBar.Collumns["TypRuchuLeft"];

    public Tuple<ObservableCollection<TitledList>, ObservableCollection<TitledList>> TypRuchu{ get; set; } = new
        (ConfigBar.Collumns["TypRuchuLeft"], ConfigBar.Collumns["TypRuchuRight"]);

    public DashboardViewModel DashboardViewModel{ get; set; } = new();
}