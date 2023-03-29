using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using AutomeasToolkit.Basic.TitledList;
using AutomeasUI.Core;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutomeasUI.DevConfig;
public static class ConfigBar
{
    public static Dictionary<string, ObservableCollection<TitledList>> Collumns { get; set; } = new()
    {
        {
            "TypRuchuLeft", new ObservableCollection<TitledList>
            {
                Combobox.Generator.GetList("Preset", new[] { "fajny cykl", "cykl", "półcykl", "ćwierćcykl" }),
                Combobox.Generator.GetList("Ilość wykonań", new[] { "1", "5", "10", "50", "100", "200", "500" })
            }
        },
        {
            "TypRuchuRight", new ObservableCollection<TitledList>
            {
                Combobox.Generator.GetList("Rodzaj kroku", new[] { "full", "half", "half_b", "1/4", "1/8", "1/16", "1/32" }),
                Combobox.Generator.GetList("Mcu COM", SerialPort.GetPortNames()),
                Combobox.Generator.GetList("Gauge COM", SerialPort.GetPortNames())
            }
        },
        {
            "PomiaryLeft", new ObservableCollection<TitledList>
            {
                Checkbox.Generator.GetList("Rodzaje pomiarów", new[] { "IB", "BR", "Power" }),
                Combobox.Generator.GetList("Ilość prób/cykl", new[] { "1", "2", "3", "5" }),
                Checkbox.Generator.GetList("", new[] { "Wylicz średnią" })
            }
        },
        {
            "PomiaryRight", new ObservableCollection<TitledList>
            {
                Checkbox.Generator.GetList("Opcje dodatkowe", new[]
                {
                    "Odchylenie standardowe", "Uwzględnij...", "Placeholder1", "Placeholder2", "Placeholder3"
                })
            }
        },
    };
}