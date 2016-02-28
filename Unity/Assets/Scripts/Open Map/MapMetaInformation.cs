using UnityEngine;
using System.Collections;

public class MapMetaInformation : MonoBehaviour {

	private float MapWidth;
	private float MapHeight;

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
	
	private float _minLat { get; set; }
	private float _minLon { get; set; }
	private float _maxLat { get; set; }
	private float _maxLon { get; set; }

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

		MapWidth = Mathf.Abs(_minLat - _maxLat) * OneLatitudeDegreeInUnits;
		MapHeight = Mathf.Abs(_minLon - _maxLon) * OneLongitudeDegreeInUnits;

	}

	public float MapLonValue(float value){

		return (value - _minLon) * (MapHeight - 0.0f) / (_maxLon - _minLon) + 0.0f;
//		float R = (Mathf.Abs (_maxLon) - Mathf.Abs(_minLon)) / (MapHeight - 0);
//		return (value - 0) * (R + Mathf.Abs (_minLon));

		//return (Mathf.Abs(value) * (Mathf.Abs(Mathf.Abs (_maxLon) - Mathf.Abs(_minLon)))) * MapHeight;
	}

	public float MapLatValue(float value){
		return (value - _minLat) * (MapWidth - 0.0f) / (_maxLat - _minLat) + 0.0f;
		//return (Mathf.Abs(value) * Mathf.Abs(Mathf.Abs (_maxLat) - Mathf.Abs(_minLat))) * MapWidth;
	}
	
}
