using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvenSensorReader.Settings
{
    internal class SettingsProvider
    {
        // singleton
        private static SettingsProvider _instance;
        public static SettingsProvider Instance {
            get {
                if (_instance == null) {
                    _instance = new SettingsProvider();
                }
                return _instance;
            }
        }

    }
}
