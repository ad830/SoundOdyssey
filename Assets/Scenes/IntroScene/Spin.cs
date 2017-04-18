using UnityEngine;
using System.Collections;

public class Spin : MonoBehaviour {
    public Vector3 spinDelta = Vector3.one * 5;
    public Vector3 startAngles = Vector3.zero;

	// Use this for initialization
	void Start () {
        base.transform.localEulerAngles = startAngles;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 angles = base.transform.localEulerAngles;
        float x = Mathf.MoveTowardsAngle(angles.x, angles.x + spinDelta.x * Time.deltaTime, spinDelta.x * Time.deltaTime);
        float y = Mathf.MoveTowardsAngle(angles.y, angles.y + spinDelta.y * Time.deltaTime, spinDelta.y * Time.deltaTime);
        float z = Mathf.MoveTowardsAngle(angles.z, angles.z + spinDelta.z * Time.deltaTime, spinDelta.z * Time.deltaTime);
        base.transform.localEulerAngles = new Vector3(x, y, z);
	}
}
