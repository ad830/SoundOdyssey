using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using MidiJack;
using SoundOdyssey;

public class PitchPartOne : MonoBehaviour {

	// General Game Objects
	public GameObject PitchStartScreen;
	public GameObject PitchEndScreen;
	public GameObject PitchScreenOne;
	public GameObject PitchScreenTwo;
	public GameObject WholeNote;
	public GameObject BlackKeysScreenTwo;
	public GameObject BlackKeysScreenOne;
	public GameObject PrevButton;
	public GameObject ShowTipsButton;

    // Objectives
    public GameObject MainObjective;

	// Arrays
	GameObject[] Keys;
	GameObject[] KeysScreenTwo;
	GameObject[] Notes;
	GameObject[] Notes2;

	// Tool Tips:
	// screen one
	public GameObject StaffTip;
	public GameObject TrebleTip;
	public GameObject VirtualPianoTip;
	public GameObject BlackKeysTip;
	public GameObject WhiteKeysTip;
	public GameObject FlatsSharpsTip;
	public GameObject NotesTip;
	// screen two
	public GameObject OctaveTip;
	public GameObject BassClefTip;
	public GameObject GrandStaffTip;
	public GameObject LeftRightTip;

	// Check what screen or part of the level you're currently on
	int screen;
	bool ShowingAllTips;

	public void OnClickShowTipsButton(){

		if (ShowingAllTips == false) {
			ShowingAllTips = true;
			if (screen == 1) {
				ShowScreenOneTips(true);
			} else if (screen == 2) {
				ShowScreenTwoTips(true);
			}
		} else {
			ShowingAllTips = false;
			if (screen == 1) {
				ShowScreenOneTips(false);
			} else if (screen == 2) {
				ShowScreenTwoTips(false);
			}
		}
	}

	// Functions used for showing tool tips
	public void ShowStaffTip(){
			StaffTip.SetActive (true);
	}
	public void UnShowStaffTip(){
		if (ShowingAllTips == false) {
			StaffTip.SetActive (false);
		}
	}

	public void ShowTrebleTip(){
		TrebleTip.SetActive (true);
	}
	public void UnShowTrebleTip(){
		if (ShowingAllTips == false) {
			TrebleTip.SetActive (false);
		}
	}
	
	public void ShowNotesTip(){
		NotesTip.SetActive (true);
	}
	public void UnShowNotesTip(){
		if (ShowingAllTips == false) {
			NotesTip.SetActive (false);
		}
	}

	public void ShowBassClefTip(){
		BassClefTip.SetActive (true);
	}
	public void UnShowBassClefTip(){
		if (ShowingAllTips == false) {
			BassClefTip.SetActive (false);
		}
	}
	
	public void ShowGrandStaffTip(){
		GrandStaffTip.SetActive (true);
		LeftRightTip.SetActive (true);
	}
	public void UnShowGrandStaffTip(){
		if (ShowingAllTips == false) {
			GrandStaffTip.SetActive (false);
			LeftRightTip.SetActive (false);
		}
	}
	
	public void ShowVirtualPianoTips(){
		if (screen == 1) {
			VirtualPianoTip.SetActive (true);
			BlackKeysTip.SetActive (true);
			WhiteKeysTip.SetActive (true);
			FlatsSharpsTip.SetActive (true);
		} else if (screen == 2) {
			OctaveTip.SetActive (true);
		}

	}
	public void UnShowVirtualPianoTips(){
		if (ShowingAllTips == false) {
			if (screen == 1) {
				VirtualPianoTip.SetActive (false);
				BlackKeysTip.SetActive (false);
				WhiteKeysTip.SetActive (false);
				FlatsSharpsTip.SetActive (false);
			} else if (screen == 2) {
				OctaveTip.SetActive (false);
			}
		}
	}
	
	public void ShowScreenOneTips(bool val){
		StaffTip.SetActive (val);
		TrebleTip.SetActive (val);
		VirtualPianoTip.SetActive (val);
		BlackKeysTip.SetActive (val);
		WhiteKeysTip.SetActive (val);
		FlatsSharpsTip.SetActive (val);
		NotesTip.SetActive (val);		
	}

