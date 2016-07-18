using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapDrawer : MonoBehaviour {

	public Transform NodeObject;

	public bool OnlyDrawHighways = true;

	private MapController _mapController;

	public MapController MapController {
		get {
			return _mapController;
		}
		set {
			_mapController = value;
		}
	}

	public void DrawNodes(IDictionary<double, float[]> nodeDict){
		foreach (float[] n in nodeDict.Values) {
			float x = MapMetaInformation.Instance.MapLatValue (n[0]);
			float y = MapMetaInformation.Instance.MapLonValue (n[1]);
			Instantiate (NodeObject, new Vector3 (x, 0, y), Quaternion.identity);
		}
	}

	public void DrawWays(IList<MapWay> wayList){
		foreach (MapWay mapway in wayList) {
			if (OnlyDrawHighways){
				if (!mapway._tags.ContainsKey("highway")){
					continue;
				}
			}
			MapNode to = null;
			MapNode from = null;
			Color randomCol = new Color (Random.Range (0.00f, 1.00f),Random.Range (0.00f, 1.00f),Random.Range (0.00f, 1.00f));
			for (int i=0; i < mapway._nodesInWay.Count; i++){
				//Get nodes
				to = _mapController.GetMapNodeById(mapway._nodesInWay[i]._id);
				if (to == null){
					continue;
				}
				if (from == null){
					from = to;
					continue;
				}
				Debug.DrawLine(from.LocationInUnits, to.LocationInUnits, randomCol, 2000, false);
				from = to;
			}
		}
	}

//	void DrawRandomNode(){
//		int randomNodeIndex = Random.Range (0, _nodeConnectionDictionary.Count);
//		//Debug.Log (_nodeConnectionDictionary.Count);
//		KeyValuePair<double, IList<double>> selectedNode = _nodeConnectionDictionary.ElementAt (randomNodeIndex);
//		//		Debug.Log ("Drawing node "+selectedNode.Key+" and connections.");
//		DrawNode(selectedNode.Key);
//		foreach (double nID in selectedNode.Value.AsEnumerable()) {
//			//Debug.Log(nID);
//			DrawNode(nID);
//		}
//	}
//
//	void DrawNode(double id){
//		float[] f = getNodeLatLonByID (id, _nodeDictionary);
//		if (f == null) {
//			return;
//		}
//		float Lat = MapMetaInformation.Instance.MapLatValue (f[0]);
//		float Lon = MapMetaInformation.Instance.MapLonValue (f[1]);
//		Instantiate (NodeObject, new Vector3 (Lat, 0, Lon), Quaternion.identity);
//	}
}
