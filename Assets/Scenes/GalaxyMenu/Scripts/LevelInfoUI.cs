using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SoundOdyssey;
using Midi;

namespace SoundOdyssey
{
    public class LevelInfoUI : MonoBehaviour
    {
        [SerializeField]
        Animator animator;
        private MenuSession session;

        private Text headingText;
        private Text levelTypeText;
        private Text descriptionText;

        // Navigation button
        private Button playButton;
        private Button practiceButton;
        private Button backButton;

        // Done tick
        private GameObject doneTick;

        // Exam
        private GameObject scalesSection;
        private Transform scalesSectionButtons;
        [SerializeField]
        private GameObject scalesSectionButtonPrefab;

        // Song
        private GameObject statistics;
        private Transform recordsTransform;
        [SerializeField]
        private GameObject noRecord;
        [SerializeField]
        private GameObject statRecordPrefab;
        [SerializeField]
        private GameObject eraseStatsButtonPrefab;

        private void ClearAllButtonListeners()
        {
            ClearButtonListener(playButton);
            ClearButtonListener(practiceButton);
            ClearButtonListener(backButton);
        }

        private void ClearButtonListener(Button button)
        {
            button.onClick.RemoveAllListeners();
        }

        private void ResetAllChildren(Transform node)
        {
            for (int i = 0; i < node.childCount; i++)
            {
                GameObject child = node.GetChild(i).gameObject;
                Destroy(child);
            }
        }

        private void SetupReferences()
        {
            // set up buttons
            Transform buttonsTransform = base.transform.Find("Buttons");
            playButton = buttonsTransform.Find("PlayButton").GetComponent<Button>();
            practiceButton = buttonsTransform.Find("PracticeButton").GetComponent<Button>();
            backButton = buttonsTransform.Find("BackButton").GetComponent<Button>();

            // set up title texts
            Transform titleTransform = base.transform.Find("Title");
            headingText = titleTransform.Find("LevelName").GetComponent<Text>();
            levelTypeText = titleTransform.Find("LevelType").GetComponent<Text>();

            // set up body 
            Transform bodyTransform = base.transform.Find("Body").Find("Inner");
            descriptionText = bodyTransform.Find("Description").GetComponentInChildren<Text>();
            doneTick = bodyTransform.Find("Description").Find("DoneIcon").gameObject;
            doneTick.SetActive(false);

            // set up exam sections
            scalesSection = bodyTransform.Find("ExamScalesSection").gameObject;
            scalesSection.SetActive(false);

            // set up song statistics
            statistics = bodyTransform.Find("Statistics").gameObject;
            recordsTransform = statistics.transform.Find("Records");
            statistics.SetActive(false);

            ClearAllButtonListeners();
        }

        private void TestInit()
        {
            headingText.text = "Base Level";
            levelTypeText.text = "Teaching Level";
            descriptionText.text = "NORMAL DESCRIPTION";

            playButton.onClick.AddListener(() =>
            {
                Debug.Log("PLAY");
            });
            practiceButton.onClick.AddListener(() =>
            {
                Debug.Log("PRACTICE");
            });
            backButton.onClick.AddListener(() =>
            {
                Debug.Log("BACK");
            });
        }

        private void ResetButtons()
        {
            playButton.gameObject.SetActive(true);
            practiceButton.gameObject.SetActive(true);
            backButton.gameObject.SetActive(false);
        }

        private void Init()
        {
            backButton.gameObject.SetActive(false);
        }

        public void ShowPanel()
        {
            animator.SetBool("isOpen", true);
        }
        public void HidePanel()
        {
            animator.SetBool("isOpen", false);
        }

        private void TickIfPlayed(string levelName)
        {
            SoundOdyssey.Profile currentProfile = GameDirector.Instance.CurrentProfile;
            SoundOdyssey.LevelProgress progress = currentProfile.LevelProgress.GetLevelProgress(levelName, true);
            doneTick.SetActive(progress.played);
        }

