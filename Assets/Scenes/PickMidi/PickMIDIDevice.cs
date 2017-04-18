using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.IO;
using Midi;
using IniParser;
using SoundOdyssey;


namespace SoundOdyssey
{
    public class PickMIDIDevice : MonoBehaviour
    {
        public enum MidiMode { Input, Output };

        [SerializeField]
        private GameObject buttonPrefab;
        [SerializeField]
        private MidiMode midiMode = MidiMode.Output;
        [SerializeField]
        private GameObject settingsPanel;

        [SerializeField]
        private int primaryDeviceIdx = 0;
        [SerializeField]
        private bool[] useDeviceStates;

        private Toggle primaryToggle;
        private Toggle useDeviceToggle;
        private Toggle outputSendPercussionToggle;
        private Toggle outputSendAccompToggle;
        private GameObject outputToggles;
        private GameObject inputToggles;

        private ReadOnlyCollection<InputDevice> lastInputDevs;
        private ReadOnlyCollection<OutputDevice> lastOutputDevs;
        private float refreshTimer = 0;
        private float refreshPeriod = 30.0f;

        #region Public properties
        public int DeviceCount
        {
            get
            {
                if (midiMode == MidiMode.Output)
                {
                    if (lastOutputDevs == null)
                    {
                        Start();
                    }
                    return lastOutputDevs.Count;
                }
                else
                {
                    if (lastInputDevs == null)
                    {
                        Start();
                    }
                    return lastInputDevs.Count;
                }
            }
        }

        public string GetDeviceName(int idx)
        {
            if (midiMode == MidiMode.Output)
            {
                return lastOutputDevs[idx].Name;
            }
            else
            {
                return lastInputDevs[idx].Name;
            }
        }

        public bool GetDeviceUsed(int idx)
        {
            if (idx < 0 || idx >= useDeviceStates.Length)
            {
                throw new ArgumentOutOfRangeException("idx", idx, string.Format("{0} : Invalid index {1} Length was: {2}", midiMode, idx, useDeviceStates.Length));
            }
            return useDeviceStates[idx];
        } 
        #endregion

        void CreateButton(int idx, Transform panel, string label, UnityEngine.Events.UnityAction method)
        {
            GameObject button = Instantiate(buttonPrefab);
            button.name = "Device - " + label;
            button.transform.SetParent(panel);
            Transform buttonTransform = button.GetComponent<Transform>();

            if (label != null)
            {
                GameObject labelObj = buttonTransform.GetChild(0).gameObject;
                labelObj.GetComponent<Text>().text = label;
            }

            if (method != null)
            {
                button.GetComponent<Button>().onClick.AddListener(method);
            }
            else
            {
                button.GetComponent<Button>().onClick.AddListener(() =>
                {
                    Debug.LogFormat("You clicked {0} for {1}", button.name, midiMode);
                    settingsPanel.SetActive(true);
                    Text deviceNameText = settingsPanel.GetComponent<Transform>().Find("DeviceNameText").gameObject.GetComponent<Text>();
                    deviceNameText.text = string.Format("{0} [{1}]", label, midiMode);

                    outputToggles.SetActive(midiMode == MidiMode.Output);
                    inputToggles.SetActive(midiMode == MidiMode.Input);

                    primaryToggle.onValueChanged.RemoveAllListeners();
                    useDeviceToggle.onValueChanged.RemoveAllListeners();

                    primaryToggle.isOn = idx == primaryDeviceIdx;
                    useDeviceToggle.isOn = useDeviceStates[idx];

                    primaryToggle.onValueChanged.AddListener((on) =>
                    {
                        if (on)
                        {
                            primaryDeviceIdx = idx;
                        }
                        else if (!on && primaryDeviceIdx == idx)
                        {
                            // unchecked primary device so they might be no primary device
                            Debug.Log("might be no primary device");
                        }
                    });
                    useDeviceToggle.onValueChanged.AddListener((on) =>
                    {
                        useDeviceStates[idx] = on;
                    });
                });
            }
        }

