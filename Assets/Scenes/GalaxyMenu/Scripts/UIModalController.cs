using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIModalController : MonoBehaviour {

    [SerializeField]
    GameObject sectorLocked;

    List<GameObject> modals;

    public void DisplayModal(string modalName)
    {
        GameObject modal = GetModal(modalName);
        if (modal)
        {
            DisplayModal(modal);
        }
    }

    public void DisplayModal(GameObject obj)
    {
        obj.SetActive(true);
    }

    public void HideModal(string modalName)
    {
        GameObject modal = GetModal(modalName);
        if (modal)
        {
            HideModal(modal);
        }
    }

    public void HideModal(GameObject obj)
    {
        obj.SetActive(false);
    }

    public GameObject GetModal(string modalName)
    {
        return modals.Find((obj) =>
        {
            return obj.name == modalName;
        });
    }

	// Use this for initialization
	void Start () {
        modals = new List<GameObject>(base.transform.childCount);
        modals.Clear();
        modals.Add(sectorLocked);

        Button dismissButton = sectorLocked.transform.Find("OuterEdge").Find("InnerBody").Find("DismissButton").GetComponent<Button>();
        dismissButton.onClick.RemoveAllListeners();
        dismissButton.onClick.AddListener(() =>
        {
            HideModal(sectorLocked);
        });
        //HideModal(sectorLocked);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
