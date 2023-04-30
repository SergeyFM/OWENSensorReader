
namespace OvenSensorReader.Log;


/// <summary>
/// Logger class.
/// Prints info to the console and tries to update info on the form (if provided).
/// </summary>
internal class Logger
{
    public static FormMain FormPtr { get; set; }
    public void Log(string message, bool critical = false)
    {
        Console.WriteLine($">> {message}");
        if (FormPtr is not null)
        {
            Application.DoEvents();
            FormPtr.toolStripStatusLabel.Text = message;
            FormPtr.statusStrip.Refresh();
            Application.DoEvents();
            if (critical)
            {

            }

        }

    }
}
