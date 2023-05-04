namespace OvenSensorReader;

public partial class FormMain : Form {

    public List<CheckBox> checkboxes = new();
    public List<TextBox[]> fields = new();
    public List<TextBox> slaveIDs = new();
    public List<ComboBox> ovenModelsFields = new();
    public List<Button> buttons = new();

    public FormMain() {
        InitializeComponent();
    }

    private void FormMain_Load(object sender, EventArgs e) {
        Logger.FormPtr = this;
        glLogger.Log("Loading the main form...");
        LoadFormElements();
        LoadSettings();
    }

    private void buttonConnectToCOM_Click(object sender, EventArgs e) {
        ConnectToCOMPort();
    }

    private void buttonDisconnectCOM_Click(object sender, EventArgs e) {
        DisconnectFromCOMPort();
    }

    private void buttonReadValuesOnce_ClickAsync(object sender, EventArgs e) {
        StartReadingValuesAsync();
    }

    private void buttonSetAllCheckboxesON_Click(object sender, EventArgs e) {
        checkboxes.ForEach(x => x.Checked = true);
    }

    private void FormMain_FormClosing(object sender, FormClosingEventArgs e) {
        SaveSettings();
    }
    public void SetButtonsEnable(bool enable) {
        buttons.ForEach(b => b.Enabled = enable);
    }



    public void ConnectToCOMPort() {
        ModbusReader.TIMEOUT = int.TryParse(textBoxTIMEOUT.Text, out ModbusReader.TIMEOUT) ? ModbusReader.TIMEOUT : 200;
        this.labelCOMPortConnectionResult.Text = ModbusReader.OpenPort(this.textBoxCOMPort.Text) ? "COM_cnn OK" : "COM_cnn ERR";
        Application.DoEvents();
        if (ModbusReader._PORT is null) return;
        string createMasterResult = ModbusReader.CreateRtuMaster(ModbusReader._PORT) ? ", RtuMaster OK" : ", RtuMaster ERR";
        this.labelCOMPortConnectionResult.Text += createMasterResult;
    }

    public void DisconnectFromCOMPort() {
        if (ModbusReader._PORT != null && ModbusReader._PORT.IsOpen)
            this.labelCOMPortConnectionResult.Text = ModbusReader.ClosePort() ? "Closing OK" : "Error closing";
    }


    public async Task StartReadingValuesAsync() {
        SetButtonsEnable(false);
        glLogger.Log("Start reading data...", true);
        List<string> ovenModelsFieldsSelected = ovenModelsFields.Select(x => x.SelectedItem.ToString()).ToList();

        ModbusReader.TIMEOUT = int.TryParse(textBoxTIMEOUT.Text, out ModbusReader.TIMEOUT) ? ModbusReader.TIMEOUT : 200;
        if (ModbusReader._PORT == null || ModbusReader._PORT.IsOpen == false) ConnectToCOMPort();
        int ind = 0;
        do {
            Application.DoEvents();
            if (ModbusReader._PORT is null || ModbusReader._PORT.IsOpen == false) break;

            if (checkboxes.Count(c => c.Checked) == 0) checkBoxLoop.Checked = false;
            Application.DoEvents();
            byte slaveId = Convert.ToByte(slaveIDs[ind].Text);
            string ovenModelName = ovenModelsFieldsSelected[ind];
            glLogger.Log($"SlaveID: {slaveId}", true);

            var res = await Task.Run(() => fillTheLine(slaveId, ovenModelName));

            if (res != null) {
                for (int i = 0; i < 8; i++) fields[ind][i].Text = res[i];
            } else {
                checkboxes[ind].Checked = false;
                glLogger.Log($"Couldn't read SlaveID: {slaveId}", true);
            }
            ind++;
            if (ind >= 6 && checkBoxLoop.Checked) ind = 0;
        } while (ind < 6);
        DisconnectFromCOMPort();
        SetButtonsEnable(true);
    }

    private List<string> fillTheLine(byte slaveId, string ovenModelName) {
        try {
            OvenModel currentOvenModel = glSettingsProvider.GetSettings_OvenModelsList().Where(m => m.Name == ovenModelName).FirstOrDefault();
            ushort startAddress = currentOvenModel.StartAdress;
            var values = ModbusReader.ReadHoldingRegisters(slaveId, startAddress, currentOvenModel.NumberOfPoints);
            if (values is null) return null;

            List<string> vals = new();

            for (int i = 0; i < 8; i++) {

                ushort offset = currentOvenModel.RegisterOffsets[i];

                var cellVal = values[offset].ToString();
                //lineTextBoxes[i].Text = cellVal;
                vals.Add(cellVal);
            }
            return vals;
        } catch (Exception ex) {
            glLogger.Log("fillTheLine: " + ex.Message);

            return null;
        }

    }

