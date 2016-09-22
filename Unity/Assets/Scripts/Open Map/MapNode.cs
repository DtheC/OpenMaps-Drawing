using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapNode {
	
	public double _id {get; set;}
	public IDictionary<string, IList<string>> _tags {get; set;}
	public float _lat {get; set;}
	public float _lon {get; set;}

	public IList<MapNode> _connectedNodes { get; set; }

	public IDictionary<Needs, float> NeedAmounts;
	public IDictionary<Needs, float> NearbyNeedAmounts;

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

		//Set default values for each need
		NeedAmounts = new Dictionary<Needs, float> ();
		NearbyNeedAmounts = new Dictionary<Needs, float> ();
		foreach (Needs need in System.Enum.GetValues(typeof(Needs))) 
		{
			NeedAmounts.Add (need, 0);
			NearbyNeedAmounts.Add (need, 0);
		}
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
		
	public void PropogateNeedsToNeighbours(Dictionary<Needs, float> valuesToPropogate){
		foreach (KeyValuePair<Needs, float> pair in valuesToPropogate) {
			NearbyNeedAmounts [pair.Key] += pair.Value;

			//TODO: This is gross and could be changed elsewhere I think. Look into how to recify this.
			if (NearbyNeedAmounts [pair.Key] + NeedAmounts [pair.Key] > 1.0f) {
				NearbyNeedAmounts [pair.Key] = 1.0f - NeedAmounts [pair.Key];
			}
		}
	}

	public void SetWaterBasedOnTags(){
        foreach (KeyValuePair<string, IList<string>> entry in _tags)
        {
            foreach (string v in entry.Value)
            {
                foreach (string x in MapMetaInformation.Instance.WaterTags)
                {
                    if (v.Contains(x))
                    {
                        NeedAmounts[Needs.Water] += 0.1f;
                    }
                }
            }
        }
	}

	public void SetFoodBasedOnTags(){
		foreach (KeyValuePair<string, IList<string>> entry in _tags) {
            foreach (string v in entry.Value)
            {
                foreach (string x in MapMetaInformation.Instance.FoodTags)
                {
                    if (v.Contains(x))
                    {
                        NeedAmounts[Needs.Food] += 0.1f;
                    }
                }
			}
		}
	}

    public void SetShelterBasedOnTags()
    {
        foreach (KeyValuePair<string, IList<string>> entry in _tags)
        {
            foreach (string v in entry.Value)
            {
                foreach (string x in MapMetaInformation.Instance.ShelterTags)
                {
                    if (v.Contains(x))
                    {
                        NeedAmounts[Needs.Shelter] += 0.1f;
                    }
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
