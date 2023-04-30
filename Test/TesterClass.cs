using System.Runtime.InteropServices;


namespace OvenSensorReader.Test;

/// <summary>
/// Test class, here you can test functions separately.
/// </summary>
internal static class TesterClass {

    public const bool TESTING = false; // when true, launches only command line and function Run().

    public static void Run() {
        AllocConsole();


        var ovenModel = new OvenModel() {
            OvenModelID = 0,
            Name = "МВ110-220.8АС",
            StartAdress = 263,
            NumberOfPoints = 16,
            RegisterOffsets = new List<ushort>() { 1, 3, 5, 7, 9, 11, 13, 15 }
        };

        var ovenModel2 = new OvenModel() {
            OvenModelID = 1,
            Name = "МВ110-224.8А",
            StartAdress = 0,
            NumberOfPoints = 16,
            RegisterOffsets = new List<ushort>() { 1, 7, 13, 19, 25, 31, 37, 43 }
        };

        var oven = new OvenSettings() {
            OvenID = 0,
            SlaveID = 100,
            OvenModel = ovenModel
        };

        var appSettings = new AppSettings() {

            AppSettingID = 0,
            ComPort = "COM1",
            Timeout = 1000,
            CheckBoxesList = new List<bool> { true, true, true, true, true, true },
            LoopCheckBox = false,
            OvenID = 0
        };


        glSettingsProvider.SaveSettings(appSettings);
        glSettingsProvider.SaveSettings(oven);
        glSettingsProvider.SaveSettings(ovenModel);
        glSettingsProvider.SaveSettings(ovenModel2);


        glSettingsProvider.SaveSettingsFile();
        glSettingsProvider.ClearSettings();
        glSettingsProvider.LoadSettingsFile();

        var appSettings2 = glSettingsProvider.GetSettings_AppSettings();
        var oven2 = glSettingsProvider.GetSettings_OvenSettingsList();
        var ovenModelsFromFiles = glSettingsProvider.GetSettings_OvenModelsList();

        Console.WriteLine("\n* App settings:\n" + appSettings2);
        Console.WriteLine("\n* Oven settings:\n" + oven2.First());
        Console.WriteLine("\n* Oven models:\n" + ovenModelsFromFiles.First());

        Console.ReadKey();

    }



    // To open a console ---------------------------
    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool AllocConsole();
    // ---------------------------------------------

}
