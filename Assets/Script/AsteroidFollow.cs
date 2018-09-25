using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidFollow : MonoBehaviour {

    private Transform player;
    private float x;
    private float y;
    private float z;
    private Vector3 rotationAxe;
    private float speed = 200.0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        transform.rotation = Random.rotation;
        x = Random.value;
        y = Random.value;
        z = Random.value;
        rotationAxe = new Vector3(x, y, z);
    }


    void Update () {

        transform.position = player.transform.position;
        transform.Rotate(rotationAxe * speed * Time.deltaTime);
    }
}
