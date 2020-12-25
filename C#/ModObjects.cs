using System.Collections.Generic;
using System;
using System.Text.Json;
using SimpleJSON;

namespace ModObjects
{
    public class ModUtils
    {
        public List<Mod> MergeModLists(ModList primary, ModList secondary, JSONNode BMBF, int major, int minor, int patch)
        {
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
                    String[] allver = oldModver.Replace("\"", "").Split('.');
                    List<int> finishedver = new List<int>();
                    String[] newver = m.downloads[0].modversion.Split('.');
                    Boolean newer = false;
                    foreach (String CV in allver)
                    {
                        finishedver.Add(Convert.ToInt32(CV));
                    }
                    int e = 0;
                    try
                    {
                        if ((Convert.ToInt32(newver[0]) >= finishedver[0] && Convert.ToInt32(newver[1]) >= finishedver[1] && Convert.ToInt32(newver[2]) >= finishedver[2]) || (Convert.ToInt32(newver[0]) >= finishedver[0] && Convert.ToInt32(newver[1]) > finishedver[1]) || (Convert.ToInt32(newver[0]) > finishedver[0]))
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
                            if (!d.download.EndsWith(".zip")) tmp[i - removed].downloads[tmp[i - removed].MatchingDownload].forward = true;
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

            return finished;
        }
    }

    public class ModList
    {
        public List<Mod> mods { get; set; } = new List<Mod>();
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
    }

    public class Download
    {
        public string modversion { get; set; } = "0.1.0";
        public List<string> gameversion { get; set; } = new List<string>();
        public string download { get; set; } = "";
        public bool forward { get; set; } = false;
        public bool coremod { get; set; } = false;
    }
}