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

        public Songs()
        {
            InitializeComponent();
            Quest.Text = MainWindow.IP;
            DownloadLable.Text = "All finished";
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

        private void ClearKey(object sender, RoutedEventArgs e)
        {
            if (SongKey.Text == "Song Key")
            {
                SongKey.Text = "";
            }

        }

        private void ClearSearch(object sender, RoutedEventArgs e)
        {
            if (SearchTerm.Text == "Search Term")
            {
                SearchTerm.Text = "";
            }

        }

        private void QuestIPCheck(object sender, RoutedEventArgs e)
        {
            if (Quest.Text == "")
            {
                Quest.Text = "Quest IP";
            }
        }

        private void SearchTermCheck(object sender, RoutedEventArgs e)
        {
            if (SearchTerm.Text == "")
            {
                SearchTerm.Text = "Search Term";
            }
        }

        private void SongKeyCheck(object sender, RoutedEventArgs e)
        {
            if (SongKey.Text == "")
            {
                SongKey.Text = "Song Key";
            }
        }


        private void LoadSongData(object sender, RoutedEventArgs e)
        {
            if (SongKey.Text == "Song Key")
            {
                txtbox.AppendText("\n\nPlease choose a Song Key. This can be found on BeatSaver.");
                return;
            }

            Key = SongKey.Text;

            BeatSaberSong song = interactor.GetBeatSaberSong(Key);
            if(!song.RequestGood)
            {
                txtbox.AppendText("\n\nThe BeatMap " + Key + " doesn't exist.");
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

            txtbox.AppendText("\n\nMetadata of the Song you choose:");

            txtbox.AppendText("\n\nSong Name: " + SongName);
            txtbox.AppendText("\nSong Artist: " + SongArtist);
            txtbox.AppendText("\nMap Author: " + MapAuthor);
            txtbox.AppendText("\nSong Sub Name: " + SubName);
            txtbox.AppendText("\nBPM: " + BPM);
            txtbox.AppendText("\nBeatMap Key: " + Key);
            txtbox.ScrollToEnd();
        }

        private void Search(object sender, RoutedEventArgs e)
        {
            SongList.Items.Clear();

            BeatSaverAPISearchResult result = interactor.SearchText(SearchTerm.Text);
            if(!result.RequestGood)
            {
                txtbox.AppendText("BeatSaver error");
                txtbox.ScrollToEnd();
                return;
            }

            foreach (BeatSaverAPISong doc in result.docs)
            {
                String Name = doc.name;
                String Mapper = doc.metadata.levelAuthorName;
                String Artist = doc.metadata.songAuthorName;

                SongList.Items.Add(new SongItem { Name = Name, Mapper = Mapper, Artist = Artist });
                SongKeys.Add(doc.key);
            }

            if(SongKeys.Count < 1)
            {
                txtbox.AppendText("\n\nNo results found. Try another Search Term");
                txtbox.ScrollToEnd();
            }

            SongList.SelectedIndex = 0;
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

        private void InstallZip(object sender, RoutedEventArgs e)
        {
            if (!CheckIP())
            {
                txtbox.AppendText("\n\nChoose a valid IP.");
                txtbox.ScrollToEnd();
                return;
            }

            String Input = "";
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Zip Files (*.zip)|*.zip";
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
                    MessageBox.Show("Please select a valid Zip File", "BMBF Manager - Zip Song Installing", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

            }
            FileInfo fi = new FileInfo(Input);
            long ZipSize = fi.Length;
            if(ZipSize < 50000000) //35 MB
            {
                if (downloadqueue.Contains(new Tuple<string, bool>(Input, true)))
                {
                    txtbox.AppendText("\nThe Song " + System.IO.Path.GetFileName(Input) + " is already in the queue");
                    return;
                }
                downloadqueue.Add(new Tuple<string, bool>(Input, true));
                txtbox.AppendText("\n\n" + System.IO.Path.GetFileName(Input) + " has been added to the queue");
                checkqueue();
                return;
            }

            MessageBoxResult result1 = MessageBox.Show("This Song is over 50MB. A experimental method to install sogns will be used. Is your Quest connected?", "BMBF Manager - Zip Song installing", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            switch (result1)
            {
                case MessageBoxResult.No:
                    txtbox.AppendText("\n\nSong Installing Aborted");
                    txtbox.ScrollToEnd();
                    return;
            }

            if (Directory.Exists(exe + "\\tmp\\unzipped")) Directory.Delete(exe + "\\tmp\\unzipped", true);

            txtbox.AppendText("\n\nunzipping Song");
            txtbox.AppendText("\n\nunzipped Song");
            ZipFile.ExtractToDirectory(Input, exe + "\\tmp\\unzipped");

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

            txtbox.AppendText("\n\nGenerated hash: " + hash);
            if (Directory.Exists(exe + "\\tmp\\custom_level_" + hash)) Directory.Delete(exe + "\\tmp\\custom_level_" + hash, true);
            Directory.Move(Input, exe + "\\tmp\\custom_level_" + hash);
            if (!adb("push \"" + exe + "\\tmp\\custom_level_" + hash + "\" /sdcard/BMBFData/CustomSongs")) return;
            Directory.Delete(exe + "\\tmp\\custom_level_" + hash, true);

            //Playlist Backup
            BackupPlaylists();

            txtbox.AppendText("\n\nsyncing Song to Beat Saber");
            txtbox.ScrollToEnd();
            Sync();
            txtbox.AppendText("\n\nsynced Song to Beat Saber");
            txtbox.ScrollToEnd();

            reloadsongsfolder();

            RestorePlaylists();

            txtbox.AppendText("\n\nInstalled Song.");
            txtbox.ScrollToEnd();
        }

        public void BackupPlaylists()
        {
            try
            {
                Sync();
                txtbox.AppendText("\n\nBacking up Playlist to " + exe + "\\Backup\\Playlists.json");
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
                txtbox.AppendText("\n\nBacked up Playlists to " + exe + "\\tmp\\Playlists.json");
                txtbox.ScrollToEnd();
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
            }
            catch
            {
                txtbox.AppendText("\n\n\nAn error occured (Code: PL100). Check following:");
                txtbox.AppendText("\n\n- You put in the Quests IP right.");
                txtbox.AppendText("\n\n- You've choosen a Backup Name.");
                txtbox.AppendText("\n\n- Your Quest is on.");

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
                txtbox.AppendText("\n\n\nAn error occured (Code: PL100). Check following:");
                txtbox.AppendText("\n\n- You put in the Quests IP right.");
                txtbox.AppendText("\n\n- Your Quest is on.");
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
            combinedBytes = combinedBytes.Concat(File.ReadAllBytes(Path + "\\info.dat")).ToArray();
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
            if (downloadqueue.Count != 0)
            {
                InstallSong();
            }
            else
            {
                txtbox.AppendText("\n\nAll finished.");
                txtbox.ScrollToEnd();
                DownloadLable.Text = "All finished";
            }
        }

        public void InstallSong(String Key)
        {
            downloadqueue.Add(new Tuple<string, bool>(Key, false));
            checkqueue();
        }

        public void AddSelectedSongToQueue(object sender, RoutedEventArgs e)
        {
            if (downloadqueue.Contains(new Tuple<string, bool>(SongKey.Text, false)))
            {
                txtbox.AppendText("\nThe Song " + SongKey.Text + " is already in the download queue");
                txtbox.ScrollToEnd();
                return;
            }
            if (SongKey.Text == "Song Key")
            {
                txtbox.AppendText("\n\nPlease Choose a Song.");
                txtbox.ScrollToEnd();
                Running = false;
                return;
            }
            txtbox.AppendText("\n\nThe Song " + SongKey.Text + " was added to the queue");
            downloadqueue.Add(new Tuple<string, bool>(SongKey.Text, false));
            checkqueue();
        }

        private void InstallSong()
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
                txtbox.AppendText("\n\nThe BeatMap " + Key + " doesn't exist.");
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

            txtbox.AppendText("\nDownloading BeatMap " + Key);
            txtbox.ScrollToEnd();
            WebClient cl = new WebClient();
            cl.Headers.Add("user-agent", "BMBF Manager/1.0");
            Uri keys = new Uri(interactor.BeatSaverLink + song.downloadURL);
            try
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                {
                    DownloadLable.Text = "Downloading BeatMap " + Key;
                    txtbox.ScrollToEnd();
                    cl.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                    cl.DownloadFileCompleted += new AsyncCompletedEventHandler(finished_download);
                    cl.DownloadFileAsync(keys, exe + "\\tmp\\" + Key + C + ".zip");
                }));
            }
            catch
            {
                txtbox.AppendText("\n\nIf you see this something in the code messed up or ComputerElite is dumb");
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
            txtbox.AppendText("\nDownloaded BeatMap " + Key + "\n");
            txtbox.ScrollToEnd();
            upload(exe + "\\tmp\\" + Key + C + ".zip");
        }

        public void upload(String path, bool uploadfile = false)
        {
            getQuestIP();

            WebClient client = new WebClient();
            Uri uri = new Uri("http://" + MainWindow.IP + ":50000/host/beatsaber/upload?overwrite");
            if(uploadfile) txtbox.AppendText("\n\nUploading BeatMap " + System.IO.Path.GetFileName(downloadqueue[0].Item1) + " to BMBF");
            else txtbox.AppendText("\n\nUploading BeatMap " + downloadqueue[0].Item1 + " to BMBF");
            txtbox.ScrollToEnd();
            try
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                {
                    if (uploadfile) DownloadLable.Text = "Uploading " + System.IO.Path.GetFileName(downloadqueue[0].Item1) + " to BMBF";
                    else DownloadLable.Text = "Uploading BeatMap " + downloadqueue[0].Item1 + " to BMBF";
                    client.UploadProgressChanged += new UploadProgressChangedEventHandler(client_uploadchanged);
                    client.UploadFileCompleted += (sender, e) => finished_upload(sender, e, uploadfile);
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

        private void finished_upload(object sender, AsyncCompletedEventArgs e, bool uploadfile)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                {
                    Sync();
                }));
                txtbox.AppendText("\n\nSong " + downloadqueue[0].Item1 + " was synced to your Quest.");
                txtbox.ScrollToEnd();
            }
            catch
            {
                txtbox.AppendText("\n\nCouldn't sync with BeatSaber. Needs to be done manually.");
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
    }

    public class SongItem
    {
        public string Name { get; set; }

        public string Mapper { get; set; }

        public string Artist { get; set; }
    }
}
