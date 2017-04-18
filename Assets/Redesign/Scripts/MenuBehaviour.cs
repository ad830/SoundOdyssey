using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SoundOdyssey;
using Midi;

namespace SoundOdyssey
{   
    public class Statistics
    {
        // General
        public uint timePlayed;                     // measured in seconds
        public uint timeLookingAtStatistics;        // why not

        // Gameplay
        public uint notesPlayed;
        public uint notesMissed;
        public float currentPlayMissRatio;

        // Teaching
        public bool durationIntroComplete;    // all objectives complete!

        public void Reset()
        {
            this.timePlayed = 0;
            this.timeLookingAtStatistics = 0;

            this.notesPlayed = 0;
            this.notesMissed = 0;
            this.currentPlayMissRatio = 1.0f;
        }

        public Dictionary<string, string> GetView(int category)
        {
            var view = new Dictionary<string, string>();
            // General Stats
            if (category == 0)
            {
                view.Add("Time Played", timePlayed.ToString());
                view.Add("Time Spent Looking At Statistics", timeLookingAtStatistics.ToString());
            }
            // Gameplay Stats
            else if (category == 1)
            {
                view.Add("Notes Played", notesPlayed.ToString());
                view.Add("Notes Missed", notesMissed.ToString());
                view.Add("Hit / Miss Ratio", currentPlayMissRatio.ToString());
            }
            // Teaching Stats
            else if (category == 2)
            {
                view.Add("Duration Intro", durationIntroComplete.ToString());
            }
            return view;
        }

        public Statistics()
        {
            Reset();
        }
    }
	public class Profile
    {
        // General information about the player
        private string name;
        private Guid id;

        // Campaign progress
        private int sector;
        private ProfileProgress levelProgress;

        // Statistics
        private Statistics statistics;

        #region Public methods
        public void UnlockNextSector()
        {
            // Should check going past the max num of sectors
            if (sector < MenuSession.Instance.SectorCount)
            {
                sector++;
                // unlock the other levels ?
            }
        }
        #endregion

        #region Public properties
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Id
        {
            get { return id.ToString(); }
            set { id = new Guid(value); }
        }
        public int Sector
        {
            get { return sector; }
            set { sector = value; }
        }
        public ProfileProgress LevelProgress
        {
            get { return levelProgress; }
            set { levelProgress = value; }
        }
        public Statistics Statistics
        {
            get { return statistics; }
            set { statistics = value; }
        }
        #endregion

        #region Constructor
        public Profile()
        {
            this.name = "NullInvalidProfile";
            this.id = Guid.Empty;
            this.sector = 0;

            this.levelProgress = new ProfileProgress();
            this.statistics = new Statistics();
            //this.levelProgress = MenuSession.Instance.DefaultLevelProgress; - CAUSES STACK OVERFLOW
        }

        public Profile(string name)
        {
            this.name = name;
            this.id = Guid.NewGuid();
            this.sector = 0;
            
            this.levelProgress = new ProfileProgress();
            this.statistics = new Statistics();
            // Add all the levels as not unlocked here
            this.levelProgress = MenuSession.Instance.DefaultLevelProgress;
            string[] examNames = MenuSession.Instance.ExamNames;
            for (int i = 0; i < examNames.Length; i++)
            {
                SoundOdyssey.LevelProgress progress = new LevelProgress(i == 0);
                this.levelProgress.SetLevelProgress(examNames[i], progress);
            }
        }
        #endregion
    }

    public class Score
    {
        #region Private data members
        // What level and profile
        private string profileName;
        private string profileId;
        private string levelName;
        private int levelId;
        private bool practice;

        // Date time of score
        private DateTime dateTime;

        // Notes and Score
        private int notesHit;
        private int notesCount;
        private int totalScore;

        // Performance Statistics
        private float expression;
        private float accuracy;
        private float fluency;
        #endregion

        #region Public properties
        public string ProfileId
        {
            get { return profileId; }
            set { profileId = value; }
        }
        public string ProfileName
        {
            get { return profileName; }
            set { profileName = value; }
        }
        public string LevelName
        {
            get { return levelName; }
            set { levelName = value; }
        }
        public int LevelId
        {
            get { return levelId; }
            set { levelId = value; }
        }
        public DateTime DateTime
        {
            get { return dateTime; }
            set { dateTime = value; }
        }
        public int NotesHit
        {
            get { return notesHit; }
            set { notesHit = value; }
        }
        public int NotesCount
        {
            get { return notesCount; }
            set { notesCount = value; }
        }
        public int TotalScore
        {
            get { return totalScore; }
            set { totalScore = value; }
        }
        public float Expression
        {
            get { return expression; }
            set { expression = value; }
        }
        public float Accuracy
        {
            get { return accuracy; }
            set { accuracy = value; }
        }
        public float Fluency
        {
            get { return fluency; }
            set { fluency = value; }
        }
        public bool PracticeMode
        {
            get { return practice; }
            set { practice = value; }
        }

        public static void SortScoreList(List<Score> scores)
        {
            if (scores == null) { return; }
            scores.Sort((a, b) =>
            {
                return a.DateTime.CompareTo(b.DateTime);
            });
        }
        #endregion

