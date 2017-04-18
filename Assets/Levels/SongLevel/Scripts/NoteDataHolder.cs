using UnityEngine;
using System.Collections;
using SmfLite;

namespace SoundOdyssey
{
    public class NoteDataHolder : MonoBehaviour
    {
        public MidiTrack.NoteDuration data;
        public bool started = false;
        public bool inZone = false;
        public bool acceptKeyPress = false; // check that the key has been pressed down in the play zone
        public bool isPlayer = true;
        public bool isReleased = false;
        public float startPosition = -5;

        public void ResetState()
        {
            startPosition = -5;
            started = false;
            inZone = false;
            acceptKeyPress = false;
            isPlayer = true;
            isReleased = false;
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }    
}

