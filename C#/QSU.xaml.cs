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
using System.Threading;
using ComputerUtils.RegxTemplates;
using BMBF.Config;
using ComputerUtils.StringFormatters;

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
            txtbox.Text = "";
            ApplyLanguage();
            if (debug)
            {
                txtbox.AppendText(exe);
            }
            
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

            Quest.Text = MainWindow.config.IP;

            Backups.SelectedIndex = 0;
            getBackups(exe + "\\Playlists");

            Playlists.Items.Add(MainWindow.globalLanguage.qSU.UI.loadPlaylists);
            Playlists.SelectedIndex = 0;

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
                uniformBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/QSU4.png", UriKind.Absolute));
                uniformBrush.Stretch = Stretch.UniformToFill;
                this.Background = uniformBrush;
            }
        }

        public void ApplyLanguage()
        {
            sr.Content = MainWindow.globalLanguage.qSU.UI.sourceFolderButton;
            destinationFolderButton.Content = MainWindow.globalLanguage.qSU.UI.destinationFolderButton;
            txtboxd.Text = MainWindow.globalLanguage.qSU.UI.sourceTextPlaceholder;
            txtboxs.Text = MainWindow.globalLanguage.qSU.UI.destinationPlaceholder;
            startButton.Content = MainWindow.globalLanguage.qSU.UI.startButton;
            backupPlaylistsButton.Content = MainWindow.globalLanguage.qSU.UI.backupPlaylistsButton;
            restorePlaylistsButton.Content = MainWindow.globalLanguage.qSU.UI.restorePlaylistsButton;
            loadPlaylistsButton.Content = MainWindow.globalLanguage.qSU.UI.loadPlaylistsButton;
            deletePlaylistButton.Content = MainWindow.globalLanguage.qSU.UI.deletePlaylistButton;
            createBPListButton.Content = MainWindow.globalLanguage.qSU.UI.createBPListButton;
            checkSongsButton.Content = MainWindow.globalLanguage.qSU.UI.checkSongsButton;
            BName.Text = MainWindow.globalLanguage.qSU.UI.backupNamePlaceholder;
            index.Content = MainWindow.globalLanguage.qSU.UI.makeListBox;
            zips.Content = MainWindow.globalLanguage.qSU.UI.onlyCheckZipsBox;
            box.Content = MainWindow.globalLanguage.qSU.UI.overwriteExistingBox;
            auto.Content = MainWindow.globalLanguage.qSU.UI.autoModeBox;
            SonglibswitcherButton.Content = MainWindow.globalLanguage.qSU.UI.SonglibswitcherButton;
        }

        public void TransferFromQSU()
        {
            if (MainWindow.config.QSUTransfered) return;
            MainWindow.config.QSUTransfered = true;
            MessageBoxResult r = MessageBox.Show(MainWindow.globalLanguage.qSU.code.qSUImportQuestion, "BMBF Manager - Quest Song Utilities", MessageBoxButton.YesNo, MessageBoxImage.Question);
            switch (r)
            {
                case MessageBoxResult.No:
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.qSU.code.nothingImported);
                    return;
            }
            MessageBox.Show(MainWindow.globalLanguage.qSU.code.qSUImportWindow, "BMBF Manager - Quest Song Utilities", MessageBoxButton.OK, MessageBoxImage.Information);

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
                                txtbox.AppendText(MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.skippingFolder, "\\CustomSongs\\" + System.IO.Path.GetDirectoryName(folder)));
                                continue;
                            }
                            String backupName = new DirectoryInfo(folder).Name;
                            Directory.Move(ofd.FileName + "\\CustomSongs\\" + backupName, exe + "\\CustomSongs\\" + backupName);
                        }

                        foreach (String file in Directory.GetFiles(ofd.FileName + "\\CustomSongs"))
                        {
                            if (File.Exists(exe + "\\CustomSongs\\" + System.IO.Path.GetFileName(file)))
                            {
                                txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.skippingFile, "\\CustomSongs\\" + System.IO.Path.GetFileName(file)));
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
                                txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.skippingFolder, "\\Playlists\\" + System.IO.Path.GetDirectoryName(folder)));
                                continue;
                            }
                            String backupName = new DirectoryInfo(folder).Name;
                            Directory.Move(ofd.FileName + "\\Playlists\\" + backupName, exe + "\\Playlists\\" + backupName);
                        }

                        foreach (String file in Directory.GetFiles(ofd.FileName + "\\Playlists"))
                        {
                            if (File.Exists(exe + "\\Playlists\\" + System.IO.Path.GetFileName(file)))
                            {
                                txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.skippingFile, "\\Playlists\\" + System.IO.Path.GetFileName(file)));
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
                                txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.skippingFolder, "\\BPLists\\" + System.IO.Path.GetDirectoryName(folder)));
                                continue;
                            }
                            String backupName = new DirectoryInfo(folder).Name;
                            Directory.Move(ofd.FileName + "\\BPLists\\" + backupName, exe + "\\BPLists\\" + backupName);
                        }

                        foreach (String file in Directory.GetFiles(ofd.FileName + "\\BPLists"))
                        {
                            if (File.Exists(exe + "\\BPLists\\" + System.IO.Path.GetFileName(file)))
                            {
                                txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.skippingFolder, "\\BPLists\\" + System.IO.Path.GetFileName(file)));
                                continue;
                            }
                            File.Move(ofd.FileName + "\\BPLists\\" + System.IO.Path.GetFileName(file), exe + "\\BPLists\\" + System.IO.Path.GetFileName(file));
                        }
                    }
                }
                else
                {
                    MessageBox.Show(MainWindow.globalLanguage.qSU.code.selectValidDir, "BMBF Manager - Quest Song Utilities", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

            }

            txtbox.AppendText("\n\n" + MainWindow.globalLanguage.qSU.code.allDataMoved);
        }

        public void Sync()
        {
            System.Threading.Thread.Sleep(2000);
            using (WebClient client = new WebClient())
            {
                client.QueryString.Add("foo", "foo");
                client.UploadValues("http://" + MainWindow.config.IP + ":50000/host/beatsaber/commitconfig", "POST", client.QueryString);
            }
        }

        public void StartBMBF()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                MainWindow.aDBI.adb("shell am start -n com.weloveoculus.BMBF/com.weloveoculus.BMBF.MainActivity", txtbox);
            }));
        }

        public void getPlaylists(object sender, RoutedEventArgs e)
        {
            StartBMBF();
            if (!MainWindow.iPUtils.CheckIP(Quest))
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.global.ipInvalid);
                return;
            }

            Playlists.Items.Clear();
            Playlists.Items.Add(MainWindow.globalLanguage.qSU.UI.playlistsName);

            WebClient client = new WebClient();

            var json2 = SimpleJSON.JSON.Parse(client.DownloadString("https://raw.githubusercontent.com/BMBF/resources/master/assets/beatsaber-knowns.json"));
            ArrayList known = new ArrayList();

            foreach (JSONNode pack in json2["knownLevelPackIds"])
            {
                known.Add(pack.ToString().Replace("\"", ""));
            }

            P = new ArrayList();

            var json = SimpleJSON.JSON.Parse(client.DownloadString("http://" + MainWindow.config.IP + ":50000/host/beatsaber/config"));
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
            txtbox.AppendText("\n\n" + MainWindow.globalLanguage.qSU.code.loadedPlaylists);
        }

        public void BPList(object sender, RoutedEventArgs e)
        {
            if (Running)
            {
                return;
            }
            if (!MainWindow.iPUtils.CheckIP(Quest))
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.global.ipInvalid);
                txtbox.ScrollToEnd();
                return;
            }
            Running = true;

            MainWindow.DCRPM.SetActivity(MainWindow.globalLanguage.dRCP.exportingBPList);

            if (Playlists.SelectedIndex == 0)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.qSU.code.choosePlaylist);
                txtbox.ScrollToEnd();
                Running = false;
                return;
            }

            WebClient c = new WebClient();
            var json = JSON.Parse(c.DownloadString("http://" + MainWindow.config.IP + ":50000/host/beatsaber/config"));
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.makingBPList, json["Config"]["Playlists"][Playlists.SelectedIndex + Lists - 1]["PlaylistName"]));
                txtbox.ScrollToEnd();
            }));
            var result = JSON.Parse("{}");
            result["playlistTitle"] = json["Config"]["Playlists"][Playlists.SelectedIndex + Lists - 1]["PlaylistName"];
            result["playlistAuthor"] = "Quest Song Utilities";
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                txtbox.AppendText("\n" + MainWindow.globalLanguage.qSU.code.downloadedPlaylistCover);
                txtbox.ScrollToEnd();
            }));
            using (WebClient client = new WebClient())
            {
                try
                {
                    client.DownloadFile("http://" + MainWindow.config.IP + ":50000/host/beatsaber/playlist/cover?PlaylistID=" + json["Config"]["Playlists"][Playlists.SelectedIndex + Lists - 1]["PlaylistID"], exe + "\\tmp\\Playlist.png");
                }
                catch
                {
                    txtbox.AppendText(MainWindow.globalLanguage.global.BMBF100);
                    txtbox.ScrollToEnd();
                    Running = false;
                    return;
                }
            }
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                txtbox.AppendText("\n" + MainWindow.globalLanguage.qSU.code.downloadedPlaylistCover);
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
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.bPListMade, json["Config"]["Playlists"][Playlists.SelectedIndex + Lists - 1]["PlaylistName"], "BPLists\\" + json["Config"]["Playlists"][Playlists.SelectedIndex + Lists - 1]["PlaylistName"] + ".bplist"));
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
            if (!MainWindow.iPUtils.CheckIP(Quest))
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.global.ipInvalid);
                txtbox.ScrollToEnd();
                return;
            }
            Running = true;
            if (Playlists.SelectedIndex == 0)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.qSU.code.choosePlaylist);
                txtbox.ScrollToEnd();
                Running = false;
                return;
            }
            MessageBoxResult result = MessageBox.Show(MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.confirmPlaylistDelete, Playlists.SelectedValue.ToString()), "BMBF Manager - Quest Song Utilities", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            switch (result)
            {
                case MessageBoxResult.No:
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.qSU.code.deletingAborted);
                    Running = false;
                    txtbox.ScrollToEnd();
                    return;
            }

            var json = SimpleJSON.JSON.Parse(File.ReadAllText(exe + "\\tmp\\config.json"));

            foreach (JSONNode song in json["Config"]["Playlists"][Playlists.SelectedIndex + Lists - 1]["SongList"])
            {
                if (!MainWindow.aDBI.adb("shell rm -rR /sdcard/BMBFData/CustomSongs/" + song["SongID"].ToString().Replace("\"", ""), txtbox))
                {
                    return;
                }
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                {
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.deletedSong, song["SongID"]));
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
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.qSU.code.deletedPlaylist);
                txtbox.ScrollToEnd();
            }
            catch
            {
                txtbox.AppendText(MainWindow.globalLanguage.global.BMBF100);
                txtbox.ScrollToEnd();
            }
            Running = false;
        }


        public void Move()
        {
            if (MainWindow.config.Converted) return;

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

            MainWindow.config.Converted = true;
        }

        public void getBackups(String Path)
        {
            ArrayList Jsons = new ArrayList();
            string[] Files = Directory.GetFiles(Path);
            Backups.Items.Clear();
            Backups.Items.Add(MainWindow.globalLanguage.qSU.UI.BackupName);

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
                Quest.Text = MainWindow.globalLanguage.global.defaultQuestIPText;
            }
        }

        private void BackupNameCheck(object sender, RoutedEventArgs e)
        {
            if (BName.Text == "")
            {
                BName.Text = MainWindow.globalLanguage.qSU.UI.backupNamePlaceholder;
            }
        }


        private void Backup(object sender, RoutedEventArgs e)
        {
            StartBMBF();
            if (Running)
            {
                return;
            }
            Boolean good = MainWindow.iPUtils.CheckIP(Quest);
            if (!good)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.global.ipInvalid);
                txtbox.ScrollToEnd();
                Running = false;
                return;
            }
            Running = true;

            MainWindow.DCRPM.SetActivity(MainWindow.globalLanguage.dRCP.backingUpPlaylists);

            try
            {
                MainWindow.iPUtils.CheckIP(Quest);
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
                BName.Text = BName.Text.Replace(@"\", "");
                BName.Text = BName.Text.Trim();

                if (File.Exists(exe + "\\Playlists\\" + BName.Text + ".json"))
                {
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.qSU.code.backupAlreadyExists);
                    txtbox.ScrollToEnd();
                    Running = false;
                    return;
                }

                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.backingUpPlaylists, "\\Playlists\\" + BName.Text + ".json"));
                txtbox.ScrollToEnd();
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));

                MainWindow.aDBI.adb("pull /sdcard/BMBFData/Playlists/ \"" + exe + "\\Playlists\"", txtbox);

                WebClient client = new WebClient();
                var json = JSON.Parse(client.DownloadString("http://" + MainWindow.config.IP + ":50000/host/beatsaber/config"));
                json["IsCommitted"] = false;
                File.WriteAllText(exe + "\\Playlists\\" + BName.Text + ".json", json["Config"].ToString());

                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.backedUpPlaylists, "\\Playlists\\" + BName.Text + ".json"));
                txtbox.ScrollToEnd();
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
            }
            catch
            {
                txtbox.AppendText(MainWindow.globalLanguage.global.PL100);
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
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.mainMenu.code.pushingPng, directories[i]));
                    MainWindow.aDBI.adb("push \"" + directories[i] + "\" /sdcard/BMBFData/Playlists/", txtbox);
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
                client.UploadFile("http://" + MainWindow.config.IP + ":50000/host/beatsaber/config", "PUT", Config);
                client.UploadValues("http://" + MainWindow.config.IP + ":50000/host/beatsaber/commitconfig", "POST", client.QueryString);
            }
        }


        private void Restore(object sender, RoutedEventArgs e)
        {
            StartBMBF();
            if (Running)
            {
                return;
            }
            if (!MainWindow.iPUtils.CheckIP(Quest))
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.global.ipInvalid);
                txtbox.ScrollToEnd();
                Running = false;
                return;
            }
            if (Backups.SelectedIndex == 0)
            {
                return;
            }
            Running = true;

            MainWindow.DCRPM.SetActivity(MainWindow.globalLanguage.dRCP.restoringPlaylists);

            try
            {
                MainWindow.iPUtils.CheckIP(Quest);

                String Playlists;
                if (dest == null)
                {
                    dest = path;
                }

                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.restoringPlaylists, "\\Playlists\\" + Backups.SelectedValue + ".json"));
                txtbox.ScrollToEnd();
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));



                if (!Directory.Exists(exe + "\\tmp"))
                {
                    Directory.CreateDirectory(exe + "\\tmp");
                }
                WebClient client = new WebClient();

                Playlists = exe + "\\Playlists\\" + Backups.SelectedValue + ".json";

                var j = JSON.Parse(client.DownloadString("http://" + MainWindow.config.IP + ":50000/host/beatsaber/config"));
                var p = JSON.Parse(File.ReadAllText(Playlists));


                j["Config"]["Playlists"] = p["Playlists"];
                File.WriteAllText(exe + "\\tmp\\config.json", j["Config"].ToString());

                PushPNG(exe + "\\Playlists\\Playlists");
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                    postChanges(exe + "\\tmp\\config.json");
                }));
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.qSU.code.restoredPlaylists);
                txtbox.ScrollToEnd();
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
            }
            catch
            {
                txtbox.AppendText(MainWindow.globalLanguage.global.BMBF100);
                txtbox.ScrollToEnd();
            }
            Running = false;
        }

        private void ClearText(object sender, RoutedEventArgs e)
        {
            if (Quest.Text == MainWindow.globalLanguage.global.ipInvalid)
            {
                Quest.Text = "";
            }

        }

        private void ClearTextN(object sender, RoutedEventArgs e)
        {
            if (BName.Text == MainWindow.globalLanguage.qSU.UI.backupNamePlaceholder)
            {
                BName.Text = "";
            }
        }


        private void Auto(object sender, RoutedEventArgs e)
        {
            if ((bool)auto.IsChecked)
            {
                automode = true;
                txtboxs.Text = MainWindow.globalLanguage.qSU.code.oculusQuestName;
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
                txtboxs.Text = MainWindow.globalLanguage.qSU.code.chooseSongFolder;
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
            MainWindow.iPUtils.CheckIP(Quest);
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
                    txtbox.Text = MainWindow.globalLanguage.qSU.code.debuggingEnabled;
                }
                else
                {
                    debug = false;
                }
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                txtboxs.Text = path;

            }
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

            MainWindow.DCRPM.SetActivity(MainWindow.globalLanguage.dRCP.zippingSongs);

            if ((bool)auto.IsChecked)
            {
                txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.autoModeOn, exe + "\\tmp"));
                txtbox.ScrollToEnd();
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                if (!copied)
                {
                    if (!MainWindow.aDBI.adb("pull /sdcard/BMBFData/CustomSongs/ \"" + exe + "\\tmp\"", txtbox))
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
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.qSU.code.zippingSongs);
                txtbox.ScrollToEnd();

            }

            string[] directories = Directory.GetDirectories(Source);



            foreach (String CD in directories)
            {
                txtbox.AppendText("\n");


                if (!File.Exists(CD + "\\" + "Info.dat") && !File.Exists(CD + "\\" + "info.dat"))
                {
                    txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.isNoSong, CD));
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
                    txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.songName, Name));
                    txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.folder, CD));

                    if ((bool)box.IsChecked)
                    {
                        bool v = File.Exists(dest + "\\" + Name + ".zip");
                        if (v)
                        {
                            File.Delete(dest + "\\" + Name + ".zip");
                            txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.overwrittenFile, dest + "\\" + Name + ".zip"));
                            if (debug)
                            {
                                over.Add("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.songName, Name));
                                over.Add("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.folder, CD));
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
                            txtbox.AppendText("\n" + MainWindow.globalLanguage.qSU.code.songExists);
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
                txtbox.AppendText(MainWindow.globalLanguage.global.QSU110);
            }
            else if (exported == 0 && !(bool)auto.IsChecked)
            {
                txtbox.AppendText(MainWindow.globalLanguage.global.QSU100);
            }
            else
            {
                txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.finishedZipping, exported.ToString())); ;
            }
            if ((bool)auto.IsChecked && dest == exe + "\\CustomSongs")
            {
                txtbox.AppendText("\n" + MainWindow.globalLanguage.qSU.code.autoModeWasEnabled);
            }
            if ((bool)box.IsChecked)
            {
                txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.showOverwritten, overwritten.ToString()));
                if (debug)
                {
                    txtbox.AppendText("\n" + MainWindow.globalLanguage.qSU.code.debugOverwritten + "\n");
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
            MessageBoxResult r = MessageBox.Show(MainWindow.globalLanguage.qSU.code.checkFoldersWarning, "BMBF Manager - Quest Song Utilities", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(r == MessageBoxResult.No) 
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.qSU.code.aborted);
                return;
            }
            Directory.CreateDirectory(exe + "\\tmp\\CustomSongs");

            MainWindow.DCRPM.SetActivity(MainWindow.globalLanguage.dRCP.correctingSongPath);

            if (!MainWindow.aDBI.adb("pull /sdcard/BMBFData/CustomSongs/ \"" + exe + "\\tmp\"", txtbox)) return;
            MainWindow.aDBI.adb("shell rm -r /sdcard/BMBFData/CustomSongs/", txtbox);
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
                MainWindow.aDBI.adb("push \"" + exe + "\\tmp\\MoveSongs\\custom_level_" + hash + "\" /sdcard/BMBFData/CustomSongs", txtbox);
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.songFinishedProcessing, hash));
                txtbox.ScrollToEnd();
            }
            SongsWindow.Close();
            txtbox.AppendText("\n\n" + MainWindow.globalLanguage.qSU.code.finishedChecking);
        }

        private void TransferSongLib(object sender, RoutedEventArgs e)
        {
            //try
            //{
            if (Running)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.qSU.code.operationRunning);
                return;
            }
            if(!MainWindow.iPUtils.CheckIP(Quest))
            {
                txtbox.AppendText(MainWindow.globalLanguage.global.ipInvalid);
                return;
            }
            MessageBoxResult r = MessageBox.Show(MainWindow.globalLanguage.qSU.code.songLibSwitchInfo, "BMBF Manager - Quest Song Utilities", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            bool QuestToPC = false;
            switch (r)
            {
                case MessageBoxResult.Yes:
                    QuestToPC = true;
                    break;
                case MessageBoxResult.Cancel:
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.qSU.code.aborted);
                    return;
            }
            MessageBox.Show(MainWindow.globalLanguage.qSU.code.choosePCModsFolder, "BMBF Manager - Quest Song Utilities", MessageBoxButton.OK, MessageBoxImage.Information);
            CommonOpenFileDialog fd = new CommonOpenFileDialog();
            fd.IsFolderPicker = true;
            String CustomSongsFolder = "";
            if (fd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if (Directory.Exists(fd.FileName))
                {
                    CustomSongsFolder = fd.FileName;
                }
                else
                {
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.qSU.code.folderDoesntExist);
                    return;
                }
            }
            else
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.qSU.code.aborted);
                return;
            }

            Running = true;

            MainWindow.Log("CustomLevels folder: " + CustomSongsFolder);

            Directory.CreateDirectory(exe + "\\tmp\\Move");

            if (QuestToPC)
            {
                MainWindow.DCRPM.SetActivity(MainWindow.globalLanguage.dRCP.switchingSongLibraryQuestPC);

                MainWindow.Log("User choose QuestToPC");
                r = MessageBox.Show(MainWindow.globalLanguage.qSU.code.startInfoQuestToPC, "BMBF Manager - Quest Song Utilities", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                if (r == MessageBoxResult.Cancel)
                {
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.qSU.code.aborted);
                    Running = false;
                    return;
                }
                if (!MainWindow.aDBI.adb("pull /sdcard/BMBFData/CustomSongs \"" + exe + "\\tmp\"", txtbox))
                {
                    Running = false;
                    return;
                }
                List<SongLibraryMoveSong> songs = new List<SongLibraryMoveSong>();
                TimeoutWebClientShort c = new TimeoutWebClientShort();
                bool importPlaylists = true;
                String configString = "";
                BMBFC config = new BMBFC();
                try
                {
                    configString = c.DownloadString("http://" + MainWindow.config.IP + ":50000/host/beatsaber/config");
                    config = JsonSerializer.Deserialize<BMBFC>(configString);
                } catch
                {
                    importPlaylists = false;
                    MainWindow.Log("Couldn't get BMBF config. Not restoring playlists!");
                }
                Songs s = new Songs();
                BeatSaverAPIInteractor i = new BeatSaverAPIInteractor();


                foreach (String dir in Directory.GetDirectories(exe + "\\tmp\\CustomSongs"))
                {
                    MainWindow.Log("Processing Song: dir: " + dir);
                    SongLibraryMoveSong moveSong = new SongLibraryMoveSong();
                    String zipName;
                    if ((zipName = s.CheckSong(dir)) == "Error") continue;
                    if (Directory.Exists(exe + "\\tmp\\Move\\" + zipName.Trim())) Directory.Delete(exe + "\\tmp\\Move\\" + zipName.Trim(), true);
                    MainWindow.Log("Extracting Song");
                    ZipFile.ExtractToDirectory(exe + "\\tmp\\finished\\" + zipName + ".zip", exe + "\\tmp\\Move\\" + zipName.Trim().TrimEnd('.'));
                    moveSong.sourceFolder = exe + "\\tmp\\Move\\" + zipName.Trim().TrimEnd('.');
                    MainWindow.Log("Getting hash");
                    moveSong.hash = Songs.GetCustomLevelHash(exe + "\\tmp\\Move\\" + zipName.Trim().TrimEnd('.'));
                    BeatSaverAPISong song = i.GetBeatSaverAPISongViaHash(moveSong.hash);
                    //Define default name
                    String finishedName = "Custom_level_" + moveSong.hash;
                    if (song.GoodRequest)
                    {
                        //Use SongInfo from BeatSaver
                        finishedName = song.name + " - " + song.metadata.songAuthorName + " (" + song.key + ")";
                        moveSong.songArtist = song.metadata.songAuthorName;
                        moveSong.songName = song.metadata.songName;
                        moveSong.key = song.key;
                    }
                    else
                    {
                        //Try to find song in BMBF Config
                        foreach (BMBFPlaylist playlist in config.Config.Playlists)
                        {
                            foreach (BMBFSong song1 in playlist.SongList)
                            {
                                if (song1.SongID.ToLower().Contains(moveSong.hash.ToLower()))
                                {
                                    finishedName = song1.SongName + " - " + song1.SongAuthorName;
                                    moveSong.songName = song1.SongName;
                                    moveSong.songArtist = song1.SongAuthorName;
                                    break;
                                }
                            }
                        }
                        if (moveSong.songName == "")
                        {
                            BeatSaberSong beatSaberSong = JsonSerializer.Deserialize<BeatSaberSong>(File.ReadAllText(Directory.GetFiles(moveSong.sourceFolder).FirstOrDefault(x => x.ToLower() == "info.dat")));
                            finishedName = beatSaberSong.SongName + " - " + beatSaberSong.SongArtist;
                            moveSong.songName = beatSaberSong.SongName;
                            moveSong.songArtist = beatSaberSong.SongArtist;
                        }
                    }
                    //Remove invalid chars
                    finishedName = StringFormatter.FileNameSafe(finishedName);
                    moveSong.targetFolder = CustomSongsFolder + "\\" + finishedName;
                    int end = 0;
                    while (Directory.Exists(moveSong.targetFolder))
                    {
                        end++;
                        //Don't ask. It increments the number at the end by 1
                        moveSong.targetFolder = moveSong.targetFolder.EndsWith((end - 1).ToString()) ? moveSong.targetFolder.Substring(0, moveSong.targetFolder.Length - (end - 1).ToString().Length) + end : moveSong.targetFolder + " " + end;
                    }
                    //Now everything should be right ig.
                    songs.Add(moveSong);
                    MainWindow.Log("Moving Song: Name: " + moveSong.songName + ", hash: " + moveSong.hash + ", source: " + moveSong.sourceFolder + ", dest: " + moveSong.targetFolder);
                    Directory.Move(moveSong.sourceFolder, moveSong.targetFolder);
                    //Wait 500ms to reduce 429's
                    Thread.Sleep(500);
                }

                //Restore Playlists
                String PlaylistFolder = String.Join("\\", CustomSongsFolder.Split('\\').ToList<String>().GetRange(0, CustomSongsFolder.Split('\\').Length - 2)) + "\\Playlists";
                if (!Directory.Exists(PlaylistFolder)) Directory.CreateDirectory(PlaylistFolder);

                BSKFile known = new BSKFile();

                WebClient client = new WebClient();
                try
                {
                    String t = client.DownloadString(PlaylistEditor.bsk);
                    known = JsonSerializer.Deserialize<BSKFile>(t);
                }
                catch
                {
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.global.anErrorOccured);
                    txtbox.ScrollToEnd();
                }

                foreach (BMBFPlaylist p in config.Config.Playlists)
                {
                    if (known.knownLevelPackIds.Contains(p.PlaylistID)) continue;
                    BPList list = new BPList();
                    list.playlistTitle = p.PlaylistName;
                    list.playlistAuthor = "BMBF Manager";
                    foreach (BMBFSong song1 in p.SongList)
                    {
                        foreach (SongLibraryMoveSong moveSong in songs)
                        {
                            if (song1.SongID.ToLower().Contains(moveSong.hash.ToLower()))
                            {
                                BPListSong song = new BPListSong();
                                song.hash = moveSong.hash;
                                song.songName = moveSong.songName;
                                list.songs.Add(song);
                                break;
                            }
                        }
                    }
                    File.WriteAllText(PlaylistFolder + "\\" + list.playlistTitle + ".bplist", JsonSerializer.Serialize(list));
                }
                if(!importPlaylists)
                {
                    MessageBox.Show(MainWindow.globalLanguage.qSU.code.notRestoringPlaylists, "BMBF Manager - Quest Song Utilities", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                MessageBox.Show(MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.songLibraryMoved, songs.Count.ToString()), "BMBF Manager - Quest Song Utilities", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MainWindow.DCRPM.SetActivity(MainWindow.globalLanguage.dRCP.switchingSongLibraryPCQuest);
                MainWindow.Log("User choose PCToQuest");
                r = MessageBox.Show(MainWindow.globalLanguage.qSU.code.startInfoPCToQuest, "BMBF Manager - Quest Song Utilities", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                if (r == MessageBoxResult.Cancel)
                {
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.qSU.code.aborted);
                    Running = false;
                    return;
                }

                Songs s = new Songs();

                TimeoutWebClientShort c = new TimeoutWebClientShort();
                bool importPlaylists = true;
                String configString = "";
                BMBFC config = new BMBFC();
                try
                {
                    configString = c.DownloadString("http://" + MainWindow.config.IP + ":50000/host/beatsaber/config");
                    config = JsonSerializer.Deserialize<BMBFC>(configString);
                }
                catch
                {
                    importPlaylists = false;
                    MainWindow.Log("Couldn't get BMBF config. Not restoring playlists!");
                }
                int songCount = Directory.GetDirectories(CustomSongsFolder).Length;
                songCount += config.Config.GetTotalSongsCount();

                int limit = -1;

                MainWindow.Log("Song count: " + songCount);

                if (songCount > 500)
                {
                    MainWindow.Log("Showing song warning");
                    r = MessageBox.Show(MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.tooMuchSongs, limit.ToString()), "BMBF Manager - Quest Song Utilities", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                    if (r == MessageBoxResult.Yes) limit = 500;
                }

                List<SongLibraryMoveSong> songs = new List<SongLibraryMoveSong>();
                int i = 0;
                foreach (String dir in Directory.GetDirectories(CustomSongsFolder))
                {
                    //Do stuff for checking...
                    if (i > limit && limit != -1) break;
                    SongLibraryMoveSong moveSong = new SongLibraryMoveSong();
                    String zipName;
                    MainWindow.Log("");
                    MainWindow.Log("Moving song: " + dir);
                    if ((zipName = s.CheckSong(dir)) == "Error") continue;
                    if (Directory.Exists(exe + "\\tmp\\Move\\" + zipName.Trim())) Directory.Delete(exe + "\\tmp\\Move\\" + zipName.Trim(), true);
                    MainWindow.Log("Extracting");
                    ZipFile.ExtractToDirectory(exe + "\\tmp\\finished\\" + zipName + ".zip", exe + "\\tmp\\Move\\" + zipName.Trim());
                    moveSong.hash = Songs.GetCustomLevelHash(exe + "\\tmp\\Move\\" + zipName.Trim().TrimEnd('.'));
                    String finishedName = "custom_level_" + moveSong.hash;
                    finishedName = StringFormatter.FileNameSafe(finishedName);
                    if (Directory.Exists(exe + "\\tmp\\Move\\" + finishedName)) Directory.Delete(exe + "\\tmp\\Move\\" + finishedName, true);
                    Directory.Move(exe + "\\tmp\\Move\\" + zipName.Trim().TrimEnd('.'), exe + "\\tmp\\Move\\" + finishedName);
                    moveSong.sourceFolder = exe + "\\tmp\\Move\\" + finishedName;
                    MainWindow.Log("Moving Song: Name: " + moveSong.songName + ", hash: " + moveSong.hash + ", source: " + moveSong.sourceFolder + ", dest: /sdcard/BMBFData/CustomSongs/" + finishedName);

                    //Push
                    if (!MainWindow.aDBI.adb("push \"" + moveSong.sourceFolder + "\" /sdcard/BMBFData/CustomSongs/", txtbox))
                    {
                        Running = false;
                        break;
                    }

                    songs.Add(moveSong);
                    i++;
                }


                //Can fail so I'm asking the user to do it
                //Support support = new Support();
                //support.reloadsongsfolder();
                //Wait for BMBF to finish stuff
                Thread.Sleep(3000);

                MainWindow.Log("Asking user to reload songs");
                Process.Start("http://" + MainWindow.config.IP + ":50000/main/tools");
                MessageBox.Show(MainWindow.globalLanguage.qSU.code.reloadSongsFolder, "BMBF Manager - Quest Song Utilities");
                r = MessageBox.Show(MainWindow.globalLanguage.qSU.code.reloadSongsFolderConfirmation, "BMBF Manager - Quest Song Utilities", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (r == MessageBoxResult.No)
                {
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.SongsFolderNotReloaded, songs.Count.ToString()));
                    Running = false;
                    return;
                }
                MainWindow.Log("User reloaded songs");

                if(importPlaylists)
                {
                    PlaylistEditor PE = new PlaylistEditor();
                    PE.Show();

                    String PlaylistFolder = String.Join("\\", CustomSongsFolder.Split('\\').ToList<String>().GetRange(0, CustomSongsFolder.Split('\\').Length - 2)) + "\\Playlists";
                    foreach (String file in Directory.GetFiles(PlaylistFolder))
                    {
                        MainWindow.Log("Importing BPList from: " + file);
                        PE.ImportBPList(JsonSerializer.Deserialize<BPList>(File.ReadAllText(file)), true);
                    }
                    MainWindow.Log("Saving Playlist configuration");
                    PE.SaveAll();
                } else
                {
                    MessageBox.Show(MainWindow.globalLanguage.qSU.code.notRestoringPlaylists, "BMBF Manager - Quest Song Utilities", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                    
                MessageBox.Show(MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.songLibraryMoved, songs.Count.ToString()), "BMBF Manager - Quest Song Utilities", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            MainWindow.Log("finished");
            Running = false;
            //} catch(Exception ex)
            //{
            //    MainWindow.Log("Crash at song library transfer: \n\n" + ex.ToString());
            //    MessageBox.Show("The program just got a error. Please contact ComputerElite and send him log.log which is located along the exe.", "BMBF Manager - Error report", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (Running)
            {
                return;
            }
            if (!Directory.Exists(path))
            {
                if ((bool)!auto.IsChecked)
                {
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.qSU.code.chooseSongFolder);
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
            txtbox.AppendText("\n" + MainWindow.globalLanguage.qSU.code.unzippingFiles);
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
            txtbox.AppendText("\n" + MainWindow.globalLanguage.qSU.code.unzippingFilesComplete);
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
            Running = true;

            MainWindow.DCRPM.SetActivity(MainWindow.globalLanguage.dRCP.makingSongList);

            if ((bool)auto.IsChecked)
            {
                txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.autoModeOn, exe + "\\tmp"));
                txtbox.ScrollToEnd();
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                if (!copied)
                {
                    if (!MainWindow.aDBI.adb("pull /sdcard/BMBFData/CustomSongs/ \"" + exe + "\\tmp\"", txtbox))
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
                    txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.isNoSong, CD));
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
                txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.songName, json.SongName));

                if (Zips)
                {
                    zip = dest + "\\" + System.IO.Path.GetDirectoryName(CD) + ".zip";
                    txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.zip, zip));
                    Folder.Add(zip);
                } else
                {
                    txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.folder, CD));
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
                txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.requirements, requiered));
                characteristics.Add(characteristic);
                txtbox.AppendText("\nBeatMap Characteristics: " + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.characteristics, characteristic));

                /////////Song Sub Name
                ///
                SubName.Add(json.SubName == "" ? "N/A" : json.SubName);
                txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.songSubName, json.SubName == "" ? "N/A" : json.SubName));

                /////////Song Author

                Author.Add(json.SongArtist == "" ? "N/A" : json.SongArtist);
                txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.songAuthor, json.SongArtist == "" ? "N/A" : json.SongArtist));

                /////////Map Author

                MAuthor.Add(json.Mapper == "" ? "N/A" : json.Mapper);
                txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.mapAuthor, json.Mapper == "" ? "N/A" : json.Mapper));


                /////////BPM

                BPM.Add(json.BPM == 0.0m ? "N/A" : json.BPM.ToString());
                txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.bPM, json.BPM == 0.0m ? "N/A" : json.BPM.ToString()));

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
            txt.Add(MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.listMeta, exported.ToString()));
            txt.Add(MainWindow.globalLanguage.qSU.code.search);
            txt.Add("");
            txt.Add("");
            for (int C = 0; C < list.Count; C++)
            {
                txt.Add(MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.songName, list[C].ToString()));
                txt.Add(MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.songSubName, SubName[C].ToString()));
                txt.Add(MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.bPM, BPM[C].ToString()));
                txt.Add(MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.songAuthor, Author[C].ToString()));
                txt.Add(MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.mapAuthor, MAuthor[C].ToString()));
                txt.Add(MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.requirements, requierments[C].ToString()));
                txt.Add(MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.characteristics, characteristics[C].ToString()));
                
                if (Zips)
                {
                    txt.Add(MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.zip, Folder[C].ToString()));
                }
                else
                {
                    txt.Add(MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.folder, Folder[C].ToString()));
                }

                txt.Add("");
            }
            String[] output = (string[])txt.ToArray(typeof(string));
            File.WriteAllLines(dest + "\\" + "Songs.txt", output);
            txtbox.AppendText("\n");
            txtbox.AppendText("\n");
            txtbox.AppendText(MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.qSU.code.finishedIndex, exported.ToString()));
            txtbox.ScrollToEnd();
            Running = false;
        }
    }
}
