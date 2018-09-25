using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {

    public GameObject panelBlue;
    public GameObject panelRed;
    public GameObject panelYellow;
    public GameObject panelPurple;

    public void BonusTimerAnimationFalse()
    {
        GameObject.FindGameObjectWithTag("GameUI").GetComponent<Animator>().SetBool("BonusTimer", false);
    }

    public void DesactiveAllPanel()
    {
        panelBlue.SetActive(false);
        panelRed.SetActive(false);
        panelYellow.SetActive(false);
        panelPurple.SetActive(false);
    }
}
