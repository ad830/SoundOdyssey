using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using MidiJack;

public class Calibration : MonoBehaviour {

	int lowest;
	int highest;
	int currentKeySize;

	
	public GameObject CalibrationStage1;
	public GameObject CalibrationStage2;
	public GameObject CalibratePopup;
	public GameObject CalibrateSlider;

	int [] keySizes = {
		49,
		61,
		76,
		88
	};

	bool isCalibrating;

	// Use this for initialization
	void Start () {
		currentKeySize = 61;
		lowest = -1;
		highest = -1;
		isCalibrating = false;

		CalibrateSlider.GetComponent<Slider>().onValueChanged.AddListener (HandleSliderChange);
	}

	public void HandleSliderChange(float value)
	{
		int idx = (int)value;
		currentKeySize = keySizes [idx];
		GameObject.Find ("KeySizeDisplay").GetComponent<Text> ().text = "Key Size: " + currentKeySize;
	}

	public void Calibrate (){
		isCalibrating = true;
		lowest = -1;
		highest = -1;
	}

	public void OnCalibrate() {
		CalibratePopup.SetActive (true);
		CalibrationStage1.SetActive (true);
	}
	
	public void OnCalibrationNext() {
		if (lowest == -1) {
			GameObject.Find ("CalibrationPrompt").GetComponent<Text> ().text = "Press the lowest key on the piano.";
		} else {
			GameObject.Find ("CalibrationPrompt").GetComponent<Text> ().text = "Press the highest key on the piano.";
		}
	}

	public void OnDoneCalibration() {


	}

	// Update is called once per frame
	void Update () {
		/*
		if (isCalibrating) {
			int message = MidiMaster.GetKey();
			if (message != -1) {
				Debug.Log ("Key was: " + message);
				if (lowest == -1)
				{
					lowest = message;
					GameObject.Find ("CalibrationPrompt").GetComponent<Text> ().text = "Press the highest key on the piano.";
				}
				else if (highest == -1)
				{
					highest = message;
				}
				if (lowest > highest) {
					int temp = highest;
					highest = lowest;
					lowest = temp;
				} else if (lowest == highest) {
					Debug.Log ("Lowest and highest were the same!");
					lowest = -1;
					highest = -1;
					GameObject.Find ("CalibrationPrompt").GetComponent<Text> ().text = "Calibration Failed! Lowest and highest keys are the same!";
				}
				if (lowest != -1 && highest != -1) {
					// check keyboard size == slider key size
					int keySize = highest - lowest + 1;
					Debug.Log(keySize);
					Debug.Log(currentKeySize);
					if (currentKeySize != keySize) {
						GameObject.Find ("CalibrationPrompt").GetComponent<Text> ().text = "Calibration Failed! Entered key sizes are different!";
					}
					else {
						Debug.Log ("Finished calibration: Keysize: " + (highest - lowest + 1));
						isCalibrating = false;
						GameObject.Find ("CalibrationPrompt").GetComponent<Text> ().text = "Calibration Complete!";
					}
				}
			}
		}*/
	}
}
