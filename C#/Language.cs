using System;

namespace BMBFManager.Language
{
    public class Language
    {
        public String language { get; set; } = "english";
        public String translator { get; set; } = "N/A";
        public MainMenu mainMenu { get; set; } = new MainMenu();
        public BBBU bBBU { get; set; } = new BBBU();
        public BPLists bPLists { get; set; } = new BPLists();
        public HitSounds hitSounds { get; set; } = new HitSounds();
        public Mods mods { get; set; } = new Mods();
        public PlaylistEditor playlistEditor { get; set; } = new PlaylistEditor();
        public Qosmetics qosmetics { get; set; } = new Qosmetics();
        public QSU qSU { get; set; } = new QSU();
        public Songs songs { get; set; } = new Songs();
        public Settings settings { get; set; } = new Settings();
        public Global global { get; set; } = new Global();
        public VariableProcesser processer = new VariableProcesser();
    }

    public class VariableProcesser
    {
        public String ReturnProcessed(String input, String arg1 = "", String arg2 = "", String arg3 = "", String arg4 = "", String arg5 = "", String arg6 = "", String arg7 = "", String arg8 = "")
        {
            return input.Replace("{0}", arg1).Replace("{1}", arg2).Replace("{2}", arg3).Replace("{3}", arg4).Replace("{4}", arg5).Replace("{5}", arg6).Replace("{6}", arg7).Replace("{7}", arg8);
        }
    }

    public class Global
    {
        public String defaultQuestIPText { get; set; } = "Quest IP";
        public String defaultOutputBoxText { get; set; } = "Output:";
        public String ipInvalid { get; set; } = "Please Type a valid IP";
        public String numberNotValid { get; set; } = "Please Type in a valid number";
        public String syncingToBS { get; set; } = "Syncing to Beat Saber";
        public String syncedToBS { get; set; } = "Synced to Beat Saber";
        public String anErrorOccured { get; set; } = "An Error Occured";
        public String allFinished { get; set; } = "All finished";

        public String BMBF100 { get; set; } = "\n\n\nAn error occured (Code: BMBF100). Check following:\n\n- Your Quest is on and BMBF opened\n\n- You put in the Quests IP right.\n\n- Your Quest hasn't gone into sleep";
        public String BMBF110 { get; set; } = "\n\n\nA error Occured (Code: BMBF110). Please try to sync manually.";
        public String ADB100 { get; set; } = "\n\n\nAn error Occured (Code: ADB100). Check following:\n\n- You have adb installed.";
        public String ADB110 { get; set; } = "\n\n\nAn error Occured (Code: ADB110). Check following:\n\n- Your Quest is connected, Developer Mode enabled and USB Debugging enabled.";
        public String PL100 { get; set; } = "\n\n\nAn error occured (Code: PL100). Check following:\n\n- You put in the Quests IP right.\n\n- You've choosen a Backup Name.\n\n- Your Quest is on.";
        public String UD100 { get; set; } = "\n\n\nAn error Occured (Code: UD100). Couldn't check for Updates. Check following:\n\n- Your PC has internet.";
        public String UD200 { get; set; } = "\n\n\nAn error Occured (Code: UD200). Couldn't download Update.";
        public String QSU100 { get; set; } = "\n\n\nAn error Occured (Code: QSU100). No Songs were zipped.";
        public String QSU110 { get; set; } = "\n\n\nAn error Occured (Code: QSU110). No Songs were zipped.";
        public String BM100 { get; set; } = "\n\n\nAn error Occured (Code: BM100). Couldn't reach the QuestBoard Website to get some available Mods. Nothing crucial.";
        public String BM200 { get; set; } = "\n\n\nAn error Occured (Code: BM200). Couldn't download Mod";
    }

    public class MainMenu
    {

        public MainMenuUI UI { get; set; } = new MainMenuUI();
        public MainMenuCode code { get; set; } = new MainMenuCode();
    }

    public class MainMenuCode
    {
        public String newBMBFAvailable { get; set; } = "New BMBF version available to download!\nYour current BMBF version is {0}\nThe new version is {1}\n";
        public String onNewestBMBF { get; set; } = "You are on the newest BMBF version";

        // Player Stats aren't used anymore
        public String tryPullPlayerStats { get; set; } = "Trying to pull Player Stats";
        public String questNotConnectedNoPlayerStats { get; set; } = "Quest isn't connected. Not displaying Player stats";
        public String pullPlayerStatsFailed { get; set; } = "Couldn't pull PlayerStats from Quest. Not displaying Player stats";
        public String noPlayerStatsSaved { get; set; }  = "No stats saved.";
        public String overallStats { get; set; }  = "Overall Stats:";
        public String goodCuts { get; set; } = "- Good Cuts count: ";
        public String badCuts { get; set; } = "- Bad Cuts count: ";
        public String missedCuts { get; set; } = "- Missed Cuts count: ";
        public String totalScore { get; set; } = "- Total Score: ";
        public String totalPlayTime { get; set; } = "- Total Time Played: ";
        public String totalHandDistance { get; set; } = "- Total Hand distance travelled: ";
        public String playedLevels { get; set; } = "- Played levels: ";
        public String clearedLevels { get; set; } = "- Cleared levels: ";
        public String failedLevels { get; set; } = "- Failed levels: ";
        public String fullCombo { get; set; } = "- Full Combo count: ";

        //Changelog
        public String updateChangelog { get; set; } = "You installed a Update (Version: {0}).\n\nUpdate posted by: {1}\n\nChangelog:\n{2}";

        //CustomProtocol
        public String customLinksEnabled { get; set; } = "Custom Links enabled";
        public String registryUnableToChangeNoCustomLinks { get; set; } = "Registry was unable to change... no Custom protocol enabled.";

        //Updates (NEVER FORGET THE "xD"!)
        public String previewVersion { get; set; } = "Looks like you have a preview version. Downgrade now from {0} to {1} xD";
        public String downgradeNow { get; set; } = "Downgrade Now xD";
        public String releaseVersionOut { get; set; } = "Looks like you have a preview version. The release version has been released. Please Update now.";

