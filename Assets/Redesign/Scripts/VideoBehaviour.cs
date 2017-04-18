using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VideoBehaviour : MonoBehaviour {

    Resolution[] resolutions;
    public Text resolutionText;
    public Text AAText;
    public Text QLText;
    string w, h;
    int tempQL;

    public GameObject[] options;
    public GameObject qualityPanel;

    private string[] qualityNames = {
                                        "Low",
                                        "Medium",
                                        "High",
                                        "Ultra",
                                        "Galactic"
                                    };

	// Use this for initialization
	void Start () {
        for (int i = 0; i < options.Length; i++)
        {
            GameObject qualityLevelButton = options[i];
            Button button = qualityLevelButton.GetComponent<Button>();
            int buttonIdx = i;
            button.onClick.AddListener(() =>
            {
                PickQualityOption(buttonIdx);
                SetQualityLevel(buttonIdx);
            });
        }

        w = Screen.currentResolution.width.ToString();
        h = Screen.currentResolution.height.ToString();

        resolutionText.text = w + " x " + h;
        AAText.text = "x" + QualitySettings.antiAliasing.ToString();

        tempQL = QualitySettings.GetQualityLevel();

        if (tempQL >= 0 && tempQL < qualityNames.Length)
        {
            QLText.text = qualityNames[tempQL];
        }

        resolutions = new Resolution[4];
        resolutions[0].width = 1280;
        resolutions[0].height = 720;
        resolutions[1].width = 1366;
        resolutions[1].height = 768;
        resolutions[2].width = 1600;
        resolutions[2].height = 900;
        resolutions[3].width = 1920;
        resolutions[3].height = 1080;
	}

    public void PickQualityOption(int idx)
    {
        qualityPanel.SetActive(false);
        foreach (GameObject option in options)
        {
            option.SetActive(false);
        }
        QLText.text = qualityNames[idx];
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ToggleFullScreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
    }

    public void SetResolution(int i)
    {
        
        /*
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            Debug.LogError(Screen.resolutions[i].ToString());
        }*/

        Screen.SetResolution(resolutions[i].width, resolutions[i].height, Screen.fullScreen);
    }

    public void SetAntiAliasing(int AA)
    {
        QualitySettings.antiAliasing = AA;
    }

    public void ToggleVSync(bool vsync)
    {
        if (!vsync)
        {
            QualitySettings.vSyncCount = 0;
        }
        else
        {
            QualitySettings.vSyncCount = 1;
        }
    }

    public void SetQualityLevel(int QL)
    {
        QualitySettings.SetQualityLevel(QL, true);
    }
}
