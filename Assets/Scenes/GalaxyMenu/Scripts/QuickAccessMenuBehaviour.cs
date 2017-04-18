using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using SoundOdyssey;
using Midi;

namespace SoundOdyssey
{
	public class QuickAccessMenuBehaviour : MonoBehaviour {

		Transform navButtons;
		Transform sectorPanelOptions;
		Transform sectorPanel;
		Transform stages;
		Transform levels;
		Button backButton;
		Button hideButton;
		Button quickButton;
		Image quickBkg;
        bool menuDisplayed = true;

		Text breadCrumbs;
		string lastBreadCrumbs;

        [SerializeField]
        SolarMenuCamera galaxyMenuCam;
        [SerializeField]
        LevelInfoUI levelInfo;
        [SerializeField]
        UIModalController uiModals;
        [SerializeField]
        WarpToSong warpSong;

		[SerializeField]
		List<Campaign.Sector> sectorData;
		List<Campaign.Level> levelCache;

		[SerializeField]
		GameObject sectorPrefab;
		[SerializeField]
		GameObject stagePrefab;
		[SerializeField]
		GameObject examPrefab;
		[SerializeField]
		GameObject levelPrefab;

        MenuSession session;
        GameDirector director;

		void ClearChildrenInTransform(Transform trans)
		{
			for (int i = 0; i < trans.childCount; i++) {
				Destroy(trans.GetChild(i).gameObject);
			}
		}

		void PopulateLevels(Transform trans, List<Campaign.Level> levels)
		{
			ClearChildrenInTransform(trans);

			foreach (Campaign.Level level in levels) {
				GameObject levelObj = Instantiate(levelPrefab) as GameObject;
				string levelName = level.info.name;
				Campaign.Level levelCopy = level;
				Campaign.Level.Song levelSong = level.song;
				Campaign.Level.Teaching levelTeaching = level.teaching;
				Campaign.Level.LevelType levelType = level.type;
				Transform levelTrans = levelObj.GetComponent<Transform>();

				levelTrans.SetParent(trans);
				levelTrans.GetChild(0).GetComponent<Text>().text = level.info.name;
                levelTrans.GetChild(0).GetComponent<Text>().fontSize = 18;

				Button levelButton = levelObj.GetComponent<Button>();
                bool isTrialSong = level.info.id == 1;

				levelButton.onClick.AddListener(() => {
					Debug.LogFormat("Clicked Level {0}", levelName);
					if (levelType == Campaign.Level.LevelType.Song)
					{
						Debug.LogFormat("LoadLevel Song {0}", levelSong.midiFilename);
                        levelInfo.SetupSong(levelCopy, isTrialSong);
					}
					else
					{
						Debug.LogFormat("LoadLevel Teaching {0}", levelTeaching.sceneFilename);
                        levelInfo.SetupTeaching(levelCopy);
					}
					//GameDirector.Instance.PlayCampaignLevel(levelCopy);
				});
			}

			/*
			backButton.onClick.RemoveAllListeners();
			backButton.onClick.AddListener(() => {
				PopulateLevels(trans, levels);
			});
			*/
		}

		void PopulateStages(Transform trans, List<Campaign.Stage> stages, Campaign.Exam exam)
		{
			ClearChildrenInTransform(trans);

			foreach (Campaign.Stage stage in stages) {
				GameObject stageObj = Instantiate(stagePrefab) as GameObject;

				Transform stageTrans = stageObj.GetComponent<Transform>();
				stageTrans.SetParent(trans);
				stageTrans.GetChild(0).GetComponent<Text>().text = stage.info.name;

				Button stageButton = stageObj.GetComponent<Button>();
				List<Campaign.Level> stageLevels = stage.levels;
				string stageName = stage.info.name;
				stageButton.onClick.AddListener(() => {
					breadCrumbs.text = string.Format("{0}:", stageName);

					// show the levels for this stage
					PopulateLevels(trans, stageLevels);

					backButton.onClick.RemoveAllListeners();
					backButton.onClick.AddListener(() => {
						PopulateStages(trans, stages, exam);
                        levelInfo.HidePanel();
					});
				});
			}

			GameObject examObj = Instantiate(examPrefab) as GameObject;

			Transform examTrans = examObj.GetComponent<Transform>();
			examTrans.SetParent(trans);
			examTrans.GetChild(0).GetComponent<Text>().text = exam.info.name;

			Button examButton = examObj.GetComponent<Button>();
			string examName = exam.info.name;
            //ScalesSection scalesData = exam.sections["Scales"] as ScalesSection;
            //Scale [] scales = scalesData.Scales;
            Campaign.Exam examCopy = exam;
			examButton.onClick.AddListener(() => {
				Debug.LogFormat("Clicked Exam {0}", examName);
                /*
                Debug.Log("SCALES:");
                for (int i = 0; i < scales.Length; i++)
                {
                    Debug.Log(scales[i].ToString());
                }
                */
                levelInfo.SetupExam(examCopy);
			});

			backButton.onClick.RemoveAllListeners();
			backButton.onClick.AddListener(() => {
				SetupMenu(trans, sectorData);
                levelInfo.HidePanel();
			});

			/*
			backButton.onClick.RemoveAllListeners();
			backButton.onClick.AddListener(() => {
				PopulateStages(trans, stages);
			});
			*/
		}

