
namespace OvenSensorReader.Settings; 
internal class AppSettings {
    public int AppSettingID { get; set; }
    public string ComPort { get; set; }
    public int Timeout { get; set; }
    public List<bool> CheckBoxesList { get; set; }
    public bool LoopCheckBox { get; set; }


    public override string ToString() {
        return $"AppSettingID: {AppSettingID}, \nComPort: {ComPort}, \nTimeout: {Timeout}, \nCheckBoxesList: {CheckBoxesList?.Aggregate("", (aggr, c) => aggr + c + " ")}, \nLoopCheckBox: {LoopCheckBox}";
    }

}
