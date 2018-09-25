using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class ShopManager : MonoBehaviour {

    public Transform particlePanel;
    public Transform trailPanel;

    public GameObject planetMenu;
    public GameObject player;
    private CharacterController cc;

    public Text particleBuySetText;
    public Text trailBuySetText;
    public Text goldText;

    public bool asteroidIsRotating;

    private int[] particleCost = new int[] { 0, 3000, 3000, 3000, 3000, 3000, 3000, 3000, 3000, 3000, 3000, 3000, 3000, 3000, 3000 };
    private int[] trailCost = new int[] { 0, 5000, 6000, 6000, 7000, 6000, 22000, 11000, 11000, 6000, 8000, 18000, 8000, 12000, 11000 };

    private int selectedParticleIndex;
    private int selectedTrailIndex;

    private int activeParticleIndex;
    private int activeTrailIndex;

    void Start () {

        // $$$ TMP
        SaveManager.Instance.UnlockParticle(0);
        SaveManager.Instance.UnlockTrail(0);

        UpdateGoldText();
        InitShop();

        //retrieve character controler
        cc = player.GetComponent<CharacterController>();

        //set player's preferences ( particle & trail )
        OnParticleSelect(SaveManager.Instance.state.activeParticle);
        SetParticle(SaveManager.Instance.state.activeParticle);
        OnTrailSelect(SaveManager.Instance.state.activeTrail);
        SetTrail(SaveManager.Instance.state.activeTrail);

        //make the button bigger for the selected items
        particlePanel.GetChild(SaveManager.Instance.state.activeParticle).GetComponent<RectTransform>().localScale = Vector3.one * 1.125f;
        trailPanel.GetChild(SaveManager.Instance.state.activeTrail).GetComponent<RectTransform>().localScale = Vector3.one * 1.125f;

    }

    void Update () {

        //we are in shop menu
        if (asteroidIsRotating == false)
        {
            cc.Move(-transform.right * 300 * Time.deltaTime);
            if (player.transform.position.x < planetMenu.transform.position.x)
            {
                asteroidIsRotating = true;
            }
        }
        else
        {
            player.transform.RotateAround(planetMenu.transform.position, Vector3.forward, -3.5f);
        }
        
	}

    private void InitShop()
    {   
        //particlePanel
        int i = 0;
        foreach (Transform t in particlePanel){

            int currentIndex = i;
            Button b = t.GetComponent<Button>();
            b.onClick.AddListener(() => OnParticleSelect(currentIndex));

            Image img = t.GetComponent<Image>();
            img.color = SaveManager.Instance.IsParticleOwned(i) ? Color.white : new Color(0.4f, 0.4f, 0.4f);

            Text text = t.GetComponentInChildren<Text>();
            text.color = SaveManager.Instance.IsParticleOwned(i) ? Color.white : new Color(0.8f, 0.8f, 0.8f);
            text.text = SaveManager.Instance.IsParticleOwned(i) ? "Unlocked" : "Price : " + (particleCost[i] / 1000).ToString() + " 000";
            i++;
        }

        //trailPanel
        i = 0;
        foreach (Transform t in trailPanel)
        {
            int currentIndex = i;
            Button b = t.GetComponent<Button>();
            b.onClick.AddListener(() => OnTrailSelect(currentIndex));


            Image img = t.GetComponent<Image>();
            img.color = SaveManager.Instance.IsTrailOwned(i) ? Color.white : new Color(0.4f, 0.4f, 0.4f);

            Text text = t.GetComponentInChildren<Text>();
            text.color = SaveManager.Instance.IsTrailOwned(i) ? Color.white : new Color(0.8f, 0.8f, 0.8f);
            text.text = SaveManager.Instance.IsTrailOwned(i) ? "Unlocked" : "Price : " + (trailCost[i] / 1000).ToString() + " 000";
            i++;
        }
    }

    //set the stuff (in buy set click)
    private void SetParticle(int index)
    {
        //set activ index
        activeParticleIndex = index;
        SaveManager.Instance.state.activeParticle = index;

        //change buy/set button
        particleBuySetText.text = "Active";

        SaveManager.Instance.Save();
    }
    private void SetTrail(int index)
    {
        //set activ index
        activeTrailIndex = index;
        SaveManager.Instance.state.activeTrail = index;

        //change buy/set button
        trailBuySetText.text = "Active";

        SaveManager.Instance.Save();
    }
    public void UpdateGoldText()
    {
        goldText.text = SaveManager.Instance.state.gold.ToString();
    }
    public void SetAsteroidIsRotatingFalse()
    {
        asteroidIsRotating = false;
    }

    //BUTTONS
    //select some stuff (click button)
    private void OnParticleSelect(int currentIndex)
    {

        //if button clicked is already selected, exit
        if (selectedParticleIndex == currentIndex)
            return;

        //make the icon a little more bigger
        particlePanel.GetChild(currentIndex).GetComponent<RectTransform>().localScale = Vector3.one * 1.125f;
        //put the previous one on normal scale;
        particlePanel.GetChild(selectedParticleIndex).GetComponent<RectTransform>().localScale = Vector3.one;


        //Set the particle system on the fake player :)
        foreach (Transform t in player.transform.GetChild(0).GetChild(0))
        {
            t.GetComponent<ParticleSystem>().Clear();
            t.gameObject.SetActive(false);
        }
        player.transform.GetChild(0).GetChild(0).GetChild(currentIndex).gameObject.SetActive(true);


        //set the selected color
        selectedParticleIndex = currentIndex;

        if (SaveManager.Instance.IsParticleOwned(currentIndex))
        {
            //particle is owned
            if(activeParticleIndex == currentIndex)
            {
                particleBuySetText.text = "Active";

            }
            else
            {
                particleBuySetText.text = "Set Active";

            }
        }
        else
        {
            particleBuySetText.text = "Buy : " + particleCost[currentIndex].ToString();
        }
    }
    private void OnTrailSelect(int currentIndex)
    {

        //if button clicked is already selected, exit
        if (selectedTrailIndex == currentIndex)
            return;

        //make the icon a little more bigger
        trailPanel.GetChild(currentIndex).GetComponent<RectTransform>().localScale = Vector3.one * 1.125f;
        //put the previous one on normal scale;
        trailPanel.GetChild(selectedTrailIndex).GetComponent<RectTransform>().localScale = Vector3.one;

        //Set the trail renderer on the fake player :)
        foreach (Transform t in player.transform.GetChild(0).GetChild(1))
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
            t.gameObject.SetActive(false);
        }
        player.transform.GetChild(0).GetChild(1).GetChild(currentIndex).gameObject.SetActive(true);

        selectedTrailIndex = currentIndex;

        if (SaveManager.Instance.IsTrailOwned(currentIndex))
        {
            //trail is owned
            if (activeTrailIndex == currentIndex)
            {
                trailBuySetText.text = "Active";

            }
            else
            {
                trailBuySetText.text = "Set Active";

            }
        }
        else
        {
            trailBuySetText.text = "Buy : " + trailCost[currentIndex].ToString();
        }
    }

    //buy or set the right stuff
    public void OnParticleBuySet()
    {
        if (SaveManager.Instance.IsParticleOwned(selectedParticleIndex))
        {
            //change of particle
            if (selectedParticleIndex != activeParticleIndex)
            {
                //success
                SetParticle(selectedParticleIndex);
                FindObjectOfType<AudioManager>().Play("UpBonus");
            }
            else // same particle
            {
                FindObjectOfType<AudioManager>().Play("ValidatePlanet");
            }
            
        }
        else
        {
            //try to buy the particle
            if (SaveManager.Instance.BuyParticle(selectedParticleIndex, particleCost[selectedParticleIndex]))
            {
                SetParticle(selectedParticleIndex);
                //change the color of the button
                particlePanel.GetChild(selectedParticleIndex).GetComponent<Image>().color = Color.white;
                particlePanel.GetChild(selectedParticleIndex).GetComponentInChildren<Text>().color = Color.white;
                particlePanel.GetChild(selectedParticleIndex).GetComponentInChildren<Text>().text = "Unlocked";
                UpdateGoldText();

                //launch success song
                FindObjectOfType<AudioManager>().Play("UpBonus");
            }
            else
            {
                //not enougth money
                FindObjectOfType<AudioManager>().Play("ValidatePlanet");
            }
        }
    }
    public void OnTrailBuySet()
    {
        if (SaveManager.Instance.IsTrailOwned(selectedTrailIndex))
        {
            //change of particle
            if (selectedTrailIndex != activeTrailIndex)
            {
                //success
                SetTrail(selectedTrailIndex);
                FindObjectOfType<AudioManager>().Play("UpBonus");
            }
            else // same particle
            {
                FindObjectOfType<AudioManager>().Play("ValidatePlanet");
            }
        }
        else
        {

            //try to buy the particle
            if (SaveManager.Instance.BuyTrail(selectedTrailIndex, trailCost[selectedTrailIndex]))
            {
                SetTrail(selectedTrailIndex);
                //change the color of the button
                trailPanel.GetChild(selectedTrailIndex).GetComponent<Image>().color = Color.white;
                trailPanel.GetChild(selectedTrailIndex).GetComponentInChildren<Text>().color = Color.white;
                trailPanel.GetChild(selectedTrailIndex).GetComponentInChildren<Text>().text = "Unlocked";
                UpdateGoldText();

                //launch success song
                FindObjectOfType<AudioManager>().Play("UpBonus");
            }
            else
            {
                //not enougth money
                FindObjectOfType<AudioManager>().Play("ValidatePlanet");
            }
        }
    }

    public void LaunchTurnArroundSound()
    {
        //Invoke("LaunchTurnArroundSound2", 1.2f);
        FindObjectOfType<AudioManager>().Play("TurnArround", true);
    }

    public void StopTurnArroundSound()
    {
        FindObjectOfType<AudioManager>().Stop("TurnArround", true);
    }

    public void ClickButtonSound()
    {
        FindObjectOfType<AudioManager>().Play("DownBonus");
    }

    public void ClickPlaySound()
    {
        FindObjectOfType<AudioManager>().Play("Play");
    }

    public void ShowRewardedVideo()
    {
        ShowOptions options = new ShowOptions();
        options.resultCallback = HandleShowResult;

        Advertisement.Show("rewardedVideo", options);
    }
    void HandleShowResult(ShowResult result)
    {
        if (result == ShowResult.Finished)
        {
            Debug.Log("Video completed - Offer a reward to the player");
            SaveManager.Instance.state.gold += 1200;
            SaveManager.Instance.Save();
            UpdateGoldText();
        }
        else if (result == ShowResult.Skipped)
        {
            Debug.LogWarning("Video was skipped - Do NOT reward the player");

        }
        else if (result == ShowResult.Failed)
        {
            Debug.LogError("Video failed to show");
        }
    }

    public void Buy10000Golds()
    {
        IAPManager.Instance.Buy10000Golds();
    }
    public void Buy25000Golds()
    {
        IAPManager.Instance.Buy25000Golds();
    }
    public void Buy50000Golds()
    {
        IAPManager.Instance.Buy50000Golds();
    }
    public void BuyNoAds()
    {
        IAPManager.Instance.BuyNoAds();
    }

    public void RateApp()
    {
        if (CheckNetworkAvailability())
        {
            Application.OpenURL("market://details?id=com.GiinSenCompany.OrbitRush");
            Invoke("AppIsRated", 2f);
        }
    }

    private void AppIsRated()
    {
        SaveManager.Instance.state.gold += 5000;
        SaveManager.Instance.state.appIsRated = true;
        SaveManager.Instance.Save();
        UpdateGoldText();
        PlaceRealShopButtons();
    }

    public void PlaceRealShopButtons()
    {
        if (SaveManager.Instance.state.appIsRated == true)
        {
            GameObject.Find("Rate").SetActive(false);
            GameObject.Find("10000").GetComponent<RectTransform>().Translate(new Vector3(0, 92, 0));
            GameObject.Find("25000").GetComponent<RectTransform>().Translate(new Vector3(0, 48, 0));
            GameObject.Find("50000").GetComponent<RectTransform>().Translate(new Vector3(0, 20, 0));
        }
    }


    private bool CheckNetworkAvailability()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
