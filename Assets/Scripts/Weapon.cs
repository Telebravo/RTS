using UnityEngine;
using System.Collections;

public enum Weapons { Unarmed, HK416, Minimi, M2Browning, RheinmetallL44, ArtCannon }

public class Weapon
{
    public string displayName;
    public int firerate;
    public int range;
    public float accuracy = 100;
    public int burstLength = 0;
    public Ammunition ammo;
    public AmmunitionSize ammoSize;

    public static Weapon Get(Weapons weapon)
    {
        switch (weapon)
        {
            case Weapons.Unarmed:
                return new Unarmed();
            case Weapons.HK416:
                return new HK416N();
            case Weapons.Minimi:
                return new Minimi();
            case Weapons.M2Browning:
                return new M2Browning();
            case Weapons.RheinmetallL44:
                return new RheinmetallL44();
            case Weapons.ArtCannon:
                return new ArtCannon();
        }
        return null;
    }
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
public class Unarmed : Weapon
{
    public Unarmed()
    {
        displayName = "Unarmed";
        firerate = 1;
        range = 1;
    }
}

//////////////////// HÅNDVÅPEN ////////////////////

public class HK416N : Weapon
{
    public HK416N()
    {
        displayName = "HK416N";
        firerate = 30;
        range = 400;
        accuracy = 100;
        ammoSize = AmmunitionSize.NATO556x45;
        ammo = new NATO556();
    }
}
public class Minimi : Weapon
{
    public Minimi()
    {
        displayName = "FN Minimi";
        firerate = 300;
        range = 800;
        accuracy = 80;
        burstLength = 5;
        ammoSize = AmmunitionSize.NATO556x45;
        ammo = new NATO556();
    }
}
public class M2Browning : Weapon
{
    public M2Browning()
    {
        displayName = "M2 Browning";
        firerate = 300;
        range = 1200;
        accuracy = 80f;
        burstLength = 5;
        ammoSize = AmmunitionSize.NATO127x99;
        ammo = new NATO127();
    }
}

//////////////////// KANONER ////////////////////

public class RheinmetallL44 : Weapon
{
    public RheinmetallL44()
    {
        displayName = "Rheinmetall L/44 120mm";
        firerate = 6;
        range = 2000;
        ammoSize = AmmunitionSize.Tank120;
        ammo = new DM63();
    }
}
public class ArtCannon : Weapon
{
    public ArtCannon()
    {
        displayName = "Artilery Cannon";
        firerate = 5;
        range = 500;
        ammoSize = AmmunitionSize.Tank120;
        ammo = new TestHE();
    }
}