        #region Constructor
        // for serialisation purposes only
        public Score()
        {
            this.dateTime = DateTime.Now;
            this.profileName = "#DefaultProfile";
            this.profileId = Guid.Empty.ToString();
            this.levelId = -1;
            
            this.notesHit = 1;
            this.notesCount = 1;
            this.totalScore = 1;

            this.expression = 1;
            this.accuracy = 1;
            this.fluency = 1;

            this.practice = false;
        }
        public Score(SoundOdyssey.Profile profile, string level, int id, int notesHit, int notesCount, int score, float exp, float acc, float flu, bool prac = false)
        {
            this.dateTime = DateTime.Now;
            this.profileName = profile.Name;
            this.profileId = profile.Id;
            this.levelName = level;
            this.levelId = id;

            this.notesHit = notesHit;
            this.notesCount = notesCount;
            this.totalScore = score;

            this.expression = exp;
            this.accuracy = acc;
            this.fluency = flu;

            this.practice = prac;
        }
        #endregion
    }

    public class ProfileStateManager
    {
        public ProfileStateManager()
        {

        }
    }
}

public class MenuSession
{
    // menu state
    bool isFirst = true;
    public bool mainMenuShown = false;

    // profiles state
    public List<Profile> allProfiles;
    public int currentProf = 0;

    // scores state
    public List<SoundOdyssey.Score> allScores;

    // settings state
    public SettingsData.GameSettings gameSettings;
    public SettingsData.AudioSettings audioSettings;
    public SettingsData.VideoSettings videoSettings;
    public SettingsData.MidiDeviceSettings midiSettings;

    // campaign cache
    private List<Campaign.Sector> sectorData;
    private List<Campaign.Level> levelCache;
    private string[] levelNames;
    private string[] examNames;
    private ProfileProgress defaultProgress;

    public List<SoundOdyssey.Score> ScoresForProfile(SoundOdyssey.Profile profile, string levelName = "")
    {
        if (allScores == null) { return null; }
        return allScores.FindAll((record) => 
        {
            if (levelName == "")
            {
                return record.ProfileId == profile.Id;
            }
            else
            {
                return (record.ProfileId == profile.Id) && (record.LevelName == levelName);
            }
        });
    }

    public int ProfileCount
    {
        get { return allProfiles.Count; }
    }

    public string [] LevelNames
    {
        get
        {
            return levelNames;
        }
    }

    public SoundOdyssey.Profile CurrentProfile
    {
        get { return allProfiles[currentProf]; }
    }

    public List<Campaign.Sector> CampaignSectors
    {
        get { return sectorData; }
    }

    public ProfileProgress DefaultLevelProgress
    {
        get { return defaultProgress; }
    }

    public int SectorCount
    {
        get
        {
            return sectorData.Count;
        }
    }

    public string[] ExamNames
    {
        get
        {
            return examNames;
        }
    }

    public bool FirstLoaded
    {
        get
        {
            return isFirst;
        }
    }

    public void SetAlreadyLoaded()
    {
        isFirst = false;
    }

    #region Settings Management and Serialisation
    public void LoadSettingsFromDisk()
    {
        var diskSettings = FileDirector.ReadSettings();
        if (diskSettings != null)
        {
            gameSettings = diskSettings.game;
            audioSettings = diskSettings.audio;
            videoSettings = diskSettings.video;
            midiSettings = diskSettings.midi;
        }
        else
        {
            videoSettings.CurrentResolution = Screen.currentResolution;
            videoSettings.AntiAliasing = QualitySettings.antiAliasing;
            videoSettings.Fullscreen = Screen.fullScreen;
            videoSettings.QualityLevel = QualitySettings.GetQualityLevel();
            videoSettings.Vsync = QualitySettings.vSyncCount;
        }
    }

    public void UpdateGameSettings(SettingsData.GameSettings g)
    {
        gameSettings = g;
        WriteSettingsToDisk();
    }
    public void UpdateVideoSettings(SettingsData.VideoSettings v)
    {
        videoSettings = v;
        WriteSettingsToDisk();
    }
    public void UpdateAudioSettings(SettingsData.AudioSettings a)
    {
        audioSettings = a;
        WriteSettingsToDisk();
    }
    public void UpdateMidiSettings(SettingsData.MidiDeviceSettings m)
    {
        midiSettings = m;
        WriteSettingsToDisk();
    }

    public void UpdateAllSettings(FileDirector.Settings s)
    {
        gameSettings = s.game;
        audioSettings = s.audio;
        videoSettings = s.video;
        midiSettings = s.midi;
        FileDirector.WriteSettings(s);
    }

    public void WriteSettingsToDisk()
    {
        FileDirector.Settings s = new FileDirector.Settings();
        s.game = gameSettings;
        s.audio = audioSettings;
        s.video = videoSettings;
        s.midi = midiSettings;
        FileDirector.WriteSettings(s);
    } 
    #endregion

    public void PickCurrentProfile(int idx)
    {
        if (idx >= allProfiles.Count)
        {
            throw new ArgumentException(
                string.Format("MenuSession.PickCurrentProfile() : Bad Index {0} Count {1}", 
                idx, allProfiles.Count), "idx");
        }
        else
        {
            currentProf = idx;
        }
    }

    public void PickCurrentProfile(string name, string id = "")
    {
        currentProf = allProfiles.FindIndex((profile) =>
        {
            bool nameMatches = profile.Name == name;
            if (id != "")
            {
                return nameMatches && profile.Id == id;
            }
            else
            {
                return nameMatches;
            }
        });
    }

