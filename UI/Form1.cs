using System.Windows.Forms;
using OvenSensorReader.Modbus;
using static System.Windows.Forms.DataFormats;
using OvenSensorReader.Log;


namespace OvenSensorReader;

public partial class FormMain : Form {

    public List<CheckBox> checkboxes;
    public List<TextBox[]> fields;
    public List<TextBox> slaveIDs;
    public List<ComboBox> ovenModelsFields;

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


        LoadSettings();

        ModbusReader.TIMEOUT = int.TryParse(textBoxTIMEOUT.Text, out ModbusReader.TIMEOUT) ? ModbusReader.TIMEOUT : 200;
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
        this.labelCOMPortConnectionResult.Text = ModbusReader.ClosePort() ? "Closing OK" : "Error closing";
    }



    /// <summary>
    /// Starts reading process.
    /// When checkbox "loop" is checked, repeats reading
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void buttonReadValuesOnce_Click(object sender, EventArgs e) {
        ModbusReader.TIMEOUT = int.TryParse(textBoxTIMEOUT.Text, out ModbusReader.TIMEOUT) ? ModbusReader.TIMEOUT : 200;
        int ind = 0;

        do {
            Application.DoEvents();
            if (ModbusReader._PORT is null || ModbusReader._PORT.IsOpen == false || checkboxes.Count(c => c.Checked) == 0) checkBoxLoop.Checked = false;
            Application.DoEvents();
            fillTheLine(ind, checkboxes, fields, slaveIDs, ovenModelsFields);
            ind++;
            if (ind >= 6 && checkBoxLoop.Checked) ind = 0;
        } while (ind < 6);
        buttonDisconnectCOM_Click(sender, e);
    }

    /// <summary>
    /// Gets register values from the OVEN and puts them in the textboxes
    /// </summary>
    /// <param name="lineInd"></param>
    /// <param name="checkboxes"></param>
    /// <param name="fields"></param>
    /// <param name="slaveIDs"></param>
    private void fillTheLine(int lineInd, List<CheckBox> checkboxes, List<TextBox[]> fields, List<TextBox> slaveIDs, List<ComboBox> ovenModelNames) {
        try {
            var ovenModelNumber = 1;
            if (checkboxes[lineInd].Checked == false) return;
            List<TextBox> lineTextBoxes = fields[lineInd].ToList();
            ushort startAddress = ModbusReader.GetOvenInputOffsets(ovenModelNumber).Last();
            byte slaveId = Convert.ToByte(slaveIDs[lineInd].Text);
            var values = ModbusReader.ReadHoldingRegisters(slaveId, startAddress, 45);
            if (values is null) {
                checkboxes[lineInd].Checked = false;
                return;
            }
            for (int i = 0; i < 8; i++) {

                ushort offset = ModbusReader.GetOvenInputOffsets(ovenModelNumber)[i];

                var cellVal = values[offset].ToString();
                lineTextBoxes[i].Text = cellVal;
                string message = $"SlaveID: {slaveId}, Offset: {offset}, Value: {cellVal}";
                glLogger.Log(message);
                Application.DoEvents();
                Thread.Sleep(40);
            }
        } catch (Exception ex) {
            glLogger.Log("fillTheLine: " + ex.Message);
            checkboxes[lineInd].Checked = false;
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

    private void buttonConnectReadDis_Click(object sender, EventArgs e) {
        buttonConnectToCOM_Click(sender, e);
        checkBoxLoop.Checked = false;
        buttonReadValuesOnce_Click(sender, e);
        buttonDisconnectCOM_Click(sender, e);
    }
}