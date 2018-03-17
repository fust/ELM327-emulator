// Decompiled with JetBrains decompiler
// Type: ELMSIM.ELM
// Assembly: ELMSIM, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3E232871-3D57-4271-9442-48EC95338D04
// Assembly location: C:\Users\Fust\Documents\Visual Studio 2013\Projects\ELMSIM\ELMSIM\bin\Debug\ELMSIM.exe

using System;
using System.IO.Ports;
using System.Threading;

namespace ELMSIM
{
  internal class ELM
  {
    protected bool echo = true;
    protected bool line = false;
    protected ELM.OBD_PROTOCOLS proto = ELM.OBD_PROTOCOLS.PROTO_AUTO;
    private const string ID = "ELM327 v1.5";
    private const string EMPTY = "";
    private const string AT_ATZ = "ATZ";
    private const string AT_ATE0 = "ATE0";
    private const string AT_ATE1 = "ATE1";
    private const string AT_ATL0 = "ATL0";
    private const string AT_ATL1 = "ATL1";
    private const string AT_ATH0 = "ATH0";
    private const string AT_ATH1 = "ATH1";
    private const string AT_ATI = "ATI";
    private const string AT_ATAT1 = "AT@1";
    private const string AT_ATAT2 = "AT@2";
    private const string AT_ATDP = "ATDP";
    private const string AT_SP = "SP";
    private SerialPort port;
    protected Form1 form;

    public ELM(SerialPort port, Form1 form)
    {
      if (!port.IsOpen)
        throw new Exception("Closed resources given");
      this.port = port;
      this.form = form;
      this.init();
    }

    protected void init()
    {
      this.echo = true;
      this.line = false;
      this.proto = ELM.OBD_PROTOCOLS.PROTO_AUTO;
    }

    public string Handle(string data)
    {
      string empty1 = string.Empty;
      string str1;
      if (data.IndexOf("AT") >= 0)
      {
        str1 = data;
        switch (data)
        {
          case "ATZ":
            this.init();
            str1 = "ELM327 v1.5";
            break;
          case "ATE0":
            this.echo = false;
            break;
          case "ATE1":
            this.echo = true;
            break;
          case "ATL0":
            this.line = false;
            break;
          case "ATL1":
            this.echo = true;
            break;
          case "ATH0":
          case "ATH1":
            break;
          case "ATS0":
          case "ATS1":
            break;
          case "ATI":
            str1 = "ELM327 v1.5";
            break;
          case "AT@1":
            str1 = "OBDII to RS232 Interpreter";
            break;
          case "AT@2":
            str1 = "?";
            break;
          case "ATDP":
            str1 = "ISO 14230-4 KWP";
            break;
          default:
            str1 = this.parseParameterCalls(data);
            break;
        }
      }
      else
      {
        int pid = 99;
        if (data.Length > 2)
        {
          try
          {
            pid = int.Parse(data.Substring(0, 2));
          }
          catch (Exception ex)
          {
            pid = 3;
          }
        }
        else if (data.Length == 2)
          pid = 3;
        string empty2 = string.Empty;
        string str2;
        try
        {
          str2 = data.Remove(0, 2);
        }
        catch (Exception ex)
        {
          return string.Empty;
        }
        string str3 = string.Empty;
        if (str2 == string.Empty && pid != 3)
          return empty1;
        if (str2 == string.Empty && pid == 3)
          str3 = data;
        if (pid != 3)
          str3 = str2.Substring(0, 2);
        str1 = this.processPID(pid, str3);
      }
      if (!this.line)
        ;
      if (str1 != "?" && str1 != "NOT SUPPORTED")
        str1 += "\r\nOK";
      string text = str1 + "\r\n>";
      Thread.Sleep(150);
      this.port.WriteLine(text);
      return text;
    }

    protected string parseParameterCalls(string data)
    {
      string str1 = "?";
      string str2 = data.Remove(0, 2);
      if (str2 == string.Empty || str2.Length < 2)
        return str1;
      string str3 = str2.Substring(0, 2);
      string proto = str2.Substring(2, str2.Length - 2);
      switch (str3)
      {
        case "SP":
          str1 = this.processProto(proto);
          break;
        case "SH":
          str1 = "OK";
          break;
      }
      return str1;
    }

    protected string processProto(string proto)
    {
      if (!Enum.IsDefined(typeof (ELM.OBD_PROTOCOLS), (object) int.Parse(proto)))
        return "BUS INIT: ...ERROR";
      this.proto = (ELM.OBD_PROTOCOLS) Enum.Parse(typeof (ELM.OBD_PROTOCOLS), proto);
      return "OK";
    }

