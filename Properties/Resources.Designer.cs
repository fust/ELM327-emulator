// Decompiled with JetBrains decompiler
// Type: ELMSIM.Properties.Resources
// Assembly: ELMSIM, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3E232871-3D57-4271-9442-48EC95338D04
// Assembly location: C:\Users\Fust\Documents\Visual Studio 2013\Projects\ELMSIM\ELMSIM\bin\Debug\ELMSIM.exe

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace ELMSIM.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [CompilerGenerated]
  [DebuggerNonUserCode]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (object.ReferenceEquals((object) ELMSIM.Properties.Resources.resourceMan, (object) null))
          ELMSIM.Properties.Resources.resourceMan = new ResourceManager("ELMSIM.Properties.Resources", typeof (ELMSIM.Properties.Resources).Assembly);
        return ELMSIM.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get
      {
        return ELMSIM.Properties.Resources.resourceCulture;
      }
      set
      {
        ELMSIM.Properties.Resources.resourceCulture = value;
      }
    }
  }
}
