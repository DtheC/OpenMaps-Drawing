using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeDrawer : MonoBehaviour {

	bool DrawRestaurants = true;

	public Transform restaurantObject;

	public void DrawNodes(IList<MapNode> NodeList){
		foreach (MapNode n in NodeList) {
			if(n._tags.ContainsKey("amenity")){
				if (n._tags["amenity"].Contains("restaurant")){
					float x = MapMetaInformation.Instance.MapLatValue(n._lat);
					float y = MapMetaInformation.Instance.MapLonValue(n._lon);
					Instantiate(restaurantObject, new Vector3(x,0,y), Quaternion.identity);
				}
			}
		}
	}
}
