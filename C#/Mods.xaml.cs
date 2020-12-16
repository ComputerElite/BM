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
        //Name, Version, DownloadLink, Creator, gameVersion, Desciption, Forward, new Tuple (CoreMod, ModID, currentversion)
        List<Tuple<String, String, String, String, String, String, Boolean, Tuple<bool, String, String>>> AllModsList = new List<Tuple<String, String, String, String, String, String, Boolean, Tuple<bool, String, String>>>();
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

            JSONNode json = JSON.Parse("{}");
            Boolean Reaching = true;

            try
            {
                json = SimpleJSON.JSON.Parse(client.DownloadString("http://www.questboard.xyz/api/mods/"));
            }
            catch
            {
                txtbox.AppendText("\n\nError (Code: BM100). Couldn't reach the Quest Boards Website to get some available Mods. Nothing crucial.");
            }

            try
            {
                BMBF = SimpleJSON.JSON.Parse(client.DownloadString("http://" + MainWindow.IP + ":50000/host/beatsaber/config"));
                MainWindow.GameVersion = BMBF["BeatSaberVersion"];
            }
            catch
            {
                txtbox.AppendText("\n\n\nError (Code: BMBF100). Couldn't acces BMBF Web Interface. Check Following:");
                txtbox.AppendText("\n\n- You've put in the right IP");
                txtbox.AppendText("\n\n- BMBF is opened");
                Reaching = false;
            }
            String[] GameVersion = MainWindow.GameVersion.ToString().Replace("\"", "").Split('.');
            //String[] GameVersion = "1.13.0".Replace("\"", "").Split('.');
            int major = Convert.ToInt32(GameVersion[0]);
            int minor = Convert.ToInt32(GameVersion[1]);
            int patch = Convert.ToInt32(GameVersion[2]);

            foreach (JSONNode mod in json["mods"])
            {
                String Name = mod["name"];
                String Creator = "";

                foreach (JSONNode Creat in mod["creator"])
                {
                    Creator = Creator + Creat + ", ";
                }
                Creator = Creator.Substring(0, Creator.Length - 2);
                String Version = mod["downloads"][0]["modversion"];

                foreach (JSONNode download in mod["downloads"])
                {
                    foreach (JSONNode gameversion in download["gameversion"])
                    {
                        String[] MGameVersion = gameversion.ToString().Replace("\"", "").Split('.');
                        int Mmajor = Convert.ToInt32(MGameVersion[0]);
                        int Mminor = Convert.ToInt32(MGameVersion[1]);
                        int Mpatch = 0;
                        if(MGameVersion.Count() == 2)
                        {
                            Mpatch = 0;
                        } else
                        {
                            Mpatch = Convert.ToInt32(MGameVersion[2]);
                        }
                        if (major == Mmajor && minor == Mminor && patch >= Mpatch)
                        {
                            Boolean existent = false;
                            foreach (Tuple<string, string, string, string, string, string, bool, Tuple<bool, String, String>> t in AllModsList)
                            {
                                if ((String)t.Item1 == Name)
                                {
                                    existent = true;
                                    break;
                                }
                            }
                            if (existent) continue;
                            
                            Version = download["modversion"];
                            //Name, Version, DownloadLink, Creator, gameVersion, Desciption, Forward, new Tuple (CoreMod, ModID, currentversion)
                            String BMBFVersion = "0.0.0";
                            foreach (JSONNode BMBFMod in BMBF["Config"]["Mods"])
                            {
                                if (BMBFMod["ID"].ToString().ToLower().Replace("\"", "") == mod["ModID"].ToString().ToLower().Replace("\"", ""))
                                {
                                    BMBFVersion = BMBFMod["Version"];
                                }
                            }
                            AllModsList.Add(new Tuple<string, string, string, string, string, string, bool, Tuple<bool, String, String>>(Name, Version, download["download"].ToString().Replace("\"", ""), Creator, gameversion.ToString().Replace("\"", ""), mod["details"].ToString().Replace("\"", "").Replace("\\r\\n", System.Environment.NewLine), download["forward"].AsBool, new Tuple<bool, String, String>(download["coremod"].AsBool, Name, BMBFVersion)));
                            break;
                        }
                    }
                }

            }

            WebClient c = new WebClient();

            //json = JSON.Parse(c.DownloadString("https://raw.githubusercontent.com/ComputerElite/BM/main/testing.json"));
            json = JSON.Parse(c.DownloadString("https://raw.githubusercontent.com/ComputerElite/BM/main/mods.json"));

            foreach (JSONNode mod in json["mods"])
            {
                String Name = mod["name"];
                String Creator = "";

                foreach (JSONNode Creat in mod["creator"])
                {
                    Creator = Creator + Creat + ", ";
                }
                Creator = Creator.Substring(0, Creator.Length - 2);
                String Version = mod["downloads"][0]["modversion"];

                foreach (JSONNode download in mod["downloads"])
                {
                    foreach (JSONNode gameversion in download["gameversion"])
                    {
                        String[] MGameVersion = gameversion.ToString().Replace("\"", "").Split('.');
                        int Mmajor = Convert.ToInt32(MGameVersion[0]);
                        int Mminor = Convert.ToInt32(MGameVersion[1]);
                        int Mpatch = 0;
                        if (MGameVersion.Count() == 2)
                        {
                            Mpatch = 0;
                        }
                        else
                        {
                            Mpatch = Convert.ToInt32(MGameVersion[2]);
                        }
                        if (major == Mmajor && minor == Mminor && patch >= Mpatch)
                        {
                            Boolean existent = false;
                            int ListIndex = 0;
                            foreach (Tuple<string, string, string, string, string, string, bool, Tuple<bool, String, String>> t in AllModsList)
                            {
                                if ((String)t.Item1 == Name)
                                {
                                    existent = true;
                                    break;
                                }
                                ListIndex++;
                            }
                            if (!existent)
                            {
                                Version = download["modversion"];
                                //Name, Version, DownloadLink, Creator, gameVersion, Desciption, Forward, new Tuple (CoreMod, ModID, currentversion, islatest)
                                String BMBFVersion = "0.0.0";
                                foreach (JSONNode BMBFMod in BMBF["Config"]["Mods"])
                                {
                                    if (BMBFMod["ID"].ToString().ToLower().Replace("\"", "") == mod["ModID"].ToString().ToLower().Replace("\"", ""))
                                    {
                                        BMBFVersion = BMBFMod["Version"];
                                    }
                                }
                                AllModsList.Add(new Tuple<string, string, string, string, string, string, bool, Tuple<bool, String, String>>(Name, Version, download["download"].ToString().Replace("\"", ""), Creator, gameversion.ToString().Replace("\"", ""), mod["details"].ToString().Replace("\"", "").Replace("\\r\\n", System.Environment.NewLine), download["forward"].AsBool, new Tuple<bool, String, String>(download["coremod"].AsBool, mod["ModID"], BMBFVersion)));
                            }
                            else
                            {
                                //Name, Version, DownloadLink, Creator, gameVersion, Desciption, Forward, new Tuple (CoreMod, ModID, currentversion, islatest)
                                String oldModver = AllModsList[ListIndex].Item2.ToString();
                                String[] allver = oldModver.Replace("\"", "").Split('.');
                                List<int> finishedver = new List<int>();
                                String[] newver = Version.Replace("\"", "").Split('.');
                                Boolean newer = false;
                                foreach (String CV in allver)
                                {
                                    finishedver.Add(Convert.ToInt32(CV));
                                }
                                int e = 0;
                                try
                                {
                                    if ((Convert.ToInt32(newver[0]) >= finishedver[0] && Convert.ToInt32(newver[1]) >= finishedver[1] && Convert.ToInt32(newver[2]) >= finishedver[2]) || (Convert.ToInt32(newver[0]) >= finishedver[0] && Convert.ToInt32(newver[1]) > finishedver[1]) || (Convert.ToInt32(newver[0]) > finishedver[0]))
                                    {
                                        newer = true;
                                    }
                                } catch
                                {
                                    continue;
                                }
                                e++;
                                if (!newer) continue;

                                AllModsList.RemoveAt(ListIndex);

                                //Name, Version, DownloadLink, Creator, gameVersion, Desciption, Forward, new Tuple (CoreMod, ModID, currentversion)
                                String BMBFVersion = "0.0.0";
                                foreach(JSONNode BMBFMod in BMBF["Config"]["Mods"])
                                {
                                    if(BMBFMod["ID"].ToString().ToLower().Replace("\"", "") == mod["ModID"].ToString().ToLower().Replace("\"", ""))
                                    {
                                        BMBFVersion = BMBFMod["Version"];
                                    }
                                }
                                if (BMBFVersion == "0.0.0") BMBFVersion = "N/A";
                                AllModsList.Add(new Tuple<string, string, string, string, string, string, bool, Tuple<bool, String, String>>(Name, Version, download["download"].ToString().Replace("\"", ""), Creator, gameversion.ToString().Replace("\"", ""), mod["details"].ToString().Replace("\"", "").Replace("\\r\\n", System.Environment.NewLine), download["forward"].AsBool, new Tuple<bool, String, String>(download["coremod"].AsBool, mod["ModID"], BMBFVersion)));
                            }

                            
                            break;
                        }
                    }
                }

            }
            ModList.SelectedIndex = 0;
            updatemodlist(false);
            if(!Reaching)
            {
                MessageBox.Show("I couldn't reach BMBF. All the mods displayed are for the last Version of BMBF you used while I noticed (" + MainWindow.GameVersion + "). Please check if you can reach BMBF so I can install mods.", "BMBF Manager - Mod Installing", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    txtbox.AppendText("\n\n\nError (Code: BMBF100). Couldn't acces BMBF Web Interface. Check Following:");
                    txtbox.AppendText("\n\n- You've put in the right IP");
                    txtbox.AppendText("\n\n- BMBF is opened");
                    txtbox.ScrollToEnd();
                }
            }
            ModList.Items.Clear();
            foreach (Tuple<string, string, string, string, string, string, bool, Tuple<bool, String, String>> cmod in AllModsList)
            {
                String BMBFVersion = "0.0.0";
                Boolean installed = false;
                foreach (JSONNode BMBFMod in BMBF["Config"]["Mods"])
                {
                    if(!BMBFMod["ID"].IsNull)
                    {
                        if (BMBFMod["ID"].ToString().ToLower().Replace("\"", "") == cmod.Rest.Item2.ToLower().Replace("\"", ""))
                        {
                            BMBFVersion = BMBFMod["Version"];
                            installed = true;
                            break;
                        }
                    }
                }
                
                //Name, Version, DownloadLink, Creator, gameVersion, Desciption, Forward, new Tuple (CoreMod, ModID, currentversion)
                ModList.Items.Add(new ModItem(cmod.Item1, cmod.Item4, cmod.Item5, BMBFVersion , cmod.Item2, installed));
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
            MessageBox.Show("Mod Name: " + AllModsList[ModList.SelectedIndex].Item1 + "\n\nDescription:\n" + AllModsList[ModList.SelectedIndex].Item6, "BMBF Manager - Mod Info", MessageBoxButton.OK);
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
            while (File.Exists(exe + "\\tmp\\" + AllModsList[downloadqueue[0]].Item1 + C + ".zip"))
            {
                C++;
            }

            Index = downloadqueue[0];

            if ((bool)AllModsList[Index].Item7)
            {
                MessageBoxResult result1 = MessageBox.Show("You have to download and install the mod " + AllModsList[Index].Item1 + " manually. If you click yes I'll redirect you to the download page and open BMBF for you.\nDo you wish to continue?", "BMBF Manager - Mod Installing", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result1)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\nMod Installing Aborted.");
                        txtbox.ScrollToEnd();
                        Running = false;
                        return;
                }
                Process.Start("http://" + MainWindow.IP + ":50000/main/upload");
                Process.Start(AllModsList[Index].Item3.ToString().Replace("\"", ""));
                return;
            }

            if ((bool)AllModsList[Index].Rest.Item1)
            {
                MessageBox.Show("The Mod you are about to install is a Core Mod. That means the Mod should get installed when you exit BMBF and open it again. Please make sure you DON'T have the mod installed. I'll open BMBF for you once you click OK.", "BMBF Manager - Mod Installing", MessageBoxButton.OK, MessageBoxImage.Warning);
                Process.Start("http://" + MainWindow.IP + ":50000/main/mods");
                MessageBoxResult result1 = MessageBox.Show("Do you have the mod " + AllModsList[Index].Item1 + " installed?", "BMBF Manager - Mod Installing", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result1)
                {
                    case MessageBoxResult.Yes:
                        txtbox.AppendText("\n\nMod is already installed aborted.");
                        txtbox.ScrollToEnd();
                        Running = false;
                        return;
                }
            }

            if (AllModsList[Index].Item5.ToString() != MainWindow.GameVersion)
            {
                MessageBoxResult result1 = MessageBox.Show("The latest Version of the Mod " + AllModsList[Index].Item1 + " (That is indexed) has been made for Beat Saber Version " + AllModsList[Index].Item5.ToString() + ". It'll be compatible with your Game but you have to enable it manually. I'll open the BMBF mod tab after installing the mod. For it to activate you scroll to the mod you installed and flip the switch to on. If you get a compatibility warning click \"Enable Mod\" and then click \"Sync to Beat Saber\" in the top right.\nDo you wish to continue?", "BMBF Manager - Mod Installing", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result1)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\nMod Installing Aborted.");
                        txtbox.ScrollToEnd();
                        Running = false;
                        return;
                }
            }

            String Download = AllModsList[Index].Item3.ToString().Replace("\"", "");
            WebClient c = new WebClient();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                txtbox.AppendText("\n\nDownloading Mod " + AllModsList[Index].Item1 + "\n");
                txtbox.ScrollToEnd();
            }));
            Uri uri = new Uri(Download);
            try
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                {
                    DownloadLable.Text = "Downloading " + AllModsList[Index].Item1;
                    c.DownloadFileCompleted += new AsyncCompletedEventHandler(finished_download);
                    c.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                    c.DownloadFileAsync(uri, exe + "\\tmp\\" + AllModsList[Index].Item1 + C + ".zip");
                }));
            }
            catch
            {
                txtbox.AppendText("\n\nError (Code: BM200). Couldn't download Mod");
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
                txtbox.AppendText("\n" + AllModsList[ModList.SelectedIndex].Item1 + " is already in the download queue");
                txtbox.ScrollToEnd();
                return;
            }
            downloadqueue.Add(ModList.SelectedIndex);
            txtbox.AppendText("\n\n" + AllModsList[ModList.SelectedIndex].Item1 + " was added to the queue");
            txtbox.ScrollToEnd();
            checkqueue();
        }

        public void UpdateMods(object sender, RoutedEventArgs e)
        {
            TimeoutWebClientShort c = new TimeoutWebClientShort();
            JSONNode BMBF = JSON.Parse(c.DownloadString("http://" + MainWindow.IP + ":50000/host/beatsaber/config"));
            int i = 0;
            foreach(JSONNode Mod in BMBF["Config"]["Mods"])
            {
                i = 0;
                foreach(Tuple<string, string, string, string, string, string, bool, Tuple<bool, String, String>> m in AllModsList)
                {
                    if(Mod["ID"].ToString().Replace("\"", "").ToLower() == m.Rest.Item2)
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

                        String[] ModVD = m.Item2.Replace("\"", "").Split('.');
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
                            txtbox.AppendText("\nAdded " + m.Item1 + " version " + m.Item2 + " to queue");
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
                                txtbox.AppendText("\n\n\nAn error Occured (Code: ADB110). Check following");
                                txtbox.AppendText("\n\n- Your Quest is connected, Developer Mode enabled and USB Debugging enabled.");
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
            txtbox.AppendText("\n\n\nAn error Occured (Code: ADB100). Check following not");
            txtbox.AppendText("\n\n- You have adb installed.");
            txtbox.ScrollToEnd();
            return false;
        }

        private void finished_download(object sender, AsyncCompletedEventArgs e)
        {
            adb("shell am start -n com.weloveoculus.BMBF/com.weloveoculus.BMBF.MainActivity");
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                txtbox.AppendText("\nDownloaded Mod " + AllModsList[Index].Item1 + "\n");
                txtbox.ScrollToEnd();
            }));
            upload(exe + "\\tmp\\" + AllModsList[Index].Item1 + C + ".zip");
        }

        public void upload(String path)
        {
            getQuestIP();

            TimeoutWebClient client = new TimeoutWebClient();

            txtbox.AppendText("\n\nUploading " + AllModsList[Index].Item1 + " to BMBF");
            txtbox.ScrollToEnd();
            Uri uri = new Uri("http://" + MainWindow.IP + ":50000/host/beatsaber/upload?overwrite");
            try
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                {
                    DownloadLable.Text = "Uploading " + AllModsList[Index].Item1 + " to BMBF";
                    client.UploadProgressChanged += new UploadProgressChangedEventHandler(client_uploadchanged);
                    client.UploadFileCompleted += new UploadFileCompletedEventHandler(finished_upload);
                    client.UploadFileAsync(uri, path);
                }));
            }
            catch
            {
                txtbox.AppendText("\n\nA error Occured (Code: BMBF100)");
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
            if (AllModsList[Index].Item5.ToString() == MainWindow.GameVersion)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                    {
                        Sync();
                    }));
                    txtbox.AppendText("\n\nMod " + AllModsList[Index].Item1 + " was synced to your Quest.");
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
                txtbox.AppendText("\n\nA error Occured (Code: BMBF100)");
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
                if (!(MajorD >= Major && MinorD >= Minor && PatchD > Patch) || !(MajorD >= Major && MinorD > Minor) || !(MajorD > Major))
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