    public void ClearProfiles()
    {
        allProfiles.Clear();
        FileDirector.ActualDeleteAllProfiles();
    }

    public bool ProfileExists(string name, string id = "")
    {
        return allProfiles.Exists((profile) =>
        {
            if (id != "")
            {
                return (profile.Name == name) && (profile.Id == id);
            }
            else
            {
                return (profile.Name == name);
            }
            
        });
    }

    public bool CreateProfile(string name)
    {
        return AddProfile(new SoundOdyssey.Profile(name));
    }

    public bool AddProfile(SoundOdyssey.Profile p)
    {
        bool canAddProfile = !ProfileExists(p.Name, p.Id);
        if (canAddProfile)
        {
            allProfiles.Add(p);
        }
        return canAddProfile;
    }

    // Returns a reference to a profile by name and id
    // in the profiles list
    // Returns:
    // SoundOdyssey.Profile if given parameters match a profile
    // null if the parameters do not match a profile
    public SoundOdyssey.Profile GetProfile(string name, string id)
    {
        return allProfiles.Find((profile) =>
        {
            return (profile.Name == name) && (profile.Id == id);
        });
    }
    // Returns a reference to a profile by its index
    // in the profiles list
    // Returns:
    // SoundOdyssey.Profile if index is valid
    // null if index is not valid
    public SoundOdyssey.Profile GetProfile(int idx)
    {
        if (idx >= this.ProfileCount)
        {
            return null;
        }
        return allProfiles[idx];
    }

    public int GetProfileIndex(SoundOdyssey.Profile matchProfile)
    {
        return allProfiles.FindIndex((profile) =>
        {
            return (profile.Name == matchProfile.Name) && (profile.Id == matchProfile.Id);
        });
    }

    public bool RemoveProfile(SoundOdyssey.Profile profile)
    {
        return allProfiles.Remove(profile);
    }

    public bool RemoveProfile(string name, string id)
    {
        return allProfiles.Remove(GetProfile(name, id));
    }

    public void SaveProfiles()
    {
        Debug.LogFormat("Writing {0} profiles", this.ProfileCount);
        FileDirector.WriteProfiles(allProfiles);
    }

    public void LoadProfiles()
    {
        allProfiles = FileDirector.ReadProfiles();
    }

    #region Handling score session management

    public void LoadScores()
    {
        Debug.Log("READING SCORES FROM FILE");
        allScores = FileDirector.ReadScores();
    }

    public void SaveScores()
    {
        if (allScores == null) { return; }
        FileDirector.WriteScores(allScores.ToArray());
    }

    public void AddScore(SoundOdyssey.Score s)
    {
        if (allScores == null)
        {
            allScores = new List<SoundOdyssey.Score>();
        }
        allScores.Add(s);
        SaveScores();
    }
    public bool RemoveScore(SoundOdyssey.Score s)
    {
        return allScores.Remove(s);
    }
    public void RemoveScoresForProfile(SoundOdyssey.Profile profile, string levelName = "")
    {
        List<SoundOdyssey.Score> matches = allScores.FindAll((record) =>
        {
            bool idMatches = record.ProfileId == profile.Id;
            if (levelName == "")
            {
                return idMatches;
            }
            else
            {
                return idMatches && record.LevelName == levelName;
            }
        });
        Debug.LogFormat("Removing {0} scores", matches.Count);

        foreach (var record in matches)
        {
            RemoveScore(record);
        }
        SaveScores();
    }
    #endregion

    public void SaveAllData()
    {
        SaveScores();
        SaveProfiles();
        WriteSettingsToDisk();
    }

    private void AutoPickMidiOutputDevice()
    {
        // auto pick first device only
        // the user can set it otherwise in settings
        if (midiSettings.OutputDeviceConfigs.Count > 0)
        {
            midiSettings.OutputDeviceConfigs[0].InUse = true;
        }
        for (int i = 0; i < OutputDevice.InstalledDevices.Count; i++)
        {
            var config = new SettingsData.MidiDeviceSettings.OutputDeviceConfig();
            config.Name = OutputDevice.InstalledDevices[i].Name;
            config.InUse = i == 0;
            midiSettings.OutputDeviceConfigs.Add(config);
        }
    }

    private static void CacheLevels(List<Campaign.Level> cache, List<Campaign.Sector> data)
    {
        cache.Clear();
        for (int i = 0; i < data.Count; i++)
        {
            for (int k = 0; k < data[i].stages.Count; k++)
            {
                cache.AddRange(data[i].stages[k].levels);
            }
        }
        Debug.LogFormat("Cached {0} levels", cache.Count);
    }

    private string[] GetLevelNames()
    {
        return levelCache.ConvertAll<string>((level) =>
        {
            return level.info.name;
        }).ToArray();
    }

