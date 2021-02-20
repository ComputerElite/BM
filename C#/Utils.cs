using BMBF_Manager;
using ComputerUtils.RegxTemplates;
using System;
using System.Diagnostics;
using System.Windows.Controls;

namespace BMBFManager.Utils
{
    public class ADBInteractor
    {
        public bool adb(String Argument, TextBox txtbox)
        {
            String User = System.Environment.GetEnvironmentVariable("USERPROFILE");

            foreach (String ADB in MainWindow.config.CachedADBPaths)
            {
                ProcessStartInfo s = new ProcessStartInfo();
                s.CreateNoWindow = true;
                s.UseShellExecute = false;
                s.FileName = ADB.Replace("User", User);
                s.WindowStyle = ProcessWindowStyle.Minimized;
                s.Arguments = Argument;
                s.RedirectStandardOutput = true;
                if (MainWindow.config.ShowADB)
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
                        if (!MainWindow.config.ShowADB)
                        {
                            String IPS = exeProcess.StandardOutput.ReadToEnd();
                            exeProcess.WaitForExit();
                            if (IPS.Contains("no devices/emulators found"))
                            {
                                txtbox.AppendText(MainWindow.globalLanguage.global.ADB110);
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
            txtbox.AppendText(MainWindow.globalLanguage.global.ADB100);
            txtbox.ScrollToEnd();
            return false;
        }

        public String adbS(String Argument, TextBox txtbox)
        {
            String User = System.Environment.GetEnvironmentVariable("USERPROFILE");

            foreach (String ADB in MainWindow.config.CachedADBPaths)
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
                            txtbox.AppendText(MainWindow.globalLanguage.global.ADB110);
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
            txtbox.AppendText(MainWindow.globalLanguage.global.ADB100);
            txtbox.ScrollToEnd();
            return "Error";
        }

        public void StartBMBF(TextBox txtbox)
        {
            adb("shell am start -n com.weloveoculus.BMBF/com.weloveoculus.BMBF.MainActivity", txtbox);
        }
    }

    public class IPUtils
    {
        public bool CheckIP(TextBox Quest)
        {
            getQuestIP(Quest);
            String found;
            if ((found = RegexTemplates.GetIP(MainWindow.config.IP)) != "")
            {
                MainWindow.config.IP = found;
                Quest.Text = MainWindow.config.IP;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void getQuestIP(TextBox Quest)
        {
            MainWindow.config.IP = Quest.Text;
        }
    }
}