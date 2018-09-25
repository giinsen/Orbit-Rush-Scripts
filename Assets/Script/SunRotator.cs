using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunRotator : MonoBehaviour {

    private float x;
    private float y;
    private float z;
    private Vector3 rotationAxe;
    private float speed = 20f;

    // Use this for initialization
    void Start () {
        transform.rotation = Random.rotation;
        x = Random.value;
        y = Random.value;
        z = Random.value;
        rotationAxe = new Vector3(x, y, z);

        
    }
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(rotationAxe * speed * Time.deltaTime);
    }
}
