using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public class MainMenu : MonoBehaviour {

    public bool clickPlay = false;
    public Text bestScore;

    public GameObject player;

    public GameObject buttonAds;


    void Start()
    {
        //adds 1671767
        Advertisement.Initialize("1671767", true);

        bestScore.text = PlayerPrefs.GetInt("bestScore").ToString();

        if (SaveManager.Instance.state.noads == true)
        {
            buttonAds.SetActive(false);
        }

        OptionsMenu.SetVolume(SaveManager.Instance.state.volume);

    }

    private void Update()
    {
        ClearPlayerTrailRenderer();
        SetAsteroidPosition();

        if (SaveManager.Instance.state.noads == true)
        {
            buttonAds.SetActive(false);
        }
    }
    public void PlayGame()
    {
        clickPlay = true;       
        //Invoke("LoadSceneGame", 2f);
    }

    public void SetAsteroidPosition()
    {
        player.transform.position = new Vector3(100, 3, 300);
        player.transform.eulerAngles = Vector3.zero;
    }

    public void ClearPlayerTrailRenderer()
    {
        //trail renderer
        foreach( Transform t in player.transform.GetChild(0).GetChild(1))
        {
            if (t.childCount == 0)
            {
                t.GetComponent<TrailRenderer>().Clear();
            }
            else
            {
                foreach (Transform tt in t)
                {
                    tt.GetComponent<TrailRenderer>().Clear();
                }
            }         
        }

        //particules
        foreach (Transform t in player.transform.GetChild(0).GetChild(0))
        {
            t.GetComponent<ParticleSystem>().Clear();          
        }
    }





}
