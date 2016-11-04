using UnityEngine;
using System.Collections;
using Gamelogic.Extensions;

public class CameraController : Singleton<CameraController> {

    public Transform MainCamera;

    public void SetCameraPosition(Vector3 pos)
    {
        Vector3 p = MainCamera.transform.position;
        p.x = pos.x;
        p.z = pos.z;
        //MainCamera.transform.position = p;
    }

}
