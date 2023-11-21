using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace AutomeasUII.Core;
// TODO fix ambiguity
public abstract class SerialDevice{
    public readonly SerialPort Port;
    protected bool IsDeactivated;

    public SerialDevice(string port, int baudrate){
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

    /// <summary>
    ///     Make device not accept instructions.
    /// </summary>
    public void Deactivate(){
        IsDeactivated = true;
    }

    public static SerialDevice operator ++(SerialDevice self){
        if (!self.Port.IsOpen) self.Port.Open();
        return self;
    }

    public static SerialDevice operator --(SerialDevice self){
        if (self.Port.IsOpen) self.Port.Close();
        return self;
    }

    public void SendRequest(string msg){
        if (Port.IsOpen)
            Port.WriteLine(msg);
        else
            throw new NotSupportedException("Port is not open");
    }

    public string SendSafeRequest(string msg){
        if (Port.IsOpen)
            Port.WriteLine(msg);
        var response = Port.ReadLine();
        if (response != "") return response;
        throw new NotSupportedException("Port is not open");
    }

    public string GetResponse(){
        string result;
        try{
            result = Port.ReadLine();
        }
        catch (TimeoutException){
            result = "???";
        }

        return result;
    }
}

public class Mcu : SerialDevice{
    public Mcu(string port, int baudrate) : base(port, baudrate){
        Thread.Sleep(100);
        Port.Open();
    }
    

    

    public void Cycle(List<byte[]> exe, List<byte[]> exe2, int delay){
        if (IsDeactivated) return;

        Port.DiscardInBuffer();
        Port.DiscardOutBuffer();
        foreach (var instruction in exe){
            Port.Write(instruction, 0, 2);
            var v = Port.ReadLine();
            if (v != "y") throw new Exception();
        }

        Thread.Sleep(delay);
        foreach (var instruction in exe2){
            Port.Write(instruction, 0, 2);
            var v = Port.ReadLine();
            if (v != "y") throw new Exception();
        }
    }
    
    public void Cycle(List<byte[]> exe, List<byte[]> exe2, List<byte[]> exe3, int delay){
        if (IsDeactivated) return;

        Port.DiscardInBuffer();
        Port.DiscardOutBuffer();
        foreach (var instruction in exe){
            Port.Write(instruction, 0, 2);
            var v = Port.ReadLine();
            if (v != "y") throw new Exception();
        }

        Thread.Sleep(delay);
        foreach (var instruction in exe2){
            Port.Write(instruction, 0, 2);
            var v = Port.ReadLine();
            if (v != "y") throw new Exception();
        }
        
        Thread.Sleep(delay);
        foreach (var instruction in exe3){
            Port.Write(instruction, 0, 2);
            var v = Port.ReadLine();
            if (v != "y") throw new Exception();
        }
    }

    public void Cycle(List<byte[]> exe, int delay){
        if (IsDeactivated) return;

        Port.DiscardInBuffer();
        Port.DiscardOutBuffer();
        foreach (var instruction in exe){
            Port.Write(instruction, 0, 2);
            var v = Port.ReadLine();
            if (v != "y") throw new Exception();
            if (delay > 0) Thread.Sleep(delay);
        }
    }

    public static byte[] StringToByteArray(string hex){
        return Enumerable.Range(0, hex.Length)
            .Where(x => x % 2 == 0)
            .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
            .ToArray();
    }
}

public class Gauge : SerialDevice{
    private const string IntToCommand = "BRA3_5";

    public Gauge(string port) : base(port, 300){
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

    private string ParseMeasurement_MakeNumeric(string measurement, Program.MeasurementType mMode){
        var result = "";
        int prefix, postfix;
        prefix = postfix = 0;
        // TODO refactor, make better parsing
        switch (mMode){
            case Program.MeasurementType.BrMeasDb: // BR=-57.8dB   1.5
                prefix = "BR=-".Length;
                postfix = "dB   1.5\r".Length;


                break;
            case Program.MeasurementType.IlMeasDbm: // P=-47.13dBr  1.3
                prefix = "P=-".Length;
                postfix = "dBr  1.3\r".Length;

                break;
            case Program.MeasurementType.PowerMeasDbm: // P<-50.00dBm  1.3
                prefix = "P<-".Length;
                postfix = "dBm  1.3\r".Length;
                break;
            default:
                throw new NotSupportedException("Invalid string or unsupported measurement type");
        }

        if (measurement[prefix - 1] != '-') prefix--;

        result = measurement.Substring(prefix);
        result = result.Substring(0, result.Length - postfix);
        return result;
    }

    public string GetMeasurement(Program.MeasurementType mMode, int sleep){
        var result = SendSafeRequest("G");
        result = ParseMeasurement_MakeNumeric(result, mMode);
        Port.DiscardInBuffer();
        Port.Close();
        Thread.Sleep(sleep);
        Port.Open();
        return result;
    }

    public void SetMode(Program.MeasurementType mMode){
        var mode = (int)mMode;
        var measMode = IntToCommand[mode];
        SendSafeRequest(measMode.ToString());
    }

    public string GetMeasurement(Program.MeasurementType mMode) // use enum MeasurementType
    {
        var mode = (int)mMode;
        var result = "";
        const char request = 'G';
        var measMode = IntToCommand[mode];
        /*result += mMode == Program.MeasurementType.BrMeasDb ? "BR measurement:\t\t"
            : mMode == Program.MeasurementType.IlMeasDbm ? "IL measurement:\t\t"
            : "Power measurement:\t";*/
        {
            // set mode
            SendSafeRequest(measMode.ToString());
            Thread.Sleep(1000);
        }
        {
            // send request
            result = SendSafeRequest(request.ToString());
            result = ParseMeasurement_MakeNumeric(result, mMode);
            Port.DiscardInBuffer();
        }
        Console.WriteLine($"{result}");
        return result;
    }
}