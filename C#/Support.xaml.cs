using Microsoft.Win32;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
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
using BeatSaverAPI;
using System.Text.RegularExpressions;
using System.Text.Json;
using ComputerUtils.RegxTemplates;
using BMBFManager.Utils;

namespace BMBF_Manager
{
    /// <summary>
    /// Interaktionslogik für Support.xaml
    /// </summary>
    public partial class Support : Window
    {
        /////////////////    Settings Now!!!!
        Boolean draggable = true;
        Boolean Running = false;
        String exe = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 1);
        Boolean ForThisVersion = true;
        List<Tuple<String, String, String, String, String, String, Boolean, Tuple<bool, String, String>>> AllModsList = new List<Tuple<String, String, String, String, String, String, Boolean, Tuple<bool, String, String>>>();
        BeatSaverAPIInteractor interactor = new BeatSaverAPIInteractor();
        bool languageMessageBlock = false;

        public Support()
        {
            InitializeComponent();
            ApplyLanguage();
            LoadLanguages();
            Quest.Text = MainWindow.config.IP;
            UpdateImage();
            MainWindow.DCRPM.SetActivity(MainWindow.globalLanguage.dCRP.changingSettings);
        }

        public void ApplyLanguage()
        {
            backgroundButton.Content = MainWindow.globalLanguage.settings.UI.backgroundButton;
            resetBackgroundButton.Content = MainWindow.globalLanguage.settings.UI.resetBackgroundButton;
            KA.Content = MainWindow.globalLanguage.settings.UI.KeepAliveButton;
            moveBBBUButton.Content = MainWindow.globalLanguage.settings.UI.moveBBBUButton;
            moveQSUButton.Content = MainWindow.globalLanguage.settings.UI.moveQSUButton;
            KA.Content = MainWindow.globalLanguage.settings.UI.KeepAliveButton;
            CustomP.Content = MainWindow.config.CustomProtocols ? MainWindow.globalLanguage.settings.UI.disableBMButton : MainWindow.globalLanguage.settings.UI.enableBMButton;
            BSaver.Content = MainWindow.config.OneClick ? MainWindow.globalLanguage.settings.UI.disableBSButton : MainWindow.globalLanguage.settings.UI.enableBSButton;
            ADB.Content = MainWindow.config.ShowADB ? MainWindow.globalLanguage.settings.UI.disableADBOutputButton : MainWindow.globalLanguage.settings.UI.enableADBOutputButton;
            CreditsButton.Content = MainWindow.globalLanguage.settings.UI.CreditsButton;
            DCRPB.Content = MainWindow.config.DCRPE ? MainWindow.globalLanguage.settings.UI.disableDCRP : MainWindow.globalLanguage.settings.UI.enableDCRP;
        }

        private void ShowCredits(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/ComputerElite/wiki/wiki/BM#credits");
        }

        public void LoadLanguages()
        {
            languageMessageBlock = true;
            Language.Items.Clear();
            Language.Items.Add(MainWindow.globalLanguage.settings.code.language);
            Language.Items.Add("English");
            Language.SelectedIndex = 0;
            foreach (String file in Directory.GetFiles(exe + "\\languages"))
            {
                if (file.EndsWith(".json")) Language.Items.Add(System.IO.Path.GetFileNameWithoutExtension(file));
            }
            languageMessageBlock = false;
        }

