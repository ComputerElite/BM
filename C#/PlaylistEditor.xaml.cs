using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
using BMBF.Config;
using System.Text.Json;
using BeatSaverAPI;
using System.IO;
using System.Net;
using SimpleJSON;
using Microsoft.Win32;

namespace BMBF_Manager
{
    /// <summary>
    /// Interaktionslogik für PlaylistEditor.xaml
    /// </summary>
    public partial class PlaylistEditor : Window
    {
        Boolean draggable = true;
        String exe = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 1);

        BMBFC BMBFConfig = new BMBFC();
        List<BMBFSong> UnsortedPlaylist = new List<BMBFSong>();
        BeatSaverAPIInteractor interactor = new BeatSaverAPIInteractor();
        BSKFile known = new BSKFile();
        bool loaded = false;

        String bsk = "https://raw.githubusercontent.com/ComputerElite/resources/master/assets/beatsaber-knowns.json";

        public static bool waiting = false;

        public PlaylistEditor()
        {
            InitializeComponent();
            Quest.Text = MainWindow.IP;

            if (!Directory.Exists(exe + "\\BPLists"))
            {
                Directory.CreateDirectory(exe + "\\BPLists");
            }

            LoadPlaylists(true);

            if(!MainWindow.PEWarningShown)
            {
                MessageBox.Show("Some Notes for the Playlist Editor: Currently it is not possible to sort songs (the sorting is only for you). If you experience any issues hit me up on Discord.", "BMBF Manager - Playlist Editor", MessageBoxButton.OK, MessageBoxImage.Information);
                MainWindow.PEWarningShown = true;
            }

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
                uniformBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/PlaylistEditor.png", UriKind.Absolute));
                uniformBrush.Stretch = Stretch.UniformToFill;
                this.Background = uniformBrush;
            }

