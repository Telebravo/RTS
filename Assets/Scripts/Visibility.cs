using UnityEngine;
using System.Collections;

[AddComponentMenu("Unit/Visibility")]
[RequireComponent(typeof(Unit))]
public class Visibility : MonoBehaviour
{
    public bool iStilling;
    public bool iBevegelse;

    private Unit unit;

    private SphereCollider maxLos;
    private float visibility;

    void Awake()
    {
        unit = GetComponent<Unit>();
    }

    public float GetVisibility()
    {
        visibility = 100 - unit.camo;
        visibility = visibility / 100;

        if (iStilling)
            visibility /= 2;
        if (iBevegelse)
            visibility *= 2; 

        return visibility;
    }
}