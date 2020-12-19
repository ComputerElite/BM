/*
    You ask what this is? I had the Idea of lerning how to work with different classes and help myself a bit better with the BeatSaver api. (DON'T SPAM ME THAT BEATSAVERSHARP EXISTS).
    Btw nice that you are interested in my code.
*/
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BeatSaverAPI
{
    public class BeatSaberSong
    {
        [JsonPropertyName("_songName")]
        public string SongName { get; set; } = "N/A";
        [JsonPropertyName("_songSubName")]
        public string SubName { get; set; } = "N/A";
        [JsonPropertyName("_songAuthorName")]
        public string SongArtist { get; set; } = "N/A";
        [JsonPropertyName("_levelAuthorName")]
        public string Mapper { get; set; } = "N/A";
        [JsonPropertyName("_beatsPerMinute")]
        public decimal BPM { get; set; } = 0.0m;
        public string Hash { get; set; } = "N/A";
        public string Key { get; set; } = "N/A";
        public bool RequestGood { get; set; } = false;
        [JsonPropertyName("_difficultyBeatmapSets")]
        public List<BeatSaberSongBeatMapCharacteristic> BeatMapCharacteristics { get; set; } = new List<BeatSaberSongBeatMapCharacteristic>();

        public string _allDirectionsEnvironmentName { get; set; } = "N/A";
        public string _environmentName { get; set; } = "N/A";
        public string _coverImageFilename { get; set; } = "N/A";
        public string _songFilename { get; set; } = "N/A";
        public decimal _previewDuration { get; set; } = 0.0m;
        public decimal _previewStartTime { get; set; } = 0.0m;
        public decimal _shufflePeriod { get; set; } = 0.0m;
        public decimal _shuffle { get; set; } = 0.0m;
        public decimal _songTimeOffset { get; set; } = 0.0m;
        public string _version { get; set; } = "N/A";

        public string GetJSON()
        {
            return JsonSerializer.Serialize(this);
        }
    }

    public class BeatSaverAPISearchResult
    {
        public List<BeatSaverAPISong> docs { get; set; } = new List<BeatSaverAPISong>();
        public int totalDocs { get; set; } = 0;
        public int lastPage { get; set; } = 0;
        public bool RequestGood { get; set; } = false;

        public bool HasResults()
        {
            if (docs.Count < 1) return false;
            return true;
        }
    }

    public class BeatSaverAPISong
    {
        public BeatSaverAPIMetadata metadata { get; set; } = new BeatSaverAPIMetadata();
        public BeatSaverAPIStats stats { get; set; } = new BeatSaverAPIStats();
        public string description { get; set; } = "N/A";
        public string deletedAt { get; set; } = "N/A";
        public string _id { get; set; } = "N/A";
        public string key { get; set; } = "N/A";
        public string name { get; set; } = "N/A";
        public BeatSaverAPIUploader uploader { get; set; } = new BeatSaverAPIUploader();
        public string hash { get; set; } = "N/A";
        public string uploaded { get; set; } = "N/A";
        public string directDownload { get; set; } = "N/A";
        public string downloadURL { get; set; } = "N/A";
        public string coverURL { get; set; } = "N/A";
        public bool GoodRequest { get; set; } = false;

        public BeatSaberSong ConvertToBeatSaberSong()
        {
            BeatSaberSong finished = new BeatSaberSong();
            finished.BPM = metadata.bpm;
            finished.Hash = hash;
            finished.Key = key;
            finished.Mapper = metadata.levelAuthorName;
            finished.SongName = name;
            finished.SongArtist = metadata.songAuthorName;
            finished.RequestGood = true;
            finished.SubName = metadata.songSubName;
            finished.RequestGood = GoodRequest;
            foreach (BeatSaverAPICharacteristic characteristic in metadata.characteristics)
            {
                BeatSaberSongBeatMapCharacteristic characteristic1 = new BeatSaberSongBeatMapCharacteristic();
                characteristic1.BeatMapCharacteristicName = characteristic.name;
                if (characteristic.difficulties.easy != null)
                {
                    BeatSaberSongDifficulty easy = new BeatSaberSongDifficulty();
                    easy.Bombs = characteristic.difficulties.easy.bombs;
                    easy.DifficultyID = 1;
                    easy.DifficultyName = "easy";
                    easy.Duration = characteristic.difficulties.easy.duration;
                    easy.Length = characteristic.difficulties.easy.length;
                    easy.NJS = characteristic.difficulties.easy.njs;
                    easy.NJSOffset = characteristic.difficulties.easy.njsOffset;
                    easy.Notes = characteristic.difficulties.easy.notes;
                    easy.Obstacles = characteristic.difficulties.easy.obstacles;
                    characteristic1.Difficulties.Add(easy);
                }
                if (characteristic.difficulties.normal != null)
                {
                    BeatSaberSongDifficulty normal = new BeatSaberSongDifficulty();
                    normal.Bombs = characteristic.difficulties.normal.bombs;
                    normal.DifficultyID = 3;
                    normal.DifficultyName = "normal";
                    normal.Duration = characteristic.difficulties.normal.duration;
                    normal.Length = characteristic.difficulties.normal.length;
                    normal.NJS = characteristic.difficulties.normal.njs;
                    normal.NJSOffset = characteristic.difficulties.normal.njsOffset;
                    normal.Notes = characteristic.difficulties.normal.notes;
                    normal.Obstacles = characteristic.difficulties.normal.obstacles;
                    characteristic1.Difficulties.Add(normal);
                }
                if (characteristic.difficulties.hard != null)
                {
                    BeatSaberSongDifficulty hard = new BeatSaberSongDifficulty();
                    hard.Bombs = characteristic.difficulties.hard.bombs;
                    hard.DifficultyID = 5;
                    hard.DifficultyName = "hard";
                    hard.Duration = characteristic.difficulties.hard.duration;
                    hard.Length = characteristic.difficulties.hard.length;
                    hard.NJS = characteristic.difficulties.hard.njs;
                    hard.NJSOffset = characteristic.difficulties.hard.njsOffset;
                    hard.Notes = characteristic.difficulties.hard.notes;
                    hard.Obstacles = characteristic.difficulties.hard.obstacles;
                    characteristic1.Difficulties.Add(hard);
                }
                if (characteristic.difficulties.expert != null)
                {
                    BeatSaberSongDifficulty expert = new BeatSaberSongDifficulty();
                    expert.Bombs = characteristic.difficulties.expert.bombs;
                    expert.DifficultyID = 7;
                    expert.DifficultyName = "expert";
                    expert.Duration = characteristic.difficulties.expert.duration;
                    expert.Length = characteristic.difficulties.expert.length;
                    expert.NJS = characteristic.difficulties.expert.njs;
                    expert.NJSOffset = characteristic.difficulties.expert.njsOffset;
                    expert.Notes = characteristic.difficulties.expert.notes;
                    expert.Obstacles = characteristic.difficulties.expert.obstacles;
                    characteristic1.Difficulties.Add(expert);
                }
                if (characteristic.difficulties.expertPlus != null)
                {
                    BeatSaberSongDifficulty expertPlus = new BeatSaberSongDifficulty();
                    expertPlus.Bombs = characteristic.difficulties.expertPlus.bombs;
                    expertPlus.DifficultyID = 9;
                    expertPlus.DifficultyName = "expertPlus";
                    expertPlus.Duration = characteristic.difficulties.expertPlus.duration;
                    expertPlus.Length = characteristic.difficulties.expertPlus.length;
                    expertPlus.NJS = characteristic.difficulties.expertPlus.njs;
                    expertPlus.NJSOffset = characteristic.difficulties.expertPlus.njsOffset;
                    expertPlus.Notes = characteristic.difficulties.expertPlus.notes;
                    expertPlus.Obstacles = characteristic.difficulties.expertPlus.obstacles;
                    characteristic1.Difficulties.Add(expertPlus);
                }
                finished.BeatMapCharacteristics.Add(characteristic1);
            }
            return finished;
        }
    }

    public class BeatSaverAPIStats
    {
        public int downloads { get; set; } = 0;
        public int plays { get; set; } = 0;
        public int downVotes { get; set; } = 0;
        public int upVotes { get; set; } = 0;
        public decimal heat { get; set; } = 0.0m;
        public decimal rating { get; set; } = 0.0m;
    }

    public class BeatSaverAPIUploader
    {
        public string _id { get; set; } = "N/A";
        public string username { get; set; } = "N/A";
    }

    public class BeatSaverAPIMetadata
    {
        public BeatSaverAPIDifficulties difficulties { get; set; } = new BeatSaverAPIDifficulties();
        public decimal duration { get; set; } = 0.0m;
        public string automapper { get; set; } = "N/A";
        public List<BeatSaverAPICharacteristic> characteristics { get; set; } = new List<BeatSaverAPICharacteristic>();
        public string levelAuthorName { get; set; } = "N/A";
        public string songAuthorName { get; set; } = "N/A";
        public string songName { get; set; } = "N/A";
        public string songSubName { get; set; } = "N/A";
        public decimal bpm { get; set; } = 0.0m;
    }

    public class BeatSaverAPICharacteristic
    {
        public BeatSaverAPICharacteristicDifficulties difficulties { get; set; } = new BeatSaverAPICharacteristicDifficulties();
        public string name { get; set; } = "N/A";

        public bool IsLawless()
        {
            if (name.ToLower() == "lawless") return true;
            else return false;
        }

        public bool IsLightshow()
        {
            if (name.ToLower() == "lightshow") return true;
            else return false;
        }

        public bool IsStandart()
        {
            if (name.ToLower() == "standard") return true;
            else return false;
        }

        public bool IsNoArrows()
        {
            if (name.ToLower() == "noarrows") return true;
            else return false;
        }

        public bool IsOneSaber()
        {
            if (name.ToLower() == "onesaber") return true;
            else return false;
        }

        public bool IsNinetyDegree()
        {
            if (name.ToLower() == "90degree") return true;
            else return false;
        }

        public bool IsThreeSixtyDegree()
        {
            if (name.ToLower() == "360Degree") return true;
            else return false;
        }
    }

    public class BeatSaverAPICharacteristicDifficulties
    {
        public BeatSaverAPICharacteristicDifficulty easy { get; set; } = new BeatSaverAPICharacteristicDifficulty();
        public BeatSaverAPICharacteristicDifficulty expert { get; set; } = new BeatSaverAPICharacteristicDifficulty();
        public BeatSaverAPICharacteristicDifficulty expertPlus { get; set; } = new BeatSaverAPICharacteristicDifficulty();
        public BeatSaverAPICharacteristicDifficulty hard { get; set; } = new BeatSaverAPICharacteristicDifficulty();
        public BeatSaverAPICharacteristicDifficulty normal { get; set; } = new BeatSaverAPICharacteristicDifficulty();
    }

    public class BeatSaverAPICharacteristicDifficulty
    {
        public decimal duration { get; set; } = 0.0m;
        public decimal length { get; set; } = 0.0m;
        public decimal njs { get; set; } = 0.0m;
        public decimal njsOffset { get; set; } = 0.0m;
        public int bombs { get; set; } = 0;
        public int notes { get; set; } = 0;
        public int obstacles { get; set; } = 0;
    }

    public class BeatSaverAPIDifficulties
    {
        public bool easy { get; set; } = false;
        public bool expert { get; set; } = false;
        public bool expertPlus { get; set; } = false;
        public bool hard { get; set; } = false;
        public bool normal { get; set; } = false;
    }

    public class BeatSaberSongDifficulty
    {
        public decimal Duration { get; set; } = 0.0m;
        public decimal Length { get; set; } = 0.0m;
        [JsonPropertyName("_noteJumpMovementSpeed")]
        public decimal NJS { get; set; } = 0.0m;
        [JsonPropertyName("_noteJumpStartBeatOffset")]
        public decimal NJSOffset { get; set; } = 0.0m;
        public int Bombs { get; set; } = 0;
        public int Notes { get; set; } = 0;
        public int Obstacles { get; set; } = 0;
        [JsonPropertyName("_difficulty")]
        public string DifficultyName { get; set; } = "N/A";
        [JsonPropertyName("_difficultyRank")]
        public int DifficultyID { get; set; } = 0;

        public string _beatmapFilename { get; set; } = "N/A";

        public bool IsEasy()
        {
            if (DifficultyName.ToLower() == "easy") return true;
            else return false;
        }

        public bool IsNormal()
        {
            if (DifficultyName.ToLower() == "normal") return true;
            else return false;
        }

        public bool IsHard()
        {
            if (DifficultyName.ToLower() == "hard") return true;
            else return false;
        }

        public bool IsExpert()
        {
            if (DifficultyName.ToLower() == "expert") return true;
            else return false;
        }

        public bool IsExpertPlus()
        {
            if (DifficultyName.ToLower() == "expertplus") return true;
            else return false;
        }
    }

    public class BeatSaberSongBeatMapCharacteristic
    {
        [JsonPropertyName("_beatmapCharacteristicName")]
        public string BeatMapCharacteristicName { get; set; } = "N/A";
        [JsonPropertyName("_difficultyBeatmaps")]
        public List<BeatSaberSongDifficulty> Difficulties { get; set; } = new List<BeatSaberSongDifficulty>();
    }

    public class WebClientUtilities
    {
        public List<string> UAs = new List<string>() { "BeatSaverUtilities/1.0", "BeatSaverUtils/1.1", "BeatSaverBot/1.2", "MSEdge/5.1", "Firefox/59.2", "Something/5.1", "Random/4.3", "lmao/6.5", "Chrome/4.3", "NET/4.3" };
        public string lastUA = "N/A";

        public string GetRandomUserAgent()
        {
            while (true)
            {
                Random r = new Random();
                int rand = r.Next(0, UAs.Count - 1);
                if (lastUA != UAs[rand])
                {
                    lastUA = UAs[rand];
                    return UAs[rand];
                }
            }
        }
    }

    public class BeatSaverAPIInteractor
    {
        public readonly string BeatSaverAPIBaseLink = "https://beatsaver.com/api/";
        public readonly string BeatSaverLink = "https://beatsaver.com";
        WebClientUtilities WCU = new WebClientUtilities();

        public BeatSaberSong LoadFromInfoDat(String json)
        {
            BeatSaberSong c = JsonSerializer.Deserialize<BeatSaberSong>(json);
            return c;
        }

        internal async Task<BeatSaverAPISong> BeatSaverAPISongHash(string Hash)
        {
            WebClientUtilities WCU = new WebClientUtilities();

            BeatSaverAPISong BeatSaverResult = new BeatSaverAPISong();

            bool RateLimit = true;
            while (RateLimit)
            {
                WebClient cl = new WebClient();
                cl.Headers.Add("user-agent", WCU.GetRandomUserAgent());
                try
                {
                    String tmp = cl.DownloadString(BeatSaverAPIBaseLink + "maps/by-hash/" + Hash.ToLower());
                    BeatSaverResult = JsonSerializer.Deserialize<BeatSaverAPISong>(tmp);
                    BeatSaverResult.GoodRequest = true;
                    RateLimit = false;
                }
                catch (WebException e)
                {
                    String response = ((HttpWebResponse)e.Response).StatusCode.ToString();
                    if (response.Contains("429"))
                    {
                        await Task.Delay(5000);
                    }
                    else if (response.Contains("404"))
                    {
                        BeatSaverResult.GoodRequest = false;
                        RateLimit = false;
                    }
                    else
                    {
                        BeatSaverResult.GoodRequest = false;
                        RateLimit = false;
                    }
                }
            }
            return BeatSaverResult;
        }

        internal async Task<BeatSaverAPISong> BeatSaverAPISongKey(string Key)
        {
            WebClientUtilities WCU = new WebClientUtilities();

            BeatSaverAPISong BeatSaverResult = new BeatSaverAPISong();

            bool RateLimit = true;
            while (RateLimit)
            {
                WebClient cl = new WebClient();
                cl.Headers.Add("user-agent", WCU.GetRandomUserAgent());
                try
                {
                    String tmp = cl.DownloadString(BeatSaverAPIBaseLink + "maps/detail/" + Key.ToLower());
                    BeatSaverResult = JsonSerializer.Deserialize<BeatSaverAPISong>(tmp);
                    RateLimit = false;
                    BeatSaverResult.GoodRequest = true;
                }
                catch (WebException e)
                {
                    String response = ((HttpWebResponse)e.Response).StatusCode.ToString();
                    if (response.Contains("429"))
                    {
                        await Task.Delay(5000);
                    }
                    else if (response.Contains("404"))
                    {
                        BeatSaverResult.GoodRequest = false;
                        RateLimit = false;
                    }
                    else
                    {
                        BeatSaverResult.GoodRequest = false;
                        RateLimit = false;
                    }
                }
            }
            return BeatSaverResult;
        }

        public BeatSaverAPISong GetBeatSaverAPISongViaKey(String key)
        {
            return BeatSaverAPISongKey(key).Result;
        }

        public BeatSaverAPISong GetBeatSaverAPISongViaHash(String hash)
        {
            return BeatSaverAPISongHash(hash).Result;
        }

        public BeatSaverAPISong GetBeatSaverAPISong(String HashOrKey)
        {
            BeatSaverAPISong song = GetBeatSaverAPISongViaKey(HashOrKey);
            if(!song.GoodRequest)
            {
                song = GetBeatSaverAPISongViaHash(HashOrKey);
            }
            return song;
        }

        public BeatSaberSong GetBeatSaberSong(String HashOrKey)
        {
            BeatSaberSong song = GetBeatSaberSongViaKey(HashOrKey);
            if(!song.RequestGood)
            {
                song = GetBeatSaberSongViaHash(HashOrKey);
            }
            return song;
        }

        public BeatSaberSong GetBeatSaberSongViaHash(String Hash)
        {
            BeatSaverAPISong convert = GetBeatSaverAPISongViaHash(Hash);
            return convert.ConvertToBeatSaberSong();
        }

        public BeatSaberSong GetBeatSaberSongViaKey(String Key)
        {
            BeatSaverAPISong convert = GetBeatSaverAPISongViaKey(Key);
            return convert.ConvertToBeatSaberSong();
        }

        internal async Task<BeatSaverAPISearchResult> SearchTextAPI(String text)
        {
            WebClientUtilities WCU = new WebClientUtilities();

            BeatSaverAPISearchResult BeatSaverResult = new BeatSaverAPISearchResult();

            bool RateLimit = true;
            while (RateLimit)
            {
                WebClient cl = new WebClient();
                cl.Headers.Add("user-agent", WCU.GetRandomUserAgent());
                try
                {
                    String tmp = cl.DownloadString(BeatSaverAPIBaseLink + "search/text?q=%22" + text + "%22");
                    BeatSaverResult = JsonSerializer.Deserialize<BeatSaverAPISearchResult>(tmp);
                    RateLimit = false;
                    BeatSaverResult.RequestGood = true;
                }
                catch (WebException e)
                {
                    String response = ((HttpWebResponse)e.Response).StatusCode.ToString();
                    if (response.Contains("429"))
                    {
                        await Task.Delay(5000);
                    }
                    else if (response.Contains("404"))
                    {
                        BeatSaverResult.RequestGood = false;
                        RateLimit = false;
                    }
                    else
                    {
                        BeatSaverResult.RequestGood = false;
                        RateLimit = false;
                    }
                }
            }
            return BeatSaverResult;
        }

        public BeatSaverAPISearchResult SearchText(String text)
        {
            return SearchTextAPI(text).Result;
        }
    }
}
