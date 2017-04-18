using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuScript : MonoBehaviour {

	public GameObject MainMenu;
	public GameObject SettingsMenu;
	public GameObject ArcadeModesMenu;
	public GameObject ExitGamePopup;
	public GameObject VideoSettingsContent,ResolutionSettingsContent, CalibrationSettingsContent;
	public Slider MasterVolumeSlider;
	public GameObject AudioSettingsContent;
	public Text MainPanelTitle;
	// Use this for initialization
	void Start () {

	}

    public void OnClickCampaign() {
		Debug.Log ("before");
		Application.LoadLevel ("GalaxyMenu");
		Debug.Log ("after");
	}

    public void OnClickArcadeModesMenu() { 
		MainMenu.SetActive (false);
		ArcadeModesMenu.SetActive (true);
    }
		
    public void OnClickSettingsMenu() {
        MainMenu.SetActive(false);
        SettingsMenu.SetActive(true);
		AudioSettingsContent.SetActive (false);
    }

	public void OnClickSettingsBack() {
		SettingsMenu.SetActive (false);
		MainMenu.SetActive (true);
	}

    public void OnClickExitGame() {
		ExitGamePopup.SetActive(true); 
    }
	
	public void OnExitGameYes() {
		Application.Quit ();
	}

	public void OnExitGameNo() {
		ExitGamePopup.SetActive (false);
	}
	
	public void OnClickSurvival() {
		//Application.LoadLevel(""); Load survival level
	}
	
	public void OnClickHyper() {
		//Application.LoadLevel(""); Load hyper level
	}
	
	public void OnClickArcadeModesBack() {
		ArcadeModesMenu.SetActive (false);
		MainMenu.SetActive (true);
	}

	public void OnClickResolution () {

	}


	public void onClickVideoSettings(){
		MainPanelTitle.text = "Video";
		VideoSettingsContent.SetActive (true);
		AudioSettingsContent.SetActive (false);
		CalibrationSettingsContent.SetActive (false);
	}

	public void onClickAudioButton(){
		CalibrationSettingsContent.SetActive (false);
		VideoSettingsContent.SetActive (false);
		AudioSettingsContent.SetActive (true);
		MainPanelTitle.text = "Audio";
	}

	public void onClickCalibrationSettings(){
		MainPanelTitle.text = "Calibration";
		VideoSettingsContent.SetActive (false);
		AudioSettingsContent.SetActive (false);
		CalibrationSettingsContent.SetActive (true);
	}


	// Update is called once per frame
	void Update () {
	
	}
}
