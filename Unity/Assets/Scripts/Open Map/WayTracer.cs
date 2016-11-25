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

    private IDictionary<Needs, EntityNeed> _needs;

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

    public IDictionary<Needs, EntityNeed> EntitiesNeeds
    {
        get
        {
            return _needs;
        }

        set
        {
            _needs = value;
        }
    }

    public Needs HighestNeed
    {
        get
        {
            return _highestNeed;
        }

        set
        {
            _highestNeed = value;
        }
    }

    public EntityNeeds()
    {
        _needs = new Dictionary<Needs, EntityNeed>();
        _needs.Add(Needs.Food, new EntityNeed(0.0f));
        _needs.Add(Needs.Water, new EntityNeed(0.0f));
        _needs.Add(Needs.Shelter, new EntityNeed(0.0f));
    }

    public float GetNeedValue(Needs need)
    {
        return _needs[need].GetValue();
    }

    public void CopyEntitiesNeeds(IDictionary<Needs, EntityNeed> needsToCopy)
    {
        foreach (KeyValuePair<Needs, EntityNeed> item in needsToCopy)
        {
            _needs[item.Key].SetValue(needsToCopy[item.Key].GetValue());
        }
    }

    public void SetEntityNeed(Needs need, float value)
    {
        _needs[need].SetValue(value);
        if (_needs[_highestNeed].GetValue() < _needs[need].GetValue())
        {
            _highestNeed = need;
        }
        if (_needs[_lowestNeed].GetValue() > _needs[need].GetValue())
        {
            _lowestNeed = need;
        }
    }

    public void AddToEntityNeed(Needs need, float value)
    {
        _needs[need].AddValue(value);
        if (_needs[_highestNeed].GetValue() < _needs[need].GetValue())
        {
            _highestNeed = need;
        }
        if (_needs[_lowestNeed].GetValue() > _needs[need].GetValue())
        {
            _lowestNeed = need;
        }
    }

    public void SetNeedsFromDictionary(IDictionary<Needs, float> nodeNeeds)
    {
        foreach (KeyValuePair<Needs, float> n in nodeNeeds)
        {
            SetEntityNeed(n.Key, n.Value);
        }
    }

    public void AddNeedsFromDictionary(IDictionary<Needs, float> nodeNeeds)
    {
        foreach (KeyValuePair<Needs, float> n in nodeNeeds)
        {
            AddToEntityNeed(n.Key, n.Value);
        }
    }

    public void UseNeeds() //Should eventually correspond with evolved values for how quickly it uses energy/food/water etc.
    {
        foreach (KeyValuePair<Needs, EntityNeed> n in _needs)
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

    public void SetValue(float value)
    {
        needValue = value;
        if (needValue > 1.0f)
        {
            needValue = 1.0f;
        }

        if (needValue < 0)
        {
            needValue = 0;
        }
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
    
    public float[] InputsForBrain;
    public EntityNeeds EntitiesPreviousNeeds = new EntityNeeds();
    public float[] EntitiesBrainOutputs = new float[4];

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

    public void Init(MapController _m, WayTracerEmitter _w, WaytracerMovement _move)
    {
        _mapController = _m;
        _parentEmitter = _w;
        _tracerMovement = _move;

        InputsForBrain = new float[15] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        _currentMapNode = null;
        _travellingToMapNode = null;

        _entityColour = GetComponent<Renderer>().material;
        _entityTrail = GetComponent<TrailRenderer>();

        //Assign random starting location
        GetRandomStartingNode();
        //Assign a connected node to move toward
        GetNextConnection();
    }

    void Update()
    {
        MoveTowardNewLocation();
        if (Vector3.Distance(transform.position, _travellingToMapNode.LocationInUnits) < triggerDistance)
        {
            _currentMapNode = _travellingToMapNode;
            GetNextConnection();
        }
        EntitiesNeeds.UseNeeds();
    }

    void MoveTowardNewLocation()
    {
        _tracerMovement.MoveTowardNewLocation();
    }

    void GetRandomStartingNode()
    {
       
        _currentMapNode = _parentEmitter.GetRandomRoadNode();
        if (_currentMapNode != null && _currentMapNode.ConnectedNodes.Count > 0)
        {
            _currentMapNodeId = _currentMapNode._id;
            gameObject.transform.position = _currentMapNode.LocationInUnits;
        } else
        {
            GetRandomStartingNode();
        }
        
    }
    
    void RewardBrain(float[] inputs, float[] outputs)
    {
        double error = 1d;
        int c = 0;
        while ((error > 0.1) && (c < 500))
        {
            c++;
            for (int i = 0; i < inputs.Length; i++)
            {
                ParentEmitter.EntityBrain.SetInput(i, inputs[i]);
            }

            for (int i = 0; i < outputs.Length; i++)
            {
                ParentEmitter.EntityBrain.SetDesiredOutput(i, outputs[i]);
            }

            ParentEmitter.EntityBrain.FeedForward();
            error = ParentEmitter.EntityBrain.CalculateError();
            ParentEmitter.EntityBrain.BackPropagate();
        }   
    }

    void GetNextConnection()
    {
        //First we need to back propogate some learning based on the previous decision that was made by the network if it was a 'good' decision
        //We define good simply as if it's lowest need is now higher than it was at the time it last made a new connection decision
		if (EntitiesPreviousNeeds.GetNeedValue(EntitiesPreviousNeeds.LowestNeed) < EntitiesNeeds.GetNeedValue(EntitiesPreviousNeeds.LowestNeed)){
            //Value is higher- so it did something right.
            RewardBrain(InputsForBrain, EntitiesBrainOutputs);
        }

        //Copy the current Need Values to the Previous Needs Variable before we modify them
        EntitiesPreviousNeeds.CopyEntitiesNeeds(EntitiesNeeds.EntitiesNeeds);

        //When we get a new connection we are making a decision with the NeuralNetwork brain.
        //We give it the need values of the four nearest neighbours and the entities current need values
        //It will then return four values and whichever is the highest is which neighbour we will visit next.
        InputsForBrain = new float[15] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}; //Fifteen points as 3 types of Need * 4 locations and the Entities
        
        //Populate the inputs field with the first available connections
        //TODO: Populate with closest neighbours first (arrange neighbours by distance once on init of MapNode?)
        if (_currentMapNode.ConnectedNodes.Count == 0)
        {
            GetRandomStartingNode();
            return;
        }
        for (int i = 0; i < _currentMapNode.ConnectedNodes.Count; i ++)
        {
            if (i*3 > InputsForBrain.Length)
            {
                break;
            }
			InputsForBrain[(i*3)] = _currentMapNode.ConnectedNodes[i].NeedAmounts[Needs.Food] + _currentMapNode.ConnectedNodes[i].NearbyNeedAmounts[Needs.Food];
			InputsForBrain[(i*3) + 1] = _currentMapNode.ConnectedNodes[i].NeedAmounts[Needs.Water] + _currentMapNode.ConnectedNodes[i].NearbyNeedAmounts[Needs.Water];
			InputsForBrain[(i*3) + 2] = _currentMapNode.ConnectedNodes[i].NeedAmounts[Needs.Shelter] + _currentMapNode.ConnectedNodes[i].NearbyNeedAmounts[Needs.Shelter];
        }

        //Set last input values to Entities current needs
        InputsForBrain[12] = EntitiesNeeds.GetNeedValue(Needs.Food);
        InputsForBrain[13] = EntitiesNeeds.GetNeedValue(Needs.Water);
        InputsForBrain[14] = EntitiesNeeds.GetNeedValue(Needs.Shelter);
        
        Debug.Log(InputsForBrain);

        //Set Brain Input Values
        for (int i = 0; i < InputsForBrain.Length; i++)
        {
            ParentEmitter.EntityBrain.SetInput(i, InputsForBrain[i]);
        }

        //Feed values forward
        ParentEmitter.EntityBrain.FeedForward();
        //Get output values and make decision
        EntitiesBrainOutputs[0] = ParentEmitter.EntityBrain.GetOutput(0);

        float direction = EntitiesBrainOutputs[0]; //Get value of first Output node
        int nodeToSelect = 0; //Default to the first node in the list
        //4 directions, check the four output nodes
        for (int i = 1; i < 4; i++)
        {
            EntitiesBrainOutputs[i] = ParentEmitter.EntityBrain.GetOutput(i);
            if (ParentEmitter.EntityBrain.GetOutput(i) > direction)
            {
                direction = ParentEmitter.EntityBrain.GetOutput(i);
                nodeToSelect = i;
            }
        }

		if (InputsForBrain [0] == 0f &&
		    InputsForBrain [3] == 0f &&
		    InputsForBrain [6] == 0f &&
		    InputsForBrain [9] == 0f) {
			_travellingToMapNode = _currentMapNode.GetRandomNeighbour ();
		} else {

			//Set next node to travel to
			if (_currentMapNode.ConnectedNodes.Count > nodeToSelect) {
				_travellingToMapNode = _currentMapNode.ConnectedNodes [nodeToSelect];
			} else {
				Debug.Log ("Entity selected node that does not exist to travel to next. Will try again next Update...");
			}
		}

        //Get all the connection nodes and find which has the highest value of each need
        //var highestNeedNodes = _currentMapNode.GetHighestNeedNeighbours();

        //Add the values of the current location to the Entities' needs
        EntitiesNeeds.AddNeedsFromDictionary(_currentMapNode.NeedAmounts);
        EntitiesNeeds.AddNeedsFromDictionary(_currentMapNode.NearbyNeedAmounts);
        
        //Update the colour based on the needs
        UpdateColour();

        //_travellingToMapNode = highestNeedNodes[EntitiesNeeds.LowestNeed];

        //_travellingToMapNode = _currentMapNode.GetRandomNeighbour();

        //Debug.Assert(_travellingToMapNode != null);

        if (_travellingToMapNode != null)
        {
            _travellingToMapNodeId = _travellingToMapNode._id;
        }
        else
        {
            _travellingToMapNode = ParentEmitter.GetRandomRoadNode();
        }
    }

    void UpdateColour()
    {
        _entityColour.color = new Color(EntitiesNeeds.GetNeedValue(Needs.Food), EntitiesNeeds.GetNeedValue(Needs.Shelter), EntitiesNeeds.GetNeedValue(Needs.Water));
        Color trailCol = Color.Lerp(_entityTrail.material.GetColor("_TintColor"), _entityColour.color, 0.1f);
        _entityTrail.material.SetColor("_TintColor", trailCol);
    }
}
