using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using AutomeasUII.AutomeasToolkit.Basic.TitledList;
using AutomeasUII.DevConfig;

namespace AutomeasUII.View;

public partial class ConfigBarView : UserControl{
    public ConfigBarView(){
        InitializeComponent();
    }

    public Tuple<ObservableCollection<TitledList>, ObservableCollection<TitledList>> TypRuchu{ get; set; } =
        new(ConfigBar.Collumns["TypRuchuRight"], ConfigBar.Collumns["TypRuchuLeft"]);

    //public Tuple<ObservableCollection<TitledList>, ObservableCollection<TitledList>> Pomiary { get; set; } =
    //    new(ConfigBar.Collumns["PomiaryLeft"], ConfigBar.Collumns["PomiaryRight"]);
}