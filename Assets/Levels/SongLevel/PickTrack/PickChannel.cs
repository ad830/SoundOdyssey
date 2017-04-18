using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Midi;
using SmfLite;
using SoundOdyssey;

namespace SoundOdyssey
{
    public class PickChannel : MonoBehaviour
    {
        // References
        [SerializeField]
        SongLevelDirector director;
        [SerializeField]
        GameObject channelDisplay;
        [SerializeField]
        GameObject navigationPanel;
        [SerializeField]
        GameObject channelButtonPrefab;

        Button selectAllButton;
        Button clearAllButton;
        Button playButton;
        List<Toggle> togglesRef;
        GameObject warnPickOnePopup;

        // state
        [SerializeField]
        bool[] channelMap;
        int totalNotesToPlay;

        // TEST
        [SerializeField]
        TextAsset testMidi;

        MidiFileContainer song;

        private void RemoveAllChildren(Transform t)
        {
            for (int i = 0; i < t.childCount; i++)
            {
                Destroy(t.GetChild(i).gameObject);
            }
        }

        public void ShowChannels()
        {
            RemoveAllChildren(channelDisplay.transform);

            // Get list of channels and their data
            // Channel Index
            // Instrument Name(s)
            // Used in the song (true if there are midi messages for this channel)
            int channelsUsed = 0;
            for (int i = 0; i < song.channelData.Length; i++)
            {
                Channel channel = (Channel)i;
                if (song.channelData[i].hasNotes)
                {
                    channelsUsed++;
                    GameObject prefabPick = Instantiate(channelButtonPrefab) as GameObject;
                    prefabPick.transform.SetParent(channelDisplay.transform);
                    prefabPick.name = string.Format("Toggle {0}", channel.Name());

                    Toggle toggle = prefabPick.GetComponent<Toggle>();
                    int localIndex = i;
                    int noteCount = song.channelData[i].noteCount;
                    toggle.onValueChanged.AddListener((value) =>
                    {
                        channelMap[localIndex] = value;
                        int addNoteCount = (value ? 1 : -1) * noteCount;
                        //Debug.LogFormat("Set Channel {0} to {1} -> Adding {2} notes", localIndex + 1, value, addNoteCount);
                        totalNotesToPlay += addNoteCount;
                    });
                    togglesRef.Add(toggle);

                    // set correct label for this channel toggle
                    Text label = prefabPick.transform.Find("Label").GetComponent<Text>();
                    string instrumentsStr = "";

                    if (channel == Channel.Channel10)
                    {
                        instrumentsStr = "Percussion";
                    }
                    else
                    {
                        if (song.channelData[i].programChanges.Count > 0)
                        {
                            string lastInstrument = "";
                            int programChangeCount = song.channelData[i].programChanges.Count;
                            for (int a = 0; a < programChangeCount; a++)
                            {
                                string nextInstrument = song.channelData[i].programChanges[a].Name();
                                if (lastInstrument != nextInstrument)
                                {
                                    instrumentsStr += nextInstrument;
                                    if (a < programChangeCount - 1 && nextInstrument != song.channelData[i].programChanges[a + 1].Name())
                                    {
                                        instrumentsStr += ", ";
                                    }
                                    lastInstrument = nextInstrument;
                                }
                            }
                        }
                        else
                        {
                            instrumentsStr = Instrument.AcousticGrandPiano.Name();
                        }
                    }

                    instrumentsStr += string.Format(" [{0}] Notes: {1}", channel.Name(), noteCount);

                    label.text = instrumentsStr;
                }
                totalNotesToPlay += song.channelData[i].noteCount;
            }

            if (channelsUsed == 1)
            {
                // just skip this screen and play already :D
                playButton.onClick.Invoke();
            }
        }

        private void SetupNavButtons()
        {
            selectAllButton = navigationPanel.transform.Find("MiddleWrapper").Find("AllButton").GetComponent<Button>();
            clearAllButton = navigationPanel.transform.Find("MiddleWrapper").Find("ClearSelectButton").GetComponent<Button>();
            playButton = navigationPanel.transform.Find("PlayButton").GetComponent<Button>();
            warnPickOnePopup = navigationPanel.transform.Find("PickOneChannelPopup").gameObject;
            warnPickOnePopup.SetActive(false);
        }

        private void SetStateForAllChannels(bool state)
        {
            // change data (model)
            for (int c = 0; c < channelMap.Length; c++)
            {
                channelMap[c] = state;
            }
            // change interactables (view/controller)
            for (int i = 0; i < togglesRef.Count; i++)
            {
                togglesRef[i].isOn = state;
            }
        }

        // Returns if there is at least one channel selected
        private bool CheckValidPick()
        {
            bool anyPicked = false;
            for (int i = 0; i < togglesRef.Count; i++)
            {
                if (togglesRef[i].isOn)
                {
                    anyPicked = true;
                    break;
                }
            }
            return anyPicked;
        }

        private void StartLevel()
        {
            Debug.LogFormat("Total Notes to play: {0}", totalNotesToPlay);
            if (director != null)
            {
                director.noteCount = totalNotesToPlay;
                director.ChannelMap = channelMap;
                base.transform.parent.gameObject.SetActive(false);
                director.Play();
            }
        }

        private void ShowWarning()
        {
            warnPickOnePopup.SetActive(true);
            CancelInvoke("HideWarning");
            Invoke("HideWarning", 3.0f);
        }

        private void HideWarning()
        {
            warnPickOnePopup.SetActive(false);
        }

        private void Init()
        {
            if (director != null)
            {
                song = director.CurrentSong;
                ShowChannels();
            }
        }

        void OnEnable()
        {
            SongLevelDirector.OnLoadedSongs += Init;
        }
        void OnDisable()
        {
            SongLevelDirector.OnLoadedSongs -= Init;
        }

        // Use this for initialization
        void Start()
        {
            totalNotesToPlay = 0;

            channelMap = new bool[16];
            togglesRef = new List<Toggle>(16);
            for (int i = 0; i < channelMap.Length; i++)
            {
                channelMap[i] = true;
            }
            SetupNavButtons();

            selectAllButton.onClick.AddListener(() =>
            {
                SetStateForAllChannels(true);
            });
            clearAllButton.onClick.AddListener(() =>
            {
                SetStateForAllChannels(false);
            });
            playButton.onClick.AddListener(() =>
            {
                if (CheckValidPick())
                {
                    StartLevel();
                }
                else
                {
                    ShowWarning();
                }
            });

            if (testMidi != null && director == null)
            {
                song = MidiFileLoader.Load(testMidi.bytes);
                ShowChannels();
            }

            
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
    
}

