using UnityEngine;
using System.Collections;

public class WayTracer : MonoBehaviour
{
	public float speedOfMovement = 2.0f;
	public float triggerDistance = 0.1f;

	private MapController _mapController;
	private WayTracerEmitter _parentEmitter;

	private MapNode _currentMapNode;
	private MapNode _travellingToMapNode;

	[SerializeField] 
	private double _currentMapNodeId;
	private double _travellingToMapNodeId;

	public void Init (MapController _m, WayTracerEmitter _w)
	{
		_mapController = _m;
		_parentEmitter = _w;

		_currentMapNode = null;
		_travellingToMapNode = null;

		//Assign random starting location
		GetRandomStartingNode ();
		//Assign a connected node to move toward
		GetNextConnection ();
	}

	void Update ()
	{
			MoveTowardNewLocation ();
			if (Vector3.Distance (transform.position, _travellingToMapNode.LocationInUnits) < triggerDistance) {
			_currentMapNode = _travellingToMapNode;
			GetNextConnection();
		}
	}

	void MoveTowardNewLocation ()
	{
		float step = speedOfMovement * Time.deltaTime;
		transform.position = Vector3.MoveTowards (transform.position, _travellingToMapNode.LocationInUnits, step);
	}

	void GetRandomStartingNode ()
	{
		_currentMapNode = _parentEmitter.GetRandomRoadNode ();
		if (_currentMapNode != null) {
			_currentMapNodeId = _currentMapNode._id;
		}
	}

	void GetNextConnection ()
	{
		_travellingToMapNode = _currentMapNode.GetRandomNeighbour ();

		Debug.Assert (_travellingToMapNode != null);

		if (_travellingToMapNode != null) {
			_travellingToMapNodeId = _travellingToMapNode._id;
		} else {
			_travellingToMapNode = _currentMapNode;
		}
	}
}
