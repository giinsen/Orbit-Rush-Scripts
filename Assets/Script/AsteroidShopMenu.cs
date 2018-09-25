using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidShopMenu : MonoBehaviour {

	private Transform player;
    private float x;
    private float y;
    private float z;
    private Vector3 rotationAxe;
    private float speed = 200.0f;

    public ParticleSystem[] ps;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        transform.rotation = Random.rotation;
        x = Random.value;
        y = Random.value;
        z = Random.value;
        rotationAxe = new Vector3(x, y, z);

        //LaunchFirstParticleEffect();

    }


    void Update () {

        transform.position = player.transform.position;
        transform.Rotate(rotationAxe * speed * Time.deltaTime);
    }

    private void LaunchFirstParticleEffect()
    {
        for (int i = 0; i < 5; i++)
        {
            foreach (ParticleSystem p in ps)
            {
                ParticleSystem particle = Instantiate(p);
                particle.transform.position = new Vector3(3000, 3000, 3000);
            }
        }
    }
}
