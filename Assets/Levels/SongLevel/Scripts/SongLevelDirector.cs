using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SmfLite;
using Midi;

namespace SoundOdyssey
{
    public enum MidiStatusType
    {
        NoteOn = 0x90,
        NoteOff = 0x80,
        ProgramChange = 0xC0,
        PitchBend = 0xE0,
        ControlChange = 0xB0
    }

    public class ScoreManager
    {
        private int pitchScore = 0;
        private int expressionScore = 0;
        private int durationScore = 0;  
        private int multiplier = 1;
        private List<float> fluencyHistory;

        public int PitchScore
        {
            get { return pitchScore; }
            set { pitchScore = value; }
        }

        public int ExpressionScore
        {
            get { return expressionScore; }
            set { expressionScore = value; }
        }

        public int DurationScore
        {
            get { return durationScore; }
            set { durationScore = value; }
        }

        public int Multiplier
        {
            get { return multiplier; }
            set { multiplier = value; }
        }
        public float Fluency
        {
            get
            {
                return GetBand(GetFluencyAvg());
            }
        }

        private float GetFluencyAvg()
        {
            float sum = 0;
            for (int i = 0; i < fluencyHistory.Count; i++)
            {
                sum += fluencyHistory[i];
            }
            if (fluencyHistory.Count > 0)
            {
                return sum / fluencyHistory.Count;
            }
            return 0;
        }

        private float GetBand(float val)
        {
            /*
                Let's say there's 4 bands:
                A - Top 25%
                B - Top 50%
                C - Top 75%
                D - Top 100%
            */
            const float maxScore = 100f;
            const int numBands = 4;
            float band = maxScore / numBands;
            float bandedValue = Mathf.Round(val / band) * band;
            Debug.LogFormat("v {0} max {1} bandNum {2} bVal {3}", val, maxScore, numBands, bandedValue);
            return bandedValue;
        }

        public ScoreManager()
        {
            this.pitchScore = 0;
            ResetMultiplier();
            this.fluencyHistory = new List<float>();
        }
        public ScoreManager(int score, int multiplier)
        {
            this.pitchScore = score;
            this.multiplier = multiplier;
            this.fluencyHistory = new List<float>();
        }

        public void ResetMultiplier()
        {
            multiplier = 1;
        }

        public void AddFluency(float value)
        {
            fluencyHistory.Add(value);
        }

        public void ClearFluency()
        {
            fluencyHistory.Clear();
        }
    }

    public class SongLevelDirector : MonoBehaviour
    {
        #region Public delegates and events
        // Behaviour that should happen before the level starts
        public delegate void LevelStartHandler ();
        public static event LevelStartHandler OnLevelStart;

        // Behaviour that should happen when a note should spawn
        public delegate void NoteSpawnHandler (MidiTrack.NoteDuration note, bool isPlayer);
        public static event NoteSpawnHandler OnNoteSpawn;

        // Behaviour that should happen when the score changes
        public delegate void ScoreEventHandler (int value);
        public static event ScoreEventHandler OnScoreChange;

        // Behaviour that should happen when the multiplier changes
        public delegate void MultiplierEventHandler (int value);
        public static event MultiplierEventHandler OnMultiplierChange;

        // Behaviour that should happen when the level ends
        public delegate void ShowEndScreenHandler(ScoreManager scoreMgr, int noteCount, bool isPracticeMode);
        public static event ShowEndScreenHandler OnShowEndScreen;

        // Behaviour that should happen when the level restarts
        public delegate void RestartLevelHandler();
        public static event RestartLevelHandler OnLevelRestart;

        // Behaviour that should happen when the gameplay pauses
        public delegate void PauseLevelHandler();
        public static event PauseLevelHandler OnLevelPause;

        // Behaviour that should happen when the gameplay resumes from being paused
        public delegate void ResumeLevelHandler();
        public static event ResumeLevelHandler OnLevelResume;

        // Behaviour that should happen when the songs have been loaded
        public delegate void LoadedSongsHandler();
        public static event LoadedSongsHandler OnLoadedSongs;

        // Behaviour that should happen when the tempo changes
        public delegate void TempoChangeHandler(float newBpm);
        public static event TempoChangeHandler OnTempoChange;

        #endregion

        #region Public properties
        public bool Started
        {
            get { return hasStarted; }
        }
        public bool AllNotesSpawned
        {
            get { return !IsPlaying(); }
        }
        #endregion

        #region Private member variables
        // Loaded data from GameDirector
        SongLevelData data;

        // MIDI objects
        MidiFileContainer[] songs;
        List<MidiSheetSequencer> sequencers;
        int songIdx = 0;
        [SerializeField]
        bool[] playChannelMap;

        // Gameplay set up objects
        List<MidiTrack.NoteDuration>.Enumerator durationEtr;
        float bpm = 120;

