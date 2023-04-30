global using OvenSensorReader.Log;
global using OvenSensorReader.Modbus;
global using OvenSensorReader.Settings;
global using OvenSensorReader.Test;
global using static OvenSensorReader.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace OvenSensorReader; 
internal class Global {
    // Global objects
    public static SettingsProvider glSettingsProvider = new SettingsProvider();
    public static ModbusReader glModbusReader = new ModbusReader();
    public static Logger glLogger = new Logger();


}
