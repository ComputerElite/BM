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

        public Support()
        {
            InitializeComponent();
            Quest.Text = MainWindow.IP;
            if(MainWindow.CustomProtocols)
            {
                CustomP.Content = "Disable BM Custom Protocol";
            } else
            {
                CustomP.Content = "Enable BM Custom Protocol";
            }
            if (MainWindow.OneClick)
            {
                BSaver.Content = "Disable BeatSaver OneClick install";
            }
            else
            {
                BSaver.Content = "Enable BeatSaver OneClick install";
            }
            UpdateImage();
            if(MainWindow.ShowADB)
            {
                ADB.Content = "Disable ADB output";
            } else
            {
                ADB.Content = "Enable ADB output";
            }
        }

        private void KeepAlive(object sender, RoutedEventArgs e)
        {
            if(MainWindow.KeepAlive)
            {
                MainWindow.KeepAlive = false;
                txtbox.AppendText("\n\nKeep Alive has been disabled.");

            } else
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to enable Keep Alive? That will result in your Quest not going to sleep until the program get's closed.\nThis will only work as long as your Quest is reachable via ADB (connected via cable)\nHightly recommended for Quest 2 Users", "BMBF Manager - Settings", MessageBoxButton.YesNo, MessageBoxImage.Information);
                switch (result)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\nAborted.");
                        txtbox.ScrollToEnd();
                        Running = false;
                        return;
                }
                MainWindow.KeepAlive = true;
                txtbox.AppendText("\n\nKeep Alive has been enabled.");
            }
        }

        private void ADBshow(object sender, RoutedEventArgs e)
        {
            if(MainWindow.ShowADB)
            {
                //Disable
                MainWindow.ShowADB = false;
                txtbox.AppendText("\n\nADB output disabled.");
                ADB.Content = "Enable ADB output";
            } else
            {
                //enable
                MessageBoxResult result = MessageBox.Show("Are you sure you want to enable ADB output? I won't check if your Quest is connected anymore and you will be able to pause the adb process when you click it.\nDo you really want to enable ADB output?", "BMBF Manager - Settings", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\nAborted.");
                        txtbox.ScrollToEnd();
                        Running = false;
                        return;
                }
                MainWindow.ShowADB = true;
                txtbox.AppendText("\n\nADB output enabled.");
                ADB.Content = "Disable ADB output";
            }
        }

        private void ChooseImage(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Pictures (*.jpg, *.png, *.bmp, *.img, *.tif, *.tiff, *.webp)|*.jpg;*.png;*.bmp;*.img;*.tif;*.tiff;*.webp";
            ofd.Multiselect = false;
            bool? result = ofd.ShowDialog();
            if (result == true)
            {
                //Get the path of specified file
                if (File.Exists(ofd.FileName))
                {
                    MainWindow.CustomImageSource = ofd.FileName;
                    MainWindow.CustomImage = true;
                    UpdateImage();
                    txtbox.AppendText("\n\nFor the changes to take effect program wide you have to restart it.");
                }
                else
                {
                    MessageBox.Show("Please select a valid file", "BMBF Manager - Settings", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

            }
        }

        private void ResetImage(object sender, RoutedEventArgs e)
        {
            MainWindow.CustomImage = false;
            UpdateImage();
        }

        private void UpdateImage()
        {
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
                uniformBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Support5.png", UriKind.Absolute));
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

        private void QuestIPCheck(object sender, RoutedEventArgs e)
        {
            if (Quest.Text == "")
            {
                Quest.Text = "Quest IP";
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

        private void EnableBBBUMove(object sender, RoutedEventArgs e)
        {
            MainWindow.BBBUTransfered = false;
            BBBU BBBUWindow = new BBBU();
            BBBUWindow.Show();
        }

        private void EnableQSUMove(object sender, RoutedEventArgs e)
        {
            MainWindow.QSUTransfered = false;
            QSU QSUWindow = new QSU();
            QSUWindow.Show();
        }

        private void EnableCustom(object sender, RoutedEventArgs e)
        {
            if(!MainWindow.CustomProtocols)
            {
                txtbox.AppendText("\n\nChanging Registry to enable BM Custom protocols");
                String regFile = "Windows Registry Editor Version 5.00\n\n[HKEY_CLASSES_ROOT\\bm]\n@=\"URL: bm\"\n\"URL Protocol\"=\"bm\"\n\n[HKEY_CLASSES_ROOT\\bm]\n@=\"" + System.Reflection.Assembly.GetEntryAssembly().Location.Replace("\\", "\\\\") + "\"\n\n[HKEY_CLASSES_ROOT\\bm\\shell]\n\n[HKEY_CLASSES_ROOT\\bm\\shell\\open]\n\n[HKEY_CLASSES_ROOT\\bm\\shell\\open\\command]\n@=\"" + System.Reflection.Assembly.GetEntryAssembly().Location.Replace("\\", "\\\\") + " \\\"%1\\\"\"";
                File.WriteAllText(exe + "\\registry.reg", regFile);
                try
                {
                    Process.Start(exe + "\\registry.reg");
                    txtbox.AppendText("\n\nCustom Links enabled");
                }
                catch
                {
                    txtbox.AppendText("\n\nRegistry was unable to change... no Custom protocol enabled.");
                    return;
                }
                CustomP.Content = "Disable BM Custom Protocol";
                MainWindow.CustomProtocols = true;
            } else
            {
                txtbox.AppendText("\n\nChanging Registry to disable BM Custom protocols");
                String regFile = "Windows Registry Editor Version 5.00\n\n[-HKEY_CLASSES_ROOT\\bm]";
                File.WriteAllText(exe + "\\registry.reg", regFile);
                try
                {
                    Process.Start(exe + "\\registry.reg");
                    txtbox.AppendText("\n\nCustom Links disabled");
                }
                catch
                {
                    txtbox.AppendText("\n\nRegistry was unable to change.");
                    return;
                }
                CustomP.Content = "Enable BM Custom Protocol";
                MainWindow.CustomProtocols = false;
            }
        }

        public void enable_BeatSaver(object sender, RoutedEventArgs e)
        {
            if (!MainWindow.OneClick)
            {
                MessageBoxResult result = MessageBox.Show("This will disable OneClick Install via Mod Assistent.\nDo you wish to continue?", "BMBF Manager - Settings", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\nOneClick Install enabeling aborted");
                        Running = false;
                        txtbox.ScrollToEnd();
                        return;
                }
                txtbox.AppendText("\n\nChanging Registry to enable OneClick Custom protocols");
                String regFile = "Windows Registry Editor Version 5.00\n\n[HKEY_CLASSES_ROOT\\beatsaver]\n@=\"URL: beatsaver\"\n\"URL Protocol\"=\"beatsaver\"\n\n[HKEY_CLASSES_ROOT\\beatsaver]\n@=\"" + System.Reflection.Assembly.GetEntryAssembly().Location.Replace("\\", "\\\\") + "\"\n\n[HKEY_CLASSES_ROOT\\beatsaver\\shell]\n\n[HKEY_CLASSES_ROOT\\beatsaver\\shell\\open]\n\n[HKEY_CLASSES_ROOT\\beatsaver\\shell\\open\\command]\n@=\"" + System.Reflection.Assembly.GetEntryAssembly().Location.Replace("\\", "\\\\") + " \\\"%1\\\"\"";
                File.WriteAllText(exe + "\\registry.reg", regFile);
                try
                {
                    Process.Start(exe + "\\registry.reg");
                    txtbox.AppendText("\n\nOneClick Install via BeatSaver enabled");
                }
                catch
                {
                    txtbox.AppendText("\n\nRegistry was unable to change... no Custom protocol disabled.");
                    return;
                }
                BSaver.Content = "Disable BeatSaver OneClick install";
                MainWindow.OneClick = true;
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("This will disable OneClick Install via BMBF Manager.\nDo you wish to continue?", "BMBF manager - Settings", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\nOneClick disabeling enabeling aborted");
                        Running = false;
                        txtbox.ScrollToEnd();
                        return;
                }
                txtbox.AppendText("\n\nChanging Registry to disable OneClick Custom protocols");
                String regFile = "Windows Registry Editor Version 5.00\n\n[-HKEY_CLASSES_ROOT\\beatsaver]";
                File.WriteAllText(exe + "\\registry.reg", regFile);
                try
                {
                    Process.Start(exe + "\\registry.reg");
                    txtbox.AppendText("\n\nOneClick Install via BeatSaver disabled");
                }
                catch
                {
                    txtbox.AppendText("\n\nRegistry was unable to change.");
                    return;
                }
                BSaver.Content = "Enable BeatSaver OneClick install";
                MainWindow.OneClick = false;
            }

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

        public void RestorePlaylists()
        {
            System.Threading.Thread.Sleep(5000);
            try
            {
                WebClient client3 = new WebClient();

                String Playlists = exe + "\\tmp\\Playlists.json";

                var j = JSON.Parse(client3.DownloadString("http://" + MainWindow.IP + ":50000/host/beatsaber/config"));
                var p = JSON.Parse(File.ReadAllText(Playlists));

                j["Config"]["Playlists"] = p["Playlists"];
                File.WriteAllText(exe + "\\tmp\\FUCKINBMBF.json", j["Config"].ToString());
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                    postChanges(exe + "\\tmp\\FUCKINBMBF.json");
                }));
            } catch
            {
                txtbox.AppendText("\n\n\nAn error occured (Code: PL100). Check following:");
                txtbox.AppendText("\n\n- You put in the Quests IP right.");
                txtbox.AppendText("\n\n- Your Quest is on.");
            }
        }

        public void resetassets()
        {
            System.Threading.Thread.Sleep(3000);
            TimeoutWebClient client = new TimeoutWebClient();
            client.UploadData("http://" + MainWindow.IP + ":50000/host/mod/resetassets", "POST", new byte[0]);
        }

        public void reloadsongsfolder()
        {
            System.Threading.Thread.Sleep(3000);
            TimeoutWebClient client = new TimeoutWebClient();
            client.QueryString.Add("foo", "foo");
            client.UploadValues("http://" + MainWindow.IP + ":50000/host/beatsaber/reloadsongfolders", "POST", client.QueryString);
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

        public void Sync()
        {
            System.Threading.Thread.Sleep(2000);
            using (TimeoutWebClient client = new TimeoutWebClient())
            {
                client.QueryString.Add("foo", "foo");
                client.UploadValues("http://" + MainWindow.IP + ":50000/host/beatsaber/commitconfig", "POST", client.QueryString);
            }
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
                            txtbox.AppendText("\n\n\nAn error Occured (Code: ADB110). Check following");
                            txtbox.AppendText("\n\n- Your Quest is connected, Developer Mode enabled and USB Debugging enabled.");
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
            txtbox.AppendText("\n\n\nAn error Occured (Code: ADB100). Check following not");
            txtbox.AppendText("\n\n- You have adb installed.");
            txtbox.ScrollToEnd();
            return "Error";
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

        public async void StartSupport(String Link)
        {
            String section = Link.Replace("bm://", "").Replace("%20", " ").ToLower();
            if(Link.ToLower().StartsWith("beatsaver://"))
            {
                String bsr = section.Replace("beatsaver://", "").ToLower();
                Songs s = new Songs();
                s.Show();
                s.InstallSong(bsr);
                this.Close();
            }
            else if(section.StartsWith("support/resetassets"))
            {
                BackupPlaylists();
                resetassets();
                reloadsongsfolder();
                RestorePlaylists();
                this.Close();
            } else if(section.StartsWith("support/quickfix"))
            {
                BackupPlaylists();
                resetassets();
                reloadsongsfolder();
                Sync();
                RestorePlaylists();
                this.Close();
            } else if(section.StartsWith("mods/install/"))
            {

                String ModName = section.Replace("mods/install/", "");
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
                MessageBoxResult result = MessageBox.Show("You have clicked a link to Update/Install BMBF\nDo you wish to continue", "BMBF Manager - BMBF Updater", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\nAborted.");
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
                MessageBoxResult result = MessageBox.Show("You have clicked a link to switch from the modded/unmodded to the unmodded/modded version of Beatsaber.\nDo you wish to continue", "BMBF Manager - Version Switcher", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result)
                {
                    case MessageBoxResult.No:
                        txtbox.AppendText("\n\nAborted.");
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
