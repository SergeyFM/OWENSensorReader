using Newtonsoft.Json;

namespace OvenSensorReader.Settings;

internal class SettingsProvider {
    private static List<AppSettings> _AppSettings = new();
    private static List<OvenSettings> _OvenSettings = new();
    private static List<OvenModel> _OvenModels = new();

    public string SaveSettings(AppSettings sett) {
        if (sett is null) return "Settings is null";
        _AppSettings.RemoveAll(x => x.AppSettingID == sett.AppSettingID);
        _AppSettings.Add(sett);
        string msg = "App settings saved";
        glLogger.Log(msg);
        return msg;
    }
    public string SaveSettings(List<OvenSettings> ovenSettList) {
        if (ovenSettList is null) return "Oven settings list is null";
        ovenSettList.ForEach(sett => {
            _OvenSettings.RemoveAll(x => x.OvenID == sett.OvenID);
            _OvenSettings.Add(sett);
        });
        return "App settings saved";
    }

    public string SaveSettings(OvenModel ovenModel) {
        if (ovenModel is null) return "ovenModel is null";
        _OvenModels.RemoveAll(x => x.Name == ovenModel.Name);
        _OvenModels.Add(ovenModel);
        string msg = "Oven model saved";
        glLogger.Log(msg);
        return msg;
    }

    public AppSettings GetSettings_AppSettings(int SettingsID = 0) {
        if (_AppSettings is null || _AppSettings.Count == 0) return new AppSettings();
        AppSettings sett = _AppSettings.FirstOrDefault(s => s.AppSettingID == SettingsID);
        return sett;
    }

    public List<OvenSettings> GetSettings_OvenSettingsList(int SettingsIDmin = 0, int SettingsIDmax = 5) {
        if (_OvenSettings is null || _OvenSettings.Count == 0) return Enumerable.Repeat(new OvenSettings(), 6).ToList();
        List<OvenSettings> sett = _OvenSettings.Where(s => s.OvenID >= SettingsIDmin && s.OvenID <= SettingsIDmax).ToList();
        return sett;
    }

    public List<OvenModel> GetSettings_OvenModelsList() {
        if (_OvenModels is null || _OvenModels.Count == 0) return Enumerable.Repeat(new OvenModel(), 1).ToList();
        List<OvenModel> sett = _OvenModels;
        return sett;
    }

    public void ClearSettings() {
        _AppSettings.Clear();
        _OvenSettings.Clear();
        _OvenModels.Clear();
    }

    public void LoadSettingsFile() {
        try {
            var json = File.ReadAllText(nameof(_AppSettings) + ".json");
            _AppSettings.Clear();
            _AppSettings.AddRange(JsonConvert.DeserializeObject<List<AppSettings>>(json));

            _AppSettings.ForEach(aset => {
                Console.WriteLine(aset);
            });

            json = File.ReadAllText(nameof(_OvenSettings) + ".json");
            _OvenSettings.Clear();
            _OvenSettings.AddRange(JsonConvert.DeserializeObject<List<OvenSettings>>(json));
            if (_OvenSettings.Count < 6)
                _OvenSettings.AddRange(Enumerable.Repeat(new OvenSettings(), 6 - _OvenSettings.Count));

            json = File.ReadAllText(nameof(_OvenModels) + ".json");
            _OvenModels.Clear();
            _OvenModels.AddRange(JsonConvert.DeserializeObject<List<OvenModel>>(json));
        } catch (Exception ex) {
            glLogger.Log(ex.Message);
        }


    }

    public void SaveSettingsFile() {
        try {
            var json = JsonConvert.SerializeObject(_AppSettings, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(nameof(_AppSettings) + ".json", json);

            json = JsonConvert.SerializeObject(_OvenSettings, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(nameof(_OvenSettings) + ".json", json);

            json = JsonConvert.SerializeObject(_OvenModels, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(nameof(_OvenModels) + ".json", json);

        } catch (Exception ex) {
            glLogger.Log(ex.Message);
        }
    }


}
