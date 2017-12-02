using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

public class XMLSaver  {

	private XmlDocument xmlDoc;


	public XMLSaver() {
		xmlDoc = new XmlDocument ();
		string filePath = Application.dataPath + "/Xmls/Player_Info.xml";
		xmlDoc.Load (filePath);

	}

	public void writeToUnits() {
		
	}


}


