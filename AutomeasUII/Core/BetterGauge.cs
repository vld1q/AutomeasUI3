using System;
using System.IO.Ports;
using System.Threading;

namespace AutomeasUII.Core;
public class BetterGauge: SerialDevice
{
    public BetterGauge(string port, int baudrate) : base(port, baudrate)
    {
        Port.RtsEnable = true;
        Port.DtrEnable = true;
        Thread.Sleep(100);
        //Port.DataReceived += HandlePortDataReceived;
        Port.ReadTimeout = 500;
        Port.WriteTimeout = 500;
        Port.ErrorReceived += HandlePortErrorReceived;
        Port.Open();
        
    }

    private Func<string> Meas { get; set; }
    private const string IntToCommand = "BRA3_5";
    public string GetMeasurement(Program.MeasurementType mMode) // use enum MeasurementType
    {
        this.Meas = () => GetMeasurement(mMode);
        var mode = (int)mMode;
        var result = "";
        const char request = 'G';
        var measMode = IntToCommand[mode];
        result += mMode == Program.MeasurementType.BrMeasDb ? "BR measurement:\t\t"
            : mMode == Program.MeasurementType.IlMeasDbm ? "IL measurement:\t\t"
            : "Power measurement:\t";
        {
            // set mode
            SendRequest(measMode.ToString());
            Thread.Sleep(1000);
        }
        {
            // send request
            SendRequest(request.ToString());
            Port.DiscardInBuffer();
            Thread.Sleep(1000);
            var response = "";
            //while (response == "" || response == "???") response = GetResponse();
            result = Port.ReadLine();
            //result += response;
        }
        Console.WriteLine($"{result}");
        return result;
    }
    private void HandlePortDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        
    }

    private void HandlePortErrorReceived(object sender, SerialErrorReceivedEventArgs e)
    {
        if (e.EventType == SerialError.RXOver)
        { // Read Timeout
            this.Port.DiscardInBuffer();
            Thread.Sleep(500);
            this.Port.DiscardOutBuffer();
            Thread.Sleep(500);
            this.Meas();
            
        }
        else
        {
            Console.Error.WriteLine($"SerialDataError:\n\t{e.ToString()}");
        }
    }
    
    
}