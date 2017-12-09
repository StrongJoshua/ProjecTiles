using UnityEngine;

public class PersistentInfo {

	static PersistentInfo instance;
	public int currentLevel;

	private float musicVolume;
    public float MusicVolume
    {
        get { return musicVolume; }
        set { musicVolume = value; PlayerPrefs.SetFloat("musicVolume", value); }
    }

	private float sfxVolume;
    public float SFXVolume
    {
        get { return sfxVolume; }
        set { sfxVolume = value; PlayerPrefs.SetFloat("sfxVolume", value); }
    }

	public Player currentPlayer;

    private PersistentInfo()
    {
        // this is 0 indexed
        currentLevel = 0;
		musicVolume = PlayerPrefs.GetFloat("musicVolume", 100);
		sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 100);
    }

	public static PersistentInfo Instance()
    {
        if (instance == null)
            instance = new PersistentInfo();
        return instance;
    }
}
