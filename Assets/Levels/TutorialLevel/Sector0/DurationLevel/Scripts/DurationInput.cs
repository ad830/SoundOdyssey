using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MidiJack;

public class DurationInput : MonoBehaviour {

    [SerializeField]
    private GameObject notes;

    [SerializeField]
    private Midi.Pitch whichNote = Midi.Pitch.C4;

    [SerializeField]
    private GameObject objectives;
    [SerializeField]
    private Text testText;

    [System.Serializable]
    public struct Range
    {
        public float start;
        public float end;
    }
    [SerializeField]
    private Range[] ranges;

    private float[] durations = {0.125f, 0.25f, 0.5f};
    private Color[] colours = { Color.cyan, Color.yellow, Color.green };
    private float acceptableRange = 0.03125f;//0.015625f;
    private float leewayPercentage = 0.05f;
    private float startRange;
    private float endRange;
    private Transform backgrounds;

    [SerializeField]
    private Text infoText;

    private List<Image> noteImages;
    private Slider slider;
    private float time;
    private bool noteHeld;

    private List<Toggle> toggles;
    private int testIdx = 0;
    private int testCount = 2;

    private float GetStartRange(float duration)
    {
        return duration - acceptableRange;//* startRange;
    }
    private float GetEndRange(float duration)
    {
        return duration + acceptableRange;//* endRange;
    }
    private void UpdateTestNumDisplay()
    {
        testText.text = string.Format("Test {0} of {1}", testIdx + 1, testCount);
    }

	// Use this for initialization
	void Start () {
        noteImages = new List<Image>(notes.transform.childCount);
        for (int i = 0; i < notes.transform.childCount; i++)
        {
            noteImages.Add(notes.transform.GetChild(i).GetComponent<Image>());
            Debug.LogFormat("{0}", notes.transform.GetChild(i).gameObject.name);
        }

        toggles = new List<Toggle>(objectives.transform.childCount - 1);
        for (int i = 0; i < objectives.transform.childCount - 1; i++)
        {
            toggles.Add(objectives.transform.GetChild(i).GetComponent<Toggle>());
        }

        startRange = 1 - leewayPercentage;
        endRange = 1 + leewayPercentage;

        UpdateInputRanges();

        // set up the visual ranges
        backgrounds = base.transform.Find("Background");
        UpdateVisualRanges();

        // display which test is current
        UpdateTestNumDisplay();


        time = 0.0f;
        noteHeld = false;

        slider = GetComponent<Slider>();
        infoText.text = "";
	}

    private void UpdateInputRanges()
    {
        ranges = new Range[durations.Length];
        for (int i = 0; i < ranges.Length; i++)
        {
            ranges[i].start = GetStartRange(durations[i]);
            ranges[i].end = GetEndRange(durations[i]);
        }
    }

    private void UpdateVisualRanges()
    {
        float sliderWidth = base.GetComponent<RectTransform>().sizeDelta.x;
        int bkgCount = backgrounds.childCount;
        for (int i = 0; i < bkgCount; i++)
        {
            RectTransform rect = backgrounds.GetChild(i).GetComponent<RectTransform>();
            Debug.LogFormat("name {0}", rect.gameObject.name);
            float startX = (ranges[i].start / 0.5f) * sliderWidth;
            float endX = (ranges[i].end / 0.5f) * sliderWidth;
            float width = endX - startX;
            rect.anchoredPosition = new Vector2(startX, 0);
            rect.sizeDelta = new Vector2(width, 0);

            Debug.LogFormat("pos {0} 0 0", startX);
            Debug.LogFormat("sizeDelta {0} 0", width);
        }
    }
	
	// Update is called once per frame
	void Update () 
    {
	    if (MidiMaster.GetKeyDown((int)whichNote))
        {
            noteHeld = true;
            foreach (var image in noteImages) 
            {
                image.color = Color.clear;
            }
            slider.fillRect.GetComponent<Image>().color = Color.white;
            infoText.text = "";
        }
        if (MidiMaster.GetKeyUp((int)whichNote))
        {
            noteHeld = false;

            for (int i = 0; i < durations.Length; i++)
            {
                float start = GetStartRange(durations[i]);
                float end = GetEndRange(durations[i]);
                //float difference = Mathf.Abs(time - durations[i]);

                if (time >= start && time <= end)
                {
                    noteImages[i].color = colours[i];
                    slider.fillRect.GetComponent<Image>().color = colours[i];
                    toggles[i].isOn = true;
                }

                if (toggles.TrueForAll((toggle) => {return toggle.isOn;}))
                {
                    
                    if (testIdx < testCount)
                    {
                        Debug.LogFormat("testIdx {0}", testIdx);
                        testIdx++;
                        // go to next test index
                        if (testIdx < testCount)
                        {
                            // detoggle all objectives
                            toggles.ForEach((toggle) => { toggle.isOn = false; });

                            // shrink ranges
                            acceptableRange *= 0.5f;
                            UpdateInputRanges();

                            // update display
                            UpdateVisualRanges();

                            // update test display
                            UpdateTestNumDisplay();
                        }
                    }
                }
            }
            //if (Mathf.Abs(time - 0.5f) < 0.0625f)
            //{
            //    noteImages[0].color = Color.green;
            //    slider.fillRect.GetComponent<Image>().color = Color.green;
            //}
            //else if (Mathf.Abs(time - 0.25f) < 0.0625f)
            //{
            //    noteImages[1].color = Color.yellow;
            //    slider.fillRect.GetComponent<Image>().color = Color.yellow;
            //}
            //else if (Mathf.Abs(time - 0.125f) < 0.0625f)
            //{
            //    noteImages[2].color = Color.cyan;
            //    slider.fillRect.GetComponent<Image>().color = Color.cyan;
            //}

            Debug.LogFormat("Time held: {0}", time);
            time = 0.0f;
        }

        if(noteHeld)
        {
            time += Time.deltaTime;
            slider.value = time * 2;
            if (time > GetEndRange(durations[2]))
            {
                slider.fillRect.GetComponent<Image>().color = Color.red;
                if (infoText.text == "")
                {
                    infoText.text = "Held it for too long!";
                }
            }
        }
        
	}
}
