using UnityEngine;
using System.Collections;

public class OrbitBehaviour : MonoBehaviour {

    public enum OrbitDirection { Clockwise, Anticlockwise };

    public float distance = 1f;
    public float speed = 2f;
    public string orbitObjectName = "PB::DEFAULT_ORBIT_NAME";
    public Color colour = Color.white;
    public float size = 1f;
    public OrbitDirection direction = OrbitDirection.Clockwise;
    public float startAngle = -1f;
    public float angle;

    private float targetAngle;
    private Vector2 orbitPosition = Vector2.zero;
    private GameObject orbitObject;
    private Vector2 nextPos;

	// Use this for initialization
	void Start () {
        if (startAngle != -1f)
        {
            angle = startAngle;
        }
        else
        {
            angle = Random.value * 360f;
        }
        

        orbitObject = GameObject.Find(orbitObjectName);
        if (orbitObject)
        {
            orbitPosition = orbitObject.transform.position;
        }
        else
        {
            Debug.Log("Could not find game object with name " + orbitObjectName);
        }
        
        GetComponent<MeshRenderer>().material.color = colour;
        GetComponent<Transform>().localScale = new Vector3(size, size, size);

        ApplyOrbit();
	}

    void ApplyOrbit () {
        nextPos.x = orbitPosition.x + distance * Mathf.Cos(angle * Mathf.Deg2Rad);
        nextPos.y = orbitPosition.y + distance * Mathf.Sin(angle * Mathf.Deg2Rad);
        transform.position = nextPos;
    }
	
	// Update is called once per frame
	void Update () {
	    // Orbit
        if (direction == OrbitDirection.Clockwise)
        {
            targetAngle = angle - speed;
        }
        else
        {
            targetAngle = angle + speed;
        }
        
        angle = Mathf.MoveTowardsAngle(angle, targetAngle, speed);
        
        orbitPosition = orbitObject.transform.position;

        ApplyOrbit();
	}
}
