using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WayTracerEmitter : MonoBehaviour {

	public GameObject wayTracerObject;
	public MapController mapControl;

	void Update(){

		if (Input.anyKey) {
			WayTracer _w = Instantiate (wayTracerObject).GetComponent<WayTracer> ();
			_w.Init (mapControl, this);
		}
	}

	public MapNode GetRandomRoadNode(){
		return mapControl.GetRandomNode();
		//int randomIndex = Random.Range (0, RoadWays.Count);
		//return mapControl.GetMapNodeById(RoadWays [randomIndex]._nodesInWay [Random.Range (0, RoadWays [randomIndex]._nodesInWay.Count)]);
	}

}