        //BMBF Updates
        public String operationRunning { get; set; } = "A operation is already running. Please try again after it has finished.";
        public String onQuest2 { get; set; } = "Are you on an Oculus Quest 2?";
        public String moddedBSDetected { get; set; } = "Modded Beat Saber has been detected. If you press yes I'll uninstall Beat Saber and BMBF and make a Backup of it to restore. If you press no you'll cancle Updating.";
        public String bMBFUpdatatingAborted { get; set; } = "BMBF Updating aborted.";
        public String playlistBackup { get; set; } = "Backing up Playlist to {0}";
        public String playlistBackupFinished { get; set; } = "Backed up Playlists to {0}";
        public String downloadBS { get; set; } = "Please download Beat Saber from the oculus store, play a song and then close it. Press OK once you finished.";
        public String makingSure { get; set; } = "I want to make sure. Do you have unmodded Beat Saber installed and opened it at least once?";
        public String bMBFUpdatatingAbortedInstallBS { get; set; } = "BMBF Updating aborted. Please Install unmodded Beat Saber and start it once";
        public String unmoddedBSDetected { get; set; } = "Looks like you have unmodded Beat Saber installed. Did you open it at least once?";
        public String newestBMBFDoesntWork { get; set; } = "The newest BMBF Version ({0}) doesn't work for many people. I'd suggest you update to a more stable version. The last entry that's not listed as not working is BMBF version {1}.\nIf you want to install the recommended version of BMBF press yes. If you want to install the newest one press no.";
        public String downloadingNewestBMBFVersion { get; set; } = "Downloading newest BMBF version";
        public String downloadingRecommendedBMBFVersion { get; set; } = "Downloading recommended BMBF version";
        public String downloadComplete { get; set; } = "Download Complete";
        public String installingBMBF { get; set; } = "Installing new BMBF";
        public String moddingBS { get; set; } = "Modding Beat Saber. Please wait...";
        public String userOnQ2Reminder { get; set; } = "You told me you were on Quest 2. I installed BMBF for you but DIDN'T Mod Beat Saber. You'll have to continue from here manually. I'm currently not able to automate it on Quest 2.\nOnce you modded Beat Saber press OK.";
        public String tryRestoreSaveData { get; set; } = "I'm now attempting to restore Beat Saber save data. \nDid you mod Beat Saber, started it once an want me to restore it?";
        public String q2BMBFInstallFinished { get; set; } = "Ok, I've finished my part. Have fun now!";
        public String stepFinished { get; set; } = "Step {0} finished";
        public String bMBFInstallationFinished { get; set; } = "Finished Installing BMBF and modding Beat Saber. Please click \"Reload Songs Folder\" in BMBF to reload your Songs if you Updated BMBF.";
        public String playlistsRestored { get; set; } = "Restored old Playlists.";
        public String pushingPng { get; set; } = "Pushing {0} to Quest";

        //SwitchVersion
        public String lastActionUninstallingBS { get; set; } = "It looks like your last Action was installing unmodded Beat Saber. If you continue and have unmodded Beat Saber installed you must mod Beat Saber By hand.\nDo you wish to continue?";
        public String aborted { get; set; } = "Aborted.";
        public String unmodBS { get; set; } = "I'll unmod Beat Saber for you.\nDo you want to proceed?";
        public String backingUpAll { get; set; } = "Backing up everything.";
        public String installingUnmodded { get; set; } = "Installing unmodded Beat Saber.";
        public String restoringScores { get; set; } = "Restoring Scores";
        public String finishedVanilla { get; set; } = "Finished. You can now play vanilla Beat Saber.";
        public String modGame { get; set; } = "Please Click \"Install/Update BMBF\" to mod Beat Saber the first time.";
        public String switchToModded { get; set; } = "I'll switch back to the modded Version of Beat Saber for you.\nDo you want to proceed?";
        public String uninstallingBS { get; set; } = "Uninstalling Beat Saber.";
        public String installingModded { get; set; } = "Installing Modded Beat Saber";
        public String restoringSaveData { get; set; } = "Restoring Game Data";
        public String grantingPerms { get; set; } = "Granting Permissions";
        public String finishedModded { get; set; } = "finished. You can now play your Custom Songs again.";

        //Open BMBF
        public String bMBFNotReachable { get; set; } = "I couldn't reach BMBF. The IP you typed is: \"{0}\". Is this right? If it is check that BMBF is opened on your Quest and that your Quest and PC are on the same Wifi network.";
    }

    public class MainMenuUI
    {
        public String updateButton { get; set; } = "Update";
        public String installSongsButton { get; set; } = "Install Songs";
        public String installModsButton { get; set; } = "Install Mods";
        public String updateBMBFButton { get; set; } = "Update/Install BMBF";
        public String switchButton { get; set; } = "Switch between modded and\nunmodded Beat Saber";
        public String downloadBPListsButton { get; set; } = "Download BPLists";
        public String openBMBFButton { get; set; } = "Open BMBF";
        public String installSoundsButton { get; set; } = "Install Sounds";
        public String bBBUButton { get; set; } = "BMBF and Beat Saber\nBackup Utility";
        public String qSUButton { get; set; } = "Quest Song Utilities";
        public String installQosmeticsButton { get; set; } = "Install Qosmetics";
        public String playlistEditorButton { get; set; } = "Playlist Editor";
        public String settingsButton { get; set; } = "Settings";
    }

    public class BBBU
    {
        public BBBUUI UI { get; set; } = new BBBUUI();
        public BBBUCode code { get; set; } = new BBBUCode();
    }

    public class BBBUCode
    {
        //Import stuff from BBBU
        public String importBBBUQuestion { get; set; } = "Hi. I'm asking you if I should import Backups from BMBF Beat Saber Backup Utility. Only click yes if you've used the seperate program before. You can always import again if you wish to from the settings.";
        public String nothingImported { get; set; } = "Nothing Imported";
        public String selectBBBUFolderInfo { get; set; } = "I'll open a window for you. Please choose the folder in which your BMBF Beat Saber Backup Utility Installation is located. I'll then transfer all Backups";
        public String selectValidDir { get; set; } = "Please select a valid Directory";
        public String movedBackup { get; set; } = "Moved Backup {0}";
        public String backupsMoved { get; set; } = "All Backups moved";

