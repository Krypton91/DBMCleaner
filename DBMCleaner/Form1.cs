using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBMCleaner
{
    public partial class Cleaner : Form
    {
        private List<string> logs = new List<string>();
        static string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\DayZ\\";
        static string zipPath = path + Environment.UserName + "`s Dayz Logs.zip";
        public static int x;
        public static int y;
        public bool m_isGenerated;
        bool hasSendet;
        public const string Prefix = "DBM Cleaner";
        public static System.Drawing.Point newpoint = new System.Drawing.Point();
        public Cleaner()
        {
            InitializeComponent();
        }
        bool IsZipFileGenerated() 
        {
            if (File.Exists(zipPath)) 
            {
                return true;
            }
            return false;
        }
        internal void SafeLogs()
        {
            if (File.Exists(zipPath)) 
            {
                File.Delete(zipPath);
            }
            var zip = ZipFile.Open(zipPath, ZipArchiveMode.Create);
            foreach (string fileName in logs)
            {
                if (fileName.Contains(".log") || fileName.Contains(".log") || fileName.Contains(".mdmp") && logs.Count < 30)
                {
                    zip.CreateEntryFromFile(fileName, Path.GetFileName(fileName), CompressionLevel.Optimal);
                }
            }
            zip.Dispose();
        }
        private void Cleaner_Load(object sender, EventArgs e)
        {
            DetectLogs();
        }
        internal List<string> fileList = new List<string>
        {
            "log",
            "rpt",
            "ADM",
            "mdmp",
        };
        internal string ConvertBytes(float Size, int R)
        {
            float num = Size / 1024f;
            if (num < 1f)
            {
                switch (R)
                {
                    case 0:
                        return string.Format("{0:0.00} byte", Size);
                    case 1:
                        return string.Format("{0:0.00} kb", Size);
                    case 2:
                        return string.Format("{0:0.00} mb", Size);
                    case 3:
                        return string.Format("{0:0.00} gb", Size);
                    case 4:
                        return string.Format("{0:0.00} tb", Size);
                }
            }
            return ConvertBytes(num, ++R);
        }
        internal string BytesToString(float Size)
        {
            return ConvertBytes(Size, 0);
        }
        internal async Task DetectLogs() 
        {
            long fileSize = 0L;
            foreach (string str in fileList)
            {
                logs.AddRange(Directory.GetFiles(path, "*." + str));
            }
            foreach (string fileName in logs)
            {
                fileSize += new FileInfo(fileName).Length;
            }
            await Task.Delay(1);
            label2.Text = string.Format("Found: {0} files with a size of: {1}.", logs.Count, BytesToString(fileSize));
        }
        internal void StartCleaning() 
        {
            foreach (string fileName in logs)
            {
                try
                {
                    new FileInfo(fileName).Delete();
                    DialogResult dialogResult = MessageBox.Show("All Dayz Log Files has been deleted!\nYou want to exit now ?", Prefix, MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        this.Close();
                    }
                    else if (dialogResult == DialogResult.No)
                    {

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            MessageBox.Show("No Logs to delete DBM Cleaner Closing now!", Prefix);
            this.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void label3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        
        internal async Task SendWebReq() 
        {
            try
            {
                if (IsZipFileGenerated() == true)
                {
                    System.Net.WebClient Client = new System.Net.WebClient();

                    Client.Headers.Add("Content-Type", "binary/octet-stream");

                    byte[] result = Client.UploadFile("https://report.deutschebohrmaschine.de/upload.php", "POST", zipPath);

                    string response = System.Text.Encoding.UTF8.GetString(result, 0, result.Length);
                    MessageBox.Show(response);
                }
                else 
                {
                    SafeLogs();
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
            await Task.Delay(1);
        }
        private void label4_Click(object sender, EventArgs e)
        {
            base.WindowState = FormWindowState.Minimized;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("WARNING \n Can i send your crash logs to Krypton?\n Please Accept this with yes\nIt will only send .log files in Dayz Folder!\nWhen you click no it will clean you PC!", Prefix, MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                SendLogsToKrypton();
                StartCleaning();
            }
            else if (dialogResult == DialogResult.No)
            {
                StartCleaning();
            }
        }

        private void Cleaner_MouseDown(object sender, MouseEventArgs e)
        {
            Cleaner.x = Control.MousePosition.X - base.Location.X;
            Cleaner.y = Control.MousePosition.Y - base.Location.Y;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Cleaner.newpoint = Control.MousePosition;
                Cleaner.newpoint.X -= Cleaner.x;
                Cleaner.newpoint.Y -= Cleaner.y;
                base.Location = Cleaner.newpoint;
            }
        }
        internal void SendLogsToKrypton() 
        {
            if (hasSendet) 
            {
                MessageBox.Show("Dont spam this you IP Adress is logged and you have been warned know! ;)");
            }
            if(IsZipFileGenerated() == true) 
            {
                SendWebReq();
            }
            else 
            {
                SafeLogs();
                SendLogsToKrypton();
            }
        }
     
        private void button1_Click_1(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("WARNING \n This Option Uploads your Logs!!\n Your Logs are keep private we use it to solve the client crashes!\nIt will only send .log and dump files in Dayz Folder!\nClick Yes to send the Logs!", Prefix, MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                SendLogsToKrypton();
            }
            else if (dialogResult == DialogResult.No)
            {
                
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Krypton91/DBMCleaner");
        }
    }
}
