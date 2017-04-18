using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using SoundOdyssey;
using IniParser;

namespace SoundOdyssey
{
    public class GameDirector
    {
        #region Private member variables
        private SoundOdyssey.Profile currentProfile;

        private SongLevelData songLevelData;
        private TutorialLevelData tutorialLevelData;

        // Current exam session
        private Campaign.Exam examData;
        private ScalesSection scalesData;
        private bool scalesPracticeMode;
        private int scaleIdx = 0;
        int currentScaleProgress = 0;
        int scaleScore = 0;
        int maxScaleScore = 0;
        private Dictionary<string, bool> examSectionPassed;

        private MidiResourceHolder midiResources;
        private AudioAvatar audioMaster = AudioAvatar.Instance;
        #endregion

        #region Public methods and properties
        public enum MusicStatus { Play, Stop };


        public int MaxScaleScore
        {
            get
            {
                return maxScaleScore;
            }
            set
            {
                maxScaleScore = value;
            }
        }

        public int CurrentScaleProgress
        {
            get
            {
                return currentScaleProgress;
            }

            set
            {
                currentScaleProgress = value;
            }

        }

        public int ScaleScore
        {
            get
            {
                return scaleScore;
            }

            set
            {
                scaleScore = value;
            }

        }

        public SoundOdyssey.Profile CurrentProfile
        {
            get
            {
                return currentProfile;
            }
            set
            {
                currentProfile = value;
            }
        }

        public SongLevelData SongLevel
        {
            get
            {
                return songLevelData;
            }
        }

        public TutorialLevelData TutorialLevel
        {
            get
            {
                return tutorialLevelData;
            }
        }

        // Returns the current exam data, null if there is no current exam
        public Campaign.Exam ExamLevel
        {
            get
            {
                return examData;
            }
            set
            {
                examData = value;
            }
        }
        // Returns the current passed exam passed status, null if there is no current exam
        public List<KeyValuePair<string, bool>> CurrentExamPassed
        {
            get
            {
                if (examSectionPassed == null)
                {
                    return null;
                }
                var list = new List<KeyValuePair<string, bool>>(examSectionPassed.Count);
                foreach (var key in examSectionPassed.Keys)
                {
                    list.Add(new KeyValuePair<string, bool>(key, examSectionPassed[key]));
                }
                return list;
            }
        }

        public bool IsScalesPractice
        {
            get
            {
                return scalesPracticeMode;
            }
        }
        public ScalesSection ExamScalesSection
        {
            get
            {
                return scalesData;
            }
        }
        public Midi.Scale CurrentScale
        {
            get
            {
                return scalesData.Scales[scaleIdx];
            }
        }

        public int CurrentScaleIndex
        {
            get
            {
                return scaleIdx;
            }
        }

        public void PlayExamScales(ScalesSection scalesSection, int whichScale, bool practiceMode = true)
        {
            scalesData = scalesSection;
            scaleIdx = whichScale;
            Debug.LogFormat("scale Idx {0}", scaleIdx);
            LoadLevel("ScalesSection", MusicStatus.Stop);
            scalesPracticeMode = practiceMode;
        }

        public void StartExam(Campaign.Exam exam)
        {
            examData = exam;

            // initialise the current scores
            if (examSectionPassed == null)
            {
                examSectionPassed = new Dictionary<string, bool>(10);
            }
            examSectionPassed.Clear();
            foreach (var section in exam.sections)
            {
                examSectionPassed.Add(section.Key, false);
            }
        }

        public void EndExam()
        {
            examData = null;
            examSectionPassed.Clear();
        }

        public void PlayCampaignLevel(Campaign.Level level, SongLevelParams songParams = new SongLevelParams())
        {
            if (level.type == Campaign.Level.LevelType.Song)
            {
                MidiAsset[] midiFiles = new MidiAsset[1];
                MidiAsset loadedMidi = midiResources.GetMidiByName(level.song.midiFilename);
                if (loadedMidi == null)
                {
                    Debug.Log("No Song Level for midi file " + level.song.midiFilename);
                    Debug.Break();
                }
                else
                {
                    Debug.LogFormat("Song loaded {0}", level.song.midiFilename);
                    midiFiles[0] = loadedMidi;
                }
                Debug.LogFormat("Play Song - {0}", songParams.ToString());
                songLevelData = new SongLevelData(level.info.name, level.info.id, midiFiles, 0, songParams);
                LoadLevel("SongLevel", MusicStatus.Stop);
            }
            else
            {
                Debug.LogFormat("Will play teaching level {0}", level.info.name);
                tutorialLevelData = new TutorialLevelData(level.info.name, level.info.id);
                LoadLevel(level.teaching.sceneFilename, MusicStatus.Stop);
            }
        }

        public void PlayGalaxySongLevel(string midiFileName, SongLevelParams songParams)
        {
            MidiAsset[] midiFiles = new MidiAsset[1];
            midiFiles[0] = midiResources.GetMidiByName(midiFileName);

            songLevelData = new SongLevelData("galaxy song", -1, midiFiles, 0, songParams);
            LoadLevel("SongLevel", MusicStatus.Stop);

        }

        public void LoadLevel(string levelName, MusicStatus shouldPlayMusic = MusicStatus.Play)
        {
            if (shouldPlayMusic == MusicStatus.Play)
            {
                audioMaster.isSilent = false;
                audioMaster.PlayRandomMusic();
            }
            else if (shouldPlayMusic == MusicStatus.Stop)
            {
                // stop music playing
                audioMaster.Stop();
                audioMaster.isSilent = true;
            }
            Application.LoadLevel(levelName);
        }
        /*
        public void PlaySongLevel(string midiFilename, string title)
        {

        }
        public void PlayTeachingLevel(string sceneFilename)
        {

        }
        */
        #endregion

        #region Private methods
        // to make it easier to test different songs
        string LoadTestSong(string path)
        {
            FileIniDataParser parser = new FileIniDataParser();
            IniParser.Model.IniData data = parser.ReadFile(path);
            return data["TestSong"]["filename"];
        }



        #endregion

        #region Public methods
        GameDirector()
        {
            midiResources = MidiResourceHolder.Instance;

            MidiAsset[] midiFiles = new MidiAsset[1];
            if (Debug.isDebugBuild)
            {
                for (int i = 0; i < midiFiles.Length; i++)
                {
                    // 5, 6
                    // example6.mid
                    // Fur Elise-Beethoven.mid
                    //midiFiles[i] = midiResources.GetMidiByName("Hot Cross Buns.mid");
                    string testMidiFile = LoadTestSong(Path.Combine(Application.dataPath, "testDirector.ini"));
                    midiFiles[i] = midiResources.GetMidiByName(testMidiFile);
                    //midiFiles[i] = midiResources.GetMidiByName("example6.mid");
                }
            }

            songLevelData = new SongLevelData("Default Song Level", -1, midiFiles, 0);
            tutorialLevelData = new TutorialLevelData("Default Tutorial Level", -1);
            currentProfile = null;
            scalesPracticeMode = true;
        }
        #endregion

        #region Singleton class instance handling
        static GameDirector instance;

        public static GameDirector Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameDirector();
                }
                return instance;
            }
        }
        #endregion
    }

}

