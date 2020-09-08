using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBMReportManager.Constants;
using DBMReportManager.Helper;
using DBMReportManager.Manager;

namespace DBMCleaner
{
  public partial class Cleaner : Form
  {
    //WIN-API Hooks
    public const int WM_NCLBUTTONDOWN = 0xA1;
    public const int HT_CAPTION = 0x2;

    [System.Runtime.InteropServices.DllImport ("user32.dll")]
    public static extern int SendMessage (IntPtr hWnd, int Msg, int wParam, int lParam);
    [System.Runtime.InteropServices.DllImport ("user32.dll")]
    public static extern bool ReleaseCapture ();
    //

    private BackgroundWorker backgroundWorker = new BackgroundWorker();
    private static string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\DayZ\\";
    private static string zipPath = path + Environment.UserName + "s Dayz Logs";

    public Cleaner()
    {
      InitializeComponent ();

      backgroundWorker.WorkerReportsProgress = true;
      backgroundWorker.DoWork += BackgroundWorker_DoWork;
      backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
      backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

      progressBar1.Minimum = 0;
      progressBar1.Maximum = 100;
    }

    private void BackgroundWorker_RunWorkerCompleted (object sender, RunWorkerCompletedEventArgs e)
    {
      var dialogResult = MessageBox.Show ("Do you want to close the application ?", AppConstants.WindowCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
      if (dialogResult == DialogResult.Yes)
      {
        this.Close ();
      }
    }

    private void ControlWorker()
    {
      if(backgroundWorker.IsBusy)
      {
        return;
      }
      else
      {
        backgroundWorker.RunWorkerAsync ();
      }
    }

    private void BackgroundWorker_DoWork (object sender, DoWorkEventArgs e)
    {
      var logFiles = DBMLogFileManager.Instance.DetectLogs (path);

      if (logFiles.Count > 0)
      {
        progressBar1.Invoke (new Action(() =>
        {
          progressBar1.Maximum = logFiles.Count - 1;
        }));

        zipPath = $"{zipPath}.zip";
        using (ZipArchive zip = ZipFile.Open (zipPath, ZipArchiveMode.Create))
        {
          for (int i = 0; i < logFiles.Count; i++)
          {
            backgroundWorker.ReportProgress (i);
            DBMLogFileManager.Instance.SaveLogs (zipPath, zip, logFiles[i]);
          }
        }

        if (MessageBox.Show ("Do you want to delete the Log-Files ?", AppConstants.WindowCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
          DBMLogFileManager.Instance.DeleteLogs (this, logFiles);
        }

        if (MessageBox.Show ("Do you want to transmit the Log-Files ?", AppConstants.WindowCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
          if (DBMUtils.IsZipFileGenerated (zipPath))
          {
            DBMLogFileManager.Instance.SendWebReq (zipPath);
                        File.Delete(zipPath);
          }
          else
          {
            MessageBox.Show ("There is no Zip-File to transmit !", AppConstants.WindowCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
          }
        }
      }
      else
      {
        MessageBox.Show ("No Log-Files found!", AppConstants.WindowCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
    }
    private void BackgroundWorker_ProgressChanged (object sender, ProgressChangedEventArgs e)
    {
      progressBar1.Value = e.ProgressPercentage;
    }

    private void pictureBox2_Click(object sender, EventArgs e)
    {
        Process.Start("https://github.com/Krypton91/DBMCleaner");
    }

    private void panel1_MouseDown (object sender, MouseEventArgs e)
    {
      DetectFormMovement (e);
    }

    private void label5_MouseDown (object sender, MouseEventArgs e)
    {
      DetectFormMovement (e);
    }

    private void DetectFormMovement(MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        ReleaseCapture ();
        SendMessage (Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
      }
    }

    private void button12_Click (object sender, EventArgs e)
    {
      ControlWorker ();
    }

    private void checkBox1_CheckedChanged (object sender, EventArgs e)
    {

    }

    private void label4_Click (object sender, EventArgs e)
    {
      this.WindowState = FormWindowState.Minimized;
    }

    private void label3_Click (object sender, EventArgs e)
    {
      this.Close ();
    }

        private void Cleaner_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }
    }
}
