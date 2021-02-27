using SimpleJSON;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BM_Update
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        String exe = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 1);

        public MainWindow()
        {
            InitializeComponent();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));

            if (!Directory.Exists(exe + "\\tmp"))
            {
                Directory.CreateDirectory(exe + "\\tmp");
            }

            JSONNode Update = JSON.Parse("{}");

            using (WebClient client = new WebClient())
            {
                Update = JSON.Parse(client.DownloadString("https://raw.githubusercontent.com/ComputerElite/BM/main/update.json"));
            }

            txtbox.Text = "Downloading BMBF Manager";

            using (WebClient client = new WebClient())
            {
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                client.DownloadFileAsync(new Uri(Update["Updates"][0]["Download"]), exe + "\\tmp\\BM.zip");
            }
        }

        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            using (ZipArchive archive = ZipFile.OpenRead(exe + "\\tmp\\BM.zip"))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    String name = entry.FullName;
                    if (name.EndsWith("/")) continue;
                    if (name.Contains("/")) Directory.CreateDirectory(exe + "\\" + System.IO.Path.GetDirectoryName(name));
                    entry.ExtractToFile(exe + "\\" + entry.FullName, true);
                }
            }
            File.Delete(exe + "\\tmp\\BM.zip");
            try
            {
                Process.Start(exe + "\\BMBF Manager.exe");
                Process.GetCurrentProcess().Kill();
            }
            catch
            {
            }
        }

        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;
            Progress.Value = int.Parse(Math.Truncate(percentage).ToString());
        }
    }
}
