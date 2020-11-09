using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BMBF_Manager
{
    /// <summary>
    /// Interaktionslogik für BBBU.xaml
    /// </summary>
    public partial class BBBU : Window
    {
        Boolean draggable = true;
        Boolean running = false;
        String exe = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 1);
        String Songs = "";
        String Playlists = "";
        String Mods = "";
        String Scores = "";
        String BackupF = "";

        public BBBU()
        {
            InitializeComponent();

            BackupF = exe + "\\BBBUBackups";
            Quest.Text = MainWindow.IP;
            txtbox.Text = "Output:";

            if (!Directory.Exists(exe + "\\BBBUBackups"))
            {
                Directory.CreateDirectory(exe + "\\BBBUBackups");
            }
            if (!Directory.Exists(exe + "\\tmp"))
            {
                Directory.CreateDirectory(exe + "\\tmp");
            }
            getBackups();
            QuestIP();

            RSongs.IsChecked = true;
            RPlaylists.IsChecked = true;
            RScores.IsChecked = true;
            RMods.IsChecked = true;
            RReplays.IsChecked = true;
            RSounds.IsChecked = true;
            RConfigs.IsChecked = true;
        }

        public void BackupLink(String Name)
        {
            StartBMBF();
            if (running)
            {
                running = false;
                return;
            }
            running = true;

            BName.Text = Name;

            //Check Quest IP
            Boolean good = CheckIP();
            if (!good)
            {
                txtbox.AppendText("\n\nChoose a valid IP!");
                running = false;
                return;
            }

            //Create all Backup Folders
            Boolean good2 = BackupFSet();
            if (!good2)
            {
                txtbox.AppendText("\n\nThis Backup already exists!");
                running = false;
                return;
            }

            //Scores
            txtbox.AppendText("\n\nBacking up scores");
            adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/LocalDailyLeaderboards.dat \"" + Scores + "\"");
            adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/LocalLeaderboards.dat \"" + Scores + "\"");
            adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/PlayerData.dat \"" + Scores + "\"");
            adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/AvatarData.dat \"" + Scores + "\"");
            txtbox.AppendText("\nBacked up scores\n");
            txtbox.ScrollToEnd();

            //Songs

            QSE();
            //adb("pull /sdcard/BMBFData/CustomSongs \"" + BackupF + "\"");

            //Playlists

            PlaylistB();
            adb("pull /sdcard/BMBFData/Playlists/ \"" + Playlists + "\"");
            txtbox.ScrollToEnd();

            //Replays

            txtbox.AppendText("\n\nBacking up replays");
            adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/replays \"" + BackupF + "\"");
            txtbox.AppendText("\nBacked up replays\n");
            txtbox.ScrollToEnd();

            //Sounds

            txtbox.AppendText("\n\nBacking up sounds");
            adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/sounds \"" + BackupF + "\"");
            txtbox.AppendText("\nBacked up sounds\n");
            txtbox.ScrollToEnd();

            //Mods

            ModsB();


            //Mod cfgs
            txtbox.AppendText("\n\nBacking up Mod Configs");
            adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/mod_cfgs \"" + BackupF + "\"");
            txtbox.AppendText("\nBacked up Mod Configs\n");

            txtbox.AppendText("\n\n\nBMBF and Beat Saber Backup has been made.");
            txtbox.ScrollToEnd();
            running = false;
        }

        public void Backup(object sender, RoutedEventArgs e)
        {
            StartBMBF();
            if (running)
            {
                running = false;
                return;
            }
            running = true;

            //Check Quest IP
            Boolean good = CheckIP();
            if (!good)
            {
                txtbox.AppendText("\n\nChoose a valid IP!");
                running = false;
                return;
            }

            //Create all Backup Folders
            Boolean good2 = BackupFSet();
            if (!good2)
            {
                txtbox.AppendText("\n\nThis Backup already exists!");
                running = false;
                return;
            }

            //Scores
            txtbox.AppendText("\n\nBacking up scores");
            adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/LocalDailyLeaderboards.dat \"" + Scores + "\"");
            adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/LocalLeaderboards.dat \"" + Scores + "\"");
            adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/PlayerData.dat \"" + Scores + "\"");
            adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/AvatarData.dat \"" + Scores + "\"");
            txtbox.AppendText("\nBacked up scores\n");
            txtbox.ScrollToEnd();

            //Songs

            QSE();
            //adb("pull /sdcard/BMBFData/CustomSongs \"" + BackupF + "\"");

            //Playlists

            PlaylistB();
            adb("pull /sdcard/BMBFData/Playlists/ \"" + Playlists + "\"");
            txtbox.ScrollToEnd();

            //Replays

            txtbox.AppendText("\n\nBacking up replays");
            adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/replays \"" + BackupF + "\"");
            txtbox.AppendText("\nBacked up replays\n");
            txtbox.ScrollToEnd();

            //Sounds

            txtbox.AppendText("\n\nBacking up sounds");
            adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/sounds \"" + BackupF + "\"");
            txtbox.AppendText("\nBacked up sounds\n");
            txtbox.ScrollToEnd();

            //Mods

            ModsB();


            //Mod cfgs
            txtbox.AppendText("\n\nBacking up Mod Configs");
            adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/mod_cfgs \"" + BackupF + "\"");
            txtbox.AppendText("\nBacked up Mod Configs\n");

            txtbox.AppendText("\n\n\nBMBF and Beat Saber Backup has been made.");
            txtbox.ScrollToEnd();
            running = false;
        }

        public void selectBackup(String Name)
        {
            for(int i = 0; Backups.Items.Count > i; i++)
            {
                Backups.SelectedIndex = i + 1;
                if(Backups.SelectedValue.ToString() == Name)
                {
                    Backups.SelectedIndex = i + 1;
                    break;
                }
            }
        }

        public void Restore(object sender, RoutedEventArgs e)
        {
            StartBMBF();
            if (running)
            {
                return;
            }
            running = true;

            if (Backups.SelectedIndex == 0)
            {
                txtbox.AppendText("\n\nSelect a valid Backup!");
                running = false;
                return;
            }

            //Get Backup Folders
            BackupFGet();

            //Check Quest IP
            Boolean good = CheckIP();
            if (!good)
            {
                txtbox.AppendText("\n\nChoose a valid IP!");
                running = false;
                return;
            }

            //Scores
            if ((bool)RScores.IsChecked == true)
            {
                txtbox.AppendText("\n\nPushing Scores");
                adb("push \"" + Scores + "\\LocalDailyLeaderboards.dat\" /sdcard/Android/data/com.beatgames.beatsaber/files/LocalDailyLeaderboards.dat");
                adb("push \"" + Scores + "\\LocalLeaderboards.dat\" /sdcard/Android/data/com.beatgames.beatsaber/files/LocalLeaderboards.dat");
                adb("push \"" + Scores + "\\PlayerData.dat\" /sdcard/Android/data/com.beatgames.beatsaber/files/PlayerData.dat");
                adb("push \"" + Scores + "\\AvatarData.dat\" /sdcard/Android/data/com.beatgames.beatsaber/files/AvatarData.dat");
                txtbox.AppendText("\nPushed Scores");
                txtbox.ScrollToEnd();
            }

            //Replays
            if ((bool)RReplays.IsChecked)
            {
                txtbox.AppendText("\n\nPushing Replays");
                adb("push \"" + BackupF + "//replays\" /sdcard/Android/data/com.beatgames.beatsaber/files/");
                txtbox.AppendText("\nFinished Pushing Replays");
                txtbox.ScrollToEnd();
            }

            //Sounds
            if ((bool)RSounds.IsChecked)
            {
                txtbox.AppendText("\n\nPushing Sounds");
                adb("push \"" + BackupF + "//sounds\" /sdcard/Android/data/com.beatgames.beatsaber/files/");
                txtbox.AppendText("\nFinished Pushing Sounds");
                txtbox.ScrollToEnd();
            }

            //Songs
            if ((bool)RSongs.IsChecked)
            {
                if (CheckVer() == 0)
                {
                    txtbox.AppendText("\n\nUploading Songs");
                    Upload(Songs);
                    txtbox.AppendText("\nUploaded Songs");
                }
                else if (CheckVer() == 1)
                {
                    txtbox.AppendText("\nPushing Songs");
                    adb("push \"" + Songs + "\" /sdcard/BMBFData");
                    txtbox.AppendText("\nPushed Songs");
                }
                txtbox.ScrollToEnd();
            }

            //Playlists
            if ((bool)RPlaylists.IsChecked)
            {
                PlaylistsR();
                PushPNG(Playlists + "\\Playlists");
                txtbox.ScrollToEnd();
            }

            //Mods
            if ((bool)RMods.IsChecked)
            {
                txtbox.AppendText("\n\nUploading Mods");
                Upload(Mods);
                txtbox.AppendText("\nUploaded Mods");
                txtbox.ScrollToEnd();
            }

            if ((bool)RConfigs.IsChecked)
            {
                txtbox.AppendText("\n\nPushing Configs");
                adb("push \"" + BackupF + "\\mod_cfgs\" /sdcard/Android/data/com.beatgames.beatsaber/files");
                txtbox.AppendText("\nPushed Configs");
                txtbox.ScrollToEnd();
            }

            txtbox.AppendText("\n\n\nBMBF and Beat Saber has been restored with the selected components.");
            txtbox.ScrollToEnd();
            running = false;
        }



        public String adbS(String Argument)
        {
            String User = System.Environment.GetEnvironmentVariable("USERPROFILE");
            ProcessStartInfo s = new ProcessStartInfo();
            s.CreateNoWindow = false;
            s.UseShellExecute = false;
            s.FileName = "adb.exe";
            s.WindowStyle = ProcessWindowStyle.Hidden;
            s.RedirectStandardOutput = true;
            s.Arguments = Argument;
            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(s))
                {
                    String IPS = exeProcess.StandardOutput.ReadToEnd();
                    exeProcess.WaitForExit();
                    return IPS;
                }
            }
            catch
            {

                ProcessStartInfo se = new ProcessStartInfo();
                se.CreateNoWindow = false;
                se.UseShellExecute = false;
                se.FileName = User + "\\AppData\\Roaming\\SideQuest\\platform-tools\\adb.exe";
                se.WindowStyle = ProcessWindowStyle.Hidden;
                se.RedirectStandardOutput = true;
                se.Arguments = Argument;
                try
                {
                    // Start the process with the info we specified.
                    // Call WaitForExit and then the using statement will close.
                    using (Process exeProcess = Process.Start(se))
                    {
                        String IPS = exeProcess.StandardOutput.ReadToEnd();
                        exeProcess.WaitForExit();
                        return IPS;

                    }
                }
                catch
                {
                    // Log error.
                    txtbox.AppendText("\n\n\nAn error Occured (Code: ADB100). Check following");
                    txtbox.AppendText("\n\n- Your Quest is connected and USB Debugging enabled.");
                    txtbox.AppendText("\n\n- You have adb installed.");
                }
            }
            return "";
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

            if (FIP == "")
            {
                return;
            }
            MainWindow.IP = FIP;
            Quest.Text = FIP;
            if (MainWindow.IP == "")
            {
                Quest.Text = "Quest IP";
            }

        }

        public void StartBMBF()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                adb("shell am start -n com.weloveoculus.BMBF/com.weloveoculus.BMBF.MainActivity");
            }));
        }

        public int CheckVer()
        {
            String[] directories = Directory.GetFiles(Songs);



            for (int i = 0; i < directories.Length; i++)
            {
                if (directories[i].EndsWith(".zip"))
                {
                    return 0;
                }
            }
            return 1;
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

            int count = 0;
            for (int i = 0; i < MainWindow.IP.Length; i++)
            {
                if (MainWindow.IP.Substring(i, 1) == ".")
                {
                    count++;
                }
            }
            if (count != 3)
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                    Quest.Text = MainWindow.IP;
                }));
                return false;
            }
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                Quest.Text = MainWindow.IP;
            }));
            return true;
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
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                }
            }
        }

        public void PlaylistsR()
        {
            try
            {
                getQuestIP();


                String PlaylistsX;

                txtbox.AppendText("\n\nRestoring Playlist from " + Playlists + "\\Playlists.json");
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));



                if (!Directory.Exists(exe + "\\tmp"))
                {
                    Directory.CreateDirectory(exe + "\\tmp");
                }
                using (WebClient client = new WebClient())
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                        client.DownloadFile("http://" + MainWindow.IP + ":50000/host/beatsaber/config", exe + "\\tmp\\OConfig.json");
                    }));

                }

                String Config = exe + "\\tmp\\OConfig.json";

                PlaylistsX = Playlists + "\\Playlists.json";

                var j = JSON.Parse(File.ReadAllText(Config));
                var p = JSON.Parse(File.ReadAllText(PlaylistsX));

                j["Config"]["Playlists"] = p["Playlists"];
                File.WriteAllText(exe + "\\tmp\\config.json", j["Config"].ToString());

                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                    postChanges(exe + "\\tmp\\config.json");
                }));
                txtbox.AppendText("\n\nRestored old Playlists.");
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
            }
            catch
            {
                txtbox.AppendText("\n\n\nAn error occured (Code: BMBF100). Couldn't access BMBF Check following:");
                txtbox.AppendText("\n\n- Your Quest is on and BMBF opened");
                txtbox.AppendText("\n\n- You put in the Quests IP right.");
            }
        }

        public void postChanges(String Config)
        {
            using (WebClient client = new WebClient())
            {
                client.QueryString.Add("foo", "foo");
                client.UploadFile("http://" + MainWindow.IP + ":50000/host/beatsaber/config", "PUT", Config);
                client.UploadValues("http://" + MainWindow.IP + ":50000/host/beatsaber/commitconfig", "POST", client.QueryString);
            }
        }
        public void Sync()
        {
            System.Threading.Thread.Sleep(2000);
            using (WebClient client = new WebClient())
            {
                client.QueryString.Add("foo", "foo");
                client.UploadValues("http://" + MainWindow.IP + ":50000/host/beatsaber/commitconfig", "POST", client.QueryString);
            }
        }

        public void Upload(String Path)
        {
            getQuestIP();
            String[] directories = Directory.GetFiles(Path);


            for (int i = 0; i < directories.Length; i++)
            {
                WebClient client = new WebClient();

                txtbox.AppendText("\n\nUploading " + directories[i] + " to BMBF");
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                    try
                    {
                        client.UploadFile("http://" + MainWindow.IP + ":50000/host/beatsaber/upload?overwrite", directories[i]);
                        return;
                    }
                    catch
                    {
                        txtbox.AppendText("\n\n\nAn error occured (Code: BMBF100). Couldn't access BMBF. Check following:");
                        txtbox.AppendText("\n\n- You put in the Quests IP right.");
                        txtbox.AppendText("\n\n- Your Quest is on and BMBF is opened.");
                    }
                }));

                if (i % 20 == 0 && i != 0)
                {
                    txtbox.AppendText("\n\nSyncing to Beat Saber");
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                    Sync();
                    System.Threading.Thread.Sleep(2000);
                }
            }
            Sync();
        }

        public void BackupFGet()
        {

            BackupF = exe + "\\BBBUBackups\\" + Backups.SelectedValue;
            Songs = BackupF + "\\CustomSongs";
            Mods = BackupF + "\\Mods";
            Scores = BackupF + "\\Scores";
            Playlists = BackupF + "\\Playlists";
        }

        public void ModsB()
        {
            ArrayList list = new ArrayList();
            int overwritten = 0;
            int exported = 0;
            String Name = "";
            String Source = "";

            txtbox.AppendText("\nCopying all Mods to " + exe + "\\tmp. Please be patient.");
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
            adb("pull /sdcard/BMBFData/Mods/ \"" + exe + "\\tmp\"");
            if (Directory.Exists(exe + "\\tmp\\Mods"))
            {
                Source = exe + "\\tmp\\Mods";
            }
            else
            {
                Source = exe + "\\tmp";
            }

            string[] directories = Directory.GetDirectories(Source);



            for (int i = 0; i < directories.Length; i++)
            {
                txtbox.AppendText("\n");

                try
                {
                    String dat = directories[i] + "\\" + "bmbfmod.json";
                    StreamReader reader = new StreamReader(@dat);
                    String line;
                    while ((line = reader.ReadLine()) != null)
                    {

                        if (line.Contains("\"name\":"))
                        {
                            Name = Strings(line, 3);

                            Name = Name.Substring(0, Name.Length - 1);

                            Name = Name.Replace("/", "");
                            Name = Name.Replace(":", "");
                            Name = Name.Replace("*", "");
                            Name = Name.Replace("?", "");
                            Name = Name.Replace("\"", "");
                            Name = Name.Replace("<", "");
                            Name = Name.Replace(">", "");
                            Name = Name.Replace("|", "");

                            for (int f = 0; f < Name.Length; f++)
                            {
                                if (Name.Substring(f, 1).Equals("\\"))
                                {
                                    Name = Name.Substring(0, f - 1) + Name.Substring(f + 1, Name.Length - f - 1);
                                }
                            }
                            int Time = 0;
                            while (Name.Substring(Name.Length - 1, 1).Equals(" "))
                            {
                                Name = Name.Substring(0, Name.Length - 1);
                            }

                            while (list.Contains(Name.ToLower()))
                            {
                                Time++;
                                if (Time > 1)
                                {
                                    Name = Name.Substring(0, Name.Length - 1);
                                    Name = Name + Time;
                                }
                                else
                                {
                                    Name = Name + " " + Time;
                                }

                            }
                            list.Add(Name.ToLower());
                            txtbox.AppendText("\nMod Name: " + Name);
                            txtbox.AppendText("\nFolder: " + directories[i]);

                            bool v = File.Exists(Mods + "\\" + Name + ".zip");
                            if (v)
                            {
                                File.Delete(Mods + "\\" + Name + ".zip");
                                txtbox.AppendText("\noverwritten file: " + Mods + "\\" + Name + ".zip");

                                overwritten++;
                            }

                            zip(directories[i], Mods + "\\" + Name + ".zip");
                            exported++;
                            Name = "";
                        }
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                        txtbox.ScrollToEnd();

                    }
                    reader.Close();
                }
                catch
                {

                }


            }

            txtbox.AppendText("\n");
            txtbox.AppendText("\n");
            txtbox.AppendText("\nFinished! Backed up " + exported + " Mods");
            txtbox.ScrollToEnd();
        }

        public void PlaylistB()
        {
            try
            {
                getQuestIP();

                txtbox.AppendText("\n\nBacking up Playlist to " + Playlists + "\\Playlists.json");
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));



                if (!Directory.Exists(exe + "\\tmp"))
                {
                    Directory.CreateDirectory(exe + "\\tmp");
                }
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile("http://" + MainWindow.IP + ":50000/host/beatsaber/config", exe + "\\tmp\\Config.json");
                }


                String Config = exe + "\\tmp\\Config.json";

                var j = JSON.Parse(File.ReadAllText(Config));
                File.WriteAllText(Playlists + "\\Playlists.json", j["Config"].ToString()); 
                txtbox.AppendText("\n\nBacked up Playlists to " + Playlists + "Playlists.json");
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
            }
            catch
            {
                txtbox.AppendText("\n\n\nAn error occured (Code: BMBF100). Couldn't access BMBF. Check following:");
                txtbox.AppendText("\n\n- You put in the Quests IP right.");
                txtbox.AppendText("\n\n- Your Quest is on and BMBF opened.");

            }
            getBackups();
        }

        public void QSE()
        {
            ArrayList list = new ArrayList();
            ArrayList content = new ArrayList();
            ArrayList over = new ArrayList();
            int exported = 0;
            String Name = "";
            String Source = "";

            txtbox.AppendText("\nCopying all Songs to " + exe + "\\tmp. Please be patient.");
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
            adb("pull /sdcard/BMBFData/CustomSongs/ \"" + exe + "\\tmp\"");
            if (Directory.Exists(exe + "\\tmp\\CustomSongs"))
            {
                Source = exe + "\\tmp\\CustomSongs";
            }
            else
            {
                Source = exe + "\\tmp";
            }

            string[] directories = Directory.GetDirectories(Source);



            for (int i = 0; i < directories.Length; i++)
            {
                txtbox.AppendText("\n");


                if (!File.Exists(directories[i] + "\\" + "Info.dat") && !File.Exists(directories[i] + "\\" + "info.dat"))
                {
                    txtbox.AppendText("\n" + directories[i] + " is no Song");
                    continue;
                }
                String dat = "";
                if (File.Exists(directories[i] + "\\" + "Info.dat"))
                {
                    dat = directories[i] + "\\" + "Info.dat";

                }
                if (File.Exists(directories[i] + "\\" + "info.dat"))
                {
                    dat = directories[i] + "\\" + "info.dat";

                }
                try
                {
                    StreamReader reader = new StreamReader(@dat);
                    String text = "";
                    String line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        text = text + line;
                    }

                    var json = SimpleJSON.JSON.Parse(text);
                    Name = json["_songName"].ToString();

                    Name = Name.Replace("/", "");
                    Name = Name.Replace(":", "");
                    Name = Name.Replace("*", "");
                    Name = Name.Replace("?", "");
                    Name = Name.Replace("\"", "");
                    Name = Name.Replace("<", "");
                    Name = Name.Replace(">", "");
                    Name = Name.Replace("|", "");

                    for (int f = 0; f < Name.Length; f++)
                    {
                        if (Name.Substring(f, 1).Equals("\\"))
                        {
                            Name = Name.Substring(0, f - 1) + Name.Substring(f + 1, Name.Length - f - 1);
                        }
                    }
                    int Time = 0;
                    while (Name.Substring(Name.Length - 1, 1).Equals(" "))
                    {
                        Name = Name.Substring(0, Name.Length - 1);
                    }

                    while (list.Contains(Name.ToLower()))
                    {
                        Time++;
                        if (Time > 1)
                        {
                            Name = Name.Substring(0, Name.Length - 1);
                            Name = Name + Time;
                        }
                        else
                        {
                            Name = Name + " " + Time;
                        }

                    }
                    list.Add(Name.ToLower());
                    txtbox.AppendText("\nSong Name: " + Name);
                    txtbox.AppendText("\nFolder: " + directories[i]);



                    zip(directories[i], Songs + "\\" + Name + ".zip");
                    exported++;
                    Name = "";
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                    txtbox.ScrollToEnd();

                    reader.Close();
                }
                catch
                {

                }
            }
            txtbox.AppendText("\n");
            txtbox.AppendText("\n");

            if (exported == 0)
            {
                txtbox.AppendText("\nerror (Code: QSU110). ");
            }
            else
            {
                txtbox.AppendText("\nFinished! Backed up " + exported + " Songs.");
            }
            txtbox.ScrollToEnd();
        }

        public static void zip(String src, String Output)
        {
            ZipFile.CreateFromDirectory(src, Output);

        }

        public String Strings(String line, int StartIndex)
        {
            int count = 0;
            String Name = "";
            for (int n = 0; n < line.Length; n++)
            {
                if (count == StartIndex)
                {

                    Name = Name + line.Substring(n, 1);
                }

                if (line.Substring(n, 1).Equals("\""))
                {
                    count++;
                }
            }
            return Name;
        }

        public Boolean BackupFSet()
        {
            BName.Text = BName.Text.Replace("/", "");
            BName.Text = BName.Text.Replace(":", "");
            BName.Text = BName.Text.Replace("*", "");
            BName.Text = BName.Text.Replace("?", "");
            BName.Text = BName.Text.Replace("\"", "");
            BName.Text = BName.Text.Replace("<", "");
            BName.Text = BName.Text.Replace(">", "");
            BName.Text = BName.Text.Replace("|", "");

            for (int f = 0; f < BName.Text.Length; f++)
            {
                if (BName.Text.Substring(f, 1).Equals("\\"))
                {
                    BName.Text = BName.Text.Substring(0, f - 1) + BName.Text.Substring(f + 1, BName.Text.Length - f - 1);
                }
            }

            BackupF = exe + "\\BBBUBackups\\" + BName.Text;

            if (Directory.Exists(BackupF))
            {
                return false;
            }

            Songs = BackupF + "\\CustomSongs";
            Mods = BackupF + "\\Mods";
            Scores = BackupF + "\\Scores";
            Playlists = BackupF + "\\Playlists";

            if (!Directory.Exists(Songs))
            {
                Directory.CreateDirectory(Songs);
            }
            if (!Directory.Exists(Mods))
            {
                Directory.CreateDirectory(Mods);
            }
            if (!Directory.Exists(Scores))
            {
                Directory.CreateDirectory(Scores);
            }
            if (!Directory.Exists(Playlists))
            {
                Directory.CreateDirectory(Playlists);
            }
            return true;
        }

        public void getBackups()
        {
            ArrayList Folders = new ArrayList();
            string[] Files = Directory.GetDirectories(exe + "\\BBBUBackups");
            Backups.Items.Clear();
            Backups.Items.Add("Backups");

            for (int i = 0; i < Files.Length; i++)
            {
                Folders.Add(Files[i].Substring(Files[i].LastIndexOf("\\") + 1, Files[i].Length - 1 - Files[i].LastIndexOf("\\")));
            }

            for (int o = 0; o < Folders.Count; o++)
            {
                Backups.Items.Add(Folders[o]);
            }
            Backups.SelectedIndex = 0;
        }

        public void getQuestIP()
        {
            MainWindow.IP = Quest.Text;
        }

        public void adb(String Argument)
        {
            String User = System.Environment.GetEnvironmentVariable("USERPROFILE");
            ProcessStartInfo s = new ProcessStartInfo();
            s.CreateNoWindow = false;
            s.UseShellExecute = false;
            s.FileName = "adb.exe";
            s.WindowStyle = ProcessWindowStyle.Hidden;
            s.Arguments = Argument;
            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(s))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch
            {

                ProcessStartInfo se = new ProcessStartInfo();
                se.CreateNoWindow = false;
                se.UseShellExecute = false;
                se.FileName = User + "\\AppData\\Roaming\\SideQuest\\platform-tools\\adb.exe";
                se.WindowStyle = ProcessWindowStyle.Hidden;
                se.Arguments = Argument;
                try
                {
                    // Start the process with the info we specified.
                    // Call WaitForExit and then the using statement will close.
                    using (Process exeProcess = Process.Start(se))
                    {
                        exeProcess.WaitForExit();
                    }
                }
                catch
                {
                    // Log error.
                    txtbox.AppendText("\n\n\nAn error Occured (Code: ADB100). Check following");
                    txtbox.AppendText("\n\n- Your Quest is connected and USB Debugging enabled.");
                    txtbox.AppendText("\n\n- You have adb installed.");
                }

            }
        }


        private void Mini(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void QuestIPCheck(object sender, RoutedEventArgs e)
        {
            if (Quest.Text == "")
            {
                Quest.Text = "Quest IP";
            }
        }

        private void BackupNameCheck(object sender, RoutedEventArgs e)
        {
            if (BName.Text == "")
            {
                BName.Text = "Backup Name";
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
            if (Directory.Exists(exe + "\\tmp"))
            {
                Directory.Delete(exe + "\\tmp", true);
            }
            this.Close();
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

        private void ClearText(object sender, RoutedEventArgs e)
        {
            if (Quest.Text == "Quest IP")
            {
                Quest.Text = "";
            }

        }

        private void ClearTextN(object sender, RoutedEventArgs e)
        {
            if (BName.Text == "Backup Name")
            {
                BName.Text = "";
            }
        }
    }
}
