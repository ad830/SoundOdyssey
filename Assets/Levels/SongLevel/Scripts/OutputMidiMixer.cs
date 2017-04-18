using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Midi;

namespace SoundOdyssey
{
    public class OutputMidiMixer : MonoBehaviour
    {
        public class QueuedNoteOff
        {
            public Channel channel;
            public Pitch pitch;
            public int velocity;
            public float pulseDuration;

            public QueuedNoteOff(Channel channel, Pitch pitch, int velocity, float pulseDuration)
            {
                this.channel = channel;
                this.pitch = pitch;
                this.velocity = velocity;
                this.pulseDuration = pulseDuration;
            }
        }

        void OpenMidiDevices()
        {
            Debug.LogFormat("There are {0} MIDI Outputs", OutputDevice.InstalledDevices.Count);
            outputDevices = new List<OutputDevice>(OutputDevice.InstalledDevices);
            for (int i = 0; i < OutputDevice.InstalledDevices.Count; i++)
            {
                if (sendToAllDevices || !sendToAllDevices && i == preferredDevice)
                {
                    outputDevices[i].Open();
                    Debug.LogFormat("Opening Device {0}", i);
                }
            }
        }

        void CloseMidiDevices()
        {
            if (outputDevices == null) { return; }
            for (int i = 0; i < outputDevices.Count; i++)
            {
                if (outputDevices[i].IsOpen)
                {
                    outputDevices[i].SilenceAllNotes();
                    outputDevices[i].Close();
                    Debug.LogFormat("Closing Device {0}", i);
                }
            }
        }

        void ResetMidiDevices()
        {
            CloseMidiDevices();
            OpenMidiDevices();
        }

        void SilenceMidiDevices()
        {
            if (outputDevices == null) { return; }
            for (int i = 0; i < outputDevices.Count; i++)
            {
                if (outputDevices[i].IsOpen)
                {
                    outputDevices[i].SilenceAllNotes();
                }
            }
        }

        void HandleLevelRestart()
        {
            SilenceMidiDevices();
        }

        void OnEnable()
        {
            ResetMidiDevices();
            SongLevelDirector.OnLevelRestart += HandleLevelRestart;
        }

        void OnDisable()
        {
            CloseMidiDevices();
            SongLevelDirector.OnLevelRestart -= HandleLevelRestart;
        }

        void OnApplicationQuit()
        {
            CloseMidiDevices();
        }

        void OnDestroy()
        {
            CloseMidiDevices();
        }

        void OnApplicationPause()
        {
            ResetMidiDevices();
        }

        void Reset()
        {
            ResetMidiDevices();
        }

