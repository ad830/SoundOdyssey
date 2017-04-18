using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ProfileSelect : MonoBehaviour {

    public GameObject ProfileSelection;
    public GameObject EditPopup;
    public GameObject SplashScreenButton;
    
    bool[] profiles = new bool[3];

    public class Profile
    {
        bool filled;
        string name;
        float[] fluency;
        float[] accuracy;

        public bool Filled
        {
            get { return filled; }
        }
        public string Name
        {
            get { return name; }
        }
        public float[] Fluency
        {
            get { return fluency; }
        }
        public float[] Accuracy
        {
            get { return accuracy; }
        }

        public Profile()
        {
            this.name = "Default";
            this.filled = false;
            this.fluency = new float[5];
            this.accuracy = new float[5];
        }

        public Profile(string _name)
        {
            this.name = _name;
            this.fluency = new float[5];
            this.accuracy = new float[5];
            this.filled = true;
        }

        public void MakeStats()
        {
            // RANDOM STATS
            for (int i = 0; i < 5; i++)
            {
                this.fluency[i] = Random.value;
                this.accuracy[i] = Random.value;
            }
        }

        public void Fill(string _name)
        {
            this.name = _name;
            MakeStats();
        }
    }
    Profile[] profileDatas = new Profile[3];
    
    int currentProf = 0;
    InputField field;
    GameObject ProfileTemp;

    string inputName = "";
    
	// Use this for initialization
	void Start () 
    {
        for (int i = 0; i < 3; i++ )
        {
            profiles[i] = false;
            profileDatas[i] = new Profile();
        }
        SplashScreenButton.SetActive(true);
        ProfileSelection.SetActive(false);

        field = EditPopup.transform.Find("InputField").gameObject.GetComponent<InputField>();
        HideEditNamePopup();
	}
	
	// Update is called once per frame
	void Update () 
    {
	    
	}

    public void OnClickBegin()
    {
        ProfileSelection.SetActive(true);
        SplashScreenButton.SetActive(false);
    }

    public void OnClickProfile(int pIdx)
    {
        if (pIdx < 0 || pIdx > profiles.Length)
        {
            Debug.Log("Pick a correct fucking profile for once");
        }
        else
        {
            currentProf = pIdx;
            ProfileTemp = GameObject.FindGameObjectsWithTag("Profile")[pIdx];
            //currentProfName = ProfileTemp.transform.GetChild(0).GetComponent<Text>().text;
            if (profiles[pIdx] == false)
            {
                ShowEditNamePopup();
            }
            else
            {
                //Show Main Menu
                //ShowProfileMenu();
                ShowMainMenu(pIdx);
            }
        }
    }

    void ShowEditNamePopup()
    {
        EditPopup.SetActive(true);
        EventSystem.current.SetSelectedGameObject(field.gameObject, null);
        field.ActivateInputField();
    }

    void HideEditNamePopup()
    {
        EditPopup.SetActive(false);
        field.text = "";
    }

    void ShowMainMenu(int pIdx)
    {
        field.text = "";
        //Debug.Log("Oh hey, there is meant to be a main menu here, one sec");/*
        /*while (Camera.main.transform.position.z >= -15.0f)
        {
            Camera.main.transform.Translate(new Vector3(0.0f, 0.0f, 10.0f) * Time.deltaTime * 1.0f);
        }*/
    }

    public void ConfirmName()
    {
        //Debug.Log(currentProf);
        inputName = field.text;

        if (inputName.Length == 0)
        {
            // TO DO display error message about no empty names
            Debug.Log("Tried to enter an empty name");
            return;
        }

        if (!profiles[currentProf])
        {
            profiles[currentProf] = true;
        }
        else
        {
            // updating name of profile

        }

        // change displayed profile name in profile selection
        Transform trans = ProfileTemp.transform;
        GameObject obj = trans.GetChild(0).gameObject;
        obj.GetComponent<Text>().text = inputName;

        // test profile data
        profileDatas[currentProf].Fill(inputName);

        HideEditNamePopup();
        ShowMainMenu(currentProf);
    }

    public void BackFromPopup()
    {
        HideEditNamePopup();
    }
}