    // Walks the hierarchy of the level structure to get the sector for a particular 
    // level name and or id
    // Returns:
    // 0 -> correct sector index
    // -1 if level not found
    public int GetSectorForLevel(string levelName, int levelId = -1)
    {
        for (int sectorNum = 0; sectorNum < sectorData.Count; sectorNum++)
        {
            Campaign.Sector sector = sectorData[sectorNum];
            for (int stageNum = 0; stageNum < sector.stages.Count; stageNum++)
            {
                Campaign.Stage stage = sector.stages[stageNum];
                for (int levelNum = 0; levelNum < stage.levels.Count; levelNum++)
                {
                    Campaign.Level level = stage.levels[levelNum];
                    bool nameMatches = (level.info.name == levelName);
                    if (levelId != -1)
                    {
                        if (nameMatches && level.info.id == levelId)
                        {
                            return sectorNum;
                        }
                    }
                    else
                    {
                        if (nameMatches)
                        {
                            return sectorNum;
                        }
                    }
                }
            }
        }
        return -1;
    }

    // Walks the hierarchy of the level structure to get the sector for a particular 
    // exam name
    // Returns:
    // 0 -> correct sector index
    // -1 if level not found
    public int GetSectorForExam(string examName)
    {
        for (int sectorNum = 0; sectorNum < sectorData.Count; sectorNum++)
        {
            Campaign.Sector sector = sectorData[sectorNum];
            if (sector.exam.info.name == examName)
            {
                return sectorNum;
            }
        }
        return -1;
    }

    // Sets that the current profile has played the given level
    public void SetPlayedLevel(string level)
    {
        Debug.LogFormat("Player has now played {0}", level);
        SoundOdyssey.LevelProgress progress = this.CurrentProfile.LevelProgress.GetLevelProgress(level, true);
        if (!progress.played)
        {
            progress.played = true;
            progress.unlocked = true;
            this.CurrentProfile.LevelProgress.SetLevelProgress(level, progress);
            SaveProfiles();
        }
    }

    private ProfileProgress CreateStartingLevelProgress()
    {
        ProfileProgress levelProgress = new ProfileProgress();
        for (int i = 0; i < levelCache.Count; i++)
        {
            string levelName = levelCache[i].info.name;
            int whichSector = GetSectorForLevel(levelName);
            bool startsUnlocked = whichSector == 0;
            LevelProgress progress = new SoundOdyssey.LevelProgress(startsUnlocked);
            levelProgress.SetLevelProgress(levelName, progress);
        }
        return levelProgress;
    }

    private string[] GetExamNames()
    {
        List<string> names = new List<string>();
        for (int sectorNum = 0; sectorNum < sectorData.Count; sectorNum++)
        {
            Campaign.Sector sector = sectorData[sectorNum];
            names.Add(sector.exam.info.name);
        }
        return names.ToArray();
    }

    MenuSession()
    {
        isFirst = true;
        //allProfiles = new List<Profile>(3);
        LoadProfiles();

        gameSettings = new SettingsData.GameSettings();
        audioSettings = new SettingsData.AudioSettings();
        videoSettings = new SettingsData.VideoSettings();
        midiSettings = new SettingsData.MidiDeviceSettings();

        LoadSettingsFromDisk();
        // scan for output devices and auto pick first one
        if (midiSettings.OutputDeviceConfigs.Count == 0)
        {
            AutoPickMidiOutputDevice();
        }

        // load campaign data
        sectorData = GalaxyLevelFormatReader.LoadSectorsFromResources("Levels/LevelStructure");
        levelCache = new List<Campaign.Level>();
        CacheLevels(levelCache, sectorData);
        levelNames = GetLevelNames();
        defaultProgress = CreateStartingLevelProgress();
        examNames = GetExamNames();

        // load all scores
        allScores = new List<SoundOdyssey.Score>(100);

        
    }

    private static MenuSession instance;

    public static MenuSession Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new MenuSession();
            }
            return instance;
        }
    }
}

public class MenuBehaviour : MonoBehaviour {

    public GameObject ProfileSelectionMenu;
    public GameObject ProfileMenu;
    public GameObject EditPopup;
    public GameObject StatPopup;
    public GameObject SplashScreenButton;
    public GameObject MainMenu;
    public GameObject WelcomeDialog;
    public GameObject SettingsMenu;
    public GameObject PickDevicePopup;
    public GameObject FeatureInDevPopup;
    public Text title;

    GameObject EditWarningPopup;
    GameObject ProfileTemp;
    GameObject[] ProfileButtons;
    GameObject button;
    string currentProfName;

    InputField field;

    KeyValueViewBehaviour kvBehaviour;
    Dictionary<string, string> testGeneralStats;
    Dictionary<string, string> testGameplayStats;

    Animator anim;
    Animator textAnim;
    Animator titleAnim;
    Animator welcomeAnim;
    Animator mainAnim;

    MenuSession session;

    public SettingsData.GameSettings tempGameSettings;
    public SettingsData.AudioSettings tempAudioSettings;
    public SettingsData.VideoSettings tempVideoSettings;
    public SettingsData.MidiDeviceSettings tempMidiSettings;

    private enum EnterNameMode { CreateNew, RenameExisting };
    EnterNameMode enterNameMode = EnterNameMode.CreateNew;