        // Use this for initialization
        void Start()
        {
            pulseToNext = 0;
            
            keyMap = new Dictionary<Channel, bool[]>(16);
            for (Channel i = 0; i <= Channel.Channel16; i++)
            {
                keyMap[i] = new bool[128];
                for (int a = 0; a < 128; a++)
                {
                    keyMap[i][a] = false;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

            // decrement all scheduled messages
            /*
            for (int i = 0; i < queuedMessages.Count; i++)
            {
                queuedMessages[i].pulseDuration -= Time.deltaTime;
                if (queuedMessages[i].pulseDuration <= 0)
                {
                    SendNoteOff(queuedMessages[i].channel, queuedMessages[i].pitch, queuedMessages[i].velocity);
                    queuedMessages.RemoveAt(i);
                    //Debug.LogFormat("Sending message at index {0}", i);
                }
            }
            */

            // pop all that are below 0 and send them

            /*
            pulseCounter += pulsePerSecond * Time.deltaTime;

            if (pulseCounter < pulseToNext)
            {
                return;
            }

            while (pulseCounter >= pulseToNext)
            {
                var pair = queuedMessages.Dequeue();

                pulseCounter -= pulseToNext;
                pulseToNext = enumerator.Current.delta;
            }
            */
        }

        #region Private methods
        IEnumerator ScheduleNoteOff(Channel channel, Pitch pitch, int velocity, float duration)
        {
            yield return new WaitForSeconds(duration);
            SendNoteOff(channel, pitch, velocity);
        }
        #endregion

        #region Public methods
        public void ChangeTempo(float bpm, int ppqn)
        {
            pulsePerSecond = bpm / 60.0f * ppqn;
        }

        public void SendNoteOn(Channel channel, Pitch pitch, int velocity)
        {
            if (keyMap[channel][(int)pitch])
            {
                SendNoteOff(channel, pitch, velocity);
            }
            keyMap[channel][(int)pitch] = true;
            if (!sendToAllDevices)
            {
                outputDevices[preferredDevice].SendNoteOn(channel, pitch, velocity);
            }
            else
            {
                for (int i = 0; i < outputDevices.Count; i++)
                {
                    outputDevices[i].SendNoteOn(channel, pitch, velocity);
                }
            }
        }
        public void SendNoteOff(Channel channel, Pitch pitch, int velocity)
        {
            if (!keyMap[channel][(int)pitch]) { return; }
            keyMap[channel][(int)pitch] = false;
            if (!sendToAllDevices)
            {
                try
                {
                    outputDevices[preferredDevice].SendNoteOff(channel, pitch, velocity);
                }
                catch (DeviceException e)
                {
                    Debug.Log("Couldn't send note off " + e.Message);
                }
            }
            else
            {
                try
                {
                    for (int i = 0; i < outputDevices.Count; i++)
                    {
                        outputDevices[i].SendNoteOff(channel, pitch, velocity);
                    }
                }
                catch (DeviceException e)
                {
                    Debug.Log("Couldn't send note off " + e.Message);
                }
            }
        }
        public void SendControlChange(Channel channel, Control control, int value)
        {
            if (!sendToAllDevices)
            {
                outputDevices[preferredDevice].SendControlChange(channel, control, value);
            }
            else
            {
                for (int i = 0; i < outputDevices.Count; i++)
                {
                    outputDevices[i].SendControlChange(channel, control, value);
                }
            }
        }
        public void SendPitchBend(Channel channel, int value)
        {
            if (!sendToAllDevices)
            {
                outputDevices[preferredDevice].SendPitchBend(channel, value);
            }
            else
            {
                for (int i = 0; i < outputDevices.Count; i++)
                {
                    outputDevices[i].SendPitchBend(channel, value);
                }
            }
        }
        public void SendProgramChange(Channel channel, Instrument instrument)
        {
            if (!sendToAllDevices)
            {
                outputDevices[preferredDevice].SendProgramChange(channel, instrument);
            }
            else
            {
                for (int i = 0; i < outputDevices.Count; i++)
                {
                    outputDevices[i].SendProgramChange(channel, instrument);
                }
            }
        }
        public void SendPercussion(Percussion percussion, int velocity)
        {
            if (!sendToAllDevices)
            {
                outputDevices[preferredDevice].SendPercussion(percussion, velocity);
            }
            else
            {
                for (int i = 0; i < outputDevices.Count; i++)
                {
                    outputDevices[i].SendPercussion(percussion, velocity);
                }
            }
        }
        /*
        public void SendNoteDuration(Channel channel, Pitch pitch, int velocity, float duration)
        {
            SendNoteOn(channel, pitch, velocity);
            queuedMessages.Add(new QueuedNoteOff(channel, pitch, velocity, duration));
            //Debug.LogFormat("Note off in {0}", duration);
        }
        */
        #endregion

        #region Private members
        List<OutputDevice> outputDevices;

        Dictionary<Channel, bool[]> keyMap;

        float pulsePerSecond;
        float pulseToNext;
        float pulseCounter;
        #endregion

        #region Public members
        public int preferredDevice = 0;
        public bool sendToAllDevices = false;
        #endregion
    }
}
