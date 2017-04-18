using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using IniParser;
using SoundOdyssey;

namespace SoundOdyssey
{

    public static class FileDirector
    {
        #region File paths
        public const string settingsFilePath = "settings.xml";
        public const string profilesDirectoryPath = "/Profiles/";
        public const string scoresFilePath = "scores.xml";
        public const string profilesListFilename = "profileNames.xml";

        #endregion

        /*
        public class Score
        {
            int playerName;
            string levelName;

            float expression;
            float accuracy;
            float fluency;
            int totalNotesHit;
            int totalNotes;
            int totalScore;

            public override string ToString()
            {
                string format = "Player Name {0} | LevelName {1} | Exp {2} | Acc {3} | Flu {4} | TotalHit {5} | TotalNotes {6} | TotalScore {7}";
                string str = string.Format(format, playerName, levelName, expression, accuracy, fluency, totalNotesHit, totalNotes, totalScore);
                return str;
            }
        }
        */
        /*
        public class Profile
        {
            public string playerName;
            public byte lastLevelUnlocked;
            public uint timePlayed;
            public int grade;
            public Score [] scores;
            public ProfileProgress progress;

            public Profile()
            {
                this.scores = new Score[1];
                this.progress = new ProfileProgress();
            }
        }
        */

        public class Settings
        {
            public SettingsData.AudioSettings audio;
            public SettingsData.VideoSettings video;
            public SettingsData.GameSettings game;
            public SettingsData.MidiDeviceSettings midi;

            public Settings()
            {
                audio = new SettingsData.AudioSettings();
                video = new SettingsData.VideoSettings();
                game = new SettingsData.GameSettings();
                midi = new SettingsData.MidiDeviceSettings();
            }
        }

        /*
        public class Level
        {
            int id;
            int grade;
            string name;
            string type; // lesson, song or exam.
            string description;
        }
        */


        /* MANAGING SETTINGS IO */
        #region Settings Input / Output
        public static void WriteSettings(Settings s)
        {
            var path = Path.Combine(Application.persistentDataPath, settingsFilePath);

            // overwrite file
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(Settings));

            System.IO.FileStream outFile = System.IO.File.Create(path);
            writer.Serialize(outFile, s);
            outFile.Close();
        }
        // Returns null if no existing settings were found
        public static Settings ReadSettings()
        {
            var path = Path.Combine(Application.persistentDataPath, settingsFilePath);

            if (!File.Exists(path))
            {
                return null;
            }

            // read from file
            System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof(Settings));

