using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraMotor : MonoBehaviour {

    private Transform nearestPlanet;
    private Transform playerTransform;
    private GameObject playerGameObject;
    private bool inRotation;
    private Vector3 smoothedPosition;
    private Vector3 verySmoothedPosition;
    private Vector3 moveVector;
    private float smoothSpeed = 0.125f;
    private float verySmoothSpeed = 0.035f;

    private float transition = 0f;
    private float animationDuration = 4f;

    private double timer;
    private double tick;

    private double myTimer;
    public Text timerText;

    private Vector3 PostionAnimationOffset = new Vector3(-52, -6, -200);
    private Vector3 PositionAnimationSet = new Vector3(-52, -6, -500);
    private Vector3 RotationAnimationOffset = new Vector3(0, 80, 0);
    private Vector3 RotationAnimationSet = new Vector3(0, 0, 0);

    private float timeShake = 2f;



    void LateUpdate () {

        playerGameObject = GameObject.FindGameObjectWithTag("Player");
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        if (playerGameObject.GetComponent<Player>().imDead == true)
        {
            GameObject.FindGameObjectWithTag("Asteroid").transform.localScale = new Vector3(1, 1, 1);
            nearestPlanet = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().nearestPlanet.transform;
            playerGameObject.transform.position = new Vector3(nearestPlanet.position.x, nearestPlanet.position.y, 0);           
            
            if (playerGameObject.GetComponent<Player>().deadBySun == true)
            {
                moveVector.x = playerGameObject.GetComponent<Player>().myDeadlySun.transform.position.x;
                moveVector.y = playerGameObject.GetComponent<Player>().myDeadlySun.transform.position.y;
                moveVector.z = -900;
                playerGameObject.transform.position = new Vector3(playerGameObject.GetComponent<Player>().myDeadlySun.transform.position.x, playerGameObject.GetComponent<Player>().myDeadlySun.transform.position.y, 0);
            }
            else
            {
                moveVector.x = nearestPlanet.position.x;
                moveVector.y = nearestPlanet.position.y + 10;
                moveVector.z = -200;
                playerGameObject.transform.position = new Vector3(nearestPlanet.position.x, nearestPlanet.position.y, 0);
            }

            //cameraShake
            timeShake -= Time.deltaTime;
            if (timeShake < 0)
                timeShake = 0;
            moveVector.x += (Random.Range(-100 , 100) * timeShake);
            moveVector.y += (Random.Range(-100 , 100) * timeShake);

            //smooooooth
            smoothedPosition = Vector3.Lerp(transform.position, moveVector, smoothSpeed/2);
            transform.position = smoothedPosition;
            return;
        }

        if (transition > 1f)
        {
            //startOffset = transform.position - player.position;
            timerText.text = "";
            nearestPlanet = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().nearestPlanet.transform; //ok
            inRotation = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().inRotation;


            if (inRotation)
            {
                moveVector.x = nearestPlanet.position.x;
                moveVector.y = nearestPlanet.position.y;
                moveVector.z = -600;
            }
            else
            {
                moveVector.x = playerTransform.position.x;
                moveVector.y = playerTransform.position.y;
                moveVector.z = -500;
            }

            smoothedPosition = Vector3.Lerp(transform.position, moveVector, smoothSpeed);
            transform.position = smoothedPosition;

            if (inRotation)
            {
                moveVector.x = playerTransform.position.x;
                moveVector.y = playerTransform.position.y;
                moveVector.z = -600;
                verySmoothedPosition = Vector3.Lerp(transform.position, moveVector, verySmoothSpeed);
                transform.position = verySmoothedPosition;
            }
            
        }
        else
        {
            //animation at the start of the game.
            transform.position = Vector3.Lerp(PostionAnimationOffset, PositionAnimationSet, transition);
            transform.eulerAngles = Vector3.Lerp(RotationAnimationOffset, RotationAnimationSet, transition);
            transition += Time.deltaTime * 1 / animationDuration;
            //transform.LookAt(playerTransform.position);

            //timer
            tick += Time.deltaTime;
            timer = 4 - System.Math.Round(tick);

            if (myTimer != timer)
            {
                myTimer = timer;
                if (timer == 0)
                {
                    FindObjectOfType<AudioManager>().Play("Go");
                    timerText.text = "Go !";
                }
                else if (timer == 4)
                {
                    timerText.text = "";
                }
                else
                {
                    timerText.text = timer.ToString();
                    FindObjectOfType<AudioManager>().Play("Tick");
                }
            }       
        }        
    }
}
