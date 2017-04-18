using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class KeyValueViewBehaviour : MonoBehaviour {

    [SerializeField]
    private GameObject keyValueViewPrefab;

    private string FormatKeyText(string key)
    {
        return string.Format("{0}:", key);
    }

    public void ClearView(Transform node)
    {
        int childCount = node.childCount;
        for (int i = 0; i < childCount; i++)
        {
            GameObject child = node.GetChild(i).gameObject;
            Destroy(child);
        }
    }

    public void FillView(Dictionary<string, string> data)
    {
        Transform viewTransform = base.transform.GetChild(0);
        ClearView(viewTransform);
        foreach (KeyValuePair<string, string> pair in data)
        {
            GameObject pairObj = Instantiate(keyValueViewPrefab) as GameObject;
            pairObj.name = "KeyValuePair - " + pair.Key;
            Transform pairTransform = pairObj.GetComponent<Transform>();
            pairTransform.Find("KeyText").GetComponent<Text>().text = FormatKeyText(pair.Key);
            pairTransform.Find("ValueText").GetComponent<Text>().text = pair.Value;
            pairTransform.SetParent(viewTransform);
        }
    }

    public void LOL()
    {
        Dictionary<string, string> testData = new Dictionary<string, string>(5);
        testData.Add("Notes Hit", "1580");
        testData.Add("Notes Missed", "670");
        testData.Add("Hit / Miss Ratio", string.Format("{0}", 1580.0f / 670.0f));
        testData.Add("Git Gud", "false");

        FillView(testData);
    }

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
