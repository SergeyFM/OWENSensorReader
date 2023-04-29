using OvenSensorReader.Modbus;
using OvenSensorReader.Settings;
using System.Runtime.InteropServices;


namespace OvenSensorReader.Test
{
    /// <summary>
    /// Test class, here you can test functions separately.
    /// </summary>
    internal static class TesterClass
    {

        public const bool TESTING = false; // when true, launches only command line and function Run().

        public static void Run()
        {
            AllocConsole();

            var oven = new OvenSettings() {
                SlaveID = 100,
                Name = "МВ110-220.8АС",
                StartAdress = 263,
                NumberOfPoints = 16,
                RegisterOffsets = new List<ushort>() { 1, 3, 5, 7, 9, 11, 13, 15, 263 }
            };

 

            Console.ReadKey();

        }



        // To open a console ---------------------------
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
        // ---------------------------------------------

    }
}
