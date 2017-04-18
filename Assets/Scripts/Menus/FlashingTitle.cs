using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FlashingTitle : MonoBehaviour {

	Text Title;
	bool isBlinking = true;

	// Use this for initialization
	void Start () {
		Title = GetComponent<Text> ();
		StartCoroutine (BlinkText ());
	}


	public IEnumerator BlinkText(){
		while (isBlinking) {
			Title.color = new Color(0, 190, 255);
			yield return new WaitForSeconds(1f);
			Title.color = new Color(128, 0, 255);
			yield return new WaitForSeconds(1f);
			Title.color = new Color(255, 255, 255);
			yield return new WaitForSeconds(1f);
		}

	}

	// Update is called once per frame
	void Update () {
	
	}
}
