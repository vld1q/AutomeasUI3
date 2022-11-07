using System.ComponentModel;

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