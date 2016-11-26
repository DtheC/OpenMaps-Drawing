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
            n.Value.AddValue(-0.1f);
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

public class EntityGenes : MonoBehaviour
{
    //TODO: Add genes for how quickly it uses these up
    public float SightLength = 0.0f;
    public float Speed = 0.0f;
    public float PullFood = 0.0f;
    public float PullWater = 0.0f;
    public float PullShelter = 0.0f;
    public float NodesVisited = -1.0f;
    //Best to actually just hold them all in an array? Less user friendly...
    public float[] Genes = new float[] { 0, 0, 0, 0, 0, -1.0f };

    public EntityGenes() {
		RandomiseGenes ();
    }
    public EntityGenes(float s, float sp, float pf, float pw, float ps)
    {
        SightLength = s;
        Speed = sp;
        PullFood = pf;
        PullWater = pw;
        PullShelter = ps;
        NodesVisited = 0.0f;
        Genes[5] = 0.0f;
    }

    public EntityGenes(float[] g)
    {
        Genes[0] = g[0];
        Genes[1] = g[1];
        Genes[2] = g[2];
        Genes[3] = g[3];
        Genes[4] = g[4];
        NodesVisited = 0.0f;
        Genes[5] = 0.0f;
    }

    void Start()
    {
        if (Genes[5] == -1.0f)
        {
            RandomiseGenes();
        }
    }

    public void LogGenes()
    {
        foreach (var f in Genes)
        {
            Debug.Log(f);
        }
    }

    public float[] MixGenes(float[] otherGenes)
    {
        float[] mixedGenes = new float[6];
        for (int i = 0; i < otherGenes.Length; i++)
        {
            float r = Random.value;
            if (r > 0.9) //10% chance of random mutation
            {
                mixedGenes[i] = Random.Range(0.00f, 1.00f);
            }
            else if (r > 0.45f)
            {
                mixedGenes[i] = otherGenes[i];
            }
            else
            {
                mixedGenes[i] = Genes[i];
            }
        }
        return mixedGenes;
    }
    
    public void RandomiseGenes()
    {
        for (int i = 0; i < Genes.Length-1; i++)
        {
            Genes[i] = Random.Range(0.00f, 1.00f);
        }
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

    public EntityGenes Genetics = new EntityGenes();

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
		if (Genetics == null) {
			Genetics = new EntityGenes ();
		}

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

    float GeneAlgorithm(float geneLevel, float currentNeedLevel, float nodeNeedLevel)
    {
        //Debug.Log(geneLevel + ", " + currentNeedLevel + ", " + nodeNeedLevel);
        return Mathf.Sqrt((geneLevel * nodeNeedLevel)) + (currentNeedLevel * nodeNeedLevel);
    }

    void GetNextConnection()
    {
        if (_currentMapNode.ConnectedNodes.Count == 0)
        {
            Debug.Log("HOW DID THIS HAPPEN?!");
			_travellingToMapNode = ParentEmitter.GetRandomRoadNode ();
		} 
        else
        {

            //Pull in neightbours and make decision based on genes as to which to attend to next.
            //For the three Needs, check each neighbour node to find which returns the highest value, then of those three go in the direction of
            //the highest.

            float hungerValue = 0;
            float waterValue = 0;
            float shelterValue = 0;
            int hungerIndex = 0;
            int waterIndex = 0;
            int shelterIndex = 0;

            for (int i = 0; i < _currentMapNode.ConnectedNodes.Count; i++)
            {
                float h = GeneAlgorithm(Genetics.Genes[2], EntitiesNeeds.GetNeedValue(Needs.Food), (_currentMapNode.ConnectedNodes[i].NeedAmounts[Needs.Food] + _currentMapNode.ConnectedNodes[i].NearbyNeedAmounts[Needs.Food]));
                float w = GeneAlgorithm(Genetics.Genes[3], EntitiesNeeds.GetNeedValue(Needs.Water), (_currentMapNode.ConnectedNodes[i].NeedAmounts[Needs.Water] + _currentMapNode.ConnectedNodes[i].NearbyNeedAmounts[Needs.Water]));
                float s = GeneAlgorithm(Genetics.Genes[4], EntitiesNeeds.GetNeedValue(Needs.Shelter), (_currentMapNode.ConnectedNodes[i].NeedAmounts[Needs.Shelter] + _currentMapNode.ConnectedNodes[i].NearbyNeedAmounts[Needs.Shelter]));
                //Debug.Log("Gene values: " + h + ", " + w + ", " + s);

                if (h > hungerValue)
                {
                    hungerValue = h;
                    hungerIndex = i;
                }
                if (w > waterValue)
                {
                    waterValue = w;
                    waterIndex = i;
                }
                if (s > shelterValue)
                {
                    shelterValue = s;
                    shelterIndex = i;
                }
            }

            int directionToMove = hungerValue > waterValue ? hungerValue > shelterValue ? hungerIndex : waterValue > shelterValue ? waterIndex : shelterIndex : waterValue > shelterValue ? waterIndex : shelterIndex; //Return the index of the largest value
                                                                                                                                                                                                                       //Debug.Log(hungerValue + ", " + waterValue + ", "+shelterValue);
            _travellingToMapNode = _currentMapNode.ConnectedNodes[directionToMove];
        }
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
