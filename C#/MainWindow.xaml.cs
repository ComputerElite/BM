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

namespace BMBF_Manager
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int MajorV = 1;
        int MinorV = 12;
        int PatchV = 0;
        Boolean Preview = false;

        public static Boolean CustomProtocols = false;
        public static Boolean QuestSoundsInstalled = false;
        public static bool QosmeticsInstalled = false;
        public static Boolean CustomImage = false;
        public static Boolean BBBUTransfered = false;
        public static Boolean QSUTransfered = false;
        public static Boolean ShowADB = false;
        public static Boolean Converted = false;
        public static Boolean OneClick = false;
        public static Boolean KeepAlive = true;
        public static bool QosmeticsWarningShown = false;
        public static bool PEWarningShown = false;
        Boolean draggable = true;
        Boolean Running = false;
        Boolean ComeFromUpdate = false;
        String exe = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 1);
        public static String IP = "";
        public static String BMBF = "https://bmbf.dev/stable/27153984";
        public static String CustomImageSource = "N/A";
        public static String GameVersion = "1.13.0";
        public static String language = "en";
        JSONNode json = JSON.Parse("{}");
        public static JSONNode UpdateJSON = JSON.Parse("{}");
        JSONNode BMBFStable = JSON.Parse("{}");
        public static ArrayList ADBPaths = new ArrayList();
        bool Quest2 = false;

        public static Language globalLanguage = new Language();

        public MainWindow()
        {
            InitializeComponent();
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
                uniformBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Main10.png", UriKind.Absolute));
                uniformBrush.Stretch = Stretch.UniformToFill;
                this.Background = uniformBrush;
            }
            Changelog();
            ComeFromUpdate = false;
            CheckBMBFUpdate();
            //TryGetStats();
            KeepAliveTask();
        }

        private void MakeRandomLanguageFile()
        {
            //Generate random text for testing
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
            if (language == "en" || !File.Exists(language))
            {
                language = "en";
                globalLanguage = new Language(); // Sets up english
            }
            else
            {
                globalLanguage = JsonSerializer.Deserialize<BMBFManager.Language.Language>(File.ReadAllText(language));
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

            File.WriteAllText("D:\\en_BM.json", JsonSerializer.Serialize(globalLanguage));
        }

        private async Task KeepAliveTask()
        {
            while(true)
            {
                if(KeepAlive)
                {
                    adb("shell input keyevent KEYCODE_WAKEUP");
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
                String l = await c.DownloadStringTaskAsync(new Uri("http://" + MainWindow.IP + ":50000/host/version/local"));
                String n = await c.DownloadStringTaskAsync(new Uri("https://bmbf.dev/stable/json"));
                local = JsonSerializer.Deserialize<BMBFlocal>(l);
                stable = JsonSerializer.Deserialize<List<BMBFStableVersions>>(n);
                String[] lo = local.version.Split('.');
                List<int> vl = new List<int>();
                vl.Add(0);
                vl.Add(0);
                vl.Add(0);
                for (int i = 0; i < lo.Count(); i++) vl[i] = Convert.ToInt32(lo[i]);

                String v = stable[0].tag.Replace("v", "");
                String[] ne = v.Split('.');
                List<int> vn = new List<int>();
                vn.Add(0);
                vn.Add(0);
                vn.Add(0);
                for (int i = 0; i < ne.Count(); i++) vn[i] = Convert.ToInt32(ne[i]);

                if ((vn[0] >= vl[0] && vn[1] >= vl[1] && vn[2] > vl[2]) || (vn[0] >= vl[0] && vn[1] > vl[1]) || (vn[0] > vl[0]))
                {
                    txtbox.AppendText("\n\n" + globalLanguage.processer.ReturnProcessed(globalLanguage.mainMenu.code.newBMBFAvailable, local.version, v));
                }
                txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.onNewestBMBF);
            } catch { }
        }

        private void TryGetStats()
        {
            txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.tryPullPlayerStats);
            if(!adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/PlayerData.dat \"" + exe + "\\tmp\\PlayerData.dat\"", false))
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
                IP = globalLanguage.global.defaultQuestIPText;
                enablecustom();
                return;
            }
            json = JSON.Parse(File.ReadAllText(exe + "\\Config.json"));

            foreach(JSONNode ADBPath in json["CachedADBPaths"])
            {
                ADBPaths.Add(ADBPath.ToString().Replace("\"", ""));
            }

            CustomProtocols = json["CustomProtocols"].AsBool;
            Converted = json["Converted"].AsBool;
            OneClick = json["OneClick"].AsBool;
            KeepAlive = json["KeepAlive"].AsBool;
            IP = json["IP"];
            QosmeticsWarningShown = json["QosmeticsWarningShown"].AsBool;
            PEWarningShown = json["PEWarningShown"].AsBool;
            BBBUTransfered = json["BBBUTransfered"].AsBool;
            QSUTransfered = json["QSUTransfered"].AsBool;
            ShowADB = json["ShowADB"].AsBool;
            if (json["GameVersion"] != null)
            {
                GameVersion = json["GameVersion"];
            }
            if(json["language"] != null)
            {
                language = json["language"];
            }
            QuestSoundsInstalled = json["QSoundsInstalled"].AsBool;
            QosmeticsInstalled = json["QosmeticsInstalled"].AsBool;

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
                txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.customLinksEnabled);
            }
            catch
            {
                txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.registryUnableToChangeNoCustomLinks);
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
            json["QosmeticsInstalled"] = QosmeticsInstalled;
            json["CustomImage"] = CustomImage;
            json["CustomImageSource"] = CustomImageSource;
            json["GameVersion"] = GameVersion;
            json["ComeFromUpdate"] = ComeFromUpdate;
            json["BBBUTransfered"] = BBBUTransfered;
            json["QSUTransfered"] = QSUTransfered;
            json["ShowADB"] = ShowADB;
            json["Converted"] = Converted;
            json["OneClick"] = OneClick;
            json["KeepAlive"] = KeepAlive;
            json["QosmeticsWarningShown"] = QosmeticsWarningShown;
            json["PEWarningShown"] = PEWarningShown;
            json["language"] = language;
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

            if (FIP == "" && IP == globalLanguage.global.defaultQuestIPText)
            {
                IP = globalLanguage.global.defaultQuestIPText;
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
            String found;
            if((found = RegexTemplates.GetIP(IP)) != "")
            {
                IP = found;
                Quest.Text = IP;
                return true;
            } else
            {
                return false;
            }
        }

        public Boolean adb(String Argument, bool showErrors = true)
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
                                if(showErrors)
                                {
                                    txtbox.AppendText(globalLanguage.global.ADB110);
                                    txtbox.ScrollToEnd();
                                }
                                
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
            if (showErrors)
            {
                txtbox.AppendText(globalLanguage.global.ADB100);
                txtbox.ScrollToEnd();
            }
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
                            txtbox.AppendText(globalLanguage.global.ADB110);
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
            txtbox.AppendText(globalLanguage.global.ADB100);
            txtbox.ScrollToEnd();
            return "Error";
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
            if (!CheckIP())
            {
                txtbox.AppendText("\n\n" + globalLanguage.global.ipInvalid);
                txtbox.ScrollToEnd();
                Running = false;
                return;
            }
            getQuestIP();
            BMBF_Link();
            if (!adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/mods \"" + exe + "\\ModChecks"))
            {
                Running = false;
                return;
            }

            MessageBoxResult r0 = MessageBox.Show(globalLanguage.mainMenu.code.onQuest2, "BMBF Manager", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (r0 == MessageBoxResult.Yes) Quest2 = true;

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
                if (!adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/ \"" + exe + "\\Backup\""))
                {
                    Running = false;
                    return;
                }

                //Backup Playlists
                try
                {
                    txtbox.AppendText("\n\n" + globalLanguage.processer.ReturnProcessed(globalLanguage.mainMenu.code.playlistBackup, exe + "\\Backup\\Playlists.json"));
                    txtbox.ScrollToEnd();
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));

                    if (!adb("pull /sdcard/BMBFData/Playlists/ \"" + exe + "\\Backup\"")) return;

                    WebClient client2 = new WebClient();

                    var j = JSON.Parse(client2.DownloadString("http://" + MainWindow.IP + ":50000/host/beatsaber/config"));
                    File.WriteAllText(exe + "\\Backup\\Playlists.json", j["Config"].ToString());
                    txtbox.AppendText("\n\n" + globalLanguage.processer.ReturnProcessed(globalLanguage.mainMenu.code.playlistBackupFinished, exe + "\\Backup\\Playlists.json"));
                    txtbox.ScrollToEnd();
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                }
                catch
                {
                    txtbox.AppendText(globalLanguage.global.PL100);

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
                if (!adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/ \"" + exe + "\\Backup\""))
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


            adb("install -r \"" + exe + "\\tmp\\BMBF.apk\"");

            //Mod Beat Saber
            txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.moddingBS);
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
            client.UploadDataAsync(new Uri("http://" + MainWindow.IP + ":50000/host/mod/install/step1"), "POST", new byte[0]);
            client.UploadDataCompleted += new UploadDataCompletedEventHandler(finishedstep1);
        }

        private void finishedstep1(object sender, AsyncCompletedEventArgs e)
        {
            TimeoutWebClient client = new TimeoutWebClient();
            adb("uninstall com.beatgames.beatsaber");
            client.UploadDataAsync(new Uri("http://" + MainWindow.IP + ":50000/host/mod/install/step2"), "POST", new byte[0]);
            client.UploadDataCompleted += new UploadDataCompletedEventHandler(finishedstep2);
            txtbox.AppendText("\n" + globalLanguage.processer.ReturnProcessed(globalLanguage.mainMenu.code.stepFinished, "1"));
        }

        private void finishedstep2(object sender, UploadDataCompletedEventArgs e)
        {
            TimeoutWebClient client = new TimeoutWebClient();
            adb("pull /sdcard/Android/data/com.weloveoculus.BMBF/cache/beatsabermod.apk \"" + exe + "\\tmp\\beatsabermod.apk\"");
            adb("install -r \"" + exe + "\\tmp\\beatsabermod.apk\"");
            client.UploadDataAsync(new Uri("http://" + MainWindow.IP + ":50000/host/mod/install/step3"), "POST", new byte[0]);
            client.UploadDataCompleted += new UploadDataCompletedEventHandler(finishedstep3);
            txtbox.AppendText("\n" + globalLanguage.processer.ReturnProcessed(globalLanguage.mainMenu.code.stepFinished, "2"));
        }

        private void finishedstep3(object sender, UploadDataCompletedEventArgs e)
        {
            txtbox.AppendText("\n" + globalLanguage.processer.ReturnProcessed(globalLanguage.mainMenu.code.stepFinished, "3"));
            adb("shell am force-stop com.weloveoculus.BMBF");
            adb("shell am start -n com.weloveoculus.BMBF/com.weloveoculus.BMBF.MainActivity"); //Start BMBF
            adb("shell pm grant com.beatgames.beatsaber android.permission.READ_EXTERNAL_STORAGE"); //Grant permission read
            adb("shell pm grant com.beatgames.beatsaber android.permission.WRITE_EXTERNAL_STORAGE"); //Grant permission write

            RestoreStuff();

            txtbox.ScrollToEnd();
            Running = false;
        }

        public void RestoreStuff()
        {
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
                    txtbox.AppendText("\n\n\n" + globalLanguage.mainMenu.code.bMBFInstallationFinished);
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
                txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.playlistsRestored);
                txtbox.ScrollToEnd();
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
            }
            catch
            {
                txtbox.AppendText(globalLanguage.global.BMBF100);
            }
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
                    txtbox.AppendText("\n\n" + globalLanguage.processer.ReturnProcessed(globalLanguage.mainMenu.code.pushingPng, directories[i]));
                    adb("push \"" + directories[i] + "\" /sdcard/BMBFData/Playlists/");
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
                    txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.installingUnmodded);
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
                    txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.restoringScores);
                    txtbox.ScrollToEnd();
                }));
                adb("push \"" + exe + "\\Backups\\files\\LocalDailyLeaderboards.dat\" /sdcard/Android/data/com.beatgames.beatsaber/files/LocalDailyLeaderboards.dat");
                adb("push \"" + exe + "\\Backups\\files\\LocalLeaderboards.dat\" /sdcard/Android/data/com.beatgames.beatsaber/files/LocalLeaderboards.dat");
                adb("push \"" + exe + "\\Backups\\files\\PlayerData.dat\" /sdcard/Android/data/com.beatgames.beatsaber/files/PlayerData.dat");
                adb("push \"" + exe + "\\Backups\\files\\AvatarData.dat\" /sdcard/Android/data/com.beatgames.beatsaber/files/AvatarData.dat");
                adb("push \"" + exe + "\\Backups\\files\\settings.cfg\" /sdcard/Android/data/com.beatgames.beatsaber/files/settings.cfg");


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
                adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/LocalDailyLeaderboards.dat \"" + exe + "\\Backups\\files\\LocalDailyLeaderboards.dat\"");
                adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/LocalLeaderboards.dat \"" + exe + "\\Backups\\files\\LocalLeaderboards.dat\"");
                adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/PlayerData.dat \"" + exe + "\\Backups\\files\\PlayerData.dat\"");
                adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/AvatarData.dat \"" + exe + "\\Backups\\files\\AvatarData.dat\"");
                adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/settings.cfg \"" + exe + "\\Backups\\files\\settings.cfg\"");
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                    txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.uninstallingBS);
                    txtbox.ScrollToEnd();
                }));
                if (!adb("uninstall com.beatgames.beatsaber"))
                {
                    Running = false;
                    return;
                }
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                    txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.installingModded);
                    txtbox.ScrollToEnd();
                }));
                if (!adb("install \"" + exe + "\\Backups\\modded.apk\""))
                {
                    Running = false;
                    return;
                }
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                    txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.restoringSaveData);
                    txtbox.ScrollToEnd();
                }));
                if (!adb("push \"" + exe + "\\Backups\\files\" /sdcard/Android/data/com.beatgames.beatsaber/files"))
                {
                    Running = false;
                    return;
                }
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                    txtbox.AppendText("\n\n" + globalLanguage.mainMenu.code.grantingPerms);
                    txtbox.ScrollToEnd();
                }));
                adb("shell pm grant com.beatgames.beatsaber android.permission.READ_EXTERNAL_STORAGE"); //Grant permission read
                adb("shell pm grant com.beatgames.beatsaber android.permission.WRITE_EXTERNAL_STORAGE"); //Grant permission write
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
                MessageBox.Show(globalLanguage.processer.ReturnProcessed(globalLanguage.mainMenu.code.bMBFNotReachable, IP), "BMBF Manager - BMBF opening", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        private void QSU(object sender, RoutedEventArgs e)
        {
            CheckIP();
            QSU QSUWindow = new QSU();
            QSUWindow.Show();
        }

        private void Qosmetics(object sender, RoutedEventArgs e)
        {
            CheckIP();
            Qosmetics QosmeticsWindow = new Qosmetics();
            QosmeticsWindow.Show();
        }

        private void PE(object sender, RoutedEventArgs e)
        {
            CheckIP();
            PlaylistEditor PlaylistEditorWindow = new PlaylistEditor();
            PlaylistEditorWindow.Show();
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