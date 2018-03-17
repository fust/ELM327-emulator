// Decompiled with JetBrains decompiler
// Type: ELMSIM.Form1
// Assembly: ELMSIM, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3E232871-3D57-4271-9442-48EC95338D04
// Assembly location: C:\Users\Fust\Documents\Visual Studio 2013\Projects\ELMSIM\ELMSIM\bin\Debug\ELMSIM.exe

using ELMSIM.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO.Ports;
using System.Management;
using System.Threading;
using System.Windows.Forms;

namespace ELMSIM
{
  public class Form1 : Form
  {
    private IContainer components = (IContainer) null;
    private const int WM_DEVICECHANGE = 537;
    private const int DBT_DEVICEARRIVAL = 32768;
    private const int DBT_DEVICEREMOVALCOMPLETE = 32772;
    private SerialPort ComPort;
    private ELM emulator;
    private ComboBox cboPorts;
    private ComboBox cboBaudRate;
    private Label label1;
    private Label label2;
    private Button btnConnect;
    private Button btnClose;
    private TextBox tbReceived;
    private TrackBar tbRpm;
    private TrackBar tbEct;
    private Label label3;
    private Label label4;
    private Label label5;
    private TrackBar tbSpeed;
    private Label label6;
    private TrackBar tbConsumption;
    private BackgroundWorker backgroundWorker1;

    public Form1()
    {
      this.InitializeComponent();
      this.ComPort = new SerialPort();
      this.ComPort.DataReceived += new SerialDataReceivedEventHandler(this.comReveive);
      try
      {
        this.cboPorts.SelectedIndex = this.cboPorts.FindString(Settings.Default["LastCOM"].ToString());
        this.cboBaudRate.SelectedIndex = this.cboBaudRate.FindString(Settings.Default["LastBaud"].ToString());
      }
      catch (Exception ex)
      {
        this.cboBaudRate.SelectedIndex = this.cboBaudRate.FindString("38400");
      }
      this.SearchComPorts();
      this.backgroundWorker1.RunWorkerAsync();
    }

    private void watcher_EventArrived(object sender, EventArrivedEventArgs e)
    {
      if (this.cboPorts.InvokeRequired)
        this.Invoke((Delegate) new Form1.SearchComPortsCallback(this.SearchComPorts), new object[0]);
      else
        this.SearchComPorts();
    }

    protected void SearchComPorts()
    {
      string[] portNames = SerialPort.GetPortNames();
      int index = -1;
      string str1 = (string) null;
      this.cboPorts.Items.Clear();
      string selectedText = this.cboPorts.SelectedText;
      do
      {
        ++index;
        this.cboPorts.Items.Add((object) portNames[index]);
      }
      while (!(portNames[index] == str1) && index != portNames.GetUpperBound(0));
      Array.Sort<string>(portNames);
      if (index == portNames.GetUpperBound(0))
      {
        string str2 = portNames[0];
      }
      this.cboPorts.Text = portNames[0];
      if (!this.cboPorts.Items.Contains((object) selectedText))
        return;
      this.cboPorts.SelectedText = selectedText;
    }

