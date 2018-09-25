using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {

    public static float[] volumeMaxValue = { .5f, .5f, .5f, 1f, .25f, .7f, 1f, 1f, 1f };

	public static void SetVolume(float volume)
    {
        FindObjectOfType<AudioManager>().SetVolume("TurnArround", volumeMaxValue[0] * volume);
        FindObjectOfType<AudioManager>().SetVolume("Theme", volumeMaxValue[1] * volume);
        FindObjectOfType<AudioManager>().SetVolume("UpBonus", volumeMaxValue[2] * volume);
        FindObjectOfType<AudioManager>().SetVolume("DownBonus", volumeMaxValue[3] * volume);
        FindObjectOfType<AudioManager>().SetVolume("ValidatePlanet", volumeMaxValue[4] * volume);
        FindObjectOfType<AudioManager>().SetVolume("Explosion", volumeMaxValue[5] * volume);
        FindObjectOfType<AudioManager>().SetVolume("Play", volumeMaxValue[6] * volume);
        FindObjectOfType<AudioManager>().SetVolume("Tick", volumeMaxValue[7] * volume);
        FindObjectOfType<AudioManager>().SetVolume("Go", volumeMaxValue[8] * volume);

        SaveManager.Instance.state.volume = volume;
        SaveManager.Instance.Save();
    }

    public void SetVolumeOptions(float volume)
    {
        SetVolume(volume);
    }

    public void SetSlider(Slider slider)
    {
       slider.value = SaveManager.Instance.state.volume;
    }


    public static void SetLineRenderer(bool b)
    {
        SaveManager.Instance.state.lineRendererIsOn = b;
        SaveManager.Instance.Save();
    }

    public void SetLineRendererOptions(bool b)
    {
        SetLineRenderer(b);
    }

    public void SetToggle(Toggle toggle)
    {
        toggle.isOn = SaveManager.Instance.state.lineRendererIsOn;
    }

}
