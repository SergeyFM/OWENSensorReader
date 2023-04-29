using System.IO.Ports;
using NModbus;
using NModbus.Serial;
using OvenSensorReader.Log;

namespace OvenSensorReader.Modbus
{
    /// <summary>
    /// Static class with modbus functions
    /// </summary>
    public static class ModbusReader
    {


        public static SerialPort _PORT = null;
        public static IModbusMaster _MASTER = null;
        public static int TIMEOUT = 80;

        /// <summary>
        /// Test function
        /// </summary>
        public static void DoAll_TEST()
        {
            SerialPort port = new SerialPort("COM9");
            port.BaudRate = 9600;
            port.DataBits = 8;
            port.Parity = Parity.None;
            port.StopBits = StopBits.One;
            port.ReadTimeout = TIMEOUT;
            port.WriteTimeout = TIMEOUT;

            port.Open();

            _PORT = port;

            var factory = new ModbusFactory();
            IModbusMaster? master = null;
            master = factory.CreateRtuMaster(_PORT);
            master.Transport.ReadTimeout = TIMEOUT;
            master.Transport.WriteTimeout = TIMEOUT;
            _MASTER = master;

            ushort[] data;
            try
            {
                data = _MASTER.ReadHoldingRegisters(182, 1, 16);

            }
            catch (Exception ex)
            {
                Logger.Log($"ReadHoldingRegisters: {ex}");
                data = null;
            }

            Console.WriteLine($"Result: {data?.First()}");

        }
        /// <summary>
        /// Opens COM port and saves it in the global variable _PORT
        /// </summary>
        /// <param name="PrimarySerialPortName"></param>
        /// <returns></returns>
        public static bool OpenPort(string PrimarySerialPortName)
        {

            try
            {

                SerialPort port = new SerialPort(PrimarySerialPortName);
                port.BaudRate = 9600;
                port.DataBits = 8;
                port.Parity = Parity.None;
                port.StopBits = StopBits.One;
                port.ReadTimeout = TIMEOUT;
                port.WriteTimeout = TIMEOUT;

                port.Open();

                _PORT = port;

            }
            catch (Exception ex)
            {
                Logger.Log($"OpenPort: {ex.Message}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Closts COM port
        /// </summary>
        /// <returns></returns>
        public static bool ClosePort()
        {
            if (_PORT == null || _PORT.IsOpen == false) return true;
            try
            {
                _PORT.Close();
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log($"ClosePort: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Creates RTU master
        /// </summary>
        /// <param name="_PORT"></param>
        /// <returns></returns>
        public static bool CreateRtuMaster(SerialPort _PORT)
        {
            try
            {
                var factory = new ModbusFactory();
                IModbusMaster? master = null;
                master = factory.CreateRtuMaster(_PORT);
                //var serialTransport = factory.CreateRtuTransport(_PORT);
                //master = factory.CreateMaster(serialTransport);
                master.Transport.ReadTimeout = TIMEOUT;
                master.Transport.WriteTimeout = TIMEOUT;
                _MASTER = master;
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log($"CreateRtuMaster: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Creates ASCII master
        /// </summary>
        /// <param name="_PORT"></param>
        /// <returns></returns>
        public static bool CreateAsciiMaster(SerialPort _PORT)
        {
            try
            {
                var factory = new ModbusFactory();
                IModbusMaster? master = null;
                master = factory.CreateAsciiMaster(_PORT);
                master.Transport.ReadTimeout = TIMEOUT;
                master.Transport.WriteTimeout = TIMEOUT;
                _MASTER = master;
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log($"CreateAsciiMaster: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Reads registers
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="startAddress"></param>
        /// <param name="numberOfPoints"></param>
        /// <returns></returns>
        public static ushort[] ReadHoldingRegisters(byte slaveId, ushort startAddress, ushort numberOfPoints)
        {
            if (_PORT == null || _PORT.IsOpen == false)
            {
                Logger.Log("COM Port is not ready");
                return null;
            }
            if (_MASTER is null)
            {
                Logger.Log("_MASTER is null");
                return null;
            }
            try
            {
                var data = _MASTER?.ReadHoldingRegisters(slaveId, startAddress, numberOfPoints);
                return data;
            }
            catch (Exception ex)
            {
                Logger.Log($"ReadHoldingRegisters({slaveId}, {startAddress}, {numberOfPoints}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Another way to read registers
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="startAddress"></param>
        /// <param name="numberOfPoints"></param>
        /// <returns></returns>
        public static ushort[] ReadInputRegisters(byte slaveId, ushort startAddress, ushort numberOfPoints)
        {
            try
            {
                var data = _MASTER?.ReadInputRegisters(slaveId, startAddress, numberOfPoints);
                return data;
            }
            catch (Exception ex)
            {
                Logger.Log($"ReadInputRegisters: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Returns offsets to read singular registers
        /// </summary>
        /// <param name="ovenVariant"></param>
        /// <returns></returns>
        public static List<ushort> GetOvenInputOffsets(int ovenVariant = 0)
        {
            // 0 ОВЕН МВ110-220.8АС
            // 1 ОВЕН МВ110-224.8А
            // first go register offsets
            // then address
            var ovenNames = new List<string>() { "МВ110-220.8АС", "МВ110-224.8А" };
            //Logger.Log($"Oven model: {ovenNames[ovenVariant]}");
            return ovenVariant switch
            {
                0 => new List<ushort>() { 1, 3, 5, 7, 9, 11, 13, 15, 263 },
                1 => new List<ushort>() { 1, 7, 13, 19, 25, 31, 37, 43, 0 },
                _ => null
            };
        }

    }
}
