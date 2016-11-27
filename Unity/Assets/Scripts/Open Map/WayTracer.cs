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
        _needs.Add(Needs.Food, new EntityNeed(0.5f));
        _needs.Add(Needs.Water, new EntityNeed(0.5f));
        _needs.Add(Needs.Shelter, new EntityNeed(0.5f));
    }

    public bool Dead()
    {
        int count = 0;
        foreach (var n in _needs)
        {
            if (n.Value.GetValue() <= 0.001f)
            {
                count++;
            }
        }

        if (count == _needs.Count)
        {
            return true;
        }

        return false;
    }

    public bool HasPressingNeed(float value)
    {
        foreach (var n in _needs)
        {
            if (n.Value.GetValue() < value)
            {
                return true;
            }
        }
        return false;
    }

    public List<Needs> GetPressingNeeds(float value)
    {
        List<Needs> r = new List<Needs>();
        foreach(var n in _needs)
        {
            if (n.Value.GetValue() < value)
            {
                r.Add(n.Key);
            }
        }
        return r;
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
            //Debug.Log(n.Key + " / " + n.Value);
            AddToEntityNeed(n.Key, n.Value);
        }
    }

    public void UseNeeds(float metabolism) //Should eventually correspond with evolved values for how quickly it uses energy/food/water etc.
    {
        foreach (KeyValuePair<Needs, EntityNeed> n in _needs)
        {
            n.Value.AddValue(-metabolism);
        }
    }

    public override string ToString()
    {
        string r = "";

        foreach (KeyValuePair<Needs, EntityNeed> n in _needs)
        {
            r += " | Need: " + n.Key + " / " + n.Value.GetValue();
        }

        return r;
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

public class EntityGenes
{
    public float Speed = Random.Range(1.00f, 5.00f); //How quickly it moves
    public float PullFood = Random.Range(0.00f, 1.00f); //How much it wants to find food
    public float PullWater = Random.Range(0.00f, 1.00f); //How much it wants to find water
    public float PullShelter = Random.Range(0.00f, 1.00f); //How much it wants to find shelter
    public float DesperationValue = Random.Range(0.00f, 1.00f); //How low a need has to be before it panics and runs around looking to meet it
    public float Metabolism = Random.Range(0.00f, 1.00f); //How quickly it chews through its reserves of needs

    private float[] genes;
    public float[] Genes
    {
        get
        {
            return new float[] {Speed, PullFood, PullWater, PullShelter, DesperationValue, Metabolism};
        }

        set
        {
            Speed = value[0];
            PullFood = value[1];
            PullWater = value[2];
            PullShelter = value[3];
            DesperationValue = value[4];
            Metabolism = value[5];
        }
    }

    public EntityGenes() {
        //Debug.Log(Metabolism);
    }

    public EntityGenes(float[] g)
    {
        Speed = g[0];
        PullFood = g[1];
        PullWater = g[2];
        PullShelter = g[3];
        DesperationValue = g[4];
        Metabolism = g[5];
    }
    

    public override string ToString()
    {
        string output = "";
        for(int i = 0; i < Genes.Length; i++)
        {
            output += "Gene " + i + ": " + Genes[i];
        }
        return output;
    }

    public float[] MixGenes(float[] otherGenes)
    {
        float[] mixedGenes = new float[6];
        for (int i = 0; i < otherGenes.Length; i++)
        {
            float r = Random.Range(0.00f, 1.00f);
            if (r > 0.99f) //1% chance of random mutation
            {
                if (i == 0)
                {
                    mixedGenes[i] = Random.Range(1.00f, 5.00f);
                }
                else
                {
                    mixedGenes[i] = Random.Range(0.00f, 1.00f);
                }
            }

            else if (r > 0.5f && r < 0.99f)
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
   
}

public class NodeTracker
{
    IList _visitedNodes = new List<double>();

    public void AddNode(double id)
    {
        if (_visitedNodes.Count > 10)
        {
            _visitedNodes.RemoveAt(0);
            _visitedNodes.Add(id);
        }
        else
        {
            _visitedNodes.Add(id);
        }
    }

    public bool NodeVisited(double id)
    {
        if (_visitedNodes.Contains(id))
        {
            return true;
        }
        else
        {
            return false;
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
    
    public float triggerDistance = 0.01f;

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
    public NodeTracker NodeTracker = new NodeTracker();
    public TrailRenderer trail;

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

    public void Init(MapController _m, WayTracerEmitter _w, WaytracerMovement _move, double _startingNode)
    {

        //Debug.Log("New Entity created!");
        //Debug.Log(Genetics.ToString());

        _mapController = _m;
        _parentEmitter = _w;
        _tracerMovement = _move;

        InputsForBrain = new float[15] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        _currentMapNode = null;
        _travellingToMapNode = null;

        _entityColour = GetComponent<Renderer>().material;

        _currentMapNode = _mapController.GetMapNodeById(_startingNode);
        _currentMapNodeId = _currentMapNode._id;
        gameObject.transform.position = _currentMapNode.LocationInUnits;
        //Assign a connected node to move toward
        GetNextConnection();
    }

    public void Init(MapController _m, WayTracerEmitter _w, WaytracerMovement _move)
    {
        if (Genetics == null)
        {
            Genetics = new EntityGenes ();
		}
            //Debug.Log("New Entity created!");
            //Debug.Log(Genetics.ToString());
        
        _mapController = _m;
        _parentEmitter = _w;
        _tracerMovement = _move;

        InputsForBrain = new float[15] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        _currentMapNode = null;
        _travellingToMapNode = null;

        _entityColour = GetComponent<Renderer>().material;

        //Assign random starting location
        GetRandomStartingNode();
        //Assign a connected node to move toward
        GetNextConnection();
    }

    void Update()
    {
        /*
        foreach (MapNode n in _currentMapNode.ConnectedNodes)
        {
            Debug.DrawLine(_currentMapNode.LocationInUnits, n.LocationInUnits, new Color(255, 0, 255), 10f);
        }
        */
        MoveTowardNewLocation();
        if (Vector3.Distance(transform.position, _travellingToMapNode.LocationInUnits) < triggerDistance)
        {
            _currentMapNode = _travellingToMapNode;
            GetNextConnection();
        }
        //EntitiesNeeds.AddNeedsFromDictionary(_currentMapNode.NeedAmounts);
        EntitiesNeeds.UseNeeds(Genetics.Metabolism);

        CheckIfDead();
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

    float GeneAlgorithm(float geneLevel, float currentNeedLevel, float nodeNeedLevel)
    {
        //Debug.Log(geneLevel + ", " + currentNeedLevel + ", " + nodeNeedLevel);
        //return Mathf.Sqrt((geneLevel * nodeNeedLevel)) + (currentNeedLevel * nodeNeedLevel);
        return (geneLevel * Mathf.Sqrt(nodeNeedLevel)) + (1 - currentNeedLevel);
    }

    void GetNextConnection()
    {
        //if (EntitiesNeeds.EntitiesNeeds[EntitiesNeeds.HighestNeed].GetValue() < 0.4)
        //{
        //    _travellingToMapNode = _currentMapNode.GetRandomNeighbour();
        //} else

        if (_currentMapNode.ConnectedNodes.Count == 0)
        {
            Debug.Log("No connected nodes. This shouldn't happen!");
            //TODO: Destroy this entity
            ParentEmitter.EntityDies(this);
            Destroy(gameObject);
        }
        else if (_currentMapNode.ConnectedNodes.Count == 1)
        {
            //Debug.Log("Only 1 node");
            _travellingToMapNode = _currentMapNode.ConnectedNodes[0];
        }
        else
        {

           // _travellingToMapNode = _currentMapNode.GetRandomNeighbour();

            
            //Pull in neightbours and make decision based on genes as to which to attend to next.
            //For the three Needs, check each neighbour node to find which returns the highest value, then of those three go in the direction of
            //the highest.

            //We want to find the one that can cover most needs, but also discount that need from the equation if it is low enough
            //and also go directly to that ignoring the others if it is high enough.
            //To do this we weight the decision based on how pressing the need is

            //Institute a rule whereby if there's a pressing need and you can't find it then run randomly for a few nodes to get to a new area.

            float hungerValue = 0;
            float waterValue = 0;
            float shelterValue = 0;
            int hungerIndex = Random.Range(0, _currentMapNode.ConnectedNodes.Count);
            int waterIndex = Random.Range(0, _currentMapNode.ConnectedNodes.Count);
            int shelterIndex = Random.Range(0, _currentMapNode.ConnectedNodes.Count);

            for (int i = 0; i < _currentMapNode.ConnectedNodes.Count; i++)
            {
                float weight = 0f;
                if (NodeTracker.NodeVisited(_currentMapNode.ConnectedNodes[i]._id))
                {
                    //Debug.Log("Weighted against index: " + i);
                    weight = 0.01f;
                }
                float h = GeneAlgorithm(Genetics.Genes[1], EntitiesNeeds.GetNeedValue(Needs.Food), (_currentMapNode.ConnectedNodes[i].NeedAmounts[Needs.Food] + _currentMapNode.ConnectedNodes[i].NearbyNeedAmounts[Needs.Food] - _currentMapNode.ConnectedNodes[i].MaxNeeds[Needs.Food]));
                float w = GeneAlgorithm(Genetics.Genes[2], EntitiesNeeds.GetNeedValue(Needs.Water), (_currentMapNode.ConnectedNodes[i].NeedAmounts[Needs.Water] + _currentMapNode.ConnectedNodes[i].NearbyNeedAmounts[Needs.Water] - _currentMapNode.ConnectedNodes[i].MaxNeeds[Needs.Water]));
                float s = GeneAlgorithm(Genetics.Genes[3], EntitiesNeeds.GetNeedValue(Needs.Shelter), (_currentMapNode.ConnectedNodes[i].NeedAmounts[Needs.Shelter] + _currentMapNode.ConnectedNodes[i].NearbyNeedAmounts[Needs.Shelter] - _currentMapNode.ConnectedNodes[i].MaxNeeds[Needs.Shelter]));
                //Debug.Log("Index: "+i+". Gene values: " + h + ", " + w + ", " + s);

                h = h * weight;
                w = w * weight;
                s = s * weight;

                //Debug.Log("Index: " + i + ". Gene values: " + h + ", " + w + ", " + s+ "After weighting is applied");

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

            int directionToMove = Random.Range(0, _currentMapNode.ConnectedNodes.Count);

            if (EntitiesNeeds.HasPressingNeed(Genetics.DesperationValue))
            {
                
                //Desperation mode should take into account visited nodes.
                //Debug.Log("Desperation mode activated");
                for (int i = 0; i < _currentMapNode.ConnectedNodes.Count; i++)
                {
                    if (!NodeTracker.NodeVisited(_currentMapNode.ConnectedNodes[i]._id)){
                        directionToMove = i;
                        if (Random.value > 0.5f)
                        {
                            break;
                        }
                    }
                }


                //directionToMove = Random.Range(0, _currentMapNode.ConnectedNodes.Count);
                
                //directionToMove = hungerValue > waterValue ? hungerValue > shelterValue ? hungerIndex : waterValue > shelterValue ? waterIndex : shelterIndex : waterValue > shelterValue ? waterIndex : shelterIndex; //Return the index of the largest value
                /*
                List<Needs> x = EntitiesNeeds.GetPressingNeeds(Genetics.DesperationValue);
                //Run toward the most pressing need
                switch (x[0])
                {
                    case Needs.Water:
                        directionToMove = waterIndex;
                        break;
                    case Needs.Food:
                        directionToMove = hungerIndex;
                        break;
                    case Needs.Shelter:
                        directionToMove = shelterIndex;
                        break;
                    default:
                        break;
                }
                */
                
            } else
            {

                if (hungerValue > waterValue && hungerValue > shelterValue)
                {
                    directionToMove = hungerIndex;
                }
                if (waterValue > hungerValue && waterValue > shelterValue)
                {
                    directionToMove = waterIndex;
                }
                if (shelterValue > waterValue && shelterValue > hungerValue)
                {
                    directionToMove = shelterIndex;
                }
                //directionToMove = hungerValue > waterValue ? hungerValue > shelterValue ? hungerIndex : waterValue > shelterValue ? waterIndex : shelterIndex : waterValue > shelterValue ? waterIndex : shelterIndex; //Return the index of the largest value
            }

            
            
            
            //Debug.Log("Direction chosen: " + directionToMove+" of a possible "+_currentMapNode.ConnectedNodes.Count);
            //Debug.Log("Values at play were: HungerValue/Index: " + hungerValue + "/" + hungerIndex + " WaterValue/Index: " + waterValue + "/" + waterIndex + "ShelterValue/Index: " + shelterValue + "/" + shelterIndex);
                //Debug.Log(hungerValue + ", " + waterValue + ", "+shelterValue);
            _travellingToMapNode = _currentMapNode.ConnectedNodes[directionToMove];
            NodeTracker.AddNode(_currentMapNode.ConnectedNodes[directionToMove]._id);
            //Debug.Log(_currentMapNode.ConnectedNodes[directionToMove]._id);
    
        }

        //Add the values of the current location to the Entities' needs
        //_currentMapNode.ConsumeNeeds();
        EntitiesNeeds.AddNeedsFromDictionary(_currentMapNode.NeedAmounts);
        EntitiesNeeds.AddNeedsFromDictionary(_currentMapNode.NearbyNeedAmounts);
        //Debug.Log(EntitiesNeeds.ToString());

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
            //Something went wrong- kill this Entity
            ParentEmitter.EntityDies(this);
            Destroy(gameObject);
        }
        CheckIfDead();
    }

    public void Instakill()
    {
        ParentEmitter.EntityDies(this);
        DestroyImmediate(gameObject);
    }

    void CheckIfDead()
    {
        if (EntitiesNeeds.Dead())
        {
            //Goodbye :(
            ParentEmitter.EntityDies(this);
            Destroy(gameObject);
        }
    }

    void UpdateColour()
    {
        _entityColour.color = new Color(EntitiesNeeds.GetNeedValue(Needs.Food), EntitiesNeeds.GetNeedValue(Needs.Shelter), EntitiesNeeds.GetNeedValue(Needs.Water));
        Color trailCol = Color.Lerp(trail.material.GetColor("_TintColor"), _entityColour.color, 0.1f);
        trail.material.SetColor("_TintColor", trailCol);
    }
}
