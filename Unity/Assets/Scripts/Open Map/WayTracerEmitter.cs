using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WayTracerEmitter : MonoBehaviour
{
    public NeuralNetwork EntityBrain;
    public double EmitterNode = 2247259686;

    bool init = true;

    public int GenerationCount = 0;
    public int EntitiesPerGeneration = 500;
    private int _currentEntities = 0;
    List<WayTracer> _entities = new List<WayTracer>();
    
    public enum WayTracerMovementType
    {
        Rigid,
        Free
    }

    [SerializeField]
    public WayTracerMovementType MovementType;
    
    public GameObject wayTracerObject;
    public MapController mapControl;

    void Init()
    {
        _entities = new List<WayTracer>();
        GenerationCount++;
        for (int i = 0; i < EntitiesPerGeneration; i++)
        {
            _currentEntities++;
            GameObject _w = (GameObject)Instantiate(wayTracerObject, transform.position, Quaternion.identity);
            WayTracer _ww = _w.GetComponent<WayTracer>();
            _ww.Genetics = new EntityGenes();
            _entities.Add(_ww);
            switch (MovementType)
            {
                case WayTracerMovementType.Rigid:
                    _ww.Init(mapControl, this, new WayTracerMovementRigid(_ww));
                    break;
                case WayTracerMovementType.Free:
                    _ww.Init(mapControl, this, new WaytracerMovementFree(_ww));
                    break;
                default:
                    _ww.Init(mapControl, this, new WayTracerMovementRigid(_ww));
                    break;
            }
        }
        Debug.Log(_entities.Count);
    }

    void Update()
    {
        if (init)
        {
            init = false;
            Init();
        }
        

        /*
        if (Input.GetKey("left"))
        {
            GameObject _w = (GameObject)Instantiate(wayTracerObject, transform.position, Quaternion.identity);
            WayTracer _ww = _w.GetComponent<WayTracer>();
            switch (MovementType)
            {
                case WayTracerMovementType.Rigid:
                    _ww.Init(mapControl, this, new WayTracerMovementRigid(_ww), EmitterNode);
                    break;
                case WayTracerMovementType.Free:
                    _ww.Init(mapControl, this, new WaytracerMovementFree(_ww), EmitterNode);
                    break;
                default:
                    _ww.Init(mapControl, this, new WayTracerMovementRigid(_ww), EmitterNode);
                    break;
            }
        }
        */
    }

    void NewGen()
    {
        //_currentEntities = 5000;
        GenerationCount++;
        //Debug.Log(_entities.Count);

        List<WayTracer> newGen = CreateNewGeneration();

       // Debug.Log(_entities.Count);

        _entities = new List<WayTracer>(newGen);
        _currentEntities = newGen.Count;
        Debug.Log(_entities.Count);
    }

    List<WayTracer> CreateNewGeneration()
    {
        List<WayTracer> newGeneration = new List<WayTracer>();
        List<WayTracer> oldGeneration = new List<WayTracer>(_entities);
        Debug.Log("Oldgen: "+oldGeneration.Count);
        
            oldGeneration.RemoveRange(0, oldGeneration.Count - 20);
        
        for (int i = 0; i < oldGeneration.Count; i++)
        {
            for (int j = 0; j < oldGeneration.Count; j++)
            {
                GameObject _w = (GameObject)Instantiate(wayTracerObject, transform.position, Quaternion.identity);
                WayTracer _ww = _w.GetComponent<WayTracer>();
                switch (MovementType)
                {
                    case WayTracerMovementType.Rigid:
                        _ww.Init(mapControl, this, new WayTracerMovementRigid(_ww));
                        break;
                    case WayTracerMovementType.Free:
                        _ww.Init(mapControl, this, new WaytracerMovementFree(_ww));
                        break;
                    default:
                        _ww.Init(mapControl, this, new WayTracerMovementRigid(_ww));
                        break;
                }
                var newGenes = oldGeneration[i].Genetics.MixGenes(oldGeneration[j].Genetics.Genes);
                _ww.Genetics = new EntityGenes(newGenes);
                newGeneration.Add(_ww);
            }
        }

        for (int i = 0; i < _entities.Count; i++)
        {
            if (_entities[i])
            {
                _entities[i].Instakill();
            }
        }

        return newGeneration;
    }

    public void EntityDies(WayTracer deadWaytracer)
    {
        _currentEntities--;
        _entities.Remove(deadWaytracer);

        if (_currentEntities == 20)
        {
            NewGen();
        }

        //Debug.Log(_currentEntities);
    }
    public MapNode GetRandomRoadNode()
    {
        return mapControl.GetRandomNode();
        //int randomIndex = Random.Range (0, RoadWays.Count);
        //return mapControl.GetMapNodeById(RoadWays [randomIndex]._nodesInWay [Random.Range (0, RoadWays [randomIndex]._nodesInWay.Count)]);
    }

}