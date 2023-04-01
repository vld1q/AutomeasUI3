using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;


namespace  AutomeasUII.Core;
public partial class Program
{
    public enum MeasurementType
    {
        BrMeasDb = 0,
        PowerMeasDbm = 2,
        IlMeasDbm = 1,
        Nm1310 = 3,
        Nm1550 = 5
    }
    public static ObservableCollection<double?> MakeMeasurement(Mcu mcu, int numberOfMeasurements = 1)
    {
        var result = new ObservableCollection<double?>();
        for (var i = 0; i < numberOfMeasurements; i++) result.Add(null);

        //for (var i = 0; i < numberOfMeasurements; i++) result[i] = SingularMeasurement(mcu);
        return result;
    }
}

        