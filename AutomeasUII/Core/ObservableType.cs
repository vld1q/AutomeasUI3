using System.ComponentModel;
using System.Windows;

namespace AutomeasUII.Core;

public static class ExceptionWindow{
    public static void DisplayErrorBox(string title, string context){
        var messageBoxText = context;
        var caption = title;
        var button = MessageBoxButton.OK;
        var icon = MessageBoxImage.Error;
        MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
    }
}

public class ObservableType<T> : INotifyPropertyChanged{
    // Values
    private T? _value;

    public ObservableType(T? val){
        Value = val;
    }

    public T? Value{
        get => _value;
        set{
            _value = value;
            OnPropertyChanged("Value");
        }
    }

    // Interface implementation
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName){
        var handler = PropertyChanged;
        if (handler != null)
            handler(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class ObservableBool : INotifyPropertyChanged{
    // Values
    private bool? _value;

    public ObservableBool(bool val){
        Value = val;
    }

    public bool? NotValue => !_value ?? null;

    public bool? Value{
        get => _value;
        set{
            _value = value;
            OnPropertyChanged("Value");
            OnPropertyChanged("NotValue");
        }
    }

    // Interface implementation
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName){
        var handler = PropertyChanged;
        if (handler != null)
            handler(this, new PropertyChangedEventArgs(propertyName));
    }
}