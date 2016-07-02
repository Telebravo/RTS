using UnityEngine;
using System.Collections;

public enum AmmunitionSize { NATO556x45, NATO762x51, NATO127x99, Tank120 }
public enum DamageType { Kinetic, Explosive }

public class Ammunition : MonoBehaviour
{
    public string dispayName;
    public AmmunitionSize size;
    public DamageType damageType = DamageType.Kinetic;
    public float damage = 0;
    public float armorPenetration = 1;
    public float explosionRadius = 0;
}
public class NATO556 : Ammunition
{
    public NATO556()
    {
        dispayName = "5.56 NATO";
        size = AmmunitionSize.NATO556x45;
        damage = 55;
    }
}
public class NATO556AP : Ammunition
{
    public NATO556AP()
    {
        dispayName = "5.56 NATO AP";
        size = AmmunitionSize.NATO556x45;
        damage = 55;
        armorPenetration = 3;
        
    }
}
public class NATO762 : Ammunition
{
    public NATO762()
    {
        dispayName = "7.62 NATO";
        size = AmmunitionSize.NATO762x51;
        damage = 76;
        armorPenetration = 2;
    }
}

public class NATO127 : Ammunition
{
    public NATO127()
    {
        dispayName = "12.7 NATO";
        size = AmmunitionSize.NATO127x99;
        damage = 127;
        armorPenetration = 3;
    }
}
public class DM63 : Ammunition
{
    public DM63()
    {
        dispayName = "DM63 APDS";
        size = AmmunitionSize.Tank120;
        damage = 1200;
        armorPenetration = 50;
    }
}
public class TestHE : Ammunition
{
    public TestHE()
    {
        dispayName = "Test HE";
        size = AmmunitionSize.Tank120;
        damageType = DamageType.Explosive;
        damage = 3000;
        explosionRadius = 10;
    }
}