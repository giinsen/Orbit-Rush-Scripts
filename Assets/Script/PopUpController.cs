using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpController : MonoBehaviour {

    //https://www.youtube.com/watch?v=N1zHC6vSGLI
    public PopUpScript popupText;
    private static GameObject canvas;

    public static void Initialize()
    {
        canvas = GameObject.Find("Canvas");
    }

    public static void CreatingPopUpText(string text, Transform location)
    {
        //PopUpScript instance = Instantiate(popupText);
        //instance.transform.SetParent(canvas.transform, false);
        //instance.SetText(text);
    }
}