            System.IO.StreamReader file = new System.IO.StreamReader(path);
            Settings readBack = (Settings)reader.Deserialize(file);
            file.Close();
            return readBack;
        } 
        #endregion

        /* MANAGING PROFILE IO */
        /*
        public static void CreateNewProfile(Profile p)
        {
            if (!File.Exists(p.playerName + ".xml"))
            {
                System.Xml.Serialization.XmlSerializer writer =
                    new System.Xml.Serialization.XmlSerializer(typeof(Profile));

                var path = Application.persistentDataPath + profilesDirectoryPath + p.playerName + ".xml";
                System.IO.FileStream file = System.IO.File.Create(path);

                writer.Serialize(file, p);
                file.Close();
            }
        }

		public static void RenameProfile(Profile old, Profile newP)
		{
            string dirPath = Application.persistentDataPath + profilesDirectoryPath;
			string oldFile = dirPath + old.playerName + ".xml";
			if (File.Exists(oldFile))
			{
				File.Delete(oldFile);
			}
			CreateNewProfile(newP);
		}

        public static void UpdateProfile(Profile p)
        {
            if (File.Exists(Application.persistentDataPath + profilesDirectoryPath + p.playerName + ".xml"))
            {
                System.Xml.Serialization.XmlSerializer writer =
                    new System.Xml.Serialization.XmlSerializer(typeof(Profile));

                var path = Application.persistentDataPath + profilesDirectoryPath + p.playerName + ".xml";
                System.IO.FileStream file = System.IO.File.Create(path);

                writer.Serialize(file, p);
                file.Close();
            }
        }

        public static void DeleteProfile(string name)
        {
            string path = Application.persistentDataPath + profilesDirectoryPath + name + ".xml";
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }

        public static Profile LoadProfile(string name)
        {
            System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof(Profile));

            System.IO.StreamReader file = new System.IO.StreamReader(Application.persistentDataPath + profilesDirectoryPath + name + ".xml");
            Profile p = (Profile)reader.Deserialize(file);
            file.Close();

            return p;
        }
        */

        #region Rewritten Profile I/O
        public static string GetProfilePath(string name)
        {
            return Application.persistentDataPath + profilesDirectoryPath + name + ".xml";
        }

        public static void WriteProfiles(List<SoundOdyssey.Profile> profileArray)
        {
            foreach (SoundOdyssey.Profile profile in profileArray)
            {
                ActualWriteProfile(profile);
            }
            // ones that don't match any more are deleted
            string[] filenames = LoadProfileNames();
            foreach (string file in filenames)
            {
                if (!profileArray.Exists((prof) => { return prof.Name == file; }))
                {
                    ActualDeleteProfile(file);
                }
            }
        }

        public static bool ActualRenameProfile(string oldName, SoundOdyssey.Profile profile)
        {
            bool canRename = ActualProfileExists(oldName);
            if (canRename)
            {
                ActualDeleteProfile(oldName);
                ActualWriteProfile(profile);
            }
            return canRename;
        }

        public static void ActualWriteProfile(SoundOdyssey.Profile profile)
        {
            // check if the file exists
            if (ActualProfileExists(profile.Name))
            {
                Debug.LogWarningFormat("Writing Profile [WARNING]: Overwriting existing file {0}", profile.Name + ".xml");
                ActualDeleteProfile(profile.Name);
            }
            string path = GetProfilePath(profile.Name);

            System.Xml.Serialization.XmlSerializer writer =
                    new System.Xml.Serialization.XmlSerializer(typeof(SoundOdyssey.Profile));

            System.IO.FileStream file = System.IO.File.Create(path);
            writer.Serialize(file, profile);
            file.Close();
        }

        public static void ActualDeleteProfile(string name)
        {
            string path = GetProfilePath(name);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public static void ActualDeleteAllProfiles()
        {
            string[] profileNames = LoadProfileNames();
            foreach (string name in profileNames)
            {
                ActualDeleteProfile(name);
            }
        }

        public static bool ActualProfileExists(string name)
        {
            string path = GetProfilePath(name);
            return File.Exists(path);
        }

        public static SoundOdyssey.Profile ActualLoadProfile(string name)
        {
            System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof(SoundOdyssey.Profile));

            System.IO.StreamReader file = new System.IO.StreamReader(GetProfilePath(name));
            SoundOdyssey.Profile p = (SoundOdyssey.Profile)reader.Deserialize(file);
            file.Close();

            return p;
        }

        public static List<SoundOdyssey.Profile> ReadProfiles()
        {
            // get a list of profile names
            string[] profileNames = LoadProfileNames();
            List<SoundOdyssey.Profile> profileList = new List<SoundOdyssey.Profile>();
            foreach (string name in profileNames)
            {
                profileList.Add(ActualLoadProfile(name));
            }
            return profileList;
        }

        public static string[] LoadProfileNames()
        {
            var path = Application.persistentDataPath + profilesDirectoryPath;
			//check if directory doesn't exit
			if(!Directory.Exists(path))
			{    
				//if it doesn't, create it
				Directory.CreateDirectory(path);
                //Debug.LogFormat("CREATING FOLDER AT: {0}", path);
			}

            Debug.LogFormat("Loading profile names from {0}", path);
            string[] profileFiles = GetFileNames(path, "*.xml");
            Debug.LogFormat("Size {0}", profileFiles.Length);
            for (int i = 0; i < profileFiles.Length; i++)
            {
                profileFiles[i] = Path.GetFileNameWithoutExtension(profileFiles[i]);
               // Debug.Log(profileFiles[i]);
            }

            return profileFiles;
        }
        #endregion

        #region Score I/O
        private static string GetScoresPath()
        {
            return Path.Combine(Application.persistentDataPath, scoresFilePath);
        }
        public static List<SoundOdyssey.Score> ReadScores()
        {
            var path = GetScoresPath();

            if (!File.Exists(path))
            {
                return null;
            }

            System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof(SoundOdyssey.Score[]));

            System.IO.StreamReader file = new System.IO.StreamReader(path);
            SoundOdyssey.Score[] scoresArray = (SoundOdyssey.Score[])reader.Deserialize(file);
            file.Close();

            return new List<SoundOdyssey.Score>(scoresArray);
        }
        public static void WriteScores(SoundOdyssey.Score[] scores)
        {
            var path = GetScoresPath();

            // overwrite file
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(SoundOdyssey.Score[]));

            System.IO.FileStream outFile = System.IO.File.Create(path);
            writer.Serialize(outFile, scores);
            outFile.Close();
        }
        public static void AppendScore(SoundOdyssey.Score s)
        {
            var path = GetScoresPath();

            List<SoundOdyssey.Score> scores = new List<SoundOdyssey.Score>();

            if (File.Exists(path))
            {
                // read scores from file
                System.Xml.Serialization.XmlSerializer reader =
                    new System.Xml.Serialization.XmlSerializer(typeof(SoundOdyssey.Score[]));

                System.IO.StreamReader file = new System.IO.StreamReader(path);
                SoundOdyssey.Score[] fileScores = (SoundOdyssey.Score[])reader.Deserialize(file);
                file.Close();

                scores.AddRange(fileScores);
            }

            // append new score
            scores.Add(s);

            // overwrite file
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(SoundOdyssey.Score[]));

            System.IO.FileStream outFile = System.IO.File.Create(path);
            writer.Serialize(outFile, scores.ToArray());
            outFile.Close();
        }
        #endregion

        private static string[] GetFileNames(string path, string filter)
        {
            string[] files = Directory.GetFiles(path, filter);
            for(int i = 0; i < files.Length; i++)
                files[i] = Path.GetFileName(files[i]);
            return files;
        }

    }
}
