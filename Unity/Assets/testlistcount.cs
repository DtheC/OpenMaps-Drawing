using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class testlistcount : MonoBehaviour {
    public IList<float> ConnectedNodes { get; set; }
    // Use this for initialization
    void Start () {
        ConnectedNodes = new List<float>();
        Debug.Log(ConnectedNodes.Count);
        ConnectedNodes.Add(4f);
        Debug.Log(ConnectedNodes.Count);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
