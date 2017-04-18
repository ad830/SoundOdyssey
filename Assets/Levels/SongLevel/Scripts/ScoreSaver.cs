using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Globalization;
using SoundOdyssey;
using IniParser;

namespace SoundOdyssey
{
    public struct ScoreInfo
    {
        public string profileName;
        public string levelName;
        public string midiFile;
        public int levelId;
        public DateTime dateTime;
        public int notesHit;
        public int noteCount;
        public float fluency;
    }

    public class ScoreSaver : MonoBehaviour
    {
        void SaveMostRecentScore (ScoreManager scoreMgr, int noteCount, bool isPracticeMode)
        {
            // Get the profile information

            // Save score information with profile information to file

            GameDirector director = GameDirector.Instance;
            MenuSession session = MenuSession.Instance;

            if (director.CurrentProfile == null)
            {
                Debug.LogError("No current profile loaded, please reload the game with a profile");
                Debug.Break();
                return;
            }

            // don't save practice mode scores?
            float clampedExpression = Mathf.Clamp01((float)scoreMgr.ExpressionScore / (float)noteCount);

            if (!director.SongLevel.parameters.isGalaxySong)
            {
                Debug.Log("added score");
                SoundOdyssey.Score scoreRecord = new SoundOdyssey.Score(
                    director.CurrentProfile,
                    director.SongLevel.common.title,
                    director.SongLevel.common.id,
                    scoreMgr.PitchScore,
                    noteCount,
                    scoreMgr.PitchScore,
                    clampedExpression,
                    (float)scoreMgr.PitchScore / (float)noteCount,
                    scoreMgr.Fluency / 100.0f,
                    isPracticeMode
                );
                session.AddScore(scoreRecord);
            }
            else
            {
                Debug.Log("added galaxy song score");
                SoundOdyssey.Score scoreRecord = new SoundOdyssey.Score(
                    director.CurrentProfile,
                    director.SongLevel.midiFiles[0].name,
                    -2,
                    scoreMgr.PitchScore,
                    noteCount,
                    scoreMgr.PitchScore,
                    clampedExpression,
                    (float)scoreMgr.PitchScore / (float)noteCount,
                    scoreMgr.Fluency / 100.0f,
                    isPracticeMode
                );
                session.AddScore(scoreRecord);
            }
            /*
            var path = Path.Combine(Application.persistentDataPath, "scores.xml");

            List<ScoreInfo> scores = new List<ScoreInfo>();

            if (File.Exists(path))
            {
                // read from doc
                System.Xml.Serialization.XmlSerializer reader =
                    new System.Xml.Serialization.XmlSerializer(typeof(ScoreInfo []));

                System.IO.StreamReader file = new System.IO.StreamReader(path);
                ScoreInfo [] fileScores = (ScoreInfo [])reader.Deserialize(file);
                file.Close();

                scores.AddRange(fileScores);
            }

            ScoreInfo tempScore = new ScoreInfo();
            string profileName = "NullProfile";
            if (director.CurrentProfile != null)
            {
                profileName = director.CurrentProfile.Name;
            }
            tempScore.profileName = profileName;
            tempScore.levelName = director.SongLevel.common.title;
            tempScore.midiFile = director.SongLevel.midiFiles[0].name;
            tempScore.dateTime = DateTime.Now;
            tempScore.levelId = director.SongLevel.common.id;
            tempScore.notesHit = score;
            tempScore.noteCount = noteCount;
            tempScore.fluency = fluency;

            scores.Add(tempScore);

            // overwrite file
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(ScoreInfo[]));

            System.IO.FileStream outFile = System.IO.File.Create(path);
            writer.Serialize(outFile, scores.ToArray());
            outFile.Close();
            */
            /*
            IniParser.Model.IniData data = null;
            FileIniDataParser parser = new FileIniDataParser();
            string filePath = Path.Combine(Application.dataPath, "recent_scores.ini");
            if (File.Exists(filePath))
            {
                data = parser.ReadFile(filePath);
            }
            else
            {
                data = new IniParser.Model.IniData();
            }

            DateTime dateTime = DateTime.Today;

            data.Sections.AddSection(sectionName);
            data[sectionName].AddKey("profile", director.currentProfile.playerName);
            data[sectionName].AddKey("midifile", director.SongLevel.midiFiles[0].name);
            data[sectionName].AddKey("score", score.ToString());
            data[sectionName].AddKey("note-count", noteCount.ToString());
            data[sectionName].AddKey("fluency", fluency.ToString());

            parser.WriteFile(filePath, data);
            */
            Debug.Log("Added score to session scores.");
        }

        void OnEnable()
        {
            SongLevelDirector.OnShowEndScreen += SaveMostRecentScore;
        }
        void OnDisable()
        {
            SongLevelDirector.OnShowEndScreen -= SaveMostRecentScore;
        }

        // Use this for initialization
        void Start()
        {

        }
    }
}
