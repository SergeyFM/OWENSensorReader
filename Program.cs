using OvenSensorReader.Log;
using OvenSensorReader.Modbus;
using OvenSensorReader.Settings;
using OvenSensorReader.Test;

namespace OvenSensorReader
{



    internal class Program {
        // Global objects
        SettingsProvider settingsProvider = new SettingsProvider();
        ModbusReader modbusReader = new ModbusReader();
        Logger logger = new Logger();

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            LoadObjects();
            if (TesterClass.TESTING) {
                TesterClass.Run();
                return;
            }
            ApplicationConfiguration.Initialize();
            Application.Run(new FormMain());
        }


        static void LoadObjects() {
            
        }
    }

    
}