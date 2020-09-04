using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBMReportManager.Enum;

namespace DBMReportManager.Helper
{
  public class DBMUtils
  {
    public static string ConvertBytes (float Size, int R)
    {
      float num = Size / 1024f;
      if (num < 1f)
      {
        switch (R)
        {
          case 0:
            return string.Format ("{0:0.00} byte", Size);
          case 1:
            return string.Format ("{0:0.00} kb", Size);
          case 2:
            return string.Format ("{0:0.00} mb", Size);
          case 3:
            return string.Format ("{0:0.00} gb", Size);
          case 4:
            return string.Format ("{0:0.00} tb", Size);
        }
      }
      return ConvertBytes (num, ++R);
    }
    public static string BytesToString (float Size)
    {
      return ConvertBytes (Size, 0);
    }
    public static bool IsZipFileGenerated (string zipPath)
    {
        if (File.Exists (zipPath))
        {
          return true;
        }
        return false;
    }
  }
}
