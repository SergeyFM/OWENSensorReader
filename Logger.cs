using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvenSensorReader;

namespace PassportGenerator.mod {
    /// <summary>
    /// Logger class.
    /// Prints info to the console and tries to update info on the form (if provided).
    /// </summary>
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