        public void SetupGalaxySong(string heading, string midiFilename)
        {
            ResetLevelPanel();
            headingText.text = heading;
            levelTypeText.text = "Galaxy Song Level";
            descriptionText.text = "Play or practice " + heading;

            // show if played
            TickIfPlayed(midiFilename);

            // setup score history
            SetupScoreHistory(midiFilename);
            statistics.SetActive(true);

            playButton.onClick.AddListener(() =>
            {
                // play the song
                SongLevelParams songParams = new SongLevelParams(false, false, false, true);
                GameDirector.Instance.PlayGalaxySongLevel(midiFilename, songParams);
            });

            practiceButton.onClick.AddListener(() =>
            {
                // practice the song
                SongLevelParams songParams = new SongLevelParams(true, false, false, true);
                GameDirector.Instance.PlayGalaxySongLevel(midiFilename, songParams);
            });
            ShowPanel();
        }

        public void SetupSong(Campaign.Level level, bool isTrialSong)
        {
            ResetLevelPanel();
            headingText.text = level.info.name;
            if (isTrialSong)
            {
                levelTypeText.text = "Trial Song Level";
                descriptionText.text = "Try playing the notes in a song!";
            }
            else
            {
                levelTypeText.text = "Song Level";
                descriptionText.text = "Play or practice a song!";
            }

            // show if played
            TickIfPlayed(level.info.name);

            // setup score history here
            SetupScoreHistory(level.info.name);
            statistics.SetActive(true);

            playButton.onClick.AddListener(() =>
            {
                // play the song
                SongLevelParams songParams = new SongLevelParams(false, isTrialSong);
                GameDirector.Instance.PlayCampaignLevel(level, songParams);
            });
            practiceButton.gameObject.SetActive(!isTrialSong);
            if (!isTrialSong)
            {
                practiceButton.onClick.AddListener(() =>
                {
                    // practice the song
                    SongLevelParams songParams = new SongLevelParams(true);
                    GameDirector.Instance.PlayCampaignLevel(level, songParams);
                });
            }
            

            ShowPanel();
        }

