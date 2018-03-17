// Decompiled with JetBrains decompiler
// Type: ELMSIM.Program
// Assembly: ELMSIM, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3E232871-3D57-4271-9442-48EC95338D04
// Assembly location: C:\Users\Fust\Documents\Visual Studio 2013\Projects\ELMSIM\ELMSIM\bin\Debug\ELMSIM.exe

using System;
using System.Windows.Forms;

namespace ELMSIM
{
  internal static class Program
  {
    [STAThread]
    private static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run((Form) new Form1());
    }
  }
}
