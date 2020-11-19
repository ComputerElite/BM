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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

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

        public Songs()
        {
            InitializeComponent();
            Quest.Text = MainWindow.IP;
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
                uniformBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Songs3.png", UriKind.Absolute));
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

            WebClient c = new WebClient();
            c.Headers.Add("user-agent", "BMBF Manager/1.0");
            Key = SongKey.Text;
            String Details = "";
            try
            {
                Details = c.DownloadString("https://beatsaver.com/api/maps/detail/" + Key);
            }
            catch
            {
                txtbox.AppendText("\n\nThe BeatMap " + Key + " doesn't exist.");
                return;
            }

            var json = JSON.Parse(Details);

            String SongName = json["metadata"]["songName"];
            String SongArtist = json["metadata"]["songAuthorName"];
            String MapAuthor = json["metadata"]["levelAuthorName"];
            String SubName = json["metadata"]["songSubName"];
            String BPM = json["metadata"]["bpm"];

            if (SongName == "") SongName = "N/A";
            if (SongArtist == "") SongArtist = "N/A";
            if (MapAuthor == "") MapAuthor = "N/A";
            if (SubName == "") SubName = "N/A";
            if (BPM == "") BPM = "N/A";

            txtbox.Text = "Metadata of the Song you choose:";

            txtbox.AppendText("\n\nSong Name: " + SongName);
            txtbox.AppendText("\nSong Artist: " + SongArtist);
            txtbox.AppendText("\nMap Author: " + MapAuthor);
            txtbox.AppendText("\nSong Sub Name: " + SubName);
            txtbox.AppendText("\nBPM: " + BPM);
            txtbox.AppendText("\nBeatMap Key: " + Key);
        }

        private void Search(object sender, RoutedEventArgs e)
        {
            SongList.Items.Clear();

            WebClient c = new WebClient();
            c.Headers.Add("user-agent", "BMBF Manager/1.0");
            String Term = "";
            Uri u = new Uri("https://beatsaver.com/api/search/text?q=%22" + SearchTerm.Text + "%22");
            try
            {
                Term = c.DownloadString(u);
            }
            catch
            {
                txtbox.AppendText("BeatSaver error");
                return;
            }

            var result = JSON.Parse(Term);

            foreach (JSONNode doc in result["docs"])
            {
                String Name = doc["metadata"]["songName"];
                String Mapper = doc["metadata"]["levelAuthorName"];
                String Artist = doc["metadata"]["songAuthorName"];

                SongList.Items.Add(new SongItem { Name = Name, Mapper = Mapper, Artist = Artist });
                SongKeys.Add(doc["key"]);
            }

            if(SongKeys.Count < 1)
            {
                txtbox.AppendText("\n\nNo results found. Try another Search Term");
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

        public static Boolean adb(String Argument)
        {
            String User = System.Environment.GetEnvironmentVariable("USERPROFILE");
            ProcessStartInfo s = new ProcessStartInfo();
            s.CreateNoWindow = false;
            s.UseShellExecute = false;
            s.FileName = "adb.exe";
            s.WindowStyle = ProcessWindowStyle.Minimized;
            s.Arguments = Argument;
            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(s))
                {
                    exeProcess.WaitForExit();
                    return true;
                }
            }
            catch
            {

                ProcessStartInfo se = new ProcessStartInfo();
                se.CreateNoWindow = false;
                se.UseShellExecute = false;
                se.FileName = User + "\\AppData\\Roaming\\SideQuest\\platform-tools\\adb.exe";
                se.WindowStyle = ProcessWindowStyle.Minimized;
                se.Arguments = Argument;
                try
                {
                    // Start the process with the info we specified.
                    // Call WaitForExit and then the using statement will close.
                    using (Process exeProcess = Process.Start(se))
                    {
                        exeProcess.WaitForExit();
                        return true;
                    }
                }
                catch
                {
                    // Log error.
                    return false;
                }
            }
        }

        private void InstallZip(object sender, RoutedEventArgs e)
        {
            if (!CheckIP())
            {
                txtbox.AppendText("\n\nChoose a valid IP.");
                return;
            }
            if (Running)
            {
                txtbox.AppendText("\n\nA Song Install is already running.");
                return;
            }
            Running = true;

            String Input = "";
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Zip Files (*.zip)|*.zip";
            bool? result = ofd.ShowDialog();
            if (result == true)
            {
                //Get the path of specified file
                if (File.Exists(ofd.FileName))
                {
                    Input = ofd.FileName;
                }
                else
                {
                    MessageBox.Show("Please select a valid Zip File", "BMBF Manager - Zip Song Installing", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Running = false;
                    return;
                }

            }

            FileInfo fi = new FileInfo(Input);
            long ZipSize = fi.Length;
            if(ZipSize < 35000000) //35 MB
            {
                upload(Input);
                Running = false;
                return;
            }

            MessageBoxResult result1 = MessageBox.Show("This Song is over 35MB. I will install it manually is your Quest connected?", "BMBF Manager - Zip Song installing", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            switch (result1)
            {
                case MessageBoxResult.No:
                    txtbox.AppendText("\n\nSong Installing Aborted");
                    txtbox.ScrollToEnd();
                    Running = false;
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
            Sync();
            txtbox.AppendText("\n\nsynced Song to Beat Saber");

            reloadsongsfolder();

            //RestorePlaylists();

            //txtbox.AppendText("\n\nInstalled Song.");
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
                File.WriteAllText(exe + "\\tmp\\Config.json", json.ToString());

                String Config = exe + "\\tmp\\config.json";

                var j = JSON.Parse(File.ReadAllText(Config));
                File.WriteAllText(exe + "\\tmp\\Playlists.json", j["Config"].ToString());
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
                using (TimeoutWebClient client3 = new TimeoutWebClient())
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                        client3.DownloadFile("http://" + MainWindow.IP + ":50000/host/beatsaber/config", exe + "\\tmp\\OConfig.json");
                    }));

                }

                String Config = exe + "\\tmp\\OConfig.json";

                String Playlists = exe + "\\tmp\\Playlists.json";

                var j = JSON.Parse(File.ReadAllText(Config));
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

        private void InstallSong(object sender, RoutedEventArgs e)
        {
            if (!CheckIP())
            {
                txtbox.AppendText("\n\nChoose a valid IP.");
                return;
            }
            if (Running)
            {
                txtbox.AppendText("\n\nA Song Install is already running.");
                return;
            }
            Running = true;
            Key = SongKey.Text;
            WebClient c = new WebClient();
            c.Headers.Add("user-agent", "BMBF Manager/1.0");
            if(SongKey.Text == "Song Key")
            {
                txtbox.AppendText("\n\nPlease Choose a Song.");
                Running = false;
                return;
            }
            try
            {
                c.OpenRead("https://beatsaver.com/api/download/key/" + Key);
            }
            catch
            {
                txtbox.AppendText("\n\nThe BeatMap " + Key + " doesn't exist.");
                Running = false;
                return;
            }
            C = 0;
            while (File.Exists(exe + "\\tmp\\Song" + C + ".zip"))
            {
                C++;
            }

            txtbox.AppendText("\nDownloading BeatMap " + Key);
            WebClient cl = new WebClient();
            cl.Headers.Add("user-agent", "BMBF Manager/1.0");
            Uri keys = new Uri("https://beatsaver.com/api/download/key/" + Key);
            try
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                {
                    cl.DownloadFileCompleted += new AsyncCompletedEventHandler(finished_download);
                    cl.DownloadFileAsync(keys, exe + "\\tmp\\Song" + C + ".zip");
                }));
            }
            catch
            {
                txtbox.AppendText("\n\nAn Error Occured");
                Running = false;
                return;
            }
        }

        public void finished_download(object sender, AsyncCompletedEventArgs e)
        {
            txtbox.AppendText("\nDownloaded BeatMap " + Key + "\n");
            upload(exe + "\\tmp\\Song" + C + ".zip");
            Running = false;
        }

        public void upload(String path)
        {
            getQuestIP();

            WebClient client = new WebClient();
            Uri uri = new Uri("http://" + MainWindow.IP + ":50000/host/beatsaber/upload?overwrite");
            txtbox.AppendText("\n\nUploading BeatMap " + SongKey.Text + " to BMBF");
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
            }
        }

        private void finished_upload(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                {
                    Sync();
                }));
                txtbox.AppendText("\n\nSong " + SongKey.Text + " was synced to your Quest.");
            }
            catch
            {
                txtbox.AppendText("\n\nCouldn't sync with BeatSaber. Needs to be done manually.");
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

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SongKeys.Count < 1) return;
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
