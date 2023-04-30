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
        return "App settings saved";
    }
    public string SaveSettings(OvenSettings sett) {
        if (sett is null) return "Settings is null";
        _OvenSettings.RemoveAll(x => x.OvenID == sett.OvenID);
        _OvenSettings.Add(sett);
        return "App settings saved";
    }

    public string SaveSettings(OvenModel ovenModel) {
        if (ovenModel is null) return "ovenModel is null";
        _OvenModels.RemoveAll(x => x.Name == ovenModel.Name);
        _OvenModels.Add(ovenModel);
        return "Oven model saved";
    }

    public AppSettings GetSettings_AppSettings(int SettingsID = 0) {
        if (_AppSettings is null || _AppSettings.Count == 0) return default(AppSettings);
        AppSettings sett = _AppSettings.FirstOrDefault(s => s.AppSettingID == SettingsID);
        return sett;
    }

    public List<OvenSettings> GetSettings_OvenSettingsList(int SettingsIDmin = 0, int SettingsIDmax = 5) {
        if (_OvenSettings is null || _OvenSettings.Count == 0) return Enumerable.Repeat(default(OvenSettings),6).ToList();
        List<OvenSettings> sett = _OvenSettings.Where(s => s.OvenID >= SettingsIDmin && s.OvenID <= SettingsIDmax).ToList();
        return sett;
    }

    public List<OvenModel> GetSettings_OvenModelsList() {
        if (_OvenModels is null || _OvenModels.Count == 0) return Enumerable.Repeat(default(OvenModel), 1).ToList();
        List<OvenModel> sett = _OvenModels;
        return sett;
    }

    public void ClearSettings() {
        _AppSettings.Clear();
        _OvenSettings.Clear();
        _OvenModels.Clear();
    }

    public void LoadSettingsFile() {

        var json = File.ReadAllText(nameof(_AppSettings) + ".json");
        _AppSettings.AddRange(JsonConvert.DeserializeObject<List<AppSettings>>(json));

        json = File.ReadAllText(nameof(_OvenSettings) + ".json");
        _OvenSettings.AddRange(JsonConvert.DeserializeObject<List<OvenSettings>>(json));
        if (_OvenSettings.Count < 6) 
            _OvenSettings.AddRange(Enumerable.Repeat(new OvenSettings(), 6 - _OvenSettings.Count));
        

        json = File.ReadAllText(nameof(_OvenModels) + ".json");
        _OvenModels.AddRange(JsonConvert.DeserializeObject<List<OvenModel>>(json));



    }

    public void SaveSettingsFile() {

        var json = JsonConvert.SerializeObject(_AppSettings, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(nameof(_AppSettings) + ".json", json);

        json = JsonConvert.SerializeObject(_OvenSettings, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(nameof(_OvenSettings) + ".json", json);

        json = JsonConvert.SerializeObject(_OvenModels, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(nameof(_OvenModels) + ".json", json);

    }


}
