using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WayTracerEmitter : MonoBehaviour {

	public GameObject wayTracerObject;
	public MapController mapControl;

	public IList<MapWay> RoadWays { get; set; }

	void Update(){
		if (RoadWays == null) {
			initRoadWays();
		}

		if (Input.anyKey) {
			WayTracer _w = Instantiate (wayTracerObject).GetComponent<WayTracer> ();
			_w.Init (mapControl, this);
		}
	}

	void initRoadWays(){
		if (mapControl.WayList != null) {
			RoadWays = new List<MapWay>();
			foreach (MapWay w in mapControl.WayList){
				if (w._tags.Keys.Contains("highway")){
					RoadWays.Add(w);
				}
			}
		}
	}

	public MapNode GetRandomRoadNode(){
		int randomIndex = Random.Range (0, RoadWays.Count);
		return mapControl.GetMapNodeById(RoadWays [randomIndex]._nodesInWay [Random.Range (0, RoadWays [randomIndex]._nodesInWay.Count)]);
	}

}