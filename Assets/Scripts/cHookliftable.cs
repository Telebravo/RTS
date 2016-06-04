using UnityEngine;
using System.Collections;

public class CHookliftable : MonoBehaviour
{
    public Vector3 lockPossition;
    public float onGroundY;
    public bool onTruck = false;

    void OnBecameVisible()
    {
        GameManager.controlls.SetVisible(this.transform, true);
    }
    void OnBecameInvisible()
    {
        GameManager.controlls.SetVisible(this.transform, false);
    }
}
