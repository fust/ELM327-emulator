// Decompiled with JetBrains decompiler
// Type: ELMSIM.Properties.Settings
// Assembly: ELMSIM, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3E232871-3D57-4271-9442-48EC95338D04
// Assembly location: C:\Users\Fust\Documents\Visual Studio 2013\Projects\ELMSIM\ELMSIM\bin\Debug\ELMSIM.exe

using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ELMSIM.Properties
{
  [CompilerGenerated]
  [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
  internal sealed class Settings : ApplicationSettingsBase
  {
    private static Settings defaultInstance = (Settings) SettingsBase.Synchronized((SettingsBase) new Settings());

    public static Settings Default
    {
      get
      {
        Settings defaultInstance = Settings.defaultInstance;
        return defaultInstance;
      }
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("COM5")]
    public string LastCOM
    {
      get
      {
        return (string) this["LastCOM"];
      }
      set
      {
        this["LastCOM"] = (object) value;
      }
    }

    [DefaultSettingValue("38400")]
    [DebuggerNonUserCode]
    [UserScopedSetting]
    public string LastBaud
    {
      get
      {
        return (string) this["LastBaud"];
      }
      set
      {
        this["LastBaud"] = (object) value;
      }
    }
  }
}