            WebClient client = new WebClient();
            try
            {
                String t = client.DownloadString(bsk);
                known = JsonSerializer.Deserialize<BSKFile>(t);
                loaded = true;
            }
            catch
            {
                txtbox.AppendText("A error Occured");
            }
        }

        private void getPlaylists(object sender, RoutedEventArgs e)
        {
            LoadPlaylists();
        }

        private void LoadPlaylists(bool onstart = false)
        {
            if(!CheckIP())
            {
                txtbox.AppendText("Please type in a valid IP.");
                txtbox.ScrollToEnd();
                return;
            }
            TimeoutWebClientShort client = new TimeoutWebClientShort();

            try
            {
                BMBFConfig = JsonSerializer.Deserialize<BMBFC>(client.DownloadString("http://" + MainWindow.IP + ":50000/host/beatsaber/config"));

            } catch { 
                if(onstart)
                {
                    txtbox.AppendText("\n\nPlease load your Playlists.");
                    txtbox.ScrollToEnd();
                } else
                {
                    txtbox.AppendText(MainWindow.BMBF100);
                }
                return;
            }

            Playlists.Items.Clear();
            UnsortedPlaylist.Clear();
            UnsortedSongsPlaylist.Items.Clear();
            foreach(BMBFPlaylist Playlist in BMBFConfig.Config.Playlists)
            {
                Playlists.Items.Add(Playlist.PlaylistName);
            }
            if(Playlists.Items.Count < 1)
            {
                txtbox.AppendText("\n\nSomething went wrong.");
                txtbox.ScrollToEnd();
                return;
            }
            Playlists.SelectedIndex = 0;
        }

        private void ChangeCurrentPlaylist(object sender, SelectionChangedEventArgs e)
        {
            reloadPLs(true);
        }

        private void reloadPLs(bool changed = false)
        {
            if (Playlists.SelectedIndex < 0 || Playlists.SelectedIndex >= BMBFConfig.Config.Playlists.Count)
            {
                //txtbox.AppendText("\n\nPlease select a valid Playlist");
                txtbox.ScrollToEnd();
                return;
            }

            PlaylistSongList.Items.Clear();
            foreach (BMBFSong song in BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList)
            {
                BMBFSong s = new BMBFSong();
                s.SongName = song.SongName == "" ? "N/A" : song.SongName;
                s.SongSubName = song.SongSubName == "" ? "N/A" : song.SongSubName;
                s.LevelAuthorName = song.LevelAuthorName == "" ? "N/A" : song.LevelAuthorName;
                s.SongAuthorName = song.SongAuthorName == "" ? "N/A" : song.SongAuthorName;

                PlaylistSongList.Items.Add(s);
            }
            
            if (BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList.Count < 1)
            {
                if(changed) txtbox.AppendText("\n\nThe Playlists " + BMBFConfig.Config.Playlists[Playlists.SelectedIndex].PlaylistName + " doesn't contain any Songs.");
            }
            else
            {
                PlaylistSongList.SelectedIndex = 0;
            }

            UnsortedSongsPlaylist.Items.Clear();
            foreach (BMBFSong song in UnsortedPlaylist)
            {
                BMBFSong s = new BMBFSong();
                s.SongName = song.SongName == "" ? "N/A" : song.SongName;
                s.SongSubName = song.SongSubName == "" ? "N/A" : song.SongSubName;
                s.LevelAuthorName = song.LevelAuthorName == "" ? "N/A" : song.LevelAuthorName;
                s.SongAuthorName = song.SongAuthorName == "" ? "N/A" : song.SongAuthorName;
                UnsortedSongsPlaylist.Items.Add(s);
            }

            if (!(UnsortedPlaylist.Count < 1))
            {
                UnsortedSongsPlaylist.SelectedIndex = 0;
            }

            BitmapImage i = new BitmapImage();
            i.BeginInit();

            if (BMBFConfig.Config.Playlists[Playlists.SelectedIndex].CoverImageBytes == null)
            {
                i.UriSource = new Uri("http://" + MainWindow.IP + ":50000/host/beatsaber/playlist/cover?PlaylistID=" + BMBFConfig.Config.Playlists[Playlists.SelectedIndex].PlaylistID, UriKind.Absolute);
            } else
            {
                Console.WriteLine(BMBFConfig.Config.Playlists[Playlists.SelectedIndex].CoverImageBytes.Replace("data:image/png;base64,", "").Replace("data:image/jpg;base64,", "").Replace("data:image/jpeg;base64,", ""));
                byte[] binaryData = Convert.FromBase64String(BMBFConfig.Config.Playlists[Playlists.SelectedIndex].CoverImageBytes.Replace("data:image/png;base64,", "").Replace("data:image/jpg;base64,", "").Replace("data:image/jpeg;base64,", "").Replace(",", ""));
                i.StreamSource = new MemoryStream(binaryData);
            }

            i.EndInit();
            PlaylistCoverImage.Source = i;

            PlaylistSongCount.Text = PlaylistSongList.Items.Count + " Song(s)";
            UnsortedSongcount.Text = UnsortedSongsPlaylist.Items.Count + " Song(s)";
            txtbox.ScrollToEnd();

            int total = 0;
            foreach(BMBFPlaylist playlist in BMBFConfig.Config.Playlists)
            {
                total += playlist.SongList.Count;
            }
            TotalSongs.Text = total + " Song(s)";
        }

        private void ChangeCurrentSong(object sender, SelectionChangedEventArgs e)
        {
            if (PlaylistSongList.SelectedIndex < 0 || PlaylistSongList.SelectedIndex >= BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList.Count)
            {
                return;
            }
            Name.Text = BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList[PlaylistSongList.SelectedIndex].SongName;
            SubName.Text = BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList[PlaylistSongList.SelectedIndex].SongSubName;
            Author.Text = BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList[PlaylistSongList.SelectedIndex].SongAuthorName;
            Mapper.Text = BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList[PlaylistSongList.SelectedIndex].LevelAuthorName;
            ID.Text = BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList[PlaylistSongList.SelectedIndex].SongID;
            BitmapImage i = new BitmapImage();
            i.BeginInit();
            i.UriSource = new Uri("http://" + MainWindow.IP + ":50000/host/beatsaber/song/cover?SongID=" + BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList[PlaylistSongList.SelectedIndex].SongID, UriKind.Absolute);
            i.EndInit();
            SongCoverImage.Source = i;
            txtbox.ScrollToEnd();
        }

        private bool IsOST(BMBFSong song)
        {
            if (!loaded)
            {
                WebClient client = new WebClient();
                try
                {
                    String t = client.DownloadString(bsk);
                    known = JsonSerializer.Deserialize<BSKFile>(t);
                    loaded = true;
                }
                catch
                {
                    txtbox.AppendText("A error Occured");
                    txtbox.ScrollToEnd();
                }
            }

            if (known.knownLevelIds.Contains(song.SongID)) return true;
            return false;
        } 

        private void DelSong(object sender, RoutedEventArgs e)
        {
            if (PlaylistSongList.SelectedIndex < 0 || PlaylistSongList.SelectedIndex >= BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList.Count)
            {
                txtbox.AppendText("\n\nYou must have a Song selected");
                txtbox.ScrollToEnd();
                return;
            }
            
            if(IsOST(BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList[PlaylistSongList.SelectedIndex]))
            {
                txtbox.AppendText("\n\nI'll not allow you to delete any OST Song.");
                txtbox.ScrollToEnd();
                return;
            }

            MessageBoxResult r = MessageBox.Show("Are you sure you want to delete " + BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList[PlaylistSongList.SelectedIndex].SongName + "? This can NOT be undone after saving!", "BMBF Manager - Playlist Editor", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            switch(r)
            {
                case MessageBoxResult.No:
                    txtbox.AppendText("\n\nDeleting of " + BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList[PlaylistSongList.SelectedIndex].SongName + " aborted.");
                    txtbox.ScrollToEnd();
                    return;
            }
            BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList.RemoveAt(PlaylistSongList.SelectedIndex);

            reloadPLs();
        }

        private void DelPl(object sender, RoutedEventArgs e)
        {
            if (Playlists.SelectedIndex < 0 || Playlists.SelectedIndex >= BMBFConfig.Config.Playlists.Count)
            {
                txtbox.AppendText("\n\nYou must have a Playlist selected");
                txtbox.ScrollToEnd();
                return;
            }

            if(!loaded)
            {
                WebClient client = new WebClient();
                try
                {
                    String t = client.DownloadString(bsk);
                    known = JsonSerializer.Deserialize<BSKFile>(t);
                    loaded = true;
                }
                catch
                {
                    txtbox.AppendText("\n\nA error Occured");
                    txtbox.ScrollToEnd();
                }
            }

            if(BMBFConfig.Config.Playlists[Playlists.SelectedIndex].PlaylistID == "CustomSongs")
            {
                txtbox.AppendText("\n\nI'll not allow you to delete the CustomSongs Playlist.");
                txtbox.ScrollToEnd();
                return;
            }
            if(known.knownLevelPackIds.Contains(BMBFConfig.Config.Playlists[Playlists.SelectedIndex].PlaylistID))
            {
                txtbox.AppendText("\n\nI'll not allow you to delete any OST Playlist (" + BMBFConfig.Config.Playlists[Playlists.SelectedIndex].PlaylistName + ") for your own safety.");
                txtbox.ScrollToEnd();
                return;
            }

            MessageBoxResult r = MessageBox.Show("Are you sure you want to delete the Playlist " + BMBFConfig.Config.Playlists[Playlists.SelectedIndex].PlaylistName + "? This can NOT be undone after saving!", "BMBF Manager - Playlist Editor", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            switch (r)
            {
                case MessageBoxResult.No:
                    txtbox.AppendText("\n\nDeleting of " + BMBFConfig.Config.Playlists[Playlists.SelectedIndex].PlaylistName + " aborted.");
                    txtbox.ScrollToEnd();
                    return;
            }

            BMBFConfig.Config.Playlists.RemoveAt(Playlists.SelectedIndex);

            Playlists.Items.Clear();
            foreach (BMBFPlaylist Playlist in BMBFConfig.Config.Playlists)
            {
                Playlists.Items.Add(Playlist.PlaylistName);
            }
            if (Playlists.Items.Count < 1)
            {
                txtbox.AppendText("\n\nSomething went wrong.");
                txtbox.ScrollToEnd();
                return;
            }
            Playlists.SelectedIndex = 0;
        }

        /*
         * Code might come in handy some time in future
         * 
        private void PLUp(object sender, RoutedEventArgs e)
        {
            if (Playlists.SelectedIndex < 0 || Playlists.SelectedIndex >= BMBFConfig.Config.Playlists.Count)
            {
                txtbox.AppendText("\n\nYou must have a valid Playlist selected");
                txtbox.ScrollToEnd();
                return;
            }
            if(Playlists.SelectedIndex == 0)
            {
                txtbox.AppendText("\n\nYou can't move the top playlist up.");
                return;
            }
            BMBFConfig.Config.Playlists.Insert(Playlists.SelectedIndex - 1, BMBFConfig.Config.Playlists[Playlists.SelectedIndex]);
            BMBFConfig.Config.Playlists.RemoveAt(Playlists.SelectedIndex + 1);

            Playlists.Items.Clear();
            foreach (BMBFPlaylist Playlist in BMBFConfig.Config.Playlists)
            {
                Playlists.Items.Add(Playlist.PlaylistName);
            }
            if (Playlists.Items.Count < 1)
            {
                txtbox.AppendText("\n\nSomething went wrong.");
                txtbox.ScrollToEnd();
                return;
            }
            Playlists.SelectedIndex = 0;
        }

        private void PLDown(object sender, RoutedEventArgs e)
        {
            if (Playlists.SelectedIndex < 0 || Playlists.SelectedIndex >= BMBFConfig.Config.Playlists.Count)
            {
                txtbox.AppendText("\n\nYou must have a valid Playlist selected");
                txtbox.ScrollToEnd();
                return;
            }
            if (Playlists.SelectedIndex == BMBFConfig.Config.Playlists.Count - 1)
            {
                txtbox.AppendText("\n\nYou can't move the bottom playlist down.");
                return;
            }
            BMBFConfig.Config.Playlists.Insert(Playlists.SelectedIndex + 2, BMBFConfig.Config.Playlists[Playlists.SelectedIndex]);
            BMBFConfig.Config.Playlists.RemoveAt(Playlists.SelectedIndex);

            Playlists.Items.Clear();
            foreach (BMBFPlaylist Playlist in BMBFConfig.Config.Playlists)
            {
                Playlists.Items.Add(Playlist.PlaylistName);
            }
            if (Playlists.Items.Count < 1)
            {
                txtbox.AppendText("\n\nSomething went wrong.");
                txtbox.ScrollToEnd();
                return;
            }
            Playlists.SelectedIndex = 0;
        }
        */

        private void MoveSongRight(object sender, RoutedEventArgs e)
        {
            if (PlaylistSongList.SelectedIndex < 0 || PlaylistSongList.SelectedIndex >= BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList.Count)
            {
                txtbox.AppendText("\n\nYou must have a Song selected");
                txtbox.ScrollToEnd();
                return;
            }

            if (IsOST(BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList[PlaylistSongList.SelectedIndex]))
            {
                txtbox.AppendText("\n\nI'll not allow you to move any OST Song (" + BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList[PlaylistSongList.SelectedIndex].SongName + ") to prevent issues.");
                txtbox.ScrollToEnd();
                return;
            }

            if (UnsortedSongsPlaylist.SelectedIndex < 0 || UnsortedSongsPlaylist.SelectedIndex >= UnsortedPlaylist.Count)
            {
                UnsortedPlaylist.Add(BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList[PlaylistSongList.SelectedIndex]);
            }
            else
            {
                UnsortedPlaylist.Insert(UnsortedSongsPlaylist.SelectedIndex + 1, BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList[PlaylistSongList.SelectedIndex]);
            }
            BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList.RemoveAt(PlaylistSongList.SelectedIndex);

            reloadPLs();
        }

        private void MovePlaylistRight(object sender, RoutedEventArgs e)
        {
            if (Playlists.SelectedIndex < 0 || Playlists.SelectedIndex >= BMBFConfig.Config.Playlists.Count)
            {
                txtbox.AppendText("\n\nYou must have a Playlist selected");
                txtbox.ScrollToEnd();
                return;
            }
            List<BMBFSong> tmp = new List<BMBFSong>(BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList);
            foreach (BMBFSong s in tmp)
            {
                if (IsOST(s))
                {
                    txtbox.AppendText("\n\nI'll not allow you to move any OST Song (" + s.SongName + ") to prevent issues.");
                    txtbox.ScrollToEnd();
                    continue;
                }

                if (UnsortedSongsPlaylist.SelectedIndex < 0 || UnsortedSongsPlaylist.SelectedIndex >= UnsortedPlaylist.Count)
                {
                    UnsortedPlaylist.Add(s);
                }
                else
                {
                    UnsortedPlaylist.Insert(UnsortedSongsPlaylist.SelectedIndex + 1, s);
                }
                BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList.RemoveAt(0);
            }

            reloadPLs();
        }

        private void MoveSongLeft(object sender, RoutedEventArgs e)
        {
            if (UnsortedSongsPlaylist.SelectedIndex < 0 || UnsortedSongsPlaylist.SelectedIndex >= UnsortedPlaylist.Count)
            {
                txtbox.AppendText("\n\nYou must have a Song selected");
                txtbox.ScrollToEnd();
                return;
            }
            if (PlaylistSongList.SelectedIndex < 0 || PlaylistSongList.SelectedIndex >= BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList.Count)
            {
                BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList.Add(UnsortedPlaylist[UnsortedSongsPlaylist.SelectedIndex]);
            }
            else
            {
                BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList.Insert(PlaylistSongList.SelectedIndex + 1, UnsortedPlaylist[UnsortedSongsPlaylist.SelectedIndex]);
            }
            UnsortedPlaylist.RemoveAt(UnsortedSongsPlaylist.SelectedIndex);

            reloadPLs();
        }

        private void MovePlaylistLeft(object sender, RoutedEventArgs e)
        {
            if (Playlists.SelectedIndex < 0 || Playlists.SelectedIndex >= BMBFConfig.Config.Playlists.Count)
            {
                txtbox.AppendText("\n\nYou must have a Playlist selected");
                txtbox.ScrollToEnd();
                return;
            }
            List<BMBFSong> tmp = new List<BMBFSong>(UnsortedPlaylist);
            foreach (BMBFSong s in tmp)
            {
                if (PlaylistSongList.SelectedIndex < 0 || PlaylistSongList.SelectedIndex >= BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList.Count)
                {
                    BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList.Add(s);
                } else
                {
                    BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList.Insert(PlaylistSongList.SelectedIndex + 1, s);
                }
                UnsortedPlaylist.RemoveAt(0);
            }

            reloadPLs();
        }

        private void BeastSShow(object sender, RoutedEventArgs e)
        {
            if (PlaylistSongList.SelectedIndex < 0 || PlaylistSongList.SelectedIndex >= BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList.Count)
            {
                txtbox.AppendText("\n\nYou must have a valid Song selected");
                txtbox.ScrollToEnd();
                return;
            }
            BeatSaverAPISong s = interactor.GetBeatSaverAPISongViaHash(BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList[PlaylistSongList.SelectedIndex].SongID.Replace("custom_level_", "").Replace("customlevel_", ""));
            if(!s.GoodRequest)
            {
                txtbox.AppendText("\n\nI couldn't look up " + BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList[PlaylistSongList.SelectedIndex].SongName + " on BeatSaver");
                txtbox.ScrollToEnd();
                return;
            }
            Process.Start("https://bsaber.com/songs/" + s.key);
        }

        private void BeatSShow(object sender, RoutedEventArgs e)
        {
            if (PlaylistSongList.SelectedIndex < 0 || PlaylistSongList.SelectedIndex >= BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList.Count)
            {
                txtbox.AppendText("\n\nYou must have a valid Song selected");
                txtbox.ScrollToEnd();
                return;
            }
            BeatSaverAPISong s = interactor.GetBeatSaverAPISongViaHash(BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList[PlaylistSongList.SelectedIndex].SongID.Replace("custom_level_", "").Replace("customlevel_", ""));
            if (!s.GoodRequest)
            {
                txtbox.AppendText("\n\nI couldn't look up " + BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList[PlaylistSongList.SelectedIndex].SongName + " on BeatSaver");
                txtbox.ScrollToEnd();
                return;
            }
            Process.Start("https://beatsaver.com/beatmap/" + s.key);
        }

        private void SSSearch(object sender, RoutedEventArgs e)
        {
            if (PlaylistSongList.SelectedIndex < 0 || PlaylistSongList.SelectedIndex >= BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList.Count)
            {
                txtbox.AppendText("\n\nYou must have a valid Song selected");
                txtbox.ScrollToEnd();
                return;
            }
            Process.Start("https://scoresaber.com/?search=" + BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList[PlaylistSongList.SelectedIndex].SongName);
        }

        private void SPName(object sender, RoutedEventArgs e)
        {
            BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList = BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList.OrderBy(x => x.SongName).ToList<BMBFSong>();

            reloadPLs();
        }

        private void SPArtist(object sender, RoutedEventArgs e)
        {
            BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList = BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList.OrderBy(x => x.SongAuthorName).ToList<BMBFSong>();

            reloadPLs();
        }

        private void SPMapper(object sender, RoutedEventArgs e)
        {
            BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList = BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList.OrderBy(x => x.LevelAuthorName).ToList<BMBFSong>();

            reloadPLs();
        }

        private void SUName(object sender, RoutedEventArgs e)
        {
            UnsortedPlaylist = UnsortedPlaylist.OrderBy(x => x.SongName).ToList<BMBFSong>();

            reloadPLs();
        }

        private void SUArtist(object sender, RoutedEventArgs e)
        {
            UnsortedPlaylist = UnsortedPlaylist.OrderBy(x => x.SongAuthorName).ToList<BMBFSong>();

            reloadPLs();
        }

        private void SUMapper(object sender, RoutedEventArgs e)
        {
            UnsortedPlaylist = UnsortedPlaylist.OrderBy(x => x.LevelAuthorName).ToList<BMBFSong>();

            reloadPLs();
        }

        private void EBPList(object sender, RoutedEventArgs e)
        {
            if (Playlists.SelectedIndex < 0 || Playlists.SelectedIndex >= BMBFConfig.Config.Playlists.Count)
            {
                txtbox.AppendText("\n\nYou must have a valid Playlist selected");
                txtbox.ScrollToEnd();
                return;
            }
            txtbox.AppendText("\n\nDownloading Playlist Cover");
            txtbox.ScrollToEnd();
            BPList l = new BPList();

            l.playlistAuthor = "BMBF Manager";
            l.playlistTitle = BMBFConfig.Config.Playlists[Playlists.SelectedIndex].PlaylistName;

            using (WebClient client = new WebClient())
            {
                try
                {
                    client.DownloadFile("http://" + MainWindow.IP + ":50000/host/beatsaber/playlist/cover?PlaylistID=" + BMBFConfig.Config.Playlists[Playlists.SelectedIndex].PlaylistID, exe + "\\tmp\\Playlist.png");
                }
                catch
                {
                    txtbox.AppendText(MainWindow.BMBF100);
                    txtbox.ScrollToEnd();
                    return;
                }
            }

            l.image += Convert.ToBase64String(File.ReadAllBytes(exe + "\\tmp\\Playlist.png"));
            foreach(BMBFSong s in BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList)
            {
                if (!s.SongID.Contains("custom_level_")) continue;
                BPListSong song = new BPListSong();
                song.hash = s.SongID.Replace("custom_level_", "");
                song.songName = s.SongName;
                l.songs.Add(song);
            }
            File.WriteAllText(exe + "\\BPLists\\" + l.playlistTitle + ".bplist", JsonSerializer.Serialize(l));
            txtbox.AppendText("\n\nExported BPList to /BPLists/" + l.playlistTitle + ".bplist with " + l.songs.Count + " songs.");
            txtbox.ScrollToEnd();
        }

        private void SPreview(object sender, RoutedEventArgs e)
        {
            if (PlaylistSongList.SelectedIndex < 0 || PlaylistSongList.SelectedIndex >= BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList.Count)
            {
                txtbox.AppendText("\n\nYou must have a valid Song selected");
                txtbox.ScrollToEnd();
                return;
            }
            BeatSaverAPISong s = interactor.GetBeatSaverAPISongViaHash(BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList[PlaylistSongList.SelectedIndex].SongID.Replace("custom_level_", "").Replace("customlevel_", ""));
            if (!s.GoodRequest)
            {
                txtbox.AppendText("\n\nI couldn't look up " + BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList[PlaylistSongList.SelectedIndex].SongName + " on BeatSaver");
                txtbox.ScrollToEnd();
                return;
            }
            Process.Start("https://skystudioapps.com/bs-viewer/?id=" + s.key);
        }

        private async void IBPList(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "BPLists (*.json, *.bplist) | *.json;*.bplist";

            bool? result = ofd.ShowDialog();
            String BPListFile = "";
            if (result == true)
            {
                //Get the path of specified file
                if (File.Exists(ofd.FileName))
                {
                    BPListFile = ofd.FileName;
                }
                else
                {
                    MessageBox.Show("Please select a valid file", "BMBF Manager - Playlist Editor", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            } else
            {
                txtbox.AppendText("\n\nBPList importing aborted.");
                txtbox.ScrollToEnd();
                return;
            }

            
            BPList BPList;
            try
            {
                BPList = JsonSerializer.Deserialize<BPList>(File.ReadAllText(BPListFile), new JsonSerializerOptions {PropertyNameCaseInsensitive = false });
            } catch
            {
                txtbox.AppendText("\n\nThe BPList you choose is not valid");
                txtbox.ScrollToEnd();
                return;
            }
            if(BPList == null)
            {
                txtbox.AppendText("\n\nThe BPList you choose is not valid");
                txtbox.ScrollToEnd();
                return;
            }

            if(BPList.songs.Count < 1)
            {
                txtbox.AppendText("\n\nThe BPList doesn't contain any songs.");
                txtbox.ScrollToEnd();
                return;
            }

            List<BPListSong> tmp = new List<BPListSong>(BPList.songs);
            List<BMBFSong> existing = new List<BMBFSong>();
            bool PLExists = false;
            foreach(BMBFPlaylist p in BMBFConfig.Config.Playlists)
            {
                if (p.PlaylistName == BPList.playlistTitle) PLExists = true;
                foreach(BMBFSong s in p.SongList)
                {
                    int i = 0;
                    int removed = 0;
                    foreach(BPListSong search in tmp)
                    {
                        if(search.hash == "")
                        {
                            txtbox.AppendText("Removed " + search.songName + " due to not having a hash.");
                            BPList.songs.RemoveAt(i - removed);
                            tmp.RemoveAt(i - removed);
                            removed++;
                        }
                        if(s.SongID.ToLower().Contains(search.hash.ToLower()))
                        {
                            txtbox.AppendText("\n\n" + s.SongName + " already exists.");
                            existing.Add(s);
                            tmp.RemoveAt(i - removed);
                            removed++;
                            break;
                        }
                        i++;
                    }
                }
            }

            txtbox.ScrollToEnd();

            bool moveexisting = false;

            // 0 = make new Playlist, 1 = add all songs to existing PL, 2 = delete existing PL and make new one
            int action = 0;
            if(existing.Count > 0)
            {
                MessageBoxResult r = MessageBox.Show("You already have " + existing.Count + " out of " + BPList.songs.Count + " Songs from the BPList " + BPList.playlistTitle + " by " + BPList.playlistAuthor + " installed. Do you want to move the already installed Songs to the BPLists new Playlist?", "BMBF Manager - Playlist Editor", MessageBoxButton.YesNo, MessageBoxImage.Question);
                switch(r)
                {
                    case MessageBoxResult.Yes:
                        moveexisting = true;
                        break;
                }
            }

            if(PLExists)
            {
                MessageBoxResult r = MessageBox.Show("You already the Playlist " + BPList.playlistTitle + ". Do you want to make a new Playlist (yes), add all new songs into the existing Playlist (no) or delete the existing Playlist and Create a mew one (cancel)?", "BMBF Manager - Playlist Editor", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                switch (r)
                {
                    case MessageBoxResult.No:
                        action = 1;
                        break;
                    case MessageBoxResult.Cancel:
                        action = 2;
                        break;
                }
            }

            if(PLExists || existing.Count > 0)
            {
                MessageBoxResult r = MessageBox.Show("You are about to import a BPList. You already have " + existing.Count + " songs. " + (BPList.songs.Count - existing.Count) + " songs will get installed. You choose to " + (action == 0 ? "make a new Playlist" : "") + (action == 1 ? "add new songs into the existing Playlist" : "") + (action == 2 ? "delete the existing Playlist with all it's songs and make a new one.": "") + ". Do you want to continue?", "BMBF Manager - Playlist Editor", MessageBoxButton.YesNo, MessageBoxImage.Question);
                switch (r)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\nBPList importing aborted.");
                        txtbox.ScrollToEnd();
                        return;
                }
            }
            txtbox.AppendText("\n\npl: " + BPList.songs.Count + ", tmp: " + tmp.Count);
            txtbox.ScrollToEnd();
            if (tmp.Count > 0)
            {
                Songs SongsWindow = new Songs();
                SongsWindow.Show();
                waiting = true;

                foreach (BPListSong s in tmp)
                {
                    BeatSaverAPISong r = interactor.GetBeatSaverAPISongViaHash(s.hash);
                    if (!r.GoodRequest)
                    {
                        txtbox.AppendText("\n\n" + s.songName + " (" + s.hash + ") couldn't be found on BeatSaver");
                        txtbox.ScrollToEnd();
                        continue;
                    }
                    SongsWindow.InstallSongPE(r.key);
                }

                txtbox.AppendText("\n\nInstalling Songs");
                txtbox.ScrollToEnd();
                while (waiting)
                {
                    await Task.Delay(1000);
                }
                txtbox.AppendText("\nInstalled Songs");
                txtbox.ScrollToEnd();

                await Task.Delay(5000);
            }

            TimeoutWebClientShort client = new TimeoutWebClientShort();
            BMBFC current = new BMBFC();

            try
            {
                current = JsonSerializer.Deserialize<BMBFC>(client.DownloadString("http://" + MainWindow.IP + ":50000/host/beatsaber/config"));

            }
            catch
            {
                txtbox.AppendText(MainWindow.BMBF100);
                txtbox.AppendText("\n\nAborting");
                txtbox.ScrollToEnd();
                return;
            }

            if (action == 0)
            {
                BMBFPlaylist newpl = new BMBFPlaylist();
                newpl.PlaylistName = BPList.playlistTitle;
                newpl.CoverImageBytes = BPList.image.Replace("data:image/png;base64,", "").Replace("data:image/jpg;base64,", "").Replace("data:image/jpeg;base64,", "").Replace(",", "");
                if(moveexisting)
                {
                    // Move existing Songs
                    int removed2 = 0;
                    int pl2 = 0;
                    foreach (BMBFPlaylist p in BMBFConfig.Config.Playlists)
                    {
                        removed2 = 0;
                        List<BMBFSong> tmp2 = new List<BMBFSong>(p.SongList);
                        int i = 0;
                        foreach (BMBFSong s in tmp2)
                        {
                            foreach (BMBFSong search in existing)
                            {
                                if (search.SongID.ToLower() == s.SongID.ToLower())
                                {
                                    txtbox.AppendText("\n\nRemoved existing song " + s.SongName + " from Playlist");
                                    BMBFConfig.Config.Playlists[pl2].SongList.RemoveAt(i - removed2);
                                    removed2++;
                                    break;
                                }
                            }
                            i++;
                        }
                        pl2++;
                    }

                    foreach (BMBFSong s in existing) newpl.SongList.Add(s);
                    txtbox.AppendText("\n\nAdded existing Songs to BPList Playlist");
                }
                txtbox.ScrollToEnd();

                // Move new Songs
                int removed3 = 0;
                foreach (BMBFPlaylist p in current.Config.Playlists)
                {
                    if (p.PlaylistName == BPList.playlistTitle) PLExists = true;
                    List<BMBFSong> tmp2 = new List<BMBFSong>(p.SongList);
                    foreach (BMBFSong s in tmp2)
                    {
                        int i = 0;
                        foreach(BPListSong search in tmp)
                        {
                            if(search.hash.ToLower() == s.SongID.ToLower().Replace("custom_level_", ""))
                            {
                                txtbox.AppendText("\n\nMoved Songs " + s.SongName + " to BPList");
                                newpl.SongList.Add(s);
                                p.SongList.RemoveAt(i - removed3);
                                removed3++;
                                break;
                            }
                        }
                    }
                }
                txtbox.ScrollToEnd();
                BMBFConfig.Config.Playlists.Add(newpl);
            } else if(action == 1)
            {
                int pl = 0;
                foreach (BMBFPlaylist p in BMBFConfig.Config.Playlists)
                {
                    if (p.PlaylistName == BPList.playlistTitle) break;
                    pl++;
                }
                BMBFConfig.Config.Playlists[pl].CoverImageBytes = BPList.image.Replace("data:image/png;base64,", "").Replace("data:image/jpg;base64,", "").Replace("data:image/jpeg;base64,", "").Replace(",", "");
                if (moveexisting)
                {
                    // Move existing Songs
                    int removed2 = 0;
                    int pl2 = 0;
                    foreach (BMBFPlaylist p in BMBFConfig.Config.Playlists)
                    {
                        removed2 = 0;
                        if (p.PlaylistName == BPList.playlistTitle) PLExists = true;
                        List<BMBFSong> tmp2 = new List<BMBFSong>(p.SongList);
                        int i = 0;
                        foreach (BMBFSong s in tmp2)
                        {
                            foreach (BMBFSong search in existing)
                            {
                                if (search.SongID.ToLower() == s.SongID.ToLower())
                                {
                                    txtbox.AppendText("\n\nRemoved existing song " + s.SongName + " from Playlist");
                                    BMBFConfig.Config.Playlists[pl2].SongList.RemoveAt(i - removed2);
                                    removed2++;
                                    break;
                                }
                            }
                            i++;
                        }
                        pl2++;
                    }

                    foreach (BMBFSong s in existing) BMBFConfig.Config.Playlists[pl].SongList.Add(s);
                    txtbox.AppendText("\n\nAdded existing Songs to BPList Playlist");
                    txtbox.ScrollToEnd();
                }

                int removed3 = 0;
                foreach (BMBFPlaylist p in current.Config.Playlists)
                {
                    if (p.PlaylistName == BPList.playlistTitle) PLExists = true;
                    List<BMBFSong> tmp2 = new List<BMBFSong>(p.SongList);
                    foreach (BMBFSong s in tmp2)
                    {
                        int i = 0;
                        foreach (BPListSong search in tmp)
                        {
                            if (search.hash.ToLower() == s.SongID.ToLower().Replace("custom_level_", ""))
                            {
                                txtbox.AppendText("\n\nMoved Songs " + s.SongName + " to BPList");
                                BMBFConfig.Config.Playlists[pl].SongList.Add(s);
                                p.SongList.RemoveAt(i - removed3);
                                removed3++;
                                break;
                            }
                        }
                    }
                }
            } else if(action == 2)
            {
                int pl = 0;
                foreach (BMBFPlaylist p in BMBFConfig.Config.Playlists)
                {
                    if (p.PlaylistName == BPList.playlistTitle)
                    {
                        BMBFConfig.Config.Playlists.RemoveAt(pl);
                        break;
                    }
                    pl++;
                }

                BMBFPlaylist newpl = new BMBFPlaylist();
                newpl.PlaylistName = BPList.playlistTitle;
                newpl.CoverImageBytes = BPList.image.Replace("data:image/png;base64,", "").Replace("data:image/jpg;base64", "").Replace("data:image/jpeg;base64", "").Replace(",", "");

                if (moveexisting)
                {
                    // Move existing Songs
                    int removed2 = 0;
                    int pl2 = 0;
                    foreach (BMBFPlaylist p in current.Config.Playlists)
                    {
                        removed2 = 0;
                        if (p.PlaylistName == BPList.playlistTitle) PLExists = true;
                        List<BMBFSong> tmp2 = new List<BMBFSong>(p.SongList);
                        int i = 0;
                        foreach (BMBFSong s in tmp2)
                        {
                            foreach (BMBFSong search in existing)
                            {
                                if (search.SongID.ToLower() == s.SongID.ToLower())
                                {
                                    txtbox.AppendText("\n\nRemoved existing song " + s.SongName + " from Playlist");
                                    BMBFConfig.Config.Playlists[pl2].SongList.RemoveAt(i - removed2);
                                    removed2++;
                                    break;
                                }
                            }
                            i++;
                        }
                        pl2++;
                        if (pl2 >= BMBFConfig.Config.Playlists.Count) break;
                    }

                    foreach (BMBFSong s in existing) newpl.SongList.Add(s);
                    txtbox.AppendText("\n\nAdded existing Songs to BPList Playlist");
                }

                // Move new Songs
                int removed3 = 0;
                foreach (BMBFPlaylist p in BMBFConfig.Config.Playlists)
                {
                    if (p.PlaylistName == BPList.playlistTitle) PLExists = true;
                    List<BMBFSong> tmp2 = new List<BMBFSong>(p.SongList);
                    foreach (BMBFSong s in tmp2)
                    {
                        int i = 0;
                        foreach (BPListSong search in tmp)
                        {
                            if (search.hash.ToLower() == s.SongID.ToLower().Replace("custom_level_", ""))
                            {
                                txtbox.AppendText("\n\nMoved Songs " + s.SongName + " to BPList");
                                newpl.SongList.Add(s);
                                p.SongList.RemoveAt(i - removed3);
                                removed3++;
                                break;
                            }
                        }
                    }
                }

                BMBFConfig.Config.Playlists.Add(newpl);
            }

            txtbox.AppendText("\n\n\nInstalled BPList " + BPList.playlistTitle + " by " + BPList.playlistAuthor);
            txtbox.ScrollToEnd();
            // Update All Lists
            Playlists.Items.Clear();
            foreach (BMBFPlaylist Playlist in BMBFConfig.Config.Playlists)
            {
                Playlists.Items.Add(Playlist.PlaylistName);
            }
            if (Playlists.Items.Count < 1)
            {
                txtbox.AppendText("\n\nSomething went wrong.");
                txtbox.ScrollToEnd();
                return;
            }
            Playlists.SelectedIndex = 0;
        }

        private void ChaPlC(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "Picture (*.png, *.jpg, *.jpeg) | *.png;*.jpg;*.jpeg";

            bool? result = ofd.ShowDialog();
            if (result == true)
            {
                //Get the path of specified file
                if(!File.Exists(ofd.FileName))
                {
                    MessageBox.Show("Please select a valid file", "BMBF Manager - Playlist Editor", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else
            {
                txtbox.AppendText("\n\nCover changing aborted");
                txtbox.ScrollToEnd();
                return;
            }

            BMBFConfig.Config.Playlists[Playlists.SelectedIndex].CoverImageBytes = Convert.ToBase64String(File.ReadAllBytes(ofd.FileName));
            txtbox.AppendText("\n\nChanged Playlist Cover");
            txtbox.ScrollToEnd();
            int s = Playlists.SelectedIndex;
            Playlists.SelectedIndex = 0;
            Playlists.SelectedIndex = s;
        }

        private void NewPl(object sender, RoutedEventArgs e)
        {
            if(PlaylistName.Text == "Playlist Name")
            {
                txtbox.AppendText("\n\nPlease type in a Playlist name");
                txtbox.ScrollToEnd();
                return;
            }
            BMBFPlaylist p = new BMBFPlaylist();
            p.PlaylistName = PlaylistName.Text;
            BMBFConfig.Config.Playlists.Add(p);

            txtbox.AppendText("\n\nCreated Playlist " + PlaylistName.Text);
            txtbox.ScrollToEnd();

            Playlists.Items.Clear();
            foreach (BMBFPlaylist Playlist in BMBFConfig.Config.Playlists)
            {
                Playlists.Items.Add(Playlist.PlaylistName);
            }
            if (Playlists.Items.Count < 1)
            {
                txtbox.AppendText("\n\nSomething went wrong.");
                txtbox.ScrollToEnd();
                return;
            }
            Playlists.SelectedIndex = 0;
        }

        private void RenPl(object sender, RoutedEventArgs e)
        {
            if (PlaylistName.Text == "Playlist Name")
            {
                txtbox.AppendText("\n\nPlease type in a Playlist name");
                txtbox.ScrollToEnd();
                return;
            }

            txtbox.AppendText("\n\nRenamed " + BMBFConfig.Config.Playlists[Playlists.SelectedIndex].PlaylistName + " to " + PlaylistName.Text);
            txtbox.ScrollToEnd();

            BMBFConfig.Config.Playlists[Playlists.SelectedIndex].PlaylistName = PlaylistName.Text;

            Playlists.Items.Clear();
            foreach (BMBFPlaylist Playlist in BMBFConfig.Config.Playlists)
            {
                Playlists.Items.Add(Playlist.PlaylistName);
            }
            if (Playlists.Items.Count < 1)
            {
                txtbox.AppendText("\n\nSomething went wrong.");
                txtbox.ScrollToEnd();
                return;
            }
            Playlists.SelectedIndex = 0;
        }

        private void SavePlaylists(object sender, RoutedEventArgs e)
        {
            if(UnsortedPlaylist.Count > 0)
            {
                MessageBoxResult r = MessageBox.Show("Warning! You still have unsorted songs (the playlist at the right). If you save you'll loose all those Songs! Do you wish to abort?", "BMBF Manager - Playlist Editor", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch(r)
                {
                    case MessageBoxResult.Yes:
                        txtbox.AppendText("\n\nSaving aborted");
                        txtbox.ScrollToEnd();
                        return;
                }
            }

            if(BMBFConfig.Config.Playlists.Count < 0)
            {
                txtbox.AppendText("\n\nSaving was aborted due to not having any Playlists at all.");
                txtbox.ScrollToEnd();
                return;
            }
            File.WriteAllText(exe + "\\tmp\\config.json", JsonSerializer.Serialize(BMBFConfig.Config));
            postChanges(exe + "\\tmp\\config.json");
            txtbox.AppendText("\n\nSaved Playlists");
            txtbox.ScrollToEnd();
        }

        public void postChanges(String Config)
        {

            using (WebClient client = new WebClient())
            {
                try
                {
                    client.QueryString.Add("foo", "foo");
                    client.UploadFile("http://" + MainWindow.IP + ":50000/host/beatsaber/config", "PUT", Config);
                    System.Threading.Thread.Sleep(2000);
                    client.UploadValues("http://" + MainWindow.IP + ":50000/host/beatsaber/commitconfig", "POST", client.QueryString);
                }
                catch
                {
                    txtbox.AppendText(MainWindow.BMBF100);
                    txtbox.ScrollToEnd();
                    return;
                }
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

        private void ClearTextPN(object sender, RoutedEventArgs e)
        {
            if (PlaylistName.Text == "Playlist Name")
            {
                PlaylistName.Text = "";
            }

        }

        private void QuestIPCheck(object sender, RoutedEventArgs e)
        {
            if (Quest.Text == "")
            {
                Quest.Text = "Quest IP";
            }
        }

        private void PNameCheck(object sender, RoutedEventArgs e)
        {
            if (PlaylistName.Text == "")
            {
                PlaylistName.Text = "Playlist Name";
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
            txtbox.AppendText(MainWindow.ADB110);
            txtbox.ScrollToEnd();
            return false;
        }
    }
}
