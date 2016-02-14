using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

public class MapController : MonoBehaviour {
	
	[XmlAttribute("name")]
	public string Name;
	public Transform NodeObject;

	private XmlDocument _mapXML;
	private XmlNodeList _nodes;
	private XmlNodeList _ways;

	private float _minLat = float.MaxValue;
	private float _minLon = float.MaxValue;
	private float _maxLat = float.MaxValue;
	private float _maxLon = float.MaxValue;
	
	private IDictionary<double, IList<double>> _wayDictionary;
	private IDictionary<double, float[]> _nodeDictionary;

	// Use this for initialization
	void Start () {
		_mapXML = new XmlDocument ();
		_mapXML.Load (Name);
		
		_nodes = _mapXML.SelectNodes ("//node");
		_ways = _mapXML.SelectNodes ("//way");

		_wayDictionary = new Dictionary<double, IList<double>> ();
		_nodeDictionary = new Dictionary<double, float[]> ();

		XmlNodeList _bounds = _mapXML.SelectNodes ("//bounds");

//		foreach (XmlNode b in _bounds) {
//			//Debug.Log (b.Attributes.GetNamedItem("minlat").Value);
//			MapMetaInformation.Instance.SetMetaValues(
//				float.Parse(b.Attributes.GetNamedItem("minlat").Value),
//				float.Parse(b.Attributes.GetNamedItem("maxlat").Value),
//				float.Parse(b.Attributes.GetNamedItem("minlon").Value),
//				float.Parse(b.Attributes.GetNamedItem("maxlon").Value));
//		}

		foreach (XmlNode n in _nodes) {
			float x = float.Parse (n.Attributes.GetNamedItem ("lat").Value);
			float y = float.Parse (n.Attributes.GetNamedItem ("lon").Value);

			_nodeDictionary.Add (double.Parse (n.Attributes.GetNamedItem ("id").Value), new float[] {x, y});

			if (_minLat > x || _minLat == float.MaxValue) {
				_minLat = x;
			}
			if (_maxLat < x || _maxLat == float.MaxValue) {
				_maxLat = x;
			}
			if (_minLon > y || _minLon == float.MaxValue) {
				_minLon = y;
			}
			if (_maxLon < y || _maxLon == float.MaxValue) {
				_maxLon = y;
			}
		}

		MapMetaInformation.Instance.SetMetaValues (_minLat, _maxLat, _minLon, _maxLon);

		foreach (XmlNode n in _nodes) {
			float x = float.Parse (n.Attributes.GetNamedItem ("lat").Value);
			float y = float.Parse (n.Attributes.GetNamedItem ("lon").Value);
			x = MapMetaInformation.Instance.MapLatValue (x);
			y = MapMetaInformation.Instance.MapLonValue (y);
			//Debug.Log(x/MapMetaInformation.Instance.MapWidth);
			Instantiate (NodeObject, new Vector3 (x, 0, y), Quaternion.identity);
		}

		foreach (XmlNode w in _ways) {
			bool isHighway = false;
			//First check if this way has a highway tag.
			foreach (XmlNode t in w.ChildNodes) {
				if (t.Name == "tag") {
					if (t.Attributes.GetNamedItem ("k").Value.ToString () == "highway") {
						isHighway = true;
					}
				}
			}
			
			if (isHighway) {
				//If it is a highway then get all the nd references into a list then add the way id and the node refs into the wayDictionary
				double wayId = double.Parse (w.Attributes.GetNamedItem ("id").Value);
				IList<double> wayNodes = new List<double> ();
				//If is highway add it to the way Dictionary.
				XmlNodeList nd = w.SelectNodes ("nd");
				foreach (XmlNode wayNode in nd) {
					wayNodes.Add (double.Parse (wayNode.Attributes.GetNamedItem ("ref").Value));
				}
				_wayDictionary.Add (wayId, wayNodes);
			}
		}
	}

//					XmlNode from = null;
//					XmlNode to = null;
//					foreach (XmlNode wayNode in nd) {
//						if (wayNode.Name == "nd") {
//							to = getNodeByID (double.Parse (wayNode.Attributes.GetNamedItem ("ref").Value), _nodes);
//
//							if (to == null) {
//								continue;
//							}
//							if (from == null) {
//								from = to;
//								continue;
//							}
//
//							float fromLat = MapMetaInformation.Instance.MapLatValue (float.Parse (from.Attributes.GetNamedItem ("lat").Value));
//							float fromLon = MapMetaInformation.Instance.MapLonValue (float.Parse (from.Attributes.GetNamedItem ("lon").Value));
//
//							float toLat = MapMetaInformation.Instance.MapLatValue (float.Parse (to.Attributes.GetNamedItem ("lat").Value));
//							float toLon = MapMetaInformation.Instance.MapLonValue (float.Parse (to.Attributes.GetNamedItem ("lon").Value));
//
//							//Debug.DrawLine (new Vector3 (fromLat, 0, fromLon), new Vector3 (toLat, 0, toLon));



//		while (_mapXML.Read()) {
//			//If name == bounds then use it to update the meta information
//			switch (_mapXML.Name){
//			case ("bounds"):
//				MapMetaInformation.Instance.SetMetaValues(float.Parse(_mapXML.GetAttribute(0)),float.Parse(_mapXML.GetAttribute(1)),float.Parse(_mapXML.GetAttribute(2)),float.Parse(_mapXML.GetAttribute(3)));
//				break;
//			case ("node"):
////				_nodes.Add((XmlNode) _mapXML);
//				break;
//			}
//
//		}


	private XmlNode getNodeByID(double id, XmlNodeList nodes) {
		foreach (XmlNode n in nodes) {
			if (double.Parse (n.Attributes.GetNamedItem("id").Value) == id){
				return n;
			}
		}
		return null;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}