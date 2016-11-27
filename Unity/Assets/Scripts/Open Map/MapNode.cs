using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MapNode
{

    public double _id { get; set; }
    public IDictionary<string, IList<string>> _tags { get; set; }
    public float _lat { get; set; }
    public float _lon { get; set; }

    public IList<MapNode> ConnectedNodes { get; set; }

    public IDictionary<Needs, float> MaxNeeds;
    public IDictionary<Needs, float> NeedAmounts;
    public IDictionary<Needs, float> NearbyNeedAmounts;

    public bool NeedsDepleted = false;

    private Vector3? _locationInUnits;
    public Vector3 LocationInUnits
    {
        get
        {
            if (!_locationInUnits.HasValue)
            {
                updateUnitLocationVectors();
            }
            return (Vector3)_locationInUnits;
        }

        set { _locationInUnits = value; }
    }

    public MapNode(double id, float lat, float lon)
    {
        _id = id;
        _lat = lat;
        _lon = lon;

        _tags = null;

		ConnectedNodes = new List<MapNode>();

        //Set default values for each need
        NeedAmounts = new Dictionary<Needs, float>();
        NearbyNeedAmounts = new Dictionary<Needs, float>();
        MaxNeeds = new Dictionary<Needs, float>();
        foreach (Needs need in System.Enum.GetValues(typeof(Needs)))
        {
            MaxNeeds.Add(need, 0.0f);
            NeedAmounts.Add(need, 0.0f);
            NearbyNeedAmounts.Add(need, 0.0f);
        }
    }

    public void ReplenishNeeds()
    {
        if (NeedsDepleted)
        {
            foreach (KeyValuePair<Needs, float> n in MaxNeeds)
            {
                if (n.Value > 0)
                {
                    MaxNeeds[n.Key] -= 0.005f;
                } else
                {
                    NeedsDepleted = false;
                }
            }
        }
    }

    public void updateUnitLocationVectors()
    {
        LocationInUnits = new Vector3(MapMetaInformation.Instance.MapLatValue(_lat), 0, MapMetaInformation.Instance.MapLonValue(_lon));
    }

    public void AddNeighbouringNode(MapNode neighbour)
    {
        if (ConnectedNodes == null)
        {
            ConnectedNodes = new List<MapNode>();
        }
        //Make sure this neighbour isn't already in the list.
        foreach (MapNode n in ConnectedNodes)
        {
            if (n._id == neighbour._id)
            {
                return;
            }
        }
        ConnectedNodes.Add(neighbour);
    }

    public void PropogateNeedsToNeighbours(Dictionary<Needs, float> valuesToPropogate)
    {
        foreach (KeyValuePair<Needs, float> pair in valuesToPropogate)
        {
            NearbyNeedAmounts[pair.Key] += pair.Value;

            //TODO: This is gross and could be changed elsewhere I think. Look into how to recify this.
            if (NearbyNeedAmounts[pair.Key] + NeedAmounts[pair.Key] > 1.0f)
            {
                NearbyNeedAmounts[pair.Key] = 1.0f - NeedAmounts[pair.Key];
            }
        }
    }

    public void SetWaterBasedOnTags()
    {
        foreach (KeyValuePair<string, IList<string>> entry in _tags)
        {
            foreach (string v in entry.Value)
            {
                foreach (string x in MapMetaInformation.Instance.WaterTags)
                {
                    if (v.Contains(x))
                    {
                        NeedAmounts[Needs.Water] += 0.2f;
                    }
                }
            }
        }
        MaxNeeds[Needs.Water] = NeedAmounts[Needs.Water];
        if (NeedAmounts[Needs.Water] > 0)
        {
            Debug.Log("Water: " + NeedAmounts[Needs.Water]);
        }
    }

    public void SetFoodBasedOnTags()
    {
        foreach (KeyValuePair<string, IList<string>> entry in _tags)
        {
            foreach (string v in entry.Value)
            {
                foreach (string x in MapMetaInformation.Instance.FoodTags)
                {
                    if (v.Contains(x))
                    {
                        NeedAmounts[Needs.Food] += 0.05f;
                    }
                }
            }
        }
        MaxNeeds[Needs.Food] = NeedAmounts[Needs.Food];
        if (NeedAmounts[Needs.Food] > 0)
        {
            Debug.Log("Food: " + NeedAmounts[Needs.Food]);
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
                        NeedAmounts[Needs.Shelter] += 0.2f;
                    }
                }
            }
        }
        MaxNeeds[Needs.Shelter] = NeedAmounts[Needs.Shelter];
        if (NeedAmounts[Needs.Shelter] > 0)
        {
            Debug.Log("Shelter: " + NeedAmounts[Needs.Shelter]);
        }
    }

    public Dictionary<Needs, MapNode> GetHighestNeedNeighbours()
    {
        var returned = new Dictionary<Needs, MapNode>();

        foreach (Needs n in Enum.GetValues(typeof(Needs)))
        {
            returned[n] = ConnectedNodes[0];
        }

        foreach (MapNode node in ConnectedNodes)
        {
            foreach (KeyValuePair<Needs, MapNode> n in returned)
            {
                if (node.NeedAmounts[n.Key] > n.Value.NeedAmounts[n.Key])
                {
                    returned[n.Key] = node;
                }
            }
        }

        return returned;
    }

    public MapNode GetRandomNeighbour()
    {
		if (ConnectedNodes != null && ConnectedNodes.Count > 1)
        {
            System.Random rnd = new System.Random();
            int i = rnd.Next(ConnectedNodes.Count);
			//Debug.LogError ("Index selected: "+i + "of "+ConnectedNodes.Count);
            return ConnectedNodes[i];
        }
        return null;
    }

    public void LogTags()
    {
        foreach (KeyValuePair<string, IList<string>> entry in _tags)
        {
            foreach (string v in entry.Value)
            {
                Debug.Log(entry.Key + ": " + v);
            }
        }
    }

    public void LogTags(string searchterm)
    {
        foreach (KeyValuePair<string, IList<string>> entry in _tags)
        {
            foreach (string v in entry.Value)
            {
                if (entry.Key.Contains(searchterm) || v.Contains(searchterm))
                {
                    Debug.Log(entry.Key + ": " + v);
                }
            }
        }
    }

    public void LogNeighbourNodes()
    {
        string output = "";
        output += _id + ": ";
        foreach (MapNode d in ConnectedNodes)
        {
            output += d._id + ", ";
        }
        Debug.Log(output);
    }

    public void ConsumeNeeds()
    {
        //NeedsDepleted = true;
      foreach (var n in MaxNeeds)
        {
            //MaxNeeds[n.Key] += 0.01f;
        }
    }
}
