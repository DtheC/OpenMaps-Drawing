using UnityEngine;
using System.Collections;


//SO currently these follow any neightbour of the current node which means they can move along
//ways that we may not want them to. What we need to do instead is set certain variable on the tracer (at this stage, can travel on highways only)
//Then we move between nodes on a way and when we hit a new node we check that node's neighbours for ones which match our abilities.
//So maybe what we actually need to do it set MapNodes to have a tag list as well of what they can do. Can we check for the ways that are assocaited with
//a given node and see which are highways? 

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
