using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanetMotor : MonoBehaviour {

    private float x;
    private float y;
    private float z;
    private float scale;
    private Vector3 planetRotationAxe;
    private Vector3 supernovaRotationAxe;
    private float speed = 100.0f;

    public Material[] materialArray;
    public Material materialBonusPlanet;
    public Material materialDeadBonusPlanet;

    public string planetColor;
    private Material mat;

    public GameObject[] ringArray;
    private GameObject ring;

    public GameObject[] supernovaArray;
    private GameObject supernova;

    public ParticleSystem[] visitedParticleArray;
    private ParticleSystem visitedParticle;

    public ParticleSystem bonus;
    private ParticleSystem bonusParticle;
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

    public Material myMaterial;

    private bool lateStartAlreadyLaunch = false;

    public bool firstPlanet;

    // Use this for initialization
    void Start() {

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        gameManager = FindObjectOfType<GameManager>();
    }

    private void LateStart()
    {
        transform.rotation = Random.rotation;
        x = Random.value;
        y = Random.value;
        z = Random.value;
        planetRotationAxe = new Vector3(x, y, z);

        x = Random.value;
        y = Random.value;
        z = Random.value;
        supernovaRotationAxe = new Vector3(x, y, z);

        //première utilisation ! on force la planète en rouge
        if (SaveManager.Instance.state.nbTimesPlayed == 0 && firstPlanet == true)
        {
            scale = (Random.Range(15, 40)) + 25f;
            transform.localScale = new Vector3(scale, scale, scale);

            randomMat = 1;
            GetComponent<Renderer>().material = materialArray[randomMat];
            myMaterial = GetComponent<Renderer>().material;
            SetPlanetColor(randomMat);
        }
        //1 chance sur 60 de devenir une planète qui rajoute du temps
        else if (Random.Range(0, 60) <= 1)
        {
            GetComponent<Renderer>().material = materialBonusPlanet;
            myMaterial = GetComponent<Renderer>().material;
            transform.gameObject.tag = "PlanetBonusTimer";
            scale = 60f;
            transform.localScale = new Vector3(scale, scale, scale);
        }
        else
        {
            scale = (Random.Range(15, 40)) + 25f;
            transform.localScale = new Vector3(scale, scale, scale);

            randomMat = Random.Range(0, 4);
            GetComponent<Renderer>().material = materialArray[randomMat];
            myMaterial = GetComponent<Renderer>().material;
            SetPlanetColor(randomMat);

            if (Random.Range(0, 8) <= 1)
            {
                ring = Instantiate(ringArray[randomMat]);
                ring.transform.SetParent(transform.parent);
                ring.transform.position = transform.parent.position + (Vector3.up * 10);
                ring.transform.rotation = Random.rotation;
                int scaleRing = Random.Range(8, 10);
                ring.transform.localScale = new Vector3(scaleRing, scaleRing, scaleRing);
                haveRing = true;
            }
        }
    }

    void Update() {

        //on détruit l'objet s'il est trop loin du joueur
        if (Vector3.Distance(transform.position, player.transform.position) >= 3000)
        {
            Destroy(transform.parent.gameObject);
        }

        //on l'instancie s'il assez proche
        if (Vector3.Distance(transform.position, player.transform.position) <= 1000 && lateStartAlreadyLaunch == false)
        {
            lateStartAlreadyLaunch = true;
            LateStart();
        }
        if (lateStartAlreadyLaunch == false)
            return;

        //On fait tourner la planète
        transform.Rotate(planetRotationAxe * speed * Time.deltaTime);

        //On fait tourner la supernova si celle-ci existe
        if (supernova != null)
        {
            supernova.transform.Rotate(supernovaRotationAxe * speed * Time.deltaTime);
        }

        //VisitedAnimation : la planète viens d'être visitée, on lance les effets de particule
        if (visited == true && alreadyLaunchParticle == false)
        {
            VisitedAnimation();
        }

        //DeathAnimation : la joueur est rentré en collision avec cette planète
        if (SceneManager.GetActiveScene().name == "Gravity")
        {
            if (player.imDead == true && isCollided == true && alreadyDead == false)
            {
                gameManager.GameOver(this);
                player.deathPlanet = this;
            }
        }
    }


    private void VisitedAnimation()
    {   
        if (transform.tag == "PlanetBonusTimer")
        {
            //Destroy(transform.parent.gameObject);
            alreadyLaunchParticle = true;
            gameManager.generalTimer += 5;
            GameObject.FindGameObjectWithTag("GameUI").GetComponent<Animator>().SetBool("BonusTimer", true);
            //GetComponent<Renderer>().material = materialDeadBonusPlanet;
            player.lastBonusPlanet = transform.gameObject;
            player.nbBonusPlanet++;
            transform.localScale = new Vector3(1, 1, 1);

            bonusParticle = Instantiate(bonus);
            //bonusParticle.transform.SetParent(transform.parent);
            bonusParticle.transform.position = transform.parent.position;
            bonusParticle.transform.localScale = new Vector3(50, 50, 50);
        }
        else
        {
            alreadyLaunchParticle = true;
            supernova = Instantiate(supernovaArray[randomMat]);
            supernova.transform.SetParent(transform.parent);
            supernova.transform.position = transform.parent.position + (Vector3.up * 10);
            supernova.transform.rotation = Random.rotation;
            supernova.transform.localScale = new Vector3(scale, scale, scale);

            transform.localScale = new Vector3(scale - 10, scale - 10, scale - 10);

            visitedParticle = Instantiate(visitedParticleArray[randomMat]);
            visitedParticle.transform.SetParent(transform.parent);
            visitedParticle.transform.position = transform.parent.position + (Vector3.up * 10);
            visitedParticle.transform.localScale = new Vector3(30, 30, 30);
        }
        
    }

    public void DeathAnimation()
    {
        //lancement des animations de mort
        alreadyDead = true;
        deathParticle = Instantiate(death);
        deathParticle.transform.SetParent(transform.parent);
        deathParticle.transform.position = transform.parent.position;
        deathParticle.transform.eulerAngles = new Vector3(0, 0, 0);
        deathParticle.transform.localScale = new Vector3(70, 70, 70);

        deathParticle2 = Instantiate(death2);
        deathParticle2.transform.SetParent(transform.parent);
        deathParticle2.transform.position = transform.parent.position - (Vector3.up * 10);
        deathParticle2.transform.eulerAngles = new Vector3(0, 0, 0);
        deathParticle2.transform.localScale = new Vector3(100, 100, 100);
        Invoke("MooveThisParticleEffect", 3);

        transform.localScale = new Vector3(1, 1, 1);
        if (!visited)
        {
            supernova = Instantiate(supernovaArray[randomMat]);
            supernova.transform.SetParent(transform.parent);
            supernova.transform.position = transform.parent.position + (Vector3.up * 10);
            supernova.transform.rotation = Random.rotation;
            supernova.transform.localScale = new Vector3(scale, scale, scale);
        }

        explosedPlanet = Instantiate(explosedSun);
        explosedPlanet.transform.SetParent(transform.parent);
        explosedPlanet.transform.position = transform.parent.position + (Vector3.up * 10);
        explosedPlanet.transform.localScale = new Vector3(scale - 1, scale - 1, scale - 1);
    }

    private void MooveThisParticleEffect()
    {
        deathParticle2.transform.position = transform.parent.position - (Vector3.up * 1000) + (Vector3.left * 1000);
        deathParticle2.transform.localScale = new Vector3(1, 1, 1);
    }

    private void SetPlanetColor(int mat)
    {
        //bleu rouge violet jaune
        if (mat == 0)
        {
            planetColor = "blue";
        } else if (mat == 1)
        {
            planetColor = "red";
        } else if (mat == 2)
        {
            planetColor = "purple";
        } else if (mat == 3)
        {
            planetColor = "yellow";
        }
    }

}
