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

        public int GetTotalSongsCount()
        {
            int i = 0;
            foreach (BMBFPlaylist pl in Playlists) i += pl.SongList.Count;
            return i;
        }
    }

    public class BMBFPlaylist
    {
        public String PlaylistID { get; set; } = "";
        public String PlaylistName { get; set; } = "N/A";
        public List<BMBFSong> SongList { get; set; } = new List<BMBFSong>();
        public string CoverImageBytes { get; set; } = null;
        public bool IsCoverLoaded { get; set; } = true;

        public int GetSongCount()
        {
            return SongList.Count;
        }
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
        public string image { get; set; } = "";
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

    public class BMBFlocal
    {
        public String version { get; set; } = "1.13.0";
        public bool isNightly { get; set; } = false;
    }

    public class BMBFModstatus
    {
        public String BMBFVersion { get; set; } = "1.13.0";
        public String CurrentStatus { get; set; } = "1.13.0";
        public bool IsBeatSaberInstalled { get; set; } = true;
        public bool HasGoodBackup { get; set; } = true;
        public bool HasHalfAssBackup { get; set; } = true;
    }

    public class BMBFStableVersions
    {
        public int id { get; set; } = 0;
        public String name { get; set; } = "N/A";
        public String tag { get; set; } = "N/A";
        public String body { get; set; } = "N/A";
        public String created { get; set; } = "N/A";
        public String updated { get; set; } = "N/A";
        public List<BMBFAssets> assets { get; set; } = new List<BMBFAssets>();
    }

    public class BMBFAssets {
        public int id { get; set; } = 0;
        public String name { get; set; } = "N/A";
        public int size { get; set; } = 0;
        public String checksum { get; set; } = "N/A";
        public String created { get; set; } = "N/A";
        public String updated { get; set; } = "N/A";
        public String content_type { get; set; } = "N/A";
    }

    public class SongLibraryMoveSong
    {
        public String sourceFolder { get; set; } = "";
        public String hash { get; set; } = "";
        public String targetFolder { get; set; } = "";
        public String songName { get; set; } = "";
        public String key { get; set; } = "";
        public String songArtist { get; set; } = "";
        public String Playlist { get; set; } = "default";
    }
}