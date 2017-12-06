public class PersistentInfo {

	static PersistentInfo instance;
	public int currentLevel;
	public int musicVolume;
	public int sfxVolume;
	public Player currentPlayer;

    private PersistentInfo()
    {
        // this is 0 indexed
        currentLevel = 0;
		musicVolume = 100;
		sfxVolume = 100;
    }

	public static PersistentInfo Instance()
    {
        if (instance == null)
            instance = new PersistentInfo();
        return instance;
    }
}