	public void ShowScreenTwoTips(bool val){
		OctaveTip.SetActive (val);
		BassClefTip.SetActive (val);
		GrandStaffTip.SetActive (val);
		LeftRightTip.SetActive (val);
	}
	
	public void OnClickNext(){

		screen++;
		if (screen == 1)
		{
            MainObjective.SetActive(true);
			PitchScreenOne.SetActive(true);
			ShowTipsButton.SetActive(true);
			foreach (GameObject akey in Keys)
			{
				akey.SetActive(true);
			}
			BlackKeysScreenOne.SetActive (true);
			ShowScreenTwoTips(false);
			ShowingAllTips = false;
			PitchStartScreen.SetActive(false);
		}

        if (screen == 2)
        {
            PitchScreenOne.SetActive(false);
            PitchScreenTwo.SetActive(true);

            ShowScreenOneTips(false);
            ShowingAllTips = false;

            // hide black keys, and show new set of black keys with 2 octaves
            BlackKeysScreenOne.SetActive(false);
            BlackKeysScreenTwo.SetActive(true);

            // show second set of octaves of white keys
            foreach (GameObject akey in KeysScreenTwo)
            {
                akey.SetActive(true);
            }

            // show note letters on stave on screen two
            foreach (GameObject anote in Notes2)
            {
                anote.SetActive(false);
            }

            PrevButton.SetActive(true);
        }
		else if (screen == 3)
		{
            MainObjective.SetActive(false);
			PrevButton.SetActive(false);
			PitchScreenTwo.SetActive(false);
			foreach (GameObject akey in KeysScreenTwo) {
				akey.SetActive (false);
			}
			ShowTipsButton.SetActive(false);
			ShowScreenTwoTips (false);
			ShowingAllTips = false;
			BlackKeysScreenOne.SetActive(false);
			PitchEndScreen.SetActive(true);
        }
		if (screen == 4) {
            // set that player has played this level
            string levelName = GameDirector.Instance.TutorialLevel.common.title;
            MenuSession.Instance.SetPlayedLevel(levelName);
			GameDirector.Instance.LoadLevel("GalaxyMenu");
		}
	}

	public void OnClickPrev(){

        if (screen > 1)
        {
            screen--;
        }

		if (screen == 1)
		{
			PitchScreenTwo.SetActive (false);
			PitchScreenOne.SetActive (true);

			ShowScreenTwoTips (false);
			ShowingAllTips = false;
			BlackKeysScreenTwo.SetActive (false);
			BlackKeysScreenOne.SetActive (true);

			foreach (GameObject akey in KeysScreenTwo) {
				akey.SetActive (false);
			}
			foreach (GameObject akey in Keys) {
				akey.SetActive(true);
			}
		}

		PrevButton.SetActive (false);
	}

