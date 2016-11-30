using UnityEngine;
using System.Collections;

public class MapMetaInformation : MonoBehaviour {

	private float _mapWidth;

	public float MapWidth {
		get {
			return _mapWidth;
		}
	}

	private float _mapHeight;

	public float MapHeight {
		get {
			return _mapHeight;
		}
	}

	public float OneLatitudeDegreeInUnits = 100;
	public float OneLongitudeDegreeInUnits = 100;

	private static MapMetaInformation m_Instance;
	public static MapMetaInformation Instance { get
		{
			if (m_Instance == null)
			{
				m_Instance = new MapMetaInformation();
			}
			return m_Instance;
		}}
	
	private float _minLat;
    
	public float MinLat {
		get {
			return _minLat;
		}
	}

	private float _minLon;

	public float MinLon {
		get {
			return _minLon;
		}
	}

	private float _maxLat;

	public float MaxLat {
		get {
			return _maxLat;
		}
	}

	private float _maxLon;

	public float MaxLon {
		get {
			return _maxLon;
		}
	}

    public string[] WaterTags = { "drinking_water", "water", "refreshments", "pool", "river", "lake", "pond", "reservoir", "harbour", "drinks" };
    public string[] FoodTags = { "restaurant", "cafe", "groceries", "lunch" };
    public string[] ShelterTags = { "hotel", "motel", "bed", "shelter" };

    void Awake() {
		m_Instance = this;
	}

	public void SetMetaValues(float mlat, float xlat, float mlon, float xlon){

		if (mlat < xlat) {
			_minLat = mlat;
			_maxLat = xlat;
		} else {
			_minLat = xlat;
			_maxLat = mlat;
		}

		if (mlon > xlon) {
			_minLon = mlon;
			_maxLon = xlon;
		} else {
			_minLon = xlon;
			_maxLon = mlon;
		}
	
		Debug.Log ("New meta values: " + _minLat + ", " + _maxLat + ", " + _minLon + ", " + _maxLon);

		_mapWidth = Mathf.Abs(_minLat - _maxLat) * OneLatitudeDegreeInUnits;
		_mapHeight = Mathf.Abs(_minLon - _maxLon) * OneLongitudeDegreeInUnits;

        //CameraController.Instance.SetCameraPosition(new Vector3(MapLatValue(_minLat)+(_mapWidth/2), 0, MapLonValue(_minLon) +(_mapHeight/2)));
	}

	public float MapLonValue(float value){

		return (value - _minLon) * (_mapHeight - 0.0f) / (_maxLon - _minLon) + 0.0f;
//		float R = (Mathf.Abs (_maxLon) - Mathf.Abs(_minLon)) / (MapHeight - 0);
//		return (value - 0) * (R + Mathf.Abs (_minLon));

		//return (Mathf.Abs(value) * (Mathf.Abs(Mathf.Abs (_maxLon) - Mathf.Abs(_minLon)))) * MapHeight;
	}

	public float MapLatValue(float value){
		return (value - _minLat) * (_mapWidth - 0.0f) / (_maxLat - _minLat) + 0.0f;
       
		//return (Mathf.Abs(value) * Mathf.Abs(Mathf.Abs (_maxLat) - Mathf.Abs(_minLat))) * MapWidth;
	}
	
}