    protected string processPID(int pid, string param)
    {
      if (pid > 3)
        return "?";
      int num = -1;
      if (param != string.Empty)
        num = Convert.ToInt32(param, 16);
      switch (pid)
      {
        case 1:
          switch (num)
          {
            case 0:
              return "41 00 983B8010";
            case 1:
              return "41 01 81076504";
            case 4:
              return "41 04 " + ((int) byte.MaxValue).ToString("X2");
            case 5:
              return "41 05 " + this.form.getEct().ToString("X2");
            case 12:
              return "41 0C " + this.form.getRpm().ToString("X4");
            case 13:
              return "41 0D " + this.form.getSpeed().ToString("X2");
            case 16:
              return "41 10 " + 5000.ToString("X2");
            case 32:
            case 64:
            case 96:
              return "NOT SUPPORTED";
            case 94:
              return "415E " + this.form.getConsumption().ToString("X2");
          }
          break;
        case 2:
          if (num == 2)
            return "4202 00000000";
          break;
        case 3:
          switch (num)
          {
            case 3:
              return "00000000";
            case 10:
              return this.ConvertStringToHex("BOSCH".PadRight(20, char.MinValue));
          }
          break;
        case 9:
          return "NOT SUPPORTED";
      }
      return "";
    }

    protected string ConvertStringToHex(string asciiString)
    {
      string str = "";
      foreach (int num in asciiString)
        str += string.Format("{0:x2}", (object) Convert.ToUInt32(num.ToString()));
      return str;
    }

    public enum OBD_PROTOCOLS
    {
      PROTO_AUTO = 0,
      PROTO_KWP2000_5KBPS = 4,
      PROTO_KWP2000_FAST = 5,
    }

    public enum OBD_PIDS
    {
      PID_ENGINE_LOAD = 4,
      PID_COOLANT_TEMP = 5,
      PID_SHORT_TERM_FUEL_TRIM_1 = 6,
      PID_LONG_TERM_FUEL_TRIM_1 = 7,
      PID_SHORT_TERM_FUEL_TRIM_2 = 8,
      PID_LONG_TERM_FUEL_TRIM_2 = 9,
      PID_FUEL_PRESSURE = 10, // 0x0000000A
      PID_INTAKE_MAP = 11, // 0x0000000B
      PID_RPM = 12, // 0x0000000C
      PID_SPEED = 13, // 0x0000000D
      PID_TIMING_ADVANCE = 14, // 0x0000000E
      PID_INTAKE_TEMP = 15, // 0x0000000F
      PID_MAF_FLOW = 16, // 0x00000010
      PID_THROTTLE = 17, // 0x00000011
      PID_AUX_INPUT = 30, // 0x0000001E
      PID_RUNTIME = 31, // 0x0000001F
      PID_DISTANCE_WITH_MIL = 33, // 0x00000021
      PID_COMMANDED_EGR = 44, // 0x0000002C
      PID_EGR_ERROR = 45, // 0x0000002D
      PID_COMMANDED_EVAPORATIVE_PURGE = 46, // 0x0000002E
      PID_FUEL_LEVEL = 47, // 0x0000002F
      PID_WARMS_UPS = 48, // 0x00000030
      PID_DISTANCE = 49, // 0x00000031
      PID_EVAP_SYS_VAPOR_PRESSURE = 50, // 0x00000032
      PID_BAROMETRIC = 51, // 0x00000033
      PID_CATALYST_TEMP_B1S1 = 60, // 0x0000003C
      PID_CATALYST_TEMP_B2S1 = 61, // 0x0000003D
      PID_CATALYST_TEMP_B1S2 = 62, // 0x0000003E
      PID_CATALYST_TEMP_B2S2 = 63, // 0x0000003F
      PID_CONTROL_MODULE_VOLTAGE = 66, // 0x00000042
      PID_ABSOLUTE_ENGINE_LOAD = 67, // 0x00000043
      PID_RELATIVE_THROTTLE_POS = 69, // 0x00000045
      PID_AMBIENT_TEMP = 70, // 0x00000046
      PID_ABSOLUTE_THROTTLE_POS_B = 71, // 0x00000047
      PID_ABSOLUTE_THROTTLE_POS_C = 72, // 0x00000048
      PID_ACC_PEDAL_POS_D = 73, // 0x00000049
      PID_ACC_PEDAL_POS_E = 74, // 0x0000004A
      PID_ACC_PEDAL_POS_F = 75, // 0x0000004B
      PID_COMMANDED_THROTTLE_ACTUATOR = 76, // 0x0000004C
      PID_TIME_WITH_MIL = 77, // 0x0000004D
      PID_TIME_SINCE_CODES_CLEARED = 78, // 0x0000004E
      PID_ETHANOL_FUEL = 82, // 0x00000052
      PID_FUEL_RAIL_PRESSURE = 89, // 0x00000059
      PID_HYBRID_BATTERY_PERCENTAGE = 91, // 0x0000005B
      PID_ENGINE_OIL_TEMP = 92, // 0x0000005C
      PID_FUEL_INJECTION_TIMING = 93, // 0x0000005D
      PID_ENGINE_FUEL_RATE = 94, // 0x0000005E
      PID_ENGINE_TORQUE_DEMANDED = 97, // 0x00000061
      PID_ENGINE_TORQUE_PERCENTAGE = 98, // 0x00000062
      PID_ENGINE_REF_TORQUE = 99, // 0x00000063
    }
  }
}
