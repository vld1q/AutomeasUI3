using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AutomeasUII.AutomeasToolkit.Basic.TitledList;

public partial class Combobox : TitledList
{
    private ObservableCollection<string> _content;
    private int _index;

    public string GetValue()
    {
        return Content[Index];
    }

    
    public Combobox(ObservableCollection<string> content, int index = 0, string name = "") : base("combo", name)
    {
        _content = content;
        _index = index;
    }

    public Combobox(List<string> content, int index = 0) : base("combo")
    {
        ObservableCollection<string> result = new();
        foreach (var element in content) result.Add(element);

        _content = result;
        _index = index;
    }

    //
    public int Index
    {
        get => _index;
        set
        {
            _index = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<string> Content
    {
        get => _content;
        set
        {
            _content = value;
            OnPropertyChanged();
        }
    }

    public static class Generator
    {
        public static Combobox GetList(string name, string[] content)
        {
            Combobox result = new(content.ToList());
            result.Name = name;
            return result;
        }
    }
}

public partial class Combobox : INotifyPropertyChanged
{
    public override event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}