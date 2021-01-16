using Microsoft.Win32;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaktionslogik für HitSounds.xaml
    /// </summary>
    public partial class HitSounds : Window
    {

        Boolean draggable = true;
        String exe = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 1);

        String SelectedSound = "Nothing";


        public HitSounds()
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
                uniformBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/HitSound3.png", UriKind.Absolute));
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
            txtbox.AppendText(MainWindow.ADB100);
            txtbox.ScrollToEnd();
            return false;
        }

        private void Choose(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Sound Files (*.mp3, *.ogg, *.wav)|*.mp3;*.ogg;*.wav";
            bool? result = ofd.ShowDialog();
            if (result == true)
            {
                //Get the path of specified file
                if(File.Exists(ofd.FileName))
                {
                    SelectedSound = ofd.FileName;
                    Sound.Text = SelectedSound;
                } else
                {
                    SelectedSound = "Nothing";
                    MessageBox.Show("Please select a valid file", "BMBF Manager - HitSound installing", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                
            }
        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            txtbox.AppendText("\n\nChanging Sound to default");
            if (!adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/mod_cfgs/QuestSounds.json \"" + exe + "\\tmp\\QSounds.json\"")) return;
            if (!File.Exists(exe + "\\tmp\\QSounds.json"))
            {
                txtbox.AppendText("\n\nDo you have your Quest plugged into your PC? Do you have the QuestSounds mod installed? I was unable to change the config");
                return;
            }
            JSONNode config = JSON.Parse(File.ReadAllText(exe + "\\tmp\\QSounds.json"));
            if ((bool)GoodHitSound.IsChecked)
            {
                if (!adb("shell rm -f " + config["Sounds"]["HitSound"]["filepath"])) return;
            }
            else if ((bool)BadHitSounds.IsChecked)
            {
                if (!adb("shell rm -f " + config["Sounds"]["BadHitSound"]["filepath"])) return;
            }
            else if ((bool)MenuMusic.IsChecked)
            {
                if (!adb("shell rm -f " + config["Sounds"]["MenuMusic"]["filepath"])) return;
            }
            else if ((bool)MenuClickSound.IsChecked)
            {
                if (!adb("shell rm -f " + config["Sounds"]["MenuClick"]["filepath"])) return;
            }
            else if ((bool)FireWorks.IsChecked)
            {
                if (!adb("shell rm -f " + config["Sounds"]["Firework"]["filepath"])) return;
            }
            else if ((bool)LevelCleared.IsChecked)
            {
                if (!adb("shell rm -f " + config["Sounds"]["LevelCleared"]["filepath"])) return;
            }
            else
            {
                txtbox.AppendText("\n\nPlease choose a Sound Type.");
                return;
            }
            txtbox.AppendText("\nChanged Sound to default");
        }

        private void Install(object sender, RoutedEventArgs e)
        {
            if (!CheckIP()) return;
            if(SelectedSound == "Nothing")
            {
                txtbox.AppendText("\n\nPlease select a sound");
                return;
            }
            JSONNode BMBF = JSON.Parse("{}");

            //Check if QuestSoudns is installed
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                txtbox.AppendText("\nChecking if QuestSoudns is installed.");
            }));
            try
            {
                WebClient c = new WebClient();
                BMBF = JSON.Parse(c.DownloadString("http://" + MainWindow.IP + ":50000/host/beatsaber/config"));
                Boolean Installed = false;
                foreach(JSONNode mod in BMBF["Config"]["Mods"])
                {
                    if(mod["ID"] == "questsounds" && mod["Status"] == "Installed")
                    {
                        Installed = true;
                        break;
                    }
                }
                if(!Installed)
                {
                    txtbox.AppendText("\n\nI'll install QuestSounds for you");
                    txtbox.ScrollToEnd();
                    Support s = new Support();
                    s.Show();
                    s.StartSupport("bm://mods/install/QuestSounds");
                    MessageBox.Show("Please start Beat Saber and check if it works. Then press the install button again.", "BMBF Manager - HitSound Installing", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            } catch
            {
                if (!MainWindow.QuestSoundsInstalled)
                {
                    MessageBoxResult result = MessageBox.Show("Do you have the QuestSounds mod installed;", "BMBF Manager - Hitsound Installing", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    switch (result)
                    {
                        case MessageBoxResult.No:
                            txtbox.AppendText("\n\nI'll install QuestSounds for you");
                            txtbox.ScrollToEnd();
                            Support s = new Support();
                            s.Show();
                            s.StartSupport("bm://mods/install/QuestSounds");
                            MessageBox.Show("Please start Beat Saber and check if it works. Then press the install button again.", "BMBF Manager - HitSound Installing", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                    }
                }
            }

            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                txtbox.AppendText("\nChanging Sounds");
            }));
            //Change Config
            if (!adb("pull /sdcard/Android/data/com.beatgames.beatsaber/files/mod_cfgs/QuestSounds.json \"" + exe + "\\tmp\\QSounds.json\"")) return;
            String SoundType = SelectedSound.Substring(SelectedSound.Length - 3, 3).ToLower();
            if(!File.Exists(exe + "\\tmp\\QSounds.json"))
            {
                txtbox.AppendText("\n\nDo you have your Quest pluggwd into your PC? Do you have the QuestSounds mod installed? I was unable to change the config");
                return;
            }
            JSONNode config = JSON.Parse(File.ReadAllText(exe + "\\tmp\\QSounds.json"));
            if ((bool)GoodHitSound.IsChecked)
            {
                if (!adb("push \"" + SelectedSound + "\" /sdcard/Android/data/com.beatgames.beatsaber/files/sounds/HitSound." + SoundType)) return;
                config["Sounds"]["HitSound"]["activated"] = true;
                config["Sounds"]["HitSound"]["filepath"] = "/sdcard/Android/data/com.beatgames.beatsaber/files/sounds/HitSound." + SoundType;
            }
            else if((bool)BadHitSounds.IsChecked)
            {
                if (!adb("push \"" + SelectedSound + "\" /sdcard/Android/data/com.beatgames.beatsaber/files/sounds/BadHitSound." + SoundType)) return;
                config["Sounds"]["BadHitSound"]["activated"] = true;
                config["Sounds"]["BadHitSound"]["filepath"] = "/sdcard/Android/data/com.beatgames.beatsaber/files/sounds/BadHitSound." + SoundType;
            }
            else if ((bool)MenuMusic.IsChecked)
            {
                if (!adb("push \"" + SelectedSound + "\" /sdcard/Android/data/com.beatgames.beatsaber/files/sounds/MenuMusic." + SoundType)) return;
                config["Sounds"]["MenuMusic"]["activated"] = true;
                config["Sounds"]["MenuMusic"]["filepath"] = "/sdcard/Android/data/com.beatgames.beatsaber/files/sounds/MenuMusic." + SoundType;
            }
            else if ((bool)MenuClickSound.IsChecked)
            {
                if (!adb("push \"" + SelectedSound + "\" /sdcard/Android/data/com.beatgames.beatsaber/files/sounds/MenuClick." + SoundType)) return;
                config["Sounds"]["MenuClick"]["activated"] = true;
                config["Sounds"]["MenuClick"]["filepath"] = "/sdcard/Android/data/com.beatgames.beatsaber/files/sounds/MenuClick." + SoundType;
            }
            else if ((bool)FireWorks.IsChecked)
            {
                if (!adb("push \"" + SelectedSound + "\" /sdcard/Android/data/com.beatgames.beatsaber/files/sounds/Firework." + SoundType)) return;
                config["Sounds"]["Firework"]["activated"] = true;
                config["Sounds"]["Firework"]["filepath"] = "/sdcard/Android/data/com.beatgames.beatsaber/files/sounds/Firework." + SoundType;
            }
            else if ((bool)LevelCleared.IsChecked)
            {
                if (!adb("push \"" + SelectedSound + "\" /sdcard/Android/data/com.beatgames.beatsaber/files/sounds/LevelCleared." + SoundType)) return;
                config["Sounds"]["LevelCleared"]["activated"] = true;
                config["Sounds"]["LevelCleared"]["filepath"] = "/sdcard/Android/data/com.beatgames.beatsaber/files/sounds/LevelCleared." + SoundType;
            } else
            {
                txtbox.AppendText("\n\nPlease choose a Sound Type.");
                return;
            }
            File.WriteAllText(exe + "\\tmp\\QSoundsChanged.json", config.ToString());
            if (!adb("push \"" + exe + "\\tmp\\QSoundsChanged.json\" /sdcard/Android/data/com.beatgames.beatsaber/files/mod_cfgs/QuestSounds.json")) return;
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                txtbox.AppendText("\nSounds changed.");
            }));
        }

        private void GoodHit(object sender, RoutedEventArgs e)
        {
            GoodHitSound.IsChecked = true;
            BadHitSounds.IsChecked = false;
            MenuMusic.IsChecked = false;
            MenuClickSound.IsChecked = false;
            FireWorks.IsChecked = false;
            LevelCleared.IsChecked = false;
        }

        private void BadHit(object sender, RoutedEventArgs e)
        {
            GoodHitSound.IsChecked = false;
            BadHitSounds.IsChecked = true;
            MenuMusic.IsChecked = false;
            MenuClickSound.IsChecked = false;
            FireWorks.IsChecked = false;
            LevelCleared.IsChecked = false;
        }

        private void Menu(object sender, RoutedEventArgs e)
        {
            GoodHitSound.IsChecked = false;
            BadHitSounds.IsChecked = false;
            MenuMusic.IsChecked = true;
            MenuClickSound.IsChecked = false;
            FireWorks.IsChecked = false;
            LevelCleared.IsChecked = false;
        }

        private void MenuClick(object sender, RoutedEventArgs e)
        {
            GoodHitSound.IsChecked = false;
            BadHitSounds.IsChecked = false;
            MenuMusic.IsChecked = false;
            MenuClickSound.IsChecked = true;
            FireWorks.IsChecked = false;
            LevelCleared.IsChecked = false;
        }

        private void Highscore(object sender, RoutedEventArgs e)
        {
            GoodHitSound.IsChecked = false;
            BadHitSounds.IsChecked = false;
            MenuMusic.IsChecked = false;
            MenuClickSound.IsChecked = false;
            FireWorks.IsChecked = true;
            LevelCleared.IsChecked = false;
        }

        private void Cleared(object sender, RoutedEventArgs e)
        {
            GoodHitSound.IsChecked = false;
            BadHitSounds.IsChecked = false;
            MenuMusic.IsChecked = false;
            MenuClickSound.IsChecked = false;
            FireWorks.IsChecked = false;
            LevelCleared.IsChecked = true;
        }
    }
}
