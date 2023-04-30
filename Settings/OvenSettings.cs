
namespace OvenSensorReader.Settings;
internal class OvenSettings {
    public int OvenID { get; set; } = 0;
    public byte SlaveID { get; set; } = 0;

    public OvenModel OvenModel { get; set; } = new OvenModel();


    public override string ToString() {
        return $"OvenID: {OvenID}, \nSlaveID: {SlaveID}, \nOvenModel: {OvenModel.Name}";
    }

}
