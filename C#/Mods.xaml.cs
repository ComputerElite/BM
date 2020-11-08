using Newtonsoft.Json.Linq;
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
        ArrayList CompatibleMods = new ArrayList();
        ArrayList ModNames = new ArrayList();
        ArrayList ModVersions = new ArrayList();
        ArrayList ModDescriptions = new ArrayList();
        String BSVersion = "1.12.2";
        int C = 0;
        int Index = 0;

        public Mods()
        {
            InitializeComponent();
            Quest.Text = MainWindow.IP;
            getMods();
        }

        public void getMods()
        {
            getQuestIP();
            System.Net.WebClient client = new System.Net.WebClient();

            JSONNode json = JSON.Parse("{}");
            JSONNode BMBF = JSON.Parse("{}");

            try
            {
                json = SimpleJSON.JSON.Parse(client.DownloadString("http://www.questboard.xyz/api/mods/"));
            }
            catch
            {
                txtbox.AppendText("\n\nError (Code: BM100). Couldn't reach the Quest Boards Website to get available Mods. You can't install mods. Please restart the program.");
                return;
            }

            try
            {
                BMBF = SimpleJSON.JSON.Parse(client.DownloadString("http://" + MainWindow.IP + ":50000/host/beatsaber/config"));
            }
            catch
            {
                txtbox.AppendText("\n\n\nError (Code: BMBF100). Couldn't acces BMBF Web Interface. Check Following:");
                txtbox.AppendText("\n\n- You've put in the right IP");
                txtbox.AppendText("\n\n- BMBF is opened");
                return;
            }
            BSVersion = BMBF["BeatSaberVersion"].ToString().Replace("\"", "");
            String[] GameVersion = BMBF["BeatSaberVersion"].ToString().Replace("\"", "").Split('.');
            //String[] GameVersion = "1.11.1".Replace("\"", "").Split('.');
            int major = Convert.ToInt32(GameVersion[0]);
            int minor = Convert.ToInt32(GameVersion[1]);
            int patch = Convert.ToInt32(GameVersion[2]);

            for (int i = 0; json["mods"][i]["name"] != null; i++)
            {
                String Name = json["mods"][i]["name"];
                String Creator = "";

                for (int u = 0; json["mods"][i]["creator"][u] != null; u++)
                {
                    Creator = Creator + json["mods"][i]["creator"][u] + ", ";
                }
                Creator = Creator.Substring(0, Creator.Length - 2);
                String Version = json["mods"][i]["downloads"][0]["modversion"];

                for (int z = 0; json["mods"][i]["downloads"][z]["modversion"] != null; z++)
                {
                    for (int u = 0; json["mods"][i]["downloads"][z]["gameversion"][u] != null; u++)
                    {
                        String[] MGameVersion = json["mods"][i]["downloads"][z]["gameversion"][u].ToString().Replace("\"", "").Split('.');
                        int Mmajor = Convert.ToInt32(MGameVersion[0]);
                        int Mminor = Convert.ToInt32(MGameVersion[1]);
                        int Mpatch = Convert.ToInt32(MGameVersion[2]);
                        if (major == Mmajor && minor == Mminor && patch >= Mpatch)
                        {
                            Boolean existent = false;
                            try
                            {
                                for (int o = 0; ModNames[o] != null; o++)
                                {
                                    if ((String)ModNames[o] == Name)
                                    {
                                        existent = true;
                                    }
                                }
                            } catch { }
                            if (existent) continue;
                            Version = json["mods"][i]["downloads"][z]["modversion"];
                            ModVersions.Add(json["mods"][i]["downloads"][z]["gameversion"][u].ToString().Replace("\"", ""));
                            ModList.Items.Add(new ModItem { Name = Name, Creator = Creator, GameVersion = json["mods"][i]["downloads"][z]["gameversion"][u], ModVersion = Version });
                            CompatibleMods.Add(json["mods"][i]["downloads"][z]["download"].ToString().Replace("\"", ""));
                            ModDescriptions.Add(json["mods"][i]["details"].ToString().Replace("\"", "").Replace("\\r\\n", System.Environment.NewLine));
                            ModNames.Add(Name);
                            break;
                        }
                    }
                }

            }

            WebClient c = new WebClient();

            json = JSON.Parse(c.DownloadString("https://raw.githubusercontent.com/ComputerElite/BM/main/mods.json"));

            for (int i = 0; json["mods"][i]["name"] != null; i++)
            {
                String Name = json["mods"][i]["name"];
                String Creator = "";

                for (int u = 0; json["mods"][i]["creator"][u] != null; u++)
                {
                    Creator = Creator + json["mods"][i]["creator"][u] + ", ";
                }
                Creator = Creator.Substring(0, Creator.Length - 2);
                String Version = json["mods"][i]["downloads"][0]["modversion"];

                for (int z = 0; json["mods"][i]["downloads"][z]["modversion"] != null; z++)
                {
                    for (int u = 0; json["mods"][i]["downloads"][z]["gameversion"][u] != null; u++)
                    {
                        String[] MGameVersion = json["mods"][i]["downloads"][z]["gameversion"][u].ToString().Replace("\"", "").Split('.');
                        int Mmajor = Convert.ToInt32(MGameVersion[0]);
                        int Mminor = Convert.ToInt32(MGameVersion[1]);
                        int Mpatch = Convert.ToInt32(MGameVersion[2]);
                        if (major == Mmajor && minor == Mminor && patch >= Mpatch)
                        {
                            Boolean existent = false;
                            try
                            {
                                for (int o = 0; ModNames[o] != null; o++)
                                {
                                    if ((String)ModNames[o] == Name)
                                    {
                                        existent = true;
                                    }
                                }
                            }
                            catch { }
                            if (existent) continue;
                            Version = json["mods"][i]["downloads"][z]["modversion"];
                            ModVersions.Add(json["mods"][i]["downloads"][z]["gameversion"][u].ToString().Replace("\"", ""));
                            ModList.Items.Add(new ModItem { Name = Name, Creator = Creator, GameVersion = json["mods"][i]["downloads"][z]["gameversion"][u], ModVersion = Version });
                            CompatibleMods.Add(json["mods"][i]["downloads"][z]["download"].ToString().Replace("\"", ""));
                            ModDescriptions.Add(json["mods"][i]["details"].ToString().Replace("\"", "").Replace("\\r\\n", System.Environment.NewLine));
                            ModNames.Add(Name);
                            break;
                        }
                    }
                }

            }

            ModList.SelectedIndex = 0;
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
            MessageBox.Show("Mod Name: " + ModNames[ModList.SelectedIndex] + "\n\nDescription:\n" + ModDescriptions[ModList.SelectedIndex], "BMBF Manager Mod Info", MessageBoxButton.OK);
        }

        public void InstallMod(object sender, RoutedEventArgs e)
        {
            if (!CheckIP())
            {
                txtbox.AppendText("\n\nChoose a valid IP.");
                return;
            }
            if(Running)
            {
                txtbox.AppendText("\n\nA Mod Install is already running.");
                return;
            }
            Running = true;

            C = 0;
            while (File.Exists(exe + "\\tmp\\Mod" + C + ".zip"))
            {
                C++;
            }

            Index = ModList.SelectedIndex;

            if (ModVersions[Index].ToString() != BSVersion)
            {
                MessageBoxResult result1 = MessageBox.Show("The latest Version of the Mod " + ModNames[Index] + " on QuestBoard has been made for Beat Saber Version " + ModVersions[Index].ToString() + ". It'll be compatible with your Game but you have to enable it manually. I'll open the BMBF mod tab after installing the mod. For it to activate you scroll to the mod you installed and flip the switch to on. If you get a compatibility warning click \"Enable Mod\" and then click \"Sync to Beat Saber\" in the top right.\nDo you wish to continue?", "BMBF Manager Mod Installing", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result1)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\nMod Installing Aborted.");
                        txtbox.ScrollToEnd();
                        Running = false;
                        return;
                }
            }

            String Download = CompatibleMods[Index].ToString().Replace("\"", "");
            WebClient c = new WebClient();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                txtbox.AppendText("\n\nDownloading Mod " + ModNames[Index] + "\n");
                txtbox.ScrollToEnd();
            }));
            Uri uri = new Uri(Download);
            try
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                {
                    c.DownloadFileCompleted += new AsyncCompletedEventHandler(finished_download);
                    c.DownloadFileAsync(uri, exe + "\\tmp\\Mod" + C + ".zip");
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

        private void finished_download(object sender, AsyncCompletedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                txtbox.AppendText("\nDownloaded Mod " + ModNames[Index] + "\n");
                txtbox.ScrollToEnd();
            }));
            upload(exe + "\\tmp\\Mod" + C + ".zip");
            Running = false;
        }

        public void upload(String path)
        {
            getQuestIP();

            TimeoutWebClient client = new TimeoutWebClient();

            txtbox.AppendText("\n\nUploading Mod " + ModNames[Index] + " to BMBF");
            txtbox.ScrollToEnd();
            Uri uri = new Uri("http://" + MainWindow.IP + ":50000/host/beatsaber/upload?overwrite");
            try
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                {
                    client.UploadFileCompleted += new UploadFileCompletedEventHandler(finished_upload);
                    client.UploadFileAsync(uri, path);
                }));
            }
            catch
            {
                txtbox.AppendText("\n\nA error Occured (Code: BMBF100)");
                txtbox.ScrollToEnd();
            }

            /*
            
            JSONNode BMBF = JSON.Parse("{}");
            try
            {
                BMBF = SimpleJSON.JSON.Parse(client.DownloadString("http://" + MainWindow.IP + ":50000/host/beatsaber/config"));
            }
            catch
            {
                txtbox.AppendText("\n\n\nError (Code: BMBF100). Couldn't acces BMBF Web Interface. Check Following:");
                txtbox.AppendText("\n\n- You've put in the right IP");
                txtbox.AppendText("\n\n- BMBF is opened");
                return;
            }

            ZipFile.ExtractToDirectory(exe + "\\tmp\\Mod" + C + ".zip", exe + "\\tmp\\Mod" + C);
            var Mod = JSON.Parse(File.ReadAllText(exe + "\\tmp\\Mod" + C + "\\bmbfmod.json"));

            String ModID = Mod["id"];

            for(int i = 0; BMBF["Config"]["Mods"][i]["ID"] != null; i++)
            {
                if(BMBF["Config"]["Mods"][i]["ID"] == ModID)
                {
                    BMBF["Config"]["Mods"][i]["Status"] = "Installed";
                    break;
                }
            }

            JObject o = JObject.Parse(BMBF.ToString());
            o.Property("SyncConfig").Remove();
            o.Property("IsCommitted").Remove();
            o.Property("BeatSaberVersion").Remove();

            JProperty lrs = o.Property("Config");
            o.Add(lrs.Value.Children<JProperty>());
            lrs.Remove();

            

            //var Config = JSON.Parse(BMBF["Config"].ToString().Substring(10, BMBF["Config"].ToString().Length - 11));
            File.WriteAllText(exe + "\\tmp\\config.json", o.ToString());
            txtbox.AppendText("\n\n" + MainWindow.IP);
            postChanges(exe + "\\tmp\\config.json");
            */
            
        }

        private void finished_upload(object sender, AsyncCompletedEventArgs e)
        {
            if (ModVersions[Index].ToString() == BSVersion)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                    {
                        Sync();
                    }));
                    txtbox.AppendText("\n\nMod " + ModNames[Index] + " was synced to your Quest.");
                    txtbox.ScrollToEnd();
                }
                catch
                {
                    txtbox.AppendText("\n\nCouldn't sync with BeatSaber. Needs to be done manually.");
                    txtbox.ScrollToEnd();
                    Running = false;
                    return;
                }
            }
            else
            {
                Process.Start("http://" + MainWindow.IP + ":50000/main/mods");
                txtbox.AppendText("\n\nSince you choose to install this mod... you need to enable it manually. I uploaded it.");
                Running = false;
                return;
            }
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
            System.Threading.Thread.Sleep(2000);
            using (WebClient client = new WebClient())
            {
                client.QueryString.Add("foo", "foo");
                client.UploadValues("http://" + MainWindow.IP + ":50000/host/beatsaber/commitconfig", "POST", client.QueryString);
            }
        }
    }

    public class ModItem
    {
        public string Name { get; set; }

        public string Creator { get; set; }

        public string GameVersion { get; set; }

        public string ModVersion { get; set; }
    }
}


