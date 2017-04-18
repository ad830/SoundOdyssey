using UnityEngine;
using System.Collections;

namespace SoundOdyssey
{
    public struct CommonLevelData
    {
        public string title;
        public int id;

        public CommonLevelData (string title, int id)
        {
            this.title = title;
            this.id = id;
        }

        public override string ToString()
        {
            const string fmt = "Title:{0} Id:{1}";
            return string.Format(fmt, title, id);
        }
    }

    public struct TutorialLevelData
    {
        public CommonLevelData common;

        public TutorialLevelData (string title, int id)
        {
            this.common = new CommonLevelData(title, id);
        }

        public override string ToString()
        {
            const string fmt = "Common:{0}";
            return string.Format(fmt, common.ToString());
        }
    }

    public struct SongLevelData
    {
        public CommonLevelData common;
        public MidiAsset [] midiFiles;              // Supports more than one midi file per song (potentially)
        public int progress;                        // TODO: Create a struct or class to represent progress in the song by Song:Measure
        public SongLevelParams parameters;
        // public int idealTrack;

        public SongLevelData(string title, int id, MidiAsset[] midiFiles, int progress, SongLevelParams levelParameters = new SongLevelParams())
        {
            this.common = new CommonLevelData(title, id);
            this.midiFiles = midiFiles;
            this.progress = progress;
            this.parameters = levelParameters;
        }

        public override string ToString()
        {
            const string fmt = "Common:{0} MidiFilesCount:{1} MidiFilesName:{2} Progress:{3} Params: {4}";
            Debug.Log(common);
            Debug.Log(midiFiles[0]);
            Debug.Log(progress);
            return string.Format(fmt, common.ToString(), midiFiles.Length, midiFiles[0].name, progress, parameters);
        }
    }
}
