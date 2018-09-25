using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalaxyRotator : MonoBehaviour {

	void Update () {
        float speed = 20f;
        transform.Rotate(Vector3.up * speed * Time.deltaTime);
	}
}
