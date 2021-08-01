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
using System.Text.RegularExpressions;
using ComputerUtils.RegxTemplates;
using ComputerUtils.StringFormatters;
using BMBFManager.Utils;

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
        BMBFC orgConfig = new BMBFC();
        List<BMBFSong> UnsortedPlaylist = new List<BMBFSong>();
        BeatSaverAPIInteractor interactor = new BeatSaverAPIInteractor();
        BSKFile known = new BSKFile();
        bool loaded = false;

        //public static String bsk = "https://raw.githubusercontent.com/ComputerElite/resources/master/assets/beatsaber-knowns.json";

        public static bool waiting = false;
        BMBFSong currentSong = new BMBFSong();

        public PlaylistEditor()
        {
            InitializeComponent();
            ApplyLanguage();
            Quest.Text = MainWindow.config.IP;

            if (!Directory.Exists(exe + "\\BPLists"))
            {
                Directory.CreateDirectory(exe + "\\BPLists");
            }

            LoadPlaylists(true);

            if (!MainWindow.config.PEWarningShown)
            {
                MessageBox.Show(MainWindow.globalLanguage.playlistEditor.code.pENotes, "BMBF Manager - Playlist Editor", MessageBoxButton.OK, MessageBoxImage.Information);
                MainWindow.config.PEWarningShown = true;
            }

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
                uniformBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/PlaylistEditor3.png", UriKind.Absolute));
                uniformBrush.Stretch = Stretch.UniformToFill;
                this.Background = uniformBrush;
            }

            WebClient client = new WebClient();
            /*
            try
            {
                String t = client.DownloadString(bsk);
                known = JsonSerializer.Deserialize<BSKFile>(t);
                loaded = true;
            }
            catch
            {
                txtbox.AppendText(MainWindow.globalLanguage.global.anErrorOccured);
            }
            */
            MainWindow.DCRPM.SetActivity(MainWindow.globalLanguage.dCRP.editingPlaylists);
        }

        public void ApplyLanguage()
        {
            loadPlaylistsButton.Content = MainWindow.globalLanguage.playlistEditor.UI.loadPlaylistsButton;
            savePlaylistsButton.Content = MainWindow.globalLanguage.playlistEditor.UI.savePlaylistsButton;
            exportBPListButton.Content = MainWindow.globalLanguage.playlistEditor.UI.exportBPListButton;
            createPlaylistButton.Content = MainWindow.globalLanguage.playlistEditor.UI.createPlaylistButton;
            renamePlaylistButton.Content = MainWindow.globalLanguage.playlistEditor.UI.renamePlaylistButton;
            deletePlaylistButton.Content = MainWindow.globalLanguage.playlistEditor.UI.deletePlaylistButton;
            changeCoverButton.Content = MainWindow.globalLanguage.playlistEditor.UI.changeCoverButton;
            importBPListButton.Content = MainWindow.globalLanguage.playlistEditor.UI.importBPListButton;
            MovePlaylistLeftButton.Content = MainWindow.globalLanguage.playlistEditor.UI.movePlaylistLeftButton;
            MoveSongLeftButton.Content = MainWindow.globalLanguage.playlistEditor.UI.moveSongLeftButton;
            MovePlaylistRightButton.Content = MainWindow.globalLanguage.playlistEditor.UI.movePlaylistRightButton;
            MoveSongRightButton.Content = MainWindow.globalLanguage.playlistEditor.UI.moveSongRightButton;
            //DeleteSongButton.Content = MainWindow.globalLanguage.playlistEditor.UI.deleteSongButton;
            BeastSaberButton.Content = MainWindow.globalLanguage.playlistEditor.UI.beastSaberButton;
            BeatSaverButton.Content = MainWindow.globalLanguage.playlistEditor.UI.beatSaverButton;
            ScoreSaberButton.Content = MainWindow.globalLanguage.playlistEditor.UI.scoreSaberButton;
            PreviewButton.Content = MainWindow.globalLanguage.playlistEditor.UI.previewButton;
            SortNameButton1.Content = MainWindow.globalLanguage.playlistEditor.UI.sortNameButton;
            SortNameButton2.Content = MainWindow.globalLanguage.playlistEditor.UI.sortNameButton;
            SortArtistButton1.Content = MainWindow.globalLanguage.playlistEditor.UI.sortArtistButton;
            SortArtistButton2.Content = MainWindow.globalLanguage.playlistEditor.UI.sortArtistButton;
            SortMapperButton1.Content = MainWindow.globalLanguage.playlistEditor.UI.sortMapperButton;
            SortMapperButton2.Content = MainWindow.globalLanguage.playlistEditor.UI.sortMapperButton;
            SortByText1.Text = MainWindow.globalLanguage.playlistEditor.UI.sortByText;
            SortByText2.Text = MainWindow.globalLanguage.playlistEditor.UI.sortByText;
            TotalSongs.Text = MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.playlistEditor.UI.totalSongs, "");
            PlaylistSongCount.Text = MainWindow.globalLanguage.playlistEditor.UI.amountSongs;
            UnsortedSongcount.Text = MainWindow.globalLanguage.playlistEditor.UI.amountSongs;
            PlaylistName.Text = MainWindow.globalLanguage.playlistEditor.UI.playlistName;
            Name.Text = MainWindow.globalLanguage.playlistEditor.UI.chooseSong;
            plUpButton.Content = MainWindow.globalLanguage.playlistEditor.UI.pLUpButton;
            plDownButton.Content = MainWindow.globalLanguage.playlistEditor.UI.pLDownButton;
            ((GridView)PlaylistSongList.View).Columns[0].Header = MainWindow.globalLanguage.playlistEditor.UI.songNameList;
            ((GridView)PlaylistSongList.View).Columns[1].Header = MainWindow.globalLanguage.playlistEditor.UI.artistList;
            ((GridView)PlaylistSongList.View).Columns[2].Header = MainWindow.globalLanguage.playlistEditor.UI.mapperList;
            ((GridView)UnsortedSongsPlaylist.View).Columns[0].Header = MainWindow.globalLanguage.playlistEditor.UI.songNameList;
            ((GridView)UnsortedSongsPlaylist.View).Columns[1].Header = MainWindow.globalLanguage.playlistEditor.UI.artistList;
            ((GridView)UnsortedSongsPlaylist.View).Columns[2].Header = MainWindow.globalLanguage.playlistEditor.UI.mapperList;
            AllSongsText.Text = MainWindow.globalLanguage.playlistEditor.UI.allSongsText;
            SelectedPlaylistText.Text = MainWindow.globalLanguage.playlistEditor.UI.selectedPlaylistText;
        }

        private void getPlaylists(object sender, RoutedEventArgs e)
        {
            LoadPlaylists();
        }

        private void LoadPlaylists(bool onstart = false)
        {
            if (!MainWindow.iPUtils.CheckIP(Quest))
            {
                txtbox.AppendText(MainWindow.globalLanguage.global.ipInvalid);
                txtbox.ScrollToEnd();
                return;
            }
            TimeoutWebClientShort client = new TimeoutWebClientShort();

            try
            {
                BMBFConfig = BMBFUtils.GetBMBFConfig();
                orgConfig = BMBFConfig;
                //BMBFConfig.AdjustForPLEditing();
            }
            catch
            {
                if (onstart)
                {
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.loadPlaylists);
                    txtbox.ScrollToEnd();
                }
                else
                {
                    txtbox.AppendText(MainWindow.globalLanguage.global.BMBF100);
                }
                return;
            }
            Playlists.Items.Clear();
            UnsortedPlaylist.Clear();
            UnsortedSongsPlaylist.Items.Clear();
            foreach (BMBFPlaylist Playlist in BMBFConfig.Config.Playlists)
            {
                Playlists.Items.Add(Playlist.PlaylistName);
            }
            foreach (BMBFSong s in BMBFConfig.Config.GetAllSongs()) UnsortedPlaylist.Add(s);
            if (Playlists.Items.Count < 1)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.zeroPlaylists);
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
            MainWindow.DCRPM.SetActivity(MainWindow.globalLanguage.dCRP.editingPlaylists);
            if (Playlists.SelectedIndex < 0 || Playlists.SelectedIndex >= BMBFConfig.Config.Playlists.Count)
            {
                //txtbox.AppendText("\n\nPlease select a valid Playlist");
                txtbox.ScrollToEnd();
                return;
            }
            int leftSel = PlaylistSongList.SelectedIndex;
            int rightSel = UnsortedSongsPlaylist.SelectedIndex;
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
                if (changed) txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.playlistEditor.code.playlistDoesntContainSongs, BMBFConfig.Config.Playlists[Playlists.SelectedIndex].PlaylistName));
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

            try
            {
                BitmapImage i = new BitmapImage();
                i.BeginInit();
                i.UriSource = new Uri("http://" + MainWindow.config.IP + ":50000/host/beatsaber/playlist/cover?playlistid=" + BMBFConfig.Config.Playlists[Playlists.SelectedIndex].PlaylistId, UriKind.Absolute);

                i.EndInit();
                PlaylistCoverImage.Source = i;
            }
            catch
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.couldntGetCover);
            }


            PlaylistSongCount.Text = MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.playlistEditor.UI.songsCounter, PlaylistSongList.Items.Count.ToString());
            UnsortedSongcount.Text = MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.playlistEditor.UI.songsCounter, UnsortedSongsPlaylist.Items.Count.ToString());
            txtbox.ScrollToEnd();

            TotalSongs.Text = MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.playlistEditor.UI.totalSongs, BMBFConfig.Config.GetTotalSongsCount().ToString());

            if (leftSel >= PlaylistSongList.Items.Count) leftSel = PlaylistSongList.Items.Count - 1;
            if (leftSel < 0) leftSel = 0;
            PlaylistSongList.SelectedIndex = leftSel;

            if (rightSel >= UnsortedSongsPlaylist.Items.Count) rightSel = UnsortedSongsPlaylist.Items.Count - 1;
            if (rightSel < 0) rightSel = 0;
            UnsortedSongsPlaylist.SelectedIndex = rightSel;
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
            ID.Text = BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList[PlaylistSongList.SelectedIndex].Hash;

            BitmapImage i = new BitmapImage();
            i.BeginInit();
            i.UriSource = new Uri("http://" + MainWindow.config.IP + ":50000/host/beatsaber/song/cover?Hash=" + BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList[PlaylistSongList.SelectedIndex].Hash, UriKind.Absolute);
            i.EndInit();
            SongCoverImage.Source = i;
            currentSong = BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList[PlaylistSongList.SelectedIndex];
            txtbox.ScrollToEnd();
        }

        private void ChangeCurrentSongU(object sender, SelectionChangedEventArgs e)
        {
            if (UnsortedSongsPlaylist.SelectedIndex < 0 || UnsortedSongsPlaylist.SelectedIndex >= UnsortedPlaylist.Count)
            {
                return;
            }
            int index = UnsortedSongsPlaylist.SelectedIndex;
            Name.Text = UnsortedPlaylist[index].SongName;
            SubName.Text = UnsortedPlaylist[index].SongSubName;
            Author.Text = UnsortedPlaylist[index].SongAuthorName;
            Mapper.Text = UnsortedPlaylist[index].LevelAuthorName;
            ID.Text = UnsortedPlaylist[index].Hash;

            BitmapImage i = new BitmapImage();
            i.BeginInit();
            i.UriSource = new Uri("http://" + MainWindow.config.IP + ":50000/host/beatsaber/song/cover?Hash=" + UnsortedPlaylist[index].Hash, UriKind.Absolute);
            i.EndInit();
            SongCoverImage.Source = i;
            currentSong = UnsortedPlaylist[index];
            txtbox.ScrollToEnd();
        }

        /* Unneeded atm
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
                    txtbox.AppendText(MainWindow.globalLanguage.global.anErrorOccured);
                    txtbox.ScrollToEnd();
                }
            }

            if (known.knownLevelIds.Contains(song.Hash)) return true;
            return false;
        }
        */

        /* Button Removed
        private void DelSong(object sender, RoutedEventArgs e)
        {
            List<int> indexe = GetSelectedNormalItemIndexAdjusted(PlaylistSongList);
            int NotProcessed = 0;
            foreach (int i in indexe)
            {
                int ii = i + NotProcessed;
                if (ii < 0 || ii >= BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList.Count)
                {
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.songMustBeSelected);
                    txtbox.ScrollToEnd();
                    NotProcessed++;
                    continue;
                }

                if (IsOST(BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList[ii]))
                {
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.oSTDeletingNotAllowed);
                    txtbox.ScrollToEnd();
                    NotProcessed++;
                    continue;
                }

                //MessageBoxResult r = MessageBox.Show(MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.playlistEditor.code.sureDeleteSong, BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList[ii].SongName), "BMBF Manager - Playlist Editor", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                //switch (r)
                //{
                //    case MessageBoxResult.No:
                //        txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.playlistEditor.code.deletingSongAborted, BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList[ii].SongName));
                //        txtbox.ScrollToEnd();
                //        NotProcessed++;
                //        continue;
                //}
                BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList.RemoveAt(ii);
                reloadPLs();
            }
        }
        */

        private void DelPl(object sender, RoutedEventArgs e)
        {
            if (Playlists.SelectedIndex < 0 || Playlists.SelectedIndex >= BMBFConfig.Config.Playlists.Count)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.mustHavePlaylistSelected);
                txtbox.ScrollToEnd();
                return;
            }

            /* Unneeded atm
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
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.global.anErrorOccured);
                    txtbox.ScrollToEnd();
                }
            }
            */
            if (known.knownLevelPackIds.Contains(BMBFConfig.Config.Playlists[Playlists.SelectedIndex].PlaylistId))
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.playlistEditor.code.oSTPlaylistDeletingNotAllowed, BMBFConfig.Config.Playlists[Playlists.SelectedIndex].PlaylistName));
                txtbox.ScrollToEnd();
                return;
            }

            MessageBoxResult r = MessageBox.Show(MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.playlistEditor.code.sureDeletePlaylist, BMBFConfig.Config.Playlists[Playlists.SelectedIndex].PlaylistName), "BMBF Manager - Playlist Editor", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            switch (r)
            {
                case MessageBoxResult.No:
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.playlistEditor.code.deletingPlaylistAborted, BMBFConfig.Config.Playlists[Playlists.SelectedIndex].PlaylistName));
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
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.zeroPlaylists);
                txtbox.ScrollToEnd();
                return;
            }
            Playlists.SelectedIndex = 0;
        }


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

        private void MoveSongRight(object sender, RoutedEventArgs e)
        {
            int selected = PlaylistSongList.SelectedIndex;
            if (selected < 0 || selected >= BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList.Count)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.songMustBeSelected);
                txtbox.ScrollToEnd();
                return;
            }

            /*
            if (IsOST(BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList[selected]))
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.playlistEditor.code.oSTMovingNotAllowed, BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList[selected].SongName));
                txtbox.ScrollToEnd();
                return;
            }
            */

            //if (UnsortedSongsPlaylist.SelectedIndex < 0 || UnsortedSongsPlaylist.SelectedIndex >= UnsortedPlaylist.Count)
            //{
            //    UnsortedPlaylist.Add(BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList[ii]);
            //}
            //else
            //{
            //    UnsortedPlaylist.Insert(UnsortedSongsPlaylist.SelectedIndex + 1, BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList[ii]);
            //}
            BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList.RemoveAt(selected);
            reloadPLs();
        }

        private void MovePlaylistRight(object sender, RoutedEventArgs e)
        {
            if (Playlists.SelectedIndex < 0 || Playlists.SelectedIndex >= BMBFConfig.Config.Playlists.Count)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.mustHavePlaylistSelected);
                txtbox.ScrollToEnd();
                return;
            }
            List<BMBFSong> tmp = new List<BMBFSong>(BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList);
            foreach (BMBFSong s in tmp)
            {
                /*
                if (IsOST(s))
                {
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.playlistEditor.code.oSTMovingNotAllowed, s.SongName));
                    txtbox.ScrollToEnd();
                    continue;
                }
                */

                //if (UnsortedSongsPlaylist.SelectedIndex < 0 || UnsortedSongsPlaylist.SelectedIndex >= UnsortedPlaylist.Count)
                //{
                //    UnsortedPlaylist.Add(s);
                //}
                //else
                //{
                //    UnsortedPlaylist.Insert(UnsortedSongsPlaylist.SelectedIndex + 1, s);
                //}
                BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList.RemoveAt(0);
            }

            reloadPLs();
        }

        private void MoveSongLeft(object sender, RoutedEventArgs e)
        {
            List<int> indexe = GetSelectedNormalItemIndexAdjusted(UnsortedSongsPlaylist);
            bool addAnyways = false;
            bool asked = false;

            int selected = UnsortedSongsPlaylist.SelectedIndex;
            if (selected < 0 || selected >= UnsortedPlaylist.Count)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.songMustBeSelected);
                txtbox.ScrollToEnd();
                return;
            }
            bool exists = false;
            foreach (BMBFSong song in BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList)
            {
                if (song.Equals(UnsortedPlaylist[selected]))
                {
                    exists = true;
                    if (!asked)
                    {
                        MessageBoxResult r = MessageBox.Show(MainWindow.globalLanguage.playlistEditor.code.songContained, "BMBF Manager - Playlist Editor", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (r == MessageBoxResult.Yes) addAnyways = true;
                    }
                    asked = true;
                    break;
                }
            }
            if (!exists || exists && addAnyways)
            {
                if (selected < 0 || selected >= BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList.Count)
                {
                    BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList.Add(UnsortedPlaylist[selected]);
                }
                else
                {
                    BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList.Insert(PlaylistSongList.SelectedIndex + 1, UnsortedPlaylist[selected]);
                }
            }
            //UnsortedPlaylist.RemoveAt(ii);
            reloadPLs();
        }

        private void MovePlaylistLeft(object sender, RoutedEventArgs e)
        {
            if (Playlists.SelectedIndex < 0 || Playlists.SelectedIndex >= BMBFConfig.Config.Playlists.Count)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.mustHavePlaylistSelected);
                txtbox.ScrollToEnd();
                return;
            }
            List<BMBFSong> tmp = new List<BMBFSong>(UnsortedPlaylist);
            bool addAnyways = false;
            bool asked = false;
            foreach (BMBFSong s in tmp)
            {
                bool exists = false;
                foreach (BMBFSong song in BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList)
                {
                    if (song.Equals(s))
                    {
                        exists = true;
                        if(!asked)
                        {
                            MessageBoxResult r = MessageBox.Show(MainWindow.globalLanguage.playlistEditor.code.oneOrMoreSongsContained, "BMBF Manager - Playlist Editor", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (r == MessageBoxResult.No) addAnyways = true;
                        }
                        asked = true;
                        break;
                    }
                }
                if(!exists || exists && addAnyways)
                {
                    if (PlaylistSongList.SelectedIndex < 0 || PlaylistSongList.SelectedIndex >= BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList.Count)
                    {
                        BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList.Add(s);
                    }
                    else
                    {
                        BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList.Insert(PlaylistSongList.SelectedIndex + 1, s);
                    }
                }
                
                //UnsortedPlaylist.RemoveAt(0);
            }

            reloadPLs();
        }

        private void BeastSShow(object sender, RoutedEventArgs e)
        {
            BeatSaverAPISong s = interactor.GetBeatSaverAPISongViaHash(currentSong.Hash.ToLower());
            if (!s.GoodRequest)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.playlistEditor.code.beatSaverLookupFailed, currentSong.SongName));
                txtbox.ScrollToEnd();
                return;
            }
            Process.Start("https://bsaber.com/songs/" + s.key);
        }

        private void BeatSShow(object sender, RoutedEventArgs e)
        {
            BeatSaverAPISong s = interactor.GetBeatSaverAPISongViaHash(currentSong.Hash.ToLower());
            if (!s.GoodRequest)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.playlistEditor.code.beatSaverLookupFailed, currentSong.SongName));
                txtbox.ScrollToEnd();
                return;
            }
            Process.Start("https://beatsaver.com/beatmap/" + s.key);
        }

        private void SSSearch(object sender, RoutedEventArgs e)
        {
            Process.Start("https://scoresaber.com/?search=" + currentSong.SongName);
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
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.mustHavePlaylistSelected);
                txtbox.ScrollToEnd();
                return;
            }

            MainWindow.DCRPM.SetActivity(MainWindow.globalLanguage.dCRP.exportingBPList);

            txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.downloadingPlaylistCover);
            txtbox.ScrollToEnd();
            BPList l = new BPList();

            l.playlistAuthor = "BMBF Manager";
            l.playlistTitle = BMBFConfig.Config.Playlists[Playlists.SelectedIndex].PlaylistName;

            using (WebClient client = new WebClient())
            {
                try
                {
                    client.DownloadFile("http://" + MainWindow.config.IP + ":50000/host/beatsaber/playlist/cover?PlaylistId=" + BMBFConfig.Config.Playlists[Playlists.SelectedIndex].PlaylistId, exe + "\\tmp\\Playlist.png");
                    l.image += Convert.ToBase64String(File.ReadAllBytes(exe + "\\tmp\\Playlist.png"));
                }
                catch
                {
                    txtbox.AppendText(MainWindow.globalLanguage.global.BMBF100);
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.qSU.code.skippingPlaylistCover);
                    txtbox.ScrollToEnd();
                }
            }
            
            foreach (BMBFSong s in BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList)
            {
                BPListSong song = new BPListSong();
                song.hash = s.Hash;
                song.songName = s.SongName;
                l.songs.Add(song);
            }
            File.WriteAllText(exe + "\\BPLists\\" + StringFormatter.FileNameSafe(l.playlistTitle) + ".bplist", JsonSerializer.Serialize(l));
            txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.playlistEditor.code.exportedBPList, l.playlistTitle, l.songs.Count.ToString()));
            txtbox.ScrollToEnd();
        }

        private List<int> GetSelectedNormalItemIndexAdjusted(ListView l)
        {
            List<int> index = new List<int>();
            foreach (var item in l.SelectedItems)
            {
                index.Add(l.Items.IndexOf(item));
            }
            index.Sort();
            int i = 0;
            List<int> tmp = new List<int>(index);
            foreach (int c in tmp)
            {
                index[i] = index[i] - i;
                i++;
            }
            return index;
        }

        private List<int> GetSelectedNormalItemIndex(ListView l)
        {
            List<int> index = new List<int>();
            foreach (var item in l.SelectedItems)
            {
                index.Add(l.Items.IndexOf(item));
            }
            return index;
        }

        private void SPreview(object sender, RoutedEventArgs e)
        {
            List<int> indexe = GetSelectedNormalItemIndex(PlaylistSongList);
            foreach (int i in indexe)
            {
                if (i < 0 || i >= BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList.Count)
                {
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.songMustBeSelected);
                    txtbox.ScrollToEnd();
                    return;
                }
                BeatSaverAPISong s = interactor.GetBeatSaverAPISongViaHash(BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList[i].Hash);
                if (!s.GoodRequest)
                {
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.playlistEditor.code.beatSaverLookupFailed, BMBFConfig.Config.Playlists[Playlists.SelectedIndex].SongList[i].SongName));
                    txtbox.ScrollToEnd();
                    return;
                }
                Process.Start("https://skystudioapps.com/bs-viewer/?id=" + s.key);
            }

        }

        private async void IBPList(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
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
                    MessageBox.Show(MainWindow.globalLanguage.playlistEditor.code.selectValidFile, "BMBF Manager - Playlist Editor", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.bPListImportingAborted);
                txtbox.ScrollToEnd();
                return;
            }
            BPList BPList = new BPList();
            try
            {
                BPList = JsonSerializer.Deserialize<BPList>(File.ReadAllText(BPListFile), new JsonSerializerOptions { PropertyNameCaseInsensitive = false });
            }
            catch
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.bPListNotValid);
                txtbox.ScrollToEnd();
                return;
            }
            ImportBPList(BPList);
        }

        public async void ImportBPList(BPList BPList, bool AutoMode = false)
        {
            if (BPList == null)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.bPListNotValid);
                txtbox.ScrollToEnd();
                return;
            }

            if (BPList.songs.Count < 1)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.bPListEmpty);
                txtbox.ScrollToEnd();
                return;
            }

            MainWindow.DCRPM.SetActivity(MainWindow.globalLanguage.dCRP.importingBPList);

            List<BPListSong> tmp = new List<BPListSong>(BPList.songs);
            List<BMBFSong> existing = new List<BMBFSong>();
            bool PLExists = false;
            foreach (BMBFPlaylist p in BMBFConfig.Config.Playlists)
            {
                if (p.PlaylistName == BPList.playlistTitle) PLExists = true;
            }

            foreach (BMBFSong s in BMBFConfig.Config.GetAllSongs())
            {
                int i = 0;
                int removed = 0;
                foreach (BPListSong search in tmp)
                {
                    if (search.hash == "")
                    {
                        txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.playlistEditor.code.removedBCMissingHash, search.songName));
                        BPList.songs.RemoveAt(i - removed);
                        tmp.RemoveAt(i - removed);
                        removed++;
                    }
                    if (s.Hash.ToLower().Contains(search.hash.ToLower()))
                    {
                        txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.playlistEditor.code.songExists, s.SongName));
                        existing.Add(s);
                        tmp.RemoveAt(i - removed);
                        removed++;
                        break;
                    }
                    i++;
                }
            }

            txtbox.ScrollToEnd();

            bool moveexisting = false;


            // 0 = make new Playlist, 1 = add all songs to existing PL, 2 = delete existing PL and make new one
            int action = 0;
            if (!AutoMode)
            {
                if (existing.Count > 0)
                {
                    MessageBoxResult r = MessageBox.Show(MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.playlistEditor.code.existingSongsFound, existing.Count.ToString(), BPList.songs.Count.ToString(), BPList.playlistTitle, BPList.playlistAuthor), "BMBF Manager - Playlist Editor", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    switch (r)
                    {
                        case MessageBoxResult.Yes:
                            moveexisting = true;
                            break;
                    }
                }

                if (PLExists)
                {
                    MessageBoxResult r = MessageBox.Show(MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.playlistEditor.code.playlistExists, BPList.playlistTitle), "BMBF Manager - Playlist Editor", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
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

                if (PLExists || existing.Count > 0)
                {
                    MessageBoxResult r = MessageBox.Show(MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.playlistEditor.code.bPListImportSummaryPart1, existing.Count.ToString(), (BPList.songs.Count - existing.Count).ToString()) + " " + (action == 0 ? MainWindow.globalLanguage.playlistEditor.code.bPListImportSummaryPart2a : "") + (action == 1 ? MainWindow.globalLanguage.playlistEditor.code.bPListImportSummaryPart2b : "") + (action == 2 ? MainWindow.globalLanguage.playlistEditor.code.bPListImportSummaryPart2c : "") + " " + MainWindow.globalLanguage.playlistEditor.code.bPListImportSummaryPart3, "BMBF Manager - Playlist Editor", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    switch (r)
                    {
                        case MessageBoxResult.No:
                            txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.bPListImportingAborted);
                            txtbox.ScrollToEnd();
                            return;
                    }
                }
            }
            else
            {
                action = 1;
                moveexisting = true;
            }

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
                        txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.playlistEditor.code.hashCouldntBeFoundOnBeatSaver, s.songName, s.hash));
                        txtbox.ScrollToEnd();
                        continue;
                    }
                    SongsWindow.InstallSongPE(r.key);
                    await Task.Delay(1000);
                }

                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.installingSongs);
                txtbox.ScrollToEnd();
                while (waiting)
                {
                    await Task.Delay(1000);
                }
                txtbox.AppendText("\n" + MainWindow.globalLanguage.playlistEditor.code.installedSongs);
                txtbox.ScrollToEnd();

                await Task.Delay(2000);
            }

            TimeoutWebClientShort client = new TimeoutWebClientShort();
            BMBFC current = new BMBFC();

            try
            {
                current = BMBFUtils.GetBMBFConfig();
                //current.AdjustForPLEditing();
            }
            catch
            {
                txtbox.AppendText(MainWindow.globalLanguage.global.BMBF100);
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.aborting);
                txtbox.ScrollToEnd();
                return;
            }
            if (action == 0)
            {
                BMBFPlaylist newpl = new BMBFPlaylist();
                newpl.PlaylistName = BPList.playlistTitle;
                newpl.Init();
                //newpl.CoverImageBytes = BPList.image.Replace("data:image/png;base64,", "").Replace("data:image/jpg;base64,", "").Replace("data:image/jpeg;base64,", "").Replace(",", "");
                if (moveexisting)
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
                                if (search.Hash.ToLower() == s.Hash.ToLower())
                                {
                                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.playlistEditor.code.removedSongFromPlaylist, s.SongName));
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
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.addedExistingToPlaylist);
                }
                txtbox.ScrollToEnd();

                // Move new Songs
                foreach (BMBFSong s in BMBFConfig.Config.GetAllSongs())
                {
                    foreach (BPListSong search in tmp)
                    {
                        if (search.hash.ToLower().Contains(s.Hash.ToLower()))
                        {
                            txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.playlistEditor.code.movedSongToBPList, s.SongName));
                            newpl.SongList.Add(s);
                            break;
                        }
                    }
                }
                txtbox.ScrollToEnd();
                BMBFConfig.Config.Playlists.Add(newpl);
            }
            else if (action == 1)
            {
                int pl = 0;
                foreach (BMBFPlaylist p in BMBFConfig.Config.Playlists)
                {
                    if (p.PlaylistName == BPList.playlistTitle) break;
                    pl++;
                }
                //BMBFConfig.Config.Playlists[pl].CoverImageBytes = BPList.image.Replace("data:image/png;base64,", "").Replace("data:image/jpg;base64,", "").Replace("data:image/jpeg;base64,", "").Replace(",", "");
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
                                if (search.Hash.ToLower() == s.Hash.ToLower())
                                {
                                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.playlistEditor.code.removedSongFromPlaylist, s.SongName));
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
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.addedExistingToPlaylist);
                    txtbox.ScrollToEnd();
                }

                // Move new Songs
                foreach (BMBFSong s in BMBFConfig.Config.GetAllSongs())
                {
                    foreach (BPListSong search in tmp)
                    {
                        if (search.hash.ToLower().Contains(s.Hash.ToLower()))
                        {
                            txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.playlistEditor.code.movedSongToBPList, s.SongName));
                            BMBFConfig.Config.Playlists[pl].SongList.Add(s);
                            break;
                        }
                    }
                }
            }
            else if (action == 2)
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
                newpl.Init();
                //newpl.CoverImageBytes = BPList.image.Replace("data:image/png;base64,", "").Replace("data:image/jpg;base64", "").Replace("data:image/jpeg;base64", "").Replace(",", "");

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
                                if (search.Hash.ToLower() == s.Hash.ToLower())
                                {
                                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.playlistEditor.code.removedSongFromPlaylist, s.SongName));
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
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.addedExistingToPlaylist);
                }

                // Move new Songs
                foreach (BMBFSong s in BMBFConfig.Config.GetAllSongs())
                {
                    foreach (BPListSong search in tmp)
                    {
                        if (search.hash.ToLower().Contains(s.Hash.ToLower()))
                        {
                            txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.playlistEditor.code.movedSongToBPList, s.SongName));
                            newpl.SongList.Add(s);
                            break;
                        }
                    }
                }

                BMBFConfig.Config.Playlists.Add(newpl);
            }

            txtbox.AppendText("\n\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.playlistEditor.code.installedBPList, BPList.playlistTitle, BPList.playlistAuthor));
            txtbox.ScrollToEnd();
            // Update All Lists
            Playlists.Items.Clear();
            foreach (BMBFPlaylist Playlist in BMBFConfig.Config.Playlists)
            {
                Playlists.Items.Add(Playlist.PlaylistName);
            }
            if (Playlists.Items.Count < 1)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.zeroPlaylists);
                txtbox.ScrollToEnd();
                return;
            }
            Playlists.SelectedIndex = 0;
        }

        private void ChaPlC(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = MainWindow.globalLanguage.playlistEditor.code.picture + " (*.png, *.jpg, *.jpeg) | *.png;*.jpg;*.jpeg";

            bool exists = false;
            foreach (BMBFPlaylist p in orgConfig.Config.Playlists) if (p.PlaylistId == BMBFConfig.Config.Playlists[Playlists.SelectedIndex].PlaylistId) exists = true;
            if(!exists)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.pleaseSync);
                txtbox.ScrollToEnd();
                return;
            }

            bool? result = ofd.ShowDialog();
            if (result == true)
            {
                //Get the path of specified file
                if (!File.Exists(ofd.FileName))
                {
                    MessageBox.Show(MainWindow.globalLanguage.playlistEditor.code.selectValidFile, "BMBF Manager - Playlist Editor", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.coverChangingAborted);
                txtbox.ScrollToEnd();
                return;
            }

            //BMBFConfig.Config.Playlists[Playlists.SelectedIndex].CoverImageBytes = Convert.ToBase64String(File.ReadAllBytes(ofd.FileName));
            try
            {
                WebClient c = new WebClient();
                c.UploadFile("http://" + MainWindow.config.IP + ":50000/host/beatsaber/playlist/cover?playlistId=" + BMBFConfig.Config.Playlists[Playlists.SelectedIndex].PlaylistId, ofd.FileName);
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.changedPlaylistCover);
                txtbox.ScrollToEnd();
            } catch
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.coverChangingFailed);
                txtbox.ScrollToEnd();
            }
            int s = Playlists.SelectedIndex;
            Playlists.SelectedIndex = 0;
            Playlists.SelectedIndex = s;
        }

        private void NewPl(object sender, RoutedEventArgs e)
        {
            if (PlaylistName.Text == MainWindow.globalLanguage.playlistEditor.UI.playlistName)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.typeAName);
                txtbox.ScrollToEnd();
                return;
            }
            BMBFPlaylist p = new BMBFPlaylist();
            p.PlaylistName = PlaylistName.Text;
            p.Init();
            BMBFConfig.Config.Playlists.Add(p);

            txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.playlistEditor.code.createdPlaylist, PlaylistName.Text));
            txtbox.ScrollToEnd();

            Playlists.Items.Clear();
            foreach (BMBFPlaylist Playlist in BMBFConfig.Config.Playlists)
            {
                Playlists.Items.Add(Playlist.PlaylistName);
            }
            if (Playlists.Items.Count < 1)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.zeroPlaylists);
                txtbox.ScrollToEnd();
                return;
            }
            Playlists.SelectedIndex = 0;
        }

        private void RenPl(object sender, RoutedEventArgs e)
        {
            if (PlaylistName.Text == MainWindow.globalLanguage.playlistEditor.UI.playlistName)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.typeAName);
                txtbox.ScrollToEnd();
                return;
            }

            txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.playlistEditor.code.renamePlaylist, BMBFConfig.Config.Playlists[Playlists.SelectedIndex].PlaylistName, PlaylistName.Text));
            txtbox.ScrollToEnd();

            BMBFConfig.Config.Playlists[Playlists.SelectedIndex].PlaylistName = PlaylistName.Text;

            Playlists.Items.Clear();
            foreach (BMBFPlaylist Playlist in BMBFConfig.Config.Playlists)
            {
                Playlists.Items.Add(Playlist.PlaylistName);
            }
            if (Playlists.Items.Count < 1)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.zeroPlaylists);
                txtbox.ScrollToEnd();
                return;
            }
            Playlists.SelectedIndex = 0;
        }

        private void SavePlaylists(object sender, RoutedEventArgs e)
        {
            SaveAll();
        }

        public void SaveAll()
        {
            if(!BMBFUtils.PostChangesAndSync(txtbox, JsonSerializer.Serialize(BMBFConfig.Config)))
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.savingFailed);
                txtbox.ScrollToEnd();
                return;
            }
            orgConfig = BMBFConfig;
            txtbox.AppendText("\n\n" + MainWindow.globalLanguage.playlistEditor.code.saved);
            txtbox.ScrollToEnd();
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

        private void ClearTextPN(object sender, RoutedEventArgs e)
        {
            if (PlaylistName.Text == MainWindow.globalLanguage.playlistEditor.UI.playlistName)
            {
                PlaylistName.Text = "";
            }

        }

        private void QuestIPCheck(object sender, RoutedEventArgs e)
        {
            if (Quest.Text == "")
            {
                Quest.Text = MainWindow.globalLanguage.global.defaultQuestIPText;
            }
        }

        private void PNameCheck(object sender, RoutedEventArgs e)
        {
            if (PlaylistName.Text == "")
            {
                PlaylistName.Text = MainWindow.globalLanguage.playlistEditor.UI.playlistName;
            }
        }
    }
}
