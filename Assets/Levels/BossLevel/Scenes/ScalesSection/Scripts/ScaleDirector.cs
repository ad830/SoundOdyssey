using Midi;
using SoundOdyssey;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SoundOdyssey
{
    public class ScaleDirector : MonoBehaviour
    {
        // References
        [SerializeField]
        PlayScales playScalesObject;
        [SerializeField]
        Text scaleHeadingObject;
        [SerializeField]
        Text infoText;
        [SerializeField]
        GameObject navigation;
        [SerializeField]
        GameObject propsPanel;
        [SerializeField]
        GameObject pracButPanel;
        [SerializeField]
        GameObject topPanel;

        private GameObject confirmQuitPanel;

        [SerializeField]
        string tonicNote = "C";
        [SerializeField]
        string scalePattern = "Major";
        [SerializeField]
        int octaveCount = 1;
        [SerializeField]
        int startOctave = 4;
        [SerializeField]
        string motion = "Similar";
        [SerializeField]
        bool isPracticeMode = true;

        public bool IsPracticeMode
        {
            get { return isPracticeMode; }
        }

        Scale scale;

        GameDirector director;

        Button nextButton;

        public void ResetLevel()
        {
            playScalesObject.SetupScale(scale, octaveCount, startOctave);
            scaleHeadingObject.text = string.Format("Scale : {0}", scale.ToString());
            
            // set up the properties panel
            for (int i = 0; i < propsPanel.transform.childCount; i++)
            {
                Debug.LogFormat("Child {0} {1}", i, propsPanel.transform.GetChild(i));
            }

            Text octaveText = propsPanel.transform.Find("OctaveCount").GetComponent<Text>();
            Text motionText = propsPanel.transform.Find("Motion").GetComponent<Text>();
            Text handsText = propsPanel.transform.Find("Hands").GetComponent<Text>();
            Text directionText = propsPanel.transform.Find("Direction").GetComponent<Text>();

            octaveText.text = string.Format("Octaves: {0}", octaveCount);
            motionText.text = string.Format("Motion: {0}", motion);
            handsText.text = string.Format("Hands: Separate");
            directionText.text = string.Format("Direction: Ascending/Descending");

            if (!isPracticeMode)
            {
                pracButPanel.SetActive(false);
            }
        }

        public void QuitLevel()
        {
            Transform buttons = confirmQuitPanel.transform.Find("Buttons");
            Button confirmButton = buttons.Find("Confirm").GetComponent<Button>();

            if (isPracticeMode)
            {
                ExitLevel();
            }
            else
            {
                confirmQuitPanel.SetActive(true);
                confirmButton.onClick.RemoveAllListeners();
                confirmButton.onClick.AddListener(ExitLevel);
            }
        }

        public void ExitLevel()
        {
            GameDirector instance = GameDirector.Instance;
            instance.CurrentScaleProgress = 0;
            instance.ScaleScore = 0;
            instance.MaxScaleScore = 0;
            instance.LoadLevel("GalaxyMenu");
        }

        public void LoadNextScale()
        {
            // load the next scale in the exam
            Debug.LogFormat("load next scale");
       
            ScalesSection scalesData = GameDirector.Instance.ExamScalesSection;

            int whichScale = 0;
            int currentScale = GameDirector.Instance.CurrentScaleIndex;

            do
            {
                whichScale = UnityEngine.Random.Range(0, scalesData.Scales.Length);
            } while (whichScale == currentScale);

            GameDirector.Instance.PlayExamScales(scalesData, whichScale, false);
        }

        public void ShowNextButton()
        {
            nextButton.gameObject.SetActive(true);
        }

        public void ShowGetResultsButton()
        {
            Button quitButton = topPanel.transform.Find("QuitButton").GetComponent<Button>();

            //may aswell see their results, why quit now?
            quitButton.enabled = false;
            quitButton.gameObject.SetActive(false);

            Transform trans = nextButton.transform;
            GameObject obj = trans.GetChild(0).gameObject;
            obj.GetComponent<Text>().text = "Get Results";

            
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(ShowResults);

            nextButton.gameObject.SetActive(true);
        }

        private void ShowResults()
        {

            GameDirector instance = GameDirector.Instance;

            float percentage = instance.ScaleScore / (float)instance.MaxScaleScore;
         
            Debug.LogFormat("Correct Notes: {0}", GameDirector.Instance.ScaleScore);
            Debug.LogFormat("Percentage: {0}", percentage);

            if(percentage > 0.85)
            {
                MenuSession.Instance.CurrentProfile.UnlockNextSector();
                MenuSession.Instance.SetPlayedLevel(GameDirector.Instance.ExamLevel.info.name);
            }

            instance.LoadLevel("ExamResults");

        }


        // Use this for initialization
        void Start()
        {
            director = GameDirector.Instance;
            if (director.ExamScalesSection != null)
            {
                scale = director.CurrentScale;
                octaveCount = director.ExamScalesSection.Octaves;
                motion = director.ExamScalesSection.Motion;
                isPracticeMode = director.IsScalesPractice;
            }
            else
            {
                // load the scale from game director
                ScalePattern pattern = null;
                for (int i = 0; i < Scale.Patterns.Length; i++)
                {
                    if (scalePattern == Scale.Patterns[i].Name)
                    {
                        pattern = Scale.Patterns[i];
                    }
                }
                if (pattern == null)
                {
                    Debug.Log("Incorrect pattern name");
                    Debug.Break();
                }
                scale = new Scale(new Note(tonicNote), pattern);
            }

            // setting up top panel
            nextButton = topPanel.transform.Find("NextButton").GetComponent<Button>();
            nextButton.gameObject.SetActive(false);
            if (!isPracticeMode)
            {
                nextButton.onClick.AddListener(LoadNextScale);
            }

            confirmQuitPanel = topPanel.transform.Find("QuitButton").Find("ReallyQuitPanel").gameObject;

            ResetLevel();
        }
    }    
}