using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentInfo {

	static PersistentInfo instance;
	public int currentLevel;
	public Player currentPlayer;

    private PersistentInfo()
    {
        currentLevel = 0;
    }

	public static PersistentInfo Instance()
    {
        if (instance == null)
            instance = new PersistentInfo();
        return instance;
    }
}
