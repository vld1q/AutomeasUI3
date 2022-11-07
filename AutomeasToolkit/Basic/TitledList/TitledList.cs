using System.ComponentModel;

namespace AutomeasToolkit.Basic.TitledList;

public abstract class TitledList : INotifyPropertyChanged
{
    public TitledList(string type, string name = "")
    {
        Type = type;
        Name = name;
    }

    public string Type { get; set; }
    public string Name { get; set; }
    public abstract event PropertyChangedEventHandler? PropertyChanged;
}