        public void Save()
        {

        }

        void SetupSettingsPanel()
        {
            settingsPanel.SetActive(true);
            Transform toggleTrans = settingsPanel.GetComponent<Transform>().Find("Toggles");
            primaryToggle = toggleTrans.Find("PrimaryToggle").gameObject.GetComponent<Toggle>();
            useDeviceToggle = toggleTrans.Find("UseDeviceToggle").gameObject.GetComponent<Toggle>();
            outputToggles = toggleTrans.Find("OutputToggles").gameObject;
            inputToggles = toggleTrans.Find("InputOptions").gameObject;
            outputSendPercussionToggle = outputToggles.GetComponent<Transform>().Find("SendPercussion").gameObject.GetComponent<Toggle>();
            outputSendAccompToggle = outputToggles.GetComponent<Transform>().Find("SendAccompaniment").gameObject.GetComponent<Toggle>();
            settingsPanel.SetActive(false);
        }

        void SetupStates(int count)
        {
            Debug.LogFormat("{0}: There are {1} devices", midiMode, count);
            Debug.LogFormat("{0}: Win32DevCount {1}", midiMode, API.midiOutGetNumDevs());
            useDeviceStates = new bool[count];
            /*
            if (count > 0)
            {
                useDeviceStates[0] = true;
            }
            */
        }

        public void RefreshDetectedMidiDevices()
        {
            ClearMidiDevices();

            if (midiMode == MidiMode.Output)
            {
                var devices = OutputDevice.InstalledDevices;
                SetupStates(devices.Count);
                int idx = 0;
                var outputConfigs = MenuSession.Instance.midiSettings.OutputDeviceConfigs;
                if (outputConfigs.Count == 0)
                {
                    useDeviceStates[0] = true;
                }
                foreach (OutputDevice device in devices)
                {
                    for (int i = 0; i < outputConfigs.Count; i++)
                    {
                        if (device.Name == outputConfigs[i].Name)
                        {
                            useDeviceStates[idx] = outputConfigs[i].InUse;
                            break;
                        }
                    }
                    CreateButton(idx++, base.transform.GetChild(0), device.Name, null);
                }
                lastOutputDevs = new ReadOnlyCollection<OutputDevice>(devices);
            }
            else
            {
                var devices = InputDevice.InstalledDevices;
                SetupStates(devices.Count);
                int idx = 0;
                for (int a = 0; a < devices.Count; a++)
                {
                    useDeviceStates[a] = true;      // use all input devices
                }
                foreach (InputDevice device in devices)
                {
                    CreateButton(idx++, base.transform.GetChild(0), device.Name, null);
                }
                lastInputDevs = new ReadOnlyCollection<InputDevice>(devices);
            }
        }

        void ClearMidiDevices()
        {
            primaryToggle.onValueChanged.RemoveAllListeners();
            useDeviceToggle.onValueChanged.RemoveAllListeners();

            Transform trans = base.GetComponent<Transform>().GetChild(0);
            int deviceCount = trans.childCount;
            for (int i = 0; i < deviceCount; i++)
            {
                GameObject child = trans.GetChild(i).gameObject;
                Destroy(child);
            }
        }

        // Use this for initialization
        void Start()
        {
            SetupSettingsPanel();
            RefreshDetectedMidiDevices();
        }

        void Update()
        {
            /*
            refreshTimer += Time.deltaTime;
            if (refreshTimer > refreshPeriod)
            {
                // refresh devices
                refreshTimer = 0;
                Debug.Log("Refreshing devices...");
                //RefreshDetectedMidiDevices();
            }
            */
        }

        private static class API
        {
            [DllImport("winmm.dll", SetLastError = true)]
            public static extern UInt32 midiOutGetNumDevs();
        }
    }
}

