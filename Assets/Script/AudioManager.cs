using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

    public Sound[] sounds;
    public static AudioManager instance;

    private Coroutine fadeCoroutine = null;

	void Awake () {

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }		
	}

    private void Start()
    {
        Play("Theme");
    }

    public void Play (string name, bool b = false)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s.name == "TurnArround" && b == false)
        {
            s.source.volume = OptionsMenu.volumeMaxValue[0] * SaveManager.Instance.state.volume;
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
            s.stopFade = true;  
        }
        s.source.Play();
	}

    public void Stop(string name, bool b = false)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s.name == "TurnArround" && b == true)
        {
            s.source.Stop();
            return;
        }
        if (s.name == "TurnArround")
        {
            s.stopFade = false;
            fadeCoroutine = StartCoroutine(FadeOut(s, .6f));
        }        
    }

    public static IEnumerator FadeOut(Sound audioSource, float FadeTime)
    {
        float startVolume = audioSource.source.volume;
        while (audioSource.source.volume > 0)
        {
            audioSource.source.volume -= startVolume * Time.deltaTime / FadeTime;
            yield return null;
        }

        //audioSource.stopFade = false;
        audioSource.source.volume = startVolume;
        audioSource.source.Stop();        
    }

    public void SetVolume(string name, float volume)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.volume = volume;
    }

    public void SetPitch(string name, float pitch)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.pitch = pitch;
    }
}
