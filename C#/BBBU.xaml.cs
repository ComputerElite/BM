using Microsoft.WindowsAPICodePack.Dialogs;
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
        String APKs = "";
        String BackupF = "";
        JSONNode BackupConfig = JSON.Parse("{}");

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
            
            QuestIP();
            TransferFromBBBU();
            Convert();
            getBackups();

            RSongs.IsChecked = true;
            RPlaylists.IsChecked = true;
            RScores.IsChecked = true;
            RMods.IsChecked = true;
            RReplays.IsChecked = true;
            RSounds.IsChecked = true;
            RConfigs.IsChecked = true;
            RAPK.IsChecked = false;
            RAPK.Visibility = Visibility.Hidden;

            ChangeImage("BBBU2_B.png");
        }

        public void TransferFromBBBU()
        {
            if (MainWindow.BBBUTransfered) return;
            MainWindow.BBBUTransfered = true;
            MessageBoxResult r = MessageBox.Show("Hi. I'm asking you if I should import Backups from BMBF Beat Saber Backup Utility. Only click yes if you've used the seperate program before. You can always import again if you wish to from the settings.", "BMBF Manager - BMBF Beat Saber Backup Utility", MessageBoxButton.YesNo, MessageBoxImage.Question);
            switch (r)
            {
                case MessageBoxResult.No:
                    txtbox.AppendText("\n\nNothing Imported");
                    return;
            }
            MessageBox.Show("I'll open a window for you. Please choose the folder in which your BMBF Beat Saber Backup Utility Installation is located. I'll then transfer all Backups", "BMBF Manager - BMBF Beat Saber Backup Utility", MessageBoxButton.OK, MessageBoxImage.Information);

            CommonOpenFileDialog ofd = new CommonOpenFileDialog();
            ofd.IsFolderPicker = true;
            if (ofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                //Get the path of specified file
                if (Directory.Exists(ofd.FileName))
                {
                    foreach(String folder in Directory.GetDirectories(ofd.FileName))
                    {
                        Console.WriteLine(folder);
                        String backupName = new DirectoryInfo(folder).Name;
                        Directory.Move(folder, exe + "\\BBBUBackups\\" + backupName);
                        txtbox.AppendText("\n\nMoved Backup " + backupName);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a valid Directory", "BMBF Manager - BMBF Beat Saber backup Utility", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

            }

            txtbox.AppendText("\n\nAll Backups moved");
        }

        public void Convert()
        {
            foreach(String file in Directory.GetDirectories(exe + "\\BBBUBackups"))
            {
                if (file.EndsWith(".json"))
                {
                    String contents = File.ReadAllText(file);
                    if (contents.EndsWith(","))
                    {
                        contents = contents.Substring(0, contents.Length - 1) + "}}";
                        JSONNode c = JSON.Parse(contents);
                        File.Delete(file);
                        File.WriteAllText(file, c["Config"].ToString());
                    }
                }
            }
        }

        public void BackupLink(String Name)
        {
            StartBackup(Name, false);
        }

        public void ABackupLink(String Name)
        {
            StartBackup(Name, true);
        }

        public void Backup(object sender, RoutedEventArgs e)
        {
            StartBackup("", false);
        }

        public void ABackup(object sender, RoutedEventArgs e)
        {
            StartBackup("", true);
        }

        public void StartBackup(String BackupName, Boolean Advanced)
        {
            StartBMBF();
            if (running)
            {
                running = false;
                return;
            }
            running = true;

            if(BackupName != "")
            {
                BName.Text = Name;
            }

            //Check Quest IP
            if (!CheckIP())
            {
                txtbox.AppendText("\n\nChoose a valid IP!");
                running = false;
                return;
            }

            if(Advanced)
            {
                MessageBoxResult r = MessageBox.Show("This Backup Method will Backup the Beat Saber APK and BMBF APK as well. If you don't make another Backup before you restore this Backup you have to mod Beat Saber again. Only do this when you know what you're doing. Note: This has only been tested on Quest 1. If you are on Quest 2 feel free to contact me and say if it worked.\nDo you want to continue?", "BMBF Manager - BMBF Beat Saber Backup Utility", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (r)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\nBackup Aborted.");
                        return;
                }
            }

            //Create all Backup Folders
            if (!BackupFSet())
            {
                txtbox.AppendText("\n\nThis Backup already exists!");
                running = false;
                return;
            }

            BackupConfig = JSON.Parse("{}");

            //Scores
            txtbox.AppendText("\n\nBacking up scores");
            adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/LocalDailyLeaderboards.dat \"" + Scores + "\"");
            adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/LocalLeaderboards.dat \"" + Scores + "\"");
            adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/PlayerData.dat \"" + Scores + "\"");
            adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/AvatarData.dat \"" + Scores + "\"");
            adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/settings.cfg \"" + Scores + "\"");
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

            if (Advanced)
            {
                BackupAPK();
                if(!BackupConfig["BMBFBackup"])
                {
                    MessageBox.Show("I couldn't make a BMBF APK Backup. If you want to restore to this game Version you may want to install the right BMBF Version.", "BMBF Manager - BMBF Beat Saber Backup Utility", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                if (!BackupConfig["BSBackup"])
                {
                    MessageBox.Show("I couldn't make a Beat Saber APK Backup. If you want to restore to this game Version it will not work.", "BMBF Manager - BMBF Beat Saber Backup Utility", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            if(Advanced)
            {
                BackupConfig["BackupType"] = 1;
            } else
            {
                BackupConfig["BackupType"] = 0;
            }
            File.WriteAllText(BackupF + "\\Backup.json", BackupConfig.ToString());

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
            if (!CheckIP())
            {
                txtbox.AppendText("\n\nChoose a valid IP!");
                running = false;
                return;
            }

            if ((bool)RAPK.IsChecked)
            {
                MessageBoxResult r = MessageBox.Show("You choose to restore the Beat Saber APK. This will install another Beat Saber version. If you didn't make a Backup you have to mod Beat Saber again. If you want to go back to the current Beat Saber Version make a Advanced Backup first. Also if you restore, BMBF Mods will be messed up a bit. Note: This has only been tested on Quest 1. If you are on Quest 2 feel free to contact me and say if it worked.\nDo you wish to abort?", "BMBF Manager - BMBF Beat Saber Backup Utility", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (r)
                {
                    case MessageBoxResult.Yes:
                        txtbox.AppendText("\n\nRestoring aborted.");
                        return;
                }
            }

            //APKs
            if ((bool)RAPK.IsChecked)
            {
                RestoreAPK();
            }

            //Scores
            if ((bool)RScores.IsChecked == true && (bool)RAPK.IsChecked == false)
            {
                txtbox.AppendText("\n\nPushing Scores");
                adb("push \"" + Scores + "\\LocalDailyLeaderboards.dat\" /sdcard/Android/data/com.beatgames.beatsaber/files/LocalDailyLeaderboards.dat");
                adb("push \"" + Scores + "\\LocalLeaderboards.dat\" /sdcard/Android/data/com.beatgames.beatsaber/files/LocalLeaderboards.dat");
                adb("push \"" + Scores + "\\PlayerData.dat\" /sdcard/Android/data/com.beatgames.beatsaber/files/PlayerData.dat");
                adb("push \"" + Scores + "\\AvatarData.dat\" /sdcard/Android/data/com.beatgames.beatsaber/files/AvatarData.dat");
                adb("push \"" + Scores + "\\settings.cfg\" /sdcard/Android/data/com.beatgames.beatsaber/files/setting.cfg");
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

            //Mod Configs
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

        public void BackupAPK()
        {
            BackupConfig["BSBackup"] = false;
            BackupConfig["BMBFBackup"] = false;
            txtbox.AppendText("\n\nBacking up Beat Saber APK");
            String moddedBS = adbS("shell pm path com.beatgames.beatsaber").Replace("package:", "").Replace(System.Environment.NewLine, "");
            if (adb("pull " + moddedBS + " \"" + APKs + "\\BeatSaber.apk\""))
            {
                BackupConfig["BSBackup"] = true;
                if(!File.Exists(APKs + "\\BeatSaber.apk")) BackupConfig["BSBackup"] = false;
                txtbox.AppendText("\nBacked up Beat Saber APK");
            }

            txtbox.AppendText("\n\nBacking up BMBF APK");
            String BMBF = adbS("shell pm path com.weloveoculus.BMBF").Replace("package:", "").Replace(System.Environment.NewLine, "");
            if (adb("pull " + BMBF + " \"" + APKs + "\\BMBF.apk\""))
            {
                BackupConfig["BMBFBackup"] = true;
                if (!File.Exists(APKs + "\\BMBF.apk")) BackupConfig["BMBFBackup"] = false;
                txtbox.AppendText("\nBacked up BMBF APK");
            }

            txtbox.AppendText("\n\nBacking up Game Data");
            if (adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files \"" + BackupF + "\""))
            {
                txtbox.AppendText("\nBacked up Game Data");
            }
        }

        public void RestoreAPK()
        {
            if (!BackupConfig["BSBackup"].AsBool)
            {
                MessageBox.Show("You Backup doesn't contain any Beat Saber APK backup. I must abort to prevent anything from going wrong.", "BMBF Manager - BMBF Beat Saber Backup Utility", MessageBoxButton.OK, MessageBoxImage.Error);
                txtbox.AppendText("\n\nAPK Backup Restoring Aborted.");
                return;
            }
            txtbox.AppendText("\n\nBacking up scores");
            adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/LocalDailyLeaderboards.dat \"" + Scores + "\"");
            adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/LocalLeaderboards.dat \"" + Scores + "\"");
            adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/PlayerData.dat \"" + Scores + "\"");
            adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/AvatarData.dat \"" + Scores + "\"");
            adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/settings.cfg \"" + Scores + "\"");
            txtbox.AppendText("\nBacked up scores");
            txtbox.AppendText("\n\nInstalling Beat Saber");
            if (!adb("uninstall com.beatgames.beatsaber")) return;
            if (!adb("install \"" + APKs + "\\BeatSaber.apk\"")) return;
            txtbox.AppendText("\nInstalled Beat Saber");
            txtbox.AppendText("\n\nInstalling BMBF");
            if (!adb("uninstall com.weloveoculus.BMBF")) return;
            if (!adb("install \"" + APKs + "\\BMBF.apk\"")) return;
            txtbox.AppendText("\nInstalled BMBF");
            txtbox.AppendText("\n\nRestoring Game Data");
            if (!adb("push \"" + BackupF + "\\files\" /sdcard/Android/data/com.beatgames.beatsaber/")) return;
            txtbox.AppendText("\n\nPushing Scores");
            adb("push \"" + Scores + "\\LocalDailyLeaderboards.dat\" /sdcard/Android/data/com.beatgames.beatsaber/files/LocalDailyLeaderboards.dat");
            adb("push \"" + Scores + "\\LocalLeaderboards.dat\" /sdcard/Android/data/com.beatgames.beatsaber/files/LocalLeaderboards.dat");
            adb("push \"" + Scores + "\\PlayerData.dat\" /sdcard/Android/data/com.beatgames.beatsaber/files/PlayerData.dat");
            adb("push \"" + Scores + "\\AvatarData.dat\" /sdcard/Android/data/com.beatgames.beatsaber/files/AvatarData.dat");
            adb("push \"" + Scores + "\\AvatarData.dat\" /sdcard/Android/data/com.beatgames.beatsaber/files/setting.cfg");
            txtbox.AppendText("\nPushed Scores");
            txtbox.AppendText("\nRestored game Data");
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

            int count = MainWindow.IP.Split('.').Count();
            if (count != 4)
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
                WebClient client = new WebClient();

                PlaylistsX = Playlists + "\\Playlists.json";

                var j = JSON.Parse(client.DownloadString("http://" + MainWindow.IP + ":50000/host/beatsaber/config"));
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
            APKs = BackupF + "\\APK";
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


            foreach (String cd in Directory.GetDirectories(Source))
            {
                txtbox.AppendText("\n");

                try
                {
                    JSONNode bmbfmod = JSON.Parse(File.ReadAllText(cd + "\\" + "bmbfmod.json"));
                    Name = bmbfmod["name"];

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
                    txtbox.AppendText("\nFolder: " + cd);

                    bool v = File.Exists(Mods + "\\" + Name + ".zip");
                    if (v)
                    {
                        File.Delete(Mods + "\\" + Name + ".zip");
                        txtbox.AppendText("\noverwritten file: " + Mods + "\\" + Name + ".zip");

                        overwritten++;
                    }

                    zip(cd, Mods + "\\" + Name + ".zip");
                    exported++;
                    Name = "";
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                    txtbox.ScrollToEnd();
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
                WebClient client = new WebClient();

                var j = JSON.Parse(client.DownloadString("http://" + MainWindow.IP + ":50000/host/beatsaber/config"));
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

            foreach (String cd in Directory.GetDirectories(Source))
            {
                txtbox.AppendText("\n");


                if (!File.Exists(cd + "\\" + "Info.dat") && !File.Exists(cd + "\\" + "info.dat"))
                {
                    txtbox.AppendText("\n" + cd + " is no Song");
                    continue;
                }
                String dat = "";
                if (File.Exists(cd + "\\" + "Info.dat"))
                {
                    dat = cd + "\\" + "Info.dat";

                }
                if (File.Exists(cd + "\\" + "info.dat"))
                {
                    dat = cd + "\\" + "info.dat";

                }
                try
                {
                    var json = SimpleJSON.JSON.Parse(File.ReadAllText(dat));
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
                    txtbox.AppendText("\nFolder: " + cd);



                    zip(cd, Songs + "\\" + Name + ".zip");
                    exported++;
                    Name = "";
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                    txtbox.ScrollToEnd();
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
            APKs = BackupF + "\\APK";

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
            if (!Directory.Exists(APKs))
            {
                Directory.CreateDirectory(APKs);
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

        private void GetBackupInfo(object sender, SelectionChangedEventArgs e)
        {
            if (Backups.SelectedIndex == 0) return;
            if(File.Exists(exe + "\\BBBUBackups\\" + Backups.SelectedValue + "\\Backup.json"))
            {
                BackupConfig = JSON.Parse(File.ReadAllText(exe + "\\BBBUBackups\\" + Backups.SelectedValue + "\\Backup.json"));
            } else
            {
                BackupConfig = JSON.Parse("{}");
                BackupConfig["BackupType"] = 0;
            }

            if(BackupConfig["BackupType"] == 0)
            {
                RAPK.Visibility = Visibility.Hidden;
                RAPK.IsChecked = false;
                //Change Background Image
                ChangeImage("BBBU2_B.png");
            } else
            {
                RAPK.Visibility = Visibility.Visible;
                RAPK.IsChecked = true;
                //Change Background Image
                ChangeImage("BBBU2_A.png");
            }
        }

        public void ChangeImage(String ImageName)
        {
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
                uniformBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/" + ImageName, UriKind.Absolute));
                uniformBrush.Stretch = Stretch.UniformToFill;
                this.Background = uniformBrush;
            }
        }
    }
}
