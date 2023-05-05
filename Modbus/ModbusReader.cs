using System.IO.Ports;
using NModbus;
using NModbus.Serial;

namespace OvenSensorReader.Modbus;

/// <summary>
/// Static class with modbus functions
/// </summary>
public class ModbusReader {


    public SerialPort _PORT = null;
    public IModbusMaster _MASTER = null;
    public int TIMEOUT = 80;


    /// <summary>
    /// Opens COM port and saves it in the global variable _PORT
    /// </summary>
    /// <param name="PrimarySerialPortName"></param>
    /// <returns></returns>
    public bool OpenPort(string PrimarySerialPortName) {

        try {

            SerialPort port = new SerialPort(PrimarySerialPortName);
            port.BaudRate = 9600;
            port.DataBits = 8;
            port.Parity = Parity.None;
            port.StopBits = StopBits.One;
            port.ReadTimeout = TIMEOUT;
            port.WriteTimeout = TIMEOUT;

            glLogger.Log("Open port...");

            port.Open();

            if (!port.IsOpen) {
                glLogger.Log($"Unsucessfully tried to open the port {PrimarySerialPortName}");
                return false;
            } else {
                glLogger.Log($"Opened the port {PrimarySerialPortName}");
            }

            _PORT = port;

        } catch (Exception ex) {
            glLogger.Log($"OpenPort: {ex.Message}");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Closts COM port
    /// </summary>
    /// <returns></returns>
    public bool ClosePort() {
        if (_PORT == null || _PORT.IsOpen == false) return true;
        try {
            _PORT.Close();
            return true;
        } catch (Exception ex) {
            glLogger.Log($"ClosePort: {ex.Message}", true);
            return false;
        }
    }

    /// <summary>
    /// Creates RTU master
    /// </summary>
    /// <param name="_PORT"></param>
    /// <returns></returns>
    public bool CreateRtuMaster(SerialPort _PORT) {
        if (glModbusReader._PORT == null || glModbusReader._PORT.IsOpen == false) return false;
        try {
            var factory = new ModbusFactory();
            IModbusMaster? master = null;
            master = factory.CreateRtuMaster(_PORT);
            //var serialTransport = factory.CreateRtuTransport(_PORT);
            //master = factory.CreateMaster(serialTransport);
            master.Transport.ReadTimeout = TIMEOUT;
            master.Transport.WriteTimeout = TIMEOUT;
            _MASTER = master;
            return true;
        } catch (Exception ex) {
            glLogger.Log($"CreateRtuMaster: {ex.Message}", true);
            return false;
        }
    }

    /// <summary>
    /// Creates ASCII master
    /// </summary>
    /// <param name="_PORT"></param>
    /// <returns></returns>
    public bool CreateAsciiMaster(SerialPort _PORT) {
        try {
            var factory = new ModbusFactory();
            IModbusMaster? master = null;
            master = factory.CreateAsciiMaster(_PORT);
            master.Transport.ReadTimeout = TIMEOUT;
            master.Transport.WriteTimeout = TIMEOUT;
            _MASTER = master;
            return true;
        } catch (Exception ex) {
            glLogger.Log($"CreateAsciiMaster: {ex.Message}");
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
    public ushort[] ReadHoldingRegisters(byte slaveId, ushort startAddress, ushort numberOfPoints) {
        if (_PORT == null || _PORT.IsOpen == false) {
            glLogger.Log("COM Port is not ready");
            return null;
        }
        if (_MASTER is null) {
            glLogger.Log("_MASTER is null");
            return null;
        }
        try {
            var data = _MASTER?.ReadHoldingRegisters(slaveId, startAddress, numberOfPoints);
            return data;
        } catch (Exception ex) {
            glLogger.Log($"ReadHoldingRegisters({slaveId}, {startAddress}, {numberOfPoints}: {ex.Message}");
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
    public ushort[] ReadInputRegisters(byte slaveId, ushort startAddress, ushort numberOfPoints) {
        try {
            var data = _MASTER?.ReadInputRegisters(slaveId, startAddress, numberOfPoints);
            return data;
        } catch (Exception ex) {
            glLogger.Log($"ReadInputRegisters: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Returns offsets to read singular registers
    /// </summary>
    /// <param name="ovenVariant"></param>
    /// <returns></returns>
    public List<ushort> GetOvenInputOffsets(int ovenVariant = 0) {
        // 0 ОВЕН МВ110-220.8АС
        // 1 ОВЕН МВ110-224.8А
        // first go register offsets
        // then address
        var ovenNames = new List<string>() { "МВ110-220.8АС", "МВ110-224.8А" };
        return ovenVariant switch {
            0 => new List<ushort>() { 1, 3, 5, 7, 9, 11, 13, 15, 263 },
            1 => new List<ushort>() { 1, 7, 13, 19, 25, 31, 37, 43, 0 },
            _ => null
        };
    }

    /// <summary>
    /// Returns a list of values from the oven
    /// </summary>
    /// <param name="slaveId"></param>
    /// <param name="ovenModelName"></param>
    /// <returns></returns>
    public List<string> ReadListOfValues(byte slaveId, string ovenModelName) {
        try {
            OvenModel currentOvenModel = glSettingsProvider.GetSettings_OvenModelsList().Where(m => m.Name == ovenModelName).FirstOrDefault();
            ushort startAddress = currentOvenModel.StartAdress;
            var values = glModbusReader.ReadHoldingRegisters(slaveId, startAddress, currentOvenModel.NumberOfPoints);
            if (values is null) return null;

            List<string> vals = new();

            for (int i = 0; i < 8; i++) {

                ushort offset = currentOvenModel.RegisterOffsets[i];

                var cellVal = values[offset].ToString();
                //lineTextBoxes[i].Text = cellVal;
                vals.Add(cellVal);
            }
            return vals;
        } catch (Exception ex) {
            glLogger.Log("ReadListOfValues: " + ex.Message);

            return null;
        }

    }


}