    void UpdateAllSettings()
    {
        // Audio
        // update each section's settings
        Transform settingsButtons = SettingsMenu.transform.Find("Settings List Panel");

        // save all audio slider values
        Transform audio = settingsButtons.Find("Audio");
        Transform audioOptions = audio.Find("Options");
        for (int i = 0; i < audioOptions.childCount; i++)
        {

            Slider slider = audioOptions.GetChild(i).Find("Slider").GetComponent<Slider>();
            switch (i)
            {
                case 0:
                    tempAudioSettings.MasterVolume = slider.value;
                    break;
                case 1:
                    tempAudioSettings.PercussionVolume = (int)slider.value;
                    break;
                case 2:
                    tempAudioSettings.PlayerVolume = (int)slider.value;
                    break;
                case 3:
                    tempAudioSettings.AccompVolume = (int)slider.value;
                    break;
                case 4:
                    tempAudioSettings.EffectsVolume = (int)slider.value;
                    break;
                case 5:
                    tempAudioSettings.MusicVolume = slider.value;
                    break;
                default:
                    break;
            }
        }

        // Game
        Transform game = settingsButtons.Find("Game");
        Transform gameOptions = game.Find("Options");
        for(int i = 0; i < gameOptions.childCount; i++)
        {
            Toggle toggle = gameOptions.GetChild(i).Find("Toggle").GetComponent<Toggle>();
            switch(i)
            {
                case 0:
                    tempGameSettings.NoteLabels = toggle.isOn;
                    break;
                case 1:
                    tempGameSettings.AuraVisible = toggle.isOn;
                    break;
                case 2:
                    tempGameSettings.PowerGauge = toggle.isOn;
                    break;
                case 3:
                    tempGameSettings.BackgroundEffects = toggle.isOn;
                    break;
                default:
                    break;
            }
        }

        // Video Settings
        tempVideoSettings.Fullscreen = Screen.fullScreen;
        tempVideoSettings.CurrentResolution = Screen.currentResolution;
        tempVideoSettings.AntiAliasing = QualitySettings.antiAliasing;
        tempVideoSettings.QualityLevel = QualitySettings.GetQualityLevel();
        tempVideoSettings.Vsync = QualitySettings.vSyncCount;

        // Midi Settings
        Transform devicePanels = PickDevicePopup.transform.GetChild(0).GetChild(0).Find("DevicePanels");
        PickMIDIDevice inputPicker = devicePanels.Find("InputDevicePanel").Find("Devices").GetComponent<PickMIDIDevice>();
        PickMIDIDevice outputPicker = devicePanels.Find("OutputDevicePanel").Find("Devices").GetComponent<PickMIDIDevice>();
        tempMidiSettings.InputDelay = 0;    // get input delay from calibration : TODO

        tempMidiSettings.InputDeviceConfigs.Clear();
        int inputConfigCount = inputPicker.DeviceCount;
        for (int i = 0; i < inputConfigCount; i++)
        {
            string name = inputPicker.GetDeviceName(i);
            //bool used = inputPicker.GetDeviceUsed(i);
            var dev = new SettingsData.MidiDeviceSettings.InputDeviceConfig();
            dev.Name = name;
            dev.KeyRange = 61;      // get calibrated key range : TODO
            tempMidiSettings.InputDeviceConfigs.Add(dev);
        }

        tempMidiSettings.OutputDeviceConfigs.Clear();
        int outputConfigCount = outputPicker.DeviceCount;
        for (int i = 0; i < outputConfigCount; i++)
        {
            string name = outputPicker.GetDeviceName(i);
            bool used = outputPicker.GetDeviceUsed(i);
            var dev = new SettingsData.MidiDeviceSettings.OutputDeviceConfig();
            dev.Name = name;
            dev.InUse = used;
            tempMidiSettings.OutputDeviceConfigs.Add(dev);
        }
        tempMidiSettings.WhichInputDevice = 0;      // get which device : TODO


        FileDirector.Settings s = new FileDirector.Settings();
        s.audio = tempAudioSettings;
        s.game = tempGameSettings;
        s.midi = tempMidiSettings;
        s.video = tempVideoSettings;
        session.UpdateAllSettings(s);
    }

    private void SetupAudio()
    {
        // update each section's settings
        Transform settingsButtons = SettingsMenu.transform.Find("Settings List Panel");

        // save all audio slider values
        Transform audio = settingsButtons.Find("Audio");
        Transform audioOptions = audio.Find("Options");
        for (int i = 0; i < audioOptions.childCount; i++)
        {

            Slider slider = audioOptions.GetChild(i).Find("Slider").GetComponent<Slider>();
            switch (i)
            {
                case 0:
                    slider.value = tempAudioSettings.MasterVolume;
                    break;
                case 1:
                    slider.value = tempAudioSettings.PercussionVolume;
                    break;
                case 2:
                    slider.value = tempAudioSettings.PlayerVolume;
                    break;
                case 3:
                    slider.value = tempAudioSettings.AccompVolume;
                    break;
                case 4:
                    slider.value = tempAudioSettings.EffectsVolume;
                    break;
                case 5:
                    slider.value = tempAudioSettings.MusicVolume;
                    break;
                default:
                    break;
            }
        }
    }
    private void SetupVideo()
    {

    }
    private void SetupGame()
    {
        // update each section's settings
        Transform settingsButtons = SettingsMenu.transform.Find("Settings List Panel");

        Transform game = settingsButtons.Find("Game");
        Transform gameOptions = game.Find("Options");
        for (int i = 0; i < gameOptions.childCount; i++)
        {
            Toggle toggle = gameOptions.GetChild(i).Find("Toggle").GetComponent<Toggle>();
            switch (i)
            {
                case 0:
                    toggle.isOn = tempGameSettings.NoteLabels;
                    break;
                case 1:
                    toggle.isOn = tempGameSettings.AuraVisible;
                    break;
                case 2:
                    toggle.isOn = tempGameSettings.PowerGauge;
                    break;
                case 3:
                    toggle.isOn = tempGameSettings.BackgroundEffects;
                    break;
                default:
                    break;
            }
        }
    }
    private void SetupMidi()
    {

    }