        private void SetupScoreHistory(string levelName)
        {
            ResetAllChildren(recordsTransform);
            Profile currentProfile = GameDirector.Instance.CurrentProfile;
            List<Score> scoreHistory = MenuSession.Instance.ScoresForProfile(currentProfile, levelName);
            
            if (scoreHistory != null && scoreHistory.Count > 0)
            {
                // sort by chronological order
                scoreHistory.Sort((a, b) =>
                {
                    return a.DateTime.CompareTo(b.DateTime);
                });
                // reverse it so it is latest to earliest
                scoreHistory.Reverse();
                foreach (Score score in scoreHistory)
                {
                    GameObject record = Instantiate(statRecordPrefab) as GameObject;
                    record.transform.SetParent(recordsTransform);
                    record.name = score.DateTime.ToShortDateString();
                    
                    // set correct fields on record
                    Text dateTime = record.transform.Find("DateTime").GetComponent<Text>();
                    Text overallScore = record.transform.Find("OverallScore").GetComponent<Text>();
                    Text accuracy = record.transform.Find("Accuracy").GetComponent<Text>();
                    Text fluency = record.transform.Find("Fluency").GetComponent<Text>();
                    Text expression = record.transform.Find("Expression").GetComponent<Text>();

                    System.DateTime localTime = score.DateTime.ToLocalTime();
                    string practiceStr = (score.PracticeMode ? " [Practice]" : "");
                    dateTime.text = string.Format("{0} - {1}{2}", localTime.ToShortDateString(), localTime.ToShortTimeString(), practiceStr);
                    overallScore.text = string.Format("Score: {0} pts", score.TotalScore);
                    accuracy.text = string.Format("Accuracy: {0:0.##}%", score.Accuracy * 100.0f);
                    fluency.text = string.Format("Fluency: {0:0.##}%", score.Fluency * 100.0f);
                    expression.text = string.Format("Expression: {0:0.##}%", score.Expression * 100.0f);
                }
                GameObject eraseButton = Instantiate(eraseStatsButtonPrefab) as GameObject;
                eraseButton.transform.SetParent(recordsTransform);
                eraseButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    session.RemoveScoresForProfile(currentProfile, levelName);
                    SetupScoreHistory(levelName);
                });
            }
            else
            {
                GameObject obj = Instantiate(noRecord) as GameObject;
                obj.transform.SetParent(recordsTransform);
                obj.name = "NoRecords";
            }
        }

        public void SetupTeaching(Campaign.Level level)
        {
            ResetLevelPanel();
            practiceButton.gameObject.SetActive(false);     // no need to practice here
            headingText.text = level.info.name;
            levelTypeText.text = "Teaching Level";
            descriptionText.text = "Play with a new musical concept!";

            // show if played
            TickIfPlayed(level.info.name);

            playButton.onClick.AddListener(() =>
            {
                // play the song
                GameDirector.Instance.PlayCampaignLevel(level);
            });

            ShowPanel();
        }

        public void SetupExam(Campaign.Exam exam)
        {
            ResetLevelPanel();
            headingText.text = exam.info.name;
            levelTypeText.text = "Exam Level";
            descriptionText.text = "Play or practice an exam!";
            GameDirector.Instance.ExamLevel = exam;

            // show if played
            TickIfPlayed(exam.info.name);

            // spawn in scales section
            Transform scalesButtons = scalesSection.transform.Find("ScalesButtons");
            ResetAllChildren(scalesButtons);

            ScalesSection scalesData = exam.sections["Scales"] as ScalesSection;
            if (scalesData == null)
            {
                Debug.LogWarning("No Scales Section!");
                Debug.Break();
                return;
            }
            int scaleCount = scalesData.Scales.Length;
            for (int i = 0; i < scaleCount; i++)
            {
                GameObject obj = Instantiate(scalesSectionButtonPrefab) as GameObject;
                Text label = obj.GetComponentInChildren<Text>();
                Button objButton = obj.GetComponent<Button>();
                int whichScale = i;

                string scaleName = scalesData.Scales[i].Name;
                label.text = scaleName;
                obj.transform.SetParent(scalesButtons);
                objButton.onClick.AddListener(() =>
                {
                    GameDirector.Instance.PlayExamScales(scalesData, whichScale);
                });
            }

            playButton.onClick.AddListener(() =>
            {
                // play through the exam in ... order here
                // FAKING EXAM PROGRESS FOR NOW
                //session.CurrentProfile.UnlockNextSector();
                //session.SetPlayedLevel(exam.info.name);
                //SetupExam(exam);


                //scalesData.Scales.Length;
                GameDirector.Instance.PlayExamScales(scalesData, Random.Range(0, scalesData.Scales.Length), false);
            });
            practiceButton.onClick.AddListener(() =>
            {
                // enable the section buttons here
                scalesSection.SetActive(true);
                playButton.gameObject.SetActive(false);
                practiceButton.gameObject.SetActive(false);
                backButton.gameObject.SetActive(true);

                ClearButtonListener(backButton);
                backButton.onClick.AddListener(() =>
                {
                    playButton.gameObject.SetActive(true);
                    practiceButton.gameObject.SetActive(true);
                    backButton.gameObject.SetActive(false);

                    scalesSection.SetActive(false);
                });
            });
            backButton.onClick.AddListener(() =>
            {
                // do back button stuff here
            });
            
            // set up scales section

            ShowPanel();
        }

        private void ResetPanelBody()
        {
            // set done tick to false
            doneTick.SetActive(false);

            // exam scales section
            scalesSection.SetActive(false);

            // song statistics
            statistics.SetActive(false);
        }

        private void ResetLevelPanel()
        {
            ClearAllButtonListeners();
            ResetButtons();
            ResetPanelBody();
        }

        // Use this for initialization
        void Start()
        {
            session = MenuSession.Instance;
            SetupReferences();
            //TestInit();
            Init();
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }    
}

