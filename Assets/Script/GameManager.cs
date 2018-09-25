using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public class GameManager : MonoBehaviour {

    private bool gameHasEnded = false;
    private float showGameOverScreenDelay = 1f;
    public GameObject gameOverPanel;
    public GameObject UIPanel;
    public GameObject pausePanel;

    private Player player;

    public float generalTimer = 60;
    private float totalTime = 0;
    public bool endTimer = false;
    public Text generelTimerText;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void Update()
    {
        GeneralTimer();
    }


    public void GameOver(PlanetMotor p)
    {
        if (gameHasEnded == false)
        {
            //sound effect
            FindObjectOfType<AudioManager>().SetVolume("Theme", (OptionsMenu.volumeMaxValue[1] * SaveManager.Instance.state.volume) / 2);
            FindObjectOfType<AudioManager>().Play("Explosion");
            

            //save manager things
            SaveManager.Instance.state.firstTimePlayed = false;
            SaveManager.Instance.state.nbTimesPlayed++;

            gameHasEnded = true;
            UIPanel.SetActive(false);            
            player.GetComponent<LineRenderer>().enabled = false;
            player.GetComponent<Score>().GameOverScore();
            p.DeathAnimation();                     
            Invoke("ShowGameOverScreen", showGameOverScreenDelay);
        }           
    }
    public void GameOver()
    {

        if (gameHasEnded == false)
        {
            //sound effect
            FindObjectOfType<AudioManager>().SetVolume("Theme", (OptionsMenu.volumeMaxValue[1] * SaveManager.Instance.state.volume) / 2);
            FindObjectOfType<AudioManager>().Play("Explosion");


            //save manager things
            SaveManager.Instance.state.firstTimePlayed = false;
            SaveManager.Instance.state.nbTimesPlayed++;

            gameHasEnded = true;
            UIPanel.SetActive(false);
            player.GetComponent<LineRenderer>().enabled = false;
            player.GetComponent<Score>().GameOverScore();
            Invoke("ShowGameOverScreen", showGameOverScreenDelay);
        }
    }

    private void ShowGameOverScreen()
    {
        //pub
        if (SaveManager.Instance.state.nbTimesPlayed % 7 == 0 && SaveManager.Instance.state.noads == false)
        {
            Advertisement.Show();
        }
        gameOverPanel.SetActive(true);
    }

    public void Replay()
    {
        Time.timeScale = 1f;
        FindObjectOfType<AudioManager>().SetVolume("Theme", OptionsMenu.volumeMaxValue[1] * SaveManager.Instance.state.volume);
        FindObjectOfType<AudioManager>().Stop("Theme");
        FindObjectOfType<AudioManager>().Play("Theme");

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void GeneralTimer()
    {
        string minute;
        string seconde;
        if (endTimer)
        {   
            totalTime += Time.deltaTime;
            if (totalTime >= 1)
            {
                totalTime = 0;
                generalTimer--;

                //text color
                if (generalTimer <= 5)
                {
                    generelTimerText.color = Color.red;
                }
                else
                {
                    generelTimerText.color = Color.magenta;
                }


                //change the timer
                if (generalTimer < 0)
                {
                    player.imDead = true;
                    GameOver(player.nearestPlanet.transform.parent.GetChild(0).GetComponent<PlanetMotor>());
                    return;
                }

                if (generalTimer <= 5)
                {
                    GameObject.FindGameObjectWithTag("GameUI").GetComponent<Animator>().SetBool("BonusTimer", true);
                    FindObjectOfType<AudioManager>().Play("Tick");
                }

                if (generalTimer - 60 >= 0)
                {
                    minute = 1.ToString();
                    seconde = ((int)generalTimer - 60).ToString();
                    if ((int)generalTimer - 60 < 10)
                    {
                        seconde = "0" + ((int)generalTimer - 60).ToString();
                    }
                }
                else
                {
                    minute = 0.ToString();
                    seconde = ((int)generalTimer).ToString();
                    if ((int)generalTimer < 10)
                    {
                        seconde = "0" + ((int)generalTimer).ToString();
                    }
                }

                generelTimerText.text = minute + " : " + seconde;
            }
        }
             
    }

    public void Pause()
    {
        UIPanel.SetActive(false);
        pausePanel.SetActive(true);
        FindObjectOfType<AudioManager>().SetVolume("Theme", (OptionsMenu.volumeMaxValue[1] * SaveManager.Instance.state.volume) / 2);
        Time.timeScale = 0f;
    }

    public void Reprendre()
    {
        UIPanel.SetActive(true);
        pausePanel.SetActive(false);
        FindObjectOfType<AudioManager>().SetVolume("Theme", OptionsMenu.volumeMaxValue[1] * SaveManager.Instance.state.volume);
        Time.timeScale = 1f;
    }

    public void Menu()
    {
        Time.timeScale = 1f;
        FindObjectOfType<AudioManager>().SetVolume("Theme", OptionsMenu.volumeMaxValue[1] * SaveManager.Instance.state.volume);

        SceneManager.LoadScene("Menu");
    }

    public void Quitter()
    {
        Application.Quit();
    }

    public void ClickButtonSound()
    {
        FindObjectOfType<AudioManager>().Play("DownBonus");
    }

    public void ClickTutorial()
    {
        Time.timeScale = 1f;
        player.tutorialIsOn = false;
    }

    public void ClickTutorial2()
    {
        Time.timeScale = 1f;
    }

    public void TutorialPointerUp()
    {
        if (player.nbSecondTurnFirstPlanet < 3f)
        {
            player.LaunchTutorial2();
        }
    }
}
