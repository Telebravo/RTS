using UnityEngine;
using System.Collections;

public class CHookliftable : MonoBehaviour
{
    public Vector3 lockPossition;
    public float onGroundY;
    public bool onTruck = false;

    void OnBecameVisible()
    {
        if (!CameraControlls.visibleObjects.Contains(transform))
        CameraControlls.visibleObjects.Add(this.transform);
    }
    void OnBecameInvisible()
    {
        if (CameraControlls.visibleObjects.Contains(transform))
            CameraControlls.visibleObjects.Remove(this.transform);
    }
}
