
namespace OvenSensorReader.Settings; 
internal class AppSettings {
    public int AppSettingID { get; set; } = 0;
    public string ComPort { get; set; } = "COM1";
    public int Timeout { get; set; } = 1000;
    public List<bool> CheckBoxesList { get; set; } = new List<bool>() { true, true, true, true, true, true };
    public bool LoopCheckBox { get; set; }  = false;


    public override string ToString() {
        return $"AppSettingID: {AppSettingID}, \nComPort: {ComPort}, \nTimeout: {Timeout}, \nCheckBoxesList: {CheckBoxesList?.Aggregate("", (aggr, c) => aggr + c + " ")}, \nLoopCheckBox: {LoopCheckBox}";
    }

}
