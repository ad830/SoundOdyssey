using UnityEngine;
using System.Collections;

public class QuickMenuHandler : MonoBehaviour {

    Animator anim;

	// Use this for initialization
	void Start () {
        anim = GameObject.Find("NewUI").GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OpenMenu()
    {
        
    }
}
