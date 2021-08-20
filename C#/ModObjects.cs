using System.Collections.Generic;
using System;
using System.Text.Json;
using SimpleJSON;
using BMBF.Config;

namespace ModObjects
{
    public class ModUtils
    {
        public List<Mod> MergeModLists(ModList primary, ModList secondary, BMBFC BMBF, int major, int minor, int patch)
        {
            TimeoutWebClientShort c = new TimeoutWebClientShort();
            JSONNode CoreMods = JSON.Parse(c.DownloadString("https://raw.githubusercontent.com/BMBF/resources/master/com.beatgames.beatsaber/core-mods.json"));
            List<Mod> finished = new List<Mod>();

            foreach(Mod m in secondary.mods)
            {
                finished.Add(m);
            }

            foreach(Mod m in primary.mods)
            {
                bool existent = false;
                int Index = 0;
                foreach (Mod s in finished)
                {
                    if (s.name.ToLower() == m.name.ToLower())
                    {
                        existent = true;
                        break;
                    }
                    Index++;
                }
                if (!existent) finished.Add(m);
                else
                {
                    String oldModver = finished[Index].downloads[0].modversion;
                    Boolean newer = false;
                    int e = 0;
                    try
                    {
                        if (new Version(m.downloads[0].modversion).CompareTo(new Version(oldModver)) == 1)
                        {
                            newer = true;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                    e++;
                    if (!newer) continue;

                    finished.RemoveAt(Index);

                    finished.Add(m);
                }
            }

            List<Mod> tmp = new List<Mod>(finished);
            int i = 0;
            int removed = 0;
            foreach(Mod m in finished)
            {
                int download = 0;
                bool found = false;
                foreach(Download d in m.downloads)
                {
                    int gv = 0;
                    foreach(string g in d.gameversion)
                    {
                        String[] GameVersion = g.Split('.');
                        int Mmajor = Convert.ToInt32(GameVersion[0]);
                        int Mminor = Convert.ToInt32(GameVersion[1]);
                        int Mpatch = 0;
                        if (GameVersion.Length == 3)
                        {
                            Mpatch = Convert.ToInt32(GameVersion[2]);
                        }
                        if (major == Mmajor && minor == Mminor && patch >= Mpatch)
                        {
                            found = true;
                            
                            tmp[i - removed].MatchingDownload = download;
                            tmp[i - removed].MatchingGameVersion = gv;
                            foreach (JSONNode n in CoreMods[major + "." + minor + "." + patch]["mods"])
                            {
                                if (m.ModID.ToLower() == n["id"].ToString().Replace("\"", "").ToLower() && d.modversion == n["version"].ToString().Replace("\"", "").ToLower())
                                {
                                    tmp[i - removed].downloads[tmp[i - removed].MatchingDownload].coremod = true;
                                    break;
                                }
                            }
                            
                            if (!d.download.EndsWith(".zip") && !d.download.ToLower().EndsWith(".qmod")) tmp[i - removed].downloads[tmp[i - removed].MatchingDownload].forward = true;
                            break;
                        }
                        gv++;
                    }
                    if (found) break;
                    download++;
                }
                if (!found)
                {
                    tmp.RemoveAt(i - removed);
                    removed++;
                }
                i++;
            }

            finished = new List<Mod>(tmp);

            i = 0;
            foreach(Mod m in finished)
            {
                if (m.ModID == "") tmp[i].ModID = m.name;
                i++;
            }
            finished = new List<Mod>(tmp);

            //Check if mod is installed
            for(int ii = 0; ii < finished.Count; ii++)
            {
                foreach(BMBF.Config.Mod m in BMBF.Config.Mods)
                {
                    if (m.Id == finished[ii].ModID)
                    {
                        finished[ii].installed = true;
                        finished[ii].Version = m.Version;
                        finished[ii].GameVersion = m.Version;
                        break;
                    }
                }
            }

            //Add installed mods without download
            foreach(BMBF.Config.Mod m in BMBF.Config.Mods)
            {
                bool exists = false;
                foreach (Mod m2 in finished)
                {
                    if(m2.ModID == m.Id)
                    {
                        exists = true;
                        break;
                    }
                }
                if(!exists)
                {
                    Mod add = new Mod();
                    add.installed = true;
                    add.core = m.Uninstallable;
                    add.downloadable = false;
                    add.GameVersion = m.TargetBeatsaberVersion;
                    add.Version = m.Version;
                    List<String> authors = new List<String>();
                    authors.Add(m.Author);
                    add.creator = authors;
                    add.name = m.Name;
                    add.ModID = m.Id;
                    add.details = m.Description;
                    finished.Add(add);
                }
            }

            return finished;
        }

        public ModList RemoveIncompatibleMods(ModList list, String GameVersion, ModList listincomp)
        {
            List<string> incompatible = new List<string>();
            foreach(GameVersionIncompatibility g in listincomp.GameVersionIncompatibilities)
            {
                foreach(GameVersion v in g.GameVersions)
                {
                    if(v.Version.ToLower() == GameVersion.ToLower())
                    {
                        List<GameVersion> tmp = new List<GameVersion>(g.GameVersions);
                        foreach (GameVersion ver in g.GameVersions)
                        {
                            if (ver.Version.ToLower() != GameVersion.ToLower()) incompatible.Add(ver.Version.ToLower());
                        }
                        break;
                    }
                }
            }

            List<Mod> returnMod = new List<Mod>();

            foreach(Mod m in list.mods)
            {
                Mod addMod = new Mod();
                addMod.creator = m.creator;
                addMod.details = m.details;
                addMod.ModID = m.ModID;
                addMod.name = m.name;
                foreach(Download d in m.downloads)
                {
                    bool incomp = false;
                    foreach(string s in d.gameversion)
                    {
                        if(incompatible.Contains(s.ToLower())) {
                            incomp = true;
                            break;
                        }
                    }
                    if (!incomp) addMod.downloads.Add(d);
                }
                if(addMod.downloads.Count > 0) returnMod.Add(addMod);
            }
            list.mods = returnMod;
            return list;
        }
    }

    public class ModList
    {
        public List<Mod> mods { get; set; } = new List<Mod>();
        public List<GameVersionIncompatibility> GameVersionIncompatibilities { get; set; } = new List<GameVersionIncompatibility>();
    }

    public class GameVersionIncompatibility
    {
        public List<GameVersion> GameVersions { get; set; } = new List<GameVersion>();
    }

    public class GameVersion
    {
        public int Major { get; set; } = 0;
        public int Minor { get; set; } = 0;
        public int Patch { get; set; } = 0;
        public String Version { get
            {
                return Major + "." + Minor + "." + Patch;
            }
            set
            {
                try
                {
                    String[] strings = value.Replace("\"", "").Split('.');
                    List<int> ints = new List<int>();
                    foreach(string s in strings)
                    {
                        ints.Add(Convert.ToInt32(s));
                    }
                    Major = ints[0];
                    Minor = ints[1];
                    Patch = ints[2];
                }catch { }
            }
        }
    }

    public class Mod
    {
        public string name { get; set; } = "";
        public string details { get; set; } = "";
        public string ModID { get; set; } = "";
        public List<string> creator { get; set; } = new List<string>();
        public List<Download> downloads { get; set; } = new List<Download>();
        public int MatchingDownload = 0;
        public int MatchingGameVersion = 0;
        public bool installed = false;
        public bool downloadable = true;
        public bool core = false;
        public string GameVersion = "";
        public string Version = "";
    }

    public class Download
    {
        public string modversion { get; set; } = "0.1.0";
        public List<string> gameversion { get; set; } = new List<string>();
        public string download { get; set; } = "";
        public bool forward { get; set; } = false;
        public bool coremod { get; set; } = false;
        public string notes { get; set; } = "N/A";
    }
}