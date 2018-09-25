using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    private CharacterController cc;

    public float speed = 300.0f;

    private float zRotation;

    private GameObject[] planetList;
    public GameObject nearestPlanet;

    private int sensAiguille;

    private float minDistanceMaxSpeed = 40f;

    public bool inRotation = false;

    private float animationDuration = 4f;
    private float transition = 0f;
    private Vector3 animationOffset = new Vector3(0, 5, 5);

    public int nbPlanetVisited = 0;

    float distance = Mathf.Infinity;

    public bool imDead = false;
    
    public string playerColor = "";
    public float nbGoodColor = 0;
    private bool samePlanet = false;

    private GameManager gameManager;

    private float timeDifficulty = 0f;

    public GameObject lastBonusPlanet;
    public int nbBonusPlanet = 0;

    public PlanetMotor myPlanet;

    public PlanetMotor deathPlanet;
    public bool deadBySun = false;
    public GameObject myDeadlySun;

    public Transform particles;
    public Transform trails;

    public GameObject UpPopText;
    public GameObject DownPopText;
    public GameObject TransitionPopText;
    public GameObject VisitedPopText;
    public GameObject ProgressionPopText;
    private int realScore = 1;

    public GameObject newPlanet;

    private bool launchTurnArroundSound = false;

    public bool tutorialIsOn = true;
    public bool tutorialIsOn2 = false;
    public GameObject panelTutorial;
    public GameObject panelTutorial2;

    public ParticleSystem[] ps;

    public GameObject panelBlue;
    public GameObject panelRed;
    public GameObject panelYellow;
    public GameObject panelPurple;
    private int nbRightClickInARow = 0;

    public float nbSecondTurnFirstPlanet = 0f;

    void Start() {
        gameManager = FindObjectOfType<GameManager>();
        cc = GetComponent<CharacterController>();
        transform.eulerAngles = new Vector3(0, 0, 90);
        SetParticleAndTrail();

        //LaunchFirstParticleEffect();
    }

    void Update()
    {
        //return si le joueur est mort
        if (imDead == true)
        {
            return;
        }

        //return si c'est l'animation de départ
        if (Time.timeSinceLevelLoad < animationDuration)
        {
            cc.Move(transform.up * speed * Time.deltaTime);
            planetList = GameObject.FindGameObjectsWithTag("Planet");
            nearestPlanet = GetClosestPlanet(planetList);
            sensAiguille = SensRotation(nearestPlanet.transform);
            return;
        }

        //lancer le tutoriel si c'est la première partie 
        if (SaveManager.Instance.state.firstTimePlayed == true && tutorialIsOn == true)
        {
            LaunchTutorial();
        }

        //augmenter la vitesse périodiquement
        IncreaseSpeed();

        //afficher la ligne d'aide si l'option est activée
        if (SaveManager.Instance.state.lineRendererIsOn)
        {
            LineRenderer();
        }
            
        //2 options ici : tourner ou avancer

        //on tourne si le joueur appui sur le bouttoon corresppondant a une couleur
        if (Input.GetButton("Jump") || playerColor != "")
        {
            RotateAroundNearestPlanet();
            DetermiateVisitedPlanet();
            PlayTurnArroundSound();
            gameManager.endTimer = true;
        }
        //on avance tout droit
        else
        {

            if (lastBonusPlanet != null)
            {
                Destroy(lastBonusPlanet.transform.parent.gameObject);
            }
            inRotation = false;
            samePlanet = false;
            gameManager.endTimer = true;
            planetList = GameObject.FindGameObjectsWithTag("Planet");
            nearestPlanet = GetClosestPlanet(planetList);
            sensAiguille = SensRotation(nearestPlanet.transform);
            cc.Move(transform.up * speed * Time.deltaTime);
            StopTurnArroundSound();
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        //détection si le joueur rentre en collision avec un object 
        if (collision.gameObject.tag == "Sun")
        {           
            myDeadlySun = collision.gameObject;
            gameManager.GameOver();
            deadBySun = true;
            imDead = true;            
        }
        //myPlaney != collision.gameObject  :::::  si on tourne trop proche de la planète, l'asteroid peut rentrer en collision
        else if (collision.gameObject.tag == "Surface" && myPlanet != collision.gameObject)
        {
            FindObjectOfType<AudioManager>().Stop("TurnArround");
            //nearestPlanet = GetClosestPlanet(planetList);
            nearestPlanet = collision.gameObject;
            collision.gameObject.GetComponent<PlanetMotor>().isCollided = true;
            imDead = true;
        }


    }

    private GameObject GetClosestPlanet(GameObject[] planets)
    {
        int nbPlanets = 0;
        GameObject nearestPlanet = null;
        distance = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        //parcours de toutes les planètes pour savoir laquelle est la plus proche
        foreach (GameObject t in planets)
        {
            if (t.transform.parent.Find("Surface").gameObject != lastBonusPlanet)
            {
                //compter le nombre de planètes
                if (Vector3.Distance(t.transform.position, currentPos) <= 2000)
                {
                    nbPlanets++;
                }

                //trouver la plus proche
                float dist = Vector3.Distance(t.transform.position, currentPos);
                if (dist < distance)
                {
                    nearestPlanet = t;
                    distance = dist;
                }
            }                 
        }

        DeterminateMinDistanceMaxSpeed(distance);

        if (nbPlanets < 150 && Time.timeSinceLevelLoad > animationDuration) // nb planetes max autour du joueur (2000 de rayon)
        {
            int nbFois = 0;
            float minPlanetX = transform.position.x - 2000;
            float minPlanetY = transform.position.y - 2000;
            float maxPlanetX = transform.position.x + 2000;
            float maxPlanetY = transform.position.y + 2000;

            float x = Random.Range(minPlanetX, maxPlanetX);
            float y = Random.Range(minPlanetY, maxPlanetY);

            Vector3 v = new Vector3(x, y, 0);

            while (Vector3.Distance(transform.position, v) > 2000 || PlanetTooNear(v,planets) || SunTooNear(v) || Vector3.Distance(transform.position, v) < 450)
            {
                x = Random.Range(minPlanetX, maxPlanetX);
                y = Random.Range(minPlanetY, maxPlanetY);

                v = new Vector3(x, y, 0);
                nbFois++;
            }

            //Debug.Log("nbFois : " + nbFois);
            GameObject newP = Instantiate(newPlanet);
            newP.transform.position = v;
            newP.transform.SetParent(GameObject.Find("Planets").transform);
        }        

        return nearestPlanet;
    }

    private bool PlanetTooNear(Vector3 v, GameObject[] planets)
    {
        foreach (GameObject t in planets)
        {
            if (Vector3.Distance(t.transform.position, v) < 170)
            {
                return true;
            }
        }
        return false;
    }

    private bool SunTooNear(Vector3 v)
    {
        foreach (GameObject t in GameObject.FindGameObjectsWithTag("Sun"))
        {
            if (Vector3.Distance(t.transform.position, v) < 280)
            {
                return true;
            }
        }
        return false;
    }

    private void LaunchTutorial()
    {
        panelTutorial.SetActive(true);
        Time.timeScale = 0f;
    }

    public void LaunchTutorial2()
    {
        panelTutorial2.SetActive(true);
        Time.timeScale = 0f;
    }

    private void LaunchFirstParticleEffect()
    {
        for (int i=0; i<5; i++)
        {
            foreach (ParticleSystem p in ps)
            {
                ParticleSystem particle = Instantiate(p);
                particle.transform.position = new Vector3(0, 0, 0);
                particle.transform.localScale = new Vector3(50, 50, 50);
            }
        }
    }

    private void DeterminateMinDistanceMaxSpeed(float distance)
    {
        if (distance > 100)
        {
            minDistanceMaxSpeed = 40f;
        }
        else if (distance <= 100 && distance > 70)
        {
            minDistanceMaxSpeed = 34f;
        }
        else if (distance <= 70 && distance > 40)
        {
            minDistanceMaxSpeed = 27f;
        }
        else
        {
            minDistanceMaxSpeed = 20f;
        }
    }

    private int SensRotation(Transform planetTransform)
    {
        GameObject copy = this.gameObject;
        Vector3 positionLocale = transform.InverseTransformPoint(planetTransform.position);

        if (positionLocale.x < 0)
        {
            // sens aiguille
            return 1;
        }
        if (positionLocale.x > 0)
        {
            // sens inverse aiguille
            return -1;
        }
        else
        {
            // au centre
            return 0;
        }
        
    }

    private void RotateAroundNearestPlanet()
    {
        //On fait tourner le player (pas l'asteroid) pour se préparer au moment ou le joueur va lacher le bouton
        inRotation = true;
        ComparePlanetColorAndPlayerColor();
        if (sensAiguille == 1)
        {
            if (transform.position.y < nearestPlanet.transform.position.y)
            {
                float zRotationBis = Vector3.Angle(nearestPlanet.transform.right, transform.position - nearestPlanet.transform.position);
                zRotation = zRotationBis + (180f - zRotationBis) * 2;
            }
            else
            {
                zRotation = Vector3.Angle(nearestPlanet.transform.right, transform.position - nearestPlanet.transform.position);
            }
        }
        else if (sensAiguille == -1)
        {
            if (transform.position.y < nearestPlanet.transform.position.y)
            {
                float zRotationBis = Vector3.Angle(nearestPlanet.transform.right, transform.position - nearestPlanet.transform.position);
                zRotation = (zRotationBis + (180f - zRotationBis) * 2) - 180f;
            }
            else
            {
                zRotation = Vector3.Angle(nearestPlanet.transform.right, transform.position - nearestPlanet.transform.position) - 180f;
            }
        }
        else
        {
            // on fonce sur la planète la plus proche !!! au secours
        }

        //on fait tourner le player autour de la planète avec la bonne vitesse (proche + vite // loin - vite)
        transform.eulerAngles = new Vector3(transform.rotation.x, transform.rotation.y, zRotation);
        float radius = Vector3.Distance(nearestPlanet.transform.position, transform.position);
        float curSpeed = speed / Mathf.Sqrt(radius);
        transform.RotateAround(nearestPlanet.transform.position + Vector3.up * 10, nearestPlanet.transform.forward, sensAiguille * (curSpeed * minDistanceMaxSpeed) * Time.deltaTime * 180 / (2 * Mathf.PI * radius));
        myPlanet = nearestPlanet.transform.parent.GetChild(0).GetComponent<PlanetMotor>();

        if (SaveManager.Instance.state.nbTimesPlayed == 0 && myPlanet.firstPlanet == true)
        {
            nbSecondTurnFirstPlanet += Time.deltaTime;
        }
    }

    private void PlayTurnArroundSound()
    {       
        if (inRotation && launchTurnArroundSound == false)
        {
            launchTurnArroundSound = true;
            FindObjectOfType<AudioManager>().Play("TurnArround");                    
        }       
    }

    private void StopTurnArroundSound()
    {
        if (launchTurnArroundSound == true)
        {
            launchTurnArroundSound = false;
            FindObjectOfType<AudioManager>().Stop("TurnArround");
        }
    }

    private void DetermiateVisitedPlanet()
    {
        PlanetMotor myPlanet = nearestPlanet.transform.parent.GetChild(0).GetComponent<PlanetMotor>();
        myPlanet.nbSecondRotation = myPlanet.nbSecondRotation + Time.deltaTime;
        //on a tourné autour 0.3s au moins
        //(myPlanet.visited == false && myPlanet.nbSecondRotation > 0.3f && myPlanet.planetColor == playerColor && myPlanet.tag == "Surface")
        //else if
        //(myPlanet.visited == false && myPlanet.nbSecondRotation > 0.3f && myPlanet.tag == "PlanetBonusTimer"
        if (myPlanet.visited == false && myPlanet.nbSecondRotation > 0.3f)
        {
            myPlanet.visited = true;
            nbPlanetVisited++;
            if (myPlanet.haveRing && myPlanet.tag != "PlanetBonusTimer")
            {
                InitPopUP("+ O.3", "visited");
                FindObjectOfType<AudioManager>().Play("ValidatePlanet");
            }
            else if (!myPlanet.haveRing && myPlanet.tag != "PlanetBonusTimer")
            {
                InitPopUP("+ O.1", "visited");
                FindObjectOfType<AudioManager>().Play("ValidatePlanet");
            }
            else if (myPlanet.tag == "PlanetBonusTimer")
            {
                InitPopUP("+ 5 secondes", "visited");
                FindObjectOfType<AudioManager>().Play("UpBonus");
            }           
        }
    }

    private void ComparePlanetColorAndPlayerColor()
    {
        PlanetMotor thePlanet = nearestPlanet.transform.parent.GetChild(0).GetComponent<PlanetMotor>();
        if (playerColor == thePlanet.planetColor && samePlanet == false && thePlanet.visited == false && thePlanet.tag == "Surface")
        {
            samePlanet = true;
            ActiveColorPanel(playerColor);
            if (nbGoodColor < 20)
            {              
                nbGoodColor++;

                if (realScore != ((int)((nbGoodColor + 4) / 4)))
                {
                    realScore = (int)((nbGoodColor + 4) / 4);
                    InitPopUP(realScore.ToString(), "up");
                    FindObjectOfType<AudioManager>().Play("UpBonus");
                }
                else
                {
                    int tmp = (int)nbGoodColor % 4;
                    InitPopUP(tmp.ToString() + " /4", "transition");
                }
            }
        }
        else if (playerColor != thePlanet.planetColor && samePlanet == false && thePlanet.tag == "Surface")
        {
            samePlanet = true;
            nbRightClickInARow = 0;
            InitPopUP("Miss ...", "progression");
            if (nbGoodColor < 8)
            {
                nbGoodColor = 0;
            }
            else if (nbGoodColor >= 8 && nbGoodColor < 12)
            {
                nbGoodColor = 4;
            }
            else if (nbGoodColor >= 12 && nbGoodColor < 16)
            {
                nbGoodColor = 8;
            }
            else if (nbGoodColor >= 16 && nbGoodColor < 20)
            {
                nbGoodColor = 12;
            }
            else if (nbGoodColor == 20)
            {
                nbGoodColor = 16;
            }

            if (realScore != ((int)((nbGoodColor + 4) / 4)))
            {
                realScore = ((int)((nbGoodColor + 4) / 4));
                InitPopUP(realScore.ToString(), "down");
                FindObjectOfType<AudioManager>().Play("DownBonus");
            }           
        }
    }

    private void ActiveColorPanel(string playerColor)
    {
        nbRightClickInARow++;
        if (nbRightClickInARow <= 3)
        {
            InitPopUP("Good", "progression");
        }
        else if (nbRightClickInARow > 3 && nbRightClickInARow <= 5)
        {
            InitPopUP("Nice", "progression");
        }
        else if (nbRightClickInARow > 5 && nbRightClickInARow <= 8)
        {
            InitPopUP("Great !", "progression");
        }
        else if (nbRightClickInARow > 8)
        {
            InitPopUP("Awsome !", "progression");
        }


        if (playerColor == "blue")
        {
            panelBlue.SetActive(true);
        }
        else if(playerColor == "red")
        {
            panelRed.SetActive(true);
        }
        else if (playerColor == "purple")
        {
            panelPurple.SetActive(true);
        }
        else if (playerColor == "yellow")
        {
            panelYellow.SetActive(true);
        }
    }

    private void IncreaseSpeed()
    {
        if (gameManager.endTimer)
        {
            timeDifficulty += Time.deltaTime;
            if (timeDifficulty >= 10)
            {
                timeDifficulty = 0f;
                LevelUp();
            }
        }
    }

    private void LevelUp()
    {
        if (speed < 450f) speed += 15f;
    }

    private void LineRenderer()
    {
        LineRenderer lr = GetComponent<LineRenderer>();
        Vector3[] positions = new Vector3[] { transform.position, nearestPlanet.transform.position };
        lr.SetPositions(positions);
        lr.material = nearestPlanet.transform.parent.GetChild(0).GetComponent<PlanetMotor>().myMaterial;
        lr.startWidth = 2f;
        lr.endWidth = 5f;
    }

    private void SetParticleAndTrail()
    {
        int particle = SaveManager.Instance.state.activeParticle;
        int trail = SaveManager.Instance.state.activeTrail;

        //setActive false every particles
        foreach (Transform t in particles)
        {
            t.gameObject.SetActive(false);
        }
        //setActive true the right particle
        particles.transform.GetChild(particle).gameObject.SetActive(true);

        //setActive false every trails
        foreach (Transform t in trails)
        {
            t.gameObject.SetActive(false);
        }
        //setActive true the right trailf
        trails.transform.GetChild(trail).gameObject.SetActive(true);
    }

    private void InitPopUP(string text, string type)
    {
        if (type == "up")
        {
            GameObject temp = Instantiate(UpPopText) as GameObject;
            RectTransform tempRect = temp.GetComponent<RectTransform>();
            temp.transform.SetParent(GameObject.Find("GameUI").transform);
            tempRect.transform.localPosition = UpPopText.transform.localPosition;
            tempRect.transform.localScale = UpPopText.transform.localScale;
            temp.GetComponent<Text>().text = "x " + text;
            Destroy(temp, 1);
        }
        else if (type == "down")
        {
            GameObject temp = Instantiate(DownPopText) as GameObject;
            RectTransform tempRect = temp.GetComponent<RectTransform>();
            temp.transform.SetParent(GameObject.Find("GameUI").transform);
            tempRect.transform.localPosition = DownPopText.transform.localPosition;
            tempRect.transform.localScale = DownPopText.transform.localScale;
            temp.GetComponent<Text>().text = "x " + text;
            Destroy(temp, 1);
        }
        else if (type == "transition")
        {
            GameObject temp = Instantiate(TransitionPopText) as GameObject;
            RectTransform tempRect = temp.GetComponent<RectTransform>();
            temp.transform.SetParent(GameObject.Find("GameUI").transform);
            tempRect.transform.localPosition = TransitionPopText.transform.localPosition;
            tempRect.transform.localScale = TransitionPopText.transform.localScale;
            temp.GetComponent<Text>().text = text;
            Destroy(temp, 1);
        }
        else if (type == "visited")
        {
            GameObject temp = Instantiate(VisitedPopText) as GameObject;
            RectTransform tempRect = temp.GetComponent<RectTransform>();
            temp.transform.SetParent(GameObject.Find("GameUI").transform);
            tempRect.transform.localPosition = VisitedPopText.transform.localPosition;
            tempRect.transform.localScale = VisitedPopText.transform.localScale;
            temp.GetComponent<Text>().text = text;
            Destroy(temp, 1);
        }
        else if (type == "progression")
        {
            GameObject temp = Instantiate(ProgressionPopText) as GameObject;
            RectTransform tempRect = temp.GetComponent<RectTransform>();
            temp.transform.SetParent(GameObject.Find("GameUI").transform);
            tempRect.transform.localPosition = ProgressionPopText.transform.localPosition;
            tempRect.transform.localScale = ProgressionPopText.transform.localScale;
            temp.GetComponent<Text>().text = text;
            if (text == "Miss ...")
            {
                temp.GetComponent<Text>().color = Color.red;
            }
            else
            {
                temp.GetComponent<Text>().color = Color.green;
            }
            Destroy(temp, 1);
        }
    }

    public void SetColorYellow()
    {
        playerColor = "yellow";
    }
    public void SetColorRed()
    {
        playerColor = "red";
    }      
    public void SetColorPurple()
    {
        playerColor = "purple";
    }
    public void SetColorBlue()
    {
        playerColor = "blue";
    }
    public void SetColorEmpty()
    {
        playerColor = "";
    }
}
