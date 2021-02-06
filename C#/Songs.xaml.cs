using Microsoft.Win32;
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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using BeatSaverAPI;
using System.Text.RegularExpressions;
using ComputerUtils.RegxTemplates;

namespace BMBF_Manager
{
    /// <summary>
    /// Interaktionslogik für Songs.xaml
    /// </summary>
    public partial class Songs : Window
    {
        Boolean draggable = true;
        Boolean Running = false;
        String exe = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 1);
        String Key = "";
        int C = 0;
        private static Songs instance = new Songs();
        ArrayList SongKeys = new ArrayList();
        List<Tuple<String, bool>> downloadqueue = new List<Tuple<String, bool>>();
        BeatSaverAPIInteractor interactor = new BeatSaverAPIInteractor();
        int installed = 0;

        bool OneClick = false;
        bool PEO = false;
        bool canceled = false;

        public Songs()
        {
            InitializeComponent();
            ApplyLanguage();
            Quest.Text = MainWindow.IP;
            DownloadLable.Text = MainWindow.globalLanguage.global.allFinished;
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
                uniformBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Songs5.png", UriKind.Absolute));
                uniformBrush.Stretch = Stretch.UniformToFill;
                this.Background = uniformBrush;
            }
        }

        public void ApplyLanguage()
        {
            searchButton.Content = MainWindow.globalLanguage.songs.UI.searchButton;
            showMetadataButton.Content = MainWindow.globalLanguage.songs.UI.showMetadataButton;
            cancleDownloadsButton.Content = MainWindow.globalLanguage.songs.UI.cancleDownloadsButton;
            installSongButton.Content = MainWindow.globalLanguage.songs.UI.installSongButton;
            installPCSongButton.Content = MainWindow.globalLanguage.songs.UI.installPCSongButton;
            SongKey.Text = MainWindow.globalLanguage.songs.UI.songKeyPlaceholder;
            SearchTerm.Text = MainWindow.globalLanguage.songs.UI.searchTermPlaceholder;
            ((GridView)SongList.View).Columns[0].Header = MainWindow.globalLanguage.playlistEditor.UI.songNameList;
            ((GridView)SongList.View).Columns[1].Header = MainWindow.globalLanguage.playlistEditor.UI.mapperList;
            ((GridView)SongList.View).Columns[2].Header = MainWindow.globalLanguage.playlistEditor.UI.artistList;
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
            if (Quest.Text == MainWindow.globalLanguage.global.defaultQuestIPText)
            {
                Quest.Text = "";
            }

        }

        private void ClearKey(object sender, RoutedEventArgs e)
        {
            if (SongKey.Text == MainWindow.globalLanguage.songs.UI.songKeyPlaceholder)
            {
                SongKey.Text = "";
            }

        }

        private void ClearSearch(object sender, RoutedEventArgs e)
        {
            if (SearchTerm.Text == MainWindow.globalLanguage.songs.UI.searchTermPlaceholder)
            {
                SearchTerm.Text = "";
            }

        }

        private void QuestIPCheck(object sender, RoutedEventArgs e)
        {
            if (Quest.Text == "")
            {
                Quest.Text = MainWindow.globalLanguage.global.defaultQuestIPText;
            }
        }

        private void SearchTermCheck(object sender, RoutedEventArgs e)
        {
            if (SearchTerm.Text == "")
            {
                SearchTerm.Text = MainWindow.globalLanguage.songs.UI.searchTermPlaceholder;
            }
        }

        private void SongKeyCheck(object sender, RoutedEventArgs e)
        {
            if (SongKey.Text == "")
            {
                SongKey.Text = MainWindow.globalLanguage.songs.UI.songKeyPlaceholder;
            }
        }


        private void LoadSongData(object sender, RoutedEventArgs e)
        {
            if (SongKey.Text == MainWindow.globalLanguage.songs.UI.searchTermPlaceholder)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.songs.code.chooseSongKey);
                return;
            }

            Key = SongKey.Text;

            BeatSaberSong song = interactor.GetBeatSaberSong(Key);
            if(!song.RequestGood)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.songs.code.beatmapDoesntExist, Key)) ;
                txtbox.ScrollToEnd();
                return;
            }
            
            String SongName = song.SongName;
            String SongArtist = song.SongArtist;
            String MapAuthor = song.Mapper;
            String SubName = song.SubName;
            String BPM = song.BPM.ToString();

            if (SongName == "") SongName = "N/A";
            if (SongArtist == "") SongArtist = "N/A";
            if (MapAuthor == "") MapAuthor = "N/A";
            if (SubName == "") SubName = "N/A";
            if (BPM == "") BPM = "N/A";

            txtbox.AppendText("\n\n" + MainWindow.globalLanguage.songs.code.metadataShowing);

            txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.songs.code.songName, SongName));
            txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.songs.code.songArtist, SongArtist));
            txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.songs.code.mapAuthor, MapAuthor));
            txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.songs.code.songSubName, SubName));
            txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.songs.code.bPM, BPM));
            txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.songs.code.beatMapKey, Key));
            txtbox.ScrollToEnd();
        }

        private void Search(object sender, RoutedEventArgs e)
        {
            SongList.Items.Clear();

            BeatSaverAPISearchResult result = interactor.SearchText(SearchTerm.Text);
            if(!result.RequestGood)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.songs.code.beatSaverError);
                txtbox.ScrollToEnd();
                return;
            }

            foreach (BeatSaverAPISong doc in result.docs)
            {
                String Name = doc.name;
                String Mapper = doc.metadata.levelAuthorName;
                String Artist = doc.metadata.songAuthorName;

                SongKeys.Add(doc.key);
                SongList.Items.Add(new SongItem { Name = Name, Mapper = Mapper, Artist = Artist });
            }

            if(SongKeys.Count < 1)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.songs.code.noResultsFound);
                txtbox.ScrollToEnd();
            }

            SongList.SelectedIndex = 0;
        }

        public Boolean CheckIP()
        {
            getQuestIP();
            String found;
            if ((found = RegexTemplates.GetIP(MainWindow.IP)) != "")
            {
                MainWindow.IP = found;
                Quest.Text = MainWindow.IP;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void getQuestIP()
        {
            MainWindow.IP = Quest.Text;
            return;
        }

        private void cancel(object sender, RoutedEventArgs e)
        {
            if(downloadqueue.Count() == 0)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.songs.code.cantCancleNonActive);
                txtbox.ScrollToEnd();
            } else
            {
                canceled = true;
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.songs.code.clearedQueue);
                txtbox.ScrollToEnd();
            }
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
                                txtbox.AppendText(MainWindow.globalLanguage.global.ADB110);
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
            txtbox.AppendText(MainWindow.globalLanguage.global.ADB100);
            txtbox.ScrollToEnd();
            return false;
        }

        private void InstallZip(object sender, RoutedEventArgs e)
        {
            if (!CheckIP())
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.global.ipInvalid);
                txtbox.ScrollToEnd();
                return;
            }

            String Input = "";
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = MainWindow.globalLanguage.songs.code.zipFile + " (*.zip)|*.zip";
            DialogResult result = ofd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.Cancel) { Running = false; return; }
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                //Get the path of specified file
                if (File.Exists(ofd.FileName))
                {
                    Input = ofd.FileName;
                }
                else
                {
                    MessageBox.Show(MainWindow.globalLanguage.songs.code.selectValidZip, "BMBF Manager - Zip Song Installing", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

            }
            FileInfo fi = new FileInfo(Input);
            long ZipSize = fi.Length;
            if(ZipSize < 50000000) //35 MB
            {
                if (downloadqueue.Contains(new Tuple<string, bool>(Input, true)))
                {
                    txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.songs.code.songAlreadyInQueue, System.IO.Path.GetFileName(Input)));
                    return;
                }
                downloadqueue.Add(new Tuple<string, bool>(Input, true));
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.songs.code.songAddedToQueue, System.IO.Path.GetFileName(Input)));
                checkqueue();
                return;
            }

            MessageBoxResult result1 = MessageBox.Show(MainWindow.globalLanguage.songs.code.songBig, "BMBF Manager - Zip Song installing", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            switch (result1)
            {
                case MessageBoxResult.No:
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.songs.code.songInstallAborted);
                    txtbox.ScrollToEnd();
                    return;
            }

            if (Directory.Exists(exe + "\\tmp\\unzipped")) Directory.Delete(exe + "\\tmp\\unzipped", true);

            String name = CheckSongZip(Input);
            if (name == "Error")
            {
                downloadqueue.RemoveAt(0);
                Running = false;
                Progress.Value = 0;
                checkqueue();
                return;
            }

            txtbox.AppendText("\n\n" + MainWindow.globalLanguage.songs.code.unzippingSong);
            ZipFile.ExtractToDirectory(exe + "\\tmp\\finished\\" + name + ".zip", exe + "\\tmp\\unzipped");
            txtbox.AppendText("\n\n" + MainWindow.globalLanguage.songs.code.unzippedSong);
            
            String[] f = Directory.GetDirectories(exe + "\\tmp\\unzipped");
            if (f.Count() != 0)
            {
                Input = f[0];
            }
            else
            {
                Input = exe + "\\tmp\\unzipped";
            }



            String hash = GetCustomLevelHash(Input);

            txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.songs.code.generatedHash, hash));
            if (Directory.Exists(exe + "\\tmp\\custom_level_" + hash)) Directory.Delete(exe + "\\tmp\\custom_level_" + hash, true);
            Directory.Move(Input, exe + "\\tmp\\custom_level_" + hash);
            if (!adb("push \"" + exe + "\\tmp\\custom_level_" + hash + "\" /sdcard/BMBFData/CustomSongs")) return;
            Directory.Delete(exe + "\\tmp\\custom_level_" + hash, true);

            //Playlist Backup
            BackupPlaylists();

            txtbox.AppendText("\n\n" + MainWindow.globalLanguage.global.syncingToBS);
            txtbox.ScrollToEnd();
            Sync();
            txtbox.AppendText("\n\n" + MainWindow.globalLanguage.global.syncedToBS);
            txtbox.ScrollToEnd();

            reloadsongsfolder();

            RestorePlaylists();

            txtbox.AppendText("\n\n" + MainWindow.globalLanguage.songs.code.installedSong);
            txtbox.ScrollToEnd();
        }

        public void BackupPlaylists()
        {
            try
            {
                Sync();
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.mainMenu.code.playlistBackup, "\\Backup\\Playlists.json"));
                txtbox.ScrollToEnd();
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                String BMBF = "";
                using (TimeoutWebClient client2 = new TimeoutWebClient())
                {
                    BMBF = client2.DownloadString("http://" + MainWindow.IP + ":50000/host/beatsaber/config");
                }
                var json = JSON.Parse(BMBF);
                json["IsCommitted"] = false;
                File.WriteAllText(exe + "\\tmp\\Playlists.json", json["Config"].ToString());
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.mainMenu.code.playlistBackupFinished, "\\Backup\\Playlists.json"));
                txtbox.ScrollToEnd();
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
            }
            catch
            {
                txtbox.AppendText(MainWindow.globalLanguage.global.PL100);

            }
        }

        public void reloadsongsfolder()
        {
            System.Threading.Thread.Sleep(3000);
            TimeoutWebClient client = new TimeoutWebClient();
            client.QueryString.Add("foo", "foo");
            client.UploadValues("http://" + MainWindow.IP + ":50000/host/beatsaber/reloadsongfolders", "POST", client.QueryString);
        }

        public void RestorePlaylists()
        {
            System.Threading.Thread.Sleep(5000);
            try
            {
                TimeoutWebClient client3 = new TimeoutWebClient();

                String Playlists = exe + "\\tmp\\Playlists.json";

                var j = JSON.Parse(client3.DownloadString("http://" + MainWindow.IP + ":50000/host/beatsaber/config"));
                var p = JSON.Parse(File.ReadAllText(Playlists));

                j["Config"]["Playlists"] = p["Playlists"];
                File.WriteAllText(exe + "\\tmp\\FUCKINBMBF.json", j["Config"].ToString());
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                    postChanges(exe + "\\tmp\\FUCKINBMBF.json");
                }));
            }
            catch
            {
                txtbox.AppendText(MainWindow.globalLanguage.global.PL100);
            }
        }

        public void postChanges(String Config)
        {
            System.Threading.Thread.Sleep(5000);
            using (TimeoutWebClient client = new TimeoutWebClient())
            {
                client.QueryString.Add("foo", "foo");
                client.UploadFile("http://" + MainWindow.IP + ":50000/host/beatsaber/config", "PUT", Config);
                client.UploadValues("http://" + MainWindow.IP + ":50000/host/beatsaber/commitconfig", "POST", client.QueryString);
            }
        }

        public static string CreateSha1FromBytes(byte[] input)
        {
            // Use input string to calculate MD5 hash
            using (var sha1 = SHA1.Create())
            {
                var inputBytes = input;
                var hashBytes = sha1.ComputeHash(inputBytes);

                return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
            }
        }

        public static string GetCustomLevelHash(String Path)
        {
            byte[] combinedBytes = new byte[0];
            combinedBytes = combinedBytes.Concat(File.ReadAllBytes(Directory.GetFiles(Path).FirstOrDefault(x => x.ToLower().EndsWith("info.dat")))).ToArray();
            String CustomLevelPath = Path;
            var json = JSON.Parse(File.ReadAllText(Path + "\\info.dat"));

            for (int i = 0; i < json["_difficultyBeatmapSets"].Count; i++)
            {
                for (int i2 = 0; i2 < json["_difficultyBeatmapSets"][i]["_difficultyBeatmaps"].Count; i2++)
                    if (File.Exists(Path + "\\" + json["_difficultyBeatmapSets"][i]["_difficultyBeatmaps"][i2]["_beatmapFilename"]))
                        combinedBytes = combinedBytes.Concat(File.ReadAllBytes(Path + "\\" + json["_difficultyBeatmapSets"][i]["_difficultyBeatmaps"][i2]["_beatmapFilename"])).ToArray();
            }


            String hash = CreateSha1FromBytes(combinedBytes.ToArray());
            return hash.ToLower();
        }

        public void checkqueue()
        {
            if (downloadqueue.Count != 0 && !canceled)
            {
                if(PEO && installed % 20 == 0 && installed != 0)
                {
                    Sync();
                }
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.songs.code.remainingToInstall, downloadqueue.Count.ToString()));
                txtbox.ScrollToEnd();
                InstallSong();
            }
            else
            {
                canceled = false;
                downloadqueue.Clear();
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.global.allFinished);
                txtbox.ScrollToEnd();
                DownloadLable.Text = MainWindow.globalLanguage.global.allFinished;
                if (PEO)
                {
                    PlaylistEditor.waiting = false;
                    this.Close();
                }
                if(OneClick)
                {
                    Process.GetCurrentProcess().Kill();
                }
            }
        }

        public void InstallSong(String Key)
        {
            downloadqueue.Add(new Tuple<string, bool>(Key, false));
            OneClick = true;
            checkqueue();
        }

        public void InstallSongPE(String Key)
        {
            txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.songs.code.songKeyAddedToQueue, Key));
            txtbox.ScrollToEnd();
            downloadqueue.Add(new Tuple<string, bool>(Key, false));
            PEO = true;
            checkqueue();
        }

        public void AddSelectedSongToQueue(object sender, RoutedEventArgs e)
        {
            if (downloadqueue.Contains(new Tuple<string, bool>(SongKey.Text, false)))
            {
                txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.songs.code.songAlreadyInQueue, SongKey.Text));
                txtbox.ScrollToEnd();
                return;
            }
            if (SongKey.Text == MainWindow.globalLanguage.songs.UI.songKeyPlaceholder)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.songs.code.chooseSong);
                txtbox.ScrollToEnd();
                Running = false;
                return;
            }
            txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.songs.code.songKeyAddedToQueue, SongKey.Text));
            downloadqueue.Add(new Tuple<string, bool>(SongKey.Text, false));
            checkqueue();
        }

        private void InstallSong()
        {
            if (!CheckIP())
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.global.ipInvalid);
                txtbox.ScrollToEnd();
                return;
            }
            if (Running)
            {
                return;
            }
            Running = true;
            Key = downloadqueue[0].Item1;
            if(downloadqueue[0].Item2)
            {
                adb("shell am start -n com.weloveoculus.BMBF/com.weloveoculus.BMBF.MainActivity");
                upload(Key, true);
                return;
            }
            BeatSaverAPISong song = interactor.GetBeatSaverAPISong(Key);
            if(!song.GoodRequest)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.songs.code.beatMapCantBeFound, Key));
                txtbox.ScrollToEnd();
                Running = false;
                downloadqueue.RemoveAt(0);
                checkqueue();
                return;
            }
            C = 0;
            while (File.Exists(exe + "\\tmp\\" + Key + C + ".zip"))
            {
                C++;
            }

            txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.songs.code.downloadingBeatMap, Key));
            txtbox.ScrollToEnd();
            WebClient cl = new WebClient();
            cl.Headers.Add("user-agent", "BMBF Manager/1.0");
            Uri keys = new Uri(interactor.BeatSaverLink + song.downloadURL);
            try
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                {
                    DownloadLable.Text = MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.songs.code.downloadingBeatMap, Key);
                    txtbox.ScrollToEnd();
                    cl.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                    cl.DownloadFileCompleted += new AsyncCompletedEventHandler(finished_download);
                    cl.DownloadFileAsync(keys, exe + "\\tmp\\" + Key + C + ".zip");
                }));
            }
            catch
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.songs.code.downloadingBeatMap, Key));
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

        public void finished_download(object sender, AsyncCompletedEventArgs e)
        {
            adb("shell am start -n com.weloveoculus.BMBF/com.weloveoculus.BMBF.MainActivity");
            txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.songs.code.downloadedBeatMap, Key) + "\n");
            txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.songs.code.checkingBeatMap, Key));
            txtbox.ScrollToEnd();
            String name = CheckSongZip(exe + "\\tmp\\" + Key + C + ".zip");
            if (name == "Error")
            {
                downloadqueue.RemoveAt(0);
                Running = false;
                Progress.Value = 0;
                checkqueue();
                return;
            }
            upload(exe + "\\tmp\\finished\\" + name.Trim() + ".zip");
        }

        public void upload(String path, bool uploadfile = false)
        {
            getQuestIP();

            WebClient client = new WebClient();
            Uri uri = new Uri("http://" + MainWindow.IP + ":50000/host/beatsaber/upload?overwrite");
            if(uploadfile) txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.songs.code.uploadingBeatMap, System.IO.Path.GetFileName(downloadqueue[0].Item1)));
            else txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.songs.code.uploadingBeatMap, downloadqueue[0].Item1));
            txtbox.ScrollToEnd();
            try
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                {
                    if (uploadfile) DownloadLable.Text = MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.songs.code.uploadingBeatMap, System.IO.Path.GetFileName(downloadqueue[0].Item1));
                    else DownloadLable.Text = MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.songs.code.uploadingBeatMap, downloadqueue[0].Item1);
                    client.UploadProgressChanged += new UploadProgressChangedEventHandler(client_uploadchanged);
                    client.UploadFileCompleted += (sender, e) => finished_upload(sender, e, uploadfile);
                    client.UploadFileAsync(uri, path);
                }));

            }
            catch
            {
                txtbox.AppendText(MainWindow.globalLanguage.global.BMBF100);
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

        private void finished_upload(object sender, AsyncCompletedEventArgs e, bool uploadfile)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                {
                    if (!PEO) Sync();
                }));
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.songs.code.songWasSynced, downloadqueue[0].Item1));
                txtbox.ScrollToEnd();
            }
            catch
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.songs.code.couldntSync);
                txtbox.ScrollToEnd();
            }
            downloadqueue.RemoveAt(0);
            Running = false;
            Progress.Value = 0;
            checkqueue();
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

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SongList.Items.Count < 1) return;
            int index = SongList.SelectedIndex;

            SongKey.Text = SongKeys[index].ToString().Replace("\"", "");
        }

        public String CheckSongZip(String zip)
        {
            ZipFile.ExtractToDirectory(zip, exe + "\\tmp\\correct");
            return CheckSong(exe + "\\tmp\\correct");
        }


        public String CheckSong(String folder)
        {
            if (!Directory.Exists(exe + "\\tmp\\finished")) Directory.CreateDirectory(exe + "\\tmp\\finished");
            ArrayList found = new ArrayList();
            String entry = folder;
            String dat = entry + "\\Info.dat";

            //get Info.dat
            MoveOutOfFolder(entry, found);
            if (!File.Exists(entry + "\\Info.dat") || !File.Exists(entry + "\\info.dat"))
            {
                if (Directory.GetDirectories(entry).Count() > 0)
                {
                    if (!File.Exists(entry + "\\Info.dat") || !File.Exists(entry + "\\info.dat"))
                    {
                        found.Add(MainWindow.globalLanguage.songs.code.infoMissing);
                        sendfoundings(found, false);
                        return "Error";
                    }
                }
                else
                {
                    found.Add(MainWindow.globalLanguage.songs.code.infoMissing);
                    sendfoundings(found, false);
                    return "Error";
                }
            }

            JSONNode info = JSON.Parse(File.ReadAllText(dat));


            if (!File.Exists(entry + "\\" + info["_songFilename"]))
            {
                Boolean corrected = false;
                foreach (String file in Directory.GetFiles(entry))
                {
                    if (file.EndsWith(".ogg") || file.EndsWith(".egg") || file.EndsWith(".wav") || file.EndsWith(".bmp") || file.EndsWith(".exr") || file.EndsWith(".gif") || file.EndsWith(".hdr") || file.EndsWith(".iff") || file.EndsWith(".pict") || file.EndsWith(".psd") || file.EndsWith(".tga") || file.EndsWith(".tiff"))
                    {
                        info["_songFilename"] = System.IO.Path.GetFileName(file);
                        found.Add(MainWindow.globalLanguage.songs.code.wrongSong);
                        corrected = true;
                        break;
                    }
                    if (info["_songFilename"].ToString().Replace("\"", "").StartsWith(System.IO.Path.GetFileNameWithoutExtension(file)))
                    {
                        info["_songFilename"] = System.IO.Path.GetFileName(file);
                        found.Add(MainWindow.globalLanguage.songs.code.wrongSongExtension);
                        corrected = true;
                        break;
                    }
                }
                if (!corrected)
                {
                    found.Add(MainWindow.globalLanguage.songs.code.noSong);
                    sendfoundings(found, false);
                    return "Error";
                }
            }

            if (!File.Exists(entry + "\\" + info["_coverImageFilename"]))
            {
                Boolean corrected = false;
                foreach (String file in Directory.GetFiles(entry))
                {
                    if (file.EndsWith(".png") || file.EndsWith(".jpg") || file.EndsWith("."))
                    {
                        info["_coverImageFilename"] = System.IO.Path.GetFileName(file);
                        found.Add(MainWindow.globalLanguage.songs.code.wrongCover);
                        corrected = true;
                        break;
                    }
                    if (info["_coverImageFilename"].ToString().Replace("\"", "").StartsWith(System.IO.Path.GetFileNameWithoutExtension(file)))
                    {
                        info["_coverImageFilename"] = System.IO.Path.GetFileName(file);
                        found.Add(MainWindow.globalLanguage.songs.code.wrongCoverExtension);
                        corrected = true;
                        break;
                    }
                }
                if (!corrected)
                {
                    found.Add(MainWindow.globalLanguage.songs.code.noCover);
                    sendfoundings(found, false);
                    return "Error";
                }
            }

            Boolean baddiff = false;
            foreach (JSONNode BeatmapSet in info["_difficultyBeatmapSets"])
            {
                foreach (JSONNode difficulty in BeatmapSet["_difficultyBeatmaps"])
                {
                    if (!File.Exists(entry + "\\" + difficulty["_beatmapFilename"]))
                    {
                        found.Add(MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.songs.code.noDifficulty, difficulty["_difficulty"], BeatmapSet["_beatmapCharacteristicName"]));
                        baddiff = true;
                    }
                }
            }

            if (baddiff)
            {
                sendfoundings(found, false);
                return "Error";
            }

            int index = 0;
            List<String> ka = new List<String>();
            foreach (KeyValuePair<string, JSONNode> c in (JSONObject)info)
            {
                if (c.Value.IsArray)
                {
                    index++;
                    continue;
                }
                if (c.Value.ToString().Replace("\"", "") == "unknown")
                {

                    ka.Add(c.Key);
                    found.Add(MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.songs.code.changedUnknown, c.Key));
                }
                index++;
            }

            foreach (String c in ka)
            {
                info[c] = "k. A.";
            }

            String Name = info["_songName"];
            Name = Name.Replace("/", "");
            Name = Name.Replace(":", "");
            Name = Name.Replace("*", "");
            Name = Name.Replace("?", "");
            Name = Name.Replace("\"", "");
            Name = Name.Replace("<", "");
            Name = Name.Replace(">", "");
            Name = Name.Replace("|", "");
            Name = Name.Replace(@"\", "");
            Name = Name.Trim();

            if (File.Exists(exe + "\\tmp\\finished\\" + Name + ".zip")) File.Delete(exe + "\\tmp\\finished\\" + Name + ".zip");
            ZipFile.CreateFromDirectory(folder, exe + "\\tmp\\finished\\" + Name + ".zip");
            if (found.Count == 0)
            {
                found.Add(MainWindow.globalLanguage.songs.code.allGood);
                sendfoundings(found, true, true);
                return Name;
            }
            File.Delete(dat);
            File.WriteAllText(entry + "\\Info.dat", info.ToString());
            sendfoundings(found, true);
            return Name;
        }

        public void MoveOutOfFolder(String FolderToMoveAll, ArrayList found)
        {
            foreach (String folder in Directory.GetDirectories(FolderToMoveAll))
            {
                if (found.Count == 0)
                {
                    found.Add(MainWindow.globalLanguage.songs.code.correctedFolders);
                }

                MoveOutOfFolder(folder, found);
                foreach (String file in Directory.GetFiles(folder))
                {
                    File.Move(file, FolderToMoveAll + "\\" + System.IO.Path.GetFileName(file));
                }
                Directory.Delete(folder);
            }
        }


        public void sendfoundings(ArrayList found, bool sendZip, bool check = false)
        {
            if(Directory.Exists(exe + "\\tmp\\correct")) Directory.Delete(exe + "\\tmp\\correct", true);
            if(File.Exists(exe + "\\tmp\\correct.zip"))File.Delete(exe + "\\tmp\\correct.zip");
            String tosend = "";
            foreach (String c in found)
            {
                tosend += "\n- " + c;
            }
            if (!sendZip)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.songs.code.noCorrectionPossible, tosend));
            }
            else
            {
                if (check)
                {
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.songs.code.songAllGood);
                }
                else
                {
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.songs.code.corrected, tosend));
                }

            }
            Console.ReadLine();
        }
    }

    public class SongItem
    {
        public string Name { get; set; }

        public string Mapper { get; set; }

        public string Artist { get; set; }
    }
}