        //Backup
        public String advancedBackupWarning { get; set; } = "This Backup Method will Backup the Beat Saber APK and BMBF APK as well. If you don't make another Backup before you restore this Backup you have to mod Beat Saber again. Only do this when you know what you're doing. Note: This has only been tested on Quest 1. If you are on Quest 2 feel free to contact me and say if it worked.\nDo you want to continue?";
        public String backupAborted { get; set; } = "Backup Aborted.";
        public String backupExists { get; set; } = "This Backup already exists!";
        public String backingUpScores { get; set; } = "Backing up scores";
        public String backedUpScores { get; set; } = "Backed up scores";
        public String backingUpModData { get; set; } = "Backing up ModData";
        public String backedUpModData { get; set; } = "Backed up ModData";
        public String bMBFAPKBackupFailed { get; set; } = "I couldn't make a BMBF APK Backup. If you want to restore to this game Version you may want to install the right BMBF Version.";
        public String bSAPKBackupFailed { get; set; } = "I couldn't make a Beat Saber APK Backup. If you want to restore to this game Version it will not work.";
        public String backupMade { get; set; } = "BMBF and Beat Saber Backup has been made.";
        public String backingUpBSAPK { get; set; } = "Backing up Beat Saber APK";
        public String backedUpBSAPK { get; set; } = "Backed up Beat Saber APK";
        public String backingUpBMBFAPK { get; set; } = "Backing up BMBF APK";
        public String backedUpBMBFAPK { get; set; } = "Backed up BMBF APK";
        public String backingUpGameData { get; set; } = "Backing up Game Data";
        public String backedUpGameData { get; set; } = "Backed up Game Data";
        public String copyingModsToTMP { get; set; } = "Copying all Mods to {0}. Please be patient.";
        public String modName { get; set; } = "Mod Name: {0}";
        public String folder { get; set; } = "Folder: {0}";
        public String fileOverwritten { get; set; } = "overwritten file: {0}";
        public String finishedModBackup { get; set; } = "Finished! Backed up {0} Mods";
        public String copyingSongsToTMP { get; set; } = "Copying all Songs to {0}. Please be patient.";
        public String isNotSongs { get; set; } = "{0} is no Song";
        public String songName { get; set; } = "Song Name: {0}";
        public String finishedSongBackup { get; set; } = "Finished! Backed up {0} Songs";
        public String backupName { get; set; } = "Backups";
        public String backupNameName { get; set; } = "Backup Name";

        //Restore
        public String selectValidBackup { get; set; } = "Select a valid Backup!";
        public String rAPKWarning { get; set; } = "You choose to restore the Beat Saber APK. This will install another Beat Saber version. If you didn't make a Backup you have to mod Beat Saber again. If you want to go back to the current Beat Saber Version make a Advanced Backup first. Also if you restore, BMBF Mods will be messed up a bit. Note: This has only been tested on Quest 1. If you are on Quest 2 feel free to contact me and say if it worked.\nDo you wish to abort?";
        public String restoringAborted { get; set; } = "Restoring aborted.";
        public String pushingScores { get; set; } = "Pushing Scores";
        public String pushedScores { get; set; } = "Pushed Scores";
        public String pushingModData { get; set; } = "Pushing ModData";
        public String pushedModData { get; set; } = "Pushed ModData";
        public String uploadingSongs { get; set; } = "Uploading Songs";
        public String uploadedSongs { get; set; } = "Uploaded Songs";
        public String pushingSongs { get; set; } = "Pushing Songs";
        public String pushedSongs { get; set; } = "Pushed Songs";
        public String uploadingMods { get; set; } = "Uploading Mods"; 
        public String uploadedMods { get; set; } = "Uploaded Mods";
        public String restored { get; set; } = "BMBF and Beat Saber has been restored with the selected components.";
        public String bSAPKNotFound { get; set; } = "Your Backup doesn't contain any Beat Saber APK backup. I must abort to prevent anything from going wrong.";
        public String aPKRestoringAborted { get; set; } = "APK Backup Restoring Aborted.";
        public String installingBS { get; set; } = "Installing Beat Saber";
        public String installedBS { get; set; } = "Installed Beat Saber";
        public String installingBMBF { get; set; } = "Installing BMBF";
        public String installedBMBF { get; set; } = "Installed BMBF";
        public String restoringGameData { get; set; } = "Restoring Game Data";
        public String restoredGameData { get; set; } = "Restored Game Data";
        public String restoringPlaylists { get; set; } = "Restoring Playlist from {0}";
        public String uploadingToBMBF { get; set; } = "Uploading {0} to BMBF";
    }

    public class BBBUUI
    {
        public String backupButton { get; set; } = "Backup";
        public String AdvancedBackupButton { get; set; } = "Advanced backup";
        public String restoreSongsBox { get; set; } = "Restore Songs";
        public String restorePlaylistsBox { get; set; } = "Restore Playlists";
        public String restoreScoresBox { get; set; } = "Restore Scores";
        public String restoreModsBox { get; set; } = "Restore Mods";
        public String restoreModDataBox { get; set; } = "Restore ModData\n(includes configs\nand way more)";
        public String restoreVersionBox { get; set; } = "Restore old Game Version";
        public String restoreButton { get; set; } = "Restore";
    }

    public class BPLists
    {
        public BPListsUI UI { get; set; } = new BPListsUI();
        public BPListsCode code { get; set; } = new BPListsCode();
    }

    public class BPListsUI
    {
        public String installRankedButton { get; set; } = "Install ranked Songs\nfrom ScoreSaber";
        public String installBookmarkedButton { get; set; } = "Install Bookmarked Songs\nof User";
        public String maxSongsText { get; set; } = "Max Songs";
        public String usernameText { get; set; } = "Username";
    }

    public class BPListsCode
    {
        public String finishedDownload { get; set; } = "Finished Downloading Data";
        public String bplistDownloadRunning { get; set; } = "A BPList download is already running. Please Wait";
        public String makingBookmarks { get; set; } = "Making BPList of Bookmarked songs of User {0}";
        
        public String noSongsBookmarked { get; set; } = "The User {0} doesn't have any Songs Bookmarked.";
        public String makingRanked { get; set; } = "Creating BPList with ranked Songs.";
        public String uploadingBPList { get; set; } = "Uploading BPList to BMBF";
    }

    public class HitSounds
    {
        public HitSoundsUI UI { get; set; } = new HitSoundsUI();
        public HitSoundsCode code { get; set; } = new HitSoundsCode();
    }

