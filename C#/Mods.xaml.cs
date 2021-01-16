using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
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
using ModObjects;
using System.Text.Json;

namespace BMBF_Manager
{
    /// <summary>
    /// Interaktionslogik für Mods.xaml
    /// </summary>
    public partial class Mods : Window
    {
        Boolean draggable = true;
        Boolean Running = false;
        String exe = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 1);
        List<Mod> AllModList = new List<Mod>();
        List<int> downloadqueue = new List<int>();
        JSONNode BMBF = JSON.Parse("{}");
        int C = 0;
        int Index = 0;

        public Mods()
        {
            InitializeComponent();
            Quest.Text = MainWindow.IP;
            DownloadLable.Text = "All finished";
            getMods();
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
                uniformBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Mods6.png", UriKind.Absolute));
                uniformBrush.Stretch = Stretch.UniformToFill;
                this.Background = uniformBrush;
            }
        }

        public void getMods()
        {
            getQuestIP();
            TimeoutWebClientShort client = new TimeoutWebClientShort();

            string json = "";
            Boolean Reaching = true;

            try
            {
                json = client.DownloadString("http://www.questmodding.com/api/mods/");
            }
            catch
            {
                txtbox.AppendText(MainWindow.BM100);
            }

            try
            {
                BMBF = SimpleJSON.JSON.Parse(client.DownloadString("http://" + MainWindow.IP + ":50000/host/beatsaber/config"));
                MainWindow.GameVersion = BMBF["BeatSaberVersion"];
            }
            catch
            {
                txtbox.AppendText(MainWindow.BMBF100);
                Reaching = false;
            }
            String[] GameVersion = MainWindow.GameVersion.ToString().Replace("\"", "").Split('.');
            //String[] GameVersion = "1.11.0".Replace("\"", "").Split('.');
            int major = Convert.ToInt32(GameVersion[0]);
            int minor = Convert.ToInt32(GameVersion[1]);
            int patch = Convert.ToInt32(GameVersion[2]);

            ModList QB = new ModList();

            QB = JsonSerializer.Deserialize<ModList>(json);



            WebClient c = new WebClient();

            ModList CE = new ModList();

            //json = JSON.Parse(c.DownloadString("https://raw.githubusercontent.com/ComputerElite/BM/main/testing.json"));
            CE = JsonSerializer.Deserialize<ModList>(c.DownloadString("https://raw.githubusercontent.com/ComputerElite/BM/main/mods.json"));

            ModUtils mu = new ModUtils();
            AllModList = mu.MergeModLists(CE, QB, BMBF, major, minor, patch);
            
            ModList.SelectedIndex = 0;
            updatemodlist(false);
            if(!Reaching)
            {
                MessageBox.Show("I couldn't reach BMBF. All the mods displayed are for the last Version of BMBF you used while I noticed (" + MainWindow.GameVersion + "). Please check if you can reach BMBF so I can install mods.", "BMBF Manager - Mod Installing", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void InstallMod(String ModName)
        {
            int i = 0;
            bool found = false;
            foreach (Mod m in AllModList)
            {
                if (m.name.ToLower() == ModName.ToLower())
                {
                    found = true;
                    downloadqueue.Add(i);
                    checkqueue();
                }
                i++;
            }
            if(!found)
            {
                txtbox.AppendText("\n\nI couldn't find " + ModName + " for your Game Version");
            }
        }

        private void updatemodlist(Boolean downloadbmbf = true)
        {
            if(downloadbmbf)
            {
                WebClient client = new WebClient();
                try
                {
                    BMBF = SimpleJSON.JSON.Parse(client.DownloadString("http://" + MainWindow.IP + ":50000/host/beatsaber/config"));
                }
                catch
                {
                    txtbox.AppendText(MainWindow.BMBF100);
                    txtbox.ScrollToEnd();
                }
            }
            ModList.Items.Clear();
            foreach (Mod cmod in AllModList)
            {
                String BMBFVersion = "0.0.0";
                Boolean installed = false;
                foreach (JSONNode BMBFMod in BMBF["Config"]["Mods"])
                {
                    String ModID = cmod.name;
                    if (cmod.ModID != "") ModID = cmod.ModID;
                    if(BMBFMod["ID"] != null)
                    {
                        if (BMBFMod["ID"].ToString().ToLower().Replace("\"", "") == ModID.ToLower())
                        {
                            BMBFVersion = BMBFMod["Version"];
                            installed = true;
                            break;
                        }
                    }
                }

                //Name, Version, DownloadLink, Creator, gameVersion, Desciption, Forward, new Tuple (CoreMod, ModID, currentversion)
                ModList.Items.Add(new ModItem(cmod.name, String.Join(", ", cmod.creator), cmod.downloads[cmod.MatchingDownload].gameversion[cmod.MatchingGameVersion], BMBFVersion, cmod.downloads[cmod.MatchingDownload].modversion, installed)); ;
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
        public void MoreInfo(object sender, RoutedEventArgs e)
        {
            //Name, Version, DownloadLink, Creator, gameVersion, Desciption, Forward, new Tuple (CoreMod, ModID, currentversion, islatest)
            MessageBox.Show("Mod Name: " + AllModList[ModList.SelectedIndex].name + "\n\nDescription:\n" + AllModList[ModList.SelectedIndex].details + "\n\nNotes for the latest mod release:\n" + AllModList[ModList.SelectedIndex].downloads[AllModList[ModList.SelectedIndex].MatchingDownload].notes, "BMBF Manager - Mod Info", MessageBoxButton.OK);
        }

        public void checkqueue()
        {
            if(downloadqueue.Count != 0)
            {
                InstallMod();
            } else
            {
                txtbox.AppendText("\n\nAll finished.");
                txtbox.ScrollToEnd();
            }
        }

        public void InstallMod()
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
            while (File.Exists(exe + "\\tmp\\" + AllModList[downloadqueue[0]].name + C + ".zip"))
            {
                C++;
            }

            Index = downloadqueue[0];

            if (AllModList[Index].downloads[AllModList[Index].MatchingDownload].forward)
            {
                MessageBoxResult result1 = MessageBox.Show("You have to download and install the mod " + AllModList[Index].name + " manually. If you click yes I'll redirect you to the download page and open BMBF for you.\nDo you wish to continue?", "BMBF Manager - Mod Installing", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result1)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\nMod Installing Aborted.");
                        txtbox.ScrollToEnd();
                        Running = false;
                        downloadqueue.RemoveAt(0);
                        checkqueue();
                        return;
                }
                Process.Start("http://" + MainWindow.IP + ":50000/main/upload");
                Process.Start(AllModList[Index].downloads[AllModList[Index].MatchingDownload].download);
                return;
            }

            if (AllModList[Index].downloads[AllModList[Index].MatchingDownload].coremod)
            {
                MessageBox.Show("The Mod you are about to install is a Core Mod. That means the Mod should get installed when you exit BMBF and open it again. Please make sure you DON'T have the mod installed. I'll open BMBF for you once you click OK.", "BMBF Manager - Mod Installing", MessageBoxButton.OK, MessageBoxImage.Warning);
                Process.Start("http://" + MainWindow.IP + ":50000/main/mods");
                MessageBoxResult result1 = MessageBox.Show("Do you have the mod " + AllModList[Index].name + " installed?", "BMBF Manager - Mod Installing", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result1)
                {
                    case MessageBoxResult.Yes:
                        txtbox.AppendText("\n\nMod is already installed aborted.");
                        txtbox.ScrollToEnd();
                        Running = false;
                        checkqueue();
                        downloadqueue.RemoveAt(0);
                        return;
                }
            }

            if (AllModList[Index].downloads[AllModList[Index].MatchingDownload].gameversion[AllModList[Index].MatchingGameVersion] != MainWindow.GameVersion)
            {
                MessageBoxResult result1 = MessageBox.Show("The latest Version of the Mod " + AllModList[Index].name + " (That is indexed) has been made for Beat Saber Version " + AllModList[Index].downloads[AllModList[Index].MatchingDownload].gameversion[AllModList[Index].MatchingGameVersion] + ". It'll most likelybe compatible with your Game but you have to enable it manually. I'll open the BMBF mod tab after installing the mod. For it to activate you scroll to the mod you installed and flip the switch to on. If you get a compatibility warning click \"Enable Mod\" and then click \"Sync to Beat Saber\" in the top right.\nDo you wish to continue?", "BMBF Manager - Mod Installing", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result1)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\nMod Installing Aborted.");
                        txtbox.ScrollToEnd();
                        Running = false;
                        downloadqueue.RemoveAt(0);
                        checkqueue();
                        return;
                }
            }

            String Download = AllModList[Index].downloads[AllModList[Index].MatchingDownload].download;
            WebClient c = new WebClient();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                txtbox.AppendText("\n\nDownloading Mod " + AllModList[Index].name + "\n");
                txtbox.ScrollToEnd();
            }));
            Uri uri = new Uri(Download);
            try
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                {
                    DownloadLable.Text = "Downloading " + AllModList[Index].name;
                    c.DownloadFileCompleted += new AsyncCompletedEventHandler(finished_download);
                    c.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                    c.DownloadFileAsync(uri, exe + "\\tmp\\" + AllModList[Index].name + C + ".zip");
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

        public void AddSelectedModToQueue(object sender, RoutedEventArgs e)
        {   
            if(ModList.SelectedIndex < 0 || ModList.SelectedIndex > (ModList.Items.Count - 1))
            {
                txtbox.AppendText("\n\nPlease select a mod");
                return;
            }
            if (downloadqueue.Contains(ModList.SelectedIndex))
            {
                txtbox.AppendText("\n" + AllModList[ModList.SelectedIndex].name + " is already in the download queue");
                txtbox.ScrollToEnd();
                return;
            }
            downloadqueue.Add(ModList.SelectedIndex);
            txtbox.AppendText("\n\n" + AllModList[ModList.SelectedIndex].name + " was added to the queue");
            txtbox.ScrollToEnd();
            checkqueue();
        }

        public void UpdateMods(object sender, RoutedEventArgs e)
        {
            TimeoutWebClientShort c = new TimeoutWebClientShort();
            try
            {
                BMBF = JSON.Parse(c.DownloadString("http://" + MainWindow.IP + ":50000/host/beatsaber/config"));
            }
            catch
            {
                txtbox.AppendText(MainWindow.BMBF100);
                txtbox.ScrollToEnd();
                return;
            }
            int i = 0;
            foreach(JSONNode Mod in BMBF["Config"]["Mods"])
            {
                i = 0;
                foreach(Mod m in AllModList)
                {
                    if(Mod["ID"].ToString().Replace("\"", "").ToLower() == m.ModID.ToLower())
                    {
                        String[] ModV = Mod["Version"].ToString().Replace("\"", "").Split('.');
                        int Major = 0;
                        int Minor = 0;
                        int Patch = 0;
                        try
                        {
                            Major = Convert.ToInt32(ModV[0]);
                            Minor = Convert.ToInt32(ModV[1]);
                            Patch = Convert.ToInt32(ModV[2]);
                        } catch { }

                        String[] ModVD = m.downloads[m.MatchingDownload].modversion.Split('.');
                        int MajorD = 0;
                        int MinorD = 0;
                        int PatchD = 0;
                        try
                        {
                            MajorD = Convert.ToInt32(ModVD[0]);
                            MinorD = Convert.ToInt32(ModVD[1]);
                            PatchD = Convert.ToInt32(ModVD[2]);
                        } catch { }
                        if((MajorD >= Major && MinorD >= Minor && PatchD > Patch) || (MajorD >= Major && MinorD > Minor) || (MajorD > Major))
                        {
                            downloadqueue.Add(i);
                            txtbox.AppendText("\nAdded " + m.name + " version " + m.downloads[m.MatchingDownload].modversion + " to queue");
                        }
                        break;
                    }
                    i++;
                }
            }
            checkqueue();
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

        private void finished_download(object sender, AsyncCompletedEventArgs e)
        {
            adb("shell am start -n com.weloveoculus.BMBF/com.weloveoculus.BMBF.MainActivity");
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                txtbox.AppendText("\nDownloaded Mod " + AllModList[Index].name + "\n");
                txtbox.ScrollToEnd();
            }));
            upload(exe + "\\tmp\\" + AllModList[Index].name + C + ".zip");
        }

        public void upload(String path)
        {
            getQuestIP();

            TimeoutWebClient client = new TimeoutWebClient();

            txtbox.AppendText("\n\nUploading " + AllModList[Index].name + " to BMBF");
            txtbox.ScrollToEnd();
            Uri uri = new Uri("http://" + MainWindow.IP + ":50000/host/beatsaber/upload?overwrite");
            try
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                {
                    DownloadLable.Text = "Uploading " + AllModList[Index].name + " to BMBF";
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
            if (AllModList[Index].downloads[AllModList[Index].MatchingDownload].gameversion[AllModList[Index].MatchingGameVersion] == MainWindow.GameVersion)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                    {
                        Sync();
                    }));
                    txtbox.AppendText("\n\nMod " + AllModList[Index].name + " was synced to your Quest.");
                    txtbox.ScrollToEnd();
                }
                catch
                {
                    txtbox.AppendText("\n\nCouldn't sync with BeatSaber. Needs to be done manually.");
                    txtbox.ScrollToEnd();
                }
            }
            else
            {
                Process.Start("http://" + MainWindow.IP + ":50000/main/mods");
                txtbox.AppendText("\n\nSince you choose to install this mod... you need to enable it manually. I uploaded it.");
                txtbox.ScrollToEnd();
            }
            Running = false;
            DownloadLable.Text = "All finished";
            Progress.Value = 0;
            downloadqueue.RemoveAt(0);
            updatemodlist();
            checkqueue();
            return;
        }

        public void postChanges(String Config)
        {
            System.Threading.Thread.Sleep(10000);
            using (WebClient client = new WebClient())
            {
                client.QueryString.Add("foo", "foo");
                client.UploadFile("http://" + MainWindow.IP + ":50000/host/beatsaber/config", "PUT", Config);
                client.UploadValues("http://" + MainWindow.IP + ":50000/host/beatsaber/commitconfig", "POST", client.QueryString);
            }
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

    public class ModItem
    {
        public string Name { get; set; }

        public string Creator { get; set; }

        public string GameVersion { get; set; }

        public string ModVersion { get; set; }

        public string latest { get; set; }

        public System.Windows.Media.SolidColorBrush Color { get; set; }
        public ModItem(String Name, String Creator, String GameVersion, String ModVersion, String latest, Boolean installed = true)
        {
            this.Name = Name;
            this.Creator = Creator;
            this.GameVersion = GameVersion;
            this.latest = latest;
            
            SolidColorBrush newColor = new SolidColorBrush();
            
            if (!installed)
            {
                this.ModVersion = "N/A";
                newColor.Color = Colors.Yellow;
            } else
            {
                this.ModVersion = ModVersion;
                String[] ModV = ModVersion.Split('.');
                int Major = 0;
                int Minor = 0;
                int Patch = 0;
                try
                {
                    Major = Convert.ToInt32(ModV[0]);
                    Minor = Convert.ToInt32(ModV[1]);
                    Patch = Convert.ToInt32(ModV[2]);
                }
                catch { }

                String[] ModVD = latest.Split('.');
                int MajorD = 0;
                int MinorD = 0;
                int PatchD = 0;
                try
                {
                    MajorD = Convert.ToInt32(ModVD[0]);
                    MinorD = Convert.ToInt32(ModVD[1]);
                    PatchD = Convert.ToInt32(ModVD[2]);
                }
                catch { }
                if (!(MajorD >= Major && MinorD >= Minor && PatchD > Patch) && !(MajorD >= Major && MinorD > Minor) && !(MajorD > Major))
                {
                    newColor.Color = Colors.Green;
                }
                else
                {
                    newColor.Color = Colors.Red;
                }
            }
            this.Color = newColor;
        }
    }
}


