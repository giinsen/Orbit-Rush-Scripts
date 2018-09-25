using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraMenuMotor : MonoBehaviour {

    private bool alreadyClick = false;
    private GameObject mn;
    private float animationDuration = 3f;
    private float transition = 0f;

    public CanvasGroup element;

    private Vector3 offset;
    private Vector3 set;

    // Use this for initialization
    void Start () {
        mn = GameObject.FindGameObjectWithTag("MainMenu");
        offset = transform.position;
        set = offset + Vector3.forward * 7000;
        //element = GetComponent<CanvasGroup>();
	}
	
	// Update is called once per frame
	void Update () {

		if (mn.GetComponent<MainMenu>().clickPlay == true) {    
            
            transform.position = Vector3.Lerp(offset, set, transition);
            transition += Time.deltaTime / animationDuration;
            if (alreadyClick == false)
            {
                alreadyClick = true;
                StartCoroutine(FadeCanvas(element, element.alpha, 1f));            
            }           
        }
	}

    private IEnumerator FadeCanvas(CanvasGroup cg, float start, float end, float lerpTime = 3f)
    {
        float timeStartedLerping = Time.time;
        float timeSinceStarted = Time.time - timeStartedLerping;
        float pourcentageComplete = timeSinceStarted / lerpTime;
        
        while (true)
        {
            timeSinceStarted = Time.time - timeStartedLerping;
            pourcentageComplete = timeSinceStarted / lerpTime;
            float currentValue = Mathf.Lerp(start, end, pourcentageComplete);
            cg.alpha = currentValue;
            if (pourcentageComplete >= 1) break;

            yield return new WaitForEndOfFrame();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
