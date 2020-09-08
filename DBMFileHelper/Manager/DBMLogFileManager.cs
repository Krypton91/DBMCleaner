using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Forms;
using DBMReportManager.Constants;
using DBMReportManager.Enum;
using DBMReportManager.Helper;

namespace DBMReportManager.Manager
{
  public class DBMLogFileManager
  {
    private static DBMLogFileManager _instance = null;
    public static DBMLogFileManager Instance
    {
      get
      {
        if(_instance == null)
        {
          _instance = new DBMLogFileManager ();
        }
        return _instance;
      }
    }


    #region FileSystem-Operations
    public List<string> DetectLogs (string dayzPath)
    {
      List<string> logs = new List<string>();

      long fileSize = 0L;

      var extensionCollection = System.Enum.GetNames(typeof(DayZLogFileExt));

      for (int i = 0; i < extensionCollection.Count (); i++)
      {
        logs.AddRange (Directory.GetFiles (dayzPath, $"*.{extensionCollection[i]}"));
      }

      for (int j = 0; j < logs.Count; j++)
      {
        fileSize += new FileInfo (logs[j]).Length;
      }

      MessageBox.Show ($"Found: {logs.Count} files with a size of: {DBMUtils.BytesToString (fileSize)} !", AppConstants.WindowCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

      return logs;
    }

    public void SaveLogs (string zipPath, ZipArchive zip, string log)
    {
       var fileExtension = Path.GetExtension (log);
       try
       {
            if (fileExtension == ".log" || fileExtension == ".mdmp")
            {
               zip.CreateEntryFromFile(log, Path.GetFileName(log));
            }
       }
       catch (Exception ex)
       {
          MessageBox.Show(ex.Message);
       }
    }

    public void DeleteLogs (Form frm, List<string> logs)
    {
      string msg = string.Empty;

      for (int i = 0; i < logs.Count; i++)
      {
        try
        {
          File.Delete (logs[i]);

          if(i+1 == logs.Count)
          {
            msg = "All DayZ Log-Files has been deleted !";
          }
        }
        catch (Exception ex)
        {
          msg = $"{ex.Message} ?";
        }
      }

      if(logs.Count == 0)
      {
        msg = "No Logs to delete !";
      }
    }
    #endregion

    #region FileStream-Operations
    public void SendWebReq (string zipPath)
    {
      try
      {
        if (DBMUtils.IsZipFileGenerated (zipPath) == true)
        {
          System.Net.WebClient Client = new System.Net.WebClient();
          Client.Headers.Add ("Content-Type", "binary/octet-stream");

          byte[] result = Client.UploadFile("https://report.deutschebohrmaschine.de/upload.php", "POST", zipPath);

          string response = System.Text.Encoding.UTF8.GetString(result, 0, result.Length);
          MessageBox.Show (response);
        }
        else
        {
          MessageBox.Show ("No Zip-Archive has been found!");
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show (ex.Message);
      }
    }
    #endregion

  }
}
