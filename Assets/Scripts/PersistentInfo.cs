public class PersistentInfo {

	static PersistentInfo instance;
	public int currentLevel;
	public Player currentPlayer;

    private PersistentInfo()
    {
        // this is 0 indexed
        currentLevel = 0;
    }

	public static PersistentInfo Instance()
    {
        if (instance == null)
            instance = new PersistentInfo();
        return instance;
    }
}