    public class HitSoundsUI
    {
        public String chooseSoundButton { get; set; } = "Choose Sound";
        public String hitSoundText { get; set; } = "As HitSound";
        public String badHitSoundText { get; set; } = "As BadHitSound";
        public String menuMusicText { get; set; } = "As Menu Music";
        public String menuClickText { get; set; } = "As Menu Click";
        public String highscoreText { get; set; } = "As Highscore Music";
        public String levelClearedText { get; set; } = "As Level cleared";
        public String installSoundButton { get; set; } = "Install Sound";
        public String defaultButton { get; set; } = "Change Sound to Default";
    }

    public class HitSoundsCode
    {
        public String selectValidFile { get; set; } = "Please select a valid file";
        public String soundFile { get; set; } = "Sound Files";
        public String nothing { get; set; } = "Nothing";
        public String changingToDefault { get; set; } = "Changing Sound to default";
        public String configUnableToChange { get; set; } = "Do you have your Quest plugged into your PC? Do you have the QuestSounds mod installed? I was unable to change the config";
        public String chooseASoundType { get; set; } = "Please choose a Sound Type.";
        public String changedToDefault { get; set; } = "Changed Sound to default";
        public String selectSound { get; set; } = "Please select a sound";
        public String qsoundsInstalled { get; set; } = "Checking if QuestSounds is installed.";
        public String willInstallQSounds { get; set; } = "I'll install QuestSounds for you";
        public String checkIfQSoundsWorks { get; set; } = "Please start Beat Saber and check if it works. Then press the install button again.";
        public String DoYouHaveQSounds { get; set; } = "Do you have the QuestSounds mod installed?";
        public String changingSound { get; set; } = "Changing Sound";
        public String changedSound { get; set; } = "Sound changed";
    }

    public class Mods
    {
        public ModsUI UI { get; set; } = new ModsUI();
        public ModsCode code { get; set; } = new ModsCode();
    }

    public class ModsUI
    {
        public String moreInfoButton { get; set; } = "More Info";
        public String updateAllModsButton { get; set; } = "Update all Mods";
        public String installModButton { get; set; } = "Install/Update selected Mod";
        public String ModNameList { get; set; } = "Mod Name";
        public String ModCreatorList { get; set; } = "Creator(s)";
        public String ModInstalledList { get; set; } = "installed";
        public String ModLatestList { get; set; } = "latest";
        public String ModGameVersionList { get; set; } = "Game Version";
    }

    public class ModsCode
    {
        public String couldntReachBMBFForVersion { get; set; } = "I couldn't reach BMBF. All the mods displayed are for the last Version of BMBF you used while I noticed ({0}). Please check if you can reach BMBF so I can install mods.";
        public String couldntFindMods { get; set; } = "I couldn't find {0} for your Game Version";
        public String modInfo { get; set; } = "Mod Name: {0}\n\nDescription:\n{1}\n\nNotes for the latest mod release:\n{2}";
        public String manualInstall { get; set; } = "You have to download and install the mod {0} manually. If you click yes I'll redirect you to the download page and open BMBF for you.\nDo you wish to continue?";
        public String coreMod { get; set; } = "The Mod you are about to install is a Core Mod. That means the Mod should get installed when you exit BMBF and open it again. Please make sure you DON'T have the mod installed. I'll open BMBF for you once you click OK.";
        public String isCoreModInstalled { get; set; } = "Do you have the mod {0} installed?";
        public String alreadyInstalledAbort { get; set; } = "Mod is already installed aborted.";
        public String oldVerInstall { get; set; } = "The latest Version of the Mod {0} (That is indexed) has been made for Beat Saber Version {1}. may be compatible with your Game but you have to enable it manually. I'll open the BMBF mod tab after installing the mod. For it to activate you scroll to the mod you installed and flip the switch to on. If you get a compatibility warning click \"Enable Mod\" and then click \"Sync to Beat Saber\" in the top right.\nDo you wish to continue?";
        public String installAborted { get; set; } = "Mod Installing Aborted.";
        public String downloadingMod { get; set; } = "Downloading {0}";
        public String selectMod { get; set; } = "Please select a mod";
        public String alreadyInQueue { get; set; } = "{0} is already in the download queue";
        public String addedToQueue { get; set; } = "{0} was added to the queue";
        public String updateAddedToQueue { get; set; } = "Added {0} version {1} to queue";
        public String downloadedMod { get; set; } = "Downloaded Mod {0}";
        public String uploadingToBMBF { get; set; } = "Uploading {0} to BMBF";
        public String syncedToQuest { get; set; } = "Mod {0} was synced to your Quest.";
        public String unableToSync { get; set; } = "Couldn't sync with BeatSaber. Needs to be done manually.";
        public String enableManually { get; set; } = "Since you choose to install this mod you need to enable it manually. I uploaded it.";
    }

    public class PlaylistEditor
    {
        public PlaylistEditorUI UI { get; set; } = new PlaylistEditorUI();
        public PlaylistEditorCode code { get; set; } = new PlaylistEditorCode();
    }

    public class PlaylistEditorUI
    {
        public String loadPlaylistsButton { get; set; } = "Load Playlists";
        public String savePlaylistsButton { get; set; } = "Save Playlists";
        public String exportBPListButton { get; set; } = "Export Playlist as BPList";
        public String createPlaylistButton { get; set; } = "Create new Playlist";
        public String renamePlaylistButton { get; set; } = "Rename selected Playlist";
        public String deletePlaylistButton { get; set; } = "Delete Playlist";
        public String changeCoverButton { get; set; } = "Change Playlist Cover";
        public String importBPListButton { get; set; } = "Import BPList";
        public String movePlaylistLeftButton { get; set; } = "<< Playlist";
        public String moveSongLeftButton { get; set; } = "< Song";
        public String movePlaylistRightButton { get; set; } = "Playlist >>";
        public String moveSongRightButton { get; set; } = "Song >";
        public String deleteSongButton { get; set; } = "Delete Song";
        public String beastSaberButton { get; set; } = "Show Song on\nBeastSaber";
        public String beatSaverButton { get; set; } = "Show Song on\nBeatSaver";
        public String scoreSaberButton { get; set; } = "Search Song on\nScoreSaber";
        public String previewButton { get; set; } = "Show Song\nPreview";
        public String sortNameButton { get; set; } = "Name";
        public String sortArtistButton { get; set; } = "Artist";
        public String sortMapperButton { get; set; } = "Mapper";
        public String sortByText { get; set; } = "Sort by";
        public String songsCounter { get; set; } = "{0} Song(s)";
        public String totalSongs { get; set; } = "{0} Total Song(s)";
        public String amountSongs { get; set; } = "Amount of Songs";
        public String playlistName { get; set; } = "Playlist Name";
        public String songNameList { get; set; } = "Song Name";
        public String artistList { get; set; } = "Artist";
        public String mapperList { get; set; } = "Mapper";
        public String chooseSong { get; set; } = "Choose a Song";
    }

