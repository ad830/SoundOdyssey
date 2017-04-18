using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace SoundOdyssey
{
    public class MidiAsset
    {
        private byte[] _bytes;
        private string _name;
        private uint _duration;                           // in seconds

        public byte[] bytes
        {
            get { return _bytes; }
        }
        public string name
        {
            get { return _name; }
        }
        public uint duration
        {
            get { return _duration; }
            set { _duration = value; }
        }
        
        public MidiAsset(byte[] bytes, string name)
        {
            this._bytes = bytes;
            this._name = name;
            this._duration = 0;
        }
    }

    public class MidiResourceHolder
    {
        #region Public filepaths
        public const string customMidiDirectory = "Sound Odyssey Custom Songs/";
        #endregion

        #region Public member variables
        public static bool displayFileNames = false;
        #endregion

        #region Private member variables
        List<MidiAsset> midiFiles;                  // TODO: Change the list type to a struct that also stores metadata about the midi file
                                                    // Like duration in seconds, lowest and highest note, ...
        #endregion

        #region Public property
        public int MidiFileCount
        {
            get
            {
                return midiFiles.Count;
            }
        }
        #endregion

        #region Public methods
        MidiResourceHolder()
        {
            List<TextAsset> midiAssets = new List<TextAsset>(Resources.LoadAll<TextAsset>("Midi"));
            
            // only add MIDI files with the correct header chunk
            midiAssets = midiAssets.FindAll((asset) => { return VerifyMidiFile(asset.bytes); });

            midiFiles = new List<MidiAsset>(midiAssets.Count);
            midiFiles.AddRange(midiAssets.ConvertAll<MidiAsset>((textAsset) => {return new MidiAsset(textAsset.bytes, textAsset.name);}));
            Debug.LogFormat("Loaded {0} midi files from Resources/Midi", midiFiles.Count);

            if (displayFileNames)
            {
                for (int i = 0; i < midiFiles.Count; i++)
                {
                    Debug.LogFormat("Midi File {0} - {1}", i, midiFiles[i].name);
                }
            }

            Debug.Log("Loading midi files from custom midi folder");
            string docPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            string customMidiPath = Path.Combine(docPath, customMidiDirectory);
            //Debug.LogFormat("Folder exists: {0}", Directory.Exists(customMidiPath));
            if (!Directory.Exists(customMidiPath))
            {
                // create folder if it doesn't exist
                Directory.CreateDirectory(customMidiPath);
            }
            // load all midi files from there
            List<MidiAsset> customMidis = new List<MidiAsset>();
            string[] customMidiFiles = Directory.GetFiles(customMidiPath, "*.mid");
            foreach (var customPath in customMidiFiles)
            {
                string fileName = Path.GetFileName(customPath);
                byte[] customMidiBytes = System.IO.File.ReadAllBytes(customPath);
                MidiAsset testAsset = new MidiAsset(customMidiBytes, fileName);

                System.Predicate<MidiAsset> matchMidiName = (midi) => { return midi.name == fileName; };
                bool isCurrentDuplicate = customMidis.Exists(matchMidiName);
                bool isExistingDuplicate = midiFiles.Exists(matchMidiName);
                bool validMidiFile = VerifyMidiFile(testAsset.bytes);

                if (validMidiFile && !isCurrentDuplicate && !isExistingDuplicate)
                {
                    customMidis.Add(testAsset);
                }
                if (isCurrentDuplicate || isExistingDuplicate)
                {
                    Debug.LogFormat("Duplicate {0} already loaded", fileName);
                }
                if (!validMidiFile)
                {
                    Debug.LogFormat("{0} is not a valid midi file", fileName);
                }
            }
            midiFiles.AddRange(customMidis);
        }

        public MidiAsset GetMidiByName(string name)
        {
            return midiFiles.Find(item => item.name == name);
        }

        public MidiAsset GetMidiAt(int id)
        {
            if (id < 0 || id > midiFiles.Count - 1)
            {
                return null;
            }
            return midiFiles[id];
        }

        public MidiAsset GetRandomMidi()
        {
            if (midiFiles.Count == 0)
            {
                return null;
            }
            return midiFiles[Random.Range(0, midiFiles.Count)];
        }
        #endregion

        #region Private methods

        private bool VerifyMidiFile(byte[] data)
        {
            // read the first 4 chars and make sure it matches a MIDI header chunk
            char[] headerChunk = new char[4];
            for (int i = 0; i < headerChunk.Length; i++)
            {
                headerChunk[i] = (char)data[i];
            }
            string headerString = new string(headerChunk);
            return headerString == "MThd";
        } 
        #endregion

        #region Singleton class handling
        static MidiResourceHolder instance;
        public static MidiResourceHolder Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MidiResourceHolder();
                }
                return instance;
            }
        }
        #endregion

    }
}
