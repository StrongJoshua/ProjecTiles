using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

public class XMLParser  {

	private XmlDocument xmlDoc;
	public Dictionary<string, float> growthRates;

	public XMLParser(TextAsset statsXML) {
		xmlDoc = new XmlDocument ();
        xmlDoc.LoadXml(statsXML.text);

		growthRates = getBaseStats ("GrowthRates");
	}

	public Dictionary<string, float> getBaseStats(string unitName) {
		XmlNodeList units = xmlDoc.GetElementsByTagName (unitName);
		Dictionary<string, float> statsDic = new Dictionary<string, float> ();

		if (units != null) {
			foreach (XmlNode unit in units) {
				XmlNodeList stats = unit.ChildNodes;

				foreach (XmlNode stat in stats) {
					statsDic.Add (stat.Name, float.Parse (stat.InnerText));

					if(stat.Attributes.Count > 0) {
						float growth = float.Parse (stat.Attributes ["growth"].Value);
						statsDic.Add (stat.Name + "Growth", growth);
					}
				}
			}
		}

		return statsDic;
	}

	public Dictionary<string, float> getStatsAtLevel(string unitName, float level) {
		XmlNodeList units = xmlDoc.GetElementsByTagName (unitName);
		Dictionary<string, float> statsDic = new Dictionary<string, float> ();

		if (units != null) {
			foreach (XmlNode unit in units) {
				XmlNodeList stats = unit.ChildNodes;

				foreach (XmlNode stat in stats) {
					float growthRate = growthRates [stat.Name];
					statsDic.Add (stat.Name, statAtLevelOptimal(float.Parse (stat.InnerText), level, growthRate));
					statsDic.Add (stat.Name + "Growth", float.Parse (stat.Attributes ["growth"].Value));
				}
			}
		}

		return statsDic;
	}

	public float statAtLevelOptimal(float stat, float level, float growthRate) {
		return stat * Mathf.Pow ((1 + growthRate), level);
	}

}
