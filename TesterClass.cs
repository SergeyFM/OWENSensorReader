using OvenSensorReader;
using System.Runtime.InteropServices;


namespace PassportGenerator.test {
    /// <summary>
    /// Test class, here you can test functions separately.
    /// </summary>
    internal static class TesterClass {
        
        public const bool TESTING = false; // when true, launches only command line and function Run().

        public static void Run() {
            AllocConsole();

           
            ModbusReader.OpenPort("COM9");
            Console.WriteLine($"{ModbusReader._PORT.PortName} port is open: {ModbusReader._PORT.IsOpen}");

            Console.WriteLine("master: " + ModbusReader._MASTER);
            ModbusReader.CreateRtuMaster(ModbusReader._PORT);
            Console.WriteLine("master: " + ModbusReader._MASTER);

            ushort[] data = ModbusReader.ReadHoldingRegisters(182, 13, 1);
            Console.WriteLine($"data: {data?.First()}");


            ModbusReader.ClosePort();
            Console.WriteLine("COM port is open: " + ModbusReader._PORT.IsOpen);

            Console.ReadKey();

        }

      

        // To open a console ---------------------------
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
        // ---------------------------------------------

    }
}
