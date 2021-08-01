using System.Collections.Generic;
using System;
using System.Text.Json;

namespace BMBF.Config
{
    public class BMBFC
    {
        public BMBFConfig Config { get; set; } = new BMBFConfig();
        public String BeatSaberVersion { get; set; } = "";
    }

    public class Mod
    {
        public string Id { get; set; } = "";
        public string Path { get; set; } = "";
        public string Version { get; set; } = "";
        public string TargetBeatsaberVersion { get; set; } = "";
        public string Author { get; set; } = "";
        public string Name { get; set; } = "";
        public string CoverImageFilename { get; set; } = "";
        public string Description { get; set; } = "";
        public bool Installed { get; set; } = false;
        public bool IsAwaitingSync { get; set; } = false;
        public bool Uninstallable { get; set; } = false;
        public JsonElement Porter { get; set; } = new JsonElement();
    }

    public class BMBFConfig
    {
        public string ModsPath { get; set; } = "/sdcard/BMBFData/Mods/";
        public string PlaylistsPath { get; set; } = "/sdcard/BMBFData/Playlists";
        public List<BMBFPlaylist> Playlists { get; set; } = new List<BMBFPlaylist>();
        public List<Mod> Mods { get; set; } = new List<Mod>();
        public Dictionary<String, BMBFSong> KnownSongs { get; set; } = new Dictionary<String, BMBFSong>();

        public List<BMBFSong> GetAllSongs()
        {
            List<BMBFSong> songs = new List<BMBFSong>();
            foreach(String s in KnownSongs.Keys)
            {
                songs.Add(KnownSongs[s]);
                //Console.WriteLine(KnownSongs[s].Hash);
            }
            return songs;
        }

        public int GetTotalSongsCount()
        {
            return GetAllSongs().Count;
        }
    }

    public class BMBFPlaylist
    {
        public String PlaylistId { get; set; } = "";
        public String PlaylistName { get; set; } = "N/A";
        public List<BMBFSong> SongList { get; set; } = new List<BMBFSong>();
        public string Path { get; set; } = "";
        public string CoverImageFilename { get; set; } = null;

        public void Init()
        {
            this.PlaylistId = PlaylistName + DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        }

        public int GetSongCount()
        {
            return SongList.Count;
        }
    }

    public class BMBFSong
    {
        public String Hash { get; set; } = "";
        public String SongName { get; set; } = "";
        public String SongSubName { get; set; } = "";
        public String SongAuthorName { get; set; } = "";
        public String LevelAuthorName { get; set; } = "";
        public String Path { get; set; } = null;

        public override bool Equals(object obj)
        {
            BMBFSong s = (BMBFSong)obj;
            return Hash == s.Hash && SongName == s.SongName && SongSubName == s.SongSubName && SongAuthorName == s.SongAuthorName && LevelAuthorName == s.LevelAuthorName && Path == s.Path;
        }
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