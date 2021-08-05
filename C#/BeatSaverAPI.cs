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
        public JsonElement _customData { get; set; } = JsonSerializer.Deserialize<JsonElement>("{}");

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
        public bool RequestGood { get; set; } = false;

        public bool HasResults()
        {
            if (docs.Count < 1) return false;
            return true;
        }

        public BeatSaverAPISong GetFirstResult()
        {
            BeatSaverAPISong song = new BeatSaverAPISong();
            if (HasResults())
            {
                song = docs[0];
                song.GoodRequest = true;
            }
            return song;
        }
    }

    public class BeatSaverAPISong
    {
        public BeatSaverAPIMetadata metadata { get; set; } = new BeatSaverAPIMetadata();
        public BeatSaverAPIStats stats { get; set; } = new BeatSaverAPIStats();
        public string description { get; set; } = "N/A";
        public string id { get; set; } = "N/A";
        public string name { get; set; } = "N/A";
        public BeatSaverAPIUploader uploader { get; set; } = new BeatSaverAPIUploader();
        public DateTime uploaded { get; set; } = DateTime.MinValue;
        public bool automapper { get; set; } = false;
        public bool ranked { get; set; } = false;
        public bool qualified { get; set; } = false;
        public List<BeatSaverAPISongVersion> versions { get; set; } = new List<BeatSaverAPISongVersion>();
        public bool GoodRequest { get; set; } = false;

        //public BeatSaberSong ConvertToBeatSaberSong()
        //{
        //    BeatSaberSong finished = new BeatSaberSong();
        //    finished.BPM = metadata.bpm;
        //    finished.Hash = versions[0].hash;
        //    finished.Key = id;
        //    finished.Mapper = metadata.levelAuthorName;
        //    finished.SongName = name;
        //    finished.SongArtist = metadata.songAuthorName;
        //    finished.RequestGood = GoodRequest;
        //    finished.SubName = metadata.songSubName;
        //    finished.RequestGood = GoodRequest;
        //    foreach (BeatSaverAPISongDifficulty diff in versions[0].diffs)
        //    {
        //        if(finished.BeatMapCharacteristics)
        //        BeatSaberSongBeatMapCharacteristic characteristic1 = new BeatSaberSongBeatMapCharacteristic();
        //        characteristic1.BeatMapCharacteristicName = characteristic.name;
        //        if (characteristic.difficulties.easy != null)
        //        {
        //            BeatSaberSongDifficulty easy = new BeatSaberSongDifficulty();
        //            easy.Bombs = characteristic.difficulties.easy.bombs;
        //            easy.DifficultyID = 1;
        //            easy.DifficultyName = "easy";
        //            easy.Duration = characteristic.difficulties.easy.duration;
        //            easy.Length = characteristic.difficulties.easy.length;
        //            easy.NJS = characteristic.difficulties.easy.njs;
        //            easy.NJSOffset = characteristic.difficulties.easy.njsOffset;
        //            easy.Notes = characteristic.difficulties.easy.notes;
        //            easy.Obstacles = characteristic.difficulties.easy.obstacles;
        //            characteristic1.Difficulties.Add(easy);
        //        }
        //        if (characteristic.difficulties.normal != null)
        //        {
        //            BeatSaberSongDifficulty normal = new BeatSaberSongDifficulty();
        //            normal.Bombs = characteristic.difficulties.normal.bombs;
        //            normal.DifficultyID = 3;
        //            normal.DifficultyName = "normal";
        //            normal.Duration = characteristic.difficulties.normal.duration;
        //            normal.Length = characteristic.difficulties.normal.length;
        //            normal.NJS = characteristic.difficulties.normal.njs;
        //            normal.NJSOffset = characteristic.difficulties.normal.njsOffset;
        //            normal.Notes = characteristic.difficulties.normal.notes;
        //            normal.Obstacles = characteristic.difficulties.normal.obstacles;
        //            characteristic1.Difficulties.Add(normal);
        //        }
        //        if (characteristic.difficulties.hard != null)
        //        {
        //            BeatSaberSongDifficulty hard = new BeatSaberSongDifficulty();
        //            hard.Bombs = characteristic.difficulties.hard.bombs;
        //            hard.DifficultyID = 5;
        //            hard.DifficultyName = "hard";
        //            hard.Duration = characteristic.difficulties.hard.duration;
        //            hard.Length = characteristic.difficulties.hard.length;
        //            hard.NJS = characteristic.difficulties.hard.njs;
        //            hard.NJSOffset = characteristic.difficulties.hard.njsOffset;
        //            hard.Notes = characteristic.difficulties.hard.notes;
        //            hard.Obstacles = characteristic.difficulties.hard.obstacles;
        //            characteristic1.Difficulties.Add(hard);
        //        }
        //        if (characteristic.difficulties.expert != null)
        //        {
        //            BeatSaberSongDifficulty expert = new BeatSaberSongDifficulty();
        //            expert.Bombs = characteristic.difficulties.expert.bombs;
        //            expert.DifficultyID = 7;
        //            expert.DifficultyName = "expert";
        //            expert.Duration = characteristic.difficulties.expert.duration;
        //            expert.Length = characteristic.difficulties.expert.length;
        //            expert.NJS = characteristic.difficulties.expert.njs;
        //            expert.NJSOffset = characteristic.difficulties.expert.njsOffset;
        //            expert.Notes = characteristic.difficulties.expert.notes;
        //            expert.Obstacles = characteristic.difficulties.expert.obstacles;
        //            characteristic1.Difficulties.Add(expert);
        //        }
        //        if (characteristic.difficulties.expertPlus != null)
        //        {
        //            BeatSaberSongDifficulty expertPlus = new BeatSaberSongDifficulty();
        //            expertPlus.Bombs = characteristic.difficulties.expertPlus.bombs;
        //            expertPlus.DifficultyID = 9;
        //            expertPlus.DifficultyName = "expertPlus";
        //            expertPlus.Duration = characteristic.difficulties.expertPlus.duration;
        //            expertPlus.Length = characteristic.difficulties.expertPlus.length;
        //            expertPlus.NJS = characteristic.difficulties.expertPlus.njs;
        //            expertPlus.NJSOffset = characteristic.difficulties.expertPlus.njsOffset;
        //            expertPlus.Notes = characteristic.difficulties.expertPlus.notes;
        //            expertPlus.Obstacles = characteristic.difficulties.expertPlus.obstacles;
        //            characteristic1.Difficulties.Add(expertPlus);
        //        }
        //        finished.BeatMapCharacteristics.Add(characteristic1);
        //    }
        //    return finished;
        //}
    }

    public class BeatSaverAPIStats
    {
        public int downloads { get; set; } = 0;
        public int plays { get; set; } = 0;
        public int downVotes { get; set; } = 0;
        public int upVotes { get; set; } = 0;
        public decimal score { get; set; } = 0.0m;
    }

    public class BeatSaverAPIUploader
    {
        public string _id { get; set; } = "N/A";
        public string username { get; set; } = "N/A";
        public string hash { get; set; } = "N/A";
        public string avatar { get; set; } = "N/A";
    }

    public class BeatSaverAPIMetadata
    {
        public decimal duration { get; set; } = 0.0m;
        public string levelAuthorName { get; set; } = "N/A";
        public string songAuthorName { get; set; } = "N/A";
        public string songName { get; set; } = "N/A";
        public string songSubName { get; set; } = "N/A";
        public decimal bpm { get; set; } = 0.0m;
    }

    public class BeatSaverAPISongVersion
    {
        public string hash { get; set; } = "";
        public string state { get; set; } = "Published";
        public DateTime createdAt { get; set; } = DateTime.MinValue;
        public int sageScore { get; set; } = 0;
        public List<BeatSaverAPISongDifficulty> diffs { get; set; } = new List<BeatSaverAPISongDifficulty>();
        public string downloadURL { get; set; } = "";
        public string coverURL { get; set; } = "";
        public string previewURL { get; set; } = "";
    }

    public class BeatSaverAPISongDifficulty
    {
        public decimal njs { get; set; } = 0.0m;
        public decimal offset { get; set; } = 0.0m;
        public int notes { get; set; } = 0;
        public int bombs { get; set; } = 0;
        public int obstacles { get; set; } = 0;
        public decimal nps { get; set; } = 0.0m;
        public decimal length { get; set; } = 0.0m;
        public string characteristic { get; set; } = "";
        public string difficulty { get; set; } = "";
        public int events { get; set; } = 0;
        public bool chroma { get; set; } = false;
        public bool me { get; set; } = false;
        public bool ne { get; set; } = false;
        public bool cinema { get; set; } = false;
        public BeatSaverAPISongParitySummary paritySummary { get; set; } = new BeatSaverAPISongParitySummary();
        public bool IsLawless()
        {
            if (characteristic.ToLower() == "lawless") return true;
            return false;
        }

        public bool IsLightshow()
        {
            if (characteristic.ToLower() == "lightshow") return true;
            return false;
        }

        public bool IsStandart()
        {
            if (characteristic.ToLower() == "standard") return true;
            return false;
        }

        public bool IsNoArrows()
        {
            if (characteristic.ToLower() == "noarrows") return true;
            return false;
        }

        public bool IsOneSaber()
        {
            if (characteristic.ToLower() == "onesaber") return true;
            return false;
        }

        public bool IsNinetyDegree()
        {
            if (characteristic.ToLower() == "90degree") return true;
            return false;
        }

        public bool IsThreeSixtyDegree()
        {
            if (characteristic.ToLower() == "360degree") return true;
            return false;
        }
    }

    public class BeatSaverAPISongParitySummary
    {
        public int errors { get; set; } = 0;
        public int warns { get; set; } = 0;
        public int resets { get; set; } = 0;
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
        public JsonElement _customData { get; set; } = JsonSerializer.Deserialize<JsonElement>("{}");

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

    public class BeatSaverAPIInteractor
    {
        public readonly string BeatSaverAPIBaseLink = "https://api.beatmaps.io/";
        public readonly string BeatSaverLink = "https://beatmaps.io";

        public BeatSaberSong LoadFromInfoDat(String json)
        {
            BeatSaberSong c = JsonSerializer.Deserialize<BeatSaberSong>(json);
            return c;
        }

        internal async Task<BeatSaverAPISong> BeatSaverAPISongHash(string Hash)
        {
            BeatSaverAPISong BeatSaverResult = new BeatSaverAPISong();

            bool RateLimit = true;
            while (RateLimit)
            {
                WebClient cl = new WebClient();
                cl.Headers.Add("user-agent", "BeatSaverAPIInteractor/1.0");
                try
                {
                    String tmp = cl.DownloadString(BeatSaverAPIBaseLink + "maps/hash/" + Hash.ToLower());
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
            BeatSaverAPISong BeatSaverResult = new BeatSaverAPISong();

            bool RateLimit = true;
            while (RateLimit)
            {
                WebClient cl = new WebClient();
                cl.Headers.Add("user-agent", "BeatSaverAPIInteractor/1.0");
                try
                {
                    String tmp = cl.DownloadString(BeatSaverAPIBaseLink + "maps/beatsaver/" + Key.ToLower());
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
            try
            {
                BeatSaverAPISong s = BeatSaverAPISongKey(key).Result;
                return s;
            } catch
            {
                return new BeatSaverAPISong();
            }
        }

        public BeatSaverAPISong GetBeatSaverAPISongViaHash(String hash)
        {
            try
            {
                BeatSaverAPISong s = BeatSaverAPISongHash(hash).Result;
                return s;
            } catch
            {
                return new BeatSaverAPISong();
            }
        }

        public BeatSaverAPISong GetBeatSaverAPISong(String HashOrKey)
        {
            BeatSaverAPISong song = GetBeatSaverAPISongViaKey(HashOrKey);
            if (!song.GoodRequest)
            {
                song = GetBeatSaverAPISongViaHash(HashOrKey);
            }
            return song;
        }

        //public BeatSaberSong GetBeatSaberSong(String HashOrKey)
        //{
        //    BeatSaberSong song = GetBeatSaberSongViaKey(HashOrKey);
        //    if (!song.RequestGood)
        //    {
        //        song = GetBeatSaberSongViaHash(HashOrKey);
        //    }
        //    return song;
        //}

        //public BeatSaberSong GetBeatSaberSongViaHash(String Hash)
        //{
        //    BeatSaverAPISong convert = GetBeatSaverAPISongViaHash(Hash);
        //    return convert.ConvertToBeatSaberSong();
        //}

        //public BeatSaberSong GetBeatSaberSongViaKey(String Key)
        //{
        //    BeatSaverAPISong convert = GetBeatSaverAPISongViaKey(Key);
        //    return convert.ConvertToBeatSaberSong();
        //}

        internal async Task<BeatSaverAPISearchResult> SearchTextAPI(String text)
        {

            BeatSaverAPISearchResult BeatSaverResult = new BeatSaverAPISearchResult();

            bool RateLimit = true;
            while (RateLimit)
            {
                WebClient cl = new WebClient();
                cl.Headers.Add("user-agent", "BeatSaverAPIInteractor/1.0");
                try
                {
                    String tmp = cl.DownloadString(BeatSaverAPIBaseLink + "search/text/0?q=" + text + "&sortOrder=Latest");
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
