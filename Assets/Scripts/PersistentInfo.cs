using UnityEngine;

public class PersistentInfo {

	static PersistentInfo instance;
	public int currentLevel;

	private int musicVolume;
    public int MusicVolume
    {
        get { return musicVolume; }
        set { musicVolume = value; PlayerPrefs.SetInt("musicVolume", value); }
    }

	private int sfxVolume;
    public int SFXVolume
    {
        get { return sfxVolume; }
        set { sfxVolume = value; PlayerPrefs.SetInt("sfxVolume", value); }
    }

	public Player currentPlayer;

    private PersistentInfo()
    {
        // this is 0 indexed
        currentLevel = 0;
		musicVolume = PlayerPrefs.GetInt("musicVolume", 100);
		sfxVolume = PlayerPrefs.GetInt("sfxVolume", 100);
    }

	public static PersistentInfo Instance()
    {
        if (instance == null)
            instance = new PersistentInfo();
        return instance;
    }
}
