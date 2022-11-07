using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace AutomeasAsyncCommunication
{
    public abstract class SerialDevice
    {
        public readonly SerialPort Port;

        public SerialDevice(string port, int baudrate)
        {
            Port = new SerialPort();
            {
                // _port settings
                Port.PortName = port;
                Port.BaudRate = baudrate;
                Port.Parity = Parity.None;
                Port.DataBits = 8;
                Port.StopBits = StopBits.Two;
                //Port.Handshake = Handshake.None;
                //Port.ReadTimeout = 500;
                //_port.WriteTimeout = 500;
            }
        }

        public static SerialDevice operator ++(SerialDevice self)
        {
            if (!self.Port.IsOpen) self.Port.Open();
            return self;
        }

        public static SerialDevice operator --(SerialDevice self)
        {
            if (self.Port.IsOpen) self.Port.Close();
            return self;
        }

        public void SendRequest(string msg)
        {
            if (Port.IsOpen)
                Port.WriteLine(msg);
            else
                throw new NotSupportedException("Port is not open");
        }

        public string GetResponse()
        {
            string result;
            try
            {
                result = Port.ReadLine();
            }
            catch (TimeoutException)
            {
                result = "???";
            }

            return result;
        }
    }

    public class Mcu : SerialDevice
    {
        public Mcu(string port, int baudrate) : base(port, baudrate)
        {
            Thread.Sleep(100);
            Port.Open();
        }

        public void Cycle()
        {
            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();
            var response = "";
            //Thread.Sleep(3000);
            string left, right;
            left = "";
            right = "";
            var b = StringToByteArray("021E");
            Port.Write(b, 0, 2);
            var v = Port.ReadLine();
            if (v != "y") throw new NotImplementedException();

            b = StringToByteArray("031E");
            Port.Write(b, 0, 2);
            v = Port.ReadLine();
            if (v != "y") throw new NotImplementedException();
            b = StringToByteArray("0001");
            Port.Write(b, 0, 2);
            v = Port.ReadLine();
            if (v != "y") throw new NotImplementedException();
            /*Port.WriteLine("l065x\0");
            while(response != "done") 
                response = Port.ReadLine();
            Thread.Sleep(1500);
            if (response == "done") Console.WriteLine("MOVE LEFT SUCCESSFUL");
            SendRequest("r075x");
            response = GetResponse();
            Thread.Sleep(1500);
            if (response == "done") Console.WriteLine("MOVE RIGHT SUCCESSFUL");*/
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }
    }

    internal class Gauge : SerialDevice
    {
        private const string IntToCommand = "BRA3_5";

        public Gauge(string port) : base(port, 300)
        {
            /*_port.PortName = port;
            _port.BaudRate = baudrate;
            _port.Parity = Parity.None;
            _port.DataBits = 8;
            _port.StopBits = StopBits.Two;
            _port.Handshake = Handshake.None;
            _port.ReadTimeout = 500;*/
            //_port.WriteTimeout = 500;
            Port.RtsEnable = true;
            Port.DtrEnable = true;
            //Port.ReadTimeout = 1000;
            Thread.Sleep(100);
            Port.Open();
        }

        public string GetMeasurement(Program.MeasurementType mMode) // use enum MeasurementType
        {
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
    }
}