using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WayTracerEmitter : MonoBehaviour {
    public enum WayTracerMovementType
    {
        Rigid,
        Free
    }

    [SerializeField]
    public WayTracerMovementType MovementType;


	public GameObject wayTracerObject;
	public MapController mapControl;

	void Update(){

		if (Input.GetKey("left")) {
            GameObject _w = (GameObject) Instantiate(wayTracerObject, transform.position, Quaternion.identity);
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
            
		}
	}

	public MapNode GetRandomRoadNode(){
		return mapControl.GetRandomNode();
		//int randomIndex = Random.Range (0, RoadWays.Count);
		//return mapControl.GetMapNodeById(RoadWays [randomIndex]._nodesInWay [Random.Range (0, RoadWays [randomIndex]._nodesInWay.Count)]);
	}

}