using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SoundOdyssey;

public class PauseMenuHandler : MonoBehaviour {

	[SerializeField]
	GameObject pauseMenuObj;
    GameObject optionsPopup;
    Toggle noteLabelsToggle;
    Toggle noteAurasToggle;

    MusicMatterManager musicMatterMgr;

	Transform trans;
	SongLevelDirector director;

	void HandlePause()
	{
		trans.gameObject.SetActive(true);
	}

	void HandleResume()
	{
		HideMenu();
	}

	void HideMenu()
	{
		trans.gameObject.SetActive(false);
	}

	void OnEnable()
	{
		SongLevelDirector.OnLevelPause += HandlePause;
		SongLevelDirector.OnLevelResume += HandleResume;
	}
	void OnDisable()
	{
		SongLevelDirector.OnLevelPause -= HandlePause;
		SongLevelDirector.OnLevelResume -= HandleResume;
	}

	void Setup()
	{
		Transform optTrans = trans.Find("Background").GetChild(0).Find("Options");
		Button resumeButton = optTrans.Find("Resume").GetComponent<Button>();
		Button restartButton = optTrans.Find("Restart Song").GetComponent<Button>();
        Button optionsButton = optTrans.Find("Options").GetComponent<Button>();
		Button galaxyButton = optTrans.Find("Exit to Galaxy").GetComponent<Button>();
		Button mainMenuButton = optTrans.Find("Exit to main menu").GetComponent<Button>();
        optionsPopup = trans.Find("OptionsPopup").gameObject;
        Transform innerPopup = optionsPopup.transform.GetChild(0);
        Button doneOptionsButton = innerPopup.Find("DoneButton").GetComponent<Button>();
        Transform popupToggles = innerPopup.Find("Toggles");
        noteLabelsToggle = popupToggles.Find("ShowNoteLabels").GetComponentInChildren<Toggle>();
        noteAurasToggle = popupToggles.Find("ShowNoteAuras").GetComponentInChildren<Toggle>();

        noteLabelsToggle.onValueChanged.RemoveAllListeners();
        noteLabelsToggle.onValueChanged.AddListener((value) =>
        {
            MenuSession.Instance.gameSettings.NoteLabels = value;
            
        });
        noteAurasToggle.onValueChanged.RemoveAllListeners();
        noteAurasToggle.onValueChanged.AddListener((value) =>
        {
            MenuSession.Instance.gameSettings.AuraVisible = value;
        });

		resumeButton.onClick.RemoveAllListeners();
		resumeButton.onClick.AddListener(() => {
			director.PauseGame();
			HideMenu();
		});
		restartButton.onClick.RemoveAllListeners();
		restartButton.onClick.AddListener(() => {
			director.Restart();
			director.PauseGame();
			HideMenu();
		});
        optionsButton.onClick.RemoveAllListeners();
        optionsButton.onClick.AddListener(() => {
            optionsPopup.SetActive(true);
            // set up toggles 
            noteLabelsToggle.isOn = MenuSession.Instance.gameSettings.NoteLabels;
            noteAurasToggle.isOn = MenuSession.Instance.gameSettings.AuraVisible;
        });
        doneOptionsButton.onClick.RemoveAllListeners();
        doneOptionsButton.onClick.AddListener(() =>
        {
            optionsPopup.SetActive(false);
            musicMatterMgr.SyncWithGameSettings();
        });
		galaxyButton.onClick.RemoveAllListeners();
		galaxyButton.onClick.AddListener(() => {
			director.PauseGame();
			director.ExitToGalaxy();
		});
		mainMenuButton.onClick.RemoveAllListeners();
		mainMenuButton.onClick.AddListener(() => {
			director.PauseGame();
			director.ExitToMainMenu();
		});

		trans.gameObject.SetActive(false);
        optionsPopup.SetActive(false);
	}

	// Use this for initialization
	void Start () {
		director = GameObject.Find("Director").GetComponent<SongLevelDirector>();
        musicMatterMgr = FindObjectOfType<MusicMatterManager>();

		if (pauseMenuObj == null)
		{
			pauseMenuObj = base.GetComponent<Transform>().Find("Pause Menu").gameObject;
		}
		trans = pauseMenuObj.GetComponent<Transform>();

		Setup();
	}
}