    public class PlaylistEditorCode
    {
        public String pENotes { get; set; } = "Some Notes for the Playlist Editor: Currently it is not possible to sort songs (the sorting is only for you). If you experience any issues hit me up on Discord.";
        public String loadPlaylists { get; set; } = "Please load your Playlists.";
        public String somethingWentWrong { get; set; } = "Something went wrong";
        public String playlistDoesntContainSongs { get; set; } = "The Playlists {0} doesn't contain any Songs.";
        public String couldntGetCover { get; set; } = "I couldn't get the Playlist Cover";
        public String songMustBeSelected { get; set; } = "You must have a Song selected";
        public String oSTDeletingNotAllowed { get; set; } = "I'll not allow you to delete any OST Song.";
        public String sureDeleteSong { get; set; } = "Are you sure you want to delete {0}? This can NOT be undone after saving!";
        public String deletingSongAborted { get; set; } = "Deleting of {0} aborted.";
        public String mustHavePlaylistSelected { get; set; } = "You must have a Playlist selected";
        public String notAllowedToDeleteCustomSongs { get; set; } = "I'll not allow you to delete the CustomSongs Playlist.";
        public String oSTPlaylistDeletingNotAllowed { get; set; } = "I'll not allow you to delete any OST Playlist ({0}) for your own safety.";
        public String sureDeletePlaylist { get; set; } = "Are you sure you want to delete the Playlist {0}? This can NOT be undone after saving!";
        public String deletingPlaylistAborted { get; set; } = "Deleting of {0} aborted.";
        public String oSTMovingNotAllowed { get; set; } = "I'll not allow you to move any OST Song ({0}) to prevent issues.";
        public String beatSaverLookupFailed { get; set; } = "I couldn't look up {0} on BeatSaver";
        public String downloadingPlaylistCover { get; set; } = "Downloading Playlist Cover";

        //BPList Importing
        public String exportedBPList { get; set; } = "Exported BPList to /BPLists/{0}.bplist with {1} songs.";
        public String selectValidFile { get; set; } = "Please select a valid file";
        public String bPListImportingAborted { get; set; } = "BPList importing aborted.";
        public String bPListNotValid { get; set; } = "The BPList you choose is not valid";
        public String bPListEmpty { get; set; } = "The BPList doesn't contain any songs.";
        public String removedBCMissingHash { get; set; } = "Removed {0} due to not having a hash.";
        public String songExists { get; set; } = "{0} already exists";
        public String existingSongsFound { get; set; } = "You already have {0} out of {1} Songs from the BPList {2} by {3} installed.Do you want to move the already installed Songs to the BPLists new Playlist?";
        public String playlistExists { get; set; } = "You already the Playlist {0}. Do you want to make a new Playlist (yes), add all new songs into the existing Playlist (no) or delete the existing Playlist and Create a mew one (cancel)?";
        public String bPListImportSummaryPart1 { get; set; } = "You are about to import a BPList. You already have {0} songs. {1} songs will get installed.";
        public String bPListImportSummaryPart2a { get; set; } = "You choose to make a new Playlist.";
        public String bPListImportSummaryPart2b { get; set; } = "You choose to add new songs into the existing Playlist.";
        public String bPListImportSummaryPart2c { get; set; } = "You choose to delete the existing Playlist with all it's songs and make a new one.";
        public String bPListImportSummaryPart3 { get; set; } = "Do you want to continue?";
        public String hashCouldntBeFoundOnBeatSaver { get; set; } = "{0} ({1}) couldn't be found on BeatSaver";
        public String installingSongs { get; set; } = "Installing Songs";
        public String installedSongs { get; set; } = "Installed Songs";
        public String aborting { get; set; } = "Aborting";
        public String removedSongFromPlaylist { get; set; } = "Removed existing song {0} from Playlist";
        public String addedExistingToPlaylist { get; set; } = "Added existing Songs to BPList Playlist";
        public String movedSongToBPList { get; set; } = "Moved Song {0} to BPList";
        public String installedBPList { get; set; } = "Installed BPList {0} by {1}";

        //Cover changing
        public String picture { get; set; } = "Picture";
        public String coverChangingAborted { get; set; } = "Cover changing aborted";
        public String changedPlaylistCover { get; set; } = "Changed Playlist Cover";

        //Create new PLaylist
        public String typeAName { get; set; } = "Please type in a Playlist Name";
        public String createdPlaylist { get; set; } = "Created Playlist {0}";

        //Rename Playlist
        public String renamePlaylist { get; set; } = "Renamed {0} to {1}";

        public String unsortedSongsWarning { get; set; } = "Warning! You still have unsorted songs (the playlist at the right). If you save you'll loose all those Songs! Do you wish to abort?";
        public String savingAborted { get; set; } = "Saving Aborted"; 
        public String savingAbortedNoPlaylists { get; set; } = "Saving was aborted due to not having any Playlists at all.";
        public String saved { get; set; } = "Saved Playlists";
    }

    public class Qosmetics
    {
        public QosmeticsUI UI { get; set; } = new QosmeticsUI();
        public QosmeticsCode code { get; set; } = new QosmeticsCode();
    }

