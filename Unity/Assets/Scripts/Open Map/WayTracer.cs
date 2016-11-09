using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EntityNeeds
{
    private EntityNeed FoodNeed;
    private EntityNeed WaterNeed;
    private EntityNeed ShelterNeed;

    private Needs _highestNeed = Needs.Food;
    private Needs _lowestNeed = Needs.Food;

    private IDictionary<Needs, EntityNeed> needs;

    public Needs LowestNeed
    {
        get
        {
            return _lowestNeed;
        }

        set
        {
            _lowestNeed = value;
        }
    }

    public EntityNeeds()
    {
        needs = new Dictionary<Needs, EntityNeed>();
        needs.Add(Needs.Food, new EntityNeed(0.0f));
        needs.Add(Needs.Water, new EntityNeed(0.0f));
        needs.Add(Needs.Shelter, new EntityNeed(0.0f));
    }

    public float GetNeedValue(Needs need)
    {
        return needs[need].GetValue();
    }

    public void AddToEntityNeed(Needs need, float value)
    {
        needs[need].AddValue(value);
        if (needs[_highestNeed].GetValue() < needs[need].GetValue())
        {
            _highestNeed = need;
        }
        if (needs[_lowestNeed].GetValue() > needs[need].GetValue())
        {
            _lowestNeed = need;
        }
    }

    public void SetNeedsFromNode(IDictionary<Needs, float> nodeNeeds)
    {
        foreach (KeyValuePair<Needs, float> n in nodeNeeds)
        {
            AddToEntityNeed(n.Key, n.Value);
        }
    }

    public void UseNeeds() //Should eventually correspond with evolved values for how quickly it uses energy/food/water etc.
    {
        foreach (KeyValuePair<Needs, EntityNeed> n in needs)
        {
            n.Value.AddValue(-0.0001f);
        }
    }
}
public class EntityNeed
{
    private float needValue = 0.0f;

    public EntityNeed(float startingValue)
    {
        needValue = startingValue;
    }

    public void AddValue(float value)
    {
        needValue += value;
        if (needValue > 1.0f)
        {
            needValue = 1.0f;
        }

        if (needValue < 0)
        {
            needValue = 0;
        }
    }

    public float GetValue()
    {
        return needValue;
    }
}

public class WayTracer : MonoBehaviour
{
    public EntityNeeds EntitiesNeeds = new EntityNeeds();
    private WaytracerMovement _tracerMovement;

    public float speedOfMovement = 2.0f;
	public float triggerDistance = 0.1f;

	private MapController _mapController;
	private WayTracerEmitter _parentEmitter;

	private MapNode _currentMapNode;
	private MapNode _travellingToMapNode;

	[SerializeField] 
	private double _currentMapNodeId;
	private double _travellingToMapNodeId;

    private Material _entityColour;
    private TrailRenderer _entityTrail;

    public MapNode TravellingToMapNode
    {
        get
        {
            return _travellingToMapNode;
        }

        set
        {
            _travellingToMapNode = value;
        }
    }

    public WayTracerEmitter ParentEmitter
    {
        get
        {
            return _parentEmitter;
        }

        set
        {
            _parentEmitter = value;
        }
    }

    public void Init (MapController _m, WayTracerEmitter _w, WaytracerMovement _move)
	{
		_mapController = _m;
		_parentEmitter = _w;
        _tracerMovement = _move;

		_currentMapNode = null;
		_travellingToMapNode = null;

        _entityColour = GetComponent<Renderer>().material;
        _entityTrail = GetComponent<TrailRenderer>();

		//Assign random starting location
		GetRandomStartingNode ();
		//Assign a connected node to move toward
		GetNextConnection ();
	}

	void Update ()
	{
		MoveTowardNewLocation ();
        if (Vector3.Distance(transform.position, _travellingToMapNode.LocationInUnits) < triggerDistance)
        {
            _currentMapNode = _travellingToMapNode;
            GetNextConnection();
        }
        EntitiesNeeds.UseNeeds();
	}

	void MoveTowardNewLocation ()
	{
        _tracerMovement.MoveTowardNewLocation();
	}

	void GetRandomStartingNode ()
	{
		_currentMapNode = _parentEmitter.GetRandomRoadNode ();
		if (_currentMapNode != null) {
			_currentMapNodeId = _currentMapNode._id;
		}
	}

	void GetNextConnection ()
	{
        //Get all the connection nodes and find which has the highest value of each need
        var highestNeedNodes = _currentMapNode.GetHighestNeedNeighbours();
        
        //Add the values of the current location to the Entities' needs
        EntitiesNeeds.SetNeedsFromNode(_currentMapNode.NeedAmounts);
        EntitiesNeeds.SetNeedsFromNode(_currentMapNode.NearbyNeedAmounts);
        //Update the colour based on the needs
        UpdateColour();

        //_travellingToMapNode = highestNeedNodes[EntitiesNeeds.LowestNeed];

        _travellingToMapNode = _currentMapNode.GetRandomNeighbour();

		Debug.Assert (_travellingToMapNode != null);

		if (_travellingToMapNode != null) {
			_travellingToMapNodeId = _travellingToMapNode._id;
		} else {
			_travellingToMapNode = _currentMapNode;
		}
	}

    void UpdateColour()
    {
        _entityColour.color = new Color(EntitiesNeeds.GetNeedValue(Needs.Food), EntitiesNeeds.GetNeedValue(Needs.Shelter), EntitiesNeeds.GetNeedValue(Needs.Water));
        Color trailCol = Color.Lerp(_entityTrail.material.GetColor("_TintColor"), _entityColour.color, 0.1f);
        _entityTrail.material.SetColor("_TintColor", trailCol);
    }
}
