using UnityEngine;
using System.Collections;

public enum AmmunitionSize { NATO556x45, NATO762x51, NATO127x99, Tank120, Shell30x173 }
public enum DamageType { Kinetic, Explosive }

public class Ammunition : MonoBehaviour
{
    new public string name = "New ammo";
    public AmmunitionSize size = AmmunitionSize.NATO556x45;
    public DamageType damageType = DamageType.Kinetic;
    public float damage = 0;
    public float armorPenetration = 1;
    public float explosionRadius = 0;
    public GameObject prefab;
}