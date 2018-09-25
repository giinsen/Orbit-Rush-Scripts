using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {

    private float score = 0f;
    public Text scoreText;
    private float gameOverScore;
    public Text gameOverText;
    public Text bestGameOverText;
    public Text gameOverGoldtext;

    public Text multiplicateurPlanetCounterText;
    private float multiplicateurPlanetCounter = 1f;
    private float nbPlanetVisited = 0f;

    public Text multiplicateurColorCounterText;
    private float multiplicateurColorCounter = 1f;
    //private float nbGoodColor = 0;
    public Text multiplicateurTotal;
    public Text pointsParSeconde;

    private Player player;

    private GameManager gameManager;

    private void Start()
    {
        player = GetComponent<Player>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update () {

        PlanetMotor myPlanet = player.nearestPlanet.transform.parent.GetChild(0).GetComponent<PlanetMotor>();

        if (gameManager.endTimer)
        {
            
            scoreText.text = (System.Math.Round(score)).ToString();
            multiplicateurPlanetCounterText.text = "x " + (System.Math.Round(multiplicateurPlanetCounter, 1)).ToString();
            multiplicateurColorCounterText.text = "x " + multiplicateurColorCounter.ToString();
            float total = multiplicateurPlanetCounter * multiplicateurColorCounter;
            multiplicateurTotal.text = "x " + (System.Math.Round(total, 1)).ToString();

            if (player.nbPlanetVisited > nbPlanetVisited)
            {
                nbPlanetVisited = player.nbPlanetVisited;

                if (myPlanet.haveRing)
                {
                    multiplicateurPlanetCounter += 0.3f;
                }
                else
                {
                    multiplicateurPlanetCounter += 0.1f;
                }

            }

            multiplicateurColorCounter = (int)((player.nbGoodColor+4)/4);

            pointsParSeconde.text = ((int)(10 * multiplicateurPlanetCounter * multiplicateurColorCounter)).ToString() + " / s";
            score += Time.deltaTime * 10 * multiplicateurPlanetCounter * multiplicateurColorCounter;
        }            
    }

    public void GameOverScore()
    {
        double finalScore = System.Math.Round(score);
        gameOverText.text = "Score : " + finalScore.ToString();
        gameOverScore = (int)System.Math.Round(score);

        if (gameOverScore > PlayerPrefs.GetInt("bestScore"))
        {
            PlayerPrefs.SetInt("bestScore", (int)gameOverScore);
        }
        bestGameOverText.text = "Meilleur Score : " + PlayerPrefs.GetInt("bestScore").ToString();

        int nbGolds = (int)(gameOverScore / 20);
        gameOverGoldtext.text = nbGolds.ToString();
        SaveManager.Instance.state.gold += nbGolds;
        SaveManager.Instance.Save();
    }

}
