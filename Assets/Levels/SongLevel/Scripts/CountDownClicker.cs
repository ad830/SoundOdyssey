using UnityEngine;
using System.Collections;
using SoundOdyssey;
using Midi;

namespace SoundOdyssey
{
    public class CountDownClicker : MonoBehaviour
    {
        OutputMidiMixer mixer;
        Percussion currentPercussion;

        public Percussion tickPercussion = Percussion.OpenTriangle;
        public Percussion endTickPercussion = Percussion.SplashCymbal;
        public float percussionDuration = 0.125f;

        void HandleTick()
        {
            currentPercussion = tickPercussion;
            DoPercussion(percussionDuration);
        }

        void DoPercussion(float duration)
        {
            mixer.SendNoteOn(Channel.Channel10, (Pitch)currentPercussion, 127);
            Invoke("PercussionOff", duration);
        }

        void PercussionOff()
        {
            mixer.SendNoteOff(Channel.Channel10, (Pitch)currentPercussion, 127);
        }

        void HandleLastTick()
        {
            currentPercussion = endTickPercussion;
            DoPercussion(percussionDuration);
        }

        void OnEnable()
        {
            CountdownTimer.OnTick += HandleTick;
            CountdownTimer.OnLastTick += HandleLastTick;
        }
        void OnDisable()
        {
            CountdownTimer.OnTick -= HandleTick;
            CountdownTimer.OnLastTick -= HandleLastTick;
        }

        // Use this for initialization
        void Start()
        {
            mixer = base.transform.parent.GetComponent<OutputMidiMixer>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }    
}
