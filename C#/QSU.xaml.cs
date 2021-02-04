using Microsoft.WindowsAPICodePack.Dialogs;
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
using System.Text.Json;
using BeatSaverAPI;
using System.Text.RegularExpressions;

namespace BMBF_Manager
{
    /// <summary>
    /// Interaktionslogik für QSU.xaml
    /// </summary>
    public partial class QSU : Window
    {
        String path;
        String dest;

        Boolean debug = false;
        Boolean automode = false;
        Boolean copied = false;
        Boolean draggable = true;
        Boolean Running = false;
        String exe = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 1);
        ArrayList P = new ArrayList();
        int Lists = 0;

        public QSU()
        {
            InitializeComponent();
            txtbox.Text = "Output:\n";
            if (debug)
            {
                txtbox.AppendText(exe);
            }
            txtboxd.Text = "Please choose your destination folder.";
            txtboxs.Text = "Please choose your Song folder.";
            if (!Directory.Exists(exe + "\\CustomSongs"))
            {
                Directory.CreateDirectory(exe + "\\CustomSongs");
            }
            if (!Directory.Exists(exe + "\\Playlists"))
            {
                Directory.CreateDirectory(exe + "\\Playlists");
            }
            if (!Directory.Exists(exe + "\\BPLists"))
            {
                Directory.CreateDirectory(exe + "\\BPLists");
            }
            TransferFromQSU();
            Move();

            Quest.Text = MainWindow.IP;

            Backups.SelectedIndex = 0;
            getBackups(exe + "\\Playlists");

            Playlists.Items.Add("Load Playlists!");
            Playlists.SelectedIndex = 0;

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
                uniformBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/QSU3.png", UriKind.Absolute));
                uniformBrush.Stretch = Stretch.UniformToFill;
                this.Background = uniformBrush;
            }
        }

        public void TransferFromQSU()
        {
            if (MainWindow.QSUTransfered) return;
            MainWindow.QSUTransfered = true;
            MessageBoxResult r = MessageBox.Show("Hi. I'm asking you if I should import data from Quest Song Utilities. Only click yes if you've used the seperate program before. You can always import again if you wish to from the settings.", "BMBF Manager - Quest Song Utilities", MessageBoxButton.YesNo, MessageBoxImage.Question);
            switch (r)
            {
                case MessageBoxResult.No:
                    txtbox.AppendText("\n\nNothing Imported");
                    return;
            }
            MessageBox.Show("I'll open a window for you. Please choose the folder in which your Quest Song Utilities Installation is located. I'll then transfer all the data", "BMBF Manager - Quest Song Utilities", MessageBoxButton.OK, MessageBoxImage.Information);

            CommonOpenFileDialog ofd = new CommonOpenFileDialog();
            ofd.IsFolderPicker = true;
            if (ofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                //Get the path of specified file
                if (Directory.Exists(ofd.FileName))
                {
                    if (Directory.Exists(ofd.FileName + "\\CustomSongs"))
                    {
                        foreach (String folder in Directory.GetDirectories(ofd.FileName + "\\CustomSongs"))
                        {
                            if (Directory.Exists(exe + "\\CustomSongs\\" + System.IO.Path.GetDirectoryName(folder)))
                            {
                                txtbox.AppendText("\nskipping folder " + "\\CustomSongs\\" + System.IO.Path.GetDirectoryName(folder) + " it already exists");
                                continue;
                            }
                            String backupName = new DirectoryInfo(folder).Name;
                            Directory.Move(ofd.FileName + "\\CustomSongs\\" + backupName, exe + "\\CustomSongs\\" + backupName);
                        }

                        foreach (String file in Directory.GetFiles(ofd.FileName + "\\CustomSongs"))
                        {
                            if (File.Exists(exe + "\\CustomSongs\\" + System.IO.Path.GetFileName(file)))
                            {
                                txtbox.AppendText("\nskipping file " + "\\CustomSongs\\" + System.IO.Path.GetFileName(file) + " it already exists");
                                continue;
                            }
                            File.Move(ofd.FileName + "\\CustomSongs\\" + System.IO.Path.GetFileName(file), exe + "\\CustomSongs\\" + System.IO.Path.GetFileName(file));
                        }
                    }
                    if (Directory.Exists(ofd.FileName + "\\Playlists"))
                    {
                        foreach (String folder in Directory.GetDirectories(ofd.FileName + "\\Playlists"))
                        {
                            if (Directory.Exists(exe + "\\Playlists\\" + System.IO.Path.GetDirectoryName(folder)))
                            {
                                txtbox.AppendText("\nskipping folder " + "\\Playlists\\" + System.IO.Path.GetDirectoryName(folder) + " it already exists");
                                continue;
                            }
                            String backupName = new DirectoryInfo(folder).Name;
                            Directory.Move(ofd.FileName + "\\Playlists\\" + backupName, exe + "\\Playlists\\" + backupName);
                        }

                        foreach (String file in Directory.GetFiles(ofd.FileName + "\\Playlists"))
                        {
                            if (File.Exists(exe + "\\Playlists\\" + System.IO.Path.GetFileName(file)))
                            {
                                txtbox.AppendText("\nskipping file " + "\\Playlists\\" + System.IO.Path.GetFileName(file) + " it already exists");
                                continue;
                            }
                            File.Move(ofd.FileName + "\\Playlists\\" + System.IO.Path.GetFileName(file), exe + "\\Playlists\\" + System.IO.Path.GetFileName(file));
                        }
                    }
                    if (Directory.Exists(ofd.FileName + "\\BPLists"))
                    {
                        foreach(String folder in Directory.GetDirectories(ofd.FileName + "\\BPLists"))
                        {
                            if(Directory.Exists(exe + "\\BPLists\\" + System.IO.Path.GetDirectoryName(folder)))
                            {
                                txtbox.AppendText("\nskipping folder " + "\\BPLists\\" + System.IO.Path.GetDirectoryName(folder) + " it already exists");
                                continue;
                            }
                            String backupName = new DirectoryInfo(folder).Name;
                            Directory.Move(ofd.FileName + "\\BPLists\\" + backupName, exe + "\\BPLists\\" + backupName);
                        }

                        foreach (String file in Directory.GetFiles(ofd.FileName + "\\BPLists"))
                        {
                            if (File.Exists(exe + "\\BPLists\\" + System.IO.Path.GetFileName(file)))
                            {
                                txtbox.AppendText("\nskipping file " + "\\BPLists\\" + System.IO.Path.GetFileName(file) + " it already exists");
                                continue;
                            }
                            File.Move(ofd.FileName + "\\BPLists\\" + System.IO.Path.GetFileName(file), exe + "\\BPLists\\" + System.IO.Path.GetFileName(file));
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please select a valid Directory", "BMBF Manager - Quest Song Utilities", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

            }

            txtbox.AppendText("\n\nAll Data moved");
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

        public void StartBMBF()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                adb("shell am start -n com.weloveoculus.BMBF/com.weloveoculus.BMBF.MainActivity");
            }));
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
                            txtbox.AppendText(MainWindow.ADB110);
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
            txtbox.AppendText(MainWindow.ADB100);
            txtbox.ScrollToEnd();
            return "Error";
        }

        public void getPlaylists(object sender, RoutedEventArgs e)
        {
            StartBMBF();
            if (!CheckIP())
            {
                txtbox.AppendText("\n\nChoose a valid IP!");
                return;
            }

            Playlists.Items.Clear();
            Playlists.Items.Add("Playlists");

            WebClient client = new WebClient();

            var json2 = SimpleJSON.JSON.Parse(client.DownloadString("https://raw.githubusercontent.com/BMBF/resources/master/assets/beatsaber-knowns.json"));
            ArrayList known = new ArrayList();

            foreach (JSONNode pack in json2["knownLevelPackIds"])
            {
                known.Add(pack.ToString().Replace("\"", ""));
            }

            P = new ArrayList();

            var json = SimpleJSON.JSON.Parse(client.DownloadString("http://" + MainWindow.IP + ":50000/host/beatsaber/config"));
            int index = 0;

            ArrayList PN = new ArrayList();

            foreach (JSONNode Playlist in json["Config"]["Playlists"])
            {
                P.Add(Playlist["PlaylistID"].ToString().Replace("\"", ""));
                PN.Add(Playlist["PlaylistName"].ToString().Replace("\"", ""));
            }

            foreach (String c in P)
            {
                if (!known.Contains(c))
                {
                    Playlists.Items.Add(PN[index]);
                }
                else
                {
                    Lists++;
                }
                index++;
            }

            Playlists.SelectedIndex = 0;
            txtbox.AppendText("\n\nLoaded Playlists.");
        }

        public void BPList(object sender, RoutedEventArgs e)
        {
            if (Running)
            {
                return;
            }
            if (!CheckIP())
            {
                txtbox.AppendText("\n\nChoose a valid IP!");
                txtbox.ScrollToEnd();
                return;
            }
            Running = true;
            CheckIP();
            if (Playlists.SelectedIndex == 0)
            {
                txtbox.AppendText("\n\nChoose a Playlist!");
                txtbox.ScrollToEnd();
                Running = false;
                return;
            }

            WebClient c = new WebClient();
            var json = JSON.Parse(c.DownloadString("http://" + MainWindow.IP + ":50000/host/beatsaber/config"));
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                txtbox.AppendText("\n\nmaking BPList " + json["Config"]["Playlists"][Playlists.SelectedIndex + Lists - 1]["PlaylistName"]);
                txtbox.ScrollToEnd();
            }));
            var result = JSON.Parse("{}");
            result["playlistTitle"] = json["Config"]["Playlists"][Playlists.SelectedIndex + Lists - 1]["PlaylistName"];
            result["playlistAuthor"] = "Quest Song Utilities";
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                txtbox.AppendText("\nDownloading Playlist Cover");
                txtbox.ScrollToEnd();
            }));
            using (WebClient client = new WebClient())
            {
                try
                {
                    client.DownloadFile("http://" + MainWindow.IP + ":50000/host/beatsaber/playlist/cover?PlaylistID=" + json["Config"]["Playlists"][Playlists.SelectedIndex + Lists - 1]["PlaylistID"], exe + "\\tmp\\Playlist.png");
                }
                catch
                {
                    txtbox.AppendText(MainWindow.BMBF100);
                    txtbox.ScrollToEnd();
                    Running = false;
                    return;
                }
            }
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                txtbox.AppendText("\nDownloaded Playlist Cover");
                txtbox.ScrollToEnd();
            }));
            result["image"] = "data:image/png;base64," + Convert.ToBase64String(File.ReadAllBytes(exe + "\\tmp\\Playlist.png"));

            int i = 0;
            foreach (JSONNode Song in json["Config"]["Playlists"][Playlists.SelectedIndex + Lists - 1]["SongList"])
            {
                String SongHash = Song["SongID"];
                SongHash = SongHash.Replace("custom_level_", "");
                String SongName = Song["SongName"];
                result["songs"][i]["hash"] = SongHash;
                result["songs"][i]["songName"] = SongName;
                i++;
            }

            File.WriteAllText(exe + "\\BPLists\\" + json["Config"]["Playlists"][Playlists.SelectedIndex + Lists - 1]["PlaylistName"] + ".bplist", result.ToString());
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                txtbox.AppendText("\n\nBPList " + json["Config"]["Playlists"][Playlists.SelectedIndex + Lists - 1]["PlaylistName"] + " has been made at " + "BPLists\\" + json["Config"]["Playlists"][Playlists.SelectedIndex + Lists - 1]["PlaylistName"] + ".bplist");
                txtbox.ScrollToEnd();
            }));
            Running = false;
        }

        public void DeleteP(object sender, RoutedEventArgs e)
        {
            if (Running)
            {
                return;
            }
            if (!CheckIP())
            {
                txtbox.AppendText("\n\nChoose a valid IP!");
                txtbox.ScrollToEnd();
                return;
            }
            Running = true;
            CheckIP();
            if (Playlists.SelectedIndex == 0)
            {
                txtbox.AppendText("\n\nChoose a Playlist!");
                txtbox.ScrollToEnd();
                Running = false;
                return;
            }
            MessageBoxResult result = MessageBox.Show("Are you Sure to delete the Playlists named \"" + Playlists.SelectedValue + "\"?\n\n THIS IS NOT UNDOABLE!!!", "BMBF Manager - Quest Song Utilities", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            switch (result)
            {
                case MessageBoxResult.No:
                    txtbox.AppendText("\n\nDeleting aborted");
                    Running = false;
                    txtbox.ScrollToEnd();
                    return;
            }

            var json = SimpleJSON.JSON.Parse(File.ReadAllText(exe + "\\tmp\\config.json"));

            foreach (JSONNode song in json["Config"]["Playlists"][Playlists.SelectedIndex + Lists - 1]["SongList"])
            {
                if (!adb("shell rm -rR /sdcard/BMBFData/CustomSongs/" + song["SongID"].ToString().Replace("\"", "")))
                {
                    return;
                }
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                {
                    txtbox.AppendText("\n\nDeleted " + song["SongID"]);
                    txtbox.ScrollToEnd();
                }));
            }
            json["Config"]["Playlists"].Remove(Playlists.SelectedIndex + Lists - 1);

            try
            {
                File.WriteAllText(exe + "\\tmp\\config.json", json["Config"].ToString());

                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                {
                    postChanges(exe + "\\tmp\\config.json");
                }));
                txtbox.AppendText("\n\nDeleted Playlist with all Data");
                txtbox.ScrollToEnd();
            }
            catch
            {
                txtbox.AppendText(MainWindow.BMBF100);
                txtbox.ScrollToEnd();
            }
            Running = false;
        }


        public void Move()
        {
            if (MainWindow.Converted) return;

            foreach (String file in Directory.GetFiles(exe + "\\CustomSongs"))
            {
                if (file.EndsWith(".json"))
                {
                    File.Move(file, exe + "\\Playlists\\" + System.IO.Path.GetFileName(file));
                }
            }

            foreach (String file in Directory.GetFiles(exe + "\\Playlists"))
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

            MainWindow.Converted = true;
        }

        public Boolean CheckIP()
        {
            getQuestIP();
            if (MainWindow.IP == "Quest IP")
            {
                return false;
            }
            Match found = Regex.Match(MainWindow.IP, "((1?[0-9]?[0-9]|2(5[0-5]|[0-4][0-9]))\\.){3}(1?[0-9]?[0-9]|2(5[0-5]|[0-4][0-9]))");
            if (found.Success)
            {
                MainWindow.IP = found.Value;
                Quest.Text = MainWindow.IP;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void getBackups(String Path)
        {
            ArrayList Jsons = new ArrayList();
            string[] Files = Directory.GetFiles(Path);
            Backups.Items.Clear();
            Backups.Items.Add("Backups");

            foreach (String cfile in Files)
            {
                if (cfile.EndsWith(".json"))
                {
                    Backups.Items.Add(System.IO.Path.GetFileNameWithoutExtension(cfile));
                }
            }
            Backups.SelectedIndex = 0;
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


        private void Backup(object sender, RoutedEventArgs e)
        {
            StartBMBF();
            if (Running)
            {
                return;
            }
            Boolean good = CheckIP();
            if (!good)
            {
                txtbox.AppendText("\n\nChoose a valid IP!");
                txtbox.ScrollToEnd();
                Running = false;
                return;
            }
            Running = true;
            try
            {
                CheckIP();
                if (dest == null)
                {
                    dest = exe + "\\Playlists";
                    if (!Directory.Exists(exe + "\\Playlists"))
                    {
                        Directory.CreateDirectory(exe + "\\Playlists");
                    }
                }

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

                if (File.Exists(exe + "\\Playlists\\" + BName.Text + ".json"))
                {
                    txtbox.AppendText("\n\nThis Playlist Backup already exists!");
                    txtbox.ScrollToEnd();
                    Running = false;
                    return;
                }

                txtbox.AppendText("\n\nBacking up Playlist to " + exe + "\\Playlists\\" + BName.Text + ".json");
                txtbox.ScrollToEnd();
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));

                adb("pull /sdcard/BMBFData/Playlists/ \"" + exe + "\\Playlists\"");

                WebClient client = new WebClient();
                var json = JSON.Parse(client.DownloadString("http://" + MainWindow.IP + ":50000/host/beatsaber/config"));
                json["IsCommitted"] = false;
                File.WriteAllText(exe + "\\Playlists\\" + BName.Text + ".json", json["Config"].ToString());

                txtbox.AppendText("\n\nBacked up Playlists to " + exe + "\\Playlists\\" + BName.Text + ".json");
                txtbox.ScrollToEnd();
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
            }
            catch
            {
                txtbox.AppendText(MainWindow.PL100);
                txtbox.ScrollToEnd();

            }
            getBackups(exe + "\\Playlists");
            Running = false;

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

        public void postChanges(String Config)
        {
            using (WebClient client = new WebClient())
            {
                client.QueryString.Add("foo", "foo");
                client.UploadFile("http://" + MainWindow.IP + ":50000/host/beatsaber/config", "PUT", Config);
                client.UploadValues("http://" + MainWindow.IP + ":50000/host/beatsaber/commitconfig", "POST", client.QueryString);
            }
        }


        private void Restore(object sender, RoutedEventArgs e)
        {
            StartBMBF();
            if (Running)
            {
                return;
            }
            if (!CheckIP())
            {
                txtbox.AppendText("\n\nChoose a valid IP!");
                txtbox.ScrollToEnd();
                Running = false;
                return;
            }
            if (Backups.SelectedIndex == 0)
            {
                return;
            }
            Running = true;
            try
            {
                CheckIP();

                String Playlists;
                if (dest == null)
                {
                    dest = path;
                }

                txtbox.AppendText("\n\nRestoring Playlist from " + exe + "\\Playlists\\" + Backups.SelectedValue + ".json");
                txtbox.ScrollToEnd();
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));



                if (!Directory.Exists(exe + "\\tmp"))
                {
                    Directory.CreateDirectory(exe + "\\tmp");
                }
                WebClient client = new WebClient();

                Playlists = exe + "\\Playlists\\" + Backups.SelectedValue + ".json";

                var j = JSON.Parse(client.DownloadString("http://" + MainWindow.IP + ":50000/host/beatsaber/config"));
                var p = JSON.Parse(File.ReadAllText(Playlists));


                j["Config"]["Playlists"] = p["Playlists"];
                File.WriteAllText(exe + "\\tmp\\config.json", j["Config"].ToString());

                PushPNG(exe + "\\Playlists\\Playlists");
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                    postChanges(exe + "\\tmp\\config.json");
                }));
                txtbox.AppendText("\n\nRestored old Playlists.");
                txtbox.ScrollToEnd();
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
            }
            catch
            {
                txtbox.AppendText(MainWindow.BMBF100);
                txtbox.ScrollToEnd();
            }
            Running = false;
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


        private void Auto(object sender, RoutedEventArgs e)
        {
            if ((bool)auto.IsChecked)
            {
                automode = true;
                txtboxs.Text = "Oculus Quest";
                txtboxs.Opacity = 0.6;
                sr.Opacity = 0.6;

                if(dest == null)
                {
                    dest = exe + "\\CustomSongs";
                    if (!Directory.Exists(dest))
                    {
                        Directory.CreateDirectory(dest);
                    }
                }
            }
            else
            {
                automode = false;
                txtboxs.Text = "Please choose your Song Folder";
                txtboxs.Opacity = 0.9;
                sr.Opacity = 0.9;
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
            this.Close();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (automode)
            {
                return;
            }
            if (Running)
            {
                return;
            }
            CommonOpenFileDialog fd = new CommonOpenFileDialog();
            fd.IsFolderPicker = true;
            if (fd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                path = fd.FileName;
                if (path.Contains("Debug"))
                {
                    debug = true;
                    txtbox.Text = "Output (Debugging enabled):";
                }
                else
                {
                    debug = false;
                    txtbox.Text = "Output:";
                }
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                txtboxs.Text = path;

            }
        }

        public void getQuestIP()
        {
            MainWindow.IP = Quest.Text;
            return;
        }

        public void input(String Path, String dest)
        {
            if (Running)
            {
                return;
            }
            ArrayList list = new ArrayList();
            ArrayList content = new ArrayList();
            ArrayList over = new ArrayList();
            int overwritten = 0;
            int exported = 0;
            String Name = "";
            String Source = Path;
            Running = true;

            if ((bool)auto.IsChecked)
            {
                txtbox.AppendText("\nAuto Mode enabled! Copying all Songs to " + exe + "\\tmp. Please be patient.");
                txtbox.ScrollToEnd();
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                if (!copied)
                {
                    if (!adb("pull /sdcard/BMBFData/CustomSongs/ \"" + exe + "\\tmp\""))
                    {
                        return;
                    }
                    copied = true;
                }
                if (Directory.Exists(exe + "\\tmp\\CustomSongs"))
                {
                    Source = exe + "\\tmp\\CustomSongs";
                }
                else
                {
                    Source = exe + "\\tmp";
                }
                txtbox.AppendText("\n\nZipping Songs");
                txtbox.ScrollToEnd();

            }

            string[] directories = Directory.GetDirectories(Source);



            foreach (String CD in directories)
            {
                txtbox.AppendText("\n");


                if (!File.Exists(CD + "\\" + "Info.dat") && !File.Exists(CD + "\\" + "info.dat"))
                {
                    txtbox.AppendText("\n" + CD + " is no Song");
                    txtbox.ScrollToEnd();
                    continue;
                }
                String dat = "";
                if (File.Exists(CD + "\\" + "Info.dat"))
                {
                    dat = CD + "\\" + "Info.dat";

                }
                if (File.Exists(CD + "\\" + "info.dat"))
                {
                    dat = CD + "\\" + "info.dat";

                }
                try
                {
                    BeatSaberSong json = JsonSerializer.Deserialize<BeatSaberSong>(File.ReadAllText(dat));
                    Name = json.SongName;

                    Name = Name.Replace("/", "");
                    Name = Name.Replace(":", "");
                    Name = Name.Replace("*", "");
                    Name = Name.Replace("?", "");
                    Name = Name.Replace("\"", "");
                    Name = Name.Replace("<", "");
                    Name = Name.Replace(">", "");
                    Name = Name.Replace("|", "");
                    Name = Name.Replace(@"\", "");
                    int Time = 0;
                    Name.Trim();

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
                    txtbox.AppendText("\nFolder: " + CD);

                    if ((bool)box.IsChecked)
                    {
                        bool v = File.Exists(dest + "\\" + Name + ".zip");
                        if (v)
                        {
                            File.Delete(dest + "\\" + Name + ".zip");
                            txtbox.AppendText("\noverwritten file: " + dest + "\\" + Name + ".zip");
                            if (debug)
                            {
                                over.Add("\nSong Name: " + Name);
                                over.Add("\nFolder: " + CD);
                                over.Add("\n");
                            }

                            overwritten++;
                        }
                    }
                    else
                    {
                        bool v = File.Exists(dest + "\\" + Name + ".zip");
                        if (v)
                        {
                            txtbox.AppendText("\nthis Song already exists");
                        }
                    }

                    zip(CD, dest + "\\" + Name + ".zip");
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
            txtbox.ScrollToEnd();
            if (exported == 0 && (bool)auto.IsChecked)
            {
                txtbox.AppendText(MainWindow.QSU110);
            }
            else if (exported == 0 && !(bool)auto.IsChecked)
            {
                txtbox.AppendText(MainWindow.QSU100);
            }
            else
            {
                txtbox.AppendText("\nFinished! Backed up " + exported + " Songs.");
            }
            if ((bool)auto.IsChecked && dest == exe + "\\CustomSongs")
            {
                txtbox.AppendText("\nAuto Mode was enabled. Your finished Songs are at the program location in a folder named CustomSongs.");
            }
            if ((bool)box.IsChecked)
            {
                txtbox.AppendText("\nOverwritten " + overwritten + " existing zips");
                if (debug)
                {
                    txtbox.AppendText("\nOverwritten Files:\n");
                    for (int cc = 0; cc < over.Count; cc++)
                    {
                        txtbox.AppendText(" " + over[cc]);
                    }
                }
            }
            txtbox.ScrollToEnd();
            Running = false;
        }

        public static void zip(String src, String Output)
        {
            ZipFile.CreateFromDirectory(src, Output);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (Running)
            {
                return;
            }
            CommonOpenFileDialog fd = new CommonOpenFileDialog();
            fd.IsFolderPicker = true;
            if (fd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                {
                    dest = fd.FileName;
                    if (!System.IO.Directory.Exists(dest))
                    {
                        System.IO.Directory.CreateDirectory(dest);
                    }
                    txtboxd.Text = dest;

                }
            }

        }


        private void Check(object sender, RoutedEventArgs e)
        {
            if (!(bool)index.IsChecked)
            {
                index.IsChecked = true;
            }
            if ((bool)auto.IsChecked)
            {
                zips.IsChecked = false;
            }
        }

        private void Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)box.IsChecked)
            {
                index.IsChecked = false;
            }
        }

        private void Overwrite(object sender, RoutedEventArgs e)
        {
            if ((bool)index.IsChecked)
            {
                box.IsChecked = false;
            }
        }

        private void Uncheck(object sender, RoutedEventArgs e)
        {
            if ((bool)zips.IsChecked)
            {
                zips.IsChecked = false;
            }
        }

        private void AutoM(object sender, RoutedEventArgs e)
        {
            if ((bool)zips.IsChecked)
            {
                zips.IsChecked = false;
            }
        }

        private void CheckFolders(object sender, RoutedEventArgs e)
        {
            MessageBoxResult r = MessageBox.Show("This option may nuke your Playlists. It will Backup all your Songs, rename them to the right folder name, check them if they are working and then put them back on your Quest. All in all it may take a few minutes without a responding window.\nDo you want to proceed?", "BMBF Manager - Quest Song Utilities", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(r == MessageBoxResult.No) 
            {
                txtbox.AppendText("\n\nAborted");
                return;
            }
            Directory.CreateDirectory(exe + "\\tmp\\CustomSongs");
            if (!adb("pull /sdcard/BMBFData/CustomSongs/ \"" + exe + "\\tmp\"")) return;
            adb("shell rm -r /sdcard/BMBFData/CustomSongs/");
            Songs SongsWindow = new Songs();
            SongsWindow.Show();
            Directory.CreateDirectory(exe + "\\tmp\\MoveSongs");
            foreach (String song in Directory.GetDirectories(exe + "\\tmp\\CustomSongs"))
            {
                Console.WriteLine(song);
                String name = SongsWindow.CheckSong(song);
                Console.WriteLine(name);
                if (name == "Error") continue;
                if (Directory.Exists(exe + "\\tmp\\finished\\" + name)) Directory.Delete(exe + "\\tmp\\finished\\" + name, true);
                ZipFile.ExtractToDirectory(exe + "\\tmp\\finished\\" + name + ".zip", exe + "\\tmp\\finished\\" + name);
                String hash = Songs.GetCustomLevelHash(exe + "\\tmp\\finished\\" + name.Trim());
                if (Directory.Exists(exe + "\\tmp\\MoveSongs\\custom_level_" + hash)) Directory.Delete(exe + "\\tmp\\MoveSongs\\custom_level_" + hash, true); 
                Directory.Move(exe + "\\tmp\\finished\\" + name.Trim(), exe + "\\tmp\\MoveSongs\\custom_level_" + hash);
                adb("push \"" + exe + "\\tmp\\MoveSongs\\custom_level_" + hash + "\" /sdcard/BMBFData/CustomSongs");
                txtbox.AppendText("\n\nSong " + hash + " has finished processing");
                txtbox.ScrollToEnd();
            }
            SongsWindow.Close();
            txtbox.AppendText("\n\nFinished. Please reload your songs folder in BMBF");
        }


        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (Running)
            {
                return;
            }
            txtbox.Text = "Output:";
            if (!Directory.Exists(path))
            {
                if ((bool)!auto.IsChecked)
                {
                    txtbox.AppendText("\n\nChoose a Source folder!");
                    txtbox.ScrollToEnd();
                    return;
                }
            }
            if (dest == null)
            {
                dest = path;
            }
            if ((bool)index.IsChecked)
            {
                //Index Songs
                if ((bool)zips.IsChecked)
                {
                    unzip(path, false);

                }
                else
                {
                    IndexSongs(path, dest, false);
                }

            }
            else
            {
                input(path, dest);
            }
        }

        public void unzip(String Path, Boolean download)
        {
            String f;
            string[] directories = Directory.GetFiles(Path);
            if (Directory.Exists(exe + "\\tmp\\Zips"))
            {
                Directory.Delete(exe + "\\tmp\\Zips", true);
            }
            if (!Directory.Exists(exe + "\\tmp\\Zips"))
            {
                Directory.CreateDirectory(exe + "\\tmp\\Zips");
            }
            String dest = exe + "\\tmp\\Zips";
            txtbox.AppendText("\nUnziping files to temporary folder.");
            txtbox.ScrollToEnd();

            foreach (String CD in directories)
            {
                if (!CD.EndsWith(".zip"))
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                    continue;
                }

                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                    f = System.IO.Path.GetFileNameWithoutExtension(CD);
                    ZipFile.ExtractToDirectory(CD, dest + "\\" + f);
                }));

            }
            txtbox.AppendText("\nUnziping complete.");
            txtbox.ScrollToEnd();
            IndexSongs(dest, Path, true);
        }


        public void IndexSongs(String Path, String dest, Boolean Zips)
        {
            if (Running)
            {
                return;
            }
            String zip = "";
            ArrayList list = new ArrayList();
            ArrayList Folder = new ArrayList();
            ArrayList BPM = new ArrayList();
            ArrayList Author = new ArrayList();
            ArrayList SubName = new ArrayList();
            ArrayList MAuthor = new ArrayList();
            ArrayList requierments = new ArrayList();
            ArrayList characteristics = new ArrayList();
            int exported = 0;
            String Name = "";
            String Source = Path;
            String B = "";
            String A = "";
            String S = "";
            String M = "";
            Running = true;

            if ((bool)auto.IsChecked)
            {
                txtbox.AppendText("\nAuto Mode enabled! Copying all Songs to " + exe + "\\tmp. Please be patient.");
                txtbox.ScrollToEnd();
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                if (!copied)
                {
                    if (!adb("pull /sdcard/BMBFData/CustomSongs/ \"" + exe + "\\tmp\""))
                    {
                        return;
                    }
                    copied = true;
                }
                if (Directory.Exists(exe + "\\tmp\\CustomSongs"))
                {
                    Source = exe + "\\tmp\\CustomSongs";
                }
                else
                {
                    Source = exe + "\\tmp";
                }

            }

            string[] directories = Directory.GetDirectories(Source);



            foreach (String CD in directories)
            {
                //Check if Folder is Valid Song
                txtbox.AppendText("\n");
                txtbox.ScrollToEnd();

                if (!File.Exists(CD + "\\" + "Info.dat") && !File.Exists(CD + "\\" + "info.dat"))
                {
                    txtbox.AppendText("\n" + CD + " is no Song");
                    txtbox.ScrollToEnd();
                    continue;
                }
                String dat = "";
                if (File.Exists(CD + "\\" + "Info.dat"))
                {
                    dat = CD + "\\" + "Info.dat";

                }
                if (File.Exists(CD + "\\" + "info.dat"))
                {
                    dat = CD + "\\" + "info.dat";

                }



                BeatSaberSong json = JsonSerializer.Deserialize<BeatSaberSong>(File.ReadAllText(dat));
                json.SongName = json.SongName.Trim();

                list.Add(json.SongName);
                txtbox.AppendText("\nSong Name: " + json.SongName);

                if (Zips)
                {
                    zip = dest + "\\" + System.IO.Path.GetDirectoryName(CD) + ".zip";
                    txtbox.AppendText("\nZip: " + zip);
                    Folder.Add(zip);
                } else
                {
                    txtbox.AppendText("\nFolder: " + CD);
                    Folder.Add(CD);
                }
                exported++;
                Name = "";



                /////////Requirements
                String characteristic = "";
                String requiered = "";
                foreach (BeatSaberSongBeatMapCharacteristic chara in json.BeatMapCharacteristics)
                {
                    characteristic += chara.BeatMapCharacteristicName + ", ";
                    foreach (BeatSaberSongDifficulty diff in chara.Difficulties)
                    {
                        JsonElement tmp = new JsonElement();
                        try
                        {
                            if (!diff._customData.TryGetProperty("_requirements", out tmp)) continue;
                        }
                        catch
                        {
                            continue;
                        }
                        Name = "";
                        foreach (JsonElement s in tmp.EnumerateArray()) Name += s.GetString();

                        if (Name.Contains("GameSaber") && !requiered.Contains("GameSaber, ")) requiered += "GameSaber, ";

                        if (Name.Contains("Mapping Extensions") && !requiered.Contains("Mapping Extensions, ")) requiered += "Mapping Extensions, ";

                        if (Name.Contains("Noodle Extensions") && !requiered.Contains("Noodle Extensions, ")) requiered += "Noodle Extensions, ";

                        if (Name.Contains("Chroma") && !requiered.Contains("Chroma, ")) requiered += "Chroma, ";
                    }
                }

                if (requiered == "")
                {
                    requiered = "N/A";
                }
                else
                {
                    requiered = requiered.Substring(0, requiered.Count() - 2);
                }

                if (characteristic == "")
                {
                    characteristic = "N/A";
                }
                else
                {
                    characteristic = characteristic.Substring(0, characteristic.Count() - 2);
                }

                requierments.Add(requiered);
                txtbox.AppendText("\nRequierments: " + requiered);
                characteristics.Add(characteristic);
                txtbox.AppendText("\nBeatMap Characteristics: " + characteristic);

                /////////Song Sub Name
                ///
                SubName.Add(json.SubName == "" ? "N/A" : json.SubName);
                txtbox.AppendText("\nSong Sub Name: " + (json.SubName == "" ? "N/A" : json.SubName));
                S = "";

                /////////Song Author

                Author.Add(json.SongArtist == "" ? "N/A" : json.SongArtist);
                txtbox.AppendText("\nSong author: " + (json.SongArtist == "" ? "N/A" : json.SongArtist));
                A = "";

                /////////Map Author

                MAuthor.Add(json.Mapper == "" ? "N/A" : json.Mapper);
                txtbox.AppendText("\nMap author: " + (json.Mapper == "" ? "N/A" : json.Mapper));
                M = "";


                /////////BPM

                BPM.Add(json.BPM == 0.0m ? "N/A" : json.BPM.ToString());
                txtbox.AppendText("\nBPM: " + (json.BPM == 0.0m ? "N/A" : json.BPM.ToString()));
                B = "";

                txtbox.ScrollToEnd();
            }
            ArrayList txt = new ArrayList();
            //ArrayList list = new ArrayList();
            //ArrayList Folder = new ArrayList();
            //ArrayList Version = new ArrayList();
            //ArrayList BPM = new ArrayList();
            //ArrayList Author = new ArrayList();
            //ArrayList SubName = new ArrayList();
            //ArrayList MAuthor = new ArrayList();
            txt.Add("List of " + exported + " Songs");
            txt.Add("Use ctrl + f to search for Songs");
            txt.Add("");
            txt.Add("");
            for (int C = 0; C < list.Count; C++)
            {
                txt.Add("Name: " + list[C]);
                txt.Add("Song Sub Name: " + SubName[C]);
                txt.Add("BPM: " + BPM[C]);
                txt.Add("Song Author: " + Author[C]);
                txt.Add("Map Author: " + MAuthor[C]);
                txt.Add("Requiered mods: " + requierments[C]);
                txt.Add("BeatMap Characteristics: " + characteristics[C]);
                
                if (Zips)
                {
                    txt.Add("Zip: " + Folder[C]);
                }
                else
                {
                    txt.Add("Folder: " + Folder[C]);
                }

                txt.Add("");
            }
            String[] output = (string[])txt.ToArray(typeof(string));
            File.WriteAllLines(dest + "\\" + "Songs.txt", output);
            txtbox.AppendText("\n");
            txtbox.AppendText("\n");
            txtbox.AppendText("Finished! Listed " + exported + " songs in Songs.txt");



            if (debug)
            {
                txtbox.AppendText("\n\n");
                txtbox.AppendText("\nNames: " + list.Count);
                txtbox.AppendText("\nSongSubNames: " + SubName.Count);
                txtbox.AppendText("\nBMPs: " + BPM.Count);
                txtbox.AppendText("\nSong Authors: " + Author.Count);
                txtbox.AppendText("\nMap Authors: " + MAuthor.Count);
                txtbox.AppendText("\nRequiered Mods: " + requierments.Count);
                if (Zips)
                {
                    txtbox.AppendText("\nZips: " + Folder.Count);
                }
                else
                {
                    txtbox.AppendText("\nFolders: " + Folder.Count);
                }

            }
            txtbox.ScrollToEnd();
            Running = false;
        }
    }
}
