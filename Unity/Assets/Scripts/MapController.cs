using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;

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
	private IDictionary<double, IList<double>> _nodeConnectionDictionary;

	// Use this for initialization
	void Start () {
		_mapXML = new XmlDocument ();
		_mapXML.Load (Name);
		
		_nodes = _mapXML.SelectNodes ("//node");
		_ways = _mapXML.SelectNodes ("//way");

		_wayDictionary = new Dictionary<double, IList<double>> ();
		_nodeDictionary = new Dictionary<double, float[]> ();
		_nodeConnectionDictionary = new Dictionary<double, IList<double>> ();

		InitWorldBounds ();
		InitNodeDictionary ();
		InitWayDictionary ();
		InitNodeConnectionDictionary ();
		//DrawNodes (_nodeDictionary);
		DrawRandomNode ();
		DrawRandomNode ();
		DrawRandomNode ();
		DrawWays (_wayDictionary, _nodeDictionary);

	}

#region Init Functions
	void InitNodeConnectionDictionary(){
		//Get a way node and look at the one next to it. THen find the first double in the connection
		//dictionary and add the next and previous nodes to the list if not already added.
		double fromNode = double.NaN;
		double toNode = double.NaN;

		foreach (IList<double> wayNode in _wayDictionary.Values) {
			for (int i = 0; i < wayNode.Count; i++) {
				toNode = wayNode.IndexOf (i);
				if (toNode == double.NaN) {
					continue;
				}
				//Debug.Log ("To does not equal null!");
				if (fromNode == double.NaN) {
					fromNode = toNode;
					continue;
				}
				//Adding to from -> to
				if (_nodeConnectionDictionary.ContainsKey (fromNode)) {
					if (!_nodeConnectionDictionary [fromNode].Contains (toNode)) {
						_nodeConnectionDictionary [fromNode].Add (toNode);
					}
				} else {
					IList<double> newNodeList = new List<double> ();
					newNodeList.Add (toNode);
					_nodeConnectionDictionary.Add (fromNode, newNodeList);
				}
				//Adding to -> from
				if (_nodeConnectionDictionary.ContainsKey (toNode)) {
					if (!_nodeConnectionDictionary [toNode].Contains (fromNode)) {
						_nodeConnectionDictionary [toNode].Add (fromNode);
					}
				} else {
					IList<double> newNodeList = new List<double> ();
					newNodeList.Add (fromNode);
					_nodeConnectionDictionary.Add (toNode, newNodeList);
				}
			}
		}
	}

	void InitWayDictionary(){
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
		Debug.Log ("Way dictionary initalised with " + _wayDictionary.Count + " items.");
	}

	void InitNodeDictionary(){
		foreach (XmlNode n in _nodes) {
			float x = float.Parse (n.Attributes.GetNamedItem ("lat").Value);
			float y = float.Parse (n.Attributes.GetNamedItem ("lon").Value);
			_nodeDictionary.Add (double.Parse (n.Attributes.GetNamedItem ("id").Value), new float[] {x, y});
		}
		Debug.Log ("Node dictionary initalised with " + _nodeDictionary.Count + " items.");
	}

	void InitWorldBounds(){
		foreach (XmlNode n in _nodes) {
			float x = float.Parse (n.Attributes.GetNamedItem ("lat").Value);
			float y = float.Parse (n.Attributes.GetNamedItem ("lon").Value);

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
	}

#endregion

#region Draw Functions

	void DrawNodes(IDictionary<double, float[]> nodeDict){
		foreach (float[] n in nodeDict.Values) {
			float x = MapMetaInformation.Instance.MapLatValue (n[0]);
			float y = MapMetaInformation.Instance.MapLonValue (n[1]);
			Instantiate (NodeObject, new Vector3 (x, 0, y), Quaternion.identity);
		}
	}

	void DrawWays(IDictionary<double, IList<double>> wayDict, IDictionary<double, float[]> nodeDict){
		foreach (List<double> d in wayDict.Values) {
			//Debug.Log ("Way contains "+d.Count+" nodes");
			float[] to = null;
			float[] from = null;
			for (int i = 0; i < d.Count; i++){
				to = getNodeLatLonByID(d[i], nodeDict);

				if (to == null) {
					continue;
				}
				//Debug.Log ("To does not equal null!");
				if (from == null) {
					from = to;
					continue;
				}
				//Debug.Log ("From does not equal null!");

				float fromLat = MapMetaInformation.Instance.MapLatValue (from[0]);
				float fromLon = MapMetaInformation.Instance.MapLonValue (from[1]);

				float toLat = MapMetaInformation.Instance.MapLatValue (to[0]);
				float toLon = MapMetaInformation.Instance.MapLonValue (to[1]);

				Debug.DrawLine (new Vector3 (fromLat, 0, fromLon), new Vector3 (toLat, 0, toLon),Color.green, 2000, false);
			}
		}
	}

	void DrawRandomNode(){
		int randomNodeIndex = Random.Range (0, _nodeConnectionDictionary.Count);
		Debug.Log (_nodeConnectionDictionary.Count);
		KeyValuePair<double, IList<double>> selectedNode = _nodeConnectionDictionary.ElementAt (randomNodeIndex);
		Debug.Log (selectedNode.Key);

		DrawNode(selectedNode.Key);
		foreach (double nID in selectedNode.Value) {
			Debug.Log(nID);
			DrawNode(nID);
		}
	}

	void DrawNode(double id){
		float[] f = getNodeLatLonByID (id, _nodeDictionary);
		if (f == null) {
			return;
		}
		float Lat = MapMetaInformation.Instance.MapLatValue (f[0]);
		float Lon = MapMetaInformation.Instance.MapLonValue (f[1]);
		Instantiate (NodeObject, new Vector3 (Lat, 0, Lon), Quaternion.identity);
	}
	

#endregion

#region Helper functions
	/// <summary>
	/// Gets the Latitude and Longitude of a specific node by id. This lat and lon are NOT mapped to world.
	/// </summary>
	/// <returns>Lat and Lon as 2 element float array</returns>
	/// <param name="id">Node Id</param>
	/// <param name="nodes">Node Dictionary</param>
	private float[] getNodeLatLonByID(double id, IDictionary<double, float[]> nodes) {
		float[] r;
		try {
			r = nodes[id];	
		} catch (System.Exception ex) {
			return null;
		}
		return r;
	}
#endregion
}