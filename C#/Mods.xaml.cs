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
using ModObjects;
using System.Text.Json;
using System.Text.RegularExpressions;
using ComputerUtils.RegxTemplates;
using BMBF.Config;
using BMBFManager.Utils;

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
        List<ModObjects.Mod> AllModList = new List<ModObjects.Mod>();
        List<int> downloadqueue = new List<int>();
        BMBFC BMBF = new BMBFC();
        int C = 0;
        int Index = 0;

        public Mods()
        {
            InitializeComponent();
            ApplyLanguage();
            Quest.Text = MainWindow.config.IP;
            DownloadLable.Text = MainWindow.globalLanguage.global.allFinished;
            getMods();
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
                uniformBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Mods6.png", UriKind.Absolute));
                uniformBrush.Stretch = Stretch.UniformToFill;
                this.Background = uniformBrush;
            }
            MainWindow.DCRPM.SetActivity(MainWindow.globalLanguage.dCRP.browsingMods);
        }

        public void ApplyLanguage()
        {
            ((GridView)ModList.View).Columns[0].Header = MainWindow.globalLanguage.mods.UI.ModNameList; //Name
            ((GridView)ModList.View).Columns[1].Header = MainWindow.globalLanguage.mods.UI.ModCreatorList; //Creator(s)
            ((GridView)ModList.View).Columns[2].Header = MainWindow.globalLanguage.mods.UI.ModInstalledList; //installed
            ((GridView)ModList.View).Columns[3].Header = MainWindow.globalLanguage.mods.UI.ModLatestList; //latest
            ((GridView)ModList.View).Columns[4].Header = MainWindow.globalLanguage.mods.UI.ModGameVersionList; //Game Version
            moreInfoButton.Content = MainWindow.globalLanguage.mods.UI.moreInfoButton;
            updateAllModsButton.Content = MainWindow.globalLanguage.mods.UI.updateAllModsButton;
            installModButton.Content = MainWindow.globalLanguage.mods.UI.installModButton;
            UninstallModButton.Content = MainWindow.globalLanguage.mods.UI.UninstallModButton;
        }

        public void getMods()
        {
            MainWindow.iPUtils.CheckIP(Quest);
            TimeoutWebClientShort client = new TimeoutWebClientShort();

            string json = "";
            Boolean Reaching = true;

            try
            {
                json = client.DownloadString("http://www.questmodding.com/api/mods/");
            }
            catch
            {
                txtbox.AppendText(MainWindow.globalLanguage.global.BM100);
            }

            try
            {
                BMBF = BMBFUtils.GetBMBFConfig();
                MainWindow.config.GameVersion = BMBF.BeatSaberVersion;
            }
            catch
            {
                txtbox.AppendText(MainWindow.globalLanguage.global.BMBF100);
                Reaching = false;
            }
            if (MainWindow.config.GameVersion == null)
            {
                txtbox.AppendText(MainWindow.globalLanguage.mods.code.gameVerNull);
                return;
            }
            String[] GameVersion = MainWindow.config.GameVersion.ToString().Replace("\"", "").Split('.');
            //String[] GameVersion = "1.13.2".Replace("\"", "").Split('.');
            int major = Convert.ToInt32(GameVersion[0]);
            int minor = Convert.ToInt32(GameVersion[1]);
            int patch = Convert.ToInt32(GameVersion[2]);

            ModList QB = new ModList();

            QB = JsonSerializer.Deserialize<ModList>(json);



            WebClient c = new WebClient();

            ModList CE = new ModList();

            //json = JSON.Parse(c.DownloadString("https://raw.githubusercontent.com/ComputerElite/BM/main/testing.json"));
            CE = JsonSerializer.Deserialize<ModList>(c.DownloadString("https://raw.githubusercontent.com/ComputerElite/BM/main/mods.json?nonsensecauseofcaching=" + DateTime.Now));

            ModUtils mu = new ModUtils();
            QB = mu.RemoveIncompatibleMods(QB, String.Join(".", GameVersion), CE);
            CE = mu.RemoveIncompatibleMods(CE, String.Join(".", GameVersion), CE);
            AllModList = mu.MergeModLists(CE, QB, BMBF, major, minor, patch);
            
            ModList.SelectedIndex = 0;
            updatemodlist(false);
            if(!Reaching)
            {
                MessageBox.Show(MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.mods.code.couldntReachBMBFForVersion, MainWindow.config.GameVersion), "BMBF Manager - Mod Installing", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void InstallMod(String ModName)
        {
            int i = 0;
            bool found = false;
            foreach (ModObjects.Mod m in AllModList)
            {
                if (m.name.ToLower() == ModName.ToLower())
                {
                    found = true;
                    downloadqueue.Add(i);
                    checkqueue();
                }
                i++;
            }
            if(!found)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.mods.code.couldntFindMods, ModName));
            }
        }

        private void updatemodlist(Boolean downloadbmbf = true)
        {
            if(downloadbmbf)
            {
                WebClient client = new WebClient();
                try
                {
                    BMBF = BMBFUtils.GetBMBFConfig();
                }
                catch
                {
                    txtbox.AppendText(MainWindow.globalLanguage.global.BMBF100);
                    txtbox.ScrollToEnd();
                }
            }
            ModList.Items.Clear();
            foreach (ModObjects.Mod cmod in AllModList)
            {

                //Name, Version, DownloadLink, Creator, gameVersion, Desciption, Forward, new Tuple (CoreMod, ModID, currentversion)
                if(cmod.downloadable)
                {
                    ModList.Items.Add(new ModItem(cmod.name, String.Join(", ", cmod.creator), cmod.downloads[cmod.MatchingDownload].gameversion[cmod.MatchingGameVersion], cmod.Version, cmod.downloads[cmod.MatchingDownload].modversion, cmod.installed, cmod.downloadable));
                } else
                {
                    ModList.Items.Add(new ModItem(cmod.name, String.Join(", ", cmod.creator), cmod.GameVersion, cmod.Version, "N/A", cmod.installed, cmod.downloadable));
                }
                
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

        private void QuestIPCheck(object sender, RoutedEventArgs e)
        {
            if (Quest.Text == "")
            {
                Quest.Text = MainWindow.globalLanguage.global.defaultQuestIPText;
            }
        }

        public void MoreInfo(object sender, RoutedEventArgs e)
        {
            ShowInfo();
        }

        private void MoreInfoDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ShowInfo();
        }

        public void ShowInfo()
        {
            if (ModList.SelectedIndex < 0 || ModList.SelectedIndex >= AllModList.Count) return;
            MessageBox.Show(MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.mods.code.modInfo, AllModList[ModList.SelectedIndex].name, AllModList[ModList.SelectedIndex].details, AllModList[ModList.SelectedIndex].downloads[AllModList[ModList.SelectedIndex].MatchingDownload].notes), "BMBF Manager - Mod Info", MessageBoxButton.OK);
        }

        public void checkqueue()
        {
            if(downloadqueue.Count != 0)
            {
                InstallMod();
            } else
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.global.allFinished);
                txtbox.ScrollToEnd();
            }
        }

        public void InstallMod()
        {
            if (!MainWindow.iPUtils.CheckIP(Quest))
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

            MainWindow.DCRPM.SetActivity(MainWindow.globalLanguage.dCRP.installingMods);

            C = 0;
            while (File.Exists(exe + "\\tmp\\" + AllModList[downloadqueue[0]].name + C + ".zip"))
            {
                C++;
            }

            Index = downloadqueue[0];

            if (AllModList[Index].downloads[AllModList[Index].MatchingDownload].forward)
            {
                MessageBoxResult result1 = MessageBox.Show(MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.mods.code.manualInstall, AllModList[Index].name), "BMBF Manager - Mod Installing", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result1)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\nMod Installing Aborted.");
                        txtbox.ScrollToEnd();
                        Running = false;
                        downloadqueue.RemoveAt(0);
                        checkqueue();
                        return;
                }
                Process.Start("http://" + MainWindow.config.IP + ":50000/main/upload");
                Process.Start(AllModList[Index].downloads[AllModList[Index].MatchingDownload].download);
                return;
            }

            if (AllModList[Index].downloads[AllModList[Index].MatchingDownload].coremod)
            {
                MessageBox.Show(MainWindow.globalLanguage.mods.code.coreMod, "BMBF Manager - Mod Installing", MessageBoxButton.OK, MessageBoxImage.Warning);
                Process.Start("http://" + MainWindow.config.IP + ":50000/main/mods");
                MessageBoxResult result1 = MessageBox.Show(MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.mods.code.isCoreModInstalled, AllModList[Index].name), "BMBF Manager - Mod Installing", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result1)
                {
                    case MessageBoxResult.Yes:
                        txtbox.AppendText("\n\n" + MainWindow.globalLanguage.mods.code.alreadyInstalledAbort);
                        txtbox.ScrollToEnd();
                        Running = false;
                        downloadqueue.RemoveAt(0);
                        checkqueue();
                        return;
                }
            }

            if (AllModList[Index].downloads[AllModList[Index].MatchingDownload].gameversion[AllModList[Index].MatchingGameVersion] != MainWindow.config.GameVersion)
            {
                MessageBoxResult result1 = MessageBox.Show(MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.mods.code.oldVerInstall, AllModList[Index].name, AllModList[Index].downloads[AllModList[Index].MatchingDownload].gameversion[AllModList[Index].MatchingGameVersion]), "BMBF Manager - Mod Installing", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result1)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\n" + MainWindow.globalLanguage.mods.code.installAborted);
                        txtbox.ScrollToEnd();
                        Running = false;
                        downloadqueue.RemoveAt(0);
                        checkqueue();
                        return;
                }
            }

            String Download = AllModList[Index].downloads[AllModList[Index].MatchingDownload].download;
            WebClient c = new WebClient();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.mods.code.downloadingMod, AllModList[Index].name));
                txtbox.ScrollToEnd();
            }));
            Uri uri = new Uri(Download);
            try
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                {
                    DownloadLable.Text = MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.mods.code.downloadingMod, AllModList[Index].name);
                    c.DownloadFileCompleted += new AsyncCompletedEventHandler(finished_download);
                    c.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                    c.DownloadFileAsync(uri, exe + "\\tmp\\" + AllModList[Index].name + C + ".zip");
                }));
            }
            catch
            {
                txtbox.AppendText(MainWindow.globalLanguage.global.BM200);
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

        public void AddSelectedModToQueue(object sender, RoutedEventArgs e)
        {   
            if(ModList.SelectedIndex < 0 || ModList.SelectedIndex >= ModList.Items.Count)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.mods.code.selectMod);
                return;
            }
            if (downloadqueue.Contains(ModList.SelectedIndex))
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.mods.code.alreadyInQueue, AllModList[ModList.SelectedIndex].name));
                txtbox.ScrollToEnd();
                return;
            }
            if(!AllModList[ModList.SelectedIndex].downloadable)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.mods.code.modNotDownloadable, AllModList[ModList.SelectedIndex].name));
                txtbox.ScrollToEnd();
                return;
            }
            downloadqueue.Add(ModList.SelectedIndex);
            txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.mods.code.addedToQueue, AllModList[ModList.SelectedIndex].name));
            txtbox.ScrollToEnd();
            checkqueue();
        }

        public void UninstallMod(object sender, RoutedEventArgs e)
        {
            if (ModList.SelectedIndex < 0 || ModList.SelectedIndex >= ModList.Items.Count)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.mods.code.selectMod);
                txtbox.ScrollToEnd();
                return;
            }
            if(!AllModList[ModList.SelectedIndex].installed)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.mods.code.modNotInstalled, AllModList[ModList.SelectedIndex].name));
                txtbox.ScrollToEnd();
                return;
            }
            ModObjects.Mod selected = AllModList[ModList.SelectedIndex];
            for(int i = 0; i < BMBF.Config.Mods.Count; i++)
            {
                if(BMBF.Config.Mods[i].Id == selected.ModID)
                {
                    BMBF.Config.Mods.RemoveAt(i);
                    BMBFUtils.PostChangesAndSync(txtbox, JsonSerializer.Serialize(BMBF.Config));
                    AllModList[ModList.SelectedIndex].installed = false;
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.mods.code.uninstalledMod, AllModList[ModList.SelectedIndex].name));
                    txtbox.ScrollToEnd();
                    AllModList.RemoveAt(ModList.SelectedIndex);
                    updatemodlist();
                    return;
                }
            }
            txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.mods.code.modNotInstalled, AllModList[ModList.SelectedIndex].name));
            txtbox.ScrollToEnd();
        }

        public void UpdateMods(object sender, RoutedEventArgs e)
        {
            TimeoutWebClientShort c = new TimeoutWebClientShort();
            try
            {
                BMBF = BMBFUtils.GetBMBFConfig();
            }
            catch
            {
                txtbox.AppendText(MainWindow.globalLanguage.global.BMBF100);
                txtbox.ScrollToEnd();
                return;
            }
            int i = 0;
            foreach(BMBF.Config.Mod Mod in BMBF.Config.Mods)
            {
                i = 0;
                foreach(ModObjects.Mod m in AllModList)
                {
                    if(Mod.Id == m.ModID && m.downloadable)
                    {
                        String ModV = Mod.Version;

                        String ModVD = m.downloads[m.MatchingDownload].modversion;
                        if(new Version(ModV).CompareTo(new Version(ModVD)) == -1)
                        {
                            downloadqueue.Add(i);
                            txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.mods.code.updateAddedToQueue, m.name, m.downloads[m.MatchingDownload].modversion));
                        }
                        break;
                    }
                    i++;
                }
            }
            checkqueue();
        }

        private void finished_download(object sender, AsyncCompletedEventArgs e)
        {
            MainWindow.aDBI.adb("shell am start -n com.weloveoculus.BMBF/com.weloveoculus.BMBF.MainActivity", txtbox);
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                txtbox.AppendText("\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.mods.code.downloadedMod, AllModList[Index].name));
                txtbox.ScrollToEnd();
            }));
            upload(exe + "\\tmp\\" + AllModList[Index].name + C + ".zip");
        }

        public void upload(String path)
        {
            MainWindow.iPUtils.CheckIP(Quest);

            TimeoutWebClient client = new TimeoutWebClient();

            txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.mods.code.uploadingToBMBF, AllModList[Index].name));
            txtbox.ScrollToEnd();
            Uri uri = new Uri("http://" + MainWindow.config.IP + ":50000/host/beatsaber/upload?overwrite");
            try
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                {
                    DownloadLable.Text = MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.mods.code.uploadingToBMBF, AllModList[Index].name);
                    client.UploadProgressChanged += new UploadProgressChangedEventHandler(client_uploadchanged);
                    client.UploadFileCompleted += new UploadFileCompletedEventHandler(finished_upload);
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

        private void finished_upload(object sender, AsyncCompletedEventArgs e)
        {
            if (AllModList[Index].downloads[AllModList[Index].MatchingDownload].gameversion[AllModList[Index].MatchingGameVersion] == MainWindow.config.GameVersion)
            {
                try
                {
                    if (!BMBFUtils.Sync(txtbox)) throw new Exception();
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.mods.code.syncedToQuest, AllModList[Index].name));
                    txtbox.ScrollToEnd();
                }
                catch
                {
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.mods.code.unableToSync);
                    txtbox.ScrollToEnd();
                }
            }
            else
            {
                Process.Start("http://" + MainWindow.config.IP + ":50000/main/mods");
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.mods.code.enableManually);
                txtbox.ScrollToEnd();
            }
            Running = false;
            DownloadLable.Text = MainWindow.globalLanguage.global.allFinished;
            Progress.Value = 0;
            AllModList[downloadqueue[0]].installed = true;
            downloadqueue.RemoveAt(0);

            updatemodlist();
            checkqueue();
            return;
        }
    }

    public class ModItem
    {
        public string Name { get; set; }

        public string Creator { get; set; }

        public string GameVersion { get; set; }

        public string ModVersion { get; set; }

        public string latest { get; set; }

        public System.Windows.Media.SolidColorBrush Color { get; set; }
        public ModItem(String Name, String Creator, String GameVersion, String ModVersion, String latest, bool installed, bool downloadable)
        {
            this.Name = Name;
            this.Creator = Creator;
            this.GameVersion = GameVersion;
            this.latest = latest;
            
            SolidColorBrush newColor = new SolidColorBrush();
            
            if (!installed)
            {
                this.ModVersion = "N/A";
                newColor.Color = Colors.Yellow;
            } else
            {
                this.ModVersion = ModVersion;
                if(!downloadable)
                {
                    newColor.Color = Colors.Yellow;
                }
                else if (new Version(ModVersion).CompareTo(new Version(latest)) != -1)
                {
                    newColor.Color = Colors.Green;
                }
                else
                {
                    newColor.Color = Colors.Red;
                }
            }
            this.Color = newColor;
        }
    }
}


