using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

public class GeneCSV
{
    public GeneCSV() { }

    public List<float[]> ReadGenes()
    {
        if (File.Exists(@"newgenes.csv"))
        {
            //Parse CSV file of last rounds top genes and produce their objects
            var reader = new StreamReader(File.OpenRead(@"newgenes.csv"));
            List<float[]> genes = new List<float[]>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                float[] numericalValues = new float[values.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    numericalValues[i] = float.Parse(values[i]);
                }
                //Got the genes. Now what do I do with them?
                genes.Add(numericalValues);
            }
            return genes;
        }
        else
        {
            return null;
        }
    }

    public void WriteGenes(List<WayTracer> genes)
    {
        var csv = new StringBuilder();
        
        //Cycle through genes passed in
        foreach (var g in genes)
        {
            float[] output = g.Genetics.Genes;
            string line = string.Format("{0},{1},{2},{3},{4},{5}", output[0], output[1], output[2], output[3], output[4], output[5]);
            csv.AppendLine(line);
        }
        //after your loop
        File.WriteAllText(@"newgenes.csv", csv.ToString());
    }
}

public class WayTracerEmitter : MonoBehaviour
{
    public NeuralNetwork EntityBrain;
    public double EmitterNode = 2247259686;

    bool init = true;

    public int GenerationCount = 0;
    public int EntitiesPerGeneration = 500;
    public int NextGenNumber = 20;
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

    List<float[]> previousGenes = null;

    public int CurrentEntities
    {
        get
        {
            return _currentEntities;
        }

        set
        {
            _currentEntities = value;
        }
    }

    void Start()
    {
        GeneCSV g = new GeneCSV();
        previousGenes = g.ReadGenes();
    }

    void Init()
    {
        if (previousGenes != null)
        {
            _entities = new List<WayTracer>();
            GenerationCount++;
            for (int i = 0; i < previousGenes.Count; i++)
            {
                CurrentEntities++;
                GameObject _w = (GameObject)Instantiate(wayTracerObject, transform.position, Quaternion.identity);
                WayTracer _ww = _w.GetComponent<WayTracer>();
                _ww.Genetics = new EntityGenes(previousGenes[i]);
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
        }
        else
        {
            _entities = new List<WayTracer>();
            GenerationCount++;
            for (int i = 0; i < EntitiesPerGeneration; i++)
            {
                CurrentEntities++;
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
        }
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

        //CameraFade.Instance.FadingIn = true;

        List<WayTracer> newGen = CreateNewGeneration();

       // Debug.Log(_entities.Count);

        _entities = new List<WayTracer>(newGen);
        CurrentEntities = newGen.Count;
        //Debug.Log(_entities.Count);
    }

    List<WayTracer> CreateNewGeneration()
    {
        List<WayTracer> newGeneration = new List<WayTracer>();
        List<WayTracer> oldGeneration = new List<WayTracer>(_entities.Where(n => !n.Dead).ToArray());
        Debug.Log("Oldgen: " + oldGeneration.Count);

        var genewrite = new GeneCSV();
        genewrite.WriteGenes(oldGeneration);


        oldGeneration.RemoveRange(0, oldGeneration.Count - NextGenNumber);

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
            if (newGeneration.Count > EntitiesPerGeneration)
            {
                break;
            }
        }

        for (int i = 0; i < _entities.Count; i++)
        {
            _entities[i].Instakill();
        }

        return newGeneration;
    }

    List<WayTracer> CreateNewGeneration(List<float[]> genesToUse)
    {
        List<WayTracer> newGeneration = new List<WayTracer>();
        List<WayTracer> oldGeneration = new List<WayTracer>();
        foreach (var g in genesToUse)
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
            _ww.Genetics = new EntityGenes(g);
            oldGeneration.Add(_ww);
        }

        oldGeneration.RemoveRange(0, oldGeneration.Count - NextGenNumber);

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
            if (newGeneration.Count > EntitiesPerGeneration)
            {
                break;
            }
        }

        for (int i = 0; i < oldGeneration.Count; i++)
        {
            oldGeneration[i].Instakill();
        }

        return newGeneration;
    }

    public void EntityDies(WayTracer deadWaytracer)
    {
        CurrentEntities--;
        //_entities.Remove(deadWaytracer);

        if (CurrentEntities == NextGenNumber)
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

    public void HideTheDead()
    {
        foreach (WayTracer t in _entities)
        {
            if (t.Dead)
            {
                t.Disappear();
            }
        }
    }

    public void ShowTheDead()
    {
        foreach (WayTracer t in _entities)
        {
            if (t.Dead)
            {
                t.Reappear();
            }
        }
    }

}