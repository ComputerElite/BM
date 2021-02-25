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
using System.Text.RegularExpressions;
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
using ComputerUtils.RegxTemplates;
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
            ApplyLanguage();
            Quest.Text = MainWindow.config.IP;
            DownloadLable.Text = MainWindow.globalLanguage.global.allFinished;
            GetQosmetics();
            if (MainWindow.config.CustomImage)
            {
                ImageBrush uniformBrush = new ImageBrush();
                uniformBrush.ImageSource = new BitmapImage(new Uri(MainWindow.config.CustomImageSource, UriKind.Absolute));
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
            if (!MainWindow.config.QosmeticsWarningShown)
            {
                MessageBox.Show(MainWindow.globalLanguage.qosmetics.code.qosmeticsNote, "BMBF Manager - Qosmetics", MessageBoxButton.OK, MessageBoxImage.Information);
                MainWindow.config.QosmeticsWarningShown = true;
            }
            MainWindow.DCRPM.SetActivity(MainWindow.globalLanguage.dCRP.browsingQosmetics);
        }

        public void ApplyLanguage()
        {
            showImageButton1.Content = MainWindow.globalLanguage.qosmetics.UI.showImageButton;
            showImageButton2.Content = MainWindow.globalLanguage.qosmetics.UI.showImageButton;
            showImageButton3.Content = MainWindow.globalLanguage.qosmetics.UI.showImageButton;
            addSelectedQSaberButton.Content = MainWindow.globalLanguage.qosmetics.UI.addSelectedQSaberButton;
            addSelectedQWallButton.Content = MainWindow.globalLanguage.qosmetics.UI.addSelectedQWallButton;
            addSelectedQBloqButton.Content = MainWindow.globalLanguage.qosmetics.UI.addSelectedQBloqButton;
            showOriginalMessageQSaberButton.Content = MainWindow.globalLanguage.qosmetics.UI.showOriginalMessageQSaberButton;
            showOriginalMessageQWallButton.Content = MainWindow.globalLanguage.qosmetics.UI.showOriginalMessageQWallButton;
            showOriginalMessageQBloqButton.Content = MainWindow.globalLanguage.qosmetics.UI.showOriginalMessageQBloqButton;
            qSabersText.Text = MainWindow.globalLanguage.qosmetics.UI.qSabersText;
            qWallsText.Text = MainWindow.globalLanguage.qosmetics.UI.qWallsText;
            qBloqsText.Text = MainWindow.globalLanguage.qosmetics.UI.qBloqsText;
            ((GridView)QSaberList.View).Columns[0].Header = MainWindow.globalLanguage.qosmetics.UI.nameList;
            ((GridView)QSaberList.View).Columns[1].Header = MainWindow.globalLanguage.qosmetics.UI.creatorList;
            ((GridView)QWallList.View).Columns[0].Header = MainWindow.globalLanguage.qosmetics.UI.nameList;
            ((GridView)QWallList.View).Columns[1].Header = MainWindow.globalLanguage.qosmetics.UI.creatorList;
            ((GridView)QBloqList.View).Columns[0].Header = MainWindow.globalLanguage.qosmetics.UI.nameList;
            ((GridView)QBloqList.View).Columns[1].Header = MainWindow.globalLanguage.qosmetics.UI.creatorList;
            DownloadLable.Text = MainWindow.globalLanguage.global.allFinished;
        }

        public bool CheckQosmeticsInstalled()
        {
            try
            {
                WebClient c = new WebClient();
                JSONNode BMBF = JSON.Parse(c.DownloadString("http://" + MainWindow.config.IP + ":50000/host/beatsaber/config"));
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
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.qosmetics.code.illInstallQosmetics);
                    txtbox.ScrollToEnd();
                    Support s = new Support();
                    s.Show();
                    s.StartSupport("bm://mods/install/Qosmetics");
                    MessageBox.Show(MainWindow.globalLanguage.qosmetics.code.checkIfQosmeticsInstalled, "BMBF Manager - Install Qosmetics", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }
            catch
            {
                if (!MainWindow.config.QosmeticsInstalled)
                {
                    MessageBoxResult result = MessageBox.Show(MainWindow.globalLanguage.qosmetics.code.doYouHaveQomsietcsInstalled, "BMBF Manager - Install Qosmetics", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    switch (result)
                    {
                        case MessageBoxResult.No:
                            txtbox.AppendText("\n\n" + MainWindow.globalLanguage.qosmetics.code.illInstallQosmetics);
                            txtbox.ScrollToEnd();
                            Support s = new Support();
                            s.Show();
                            s.StartSupport("bm://mods/install/Qosmetics");
                            MessageBox.Show(MainWindow.globalLanguage.qosmetics.code.checkIfQosmeticsInstalled, "BMBF Manager - Install Qosmetics", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            MainWindow.iPUtils.CheckIP(Quest);
            this.Close();
        }

        private void Mini(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ClearText(object sender, RoutedEventArgs e)
        {
            if (Quest.Text == MainWindow.globalLanguage.global.defaultQuestIPText)
            {
                Quest.Text = "";
            }

        }

        private void QuestIPCheck(object sender, RoutedEventArgs e)
        {
            if (Quest.Text == "")
            {
                Quest.Text = MainWindow.globalLanguage.global.defaultQuestIPText;
            }
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

            MessageBox.Show(MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qosmetics.code.qosmeticsDescription, "QSaber", qj.AllQSabers[QSaberList.SelectedIndex].name, qj.AllQSabers[QSaberList.SelectedIndex].author, qj.AllQSabers[QSaberList.SelectedIndex].orgmessage), "BMBF Manager - Qosmetics Installing");
        }

        private void AddQSaberQueue(object sender, RoutedEventArgs e)
        {
            if (QSaberList.SelectedIndex < 0 || QSaberList.SelectedIndex > (qj.AllQSabers.Count - 1))
            {
                return;
            }

            if (downloadqueue.Contains(qj.AllQSabers[QSaberList.SelectedIndex]))
            {
                txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qosmetics.code.qosmeticsAlreadyInQueue, qj.AllQSabers[QSaberList.SelectedIndex].name));
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

            MessageBox.Show(MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qosmetics.code.qosmeticsDescription, "QWall", qj.qWalls[QWallList.SelectedIndex].name, qj.qWalls[QWallList.SelectedIndex].author, qj.qWalls[QWallList.SelectedIndex].orgmessage), "BMBF Manager - Qosmetics Installing");
        }

        private void AddQWallQueue(object sender, RoutedEventArgs e)
        {
            if (QWallList.SelectedIndex < 0 || QWallList.SelectedIndex > (qj.qWalls.Count - 1))
            {
                return;
            }

            if (downloadqueue.Contains(qj.qWalls[QWallList.SelectedIndex]))
            {
                txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qosmetics.code.qosmeticsAlreadyInQueue, qj.qWalls[QWallList.SelectedIndex].name));
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
                MainWindow.aDBI.adb("shell am start -n com.weloveoculus.BMBF/com.weloveoculus.BMBF.MainActivity", txtbox);
            }));
        }

        private void QBloqMessage(object sender, RoutedEventArgs e)
        {
            if (QBloqList.SelectedIndex < 0 || QBloqList.SelectedIndex > (qj.qBloqs.Count - 1))
            {
                return;
            }

            MessageBox.Show(MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qosmetics.code.qosmeticsDescription, "QBloq", qj.qBloqs[QBloqList.SelectedIndex].name, qj.qBloqs[QBloqList.SelectedIndex].author, qj.qBloqs[QBloqList.SelectedIndex].orgmessage), "BMBF Manager - Qosmetics Installing");
        }

        private void AddQBloqQueue(object sender, RoutedEventArgs e)
        {
            if (QBloqList.SelectedIndex < 0 || QBloqList.SelectedIndex > (qj.qBloqs.Count - 1))
            {
                return;
            }

            if (downloadqueue.Contains(qj.qBloqs[QBloqList.SelectedIndex]))
            {
                txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qosmetics.code.qosmeticsAlreadyInQueue, qj.qBloqs[QBloqList.SelectedIndex].name));
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
                DownloadLable.Text = MainWindow.globalLanguage.global.allFinished;
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.global.allFinished);
                CheckQosmeticsInstalled();
                MainWindow.DCRPM.SetActivity(MainWindow.globalLanguage.dCRP.browsingQosmetics);
            }
        }

        private void InstallQosmetic()
        {
            if (!MainWindow.iPUtils.CheckIP(Quest))
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.global.ipInvalid);
                txtbox.ScrollToEnd();
                return;
            }
            if (Running)
            {
                return;
            }
            Running = true;

            MainWindow.DCRPM.SetActivity(MainWindow.globalLanguage.dCRP.installingQosmetics);

            C = 0;
            while (File.Exists(exe + "\\tmp\\" + System.IO.Path.GetFileNameWithoutExtension(downloadqueue[0].downloadURL) + C + System.IO.Path.GetExtension(downloadqueue[0].downloadURL)))
            {
                C++;
            }

            WebClient c = new WebClient();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qosmetics.code.downloading, downloadqueue[0].name) + "\n");
                txtbox.ScrollToEnd();
            }));
            Uri uri = new Uri(downloadqueue[0].downloadURL);
            try
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                {
                    DownloadLable.Text = MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qosmetics.code.downloading, downloadqueue[0].name);
                    c.DownloadFileCompleted += new AsyncCompletedEventHandler(finished_download);
                    c.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                    c.DownloadFileAsync(uri, exe + "\\tmp\\" + System.IO.Path.GetFileNameWithoutExtension(downloadqueue[0].downloadURL) + C + System.IO.Path.GetExtension(downloadqueue[0].downloadURL));
                }));
            }
            catch
            {
                txtbox.AppendText(MainWindow.globalLanguage.global.BM200);
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
                txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qosmetics.code.downloaded, downloadqueue[0].name) + "\n");
                txtbox.ScrollToEnd();
            }));
            String file = exe + "\\tmp\\" + System.IO.Path.GetFileNameWithoutExtension(downloadqueue[0].downloadURL) + C + System.IO.Path.GetExtension(downloadqueue[0].downloadURL);
            if(!File.Exists(file))
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qosmetics.code.downloadFailed, downloadqueue[0].name));
                txtbox.ScrollToEnd();
                Running = false;
                DownloadLable.Text = MainWindow.globalLanguage.global.allFinished;
                Progress.Value = 0;
                downloadqueue.RemoveAt(0);
                checkqueue();
                return;
            }
            if (file.ToLower().EndsWith(".zip"))
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.qosmetics.code.extractingQosmetics);
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
            MainWindow.iPUtils.CheckIP(Quest);

            TimeoutWebClient client = new TimeoutWebClient();

            txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qosmetics.code.uploadingToBMBF, downloadqueue[0].name));
            txtbox.ScrollToEnd();
            Uri uri = new Uri("http://" + MainWindow.config.IP + ":50000/host/beatsaber/upload?overwrite");
            try
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                {
                    DownloadLable.Text = MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qosmetics.code.uploadingToBMBF, downloadqueue[0].name);
                    client.UploadProgressChanged += new UploadProgressChangedEventHandler(client_uploadchanged);
                    client.UploadFileCompleted += new UploadFileCompletedEventHandler(finished_upload);
                    client.UploadFileAsync(uri, path);
                }));
            }
            catch
            {
                txtbox.AppendText(MainWindow.globalLanguage.global.BMBF100);
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
            txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qosmetics.code.uploadComplete, downloadqueue[0].name));
            txtbox.ScrollToEnd();
            Running = false;
            DownloadLable.Text = MainWindow.globalLanguage.global.allFinished;
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
                    client.UploadValues("http://" + MainWindow.config.IP + ":50000/host/beatsaber/commitconfig", "POST", client.QueryString);
                }
            }
            catch
            {
                txtbox.AppendText(MainWindow.globalLanguage.global.BMBF110);
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
