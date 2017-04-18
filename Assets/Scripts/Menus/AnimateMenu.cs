using UnityEngine;
using System.Collections;

public class AnimateMenu : MonoBehaviour {

	Vector3 normalPosition;
	Vector3 offscreenPosition;

	Vector3 targetPosition;

	void OnEnable()
	{
		targetPosition = normalPosition;
	}

	void OnDisable()	
	{
		targetPosition = offscreenPosition;
	}

	// Use this for initialization
	void Start () {
		normalPosition = base.transform.position;

		offscreenPosition = new Vector3();
		offscreenPosition.x = normalPosition.x + 1;
		offscreenPosition.y = normalPosition.y + 1;
		offscreenPosition.z = normalPosition.z;

		base.transform.position = offscreenPosition;
	}

	void Update()
	{
		base.transform.position = Vector3.MoveTowards (base.transform.position, targetPosition, 10f);
	}
}