	// Use this for initialization
	void Start () {

		ShowingAllTips = false;
		screen = 0;

		PrevButton = GameObject.Find ("PrevButton").transform.GetChild(0).gameObject;
		PrevButton.SetActive (false);

		StaffTip = GameObject.Find ("StaffTip");
		TrebleTip = GameObject.Find ("TrebleTip");
		VirtualPianoTip = GameObject.Find ("VirtualPianoTip");
		BlackKeysTip = GameObject.Find ("BlackKeysTip");
		WhiteKeysTip = GameObject.Find ("WhiteKeysTip");
		FlatsSharpsTip = GameObject.Find ("FlatsSharpsTip");
		NotesTip = GameObject.Find ("NotesTip");

		OctaveTip = GameObject.Find ("OctaveTip");
		BassClefTip = GameObject.Find ("BassClefTip");
		GrandStaffTip = GameObject.Find ("GrandStaffTip");
		LeftRightTip = GameObject.Find ("LeftRightTip");

        MainObjective.SetActive(false);

		ShowScreenOneTips (false);
		ShowScreenTwoTips (false);

		Keys = new GameObject[13];
		Keys [0] = GameObject.Find ("WhiteKeyLeftC");
		Keys [1] = GameObject.Find ("BlackKeyC#");
		Keys [2] = GameObject.Find ("WhiteKeyD");
		Keys [3] = GameObject.Find ("BlackKeyD#");
		Keys [4] = GameObject.Find ("WhiteKeyE");
		Keys [5] = GameObject.Find ("WhiteKeyF");
		Keys [6] = GameObject.Find ("BlackKeyF#");
		Keys [7] = GameObject.Find ("WhiteKeyG");
		Keys [8] = GameObject.Find ("BlackKeyG#");
		Keys [9] = GameObject.Find ("WhiteKeyA");
		Keys [10] = GameObject.Find ("BlackKeyA#");
		Keys [11] = GameObject.Find ("WhiteKeyB");
		Keys [12] = GameObject.Find ("WhiteKeyRightC");

		Notes = new GameObject[13];
		Notes [0] = GameObject.Find ("WholeBotC");
		Notes [1] = GameObject.Find ("C#");
		Notes [2] = GameObject.Find ("WholeBotD");
		Notes [3] = GameObject.Find ("D#");
		Notes [4] = GameObject.Find ("WholeBotE");
		Notes [5] = GameObject.Find ("WholeBotF");
		Notes [6] = GameObject.Find ("F#");
		Notes [7] = GameObject.Find ("WholeG");
		Notes [8] = GameObject.Find ("G#");
		Notes [9] = GameObject.Find ("WholeA");
		Notes [10] = GameObject.Find ("A#");
		Notes [11] = GameObject.Find ("WholeB");
		Notes [12] = GameObject.Find ("WholeTopC");

		foreach (GameObject anote in Notes) {
			anote.SetActive(false);
		}

		KeysScreenTwo = new GameObject[25];
		KeysScreenTwo [0] = GameObject.Find ("WhiteKeyExtremeLeftC");
		KeysScreenTwo [1] = GameObject.Find ("BlackKeyC#2");
		KeysScreenTwo [2] = GameObject.Find ("WhiteKeyLeftD");
		KeysScreenTwo [3] = GameObject.Find ("BlackKeyD#2");
		KeysScreenTwo [4] = GameObject.Find ("WhiteKeyLeftE");
		KeysScreenTwo [5] = GameObject.Find ("WhiteKeyLeftF");
		KeysScreenTwo [6] = GameObject.Find ("BlackKeyF#2");
		KeysScreenTwo [7] = GameObject.Find ("WhiteKeyLeftG");
		KeysScreenTwo [8] = GameObject.Find ("BlackKeyG#2");
		KeysScreenTwo [9] = GameObject.Find ("WhiteKeyLeftA");
		KeysScreenTwo [10] = GameObject.Find ("BlackKeyA#2");
		KeysScreenTwo [11] = GameObject.Find ("WhiteKeyLeftB");
		KeysScreenTwo [12] = GameObject.Find ("WhiteKeyLeftC");
		KeysScreenTwo [13] = GameObject.Find ("BlackKeyC#3");
		KeysScreenTwo [14] = GameObject.Find ("WhiteKeyD");
		KeysScreenTwo [15] = GameObject.Find ("BlackKeyD#3");
		KeysScreenTwo [16] = GameObject.Find ("WhiteKeyE");
		KeysScreenTwo [17] = GameObject.Find ("WhiteKeyF");
		KeysScreenTwo [18] = GameObject.Find ("BlackKeyF#3");
		KeysScreenTwo [19] = GameObject.Find ("WhiteKeyG");
		KeysScreenTwo [20] = GameObject.Find ("BlackKeyG#3");
		KeysScreenTwo [21] = GameObject.Find ("WhiteKeyA");
		KeysScreenTwo [22] = GameObject.Find ("BlackKeyA#3");
		KeysScreenTwo [23] = GameObject.Find ("WhiteKeyB");
		KeysScreenTwo [24] = GameObject.Find ("WhiteKeyRightC");

		for (int i=0; i <12; i++) {
			KeysScreenTwo[i].SetActive(false);
		}
		
		foreach (GameObject anote in Keys) {
			anote.SetActive(false);
		}

		Notes2 = new GameObject[25];
		Notes2 [0] = GameObject.Find ("WholeBotC3");
		Notes2 [1] = GameObject.Find ("C#3");
		Notes2 [2] = GameObject.Find ("WholeBotD3");
		Notes2 [3] = GameObject.Find ("D#3");
		Notes2 [4] = GameObject.Find ("WholeBotE3");
		Notes2 [5] = GameObject.Find ("WholeF3");
		Notes2 [6] = GameObject.Find ("F#3");
		Notes2 [7] = GameObject.Find ("WholeG3");
		Notes2 [8] = GameObject.Find ("G#3");
		Notes2 [9] = GameObject.Find ("WholeA3");
		Notes2 [10] = GameObject.Find ("A#3");
		Notes2 [11] = GameObject.Find ("WholeTopB3");
		Notes2 [12] = GameObject.Find ("WholeMiddleC2");
		Notes2 [13] = GameObject.Find ("C#2");
		Notes2 [14] = GameObject.Find ("WholeBotD2");
		Notes2 [15] = GameObject.Find ("D#2");
		Notes2 [16] = GameObject.Find ("WholeBotE2");
		Notes2 [17] = GameObject.Find ("WholeBotF2");
		Notes2 [18] = GameObject.Find ("F#2");
		Notes2 [19] = GameObject.Find ("WholeG2");
		Notes2 [20] = GameObject.Find ("G#2");
		Notes2 [21] = GameObject.Find ("WholeA2");
		Notes2 [22] = GameObject.Find ("A#2");
		Notes2 [23] = GameObject.Find ("WholeB2");
		Notes2 [24] = GameObject.Find ("WholeTopC2");

		foreach (GameObject anote in Notes2) {
			anote.SetActive (false);
		}

		BlackKeysScreenTwo = GameObject.Find ("BlackKeysScreenTwo");
		BlackKeysScreenTwo.SetActive (false);
		BlackKeysScreenOne = GameObject.Find ("BlackKeysScreenOne");
		BlackKeysScreenOne.SetActive(false);

		ShowTipsButton = GameObject.Find ("ShowTipsButton");
		ShowTipsButton.SetActive(false);

		PitchScreenTwo = GameObject.Find ("PitchScreenTwo");
		PitchScreenTwo.SetActive (false);
		PitchScreenOne = GameObject.Find ("PitchScreenOne");
		PitchScreenOne.SetActive (false);
		PitchStartScreen = GameObject.Find ("PitchStartScreen");
		PitchEndScreen = GameObject.Find ("PitchEndScreen");
		PitchEndScreen.SetActive (false);
	}

