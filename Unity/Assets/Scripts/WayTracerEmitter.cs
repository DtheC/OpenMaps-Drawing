using UnityEngine;
using System.Collections;

public class WayTracerEmitter : MonoBehaviour {

	public GameObject wayTracerObject;
	public MapController mapControl;

	void Update(){
		if (Input.anyKey) {
			WayTracer _w = Instantiate (wayTracerObject).GetComponent<WayTracer> ();
			_w.Init (mapControl);
		}
	}

}
