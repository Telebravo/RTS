using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    new public string name = "New weapon";
    public int firerate = 0;
    public int range = 0;
    public float accuracy = 100;
    public int burstLength = 0;
    public Ammunition ammo;
    public AmmunitionSize ammoSize = AmmunitionSize.NATO556x45;
    public Transform barrelEnd;

    public bool SetAmmo(Ammunition ammo)
    {
        if (ammo.size == ammoSize)
        {
            this.ammo = ammo;
            return true;
        }
        else
        {
            return false;
        }    
    }
}