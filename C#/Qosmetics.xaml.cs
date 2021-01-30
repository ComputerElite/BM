using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Qosmetics;
using SimpleJSON;

namespace BMBF_Manager
{
    /// <summary>
    /// Interaktionslogik für Qosmetics.xaml
    /// </summary>
    public partial class Qosmetics : Window
    {
        Boolean draggable = true;
        String exe = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 1);

        List<QosmeticsObject> downloadqueue = new List<QosmeticsObject>();
        bool Running = false;
        int C = 0;
        QosmeticsJSON qj = new QosmeticsJSON();

        public Qosmetics()
        {
            InitializeComponent();
            Quest.Text = MainWindow.IP;
            DownloadLable.Text = "All finished";
            GetQosmetics();
            if (MainWindow.CustomImage)
            {
                ImageBrush uniformBrush = new ImageBrush();
                uniformBrush.ImageSource = new BitmapImage(new Uri(MainWindow.CustomImageSource, UriKind.Absolute));
                uniformBrush.Stretch = Stretch.UniformToFill;
                this.Background = uniformBrush;
            }
            else
            {
                ImageBrush uniformBrush = new ImageBrush();
                uniformBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Qosmetics2.png", UriKind.Absolute));
                uniformBrush.Stretch = Stretch.UniformToFill;
                this.Background = uniformBrush;
            }
            if (!MainWindow.QosmeticsWarningShown)
            {
                MessageBox.Show("Note: All Qosmetics got added automatically to this program. Not every Qosmetics is present here and you may see the wrong name. Check the Qosmetics Discord Server to get all available Qosmetics (https://discord.gg/qosmetics).\n\nFor Qosmetics to work the Qosmetics mod is needed. I'll check if it's installed every time all downloads are finished and if it isn't installed install it for you.", "BMBF Manager - Qosmetics", MessageBoxButton.OK, MessageBoxImage.Information);
                MainWindow.QosmeticsWarningShown = true;
            }
        }

