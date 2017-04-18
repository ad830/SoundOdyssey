using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace SoundOdyssey
{
	public static class Campaign
	{
		[System.Serializable]
		public class Identifier
		{
			public int id;
			public string name;

			public Identifier(int id, string name)
			{
				this.id = id;
				this.name = name;
			}
		}

		[System.Serializable]
		public class Level
		{
			[System.Serializable]
			public class Teaching
			{
				public string sceneFilename;

				public Teaching (string filename)
				{
					this.sceneFilename = filename;
				}
			}

			[System.Serializable]
			public class Song
			{
				public string midiFilename;
				public int idealTrack;

				public Song (string filename, int idealTrack)
				{
					this.midiFilename = filename;
					this.idealTrack = idealTrack;
				}
			}
			public enum LevelType {Teaching, Song};

			public Identifier info;
			public Teaching teaching;
			public Song song;
			public LevelType type;

			public Level(Identifier info, LevelType type)
			{
				this.info = info;
				this.type = type;
				this.teaching = null;
				this.song = null;
			}
		}

		[System.Serializable]
		public class Stage
		{
			public Identifier info;
			public List<Level> levels;

			public Stage (Identifier info, int levelCount)
			{
				this.info = info;
				this.levels = new List<Level>(levelCount);
			}
		}

        public interface IExamSection
        {
            string Name { get; }
        }

		[System.Serializable]
		public class Exam
		{
			public Identifier info;
			//public ScalesSection scales;
            //public List<IExamSection> sections;
            public Dictionary<string, IExamSection> sections;

			public Exam (Identifier info, List<IExamSection> sectionList)
			{
				this.info = info;
                this.sections = new Dictionary<string, IExamSection>(sectionList.Count);
                foreach (var section in sectionList)
                {
                    sections.Add(section.Name, section);
                }
			}
		}

		[System.Serializable]
		public class Sector
		{
			public Identifier info;
			public List<Stage> stages;
			public Exam exam;

			public Sector(Identifier info, int stageCount)
			{
				this.info = info;
				this.stages = new List<Stage>(stageCount);
				this.exam = null;
			}
		}
	}
}
