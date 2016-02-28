using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapWay {

	public double _id { get; set; }
	public IList<double> _nodesInWay { get; set; }
	public IDictionary<string, IList<string>> _tags {get; set;}

	public MapWay(double id){
		_id = id;
		_nodesInWay = null;
		_tags = null;
	}

	public void LogTags(){
		foreach (KeyValuePair <string,IList<string>> k in _tags){
			string output = "";
			output += k + ": { ";
			foreach (string v in k.Value){
				output += v + " ,";
			}
			output += " }";
			Debug.Log (output);
		}

	}

}
