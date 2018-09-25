using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetBack : MonoBehaviour
{

    private float x;
    private float y;
    private float z;
    private float scale;
    private Vector3 planetRotationAxe;
    private float speed = 50.0f;


    public Material[] materialArray;
    public string planetColor;
    private Material mat;

    public GameObject[] ringArray;
    private GameObject ring;

    public GameObject[] supernovaArray;
    private GameObject supernova;

    public ParticleSystem[] visitedParticleArray;
    private ParticleSystem visitedParticle;

    public ParticleSystem death;
    public ParticleSystem death2;
    private ParticleSystem deathParticle;
    private ParticleSystem deathParticle2;

    private Player player;
    private bool alreadyDead = false;
    public GameObject explosedSun;
    private GameObject explosedPlanet;

    public bool haveRing = false;
    public bool visited = false;

    private bool alreadyLaunchParticle = false;
    public bool isCollided = false;

    public float nbSecondRotation = 0.0f;
    int randomMat;

    private GameManager gameManager;

    bool touch = false;




    private Vector3 smoothPlanetVector;
    private GameObject p;
    private bool backwardPlanet = false;
    private bool forwardPlanet = false;
    private Vector3 setVector = new Vector3(-325, 116, 340);

    private Vector3 offsetVector = new Vector3(-468, 165, 0);


    // Use this for initialization
    void Start()
    {

        transform.rotation = Random.rotation;
        x = Random.value;
        y = Random.value;
        z = Random.value;
        planetRotationAxe = new Vector3(x, y, z);

        scale = 70f;
        transform.localScale = new Vector3(scale, scale, scale);

        randomMat = Random.Range(0, materialArray.Length);
        GetComponent<Renderer>().material = materialArray[randomMat];

        if (Random.Range(0,5) < 1)
        {
            ring = Instantiate(ringArray[randomMat]);
            ring.transform.SetParent(transform.parent);
            ring.transform.position = transform.parent.position + (Vector3.up * 10);
            Vector3[] ringRotationArray = new Vector3[] { new Vector3(-25, 0, 28), new Vector3(-40, 2, -13), new Vector3(40, -16, 11), new Vector3(4, 45, 133) };
            ring.transform.eulerAngles = ringRotationArray[Random.Range(0, 4)];
            int scaleRing = 12;
            ring.transform.localScale = new Vector3(scaleRing, scaleRing, scaleRing);
        }
    }

    void Update()
    {
        transform.Rotate(planetRotationAxe * speed * Time.deltaTime);        
    }

}
