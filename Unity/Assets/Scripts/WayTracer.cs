using UnityEngine;
using System.Collections;

public class WayTracer : MonoBehaviour
{
	public float speedOfMovement;

	private MapController _mapController;
	private Vector3 _currentLocation;
	private Vector3 _futureLocation;
	private float _countTowardFutureLocation;
	private double _currentNodeId;
	private double _travellingToNodeId;

	public void Init (MapController _m)
	{
		_mapController = _m;
		_countTowardFutureLocation = 0;
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

				_currentNodeId = _travellingToNodeId;
				GetNextConnection ();
			}

	}

	void MoveTowardNewLocation ()
	{
		transform.position = Vector3.Lerp (_currentLocation, _futureLocation, _countTowardFutureLocation);
		_countTowardFutureLocation += speedOfMovement;
	}

	void GetRandomStartingNode ()
	{
		_currentNodeId = _mapController.GetRandomNodeId ();
	}

	void GetNextConnection ()
	{
		_travellingToNodeId = _mapController.GetRandomNodeConnectionId (_currentNodeId);
		_currentLocation = _mapController.GetNodePositionAsWorldCoordinateVector3 (_currentNodeId);
		_futureLocation = _mapController.GetNodePositionAsWorldCoordinateVector3 (_travellingToNodeId);
	}
}
