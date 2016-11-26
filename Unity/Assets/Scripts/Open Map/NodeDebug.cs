using UnityEngine;
using System.Collections;

public class NodeDebug : MonoBehaviour{
    
    public MapNode node = null;

  
    void Update()
    {
        if (node != null)
        {
            foreach (MapNode n in node.ConnectedNodes)
            {
                Debug.DrawLine(gameObject.transform.position, n.LocationInUnits, new Color(255, 0, 128), 1f);
            }
        }
    }
    

}
