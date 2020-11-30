using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BMBF_Manager
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int MajorV = 1;
        int MinorV = 7;
        int PatchV = 4;
        Boolean Preview = false;

        public static Boolean CustomProtocols = false;
        public static Boolean QuestSoundsInstalled = false;
        public static Boolean CustomImage = false;
        public static Boolean BBBUTransfered = false;
        public static Boolean ShowADB = false;
        Boolean draggable = true;
        Boolean Running = false;
        Boolean ComeFromUpdate = false;
        String exe = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 1);
        public static String IP = "";
        public static String BMBF = "https://bmbf.dev/stable/27153984";
        public static String CustomImageSource = "N/A";
        public static String GameVersion = "1.13.0";
        JSONNode json = JSON.Parse("{}");
        public static JSONNode UpdateJSON = JSON.Parse("{}");
        JSONNode BMBFStable = JSON.Parse("{}");
        public static ArrayList ADBPaths = new ArrayList();


        public MainWindow()
        {
            InitializeComponent();
            if (!Directory.Exists(exe + "\\Backups")) Directory.CreateDirectory(exe + "\\Backups");
            if (!Directory.Exists(exe + "\\Backup")) Directory.CreateDirectory(exe + "\\Backup");
            if (Directory.Exists(exe + "\\ModChecks")) Directory.Delete(exe + "\\ModChecks", true);
            if (!Directory.Exists(exe + "\\ModChecks")) Directory.CreateDirectory(exe + "\\ModChecks");
            if (!Directory.Exists(exe + "\\tmp")) Directory.CreateDirectory(exe + "\\tmp");
            if (File.Exists(exe + "\\BM_Update.exe")) File.Delete(exe + "\\BM_Update.exe");
            UpdateB.Visibility = Visibility.Hidden;
            txtbox.Text = "Output:";
            loadConfig();
            Update();
            StartBMBF();
            QuestIP();
            Quest.Text = IP;
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
                uniformBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Main7.png", UriKind.Absolute));
                uniformBrush.Stretch = Stretch.UniformToFill;
                this.Background = uniformBrush;
            }
            Changelog();
            ComeFromUpdate = false;
        }

        public void Changelog()
        {
            if(ComeFromUpdate)
            {
                String creators = "";
                foreach(JSONNode Creator in UpdateJSON["Updates"][0]["Creators"])
                {
                    creators = creators + Creator.ToString().Replace("\"", "") + ", ";
                }
                if(creators.Length >= 2)
                {
                    creators = creators.Substring(0, creators.Length - 2);
                } else
                {
                    creators = "ComputerElite";
                }
                txtbox.AppendText("\n\n\nYou installed a Update (Version: " + MajorV + "." + MinorV + "." + PatchV + ").\n\nUpdate posted by: " + creators + "\n\nChangelog:\n" + UpdateJSON["Updates"][0]["Changelog"]);
            }
            
        }

        public void loadConfig()
        {
            if(!File.Exists(exe + "\\Config.json"))
            {
                IP = "Quest IP";
                enablecustom();
                return;
            }
            json = JSON.Parse(File.ReadAllText(exe + "\\Config.json"));

            foreach(JSONNode ADBPath in json["CachedADBPaths"])
            {
                ADBPaths.Add(ADBPath.ToString().Replace("\"", ""));
            }

            CustomProtocols = json["CustomProtocols"].AsBool;
            IP = json["IP"];
            BBBUTransfered = json["BBBUTransfered"].AsBool;
            ShowADB = json["ShowADB"].AsBool;
            if (json["GameVersion"] != null)
            {
                GameVersion = json["GameVersion"];
            }
            QuestSoundsInstalled = json["QSoundsInstalled"].AsBool;

            ComeFromUpdate = json["ComeFromUpdate"].AsBool;

            Quest.Text = IP;

            if (!json["NotFirstRun"].AsBool)
            {
                enablecustom();
            }
            else if (!json["Location"].Equals(System.Reflection.Assembly.GetEntryAssembly().Location))
            {
                enablecustom();
            }

            CustomImage = json["CustomImage"].AsBool;
            CustomImageSource = json["CustomImageSource"];
            
        }

        public void enablecustom()
        {
            String regFile = "Windows Registry Editor Version 5.00\n\n[HKEY_CLASSES_ROOT\\bm]\n@=\"URL: bm\"\n\"URL Protocol\"=\"bm\"\n\n[HKEY_CLASSES_ROOT\\bm]\n@=\"" + System.Reflection.Assembly.GetEntryAssembly().Location.Replace("\\", "\\\\") + "\"\n\n[HKEY_CLASSES_ROOT\\bm\\shell]\n\n[HKEY_CLASSES_ROOT\\bm\\shell\\open]\n\n[HKEY_CLASSES_ROOT\\bm\\shell\\open\\command]\n@=\"" + System.Reflection.Assembly.GetEntryAssembly().Location.Replace("\\", "\\\\") + " \\\"%1\\\"\"";
            File.WriteAllText(exe + "\\registry.reg", regFile);
            try
            {
                Process.Start(exe + "\\registry.reg");
                txtbox.AppendText("\n\nCustom Links enabled");
            }
            catch
            {
                txtbox.AppendText("\n\nRegistry was unable to change... no Custom protocol enabled.");
                return;
            }
            CustomProtocols = true;
        }

        public void saveConfig()
        {
            CheckIP();
            json["IP"] = IP;
            json["Version"] = MajorV.ToString() + MinorV.ToString() + PatchV.ToString();
            json["NotFirstRun"] = true;
            json["Location"] = System.Reflection.Assembly.GetEntryAssembly().Location;
            json["CustomProtocols"] = CustomProtocols;
            json["QSoundsInstalled"] = QuestSoundsInstalled;
            json["CustomImage"] = CustomImage;
            json["CustomImageSource"] = CustomImageSource;
            json["GameVersion"] = GameVersion;
            json["ComeFromUpdate"] = ComeFromUpdate;
            json["BBBUTransfered"] = BBBUTransfered;
            json["ShowADB"] = ShowADB;
            int i = 0;
            foreach(String ADBPath in ADBPaths)
            {
                json["CachedADBPaths"][i] = ADBPath.Replace("\\\\", "\\");
                i++;
            }
            File.WriteAllText(exe + "\\Config.json", json.ToString());
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



        public void QuestIP()
        {
            String IPS = adbS("shell ifconfig wlan0");
            int Index = IPS.IndexOf("inet addr:");
            Boolean space = false;
            String FIP = "";
            for (int i = 0; i < IPS.Length; i++)
            {
                if (i > (Index + 9) && i < (Index + 9 + 16))
                {
                    if (IPS.Substring(i, 1) == " ")
                    {
                        space = true;
                    }
                    if (!space)
                    {
                        FIP = FIP + IPS.Substring(i, 1);
                    }
                }
            }

            if (FIP == "" && IP == "Quest IP")
            {
                IP = "Quest IP";
                return;
            }
            if (FIP == "") return;
            IP = FIP;
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
            try
            {
                if (Directory.Exists(exe + "\\tmp"))
                {
                    Directory.Delete(exe + "\\tmp", true);
                }
            }
            catch
            {
            }
            saveConfig();
            Process.GetCurrentProcess().Kill();
            //this.Close();
        }

        public void StartBMBF()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                adb("shell am start -n com.weloveoculus.BMBF/com.weloveoculus.BMBF.MainActivity");
            }));
        }

        public void getQuestIP()
        {
            IP = Quest.Text;
            return;
        }

        public Boolean CheckIP()
        {
            getQuestIP();
            if (IP == "Quest IP")
            {
                return false;
            }
            IP = IP.Replace(":5000000", "");
            IP = IP.Replace(":500000", "");
            IP = IP.Replace(":50000", "");
            IP = IP.Replace(":5000", "");
            IP = IP.Replace(":500", "");
            IP = IP.Replace(":50", "");
            IP = IP.Replace(":5", "");
            IP = IP.Replace(":", "");
            IP = IP.Replace("/", "");
            IP = IP.Replace("https", "");
            IP = IP.Replace("http", "");
            IP = IP.Replace("Http", "");
            IP = IP.Replace("Https", "");

            int count = MainWindow.IP.Split('.').Count();
            if (count != 4)
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                {
                    Quest.Text = IP;
                }));
                return false;
            }
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                Quest.Text = IP;
            }));
            return true;
        }

        public Boolean adb(String Argument)
        {
            String User = System.Environment.GetEnvironmentVariable("USERPROFILE");
            
            foreach (String ADB in ADBPaths)
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
                        if(!MainWindow.ShowADB)
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
                        } else
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

        public String adbS(String Argument)
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
                try
                {
                    // Start the process with the info we specified.
                    // Call WaitForExit and then the using statement will close.
                    using (Process exeProcess = Process.Start(s))
                    {
                        String IPS = exeProcess.StandardOutput.ReadToEnd();
                        exeProcess.WaitForExit();
                        if (IPS.Contains("no devices/emulators found"))
                        {
                            txtbox.AppendText("\n\n\nAn error Occured (Code: ADB110). Check following");
                            txtbox.AppendText("\n\n- Your Quest is connected, Developer Mode enabled and USB Debugging enabled.");
                            txtbox.ScrollToEnd();
                            return "Error";
                        }

                        return IPS;
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
            return "Error";
        }

        public void Update()
        {
            try
            {
                //Download Update.txt
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        UpdateJSON = JSON.Parse(client.DownloadString("https://raw.githubusercontent.com/ComputerElite/BM/main/update.json"));
                    }
                    catch
                    {
                        txtbox.AppendText("\n\n\nAn error Occured (Code: UD100). Couldn't check for Updates. Check following");
                        txtbox.AppendText("\n\n- Your PC has internet.");
                        return;
                    }
                }

                ADBPaths.Clear();
                foreach (JSONNode adbp in UpdateJSON["ADBPaths"])
                {
                    ADBPaths.Add(adbp.ToString().Replace("\"", ""));
                }

                int MajorU = UpdateJSON["Updates"][0]["Major"];
                int MinorU = UpdateJSON["Updates"][0]["Minor"];
                int PatchU = UpdateJSON["Updates"][0]["Patch"];

                if (MajorU > MajorV || MinorU > MinorV || PatchU > PatchV)
                {
                    //Newer Version available
                    UpdateB.Visibility = Visibility.Visible;
                }

                String MajorVS = Convert.ToString(MajorV);
                String MinorVS = Convert.ToString(MinorV);
                String PatchVS = Convert.ToString(PatchV);
                String MajorUS = Convert.ToString(MajorU);
                String MinorUS = Convert.ToString(MinorU);
                String PatchUS = Convert.ToString(PatchU);

                String VersionVS = MajorVS + MinorVS + PatchVS;
                int VersionV = Convert.ToInt32(VersionVS);
                String VersionUS = MajorUS + MinorUS + PatchUS + " ";
                int VersionU = Convert.ToInt32(VersionUS);
                if (VersionV > VersionU)
                {
                    //Newer Version that hasn't been released yet
                    txtbox.AppendText("\n\nLooks like you have a preview version. Downgrade now from " + MajorV + "." + MinorV + "." + PatchV + " to " + MajorU + "." + MinorU + "." + PatchU + " xD");
                    UpdateB.Visibility = Visibility.Visible;
                    UpdateB.Content = "Downgrade Now xD";
                }
                if (VersionV == VersionU && Preview)
                {
                    //User has Preview Version but a release Version has been released
                    txtbox.AppendText("\n\nLooks like you have a preview version. The release version has been released. Please Update now. ");
                    UpdateB.Visibility = Visibility.Visible;
                }
            }
            catch
            {

            }
        }

        private void Start_Update(object sender, RoutedEventArgs e)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile("https://github.com/ComputerElite/BM/raw/main/BM_Update.exe", exe + "\\BM_Update.exe");
                }
            
                Process.Start(exe + "\\BM_Update.exe");
                ComeFromUpdate = true;
                saveConfig();
                Process.GetCurrentProcess().Kill();
            }
            catch
            {
                // Log error.
                txtbox.AppendText("\n\n\nAn error Occured (Code: UD200). Couldn't download Update.");
            }
        }

        private void Mini(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void InstallSongs(object sender, RoutedEventArgs e)
        {
            CheckIP();
            Songs SongsWindow = new Songs();
            SongsWindow.Show();
        }

        private void InstallMods (object sender, RoutedEventArgs e)
        {
            CheckIP();
            Mods ModsWindow = new Mods();
            ModsWindow.Show();
        }

        private void BMBF_Link()
        {
            using (WebClient client = new WebClient())
            {
                BMBFStable = JSON.Parse(client.DownloadString("https://bmbf.dev/stable/json"));
            }
        }

        private void UpdateBMBF(object sender, RoutedEventArgs e)
        {
            if(Running)
            {
                txtbox.AppendText("\n\nA operation is already running. Please try again after it has finished.");
                return;
            }
            Running = true;
            CheckIP();
            getQuestIP();
            BMBF_Link();
            if (!adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/mods \"" + exe + "\\ModChecks"))
            {
                Running = false;
                return;
            }

            if (Directory.Exists(exe + "\\ModChecks\\mods"))
            {
                //game is modded
                MessageBoxResult result1 = MessageBox.Show("Modded Beat Saber has been detected. If you press yes I'll uninstall Beat Saber and BMBF and make a Backup of it to restore. If you press no you'll cancle Updating.", "BMBF Manager - BMBF Updater", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result1)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\nBMBF Updating aborted.");
                        txtbox.ScrollToEnd();
                        Running = false;
                        return;
                }
                if (!adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/ \"" + exe + "\\Backup\""))
                {
                    Running = false;
                    return;
                }

                //Backup Playlists
                try {
                    txtbox.AppendText("\n\nBacking up Playlist to " + exe + "\\Backup\\Playlists.json");
                    txtbox.ScrollToEnd();
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));

                    if(!adb("pull /sdcard/BMBFData/Playlists/ \"" + exe + "\\Backup\"")) return;

                    WebClient client2 = new WebClient();

                    var j = JSON.Parse(client2.DownloadString("http://" + MainWindow.IP + ":50000/host/beatsaber/config"));
                    File.WriteAllText(exe + "\\Backup\\Playlists.json", j["Config"].ToString());
                    txtbox.AppendText("\n\nBacked up Playlists to " + exe + "\\Backup\\Playlists.json");
                    txtbox.ScrollToEnd();
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                } catch
                {
                    txtbox.AppendText("\n\n\nAn error occured (Code: PL100). Check following:");
                    txtbox.AppendText("\n\n- You put in the Quests IP right.");
                    txtbox.AppendText("\n\n- You've choosen a Backup Name.");
                    txtbox.AppendText("\n\n- Your Quest is on.");

                }


            if (!adb("uninstall com.beatgames.beatsaber"))
                {
                    Running = false;
                    return;
                }
                if (!adb("uninstall com.weloveoculus.BMBF"))
                {
                    Running = false;
                    return;
                }
                MessageBoxResult result2 = MessageBox.Show("Please download Beat Saber from the oculus store, play a song and then close it. Press OK once you finished.", "BMBF Manager - BMBF Updater", MessageBoxButton.OK, MessageBoxImage.Warning);
                MessageBoxResult result3 = MessageBox.Show("I want to make sure. Do you have unmodded Beat Saber installed and opened it at least once?", "BMBF Manager - BMBF Updater", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result3)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\nBMBF Updating aborted. Please Install unmodded Beat Saber and start it once");
                        txtbox.ScrollToEnd();
                        Running = false;
                        return;
                }
            } else
            {
                if (!adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/ \"" + exe + "\\Backup\""))
                {
                    Running = false;
                    return;
                }
                MessageBoxResult result = MessageBox.Show("Looks like you have unmodded Beat Saber installed. Did you open it at least once?", "BMBF Manager - BMBF Updater", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\nBMBF Updating aborted. Please Install unmodded Beat Saber and start it once");
                        txtbox.ScrollToEnd();
                        Running = false;
                        return;
                }
            }

            if(Directory.Exists(exe + "\\ModChecks\\mods")) Directory.Delete(exe + "\\ModChecks\\mods", true);
            
            if (Directory.Exists(exe + "\\Backup\\files\\mods")) Directory.Delete(exe + "\\Backup\\files\\mods", true);
            if (Directory.Exists(exe + "\\Backup\\files\\libs")) Directory.Delete(exe + "\\Backup\\files\\libs", true);
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));

            List<String> BadBMBF = new List<String>();
            foreach(JSONNode version in UpdateJSON["BadBMBF"])
            {
                foreach(JSONNode bmbf in BMBFStable)
                {
                    if(bmbf["tag"].ToString().Replace("\"", "") == version.ToString().Replace("\"", ""))
                    {
                        BadBMBF.Add(bmbf["id"].ToString().Replace("\"", ""));
                        break;
                    }
                }
            }

            if(BadBMBF.Contains(BMBFStable[0]["id"].ToString().Replace("\"", "")))
            {
                JSONNode lastBMBF = JSON.Parse("{}");
                foreach (JSONNode bmbf in BMBFStable)
                {
                    if(!BadBMBF.Contains(bmbf["id"].ToString().Replace("\"", "")))
                    {
                        lastBMBF = bmbf;
                        break;
                    }
                }
                MessageBoxResult result4 = MessageBox.Show("The newest BMBF Version (" + BMBFStable[0]["tag"] + ") doesn't work for many people. I'd suggest you update to a more stable version. The last entry that's not listed as not working is BMBF version " + lastBMBF["tag"] + ".\nIf you want to install the recommended version of BMBF press yes. If you want to install the newest one press no. ", "BMBF Manager - BMBF Updater", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result4)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\nDownloading newest BMBF version");
                        txtbox.ScrollToEnd();
                        foreach (JSONNode asset in BMBFStable[0]["assets"])
                        {
                            if (asset["name"].ToString().Replace("\"", "") == "com.weloveoculus.BMBF.apk")
                            {
                                BMBF = "https://bmbf.dev/stable/" + asset["id"];
                                break;
                            }
                        }
                        break;
                    case MessageBoxResult.Yes:
                        txtbox.AppendText("\n\nDownloading recommended BMBF version");
                        txtbox.ScrollToEnd();
                        foreach (JSONNode asset in lastBMBF["assets"])
                        {
                            if (asset["name"].ToString().Replace("\"", "") == "com.weloveoculus.BMBF.apk")
                            {
                                BMBF = "https://bmbf.dev/stable/" + asset["id"];
                                break;
                            }
                        }
                        break;
                }
            } else
            {
                foreach (JSONNode asset in BMBFStable[0]["assets"])
                {
                    if (asset["name"].ToString().Replace("\"", "") == "com.weloveoculus.BMBF.apk")
                    {
                        BMBF = "https://bmbf.dev/stable/" + asset["id"];
                        break;
                    }
                }
            }

            using (TimeoutWebClient client2 = new TimeoutWebClient())
            {
                client2.DownloadFileAsync(new Uri(BMBF), exe + "\\tmp\\BMBF.apk");
                client2.DownloadFileCompleted += new AsyncCompletedEventHandler(finishedBMBFDownload);
            }
            
        }

        private void finishedBMBFDownload(object sender, AsyncCompletedEventArgs e)
        {
            txtbox.AppendText("\nDownload Complete");
            txtbox.ScrollToEnd();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));


            //Install BMBF
            txtbox.AppendText("\n\nInstalling new BMBF");
            txtbox.ScrollToEnd();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));


            adb("install -r \"" + exe + "\\tmp\\BMBF.apk\"");

            //Mod Beat Saber
            txtbox.AppendText("\n\nModding Beat Saber. Please wait...");
            txtbox.ScrollToEnd();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));


            adb("shell am start -n com.weloveoculus.BMBF/com.weloveoculus.BMBF.MainActivity"); //Start BMBF
            adb("shell pm grant com.weloveoculus.BMBF android.permission.READ_EXTERNAL_STORAGE"); //Grant permission read
            adb("shell pm grant com.weloveoculus.BMBF android.permission.WRITE_EXTERNAL_STORAGE"); //Grant permission write
            // Need to add a delay
            System.Threading.Thread.Sleep(6000);
            adb("shell am start -n com.weloveoculus.BMBF/com.weloveoculus.BMBF.MainActivity"); //Start BMBF
            System.Threading.Thread.Sleep(5000);
            adb("shell am start -n com.weloveoculus.BMBF/com.weloveoculus.BMBF.MainActivity"); //Start BMBF
            System.Threading.Thread.Sleep(5000);
            TimeoutWebClient client = new TimeoutWebClient();
            client.UploadDataAsync(new Uri("http://" + MainWindow.IP + ":50000/host/mod/install/step1"), "POST", new byte[0]);
            client.UploadDataCompleted += new UploadDataCompletedEventHandler(finishedstep1);
        }

        private void finishedstep1(object sender, AsyncCompletedEventArgs e)
        {
            TimeoutWebClient client = new TimeoutWebClient();
            adb("uninstall com.beatgames.beatsaber");
            client.UploadDataAsync(new Uri("http://" + MainWindow.IP + ":50000/host/mod/install/step2"), "POST", new byte[0]);
            client.UploadDataCompleted += new UploadDataCompletedEventHandler(finishedstep2);
        }

        private void finishedstep2(object sender, UploadDataCompletedEventArgs e)
        {
            TimeoutWebClient client = new TimeoutWebClient();
            adb("pull /sdcard/Android/data/com.weloveoculus.BMBF/cache/beatsabermod.apk \"" + exe + "\\tmp\\beatsabermod.apk\"");
            adb("install -r \"" + exe + "\\tmp\\beatsabermod.apk\"");
            client.UploadDataAsync(new Uri("http://" + MainWindow.IP + ":50000/host/mod/install/step3"), "POST", new byte[0]);
            client.UploadDataCompleted += new UploadDataCompletedEventHandler(finishedstep3);
        }

        private void finishedstep3(object sender, UploadDataCompletedEventArgs e)
        {
            adb("shell am force-stop com.weloveoculus.BMBF");
            adb("shell am start -n com.weloveoculus.BMBF/com.weloveoculus.BMBF.MainActivity"); //Start BMBF
            adb("shell pm grant com.beatgames.beatsaber android.permission.READ_EXTERNAL_STORAGE"); //Grant permission read
            adb("shell pm grant com.beatgames.beatsaber android.permission.WRITE_EXTERNAL_STORAGE"); //Grant permission write

            if (!adb("push \"" + exe + "\\Backup\\files\" /sdcard/Android/data/com.beatgames.beatsaber"))
            {
                Running = false;
                return;
            }

            System.Threading.Thread.Sleep(6000);

            reloadsongsfolder();

            //restore Playlists
            try
            {
                if (!File.Exists(exe + "\\Backup\\Playlists.json"))
                {
                    txtbox.AppendText("\n\n\nFinished Installing BMBF and modding Beat Saber. Please click \"Reload Songs Folder\" in BMBF to reload your Songs if you Updated BMBF.");
                    txtbox.ScrollToEnd();
                    Running = false;
                    return;
                }

                WebClient client3 = new WebClient();

                String Playlists = exe + "\\Backup\\Playlists.json";

                var j = JSON.Parse(client3.DownloadString("http://" + MainWindow.IP + ":50000/host/beatsaber/config"));
                var p = JSON.Parse(File.ReadAllText(Playlists));

                j["Config"]["Playlists"] = p["Playlists"];
                File.WriteAllText(exe + "\\tmp\\config.json", j["Config"].ToString());

                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                    postChanges(exe + "\\tmp\\config.json");
                }));
                txtbox.AppendText("\n\nRestored old Playlists.");
                txtbox.ScrollToEnd();
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
            }
            catch
            {
                txtbox.AppendText("\n\n\nAn error occured (Code: BMBF100). Check following:");
                txtbox.AppendText("\n\n- Your Quest is on and BMBF opened");
                txtbox.AppendText("\n\n- You put in the Quests IP right.");
            }

            txtbox.AppendText("\n\n\nFinished Installing BMBF and modding Beat Saber. Please click \"Reload Songs Folder\" in BMBF to reload your Songs if you Updated BMBF.");
            txtbox.ScrollToEnd();
            Running = false;
        }

        public void postChanges(String Config)
        {
            System.Threading.Thread.Sleep(10000);
            using (WebClient client = new WebClient())
            {
                client.QueryString.Add("foo", "foo");
                client.UploadFile("http://" + IP + ":50000/host/beatsaber/config", "PUT", Config);
                client.UploadValues("http://" + IP + ":50000/host/beatsaber/commitconfig", "POST", client.QueryString);
            }
        }

        public void PushPNG(String Path)
        {
            String[] directories = Directory.GetFiles(Path);



            for (int i = 0; i < directories.Length; i++)
            {
                if (directories[i].EndsWith(".png"))
                {
                    txtbox.AppendText("\n\nPushing " + directories[i] + " to Quest");
                    adb("push \"" + directories[i] + "\" /sdcard/BMBFData/Playlists/");
                    txtbox.ScrollToEnd();
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                }
            }
        }

        public void SwitchVersion(object sender, RoutedEventArgs e)
        {
            if (Running)
            {
                txtbox.AppendText("\n\nA operation is already running. Please try again after it has finished.");
                return;
            }

            if (!adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/mods \"" + exe + "\\ModChecks"))
            {
                Running = false;
                return;
            }

            if (!adb("shell am force-stop com.weloveoculus.BMBF"))
            {
                Running = false;
                return;
            }

            if (Directory.Exists(exe + "\\ModChecks\\mods"))
            {
                //game is modded
                
                if (File.Exists(exe + "\\Backups\\modded.apk"))
                {
                    //Unmodded Beat Saber may be installed
                    MessageBoxResult result = MessageBox.Show("It looks like your last Action was installing unmodded Beat Saber. If you continue and have unmodded Beat Saber installed you must mod Beat Saber By hand.\nDo you wish to continue?", "BMBF Manager - Version Switcher", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    switch (result)
                    {
                        case MessageBoxResult.No:
                            txtbox.AppendText("\n\nAborted.");
                            txtbox.ScrollToEnd();
                            Running = false;
                            return;
                    }
                }
                MessageBoxResult result2 = MessageBox.Show("I'll unmod Beat Saber for you.\nDo you want to proceed?", "BMBF Manager - Version Switcher", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result2)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\nAborted.");
                        txtbox.ScrollToEnd();
                        Running = false;
                        return;
                }
                //Install the unmodded Version of Beat Saber
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                    txtbox.AppendText("\n\nBacking up everything.");
                    txtbox.ScrollToEnd();
                }));
                if (!adb("pull /sdcard/BMBFData/Backups/beatsaber-unmodded.apk \"" + exe + "\\tmp\\unmodded.apk\""))
                {
                    Running = false;
                    return;
                }
                if (!adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files \"" + exe + "\\Backups\""))
                {
                    Running = false;
                    return;
                }
                //Directory.Delete(exe + "\\Backups\\files\\mods", true);
                //Directory.Delete(exe + "\\Backups\\files\\libs", true);

                String moddedBS = adbS("shell pm path com.beatgames.beatsaber").Replace("package:", "").Replace(System.Environment.NewLine, "");
                if (!adb("pull " + moddedBS + " \"" + exe + "\\Backups\\modded.apk\""))
                {
                    Running = false;
                    return;
                }

                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                    txtbox.AppendText("\n\nInstalling unmodded Beat Saber.");
                    txtbox.ScrollToEnd();
                }));
                if (!adb("uninstall com.beatgames.beatsaber"))
                {
                    Running = false;
                    return;
                }
                if (!adb("install \"" + exe + "\\tmp\\unmodded.apk\""))
                {
                    Running = false;
                    return;
                }
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                    txtbox.AppendText("\n\nRestoring Scores");
                    txtbox.ScrollToEnd();
                }));
                adb("push \"" + exe + "\\Backups\\files\\LocalDailyLeaderboards.dat\" /sdcard/Android/data/com.beatgames.beatsaber/files/LocalDailyLeaderboards.dat");
                adb("push \"" + exe + "\\Backups\\files\\LocalLeaderboards.dat\" /sdcard/Android/data/com.beatgames.beatsaber/files/LocalLeaderboards.dat");
                adb("push \"" + exe + "\\Backups\\files\\PlayerData.dat\" /sdcard/Android/data/com.beatgames.beatsaber/files/PlayerData.dat");
                adb("push \"" + exe + "\\Backups\\files\\AvatarData.dat\" /sdcard/Android/data/com.beatgames.beatsaber/files/AvatarData.dat");
                adb("push \"" + exe + "\\Backups\\files\\settings.cfg\" /sdcard/Android/data/com.beatgames.beatsaber/files/settings.cfg");


                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                    txtbox.AppendText("\n\nFinished. You can now play vanilla Beat Saber.");
                    txtbox.ScrollToEnd();
                }));

            } else
            {
                //game is unmodded
                if (!File.Exists(exe + "\\Backups\\modded.apk"))
                {
                    txtbox.AppendText("\n\nPlease Click \"Install/Update BMBF\" to mod Beat Saber the first time.");
                    Running = false;
                    return;
                }
                MessageBoxResult result2 = MessageBox.Show("I'll switch back to the modded Version of Beat Saber for you.\nDo you want to proceed?", "BMBF Manager - Version Switcher", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result2)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\nAborted.");
                        txtbox.ScrollToEnd();
                        Running = false;
                        return;
                }
                adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/LocalDailyLeaderboards.dat \"" + exe + "\\Backups\\files\\LocalDailyLeaderboards.dat\"");
                adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/LocalLeaderboards.dat \"" + exe + "\\Backups\\files\\LocalLeaderboards.dat\"");
                adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/PlayerData.dat \"" + exe + "\\Backups\\files\\PlayerData.dat\"");
                adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/AvatarData.dat \"" + exe + "\\Backups\\files\\AvatarData.dat\"");
                adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/settings.cfg \"" + exe + "\\Backups\\files\\settings.cfg\"");
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                    txtbox.AppendText("\n\nUninstalling Beat Saber.");
                    txtbox.ScrollToEnd();
                }));
                if (!adb("uninstall com.beatgames.beatsaber"))
                {
                    Running = false;
                    return;
                }
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                    txtbox.AppendText("\n\nInstalling Modded Beat Saber");
                    txtbox.ScrollToEnd();
                }));
                if (!adb("install \"" + exe + "\\Backups\\modded.apk\""))
                {
                    Running = false;
                    return;
                }
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                    txtbox.AppendText("\n\nRestoring Game Data");
                    txtbox.ScrollToEnd();
                }));
                if (!adb("push \"" + exe + "\\Backups\\files\" /sdcard/Android/data/com.beatgames.beatsaber/files"))
                {
                    Running = false;
                    return;
                }
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                    txtbox.AppendText("\n\nGranting Permissions");
                    txtbox.ScrollToEnd();
                }));
                adb("shell pm grant com.beatgames.beatsaber android.permission.READ_EXTERNAL_STORAGE"); //Grant permission read
                adb("shell pm grant com.beatgames.beatsaber android.permission.WRITE_EXTERNAL_STORAGE"); //Grant permission write
                //Directory.Delete(exe + "\\Backups", true);
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                    txtbox.AppendText("\n\nfinished. You can now play your Custom Songs again.");
                    txtbox.ScrollToEnd();
                }));
            }

            if (Directory.Exists(exe + "\\ModChecks\\mods")) Directory.Delete(exe + "\\ModChecks\\mods", true);
            Running = false;
        }

        private void BPLists(object sender, RoutedEventArgs e)
        {
            CheckIP();
            BPLists BPListsWindow = new BPLists();
            BPListsWindow.Show();
        }

        public void reloadsongsfolder()
        {
            System.Threading.Thread.Sleep(3000);
            TimeoutWebClient client = new TimeoutWebClient();
            client.QueryString.Add("foo", "foo");
            client.UploadValues("http://" + MainWindow.IP + ":50000/host/beatsaber/reloadsongfolders", "POST", client.QueryString);
        }

        private void OpenBMBF(object sender, RoutedEventArgs e)
        {
            StartBMBF();
            CheckIP();
            try
            {
                TimeoutWebClientShort c = new TimeoutWebClientShort();
                c.DownloadString("http://" + IP + ":50000/host/beatsaber/config");
                Process.Start("http://" + IP + ":50000/main/upload");
            } catch 
            {
                MessageBox.Show("I couldn't reach BMBF. The IP you typed is: \"" + IP + "\". Is this right? If it is check that BMBF is opened on your Quest and that your Quest and PC are on the same Wifi network.", "BMBF Manager - BMBF opening", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            
        }

        private void Support(object sender, RoutedEventArgs e)
        {
            CheckIP();
            Support SupportWindow = new Support();
            SupportWindow.Show();
        }

        private void HitSounds(object sender, RoutedEventArgs e)
        {
            CheckIP();
            HitSounds HitSoundsWindow = new HitSounds();
            HitSoundsWindow.Show();
        }

        private void BBBU(object sender, RoutedEventArgs e)
        {
            CheckIP();
            BBBU BBBUWindow = new BBBU();
            BBBUWindow.Show();
        }

        internal void CustomProto(string Link)
        {
            CheckIP();
            Support SupportWindow = new Support();
            SupportWindow.Show();
            SupportWindow.StartSupport(Link);
        }
    }
}   