    private void SaveSettings() {

        AppSettings appSettings = new AppSettings() {
            AppSettingID = 0,
            CheckBoxesList = checkboxes.Select(c => c.Checked).ToList(),
            ComPort = textBoxCOMPort.Text,
            LoopCheckBox = checkBoxLoop.Checked,
            Timeout = int.TryParse(textBoxTIMEOUT.Text, out int timeout) ? timeout : 200
        };
        List<OvenSettings> ovenSettings = slaveIDs.Select(s => new OvenSettings() {
            OvenID = slaveIDs.IndexOf(s),
            SlaveID = byte.TryParse(s.Text, out byte slaveID) ? slaveID : (byte)0,
            OvenModel = glSettingsProvider.GetSettings_OvenModelsList().FirstOrDefault(o => o.Name == ovenModelsFields[slaveIDs.IndexOf(s)].SelectedItem.ToString())
        }).ToList();

        glSettingsProvider.SaveSettings(appSettings);
        glSettingsProvider.SaveSettings(ovenSettings);
        glSettingsProvider.SaveSettingsFile();
    }

    public void LoadFormElements() {
        checkboxes = new List<CheckBox>() { checkBox1, checkBox2, checkBox3, checkBox4, checkBox5, checkBox6 };
        fields = new List<TextBox[]>() {
            new[]{textBoxO1I1, textBoxO1I2, textBoxO1I3, textBoxO1I4, textBoxO1I5, textBoxO1I6, textBoxO1I7, textBoxO1I8 },
            new[]{textBoxO2I1, textBoxO2I2, textBoxO2I3, textBoxO2I4, textBoxO2I5, textBoxO2I6, textBoxO2I7, textBoxO2I8 },
            new[]{textBoxO3I1, textBoxO3I2, textBoxO3I3, textBoxO3I4, textBoxO3I5, textBoxO3I6, textBoxO3I7, textBoxO3I8 },
            new[]{textBoxO4I1, textBoxO4I2, textBoxO4I3, textBoxO4I4, textBoxO4I5, textBoxO4I6, textBoxO4I7, textBoxO4I8 },
            new[]{textBoxO5I1, textBoxO5I2, textBoxO5I3, textBoxO5I4, textBoxO5I5, textBoxO5I6, textBoxO5I7, textBoxO5I8 },
            new[]{textBoxO6I1, textBoxO6I2, textBoxO6I3, textBoxO6I4, textBoxO6I5, textBoxO6I6, textBoxO6I7, textBoxO6I8 }
        };
        ovenModelsFields = new List<ComboBox>() { comboBoxOvenModel1, comboBoxOvenModel2, comboBoxOvenModel3, comboBoxOvenModel4, comboBoxOvenModel5, comboBoxOvenModel6 };
        slaveIDs = new List<TextBox>() { textBoxSlaveID1, textBoxSlaveID2, textBoxSlaveID3, textBoxSlaveID4, textBoxSlaveID5, textBoxSlaveID6 };
        buttons = new List<Button>() { buttonConnectToCOM, buttonDisconnectCOM, buttonReadValuesOnce };
    }
    public void LoadSettings() {
        // Load settings from Json file
        glSettingsProvider.LoadSettingsFile();
        var appSettings = glSettingsProvider.GetSettings_AppSettings();
        var ovenSettings = glSettingsProvider.GetSettings_OvenSettingsList();
        var ovenModels = glSettingsProvider.GetSettings_OvenModelsList();
        checkboxes.ForEach(c => c.Checked = appSettings.CheckBoxesList[checkboxes.IndexOf(c)]);
        slaveIDs.ForEach(s => s.Text = ovenSettings[slaveIDs.IndexOf(s)].SlaveID.ToString());
        this.checkBoxLoop.Checked = appSettings.LoopCheckBox;
        this.textBoxCOMPort.Text = appSettings.ComPort;
        this.textBoxTIMEOUT.Text = appSettings.Timeout.ToString();
        ovenModelsFields.ForEach(o => o.DataSource = ovenModels.Select(o => o.Name).ToList());
        ovenModelsFields.ForEach(
         o => o.SelectedIndex = ovenModels.Where(m => m.Name == ovenSettings[ovenModelsFields.IndexOf(o)].OvenModel.Name)
        .Select(mid => mid.OvenModelID).FirstOrDefault()
        );


    }





    // ---------------------------------------------------------------------------------------------------------------------------



}