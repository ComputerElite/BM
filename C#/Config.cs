using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace BMBFManager.Config
{
    public class BackupConfig
    {
        public int BackupType { get; set; } = 0;
        public bool BSBackup { get; set; } = false;
        public bool BMBFBackup { get; set; } = false;
        public Version BSVersion { get; set; } = new Version(1, 13, 2);
    }
    public class ConfigFile
    {
        public string IP { get; set; } = "";
        public string Version { get; set; } = "";
        public string Location { get; set; } = "";
        public string CustomImageSource { get; set; } = "N/A";
        public string GameVersion { get; set; } = "1.13.0";
        public string language { get; set; } = "en";
        public bool NotFirstRun { get; set; } = false;
        public bool CustomProtocols { get; set; } = false;
        public bool QSoundsInstalled { get; set; } = false;
        public bool QosmeticsInstalled { get; set; } = false;
        public bool CustomImage { get; set; } = false;
        public bool ComeFromUpdate { get; set; } = false;
        public bool BBBUTransfered { get; set; } = false;
        public bool QSUTransfered { get; set; } = false;
        public bool ShowADB { get; set; } = false;
        public bool Converted { get; set; } = false;
        public bool OneClick { get; set; } = false;
        public bool KeepAlive { get; set; } = true;
        public bool QosmeticsWarningShown { get; set; } = false;
        public bool PEWarningShown { get; set; } = false;
        public bool DCRPE { get; set; } = false;
        public List<string> CachedADBPaths { get; set; } = new List<string>();

        public static ConfigFile LoadConfig(string path)
        {
            return JsonSerializer.Deserialize<ConfigFile>(File.ReadAllText(path), new JsonSerializerOptions() { IgnoreNullValues = true});
        }

        public void SaveConfig()
        {
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "Config.json", JsonSerializer.Serialize(this));
        }
    }
}