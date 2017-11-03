using System.IO;
using UnityEngine;

public class MapGenerator : MonoBehaviour {
    public TextAsset map;
    public GameObject[] tiles;
	// Use this for initialization
	void Start () {
        StringReader sr = new StringReader(map.text);
        string line;
        int x = 0, y = 0;
        while((line = sr.ReadLine()) != null)
        {
            foreach(char c in line)
            {
                if (c == ',')
                    continue;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
