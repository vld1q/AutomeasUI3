using System.ComponentModel;

namespace AutomeasUII.AutomeasToolkit.Basic.TitledList;
/// <summary>
/// Base type for all TitledLists
/// </summary>
public abstract class TitledList : INotifyPropertyChanged
{
    public TitledList(string type, string name = "")
    {
        Type = type;
        Name = name;
    }
    /// <summary>
    /// Determines type of List into which list will be translated.
    /// <c>TitledListSelector</c> is responsible for that.
    /// </summary>
    public string Type { get; set; }
    /// <summary>
    /// Will be the TitledList title
    /// </summary>
    public string Name { get; set; }
    public abstract event PropertyChangedEventHandler? PropertyChanged;
}