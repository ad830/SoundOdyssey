using UnityEngine;
using System.Collections;
using Midi;
using SoundOdyssey;

namespace SoundOdyssey
{
	/*
	Boss Level:
		- List of BossLevelComponents
			- Scales
			- Aural
			OR
			- Sight Reading
			- Pieces / Songs
	*/

	/*
	Scales Section
	Given some parameters such as:
	- which scales to be assessed
	- what manner/properties to play those scales

	It will hold all the model methods to power a Boss level scene.
	*/

	[System.Serializable]
	public class ScalesSection : Campaign.IExamSection
	{
		public struct Data
		{
			public Scale[] scales;      // which scales to play
			public int octaveCount;		// how many octaves to play
			//public float minimumTempo;
			public string direction;	// ascending or descending
			public string motion;		// similar, contrary
		}

		Data data;

		public ScalesSection(Data data)
		{
			this.data = data;
		}

        #region IExamSection interface 
        public string Name
        {
            get { return "Scales"; }
        }
        #endregion

        public Scale[] Scales
		{
			get { return data.scales; }
		}

		public int Octaves
		{
			get { return data.octaveCount; }
		}

		public string Direction
		{
			get { return data.direction; }
		}

		public string Motion
		{
			get { return data.motion; }
		}
	}

	[System.Serializable]
	public class BossLevel {
		public BossLevel()
		{
			this.scales = null;
		}

		public void SetScales(ScalesSection.Data data)
		{
			this.scales = new ScalesSection(data);
		}

		public ScalesSection ScalesSection
		{
			get { return scales; }
		}

		ScalesSection scales;
	}
}
