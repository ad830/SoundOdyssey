using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SoundOdyssey;
using SmfLite;

namespace SoundOdyssey
{
    public class SongProgressSlider : MonoBehaviour
    {
        [SerializeField]
        SongLevelDirector director;

        Slider slider;
        [SerializeField]
        int currentNote;
        [SerializeField]
        int fullCount;

        void HandleNoteSpawned(MidiTrack.NoteDuration note, bool isPlayer)
        {
            if (note.isNoteEvent)
            {
                currentNote++;
            }
            UpdateSlider();
        }

        void HandleRestart()
        {
            currentNote = 0;
            fullCount = director.FullNoteCount;
            UpdateSlider();
        }

        void UpdateSlider()
        {
            slider.value = currentNote / (float)fullCount;
        }

        void OnEnable()
        {
            SongLevelDirector.OnLevelRestart += HandleRestart;
            SongLevelDirector.OnNoteSpawn += HandleNoteSpawned;
            SongLevelDirector.OnLevelStart += HandleRestart;
        }
        void OnDisable()
        {
            SongLevelDirector.OnLevelRestart -= HandleRestart;
            SongLevelDirector.OnNoteSpawn -= HandleNoteSpawned;
            SongLevelDirector.OnLevelStart -= HandleRestart;
        }

        // Use this for initialization
        void Start()
        {
            slider = GetComponent<Slider>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
    
}

