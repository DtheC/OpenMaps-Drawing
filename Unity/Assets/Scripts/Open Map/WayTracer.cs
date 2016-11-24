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
    public NeuralNetwork EntityBrain;
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

        EntityBrain = new NeuralNetwork();
        EntityBrain.Initialize(15, 50, 4);
        EntityBrain.SetLearningRate(0.2f);
        EntityBrain.SetMomentum(true, 0.9f);

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
        if (_currentMapNode != null)
        {
            _currentMapNodeId = _currentMapNode._id;
        }
    }
    
    void RewardBrain(float[] inputs, float[] outputs)
    {
        double error = 1d;
        int c = 0;
        while ((error > 0.1) && (c < 2000))
        {
            c++;
            for (int i = 0; i < inputs.Length; i++)
            {
                EntityBrain.SetInput(i, inputs[i]);
            }

            for (int i = 0; i < outputs.Length; i++)
            {
                EntityBrain.SetDesiredOutput(i, outputs[i]);
            }

            EntityBrain.FeedForward();
            error = EntityBrain.CalculateError();
            EntityBrain.BackPropagate();
        }   
    }

    void GetNextConnection()
    {
        //First we need to back propogate some learning based on the previous decision that was made by the network if it was a 'good' decision
        //We define good simply as if it's lowest need is now higher than it was at the time it last made a new connection decision
        if (EntitiesPreviousNeeds.GetNeedValue(EntitiesPreviousNeeds.LowestNeed) < EntitiesNeeds.GetNeedValue(EntitiesNeeds.LowestNeed)){
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
        for (int i = 0; i < _currentMapNode.ConnectedNodes.Count-1; i += 3)
        {
            if (i > InputsForBrain.Length-3)
            {
                break;
            }
            InputsForBrain[i] = _currentMapNode.ConnectedNodes[0].NeedAmounts[Needs.Food];
            InputsForBrain[i + 1] = _currentMapNode.ConnectedNodes[0].NeedAmounts[Needs.Water];
            InputsForBrain[i + 2] = _currentMapNode.ConnectedNodes[0].NeedAmounts[Needs.Shelter];
        }

        //Set last input values to Entities current needs
        InputsForBrain[12] = EntitiesNeeds.GetNeedValue(Needs.Food);
        InputsForBrain[13] = EntitiesNeeds.GetNeedValue(Needs.Water);
        InputsForBrain[14] = EntitiesNeeds.GetNeedValue(Needs.Shelter);

        Debug.Log(InputsForBrain);

        //Set Brain Input Values
        for (int i = 0; i < InputsForBrain.Length; i++)
        {
            EntityBrain.SetInput(i, InputsForBrain[i]);
        }

        //Feed values forward
        EntityBrain.FeedForward();

        //Get output values and make decision
        EntitiesBrainOutputs[0] = EntityBrain.GetOutput(0);

        float direction = EntitiesBrainOutputs[0]; //Get value of first Output node
        int nodeToSelect = 0; //Default to the first node in the list
        //4 directions, check the four output nodes
        for (int i = 1; i < 4; i++)
        {
            EntitiesBrainOutputs[i] = EntityBrain.GetOutput(i);
            if (EntityBrain.GetOutput(i) > direction)
            {
                direction = EntityBrain.GetOutput(i);
                nodeToSelect = i;
            }
        }

        //Set next node to travel to
        if (_currentMapNode.ConnectedNodes.Count-1 >= nodeToSelect)
        {
            _travellingToMapNode = _currentMapNode.ConnectedNodes[nodeToSelect];
        }
        else
        {
            Debug.Log("Entity selected node that does not exist to travel to next. Will try again next Update...");
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

        Debug.Assert(_travellingToMapNode != null);

        if (_travellingToMapNode != null)
        {
            _travellingToMapNodeId = _travellingToMapNode._id;
        }
        else
        {
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
