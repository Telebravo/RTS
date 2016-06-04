using UnityEngine;
using System.Collections;

public class cHookliftable : MonoBehaviour
{
    public Vector3 lockPossition;
    public float onGroundY;
    public bool onTruck = false;

    void OnBecameVisible()
    {
        CameraControlls.visibleObjects.Add(this.transform);
    }
    void OnBecameInvisible()
    {
        CameraControlls.visibleObjects.Remove(this.transform);
    }
}
