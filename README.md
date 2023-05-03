# OVENSensorReader
Reads input registers of OVEN modules: ОВЕН MB110-220.8AC, ОВЕН MB110-224.8A
Uses Modbus RTU master via Serial Port. Uses ReadHoldingRegisters() function.

![](form.png)

1. You enter the COM port
2. Set timeout (need to reconnect to apply changes)
3. Select OVEN Slave IDs and models
4. Checkbox on the left means this device will be scanned
5. Press Read
6. If the device can't be connected, the checkbox will uncheck itself
7. For continuous monitoring, check "Loop" checkbox

Application requires NuGets: NModbus, NModbus.Serial, System.IO.Ports.
Settings are saved in the application folder in the files _AppSettings.json, _OvenModels.json, _OvenSettings.json - should be in the same folder as the executable.
File _OvenModels.json allows adding new models of OVEN devices. File _OvenSettings.json contains the settings of the devices. File _AppSettings.json contains the settings of the application.

