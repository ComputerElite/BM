using System.Collections.Generic;
using System;
using System.Text.Json;

namespace BMBF.Config
{
    public class BMBFC
    {
        public BMBFConfig Config { get; set; } = new BMBFConfig();
    }

    public class BMBFConfig
    {
        public List<BMBFPlaylist> Playlists { get; set; } = new List<BMBFPlaylist>();
        public List<JsonElement> Mods { get; set; } = new List<JsonElement>();
        public JsonElement Saber { get; set; } = new JsonElement();
        public JsonElement LeftColor { get; set; } = new JsonElement();
        public JsonElement RightColor { get; set; } = new JsonElement();
        public List<JsonElement> TextChanges { get; set; } = new List<JsonElement>();
    }

    public class BMBFPlaylist
    {
        public String PlaylistID { get; set; } = "N/A";
        public String PlaylistName { get; set; } = "N/A";
        public List<BMBFSong> SongList { get; set; } = new List<BMBFSong>();
        public string CoverImageBytes { get; set; } = null;
        public bool IsCoverLoaded { get; set; } = true;
    }

    public class BMBFSong
    {
        public String SongID { get; set; } = "";
        public String SongName { get; set; } = "";
        public String SongSubName { get; set; } = "";
        public String SongAuthorName { get; set; } = "";
        public String LevelAuthorName { get; set; } = "";
        public String CustomSongPath { get; set; } = null;
    }

    public class BPList
    {
        public string playlistTitle { get; set; } = "N/A";
        public string playlistAuthor { get; set; } = "BMBF Manager";
        public string image { get; set; } = "data:image/png;base64,";
        public List<BPListSong> songs { get; set; } = new List<BPListSong>();
    }

    public class BPListSong
    {
        public string hash { get; set; } = "";
        public string songName { get; set; } = "";
    }

    public class BSKFile
    {
        public List<string> knownLevelIds { get; set; } = new List<string>();
        public List<string> knownLevelPackIds { get; set; } = new List<string>();
    }
}