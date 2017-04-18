using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class QuickMenuDirector : MonoBehaviour {

    public GameObject QuickAccessMenu;
    public GameObject ShowQuickAccessMenu;
	// Use this for initialization
	void Start () 
    {
        QuickAccessMenu.SetActive(false);
        ShowQuickAccessMenu.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnClickQuickMenu()
    {
        ShowQuickAccessMenu.SetActive(false);
        QuickAccessMenu.SetActive(true);
    }

    public void onClickHideMenu()
    {
        QuickAccessMenu.SetActive(false);
        ShowQuickAccessMenu.SetActive(true);
    }
}
