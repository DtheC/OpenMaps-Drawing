using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;

public class MapController : MonoBehaviour {
	
	[XmlAttribute("name")]
	public TextAsset Name;

	public bool DrawNodesToScreen = false;
	public Transform NodeObject;

	public NodeDrawer NodeDrawer;
	public MapDrawer MapDrawer;

	public float NeedPropogationRange = 0.1f;

	private XmlDocument _mapXML;
	private XmlNodeList _nodes;
	private XmlNodeList _ways;

	private float _minLat = float.MaxValue;
	private float _minLon = float.MaxValue;
	private float _maxLat = float.MaxValue;
	private float _maxLon = float.MaxValue;

	private IList<MapWay> _wayList;
	public IList<MapWay> WayList {
		get {
			return _wayList;
		}
	}

	private IList<MapNode> _nodeList;

	void Start () {
		_mapXML = new XmlDocument ();
		_mapXML.LoadXml (Name.ToString());

		MapDrawer.MapController = this;
		
		_nodes = _mapXML.SelectNodes ("//node");
		_ways = _mapXML.SelectNodes ("//way");

		_wayList = new List<MapWay> ();
		_nodeList = new List<MapNode> ();

		InitWorldBounds ();
		InitNodeList ();
		InitWayList ();
		InitNodeNeighbourLists ();
		PropogateNeedsToNeighbours ();

		//foreach (MapNode m in _nodeList) {
		//	m.LogNeighbourNodes();
		//}

		if (DrawNodesToScreen) {
		//	MapDrawer.DrawNodes (_nodeDictionary);
		}

			MapDrawer.DrawWays (_wayList);

		//NodeDrawer.DrawNodes (_nodeList);
	}

#region Init Functions

	/// <summary>
	/// Propogates the needs to neighbour nodes.
	/// </summary>
	void PropogateNeedsToNeighbours(){
		foreach (MapNode n in _nodeList) {
			IDictionary<MapNode, float> nodes = GetNodesInRange (n, NeedPropogationRange);
			foreach (KeyValuePair<MapNode, float> neighbour in nodes) {
				//Construct a Needs dictionary to send
				Dictionary<Needs, float> neighbourValues = new Dictionary<Needs, float>();
				//Set the values in the dictionary to neightbour distance / needproprange * n.NeedAmounts
				foreach (Needs need in System.Enum.GetValues(typeof(Needs))) 
				{
					float needValue = (neighbour.Value / NeedPropogationRange) * n.NeedAmounts [need];
					neighbourValues.Add (need, needValue);
				}
				neighbour.Key.PropogateNeedsToNeighbours (neighbourValues);
			}
		}
	}

	void InitNodeNeighbourLists(){
		//Get a way node and look at the one next to it. THen find the first double in the connection
		//dictionary and add the next and previous nodes to the list if not already added.

		foreach (MapWay _mapway in _wayList) {
			MapNode fromNode = null;
			MapNode toNode = null;
			for (int i = 0; i < _mapway._nodesInWay.Count; i++) {
				toNode = _mapway._nodesInWay [i];
				if (toNode == null) {
					continue;
				}
				if (fromNode == null) {
					fromNode = toNode;
					continue;
				}

				//MapNode fromMapNode = GetMapNodeById (fromNode);
				//MapNode toMapNode = GetMapNodeById (toNode);
				if (fromNode != null && toNode != null) {
					fromNode.AddNeighbouringNode (toNode);
					toNode.AddNeighbouringNode (fromNode);
					fromNode = toNode;
				}
			}
			//_mapway.LogWayNodes();
		}
	}

	void InitWayList(){
		IList<MapWay> _tempWayList = new List<MapWay>();
		foreach (XmlNode w in _ways) {
			double wayId = double.Parse (w.Attributes.GetNamedItem ("id").Value);
			IList<MapNode> wayNodes = new List<MapNode> ();
			XmlNodeList nd = w.SelectNodes ("nd");
			foreach (XmlNode wayNode in nd) {
				MapNode nextNode = GetMapNodeById(double.Parse (wayNode.Attributes.GetNamedItem ("ref").Value));
				if (nextNode != null){
					wayNodes.Add (nextNode);
				}
			}
			nd = w.SelectNodes ("tag");
			IDictionary<string, IList<string>> wayTags = new Dictionary<string, IList<string>>();
			foreach (XmlNode aTag in nd) {
				string kValue = aTag.Attributes.GetNamedItem("k").Value;
				string vValue = aTag.Attributes.GetNamedItem("v").Value;
				if (!wayTags.ContainsKey(kValue)){
					wayTags[kValue] = new List<string>();
				}
				wayTags[kValue].Add(vValue);
			}

			MapWay newWay = new MapWay(wayId);
			newWay._nodesInWay = wayNodes;
			newWay._tags = wayTags;
			_tempWayList.Add(newWay);
		}
		_wayList = _tempWayList;
	}

	void InitNodeList(){
		foreach (XmlNode n in _nodes) {
			float x = float.Parse (n.Attributes.GetNamedItem ("lat").Value);
			float y = float.Parse (n.Attributes.GetNamedItem ("lon").Value);
			MapNode _newNode = new MapNode(double.Parse (n.Attributes.GetNamedItem ("id").Value), x, y);
			_newNode.updateUnitLocationVectors();

			//Get the tag list of the node
			XmlNodeList nd = n.SelectNodes ("tag");
			IDictionary<string, IList<string>> nodeTags = new Dictionary<string, IList<string>>();
			foreach (XmlNode aTag in nd) {
				string kValue = aTag.Attributes.GetNamedItem("k").Value;
				string vValue = aTag.Attributes.GetNamedItem("v").Value;
				if (!nodeTags.ContainsKey(kValue)){
					nodeTags[kValue] = new List<string>();
				}
				nodeTags[kValue].Add(vValue);
			}
			_newNode._tags = nodeTags;
			_newNode.SetWaterBasedOnTags ();
			_newNode.SetFoodBasedOnTags ();
			_nodeList.Add(_newNode);
			//_newNode.LogTags ("ater");
		}
		Debug.Log ("Node dictionary initalised with " + _nodeList.Count + " items.");
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

	public MapNode GetRandomNode(){
		return _nodeList [Random.Range (0, _nodeList.Count)];
	}

	public double GetRandomNodeId(){
		int randomNodeIndex = Random.Range (0, _nodeList.Count);
		double selectedNode = _nodeList [randomNodeIndex]._id;
		return selectedNode;
	}
		
	/// <summary>
	/// Gets the map node by identifier.
	/// </summary>
	/// <returns>The map node by identifier.</returns>
	/// <param name="nodeId">Node identifier.</param>
	public MapNode GetMapNodeById(double nodeId){
		foreach (MapNode n in _nodeList) {
			if (n._id == nodeId){
				return n;
			}
		}
		return null;
	}

	public IDictionary<MapNode, float> GetNodesInRange(MapNode node, float range){
		IDictionary<MapNode, float> nodes = new Dictionary<MapNode, float> ();
		foreach (MapNode otherNode in _nodeList){
			//Check this isn't the current node to avoid duplication
			if (node._id != otherNode._id) {
				float dist = Vector3.Distance (node.LocationInUnits, otherNode.LocationInUnits);
				if (dist < range) {
					nodes.Add (otherNode, dist);
				}
			}
		}
		return nodes;
	}

#endregion
}