    public class QosmeticsUI
    {
        public String showImageButton { get; set; } = "Show Image in browser";
        public String addSelectedQSaberButton { get; set; } = "Add selected QSaber to queue";
        public String addSelectedQWallButton { get; set; } = "Add selected QWall to queue";
        public String addSelectedQBloqButton { get; set; } = "Add selected QBloq to queue";
        public String showOriginalMessageQSaberButton { get; set; } = "Show original message of selected QSaber";
        public String showOriginalMessageQWallButton { get; set; } = "Show original message of selected QWall";
        public String showOriginalMessageQBloqButton { get; set; } = "Show original message of selected QBloq";
        public String nameList { get; set; } = "Name";
        public String creatorList { get; set; } = "Creator";
        public String qSabersText { get; set; } = "QSabers";
        public String qWallsText { get; set; } = "QWalls";
        public String qBloqsText { get; set; } = "QBloqs";
    }

    public class QosmeticsCode
    {
        public String qosmeticsNote { get; set; } = "Note: All Qosmetics got added automatically to this program. Not every Qosmetics is present here and you may see the wrong name. Check the Qosmetics Discord Server to get all available Qosmetics (https://discord.gg/qosmetics).\n\nFor Qosmetics to work the Qosmetics mod is needed. I'll check if it's installed every time all downloads are finished and if it isn't installed install it for you.";
        public String illInstallQosmetics { get; set; } = "I'll install Qosmetics for you";
        public String checkIfQosmeticsInstalled { get; set; } = "Please start Beat Saber and check if it works. Then press the install button again.";
        public String doYouHaveQomsietcsInstalled { get; set; } = "Do you have the Qosmetics mod installed?";
        public String qosmeticsDescription { get; set; } = "{0}: {1}\nMessage Author: {2}\nMessage:\n\n{3}";
        public String qosmeticsAlreadyInQueue { get; set; } = "{0} is already in the download queue";
        public String downloading { get; set; } = "Downloading {0}";
        public String downloaded { get; set; } = "Downloaded {0}";
        public String downloadFailed { get; set; } = "{0} couldn't get downloaded";
        public String extractingQosmetics { get; set; } = "Extracting Qosmetics from zip file";
        public String uploadingToBMBF { get; set; } = "Uploading {0} to BMBF";
        public String uploadComplete { get; set; } = "{0} was uploaded to your Quest. Please enable your Qosmetic manually via the BMBF Web Interface (Open BMBF in the main menu)";
    }

    public class QSU
    {
        public QSUUI UI { get; set; } = new QSUUI();
        public QSUCode code { get; set; } = new QSUCode();
    }

    public class QSUUI
    {
        public String sourceFolderButton { get; set; } = "Choose Source Folder";
        public String sourceTextPlaceholder { get; set; } = "Please choose your Song Folder";
        public String destinationFolderButton { get; set; } = "Choose Destination fodler";
        public String destinationPlaceholder { get; set; } = "Please choose your destination folder";
        public String makeListBox { get; set; } = "Make list of all songs";
        public String onlyCheckZipsBox { get; set; } = "only check zips";
        public String overwriteExistingBox { get; set; } = "Overwrite existing zips";
        public String autoModeBox { get; set; } = "Auto Mode (Quest only)";
        public String startButton { get; set; } = "Start";
        public String backupNamePlaceholder { get; set; } = "Backup Name";
        public String backupPlaylistsButton { get; set; } = "Backup Playlists";
        public String playlistsName { get; set; } = "Playlists";
        public String restorePlaylistsButton { get; set; } = "Restore Playlists";
        public String loadPlaylistsButton { get; set; } = "Load Playlists";
        public String deletePlaylistButton { get; set; } = "Delete selected Playlist";
        public String createBPListButton { get; set; } = "Create BPList";
        public String checkSongsButton { get; set; } = "Check all songs for correct\nSong folder";
        public String loadPlaylists { get; set; } = "Load Playlists!";
        public String BackupName { get; set; } = "Backups";
        public String SonglibswitcherButton { get; set; } = "Transfer song library";
    }

    public class QSUCode
    {
        //Import old data
        public String qSUImportQuestion { get; set; } = "Hi. I'm asking you if I should import data from Quest Song Utilities. Only click yes if you've used the seperate program before. You can always import again if you wish to from the settings.";
        public String nothingImported { get; set; } = "Nothing Imported";
        public String qSUImportWindow { get; set; } = "I'll open a window for you. Please choose the folder in which your Quest Song Utilities Installation is located. I'll then transfer all the data";
        public String skippingFolder { get; set; } = "Skipping folder {0}, it already exists.";
        public String skippingFile { get; set; } = "Skipping file {0}, it already exists.";
        public String selectValidDir { get; set; } = "Please select a valid Directory";
        public String allDataMoved { get; set; } = "All Data moved";

        public String loadedPlaylists { get; set; } = "Loaded Playlists";

        //Export BPList
        public String choosePlaylist { get; set; } = "Choose a Playlist!";
        public String makingBPList { get; set; } = "Making BPList {0}";
        public String downloadingPlaylistCover { get; set; } = "Downloading Playlist Cover";
        public String downloadedPlaylistCover { get; set; } = "Downloaded Playlist Cover";
        public String bPListMade { get; set; } = "BPList {0} has been made at {1}";

        //Delete Playlist
        public String confirmPlaylistDelete { get; set; } = "Are you Sure to delete the Playlists named \"{0}\"?\n\n THIS IS NOT UNDOABLE!!!";
        public String deletingAborted { get; set; } = "Deleting Aborted";
        public String deletedSong { get; set; } = "Deleted {0}";
        public String deletedPlaylist { get; set; } = "Deleted Playlist with all Data";

        public String backupAlreadyExists { get; set; } = "This Playlist Backup already exists!";
        public String backingUpPlaylists { get; set; } = "Backing up Playlist to {0}";
        public String backedUpPlaylists { get; set; } = "Backed up Playlists to {0}";
        public String restoringPlaylists { get; set; } = "Restoring Playlist from {0}";
        public String restoredPlaylists { get; set; } = "Restored old Playlists";
        public String oculusQuestName { get; set; } = "Oculus Quest";
        public String chooseSongFolder { get; set; } = "Please choose your Song Folder";
        public String debuggingEnabled { get; set; } = "(Debugging enabled)"; //No need to translate

