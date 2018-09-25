using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour {

	public static SaveManager Instance { set; get; }
    public SaveState state;

    private void Awake()
    {

        //ResetSave();


        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Load();

        OptionsMenu.SetVolume(state.volume);
        OptionsMenu.SetLineRenderer(state.lineRendererIsOn);
    }

    public void Save()
    {
        PlayerPrefs.SetString("save",Helper.Serialize<SaveState>(state));
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("save"))
        {
            state = Helper.Deserialize<SaveState>(PlayerPrefs.GetString("save"));
        }
        else
        {
            state = new SaveState();
            Save();
            Debug.Log("No save file found, creating a new one!");
        }
    }

    //check if stuff is owned
    public bool IsParticleOwned(int index)
    {
        return (state.particleOwned & (1 << index)) != 0;
    }
    public bool IsTrailOwned(int index)
    {
        return (state.trailOwned & (1 << index)) != 0;
    }

    //try buy stuff
    public bool BuyParticle(int index, int cost)
    {
        if (state.gold >= cost)
        {
            state.gold -= cost;
            UnlockParticle(index);
            Save();
            return true;
        }
        else
        {
            //not enougth money
            return false;
        }
    }
    public bool BuyTrail(int index, int cost)
    {
        if (state.gold >= cost)
        {
            state.gold -= cost;
            UnlockTrail(index);
            Save();
            return true;
        }
        else
        {
            //not enougth money
            return false;
        }
    }

    //unlock stuff
    public void UnlockParticle(int index)
    {
        state.particleOwned |= 1 << index;
    }
    public void UnlockTrail(int index)
    {
        state.trailOwned |= 1 << index;
    }

    //reset save file
    public void ResetSave()
    {
        PlayerPrefs.DeleteKey("save");
    }
}
