using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapNode {
	
	public double _id {get; set;}
	public IDictionary<string, IList<string>> _tags {get; set;}
	public float _lat {get; set;}
	public float _lon {get; set;}

	public IList<MapNode> _connectedNodes { get; set; }

	private Vector3? _locationInUnits;
	public Vector3 LocationInUnits 
	{
		get {
			if (!_locationInUnits.HasValue) {
				updateUnitLocationVectors ();
			}
			return (Vector3) _locationInUnits;
			}

		set { _locationInUnits = value; }
	}

	
	public MapNode(double id, float lat, float lon){
		_id = id;
		_lat = lat;
		_lon = lon;

		_tags = null;
	}

	public void updateUnitLocationVectors(){
		LocationInUnits = new Vector3(MapMetaInformation.Instance.MapLatValue(_lat), 0, MapMetaInformation.Instance.MapLonValue(_lon));
	}

	public void AddNeighbouringNode(MapNode neighbour){
		if (_connectedNodes == null) {
			_connectedNodes = new List<MapNode>();
		}
		//Make sure this neighbour isn't already in the list.
		foreach (MapNode n in _connectedNodes) {
			if (n._id == neighbour._id){
				return;
			}
		}
		_connectedNodes.Add (neighbour);
	}

	public MapNode GetRandomNeighbour(){
		if (_connectedNodes != null){
			System.Random rnd = new System.Random();
			int i = rnd.Next(_connectedNodes.Count);
			return _connectedNodes[i];
		}
		return null;
	}

	public void LogNeighbourNodes(){
		string output = "";
		output += _id + ": ";
		foreach (MapNode d in _connectedNodes) {
			output += d._id + ", ";
		}
		Debug.Log (output);
	}
}
