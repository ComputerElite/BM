using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BeatSaber.Stats
{
    public class PlayerData
    {
        public string version { get; set; } = "";
        public List<LocalPlayer> localPlayers { get; set; } = new List<LocalPlayer>();
    }

    public class LocalPlayer
    {
        public string playerId { get; set; } = "";
        public string playerName { get; set; } = "";
        public PlayerStats playerAllOverallStatsData { get; set; } = new PlayerStats();
    }

    public class PlayerStats
    {
        public ModusStats campaignOverallStatsData { get; set; } = new ModusStats();
        public ModusStats soloFreePlayOverallStatsData { get; set; } = new ModusStats();
        public ModusStats partyFreePlayOverallStatsData { get; set; } = new ModusStats();
    
        public int overallGoodCutsCount { get
            {
                return campaignOverallStatsData.goodCutsCount + soloFreePlayOverallStatsData.goodCutsCount + partyFreePlayOverallStatsData.goodCutsCount;
            }
        }

        public int overallBadCutsCount
        {
            get
            {
                return campaignOverallStatsData.badCutsCount + soloFreePlayOverallStatsData.badCutsCount + partyFreePlayOverallStatsData.badCutsCount;
            }
        }

        public int overallMissedCutsCount
        {
            get
            {
                return campaignOverallStatsData.missedCutsCount + soloFreePlayOverallStatsData.missedCutsCount + partyFreePlayOverallStatsData.missedCutsCount;
            }
        }

        public int overallTotalScore
        {
            get
            {
                return campaignOverallStatsData.totalScore + soloFreePlayOverallStatsData.totalScore + partyFreePlayOverallStatsData.totalScore;
            }
        }

        public int overallPlayedLevelsCount
        {
            get
            {
                return campaignOverallStatsData.playedLevelsCount + soloFreePlayOverallStatsData.playedLevelsCount + partyFreePlayOverallStatsData.playedLevelsCount;
            }
        }

        public int overallCleardLevelsCount
        {
            get
            {
                return campaignOverallStatsData.cleardLevelsCount + soloFreePlayOverallStatsData.cleardLevelsCount + partyFreePlayOverallStatsData.cleardLevelsCount;
            }
        }

        public int overallFailedLevelsCount
        {
            get
            {
                return campaignOverallStatsData.failedLevelsCount + soloFreePlayOverallStatsData.failedLevelsCount + partyFreePlayOverallStatsData.failedLevelsCount;
            }
        }

        public int overallFullComboCount
        {
            get
            {
                return campaignOverallStatsData.fullComboCount + soloFreePlayOverallStatsData.fullComboCount + partyFreePlayOverallStatsData.fullComboCount;
            }
        }

        public double overallTimePlayed
        {
            get
            {
                return campaignOverallStatsData.timePlayed + soloFreePlayOverallStatsData.timePlayed + partyFreePlayOverallStatsData.timePlayed;
            }
        }

        public double overallHandDistanceTravelled
        {
            get
            {
                return campaignOverallStatsData.handDistanceTravelled + soloFreePlayOverallStatsData.handDistanceTravelled + partyFreePlayOverallStatsData.handDistanceTravelled;
            }
        }

        public double overallCummulativeCutScoreWithoutMultiplier
        {
            get
            {
                return campaignOverallStatsData.cummulativeCutScoreWithoutMultiplier + soloFreePlayOverallStatsData.cummulativeCutScoreWithoutMultiplier + partyFreePlayOverallStatsData.cummulativeCutScoreWithoutMultiplier;
            }
        }
    }

    public class ModusStats
    {
        public int goodCutsCount { get; set; } = 0;
        public int badCutsCount { get; set; } = 0;
        public int missedCutsCount { get; set; } = 0;
        public int totalScore { get; set; } = 0;
        public int playedLevelsCount { get; set; } = 0;
        public int cleardLevelsCount { get; set; } = 0;
        public int failedLevelsCount { get; set; } = 0;
        public int fullComboCount { get; set; } = 0;
        public double timePlayed { get; set; } = 0.0;
        public double handDistanceTravelled { get; set; } = 0.0;
        public double cummulativeCutScoreWithoutMultiplier { get; set; } = 0.0;
    }
}
