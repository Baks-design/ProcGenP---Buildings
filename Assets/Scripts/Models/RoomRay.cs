using UnityEngine;

public class RoomRay 
{
    public float RayDistance { get; private set; }
    public Vector3 RayPosition { get; private set; }

    public RoomRay(Vector3 rayPostion, float distance)
    {
        this.RayPosition = rayPostion;
        this.RayDistance = distance;
    }
}