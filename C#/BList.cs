

using System;
using System.Collections.Generic;

namespace BMBFManager.Playlists
{
    public class BList
    {
        public string title { get; set; } = "";
        public string author { get; set; } = "";
        public string description { get; set; } = "";
        public Dictionary<string, object> customData { get; set; } = new Dictionary<string, object>();
        public List<BListSong> maps { get; set; } = new List<BListSong>();
        public string cover { get; set; } = "";
    }

    public class BListSong
    {
        public Dictionary<string, object> customData { get; set; } = new Dictionary<string, object>();
        public DateTime date { get; set; } = DateTime.Now;
        public string hash { get; set; } = "";
        public string key { get; set; } = "";
        public string levelID { get; set; } = "";
        public List<BListSongDifficulty> difficulties{ get; set; } = new List<BListSongDifficulty>();
        public BListType type = BListType.Hash;
    }

    public enum BListType
    {
        Hash,
        Key,
        LevelID
    }

    public class BListSongDifficulty
    {
        public string characteristic { get; set; } = "";
        public string name { get; set; } = "";
    }
}