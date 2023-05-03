using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvenSensorReader.Settings {
    internal class OvenModel {
        public int OvenModelID { get; set; } = 0;
        public string Name { get; set; } = "NONE";
        public ushort StartAdress { get; set; } = 0;
        public ushort NumberOfPoints { get; set; } = 0;
        public List<ushort> RegisterOffsets { get; set; } = new List<ushort>() { 0,0,0,0,0,0,0,0 };



        // toString()
        public override string ToString() {
            return $"OvenName: {Name}, \nStartAdress: {StartAdress}, \nNumberOfPoints: {NumberOfPoints}, \nRegisterOffsets: {RegisterOffsets?.Aggregate("", (aggr, c) => aggr + c + " ")}";
        }

    }
}
