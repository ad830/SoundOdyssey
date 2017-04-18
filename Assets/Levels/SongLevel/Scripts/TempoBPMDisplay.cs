using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SoundOdyssey;

namespace SoundOdyssey
{
    public class TempoBPMDisplay : MonoBehaviour
    {
        [SerializeField]
        Text label;

        void UpdateBPMLabel(float bpm)
        {
            label.text = string.Format("Tempo: {0} BPM", bpm);
        }

        void OnEnable()
        {
            SongLevelDirector.OnTempoChange += UpdateBPMLabel;
        }
        void OnDisable()
        {
            SongLevelDirector.OnTempoChange -= UpdateBPMLabel;
        }
    }
}


