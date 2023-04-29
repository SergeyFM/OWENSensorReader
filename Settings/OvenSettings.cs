
namespace OvenSensorReader.Settings {
    internal class OvenSettings {
        public byte SlaveID { get; set; }
        public string Name { get; set; }
        public ushort StartAdress { get; set; }
        public int NumberOfPoints { get; set; }
        public List<ushort> RegisterOffsets { get; set; }

}
}
