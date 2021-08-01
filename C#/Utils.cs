using BMBF.Config;
using BMBF_Manager;
using ComputerUtils.RegxTemplates;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace BMBFManager.Utils
{
    public class ADBInteractor
    {
        public bool adb(String Argument, TextBox txtbox)
        {
            return adbThreadHandler(Argument, txtbox).Result;
        }

        public async Task<bool> adbThreadHandler(String Argument, TextBox txtbox)
        {
            bool returnValue = false;
            String txtAppend = "N/A";
            Thread t = new Thread(() =>
            {
                switch (adbThread(Argument))
                {
                    case "true":
                        returnValue = true;
                        break;
                    case "adb110":
                        txtAppend = MainWindow.globalLanguage.global.ADB110;
                        break;
                    case "adb100":
                        txtAppend = MainWindow.globalLanguage.global.ADB100;
                        break;

                }
            });
            t.Start();
            while(txtAppend == "N/A" && returnValue == false)
            {
                await DelayCheck();
            }
            if (txtAppend != "N/A")
            {
                txtbox.AppendText(txtAppend);
                txtbox.ScrollToEnd();
            }
            return returnValue;
        }

        public string adbThread(String Argument)
        {
            String User = System.Environment.GetEnvironmentVariable("USERPROFILE");

            foreach (String ADB in MainWindow.config.CachedADBPaths)
            {
                //redirect output
                ProcessStartInfo s = new ProcessStartInfo();
                s.UseShellExecute = false;
                s.FileName = ADB.Replace("User", User);
                s.WindowStyle = ProcessWindowStyle.Minimized;
                s.Arguments = Argument;
                s.RedirectStandardOutput = true;
                s.CreateNoWindow = true;
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
                                return "adb100";
                            }
                        }
                        else
                        {
                            exeProcess.WaitForExit();
                        }
                        return "true";
                    }
                }
                catch
                {
                    continue;
                }
            }
            return "adb100";
        }

        public string adbS(String Argument, TextBox txtbox)
        {
            return adbSThreadHandler(Argument, txtbox).Result;
        }

        public async Task<string> adbSThreadHandler(String Argument, TextBox txtbox)
        {
            string returnValue = "Error";
            String txtAppend = "N/A";
            Thread t = new Thread(() =>
            {
                String MethodReturnValue = adbSThread(Argument);
                Console.WriteLine("adbS finished");
                switch (MethodReturnValue)
                {
                    case "adb110":
                        txtAppend = MainWindow.globalLanguage.global.ADB110;
                        break;
                    case "adb100":
                        txtAppend = MainWindow.globalLanguage.global.ADB100;
                        break;
                    default:
                        returnValue = MethodReturnValue;
                        break;
                }
            });
            t.Start();
            while (txtAppend == "N/A" && returnValue == "Error")
            {
                await DelayCheck();
            }
            if (txtAppend != "N/A")
            {
                txtbox.AppendText(txtAppend);
                txtbox.ScrollToEnd();
            }
            return returnValue;
        }

        public async Task DelayCheck()
        {
            //Delay 500 ms without blocking thread  
            var frame = new DispatcherFrame();
            new Thread((ThreadStart)(() =>
            {
                Thread.Sleep(500);
                frame.Continue = false;
            })).Start();
            Dispatcher.PushFrame(frame);
        }

        public string adbSThread(String Argument)
        {
            String User = System.Environment.GetEnvironmentVariable("USERPROFILE");

            foreach (String ADB in MainWindow.config.CachedADBPaths)
            {
                //redirect output
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
                            return "adb110";
                        }

                        return IPS;
                    }
                }
                catch
                {
                    continue;
                }
            }
            return "adb100";
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

    public class BMBFUtils
    {
        public static bool PostChangesAndSync(TextBox txtbox, String ConfigString)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.QueryString.Add("foo", "foo");
                    client.UploadString("http://" + MainWindow.config.IP + ":50000/host/beatsaber/config", "PUT", ConfigString);
                    client.UploadValues("http://" + MainWindow.config.IP + ":50000/host/beatsaber/commitconfig", "POST", client.QueryString);
                }
                return true;
            }
            catch
            {
                txtbox.AppendText(MainWindow.globalLanguage.global.BMBF100);
                return false;
            }
        }

        public static BMBFC GetBMBFConfig()
        {
            WebClient client = new WebClient();
            return JsonSerializer.Deserialize<BMBFC>(client.DownloadString("http://" + MainWindow.config.IP + ":50000/host/beatsaber/config"));
        }

        public static bool Sync(TextBox txtbox)
        {
            try
            {
                Thread.Sleep(2000);
                using (WebClient client = new WebClient())
                {
                    client.QueryString.Add("foo", "foo");
                    client.UploadValues("http://" + MainWindow.config.IP + ":50000/host/beatsaber/commitconfig", "POST", client.QueryString);
                }
                return true;
            }
            catch
            {
                txtbox.AppendText(MainWindow.globalLanguage.global.BMBF110);
                return false;
            }
        }
    }

    public class ZipUtils
    {
        public static bool ExtractSafe(String sourceZip, String destinationFolder)
        {
            try
            {
                if (!Directory.Exists(destinationFolder)) Directory.CreateDirectory(destinationFolder);
                using (ZipArchive archive = ZipFile.OpenRead(sourceZip))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        String name = entry.FullName;
                        if (name.EndsWith("/") || name.EndsWith("/")) continue;
                        if (name.Contains("/")) Directory.CreateDirectory(destinationFolder + "\\" + System.IO.Path.GetDirectoryName(name));
                        entry.ExtractToFile(destinationFolder + "\\" + entry.FullName, true);
                    }
                }
                return true;
            } catch
            {
                return false;
            }
            
        }
    }
}