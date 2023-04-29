global using OvenSensorReader.Log;
global using OvenSensorReader.Modbus;
global using OvenSensorReader.Settings;
global using OvenSensorReader.Test;

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
         
            if (TesterClass.TESTING) {
                TesterClass.Run();
                return;
            }
            ApplicationConfiguration.Initialize();
            Application.Run(new FormMain());
        }
    }

    
}