        // Gameplay state objects
        bool isPaused = false;
        bool hasStarted = false; // Revealed the state of this variable through a read only public property
        ScoreManager scoreMgr;
        #endregion

        #region Public variables and properties
        public int whichTrack = 0;
        public Channel whichChannel = Channel.Channel1;

        [SerializeField]
        Slider adjustTempoSlider;

        public float Bpm
        {
            get { return bpm; }
        }
        public int Ppqn
        {
            get
            {
                if (songs != null)
                {
                    return songs[songIdx].division;
                }
                else
                {
                    return 24;
                }
            }
        }
        public SongLevelParams LevelParameters
        {
            get
            {
                return data.parameters;
            }
        }
        public MidiFileContainer CurrentSong
        {
            get
            {
                return songs[songIdx];
            }
        }
        public bool[] ChannelMap
        {
            get { return playChannelMap; }
            set { playChannelMap = value; }
        }
        public int FullNoteCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < CurrentSong.channelData.Length; i++)
                {
                    count += CurrentSong.channelData[i].noteCount;
                }
                return count;
            }
        }

        public int noteCount = 0;
        public float bpmScale = 1f;
        #endregion

        #region Private methods
        void RevertToSongBpm()
        {
            bpm = songs[songIdx].bpm;
            if (OnTempoChange != null)
            {
                OnTempoChange(bpm);
            }
        }

        void LoadSongs()
        {
            Debug.Log("Load songs");
            // Load the song from data midi file name
            songs = new MidiFileContainer[data.midiFiles.Length];
            for (int i = 0; i < data.midiFiles.Length; i++)
            {
                songs[i] = MidiFileLoader.Load(data.midiFiles[i].bytes);
            }
            
            // check if practice mode
            Debug.LogFormat("Song with params {0}", data.parameters.ToString());

            songIdx = 0;
            RevertToSongBpm();
            bpm *= bpmScale;
            if (OnTempoChange != null)
            {
                OnTempoChange(bpm);
            }
            Debug.LogFormat("BPM IS {0}", bpm);
            SetupSequencers(songs[songIdx]);

            playChannelMap = null;

            //CountTrackNotes();

            // Wait a second to avoid stuttering
            //yield return new WaitForSeconds(1.0f);
        }

        void CountTrackNotes()
        {
            // pretend that each track is selected
            if (playChannelMap == null)
            {
                playChannelMap = new bool[songs[songIdx].tracks.Count];
                for (int a = 0; a < songs[songIdx].tracks.Count; a++) {
                    playChannelMap[a] = true;
                }
            }
            //noteCount = songs[songIdx].tracks[whichTrack].GetDurationNoteCount();
            // count the number of expected notes according to the tracks selected
            noteCount = 0;
            for (int b = 0; b < playChannelMap.Length; b++)
            {
                if (playChannelMap[b])
                {
                    noteCount += songs[songIdx].tracks[b].GetDurationNoteCount();
                }
            }
            Debug.LogFormat("Number of notes in song: {0}", noteCount);
        }

        void SetupSequencers(MidiFileContainer currentSong)
        {
            sequencers.Clear();
            sequencers.Capacity = currentSong.tracks.Count;
            for (int i = 0; i < sequencers.Capacity; i++)
            {
                sequencers.Add(new MidiSheetSequencer(currentSong.tracks[i], currentSong.division, currentSong.bpm));
                sequencers[i].ChangeBpm(currentSong.division, bpm);
            }
        }

        void StartSequencers()
        {
            for (int i = 0; i < sequencers.Count; i++)
            {
                DispatchEvents(i, sequencers[i].Start(Time.deltaTime));
            }
        }


        void UpdateSequencers()
        {
            for (int i = 0; i < sequencers.Count; i++)
            {
                if (sequencers[i].Playing)
                {
                    DispatchEvents(i, sequencers[i].Advance(Time.deltaTime));
                }
            }

        }

        void DispatchEvents(int idx, List<MidiTrack.NoteDuration> events)
        {
            if (events != null)
            {
                if (!hasStarted)
                {
                    hasStarted = true;
                }

                foreach (var e in events)
                {
                    bool isPlayer = playChannelMap[e.channel];
                    if (OnNoteSpawn != null)
                    {
                        OnNoteSpawn(e, isPlayer);
                    }
                }
            }
        }

        bool IsPlaying()
        {
            return sequencers.Exists(seq => seq.Playing);
        }

        void UpdateScoreDisplay()
        {
            if (OnScoreChange != null)
            {
                OnScoreChange(scoreMgr.PitchScore);
            }
        }

        void UpdateMultiplierDisplay()
        {
            if (OnMultiplierChange != null)
            {
                OnMultiplierChange(scoreMgr.Multiplier);
            }
        }
        #endregion

        #region Public methods
        public void ChangeTempoScale(float multipleOfFive)
        {
            float value = multipleOfFive * 5;
            bpmScale = value / 100;     // convert to normalised percentage
            Debug.LogFormat("NEW BPMSCALE: {0}", bpmScale);
            RevertToSongBpm();
            bpm *= bpmScale;
            for (int i = 0; i < sequencers.Count; i++)
            {
                sequencers[i].ChangeBpm(CurrentSong.division, bpm);
            }
            if (OnTempoChange != null)
            {
                OnTempoChange(bpm);
            }
        }

        public void Play()
        {
            // Start the countdown by invoking the handlers subscribed to this event
            if (OnLevelStart != null)
            {
                OnLevelStart();
            }
            else
            {
                StartLevel();
            }
        }
        public void Quit()
        {
            Debug.Log("Quitting level");

            // save any information or data analytics during this level

            // load the galaxy menu
        }

        public void Restart()
        {
            Debug.Log("Restarting level!");
            Debug.LogFormat("Fluency AVG WAS {0}", scoreMgr.Fluency);
            scoreMgr.PitchScore = 0;
            scoreMgr.DurationScore = 0;
            scoreMgr.ExpressionScore = 0;
            scoreMgr.ClearFluency();
            UpdateScoreDisplay();
            scoreMgr.ResetMultiplier();
            UpdateMultiplierDisplay();

            // Call any listeners that need to do something on restart
            if (OnLevelRestart != null)
            {
                OnLevelRestart();
            }

            SetupSequencers(songs[songIdx]);
            //CountTrackNotes();

            Play();
        }

        public void StartLevel()
        {
            Debug.Log("Count down finished, start level");
            StartSequencers();
            Debug.Log("done");
        }

        public void FinishLevel()
        {
            Debug.LogFormat("Finished Level. Score: {0}", scoreMgr.PitchScore);

            if (!data.parameters.isGalaxySong)
            {
                // set level as played if not played before
                string currentLevel = GameDirector.Instance.SongLevel.common.title;
                MenuSession.Instance.SetPlayedLevel(currentLevel);
            }
            else
            {
                MenuSession.Instance.SetPlayedLevel(GameDirector.Instance.SongLevel.midiFiles[0].name);
            }

            if (OnShowEndScreen != null)
            {
                OnShowEndScreen(scoreMgr, noteCount, data.parameters.practiceMode);
            }
        }

        public void ExitToGalaxy()
        {
            GameDirector.Instance.LoadLevel("GalaxyMenu");
        }

        public void ExitToMainMenu()
        {
            GameDirector.Instance.LoadLevel("SplashScreen");
        }

        public void PauseGame()
        {
            isPaused = !isPaused;

            if (isPaused)
            {
                Time.timeScale = 0.0f;
                if (OnLevelPause != null)
                {
                    OnLevelPause();
                }
            }
            else
            {
                Time.timeScale = 1.0f;
                if (OnLevelResume != null)
                {
                    OnLevelResume();
                }
            }
            Debug.LogFormat("PAUSED: {0}", isPaused);
        }

        public void ChangePitchScore(int value)
        {
            scoreMgr.PitchScore += value * scoreMgr.Multiplier;
            UpdateScoreDisplay();
        }

        public void ChangeExpressionScore(int value)
        {
            scoreMgr.ExpressionScore += value;
        }

        public void ChangeDurationScore(int value)
        {
            scoreMgr.DurationScore += value;
        }

        public void AdvanceMultiplier(int value)
        {
            scoreMgr.Multiplier += value;
            UpdateMultiplierDisplay();
        }
        public void ResetMultiplier()
        {
            scoreMgr.ResetMultiplier();
            UpdateMultiplierDisplay();
        }
        public void AddFluency(float value)
        {
            scoreMgr.AddFluency(value);
        }
        #endregion

        #region Monobehavior methods
        // Use this for initialization
        IEnumerator Start()
        {
            scoreMgr = new ScoreManager();

            // Get song level data from the peristent Game Director
            data = GameDirector.Instance.SongLevel;
            Debug.LogFormat("Starting Song Level with data: {0}", data);

            // disable tempo slider if not in practice mode
            adjustTempoSlider.gameObject.SetActive(data.parameters.practiceMode);

            sequencers = new List<MidiSheetSequencer>();
            LoadSongs();

            OutputMidiMixer mixer = GetComponent<OutputMidiMixer>();
            mixer.ChangeTempo(bpm, this.Ppqn);

            // load the songs then start the countdown
            yield return new WaitForSeconds(1.0f);

            if (OnLoadedSongs != null)
            {
                OnLoadedSongs();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (sequencers != null)
            {
                if (IsPlaying())
                {
                    UpdateSequencers();
                }
                else
                {
                    //Will finish level in MusicMatter Manager
                    //FinishLevel();

                }
            }
        }
        #endregion
    }
}
