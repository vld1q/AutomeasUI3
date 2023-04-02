using System.ComponentModel;

namespace AutomeasUII.ViewModel;

public partial class ConfigBarViewm{
}

public partial class ConfigBarViewm : INotifyPropertyChanged{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName){
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}