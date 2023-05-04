# OVENSensorReader
Reads input registers of OVEN modules: ОВЕН MB110-220.8AC, ОВЕН MB110-224.8A
Uses Modbus RTU master via Serial Port. Uses ReadHoldingRegisters() function. Works asynchronously.

![](form.png)

1. Enter COM port settings: port name, timeout. It's hardcoded to 8-N-1 mode, 9600 baud rate.
2. Select OVEN Slave IDs and models. Check the checkbox on the left to include this device in the monitoring.
3. Press Read
4. For continuous monitoring, click "Loop" switch

Application requires NuGets: NModbus, NModbus.Serial, System.IO.Ports.
Settings are saved in the application folder in the files _AppSettings.json, _OvenModels.json, _OvenSettings.json - should be in the same folder as the executable. File _OvenModels.json allows adding new models of devices. 

