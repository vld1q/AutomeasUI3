using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AutomeasUI.Core;

public class ObservableType<T> : INotifyPropertyChanged
{
    public ObservableType(T? val)
    {
        Value = val;
    }
    // Interface implementation
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string propertyName)
    {
        var handler = PropertyChanged;
        if (handler != null)
            handler(this, new(propertyName));
    }
    // Values
    private T? _value;
    public T? Value
    {
        get { return _value; }
        set
        {
            _value = value;
            OnPropertyChanged("Value");
        }
    }
}
public class ObservableBool : INotifyPropertyChanged
{
    public ObservableBool(bool val)
    {
        Value = val;
    }
    // Interface implementation
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string propertyName)
    {
        var handler = PropertyChanged;
        if (handler != null)
            handler(this, new(propertyName));
    }
    // Values
    private bool? _value;

    public bool? NotValue
    {
        get { return !_value ?? null; }
    }
    public bool? Value
    {
        get { return _value; }
        set
        {
            _value = value;
            OnPropertyChanged("Value");
            OnPropertyChanged("NotValue");
        }
    }
}