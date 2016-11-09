using UnityEngine;
using System.Collections;

public class WaytracerMovement {
    public WayTracer Parent;

    public WaytracerMovement(WayTracer _p)
    {
        Parent = _p;
    }

    public virtual void MoveTowardNewLocation()
    {
        Debug.Log("No movement function declared");
    }
}

public class WayTracerMovementRigid : WaytracerMovement
{
    public WayTracerMovementRigid(WayTracer _p) : base(_p) { }

    public override void MoveTowardNewLocation()
    {
        float step = Parent.speedOfMovement * Time.deltaTime;

        if (Parent.TravellingToMapNode == null)
        {
            Parent.TravellingToMapNode = Parent.ParentEmitter.GetRandomRoadNode();
        }
        Parent.transform.position = Vector3.MoveTowards(Parent.transform.position, Parent.TravellingToMapNode.LocationInUnits, step);
    }
}

public class WaytracerMovementFree : WaytracerMovement
{
    public WaytracerMovementFree(WayTracer _p) : base(_p){}

    public override void MoveTowardNewLocation()
    {
        Parent.transform.LookAt(Parent.TravellingToMapNode.LocationInUnits);

        Parent.GetComponent<Rigidbody>().AddForce(Parent.transform.forward*0.75f);
        /*
        float step = Parent.speedOfMovement * Time.deltaTime;

        if (Parent.TravellingToMapNode == null)
        {
            Parent.TravellingToMapNode = Parent.ParentEmitter.GetRandomRoadNode();
        }
        Parent.transform.position = Vector3.MoveTowards(Parent.transform.position, Parent.TravellingToMapNode.LocationInUnits, step);
        */
    }
}
