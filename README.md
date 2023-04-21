# OVENSensorReader
Reads input registers of OVEN modules: ОВЕН MB110-220.8AC, ОВЕН MB110-224.8A
Uses Modbus RTU master via Serial Port. Uses ReadHoldingRegisters() function.

![](form.png)

1. You enter the COM port
2. Set timeout, 100 by default (need to reconnect to apply changes)
3. Press Connect
4. Select OVEN model
5. Set Slave ID
6. Checkbox on the left means this device will be scanned
7. Press Read
8. If the device can't be connected, the checkbox will be unchecked
9. For continuous monitoring, check "Loop" checkbox

This is a threaded application, UI may be partially blocked during reading.
Application requires NuGets: NModbus, NModbus.Serial, System.IO.Ports.