        //main zipping
        public String autoModeOn { get; set; } = "Auto Mode enabled! Copying all Songs to {0}. Please be patient.";
        public String zippingSongs { get; set; } = "Zipping Songs";
        public String isNoSong { get; set; } = "{0} is no Song";
        public String songName { get; set; } = "Song Name: {0}";
        public String folder { get; set; } = "Folder: {0}";
        public String overwrittenFile { get; set; } = "overwritten file: {0}";
        public String songExists { get; set; } = "This Song already exists";
        public String finishedZipping { get; set; } = "Finished! Backed up {0} Songs.";
        public String autoModeWasEnabled { get; set; } = "Auto Mode was enabled. Your finished Songs are at the program location in a folder named CustomSongs.";
        public String showOverwritten { get; set; } = "Overwritten {0} existing zips";
        public String debugOverwritten { get; set; } = "Overwritten files:";

        //Check folders
        public String checkFoldersWarning { get; set; } = "This option may nuke your Playlists. It will Backup all your Songs, rename them to the right folder name, check them if they are working and then put them back on your Quest. All in all it may take a few minutes without a responding window.\nDo you want to proceed?";
        public String aborted { get; set; } = "Aborted";
        public String songFinishedProcessing { get; set; } = "Song {0} has finished processing";
        public String finishedChecking { get; set; } = "Finished. Please reload your songs folder in BMBF";

        public String unzippingFiles { get; set; } = "Unziping files to temporary folder.";
        public String unzippingFilesComplete { get; set; } = "Unziping complete.";

        //Song indexing
        public String zip { get; set; } = "Zip: {0}";
        public String bPM { get; set; } = "BPM: {0}";
        public String requirements { get; set; } = "required modss: {0}";
        public String songSubName { get; set; } = "Song Sub Name: {0}";
        public String songAuthor { get; set; } = "Song author: {0}";
        public String mapAuthor { get; set; } = "Map author: {0}";
        public String characteristics { get; set; } = "BeatMap Characteristics: {0}";
        public String listMeta { get; set; } = "List of {0} exported Songs";
        public String search { get; set; } = "use ctrl + f to search for Songs";
        public String finishedIndex { get; set; } = "Finished! Listed {0} songs in Songs.txt";

        //Song Lib switching
        public String songLibSwitchInfo { get; set; } = "So you want to transfer your Song library from the PC Version of Beat Saber to the Quest Version or from the Quest Version to the PC Version? You are in the right place! Please press following button:\nQuest to PC = Yes, PC to Quest = No, cancle = cancle";
        public String choosePCModsFolder { get; set; } = "Please choose the CustomSongs folder of your PC installation and plug in your Quest via USB";
        public String operationRunning { get; set; } = "A Operation is already running. Please wait until it has finished.";
        public String folderDoesntExist { get; set; } = "The folder you choose doesn't exist.";
        public String startInfoQuestToPC { get; set; } = "Alright I'll now attempt to pull your Quests CustomSongs and Playlist configuration. After that I'll Move all your Quests Songs and attempt to also restore the Playlists. Note: BMBF Manager will freeze. That means it IS WORKING. Just let it do the work and DO NOT close it if Windows asks you.";
        public String startInfoPCToQuest { get; set; } = "Alright I'll now attempt to Copy all your Songs to the Quest, rename them to the right folder and even attempt to restore your Playlists. Note: BMBF Manager will freeze. That means it IS WORKING. Just let it do the work and DO NOT close it if Windows asks you.";
        public String songLibraryMoved { get; set; } = "Your Songs have been moved. It were {0} in total.";
        public String reloadSongsFolder { get; set; } = "I copied all Songs over. Please click reload songs folder in the window I opened and then press OK. I'll then attempt to restore your Playlists";
        public String reloadSongsFolderConfirmation { get; set; } = "Did you reload your Songs folder AND it finished? After you confirmed I'll sort the Playlists via the Playlist editor.";
        public String SongsFolderNotReloaded { get; set; } = "Aborted Playlist restore since the Songs folder hasn't been reloaded. Installed {0} Songs.";
        public String tooMuchSongs { get; set; } = "I'm warning you, that you have more than 500 Songs ({0} Songs to be exacr) after you imported all your PC songs. This may/will cause issues. Do you want me to stop at 500 Songs (that's the safe limit)?";
    }

    public class Songs
    {
        public SongsUI UI { get; set; } = new SongsUI();
        public SongsCode code { get; set; } = new SongsCode();
    }

    public class SongsUI
    {
        public String searchButton { get; set; } = "Search";
        public String searchTermPlaceholder { get; set; } = "Search Term";
        public String songKeyPlaceholder { get; set; } = "Song Key";
        public String showMetadataButton { get; set; } = "See complete Song Metadata";
        public String cancleDownloadsButton { get; set; } = "cancel downloads";
        public String installSongButton { get; set; } = "Install selected Song";
        public String installPCSongButton { get; set; } = "Install Song that's on\nyour PC";
    }

    public class SongsCode
    {
        public String chooseSongKey { get; set; } = "Please choose a Song Key. This can be found on BeatSaver.";
        public String beatmapDoesntExist { get; set; } = "The BeatMap {0} doesn't exist.";
        public String metadataShowing { get; set; } = "Metadata of the Song you choose:";
        public String songName { get; set; } = "Song Name: {0}";
        public String songArtist { get; set; } = "Song Artist: {0}";
        public String mapAuthor { get; set; } = "Map Author: {0}";
        public String songSubName { get; set; } = "Song Sub Name: {0}";
        public String bPM { get; set; } = "BPM: {0}";
        public String beatMapKey { get; set; } = "BeatMap Key: {0}";
        public String beatSaverError { get; set; } = "Beat Saver error";
        public String noResultsFound { get; set; } = "No results found. Try another Search Term";
        public String cantCancleNonActive { get; set; } = "You can't cancle a non active download.";
        public String clearedQueue { get; set; } = "removed all queued downloads";
        public String zipFile { get; set; } = "Zip Files";
        public String selectValidZip { get; set; } = "Please select a valid Zip File";
        public String songAddedToQueue { get; set; } = "{0} has been added to the queue";
        public String songBig { get; set; } = "This Song is over 50MB. A experimental method to install sogns will be used. Is your Quest connected?";
        public String songInstallAborted { get; set; } = "Song Installing Aborted";
        public String unzippingSong { get; set; } = "unzipping Song";
        public String unzippedSong { get; set; } = "unzipped Song";
        public String generatedHash { get; set; } = "Generated hash: {0}";
        public String installedSong { get; set; } = "Installed Song.";
        public String remainingToInstall { get; set; } = "{0} Songs remaining to install";
        public String songAlreadyInQueue { get; set; } = "The Song {0} is already in the queue";
        public String songKeyAddedToQueue { get; set; } = "Added Songs key {0} to download for a BPList";
        public String chooseSong { get; set; } = "Please Choose a Song.";
        public String beatMapCantBeFound { get; set; } = "The BeatMap {0} doesn't exist.";
        public String downloadingBeatMap { get; set; } = "Downloading BeatMap {0}";
        public String downloadedBeatMap { get; set; } = "Downloaded BeatMap {0}";
        public String checkingBeatMap { get; set; } = "Checking BeatMap {0}";
        public String uploadingBeatMap { get; set; } = "Uploading BeatMap {0} to BMBF";
        public String songWasSynced { get; set; } = "Song {0} was synced to your Quest.";
        public String couldntSync { get; set; } = "Couldn't sync with BeatSaber. Needs to be done manually.";

