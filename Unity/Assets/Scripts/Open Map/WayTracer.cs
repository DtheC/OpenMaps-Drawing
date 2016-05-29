using UnityEngine;
using System.Collections;

public class WayTracer : MonoBehaviour
{
	public float speedOfMovement = 0.1f;

	private MapController _mapController;
	private WayTracerEmitter _parentEmitter;
	private float _countTowardFutureLocation;

	private MapNode _currentMapNode;
	private MapNode _travellingToMapNode;

	public void Init (MapController _m, WayTracerEmitter _w)
	{
		_mapController = _m;
		_parentEmitter = _w;
		_countTowardFutureLocation = 0;

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
			if (_countTowardFutureLocation >= 1) {
				_countTowardFutureLocation = 0;
				_currentMapNode = _travellingToMapNode;
				GetNextConnection ();
			}
	}

	void MoveTowardNewLocation ()
	{
		transform.position = Vector3.Lerp (_currentMapNode.LocationInUnits, _travellingToMapNode.LocationInUnits, _countTowardFutureLocation);
		_countTowardFutureLocation += speedOfMovement;
	}

	void GetRandomStartingNode ()
	{
		_currentMapNode = _parentEmitter.GetRandomRoadNode ();
		//_currentMapNode = _mapController.GetRandomNode ();
	}

	void GetNextConnection ()
	{
		_travellingToMapNode = _currentMapNode.GetRandomNeighbour ();
		if (_travellingToMapNode == null) {
			_travellingToMapNode = _currentMapNode;
		}
	}
}
