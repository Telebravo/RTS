using UnityEngine;
using System.Collections;

public enum AmmunitionSize { NATO556x45, NATO762x51, NATO127x99, Tank120 }
public class Ammunition : MonoBehaviour
{
    public string dispayName;
    public AmmunitionSize size;
    public DamageType damageType;
}
public class NATO556 : Ammunition
{
    public NATO556()
    {
        dispayName = "5.56 NATO";
        size = AmmunitionSize.NATO556x45;
    }
}
public class NATO762 : Ammunition
{
    public NATO762()
    {
        dispayName = "7.62 NATO";
        size = AmmunitionSize.NATO762x51;
    }
}
public class NATO127 : Ammunition
{
    public NATO127()
    {
        dispayName = "12.7 NATO";
        size = AmmunitionSize.NATO127x99;
    }
}
public class DM63 : Ammunition
{
    public DM63()
    {
        dispayName = "DM63 APDS";
        size = AmmunitionSize.Tank120;
    }
}