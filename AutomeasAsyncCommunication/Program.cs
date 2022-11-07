using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace AutomeasAsyncCommunication
{
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

        public static void Main(string[] args)
        {
            SerialDevice mcu = new Mcu("COM14", 9600);
            SerialDevice gauge = new Gauge("COM12");
            // simulations
            Task.Run(() => { SimulateMcu("COM13", 9600); });
            Task.Run(() => { SimulateGauge("COM11"); });
            // open ports
            mcu++;
            gauge++;
            while (true)
            {
                Console.Write("AUTOMEAS>\t");
                var msg = "cycle"; //Console.ReadLine();
                if (msg == "cycle")
                {
                    Console.Clear();
                    ((Mcu)mcu).Cycle();
                    ((Gauge)gauge).GetMeasurement(MeasurementType.BrMeasDb);
                    ((Gauge)gauge).GetMeasurement(MeasurementType.IlMeasDbm);
                    ((Gauge)gauge).GetMeasurement(MeasurementType.PowerMeasDbm);
                    Thread.Sleep(3000);
                }
                else if (msg == "exit")
                {
                    break;
                }
            }

            Console.WriteLine("Shutting down McuSimulator");
            mcu.SendRequest("exit");
            Console.WriteLine("Shutting down GaugeSimulator");
            gauge.SendRequest("exit");
            mcu--;
            gauge--;
            Console.WriteLine("Exit successful");
        }

        public static void RealMain()
        {
            var measBuffer = new List<string>();
            var msg = "";
            var gaugePort = "COM5";
            var gauge = new Gauge(gaugePort);
            var mcu = new Mcu("COM10", 9600);
            while (msg != "exit")
            {
                Console.Write("automeas>  ");
                msg = Console.ReadLine();
                if (msg == "")
                {
                    for (var i = 0; i < 100; i++)
                    {
                        //Console.Clear();
                        gauge.Port.DiscardInBuffer();
                        mcu.Cycle();
                        gauge.SendRequest("3");
                        //measBuffer.Add(gauge.GetMeasurement(MeasurementType.BrMeasDb));
                        measBuffer.Add(gauge.GetMeasurement(MeasurementType.IlMeasDbm));
                        gauge.SendRequest("5");
                        //measBuffer.Add(gauge.GetMeasurement(MeasurementType.BrMeasDb));
                        measBuffer.Add(gauge.GetMeasurement(MeasurementType.IlMeasDbm));
                        //measBuffer.Add(gauge.GetMeasurement(MeasurementType.PowerMeasDbm));
                    }

                    Console.WriteLine(measBuffer);
                }
            }
        }

        public static ObservableCollection<double?> MakeMeasurement(Mcu mcu, int numberOfMeasurements = 1)
        {
            var result = new ObservableCollection<double?>();
            for (var i = 0; i < numberOfMeasurements; i++) result.Add(null);

            for (var i = 0; i < numberOfMeasurements; i++) result[i] = SingularMeasurement(mcu);
            return result;
        }

        private static double? SingularMeasurement(Mcu mcu)
        {
            double? result = null;
            mcu.Cycle();
            var res = "D-= 1.314 Dbm"; //gauge.GetMeasurement(MeasurementType.IlMeasDbm);
            {
                // obtain value
                var array = res.Split(' ');
                if (array.Length != 3)
                {
                    throw new Exception("Got invalid result");
                }

                res = array[1];
                result = Convert.ToDouble(res, CultureInfo.InvariantCulture);
            }
            return result;
        }
    }
}