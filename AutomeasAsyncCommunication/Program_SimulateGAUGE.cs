using System;
using System.Globalization;
using System.Threading;

namespace AutomeasAsyncCommunication
{
    public partial class Program
    {
        internal static void SimulateGauge(string port)
        {
            SerialDevice gauge = new Gauge(port);
            var rnd = new Random();
            var mode = "B";
            var unit = "";
            gauge++;
            while (true)
            {
                var msg = gauge.GetResponse();
                if ("BAR".Contains(msg))
                {
                    mode = msg;
                    unit = mode == "B" ? "Db" : "Dbm";
                }
                else if (msg == "G")
                {
                    Thread.Sleep(3000);
                    var value = $"{rnd.NextDouble().ToString(CultureInfo.InvariantCulture)} {unit}";
                    gauge.SendRequest(value);
                }
                else if (msg == "???")
                {
                }
                else if (msg == "exit")
                {
                    break;
                }
            }
        }
    }
}