    private void btnConnect_Click(object sender, EventArgs e)
    {
      this.ComPort.PortName = Convert.ToString(this.cboPorts.Text);
      this.ComPort.BaudRate = Convert.ToInt32(this.cboBaudRate.Text);
      this.ComPort.DataBits = 8;
      this.ComPort.StopBits = StopBits.One;
      this.ComPort.Parity = Parity.None;
      this.ComPort.NewLine = "\r";
      try
      {
        this.ComPort.Open();
        if (!this.ComPort.IsOpen)
          return;
        this.tbReceived.Clear();
        this.emulator = new ELM(this.ComPort, this);
        this.btnConnect.Enabled = false;
        this.btnClose.Enabled = true;
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show("Could not open port. Message: " + ex.Message);
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      if (this.ComPort.IsOpen)
      {
        this.ComPort.Close();
        this.emulator = (ELM) null;
      }
      this.btnClose.Enabled = false;
      this.btnConnect.Enabled = true;
    }

    private void comReveive(object sender, SerialDataReceivedEventArgs e)
    {
      string empty = string.Empty;
      string str;
      try
      {
        str = this.ComPort.ReadLine();
        this.ComPort.DiscardInBuffer();
      }
      catch (Exception ex)
      {
        return;
      }
      if (!(str != string.Empty))
        return;
      this.BeginInvoke((Delegate) new Form1.SetTextCallback(this.SetText), (object) str);
    }

    private void SetText(string text)
    {
      this.tbReceived.AppendText(">>" + text + "\r\n");
      try
      {
        this.tbReceived.AppendText("<<" + this.emulator.Handle(text) + "\r\n");
      }
      catch (NullReferenceException ex)
      {
      }
    }

    public int getRpm()
    {
      return (this.tbRpm.Value + new Random().Next(1, 20)) * 4;
    }

    public int getEct()
    {
      return this.tbEct.Value + 40;
    }

    public int getSpeed()
    {
      return this.tbSpeed.Value;
    }

    public int getConsumption()
    {
      return this.tbConsumption.Value * 160;
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
      Settings.Default["LastCOM"] = this.cboPorts.SelectedValue;
      Settings.Default["LastBaud"] = this.cboBaudRate.SelectedValue;
      Settings.Default.Save();
      if (!this.ComPort.IsOpen)
        return;
      this.ComPort.Close();
    }

    private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
    {
      ManagementEventWatcher managementEventWatcher = new ManagementEventWatcher((EventQuery) new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2 or EventType = 3"));
      managementEventWatcher.EventArrived += new EventArrivedEventHandler(this.watcher_EventArrived);
      managementEventWatcher.Start();
      Thread.Sleep(5000);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.cboPorts = new ComboBox();
      this.cboBaudRate = new ComboBox();
      this.label1 = new Label();
      this.label2 = new Label();
      this.btnConnect = new Button();
      this.btnClose = new Button();
      this.tbReceived = new TextBox();
      this.tbRpm = new TrackBar();
      this.tbEct = new TrackBar();
      this.label3 = new Label();
      this.label4 = new Label();
      this.label5 = new Label();
      this.tbSpeed = new TrackBar();
      this.label6 = new Label();
      this.tbConsumption = new TrackBar();
      this.backgroundWorker1 = new BackgroundWorker();
      this.tbRpm.BeginInit();
      this.tbEct.BeginInit();
      this.tbSpeed.BeginInit();
      this.tbConsumption.BeginInit();
      this.SuspendLayout();
      this.cboPorts.FormattingEnabled = true;
      this.cboPorts.Location = new Point(79, 13);
      this.cboPorts.Name = "cboPorts";
      this.cboPorts.Size = new Size(121, 21);
      this.cboPorts.TabIndex = 0;
      this.cboBaudRate.FormattingEnabled = true;
      this.cboBaudRate.Items.AddRange(new object[10]
      {
        (object) "300",
        (object) "600",
        (object) "1200",
        (object) "2400",
        (object) "9600",
        (object) "14400",
        (object) "19200",
        (object) "38400",
        (object) "57600",
        (object) "115200"
      });
      this.cboBaudRate.Location = new Point(79, 41);
      this.cboBaudRate.Name = "cboBaudRate";
      this.cboBaudRate.Size = new Size(121, 21);
      this.cboBaudRate.TabIndex = 1;
      this.label1.AutoSize = true;
      this.label1.Location = new Point(13, 16);
      this.label1.Name = "label1";
      this.label1.Size = new Size(26, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "Port";
      this.label2.AutoSize = true;
      this.label2.Location = new Point(13, 44);
      this.label2.Name = "label2";
      this.label2.Size = new Size(53, 13);
      this.label2.TabIndex = 3;
      this.label2.Text = "Baud rate";
      this.btnConnect.Location = new Point(79, 69);
      this.btnConnect.Name = "btnConnect";
      this.btnConnect.Size = new Size(55, 23);
      this.btnConnect.TabIndex = 4;
      this.btnConnect.Text = "Connect";
      this.btnConnect.UseVisualStyleBackColor = true;
      this.btnConnect.Click += new EventHandler(this.btnConnect_Click);
      this.btnClose.Enabled = false;
      this.btnClose.Location = new Point(140, 69);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new Size(55, 23);
      this.btnClose.TabIndex = 5;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new EventHandler(this.btnClose_Click);
      this.tbReceived.Location = new Point(12, 372);
      this.tbReceived.Multiline = true;
      this.tbReceived.Name = "tbReceived";
      this.tbReceived.ScrollBars = ScrollBars.Vertical;
      this.tbReceived.Size = new Size(260, 117);
      this.tbReceived.TabIndex = 6;
      this.tbRpm.Location = new Point(12, 149);
      this.tbRpm.Maximum = 7000;
      this.tbRpm.Name = "tbRpm";
      this.tbRpm.Size = new Size(260, 45);
      this.tbRpm.TabIndex = 7;
      this.tbRpm.Value = 950;
      this.tbEct.Location = new Point(12, 205);
      this.tbEct.Maximum = 110;
      this.tbEct.Name = "tbEct";
      this.tbEct.Size = new Size(260, 45);
      this.tbEct.TabIndex = 8;
      this.label3.AutoSize = true;
      this.label3.Location = new Point(12, 133);
      this.label3.Name = "label3";
      this.label3.Size = new Size(31, 13);
      this.label3.TabIndex = 9;
      this.label3.Text = "RPM";
      this.label4.AutoSize = true;
      this.label4.Location = new Point(12, 189);
      this.label4.Name = "label4";
      this.label4.Size = new Size(28, 13);
      this.label4.TabIndex = 10;
      this.label4.Text = "ECT";
      this.label5.AutoSize = true;
      this.label5.Location = new Point(12, 243);
      this.label5.Name = "label5";
      this.label5.Size = new Size(74, 13);
      this.label5.TabIndex = 12;
      this.label5.Text = "Speed (KM/h)";
      this.tbSpeed.Location = new Point(12, 259);
      this.tbSpeed.Maximum = 190;
      this.tbSpeed.Name = "tbSpeed";
      this.tbSpeed.Size = new Size(260, 45);
      this.tbSpeed.TabIndex = 11;
      this.label6.AutoSize = true;
      this.label6.Location = new Point(12, 298);
      this.label6.Name = "label6";
      this.label6.Size = new Size(94, 13);
      this.label6.TabIndex = 14;
      this.label6.Text = "Consumption (L/h)";
      this.tbConsumption.Location = new Point(12, 314);
      this.tbConsumption.Maximum = 20;
      this.tbConsumption.Name = "tbConsumption";
      this.tbConsumption.Size = new Size(260, 45);
      this.tbConsumption.TabIndex = 13;
      this.backgroundWorker1.DoWork += new DoWorkEventHandler(this.backgroundWorker1_DoWork);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(284, 501);
      this.Controls.Add((Control) this.label6);
      this.Controls.Add((Control) this.tbConsumption);
      this.Controls.Add((Control) this.label5);
      this.Controls.Add((Control) this.tbSpeed);
      this.Controls.Add((Control) this.label4);
      this.Controls.Add((Control) this.label3);
      this.Controls.Add((Control) this.tbEct);
      this.Controls.Add((Control) this.tbRpm);
      this.Controls.Add((Control) this.tbReceived);
      this.Controls.Add((Control) this.btnClose);
      this.Controls.Add((Control) this.btnConnect);
      this.Controls.Add((Control) this.label2);
      this.Controls.Add((Control) this.label1);
      this.Controls.Add((Control) this.cboBaudRate);
      this.Controls.Add((Control) this.cboPorts);
      this.Name = "Form1";
      this.Text = "Form1";
      this.FormClosing += new FormClosingEventHandler(this.Form1_FormClosing);
      this.tbRpm.EndInit();
      this.tbEct.EndInit();
      this.tbSpeed.EndInit();
      this.tbConsumption.EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    private delegate void SetTextCallback(string text);

    private delegate void SearchComPortsCallback();
  }
}
