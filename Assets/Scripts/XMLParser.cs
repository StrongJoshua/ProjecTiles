using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

public class XMLParser  {

	private XmlDocument xmlDoc;


	public XMLParser() {
		xmlDoc = new XmlDocument ();
		string filePath = Application.dataPath + "/Xmls/Stats.xml";
		xmlDoc.Load (filePath);
	}

}
