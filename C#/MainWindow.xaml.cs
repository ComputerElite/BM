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
using System.Text.Json;
using BeatSaber.Stats;
using BMBF.Config;
using System.Text.RegularExpressions;
using BMBFManager.Language;
using ComputerUtils.RegxTemplates;
using DCRPManager;
using BMBFManager.Config;
using BMBFManager.Utils;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using Application = System.Windows.Application;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Path = System.IO.Path;
using System.Threading;

namespace BMBF_Manager
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static int MajorV = 1;
        public static int MinorV = 16;
        public static int PatchV = 5;
        public static bool Preview = false;
        public static bool log = false;

        public static ConfigFile config = new ConfigFile();
        Boolean draggable = true;
        Boolean Running = false;
        Boolean ComeFromUpdate = false;
        static String exe = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 1);
        public static String BMBF = "https://bmbf.dev/stable/27153984";
        public static JSONNode UpdateJSON = JSON.Parse("{}");
        JSONNode BMBFStable = JSON.Parse("{}");
        bool Quest2 = false;
        public static PresenceManager DCRPM;
        public static ADBInteractor aDBI = new ADBInteractor();
        public static IPUtils iPUtils = new IPUtils();
        public static BMBFUtils bMBFUtils = new BMBFUtils();
        public static List<String> lastActionsBeforeException = new List<string>();

        public static Language globalLanguage = new Language();

        public MainWindow()
        {
            InitializeComponent();
            CheckExecutionDir();

            //SetupExceptionHandlers();

            if (!Directory.Exists(exe + "\\Backups")) Directory.CreateDirectory(exe + "\\Backups");
            if (!Directory.Exists(exe + "\\Backup")) Directory.CreateDirectory(exe + "\\Backup");
            if (Directory.Exists(exe + "\\ModChecks")) Directory.Delete(exe + "\\ModChecks", true);
            if (!Directory.Exists(exe + "\\ModChecks")) Directory.CreateDirectory(exe + "\\ModChecks");
            if (!Directory.Exists(exe + "\\tmp")) Directory.CreateDirectory(exe + "\\tmp");
            if (!Directory.Exists(exe + "\\languages")) Directory.CreateDirectory(exe + "\\languages");
            if (File.Exists(exe + "\\BM_Update.exe")) File.Delete(exe + "\\BM_Update.exe");

            loadConfig();
            SetupLanguage();

            //MakeRandomLanguageFile();

            UpdateB.Visibility = Visibility.Hidden;
            txtbox.Text = globalLanguage.global.defaultOutputBoxText;
            Update();
            StartBMBF();
            QuestIP();
            Quest.Text = config.IP;
            if (config.CustomImage)
            {
                ImageBrush uniformBrush = new ImageBrush();
                uniformBrush.ImageSource = new BitmapImage(new Uri(config.CustomImageSource, UriKind.Absolute));
                uniformBrush.Stretch = Stretch.UniformToFill;
                this.Background = uniformBrush;
            }
            else
            {
                ImageBrush uniformBrush = new ImageBrush();
                uniformBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Main11.png", UriKind.Absolute));
                uniformBrush.Stretch = Stretch.UniformToFill;
                this.Background = uniformBrush;
            }
            Changelog();
            ComeFromUpdate = false;
            CheckBMBFUpdate();

            DCRPM = new PresenceManager("812060183407886376", config.DCRPE);
            DCRPM.SetOneButton(globalLanguage.dCRP.gitHubLink, "https://github.com/ComputerElite/BM");
            DCRPM.SetActivity(globalLanguage.dCRP.inMainMenu);

            //TryGetStats();
            KeepAliveTask();
            
        }

        public static void SetLastActionBeforeException(String msg, [CallerMemberName] String callerName = "N/A")
        {
            lastActionsBeforeException.Add("[" + callerName + "] " + msg);
            if (lastActionsBeforeException.Count > 20) lastActionsBeforeException.RemoveAt(lastActionsBeforeException.Count - 1);
        }

        public void CheckExecutionDir()
        {
            if (RegexTemplates.IsInSystemFolder(exe))
            {
                MessageBox.Show(globalLanguage.processer.ReturnProcessed(globalLanguage.mainMenu.code.systemDirWarning, RegexTemplates.SystemDirFolderRegex), "BMBF Manager", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        public void SetupExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            HandleExtenption((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

            Application.Current.DispatcherUnhandledException += (s, e) =>
            {
                HandleExtenption(e.Exception, "Application.Current.DispatcherUnhandledException");
                e.Handled = true;
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                HandleExtenption(e.Exception, "TaskScheduler.UnobservedTaskException");
                e.SetObserved();
            };
        }

        public void HandleExtenption(Exception e, string source)
        {
            DateTime t = DateTime.Now;
            String Save = "\n\nCrash of BMBF Manager has been catched at " + t.Day.ToString("d2") + "." + t.Month.ToString("d2") + "." + t.Year.ToString("d4") + "   " + t.Hour.ToString("d2") + ":" + t.Minute.ToString("d2") + ":" + t.Second.ToString("d2") + "." + t.Millisecond.ToString("d5");
            Save += "\nUseful information:";
            Save += "\n- IP (Quest private IP): " + config.IP;
            Save += "\n- Version: " + MajorV + "." + MinorV + "." + PatchV + "   Preview Version: " + Preview.ToString() + "   Logging enabled: " + log.ToString();
            Save += "\n- On Quest 2 (probably not right): " + Quest2.ToString();
            Save += "\n- Execution directory (Usernames Removed): " + RegexTemplates.RemoveUserName(exe);
            Save += "\n- Language: " + globalLanguage.language + " by " + globalLanguage.translator;
            Save += "\nLast actions before Exception (not every function has this implemented so it might be wrong; up to 20):\n\n" + String.Join("\n- ", lastActionsBeforeException);
            Save += "\n\nException (Usernames Removed):\n   " + RegexTemplates.RemoveUserName(e.ToString());
            File.AppendAllText(exe + "\\Crash.log", Save);
            MessageBoxResult r = MessageBox.Show(globalLanguage.mainMenu.code.Exception + "\n\n" + e.ToString(), "BMBF Manager - Exception Reporter", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            if (r == MessageBoxResult.Cancel) return;
            RealClose();
        }

        private void MakeRandomLanguageFile()
        {
            //Generate random text for testing and yes ik that's bad code
            JSONNode n = JSON.Parse(JsonSerializer.Serialize(globalLanguage));
            JSONNode random = JSON.Parse("{}");
            Random r = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            foreach (KeyValuePair<string, JSONNode> n2 in n)
            {
                if(n2.Value.IsString) random[n2.Key] = new string(Enumerable.Repeat(chars, 10).Select(s => s[r.Next(s.Length)]).ToArray());
                else
                {
                    foreach (KeyValuePair<string, JSONNode> n3 in n[n2.Key])
                    {
                        if (n3.Value.IsString) random[n2.Key][n3.Key] = new string(Enumerable.Repeat(chars, 10).Select(s => s[r.Next(s.Length)]).ToArray());
                        else
                        {
                            foreach (KeyValuePair<string, JSONNode> n4 in n[n2.Key][n3.Key])
                            {
                                if (n4.Value.IsString) random[n2.Key][n3.Key][n4.Key] = new string(Enumerable.Repeat(chars, 10).Select(s => s[r.Next(s.Length)]).ToArray());
                                else
                                {
                                    foreach (KeyValuePair<string, JSONNode> n5 in n[n2.Key][n3.Key][n4.Key])
                                    {
                                        random[n2.Key][n3.Key][n4.Key][n5.Key] = new string(Enumerable.Repeat(chars, 10).Select(s => s[r.Next(s.Length)]).ToArray());
                                    }
                                }
                            }
                        }
                    }
                }
            }

            File.WriteAllText("D:\\random.json", random.ToString());
        }

        private void SetupLanguage()
        {
            if (config.language == "en" || !File.Exists(config.language))
            {
                config.language = "en";
                globalLanguage = new Language(); // Sets up english
                globalLanguage.translator = "ComputerElite";
            }
            else
            {
                globalLanguage = JsonSerializer.Deserialize<BMBFManager.Language.Language>(File.ReadAllText(config.language));
            }

            // Setup UI language for this window
            UpdateB.Content = globalLanguage.mainMenu.UI.updateButton;
            installSongsButton.Content = globalLanguage.mainMenu.UI.installSongsButton;
            installModsButton.Content = globalLanguage.mainMenu.UI.installModsButton;
            updateBMBFButton.Content = globalLanguage.mainMenu.UI.updateBMBFButton;
            switchButton.Content = globalLanguage.mainMenu.UI.switchButton;
            downloadBPListsButton.Content = globalLanguage.mainMenu.UI.downloadBPListsButton;
            openBMBFButton.Content = globalLanguage.mainMenu.UI.openBMBFButton;
            installSoundsButton.Content = globalLanguage.mainMenu.UI.installSoundsButton;
            bBBUButton.Content = globalLanguage.mainMenu.UI.bBBUButton;
            qSUButton.Content = globalLanguage.mainMenu.UI.qSUButton;
            installQosmeticsButton.Content = globalLanguage.mainMenu.UI.installQosmeticsButton;
            playlistEditorButton.Content = globalLanguage.mainMenu.UI.playlistEditorButton;
            settingsButton.Content = globalLanguage.mainMenu.UI.settingsButton;
            uploadToBMBFButton.Content = globalLanguage.mainMenu.UI.uploadToBMBFButton;

            try
            {
                File.WriteAllText("D:\\en.json", JsonSerializer.Serialize(globalLanguage));
            }
            catch { }
        }

        public static void Log(String s)
        {
            if (!log) return;
            DateTime d = DateTime.Now;
            File.AppendAllText(exe + "\\log.log", d.Hour.ToString("d2") + ":" + d.Minute.ToString("d2") + ":" + d.Second.ToString("d2") + "." + d.Millisecond.ToString("d5") + ":     " + s + "\n");
        }

        private async Task KeepAliveTask()
        {
            while(true)
            {
                if(config.KeepAlive)
                {
                    aDBI.adb("shell input keyevent KEYCODE_WAKEUP", txtbox);
                }
                await Task.Delay(15000);
            }
        }

        private async void CheckBMBFUpdate()
        {
            List<BMBFStableVersions> stable = new List<BMBFStableVersions>();
            BMBFlocal local = new BMBFlocal();
            try
            {
                TimeoutWebClientShort c = new TimeoutWebClientShort();
                String l = await c.DownloadStringTaskAsync(new Uri("http://" + config.IP + ":50000/host/version/local"));
                String n = await c.DownloadStringTaskAsync(new Uri("https://bmbf.dev/stable/json"));
                local = JsonSerializer.Deserialize<BMBFlocal>(l);
                stable = JsonSerializer.Deserialize<List<BMBFStableVersions>>(n);
                int[] vn = Array.ConvertAll(local.version.Split('.'), s => int.Parse(s));

                String v = stable[0].tag.Replace("v", "");
                int[] vl = Array.ConvertAll(v.Split('.'), s => int.Parse(s));

                if ((vn[0] >= vl[0] && vn[1] >= vl[1] && vn[2] > vl[2]) || (vn[0] >= vl[0] && vn[1] > vl[1]) || (vn[0] > vl[0]))
                {
                    txtbox.AppendText("\n\n" + globalLanguage.processer.ReturnProcessed(globalLanguage.mainMenu.code.newBMBFAvailable, local.version, v));
                }
                else txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.onNewestBMBF);
            } catch {  }
        }

        private void TryGetStats()
        {
            txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.tryPullPlayerStats);
            if(!aDBI.adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/PlayerData.dat \"" + exe + "\\tmp\\PlayerData.dat\"", txtbox))
            {
                txtbox.AppendText("\n" + globalLanguage.mainMenu.code.questNotConnectedNoPlayerStats);
                return;
            }
            if(!File.Exists(exe + "\\tmp\\PlayerData.dat"))
            {
                txtbox.AppendText("\n" + globalLanguage.mainMenu.code.pullPlayerStatsFailed);
                return;
            }
            PlayerData stats = JsonSerializer.Deserialize<PlayerData>(File.ReadAllText(exe + "\\tmp\\PlayerData.dat"));
            if(stats.localPlayers.Count < 1)
            {
                txtbox.AppendText("\n" + globalLanguage.mainMenu.code.noPlayerStatsSaved);
                return;
            }
            txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.overallStats);
            txtbox.AppendText("\n" + globalLanguage.mainMenu.code.goodCuts + stats.localPlayers[0].playerAllOverallStatsData.overallGoodCutsCount);
            txtbox.AppendText("\n" + globalLanguage.mainMenu.code.badCuts + stats.localPlayers[0].playerAllOverallStatsData.overallBadCutsCount);
            txtbox.AppendText("\n" + globalLanguage.mainMenu.code.missedCuts + stats.localPlayers[0].playerAllOverallStatsData.overallMissedCutsCount);
            txtbox.AppendText("\n" + globalLanguage.mainMenu.code.totalScore + stats.localPlayers[0].playerAllOverallStatsData.overallTotalScore);
            txtbox.AppendText("\n" + globalLanguage.mainMenu.code.totalPlayTime + (stats.localPlayers[0].playerAllOverallStatsData.overallTimePlayed > 60.0 ? Math.Round(stats.localPlayers[0].playerAllOverallStatsData.overallTimePlayed / 60, 3) + " hours" : Math.Round(stats.localPlayers[0].playerAllOverallStatsData.overallTimePlayed, 2) + " minutes").Replace(",", "."));
            txtbox.AppendText("\n " + globalLanguage.mainMenu.code.totalHandDistance + (stats.localPlayers[0].playerAllOverallStatsData.overallHandDistanceTravelled > 1000.0 ? Math.Round(stats.localPlayers[0].playerAllOverallStatsData.overallHandDistanceTravelled / 1000, 3) + " km" : Math.Round(stats.localPlayers[0].playerAllOverallStatsData.overallHandDistanceTravelled, 2) + " metres").Replace(",", "."));
            txtbox.AppendText("\n" + globalLanguage.mainMenu.code.playedLevels + stats.localPlayers[0].playerAllOverallStatsData.overallPlayedLevelsCount);
            txtbox.AppendText("\n" + globalLanguage.mainMenu.code.clearedLevels + stats.localPlayers[0].playerAllOverallStatsData.overallCleardLevelsCount);
            txtbox.AppendText("\n" + globalLanguage.mainMenu.code.failedLevels + stats.localPlayers[0].playerAllOverallStatsData.overallFailedLevelsCount);
            txtbox.AppendText("\n" + globalLanguage.mainMenu.code.fullCombo + stats.localPlayers[0].playerAllOverallStatsData.overallFullComboCount);
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
                txtbox.AppendText("\n\n\n" + globalLanguage.processer.ReturnProcessed(globalLanguage.mainMenu.code.updateChangelog, MajorV + "." + MinorV + "." + PatchV, creators, UpdateJSON["Updates"][0]["Changelog"]));
            }
            
        }

        public void loadConfig()
        {
            if(!File.Exists(exe + "\\Config.json"))
            {
                config.IP = globalLanguage.global.defaultQuestIPText;
                enablecustom();
                return;
            }
            config = ConfigFile.LoadConfig(exe + "\\Config.json");

            Quest.Text = config.IP;

            if (!config.NotFirstRun)
            {
                enablecustom();
            }
            else if (!(config.Location == System.Reflection.Assembly.GetEntryAssembly().Location))
            {
                enablecustom();
            }
            
        }

        public void enablecustom()
        {
            String regFile = "Windows Registry Editor Version 5.00\n\n[HKEY_CLASSES_ROOT\\bm]\n@=\"URL: bm\"\n\"URL Protocol\"=\"bm\"\n\n[HKEY_CLASSES_ROOT\\bm]\n@=\"" + System.Reflection.Assembly.GetEntryAssembly().Location.Replace("\\", "\\\\") + "\"\n\n[HKEY_CLASSES_ROOT\\bm\\shell]\n\n[HKEY_CLASSES_ROOT\\bm\\shell\\open]\n\n[HKEY_CLASSES_ROOT\\bm\\shell\\open\\command]\n@=\"" + System.Reflection.Assembly.GetEntryAssembly().Location.Replace("\\", "\\\\") + " \\\"%1\\\"\"";
            File.WriteAllText(exe + "\\registry.reg", regFile);
            try
            {
                Process.Start(exe + "\\registry.reg");
                txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.customLinksEnabled);
            }
            catch
            {
                txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.registryUnableToChangeNoCustomLinks);
                return;
            }
            config.CustomProtocols = true;
        }

        public void saveConfig()
        {
            iPUtils.CheckIP(Quest);
            config.Location = System.Reflection.Assembly.GetEntryAssembly().Location;
            config.Version = MajorV.ToString() + "." + MinorV.ToString() + "." + PatchV.ToString();
            config.NotFirstRun = true;
            config.SaveConfig();
        }

        private void ClearText(object sender, RoutedEventArgs e)
        {
            if (Quest.Text == globalLanguage.global.defaultQuestIPText)
            {
                Quest.Text = "";
            }

        }

        private void QuestIPCheck(object sender, RoutedEventArgs e)
        {
            if (Quest.Text == "")
            {
                Quest.Text = globalLanguage.global.defaultQuestIPText;
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
            String IPS = aDBI.adbS("shell ifconfig wlan0", txtbox);
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

            if (FIP == "" && config.IP == globalLanguage.global.defaultQuestIPText)
            {
                config.IP = globalLanguage.global.defaultQuestIPText;
                return;
            }
            if (FIP == "") return;
            config.IP = FIP;
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
            RealClose();
        }

        private void RealClose()
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
                aDBI.adb("shell am start -n com.weloveoculus.BMBF/com.weloveoculus.BMBF.MainActivity", txtbox);
            }));
        }

        public async void Update()
        {
            try
            {
                //Download Update.txt
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        UpdateJSON = JSON.Parse(await client.DownloadStringTaskAsync(new Uri("https://raw.githubusercontent.com/ComputerElite/BM/main/update.json")));
                    }
                    catch
                    {
                        txtbox.AppendText(globalLanguage.global.UD100);
                        return;
                    }
                }

                config.CachedADBPaths.Clear();
                foreach (JSONNode adbp in UpdateJSON["ADBPaths"])
                {
                    config.CachedADBPaths.Add(adbp.ToString().Replace("\"", ""));
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
                    txtbox.AppendText("\n\n" + globalLanguage.processer.ReturnProcessed(globalLanguage.mainMenu.code.previewVersion, MajorV + "." + MinorV + "." + PatchV, MajorU + "." + MinorU + "." + PatchU));
                    UpdateB.Visibility = Visibility.Visible;
                    UpdateB.Content = globalLanguage.mainMenu.code.downgradeNow;
                }
                if (VersionV == VersionU && Preview)
                {
                    //User has Preview Version but a release Version has been released
                    txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.releaseVersionOut);
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
                txtbox.AppendText(globalLanguage.global.UD200);
            }
        }

        private void Mini(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void InstallSongs(object sender, RoutedEventArgs e)
        {
            iPUtils.CheckIP(Quest);
            Songs SongsWindow = new Songs();
            SongsWindow.Show();
        }

        private void InstallMods (object sender, RoutedEventArgs e)
        {
            iPUtils.CheckIP(Quest);
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
            StartBMBFUpdate();
        }

        public void StartBMBFUpdate()
        {
            if (Running)
            {
                txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.operationRunning);
                txtbox.ScrollToEnd();
                return;
            }
            Running = true;
            if (!iPUtils.CheckIP(Quest))
            {
                txtbox.AppendText("\n\n" + globalLanguage.global.ipInvalid);
                txtbox.ScrollToEnd();
                Running = false;
                return;
            }
            BMBF_Link();
            if (!aDBI.adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/mods \"" + exe + "\\ModChecks", txtbox))
            {
                Running = false;
                return;
            }

            MessageBoxResult r0 = MessageBox.Show(globalLanguage.mainMenu.code.onQuest2, "BMBF Manager", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (r0 == MessageBoxResult.Yes) Quest2 = true;

            DCRPM.SetActivity(globalLanguage.dCRP.updatingBMBF);
            if (Directory.Exists(exe + "\\ModChecks\\mods"))
            {
                //game is modded
                MessageBoxResult result1 = MessageBox.Show(globalLanguage.mainMenu.code.moddedBSDetected, "BMBF Manager - BMBF Updater", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result1)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.bMBFUpdatatingAborted);
                        txtbox.ScrollToEnd();
                        Running = false;
                        return;
                }
                if (!aDBI.adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/ \"" + exe + "\\Backup\"", txtbox))
                {
                    Running = false;
                    return;
                }

                //Backup Playlists
                try
                {
                    txtbox.AppendText("\n\n" + globalLanguage.processer.ReturnProcessed(globalLanguage.mainMenu.code.playlistBackup, exe + "\\Backup\\Playlists.json"));
                    txtbox.ScrollToEnd();

                    if (!aDBI.adb("pull /sdcard/BMBFData/Playlists/ \"" + exe + "\\Backup\"", txtbox)) return;

                    WebClient client2 = new WebClient();

                    var j = JSON.Parse(client2.DownloadString("http://" + config.IP + ":50000/host/beatsaber/config"));
                    File.WriteAllText(exe + "\\Backup\\Playlists.json", j["Config"].ToString());
                    txtbox.AppendText("\n\n" + globalLanguage.processer.ReturnProcessed(globalLanguage.mainMenu.code.playlistBackupFinished, exe + "\\Backup\\Playlists.json"));
                    txtbox.ScrollToEnd();
                }
                catch
                {
                    txtbox.AppendText(globalLanguage.global.PL100);

                }


                if (!aDBI.adb("uninstall com.beatgames.beatsaber", txtbox))
                {
                    Running = false;
                    return;
                }
                if (!aDBI.adb("uninstall com.weloveoculus.BMBF", txtbox))
                {
                    Running = false;
                    return;
                }
                MessageBoxResult result2 = MessageBox.Show(globalLanguage.mainMenu.code.downloadBS, "BMBF Manager - BMBF Updater", MessageBoxButton.OK, MessageBoxImage.Warning);
                MessageBoxResult result3 = MessageBox.Show(globalLanguage.mainMenu.code.makingSure, "BMBF Manager - BMBF Updater", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result3)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.bMBFUpdatatingAbortedInstallBS);
                        txtbox.ScrollToEnd();
                        Running = false;
                        return;
                }
            }
            else
            {
                if (!aDBI.adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/ \"" + exe + "\\Backup\"", txtbox))
                {
                    Running = false;
                    return;
                }
                MessageBoxResult result = MessageBox.Show(globalLanguage.mainMenu.code.unmoddedBSDetected, "BMBF Manager - BMBF Updater", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.bMBFUpdatatingAbortedInstallBS);
                        txtbox.ScrollToEnd();
                        Running = false;
                        return;
                }
            }

            if (Directory.Exists(exe + "\\ModChecks\\mods")) Directory.Delete(exe + "\\ModChecks\\mods", true);

            if (Directory.Exists(exe + "\\Backup\\files\\mods")) Directory.Delete(exe + "\\Backup\\files\\mods", true);
            if (Directory.Exists(exe + "\\Backup\\files\\libs")) Directory.Delete(exe + "\\Backup\\files\\libs", true);
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));

            List<String> BadBMBF = new List<String>();
            foreach (JSONNode version in UpdateJSON["BadBMBF"])
            {
                foreach (JSONNode bmbf in BMBFStable)
                {
                    if (bmbf["tag"].ToString().Replace("\"", "") == version.ToString().Replace("\"", ""))
                    {
                        BadBMBF.Add(bmbf["id"].ToString().Replace("\"", ""));
                        break;
                    }
                }
            }

            if (BadBMBF.Contains(BMBFStable[0]["id"].ToString().Replace("\"", "")))
            {
                JSONNode lastBMBF = JSON.Parse("{}");
                foreach (JSONNode bmbf in BMBFStable)
                {
                    if (!BadBMBF.Contains(bmbf["id"].ToString().Replace("\"", "")))
                    {
                        lastBMBF = bmbf;
                        break;
                    }
                }
                MessageBoxResult result4 = MessageBox.Show(globalLanguage.processer.ReturnProcessed(globalLanguage.mainMenu.code.newestBMBFDoesntWork, BMBFStable[0]["tag"], lastBMBF["tag"]), "BMBF Manager - BMBF Updater", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result4)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.downloadingNewestBMBFVersion);
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
                        txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.downloadingRecommendedBMBFVersion);
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
            }
            else
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
            txtbox.AppendText("\n" + globalLanguage.mainMenu.code.downloadComplete);
            txtbox.ScrollToEnd();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));


            //Install BMBF
            txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.installingBMBF);
            txtbox.ScrollToEnd();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));


            aDBI.adb("install -r \"" + exe + "\\tmp\\BMBF.apk\"", txtbox);

            //Mod Beat Saber
            txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.moddingBS);
            txtbox.ScrollToEnd();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));


            aDBI.adb("shell am start -n com.weloveoculus.BMBF/com.weloveoculus.BMBF.MainActivity", txtbox); //Start BMBF
            aDBI.adb("shell pm grant com.weloveoculus.BMBF android.permission.READ_EXTERNAL_STORAGE", txtbox); //Grant permission read
            aDBI.adb("shell pm grant com.weloveoculus.BMBF android.permission.WRITE_EXTERNAL_STORAGE", txtbox); //Grant permission write
            // Need to add a delay
            System.Threading.Thread.Sleep(6000);
            aDBI.adb("shell am start -n com.weloveoculus.BMBF/com.weloveoculus.BMBF.MainActivity", txtbox); //Start BMBF
            System.Threading.Thread.Sleep(5000);
            aDBI.adb("shell am start -n com.weloveoculus.BMBF/com.weloveoculus.BMBF.MainActivity", txtbox); //Start BMBF
            System.Threading.Thread.Sleep(5000);
            if(Quest2)
            {
                MessageBox.Show(globalLanguage.mainMenu.code.userOnQ2Reminder, "BMBF Manager", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                MessageBoxResult r3 = MessageBox.Show(globalLanguage.mainMenu.code.tryRestoreSaveData, "BMBF Manager", MessageBoxButton.YesNo, MessageBoxImage.Question);
                switch (r3)
                {
                    case MessageBoxResult.Yes:
                        RestoreStuff();
                        break;
                }
                txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.q2BMBFInstallFinished);
                Running = false;
                return;
            }
            TimeoutWebClient client = new TimeoutWebClient();
            client.UploadDataAsync(new Uri("http://" + config.IP + ":50000/host/mod/install/step1"), "POST", new byte[0]);
            client.UploadDataCompleted += new UploadDataCompletedEventHandler(finishedstep1);
        }

        private void finishedstep1(object sender, AsyncCompletedEventArgs e)
        {
            TimeoutWebClient client = new TimeoutWebClient();
            aDBI.adb("uninstall com.beatgames.beatsaber", txtbox);
            client.UploadDataAsync(new Uri("http://" + config.IP + ":50000/host/mod/install/step2"), "POST", new byte[0]);
            client.UploadDataCompleted += new UploadDataCompletedEventHandler(finishedstep2);
            txtbox.AppendText("\n" + globalLanguage.processer.ReturnProcessed(globalLanguage.mainMenu.code.stepFinished, "1"));
        }

        private void finishedstep2(object sender, UploadDataCompletedEventArgs e)
        {
            TimeoutWebClient client = new TimeoutWebClient();
            aDBI.adb("pull /sdcard/Android/data/com.weloveoculus.BMBF/cache/beatsabermod.apk \"" + exe + "\\tmp\\beatsabermod.apk\"", txtbox);
            aDBI.adb("install -r \"" + exe + "\\tmp\\beatsabermod.apk\"", txtbox);
            client.UploadDataAsync(new Uri("http://" + config.IP + ":50000/host/mod/install/step3"), "POST", new byte[0]);
            client.UploadDataCompleted += new UploadDataCompletedEventHandler(finishedstep3);
            txtbox.AppendText("\n" + globalLanguage.processer.ReturnProcessed(globalLanguage.mainMenu.code.stepFinished, "2"));
        }

        private void finishedstep3(object sender, UploadDataCompletedEventArgs e)
        {
            txtbox.AppendText("\n" + globalLanguage.processer.ReturnProcessed(globalLanguage.mainMenu.code.stepFinished, "3"));
            aDBI.adb("shell am force-stop com.weloveoculus.BMBF", txtbox);
            aDBI.adb("shell am start -n com.weloveoculus.BMBF/com.weloveoculus.BMBF.MainActivity", txtbox); //Start BMBF
            aDBI.adb("shell pm grant com.beatgames.beatsaber android.permission.READ_EXTERNAL_STORAGE", txtbox); //Grant permission read
            aDBI.adb("shell pm grant com.beatgames.beatsaber android.permission.WRITE_EXTERNAL_STORAGE", txtbox); //Grant permission write

            RestoreStuff();

            txtbox.ScrollToEnd();
            Running = false;
        }

        public void RestoreStuff()
        {
            if (!aDBI.adb("push \"" + exe + "\\Backup\\files\" /sdcard/Android/data/com.beatgames.beatsaber", txtbox))
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
                    txtbox.AppendText("\n\n\n" + globalLanguage.mainMenu.code.bMBFInstallationFinished);
                    txtbox.ScrollToEnd();
                    Running = false;
                    return;
                }

                WebClient client3 = new WebClient();

                String Playlists = exe + "\\Backup\\Playlists.json";

                var j = JSON.Parse(client3.DownloadString("http://" + config.IP + ":50000/host/beatsaber/config"));
                var p = JSON.Parse(File.ReadAllText(Playlists));

                j["Config"]["Playlists"] = p["Playlists"];
                BMBFUtils.PostChangesAndSync(txtbox, j["Config"].ToString());
                txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.playlistsRestored);
                txtbox.ScrollToEnd();
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
            }
            catch
            {
                txtbox.AppendText(globalLanguage.global.BMBF100);
            }
        }

        public void PushPNG(String Path)
        {
            String[] directories = Directory.GetFiles(Path);



            for (int i = 0; i < directories.Length; i++)
            {
                if (directories[i].EndsWith(".png"))
                {
                    txtbox.AppendText("\n\n" + globalLanguage.processer.ReturnProcessed(globalLanguage.mainMenu.code.pushingPng, directories[i]));
                    aDBI.adb("push \"" + directories[i] + "\" /sdcard/BMBFData/Playlists/", txtbox);
                    txtbox.ScrollToEnd();
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                }
            }
        }

        public void SwitchVersion(object sender, RoutedEventArgs e)
        {
            StartVersionSwitch();
        }

        public void StartVersionSwitch()
        {
            if (Running)
            {
                txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.operationRunning);
                return;
            }

            if (!aDBI.adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/mods \"" + exe + "\\ModChecks", txtbox))
            {
                Running = false;
                return;
            }

            if (!aDBI.adb("shell am force-stop com.weloveoculus.BMBF", txtbox))
            {
                Running = false;
                return;
            }

            DCRPM.SetActivity(globalLanguage.dCRP.switchingVersion);

            if (Directory.Exists(exe + "\\ModChecks\\mods"))
            {
                //game is modded

                if (File.Exists(exe + "\\Backups\\modded.apk"))
                {
                    //Unmodded Beat Saber may be installed
                    MessageBoxResult result = MessageBox.Show(globalLanguage.mainMenu.code.lastActionUninstallingBS, "BMBF Manager - Version Switcher", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    switch (result)
                    {
                        case MessageBoxResult.No:
                            txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.aborted);
                            txtbox.ScrollToEnd();
                            Running = false;
                            return;
                    }
                }
                MessageBoxResult result2 = MessageBox.Show(globalLanguage.mainMenu.code.unmodBS, "BMBF Manager - Version Switcher", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result2)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.aborted);
                        txtbox.ScrollToEnd();
                        Running = false;
                        return;
                }
                //Install the unmodded Version of Beat Saber
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                    txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.backingUpAll);
                    txtbox.ScrollToEnd();
                }));
                if (!aDBI.adb("pull /sdcard/BMBFData/Backups/beatsaber-unmodded.apk \"" + exe + "\\tmp\\unmodded.apk\"", txtbox))
                {
                    Running = false;
                    return;
                }
                if (!aDBI.adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files \"" + exe + "\\Backups\"", txtbox))
                {
                    Running = false;
                    return;
                }
                //Directory.Delete(exe + "\\Backups\\files\\mods", true);
                //Directory.Delete(exe + "\\Backups\\files\\libs", true);

                String moddedBS = aDBI.adbS("shell pm path com.beatgames.beatsaber", txtbox).Replace("package:", "").Replace(System.Environment.NewLine, "");
                if (!aDBI.adb("pull " + moddedBS + " \"" + exe + "\\Backups\\modded.apk\"", txtbox))
                {
                    Running = false;
                    return;
                }

                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                    txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.installingUnmodded);
                    txtbox.ScrollToEnd();
                }));
                if (!aDBI.adb("uninstall com.beatgames.beatsaber", txtbox))
                {
                    Running = false;
                    return;
                }
                if (!aDBI.adb("install \"" + exe + "\\tmp\\unmodded.apk\"", txtbox))
                {
                    Running = false;
                    return;
                }
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                    txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.restoringScores);
                    txtbox.ScrollToEnd();
                }));
                aDBI.adb("push \"" + exe + "\\Backups\\files\\LocalDailyLeaderboards.dat\" /sdcard/Android/data/com.beatgames.beatsaber/files/LocalDailyLeaderboards.dat", txtbox);
                aDBI.adb("push \"" + exe + "\\Backups\\files\\LocalLeaderboards.dat\" /sdcard/Android/data/com.beatgames.beatsaber/files/LocalLeaderboards.dat", txtbox);
                aDBI.adb("push \"" + exe + "\\Backups\\files\\PlayerData.dat\" /sdcard/Android/data/com.beatgames.beatsaber/files/PlayerData.dat", txtbox);
                aDBI.adb("push \"" + exe + "\\Backups\\files\\AvatarData.dat\" /sdcard/Android/data/com.beatgames.beatsaber/files/AvatarData.dat", txtbox);
                aDBI.adb("push \"" + exe + "\\Backups\\files\\settings.cfg\" /sdcard/Android/data/com.beatgames.beatsaber/files/settings.cfg", txtbox);


                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                    txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.finishedVanilla);
                    txtbox.ScrollToEnd();
                }));

            }
            else
            {
                //game is unmodded
                if (!File.Exists(exe + "\\Backups\\modded.apk"))
                {
                    txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.modGame);
                    Running = false;
                    return;
                }
                MessageBoxResult result2 = MessageBox.Show(globalLanguage.mainMenu.code.switchToModded, "BMBF Manager - Version Switcher", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result2)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.aborted);
                        txtbox.ScrollToEnd();
                        Running = false;
                        return;
                }
                aDBI.adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/LocalDailyLeaderboards.dat \"" + exe + "\\Backups\\files\\LocalDailyLeaderboards.dat\"", txtbox);
                aDBI.adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/LocalLeaderboards.dat \"" + exe + "\\Backups\\files\\LocalLeaderboards.dat\"", txtbox);
                aDBI.adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/PlayerData.dat \"" + exe + "\\Backups\\files\\PlayerData.dat\"", txtbox);
                aDBI.adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/AvatarData.dat \"" + exe + "\\Backups\\files\\AvatarData.dat\"", txtbox);
                aDBI.adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/settings.cfg \"" + exe + "\\Backups\\files\\settings.cfg\"", txtbox);
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                    txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.uninstallingBS);
                    txtbox.ScrollToEnd();
                }));
                if (!aDBI.adb("uninstall com.beatgames.beatsaber", txtbox))
                {
                    Running = false;
                    return;
                }
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                    txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.installingModded);
                    txtbox.ScrollToEnd();
                }));
                if (!aDBI.adb("install \"" + exe + "\\Backups\\modded.apk\"", txtbox))
                {
                    Running = false;
                    return;
                }
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                    txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.restoringSaveData);
                    txtbox.ScrollToEnd();
                }));
                if (!aDBI.adb("push \"" + exe + "\\Backups\\files\" /sdcard/Android/data/com.beatgames.beatsaber/files", txtbox))
                {
                    Running = false;
                    return;
                }
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                    txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.grantingPerms);
                    txtbox.ScrollToEnd();
                }));
                aDBI.adb("shell pm grant com.beatgames.beatsaber android.permission.READ_EXTERNAL_STORAGE", txtbox); //Grant permission read
                aDBI.adb("shell pm grant com.beatgames.beatsaber android.permission.WRITE_EXTERNAL_STORAGE", txtbox); //Grant permission write
                Directory.Delete(exe + "\\Backups", true);
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                    txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.finishedModded);
                    txtbox.ScrollToEnd();
                }));
            }

            if (Directory.Exists(exe + "\\ModChecks\\mods")) Directory.Delete(exe + "\\ModChecks\\mods", true);
            Running = false;
        }

        private void BPLists(object sender, RoutedEventArgs e)
        {
            iPUtils.CheckIP(Quest);
            BPLists BPListsWindow = new BPLists();
            BPListsWindow.Show();
        }

        public void reloadsongsfolder()
        {
            System.Threading.Thread.Sleep(3000);
            TimeoutWebClient client = new TimeoutWebClient();
            client.QueryString.Add("foo", "foo");
            client.UploadValues("http://" + config.IP + ":50000/host/beatsaber/reloadsongfolders", "POST", client.QueryString);
        }

        private void OpenBMBF(object sender, RoutedEventArgs e)
        {
            StartBMBF();
            iPUtils.CheckIP(Quest);
            try
            {
                TimeoutWebClientShort c = new TimeoutWebClientShort();
                c.DownloadString("http://" + config.IP + ":50000/host/beatsaber/config");
                Process.Start("http://" + config.IP + ":50000/main/upload");
            } catch 
            {
                MessageBox.Show(globalLanguage.processer.ReturnProcessed(globalLanguage.mainMenu.code.bMBFNotReachable, config.IP), "BMBF Manager - BMBF opening", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            
        }

        private void Support(object sender, RoutedEventArgs e)
        {
            iPUtils.CheckIP(Quest);
            Support SupportWindow = new Support();
            SupportWindow.Show();
        }

        private void HitSounds(object sender, RoutedEventArgs e)
        {
            iPUtils.CheckIP(Quest);
            HitSounds HitSoundsWindow = new HitSounds();
            HitSoundsWindow.Show();
        }

        private void BBBU(object sender, RoutedEventArgs e)
        {
            iPUtils.CheckIP(Quest);
            BBBU BBBUWindow = new BBBU();
            BBBUWindow.Show();
        }

        private void QSU(object sender, RoutedEventArgs e)
        {
            iPUtils.CheckIP(Quest);
            QSU QSUWindow = new QSU();
            QSUWindow.Show();
        }

        private void Qosmetics(object sender, RoutedEventArgs e)
        {
            iPUtils.CheckIP(Quest);
            Qosmetics QosmeticsWindow = new Qosmetics();
            QosmeticsWindow.Show();
        }

        private void PE(object sender, RoutedEventArgs e)
        {
            iPUtils.CheckIP(Quest);
            PlaylistEditor PlaylistEditorWindow = new PlaylistEditor();
            PlaylistEditorWindow.Show();
        }

        private void UploadToBMBF(object sender, RoutedEventArgs e)
        {
            iPUtils.CheckIP(Quest);
            
                
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Thread t = new Thread(() =>
                {
                    foreach (string f in ofd.FileNames)
                    {
                        if (File.Exists(f))
                        {
                            Dispatcher.Invoke(() => { txtbox.AppendText(globalLanguage.processer.ReturnProcessed("\n" + globalLanguage.mainMenu.code.uploading, Path.GetFileName(f))); });
                            WebClient c = new WebClient();
                            c.UploadFile("http://" + MainWindow.config.IP + ":50000/host/beatsaber/upload?overwrite", f);
                            Dispatcher.Invoke(() => { txtbox.AppendText(globalLanguage.processer.ReturnProcessed("\n" + globalLanguage.mainMenu.code.uploaded, Path.GetFileName(f))); });
                        }
                    }
                });
                t.Start();
            }
            else
            {
                txtbox.AppendText(globalLanguage.mainMenu.code.uploadingAborted);
            }
        }

        internal void CustomProto(string Link)
        {
            iPUtils.CheckIP(Quest);
            Support SupportWindow = new Support();
            SupportWindow.Show();
            SupportWindow.StartSupport(Link);
        }
    }
}   