        private void ChangeLanguage(object sender, SelectionChangedEventArgs e)
        {
            if (languageMessageBlock) return;
            if (Language.SelectedIndex == -1) return;
            if(Language.SelectedIndex == 0)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.settings.code.selectLanguage);
                return;
            }
            if (Language.SelectedIndex == 1)
            {
                MainWindow.globalLanguage = new BMBFManager.Language.Language();
                MainWindow.config.language = "en";
                MainWindow.globalLanguage.translator = "ComputerElite";
            }
            else
            {
                MainWindow.globalLanguage = JsonSerializer.Deserialize<BMBFManager.Language.Language>(File.ReadAllText(exe + "\\languages\\" + Language.SelectedItem + ".json"));
                MainWindow.config.language = exe + "\\languages\\" + Language.SelectedItem + ".json";
            }
            ApplyLanguage();
            txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.settings.code.changedLanguage, MainWindow.globalLanguage.language, MainWindow.globalLanguage.translator));
            txtbox.AppendText("\n\n" + MainWindow.globalLanguage.settings.code.restartProgram);
            LoadLanguages();
        }

        private void ToggleDCRP(object sender, RoutedEventArgs e)
        {
            MainWindow.config.DCRPE = !MainWindow.config.DCRPE;
            if (MainWindow.config.DCRPE) txtbox.AppendText("\n\n" + MainWindow.globalLanguage.settings.code.dCRPEnabled + "\n\n" + MainWindow.globalLanguage.settings.code.restartProgram);
            else txtbox.AppendText("\n\n" + MainWindow.globalLanguage.settings.code.dCRPDisabled + "\n\n" + MainWindow.globalLanguage.settings.code.restartProgram);
            DCRPB.Content = MainWindow.config.DCRPE ? MainWindow.globalLanguage.settings.UI.disableDCRP : MainWindow.globalLanguage.settings.UI.enableDCRP;
        }

        private void KeepAlive(object sender, RoutedEventArgs e)
        {
            if(MainWindow.config.KeepAlive)
            {
                MainWindow.config.KeepAlive = false;
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.settings.code.keepAliveDisabled);

            } else
            {
                MessageBoxResult result = MessageBox.Show(MainWindow.globalLanguage.settings.code.keepAliveWarning, "BMBF Manager - Settings", MessageBoxButton.YesNo, MessageBoxImage.Information);
                switch (result)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\n" + MainWindow.globalLanguage.settings.code.aborted);
                        txtbox.ScrollToEnd();
                        Running = false;
                        return;
                }
                MainWindow.config.KeepAlive = true;
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.settings.code.keepAliveEnabled);
            }
        }

        private void ADBshow(object sender, RoutedEventArgs e)
        {
            if(MainWindow.config.ShowADB)
            {
                //Disable
                MainWindow.config.ShowADB = false;
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.settings.code.aDBOutputDisabled);
                ADB.Content = MainWindow.globalLanguage.settings.UI.enableADBOutputButton;
            } else
            {
                //enable
                MessageBoxResult result = MessageBox.Show(MainWindow.globalLanguage.settings.code.aDBOutputWarning, "BMBF Manager - Settings", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\n" + MainWindow.globalLanguage.settings.code.aborted);
                        txtbox.ScrollToEnd();
                        Running = false;
                        return;
                }
                MainWindow.config.ShowADB = true;
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.settings.code.aDBOutputEnabled);
                ADB.Content = MainWindow.globalLanguage.settings.UI.disableADBOutputButton;
            }
        }

        private void ChooseImage(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = MainWindow.globalLanguage.settings.code.pictures + " (*.jpg, *.png, *.bmp, *.img, *.tif, *.tiff, *.webp)|*.jpg;*.png;*.bmp;*.img;*.tif;*.tiff;*.webp";
            ofd.Multiselect = false;
            bool? result = ofd.ShowDialog();
            if (result == true)
            {
                //Get the path of specified file
                if (File.Exists(ofd.FileName))
                {
                    MainWindow.config.CustomImageSource = ofd.FileName;
                    MainWindow.config.CustomImage = true;
                    UpdateImage();
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.settings.code.restartProgram);
                }
                else
                {
                    MessageBox.Show(MainWindow.globalLanguage.settings.code.selectFile, "BMBF Manager - Settings", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

            }
        }

        private void ResetImage(object sender, RoutedEventArgs e)
        {
            MainWindow.config.CustomImage = false;
            UpdateImage();
        }

        private void UpdateImage()
        {
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
                uniformBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Support8.png", UriKind.Absolute));
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

        private void EnableBBBUMove(object sender, RoutedEventArgs e)
        {
            MainWindow.config.BBBUTransfered = false;
            BBBU BBBUWindow = new BBBU();
            BBBUWindow.Show();
        }

        private void EnableQSUMove(object sender, RoutedEventArgs e)
        {
            MainWindow.config.QSUTransfered = false;
            QSU QSUWindow = new QSU();
            QSUWindow.Show();
        }

        private void EnableCustom(object sender, RoutedEventArgs e)
        {
            if(!MainWindow.config.CustomProtocols)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.settings.code.changingRegistryEnableBM);
                String regFile = "Windows Registry Editor Version 5.00\n\n[HKEY_CLASSES_ROOT\\bm]\n@=\"URL: bm\"\n\"URL Protocol\"=\"bm\"\n\n[HKEY_CLASSES_ROOT\\bm]\n@=\"" + System.Reflection.Assembly.GetEntryAssembly().Location.Replace("\\", "\\\\") + "\"\n\n[HKEY_CLASSES_ROOT\\bm\\shell]\n\n[HKEY_CLASSES_ROOT\\bm\\shell\\open]\n\n[HKEY_CLASSES_ROOT\\bm\\shell\\open\\command]\n@=\"" + System.Reflection.Assembly.GetEntryAssembly().Location.Replace("\\", "\\\\") + " \\\"%1\\\"\"";
                File.WriteAllText(exe + "\\registry.reg", regFile);
                try
                {
                    Process.Start(exe + "\\registry.reg");
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.settings.code.customLinksEnabled);
                }
                catch
                {
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.settings.code.registryUnableToChangeNoBM);
                    return;
                }
                CustomP.Content = MainWindow.globalLanguage.settings.UI.disableBMButton;
                MainWindow.config.CustomProtocols = true;
            } else
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.settings.code.changingRegistryDisableBM);
                String regFile = "Windows Registry Editor Version 5.00\n\n[-HKEY_CLASSES_ROOT\\bm]";
                File.WriteAllText(exe + "\\registry.reg", regFile);
                try
                {
                    Process.Start(exe + "\\registry.reg");
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.settings.code.customLinksDisabled);
                }
                catch
                {
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.settings.code.registryUnableToChange);
                    return;
                }
                CustomP.Content = MainWindow.globalLanguage.settings.UI.enableBMButton;
                MainWindow.config.CustomProtocols = false;
            }
        }

        public void enable_BeatSaver(object sender, RoutedEventArgs e)
        {
            if (!MainWindow.config.OneClick)
            {
                MessageBoxResult result = MessageBox.Show(MainWindow.globalLanguage.settings.code.oneClickWarning, "BMBF Manager - Settings", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\n" + MainWindow.globalLanguage.settings.code.oneClickAborted);
                        Running = false;
                        txtbox.ScrollToEnd();
                        return;
                }
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.settings.code.changingRegistryEnableBS);
                String regFile = "Windows Registry Editor Version 5.00\n\n[HKEY_CLASSES_ROOT\\beatsaver]\n@=\"URL: beatsaver\"\n\"URL Protocol\"=\"beatsaver\"\n\n[HKEY_CLASSES_ROOT\\beatsaver]\n@=\"" + System.Reflection.Assembly.GetEntryAssembly().Location.Replace("\\", "\\\\") + "\"\n\n[HKEY_CLASSES_ROOT\\beatsaver\\shell]\n\n[HKEY_CLASSES_ROOT\\beatsaver\\shell\\open]\n\n[HKEY_CLASSES_ROOT\\beatsaver\\shell\\open\\command]\n@=\"" + System.Reflection.Assembly.GetEntryAssembly().Location.Replace("\\", "\\\\") + " \\\"%1\\\"\"";
                File.WriteAllText(exe + "\\registry.reg", regFile);
                try
                {
                    Process.Start(exe + "\\registry.reg");
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.settings.code.oneClickEnabled);
                }
                catch
                {
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.settings.code.registryUnableToChangeNoBM);
                    return;
                }
                BSaver.Content = MainWindow.globalLanguage.settings.UI.disableBSButton;
                MainWindow.config.OneClick = true;
            }
            else
            {
                MessageBoxResult result = MessageBox.Show(MainWindow.globalLanguage.settings.code.oneClickDisableWarning, "BMBF manager - Settings", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\n" + MainWindow.globalLanguage.settings.code.oneClickDisablingAborted);
                        Running = false;
                        txtbox.ScrollToEnd();
                        return;
                }
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.settings.code.changingRegistryDisableBS);
                String regFile = "Windows Registry Editor Version 5.00\n\n[-HKEY_CLASSES_ROOT\\beatsaver]";
                File.WriteAllText(exe + "\\registry.reg", regFile);
                try
                {
                    Process.Start(exe + "\\registry.reg");
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.settings.code.oneClickDisabled);
                }
                catch
                {
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.settings.code.registryUnableToChange);
                    return;
                }
                BSaver.Content = MainWindow.globalLanguage.settings.UI.enableBSButton;
                MainWindow.config.OneClick = false;
            }

        }

        public void BackupPlaylists()
        {
            try
            {
                BMBFUtils.Sync(txtbox);
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.mainMenu.code.playlistBackup, "\\tmp\\Playlists.json"));
                txtbox.ScrollToEnd();
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                String BMBF = "";
                using (TimeoutWebClient client2 = new TimeoutWebClient())
                {
                    BMBF = client2.DownloadString("http://" + MainWindow.config.IP + ":50000/host/beatsaber/config");
                }
                var json = JSON.Parse(BMBF);
                json["IsCommitted"] = false;
                File.WriteAllText(exe + "\\tmp\\Config.json", json.ToString());

                String Config = exe + "\\tmp\\config.json";

                var j = JSON.Parse(File.ReadAllText(Config));
                File.WriteAllText(exe + "\\tmp\\Playlists.json", j["Config"].ToString());
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.processer.ReturnProcessed(MainWindow.globalLanguage.mainMenu.code.playlistBackupFinished, "\\tmp\\Playlists.json"));
                txtbox.ScrollToEnd();
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
            }
            catch
            {
                txtbox.AppendText(MainWindow.globalLanguage.global.PL100);

            }
        }

        public void RestorePlaylists()
        {
            System.Threading.Thread.Sleep(5000);
            try
            {
                WebClient client3 = new WebClient();

                String Playlists = exe + "\\tmp\\Playlists.json";

                var j = JSON.Parse(client3.DownloadString("http://" + MainWindow.config.IP + ":50000/host/beatsaber/config"));
                var p = JSON.Parse(File.ReadAllText(Playlists));

                j["Config"]["Playlists"] = p["Playlists"];
                BMBFUtils.PostChangesAndSync(txtbox, j["Config"].ToString());
            } catch
            {
                txtbox.AppendText(MainWindow.globalLanguage.global.PL100);
            }
        }

        public void resetassets()
        {
            System.Threading.Thread.Sleep(3000);
            TimeoutWebClient client = new TimeoutWebClient();
            client.UploadData("http://" + MainWindow.config.IP + ":50000/host/mod/resetassets", "POST", new byte[0]);
        }

        public void reloadsongsfolder()
        {
            System.Threading.Thread.Sleep(3000);
            TimeoutWebClient client = new TimeoutWebClient();
            client.QueryString.Add("foo", "foo");
            client.UploadValues("http://" + MainWindow.config.IP + ":50000/host/beatsaber/reloadsongfolders", "POST", client.QueryString);
        }

        public async void StartSupport(String Link)
        {
            String section = Link.Replace("bm://", "").Replace("%20", " ").ToLower();
            if(Link.ToLower().StartsWith("beatsaver://"))
            {
                String bsr = section.Replace("beatsaver://", "").Replace("/", "").ToLower();
                Songs s = new Songs();
                s.Show();
                s.InstallSong(bsr);
                this.Close();
            }
            else if(section.StartsWith("support/quickfix"))
            {
                BackupPlaylists();
                resetassets();
                reloadsongsfolder();
                BMBFUtils.Sync(txtbox);
                RestorePlaylists();
                this.Close();
            } else if(section.StartsWith("mods/install/"))
            {

                String ModName = section.Replace("mods/install/", "").Replace("/", "");
                Mods m = new Mods();
                m.Show();
                m.InstallMod(ModName);
                this.Close();
            } else if(section.StartsWith("songs/install/"))
            {
                String bsr = section.Replace("songs/install/", "").ToLower();
                Songs s = new Songs();
                s.Show();
                s.InstallSong(bsr);
                this.Close();
            } else if(section.StartsWith("bbbu/backup/"))
            {
                String Name = section.Replace("bbbu/backup/", "");
                BBBU BBBU = new BBBU();
                BBBU.Show();
                BBBU.BackupLink(Name);
                this.Close();
            }
            else if (section.StartsWith("bbbu/abackup/"))
            {
                String Name = section.Replace("bbbu/abackup/", "");
                BBBU BBBU = new BBBU();
                BBBU.Show();
                BBBU.BackupLink(Name);
                this.Close();
            }
            else if (section.StartsWith("bbbu/restore"))
            {
                String Name = section.Replace("bbbu/restore/", "");
                BBBU BBBU = new BBBU();
                BBBU.Show();
                BBBU.selectBackup(Name);
                this.Close();
            } else if(section.StartsWith("update"))
            {
                MessageBoxResult result = MessageBox.Show(MainWindow.globalLanguage.settings.code.updateBMBFWarning, "BMBF Manager - BMBF Updater", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\n" + MainWindow.globalLanguage.settings.code.aborted);
                        txtbox.ScrollToEnd();
                        Running = false;
                        return;
                }
                MainWindow m = new MainWindow();
                m.Show();
                m.StartBMBFUpdate();
                this.Close();
            } else if(section.StartsWith("switchversion"))
            {
                MessageBoxResult result = MessageBox.Show(MainWindow.globalLanguage.settings.code.switchWarning, "BMBF Manager - Version Switcher", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\n" + MainWindow.globalLanguage.settings.code.aborted);
                        txtbox.ScrollToEnd();
                        Running = false;
                        return;
                }
                MainWindow m = new MainWindow();
                m.Show();
                m.StartVersionSwitch();
                this.Close();
            }
        }
    }
}
