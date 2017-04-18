using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SoundOdyssey;

namespace SoundOdyssey
{
    public static class SettingsData
    {
        #region Game Settings
        public class GameSettings
        {
            private bool noteLabelsOn = true;
            private bool auraVisible = true;
            private bool powerGaugeOn = true;
            private bool backgroundEffects = true;

            #region Public properties
            public bool NoteLabels
            {
                get { return noteLabelsOn; }
                set { noteLabelsOn = value; }
            }

            public bool AuraVisible
            {
                get { return auraVisible; }
                set { auraVisible = value; }
            }

            public bool PowerGauge
            {
                get { return powerGaugeOn; }
                set { powerGaugeOn = value; }
            }

            public bool BackgroundEffects
            {
                get { return backgroundEffects; }
                set { backgroundEffects = value; }
            } 
            #endregion
        } 
        #endregion

        #region Audio Settings
        public class AudioSettings
        {
            // General audio settings
            private float masterVolume = 0f;        // logarithmic volume, so it ranges from -80 to 0 (max).
            private float musicVolume = 0f;         // ditto

            // Maybe remove these:
            // MIDI Volume Settings
            private int percussionVolume = 127;             // 0 min, 127 max
            private int playerVolume = 127;                 // 0 min, 127 max
            private int accompVolume = 127;                 // 0 min, 127 max
            private int effectsVolume = 127;                // 0 min, 127 max
            private bool useSongVolume = true;              // will the song level follow how the MIDI file intended?

            #region Public properties
            // General audio settings
            public float MasterVolume
            {
                get { return masterVolume; }
                set { masterVolume = value; }
            }
            public float MusicVolume
            {
                get { return musicVolume; }
                set { musicVolume = value; }
            }

            // MIDI volume settings
            public bool UseSongVolume
            {
                get { return useSongVolume; }
                set { useSongVolume = value; }
            }
            public int PercussionVolume
            {
                get { return percussionVolume; }
                set { percussionVolume = value; }
            }
            public int PlayerVolume
            {
                get { return playerVolume; }
                set { playerVolume = value; }
            }
            public int AccompVolume
            {
                get { return accompVolume; }
                set { accompVolume = value; }
            }
            public int EffectsVolume
            {
                get { return effectsVolume; }
                set { effectsVolume = value; }
            } 
            #endregion
        } 
        #endregion

        #region Video Settings
        public class VideoSettings
        {
            private bool fullscreen = true;
            private Resolution currentResolution;
            private int antiAliasing = 0;
            private int qualityLevel = 3;                // max quality settings
            private int vsync = 0;

            #region Public properties
            public bool Fullscreen
            {
                get { return fullscreen; }
                set { fullscreen = value; }
            }
            public Resolution CurrentResolution
            {
                get { return currentResolution; }
                set { currentResolution = value; }
            }
            public int AntiAliasing
            {
                get { return antiAliasing; }
                set { antiAliasing = value; }
            }
            public int QualityLevel
            {
                get { return qualityLevel; }
                set { qualityLevel = value; }
            }
            public int Vsync
            {
                get { return vsync; }
                set { vsync = value; }
            } 
            #endregion
        } 
        #endregion

        #region Midi Device Settings
        public class MidiDeviceSettings
        {
            #region Input and Output Config states
            // Every input device
            public class InputDeviceConfig
            {
                #region Static functions to handle setting key range by number of keys
                private struct MinMaxKey
                {
                    public int min;
                    public int max;

                    public MinMaxKey(int lowest, int highest)
                    {
                        this.min = lowest;
                        this.max = highest;
                    }
                }
                private static bool KeyEquals(MinMaxKey a, MinMaxKey b)
                {
                    return a.max == b.max && a.min == b.min;
                }
                private static MinMaxKey KeyboardInvalid = new MinMaxKey(-1, -1);
                private static MinMaxKey Keyboard61Key = new MinMaxKey(36, 96);
                private static MinMaxKey Keyboard88Key = new MinMaxKey(21, 108);
                private static MinMaxKey[] TypicalMinMaxKeys =
                {
                    Keyboard61Key, Keyboard88Key
                };
                private static MinMaxKey GetKeysForRange(int range)
                {
                    if (range == 61)
                    {
                        return Keyboard61Key;
                    }
                    else if (range == 88)
                    {
                        return Keyboard88Key;
                    }
                    return KeyboardInvalid;
                }
                #endregion

                private string name = "Default Input Device";
                private int minKey = -1;
                private int maxKey = -1;

                public string Name
                {
                    get { return name; }
                    set { name = value; }
                }
                public int MinKey
                {
                    get { return minKey; }
                    set { minKey = value; }
                }
                public int MaxKey
                {
                    get { return maxKey; }
                    set { maxKey = value; }
                }
                public int KeyRange
                {
                    get 
                    { 
                        return (maxKey - minKey) + 1; 
                    }
                    set
                    {
                        MinMaxKey keys = GetKeysForRange(value);
                        if (!KeyEquals(keys, KeyboardInvalid))
                        {
                            minKey = keys.min;
                            maxKey = keys.max;
                        }
                    }
                }
                public bool IsCalibrated
                {
                    get
                    {
                        return minKey != -1 && maxKey != -1;
                    }
                }

                public override string ToString()
                {
                    return string.Format("[Name:{0} MinKey:{1} MaxKey:{2}]", name, minKey, maxKey);
                }
            }
            // Every output device
            public class OutputDeviceConfig
            {
                private string name = "Default Output Device";
                private bool inUse = false;

                public string Name
                {
                    get { return name; }
                    set { name = value; }
                }
                public bool InUse
                {
                    get { return inUse; }
                    set { inUse = value; }
                }
                public override string ToString()
                {
                    return string.Format("[Name:{0} InUse:{1}]", name, inUse);
                }
            } 
            #endregion

            private float inputDelay = 0f;
            private int whichInputDevice = 0;
            private List<InputDeviceConfig> inputDeviceConfigs;
            private List<OutputDeviceConfig> outputDeviceConfigs;

            #region Public properties
            public float InputDelay
            {
                get { return inputDelay; }
                set { inputDelay = value; }
            }
            public int WhichInputDevice
            {
                get { return whichInputDevice; }
                set { whichInputDevice = value; }
            }
            public InputDeviceConfig PreferredInputDevice
            {
                get { return inputDeviceConfigs[whichInputDevice]; }
            }
            public List<InputDeviceConfig> InputDeviceConfigs
            {
                get { return inputDeviceConfigs; }
                set { inputDeviceConfigs = value; }
            }
            public List<OutputDeviceConfig> OutputDeviceConfigs
            {
                get { return outputDeviceConfigs; }
                set { outputDeviceConfigs = value; }
            } 
            #endregion

            #region Public methods
            public override string ToString()
            {
                string format = "MidiSettings:[InputDelay:{0} WhichDevice:{1} InputConfigs:{2} OutputConfigs:{3}]";
                string inputs = "", outputs = "";
                inputDeviceConfigs.ForEach((element) => { inputs += element.ToString() + " "; });
                outputDeviceConfigs.ForEach((element) => { outputs += element.ToString() + " "; });
                return string.Format(format, inputDelay, whichInputDevice, inputs, outputs);
            } 
            #endregion 

            public MidiDeviceSettings()
            {
                inputDeviceConfigs = new List<InputDeviceConfig>();
                outputDeviceConfigs = new List<OutputDeviceConfig>();
            }
        } 
        #endregion
    }   
}