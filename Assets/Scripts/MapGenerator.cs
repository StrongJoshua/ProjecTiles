using System.IO;
using UnityEngine;

public class MapGenerator : MonoBehaviour {
    public TextAsset map;
    public GameObject[] tiles;
	// Use this for initialization
	void Start () {
        StringReader sr = new StringReader(map.text);
        string line;
        int x = 0, z = 0;
        while((line = sr.ReadLine()) != null)
        {
            foreach(char c in line)
            {
                if (c == ',')
                    continue;
                Instantiate(tiles[(int)char.GetNumericValue(c)], new Vector3(x++, 0, z), Quaternion.identity);
            }
            x = 0;
            z++;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
