using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using AutomeasUII.AutomeasToolkit.Basic.TitledList;
using AutomeasUII.Core;

namespace AutomeasUII.DevConfig;
public static class ConfigBar
{
    public static Dictionary<string, ObservableCollection<TitledList>> Collumns { get; set; } = new()
    {
        {
            "TypRuchuLeft", new ObservableCollection<TitledList>
            {
                Combobox.Generator.GetList("Typ pomiaru", new[] { "IL", "BR", "Power"}),
                Combobox.Generator.GetList("Ilość wykonań", new[] { "5","50", "500", "1000", "2000", "3000",  "5000"}),
                
            }
        },
        {
            "TypRuchuRight", new ObservableCollection<TitledList>
            {
                Combobox.Generator.GetList("Preset", new[] { "full", "half", "half_b", /*"1/4", "1/8", "1/16", "1/32"*/ }),
                Combobox.Generator.GetList("Silnik COM", SerialPort.GetPortNames()),
                Combobox.Generator.GetList("Miernik COM", SerialPort.GetPortNames())
            }
        },
        {
            "PomiaryLeft", new ObservableCollection<TitledList>
            {
                Combobox.Generator.GetList("Rodzaje pomiarów", new[] { "IL", "BR", "Power"}),
                Combobox.Generator.GetList("Ilość prób/cykl", new[] { "1", "2", "3", "5" })
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