using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using AutomeasToolkit.Basic.TitledList;
using AutomeasUI.DevConfig;

namespace AutomeasUI.View;

public partial class ConfigBarView : UserControl
{
    public ConfigBarView()
    {
        InitializeComponent();
    }

    public Tuple<ObservableCollection<TitledList>, ObservableCollection<TitledList>> TypRuchu { get; set; } =
        new(ConfigBar.Collumns["TypRuchuRight"], ConfigBar.Collumns["TypRuchuLeft"]);

    //public Tuple<ObservableCollection<TitledList>, ObservableCollection<TitledList>> Pomiary { get; set; } =
    //    new(ConfigBar.Collumns["PomiaryLeft"], ConfigBar.Collumns["PomiaryRight"]);
}