using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvenSensorReader.Settings {
    internal class OvenModel {
        public int OvenModelID { get; set; }
        public string Name { get; set; }
        public ushort StartAdress { get; set; }
        public int NumberOfPoints { get; set; }
        public List<ushort> RegisterOffsets { get; set; }



        // toString()
        public override string ToString() {
            return $"OvenName: {Name}, \nStartAdress: {StartAdress}, \nNumberOfPoints: {NumberOfPoints}, \nRegisterOffsets: {RegisterOffsets?.Aggregate("", (aggr, c) => aggr + c + " ")}";
        }

    }
}
