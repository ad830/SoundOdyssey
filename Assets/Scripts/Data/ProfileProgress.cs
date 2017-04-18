using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using SoundOdyssey;

namespace SoundOdyssey
{
    public class LevelProgress
    {
        public bool played = false;
        public bool unlocked = false;

        public LevelProgress()
        {
            this.played = false;
            this.unlocked = false;
        }
        public LevelProgress(bool _unlocked, bool _played = false)
        {
            this.unlocked = _unlocked;
            this.played = _played;
        }
    }
    public class ProfileProgress
    {
        private Dictionary<string, LevelProgress> playedLevels;

        [XmlIgnore]
        public Dictionary<string, LevelProgress> PlayedLevels
        {
            get { return playedLevels; }
            set { playedLevels = value; }
        }

        [XmlElement(ElementName="PlayedLevels")]
        public KeyValuePair<string, LevelProgress> [] SerialisedPlayedLevels
        {
            get
            {
                var serialisedList = new List<KeyValuePair<string, LevelProgress>>(playedLevels.Count);
                foreach (string key in playedLevels.Keys)
                {
                    serialisedList.Add(new KeyValuePair<string, LevelProgress>(key, playedLevels[key]));
                }
                return serialisedList.ToArray();
            }
            set
            {
                playedLevels = new Dictionary<string, LevelProgress>(value.Length);
                foreach (var item in value)
                {
                    playedLevels.Add(item.Key, item.Value);                    
                }
            }
        }

        public void SetLevelProgress(string level, LevelProgress status)
        {
            if (playedLevels.ContainsKey(level))
            {
                playedLevels[level] = status;
            }
            else
            {
                playedLevels.Add(level, status);
            }
        }
        public LevelProgress GetLevelProgress(string level, bool isUnlocked)
        {
            if (playedLevels.ContainsKey(level))
            {
                return playedLevels[level];
            }
            else
            {
                return new LevelProgress(isUnlocked);
            }
        }

        public ProfileProgress()
        {
            this.playedLevels = new Dictionary<string, LevelProgress>();
        }
    }    
}


