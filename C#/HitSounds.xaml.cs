using ComputerUtils.RegxTemplates;
using Microsoft.Win32;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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

        String SelectedSound = MainWindow.globalLanguage.hitSounds.code.nothing;


        public HitSounds()
        {
            InitializeComponent();
            ApplyLanguage();
            Quest.Text = MainWindow.config.IP;
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
                uniformBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/HitSound3.png", UriKind.Absolute));
                uniformBrush.Stretch = Stretch.UniformToFill;
                this.Background = uniformBrush;
            }
            MainWindow.DCRPM.SetActivity(MainWindow.globalLanguage.dCRP.installingSounds);
        }

        public void ApplyLanguage()
        {
            chooseSoundButton.Content = MainWindow.globalLanguage.hitSounds.UI.chooseSoundButton;
            GoodHitSound.Content = MainWindow.globalLanguage.hitSounds.UI.hitSoundText;
            BadHitSounds.Content = MainWindow.globalLanguage.hitSounds.UI.badHitSoundText;
            MenuMusic.Content = MainWindow.globalLanguage.hitSounds.UI.menuMusicText;
            MenuClickSound.Content = MainWindow.globalLanguage.hitSounds.UI.menuClickText;
            FireWorks.Content = MainWindow.globalLanguage.hitSounds.UI.highscoreText;
            LevelCleared.Content = MainWindow.globalLanguage.hitSounds.UI.levelClearedText;
            installSoundButton.Content = MainWindow.globalLanguage.hitSounds.UI.installSoundButton;
            defaultButton.Content = MainWindow.globalLanguage.hitSounds.UI.defaultButton;
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

        private void Choose(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = MainWindow.globalLanguage.hitSounds.code.soundFile + " (*.mp3, *.ogg, *.wav)|*.mp3;*.ogg;*.wav";
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
                    SelectedSound = MainWindow.globalLanguage.hitSounds.code.nothing;
                    MessageBox.Show(MainWindow.globalLanguage.hitSounds.code.selectValidFile, "BMBF Manager - HitSound installing", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                
            }
        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            txtbox.AppendText("\n\n" + MainWindow.globalLanguage.hitSounds.code.changingToDefault);
            if (!MainWindow.aDBI.adb("pull /sdcard/ModData/com.beatgames.beatsaber/Configs/QuestSounds.json \"" + exe + "\\tmp\\QSounds.json\"", txtbox)) return;
            if (!File.Exists(exe + "\\tmp\\QSounds.json"))
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.hitSounds.code.configUnableToChange);
                return;
            }
            JSONNode config = JSON.Parse(File.ReadAllText(exe + "\\tmp\\QSounds.json"));
            if ((bool)GoodHitSound.IsChecked)
            {
                if (!MainWindow.aDBI.adb("shell rm -f " + config["Sounds"]["HitSound"]["filepath"], txtbox)) return;
            }
            else if ((bool)BadHitSounds.IsChecked)
            {
                if (!MainWindow.aDBI.adb("shell rm -f " + config["Sounds"]["BadHitSound"]["filepath"], txtbox)) return;
            }
            else if ((bool)MenuMusic.IsChecked)
            {
                if (!MainWindow.aDBI.adb("shell rm -f " + config["Sounds"]["MenuMusic"]["filepath"], txtbox)) return;
            }
            else if ((bool)MenuClickSound.IsChecked)
            {
                if (!MainWindow.aDBI.adb("shell rm -f " + config["Sounds"]["MenuClick"]["filepath"], txtbox)) return;
            }
            else if ((bool)FireWorks.IsChecked)
            {
                if (!MainWindow.aDBI.adb("shell rm -f " + config["Sounds"]["Firework"]["filepath"], txtbox)) return;
            }
            else if ((bool)LevelCleared.IsChecked)
            {
                if (!MainWindow.aDBI.adb("shell rm -f " + config["Sounds"]["LevelCleared"]["filepath"], txtbox)) return;
            }
            else
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.hitSounds.code.chooseASoundType);
                return;
            }
            txtbox.AppendText("\n" + MainWindow.globalLanguage.hitSounds.code.changedToDefault);
        }

        private void Install(object sender, RoutedEventArgs e)
        {
            if (!MainWindow.iPUtils.CheckIP(Quest)) return;
            if(SelectedSound == MainWindow.globalLanguage.hitSounds.code.nothing)
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.hitSounds.code.selectSound);
                return;
            }
            JSONNode BMBF = JSON.Parse("{}");

            //Check if QuestSoudns is installed
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                txtbox.AppendText("\n" + MainWindow.globalLanguage.hitSounds.code.qsoundsInstalled);
            }));
            try
            {
                WebClient c = new WebClient();
                BMBF = JSON.Parse(c.DownloadString("http://" + MainWindow.config.IP + ":50000/host/beatsaber/config"));
                Boolean Installed = false;
                foreach(JSONNode mod in BMBF["Config"]["Mods"])
                {
                    if(mod["ID"] == "QuestSounds" && mod["Status"] == "Installed")
                    {
                        Installed = true;
                        break;
                    }
                }
                if(!Installed)
                {
                    txtbox.AppendText("\n\n" + MainWindow.globalLanguage.hitSounds.code.willInstallQSounds);
                    txtbox.ScrollToEnd();
                    Support s = new Support();
                    s.Show();
                    s.StartSupport("bm://mods/install/QuestSounds");
                    MessageBox.Show(MainWindow.globalLanguage.hitSounds.code.checkIfQSoundsWorks, "BMBF Manager - HitSound Installing", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            } catch
            {
                if (!MainWindow.config.QSoundsInstalled)
                {
                    MessageBoxResult result = MessageBox.Show(MainWindow.globalLanguage.hitSounds.code.DoYouHaveQSounds, "BMBF Manager - Hitsound Installing", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    switch (result)
                    {
                        case MessageBoxResult.No:
                            txtbox.AppendText("\n\n" + MainWindow.globalLanguage.hitSounds.code.willInstallQSounds);
                            txtbox.ScrollToEnd();
                            Support s = new Support();
                            s.Show();
                            s.StartSupport("bm://mods/install/QuestSounds");
                            MessageBox.Show(MainWindow.globalLanguage.hitSounds.code.checkIfQSoundsWorks, "BMBF Manager - HitSound Installing", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                    }
                }
            }

            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                txtbox.AppendText("\n" + MainWindow.globalLanguage.hitSounds.code.changingSound);
            }));
            //Change Config
            if (!MainWindow.aDBI.adb("pull /sdcard/ModData/com.beatgames.beatsaber/Configs/QuestSounds.json \"" + exe + "\\tmp\\QSounds.json\"", txtbox)) return;
            String SoundType = SelectedSound.Substring(SelectedSound.Length - 3, 3).ToLower();
            if(!File.Exists(exe + "\\tmp\\QSounds.json"))
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.hitSounds.code.configUnableToChange);
                return;
            }
            JSONNode config = JSON.Parse(File.ReadAllText(exe + "\\tmp\\QSounds.json"));
            if ((bool)GoodHitSound.IsChecked)
            {
                if (!MainWindow.aDBI.adb("push \"" + SelectedSound + "\" /sdcard/ModData/com.beatgames.beatsaber/Mods/QuestSounds/HitSound." + SoundType, txtbox)) return;
                config["Sounds"]["HitSound"]["activated"] = true;
                config["Sounds"]["HitSound"]["filepath"] = "/sdcard/ModData/com.beatgames.beatsaber/Mods/QuestSounds/HitSound." + SoundType;
            }
            else if((bool)BadHitSounds.IsChecked)
            {
                if (!MainWindow.aDBI.adb("push \"" + SelectedSound + "\" /sdcard/ModData/com.beatgames.beatsaber/Mods/QuestSounds/BadHitSound." + SoundType, txtbox)) return;
                config["Sounds"]["BadHitSound"]["activated"] = true;
                config["Sounds"]["BadHitSound"]["filepath"] = "/sdcard/ModData/com.beatgames.beatsaber/Mods/QuestSounds/BadHitSound." + SoundType;
            }
            else if ((bool)MenuMusic.IsChecked)
            {
                if (!MainWindow.aDBI.adb("push \"" + SelectedSound + "\" /sdcard/ModData/com.beatgames.beatsaber/Mods/QuestSounds/MenuMusic." + SoundType, txtbox)) return;
                config["Sounds"]["MenuMusic"]["activated"] = true;
                config["Sounds"]["MenuMusic"]["filepath"] = "/sdcard/ModData/com.beatgames.beatsaber/Mods/QuestSounds/MenuMusic." + SoundType;
            }
            else if ((bool)MenuClickSound.IsChecked)
            {
                if (!MainWindow.aDBI.adb("push \"" + SelectedSound + "\" /sdcard/ModData/com.beatgames.beatsaber/Mods/QuestSounds/MenuClick." + SoundType, txtbox)) return;
                config["Sounds"]["MenuClick"]["activated"] = true;
                config["Sounds"]["MenuClick"]["filepath"] = "/sdcard/ModData/com.beatgames.beatsaber/Mods/QuestSounds/MenuClick." + SoundType;
            }
            else if ((bool)FireWorks.IsChecked)
            {
                if (!MainWindow.aDBI.adb("push \"" + SelectedSound + "\" /sdcard/ModData/com.beatgames.beatsaber/Mods/QuestSounds/Firework." + SoundType, txtbox)) return;
                config["Sounds"]["Firework"]["activated"] = true;
                config["Sounds"]["Firework"]["filepath"] = "/sdcard/ModData/com.beatgames.beatsaber/Mods/QuestSounds/Firework." + SoundType;
            }
            else if ((bool)LevelCleared.IsChecked)
            {
                if (!MainWindow.aDBI.adb("push \"" + SelectedSound + "\" /sdcard/ModData/com.beatgames.beatsaber/Mods/QuestSounds/LevelCleared." + SoundType, txtbox)) return;
                config["Sounds"]["LevelCleared"]["activated"] = true;
                config["Sounds"]["LevelCleared"]["filepath"] = "/sdcard/ModData/com.beatgames.beatsaber/Mods/QuestSounds/LevelCleared." + SoundType;
            } else
            {
                txtbox.AppendText("\n\n" + MainWindow.globalLanguage.hitSounds.code.chooseASoundType);
                return;
            }
            File.WriteAllText(exe + "\\tmp\\QSoundsChanged.json", config.ToString());
            if (!MainWindow.aDBI.adb("push \"" + exe + "\\tmp\\QSoundsChanged.json\" /sdcard/ModData/com.beatgames.beatsaber/Configs/QuestSounds.json", txtbox)) return;
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                txtbox.AppendText("\n" + MainWindow.globalLanguage.hitSounds.code.changedSound);
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
