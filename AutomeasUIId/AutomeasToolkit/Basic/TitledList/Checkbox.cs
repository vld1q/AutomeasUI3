using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AutomeasUII.AutomeasToolkit.Basic.TitledList;

public class CheckboxCollection : TitledList
{
    public CheckboxCollection(ObservableCollection<Checkbox> items, string name = "") : base("check", name)
    {
        Content = items;
    }

    public ObservableCollection<Checkbox> Content { get; set; }
    public override event PropertyChangedEventHandler? PropertyChanged;
}

public class Checkbox : INotifyPropertyChanged
{
    private bool _checked;
    private string _name;

    public Checkbox(string name, bool @checked = false)
    {
        _name = name;
        _checked = @checked;
    }

    //
    public bool Checked
    {
        get => _checked;
        set
        {
            _checked = value;
            OnPropertyChanged();
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

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

    public static class Generator
    {
        public static CheckboxCollection GetList(string name, string[] content)
        {
            ObservableCollection<Checkbox> result = new();
            foreach (var element in content) result.Add(new Checkbox(element));

            return new CheckboxCollection(result, name);
        }
    }
}