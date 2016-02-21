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

}
