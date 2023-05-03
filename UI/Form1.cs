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

    /// <summary>
    /// Creates global lists of textboxes, checklists
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FormMain_Load(object sender, EventArgs e) {

        Logger.FormPtr = this;

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

        LoadSettings();

        ModbusReader.TIMEOUT = int.TryParse(textBoxTIMEOUT.Text, out ModbusReader.TIMEOUT) ? ModbusReader.TIMEOUT : 200;
    }

    public void SetButtonsEnable(bool enable) {
        buttons.ForEach(b =>  b.Enabled = enable);
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


    /// <summary>
    /// Connect to COM port and creates RTU Master
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void buttonConnectToCOM_Click(object sender, EventArgs e) {
        ModbusReader.TIMEOUT = int.TryParse(textBoxTIMEOUT.Text, out ModbusReader.TIMEOUT) ? ModbusReader.TIMEOUT : 200;
        this.labelCOMPortConnectionResult.Text = ModbusReader.OpenPort(this.textBoxCOMPort.Text) ? "COM_cnn OK" : "COM_cnn ERR";
        Application.DoEvents();
        if (ModbusReader._PORT is null) return;
        string createMasterResult = ModbusReader.CreateRtuMaster(ModbusReader._PORT) ? ", RtuMaster OK" : ", RtuMaster ERR";
        this.labelCOMPortConnectionResult.Text += createMasterResult;
    }

    /// <summary>
    /// Disconnects COM port
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void buttonDisconnectCOM_Click(object sender, EventArgs e) {
        if (ModbusReader._PORT != null && ModbusReader._PORT.IsOpen)
            this.labelCOMPortConnectionResult.Text = ModbusReader.ClosePort() ? "Closing OK" : "Error closing";
    }



    /// <summary>
    /// Starts reading process.
    /// When checkbox "loop" is checked, repeats reading
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void buttonReadValuesOnce_Click(object sender, EventArgs e) {
        SetButtonsEnable(false);
        ModbusReader.TIMEOUT = int.TryParse(textBoxTIMEOUT.Text, out ModbusReader.TIMEOUT) ? ModbusReader.TIMEOUT : 200;
        if (ModbusReader._PORT != null && ModbusReader._PORT.IsOpen == false) buttonConnectToCOM_Click(sender, e);
        int ind = 0;
        do {
            Application.DoEvents();
            if (ModbusReader._PORT is null || ModbusReader._PORT.IsOpen == false || checkboxes.Count(c => c.Checked) == 0) checkBoxLoop.Checked = false;
            Application.DoEvents();
            bool res = await Task.Run(() => fillTheLine(ind, checkboxes, fields, slaveIDs, ovenModelsFields));
            checkboxes[ind].Checked = res;
            ind++;
            if (ind >= 6 && checkBoxLoop.Checked) ind = 0;
        } while (ind < 6);
        buttonDisconnectCOM_Click(sender, e);
        SetButtonsEnable(true);
    }

    /// <summary>
    /// Gets register values from the OVEN and puts them in the textboxes
    /// </summary>
    /// <param name="lineInd"></param>
    /// <param name="checkboxes"></param>
    /// <param name="fields"></param>
    /// <param name="slaveIDs"></param>
    private bool fillTheLine(int lineInd, List<CheckBox> checkboxes, List<TextBox[]> fields, List<TextBox> slaveIDs, List<ComboBox> ovenModelNames) {
        try {

            if (checkboxes[lineInd].Checked == false) return false;
            List<TextBox> lineTextBoxes = fields[lineInd].ToList();
            OvenModel currentOvenModel = glSettingsProvider.GetSettings_OvenModelsList().Where(m => m.Name == ovenModelsFields[lineInd].SelectedItem.ToString()).FirstOrDefault();
            ushort startAddress = currentOvenModel.StartAdress;
            byte slaveId = Convert.ToByte(slaveIDs[lineInd].Text);
            var values = ModbusReader.ReadHoldingRegisters(slaveId, startAddress, currentOvenModel.NumberOfPoints);
            if (values is null) {
                //checkboxes[lineInd].Checked = false;
                return false;
            }

            for (int i = 0; i < 8; i++) {

                ushort offset = currentOvenModel.RegisterOffsets[i];

                var cellVal = values[offset].ToString();
                lineTextBoxes[i].Text = cellVal;
                string message = $"SlaveID: {slaveId}, Offset: {offset}, Value: {cellVal}";
                //glLogger.Log(message);
                //Application.DoEvents();
                //Thread.Sleep(20);
            }
            return true;
        } catch (Exception ex) {
            //glLogger.Log("fillTheLine: " + ex.Message);
            //checkboxes[lineInd].Checked = false;
            return false;
        }

    }
    /// <summary>
    /// Sets all checkboxes ON
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void buttonSetAllCheckboxesON_Click(object sender, EventArgs e) {
        checkboxes.ForEach(x => x.Checked = true);
    }

    private void FormMain_FormClosing(object sender, FormClosingEventArgs e) {
        SaveSettings();
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
}