    //bool MainMenuShown = false;
	// Use this for initialization
	void Start () {
        GameDirector director = GameDirector.Instance;
        session = MenuSession.Instance;
        session.LoadScores();

        Debug.LogFormat("Sess Cur Prof {0}", session.currentProf);
        StatPopup.SetActive(true);
        kvBehaviour = GameObject.Find("KeyValues").GetComponent<KeyValueViewBehaviour>();

        tempAudioSettings = new SettingsData.AudioSettings();
        tempGameSettings = new SettingsData.GameSettings();
        tempVideoSettings = new SettingsData.VideoSettings();
        tempMidiSettings = new SettingsData.MidiDeviceSettings();

        tempAudioSettings = session.audioSettings;
        tempGameSettings = session.gameSettings;
        tempVideoSettings = session.videoSettings;
        tempMidiSettings = session.midiSettings;

        Transform settingsButtons = SettingsMenu.transform.Find("Settings List Panel");

        Button backButton = SettingsMenu.transform.Find("Back").GetComponent<Button>();

        Button audioButton = settingsButtons.Find("Audio").GetComponent<Button>();
        Button videoButton = settingsButtons.Find("Video").GetComponent<Button>();
        Button gameButton = settingsButtons.Find("Game").GetComponent<Button>();
        Button midiButton = settingsButtons.Find("Setup MIDI KeyBoard").GetComponent<Button>();
        audioButton.onClick.AddListener(() => {
            SetupAudio();
            UpdateAllSettings();
        });
        videoButton.onClick.AddListener(() =>
        {
            SetupVideo();
            UpdateAllSettings();
        });
        gameButton.onClick.AddListener(() =>
        {
            SetupGame();
            UpdateAllSettings();
        });
        midiButton.onClick.AddListener(() =>
        {
            SetupMidi();
            UpdateAllSettings();
        });
        backButton.onClick.AddListener(() =>
        {
            UpdateAllSettings();
        });

        testGeneralStats = new Dictionary<string, string>();
        testGameplayStats = new Dictionary<string, string>();

        for (int i = 0; i < 5; i++)
        {
            testGeneralStats.Add("GeneralKey " + i, "Value " + i);
            testGameplayStats.Add("GameplayKey " + i, "Value " + i);
        }
        testGeneralStats.Add("Time Spent Viewing Statistics", "3 hours, 55 minutes...");

        EditWarningPopup = EditPopup.transform.Find("WarningPopup").gameObject;
        EditWarningPopup.SetActive(false);

        ProfileSelectionMenu.SetActive(false);
        ProfileMenu.SetActive(false);
        EditPopup.SetActive(false);
        StatPopup.SetActive(false);
        SettingsMenu.SetActive(false);
        PickDevicePopup.SetActive(false);

        DismissFeatureInDev();

        ProfileSelectionMenu.SetActive(true);
        ProfileButtons = GameObject.FindGameObjectsWithTag("Profile");
        ProfileSelectionMenu.SetActive(false);

        field = EditPopup.transform.Find("InputField").gameObject.GetComponent<InputField>();
        HideEditNamePopup();
        anim = Camera.main.GetComponent<Animator>();
        textAnim = SplashScreenButton.transform.Find("Button").GetChild(0).gameObject.GetComponent<Animator>();
        titleAnim = title.GetComponent<Animator>();
        welcomeAnim = WelcomeDialog.GetComponent<Animator>();
        mainAnim = MainMenu.GetComponent<Animator>();

        anim.enabled = false;
        textAnim.enabled = true;
        titleAnim.enabled = false;
        welcomeAnim.enabled = false;
        mainAnim.enabled = false;

        // Fill the profile selection menu with the names of the loaded profiles in session
        int profileCount = session.ProfileCount;
        for (int i = 0; i < 3; i++ )
        {
            if (i >= 0 && i < profileCount)
            {
                Text profileSelectText = ProfileButtons[i].transform.GetChild(0).GetComponent<Text>();
                profileSelectText.text = session.GetProfile(i).Name;
            }
        }

        if (session.FirstLoaded)
        {
            SplashScreenButton.SetActive(true);
            session.SetAlreadyLoaded();
        }
        else
        {
            SplashScreenButton.SetActive(false);
            ShowMainMenu();
        }

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnApplicationQuit()
    {
        session.SaveAllData();
        // apply time played statistics
    }

    void ShowEditNamePopup()
    {
        EditPopup.SetActive(true);
        EventSystem.current.SetSelectedGameObject(field.gameObject, null);
        field.ActivateInputField();
    }
    void HideEditNamePopup()
    {
        EditPopup.SetActive(false);
        field.text = "";
    }
    void ShowProfileMenu()
    {
        // set the profile name up top
        string profName = session.CurrentProfile.Name;

        ProfileMenu.transform.Find("Panel").GetChild(0).Find("Profile Name").GetComponent<Text>().text = profName;
        ProfileMenu.SetActive(true);
        ProfileSelectionMenu.SetActive(false);
    }
    void UpdateWelcomeName()
    {
        Text welcomeText = WelcomeDialog.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Text>();
        welcomeText.text = "Welcome: " + GameDirector.Instance.CurrentProfile.Name;
    }

    public void OnClickProfileSelectionMenu()
    {
        textAnim.enabled = false;
        SplashScreenButton.SetActive(false);
        ProfileMenu.SetActive(false);
        EditPopup.SetActive(false);
        StatPopup.SetActive(false);
        ProfileSelectionMenu.SetActive(true);
        currentProfName = "";
    }

    public void ShowMainMenu()
    {
        ProfileMenu.SetActive(false);
        field.text = "";
        anim.enabled = true;
        titleAnim.enabled = true;
        welcomeAnim.enabled = true;
        mainAnim.enabled = true;
        UpdateWelcomeName();
    }

    public void OnClickProfileFromMainMenu()
    {
        OnClickProfileMenu(session.currentProf);
        //MainMenu.SetActive(false);
        //WelcomeDialog.SetActive(false);
    }

    public void OnClickProfileMenu(int pIdx)
    {
        if (pIdx < 0 || pIdx > 2)
        {
            Debug.LogError("clicked profile option out of range");
            Debug.Break();
        }
        else
        {
            //session.currentProf = pIdx;
            //ProfileTemp = ProfileButtons[session.currentProf];
            ProfileTemp = ProfileButtons[pIdx];
            //currentProfName = ProfileTemp.transform.GetChild(0).GetComponent<Text>().text;

            // show prompt to input name if there is no profile for that index
            if (session.GetProfile(pIdx) == null)
            {
                enterNameMode = EnterNameMode.CreateNew;
                ShowEditNamePopup();
            }
            else
            {
                session.PickCurrentProfile(pIdx);
                currentProfName = session.CurrentProfile.Name;
                //currentProfName = ProfileTemp.transform.GetChild(0).GetComponent<Text>().text;
                Debug.Log(currentProfName);
                //GameDirector.Instance.currentProfile = FileDirector.LoadProfile(currentProfName);
                GameDirector.Instance.CurrentProfile = session.CurrentProfile;
               
                if (!session.mainMenuShown)
                {
                    ProfileSelectionMenu.SetActive(false);
                    ShowMainMenu();
                    session.mainMenuShown = true;
                }
                else
                {
                    ShowProfileMenu();
                }
                UpdateWelcomeName();
            }
            
        }
    }

    public void HideEditNameWarning()
    {
        EditWarningPopup.SetActive(false);
    }

    public void ShowEditNameWarning(string message, float delay = 3.0f)
    {
        CancelInvoke("HideEditNameWarning");
        Text warningText = EditWarningPopup.transform.Find("Text").GetComponent<Text>();
        warningText.text = message;
        EditWarningPopup.SetActive(true);
        Invoke("HideEditNameWarning", delay);
    }

    public void OnClickEditDetails()
    {
        enterNameMode = EnterNameMode.RenameExisting;
        ShowEditNamePopup();
    }

    public void BackFromPopup()
    {
        HideEditNamePopup();
    }
    
    public void OnClickStatPopup()
    {
        StatPopup.SetActive(true);
        Transform statisticsPanel = StatPopup.GetComponent<Transform>().Find("Category List Panel");
        statisticsPanel.GetChild(0).GetComponent<Text>().text = session.CurrentProfile.Name;
        OnClickGeneralStatistics();
    }

    public void BackFromStatPopup()
    {
        OnClickGeneralStatistics();
        StatPopup.SetActive(false);
    }

    public void ConfirmName()
    {
        Debug.Log(session.currentProf);
        string inputName = field.text;

        if (inputName.Length == 0)
        {
            // TO DO display error message about no empty names
            Debug.Log("Tried to enter an empty name");
            ShowEditNameWarning("No empty names!");
            return;
        }

        bool existingName = session.ProfileExists(inputName);
        if (existingName)
        {
            // TO DO display error about existing names
            Debug.Log("Profile already exists");
            ShowEditNameWarning("Profile already exists!");
            return;
        }
        
        // change displayed profile name in profile selection
        Transform trans = ProfileTemp.transform;
        GameObject obj = trans.GetChild(0).gameObject;
        obj.GetComponent<Text>().text = inputName;

        // create a new profile or rename an existing profile
        if (enterNameMode == EnterNameMode.CreateNew)
        {
            session.CreateProfile(inputName);
            session.PickCurrentProfile(session.ProfileCount - 1);
        }
        else
        {
            session.CurrentProfile.Name = inputName;
        }
        session.SaveProfiles();
        GameDirector.Instance.CurrentProfile = session.CurrentProfile;

        /*
        string oldName = null;
        if (GameDirector.Instance.currentProfile != null)
        {
            oldName = GameDirector.Instance.currentProfile.playerName;
        }
       

        // Save profile to file
        if (oldName != null)
        {
            if (FileDirector.ProfileExists(oldName))
            {
                Debug.LogFormat("Updating profile. Old {0} New {1}", oldName, inputName);
                GameDirector.Instance.currentProfile.playerName = inputName;

                FileDirector.DeleteProfile(oldName);
                FileDirector.CreateNewProfile(GameDirector.Instance.currentProfile);
            }
        }
        else
        {
            FileDirector.Profile currentProfile = new FileDirector.Profile();
            currentProfile.playerName = inputName;
            currentProfile.lastLevelUnlocked = 0;
            currentProfile.timePlayed = 0;
            currentProfile.grade = 0;
            currentProfile.progress.SetLevelProgress("Main Menu", new LevelProgress(true));

            GameDirector.Instance.currentProfile = currentProfile;

            FileDirector.CreateNewProfile(GameDirector.Instance.currentProfile);
        }
        */
        
        UpdateWelcomeName();
        HideEditNamePopup();
        if (!session.mainMenuShown)
        {
            ProfileSelectionMenu.SetActive(false);
            ShowMainMenu();
            session.mainMenuShown = true;
        }
        else
        {
            ShowProfileMenu();
        }
    }

    public void OnClickProfileResetScores()
    {
        Debug.LogFormat("Reset scores for {0}", session.CurrentProfile.Name);
        session.RemoveScoresForProfile(session.CurrentProfile);
    }

    public void OnClickSettings()
    {
        // load settings from session
        SettingsMenu.SetActive(true);
        OnClickAudioSettings();
    }

    public void OnClickAudioSettings()
    {
        ClearSettings();
        GameObject SettingsHeader = SettingsMenu.transform.Find("Settings Panel").GetChild(0).gameObject;
        SettingsHeader.GetComponent<Text>().text = "Audio";

        GameObject audioOptions = SettingsMenu.transform.Find("Settings List Panel").GetChild(1).GetChild(1).gameObject;
        audioOptions.SetActive(true);
    }

    public void OnClickVideoSettings()
    {
        ClearSettings();
        GameObject SettingsHeader = SettingsMenu.transform.Find("Settings Panel").GetChild(0).gameObject;
        SettingsHeader.GetComponent<Text>().text = "Video";

        GameObject videoOptions = SettingsMenu.transform.Find("Settings List Panel").GetChild(2).GetChild(1).gameObject;
        videoOptions.SetActive(true);
    }

    public void OnClickGameSettings()
    {
        ClearSettings();
        GameObject SettingsHeader = SettingsMenu.transform.Find("Settings Panel").GetChild(0).gameObject;
        SettingsHeader.GetComponent<Text>().text = "Game";

        GameObject gameOptions = SettingsMenu.transform.Find("Settings List Panel").GetChild(3).GetChild(1).gameObject;
        gameOptions.SetActive(true);
    }

    public void OnClickMidiConfigSettings()
    {
        ClearSettings();
        GameObject SettingsHeader = SettingsMenu.transform.Find("Settings Panel").GetChild(0).gameObject;
        SettingsHeader.GetComponent<Text>().text = "Midi Config";

        GameObject midiConfigOptions = SettingsMenu.transform.Find("Settings List Panel").GetChild(4).GetChild(1).gameObject;
        midiConfigOptions.SetActive(true);
    }

    void ClearSettings()
    {
        GameObject audioOptions = SettingsMenu.transform.Find("Settings List Panel").GetChild(1).GetChild(1).gameObject;
        audioOptions.SetActive(false);

        GameObject videoOptions = SettingsMenu.transform.Find("Settings List Panel").GetChild(2).GetChild(1).gameObject;
        videoOptions.SetActive(false);

        GameObject gameOptions = SettingsMenu.transform.Find("Settings List Panel").GetChild(3).GetChild(1).gameObject;
        gameOptions.SetActive(false);

        GameObject midiConfigOptions = SettingsMenu.transform.Find("Settings List Panel").GetChild(4).GetChild(1).gameObject;
        midiConfigOptions.SetActive(false);
    }

    public void BackFromSettings()
    {
        ClearSettings();
        SettingsMenu.SetActive(false);
    }

    public void OnClickExitGame()
    {
        session.WriteSettingsToDisk();
        Application.Quit();
    }

    public void OnClickPlayGame()
    {
        Debug.Log(session.currentProf);
        Application.LoadLevel("GalaxyMenu");
    }

    public void OnClickPickDevice()
    {
        PickDevicePopup.SetActive(true);
    }

    public void BackFromPickDevice()
    {
        PickDevicePopup.SetActive(false);
    }

    public void ShowFeatureInDev()
    {
        FeatureInDevPopup.SetActive(true);
    }

    public void DismissFeatureInDev()
    {
        FeatureInDevPopup.SetActive(false);
    }

    #region Statistics menu logic
    public void OnClickGeneralStatistics()
    {
        ChangeStatisticsCategoryHeading("General");
        var generalStats = session.CurrentProfile.Statistics.GetView(0);
        //kvBehaviour.FillView(testGeneralStats);
        kvBehaviour.FillView(generalStats);
    }
    public void OnClickGameplayStatistics()
    {
        ChangeStatisticsCategoryHeading("Gameplay");
        var gameplayStats = session.CurrentProfile.Statistics.GetView(1);
        //kvBehaviour.FillView(testGameplayStats);
        kvBehaviour.FillView(gameplayStats);
    }
    void ChangeStatisticsCategoryHeading(string title)
    {
        GameObject headingObj = StatPopup.GetComponent<Transform>().Find("Statistics panel").Find("CategoryName").gameObject;
        headingObj.GetComponent<Text>().text = title;
    }
    #endregion
}