        //Song Checking
        public String infoMissing { get; set; } = "Fatal: Info.dat missing";
        public String wrongSong { get; set; } = "Corrected: Wrong song in Info.dat";
        public String wrongSongExtension { get; set; } = "Corrected: Wrong song extension in Info.dat";
        public String noSong { get; set; } = "Fatal: no valid song found";
        public String wrongCover { get; set; } = "Corrected: Wrong cover name in Info.dat";
        public String wrongCoverExtension { get; set; } = "Corrected: Wrong cover extension in Info.dat";
        public String noCover { get; set; } = "Fatal: no valid cover found";
        public String noDifficulty { get; set; } = "Fatal: Difficulty file for difficulty {0} in BeatMapSet {1} not found";
        public String changedUnknown { get; set; } = "corrected: changed unknown of key {0} to k. A. in Info.dat";
        public String allGood { get; set; } = "All should be good. please check manually if you have any issues";
        public String correctedFolders { get; set; } = "Corrected: Folder(s) in zip file";
        public String noCorrectionPossible { get; set; } = "Checked Song: Founding:\n{0}\nNo correction possible";
        public String songAllGood { get; set; } = "Checked Song: All good";
        public String corrected { get; set; } = "Checked Song: Foundings:{0}";
    }

    public class Settings
    {
        public SettingsUI UI { get; set; } = new SettingsUI();
        public SettingsCode code { get; set; } = new SettingsCode();
    }

    public class SettingsUI
    {
        public String backgroundButton { get; set; } = "Choose Background Image";
        public String resetBackgroundButton { get; set; } = "Reset Background Image";
        public String enableBMButton { get; set; } = "Enable BM Custom Protocol";
        public String disableBMButton { get; set; } = "Disable BM Custom Protocol";
        public String enableBSButton { get; set; } = "Enable BeatSaver OneClick Install";
        public String disableBSButton { get; set; } = "Disable BeatSaver OneClick Install";
        public String moveBBBUButton { get; set; } = "Move Backups from BBBU";
        public String moveQSUButton { get; set; } = "Move Data from QSU";
        public String enableADBOutputButton { get; set; } = "Enable ADB Output";
        public String disableADBOutputButton { get; set; } = "Disable ADB Output";
        public String KeepAliveButton { get; set; } = "Keep Alive (Quest 2)";
    }

    public class SettingsCode
    {
        public String keepAliveDisabled { get; set; } = "Keep Alive has been disabled.";
        public String keepAliveWarning { get; set; } = "Are you sure you want to enable Keep Alive? That will result in your Quest not going to sleep until the program get's closed.\nThis will only work as long as your Quest is reachable via ADB (connected via cable)\nHightly recommended for Quest 2 Users";
        public String aborted { get; set; } = "Aborted";
        public String keepAliveEnabled { get; set; } = "Keep Alive has been enabled";
        public String aDBOutputDisabled { get; set; } = "ADB output disabled.";
        public String aDBOutputEnabled { get; set; } = "ADB output enabled.";
        public String aDBOutputWarning { get; set; } = "Are you sure you want to enable ADB output? I won't check if your Quest is connected anymore and you will be able to pause the adb process when you click it.\nDo you really want to enable ADB output?";
        public String pictures { get; set; } = "Pictures";
        public String restartProgram { get; set; } = "For the changes to take effect program wide you have to restart it.";
        public String selectFile { get; set; } = "Please select a valid file";

        //Custom Protocol
        public String changingRegistryEnableBM { get; set; } = "Changing Registry to enable BM Custom protocol";
        public String customLinksEnabled { get; set; } = "Custom Links Enabled";
        public String registryUnableToChangeNoBM { get; set; } = "Registry was unable to change... no Custom protocol enabled.";
        public String changingRegistryDisableBM { get; set; } = "Changing Registry to disable BM Custom protocol";
        public String customLinksDisabled { get; set; } = "Custom Links disabled";
        public String registryUnableToChange { get; set; } = "Registry was unable to change.";

        //BeatSaver OneClick
        public String oneClickWarning { get; set; } = "This will disable OneClick Install via Mod Assistent.\nDo you wish to continue?";
        public String oneClickAborted { get; set; } = "OneClick Install enabeling aborted";
        public String changingRegistryEnableBS { get; set; } = "Changing Registry to enable OneClick Custom protocol";
        public String oneClickEnabled { get; set; } = "OneClick Install via BeatSaver enabled";
        public String oneClickDisableWarning { get; set; } = "This will disable OneClick Install via BMBF Manager.\nDo you wish to continue?";
        public String oneClickDisablingAborted { get; set; } = "OneClick disabeling enabeling aborted";
        public String changingRegistryDisableBS { get; set; } = "Changing Registry to disable OneClick Custom protocol";
        public String oneClickDisabled { get; set; } = "OneClick Install via BeatSaver disabled";

        public String updateBMBFWarning { get; set; } = "You have clicked a link to Update/Install BMBF\nDo you wish to continue";
        public String switchWarning { get; set; } = "You have clicked a link to switch from the modded/unmodded to the unmodded/modded version of Beatsaber.\nDo you wish to continue";
        public String language { get; set; } = "Language";

        public String selectLanguage { get; set; } = "Please select a language";
        public String changedLanguage { get; set; } = "Changed language to {0} translated by {1}";
    }
}