		void SetupMenu(Transform trans, List<Campaign.Sector> sectorData)
		{
			Debug.LogFormat("Spawning on object: {0}", trans.gameObject.name);
			ClearChildrenInTransform(trans);

			breadCrumbs.text = "Campaign:";
            lastBreadCrumbs = breadCrumbs.text;

            int idx = 0;
			foreach (Campaign.Sector sector in sectorData) {
				GameObject sectorObj = Instantiate(sectorPrefab) as GameObject;

				Transform sectorTrans = sectorObj.GetComponent<Transform>();
				sectorTrans.SetParent(trans);
				sectorTrans.GetChild(0).GetComponent<Text>().text = sector.info.name;

				Button sectorButton = sectorObj.GetComponent<Button>();
				List<Campaign.Stage> sectorStages = sector.stages;
				string sectorName = sector.info.name;
				Campaign.Exam sectorExam = sector.exam;

                // show the locked icon on the sector button
                GameObject lockedImg = sectorObj.transform.Find("LockedIcon").gameObject;
                lockedImg.SetActive(false);

                // check if the player has unlocked this sector
                bool sectorUnlocked = idx++ <= director.CurrentProfile.Sector;
                UnityAction sectorAction = null;
                if (sectorUnlocked)
                {
                    sectorAction = () =>
                    {
                        breadCrumbs.text = string.Format("{0}:", sectorName);

                        // show the stages and exam for this sector
                        PopulateStages(trans, sectorStages, sectorExam);

                        backButton.interactable = true;
                        backButton.onClick.RemoveAllListeners();
                        backButton.onClick.AddListener(() =>
                        {
                            SetupMenu(trans, sectorData);
                            levelInfo.HidePanel();
                        });
                    };
                }
                else
                {
                    sectorAction = () =>
                    {
                        uiModals.DisplayModal("SectorLocked");
                    };
                    lockedImg.SetActive(true);
                }
				sectorButton.onClick.AddListener(sectorAction);
                
                
                //sectorButton.interactable = sectorUnlocked;
			}

			backButton.onClick.RemoveAllListeners();
			backButton.interactable = false;
		}

		void AddShowQuickMenu()
		{
			quickButton.onClick.RemoveAllListeners();
			quickButton.onClick.AddListener(() => {

                menuDisplayed = !menuDisplayed;
                if (menuDisplayed)
                {
                    navButtons.gameObject.SetActive(true);
                    levels.gameObject.SetActive(true);
                    stages.gameObject.SetActive(true);
                    sectorPanel.gameObject.SetActive(true);
                    quickBkg.enabled = true;
                    warpSong.gameObject.SetActive(true);

                    galaxyMenuCam.DisableMouseControl();
                }
                else
                {
                    navButtons.gameObject.SetActive(false);
                    levels.gameObject.SetActive(false);
                    stages.gameObject.SetActive(false);
                    sectorPanel.gameObject.SetActive(false);
                    quickBkg.enabled = false;
                    warpSong.HidePlanetList();
                    warpSong.gameObject.SetActive(false);

                    galaxyMenuCam.EnableMouseControl();
                    levelInfo.HidePanel();
                }



                //navButtons.gameObject.SetActive(true);
                //levels.gameObject.SetActive(true);
                //stages.gameObject.SetActive(true);
                //sectorPanel.gameObject.SetActive(true);
                //quickBkg.enabled = true;

                //quickButton.onClick.RemoveAllListeners();
			});
		}

		void AddHideQuickMenu()
		{
			hideButton.onClick.RemoveAllListeners();
			hideButton.onClick.AddListener(() => {
				navButtons.gameObject.SetActive(false);
				levels.gameObject.SetActive(false);
				stages.gameObject.SetActive(false);
				sectorPanel.gameObject.SetActive(false);
				quickBkg.enabled = false;

				AddShowQuickMenu();
			});
		}

		void SetupNavigation()
		{
			//AddHideQuickMenu();
            AddShowQuickMenu();
		}

		void CacheLevels(List<Campaign.Level> cache)
		{
            cache.Clear();
			for (int i = 0; i < sectorData.Count; i++) {
				for (int k = 0; k < sectorData[i].stages.Count; k++) {
					cache.AddRange(sectorData[i].stages[k].levels);
				}
			}
			Debug.LogFormat("Cached {0} levels", cache.Count);
		}

		// Use this for initialization
		void Start () {
            session = MenuSession.Instance;
            director = GameDirector.Instance;

			Transform baseTransform = base.GetComponent<Transform>();
			sectorPanel = baseTransform.Find("Sectors");
			sectorPanelOptions = sectorPanel.Find("Options");
			stages = baseTransform.Find("Stages");
			levels = baseTransform.Find("Levels");
			breadCrumbs = baseTransform.Find("Sectors").Find("Breadcrumbs").GetComponent<Text>();
			navButtons = baseTransform.Find("NavButtons");
			backButton = navButtons.Find("UpperButtons").Find("BackButton").GetComponent<Button>();
			hideButton = navButtons.Find("LowerButtons").Find("HideMenuButton").GetComponent<Button>();
			quickButton = baseTransform.Find("Heading").GetComponent<Button>();
			quickBkg = base.GetComponent<Image>();
			SetupNavigation();

			sectorData = GalaxyLevelFormatReader.LoadSectorsFromResources("Levels/LevelStructure");
			levelCache = new List<Campaign.Level>();
			CacheLevels(levelCache);
			SetupMenu(sectorPanelOptions, sectorData);
		}

		void OnDestroy()
		{
			hideButton.onClick.RemoveAllListeners();
		}

		// Update is called once per frame
		void Update () {

		}
	}
}
