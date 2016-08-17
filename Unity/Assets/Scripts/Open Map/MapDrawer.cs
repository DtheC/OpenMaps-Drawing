using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapDrawer : MonoBehaviour {

	public Transform NodeObject;
	public bool DrawWaysToScreen = true;
	public bool OnlyDrawHighways = true;

	public Mesh WayMesh;

	public Material MapMaterial;

	public float RoadWidth = 0.5f;

	private IList<GameObject> _wayMeshes;
	public IList<GameObject> WayMeshes {
		get {
			return _wayMeshes;
		}
	}

	private MapController _mapController;
	public MapController MapController {
		get {
			return _mapController;
		}
		set {
			_mapController = value;
		}
	}

	void Start(){
		_wayMeshes = new List<GameObject> ();
	}

	public void DrawNodes(IDictionary<double, float[]> nodeDict){
		foreach (float[] n in nodeDict.Values) {
			float x = MapMetaInformation.Instance.MapLatValue (n[0]);
			float y = MapMetaInformation.Instance.MapLonValue (n[1]);
			Instantiate (NodeObject, new Vector3 (x, 0, y), Quaternion.identity);
		}
	}

	public void DrawWays(IList<MapWay> wayList){
		if (!DrawWaysToScreen) {
			return;
		}
		List<Color32> colours = new List<Color32>();

		//Create MeshBuilder 
		MeshBuilder meshBuilder = new MeshBuilder();
		int currentTriangleCount = 0;

		foreach (MapWay mapway in wayList) {
			if (OnlyDrawHighways){
				if (!mapway._tags.ContainsKey("highway")){
					continue;
				}
			}

			MapNode to = null;
			MapNode from = null;
			//Color randomCol = new Color (Random.Range (0.00f, 1.00f),Random.Range (0.00f, 1.00f),Random.Range (0.00f, 1.00f));
			for (int i=0; i < mapway._nodesInWay.Count; i++){
				//Get nodes
				to = _mapController.GetMapNodeById(mapway._nodesInWay[i]._id);
				if (to == null){
					continue;
				}
				if (from == null){
					from = to;
					continue;
				}
				//Debug.DrawLine(from.LocationInUnits, to.LocationInUnits, randomCol, 2000, false);

				//Draw Mesh
				Vector3 newVec = to.LocationInUnits - from.LocationInUnits;
				Vector3 newVector = Vector3.Cross (newVec, Vector3.down);
				newVector.Normalize ();

				Vector3 c = RoadWidth * newVector + to.LocationInUnits;
				Vector3 d = -RoadWidth * newVector + to.LocationInUnits;

				Vector3 e = RoadWidth * newVector + from.LocationInUnits;
				Vector3 f = -RoadWidth * newVector + from.LocationInUnits;

				//MeshBuilder from http://jayelinda.com/
				meshBuilder.Vertices.Add(e);
				meshBuilder.UVs.Add(new Vector2(0.0f, 0.0f));
				meshBuilder.Normals.Add(Vector3.up);

				meshBuilder.Vertices.Add(f);
				meshBuilder.UVs.Add(new Vector2(1.0f, 0.0f));
				meshBuilder.Normals.Add(Vector3.up);

				meshBuilder.Vertices.Add(d);
				meshBuilder.UVs.Add(new Vector2(1.0f, 1.0f));
				meshBuilder.Normals.Add(Vector3.up);

				meshBuilder.Vertices.Add(c);
				meshBuilder.UVs.Add(new Vector2(0.0f, 1.0f));
				meshBuilder.Normals.Add(Vector3.up);

				byte red = (byte) ((from.LocationInUnits.z / MapMetaInformation.Instance.MapWidth)*255);
				byte green = (byte) ((from.LocationInUnits.x / MapMetaInformation.Instance.MapHeight)*255);
				byte blue = (byte) ((from.NeedAmounts[Needs.Food] + from.NearbyNeedAmounts[Needs.Food]) * 255);

				Color32 ddd = new Color32 (0, 20, blue, 255);
				colours.Add (ddd);
				colours.Add (ddd);

				red = (byte) ((to.LocationInUnits.z / MapMetaInformation.Instance.MapWidth)*255);
				green = (byte) ((to.LocationInUnits.x / MapMetaInformation.Instance.MapHeight)*255);
				blue = (byte) ((from.NeedAmounts[Needs.Food] + from.NearbyNeedAmounts[Needs.Food]) * 255);

				ddd = new Color32 (0, 20, blue, 255); 
				colours.Add (ddd);
				colours.Add (ddd);

				//Add the triangles:
				meshBuilder.AddTriangle(currentTriangleCount, currentTriangleCount+1, currentTriangleCount+2);
				meshBuilder.AddTriangle(currentTriangleCount, currentTriangleCount+2, currentTriangleCount+3);
				currentTriangleCount+=4;

				from = to;
			}
		}
		//Create the mesh:
		Mesh mesh = meshBuilder.CreateMesh();

		//Look for a MeshFilter component attached to this GameObject:
		MeshFilter filter = GetComponent<MeshFilter>();

		//If the MeshFilter exists, attach the new mesh to it.
		//Assuming the GameObject also has a renderer attached, our new mesh will now be visible in the scene.
		if (filter != null)
		{
			filter.sharedMesh = mesh;
			GetComponent<MeshRenderer> ().material = MapMaterial;
		}

		ColourMesh (colours);
	}

	void ColourMesh(List<Color32> cols){
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;
		Color[] colors = new Color[vertices.Length];
		Debug.Log (vertices.Length);
		int i = 0;
		while (i < vertices.Length) {
			colors [i] = cols [i];
			i++;
		}
		mesh.colors = colors;
	}

//	void DrawRandomNode(){
//		int randomNodeIndex = Random.Range (0, _nodeConnectionDictionary.Count);
//		//Debug.Log (_nodeConnectionDictionary.Count);
//		KeyValuePair<double, IList<double>> selectedNode = _nodeConnectionDictionary.ElementAt (randomNodeIndex);
//		//		Debug.Log ("Drawing node "+selectedNode.Key+" and connections.");
//		DrawNode(selectedNode.Key);
//		foreach (double nID in selectedNode.Value.AsEnumerable()) {
//			//Debug.Log(nID);
//			DrawNode(nID);
//		}
//	}
//
//	void DrawNode(double id){
//		float[] f = getNodeLatLonByID (id, _nodeDictionary);
//		if (f == null) {
//			return;
//		}
//		float Lat = MapMetaInformation.Instance.MapLatValue (f[0]);
//		float Lon = MapMetaInformation.Instance.MapLonValue (f[1]);
//		Instantiate (NodeObject, new Vector3 (Lat, 0, Lon), Quaternion.identity);
//	}
}
