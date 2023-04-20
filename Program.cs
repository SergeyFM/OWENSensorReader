using PassportGenerator.test;

namespace OvenSensorReader {
    internal static class Program {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            if (TesterClass.TESTING) {
                TesterClass.Run();
                return;
            }
            ApplicationConfiguration.Initialize();
            Application.Run(new FormMain());
        }
    }
}