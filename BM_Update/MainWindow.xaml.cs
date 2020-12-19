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
        int MajorU = 0;
        int MinorU = 0;
        int PatchU = 0;

        public MainWindow()
        {
            InitializeComponent();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));

            if (File.Exists(exe + "\\BMBF Manager.exe"))
            {
                File.Delete(exe + "\\BMBF Manager.exe");
            }

            if (File.Exists(exe + "\\Newtonsoft.Json.dll"))
            {
                File.Delete(exe + "\\Newtonsoft.Json.dll");
            }

            if (File.Exists(exe + "\\Newtonsoft.Json.xml"))
            {
                File.Delete(exe + "\\Newtonsoft.Json.xml");
            }

            if (File.Exists(exe + "\\Black_Background.png")) File.Delete(exe + "\\Black_Background.png");
            if (File.Exists(exe + "\\Dark_Grey_Background.png")) File.Delete(exe + "\\Dark_Grey_Background.png");
            if (File.Exists(exe + "\\Light_Grey_Background.png")) File.Delete(exe + "\\Light_Grey_Background.png");
            if (File.Exists(exe + "\\White_Background.png")) File.Delete(exe + "\\White_Background.png");
            if (File.Exists(exe + "\\Microsoft.WindowsAPICodePack.dll")) File.Delete(exe + "\\Microsoft.WindowsAPICodePack.dll");
            if (File.Exists(exe + "\\Microsoft.WindowsAPICodePack.Shell.dll")) File.Delete(exe + "\\Microsoft.WindowsAPICodePack.Shell.dll");

            if (!Directory.Exists(exe + "\\tmp"))
            {
                Directory.CreateDirectory(exe + "\\tmp");
            }

            JSONNode Update = JSON.Parse("{}");

            using (WebClient client = new WebClient())
            {
                Update = JSON.Parse(client.DownloadString("https://raw.githubusercontent.com/ComputerElite/BM/main/update.json"));
            }

            MajorU = Update["Updates"][0]["Major"];
            MinorU = Update["Updates"][0]["Minor"];
            PatchU = Update["Updates"][0]["Patch"];

            txtbox.Text = "Downloading BMBF Manager";

            using (WebClient client = new WebClient())
            {
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                client.DownloadFileAsync(new Uri(Update["Updates"][0]["Download"]), exe + "\\tmp\\BM_V_" + MajorU + "_" + MinorU + "_" + PatchU + ".zip");
            }
        }

        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            ZipFile.ExtractToDirectory(exe + "\\tmp\\BM_V_" + MajorU + "_" + MinorU + "_" + PatchU + ".zip", exe);
            File.Delete(exe + "\\tmp\\BM_V_" + MajorU + "_" + MinorU + "_" + PatchU + ".zip");
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
