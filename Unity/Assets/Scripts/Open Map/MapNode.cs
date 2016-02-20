using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapNode {
	
	private double _id {get; set;}
	private IDictionary<string, IList<string>> _tags {get; set;}
	private float _lat {get; set;}
	private float _lon {get; set;}

	private Vector3 _latInUnits {get; set;}
	private Vector3 _lonInUnits {get; set;}
	
	public MapNode(double id, float lat, float lon){
		_id = id;
		_lat = lat;
		_lon = lon;
	}

}
