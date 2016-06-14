using UnityEngine;
using System.Collections;

public class CHookliftable : MonoBehaviour
{
    public Vector3 lockPossition;
    public Transform hookPoint;
    public float onGroundY;
    public bool onTruck = false;
    public bool liftable = true;
    public NavMeshObstacle navMeshObstacle;

    void Awake()
    {
        navMeshObstacle = GetComponent<NavMeshObstacle>();
    }
    void OnBecameVisible()
    {
        GameManager.controlls.SetVisible(this.transform, true);
    }
    void OnBecameInvisible()
    {
        GameManager.controlls.SetVisible(this.transform, false);
    }
}
