using UnityEngine;
using System.Collections;

using System.Xml;
using System.Xml.Serialization;

public class XMLReadTest : MonoBehaviour {

    [XmlAttribute("name")]
    public TextAsset Name;

    private XmlNodeList _nodes;
    private float _minLat;
    private float _maxLat;
    private float _minLon;
    private float _maxLon;

    // Use this for initialization
    void Start () {

        var _mapXML = new XmlDocument();
        _mapXML.LoadXml(Name.ToString());

        //MapDrawer.MapController = this;

        _nodes = _mapXML.SelectNodes("//node");
        Debug.Log("Nodes loaded: " + _nodes.Count);
        InitWorldBounds();

    }

    void InitWorldBounds()
    {
        foreach (XmlNode n in _nodes)
        {
            float x = float.Parse(n.Attributes.GetNamedItem("lat").Value);
            float y = float.Parse(n.Attributes.GetNamedItem("lon").Value);

            if (_minLat > x || _minLat == float.MaxValue)
            {
                _minLat = x;
            }
            if (_maxLat < x || _maxLat == float.MaxValue)
            {
                _maxLat = x;
            }
            if (_minLon > y || _minLon == float.MaxValue)
            {
                _minLon = y;
            }
            if (_maxLon < y || _maxLon == float.MaxValue)
            {
                _maxLon = y;
            }
        }
        Debug.Log(_minLat +","+ _maxLat + "," + _minLon + "," + _maxLon);
    }


}
