using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SoundOdyssey;


namespace SoundOdyssey
{
    public class PlayDuration : MonoBehaviour
    {


        [SerializeField]
        private Duration.Value value;

        [SerializeField]
        private OutputMidiMixer mixer;


        [SerializeField]
        private Midi.Pitch pitch;

        private List<Image> noteImages;

        private bool isPlaying;

        // Use this for initialization
        void Start()
        {
            isPlaying = false;

            GameObject notes = GameObject.Find("Notes");
            int noteCount = notes.transform.childCount;
            noteImages = new List<Image>(noteCount);
            for (int i = 0; i < noteCount; i++)
            {
                noteImages.Add(notes.transform.GetChild(i).GetComponent<Image>());
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
        private void Stop()
        {
            mixer.SendNoteOff(Midi.Channel.Channel1, pitch, 127);
            isPlaying = false;
        }

        public void Play()
        {
            if (!isPlaying)
            {
                isPlaying = true;
                mixer.SendNoteOn(Midi.Channel.Channel1, pitch, 127);
                // bpm is 120
                // 0.5s == 1 beat
                float duration = 0;
                switch (value)
                {
                    case Duration.Value.Whole:
                        duration = 0.5f;
                        break;
                    case Duration.Value.Half:
                        duration = 0.25f;
                        break;
                    case Duration.Value.Quarter:
                        duration = 0.125f;
                        break;
                }
                Invoke("Stop", duration);
            }
        }
    }
}

