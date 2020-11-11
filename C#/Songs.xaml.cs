using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

            for(int i = 0; result["docs"][i]["metadata"]["songName"]; i++)
            {
                String Name = result["docs"][i]["metadata"]["songName"];
                String Mapper = result["docs"][i]["metadata"]["levelAuthorName"];
                String Artist = result["docs"][i]["metadata"]["songAuthorName"];

                SongList.Items.Add(new SongItem { Name = Name, Mapper = Mapper, Artist = Artist });
                SongKeys.Add(result["docs"][i]["key"]);
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
