public class PersistentInfo {

	static PersistentInfo instance;
	public int currentLevel;
	public int volume;
	public Player currentPlayer;

    private PersistentInfo()
    {
        // this is 0 indexed
        currentLevel = 0;
		volume = 100;
    }

	public static PersistentInfo Instance()
    {
        if (instance == null)
            instance = new PersistentInfo();
        return instance;
    }
}