	// Update is called once per frame
	void Update () {
		if (screen == 1) {
			for (int i = 0; i < Keys.Length; ++i) {
				if (MidiMaster.GetKey (60 + i) > 0f) {
					Debug.Log ("Key: " + (60 + i));
					Color nextColour = Keys [i].GetComponent<Image> ().color;
					nextColour.a = 1f;
					Keys [i].GetComponent<Image> ().color = nextColour;

					// Show notes on stave.
					Notes [i].SetActive (true);
					if (i == 1 || i == 3 || i == 6 || i == 8 || i == 10) {
						Notes [i - 1].SetActive (true);
					}
				} else {
					Color nextColour = Keys [i].GetComponent<Image> ().color;
					nextColour.a = 0.3f;
					Keys [i].GetComponent<Image> ().color = nextColour;

					// Unshow note on stave.
					Notes [i].SetActive (false);
				}
			}
		} else if (screen == 2) {
			for (int i = 0; i < KeysScreenTwo.Length; ++i) {
				if (MidiMaster.GetKey (48 + i) > 0f) {
					Debug.Log ("Key: " + (48 + i));
					Color nextColour = KeysScreenTwo [i].GetComponent<Image> ().color;
					nextColour.a = 1f;
					KeysScreenTwo [i].GetComponent<Image> ().color = nextColour;
					
					// Show notes on stave.
					Notes2 [i].SetActive (true);
					if (i == 1 || i == 3 || i == 6 || i == 8 || i == 10 || i == 13 || i == 15 || i == 18 || i == 20 || i == 22) {
						Notes2 [i - 1].SetActive (true);
					}
				} else {
					Color nextColour = KeysScreenTwo [i].GetComponent<Image> ().color;
					nextColour.a = 0.3f;
					KeysScreenTwo [i].GetComponent<Image> ().color = nextColour;
					
					// Unshow note on stave.
					Notes2 [i].SetActive (false);
				}
			}
		}
	}
}
