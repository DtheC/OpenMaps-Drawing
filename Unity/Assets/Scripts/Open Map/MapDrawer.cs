using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapDrawer : MonoBehaviour {

	public GameObject NodeObject;
	public bool DrawWaysToScreen = true;
	public bool OnlyDrawHighways = true;

	public Needs NeedToColour = Needs.Food;
    public bool DrawAll = true;

	private Mesh _wayMesh;
	private IList<Color32> _wayColours;

	public Material MapMaterial;

	public float RoadWidth = 0.5f;

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
		List<Color32> _wayColours = new List<Color32>();
	}

	public void DrawNodes(){

        foreach (MapNode n in _mapController.NodeList)
        {
            var newn = Instantiate(NodeObject, n.LocationInUnits, Quaternion.identity) as Transform;
            
            var gg = newn.gameObject.AddComponent<NodeDebug>();
            gg.node = n;
        }
        
	}

	private void GenerateWayMesh(IList<MapWay> wayList){
		IList<Color32> colours = new List<Color32> ();

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

                //Don't edit colours here- do it in the GenerateMeshColours function below!
                byte needColor;
                Color32 vertexColor; 

                if (NeedToColour == Needs.Food)
                {
                    needColor = (byte)((from.NeedAmounts[Needs.Food] + from.NearbyNeedAmounts[Needs.Food]) * 255);
                    vertexColor = new Color32(needColor, 0, 0, 255);
                } else
                {
                    needColor = (byte)((from.NeedAmounts[Needs.Water] + from.NearbyNeedAmounts[Needs.Water]) * 255);
                    vertexColor = new Color32(0, 0, needColor, 255);
                }
                
                colours.Add (vertexColor);
				colours.Add (vertexColor);

                //Other side
                if (NeedToColour == Needs.Food)
                {
                    needColor = (byte)((to.NeedAmounts[Needs.Food] + to.NearbyNeedAmounts[Needs.Food]) * 255);
                    vertexColor = new Color32(needColor, 0, 0, 255);
                }
                else
                {
                    needColor = (byte)((to.NeedAmounts[Needs.Water] + to.NearbyNeedAmounts[Needs.Water]) * 255);
                    vertexColor = new Color32(0, 0, needColor, 255);
                }
                
                colours.Add (vertexColor);
				colours.Add (vertexColor);

				//Add the triangles:
				meshBuilder.AddTriangle(currentTriangleCount, currentTriangleCount+1, currentTriangleCount+2);
				meshBuilder.AddTriangle(currentTriangleCount, currentTriangleCount+2, currentTriangleCount+3);
				currentTriangleCount+=4;

				from = to;
			}
		}
		//Create the mesh:
		Mesh mesh = meshBuilder.CreateMesh();
		_wayMesh = mesh;
		_wayColours = colours;
		GenerateNeedMeshColours (wayList);
	}

	private void GenerateNeedMeshColours(IList<MapWay> wayList){
		IList<Color32> colours = new List<Color32> ();

		//Create MeshBuilder 
		MeshBuilder meshBuilder = new MeshBuilder();

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
                
                Color32 vertexColor;

                vertexColor = GetColourOfVertexBasedOnNeed(from);
                
                colours.Add (vertexColor);
				colours.Add (vertexColor);

                vertexColor = GetColourOfVertexBasedOnNeed(to);

                colours.Add(vertexColor);
                colours.Add(vertexColor);

                from = to;
			}
		}

		_wayColours = colours;
	}

    private Color32 GetColourOfVertexBasedOnNeed(MapNode node)
    {
        Color32 vertexColor = new Color32(255, 255, 255, 255);
        byte needColor;

        if (DrawAll)
        {
            byte red = (byte)((node.NeedAmounts[Needs.Food] + node.NearbyNeedAmounts[Needs.Food]) * 255);
            byte green = (byte)((node.NeedAmounts[Needs.Shelter] + node.NearbyNeedAmounts[Needs.Shelter]) * 255);
            byte blue = needColor = (byte)((node.NeedAmounts[Needs.Water] + node.NearbyNeedAmounts[Needs.Water]) * 255);

            vertexColor = new Color32(red, green, blue, 255);
        }
        else
        {
            //TODO Rewrite this as a switch statement
            if (NeedToColour == Needs.Food)
            {
                needColor = (byte)((node.NeedAmounts[Needs.Food] + node.NearbyNeedAmounts[Needs.Food]) * 255);
                vertexColor = new Color32(needColor, 0, 0, 255);
            }
            if (NeedToColour == Needs.Water)
            {
                needColor = (byte)((node.NeedAmounts[Needs.Water] + node.NearbyNeedAmounts[Needs.Water]) * 255);
                vertexColor = new Color32(0, 0, needColor, 255);
            } else
            {
                needColor = (byte)((node.NeedAmounts[Needs.Shelter] + node.NearbyNeedAmounts[Needs.Shelter]) * 255);
                vertexColor = new Color32(0, needColor, 0, 255);
            }
        }
        return vertexColor;
    }

	public void DrawWays(IList<MapWay> wayList){
		if (!DrawWaysToScreen) {
			return;
		}

		if (_wayMesh == null) {
			//Generate Mesh
			GenerateWayMesh (wayList);
		}

		//Look for a MeshFilter component attached to this GameObject:
		MeshFilter filter = GetComponent<MeshFilter>();

		//If the MeshFilter exists, attach the new mesh to it.
		//Assuming the GameObject also has a renderer attached, our new mesh will now be visible in the scene.
		if (filter != null)
		{
			filter.sharedMesh = _wayMesh;
			GetComponent<MeshRenderer> ().material = MapMaterial;
		}

		ColourMesh (_wayColours);
	}

	void ColourMesh(IList<Color32> cols){
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;
		Color[] colors = new Color[vertices.Length];

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
