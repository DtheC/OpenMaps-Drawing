using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapNode {
	
	public double _id {get; set;}
	public IDictionary<string, IList<string>> _tags {get; set;}
	public float _lat {get; set;}
	public float _lon {get; set;}

	public IList<MapNode> _connectedNodes { get; set; }

	public float AmountOfWater = 0;
	public float AmountOfFood = 0;
	public float NearbyAmountOfFood = 0;

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


	public void PropogateNeedsToNeighbours(){
		if (_connectedNodes != null) {
			foreach (MapNode neighbour in _connectedNodes) {
				neighbour.PropogateNeedsToNeighbours (AmountOfFood);
			}
		}
	}

	//TODO This should recieve a dictionary of Needs and assocaited values rather than just food
	public void PropogateNeedsToNeighbours(float topropogate){
		NearbyAmountOfFood += topropogate; 
		if (NearbyAmountOfFood+AmountOfFood > 1.0f){
			NearbyAmountOfFood = 1.0f-AmountOfFood;
		}
	}

	//TODO Change based on the need send associated with the to be created dictionary of current need values
	public void AddToNeed(Needs need, float value){
		NearbyAmountOfFood += value / 100.0f; //TODO This arbitrary value should be a variable somewhere.
		if (NearbyAmountOfFood+AmountOfFood > 1.0f){
			NearbyAmountOfFood = 1.0f-AmountOfFood;
		}
	}

	public void SetWaterBasedOnTags(){
		AmountOfWater = 0f;
		foreach (KeyValuePair<string, IList<string>> entry in _tags) {
			foreach (string v in entry.Value) {
				if (v.Contains ("drinking_water")) {
					AmountOfWater = 1.0f;
				}
			}
		}
	}

	public void SetFoodBasedOnTags(){
		AmountOfFood = 0f;
		foreach (KeyValuePair<string, IList<string>> entry in _tags) {
			foreach (string v in entry.Value) {
				if (v.Contains ("restaurant")) {
					AmountOfFood = 0.1f;

				}
			}
		}
	}

	public MapNode GetRandomNeighbour(){
		if (_connectedNodes != null){
			System.Random rnd = new System.Random();
			int i = rnd.Next(_connectedNodes.Count);
			return _connectedNodes[i];
		}
		return null;
	}

	public void LogTags(){
		foreach (KeyValuePair<string, IList<string>> entry in _tags) {
			foreach (string v in entry.Value) {
				Debug.Log (entry.Key + ": " + v);
			}
		}
	}

	public void LogTags(string searchterm){
		foreach (KeyValuePair<string, IList<string>> entry in _tags) {
			foreach (string v in entry.Value) {
				if (entry.Key.Contains (searchterm) || v.Contains (searchterm)) {
					Debug.Log (entry.Key + ": " + v);
				}
			}
		}
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
