using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvenSensorReader;

namespace PassportGenerator.mod {
    internal static class Logger {
        public static FormMain FormPtr { get; set; }
        public static void Log(string message, bool critical = false) {
            Console.WriteLine($">> {message}");
            if (FormPtr is not null) {
                Application.DoEvents();
                FormPtr.toolStripStatusLabel.Text = message;
                FormPtr.statusStrip.Refresh();
                Application.DoEvents();
                if (critical) {

                }

            }
            
        }
    }
}
