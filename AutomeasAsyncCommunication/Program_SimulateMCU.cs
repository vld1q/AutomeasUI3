namespace AutomeasAsyncCommunication
{
    public partial class Program
    {
        internal static void SimulateMcu(string port, int baudrate)
        {
            SerialDevice Mcu = new Mcu(port, baudrate);
            Mcu++;
            while (true)
            {
                var msg = Mcu.GetResponse();
                if (msg == "f000x")
                {
                    //await Mcu.SendRequest("f000x");
                }
                else if (msg == "r075x" || msg == "l075x")
                {
                    Mcu.SendRequest("done");
                }
                else if (msg == "exit")
                {
                    break;
                }
                else if (msg == "???")
                {
                }
                else
                {
                    Mcu.SendRequest("???");
                }
            }
        }
    }
}