        public bool CheckQosmeticsInstalled()
        {
            try
            {
                WebClient c = new WebClient();
                JSONNode BMBF = JSON.Parse(c.DownloadString("http://" + MainWindow.IP + ":50000/host/beatsaber/config"));
                Boolean Installed = false;
                foreach (JSONNode mod in BMBF["Config"]["Mods"])
                {
                    if (mod["ID"].ToString().ToLower().Replace("\"", "") == "qosmetics" && mod["Status"] == "Installed")
                    {
                        Installed = true;
                        break;
                    }
                }
                if (!Installed)
                {
                    txtbox.AppendText("\n\nI'll install Qosmetics for you");
                    txtbox.ScrollToEnd();
                    Support s = new Support();
                    s.Show();
                    s.StartSupport("bm://mods/install/Qosmetics");
                    MessageBox.Show("Please start Beat Saber and check if it works. Then press the install button again.", "BMBF Manager - Install Qosmetics", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }
            catch
            {
                if (!MainWindow.QosmeticsInstalled)
                {
                    MessageBoxResult result = MessageBox.Show("Do you have the Qosmetics mod installed;", "BMBF Manager - Install Qosmetics", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    switch (result)
                    {
                        case MessageBoxResult.No:
                            txtbox.AppendText("\n\nI'll install Qosmetics for you");
                            txtbox.ScrollToEnd();
                            Support s = new Support();
                            s.Show();
                            s.StartSupport("bm://mods/install/Qosmetics");
                            MessageBox.Show("Please start Beat Saber and check if it works. Then press the install button again.", "BMBF Manager - Install Qosmetics", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return false;
                    }
                }
            }
            return true;
        }

        public void GetQosmetics()
        {
            WebClient c = new WebClient();
            String tmp = "";
            try
            {
                tmp = c.DownloadString("https://raw.githubusercontent.com/ComputerElite/BM/main/Qosmetics.json");
            }
            catch { return; }
            if (tmp != "")
            {
                qj = JsonSerializer.Deserialize<QosmeticsJSON>(tmp);
            }
            qj.SetMixedSabers();
            AddToLists(qj);
        }

        private void AddToLists(QosmeticsJSON qj)
        {
            QSaberList.Items.Clear();
            QWallList.Items.Clear();
            QBloqList.Items.Clear();
            foreach (QosmeticsObject o in qj.AllQSabers)
            {
                QSaberList.Items.Add(new QosmeticsListObject { Name = o.name, Creator = o.author });
            }
            foreach (QosmeticsObject o in qj.qWalls)
            {
                QWallList.Items.Add(new QosmeticsListObject { Name = o.name, Creator = o.author });
            }
            foreach (QosmeticsObject o in qj.qBloqs)
            {
                QBloqList.Items.Add(new QosmeticsListObject { Name = o.name, Creator = o.author });
            }
        }

        private void Drag(object sender, RoutedEventArgs e)
        {
            bool mouseIsDown = System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed;


            if (mouseIsDown)
            {
                if (draggable)
                {
                    this.DragMove();
                }

            }

        }

        public void noDrag(object sender, MouseEventArgs e)
        {
            draggable = false;
        }

        public void doDrag(object sender, MouseEventArgs e)
        {
            draggable = true;
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            CheckIP();
            MainWindow.IP = Quest.Text;
            this.Close();
        }

        private void Mini(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ClearText(object sender, RoutedEventArgs e)
        {
            if (Quest.Text == "Quest IP")
            {
                Quest.Text = "";
            }

        }

        private void QuestIPCheck(object sender, RoutedEventArgs e)
        {
            if (Quest.Text == "")
            {
                Quest.Text = "Quest IP";
            }
        }

        public Boolean CheckIP()
        {
            getQuestIP();
            if (MainWindow.IP == "Quest IP")
            {
                return false;
            }
            MainWindow.IP = MainWindow.IP.Replace(":5000000", "");
            MainWindow.IP = MainWindow.IP.Replace(":500000", "");
            MainWindow.IP = MainWindow.IP.Replace(":50000", "");
            MainWindow.IP = MainWindow.IP.Replace(":5000", "");
            MainWindow.IP = MainWindow.IP.Replace(":500", "");
            MainWindow.IP = MainWindow.IP.Replace(":50", "");
            MainWindow.IP = MainWindow.IP.Replace(":5", "");
            MainWindow.IP = MainWindow.IP.Replace(":", "");
            MainWindow.IP = MainWindow.IP.Replace("/", "");
            MainWindow.IP = MainWindow.IP.Replace("https", "");
            MainWindow.IP = MainWindow.IP.Replace("http", "");
            MainWindow.IP = MainWindow.IP.Replace("Http", "");
            MainWindow.IP = MainWindow.IP.Replace("Https", "");

            int count = MainWindow.IP.Split('.').Count();
            if (count != 4)
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                {
                    Quest.Text = MainWindow.IP;
                }));
                return false;
            }
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                Quest.Text = MainWindow.IP;
            }));
            return true;
        }

        public void getQuestIP()
        {
            MainWindow.IP = Quest.Text;
            return;
        }

        public Boolean adb(String Argument)
        {
            String User = System.Environment.GetEnvironmentVariable("USERPROFILE");

            foreach (String ADB in MainWindow.ADBPaths)
            {
                ProcessStartInfo s = new ProcessStartInfo();
                s.CreateNoWindow = true;
                s.UseShellExecute = false;
                s.FileName = ADB.Replace("User", User);
                s.WindowStyle = ProcessWindowStyle.Minimized;
                s.Arguments = Argument;
                s.RedirectStandardOutput = true;
                if (MainWindow.ShowADB)
                {
                    s.RedirectStandardOutput = false;
                    s.CreateNoWindow = false;
                }
                try
                {
                    // Start the process with the info we specified.
                    // Call WaitForExit and then the using statement will close.
                    using (Process exeProcess = Process.Start(s))
                    {
                        if (!MainWindow.ShowADB)
                        {
                            String IPS = exeProcess.StandardOutput.ReadToEnd();
                            exeProcess.WaitForExit();
                            if (IPS.Contains("no devices/emulators found"))
                            {
                                txtbox.AppendText(MainWindow.ADB110);
                                txtbox.ScrollToEnd();
                                return false;
                            }
                        }
                        else
                        {
                            exeProcess.WaitForExit();
                        }

                        return true;
                    }
                }
                catch
                {
                    continue;
                }
            }
            txtbox.AppendText(MainWindow.ADB100);
            txtbox.ScrollToEnd();
            return false;
        }

        private void GetQSaberPicture(object sender, SelectionChangedEventArgs e)
        {
            if (QSaberList.SelectedIndex < 0 || QSaberList.SelectedIndex > (qj.AllQSabers.Count - 1))
            {
                return;
            }
            BitmapImage i = new BitmapImage();
            i.BeginInit();
            i.UriSource = new Uri(qj.AllQSabers[QSaberList.SelectedIndex].imageURL, UriKind.Absolute);
            i.EndInit();
            QSaberPicture.Source = i;
        }

        private void ShowQSaberImage(object sender, RoutedEventArgs e)
        {
            if (QSaberList.SelectedIndex < 0 || QSaberList.SelectedIndex > (qj.AllQSabers.Count - 1))
            {
                return;
            }
            Process.Start(qj.AllQSabers[QSaberList.SelectedIndex].imageURL);
        }

        private void QSaberMessage(object sender, RoutedEventArgs e)
        {
            if (QSaberList.SelectedIndex < 0 || QSaberList.SelectedIndex > (qj.AllQSabers.Count - 1))
            {
                return;
            }

            MessageBox.Show("QSaber: " + qj.AllQSabers[QSaberList.SelectedIndex].name + "\nMessage Author: " + qj.AllQSabers[QSaberList.SelectedIndex].author + "\nMessage:\n\n" + qj.AllQSabers[QSaberList.SelectedIndex].orgmessage, "BMBF Manager - Qosmetics Installing");
        }

        private void AddQSaberQueue(object sender, RoutedEventArgs e)
        {
            if (QSaberList.SelectedIndex < 0 || QSaberList.SelectedIndex > (qj.AllQSabers.Count - 1))
            {
                return;
            }

            if (downloadqueue.Contains(qj.AllQSabers[QSaberList.SelectedIndex]))
            {
                txtbox.AppendText("\n" + qj.AllQSabers[QSaberList.SelectedIndex].name + " is already in the download queue");
                return;
            }
            downloadqueue.Add(qj.AllQSabers[QSaberList.SelectedIndex]);
            checkqueue();
        }

        private void GetQWallPicture(object sender, SelectionChangedEventArgs e)
        {
            if (QWallList.SelectedIndex < 0 || QWallList.SelectedIndex > (qj.qWalls.Count - 1))
            {
                return;
            }
            BitmapImage i = new BitmapImage();
            i.BeginInit();
            i.UriSource = new Uri(qj.qWalls[QWallList.SelectedIndex].imageURL, UriKind.Absolute);
            i.EndInit();
            QWallPicture.Source = i;
        }

        private void ShowQWallImage(object sender, RoutedEventArgs e)
        {
            if (QWallList.SelectedIndex < 0 || QWallList.SelectedIndex > (qj.qWalls.Count - 1))
            {
                return;
            }
            Process.Start(qj.qWalls[QWallList.SelectedIndex].imageURL);
        }

        private void QWallMessage(object sender, RoutedEventArgs e)
        {
            if (QWallList.SelectedIndex < 0 || QWallList.SelectedIndex > (qj.qWalls.Count - 1))
            {
                return;
            }

            MessageBox.Show("QWall: " + qj.qWalls[QWallList.SelectedIndex].name + "\nMessage Author: " + qj.qWalls[QWallList.SelectedIndex].author + "\nMessage:\n\n" + qj.qWalls[QWallList.SelectedIndex].orgmessage, "BMBF Manager - Qosmetics Installing");
        }

        private void AddQWallQueue(object sender, RoutedEventArgs e)
        {
            if (QWallList.SelectedIndex < 0 || QWallList.SelectedIndex > (qj.qWalls.Count - 1))
            {
                return;
            }

            if (downloadqueue.Contains(qj.qWalls[QWallList.SelectedIndex]))
            {
                txtbox.AppendText("\n" + qj.qWalls[QWallList.SelectedIndex].name + " is already in the download queue");
                return;
            }
            downloadqueue.Add(qj.qWalls[QWallList.SelectedIndex]);
            checkqueue();
        }

        private void GetQBloqPicture(object sender, SelectionChangedEventArgs e)
        {
            if (QBloqList.SelectedIndex < 0 || QBloqList.SelectedIndex > (qj.qBloqs.Count - 1))
            {
                return;
            }
            BitmapImage i = new BitmapImage();
            i.BeginInit();
            i.UriSource = new Uri(qj.qBloqs[QBloqList.SelectedIndex].imageURL, UriKind.Absolute);
            i.EndInit();
            QBloqPicture.Source = i;
        }

        private void ShowQBloqImage(object sender, RoutedEventArgs e)
        {
            if (QBloqList.SelectedIndex < 0 || QBloqList.SelectedIndex > (qj.qBloqs.Count - 1))
            {
                return;
            }
            Process.Start(qj.qBloqs[QBloqList.SelectedIndex].imageURL);
        }

        public void StartBMBF()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                adb("shell am start -n com.weloveoculus.BMBF/com.weloveoculus.BMBF.MainActivity");
            }));
        }

        private void QBloqMessage(object sender, RoutedEventArgs e)
        {
            if (QBloqList.SelectedIndex < 0 || QBloqList.SelectedIndex > (qj.qBloqs.Count - 1))
            {
                return;
            }

            MessageBox.Show("QBloq: " + qj.qBloqs[QBloqList.SelectedIndex].name + "\nMessage Author: " + qj.qBloqs[QBloqList.SelectedIndex].author + "\nMessage:\n\n" + qj.qBloqs[QBloqList.SelectedIndex].orgmessage, "BMBF Manager - Qosmetics Installing");
        }

        private void AddQBloqQueue(object sender, RoutedEventArgs e)
        {
            if (QBloqList.SelectedIndex < 0 || QBloqList.SelectedIndex > (qj.qBloqs.Count - 1))
            {
                return;
            }

            if (downloadqueue.Contains(qj.qBloqs[QBloqList.SelectedIndex]))
            {
                txtbox.AppendText("\n" + qj.qBloqs[QBloqList.SelectedIndex].name + " is already in the download queue");
                return;
            }
            downloadqueue.Add(qj.qBloqs[QBloqList.SelectedIndex]);
            checkqueue();
        }

        private void checkqueue()
        {
            if (downloadqueue.Count != 0)
            {
                InstallQosmetic();
            }
            else
            {
                DownloadLable.Text = "All finished";
                txtbox.AppendText("\n\nAll finished");
                CheckQosmeticsInstalled();
            }
        }

        private void InstallQosmetic()
        {
            if (!CheckIP())
            {
                txtbox.AppendText("\n\nChoose a valid IP.");
                txtbox.ScrollToEnd();
                return;
            }
            if (Running)
            {
                return;
            }
            Running = true;

            C = 0;
            while (File.Exists(exe + "\\tmp\\" + System.IO.Path.GetFileNameWithoutExtension(downloadqueue[0].downloadURL) + C + System.IO.Path.GetExtension(downloadqueue[0].downloadURL)))
            {
                C++;
            }

            WebClient c = new WebClient();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                txtbox.AppendText("\n\nDownloading " + downloadqueue[0].name + "\n");
                txtbox.ScrollToEnd();
            }));
            Uri uri = new Uri(downloadqueue[0].downloadURL);
            try
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                {
                    DownloadLable.Text = "Downloading " + downloadqueue[0].name;
                    c.DownloadFileCompleted += new AsyncCompletedEventHandler(finished_download);
                    c.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                    c.DownloadFileAsync(uri, exe + "\\tmp\\" + System.IO.Path.GetFileNameWithoutExtension(downloadqueue[0].downloadURL) + C + System.IO.Path.GetExtension(downloadqueue[0].downloadURL));
                }));
            }
            catch
            {
                txtbox.AppendText(MainWindow.BM200);
                txtbox.ScrollToEnd();
                Running = false;
                return;
            }
        }

        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;
            Progress.Value = int.Parse(Math.Truncate(percentage).ToString());
        }

        private void finished_download(object sender, AsyncCompletedEventArgs e)
        {
            StartBMBF();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                txtbox.AppendText("\nDownloaded " + downloadqueue[0].name + "\n");
                txtbox.ScrollToEnd();
            }));
            String file = exe + "\\tmp\\" + System.IO.Path.GetFileNameWithoutExtension(downloadqueue[0].downloadURL) + C + System.IO.Path.GetExtension(downloadqueue[0].downloadURL);
            if(!File.Exists(file))
            {
                txtbox.AppendText("\n\n" + downloadqueue[0].name + " couldn't get downloaded");
                txtbox.ScrollToEnd();
                Running = false;
                DownloadLable.Text = "All finished";
                Progress.Value = 0;
                downloadqueue.RemoveAt(0);
                checkqueue();
                return;
            }
            if (file.ToLower().EndsWith(".zip"))
            {
                txtbox.AppendText("\n\nExtracting Qosmetic from zip file");
                file = Unzip(file);
                JSONNode n = JSON.Parse(File.ReadAllText(file + "\\bmbfmod.json"));
                txtbox.AppendText("\nFound " + n["components"][0]["sourceFileName"]);
                file += "\\" + n["components"][0]["sourceFileName"];
            }
            upload(file);
        }

        public static string Unzip(String file)
        {
            ZipFile.ExtractToDirectory(file, System.IO.Path.GetDirectoryName(file) + "\\" + System.IO.Path.GetFileNameWithoutExtension(file));
            return System.IO.Path.GetDirectoryName(file) + "\\" + System.IO.Path.GetFileNameWithoutExtension(file);
        }

        public void upload(String path)
        {
            getQuestIP();

            TimeoutWebClient client = new TimeoutWebClient();

            txtbox.AppendText("\n\nUploading " + downloadqueue[0].name + " to BMBF");
            txtbox.ScrollToEnd();
            Uri uri = new Uri("http://" + MainWindow.IP + ":50000/host/beatsaber/upload?overwrite");
            try
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                {
                    DownloadLable.Text = "Uploading " + downloadqueue[0].name + " to BMBF";
                    client.UploadProgressChanged += new UploadProgressChangedEventHandler(client_uploadchanged);
                    client.UploadFileCompleted += new UploadFileCompletedEventHandler(finished_upload);
                    client.UploadFileAsync(uri, path);
                }));
            }
            catch
            {
                txtbox.AppendText(MainWindow.BMBF100);
                txtbox.ScrollToEnd();
            }
        }

        private void client_uploadchanged(object sender, UploadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesSent.ToString());
            double totalBytes = double.Parse(e.TotalBytesToSend.ToString());
            double percentage = bytesIn / totalBytes * 100;
            Progress.Value = int.Parse(Math.Truncate(percentage).ToString());
        }

        private void finished_upload(object sender, AsyncCompletedEventArgs e)
        {
            txtbox.AppendText("\n\n" + downloadqueue[0].name + " was uploaded to your Quest. Please enable your Qosmetic manually cia the BMBF Web Interface (Open BMBF in the main menu)");
            txtbox.ScrollToEnd();
            Running = false;
            DownloadLable.Text = "All finished";
            Progress.Value = 0;
            downloadqueue.RemoveAt(0);
            checkqueue();
        }

        public void Sync()
        {
            try
            {
                System.Threading.Thread.Sleep(2000);
                using (WebClient client = new WebClient())
                {
                    client.QueryString.Add("foo", "foo");
                    client.UploadValues("http://" + MainWindow.IP + ":50000/host/beatsaber/commitconfig", "POST", client.QueryString);
                }
            }
            catch
            {
                txtbox.AppendText(MainWindow.BMBF110);
                Running = false;
            }
        }
    }

    public class QosmeticsListObject
    {
        public string Name { get; set; }

        public string Creator { get; set; }
    }
}
